using System.Collections.Generic;

namespace ClangSharp
{
    internal class RecordDecl : TagDecl
    {
        private readonly List<FieldDecl> _fieldDecls = new List<FieldDecl>();

        public RecordDecl(CXCursor handle, Cursor parent) : base(handle, parent)
        {
        }

        public bool IsClass => Kind == CXCursorKind.CXCursor_ClassDecl;

        public bool IsStruct => Kind == CXCursorKind.CXCursor_StructDecl;

        public bool IsUnion => Kind == CXCursorKind.CXCursor_UnionDecl;

        public IReadOnlyList<FieldDecl> FieldDecls => _fieldDecls;

        protected override CXChildVisitResult VisitChildren(CXCursor childHandle, CXCursor handle, CXClientData clientData)
        {
            ValidateVisit(ref handle);

            if (childHandle.IsDeclaration)
            {
                switch (childHandle.Kind)
                {
                    case CXCursorKind.CXCursor_FieldDecl:
                    {
                        var fieldDecl = GetOrAddChild<FieldDecl>(childHandle);
                        _fieldDecls.Add(fieldDecl);
                        return fieldDecl.Visit(clientData);
                    }
                }
            }

            return base.VisitChildren(childHandle, handle, clientData);
        }
    }
}
