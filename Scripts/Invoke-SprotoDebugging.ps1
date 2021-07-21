param
(
	[Parameter()]
	[string] $ProjectPath,
	[Parameter()]
	[switch] $Rollback
)

$ErrorActionPreference = "STOP"

Clear-Host

$versionFull = "3.0.5.0"
$version = "3.0.5"
$files = Get-ChildItem $ProjectPath *.csproj -Recurse | Select-Object Fullname

$sprotoR = "<Reference Include=`"Sproto, Version=$($versionFull), Culture=neutral, processorArchitecture=MSIL`"><HintPath>..\packages\Sproto.$version\lib\netstandard2.0\Sproto.dll</HintPath></Reference>"
$sprotoPR = "<PackageReference Include=`"Sproto`" Version=`"$version`" />"
$sprotoPR2 = "<PackageReference Include=`"Sproto`"><Version>$version</Version></PackageReference>"
$sprotoPCR = "<Reference Include=`"Sproto, Version=$($versionFull), Culture=neutral, processorArchitecture=MSIL`"><HintPath>..\packages\Sproto.$version\lib\netstandard2.0\Sproto.dll</HintPath></Reference>"
$sprotoNR = "<Reference Include=`"Sproto`"><HintPath>C:\Workspaces\GitHub\Sproto\Sproto\bin\Debug\netstandard2.0\Sproto.dll</HintPath></Reference>"

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
			$data = $data.Replace($sprotoNR, $sprotoPCR)
		}
		else
		{
			$data = $data.Replace($sprotoNR, $sprotoPR)
		}
	}
	else
	{
		$data = $data.Replace($sprotoR, $sprotoNR)
		$data = $data.Replace($sprotoPR, $sprotoNR)
		$data = $data.Replace($sprotoPR2, $sprotoNR)
	}
	
	$data = Format-Xml -Data $data -IndentCount 4 -IndentCharacter ' '
	$file.FullName
	
	Set-Content $file.FullName -Value $data -Encoding UTF8
}