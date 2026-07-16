#!/bin/bash

SOURCE="${BASH_SOURCE[0]}"
while [ -h "$SOURCE" ]; do # resolve $SOURCE until the file is no longer a symlink
  ScriptRoot="$( cd -P "$( dirname "$SOURCE" )" && pwd )"
  SOURCE="$(readlink "$SOURCE")"
  [[ $SOURCE != /* ]] && SOURCE="$ScriptRoot/$SOURCE" # if $SOURCE was a relative symlink, we need to resolve it relative to the path where the symlink file was located
done
ScriptRoot="$( cd -P "$( dirname "$SOURCE" )" && pwd )"

architecture=''
base=''
build=false
ci=false
configuration='Debug'
detectchanges=false
help=false
llvm=''
pack=false
regeneratenative=false
restore=false
rid=''
solution=''
target=''
test=false
verbosity='minimal'
verifypackages=false
properties=''

while [[ $# -gt 0 ]]; do
  lower="$(echo "$1" | awk '{print tolower($0)}')"
  case $lower in
    --architecture)
      architecture=$2
      shift 2
      ;;
    --base)
      base=$2
      shift 2
      ;;
    --build)
      build=true
      shift 1
      ;;
    --ci)
      ci=true
      shift 1
      ;;
    --configuration)
      configuration=$2
      shift 2
      ;;
    --detectchanges)
      detectchanges=true
      shift 1
      ;;
    --help)
      help=true
      shift 1
      ;;
    --llvm)
      llvm=$2
      shift 2
      ;;
    --pack)
      pack=true
      shift 1
      ;;
    --regeneratenative)
      regeneratenative=true
      shift 1
      ;;
    --restore)
      restore=true
      shift 1
      ;;
    --rid)
      rid=$2
      shift 2
      ;;
    --solution)
      solution=$2
      shift 2
      ;;
    --target)
      target=$2
      shift 2
      ;;
    --test)
      test=true
      shift 1
      ;;
    --verbosity)
      verbosity=$2
      shift 2
      ;;
    --verifypackages)
      verifypackages=true
      shift 1
      ;;
    *)
      properties="$properties $1"
      shift 1
      ;;
  esac
done

function Build {
  logFile="$LogDir/$configuration/build.binlog"

  if [[ -z "$properties" ]]; then
    dotnet build -c "$configuration" --no-restore -v "$verbosity" /bl:"$logFile" /err "$solution"
  else
    dotnet build -c "$configuration" --no-restore -v "$verbosity" /bl:"$logFile" /err "${properties[@]}" "$solution"
  fi

  LASTEXITCODE=$?

  if [ "$LASTEXITCODE" != 0 ]; then
    echo "'Build' failed for '$solution'"
    return "$LASTEXITCODE"
  fi
}

function CreateDirectory {
  if [ ! -d "$1" ]
  then
    mkdir -p "$1"
  fi
}

function Help {
  echo "Common settings:"
  echo "  --configuration <value>   Build configuration (Debug, Release)"
  echo "  --verbosity <value>       Msbuild verbosity (q[uiet], m[inimal], n[ormal], d[etailed], and diag[nostic])"
  echo "  --help                    Print help and exit"
  echo ""
  echo "Actions:"
  echo "  --restore                 Restore dependencies"
  echo "  --build                   Build solution"
  echo "  --test                    Run all tests in the solution"
  echo "  --pack                    Package build artifacts"
  echo "  --regeneratenative        Download the matching LLVM release and stage a native binary"
  echo "                            (use with --target and --rid)"
  echo "  --verifypackages          Verify the libclang/libClangSharp package versions match CMakeLists.txt"
  echo "  --detectchanges           Print which native packages need regenerating since --base <ref>"
  echo ""
  echo "Advanced settings:"
  echo "  --solution <value>        Path to solution to build"
  echo "  --ci                      Set when running on CI server"
  echo "  --architecture <value>    Test Architecture (<auto>, amd64, x64, x86, arm64, arm)"
  echo ""
  echo "Command line arguments not listed above are passed through to MSBuild."
}

function Pack {
  logFile="$LogDir/$configuration/pack"

  if [[ -z "$properties" ]]; then
    dotnet pack -c "$configuration" --no-build --no-restore -v "$verbosity" /bl:"$logFile.binlog" /err "$solution"
    dotnet pack -c "$configuration" --no-build --no-restore -v "$verbosity" /bl:"$logFile.agnostic.binlog" /err /p:SKIP_USE_CURRENT_RUNTIME=true "$solution"
  else
    dotnet pack -c "$configuration" --no-build --no-restore -v "$verbosity" /bl:"$logFile.binlog" /err "${properties[@]}" "$solution"
    dotnet pack -c "$configuration" --no-build --no-restore -v "$verbosity" /bl:"$logFile.agnostic.binlog" /err /p:SKIP_USE_CURRENT_RUNTIME=true "${properties[@]}" "$solution"
  fi

if $ci; then
  if [[ -z "$properties" ]]; then
    dotnet pack -c "$configuration" --no-build --no-restore -v "$verbosity" /bl:"$logFile.preview.binlog" /err /p:PACKAGE_PUBLISH_MODE=preview "$solution"
    dotnet pack -c "$configuration" --no-build --no-restore -v "$verbosity" /bl:"$logFile.stable.binlog" /err /p:PACKAGE_PUBLISH_MODE=stable "$solution"

    dotnet pack -c "$configuration" --no-build --no-restore -v "$verbosity" /bl:"$logFile.agnostic.preview.binlog" /err /p:SKIP_USE_CURRENT_RUNTIME=true /p:PACKAGE_PUBLISH_MODE=preview "$solution"
    dotnet pack -c "$configuration" --no-build --no-restore -v "$verbosity" /bl:"$logFile.agnostic.stable.binlog" /err /p:SKIP_USE_CURRENT_RUNTIME=true /p:PACKAGE_PUBLISH_MODE=stable "$solution"
  else
    dotnet pack -c "$configuration" --no-build --no-restore -v "$verbosity" /bl:"$logFile.preview.binlog" /err /p:PACKAGE_PUBLISH_MODE=preview "${properties[@]}" "$solution"
    dotnet pack -c "$configuration" --no-build --no-restore -v "$verbosity" /bl:"$logFile.stable.binlog" /err /p:PACKAGE_PUBLISH_MODE=stable "${properties[@]}" "$solution"

    dotnet pack -c "$configuration" --no-build --no-restore -v "$verbosity" /bl:"$logFile.agnostic.preview.binlog" /err /p:SKIP_USE_CURRENT_RUNTIME=true /p:PACKAGE_PUBLISH_MODE=preview "${properties[@]}" "$solution"
    dotnet pack -c "$configuration" --no-build --no-restore -v "$verbosity" /bl:"$logFile.agnostic.stable.binlog" /err /p:SKIP_USE_CURRENT_RUNTIME=true /p:PACKAGE_PUBLISH_MODE=stable "${properties[@]}" "$solution"
  fi
fi

  LASTEXITCODE=$?

  if [ "$LASTEXITCODE" != 0 ]; then
    echo "'Build' failed for '$solution'"
    return "$LASTEXITCODE"
  fi
}

function Restore {
  logFile="$LogDir/$configuration/restore.binlog"

  if [[ -z "$properties" ]]; then
    dotnet restore -v "$verbosity" /bl:"$logFile" /err "$solution"
  else
    dotnet restore -v "$verbosity" /bl:"$logFile" /err "${properties[@]}" "$solution"
  fi

  LASTEXITCODE=$?

  if [ "$LASTEXITCODE" != 0 ]; then
    echo "'Restore' failed for '$solution'"
    return "$LASTEXITCODE"
  fi
}

function Test {
  logFile="$LogDir/$configuration/test.binlog"

  if [[ -z "$properties" ]]; then
    dotnet test -c "$configuration" --no-build --no-restore -v "$verbosity" /bl:"$logFile" /err "$solution"
  else
    dotnet test -c "$configuration" --no-build --no-restore -v "$verbosity" /bl:"$logFile" /err "${properties[@]}" "$solution"
  fi

  LASTEXITCODE=$?

  if [ "$LASTEXITCODE" != 0 ]; then
    echo "'Test' failed for '$solution'"
    return "$LASTEXITCODE"
  fi
}

function GetLlvmVersion {
  if [[ -n "$llvm" ]]; then
    LASTEXITCODE=0
    return
  fi

  llvm="$(sed -n 's/^project(ClangSharp VERSION \([0-9.]*\)).*/\1/p' "$RepoRoot/CMakeLists.txt")"

  if [[ -z "$llvm" ]]; then
    echo "Could not parse the LLVM version from '$RepoRoot/CMakeLists.txt'"
    LASTEXITCODE=1
    return "$LASTEXITCODE"
  fi

  LASTEXITCODE=0
}

function DownloadLlvm {
  runtime="$1"
  destination="$2"

  case "$runtime" in
    win-x64)     asset="clang+llvm-${llvm}-x86_64-pc-windows-msvc.tar.xz" ;;
    win-arm64)   asset="clang+llvm-${llvm}-aarch64-pc-windows-msvc.tar.xz" ;;
    linux-x64)   asset="LLVM-${llvm}-Linux-X64.tar.xz" ;;
    linux-arm64) asset="LLVM-${llvm}-Linux-ARM64.tar.xz" ;;
    osx-arm64)   asset="LLVM-${llvm}-macOS-ARM64.tar.xz" ;;
    *)
      echo "Unsupported runtime identifier '$runtime'"
      LASTEXITCODE=1
      return "$LASTEXITCODE"
      ;;
  esac

  url="https://github.com/llvm/llvm-project/releases/download/llvmorg-${llvm}/${asset}"
  archive="$ArtifactsDir/llvm-${runtime}.tar.xz"

  CreateDirectory "$destination"

  curl -fSL "$url" -o "$archive"
  LASTEXITCODE=$?

  if [ "$LASTEXITCODE" != 0 ]; then
    echo "'curl' failed to download '$url'"
    return "$LASTEXITCODE"
  fi

  tar -xf "$archive" -C "$destination" --strip-components=1
  LASTEXITCODE=$?

  if [ "$LASTEXITCODE" != 0 ]; then
    echo "'tar' failed to extract '$archive'"
    return "$LASTEXITCODE"
  fi
}

function ExtractLibclang {
  runtime="$1"
  source="$2"
  destination="$3"

  # The Linux release ships a versioned real .so alongside symlinks, so restrict the
  # match to real files (-type f) to get actual contents.
  case "$runtime" in
    win-*)   name='libclang.dll';   src="$(find "$source" -name 'libclang.dll' -type f | head -n1)" ;;
    linux-*) name='libclang.so';    src="$(find "$source" -name 'libclang.so*' -type f | head -n1)" ;;
    osx-*)   name='libclang.dylib'; src="$(find "$source" -name 'libclang.dylib' -type f | head -n1)" ;;
  esac

  if [[ -z "$src" ]]; then
    echo "'$name' was not found in the LLVM release for '$runtime'"
    LASTEXITCODE=1
    return "$LASTEXITCODE"
  fi

  cp "$src" "$destination/$name"
  LASTEXITCODE=$?
}

function BuildLibclangsharp {
  runtime="$1"
  source="$2"
  destination="$3"

  case "$runtime" in
    linux-*|osx-*)
      ;;
    *)
      echo "'$runtime' cannot build libClangSharp here; use build.ps1 on the matching runner"
      LASTEXITCODE=1
      return "$LASTEXITCODE"
      ;;
  esac

  nativeBuildDir="$ArtifactsDir/bin/native/$runtime"

  cmake -B "$nativeBuildDir" -S "$RepoRoot" -DCMAKE_BUILD_TYPE=Release -DPATH_TO_LLVM="$source"
  LASTEXITCODE=$?

  if [ "$LASTEXITCODE" != 0 ]; then
    echo "'cmake' configure failed for '$runtime'"
    return "$LASTEXITCODE"
  fi

  cmake --build "$nativeBuildDir" --target ClangSharp
  LASTEXITCODE=$?

  if [ "$LASTEXITCODE" != 0 ]; then
    echo "'cmake' build failed for '$runtime'"
    return "$LASTEXITCODE"
  fi

  case "$runtime" in
    osx-*) name='libClangSharp.dylib' ;;
    *)     name='libClangSharp.so' ;;
  esac

  # CMake's VERSION/SOVERSION emit a versioned library plus an unversioned symlink, so
  # copy the dereferenced contents to get a real file.
  src="$(find "$nativeBuildDir" -name "$name" | head -n1)"

  if [[ -z "$src" ]]; then
    echo "'$name' was not produced for '$runtime'"
    LASTEXITCODE=1
    return "$LASTEXITCODE"
  fi

  cp -L "$src" "$destination/$name"
  LASTEXITCODE=$?
}

function RegenerateNative {
  if [[ -z "$rid" ]]; then
    echo "--rid is required with --regeneratenative"
    LASTEXITCODE=1
    return "$LASTEXITCODE"
  fi

  if [[ -z "$target" ]]; then
    echo "--target is required with --regeneratenative"
    LASTEXITCODE=1
    return "$LASTEXITCODE"
  fi

  GetLlvmVersion

  if [ "$LASTEXITCODE" != 0 ]; then
    return "$LASTEXITCODE"
  fi

  llvmDir="$ArtifactsDir/llvm/$rid"
  stagingDir="$ArtifactsDir/native/$rid"

  DownloadLlvm "$rid" "$llvmDir"

  if [ "$LASTEXITCODE" != 0 ]; then
    return "$LASTEXITCODE"
  fi

  CreateDirectory "$stagingDir"

  if [[ "$target" == "libclang" ]]; then
    ExtractLibclang "$rid" "$llvmDir" "$stagingDir"
  elif [[ "$target" == "libClangSharp" ]]; then
    BuildLibclangsharp "$rid" "$llvmDir" "$stagingDir"
  else
    echo "Unsupported target '$target'"
    LASTEXITCODE=1
  fi

  if [ "$LASTEXITCODE" != 0 ]; then
    return "$LASTEXITCODE"
  fi
}

function VerifyPackages {
  GetLlvmVersion

  if [ "$LASTEXITCODE" != 0 ]; then
    return "$LASTEXITCODE"
  fi

  rc=0

  # libclang packages track the LLVM version exactly, including the repository branch.
  for f in "$RepoRoot"/packages/libclang/libclang/libclang.nuspec "$RepoRoot"/packages/libclang/libclang.runtime.*/*.nuspec; do
    v="$(sed -n 's:.*<version>\([^<]*\)</version>.*:\1:p' "$f" | head -n1)"

    if [ "$v" != "$llvm" ]; then
      echo "$f: version '$v' does not match LLVM version '$llvm'"
      rc=1
    fi

    b="$(sed -n 's:.*<repository[^>]*branch="\([^"]*\)".*:\1:p' "$f" | head -n1)"

    if [ -n "$b" ] && [ "$b" != "llvmorg-$llvm" ]; then
      echo "$f: repository branch '$b' does not match 'llvmorg-$llvm'"
      rc=1
    fi
  done

  for v in $(grep -oE '"[0-9]+\.[0-9]+\.[0-9]+(\.[0-9]+)?"' "$RepoRoot/packages/libclang/libclang/runtime.json" | tr -d '"'); do
    if [ "$v" != "$llvm" ]; then
      echo "packages/libclang/libclang/runtime.json: mapped version '$v' does not match LLVM version '$llvm'"
      rc=1
    fi
  done

  # libClangSharp packages are the LLVM version plus an independent build revision.
  for f in "$RepoRoot"/packages/libClangSharp/libClangSharp/libClangSharp.nuspec "$RepoRoot"/packages/libClangSharp/libClangSharp.runtime.*/*.nuspec; do
    v="$(sed -n 's:.*<version>\([^<]*\)</version>.*:\1:p' "$f" | head -n1)"

    case "$v" in
      "$llvm".*) : ;;
      *)
        echo "$f: version '$v' is not 'llvm-version.<revision>' (expected '$llvm.<n>')"
        rc=1
        ;;
    esac
  done

  for v in $(grep -oE '"[0-9]+\.[0-9]+\.[0-9]+(\.[0-9]+)?"' "$RepoRoot/packages/libClangSharp/libClangSharp/runtime.json" | tr -d '"'); do
    case "$v" in
      "$llvm".*) : ;;
      *)
        echo "packages/libClangSharp/libClangSharp/runtime.json: mapped version '$v' is not '$llvm.<revision>'"
        rc=1
        ;;
    esac
  done

  # The native version string reported by clangsharp_getVersion must track the LLVM version exactly.
  cppVersion="$(sed -n 's:.*clangsharp version \([0-9][0-9.]*\).*:\1:p' "$RepoRoot/sources/libClangSharp/ClangSharp.cpp" | head -n1)"

  if [ -z "$cppVersion" ]; then
    echo "sources/libClangSharp/ClangSharp.cpp: could not find the clangsharp_getVersion version string"
    rc=1
  elif [ "$cppVersion" != "$llvm" ]; then
    echo "sources/libClangSharp/ClangSharp.cpp: clangsharp_getVersion string '$cppVersion' does not match LLVM version '$llvm'"
    rc=1
  fi

  # The managed package pins consume the native packages. Native may be updated ahead of managed,
  # but managed must never lead native (managed <= native).
  nativeLibClangSharp="$(sed -n 's:.*<version>\([^<]*\)</version>.*:\1:p' "$RepoRoot/packages/libClangSharp/libClangSharp/libClangSharp.nuspec" | head -n1)"
  managedLibClang="$(sed -n 's:.*Include="libClang"[[:space:]]\{1,\}Version="\([0-9.]*\)".*:\1:p' "$RepoRoot/Directory.Packages.props" | head -n1)"
  managedLibClangSharp="$(sed -n 's:.*Include="libClangSharp"[[:space:]]\{1,\}Version="\([0-9.]*\)".*:\1:p' "$RepoRoot/Directory.Packages.props" | head -n1)"

  if [ -n "$managedLibClang" ] && [ "$(printf '%s\n%s\n' "$managedLibClang" "$llvm" | sort -V | head -n1)" != "$managedLibClang" ]; then
    echo "Directory.Packages.props: managed libClang pin '$managedLibClang' leads the native libclang version '$llvm' (managed must be <= native)"
    rc=1
  fi

  if [ -n "$managedLibClangSharp" ] && [ -n "$nativeLibClangSharp" ] && [ "$(printf '%s\n%s\n' "$managedLibClangSharp" "$nativeLibClangSharp" | sort -V | head -n1)" != "$managedLibClangSharp" ]; then
    echo "Directory.Packages.props: managed libClangSharp pin '$managedLibClangSharp' leads the native libClangSharp version '$nativeLibClangSharp' (managed must be <= native)"
    rc=1
  fi

  if [ "$rc" != 0 ]; then
    echo "Update the package versions to match the tracked LLVM version ($llvm) before regenerating."
    LASTEXITCODE=1
    return "$LASTEXITCODE"
  fi

  echo "Package versions verified against LLVM $llvm"
  LASTEXITCODE=0
}

function DetectChanges {
  if [[ -z "$base" ]]; then
    echo "--base <ref> is required with --detectchanges"
    LASTEXITCODE=1
    return "$LASTEXITCODE"
  fi

  GetLlvmVersion

  if [ "$LASTEXITCODE" != 0 ]; then
    return "$LASTEXITCODE"
  fi

  detectLibclang=false
  detectLibclangsharp=false

  if ! git -C "$RepoRoot" cat-file -e "$base^{commit}" 2>/dev/null; then
    # No resolvable baseline to diff against, so regenerate everything conservatively.
    detectLibclang=true
    detectLibclangsharp=true
  else
    # libclang regenerates when the tracked LLVM major.minor changes.
    prevVersion="$(git -C "$RepoRoot" show "$base:CMakeLists.txt" 2>/dev/null | sed -n 's/^project(ClangSharp VERSION \([0-9.]*\)).*/\1/p')"

    if [ "${llvm%.*}" != "${prevVersion%.*}" ]; then
      detectLibclang=true
    fi

    # libClangSharp regenerates for that same reason or when its sources change.
    if [ "$detectLibclang" = true ] || ! git -C "$RepoRoot" diff --quiet "$base" HEAD -- sources/libClangSharp/; then
      detectLibclangsharp=true
    fi
  fi

  echo "libclang=$detectLibclang"
  echo "libclangsharp=$detectLibclangsharp"
  LASTEXITCODE=0
}

if $help; then
  Help
  exit 0
fi

if $ci; then
  build=true
  pack=true
  restore=true
  test=true

  if [[ -z "$architecture" ]]; then
    architecture="<auto>"
  fi
fi

RepoRoot="$ScriptRoot/.."

if [[ -z "$solution" ]]; then
  solution="$RepoRoot/ClangSharp.slnx"
fi

ArtifactsDir="$RepoRoot/artifacts"
CreateDirectory "$ArtifactsDir"

LogDir="$ArtifactsDir/log"
CreateDirectory "$LogDir"

if [[ ! -z "$architecture" ]]; then
  export DOTNET_CLI_TELEMETRY_OPTOUT=1
  export DOTNET_MULTILEVEL_LOOKUP=0
  export DOTNET_SKIP_FIRST_TIME_EXPERIENCE=1

  DotNetInstallScript="$ArtifactsDir/dotnet-install.sh"
  wget -O "$DotNetInstallScript" "https://dot.net/v1/dotnet-install.sh"

  DotNetInstallDirectory="$ArtifactsDir/dotnet"
  CreateDirectory "$DotNetInstallDirectory"

  . "$DotNetInstallScript" --channel 8.0 --version latest --install-dir "$DotNetInstallDirectory" --architecture "$architecture"
  . "$DotNetInstallScript" --channel 10.0 --version latest --install-dir "$DotNetInstallDirectory" --architecture "$architecture"

  PATH="$DotNetInstallDirectory:$PATH:"
fi

if $restore; then
  Restore

  if [ "$LASTEXITCODE" != 0 ]; then
    return "$LASTEXITCODE"
  fi
fi

if $build; then
  Build

  if [ "$LASTEXITCODE" != 0 ]; then
    return "$LASTEXITCODE"
  fi
fi

if $test; then
  Test

  if [ "$LASTEXITCODE" != 0 ]; then
    return "$LASTEXITCODE"
  fi
fi

if $pack; then
  Pack

  if [ "$LASTEXITCODE" != 0 ]; then
    return "$LASTEXITCODE"
  fi
fi

# The native subcommands below are only ever executed directly (never sourced via
# cibuild.sh), so they exit with the failure code -- a top-level return would merely
# warn and let the script fall through to a later block, masking the failure.
if $regeneratenative; then
  RegenerateNative

  if [ "$LASTEXITCODE" != 0 ]; then
    exit "$LASTEXITCODE"
  fi
fi

if $verifypackages; then
  VerifyPackages

  if [ "$LASTEXITCODE" != 0 ]; then
    exit "$LASTEXITCODE"
  fi
fi

if $detectchanges; then
  DetectChanges

  if [ "$LASTEXITCODE" != 0 ]; then
    exit "$LASTEXITCODE"
  fi
fi
