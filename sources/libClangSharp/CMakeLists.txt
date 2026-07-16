include(GenerateExportHeader)

set(CMAKE_CXX_VISIBILITY_PRESET "hidden")
set(CMAKE_VISIBILITY_INLINES_HIDDEN ON)
set(CMAKE_INCLUDE_CURRENT_DIR ON)

set(SOURCES
  CIndexDiagnostic.cpp
  ClangSharp.cpp
  CXCursor.cpp
  CXLoadedDiagnostic.cpp
  CXSourceLocation.cpp
  CXString.cpp
  CXTranslationUnit.cpp
  CXType.cpp

  CIndexDiagnostic.h
  ClangSharp.h
  CXCursor.h
  CXLoadedDiagnostic.h
  CXSourceLocation.h
  CXString.h
  CXTranslationUnit.h
  CXType.h
)

set(LLVM_SEARCH_PATHS
  ${PATH_TO_LLVM}
  ${PATH_TO_LLVM}/lib/cmake
  ${PATH_TO_LLVM}/lib/cmake/llvm
  ${PATH_TO_LLVM}/lib/cmake/clang
  ${PATH_TO_LLVM}/share/clang/cmake/
  ${PATH_TO_LLVM}/share/llvm/cmake/
)

find_package(Clang REQUIRED CONFIG
             PATHS ${LLVM_SEARCH_PATHS}
             NO_DEFAULT_PATH)

# The official LLVM Windows release bakes an absolute path to the DIA SDK's
# diaguids.lib -- taken from the Visual Studio on LLVM's own build machine --
# into the imported LLVMDebugInfoPDB target in LLVMExports.cmake. That path
# won't exist on another machine (or a CI runner) with a different Visual
# Studio, causing LNK1181. Repoint it at the DIA SDK of the Visual Studio
# actually being used for this build. The Visual Studio generator (win-x64)
# exposes its root and target as CMAKE_GENERATOR_INSTANCE/PLATFORM; Ninja +
# clang-cl (win-arm64) gets them from VSINSTALLDIR and the compiler arch id.
if(MSVC AND TARGET LLVMDebugInfoPDB)
  if(CMAKE_GENERATOR_INSTANCE)
    set(_clangsharp_vs "${CMAKE_GENERATOR_INSTANCE}")
  else()
    file(TO_CMAKE_PATH "$ENV{VSINSTALLDIR}" _clangsharp_vs)
  endif()
  string(REGEX REPLACE "/+$" "" _clangsharp_vs "${_clangsharp_vs}")

  if(CMAKE_GENERATOR_PLATFORM)
    set(_clangsharp_arch "${CMAKE_GENERATOR_PLATFORM}")
  else()
    set(_clangsharp_arch "${CMAKE_CXX_COMPILER_ARCHITECTURE_ID}")
  endif()

  if(_clangsharp_vs)
    if(_clangsharp_arch MATCHES "[Aa][Rr][Mm]64")
      set(_clangsharp_diaguids "${_clangsharp_vs}/DIA SDK/lib/arm64/diaguids.lib")
    elseif(_clangsharp_arch MATCHES "[Ww]in32|[Xx]86")
      set(_clangsharp_diaguids "${_clangsharp_vs}/DIA SDK/lib/diaguids.lib")
    else()
      set(_clangsharp_diaguids "${_clangsharp_vs}/DIA SDK/lib/amd64/diaguids.lib")
    endif()

    get_target_property(_clangsharp_pdb_libs LLVMDebugInfoPDB INTERFACE_LINK_LIBRARIES)
    if(EXISTS "${_clangsharp_diaguids}")
      string(REGEX REPLACE "[^;]*[Dd][Ii][Aa] SDK[^;]*diaguids\\.lib" "${_clangsharp_diaguids}" _clangsharp_pdb_libs "${_clangsharp_pdb_libs}")
      set_target_properties(LLVMDebugInfoPDB PROPERTIES INTERFACE_LINK_LIBRARIES "${_clangsharp_pdb_libs}")
    endif()
  endif()
endif()

# The official LLVM release links zstd, but the distro's static libzstd.a is not
# built as position-independent code, so it can't be linked into our shared
# library (ld: "recompile with -fPIC"). LLVMConfig re-runs find_package(zstd) at
# our configure time and prefers the static archive; repoint the imported target
# at the shared libzstd (which is PIC) instead. Historically ClangSharp built
# LLVM from source with -DLLVM_ENABLE_ZSTD=OFF and never hit this.
if(UNIX AND TARGET zstd::libzstd_static)
  find_library(CLANGSHARP_ZSTD_SHARED NAMES zstd libzstd)
  if(CLANGSHARP_ZSTD_SHARED)
    set_target_properties(zstd::libzstd_static PROPERTIES IMPORTED_LOCATION "${CLANGSHARP_ZSTD_SHARED}")
  endif()
endif()

add_library(ClangSharp SHARED ${SOURCES})

target_link_libraries(ClangSharp PRIVATE clangAST clangFrontend libclang)

# The official LLVM macOS release ships LTO bitcode in its static archives, so
# linking them requires a libLTO that understands LLVM 22 bitcode. Apple's system
# linker defaults to Xcode's older libLTO and fails ("could not parse bitcode
# object file ... Unknown attribute kind"). Point the linker at the libLTO.dylib
# from the LLVM release being consumed.
if(APPLE AND EXISTS "${PATH_TO_LLVM}/lib/libLTO.dylib")
  target_link_options(ClangSharp PRIVATE "LINKER:-lto_library,${PATH_TO_LLVM}/lib/libLTO.dylib")
endif()

target_include_directories(ClangSharp PRIVATE ${CLANG_INCLUDE_DIRS})

set_target_properties(ClangSharp PROPERTIES
    PREFIX lib
    VERSION ${PROJECT_VERSION}
    SOVERSION ${PROJECT_VERSION}
)

generate_export_header(ClangSharp
    EXPORT_MACRO_NAME "CLANGSHARP_LINKAGE"
    EXPORT_FILE_NAME ClangSharp_export.h
)

set_property(TARGET ClangSharp APPEND PROPERTY
    PUBLIC_HEADER ClangSharp.h "${CMAKE_CURRENT_BINARY_DIR}/ClangSharp_export.h"
)

include(GNUInstallDirs)

install(TARGETS ClangSharp
    LIBRARY DESTINATION ${CMAKE_INSTALL_LIBDIR} COMPONENT runtime
    ARCHIVE DESTINATION ${CMAKE_INSTALL_LIBDIR} COMPONENT development
    RUNTIME DESTINATION ${CMAKE_INSTALL_BINDIR} COMPONENT runtime
    PUBLIC_HEADER DESTINATION ${CMAKE_INSTALL_INCLUDEDIR} COMPONENT development
)
