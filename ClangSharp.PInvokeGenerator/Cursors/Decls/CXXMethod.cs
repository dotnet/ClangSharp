using System.Diagnostics;

namespace ClangSharp
{
    internal sealed class CXXMethod : Decl
    {
        public CXXMethod(CXCursor handle, Cursor parent) : base(handle, parent)
        {
            Debug.Assert(handle.Kind == CXCursorKind.CXCursor_CXXMethod);
        }

        public bool IsConst => Handle.CXXMethod_IsConst;

        public bool IsDefaulted => Handle.CXXMethod_IsDefaulted;

        public bool IsPureVirtual => Handle.CXXMethod_IsPureVirtual;

        public bool IsStatic => Handle.CXXMethod_IsStatic;

        public bool IsVirtual => Handle.CXXMethod_IsVirtual;
    }
}
