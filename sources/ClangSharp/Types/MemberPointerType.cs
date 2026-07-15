// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using ClangSharp.Interop;
using static ClangSharp.Interop.CXTypeKind;
using static ClangSharp.Interop.CX_TypeClass;

namespace ClangSharp;

public sealed class MemberPointerType : Type
{
    private ValueLazy<MemberPointerType, Type> _classType;

    internal unsafe MemberPointerType(CXType handle) : base(handle, CXType_MemberPointer, CX_TypeClass_MemberPointer)
    {
        _classType = new ValueLazy<MemberPointerType, Type>(&ClassTypeFactory);
    }

    public Type ClassType => _classType.GetValue(this);

    public bool IsMemberDataPointer => !IsMemberFunctionPointer;

    public bool IsMemberFunctionPointer => PointeeType.CanonicalType is FunctionProtoType;

    private static unsafe Type ClassTypeFactory(MemberPointerType self) => self.TranslationUnit.GetOrCreate<Type>(self.Handle.ClassType);
}
