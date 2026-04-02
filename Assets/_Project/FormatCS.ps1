$targetDir = if ($args) { $args } else { "." }
$fullPath = (Resolve-Path $targetDir).Path

$dotnetToolsPath = Join-Path $env:USERPROFILE ".dotnet\tools"
$csharpierExe = Join-Path $dotnetToolsPath "csharpier.exe"

Write-Host "Formatting .cs files in: $fullPath" -ForegroundColor Cyan

if (!(Test-Path $csharpierExe)) {
    Write-Host "Installing csharpier..." -ForegroundColor Yellow
    dotnet tool install -g csharpier
}

# Добавили команду 'format' перед путем
& $csharpierExe format $fullPath
