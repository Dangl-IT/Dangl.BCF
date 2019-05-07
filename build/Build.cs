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
using System.IO;

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

    [PackageExecutable("JetBrains.dotCover.CommandLineTools", "tools/dotCover.exe")] Tool DotCover;
    [PackageExecutable("ReportGenerator", "tools/ReportGenerator.exe")] Tool ReportGenerator;

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
            DotNetPack(s => s
                .SetVersion(GitVersion.NuGetVersion)
                .SetOutputDirectory(OutputDirectory)
                .EnableNoBuild());
        });

    Target Coverage => _ => _
        .DependsOn(Compile)
        .Executes(async () =>
        {
            var testProjects = new[]
            {
                RootDirectory / "test" / "iabi.BCF.Tests"
            };
            var dotnetPath = ToolPathResolver.GetPathExecutable("dotnet");

            for (var i = 0; i < testProjects.Length; i++)
            {
                var testProject = testProjects[i];

                /* DotCover */
                var projectName = Path.GetFileName(testProject);
                var snapshotIndex = i;

                DotCover($"cover /TargetExecutable=\"{dotnetPath}\" /TargetWorkingDir=\"{testProject}\" " +
                    $"/TargetArguments=\"test --no-build --test-adapter-path:. \\\"--logger:xunit;LogFilePath={OutputDirectory / projectName}_testresults.xml\\\"\" " +
                    "/Filters=\"+:iabi.BCF*;-:*Tests*\" " +
                    "/AttributeFilters=\"System.CodeDom.Compiler.GeneratedCodeAttribute\" " +
                    $"/Output=\"{OutputDirectory / $"coverage{snapshotIndex:00}.snapshot"}\"");
            }

            var snapshots = testProjects.Select((t, i) => OutputDirectory / $"coverage{i:00}.snapshot")
                .Select(p => p.ToString())
                .Aggregate((c, n) => c + ";" + n);

            DotCover($"merge /Source=\"{snapshots}\" /Output=\"{OutputDirectory / "coverage.snapshot"}\"");

            DotCover($"report /Source=\"{OutputDirectory / "coverage.snapshot"}\" /Output=\"{OutputDirectory / "coverage.xml"}\" /ReportType=\"DetailedXML\"");

            ReportGenerator($"-reports:\"{OutputDirectory / "coverage.xml"}\" -targetdir:\"{OutputDirectory / "CoverageReport"}\"");

            // This is the report in Cobertura format that integrates so nice in Jenkins
            // dashboard and allows to extract more metrics and set build health based
            // on coverage readings
            await DotCoverToCobertura(s => s
                    .SetInputFile(OutputDirectory / "coverage.xml")
                    .SetOutputFile(OutputDirectory / "cobertura_coverage.xml"))
                .ConfigureAwait(false);
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
