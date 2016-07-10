@echo off
if [%1]==[] (
  echo **ERROR**: Clang Shared Library Location is required. A good value for this parameter is 'libclang' which will translate to 'libclang.dll' or 'libclang.so' in deployment relative directory
  EXIT /B
)
if [%2]==[] (
  echo **ERROR**: Clang Include Directory is required. This is the directory which contains "clang" and "clang-c" as subdirectories
  EXIT /B
)

csc /out:ClangSharpPInvokeGenerator.exe ClangSharpPInvokeGenerator\*.cs
ClangSharpPInvokeGenerator.exe --m clang --p clang_ --namespace ClangSharp --output Generated.cs --libraryPath %1 --include %2 --file %2/clang-c/Index.h --file %2/clang-c/CXString.h --file %2/clang-c/Documentation.h --file %2/clang-c/CXErrorCode.h --file %2/clang-c/BuildSystem.h --file %2/clang-c/CXCompilationDatabase.h 
csc /target:library /out:ClangSharp.dll Generated.cs Extensions.cs