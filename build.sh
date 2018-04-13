if [ -z "$1" ]; then
  echo **ERROR**: Clang Shared Library Location is required. A good value for this parameter is 'libclang' which will translate to 'libclang.dll' or 'libclang.so' in deployment relative directory
  exit 1
fi
if [ -z "$2" ]; then
  echo **ERROR**: Clang Include Directory is required. This is the directory which contains "clang" and "clang-c" as subdirectories
  exit 1
fi

dotnet run -p ClangSharpPInvokeGenerator \
	--m clang \
	--p clang_ \
	--namespace ClangSharp \
	--output ClangSharp/Generated.cs \
	--libraryPath $1 \
	--include $2 \
	--file $2/clang-c/Index.h \
	--file $2/clang-c/CXString.h \
	--file $2/clang-c/Documentation.h \
	--file $2/clang-c/CXErrorCode.h \
	--file $2/clang-c/BuildSystem.h \
	--file $2/clang-c/CXCompilationDatabase.h

dotnet build ClangSharp
