// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Collections.Generic;
using ClangSharp.Interop;
using static ClangSharp.Interop.CX_TypeClass;
using static ClangSharp.Interop.CXTypeKind;

namespace ClangSharp;

public sealed class FunctionProtoType : FunctionType
{
    private readonly LazyList<Type> _paramTypes;

    internal FunctionProtoType(CXType handle) : base(handle, CXType_FunctionProto, CX_TypeClass_FunctionProto)
    {
        _paramTypes = LazyList.Create<Type>(Handle.NumArgTypes, (i) => TranslationUnit.GetOrCreate<Type>(Handle.GetArgType(unchecked((uint)i))));
    }

    public CXCursor_ExceptionSpecificationKind ExceptionSpecType => Handle.ExceptionSpecificationType;

    public bool IsVariadic => Handle.IsFunctionTypeVariadic;

    public uint NumParams => (uint)Handle.NumArgTypes;

    public IReadOnlyList<Type> ParamTypes => _paramTypes;

    public CXRefQualifierKind RefQualifier => Handle.CXXRefQualifier;
}
