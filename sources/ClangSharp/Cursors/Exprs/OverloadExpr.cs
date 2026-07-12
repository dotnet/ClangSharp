// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Collections.Generic;
using ClangSharp.Interop;
using static ClangSharp.Interop.CX_StmtClass;

namespace ClangSharp;

public class OverloadExpr : Expr
{
    private readonly LazyList<Decl> _decls;
    private ValueLazy<OverloadExpr, CXXRecordDecl> _namingClass;
    private readonly LazyList<TemplateArgumentLoc> _templateArgs;

    private protected unsafe OverloadExpr(CXCursor handle, CXCursorKind expectedCursorKind, CX_StmtClass expectedStmtClass) : base(handle, expectedCursorKind, expectedStmtClass)
    {
        if (handle.StmtClass is > CX_StmtClass_LastOverloadExpr or < CX_StmtClass_FirstOverloadExpr)
        {
            throw new ArgumentOutOfRangeException(nameof(handle));
        }

        _decls = LazyList.Create<Decl>(this, Handle.NumDecls, &DeclsFactory);
        _namingClass = new ValueLazy<OverloadExpr, CXXRecordDecl>(&NamingClassFactory);
        _templateArgs = LazyList.Create<TemplateArgumentLoc>(this, Handle.NumTemplateArguments, &TemplateArgsFactory);
    }

    public IReadOnlyList<Decl> Decls => _decls;

    public bool HasExplicitTemplateArgs => Handle.HasExplicitTemplateArgs;

    public bool HasTemplateKeyword => Handle.HasTemplateKeyword;

    public CXXRecordDecl NamingClass => _namingClass.GetValue(this);

    public string Name => Handle.Name.CString;

    public uint NumDecls => unchecked((uint)Handle.NumDecls);

    public uint NumTemplateArgs => unchecked((uint)Handle.NumTemplateArguments);

    public IReadOnlyList<TemplateArgumentLoc> TemplateArgs => _templateArgs;

    private static unsafe CXXRecordDecl NamingClassFactory(OverloadExpr self) => self.TranslationUnit.GetOrCreate<CXXRecordDecl>(self.Handle.Referenced);

    private static unsafe Decl DeclsFactory(object self, int i)
    {
        var @this = (OverloadExpr)self;
        return @this.TranslationUnit.GetOrCreate<Decl>(@this.Handle.GetDecl(unchecked((uint)i)));
    }

    private static unsafe TemplateArgumentLoc TemplateArgsFactory(object self, int i)
    {
        var @this = (OverloadExpr)self;
        return @this.TranslationUnit.GetOrCreate(@this.Handle.GetTemplateArgumentLoc(unchecked((uint)i)));
    }
}
