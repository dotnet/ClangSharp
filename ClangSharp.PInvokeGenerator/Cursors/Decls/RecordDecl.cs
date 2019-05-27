using System.Collections.Generic;

namespace ClangSharp
{
    internal class RecordDecl : TagDecl
    {
        private readonly List<FieldDecl> _fieldDecls = new List<FieldDecl>();

        protected RecordDecl(CXCursor handle, Cursor parent) : base(handle, parent)
        {
        }

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
