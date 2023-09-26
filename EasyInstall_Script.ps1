# Function to update the Path Enviroment Variable
function addToPathEnvironmentVariable ($appPath) {
    $Path = [Environment]::GetEnvironmentVariable('Path', [System.EnvironmentVariableTarget]::Machine)
    if ($Path -notlike "*;$appPath*") {
        [Environment]::SetEnvironmentVariable('Path', "$Path;$appPath", [System.EnvironmentVariableTarget]::Machine)
    }
}

# Function to capture the old appsettings
function SaveAppSettings ($destination) {
    $appSettingsFile = Join-Path -Path $destination -ChildPath 'appsettings.json'
    if (Test-Path -Path $appSettingsFile) {
        $oldAppSettings = Get-Content -Path $appSettingsFile -Raw
        Write-Output $oldAppSettings
    } else {
        Write-Output $null
    }
}

# Function to restore the old appsettings
function RestoreAppSettings ($destination, $appSettingsContent) {
    $appSettingsFile = Join-Path -Path $destination -ChildPath 'appsettings.json'
    if (Test-Path -Path $appSettingsFile) {
        Set-Content -Path $appSettingsFile -Value $appSettingsContent
    }
}

# Initialize HttpClient
$client = New-Object System.Net.Http.HttpClient

# URL and file path setting
$url = 'https://github.com/LemonDrop1228/Terminal.GPT/releases/latest/download/Terminal-GPT.zip'
$file = "$env:TEMP\Terminal-GPT.zip"
$shell = New-Object -ComObject shell.application
$destination = "$env:LOCALAPPDATA\TerminalGPT"
$DoMigration = $False

# Check if the user wants to migrate settings
$response = Read-Host 'Would you like to migrate your app settings? (Y/N)'
if ($response -eq 'Y') {
    Write-Host "`nSaving old app settings..."
    $oldAppSettings = SaveAppSettings -destination $destination
    $DoMigration = $True
}

# Cleanup function
function cleanupOnError() {
    try {
        # If download started, delete downloaded zip
        if (Test-Path $file) {
            Write-Host "Deleting downloaded zip at $file"
            Remove-Item -Path $file -ErrorAction SilentlyContinue -Force
        }

        # If unzip started, delete unzipped files
        if (Test-Path $destination) {
            Write-Host "Deleting Unzipped files from $destination"
            Remove-Item -Path $destination -Recurse -ErrorAction SilentlyContinue
        }

        # If shortcut was created, delete it
        if (Test-Path $shortcutLocation) {
            Write-Host "Deleting Shortcut at $shortcutLocation"
            Remove-Item -Path $shortcutLocation -ErrorAction SilentlyContinue
        }
    }
    catch {
        Write-Host "Cleanup failed with the following exception: $_"
    }
}

# Download file with progress tracking
try {
    $response = $client.GetAsync($url).Result
    $response.EnsureSuccessStatusCode()

    $totalLength = $response.Content.Headers.ContentLength
    $contentStream = [System.IO.File]::Create($file)

    try {
        $downloadStream = $response.Content.ReadAsStreamAsync().Result
        $buffer = New-Object Byte[] 8192
        $progress = @{
            Id = 1
            Activity = "Downloading..."
            Status = "Bytes read"
            CurrentOperation = $contentStream.Position
            PercentComplete = 0
        }
        $totalRead = $null

        do {
            $read = $downloadStream.Read($buffer, 0, $buffer.Length)
            $contentStream.Write($buffer, 0, $read)
            $totalRead += $read
            $progress.CurrentOperation = "Downloaded {0:N2} MB of {1:N2} MB" -f ($totalRead / 1MB), ($totalLength / 1MB)
            $progress.PercentComplete = ($totalRead / $totalLength) * 100
            Write-Progress @progress
        } until ($read -eq 0)
    }
    finally {
        $contentStream.Dispose()
    }
}
catch {
    Write-Host "`nDownload Failed with the following exception: $_. Please make sure you're connected to the internet."
    Write-Host "Press any key to clean up and exit the script."
    pause
    cleanupOnError
    exit
}

# if the destination folder exists, remove it
if(Test-Path $destination) {
    Write-Host "`nRemoving any previous files at $destination"
    Remove-Item -Path $destination -Recurse
}

# create destination folder 
Write-Host "`nCreating destination folder at $destination"
New-Item -Path $destination -ItemType Directory

# Extracting files with progress indicator
$zip = $shell.NameSpace($file)
$items = $zip.items()
$totalItems = $items.Count
$i = 0;
$progress = @{
    Activity = "Extracting files..."
    PercentComplete = 0
}

foreach($item in $items) {
    try {
        $shell.Namespace($destination).copyhere($item)
    }
    catch {
        Write-Host "`nFailed to unzip file with the following exception: $_. Please ensure you have necessary privileges."
        Write-Host "Press any key to clean up and exit the script."
        pause
        cleanupOnError
        exit
    }

    $i++
    $progress.PercentComplete = ($i / $totalItems) * 100
    Write-Progress @progress
}

Remove-Item -Path $file
$shortcutLocation = "$env:USERPROFILE\Desktop\TerminalGPT.lnk"

Write-Host "`nShortcut creation process will now start"
if(Test-Path -Path $shortcutLocation) {
    Write-Host "`nRemoving any previous shortcut at $shortcutLocation"
    Remove-Item -Path $shortcutLocation
}

$WshShell = New-Object -ComObject WScript.Shell
$Shortcut = $WshShell.CreateShortcut($shortcutLocation)
$Shortcut.TargetPath = "$destination\TerminalGPT.exe"
$SkipShortcut = $false

try {
    $Shortcut.Save()
}
catch {
    Write-Host "`nShortcut creation failed with the following exception: $_."
    # Ask the user if they want to proceed without creating the shortcut
    $response = Read-Host 'Would you like to proceed without creating the shortcut? (Y/N)'
    if ($response -eq 'Y') {
        Write-Host "`nProceeding without creating the shortcut."
        # set skipShortcut to true
        $SkipShortcut = $True
    } else {
        Write-Host "`nPress any key to clean up and exit the script."
        pause
        cleanupOnError
        exit
    }
}

# After the new app is installed, before the completion message
if ($DoMigration -eq $True) {
    Write-Host "`nRestoring old app settings..."
    RestoreAppSettings -destination $destination -appSettingsContent $oldAppSettings
}

# Installation completion message
if ($SkipShortcut) {
    Write-Host "`nTerminalGPT was installed successfully. You can find the executable at $destination\TerminalGPT.exe"
    # Open the destination folder
    explorer.exe $destination
} else {
    Write-Host "`nTerminalGPT was installed successfully. You can find the shortcut at $shortcutLocation"
}

# Ask the user if they want to add the path to the Environment Variables
$response = Read-Host 'Would you like to add TerminalGPT to system environment variables? (Y/N)'
if ($response -eq 'Y') {
    addToPathEnvironmentVariable($destination)
    Write-Host "`nTerminalGPT added to the system environment variables."
} else {
    Write-Host "`nTerminalGPT was not added to the system enironment variables."
}

Write-Host "`nPress any key to exit the script."
