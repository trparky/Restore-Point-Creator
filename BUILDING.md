## Building from Source

### Prerequisites
Install dependencies using winget:
```powershell
winget install -e --id Git.Git
winget install -e --id Microsoft.NuGet
winget install -e --id Microsoft.VisualStudio.2022.BuildTools --override "--quiet --wait --add Microsoft.VisualStudio.Workload.MSBuildTools --add Microsoft.VisualStudio.Workload.NetDesktopBuildTools --includeRecommended"
winget install -e --id Microsoft.DotNet.Framework.DeveloperPack_4 --version 4.8.1
```

### Clone the Repository
```powershell
git clone https://github.com/trparky/Restore-Point-Creator.git
cd .\Restore-Point-Creator\
```

### Restore Required Packages
```powershell
nuget restore ".\Restore Point Creator (New Design).sln"
```

### Compile Time
```powershell
& "C:\Program Files (x86)\Microsoft Visual Studio\2022\BuildTools\MSBuild\Current\Bin\MSBuild.exe" ".\Restore Point Creator (New Design).sln" /t:Rebuild /p:Configuration=Debug
& "C:\Program Files (x86)\Microsoft Visual Studio\2022\BuildTools\MSBuild\Current\Bin\MSBuild.exe" ".\Restore Point Creator (New Design).sln" /t:Rebuild /p:Configuration=Release
```
