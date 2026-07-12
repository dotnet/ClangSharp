// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using ClangSharp.Interop;
using static ClangSharp.Interop.CXTypeKind;
using static ClangSharp.Interop.CX_TypeClass;

namespace ClangSharp;

public class AdjustedType : Type
{
    private ValueLazy<AdjustedType, Type> _adjustedType;
    private ValueLazy<AdjustedType, Type> _originalType;

    internal AdjustedType(CXType handle) : this(handle, CXType_Unexposed, CX_TypeClass_Adjusted)
    {
    }

    private protected unsafe AdjustedType(CXType handle, CXTypeKind expectedTypeKind, CX_TypeClass expectedTypeClass) : base(handle, expectedTypeKind, expectedTypeClass)
    {
        _adjustedType = new ValueLazy<AdjustedType, Type>(&AdjustedTypeFactory);
        _originalType = new ValueLazy<AdjustedType, Type>(&OriginalTypeFactory);
    }

    public Type GetAdjustedType => _adjustedType.GetValue(this);

    public Type OriginalType => _originalType.GetValue(this);

    private static unsafe Type OriginalTypeFactory(AdjustedType self) => self.TranslationUnit.GetOrCreate<Type>(self.Handle.OriginalType);

    private static unsafe Type AdjustedTypeFactory(AdjustedType self) => self.TranslationUnit.GetOrCreate<Type>(self.Handle.AdjustedType);
}
