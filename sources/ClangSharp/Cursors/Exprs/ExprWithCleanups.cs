// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Collections.Generic;
using ClangSharp.Interop;
using static ClangSharp.Interop.CX_StmtClass;
using static ClangSharp.Interop.CXCursorKind;

namespace ClangSharp;

public sealed class ExprWithCleanups : FullExpr
{
    private readonly LazyList<Cursor> _objects;

    internal unsafe ExprWithCleanups(CXCursor handle) : base(handle, CXCursor_UnexposedExpr, CX_StmtClass_ExprWithCleanups)
    {
        _objects = LazyList.Create<Cursor>(this, Handle.NumArguments, &ObjectsFactory);
    }

    public uint NumObjects => unchecked((uint)Handle.NumArguments);

    public IReadOnlyList<Cursor> Objects => _objects;

    private static unsafe Cursor ObjectsFactory(object self, int i)
    {
        var @this = (ExprWithCleanups)self;
        return @this.TranslationUnit.GetOrCreate<Cursor>(@this.Handle.GetArgument(unchecked((uint)i)));
    }
}
