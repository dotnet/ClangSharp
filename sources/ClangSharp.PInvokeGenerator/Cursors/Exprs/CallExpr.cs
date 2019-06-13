using System;
using System.Collections.Generic;
using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp
{
    internal sealed class CallExpr : Expr
    {
        private readonly Expr[] _arguments;
        private readonly Lazy<Decl> _calleeDecl;

        public CallExpr(CXCursor handle, Cursor parent) : base(handle, parent)
        {
            Debug.Assert(handle.Kind == CXCursorKind.CXCursor_CallExpr);

            _arguments = new Expr[Handle.NumArguments];

            for (uint index = 0; index < Handle.NumArguments; index++)
            {
                var argumentHandle = Handle.GetArgument(index);
                var expr = GetOrAddExpr(argumentHandle);

                _arguments[index] = expr;
                expr.Visit(clientData: default);
            }

            _calleeDecl = new Lazy<Decl>(() =>
            {
                var cursor = TranslationUnit.GetOrCreateCursor(Handle.Referenced, () => Create(Handle.Referenced, this));
                cursor?.Visit(clientData: default);
                return (Decl)cursor;
            });
        }

        public IReadOnlyList<Expr> Arguments => _arguments;

        public Decl CalleeDecl => _calleeDecl.Value;

        public FunctionDecl DirectCallee => CalleeDecl as FunctionDecl;

        public CXSourceRange GetCalleeDeclNameRange(CXNameRefFlags nameFlags, uint pieceIndex) => Handle.GetReferenceNameRange(nameFlags, pieceIndex);
    }
}
