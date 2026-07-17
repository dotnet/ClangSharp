// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Collections.Generic;
using ClangSharp.Interop;
using static ClangSharp.Interop.CXTypeKind;
using static ClangSharp.Interop.CX_TypeClass;

namespace ClangSharp;

public class ObjCObjectType : Type
{
    private ValueLazy<ObjCObjectType, Type> _baseType;
    private ValueLazy<ObjCObjectType, ObjCInterfaceDecl> _interface;
    private readonly LazyList<ObjCProtocolDecl> _protocols;
    private ValueLazy<ObjCObjectType, Type> _superClassType;
    private readonly LazyList<Type> _typeArgs;

    internal ObjCObjectType(CXType handle) : this(handle, CXType_ObjCObject, CX_TypeClass_ObjCObject)
    {
    }

    private protected unsafe ObjCObjectType(CXType handle, CXTypeKind expectedTypeKind, CX_TypeClass expectedTypeClass) : base(handle, expectedTypeKind, expectedTypeClass)
    {
        _baseType = new ValueLazy<ObjCObjectType, Type>(&BaseTypeFactory);
        _interface = new ValueLazy<ObjCObjectType, ObjCInterfaceDecl>(&InterfaceFactory);
        _protocols = LazyList.Create<ObjCProtocolDecl>(this, unchecked((int)Handle.NumObjCProtocolRefs), &ProtocolsFactory);
        _superClassType = new ValueLazy<ObjCObjectType, Type>(&SuperClassTypeFactory);
        _typeArgs = LazyList.Create<Type>(this, unchecked((int)Handle.NumObjCTypeArgs), &TypeArgsFactory);
    }

    public Type BaseType => _baseType.GetValue(this);

    public ObjCInterfaceDecl Interface => _interface.GetValue(this);

    public bool IsKindOfType => Handle.IsObjCKindOfType;

    public bool IsKindOfTypeAsWritten => Handle.IsObjCKindOfTypeAsWritten;

    public bool IsSpecialized => Handle.IsObjCObjectSpecialized;

    public bool IsSpecializedAsWritten => Handle.IsObjCObjectSpecializedAsWritten;

    public IReadOnlyList<ObjCProtocolDecl> Protocols => _protocols;

    public Type SuperClassType => _superClassType.GetValue(this);

    public IReadOnlyList<Type> TypeArgs => _typeArgs;

    private static unsafe Type SuperClassTypeFactory(ObjCObjectType self) => self.TranslationUnit.GetOrCreate<Type>(self.Handle.UnderlyingType);

    private static unsafe ObjCInterfaceDecl InterfaceFactory(ObjCObjectType self) => self.TranslationUnit.GetOrCreate<ObjCInterfaceDecl>(self.Handle.Declaration);

    private static unsafe Type BaseTypeFactory(ObjCObjectType self) => self.TranslationUnit.GetOrCreate<Type>(self.Handle.ObjCObjectBaseType);

    private static unsafe ObjCProtocolDecl ProtocolsFactory(object self, int i)
    {
        var @this = (ObjCObjectType)self;
        return @this.TranslationUnit.GetOrCreate<ObjCProtocolDecl>(@this.Handle.GetObjCProtocolDecl(unchecked((uint)i)));
    }

    private static unsafe Type TypeArgsFactory(object self, int i)
    {
        var @this = (ObjCObjectType)self;
        return @this.TranslationUnit.GetOrCreate<Type>(@this.Handle.GetObjCTypeArg(unchecked((uint)i)));
    }
}
