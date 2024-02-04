// Copyright (c) .NET Foundation and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;

namespace ClangSharp;

public sealed class OverloadedDeclRef : Ref
{
    private readonly Lazy<IEnumerable<Decl>> _overloadedDecls;

    internal OverloadedDeclRef(CXCursor handle) : base(handle, CXCursor_OverloadedDeclRef)
    {
        _overloadedDecls = new Lazy<IEnumerable<Decl>>(() => {
            uint num = Handle.NumOverloadedDecls;
            return Enumerable.Range(0, (int)num)
                .Select(i => Handle.GetOverloadedDecl((uint)i))
                .Select(c => TranslationUnit.GetOrCreate<Decl>(c))
                .ToArray();
        });
    }

    public IEnumerable<Decl> OverloadedDecls => _overloadedDecls.Value;
}
