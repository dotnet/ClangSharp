// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Collections.Generic;
using System.Diagnostics;
using ClangSharp.Interop;
using static ClangSharp.Interop.CX_StmtClass;
using static ClangSharp.Interop.CXCursorKind;

namespace ClangSharp;

public sealed class MemberExpr : Expr
{
    private readonly ValueLazy<ValueDecl> _memberDecl;
    private readonly LazyList<TemplateArgumentLoc> _templateArgs;

    internal MemberExpr(CXCursor handle) : base(handle, CXCursor_MemberRefExpr, CX_StmtClass_MemberExpr)
    {
        Debug.Assert(NumChildren is 1);

        _memberDecl = new ValueLazy<ValueDecl>(() => TranslationUnit.GetOrCreate<ValueDecl>(Handle.Referenced));
        _templateArgs = LazyList.Create<TemplateArgumentLoc>(Handle.NumTemplateArguments, (i) => TranslationUnit.GetOrCreate(Handle.GetTemplateArgumentLoc(unchecked((uint)i))));
    }

    public Expr Base => (Expr)Children[0];

    public bool HadMultipleCandidates => Handle.HadMultipleCandidates;

    public bool HasExplicitTemplateArg => Handle.HasExplicitTemplateArgs;

    public bool HasTemplateKeyword => Handle.HasTemplateKeyword;

    public bool IsArrow => Handle.IsArrow;

    public bool IsImplicitAccess => Handle.IsImplicit;

    public ValueDecl MemberDecl => _memberDecl.Value;

    public string MemberName => Handle.Name.CString;

    public uint NumTemplateArgs => unchecked((uint)Handle.NumTemplateArguments);

    public IReadOnlyList<TemplateArgumentLoc> TemplateArgs => _templateArgs;
}
