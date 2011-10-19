function set-assemblyVersion ($file, $version) {
  $content = gc $file
  $replacement = "`$1`"$version`""
  $content -replace '(AssemblyVersion\b*\()(["0-9.\*\b]+)', $replacement |
    set-content $file
}

set-assemblyVersion '.\CommonInfo.cs' '3.2.1.3456'
