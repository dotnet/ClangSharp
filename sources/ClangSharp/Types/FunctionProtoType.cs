// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Collections.Generic;
using ClangSharp.Interop;
using static ClangSharp.Interop.CX_TypeClass;
using static ClangSharp.Interop.CXTypeKind;

namespace ClangSharp;

public sealed class FunctionProtoType : FunctionType
{
    private readonly LazyList<Type> _paramTypes;

    internal unsafe FunctionProtoType(CXType handle) : base(handle, CXType_FunctionProto, CX_TypeClass_FunctionProto)
    {
        _paramTypes = LazyList.Create<Type>(this, Handle.NumArgTypes, &ParamTypesFactory);
    }

    public CXCursor_ExceptionSpecificationKind ExceptionSpecType => Handle.ExceptionSpecificationType;

    public bool IsVariadic => Handle.IsFunctionTypeVariadic;

    public uint NumParams => (uint)Handle.NumArgTypes;

    public IReadOnlyList<Type> ParamTypes => _paramTypes;

    public CXRefQualifierKind RefQualifier => Handle.CXXRefQualifier;

    private static unsafe Type ParamTypesFactory(object self, int i)
    {
        var @this = (FunctionProtoType)self;
        return @this.TranslationUnit.GetOrCreate<Type>(@this.Handle.GetArgType(unchecked((uint)i)));
    }
}
