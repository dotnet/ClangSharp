// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CX_StmtClass;

namespace ClangSharp;

public sealed class MemberExpr : Expr
{
    private readonly Lazy<ValueDecl> _memberDecl;
    private readonly Lazy<IReadOnlyList<TemplateArgumentLoc>> _templateArgs;

    internal MemberExpr(CXCursor handle) : base(handle, CXCursor_MemberRefExpr, CX_StmtClass_MemberExpr)
    {
        Debug.Assert(NumChildren is 1);

        _memberDecl = new Lazy<ValueDecl>(() => TranslationUnit.GetOrCreate<ValueDecl>(Handle.Referenced));
        _templateArgs = new Lazy<IReadOnlyList<TemplateArgumentLoc>>(() => {
            var templateArgCount = Handle.NumTemplateArguments;
            var templateArgs = new List<TemplateArgumentLoc>(templateArgCount);

            for (var i = 0; i < templateArgCount; i++)
            {
                var templateArg = TranslationUnit.GetOrCreate(Handle.GetTemplateArgumentLoc(unchecked((uint)i)));
                templateArgs.Add(templateArg);
            }

            return templateArgs;
        });
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

    public IReadOnlyList<TemplateArgumentLoc> TemplateArgs => _templateArgs.Value;
}
