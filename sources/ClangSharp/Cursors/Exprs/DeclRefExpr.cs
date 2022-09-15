// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp;

public sealed class DeclRefExpr : Expr
{
    private readonly Lazy<ValueDecl> _decl;
    private readonly Lazy<NamedDecl> _foundDecl;
    private readonly Lazy<IReadOnlyList<TemplateArgumentLoc>> _templateArgs;

    internal DeclRefExpr(CXCursor handle) : base(handle, handle.Kind, CX_StmtClass.CX_StmtClass_DeclRefExpr)
    {
        if (handle.Kind is not CXCursorKind.CXCursor_DeclRefExpr and not CXCursorKind.CXCursor_ObjCSelfExpr)
        {
            throw new ArgumentOutOfRangeException(nameof(handle));
        }

        Debug.Assert(NumChildren is 0);

        _decl = new Lazy<ValueDecl>(() => TranslationUnit.GetOrCreate<ValueDecl>(Handle.Referenced));
        _foundDecl = new Lazy<NamedDecl>(() => TranslationUnit.GetOrCreate<NamedDecl>(Handle.FoundDecl));

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

    public ValueDecl Decl => _decl.Value;

    public NamedDecl FoundDecl => _foundDecl.Value;

    public bool HadMultipleCandidates => Handle.HadMultipleCandidates;

    public bool HasExplicitTemplateArgs => Handle.HasExplicitTemplateArgs;

    public bool HasTemplateKeyword => Handle.HasTemplateKeyword;

    public string Name => Handle.Name.CString;

    public uint NumTemplateArgs => unchecked((uint)Handle.NumTemplateArguments);

    public IReadOnlyList<TemplateArgumentLoc> TemplateArgs => _templateArgs.Value;
}
