using ClangSharp.Interop;

namespace ClangSharp
{
    public class CXXMethodDecl : FunctionDecl
    {
        internal CXXMethodDecl(CXCursor handle, CXCursorKind expectedKind) : base(handle, expectedKind)
        {
        }

        public bool IsConst => Handle.CXXMethod_IsConst;

        public bool IsDefaulted => Handle.CXXMethod_IsDefaulted;

        public bool IsStatic => Handle.CXXMethod_IsStatic;

        public bool IsVirtual => Handle.CXXMethod_IsVirtual;
    }
}
