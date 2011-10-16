$framework = '4.0x86'
$build_dir = "$(resolve-path .)\build"
$nuget = resolve-path .\packages\nuget.*\tools\nuget.exe

task clean {
  rm $build_dir -recurse -force -errorAction silentlyContinue
}

task build -depends clean {
  exec { msbuild .\Stimpack.sln /p:Configuration=Release }
}

task nuspec {
  copy .\Stimpack.nuspec $build_dir\
  $nuspec = [xml](gc $build_dir\Stimpack.nuspec)
  $dependencies = $nuspec.CreateElement('dependencies')
  ([xml](gc .\Stimpack\packages.config)).packages.package | % {
    $d = $nuspec.CreateElement('dependency')
    $d.SetAttribute('id', $_.id)
    $d.SetAttribute('version', $_.version)
    $dependencies.AppendChild($d) | out-null
  }
  $nuspec.package.metadata.AppendChild($dependencies) | out-null
  $nuspec.Save("$build_dir\Stimpack.nuspec")
}

task package -depends build, nuspec {
  $version = (dir $build_dir\lib\net4\Stimpack.dll).VersionInfo.FileVersion
  exec { & $nuget pack $build_dir\Stimpack.nuspec -Version $version -OutputDirectory $build_dir }
}

task publish -depends package {
  $pack = resolve-path $build_dir\Stimpack.*.nupkg | select -last 1
  exec { & $nuget push $pack }
}

task default -depends build
