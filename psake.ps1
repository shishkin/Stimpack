$psake = resolve-path .\packages\psake.*\tools\psake.ps1
$script = resolve-path .\default.ps1
& $psake $script @args
