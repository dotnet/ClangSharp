// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using ClangSharp.Interop;
using static ClangSharp.Interop.CXTypeKind;
using static ClangSharp.Interop.CX_TypeClass;

namespace ClangSharp;

public sealed class InjectedClassNameType : Type
{
    private readonly Lazy<CXXRecordDecl> _decl;
    private readonly Lazy<Type> _injectedSpecializationType;
    private readonly Lazy<TemplateSpecializationType> _injectedTST;
    private readonly Lazy<TemplateName> _templateName;

    internal InjectedClassNameType(CXType handle) : base(handle, CXType_Unexposed, CX_TypeClass_InjectedClassName)
    {
        _decl = new Lazy<CXXRecordDecl>(() => TranslationUnit.GetOrCreate<CXXRecordDecl>(handle.Declaration));
        _injectedSpecializationType = new Lazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.InjectedSpecializationType));
        _injectedTST = new Lazy<TemplateSpecializationType>(() => TranslationUnit.GetOrCreate<TemplateSpecializationType>(Handle.InjectedTST));
        _templateName = new Lazy<TemplateName>(() => TranslationUnit.GetOrCreate(Handle.TemplateName));
    }

    public CXXRecordDecl Decl => _decl.Value;

    public Type InjectedSpecializationType => _injectedSpecializationType.Value;

    public TemplateSpecializationType InjectedTST => _injectedTST.Value;

    public TemplateName TemplateName => _templateName.Value;
}
