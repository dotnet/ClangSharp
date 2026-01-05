// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ClangSharp.Interop;
using static ClangSharp.Interop.CX_StmtClass;
using static ClangSharp.Interop.CXCursorKind;

namespace ClangSharp;

public sealed class CXXDependentScopeMemberExpr : Expr
{
    private readonly ValueLazy<Type> _baseType;
    private readonly ValueLazy<NamedDecl> _firstQualifierFoundInScope;
    private readonly LazyList<TemplateArgumentLoc> _templateArgs;

    internal CXXDependentScopeMemberExpr(CXCursor handle) : base(handle, CXCursor_MemberRefExpr, CX_StmtClass_CXXDependentScopeMemberExpr)
    {
        Debug.Assert(NumChildren is 0 or 1);

        _baseType = new ValueLazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.TypeOperand));
        _firstQualifierFoundInScope = new ValueLazy<NamedDecl>(() => TranslationUnit.GetOrCreate<NamedDecl>(handle.Referenced));
        _templateArgs = LazyList.Create<TemplateArgumentLoc>(Handle.NumTemplateArguments, (i) => TranslationUnit.GetOrCreate(Handle.GetTemplateArgumentLoc(unchecked((uint)i))));
    }

    public Expr? Base => (Expr?)Children.SingleOrDefault();

    public Type BaseType => _baseType.Value;

    public NamedDecl FirstQualifierFoundInScope => _firstQualifierFoundInScope.Value;

    public bool HasExplicitTemplateArgs => Handle.HasExplicitTemplateArgs;

    public bool HasTemplateKeyword => Handle.HasTemplateKeyword;

    public bool IsArrow => Handle.IsArrow;

    public bool IsImplicitAccess => (Base is null) || Base.IsImplicitCXXThis;

    public string MemberName => Handle.Name.CString;

    public uint NumTemplateArgs => unchecked((uint)Handle.NumTemplateArguments);

    public IReadOnlyList<TemplateArgumentLoc> TemplateArgs => _templateArgs;
}
