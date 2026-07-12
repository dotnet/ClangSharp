// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Diagnostics;
using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CX_StmtClass;

namespace ClangSharp;

public sealed class CXXDefaultInitExpr : Expr
{
    private ValueLazy<CXXDefaultInitExpr, FieldDecl> _field;
    private ValueLazy<CXXDefaultInitExpr, IDeclContext?> _usedContext;

    internal unsafe CXXDefaultInitExpr(CXCursor handle) : base(handle, CXCursor_UnexposedExpr, CX_StmtClass_CXXDefaultInitExpr)
    {
        Debug.Assert(NumChildren is 0);

        _field = new ValueLazy<CXXDefaultInitExpr, FieldDecl>(&FieldFactory);
        _usedContext = new ValueLazy<CXXDefaultInitExpr, IDeclContext?>(&UsedContextFactory);
    }

    public Expr Expr => Field.InClassInitializer;

    public FieldDecl Field => _field.GetValue(this);

    public IDeclContext? UsedContext => _usedContext.GetValue(this);

    private static unsafe IDeclContext? UsedContextFactory(CXXDefaultInitExpr self) => self.TranslationUnit.GetOrCreate<Decl>(self.Handle.UsedContext) as IDeclContext;

    private static unsafe FieldDecl FieldFactory(CXXDefaultInitExpr self) => self.TranslationUnit.GetOrCreate<FieldDecl>(self.Handle.Referenced);
}
