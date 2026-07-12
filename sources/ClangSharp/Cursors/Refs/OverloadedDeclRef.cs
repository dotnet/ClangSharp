// Copyright (c) .NET Foundation and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Collections.Generic;
using System.Linq;
using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;

namespace ClangSharp;

public sealed class OverloadedDeclRef : Ref
{
    private ValueLazy<OverloadedDeclRef, IEnumerable<Decl>> _overloadedDecls;

    internal unsafe OverloadedDeclRef(CXCursor handle) : base(handle, CXCursor_OverloadedDeclRef)
    {
        _overloadedDecls = new ValueLazy<OverloadedDeclRef, IEnumerable<Decl>>(&OverloadedDeclsFactory);
    }

    public IEnumerable<Decl> OverloadedDecls => _overloadedDecls.GetValue(this);

    private static unsafe IEnumerable<Decl> OverloadedDeclsFactory(OverloadedDeclRef self) {
            var num = self.Handle.NumOverloadedDecls;
            return [.. Enumerable.Range(0, (int)num)
                .Select(i => self.Handle.GetOverloadedDecl((uint)i))
                .Select(c => self.TranslationUnit.GetOrCreate<Decl>(c))];
        }
}
