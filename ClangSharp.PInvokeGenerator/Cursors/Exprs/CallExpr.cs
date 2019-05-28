using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ClangSharp
{
    internal sealed class CallExpr : Expr
    {
        private readonly Expr[] _arguments;
        private readonly Lazy<Cursor> _referenced;

        public CallExpr(CXCursor handle, Cursor parent) : base(handle, parent)
        {
            Debug.Assert(handle.Kind == CXCursorKind.CXCursor_CallExpr);

            _arguments = new Expr[Handle.NumArguments];

            for (uint index = 0; index < Handle.NumArguments; index++)
            {
                var argumentHandle = Handle.GetArgument(index);
                var expr = GetOrAddChild<Expr>(argumentHandle);

                _arguments[index] = expr;
                expr.Visit(clientData: default);
            }

            _referenced = new Lazy<Cursor>(() =>
            {
                var cursor = TranslationUnit.GetOrCreateCursor(handle.Referenced, () => Create(handle.Referenced, this));
                cursor.Visit(clientData: default);
                return cursor;
            });
        }

        public IReadOnlyList<Expr> Arguments => _arguments;

        public Cursor Referenced => _referenced.Value;

        public CXSourceRange GetReferenceNameRange(CXNameRefFlags nameFlags, uint pieceIndex) => Handle.GetReferenceNameRange(nameFlags, pieceIndex);
    }
}
