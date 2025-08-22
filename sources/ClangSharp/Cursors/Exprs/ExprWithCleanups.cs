// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Collections.Generic;
using ClangSharp.Interop;
using static ClangSharp.Interop.CX_StmtClass;
using static ClangSharp.Interop.CXCursorKind;

namespace ClangSharp;

public sealed class ExprWithCleanups : FullExpr
{
    private readonly LazyList<Cursor> _objects;

    internal ExprWithCleanups(CXCursor handle) : base(handle, CXCursor_UnexposedExpr, CX_StmtClass_ExprWithCleanups)
    {
        _objects = LazyList.Create<Cursor>(Handle.NumArguments, (i) => TranslationUnit.GetOrCreate<Cursor>(Handle.GetArgument(unchecked((uint)i))));
    }

    public uint NumObjects => unchecked((uint)Handle.NumArguments);

    public IReadOnlyList<Cursor> Objects => _objects;
}
