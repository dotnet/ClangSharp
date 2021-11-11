// Copyright (c) .NET Foundation and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class ObjCDictionaryLiteral : Expr
    {
        private readonly Lazy<ObjCMethodDecl> _dictWithObjectsMethod;
        private readonly Lazy<IReadOnlyList<(Expr Key, Expr Value)>> _keyValueElements;

        internal ObjCDictionaryLiteral(CXCursor handle) : base(handle, CXCursorKind.CXCursor_UnexposedExpr, CX_StmtClass.CX_StmtClass_ObjCDictionaryLiteral)
        {
            Debug.Assert((NumChildren % 2) == 0);
            _dictWithObjectsMethod = new Lazy<ObjCMethodDecl>(() => TranslationUnit.GetOrCreate<ObjCMethodDecl>(Handle.Referenced));

            _keyValueElements = new Lazy<IReadOnlyList<(Expr Key, Expr Value)>>(() => {
                var numChildren = Handle.NumChildren;
                var keyValueElements = new List<(Expr Key, Expr Value)>(numChildren / 2);

                for (var i = 0; i < numChildren; i += 2)
                {
                    keyValueElements.Add(((Expr)Children[i + 0], (Expr)Children[i + 1]));
                }

                return keyValueElements;
            });
        }

        public ObjCMethodDecl DictWithObjectsMethod => _dictWithObjectsMethod.Value;

        public IReadOnlyList<(Expr Key, Expr Value)> KeyValueElements => _keyValueElements.Value;

        public uint NumElements => NumChildren / 2;
    }
}
