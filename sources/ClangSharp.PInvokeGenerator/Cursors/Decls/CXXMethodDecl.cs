using ClangSharp.Interop;

namespace ClangSharp
{
    internal class CXXMethodDecl : FunctionDecl
    {
        public CXXMethodDecl(CXCursor handle, Cursor parent) : base(handle, parent)
        {
        }

        public bool IsConst => Handle.CXXMethod_IsConst;

        public bool IsDefaulted => Handle.CXXMethod_IsDefaulted;

        public bool IsPureVirtual => Handle.CXXMethod_IsPureVirtual;

        public bool IsStatic => Handle.CXXMethod_IsStatic;

        public bool IsVirtual => Handle.CXXMethod_IsVirtual;
    }
}
