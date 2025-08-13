// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Collections.Generic;
using ClangSharp.Interop;
using static ClangSharp.Interop.CX_StmtClass;

namespace ClangSharp;

public class OverloadExpr : Expr
{
    private readonly LazyList<Decl> _decls;
    private readonly ValueLazy<CXXRecordDecl> _namingClass;
    private readonly LazyList<TemplateArgumentLoc> _templateArgs;

    private protected OverloadExpr(CXCursor handle, CXCursorKind expectedCursorKind, CX_StmtClass expectedStmtClass) : base(handle, expectedCursorKind, expectedStmtClass)
    {
        if (handle.StmtClass is > CX_StmtClass_LastOverloadExpr or < CX_StmtClass_FirstOverloadExpr)
        {
            throw new ArgumentOutOfRangeException(nameof(handle));
        }

        _decls = LazyList.Create<Decl>(Handle.NumDecls, (i) => TranslationUnit.GetOrCreate<Decl>(Handle.GetDecl(unchecked((uint)i))));
        _namingClass = new ValueLazy<CXXRecordDecl>(() => TranslationUnit.GetOrCreate<CXXRecordDecl>(Handle.Referenced));
        _templateArgs = LazyList.Create<TemplateArgumentLoc>(Handle.NumTemplateArguments, (i) => TranslationUnit.GetOrCreate(Handle.GetTemplateArgumentLoc(unchecked((uint)i))));
    }

    public IReadOnlyList<Decl> Decls => _decls;

    public bool HasExplicitTemplateArgs => Handle.HasExplicitTemplateArgs;

    public bool HasTemplateKeyword => Handle.HasTemplateKeyword;

    public CXXRecordDecl NamingClass => _namingClass.Value;

    public string Name => Handle.Name.CString;

    public uint NumDecls => unchecked((uint)Handle.NumDecls);

    public uint NumTemplateArgs => unchecked((uint)Handle.NumTemplateArguments);

    public IReadOnlyList<TemplateArgumentLoc> TemplateArgs => _templateArgs;
}
