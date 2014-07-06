Function Update-Version (
    [string]$versionFilePath,
    [string]$sharedAssemblyInfoFilePath
    )
{
    [string]$fullVersion = Get-Content $versionFilePath | Select -First 1
    Write-Host "Full version: $fullVersion"
    
    [string]$version = $fullVersion.Split('-')[0]
    Write-Host "Version: $version"

    $sharedAssemblyInfoContents = Get-Content $sharedAssemblyInfoFilePath
    $oldFileVersionMatch = $sharedAssemblyInfoContents | Select-String -Pattern "AssemblyFileVersion\(`"([0-9]+(\.([0-9]+|\*)){3})`"\)" | % { $_.Matches }  
    $oldFileVersionNumber = $oldFileVersionMatch.Groups[1].Value
    Write-Host "Old file version: $oldFileVersionNumber"

    $fileVersionParts = $oldFileVersionNumber.Split('.')  
    $fileVersionParts[3] = ([int]$fileVersionParts[3]) + 1  
    $fileVersion = "$version.{0}" -F $fileVersionParts[3]
    Write-Host "New file version: $fileVersion"

    $sharedAssemblyInfoReplace = @()
    ForEach ($line in $sharedAssemblyInfoContents) {
        $line = [RegEx]::Replace($line, "AssemblyVersion\(`"[0-9\.]*`"\)", "AssemblyVersion(`"$version`")")
        $line = [RegEx]::Replace($line, "AssemblyInformationalVersion\(`"[0-9\.]*`"\)", "AssemblyInformationalVersion(`"$version`")")
        $line = [RegEx]::Replace($line, "AssemblyFileVersion\(`"[0-9\.]*`"\)", "AssemblyFileVersion(`"$fileVersion`")")
        $sharedAssemblyInfoReplace += [Array]$line
    }
    $sharedAssemblyInfoReplace | Out-File $sharedAssemblyInfoFilePath -Encoding UTF8 -Force

    $versionDetails = New-Object PsObject -Property @{ version = $version; fullVersion = $fullVersion; fileVersion = $fileVersion }
    Return $versionDetails
}

Function Create-NugetPackage (
    [string]$packageName,
    [string]$fullVersion,
    [string]$nuspecTemplate,
    [string]$nugetExePath,
    [string]$sourceDir,
    [string[]]$assemblies,
    [string[]]$content,
    [string]$outputDir
    )
{
    New-Item -ItemType Directory -Force -Path "$sourceDir\Nuget\$packageName"
    ForEach ($filename in $content) {
        Copy-Item "$sourceDir\$filename" "$sourceDir\Nuget\$packageName"
    }

    New-Item -ItemType Directory -Force -Path "$sourceDir\Nuget\$packageName\lib\net45"
    ForEach ($filename in $assemblies) {
        Copy-Item "$sourceDir\$filename" "$sourceDir\Nuget\$packageName\lib\net45"
    }
    
    (Get-Content "$nuspecTemplate") `
        | ForEach-Object { $_ -Replace "<version>0.0.0</version>", "<version>$fullVersion</version>" } `
        | ForEach-Object { $_ -Replace "version=`"0.0.0`"", "version=`"$fullVersion`"" } `
        | Set-Content "$sourceDir\Nuget\$packageName\$packageName.nuspec" -Encoding UTF8

    & "$nugetExePath" Pack "$sourceDir\Nuget\$packageName\$packageName.nuspec" -OutputDirectory "$outputDir"
}

# Environment
$msbuild = "$Env:SystemRoot\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe"
$rootDir = Split-Path (Get-Variable MyInvocation -Scope Script).Value.MyCommand.Path

# Update version
$versionDetails = Update-Version "$rootDir\Version.txt" "$rootDir\Source\SharedAssemblyInfo.cs"

# Build dir
$rootBuildDir = "$rootDir\build"
$buildDir = "$rootBuildDir\$($versionDetails.fileVersion)"

# Compile
& $msbuild @("$rootDir\Source\Ominify.sln", "/p:Configuration=Release", "/p:OutputPath=`"$buildDir`"")

# Create Nuget packages
Copy-Item "$rootDir\License.txt" "$buildDir"

Create-NugetPackage `
    "Ominify.Core" `
    $versionDetails.fullVersion `
    "$rootDir\Nuget\Ominify.Core.template.nuspec" `
    "$rootDir\Nuget\NuGet.exe" `
    "$buildDir" `
    @("Ominify.Core.dll") `
    @("License.txt") `
    "$rootBuildDir"

Create-NugetPackage `
    "Ominify.Yui" `
    $versionDetails.fullVersion `
    "$rootDir\Nuget\Ominify.Yui.template.nuspec" `
    "$rootDir\Nuget\NuGet.exe" `
    "$buildDir" `
    @("Ominify.Yui.dll") `
    @("License.txt") `
    "$rootBuildDir"
