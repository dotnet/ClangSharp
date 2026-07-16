[CmdletBinding(PositionalBinding=$false)]
Param(
  [ValidateSet("<auto>", "amd64", "x64", "x86", "arm64", "arm")][string] $architecture = "",
  [string] $base = "",
  [switch] $build,
  [switch] $ci,
  [ValidateSet("Debug", "Release")][string] $configuration = "Debug",
  [switch] $detectchanges,
  [switch] $help,
  [string] $llvm = "",
  [switch] $pack,
  [switch] $regeneratenative,
  [switch] $restore,
  [ValidateSet("", "win-x64", "win-arm64", "linux-x64", "linux-arm64", "osx-arm64")][string] $rid = "",
  [string] $solution = "",
  [ValidateSet("", "libclang", "libClangSharp")][string] $target = "",
  [switch] $test,
  [switch] $testwin32metadata,
  [ValidateSet("quiet", "minimal", "normal", "detailed", "diagnostic")][string] $verbosity = "minimal",
  [switch] $verifypackages,
  [Parameter(ValueFromRemainingArguments=$true)][String[]]$properties
)

Set-StrictMode -Version 2.0
$ErrorActionPreference = "Stop"
[Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12

function Build() {
  $logFile = Join-Path -Path $LogDir -ChildPath "$configuration\build.binlog"
  & dotnet build -c "$configuration" --no-restore -v "$verbosity" /bl:"$logFile" /err $properties "$solution"

  if ($LastExitCode -ne 0) {
    throw "'Build' failed for '$solution'"
  }
}

function Create-Directory([string[]] $Path) {
  if (!(Test-Path -Path $Path)) {
    New-Item -Path $Path -Force -ItemType "Directory" | Out-Null
  }
}

function Help() {
    Write-Host -Object "Common settings:"
    Write-Host -Object "  -configuration <value>  Build configuration (Debug, Release)"
    Write-Host -Object "  -verbosity <value>      Msbuild verbosity (q[uiet], m[inimal], n[ormal], d[etailed], and diag[nostic])"
    Write-Host -Object "  -help                   Print help and exit"
    Write-Host -Object ""
    Write-Host -Object "Actions:"
    Write-Host -Object "  -restore                Restore dependencies"
    Write-Host -Object "  -build                  Build solution"
    Write-Host -Object "  -test                   Run all tests in the solution"
    Write-Host -Object "  -pack                   Package build artifacts"
    Write-Host -Object "  -regeneratenative       Download the matching LLVM release and stage a native binary"
    Write-Host -Object "                          (use with -target and -rid)"
    Write-Host -Object "  -verifypackages         Verify the libclang/libClangSharp package versions match CMakeLists.txt"
    Write-Host -Object "  -detectchanges          Print which native packages need regenerating since -base <ref>"
    Write-Host -Object ""
    Write-Host -Object "Advanced settings:"
    Write-Host -Object "  -solution <value>       Path to solution to build"
    Write-Host -Object "  -ci                     Set when running on CI server"
    Write-Host -Object "  -architecture <value>   Test Architecture (<auto>, amd64, x64, x86, arm64, arm)"
    Write-Host -Object "  -testwin32metadata      Test Win32Metadata to ensure it has not regressed"
    Write-Host -Object ""
    Write-Host -Object "Command line arguments not listed above are passed through to MSBuild."
    Write-Host -Object "The above arguments can be shortened as much as to be unambiguous (e.g. -co for configuration, -t for test, etc.)."
}

function Pack() {
  $logFile = Join-Path -Path $LogDir -ChildPath "$configuration\pack"

  & dotnet pack -c "$configuration" --no-build --no-restore -v "$verbosity" /bl:"$logFile.binlog" /err $properties "$solution"
  & dotnet pack -c "$configuration" --no-build --no-restore -v "$verbosity" /bl:"$logFile.agnostic.binlog" /err /p:SKIP_USE_CURRENT_RUNTIME=true $properties "$solution"

  if ($ci) {
    & dotnet pack -c "$configuration" --no-build --no-restore -v "$verbosity" /bl:"$logFile.preview.binlog" /err /p:PACKAGE_PUBLISH_MODE=preview $properties "$solution"
    & dotnet pack -c "$configuration" --no-build --no-restore -v "$verbosity" /bl:"$logFile.stable.binlog" /err /p:PACKAGE_PUBLISH_MODE=stable $properties "$solution"

    & dotnet pack -c "$configuration" --no-build --no-restore -v "$verbosity" /bl:"$logFile.agnostic.preview.binlog" /err /p:SKIP_USE_CURRENT_RUNTIME=true /p:PACKAGE_PUBLISH_MODE=preview $properties "$solution"
    & dotnet pack -c "$configuration" --no-build --no-restore -v "$verbosity" /bl:"$logFile.agnostic.stable.binlog" /err /p:SKIP_USE_CURRENT_RUNTIME=true /p:PACKAGE_PUBLISH_MODE=stable $properties "$solution"
  }

  if ($LastExitCode -ne 0) {
    throw "'Pack' failed for '$solution'"
  }
}

function Restore() {
  $logFile = Join-Path -Path $LogDir -ChildPath "$configuration\restore.binlog"
  & dotnet restore -v "$verbosity" /bl:"$logFile" /err $properties "$solution"

  if ($LastExitCode -ne 0) {
    throw "'Restore' failed for '$solution'"
  }
}

function Get-LlvmVersion() {
  if ($llvm -ne "") {
    return $llvm
  }

  $cmakeLists = Join-Path -Path $RepoRoot -ChildPath "CMakeLists.txt"
  $match = Select-String -Path $cmakeLists -Pattern 'project\(ClangSharp VERSION ([0-9.]+)\)' | Select-Object -First 1

  if (-not $match) {
    throw "Could not parse the LLVM version from '$cmakeLists'"
  }

  return $match.Matches[0].Groups[1].Value
}

function Download-Llvm([string] $version, [string] $runtime, [string] $destination) {
  $asset = switch ($runtime) {
    "win-x64"     { "clang+llvm-$version-x86_64-pc-windows-msvc.tar.xz" }
    "win-arm64"   { "clang+llvm-$version-aarch64-pc-windows-msvc.tar.xz" }
    "linux-x64"   { "LLVM-$version-Linux-X64.tar.xz" }
    "linux-arm64" { "LLVM-$version-Linux-ARM64.tar.xz" }
    "osx-arm64"   { "LLVM-$version-macOS-ARM64.tar.xz" }
    default       { throw "Unsupported runtime identifier '$runtime'" }
  }

  $url = "https://github.com/llvm/llvm-project/releases/download/llvmorg-$version/$asset"
  $archive = Join-Path -Path $ArtifactsDir -ChildPath "llvm-$runtime.tar.xz"

  Create-Directory -Path $destination

  # Windows ships bsdtar (libarchive) as tar.exe, which handles .tar.xz.
  & curl.exe -fSL $url -o $archive

  if ($LastExitCode -ne 0) {
    throw "'curl' failed to download '$url'"
  }

  & tar -xf $archive -C $destination --strip-components=1

  if ($LastExitCode -ne 0) {
    throw "'tar' failed to extract '$archive'"
  }
}

function Extract-Libclang([string] $runtime, [string] $source, [string] $destination) {
  $pattern = switch -Wildcard ($runtime) {
    "win-*"   { "libclang.dll" }
    "linux-*" { "libclang.so*" }
    "osx-*"   { "libclang.dylib" }
  }

  $name = switch -Wildcard ($runtime) {
    "win-*"   { "libclang.dll" }
    "linux-*" { "libclang.so" }
    "osx-*"   { "libclang.dylib" }
  }

  # The Linux release ships a versioned real .so alongside symlinks, so pick the real
  # file (largest) to get actual contents, matching the 'find -type f' in build.sh.
  $lib = Get-ChildItem -Recurse -Path $source -Filter $pattern -File | Sort-Object -Property "Length" -Descending | Select-Object -First 1

  if (-not $lib) {
    throw "'$name' was not found in the LLVM release for '$runtime'"
  }

  Copy-Item -Path $lib.FullName -Destination (Join-Path -Path $destination -ChildPath $name)
}

function Build-Libclangsharp([string] $runtime, [string] $source, [string] $destination) {
  $arch = switch ($runtime) {
    "win-x64"   { "x64" }
    "win-arm64" { "ARM64" }
    default     { throw "'$runtime' cannot build libClangSharp on Windows; use build.sh on the matching runner" }
  }

  $nativeBuildDir = Join-Path -Path $ArtifactsDir -ChildPath "bin\native\$runtime"
  $pathToLlvm = (Resolve-Path -Path $source).Path

  & cmake -B "$nativeBuildDir" -S "$RepoRoot" -A "$arch" "-Thost=$arch" "-DPATH_TO_LLVM=$pathToLlvm"

  if ($LastExitCode -ne 0) {
    throw "'cmake' configure failed for '$runtime'"
  }

  & cmake --build "$nativeBuildDir" --config Release --target ClangSharp

  if ($LastExitCode -ne 0) {
    throw "'cmake' build failed for '$runtime'"
  }

  $lib = Get-ChildItem -Recurse -Path $nativeBuildDir -Filter "libClangSharp.dll" -File | Select-Object -First 1

  if (-not $lib) {
    throw "'libClangSharp.dll' was not produced for '$runtime'"
  }

  Copy-Item -Path $lib.FullName -Destination (Join-Path -Path $destination -ChildPath "libClangSharp.dll")
}

function Regenerate-Native() {
  if ($rid -eq "") {
    throw "-rid is required with -regeneratenative"
  }

  if ($target -eq "") {
    throw "-target is required with -regeneratenative"
  }

  $version = Get-LlvmVersion
  $llvmDir = Join-Path -Path $ArtifactsDir -ChildPath "llvm\$rid"
  $stagingDir = Join-Path -Path $ArtifactsDir -ChildPath "native\$rid"

  Download-Llvm -version "$version" -runtime "$rid" -destination "$llvmDir"
  Create-Directory -Path $stagingDir

  if ($target -eq "libclang") {
    Extract-Libclang -runtime "$rid" -source "$llvmDir" -destination "$stagingDir"
  }
  elseif ($target -eq "libClangSharp") {
    Build-Libclangsharp -runtime "$rid" -source "$llvmDir" -destination "$stagingDir"
  }
  else {
    throw "Unsupported target '$target'"
  }
}

function Verify-Packages() {
  $version = Get-LlvmVersion
  $failed = $false

  # libclang packages track the LLVM version exactly, including the repository branch.
  $libclangNuspecs = @(Join-Path -Path $RepoRoot -ChildPath "packages\libclang\libclang\libclang.nuspec")
  $libclangNuspecs += Get-ChildItem -Path (Join-Path -Path $RepoRoot -ChildPath "packages\libclang") -Filter "*.nuspec" -Recurse | Where-Object { $_.Name -ne "libclang.nuspec" } | ForEach-Object { $_.FullName }

  foreach ($nuspec in $libclangNuspecs) {
    $content = Get-Content -Path $nuspec -Raw

    if ($content -match "<version>([^<]*)</version>") {
      if ($Matches[1] -ne $version) {
        Write-Host -Object "${nuspec}: version '$($Matches[1])' does not match LLVM version '$version'"
        $failed = $true
      }
    }

    if ($content -match 'branch="([^"]*)"') {
      if ($Matches[1] -ne "llvmorg-$version") {
        Write-Host -Object "${nuspec}: repository branch '$($Matches[1])' does not match 'llvmorg-$version'"
        $failed = $true
      }
    }
  }

  $libclangRuntimeJson = Join-Path -Path $RepoRoot -ChildPath "packages\libclang\libclang\runtime.json"

  foreach ($match in ([regex]'"(\d+\.\d+\.\d+(\.\d+)?)"').Matches((Get-Content -Path $libclangRuntimeJson -Raw))) {
    if ($match.Groups[1].Value -ne $version) {
      Write-Host -Object "packages\libclang\libclang\runtime.json: mapped version '$($match.Groups[1].Value)' does not match LLVM version '$version'"
      $failed = $true
    }
  }

  # libClangSharp packages are the LLVM version plus an independent build revision.
  $libclangsharpNuspecs = @(Join-Path -Path $RepoRoot -ChildPath "packages\libClangSharp\libClangSharp\libClangSharp.nuspec")
  $libclangsharpNuspecs += Get-ChildItem -Path (Join-Path -Path $RepoRoot -ChildPath "packages\libClangSharp") -Filter "*.nuspec" -Recurse | Where-Object { $_.Name -ne "libClangSharp.nuspec" } | ForEach-Object { $_.FullName }

  foreach ($nuspec in $libclangsharpNuspecs) {
    $content = Get-Content -Path $nuspec -Raw

    if ($content -match "<version>([^<]*)</version>") {
      if (-not $Matches[1].StartsWith("$version.")) {
        Write-Host -Object "${nuspec}: version '$($Matches[1])' is not 'llvm-version.<revision>' (expected '$version.<n>')"
        $failed = $true
      }
    }
  }

  $libclangsharpRuntimeJson = Join-Path -Path $RepoRoot -ChildPath "packages\libClangSharp\libClangSharp\runtime.json"

  foreach ($match in ([regex]'"(\d+\.\d+\.\d+(\.\d+)?)"').Matches((Get-Content -Path $libclangsharpRuntimeJson -Raw))) {
    if (-not $match.Groups[1].Value.StartsWith("$version.")) {
      Write-Host -Object "packages\libClangSharp\libClangSharp\runtime.json: mapped version '$($match.Groups[1].Value)' is not '$version.<revision>'"
      $failed = $true
    }
  }

  # The native version string reported by clangsharp_getVersion must track the LLVM version exactly.
  $clangSharpCpp = Join-Path -Path $RepoRoot -ChildPath "sources\libClangSharp\ClangSharp.cpp"

  if ((Get-Content -Path $clangSharpCpp -Raw) -match "clangsharp version ([0-9][0-9.]*)") {
    if ($Matches[1] -ne $version) {
      Write-Host -Object "sources\libClangSharp\ClangSharp.cpp: clangsharp_getVersion string '$($Matches[1])' does not match LLVM version '$version'"
      $failed = $true
    }
  }
  else {
    Write-Host -Object "sources\libClangSharp\ClangSharp.cpp: could not find the clangsharp_getVersion version string"
    $failed = $true
  }

  # The managed package pins consume the native packages. Native may be updated ahead of managed,
  # but managed must never lead native (managed <= native).
  $nativeLibClangSharp = $null

  if ((Get-Content -Path (Join-Path -Path $RepoRoot -ChildPath "packages\libClangSharp\libClangSharp\libClangSharp.nuspec") -Raw) -match "<version>([^<]*)</version>") {
    $nativeLibClangSharp = $Matches[1]
  }

  $packagesProps = Get-Content -Path (Join-Path -Path $RepoRoot -ChildPath "Directory.Packages.props") -Raw

  if ($packagesProps -match 'Include="libClang"\s+Version="([0-9.]+)"') {
    $managedLibClang = $Matches[1]

    if ([version]$managedLibClang -gt [version]$version) {
      Write-Host -Object "Directory.Packages.props: managed libClang pin '$managedLibClang' leads the native libclang version '$version' (managed must be <= native)"
      $failed = $true
    }
  }

  if (($null -ne $nativeLibClangSharp) -and ($packagesProps -match 'Include="libClangSharp"\s+Version="([0-9.]+)"')) {
    $managedLibClangSharp = $Matches[1]

    if ([version]$managedLibClangSharp -gt [version]$nativeLibClangSharp) {
      Write-Host -Object "Directory.Packages.props: managed libClangSharp pin '$managedLibClangSharp' leads the native libClangSharp version '$nativeLibClangSharp' (managed must be <= native)"
      $failed = $true
    }
  }

  if ($failed) {
    throw "Update the package versions to match the tracked LLVM version ($version) before regenerating."
  }

  Write-Host -Object "Package versions verified against LLVM $version"
}

function Detect-Changes() {
  if ($base -eq "") {
    throw "-base <ref> is required with -detectchanges"
  }

  $version = Get-LlvmVersion
  $detectLibclang = $false
  $detectLibclangsharp = $false

  & git -C "$RepoRoot" cat-file -e "$base^{commit}" 2>$null

  if ($LastExitCode -ne 0) {
    # No resolvable baseline to diff against, so regenerate everything conservatively.
    $detectLibclang = $true
    $detectLibclangsharp = $true
  }
  else {
    # libclang regenerates when the tracked LLVM major.minor changes.
    $previous = & git -C "$RepoRoot" show "${base}:CMakeLists.txt" 2>$null | Select-String -Pattern 'project\(ClangSharp VERSION ([0-9.]+)\)' | Select-Object -First 1

    $previousVersion = if ($previous) { $previous.Matches[0].Groups[1].Value } else { "" }

    $currentMajorMinor = ($version -split '\.')[0..1] -join '.'
    $previousMajorMinor = ($previousVersion -split '\.')[0..1] -join '.'

    if ($currentMajorMinor -ne $previousMajorMinor) {
      $detectLibclang = $true
    }

    # libClangSharp regenerates for that same reason or when its sources change.
    & git -C "$RepoRoot" diff --quiet "$base" HEAD -- sources/libClangSharp/

    if ($detectLibclang -or ($LastExitCode -ne 0)) {
      $detectLibclangsharp = $true
    }
  }

  Write-Output "libclang=$($detectLibclang.ToString().ToLowerInvariant())"
  Write-Output "libclangsharp=$($detectLibclangsharp.ToString().ToLowerInvariant())"
}

function Test() {
  $logFile = Join-Path -Path $LogDir -ChildPath "$configuration\test.binlog"
  & dotnet test -c "$configuration" --no-build --no-restore -v "$verbosity" /bl:"$logFile" /err $properties "$solution"

  if ($LastExitCode -ne 0) {
    throw "'Test' failed for '$solution'"
  }
}

function Test-Win32Metadata {
  $win32MetadataDir = Join-Path -Path $ArtifactsDir -ChildPath "win32metadata"

  if (!(Test-Path -Path "$win32MetadataDir")) {
    & git config --system core.longpaths true
    & git clone "https://github.com/microsoft/win32metadata" "$win32MetadataDir"
  }
  pushd "$win32MetadataDir"

  try {
    & git fetch --all
    & git reset --hard origin/master

    & git submodule add https://github.com/MicrosoftDocs/sdk-api ext/sdk-api
    & git submodule update --init --recursive

    $win32MetadataToolsDir = Join-Path -Path $win32MetadataDir -ChildPath "tools"

    $clangSharpPInvokeGeneratorOutputDir = Join-Path -Path $ArtifactsDir -ChildPath "bin\sources\ClangSharpPInvokeGenerator\$configuration\netcoreapp3.1"
    $env:PATH="$clangSharpPInvokeGeneratorOutputDir;$env:PATH"

    $generateMetadataSourcePs1 = Join-Path -Path $win32MetadataDir -ChildPath "scripts\GenerateMetadataSource.ps1"
    & "$generateMetadataSourcePs1"

    if ($LastExitCode -ne 0) {
      throw "'GenerateMetadataSource.ps1' failed"
    }

    $buildMetadataBinPs1 = Join-Path -Path $win32MetadataDir -ChildPath "scripts\BuildMetadataBin.ps1"
    & "$buildMetadataBinPs1"

    if ($LastExitCode -ne 0) {
      throw "'BuildMetadataBin.ps1' failed"
    }

    $testWinmdBinaryPs1 = Join-Path -Path $win32MetadataDir -ChildPath "scripts\TestWinmdBinary.ps1"
    & "$testWinmdBinaryPs1"

    if ($LastExitCode -ne 0) {
      throw "'TestWinmdBinary.ps1' failed"
    }
  }
  finally {
    popd
  }
}

try {
  if ($help) {
    Help
    exit 0
  }

  if ($ci) {
    $build = $true
    $pack = $true
    $restore = $true
    $test = $true

    if ($architecture -eq "") {
      $architecture = "<auto>"
    }
  }
  elseif (($architecture -ne "") -and ($architecture -ne "<auto>")) {
    $properties += "/p:PlatformTarget=$architecture"
  }

  $RepoRoot = Join-Path -Path $PSScriptRoot -ChildPath ".."

  if ($solution -eq "") {
    $solution = Join-Path -Path $RepoRoot -ChildPath "ClangSharp.slnx"
  }

  $ArtifactsDir = Join-Path -Path $RepoRoot -ChildPath "artifacts"
  Create-Directory -Path $ArtifactsDir

  $LogDir = Join-Path -Path $ArtifactsDir -ChildPath "log"
  Create-Directory -Path $LogDir

  if ($architecture -ne "") {
    $env:DOTNET_CLI_TELEMETRY_OPTOUT = 1
    $env:DOTNET_MULTILEVEL_LOOKUP = 0
    $env:DOTNET_SKIP_FIRST_TIME_EXPERIENCE = 1

    $DotNetInstallScript = Join-Path -Path $ArtifactsDir -ChildPath "dotnet-install.ps1"
    Invoke-WebRequest -Uri "https://dot.net/v1/dotnet-install.ps1" -OutFile $DotNetInstallScript -UseBasicParsing

    $DotNetInstallDirectory = Join-Path -Path $ArtifactsDir -ChildPath "dotnet"
    Create-Directory -Path $DotNetInstallDirectory

    & $DotNetInstallScript -Channel 8.0 -Version latest -InstallDir $DotNetInstallDirectory -Architecture $architecture
    & $DotNetInstallScript -Channel 10.0 -Version latest -InstallDir $DotNetInstallDirectory -Architecture $architecture

    $env:PATH="$DotNetInstallDirectory;$env:PATH"
  }

  if ($restore) {
    Restore
  }

  if ($build) {
    Build
  }

  if ($test) {
    Test
  }

  if ($testwin32metadata) {
    Test-Win32Metadata
  }

  if ($pack) {
    Pack
  }

  if ($regeneratenative) {
    Regenerate-Native
  }

  if ($verifypackages) {
    Verify-Packages
  }

  if ($detectchanges) {
    Detect-Changes
  }
}
catch {
  Write-Host -Object $_
  Write-Host -Object $_.Exception
  Write-Host -Object $_.ScriptStackTrace
  exit 1
}
