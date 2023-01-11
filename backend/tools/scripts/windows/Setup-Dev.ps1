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

function Add-HostToHostFile(
		[string] $Hostname,
		[string] $Ip
) {
	$HostFile = [Environment]::ExpandEnvironmentVariables("%systemroot%/system32/drivers/etc/hosts")
	$Line = "$( $Ip ) $( $Hostname ) #naheulbook"
	if ( (Get-Content $HostFile).Contains($Line)) {
		return;
	}
	if (!([Security.Principal.WindowsPrincipal][Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole]"Administrator")) {
		Start-Process pwsh "-NoProfile -ExecutionPolicy Bypass -c `"Add-Content '$HostFile' '$Line'`""  -Verb RunAs
		ThrowOnNativeFailure;
	}
}

Write-Host "Using backend root path: $NaheulbookRootPath"

Add-HostToHostFile 'local.naheulbook.fr' '127.0.0.1';
Write-Host -ForegroundColor Cyan Host local.naheulbook.fr added

dotnet run --project $NaheulbookRootPath\tools\Naheulbook.GenerateDevCertificate\Naheulbook.GenerateDevCertificate.csproj
ThrowOnNativeFailure;
Write-Host -ForegroundColor Cyan Certificate generated

docker compose --project-directory $NaheulbookRootPath\tools\scripts\windows\ up -d --force-recreate
ThrowOnNativeFailure;
Write-Host -ForegroundColor Cyan Db started

dotnet run --project $NaheulbookRootPath\tools\Naheulbook.SetupDevDatabase\Naheulbook.SetupDevDatabase.csproj
ThrowOnNativeFailure;
Write-Host -ForegroundColor Cyan Db initialized

iex $NaheulbookRootPath\tools\scripts\windows\Update-Config.ps1
Write-Host -ForegroundColor Cyan Config Updated

pushd $NaheulbookRootPath\tools\Naheulbook.DatabaseMigrator.Cli
try {
	dotnet run -- --operation init
	ThrowOnNativeFailure;
}
finally {
	popd
}

Write-Host -ForegroundColor Green Done
