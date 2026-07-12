// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using ClangSharp.Interop;
using static ClangSharp.Interop.CXTypeKind;
using static ClangSharp.Interop.CX_TypeClass;

namespace ClangSharp;

public sealed class InjectedClassNameType : Type
{
    private ValueLazy<InjectedClassNameType, CXXRecordDecl> _decl;
    private ValueLazy<InjectedClassNameType, Type> _injectedSpecializationType;
    private ValueLazy<InjectedClassNameType, TemplateSpecializationType> _injectedTST;
    private ValueLazy<InjectedClassNameType, TemplateName> _templateName;

    internal unsafe InjectedClassNameType(CXType handle) : base(handle, CXType_Unexposed, CX_TypeClass_InjectedClassName)
    {
        _decl = new ValueLazy<InjectedClassNameType, CXXRecordDecl>(&DeclFactory);
        _injectedSpecializationType = new ValueLazy<InjectedClassNameType, Type>(&InjectedSpecializationTypeFactory);
        _injectedTST = new ValueLazy<InjectedClassNameType, TemplateSpecializationType>(&InjectedTSTFactory);
        _templateName = new ValueLazy<InjectedClassNameType, TemplateName>(&TemplateNameFactory);
    }

    public CXXRecordDecl Decl => _decl.GetValue(this);

    public Type InjectedSpecializationType => _injectedSpecializationType.GetValue(this);

    public TemplateSpecializationType InjectedTST => _injectedTST.GetValue(this);

    public TemplateName TemplateName => _templateName.GetValue(this);

    private static unsafe TemplateName TemplateNameFactory(InjectedClassNameType self) => self.TranslationUnit.GetOrCreate(self.Handle.TemplateName);

    private static unsafe TemplateSpecializationType InjectedTSTFactory(InjectedClassNameType self) => self.TranslationUnit.GetOrCreate<TemplateSpecializationType>(self.Handle.InjectedTST);

    private static unsafe Type InjectedSpecializationTypeFactory(InjectedClassNameType self) => self.TranslationUnit.GetOrCreate<Type>(self.Handle.InjectedSpecializationType);

    private static unsafe CXXRecordDecl DeclFactory(InjectedClassNameType self) => self.TranslationUnit.GetOrCreate<CXXRecordDecl>(self.Handle.Declaration);
}
