// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using System.Collections.Generic;
using ClangSharp.Interop;

namespace ClangSharp
{
    public class CXXConstructExpr : Expr
    {
        private readonly Lazy<IReadOnlyList<Expr>> _args;

        internal CXXConstructExpr(CXCursor handle) : this(handle, CXCursorKind.CXCursor_CallExpr, CX_StmtClass.CX_StmtClass_CXXConstructExpr)
        {
        }

        private protected CXXConstructExpr(CXCursor handle, CXCursorKind expectedCursorKind, CX_StmtClass expectedStmtClass) : base(handle, expectedCursorKind, expectedStmtClass)
        {
            if ((CX_StmtClass.CX_StmtClass_LastCXXConstructExpr < handle.StmtClass) || (handle.StmtClass < CX_StmtClass.CX_StmtClass_FirstCXXConstructExpr))
            {
                throw new ArgumentException(nameof(handle));
            }

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
        }

        public IReadOnlyList<Expr> Args => _args.Value;

        public uint NumArgs => (uint)Handle.NumArguments;
    }
}
