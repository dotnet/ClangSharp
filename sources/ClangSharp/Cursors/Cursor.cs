using System;
using System.Collections.Generic;
using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp
{
    public unsafe class Cursor : IEquatable<Cursor>
    {
        private readonly Lazy<List<Cursor>> _cursorChildren;
        private readonly Lazy<TranslationUnit> _translationUnit;

        private protected Cursor(CXCursor handle, CXCursorKind expectedKind)
        {
            if (handle.kind != expectedKind)
            {
                throw new ArgumentException(nameof(handle));
            }
            Handle = handle;

            _cursorChildren = new Lazy<List<Cursor>>(() => {
                var cursors = new List<Cursor>();

                Handle.VisitChildren((cursor, parent, clientData) => {
                    var cursorChild = TranslationUnit.GetOrCreate<Cursor>(cursor);
                    cursorChild.CursorParent = this;

                    cursors.Add(cursorChild);
                    return CXChildVisitResult.CXChildVisit_Continue;
                }, clientData: default);

                return cursors;
            });
            _translationUnit = new Lazy<TranslationUnit>(() => TranslationUnit.GetOrCreate(Handle.TranslationUnit));
        }

        public IReadOnlyList<Cursor> CursorChildren => _cursorChildren.Value;

        public Cursor CursorParent { get; private set; }

        public CXSourceRange Extent => Handle.Extent;

        public CXCursor Handle { get; }

        public CXCursorKind Kind => Handle.Kind;

        public string KindSpelling => Handle.KindSpelling.ToString();

        public CXSourceLocation Location => Handle.Location;

        public string Spelling => Handle.Spelling.ToString();

        public TranslationUnit TranslationUnit => _translationUnit.Value;

        public static bool operator ==(Cursor left, Cursor right) => (left is object) ? ((right is object) && (left.Handle == right.Handle)) : (right is null);

        public static bool operator !=(Cursor left, Cursor right) => (left is object) ? ((right is null) || (left.Handle != right.Handle)) : (right is object);

        internal static Cursor Create(CXCursor handle)
        {
            Cursor result;

            if (handle.IsDeclaration)
            {
                result = Decl.Create(handle);
            }
            else if (handle.IsReference)
            {
                result = Ref.Create(handle);
            }
            else if (handle.IsExpression)
            {
                result = Expr.Create(handle);
            }
            else if (handle.IsStatement)
            {
                result = Stmt.Create(handle);
            }
            else if (handle.IsAttribute)
            {
                result = Attr.Create(handle);
            }
            else
            {
                Debug.WriteLine($"Unhandled cursor kind: {handle.KindSpelling}.");
                Debugger.Break();

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
