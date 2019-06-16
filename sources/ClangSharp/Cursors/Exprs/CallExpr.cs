using System;
using System.Collections.Generic;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class CallExpr : Expr
    {
        private readonly Lazy<IReadOnlyList<Expr>> _args;
        private readonly Lazy<Decl> _calleeDecl;

        internal CallExpr(CXCursor handle) : base(handle, CXCursorKind.CXCursor_CallExpr)
        {
            _args = new Lazy<IReadOnlyList<Expr>>(() => {
                var numArgs = NumArgs;
                var args = new List<Expr>((int)numArgs);

                for (var index = 0u; index < numArgs; index++)
                {
                    var arg = Handle.GetArgument(index);
                    args.Add(TranslationUnit.GetOrCreate<Expr>(arg));
                }

                return args;
            });

            _calleeDecl = new Lazy<Decl>(() => TranslationUnit.GetOrCreate<Decl>(Handle.Referenced));
        }

        public IReadOnlyList<Expr> Args => _args.Value;

        public Decl CalleeDecl => _calleeDecl.Value;

        public FunctionDecl DirectCallee => CalleeDecl as FunctionDecl;

        public uint NumArgs => (uint)Handle.NumArguments;
    }
}
