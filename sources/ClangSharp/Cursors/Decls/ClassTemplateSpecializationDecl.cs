// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using ClangSharp.Interop;
using static ClangSharp.Interop.CX_DeclKind;
using static ClangSharp.Interop.CX_TemplateSpecializationKind;

namespace ClangSharp;

public class ClassTemplateSpecializationDecl : CXXRecordDecl
{
    private readonly Lazy<ClassTemplateDecl> _specializedTemplate;
    private readonly LazyList<TemplateArgument> _templateArgs;

    internal ClassTemplateSpecializationDecl(CXCursor handle) : this(handle, handle.Kind, CX_DeclKind_ClassTemplateSpecialization)
    {
        _specializedTemplate = new Lazy<ClassTemplateDecl>(() => TranslationUnit.GetOrCreate<ClassTemplateDecl>(Handle.SpecializedCursorTemplate));
        _templateArgs = LazyList.Create<TemplateArgument>(Handle.NumTemplateArguments, (i) => TranslationUnit.GetOrCreate(Handle.GetTemplateArgument(unchecked((uint)i))));
    }

    private protected ClassTemplateSpecializationDecl(CXCursor handle, CXCursorKind expectedCursorKind, CX_DeclKind expectedDeclKind) : base(handle, expectedCursorKind, expectedDeclKind)
    {
        if (handle.DeclKind is > CX_DeclKind_LastClassTemplateSpecialization or < CX_DeclKind_FirstClassTemplateSpecialization)
        {
            throw new ArgumentOutOfRangeException(nameof(handle));
        }

        _specializedTemplate = new Lazy<ClassTemplateDecl>(() => TranslationUnit.GetOrCreate<ClassTemplateDecl>(Handle.SpecializedCursorTemplate));
        _templateArgs = LazyList.Create<TemplateArgument>(Handle.NumTemplateArguments, (i) => TranslationUnit.GetOrCreate(Handle.GetTemplateArgument(unchecked((uint)i))));
    }

    public bool IsClassScopeExplicitSpecialization => IsExplicitSpecialization && (LexicalDeclContext is CXXRecordDecl);

    public bool IsExplicitInstantiationOrSpecialization
    {
        get
        {
            switch (SpecializationKind)
            {
                case CX_TSK_ExplicitSpecialization:
                case CX_TSK_ExplicitInstantiationDeclaration:
                case CX_TSK_ExplicitInstantiationDefinition:
                {
                    return true;
                }

                case CX_TSK_Undeclared:
                case CX_TSK_ImplicitInstantiation:
                {
                    return false;
                }
            }

            Debug.Fail("bad template specialization kind");
            return false;
        }
    }

    public bool IsExplicitSpecialization => SpecializationKind == CX_TSK_ExplicitSpecialization;

    public new ClassTemplateSpecializationDecl MostRecentDecl => (ClassTemplateSpecializationDecl)base.MostRecentDecl;

    public CX_TemplateSpecializationKind SpecializationKind => Handle.TemplateSpecializationKind;

    public ClassTemplateDecl SpecializedTemplate => _specializedTemplate.Value;

    public IReadOnlyList<TemplateArgument> TemplateArgs => _templateArgs;
}
