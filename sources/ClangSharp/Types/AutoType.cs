// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Collections.Generic;
using ClangSharp.Interop;
using static ClangSharp.Interop.CX_TypeClass;
using static ClangSharp.Interop.CXTypeKind;

namespace ClangSharp;

public sealed class AutoType : DeducedType
{
    private readonly LazyList<TemplateArgument> _templateArgs;

    internal AutoType(CXType handle) : base(handle, CXType_Auto, CX_TypeClass_Auto)
    {
        _templateArgs = LazyList.Create<TemplateArgument>(Handle.NumTemplateArguments, (i) => TranslationUnit.GetOrCreate(Handle.GetTemplateArgument(unchecked((uint)i))));
    }

    public IReadOnlyList<TemplateArgument> Args => _templateArgs;
}
