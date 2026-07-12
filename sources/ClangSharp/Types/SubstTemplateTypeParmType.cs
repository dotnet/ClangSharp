// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using ClangSharp.Interop;
using static ClangSharp.Interop.CXTypeKind;
using static ClangSharp.Interop.CX_TypeClass;

namespace ClangSharp;

public sealed class SubstTemplateTypeParmType : Type
{
    private ValueLazy<SubstTemplateTypeParmType, Decl?> _associatedDecl;
    private ValueLazy<SubstTemplateTypeParmType, TemplateTypeParmType> _replacedParameter;

    internal unsafe SubstTemplateTypeParmType(CXType handle) : base(handle, CXType_Unexposed, CX_TypeClass_SubstTemplateTypeParm)
    {
        _associatedDecl = new ValueLazy<SubstTemplateTypeParmType, Decl?>(&AssociatedDeclFactory);
        _replacedParameter = new ValueLazy<SubstTemplateTypeParmType, TemplateTypeParmType>(&ReplacedParameterFactory);
    }

    public TemplateTypeParmType ReplacedParameter => _replacedParameter.GetValue(this);

    public Type ReplacementType => Desugar;

    public Decl? AssociatedDecl => _associatedDecl.GetValue(this);

    private static unsafe TemplateTypeParmType ReplacedParameterFactory(SubstTemplateTypeParmType self) => self.TranslationUnit.GetOrCreate<TemplateTypeParmType>(self.Handle.OriginalType);

    private static unsafe Decl? AssociatedDeclFactory(SubstTemplateTypeParmType self) {
            var cursor = clangsharp.Type_getSubstTemplateTypeParamAssociatedDecl(self.Handle);
            return cursor.IsNull ? null : self.TranslationUnit.GetOrCreate<Decl>(cursor);
        }
}
