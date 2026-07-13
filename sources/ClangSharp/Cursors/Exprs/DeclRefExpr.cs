// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using ClangSharp.Interop;
using static ClangSharp.Interop.CX_StmtClass;
using static ClangSharp.Interop.CXCursorKind;

namespace ClangSharp;

public sealed class DeclRefExpr : Expr
{
    private ValueLazy<DeclRefExpr, ValueDecl> _decl;
    private ValueLazy<DeclRefExpr, NamedDecl> _foundDecl;
    private readonly LazyList<TemplateArgumentLoc> _templateArgs;

    internal unsafe DeclRefExpr(CXCursor handle) : base(handle, handle.Kind, CX_StmtClass_DeclRefExpr)
    {
        if (handle.Kind is not CXCursor_DeclRefExpr and not CXCursor_ObjCSelfExpr)
        {
            throw new ArgumentOutOfRangeException(nameof(handle));
        }

        Debug.Assert(NumChildren is 0);

        _decl = new ValueLazy<DeclRefExpr, ValueDecl>(&DeclFactory);
        _foundDecl = new ValueLazy<DeclRefExpr, NamedDecl>(&FoundDeclFactory);
        _templateArgs = LazyList.Create<TemplateArgumentLoc>(this, Handle.NumTemplateArguments, &TemplateArgsFactory);
    }

    public ValueDecl Decl => _decl.GetValue(this);

    public NamedDecl FoundDecl => _foundDecl.GetValue(this);

    public bool HadMultipleCandidates => Handle.HadMultipleCandidates;

    public bool HasExplicitTemplateArgs => Handle.HasExplicitTemplateArgs;

    public bool HasTemplateKeyword => Handle.HasTemplateKeyword;

    public string Name => Handle.Name.CString;

    public uint NumTemplateArgs => unchecked((uint)Handle.NumTemplateArguments);

    public IReadOnlyList<TemplateArgumentLoc> TemplateArgs => _templateArgs;

    private static unsafe NamedDecl FoundDeclFactory(DeclRefExpr self) => self.TranslationUnit.GetOrCreate<NamedDecl>(self.Handle.FoundDecl);

    private static unsafe ValueDecl DeclFactory(DeclRefExpr self) => self.TranslationUnit.GetOrCreate<ValueDecl>(self.Handle.Referenced);

    private static unsafe TemplateArgumentLoc TemplateArgsFactory(object self, int i)
    {
        var @this = (DeclRefExpr)self;
        return @this.TranslationUnit.GetOrCreate(@this.Handle.GetTemplateArgumentLoc(unchecked((uint)i)));
    }
}
