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

add_library(ClangSharp SHARED ${SOURCES})

target_link_libraries(ClangSharp PRIVATE clangAST clangFrontend libclang)

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
