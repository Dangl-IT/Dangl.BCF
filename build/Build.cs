using System.Linq;
using Nuke.CoberturaConverter;
using Nuke.Common;
using Nuke.Common.Git;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.GitVersion;
using Nuke.Common.Utilities.Collections;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;
using static Nuke.Common.Tools.DotNet.DotNetTasks;
using static Nuke.CoberturaConverter.CoberturaConverterTasks;
using static Nuke.Common.Tools.Git.GitTasks;

class Build : NukeBuild
{
    public static int Main() => Execute<Build>(x => x.Compile);

    [GitVersion] readonly GitVersion GitVersion;
    [GitRepository] readonly GitRepository GitRepository;

    [Parameter] readonly string ProGetSource;
    [Parameter] readonly string ProGetApiKey;

    Target Clean => _ => _
        .Executes(() =>
        {
            DeleteDirectories(GlobDirectories(SolutionDirectory, "**/bin", "**/obj"));
            EnsureCleanDirectory(OutputDirectory);
        });

    Target Restore => _ => _
        .DependsOn(Clean)
        .Executes(() =>
        {
            DotNetRestore(s => DefaultDotNetRestore);
        });

    Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
            DotNetBuild(s => DefaultDotNetBuild
                .SetFileVersion(GitVersion.GetNormalizedFileVersion())
                .SetAssemblyVersion($"{GitVersion.Major}.{GitVersion.Minor}.{GitVersion.Patch}.0"));
        });

    Target Pack => _ => _
        .DependsOn(Compile)
        .Executes(() =>
        {
            DotNetPack(s => DefaultDotNetPack);
        });

    Target Coverage => _ => _
        .DependsOn(Compile)
        .Executes(async () =>
        {
            var openCoverSettings = new ToolSettings()
                .SetToolPath(ToolPathResolver.GetPackageExecutable("OpenCover", @"tools\OpenCover.Console.exe"))
                .SetArgumentConfigurator(args => args
                    .Add("-register:user")
                    .Add("-target:dotnet.exe")
                    .Add("-targetargs:{value}", $"xunit -nobuild -xml \"{OutputDirectory / "testresult.xml"}\"")
                    .Add("-targetdir:{value}", SolutionDirectory / "test" / "iabi.BCF.Tests")
                    .Add("-returntargetcode")
                    .Add("-output:{value}", OutputDirectory / "OpenCover.coverageresults")
                    .Add("-mergeoutput")
                    .Add("-oldStyle")
                    .Add("-excludebyattribute:System.CodeDom.Compiler.GeneratedCodeAttribute")
                    .Add("\"-filter:+[iabi.BCF]* -[*.Tests]* -[*.Tests.*]*\""));

            var coverageProcess = ProcessTasks.StartProcess(openCoverSettings);
            coverageProcess?.WaitForExit();

            await OpenCoverToCobertura(x => x
                .SetInputFile(OutputDirectory / "OpenCover.coverageresults")
                .SetOutputFile(OutputDirectory / "Cobertura.coverageresults"));
        });

    Target Push => _ => _
        .DependsOn(Pack)
        .Requires(() => ProGetSource)
        .Requires(() => ProGetApiKey)
        .Executes(() =>
        {
            GlobFiles(OutputDirectory, "*.nupkg").NotEmpty()
                .Where(x => !x.EndsWith("symbols.nupkg"))
                .ForEach(x =>
                {
                    DotNetNuGetPush(s => s
                        .SetTargetPath(x)
                        .SetSource(ProGetSource)
                        .SetApiKey(ProGetApiKey));

                    if (GitVersion.BranchName.Equals("master") || GitVersion.BranchName.Equals("origin/master"))
                    {
                        Git($"tag {GitVersion.NuGetVersion}");
                        Git("push --tags");
                    }
                });
        });
}
