// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Collections.Generic;
using ClangSharp.Interop;
using static ClangSharp.Interop.CX_DeclKind;
using static ClangSharp.Interop.CXCursorKind;

namespace ClangSharp;

public sealed class DecompositionDecl : VarDecl
{
    private readonly LazyList<BindingDecl> _bindings;

    internal unsafe DecompositionDecl(CXCursor handle) : base(handle, CXCursor_UnexposedDecl, CX_DeclKind_Decomposition)
    {
        _bindings = LazyList.Create<BindingDecl>(this, Handle.NumBindings, &BindingsFactory);
    }

    public IReadOnlyList<BindingDecl> Bindings => _bindings;

    private static unsafe BindingDecl BindingsFactory(object self, int i)
    {
        var @this = (DecompositionDecl)self;
        return @this.TranslationUnit.GetOrCreate<BindingDecl>(@this.Handle.GetBindingDecl(unchecked((uint)i)));
    }
}
