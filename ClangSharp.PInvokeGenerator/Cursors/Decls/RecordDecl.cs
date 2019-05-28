using System.Collections.Generic;

namespace ClangSharp
{
    internal class RecordDecl : TagDecl
    {
        private readonly List<FieldDecl> _fields = new List<FieldDecl>();

        public RecordDecl(CXCursor handle, Cursor parent) : base(handle, parent)
        {
        }

        public bool IsClass => Kind == CXCursorKind.CXCursor_ClassDecl;

        public bool IsStruct => Kind == CXCursorKind.CXCursor_StructDecl;

        public bool IsUnion => Kind == CXCursorKind.CXCursor_UnionDecl;

        public IReadOnlyList<FieldDecl> Fields => _fields;

        protected override Decl GetOrAddDecl(CXCursor childHandle)
        {
            var decl = base.GetOrAddDecl(childHandle);

            if (decl is FieldDecl fieldDecl)
            {
                _fields.Add(fieldDecl);
            }

            return decl;
        }
    }
}
