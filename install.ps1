Param(
    [string] $Version = "1.0.0",
    [string] $PackageOutput = [System.IO.Path]::GetFullPath("$PSScriptRoot\published"),
    [string] $Configuration = "Release"
)

Write-Host "Package output: $PackageOutput"
Write-Host "Package version: $Version"
Write-Host "Configuration: $Configuration"

if (Test-Path $PackageOutput) {
    Remove-Item -path "$PackageOutput" -Recurse -Force -ErrorAction SilentlyContinue
}

$Major = $Version.Split('.')[0]
$Minor = $Version.Split('.')[1]
$AssemblyVersion = "$Major.$Minor.0.0"

if ($AssemblyVersion -eq "0.0.0.0") {
    $AssemblyVersion = "0.0.0.1"
}

Write-Host "Assembly version: $AssemblyVersion"

& dotnet restore ./CalendarWeek/CalendarWeek.sln --no-cache
if ($LASTEXITCODE -ne 0) {throw "Solution CalendarWeek restore failed"}

& dotnet publish ./CalendarWeek/CalendarWeek.csproj --configuration $Configuration --output "$PackageOutput" /p:Version=$Version /p:AssemblyVersion=$AssemblyVersion
if ($LASTEXITCODE -ne 0) {throw "Publish of CalendarWeek failed"}

$shortcut = (New-Object -ComObject Wscript.Shell).CreateShortcut((Join-Path -Path $env:APPDATA -ChildPath 'Microsoft\Windows\Start Menu\Programs\Startup\CalendarWeek.lnk'))
$shortcut.TargetPath = (Join-Path -Path $PackageOutput -ChildPath 'CalendarWeek.exe')
$shortcut.Save()
