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

if (TARGET clang-cpp) # Linux packages install libclang and clang-cpp
    target_link_libraries(ClangSharp PRIVATE clang-cpp libclang)
else()
    target_link_libraries(ClangSharp PRIVATE clangAST clangFrontend libclang)
endif()

target_include_directories(ClangSharp PRIVATE ${CLANG_INCLUDE_DIRS})
set_target_properties(ClangSharp PROPERTIES
    PREFIX lib
    VERSION ${PROJECT_VERSION}
    SOVERSION ${PROJECT_VERSION}
)

include(GNUInstallDirs)

install(TARGETS ClangSharp
    LIBRARY DESTINATION ${CMAKE_INSTALL_LIBDIR} COMPONENT runtime
    ARCHIVE DESTINATION ${CMAKE_INSTALL_LIBDIR} COMPONENT development
    RUNTIME DESTINATION ${CMAKE_INSTALL_BINDIR} COMPONENT runtime
)
