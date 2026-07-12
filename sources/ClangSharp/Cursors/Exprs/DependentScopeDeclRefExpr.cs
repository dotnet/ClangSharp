// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Collections.Generic;
using System.Diagnostics;
using ClangSharp.Interop;
using static ClangSharp.Interop.CX_StmtClass;
using static ClangSharp.Interop.CXCursorKind;

namespace ClangSharp;

public sealed class DependentScopeDeclRefExpr : Expr
{
    private readonly LazyList<TemplateArgumentLoc> _templateArgs;

    internal unsafe DependentScopeDeclRefExpr(CXCursor handle) : base(handle, CXCursor_DeclRefExpr, CX_StmtClass_DependentScopeDeclRefExpr)
    {
        Debug.Assert(NumChildren is 0);
        _templateArgs = LazyList.Create<TemplateArgumentLoc>(this, Handle.NumTemplateArguments, &TemplateArgsFactory);
    }

    public string DeclName => Handle.Name.CString;

    public bool HasExplicitTemplateArgs => Handle.HasExplicitTemplateArgs;

    public bool HasTemplateKeyword => Handle.HasTemplateKeyword;

    public IReadOnlyList<TemplateArgumentLoc> TemplateArgs => _templateArgs;

    private static unsafe TemplateArgumentLoc TemplateArgsFactory(object self, int i)
    {
        var @this = (DependentScopeDeclRefExpr)self;
        return @this.TranslationUnit.GetOrCreate(@this.Handle.GetTemplateArgumentLoc(unchecked((uint)i)));
    }
}
