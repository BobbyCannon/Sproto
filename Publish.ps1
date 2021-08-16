param
(
	[Parameter()]
	[Switch] $Preview
)

$scriptPath = $PSScriptRoot
# $scriptPath = "C:\Workspaces\GitHub\Sproto"

$files = Get-ChildItem "$scriptPath\Binaries" -Filter "Sproto.*.nupkg" -File

foreach ($file in $files)
{
	Write-Host $file.FullName -ForegroundColor Cyan
	
	if ($Preview.IsPresent)
	{
		continue
	}

	& "nuget.exe" push $file.FullName -Source https://www.nuget.org/api/v2/package

	if (Test-Path "C:\Workspaces\Nuget\Release")
	{
		Copy-Item $file.FullName "C:\Workspaces\Nuget\Release"
	}
}