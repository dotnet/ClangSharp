// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Collections.Generic;
using ClangSharp.Interop;
using static ClangSharp.Interop.CX_TypeClass;
using static ClangSharp.Interop.CXTypeKind;

namespace ClangSharp;

public sealed class AutoType : DeducedType
{
    private readonly LazyList<TemplateArgument> _templateArgs;

    internal unsafe AutoType(CXType handle) : base(handle, CXType_Auto, CX_TypeClass_Auto)
    {
        _templateArgs = LazyList.Create<TemplateArgument>(this, Handle.NumTemplateArguments, &TemplateArgsFactory);
    }

    public IReadOnlyList<TemplateArgument> Args => _templateArgs;

    public bool IsConstrained => Handle.IsConstrained;

    public bool IsDecltypeAuto => Handle.IsDecltypeAuto;

    public bool IsGNUAutoType => Handle.IsGNUAutoType;

    public CX_AutoTypeKeyword Keyword => Handle.AutoTypeKeyword;

    private static unsafe TemplateArgument TemplateArgsFactory(object self, int i)
    {
        var @this = (AutoType)self;
        return @this.TranslationUnit.GetOrCreate(@this.Handle.GetTemplateArgument(unchecked((uint)i)));
    }
}
