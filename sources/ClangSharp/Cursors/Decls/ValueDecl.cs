// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using ClangSharp.Interop;
using static ClangSharp.Interop.CX_DeclKind;

namespace ClangSharp;

public class ValueDecl : NamedDecl
{
    private ValueLazy<ValueDecl, Type> _type;

    private protected unsafe ValueDecl(CXCursor handle, CXCursorKind expectedCursorKind, CX_DeclKind expectedDeclKind) : base(handle, expectedCursorKind, expectedDeclKind)
    {
        if (handle.DeclKind is > CX_DeclKind_LastValue or < CX_DeclKind_FirstValue)
        {
            throw new ArgumentOutOfRangeException(nameof(handle));
        }

        _type = new ValueLazy<ValueDecl, Type>(&TypeFactory);
    }

    public Type Type => _type.GetValue(this);

    private static unsafe Type TypeFactory(ValueDecl self) => self.TranslationUnit.GetOrCreate<Type>(self.Handle.Type);
}
