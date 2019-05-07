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
using Nuke.Common.ProjectModel;

class Build : NukeBuild
{
    public static int Main() => Execute<Build>(x => x.Compile);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [GitVersion] readonly GitVersion GitVersion;
    [GitRepository] readonly GitRepository GitRepository;

    [Solution] readonly Solution Solution;
    AbsolutePath OutputDirectory => RootDirectory / "output";

    [Parameter] readonly string ProGetSource;
    [Parameter] readonly string ProGetApiKey;

    [PackageExecutable("OpenCover", "tools/OpenCover.Console.exe")] Tool OpenConver;

    Target Clean => _ => _
        .Executes(() =>
        {
            GlobDirectories(RootDirectory / "src", "**/bin", "**/obj").ForEach(DeleteDirectory);
            GlobDirectories(RootDirectory / "test", "**/bin", "**/obj").ForEach(DeleteDirectory);
            EnsureCleanDirectory(OutputDirectory);
        });

    Target Restore => _ => _
        .DependsOn(Clean)
        .Executes(() =>
        {
            DotNetRestore(s => s
                .SetProjectFile(Solution));
        });

    Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
            DotNetBuild(s => s
               .SetProjectFile(Solution)
               .SetConfiguration(Configuration)
               .SetAssemblyVersion(GitVersion.GetNormalizedAssemblyVersion())
               .SetFileVersion(GitVersion.GetNormalizedFileVersion())
               .SetInformationalVersion(GitVersion.InformationalVersion)
               .EnableNoRestore());
        });

    Target Pack => _ => _
        .DependsOn(Compile)
        .Executes(() =>
        {
            DotNetPack(s => s.SetOutputDirectory(OutputDirectory));
        });

    Target Coverage => _ => _
        .DependsOn(Compile)
        .Executes(async () =>
        {
            var dotNetPath = ToolPathResolver.GetPathExecutable("dotnet.exe");
            OpenConver($"-register:user " +
                $"-target:\"{dotNetPath}\" " +
                $"-targetargs:\"{$"test --no-build --test-adapter-path:. --logger:xunit;LogFilePath=\"{OutputDirectory / "testresult.xml"}\""}\" " +
                $"-targetdir:\"{RootDirectory / "test" / "iabi.BCF.Tests"}\" " +
                $"-returntargetcode " +
                $"-output:{OutputDirectory / "OpenCover.coverageresults"} " +
                $"-mergeoutput " +
                $"-oldStyle " +
                $"-excludebyattribute:System.CodeDom.Compiler.GeneratedCodeAttribute " +
                $"\"-filter:+[iabi.BCF]* -[*.Tests]* -[*.Tests.*]*\"");

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
