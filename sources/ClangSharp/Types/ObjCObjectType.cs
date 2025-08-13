// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Collections.Generic;
using ClangSharp.Interop;
using static ClangSharp.Interop.CXTypeKind;
using static ClangSharp.Interop.CX_TypeClass;

namespace ClangSharp;

public class ObjCObjectType : Type
{
    private readonly ValueLazy<Type> _baseType;
    private readonly ValueLazy<ObjCInterfaceDecl> _interface;
    private readonly LazyList<ObjCProtocolDecl> _protocols;
    private readonly ValueLazy<Type> _superClassType;
    private readonly LazyList<Type> _typeArgs;

    internal ObjCObjectType(CXType handle) : this(handle, CXType_ObjCObject, CX_TypeClass_ObjCObject)
    {
    }

    private protected ObjCObjectType(CXType handle, CXTypeKind expectedTypeKind, CX_TypeClass expectedTypeClass) : base(handle, expectedTypeKind, expectedTypeClass)
    {
        _baseType = new ValueLazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.ObjCObjectBaseType));
        _interface = new ValueLazy<ObjCInterfaceDecl>(() => TranslationUnit.GetOrCreate<ObjCInterfaceDecl>(Handle.Declaration));
        _protocols = LazyList.Create<ObjCProtocolDecl>(unchecked((int)Handle.NumObjCProtocolRefs), (i) => TranslationUnit.GetOrCreate<ObjCProtocolDecl>(Handle.GetObjCProtocolDecl(unchecked((uint)i))));
        _superClassType = new ValueLazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.UnderlyingType));
        _typeArgs = LazyList.Create<Type>(unchecked((int)Handle.NumObjCTypeArgs), (i) => TranslationUnit.GetOrCreate<Type>(Handle.GetObjCTypeArg(unchecked((uint)i))));
    }

    public Type BaseType => _baseType.Value;

    public ObjCInterfaceDecl Interface => _interface.Value;

    public IReadOnlyList<ObjCProtocolDecl> Protocols => _protocols;

    public Type SuperClassType => _superClassType.Value;

    public IReadOnlyList<Type> TypeArgs => _typeArgs;
}
