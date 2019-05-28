using System.Collections.Generic;
using System.Diagnostics;

namespace ClangSharp
{
    internal sealed class TypedefDecl : TypedefNameDecl
    {
        private readonly List<ParmVarDecl> _parameters = new List<ParmVarDecl>();

        public TypedefDecl(CXCursor handle, Cursor parent) : base(handle, parent)
        {
            Debug.Assert(handle.Kind == CXCursorKind.CXCursor_TypedefDecl);
        }

        public IReadOnlyList<ParmVarDecl> Parameters => _parameters;

        protected override CXChildVisitResult VisitChildren(CXCursor childHandle, CXCursor handle, CXClientData clientData)
        {
            ValidateVisit(ref handle);

            if (childHandle.IsDeclaration)
            {
                switch (childHandle.Kind)
                {
                    case CXCursorKind.CXCursor_ParmDecl:
                    {
                        var parmDecl = GetOrAddChild<ParmVarDecl>(childHandle);
                        _parameters.Add(parmDecl);
                        return parmDecl.Visit(clientData);
                    }
                }
            }

            return base.VisitChildren(childHandle, handle, clientData);
        }
    }
}
