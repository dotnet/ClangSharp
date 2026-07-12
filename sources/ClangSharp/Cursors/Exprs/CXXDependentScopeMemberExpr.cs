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
    private ValueLazy<CXXDependentScopeMemberExpr, Type> _baseType;
    private ValueLazy<CXXDependentScopeMemberExpr, NamedDecl> _firstQualifierFoundInScope;
    private readonly LazyList<TemplateArgumentLoc> _templateArgs;

    internal unsafe CXXDependentScopeMemberExpr(CXCursor handle) : base(handle, CXCursor_MemberRefExpr, CX_StmtClass_CXXDependentScopeMemberExpr)
    {
        Debug.Assert(NumChildren is 0 or 1);

        _baseType = new ValueLazy<CXXDependentScopeMemberExpr, Type>(&BaseTypeFactory);
        _firstQualifierFoundInScope = new ValueLazy<CXXDependentScopeMemberExpr, NamedDecl>(&FirstQualifierFoundInScopeFactory);
        _templateArgs = LazyList.Create<TemplateArgumentLoc>(Handle.NumTemplateArguments, (i) => TranslationUnit.GetOrCreate(Handle.GetTemplateArgumentLoc(unchecked((uint)i))));
    }

    public Expr? Base => (Expr?)Children.SingleOrDefault();

    public Type BaseType => _baseType.GetValue(this);

    public NamedDecl FirstQualifierFoundInScope => _firstQualifierFoundInScope.GetValue(this);

    public bool HasExplicitTemplateArgs => Handle.HasExplicitTemplateArgs;

    public bool HasTemplateKeyword => Handle.HasTemplateKeyword;

    public bool IsArrow => Handle.IsArrow;

    public bool IsImplicitAccess => (Base is null) || Base.IsImplicitCXXThis;

    public string MemberName => Handle.Name.CString;

    public uint NumTemplateArgs => unchecked((uint)Handle.NumTemplateArguments);

    public IReadOnlyList<TemplateArgumentLoc> TemplateArgs => _templateArgs;

    private static unsafe NamedDecl FirstQualifierFoundInScopeFactory(CXXDependentScopeMemberExpr self) => self.TranslationUnit.GetOrCreate<NamedDecl>(self.Handle.Referenced);

    private static unsafe Type BaseTypeFactory(CXXDependentScopeMemberExpr self) => self.TranslationUnit.GetOrCreate<Type>(self.Handle.TypeOperand);
}
