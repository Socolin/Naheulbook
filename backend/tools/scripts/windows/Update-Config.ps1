Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"
$PSDefaultParameterValues['*:ErrorAction'] = 'Stop'
function ThrowOnNativeFailure {
	if (-not$?) {
		throw 'Native Failure'
	}
}

$NaheulbookRootPath = $PSScriptRoot;
Write-Host "Running from: " $NaheulbookRootPath
$Depth = 0;
while ($true) {
	$NaheulbookRootPath = Join-Path $NaheulbookRootPath -ChildPath ".." -Resolve
	$TestSlnFilePath = Join-Path $NaheulbookRootPath -ChildPath "Naheulbook.sln"
	if (Test-Path -Path $TestSlnFilePath -PathType Leaf) {
		break;
	}
	$Depth++;
	if ($Depth -gt 10) {
		throw "Failed to find Naheulbook root folder";
	}
}

dotnet run --project $NaheulbookRootPath\tools\Naheulbook.UpdateDevConfig\Naheulbook.UpdateDevConfig.csproj
ThrowOnNativeFailure;