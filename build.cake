#tool nuget:?package=JetBrains.dotCover.CommandLineTools
#tool "nuget:?package=GitVersion.CommandLine&prerelease"
#addin "SharpZipLib"
#addin "Cake.Compression"
#addin "Cake.FileHelpers"

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

// Define directories.
var buildDir = Directory("build");
var testDir = Directory("Test Summary");
var deployDir = Directory("deploy");
var artifactsDir = Directory("artifacts");

// define runtime environments to build
var environment =  "win81-x64";

// get version
var version = GitVersion();
TeamCity.SetBuildNumber(version.FullSemVer);

Task("Clean")
    .Does(() =>
{
    CleanDirectory(buildDir);
    CleanDirectory(testDir);
    CleanDirectory(deployDir);
    CleanDirectory(artifactsDir);
	 CleanDirectories("src/obj");
    CleanDirectories("src/bin");	
});

Task("Restore")
    .IsDependentOn("Clean")
    .Does(() =>
{
   DotNetCoreRestore();
});

Task("Build")
	.IsDependentOn("Restore")
	.Does(() =>
{
  DotNetCorePublish("src/HelloWorld.csproj",
		new DotNetCorePublishSettings {
			Configuration = "Release",
			Runtime = environment,
			OutputDirectory = buildDir + Directory("HelloWorld"),
			ArgumentCustomization = args => args
				.Append("/p:DebugType=None")
				.Append("/p:Version=" + version.FullSemVer)
		});
});

Task("Test")
    .IsDependentOn("Build")
    .Does(() =>
{
	var testProjects = GetFiles("./tests/**/*.csproj");
    foreach(var testProject in testProjects) {
        DotCoverCover(
            tool => tool.DotNetCoreTest(testProject.FullPath),
            testDir + testProject.GetFilenameWithoutExtension() + ".dcvr",
            new DotCoverCoverSettings()
				.WithFilter("-:*Tests"));
	}
    DotCoverMerge(GetFiles(testDir.Path + "/*.dcvr"), testDir + File("iae.dcvr"));
    TeamCity.ImportDotCoverCoverage(testDir + File("iae.dcvr"));
});

Task("Package")
    .IsDependentOn("Test")
    .Does( () =>
{
		var appDir= Directory("HelloWorld");	
		ZipCompress(buildDir + appDir, deployDir + File("HelloWorld_" + version.FullSemVer + ".zip") );   
});
  
Task("Delete")
       .IsDependentOn("Package")
       .Does( ()=>
{
	CleanDirectory(buildDir);
});

// set default task
Task("Default").IsDependentOn("Delete");

RunTarget(target);