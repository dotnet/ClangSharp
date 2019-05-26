using System.Diagnostics;

namespace ClangSharp
{
    internal sealed class EnumConstantDecl : Decl
    {
        private Expr _expr;

        public EnumConstantDecl(CXCursor handle, Cursor parent) : base(handle, parent)
        {
            Debug.Assert(handle.Kind == CXCursorKind.CXCursor_EnumConstantDecl);
        }

        public Expr Expr
        {
            get
            {
                return _expr;
            }

            set
            {
                Debug.Assert(_expr is null);
                _expr = value;
            }
        }

        public ulong UnsignedValue => Handle.EnumConstantDeclUnsignedValue;

        public long Value => Handle.EnumConstantDeclValue;

        protected override CXChildVisitResult VisitChildren(CXCursor childHandle, CXCursor handle, CXClientData clientData)
        {
            ValidateVisit(ref handle);

            if (childHandle.IsExpression)
            {
                var expr = GetOrAddChild<UnexposedExpr>(childHandle);
                Expr = expr;
                return expr.Visit(clientData);
            }

            return base.VisitChildren(childHandle, handle, clientData);
        }
    }
}
