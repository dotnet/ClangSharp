using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class MemberExpr : Expr
    {
        public MemberExpr(CXCursor handle, Cursor parent) : base(handle, parent)
        {
            Debug.Assert(handle.Kind == CXCursorKind.CXCursor_MemberRefExpr);
        }

        public CXSourceRange GetReferenceNameRange(CXNameRefFlags nameFlags, uint pieceIndex) => Handle.GetReferenceNameRange(nameFlags, pieceIndex);
    }
}
