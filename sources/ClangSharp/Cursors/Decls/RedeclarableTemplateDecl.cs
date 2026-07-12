// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using ClangSharp.Interop;
using static ClangSharp.Interop.CX_DeclKind;

namespace ClangSharp;

public class RedeclarableTemplateDecl : TemplateDecl, IRedeclarable<RedeclarableTemplateDecl>
{
    private ValueLazy<RedeclarableTemplateDecl, RedeclarableTemplateDecl> _instantiatedFromMemberTemplate;

    private protected unsafe RedeclarableTemplateDecl(CXCursor handle, CXCursorKind expectedCursorKind, CX_DeclKind expectedDeclKind) : base(handle, expectedCursorKind, expectedDeclKind)
    {
        if (handle.DeclKind is > CX_DeclKind_LastRedeclarableTemplate or < CX_DeclKind_FirstRedeclarableTemplate)
        {
            throw new ArgumentOutOfRangeException(nameof(handle));
        }

        _instantiatedFromMemberTemplate = new ValueLazy<RedeclarableTemplateDecl, RedeclarableTemplateDecl>(&InstantiatedFromMemberTemplateFactory);
    }

    public new RedeclarableTemplateDecl CanonicalDecl => (RedeclarableTemplateDecl)base.CanonicalDecl;

    public bool IsMemberSpecialization => Handle.IsMemberSpecialization;

    public RedeclarableTemplateDecl InstantiatedFromMemberTemplate => _instantiatedFromMemberTemplate.GetValue(this);

    private static unsafe RedeclarableTemplateDecl InstantiatedFromMemberTemplateFactory(RedeclarableTemplateDecl self) => self.TranslationUnit.GetOrCreate<RedeclarableTemplateDecl>(self.Handle.SpecializedCursorTemplate);
}
