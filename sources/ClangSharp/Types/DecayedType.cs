// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using ClangSharp.Interop;
using static ClangSharp.Interop.CXTypeKind;
using static ClangSharp.Interop.CX_TypeClass;

namespace ClangSharp;

public sealed class DecayedType : AdjustedType
{
    private ValueLazy<DecayedType, Type> _decayedType;

    internal unsafe DecayedType(CXType handle) : base(handle, CXType_Unexposed, CX_TypeClass_Decayed)
    {
        _decayedType = new ValueLazy<DecayedType, Type>(&DecayedTypeFactory);
    }

    public Type GetDecayedType => _decayedType.GetValue(this);

    private static unsafe Type DecayedTypeFactory(DecayedType self) => self.TranslationUnit.GetOrCreate<Type>(self.Handle.DecayedType);
}
