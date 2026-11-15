# Building DNN Blog

This document describes how to build the DNN Blog module on Windows.

## Tested environment
- DNN Platform 10.3.2
- .NET Framework 4.8
- Visual Studio 2022 or Visual Studio Build Tools
- MSBuild
- SQL Server
- GitHub Desktop

## Required Visual Studio components
- .NET desktop build tools
- MSBuild
- Visual Basic compiler
- C# compiler
- NuGet targets and build tasks
- .NET Framework 4.8 SDK
- .NET Framework 4.8 Targeting Pack

## DNN references
Use the matching assemblies from the target DNN site's `bin` folder.

## Build steps
1. Clone the repository.
2. Open Developer PowerShell for Visual Studio.
3. Run:

```powershell
msbuild ".\\Server\\Blog\\DotNetNuke.Modules.Blog.vbproj" `
  /t:Restore,Rebuild `
  /p:Configuration=Release `
  /p:Platform=AnyCPU
```

## Output
Primary assemblies:
- `DotNetNuke.Modules.Blog.dll`
- `DotNetNuke.Modules.Blog.Core.dll`
- `CookComputing.XmlRpcV2.dll`

## Testing
Back up the database, `/bin`, `/DesktopModules/Blog` and `web.config` before deployment.
