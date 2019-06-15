using System;
using System.Collections.Generic;
using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp
{
    public unsafe class Cursor
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

        private readonly List<Cursor> _children = new List<Cursor>();
        private bool _visited;

        protected Cursor(CXCursor handle, Cursor parent)
        {
            Debug.Assert(!handle.IsNull);

            Handle = handle;
            Parent = parent;

            if (parent is null)
            {
                Debug.Assert(this is TranslationUnitDecl);
                TranslationUnit = this as TranslationUnitDecl;
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

        public TranslationUnitDecl TranslationUnit { get; }

        public Span<CXToken> Tokenize(Cursor cursor)
        {
            return Handle.TranslationUnit.Tokenize(cursor.Extent);
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

        protected virtual Attr GetOrAddAttr(CXCursor childHandle)
        {
            Debug.Assert(childHandle.IsAttribute);
            return (Attr)GetOrAddCursor(childHandle);
        }

        protected virtual Cursor GetOrAddCursor(CXCursor childHandle)
        {
            var childCursor = TranslationUnit.GetOrCreateCursor(childHandle, () => Create(childHandle, this));
            _children.Add(childCursor);
            return childCursor;
        }

        protected virtual Decl GetOrAddDecl(CXCursor childHandle)
        {
            Debug.Assert(childHandle.IsDeclaration);
            return (Decl)GetOrAddCursor(childHandle);
        }

        protected virtual Expr GetOrAddExpr(CXCursor childHandle)
        {
            Debug.Assert(childHandle.IsExpression);
            return (Expr)GetOrAddCursor(childHandle);
        }

        protected virtual Ref GetOrAddRef(CXCursor childHandle)
        {
            Debug.Assert(childHandle.IsReference);
            return (Ref)GetOrAddCursor(childHandle);
        }

        protected virtual Stmt GetOrAddStmt(CXCursor childHandle)
        {
            if (childHandle.IsExpression)
            {
                return GetOrAddExpr(childHandle);
            }

            Debug.Assert(childHandle.IsStatement);
            return (Stmt)GetOrAddCursor(childHandle);
        }

        protected virtual void ValidateVisit(ref CXCursor handle)
        {
            Debug.Assert(handle.Equals(Handle));
        }

        private unsafe CXChildVisitResult VisitChildren(CXCursor childHandle, CXCursor handle, void* clientData)
        {
            return VisitChildren(childHandle, handle, (CXClientData)clientData);
        }

        private CXChildVisitResult VisitChildren(CXCursor childHandle, CXCursor handle, CXClientData clientData)
        {
            ValidateVisit(ref handle);

            if (childHandle.IsDeclaration)
            {
                return GetOrAddDecl(childHandle).Visit(clientData);
            }
            else if (childHandle.IsReference)
            {
                return GetOrAddRef(childHandle).Visit(clientData);
            }
            else if (childHandle.IsExpression)
            {
                return GetOrAddExpr(childHandle).Visit(clientData);
            }
            else if (childHandle.IsStatement)
            {
                return GetOrAddStmt(childHandle).Visit(clientData);
            }
            else if (childHandle.IsAttribute)
            {
                return GetOrAddAttr(childHandle).Visit(clientData);
            }
            else
            {
                return GetOrAddCursor(childHandle).Visit(clientData);
            }
        }
    }
}
