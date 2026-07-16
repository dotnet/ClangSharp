// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using ClangSharp.Interop;
using static ClangSharp.Interop.CX_TypeClass;
using static ClangSharp.Interop.CXTypeKind;

namespace ClangSharp;

public sealed class TemplateSpecializationType : TypeWithKeyword
{
    private readonly LazyList<TemplateArgument> _templateArgs;
    private ValueLazy<TemplateSpecializationType, TemplateName> _templateName;

    internal unsafe TemplateSpecializationType(CXType handle) : base(handle, CXType_Unexposed, CX_TypeClass_TemplateSpecialization)
    {
        _templateArgs = LazyList.Create<TemplateArgument>(this, Handle.NumTemplateArguments, &TemplateArgsFactory);
        _templateName = new ValueLazy<TemplateSpecializationType, TemplateName>(&TemplateNameFactory);
    }

    public Type? AliasedType => IsTypeAlias ? Desugar : null;

    public IReadOnlyList<TemplateArgument> Args => _templateArgs;

    [MemberNotNullWhen(true, nameof(AliasedType))]
    public bool IsTypeAlias => Handle.IsTypeAlias;

    public TemplateName TemplateName => _templateName.GetValue(this);

    private static unsafe TemplateName TemplateNameFactory(TemplateSpecializationType self) => self.TranslationUnit.GetOrCreate(self.Handle.TemplateName);

    private static unsafe TemplateArgument TemplateArgsFactory(object self, int i)
    {
        var @this = (TemplateSpecializationType)self;
        return @this.TranslationUnit.GetOrCreate(@this.Handle.GetTemplateArgument(unchecked((uint)i)));
    }
}
