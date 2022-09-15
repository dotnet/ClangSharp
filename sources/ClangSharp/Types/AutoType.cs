// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Collections.Generic;
using ClangSharp.Interop;

namespace ClangSharp;

public sealed class AutoType : DeducedType
{
    private readonly Lazy<IReadOnlyList<TemplateArgument>> _templateArgs;

    internal AutoType(CXType handle) : base(handle, CXTypeKind.CXType_Auto, CX_TypeClass.CX_TypeClass_Auto)
    {
        _templateArgs = new Lazy<IReadOnlyList<TemplateArgument>>(() => {
            var templateArgCount = Handle.NumTemplateArguments;
            var templateArgs = new List<TemplateArgument>(templateArgCount);

            for (var i = 0; i < templateArgCount; i++)
            {
                var templateArg = TranslationUnit.GetOrCreate(Handle.GetTemplateArgument(unchecked((uint)i)));
                templateArgs.Add(templateArg);
            }

            return templateArgs;
        });
    }

    public IReadOnlyList<TemplateArgument> Args => _templateArgs.Value;
}
