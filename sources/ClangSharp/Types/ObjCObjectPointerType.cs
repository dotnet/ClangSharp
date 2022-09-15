// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Collections.Generic;
using ClangSharp.Interop;

namespace ClangSharp;

public sealed class ObjCObjectPointerType : Type
{
    private readonly Lazy<ObjCInterfaceType> _interfaceType;
    private readonly Lazy<Type> _superClassType;

    internal ObjCObjectPointerType(CXType handle) : base(handle, CXTypeKind.CXType_ObjCObjectPointer, CX_TypeClass.CX_TypeClass_ObjCObjectPointer)
    {
        _interfaceType = new Lazy<ObjCInterfaceType>(() => TranslationUnit.GetOrCreate<ObjCInterfaceType>(Handle.OriginalType));
        _superClassType = new Lazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.UnderlyingType));
    }

    public ObjCInterfaceDecl InterfaceDecl => ObjectType.Interface;

    public ObjCInterfaceType InterfaceType => _interfaceType.Value;

    public ObjCObjectType ObjectType => PointeeType.CastAs<ObjCObjectType>();

    public IReadOnlyList<ObjCProtocolDecl> Protocols => ObjectType.Protocols;

    public Type SuperClassType => _superClassType.Value;

    public IReadOnlyList<Type> TypeArgs => ObjectType.TypeArgs;
}
