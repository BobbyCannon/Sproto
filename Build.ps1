param
(
	[Parameter(Mandatory = $false, Position = 0)]
	[string] $Configuration = "Release",
	[Parameter()]
	[string] $BuildNumber = "+"
)

$ErrorActionPreference = "Stop"
$watch = [System.Diagnostics.Stopwatch]::StartNew()
$scriptPath = $PSScriptRoot
$productName = "Sproto"

if ($scriptPath.Length -le 0)
{
	$scriptPath = "C:\Workspaces\GitHub\$productName"
}

$destination = "$scriptPath\Binaries"
$destination2 = "C:\Workspaces\Nuget\Developer"

Push-Location $scriptPath

if (Test-Path $destination -PathType Container)
{
	Remove-Item $destination -Recurse -Force
}

New-Item $destination -ItemType Directory | Out-Null
New-Item $destination\Bin -ItemType Directory | Out-Null

if (!(Test-Path $destination2 -PathType Container))
{
	New-Item $destination2 -ItemType Directory | Out-Null
}

try
{
	& "ResetAssemblyInfos.ps1"

	# Prepare the build for versioning!
	$newVersion = .\IncrementVersion.ps1 -Build $BuildNumber
	
	& nuget.exe restore "$scriptPath\$productName.sln"

	$msbuild = "C:\Program Files (x86)\Microsoft Visual Studio\2017\Professional\MSBuild\15.0\Bin\MSBuild.exe"
	& $msbuild "$scriptPath\$productName.sln" /p:Configuration="$Configuration" /p:Platform="Any CPU" /t:Rebuild /p:VisualStudioVersion=15.0 /v:m /m

	if ($LASTEXITCODE -ne 0)
	{
		Write-Host "Build has failed! " $watch.Elapsed -ForegroundColor Red
		exit $LASTEXITCODE
	}

	Copy-Item "$productName\bin\$Configuration\netstandard2.0\$productName.dll" "$destination\bin\"
	Copy-Item "$productName\bin\$Configuration\netstandard2.0\$productName.pdb" "$destination\bin\"
	
	$versionInfo = [System.Diagnostics.FileVersionInfo]::GetVersionInfo("$destination\bin\$productName.dll")
	$nugetVersion = $versionInfo.ProductVersionRaw.ToString(3)

	Copy-Item "$productName\bin\$Configuration\$productName.$nugetVersion.nupkg" "$destination\"
	Copy-Item "$productName\bin\$Configuration\$productName.$nugetVersion.nupkg" "$destination2\"
	
	if ($versionInfo.FileVersion.ToString() -ne $newVersion)
	{
		Write-Error "The new version $($versionInfo.FileVersion.ToString()) does not match the expected version ($newVersion)"
	}

	Write-Host
	Set-Location $scriptPath
	Write-Host "Build: " $watch.Elapsed -ForegroundColor Yellow
}
catch
{
	Write-Host $_.Exception.ToString() -ForegroundColor Red
	Write-Host "Build Failed:" $watch.Elapsed -ForegroundColor Red
	exit $LASTEXITCODE
}
finally
{
	Pop-Location
}