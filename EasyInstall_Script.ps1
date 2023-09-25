# Function to generate the command to be added
function addPathToEnvironmentVariableCommand ($appPath) {
    if (!(Test-Path $appPath)) {
        Write-Host "App path '$appPath' does not exist. Exiting script."
        return
    }

    $messageBody = @"
To add TerminalGPT.exe's root path to the system environment variables, please copy and paste the following command into your PowerShell:
        [Environment]::SetEnvironmentVariable('PATH','$ENV:Path+';'+'$appPath',[System.EnvironmentVariableTarget]::User)
    This command adds TerminalGPT into the PATH system environment variables, allowing you to easily run TerminalGPT from any command prompt or PowerShell session without specifying the full path to TerminalGPT.exe.
    
"@
    
    Write-Host $messageBody
}


# Initialize WebClient
$client = New-Object System.Net.WebClient

# URL and file path setting
$url = 'https://github.com/LemonDrop1228/Terminal.GPT/releases/latest/download/Terminal-GPT.zip'
$file = "$env:TEMP\Terminal-GPT.zip"

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

Write-Host "Attempting to download Terminal-GPT.zip from `n$url"
try {
    $client.DownloadFile($url, $file)
    # write the downloaded file path to the console
    Write-Host "`nDownloaded file saved to $file"
    Write-Host "`nDownload Successful. Press any key to proceed to the next step."
    pause
}
catch {
    Write-Host "`nDownload Failed with the following exception: $_. Please make sure you're connected to the internet."
    Write-Host "Press any key to clean up and exit the script."
    pause
    cleanupOnError
    exit
}
$shell = New-Object -ComObject shell.application
$zip = $shell.NameSpace($file)
$destination = "$env:LOCALAPPDATA\TerminalGPT"

# if the destination folder exists, remove it
if(Test-Path $destination) {
    Write-Host "`nRemoving any previous files at $destination"
    Remove-Item -Path $destination -Recurs
}

# create destination folder 
Write-Host "`nCreating destination folder at $destination"
New-Item -Path $destination -ItemType Directory

Write-Host "`nProcess to unzip and move the files will now start"
# write the destination path to the console
Write-Host "`nUnzipping files to $destination"

try {
    foreach($item in $zip.items()){
        $shell.Namespace($destination).copyhere($item)
    }
    Write-Host "`nFile extraction Successful. Press any key to proceed to the next step."
    pause
}
catch {
    Write-Host "`nFailed to unzip files with the following exception: $_. Please ensure you have necessary privileges."
    Write-Host "Press any key to clean up and exit the script."
    pause
    cleanupOnError
    exit
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

try {
    $Shortcut.Save()
    Write-Host "`nShortcut creation Successful. Press any key to proceed."
    pause
}
catch {
    Write-Host "`nShortcut creation failed with the following exception: $_. Please make sure you're running this script as an Administrator."
    Write-Host "Press any key to clean up and exit the script."
    pause
    cleanupOnError
    exit
}

# Final message
Write-Host "`nInstallation completed! A shortcut to TerminalGPT has been created on your Desktop."
addPathToEnvironmentVariableCommand($destination)
Write-Host "`nPress any key to exit the script."