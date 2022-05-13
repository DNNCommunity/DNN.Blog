using Cake.Common.Diagnostics;
using Cake.Common.IO;
using Cake.Common.Tools.MSBuild;
using Cake.Common.Tools.NuGet;
using Cake.Core;
using Cake.Core.Diagnostics;
using Cake.Core.IO;
using Cake.Frosting;
using Cake.Npm;
using Dnn.CakeUtils;
using Dnn.CakeUtils.Manifest;
using System.Collections.Generic;
using System.Linq;

public static class Program
{
  public static int Main(string[] args)
  {
    return new CakeHost()
        .UseContext<BuildContext>()
        .Run(args);
  }
}

public class BuildContext : FrostingContext
{
  public Solution Solution { get; set; }

  public MSBuildSettings BuildSettings { get; set; }

  public BuildContext(ICakeContext context)
      : base(context)
  {
    context.Environment.WorkingDirectory = context.Environment.WorkingDirectory.FullPath + "/../";
    this.Solution = Solution.New(".\\package.json");
    this.BuildSettings = new MSBuildSettings()
        .SetConfiguration("Release")
        .UseToolVersion(MSBuildToolVersion.VS2022)
        .WithProperty("OutDir", new System.IO.DirectoryInfo(this.Solution.dnn.pathsAndFiles.pathToAssemblies).FullName);
  }
}

[TaskName("AssemblyInfo")]
public sealed class AssemblyInfoTask : FrostingTask<BuildContext>
{
  public override void Run(BuildContext context)
  {
    var ptrn = new string[] { "./**/AssemblyInfo.*" };
    var files = context.GetFilesByPatterns(ptrn, context.Solution.dnn.pathsAndFiles.excludeFilter);
    foreach (var file in files)
    {
      context.Information("Updating Assembly: {0}", file);
      context.UpdateAssemblyInfo(context.Solution, file);
    }
  }
}

[TaskName("Build")]
[IsDependentOn(typeof(AssemblyInfoTask))]
public sealed class BuildTask : FrostingTask<BuildContext>
{
  public override void Run(BuildContext context)
  {
    context.CleanDirectory(context.Solution.dnn.pathsAndFiles.pathToAssemblies);
    context.NuGetRestore(context.Solution.dnn.pathsAndFiles.solutionFile);
    context.MSBuild(context.Solution.dnn.pathsAndFiles.solutionFile, context.BuildSettings);
    //context.NpmRunScript("build");
  }
}

[TaskName("Package")]
[IsDependentOn(typeof(BuildTask))]
public sealed class PackageTask : FrostingTask<BuildContext>
{
  public override void Run(BuildContext context)
  {
    var packageName = context.Solution.dnn.pathsAndFiles.zipName + "_" + context.Solution.version + "_Install.zip";
    var packagePath = context.Solution.dnn.pathsAndFiles.packagesPath + "/" + packageName;
    if (!System.IO.Directory.Exists(context.Solution.dnn.pathsAndFiles.packagesPath))
    {
      System.IO.Directory.CreateDirectory(context.Solution.dnn.pathsAndFiles.packagesPath);
    }
    if (System.IO.File.Exists(packagePath))
    {
      System.IO.File.Delete(packagePath);
    }
    var addedDlls = new List<string>();
    foreach (var pfolder in context.Solution.dnn.projectFolders)
    {
      var p = context.Solution.dnn.projects[pfolder];
      context.Information("Loading " + p.name);
      var devPath = System.IO.Path.Combine(context.Solution.dnn.pathsAndFiles.devFolder, pfolder);
      var releaseFiles = p.pathsAndFiles.releaseFiles == null ? context.Solution.dnn.pathsAndFiles.releaseFiles : p.pathsAndFiles.releaseFiles;
      if (releaseFiles.Length > 0)
      {
        var excludeFiles = p.pathsAndFiles.excludeFilter;
        if (excludeFiles == null)
        {
          excludeFiles = context.Solution.dnn.pathsAndFiles.excludeFilter;
        }
        else
        {
          excludeFiles = excludeFiles.Concat(context.Solution.dnn.pathsAndFiles.excludeFilter).ToArray();
        }
        context.CreateResourcesFile(new DirectoryPath(devPath), packagePath, p.packageName, releaseFiles, excludeFiles);
      }
      foreach (var a in p.pathsAndFiles.assemblies)
      {
        if (!addedDlls.Contains(a))
        {
          var files = context.GetFiles(context.Solution.dnn.pathsAndFiles.pathToAssemblies + "/" + a);
          context.AddFilesToZip(packagePath, context.Solution.dnn.pathsAndFiles.pathToAssemblies, context.Solution.dnn.pathsAndFiles.packageAssembliesFolder, files, true);
          addedDlls.Add(a);
        }
      }
      if (!string.IsNullOrEmpty(p.pathsAndFiles.pathToScripts))
      {
        var files = context.GetFiles(p.pathsAndFiles.pathToScripts + "/*.SqlDataProvider");
        context.AddFilesToZip(packagePath, p.pathsAndFiles.pathToScripts, context.Solution.dnn.pathsAndFiles.packageScriptsFolder + "/" + p.packageName, files, true);
      }
      if (!string.IsNullOrEmpty(p.pathsAndFiles.pathToCleanupFiles))
      {
        var files = context.GetFiles(p.pathsAndFiles.pathToCleanupFiles + "/*.txt");
        context.AddFilesToZip(packagePath, p.pathsAndFiles.pathToCleanupFiles, context.Solution.dnn.pathsAndFiles.packageCleanupFolder + "/" + p.packageName, files, true);
      }
    }
    if (context.Solution.dnn.pathsAndFiles.licenseFile != "")
    {
      var license = context.GetTextOrMdFile(System.IO.Path.GetFileNameWithoutExtension(context.Solution.dnn.pathsAndFiles.licenseFile));
      if (license != "")
      {
        context.AddTextFileToZip(packagePath, license, "License.txt", true);
      }
    }
    if (context.Solution.dnn.pathsAndFiles.releaseNotesFile != "")
    {
      var releaseNotes = context.GetTextOrMdFile(System.IO.Path.GetFileNameWithoutExtension(context.Solution.dnn.pathsAndFiles.releaseNotesFile));
      if (releaseNotes != "")
      {
        context.AddTextFileToZip(packagePath, releaseNotes, "ReleaseNotes.txt", true);
      }
    }
    var m = new Manifest(context.Solution);
    context.AddXmlFileToZip(packagePath, m, context.Solution.name + ".dnn", true);
  }
}

[TaskName("Default")]
[IsDependentOn(typeof(PackageTask))]
public class DefaultTask : FrostingTask
{
}