// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class LabelDecl : NamedDecl
    {
        private readonly Lazy<LabelStmt> _stmt;

        internal LabelDecl(CXCursor handle) : base(handle, CXCursorKind.CXCursor_UnexposedDecl, CX_DeclKind.CX_DeclKind_Label)
        {
            _stmt = new Lazy<LabelStmt>(() => TranslationUnit.GetOrCreate<LabelStmt>(Handle.GetExpr(0)));
        }

        public LabelStmt Stmt => _stmt.Value;
    }
}
