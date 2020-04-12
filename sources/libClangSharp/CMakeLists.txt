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

set(LIBS
  clangAST
  clangFrontend
  libclang
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

if(WIN32)
  add_library(libClangSharp SHARED ${SOURCES})
  target_include_directories(libClangSharp PUBLIC ${LLVM_INCLUDE_DIRS})
  target_link_libraries(libClangSharp ${LIBS})
else()
  add_library(ClangSharp SHARED ${SOURCES})
  target_include_directories(ClangSharp PUBLIC ${LLVM_INCLUDE_DIRS})
  target_link_libraries(ClangSharp ${LIBS})
endif()
