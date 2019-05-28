using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ClangSharp
{
    internal class Cursor
    {
        public static Cursor Create(CXCursor handle, Cursor parent)
        {
            if (handle.IsDeclaration)
            {
                return Decl.Create(handle, parent);
            }
            else if (handle.IsReference)
            {
                return Ref.Create(handle, parent);
            }
            else if (handle.IsExpression)
            {
                return Expr.Create(handle, parent);
            }
            else if (handle.IsStatement)
            {
                return Stmt.Create(handle, parent);
            }
            else if (handle.IsAttribute)
            {
                return Attr.Create(handle, parent);
            }

            switch (handle.Kind)
            {
                default:
                {
                    Debug.WriteLine($"Unhandled cursor kind: {handle.KindSpelling}.");
                    Debugger.Break();
                    return new Cursor(handle, parent);
                }
            }
        }

        private readonly List<Cursor> _children;
        private bool _visited;

        protected Cursor(CXCursor handle, Cursor parent)
        {
            Debug.Assert(!handle.IsNull);

            _children = new List<Cursor>();

            Handle = handle;
            Parent = parent;

            if (parent is null)
            {
                Debug.Assert(this is TranslationUnit);
                TranslationUnit = this as TranslationUnit;
            }
            else
            {
                Debug.Assert(parent.TranslationUnit != null);
                TranslationUnit = parent.TranslationUnit;
            }
            TranslationUnit.AddVisitedCursor(this);
        }

        public IReadOnlyList<Cursor> Children => _children;

        public CXSourceRange Extent => Handle.Extent;

        public CXCursor Handle { get; }

        public bool IsFromMainFile => Location.IsFromMainFile;

        public CXCursorKind Kind => Handle.Kind;

        public string KindSpelling => Handle.KindSpelling.ToString();

        public CXSourceLocation Location => Handle.Location;

        public Cursor Parent { get; }

        public string Spelling => Handle.Spelling.ToString();

        public TranslationUnit TranslationUnit { get; }

        public CXToken[] Tokenize(Cursor cursor)
        {
            Handle.TranslationUnit.Tokenize(cursor.Extent, out CXToken[] tokens);
            return tokens;
        }

        public CXChildVisitResult Visit(CXClientData clientData)
        {
            if (!_visited)
            {
                _visited = true;
                Handle.VisitChildren(VisitChildren, clientData);
            }

            // We always return CXChildVisit_Continue since some clang will return
            // CXChildVisit_Break for some calls, such as if they have no children.

            return CXChildVisitResult.CXChildVisit_Continue;
        }

        protected TCursor GetChild<TCursor>(CXCursor childHandle)
            where TCursor : Cursor
        {
            return (TCursor)_children.Where((childCursor) => childCursor.Handle.Equals(childHandle)).Single();
        }

        protected TCursor GetOrAddChild<TCursor>(CXCursor childHandle)
            where TCursor : Cursor
        {
            var childCursor = TranslationUnit.GetOrCreateCursor(childHandle, () => Create(childHandle, this));
            _children.Add(childCursor);
            return (TCursor)childCursor;
        }

        protected virtual CXChildVisitResult VisitChildren(CXCursor childHandle, CXCursor handle, CXClientData clientData)
        {
            ValidateVisit(ref handle);

            switch (childHandle.Kind)
            {
                default:
                {
                    return GetOrAddChild<Cursor>(childHandle).Visit(clientData);
                }
            }
        }

        protected virtual void ValidateVisit(ref CXCursor handle)
        {
            Debug.Assert(handle.Equals(Handle));
        }
    }
}
