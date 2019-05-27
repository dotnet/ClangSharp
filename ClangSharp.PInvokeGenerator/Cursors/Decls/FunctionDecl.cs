using System.Collections.Generic;
using System.Diagnostics;

namespace ClangSharp
{
    internal class FunctionDecl : DeclaratorDecl
    {
        private readonly List<ParmVarDecl> _parmDecls = new List<ParmVarDecl>();

        private bool _isDllExport;
        private bool _isDllImport;

        public FunctionDecl(CXCursor handle, Cursor parent) : base(handle, parent)
        {
        }

        public bool HasDllExport => _isDllExport;

        public bool HasDllImport => _isDllImport;

        public bool IsInlined => Handle.IsFunctionInlined;

        public bool IsVariadic => Handle.IsVariadic;

        public IReadOnlyList<ParmVarDecl> ParmDecls => _parmDecls;

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
