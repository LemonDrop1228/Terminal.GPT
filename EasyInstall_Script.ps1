# Function to generate the command to be added
function addPathToEnvironmentVariableCommand ($appPath) {
    $messageBody = @"
    To add TerminalGPT.exe's root path to the system environment variables, please copy and paste the following command into your PowerShell:

        [Environment]::SetEnvironmentVariable("PATH",`$ENV:Path+";$appPath", [System.EnvironmentVariableTarget]::User)

    This command adds TerminalGPT into the PATH system environment variables, allowing you to easily run TerminalGPT from any command prompt or PowerShell session without specifying the full path to TerminalGPT.exe.
    "@
    Write-Output $messageBody
}

# Initialize WebClient
$client = New-Object System.Net.WebClient;

# URL and file path setting
$url = 'https://github.com/LemonDrop1228/TerminalGPT/releases/latest/download/Terminal-GPT.zip';
$file = "$env:TEMP\Terminal-GPT.zip";


# Cleanup function
function cleanupOnError() {
    # If download started, delete downloaded zip
    if (Test-Path $file) {
        Write-Output "Deleting downloaded zip at $file"
        Remove-Item -Path $file -ErrorAction SilentlyContinue -Force
    }

    # If unzip started, delete unzipped files
    if (Test-Path $destination) {
        Write-Output "Deleting Unzipped files from $destination"
        Remove-Item -Path $destination -Recurse -ErrorAction SilentlyContinue
    }

    # If shortcut was created, delete it
    if (Test-Path $shortcutLocation) {
        Write-Output "Deleting Shortcut at $shortcutLocation"
        Remove-Item -Path $shortcutLocation -ErrorAction SilentlyContinue
    }
}

Write-Output "Attempting to download Terminal-GPT.zip from `n$url"
try {
    $client.DownloadFile($url, $file);
    Write-Output "`nDownload Successful. Proceeding to next step`n"
} catch {
    Write-Error "`nDownload Failed with the following exception: $_. Please make sure you're connected to the internet.`n"
    Read-Host "`nPress Enter to clean up and exit the script"
    cleanupOnError
    exit
}

$shell = new-object -com shell.application;
$zip = $shell.NameSpace($file);
$destination = "$env:LOCALAPPDATA\TerminalGPT"

Write-Output "`nProcess to unzip and move the files will now start`n"
if(Test-Path $destination) {
    Write-Output "`nRemoving any previous files at $destination"
    Remove-Item -Path $destination -Recurse
}

try {
    foreach($item in $zip.items()){
        $shell.Namespace($destination).copyhere($item);
    }
    Write-Output "`nFile extraction Successful. Proceeding to next step"
} catch {
    Write-Error "`nFailed to unzip files with the following exception: $_. Please ensure you have necessary privileges`n"
    Read-Host "`nPress Enter to clean up and exit the script"
    cleanupOnError
    exit
}

Remove-Item -Path $file;
$shortcutLocation = "$env:USERPROFILE\Desktop\TerminalGPT.lnk";

Write-Output "`nShortcut creation process will now start"
if(Test-Path -Path $shortcutLocation) {
    Write-Output "`nRemoving any previous shortcut at $shortcutLocation"
    Remove-Item -Path $shortcutLocation
}

$WshShell = New-Object -comObject WScript.Shell;
$Shortcut = $WshShell.CreateShortcut($shortcutLocation);
$Shortcut.TargetPath = "$destination\TerminalGPT.exe";

try {
    $Shortcut.Save();
    Write-Output "`nShortcut creation Successful."
} catch {
    Write-Error "`nShortcut creation failed with the following exception: $_. Please make sure you're running this script as an Administrator`n"
    Read-Host "`nPress Enter to clean up and exit the script"
    cleanupOnError
    exit
}

# Final message
Write-Output "`nInstallation completed! A shortcut to TerminalGPT has been created on your Desktop."
addPathToEnvironmentVariableCommand($destination)
