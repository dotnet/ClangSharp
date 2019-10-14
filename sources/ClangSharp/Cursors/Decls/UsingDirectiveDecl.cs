using ClangSharp.Interop;

namespace ClangSharp
{
    // e.g. using namespace std;
    // See: https://clang.llvm.org/doxygen/classclang_1_1UsingDirectiveDecl.html
    public class UsingDirectiveDecl: NamedDecl, IMergeable<UsingDecl>
    {
        internal UsingDirectiveDecl(CXCursor handle) : base(handle, CXCursorKind.CXCursor_UsingDirective)
        {
        }
    }
}
