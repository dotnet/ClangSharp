// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using ClangSharp.Interop;
using static ClangSharp.Interop.CXTypeKind;
using static ClangSharp.Interop.CX_TypeClass;

namespace ClangSharp;

public sealed class AtomicType : Type
{
    private readonly ValueLazy<Type> _valueType;

    internal AtomicType(CXType handle) : base(handle, CXType_Atomic, CX_TypeClass_Atomic)
    {
        _valueType = new ValueLazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.ValueType));
    }

    public Type ValueType => _valueType.Value;
}
