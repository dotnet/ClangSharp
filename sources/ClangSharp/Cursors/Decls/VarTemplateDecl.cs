// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CX_DeclKind;

namespace ClangSharp;

public sealed class VarTemplateDecl : RedeclarableTemplateDecl
{
    internal VarTemplateDecl(CXCursor handle) : base(handle, CXCursor_UnexposedDecl, CX_DeclKind_VarTemplate)
    {
    }

    public new VarTemplateDecl CanonicalDecl => (VarTemplateDecl)base.CanonicalDecl;

    public new VarTemplateDecl InstantiatedFromMemberTemplate => (VarTemplateDecl)base.InstantiatedFromMemberTemplate;

    public bool IsThisDeclarationADefinition => Handle.IsThisDeclarationADefinition;

    public new VarTemplateDecl MostRecentDecl => (VarTemplateDecl)base.MostRecentDecl;

    public new VarTemplateDecl PreviousDecl => (VarTemplateDecl)base.PreviousDecl;

    public new CXXRecordDecl TemplatedDecl => (CXXRecordDecl)base.TemplatedDecl;
}
