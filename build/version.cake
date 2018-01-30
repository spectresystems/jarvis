#load "./appveyor.cake"
#tool "nuget:https://api.nuget.org/v3/index.json?package=GitVersion.CommandLine&version=3.6.2"

public class BuildVersion
{
    public string MajorMinorPatch { get; private set; }
    public string MajorMinorPatchRevision { get; private set; }
    public string MsiVersion { get; private set; }
    public string SemVersion { get; private set; }
    public string DotNetAsterix { get; private set; }
    public string Milestone { get; private set; }

    public static BuildVersion Calculate(ICakeContext context, AppVeyorSettings appveyor)
    {
        if (context == null)
        {
            throw new ArgumentNullException("context");
        }

        string version = null;
        string fullVersion = null;
        string semVersion = null;
        string milestone = null;
        string msiVersion = null;

        if (context.IsRunningOnWindows())
        {
            context.Information("Calculating versions...");

            if (!appveyor.IsLocal)
            {
                // Update AppVeyor version number.
                context.GitVersion(new GitVersionSettings{
                    UpdateAssemblyInfo = true,
                    OutputType = GitVersionOutput.BuildServer,
                    UpdateAssemblyInfoFilePath = "./src/SharedAssemblyInfo.cs",
                });
            }
            
            GitVersion assertedVersions = context.GitVersion(new GitVersionSettings
            {
                OutputType = GitVersionOutput.Json,
            });

            var major = assertedVersions.Major;
            var minor = assertedVersions.Minor;
            var patch = assertedVersions.Patch;
            var revision = assertedVersions.PreReleaseNumber ?? 999;

            version = assertedVersions.MajorMinorPatch;
            semVersion = assertedVersions.LegacySemVerPadded;
            milestone = string.Concat("v", version);

            msiVersion = $"{major}.{minor}.{10000 + patch * 1000 + revision}";
            fullVersion = $"{version}.{revision}";

            context.Information("Milestone: {0}", milestone);
            context.Information("Version: {0}", version);
            context.Information("Full version: {0}", fullVersion);
            context.Information("Semantic Version: {0}", semVersion);
            context.Information("MSI Version: {0}", msiVersion);
        }

        if (string.IsNullOrEmpty(version) || string.IsNullOrEmpty(semVersion))
        {
            throw new CakeException("Could not parse version.");
        }

        return new BuildVersion
        {
            MajorMinorPatch = version,
            MajorMinorPatchRevision = fullVersion,
            MsiVersion = msiVersion,
            SemVersion = semVersion,
            DotNetAsterix = semVersion.Substring(version.Length).TrimStart('-'),
            Milestone = milestone
        };
    }
}