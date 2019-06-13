using System;
using System.Collections.Generic;
using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp
{
    internal sealed class TranslationUnit : Cursor
    {
        private readonly List<Decl> _declarations = new List<Decl>();
        private readonly Dictionary<CXCursor, Cursor> _visitedCursors = new Dictionary<CXCursor, Cursor>();
        private readonly Dictionary<CXType, Type> _visitedTypes = new Dictionary<CXType, Type>();

        public TranslationUnit(CXCursor handle) : base(handle, parent: null)
        {
            Debug.Assert(handle.Kind == CXCursorKind.CXCursor_TranslationUnit);
        }

        public IReadOnlyList<Decl> Declarations => _declarations;

        protected override Decl GetOrAddDecl(CXCursor childHandle)
        {
            var decl = base.GetOrAddDecl(childHandle);
            _declarations.Add(decl);
            return decl;
        }

        internal void AddVisitedCursor(Cursor cursor)
        {
            if (_visitedCursors.ContainsKey(cursor.Handle))
            {
                Debug.WriteLine("Attempting to add an already visited cursor.");
                Debugger.Break();
            }
            _visitedCursors.Add(cursor.Handle, cursor);
        }

        internal void AddVisitedType(Type type)
        {
            if (_visitedTypes.ContainsKey(type.Handle))
            {
                Debug.WriteLine("Attempting to add an already visited type.");
                Debugger.Break();
            }
            _visitedTypes.Add(type.Handle, type);
        }

        internal Cursor GetOrCreateCursor(CXCursor childHandle, Func<Cursor> createCursor)
        {
            if (childHandle.IsNull || (childHandle.Kind == CXCursorKind.CXCursor_NoDeclFound))
            {
                return null;
            }

            if (!_visitedCursors.TryGetValue(childHandle, out var childCursor))
            {
                childCursor = createCursor();
                Debug.Assert(_visitedCursors.ContainsKey(childHandle));
            }

            return childCursor;
        }

        internal Type GetOrCreateType(CXType handle, Func<Type> createType)
        {
            if (handle.kind == CXTypeKind.CXType_Invalid)
            {
                return null;
            }

            if (!_visitedTypes.TryGetValue(handle, out var type))
            {
                type = createType();
                Debug.Assert(_visitedTypes.ContainsKey(handle));
            }

            return type;
        }
    }
}
