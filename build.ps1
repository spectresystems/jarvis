$ENV:CAKE_NUGET_USEINPROCESSCLIENT='true'
$CakeVersion = "0.24.0"

# Make sure tools folder exists
$PSScriptRoot = Split-Path $MyInvocation.MyCommand.Path -Parent
$ToolPath = Join-Path $PSScriptRoot "tools"
if (!(Test-Path $ToolPath)) {
    Write-Verbose "Creating tools directory..."
    New-Item -Path $ToolPath -Type directory | out-null
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
#https://www.nuget.org/api/v2/package
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