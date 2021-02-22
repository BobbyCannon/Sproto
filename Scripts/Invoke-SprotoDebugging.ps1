param
(
	[Parameter()]
	[string] $ProjectPath,
	[Parameter()]
	[switch] $Rollback
)

$ErrorActionPreference = "STOP"

Clear-Host

$versionFull = "2.0.0.0"
$version = "2.0.0"
$files = Get-ChildItem $ProjectPath *.csproj -Recurse | Select-Object Fullname

$speedyR = "<Reference Include=`"Sproto, Version=$($versionFull), Culture=neutral, PublicKeyToken=8db7b042d9663bf8, processorArchitecture=MSIL`"><HintPath>..\packages\Sproto.$version\lib\netstandard2.0\Sproto.dll</HintPath></Reference>"
$speedyPR = "<PackageReference Include=`"Sproto`" Version=`"$version`" />"
$speedyPR2 = "<PackageReference Include=`"Sproto`"><Version>$version</Version></PackageReference>"
$speedyPCR = "<Reference Include=`"Sproto, Version=$($versionFull), Culture=neutral, PublicKeyToken=8db7b042d9663bf8, processorArchitecture=MSIL`"><HintPath>..\packages\Sproto.$version\lib\netstandard2.0\Sproto.dll</HintPath></Reference>"

$speedyNR = "<Reference Include=`"Sproto`"><HintPath>C:\Workspaces\GitHub\Sproto\Sproto.EntityFramework\bin\Debug\netstandard2.0\Sproto.dll</HintPath></Reference>"

foreach ($file in $files)
{
	$directory = [System.IO.Path]::GetDirectoryName($file.FullName)
	$packagePath = $directory + "\packages.config"
	$packageExists = [System.IO.File]::Exists($packagePath)
	$data = Get-Content $file.FullName -Raw | Format-Xml -Minify
		
	if (!$data.ToString().Contains("Sproto"))
	{
		continue
	}
	
	if ($Rollback.IsPresent)
	{
		if ($packageExists)
		{
			$data = $data.Replace($speedyNR, $speedyPCR)
		}
		else
		{
			$data = $data.Replace($speedyNR, $speedyPR)
		}
	}
	else
	{
		$data = $data.Replace($speedyR, $speedyNR)
		$data = $data.Replace($speedyPR, $speedyNR)
		$data = $data.Replace($speedyPR2, $speedyNR)
	}
	
	$data = Format-Xml -Data $data -IndentCount 4 -IndentCharacter ' '
	$file.FullName
	
	Set-Content $file.FullName -Value $data -Encoding UTF8
}