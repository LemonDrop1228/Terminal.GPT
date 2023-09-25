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
$url = 'https://github.com/LemonDrop1228/Terminal.GPT/releases/latest/download/Terminal-GPT.zip';
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


try {
    $client.DownloadFile($url, $file);
} catch {
    Write-Error "Download failed with the following exception: $_. Please make sure you're connected to the internet."
    cleanupOnError
    exit
}

$shell = new-object -com shell.application;
$zip = $shell.NameSpace($file);
$destination = "$env:LOCALAPPDATA\TerminalGPT"

if(Test-Path $destination) { Remove-Item -Path $destination -Recurse }

try {
    foreach($item in $zip.items()){
        $shell.Namespace($destination).copyhere($item);
    }
} catch {
    Write-Error "Unzipping failed with the following exception: $_. Please ensure you have necessary privileges."
    cleanupOnError
    exit
}

Remove-Item -Path $file;
$shortcutLocation = "$env:USERPROFILE\Desktop\TerminalGPT.lnk";

if(Test-Path -Path $shortcutLocation) { Remove-Item -Path $shortcutLocation }

$WshShell = New-Object -comObject WScript.Shell;
$Shortcut = $WshShell.CreateShortcut($shortcutLocation);
$Shortcut.TargetPath = "$destination\TerminalGPT.exe";

try {
    $Shortcut.Save();
} catch {
    Write-Error "Shortcut creation failed with the following exception: $_. Please make sure you're running this script as an Administrator."
    cleanupOnError
    exit
}

# Final message
Write-Output "Installation completed! A shortcut to TerminalGPT has been created on your Desktop."
addPathToEnvironmentVariableCommand($destination)
