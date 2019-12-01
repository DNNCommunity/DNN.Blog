#addin "Cake.FileHelpers"
#addin "Cake.Npm"
#addin nuget:?package=Dnn.CakeUtils
#addin nuget:?package=Markdig

using Dnn.CakeUtils;
using Dnn.CakeUtils.Compression;
using Dnn.CakeUtils.Manifest;

///////////////////////////////////////////////////////////////////////////////
// VARIABLES
///////////////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");
var solution = Solution.New(".\\package.json");
var buildSettings = new MSBuildSettings()
.SetConfiguration(configuration)
.UseToolVersion(MSBuildToolVersion.VS2017)
.WithProperty("OutDir", new System.IO.DirectoryInfo(solution.dnn.pathsAndFiles.pathToAssemblies).FullName);

///////////////////////////////////////////////////////////////////////////////
// TASKS
///////////////////////////////////////////////////////////////////////////////

Task("AssemblyInfo")
.Does(() => {
    var ptrn = new string[] {"./**/AssemblyInfo.*"};
    var files = GetFilesByPatterns(ptrn, solution.dnn.pathsAndFiles.excludeFilter);
    foreach(var file in files)
    {
        Information("Updating Assembly: {0}", file);
        Utilities.UpdateAssemblyInfo(solution, file.FullPath);
    }
});

Task("Build")
.IsDependentOn("AssemblyInfo")
.Does(() => {
    CleanDirectory(solution.dnn.pathsAndFiles.pathToAssemblies);
    NuGetRestore(solution.dnn.pathsAndFiles.solutionFile);
    MSBuild(solution.dnn.pathsAndFiles.solutionFile, buildSettings);
    // NpmRunScript("build");
});

Task("Package")
.IsDependentOn("Build")
.Does(() => {
    var packageName = solution.dnn.pathsAndFiles.zipName + "_" + solution.version + "_Install.zip";
    var packagePath = solution.dnn.pathsAndFiles.packagesPath + "/" + packageName;
    if (!System.IO.Directory.Exists(solution.dnn.pathsAndFiles.packagesPath)) {
        System.IO.Directory.CreateDirectory(solution.dnn.pathsAndFiles.packagesPath);
    }
    if (System.IO.File.Exists(packagePath)) {
        System.IO.File.Delete(packagePath);
    }
    var addedDlls = new List<string>();
    foreach (var pfolder in solution.dnn.projectFolders) {
        var p = solution.dnn.projects[pfolder];
        Information("Loading " + p.name);
        var devPath = System.IO.Path.Combine(solution.dnn.pathsAndFiles.devFolder, pfolder);
        var releaseFiles = p.pathsAndFiles.releaseFiles == null ? solution.dnn.pathsAndFiles.releaseFiles : p.pathsAndFiles.releaseFiles;
        if (releaseFiles.Length > 0) {
            var excludeFiles = p.pathsAndFiles.excludeFilter;
            if (excludeFiles == null) {
                excludeFiles = solution.dnn.pathsAndFiles.excludeFilter;
            } else {
                excludeFiles = excludeFiles.Concat(solution.dnn.pathsAndFiles.excludeFilter).ToArray();
            }
            var files = GetFilesByPatterns(devPath, releaseFiles, excludeFiles);
            if (files.Count > 0) {
                var resZip = ZipToBytes(devPath, files);
                Information("Zipped resources file");
                AddBinaryFileToZip(packagePath, resZip, p.packageName + ".zip", true);
            }
            Information("Added resources from " + devPath);
        }
        foreach (var a in p.pathsAndFiles.assemblies) {
            if (!addedDlls.Contains(a)) {
                var files = GetFiles(solution.dnn.pathsAndFiles.pathToAssemblies + "/" + a);
                AddFilesToZip(packagePath, solution.dnn.pathsAndFiles.pathToAssemblies, solution.dnn.pathsAndFiles.packageAssembliesFolder, files, true);
                addedDlls.Add(a);
            }
        }
        if (!string.IsNullOrEmpty(p.pathsAndFiles.pathToScripts)) {
            var files = GetFiles(p.pathsAndFiles.pathToScripts + "/*.SqlDataProvider");
            AddFilesToZip(packagePath, p.pathsAndFiles.pathToScripts, solution.dnn.pathsAndFiles.packageScriptsFolder + "/" + p.packageName, files, true);
        }
    }
    if (solution.dnn.pathsAndFiles.licenseFile != "") {
        var license = Utilities.GetTextOrMdFile(System.IO.Path.GetFileNameWithoutExtension(solution.dnn.pathsAndFiles.licenseFile));
        if (license != "") {
            AddTextFileToZip(packagePath, license, "License.txt", true);
        }
    }
    if (solution.dnn.pathsAndFiles.releaseNotesFile != "") {
        var releaseNotes = Utilities.GetTextOrMdFile(System.IO.Path.GetFileNameWithoutExtension(solution.dnn.pathsAndFiles.releaseNotesFile));
        if (releaseNotes != "") {
            AddTextFileToZip(packagePath, releaseNotes, "ReleaseNotes.txt", true);
        }
    }
    var m = new Manifest(solution);
    AddXmlFileToZip(packagePath, m, solution.name + ".dnn", true);
});

Task("Default")
.IsDependentOn("Build")
.Does(() => {
});

Task("Test")
.IsDependentOn("Build")
.Does(() => {
    Information("Test");
    Information(solution.dnn.projects.Count);
});

RunTarget(target);

