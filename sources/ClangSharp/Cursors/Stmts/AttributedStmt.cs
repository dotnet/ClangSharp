// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Collections.Generic;
using System.Diagnostics;
using ClangSharp.Interop;
using static ClangSharp.Interop.CX_StmtClass;
using static ClangSharp.Interop.CXCursorKind;

namespace ClangSharp;

public sealed class AttributedStmt : ValueStmt
{
    private readonly LazyList<Attr> _attrs;

    internal AttributedStmt(CXCursor handle) : base(handle, CXCursor_UnexposedStmt, CX_StmtClass_AttributedStmt)
    {
        Debug.Assert(NumChildren == 1);

        _attrs = LazyList.Create<Attr>(Handle.NumAttrs, (i) => TranslationUnit.GetOrCreate<Attr>(Handle.GetAttr(unchecked((uint)i))));
    }

    public IReadOnlyList<Attr> Attrs => _attrs;

    public Stmt SubStmt => Children[0];
}
