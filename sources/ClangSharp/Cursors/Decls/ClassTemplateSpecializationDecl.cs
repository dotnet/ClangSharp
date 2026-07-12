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
    private ValueLazy<ClassTemplateSpecializationDecl, ClassTemplateDecl> _specializedTemplate;
    private readonly LazyList<TemplateArgument> _templateArgs;

    internal unsafe ClassTemplateSpecializationDecl(CXCursor handle) : this(handle, handle.Kind, CX_DeclKind_ClassTemplateSpecialization)
    {
        _specializedTemplate = new ValueLazy<ClassTemplateSpecializationDecl, ClassTemplateDecl>(&SpecializedTemplateFactory);
        _templateArgs = LazyList.Create<TemplateArgument>(this, Handle.NumTemplateArguments, &TemplateArgsFactory);
    }

    private protected unsafe ClassTemplateSpecializationDecl(CXCursor handle, CXCursorKind expectedCursorKind, CX_DeclKind expectedDeclKind) : base(handle, expectedCursorKind, expectedDeclKind)
    {
        if (handle.DeclKind is > CX_DeclKind_LastClassTemplateSpecialization or < CX_DeclKind_FirstClassTemplateSpecialization)
        {
            throw new ArgumentOutOfRangeException(nameof(handle));
        }

        _specializedTemplate = new ValueLazy<ClassTemplateSpecializationDecl, ClassTemplateDecl>(&SpecializedTemplateFactory);
        _templateArgs = LazyList.Create<TemplateArgument>(this, Handle.NumTemplateArguments, &TemplateArgsFactory);
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

    public ClassTemplateDecl SpecializedTemplate => _specializedTemplate.GetValue(this);

    public IReadOnlyList<TemplateArgument> TemplateArgs => _templateArgs;

    private static unsafe ClassTemplateDecl SpecializedTemplateFactory(ClassTemplateSpecializationDecl self) => self.TranslationUnit.GetOrCreate<ClassTemplateDecl>(self.Handle.SpecializedCursorTemplate);

    private static unsafe TemplateArgument TemplateArgsFactory(object self, int i)
    {
        var @this = (ClassTemplateSpecializationDecl)self;
        return @this.TranslationUnit.GetOrCreate(@this.Handle.GetTemplateArgument(unchecked((uint)i)));
    }
}
