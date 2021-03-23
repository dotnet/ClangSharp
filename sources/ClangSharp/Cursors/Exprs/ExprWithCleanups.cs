// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using System.Collections.Generic;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class ExprWithCleanups : FullExpr
    {
        private readonly Lazy<IReadOnlyList<Cursor>> _objects;

        internal ExprWithCleanups(CXCursor handle) : base(handle, CXCursorKind.CXCursor_UnexposedExpr, CX_StmtClass.CX_StmtClass_ExprWithCleanups)
        {
            _objects = new Lazy<IReadOnlyList<Cursor>>(() => {
                var numObjects = Handle.NumArguments;
                var objects = new List<Cursor>(numObjects);

                for (var i = 0; i < numObjects; i++)
                {
                    var obj = TranslationUnit.GetOrCreate<Cursor>(Handle.GetArgument(unchecked((uint)i)));
                    objects.Add(obj);
                }

                return objects;
            });
        }

        public uint NumObjects => unchecked((uint)Handle.NumArguments);

        public IReadOnlyList<Cursor> Objects => _objects.Value;
    }
}
