// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Diagnostics;
using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CX_StmtClass;

namespace ClangSharp;

public sealed class ObjCEncodeExpr : Expr
{
    private ValueLazy<ObjCEncodeExpr, Type> _encodedType;

    internal unsafe ObjCEncodeExpr(CXCursor handle) : base(handle, CXCursor_ObjCEncodeExpr, CX_StmtClass_ObjCEncodeExpr)
    {
        Debug.Assert(NumChildren is 0);
        _encodedType = new ValueLazy<ObjCEncodeExpr, Type>(&EncodedTypeFactory);
    }

    public Type EncodedType => _encodedType.GetValue(this);

    private static unsafe Type EncodedTypeFactory(ObjCEncodeExpr self) => self.TranslationUnit.GetOrCreate<Type>(self.Handle.TypeOperand);
}
