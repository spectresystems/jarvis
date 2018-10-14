$ENV:CAKE_NUGET_USEINPROCESSCLIENT='true'
$ToolPath = Join-Path $PSScriptRoot "tools"
$NugetPath = Join-Path $ToolPath "nuget.exe"
$NugetUrl = "https://dist.nuget.org/win-x86-commandline/latest/nuget.exe"
$CakeVersion = "0.30.0"

# Make sure tools folder exists
$PSScriptRoot = Split-Path $MyInvocation.MyCommand.Path -Parent
if (!(Test-Path $ToolPath)) {
    Write-Verbose "Creating tools directory..."
    New-Item -Path $ToolPath -Type directory | out-null
}

# Try download NuGet.exe if not exists
if (!(Test-Path $NugetPath)) {
    Write-Verbose -Message "Downloading NuGet.exe..."
    (New-Object System.Net.WebClient).DownloadFile($NugetUrl, $NugetPath)
}

###########################################################################
# INSTALL CAKE
###########################################################################

Add-Type -AssemblyName System.IO.Compression.FileSystem
Function Unzip
{
    param([string]$zipfile, [string]$outpath)

    [System.IO.Compression.ZipFile]::ExtractToDirectory($zipfile, $outpath)
}

# Make sure Cake has been installed.
$NugetFeed = 'https://www.myget.org/F/cake/api/v2/package'

$CakePath = Join-Path $ToolPath "cake.$CakeVersion/Cake.exe"
if (!(Test-Path $CakePath)) {
    Write-Host "Installing Cake..."
     (New-Object System.Net.WebClient).DownloadFile("$NugetFeed/Cake/$CakeVersion", "$ToolPath\Cake.zip")
     Unzip "$ToolPath\Cake.zip" "$ToolPath/cake.$CakeVersion"
     Remove-Item "$ToolPath\Cake.zip"
}

###########################################################################
# RUN BUILD SCRIPT
###########################################################################

&"$CakePath" $args
exit $LASTEXITCODE