// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using ClangSharp.Interop;
using static ClangSharp.Interop.CX_DeclKind;

namespace ClangSharp;

public class TypeDecl : NamedDecl
{
    private ValueLazy<TypeDecl, Type> _typeForDecl;

    private protected unsafe TypeDecl(CXCursor handle, CXCursorKind expectedCursorKind, CX_DeclKind expectedDeclKind) : base(handle, expectedCursorKind, expectedDeclKind)
    {
        if (handle.DeclKind is > CX_DeclKind_LastType or < CX_DeclKind_FirstType)
        {
            throw new ArgumentOutOfRangeException(nameof(handle));
        }

        _typeForDecl = new ValueLazy<TypeDecl, Type>(&TypeForDeclFactory);
    }

    public Type TypeForDecl => _typeForDecl.GetValue(this);

    private static unsafe Type TypeForDeclFactory(TypeDecl self) => self.TranslationUnit.GetOrCreate<Type>(self.Handle.Type);
}
