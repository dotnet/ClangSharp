// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Collections.Generic;
using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CX_DeclKind;

namespace ClangSharp;

public sealed class FunctionTemplateDecl : RedeclarableTemplateDecl
{
    private readonly LazyList<TemplateArgument> _injectedTemplateArgs;
    private readonly LazyList<FunctionDecl> _specializations;

    internal FunctionTemplateDecl(CXCursor handle) : base(handle, CXCursor_FunctionTemplate, CX_DeclKind_FunctionTemplate)
    {
        _injectedTemplateArgs = LazyList.Create<TemplateArgument>(Handle.NumTemplateArguments, (i) => TranslationUnit.GetOrCreate(Handle.GetTemplateArgument(unchecked((uint)i))));
        _specializations = LazyList.Create<FunctionDecl>(Handle.NumSpecializations, (i) => TranslationUnit.GetOrCreate<FunctionDecl>(Handle.GetSpecialization(unchecked((uint)i))));
    }

    public new FunctionTemplateDecl CanonicalDecl => (FunctionTemplateDecl)base.CanonicalDecl;

    public IReadOnlyList<TemplateArgument> InjectedTemplateArgs => _injectedTemplateArgs;

    public new FunctionTemplateDecl InstantiatedFromMemberTemplate => (FunctionTemplateDecl)base.InstantiatedFromMemberTemplate;

    public bool IsThisDeclarationADefinition => Handle.IsThisDeclarationADefinition;

    public new FunctionTemplateDecl MostRecentDecl => (FunctionTemplateDecl)base.MostRecentDecl;

    public new FunctionTemplateDecl PreviousDecl => (FunctionTemplateDecl)base.PreviousDecl;

    public IReadOnlyList<FunctionDecl> Specializations => _specializations;

    public new FunctionDecl TemplatedDecl => (FunctionDecl)base.TemplatedDecl;
}
