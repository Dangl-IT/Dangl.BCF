﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Nuke.Common;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.GitVersion;
using Nuke.Common.Utilities.Collections;
using Nuke.GitHub;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;
using static Nuke.Common.Tools.DotNet.DotNetTasks;
using static Nuke.GitHub.ChangeLogExtensions;
using static Nuke.GitHub.GitHubTasks;
using static Nuke.Common.ChangeLog.ChangelogTasks;
using static Nuke.Common.IO.TextTasks;
using System.IO;
using Nuke.CoberturaConverter;
using static Nuke.CoberturaConverter.CoberturaConverterTasks;

class Build : NukeBuild
{
    public static int Main() => Execute<Build>(x => x.Compile);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [GitVersion(Framework = "netcoreapp3.1")] readonly GitVersion GitVersion;
    [GitRepository] readonly GitRepository GitRepository;

    [Solution] readonly Solution Solution;
    AbsolutePath OutputDirectory => RootDirectory / "output";
    AbsolutePath ChangeLogFile => RootDirectory / "CHANGELOG.md";

    [Parameter] readonly string IabiGitHubPackageSource = "https://nuget.pkg.github.com/iabiev/index.json";
    [Parameter] readonly string IabiGitHubPackageApiKey;
    [Parameter] readonly string GitHubAuthenticationToken;
    [Parameter] readonly string NuGetApiKey;

    [PackageExecutable("JetBrains.dotCover.CommandLineTools", "tools/dotCover.exe")] Tool DotCover;

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
            WriteFileVersionProvider();

            DotNetBuild(s => s
               .SetProjectFile(Solution)
               .SetConfiguration(Configuration)
               .SetAssemblyVersion(GitVersion.AssemblySemVer)
               .SetFileVersion(GitVersion.AssemblySemVer)
               .SetInformationalVersion(GitVersion.InformationalVersion)
               .EnableNoRestore());
        });

    void WriteFileVersionProvider()
    {
        var fileVersionPath = RootDirectory / "src" / "iabi.BCF" / "FileVersionProvider.cs";
        var date = System.DateTime.UtcNow;
        var dateCode = $"new DateTime({date.Year}, {date.Month}, {date.Day}, {date.Hour}, {date.Minute}, {date.Second}, DateTimeKind.Utc)";
        var fileVersionCode = $@"using System;
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
namespace iabi.BCF
{{
    // This file is automatically generated from the build script
    [System.CodeDom.Compiler.GeneratedCode(""GitVersionBuild"", """")]
    public static class FileVersionProvider
    {{
        public static string AssemblyVersion => ""{GitVersion.Major}.{GitVersion.Minor}.{GitVersion.Patch}.0"";
        public static string FileVersion => ""{GitVersion.MajorMinorPatch}"";
        public static string NuGetVersion => ""{GitVersion.NuGetVersion}"";
        public static DateTime BuildDateUtc => {dateCode};
    }}
}}
";

        WriteAllText(fileVersionPath, fileVersionCode);
    }

    Target Pack => _ => _
        .DependsOn(Compile)
        .Executes(() =>
        {
            var changeLog = GetCompleteChangeLog(ChangeLogFile)
                .EscapeStringPropertyForMsBuild();

            DotNetPack(s => s
                .SetPackageReleaseNotes(changeLog)
                .SetConfiguration(Configuration)
                .SetVersion(GitVersion.NuGetVersion)
                .SetOutputDirectory(OutputDirectory)
                .SetDescription("iabi.BCF")
                .SetTitle("iabi.BCF")
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

            await DotCoverToCobertura(s => s
                    .SetInputFile(OutputDirectory / "coverage.xml")
                    .SetOutputFile(OutputDirectory / "cobertura_coverage.xml"))
                .ConfigureAwait(false);
        });

    Target Push => _ => _
        .DependsOn(Pack)
        .Requires(() => IabiGitHubPackageSource)
        .Requires(() => IabiGitHubPackageApiKey)
        .Requires(() => NuGetApiKey)
        .Executes(() =>
        {
            GlobFiles(OutputDirectory, "*.nupkg").NotEmpty()
                .Where(x => !x.EndsWith("symbols.nupkg"))
                .ForEach(x =>
                {
                    DotNetNuGetPush(s => s
                        .SetTargetPath(x)
                        .SetSource(IabiGitHubPackageSource)
                        .SetApiKey(IabiGitHubPackageApiKey));

                    if (GitVersion.BranchName.Equals("master") || GitVersion.BranchName.Equals("origin/master"))
                    {
                        DotNetNuGetPush(s => s
                           .SetTargetPath(x)
                           .SetSource("https://api.nuget.org/v3/index.json")
                           .SetApiKey(NuGetApiKey));
                    }
                });
        });

    Target PublishGitHubRelease => _ => _
        .DependsOn(Push)
        .Requires(() => GitHubAuthenticationToken)
        .OnlyWhenDynamic(() => GitVersion.BranchName.Equals("master") || GitVersion.BranchName.Equals("origin/master"))
        .Executes<Task>(async () =>
        {
            var releaseTag = $"v{GitVersion.MajorMinorPatch}";

            var changeLogSectionEntries = ExtractChangelogSectionNotes(ChangeLogFile);
            var latestChangeLog = changeLogSectionEntries
                .Aggregate((c, n) => c + Environment.NewLine + n);
            var completeChangeLog = $"## {releaseTag}" + Environment.NewLine + latestChangeLog;

            var repositoryInfo = GetGitHubRepositoryInfo(GitRepository);
            var nuGetPackages = GlobFiles(OutputDirectory, "*.nupkg").NotEmpty().ToArray();

            await PublishRelease(x => x
                .SetArtifactPaths(nuGetPackages)
                .SetCommitSha(GitVersion.Sha)
                .SetReleaseNotes(completeChangeLog)
                .SetRepositoryName(repositoryInfo.repositoryName)
                .SetRepositoryOwner(repositoryInfo.gitHubOwner)
                .SetTag(releaseTag)
                .SetToken(GitHubAuthenticationToken));
        });
}
