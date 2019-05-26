using System.Collections.Generic;
using System.Diagnostics;

namespace ClangSharp
{
    internal sealed class FunctionDecl : Decl
    {
        private readonly List<ParmDecl> _parmDecls;

        private bool _isDllExport;
        private bool _isDllImport;

        public FunctionDecl(CXCursor handle, Cursor parent) : base(handle, parent)
        {
            Debug.Assert(handle.Kind == CXCursorKind.CXCursor_FunctionDecl);
            _parmDecls = new List<ParmDecl>();
        }

        public bool HasDllExport => _isDllExport;

        public bool HasDllImport => _isDllImport;

        public IReadOnlyList<ParmDecl> ParmDecls => _parmDecls;

        protected override CXChildVisitResult VisitChildren(CXCursor childHandle, CXCursor handle, CXClientData clientData)
        {
            ValidateVisit(ref handle);

            if (childHandle.IsDeclaration)
            {
                switch (childHandle.Kind)
                {
                    case CXCursorKind.CXCursor_ParmDecl:
                    {
                        var parmDecl = GetOrAddChild<ParmDecl>(childHandle);
                        parmDecl.Index = _parmDecls.Count;
                        _parmDecls.Add(parmDecl);
                        return parmDecl.Visit(clientData);
                    }
                }
            }
            else if (childHandle.IsAttribute)
            {
                switch (childHandle.Kind)
                {
                    case CXCursorKind.CXCursor_DLLExport:
                    {
                        _isDllExport = true;
                        break;
                    }

                    case CXCursorKind.CXCursor_DLLImport:
                    {
                        _isDllImport = true;
                        break;
                    }
                }
            }

            return base.VisitChildren(childHandle, handle, clientData);
        }
    }
}
