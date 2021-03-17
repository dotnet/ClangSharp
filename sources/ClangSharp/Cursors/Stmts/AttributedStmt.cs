// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class AttributedStmt : ValueStmt
    {
        private readonly Lazy<IReadOnlyList<Attr>> _attrs;

        internal AttributedStmt(CXCursor handle) : base(handle, CXCursorKind.CXCursor_UnexposedStmt, CX_StmtClass.CX_StmtClass_AttributedStmt)
        {
            Debug.Assert(NumChildren == 1);

            _attrs = new Lazy<IReadOnlyList<Attr>>(() => {
                var numAttrs = Handle.NumAttrs;
                var attrs = new List<Attr>(numAttrs);

                for (var i = 0; i < numAttrs; i++)
                {
                    var attr = TranslationUnit.GetOrCreate<Attr>(Handle.GetAttr(unchecked((uint)i)));
                    attrs.Add(attr);
                }

                return attrs;
            });
        }

        public IReadOnlyList<Attr> Attrs => _attrs.Value;

        public Stmt SubStmt => Children[0];
    }
}
