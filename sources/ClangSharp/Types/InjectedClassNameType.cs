// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using ClangSharp.Interop;
using static ClangSharp.Interop.CXTypeKind;
using static ClangSharp.Interop.CX_TypeClass;

namespace ClangSharp;

public sealed class InjectedClassNameType : Type
{
    private readonly ValueLazy<CXXRecordDecl> _decl;
    private readonly ValueLazy<Type> _injectedSpecializationType;
    private readonly ValueLazy<TemplateSpecializationType> _injectedTST;
    private readonly ValueLazy<TemplateName> _templateName;

    internal InjectedClassNameType(CXType handle) : base(handle, CXType_Unexposed, CX_TypeClass_InjectedClassName)
    {
        _decl = new ValueLazy<CXXRecordDecl>(() => TranslationUnit.GetOrCreate<CXXRecordDecl>(handle.Declaration));
        _injectedSpecializationType = new ValueLazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.InjectedSpecializationType));
        _injectedTST = new ValueLazy<TemplateSpecializationType>(() => TranslationUnit.GetOrCreate<TemplateSpecializationType>(Handle.InjectedTST));
        _templateName = new ValueLazy<TemplateName>(() => TranslationUnit.GetOrCreate(Handle.TemplateName));
    }

    public CXXRecordDecl Decl => _decl.Value;

    public Type InjectedSpecializationType => _injectedSpecializationType.Value;

    public TemplateSpecializationType InjectedTST => _injectedTST.Value;

    public TemplateName TemplateName => _templateName.Value;
}
