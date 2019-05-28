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
                var decl = GetOrAddChild<Decl>(childHandle);

                if (decl is ParmVarDecl parmVarDecl)
                {
                    _parameters.Add(parmVarDecl);
                }

                return decl.Visit(clientData);
            }

            return base.VisitChildren(childHandle, handle, clientData);
        }
    }
}
