// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp
{
    [DebuggerDisplay("{Handle.DebuggerDisplayString,nq}")]
    public unsafe class Cursor : IEquatable<Cursor>
    {
        private readonly Lazy<IReadOnlyList<Cursor>> _cursorChildren;
        private readonly Lazy<TranslationUnit> _translationUnit;

        private protected Cursor(CXCursor handle, CXCursorKind expectedCursorKind)
        {
            if (handle.kind != expectedCursorKind)
            {
                throw new ArgumentException(nameof(handle));
            }
            Handle = handle;

            _cursorChildren = new Lazy<IReadOnlyList<Cursor>>(() => {
                var cursors = new List<Cursor>();

                Handle.VisitChildren((cursor, parent, clientData) => {
                    var cursorChild = TranslationUnit.GetOrCreate<Cursor>(cursor);
                    cursors.Add(cursorChild);
                    return CXChildVisitResult.CXChildVisit_Continue;
                }, clientData: default);

                return cursors;
            });

            _translationUnit = new Lazy<TranslationUnit>(() => TranslationUnit.GetOrCreate(Handle.TranslationUnit));
        }

        public IReadOnlyList<Cursor> CursorChildren => _cursorChildren.Value;

        public CXCursorKind CursorKind => Handle.Kind;

        public string CursorKindSpelling => Handle.KindSpelling.ToString();

        public CXSourceRange Extent => Handle.Extent;

        public CXCursor Handle { get; }

        public CXSourceLocation Location => Handle.Location;

        public string Spelling => Handle.Spelling.ToString();

        public TranslationUnit TranslationUnit => _translationUnit.Value;

        public static bool operator ==(Cursor left, Cursor right) => (left is object) ? ((right is object) && (left.Handle == right.Handle)) : (right is null);

        public static bool operator !=(Cursor left, Cursor right) => (left is object) ? ((right is null) || (left.Handle != right.Handle)) : (right is object);

        internal static Cursor Create(CXCursor handle)
        {
            Cursor result;

            if (handle.IsDeclaration || handle.IsTranslationUnit)
            {
                result = Decl.Create(handle);
            }
            else if (handle.IsReference)
            {
                result = Ref.Create(handle);
            }
            else if (handle.IsExpression || handle.IsStatement)
            {
                result = Stmt.Create(handle);
            }
            else if (handle.IsAttribute)
            {
                result = Attr.Create(handle);
            }
            else if (handle.IsPreprocessing)
            {
                result = PreprocessedEntity.Create(handle);
            }
            else
            {
                Debug.WriteLine($"Unhandled cursor kind: {handle.KindSpelling}.");
                result = new Cursor(handle, handle.Kind);
            }

            return result;
        }

        public override bool Equals(object obj) => (obj is Cursor other) && Equals(other);

        public bool Equals(Cursor other) => this == other;

        public override int GetHashCode() => Handle.GetHashCode();

        public override string ToString() => Handle.ToString();
    }
}
