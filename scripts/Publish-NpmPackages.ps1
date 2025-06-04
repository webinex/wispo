param(
  [Parameter(Mandatory = $true)][string] $AuthToken,
  [bool] $KeepNpmRc = $false
)

$ErrorActionPreference = "Stop"

$slnRoot = [IO.Path]::Combine($PSScriptRoot, '..', 'src') | Resolve-Path

$packages = @(
  "Webinex.Wispo.NpmPackage",
  "Webinex.Wispo.FCM.NpmPackage"
)

function Get-PackageInfo([Parameter(Mandatory = $true)] [string] $Name) {
  $path = [IO.Path]::Combine($slnRoot, $Name);
  $package = Get-Content -Path "$path/package.json" | ConvertFrom-Json

  return [PSCustomObject]@{
    Name = $package.name;
    Version = $package.version;
    Path = $path;
  }
}

function Get-EncodedName($Name) {
  $Name = $Name.Replace("/", "%2f");
  return $Name
}

function Get-IsVersionPublished([Parameter(Mandatory = $true)] $PackageInfo) {
  $name = Get-EncodedName -Name $PackageInfo.Name
  $version = $PackageInfo.Version

  $ErrorActionPreference = "Stop"
  try {
    Invoke-RestMethod "https://registry.npmjs.org/$name/$version" | Out-Null
    return $true
  } catch {
    $global:LASTEXITCODE = $null
    return $false
  }
}

function Write-PackageBlur($Name) {
  Write-Host ""
  Write-Host -ForegroundColor Cyan "-------------------------------------------"
  Write-Host -ForegroundColor Cyan "    $Name"
  Write-Host -ForegroundColor Cyan "-------------------------------------------"
  Write-Host ""
}

function Write-PublishingBlur($Name, $Skip) {
  if ($Skip -eq $true) {
    Write-Host -ForegroundColor Cyan "[$Name]: Published. Skip...."
  } else {
    Write-Host -ForegroundColor Cyan "[$Name]: Not Published. Publishing..." 
  }
}

function Write-PackageInfo($PackageInfo) {
  $PackageInfo | Format-Table -AutoSize | Out-String | Write-Host -ForegroundColor Green
}

function Get-NpmRcPath($PackageInfo) {
  return "$($PackageInfo.Path)/.npmrc"
}

function Add-NpmRc($PackageInfo) {
  $npmRcPath = Get-NpmRcPath -PackageInfo $PackageInfo
  New-Item -Path  $npmRcPath -Force | Out-Null
  Set-Content -Path  $npmRcPath -Value "//registry.npmjs.org/:_authToken=$AuthToken" | Out-Null
}

function Remove-NpmRc($PackageInfo) {
  $npmRcPath = Get-NpmRcPath -PackageInfo $PackageInfo
  Remove-Item -Path $npmRcPath | Out-Null
}

function Invoke-PublishNpmPackage($PackageInfo) {
  Push-Location $PackageInfo.Path
  yarn install --frozen-lockfile
  if ($LASTEXITCODE -ne 0) { exit 1 }

  npm publish --access public
  if ($LASTEXITCODE -ne 0) { exit 1 }

  Pop-Location
}

function Publish-Package([Parameter(Mandatory = $true)] [string] $Name) {
  Write-PackageBlur -Name $Name
  $packageInfo = Get-PackageInfo -Name $Package
  $published = Get-IsVersionPublished -PackageInfo $packageInfo
  Write-PublishingBlur -Name $Name -Skip $published

  if ($published -eq $true) {
    return;
  }

  Write-PackageInfo -PackageInfo $packageInfo

  Add-NpmRc -PackageInfo $packageInfo

  try {
    Invoke-PublishNpmPackage -PackageInfo $packageInfo
  } finally {
    if ($KeepNpmRc -ne $true) {
      Remove-NpmRc -PackageInfo $packageInfo
    }
  }
}

function Publish-Packages() {
  foreach ($package in $packages) {    
    Publish-Package -Name $package
  }
}

Publish-Packages
exit 0