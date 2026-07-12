// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using ClangSharp.Interop;
using static ClangSharp.Interop.CX_DeclKind;

namespace ClangSharp;

public class TypedefNameDecl : TypeDecl, IRedeclarable<TypedefNameDecl>
{
    private ValueLazy<TypedefNameDecl, Type> _underlyingType;

    private protected unsafe TypedefNameDecl(CXCursor handle, CXCursorKind expectedCursorKind, CX_DeclKind expectedDeclKind) : base(handle, expectedCursorKind, expectedDeclKind)
    {
        if (handle.DeclKind is > CX_DeclKind_LastTypedefName or < CX_DeclKind_FirstTypedefName)
        {
            throw new ArgumentOutOfRangeException(nameof(handle));
        }

        _underlyingType = new ValueLazy<TypedefNameDecl, Type>(&UnderlyingTypeFactory);
    }

    public new TypedefNameDecl CanonicalDecl => (TypedefNameDecl)base.CanonicalDecl;

    public bool IsTransparentTag => Handle.IsTransparent;

    public Type UnderlyingType => _underlyingType.GetValue(this);

    private static unsafe Type UnderlyingTypeFactory(TypedefNameDecl self) => self.TranslationUnit.GetOrCreate<Type>(self.Handle.TypedefDeclUnderlyingType);
}
