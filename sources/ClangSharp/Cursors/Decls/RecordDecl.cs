// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using ClangSharp.Interop;

namespace ClangSharp
{
    public class RecordDecl : TagDecl
    {
        private readonly Lazy<IReadOnlyList<FieldDecl>> _fields;

        internal RecordDecl(CXCursor handle, CXCursorKind expectedKind) : base(handle, expectedKind)
        {
            _fields = new Lazy<IReadOnlyList<FieldDecl>>(() => Decls.OfType<FieldDecl>().ToList());
        }

        public bool IsAnonymousRecord => Handle.IsAnonymousRecordDecl;

        public IReadOnlyList<FieldDecl> Fields => _fields.Value;

        public bool IsUnion => Kind == CXCursorKind.CXCursor_UnionDecl;
    }
}
