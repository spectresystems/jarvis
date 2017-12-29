#load "./build/version.cake"
#tool nuget:?package=Wix&version=3.11.0
#tool nuget:?package=chocolatey&version=0.10.8&exclude=./tools/chocolateyinstall/redirects/choco.exe
#tool nuget:?package=xunit.runner.console&version=2.3.1

///////////////////////////////////////////////////////////////////////////////
// ARGUMENTS
///////////////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");
var patch = HasArgument("patch");

///////////////////////////////////////////////////////////////////////////////
// GLOBALS
///////////////////////////////////////////////////////////////////////////////

var version = BuildVersion.Calculate(Context);

///////////////////////////////////////////////////////////////////////////////
// TASKS
///////////////////////////////////////////////////////////////////////////////

Task("Clean")
    .Does(() =>
{
    CleanDirectory("./.artifacts");
    CleanDirectory("./.artifacts/bin");
    CleanDirectory("./.artifacts/installer");
    CleanDirectory("./.artifacts/installer/bin");
});

Task("Restore")
    .IsDependentOn("Clean")
    .Does(() =>
{
    NuGetRestore("./src/Jarvis.sln");
});

Task("Patch-Version")
    .WithCriteria(() => patch)
    .Does(() =>
{
    CreateAssemblyInfo("./src/SharedAssemblyInfo.cs", new AssemblyInfoSettings 
    {
        Version = version.Version,
        FileVersion = version.Version,
        InformationalVersion = version.SemVersion,
        Company = "Spectre Systems AB",
        Copyright = "Copyright (c) 2017 Spectre Systems AB",
        Product = "Jarvis"
    });
});

Task("Build")
    .IsDependentOn("Restore")
    .IsDependentOn("Patch-Version")
    .Does(() => 
{
   MSBuild("./src/Jarvis.sln", new MSBuildSettings()
        .SetConfiguration(configuration)
        .SetVerbosity(Verbosity.Minimal)
        .WithWarningsAsError()
        .WithProperty("AssemblyVersion", "2.0.0.0"));
});

Task("Run-Unit-Tests")
    .IsDependentOn("Build")
    .Does(() =>
{
    XUnit2($"./src/**/bin/{configuration}/*.Tests.dll");
});

Task("Copy-Binaries")
    .IsDependentOn("Build")
    .Does(() => 
{
    CopyFiles($"./src/Jarvis/bin/{configuration}/*", "./.artifacts/bin");
    CopyFiles($"./src/Jarvis/bin/{configuration}/*", "./.artifacts/installer/bin");

    DeleteFiles("./.artifacts/installer/bin/*.xml");
    DeleteFiles("./.artifacts/installer/bin/*.pdb");
});

Task("Build-Installer")
    .IsDependentOn("Run-Unit-Tests")
    .IsDependentOn("Copy-Binaries")
    .Does(() =>
{
    Information("Compiling installer...");
    WiXCandle("./installer/wix/Jarvis.wxs", new CandleSettings
    {
        NoLogo = true,
        Architecture = Architecture.X64,
        OutputDirectory = "./.artifacts/installer",
        Extensions = new[] { "WixUtilExtension" },
        Defines = new Dictionary<string, string>
        {
            { "Configuration", configuration },
            { "PublishDirectory", "./.artifacts/bin" },
            { "Platform", "x64" },
            { "ResourceDirectory", "./res" },
            { "Version", version.Version }
        }
    });

    Information("Linking installer...");
    WiXLight("./.artifacts/installer/Jarvis.wixobj", new LightSettings
    {
        NoLogo = true,
        Extensions = new [] { "WixBalExtension", "WixNetFxExtension", "WixUtilExtension" },
        OutputFile = "./.artifacts/installer/Jarvis.msi"
    });

    Information("Compiling bundle...");
    var wxsFiles = new[] {
        new FilePath("./installer/wix/Bundle.wxs"),
        new FilePath("./installer/wix/Prereqs.wxs")
    };
    WiXCandle(wxsFiles, new CandleSettings
    {
        NoLogo = true,
        Architecture = Architecture.X64,
        Extensions = new [] { "WixBalExtension", "WixNetFxExtension", "WixUtilExtension" },
        OutputDirectory = "./.artifacts/installer",
        Defines = new Dictionary<string, string>
        {
            { "JarvisInstaller", "./.artifacts/installer/Jarvis.msi" },
            { "Platform", "x64" },
            { "ResourceDirectory", "./res" },
            { "Version", version.Version }
        }
    });

    Information("Linking bundle...");
    var wixobjFiles = new[] {
        new FilePath("./.artifacts/installer/Bundle.wixobj"),
        new FilePath("./.artifacts/installer/Prereqs.wixobj")
    };
    WiXLight(wixobjFiles, new LightSettings
    {
        NoLogo = true,
        Extensions = new [] { "WixBalExtension", "WixNetFxExtension", "WixUtilExtension" },
        OutputFile = "./.artifacts/installer/Jarvis-Installer.exe"
    });

    // Move the file to the artifact root.
    if(FileExists("./.artifacts/installer/Jarvis-Installer.exe"))
    {
        MoveFile("./.artifacts/installer/Jarvis-Installer.exe", $"./.artifacts/Jarvis-{version.SemVersion}-x64.exe");
    }
});

Task("Build-Chocolatey-Package")
    .IsDependentOn("Build-Installer")
    .Does(() => 
{
    TransformTextFile("./installer/chocolatey/chocolateyinstall.ps1.template", "%{", "}")
        .WithToken("Installer", $"Jarvis-{version.SemVersion}-x64.exe")
        .WithToken("Version", version.SemVersion)
        .Save("./installer/chocolatey/chocolateyinstall.ps1");

    var nuspec = MakeAbsolute(File("./installer/chocolatey/jarvis.nuspec"));
    ChocolateyPack(nuspec, new ChocolateyPackSettings
    {
        Version = version.SemVersion,
        OutputDirectory = "./.artifacts"
    });
});

///////////////////////////////////////////////////////////////////////////////
// TARGETS
///////////////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("Build-Chocolatey-Package");

///////////////////////////////////////////////////////////////////////////////
// EXECUTION
///////////////////////////////////////////////////////////////////////////////

RunTarget(target);