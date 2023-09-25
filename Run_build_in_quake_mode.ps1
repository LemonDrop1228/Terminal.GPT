param (
    [Parameter(Mandatory=$true)]
    [string]$exeName
)

# Print the name of the executable.
Write-Host "Executable name: $exeName"

# Search for the executable in the current directory and its subdirectories. Get the location of the script beign ran.
$searchPath = Split-Path -Path $MyInvocation.MyCommand.Definition -Parent
$exePath = Get-ChildItem -Path $searchPath -Recurse -Filter $exeName -ErrorAction SilentlyContinue | Select-Object -First 1 -ExpandProperty FullName
$exeRoot = Split-Path -Path $exePath -Parent

# Check if the executable and its root directory are found.
if ($exePath -and $exeRoot) {
    # Print the path and run the command in Windows Terminal's Quake mode.
    Write-Host "Executable found at: $exePath"
    wt new-tab -p "Windows PowerShell" -d $exeRoot -c $exePath -w 169 -h 50
} else {
    Write-Host "Executable not found in: $searchPath"
}
