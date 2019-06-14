using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class EnumConstantDecl : ValueDecl
    {
        private Expr _initExpr;

        public EnumConstantDecl(CXCursor handle, Cursor parent) : base(handle, parent)
        {
            Debug.Assert(handle.Kind == CXCursorKind.CXCursor_EnumConstantDecl);
        }

        public Expr InitExpr => _initExpr;

        public long InitVal => Handle.EnumConstantDeclValue;

        public ulong UnsignedInitVal => Handle.EnumConstantDeclUnsignedValue;

        protected override Expr GetOrAddExpr(CXCursor childHandle)
        {
            var expr = base.GetOrAddExpr(childHandle);

            Debug.Assert(_initExpr is null);
            _initExpr = expr;

            return expr;
        }
    }
}
