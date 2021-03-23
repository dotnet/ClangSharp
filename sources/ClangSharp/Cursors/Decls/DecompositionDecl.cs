// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using System.Collections.Generic;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class DecompositionDecl : VarDecl
    {
        private readonly Lazy<IReadOnlyList<BindingDecl>> _bindings;

        internal DecompositionDecl(CXCursor handle) : base(handle, CXCursorKind.CXCursor_UnexposedDecl, CX_DeclKind.CX_DeclKind_Decomposition)
        {
            _bindings = new Lazy<IReadOnlyList<BindingDecl>>(() => {
                var numBindings = Handle.NumBindings;
                var bindings = new List<BindingDecl>(numBindings);

                for (var i = 0; i < numBindings; i++)
                {
                    var binding = TranslationUnit.GetOrCreate<BindingDecl>(Handle.GetBindingDecl(unchecked((uint)i)));
                    bindings.Add(binding);
                }

                return bindings;
            });
        }

        public IReadOnlyList<BindingDecl> Bindings => _bindings.Value;
    }
}
