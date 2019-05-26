using System.Collections.Generic;
using System.Diagnostics;

namespace ClangSharp
{
    internal sealed class FunctionDecl : Decl
    {
        private readonly List<ParmDecl> _parmDecls;

        private DLLExport _dllExport;
        private DLLImport _dllImport;

        public FunctionDecl(CXCursor handle, Cursor parent) : base(handle, parent)
        {
            Debug.Assert(handle.Kind == CXCursorKind.CXCursor_FunctionDecl);
            _parmDecls = new List<ParmDecl>();
        }

        public DLLExport DLLExport
        {
            get
            {
                return _dllExport;
            }

            set
            {
                Debug.Assert((_dllExport is null) && (_dllImport is null));
                _dllExport = value;
            }
        }

        public DLLImport DLLImport
        {
            get
            {
                return _dllImport;
            }

            set
            {
                Debug.Assert((_dllExport is null) && (_dllImport is null));
                _dllImport = value;
            }
        }

        public IReadOnlyList<ParmDecl> ParmDecls => _parmDecls;

        protected override CXChildVisitResult VisitChildren(CXCursor childHandle, CXCursor handle, CXClientData clientData)
        {
            ValidateVisit(ref handle);

            switch (childHandle.Kind)
            {
                case CXCursorKind.CXCursor_ParmDecl:
                {
                    var parmDecl = GetOrAddChild<ParmDecl>(childHandle);
                    parmDecl.Index = _parmDecls.Count;
                    _parmDecls.Add(parmDecl);
                    return parmDecl.Visit(clientData);
                }

                case CXCursorKind.CXCursor_TypeRef:
                {
                    return GetOrAddChild<TypeRef>(childHandle).Visit(clientData);
                }

                case CXCursorKind.CXCursor_NamespaceRef:
                {
                    return GetOrAddChild<NamespaceRef>(childHandle).Visit(clientData);
                }

                case CXCursorKind.CXCursor_CompoundStmt:
                {
                    return GetOrAddChild<CompoundStmt>(childHandle).Visit(clientData);
                }

                case CXCursorKind.CXCursor_UnexposedAttr:
                {
                    return GetOrAddChild<UnexposedAttr>(childHandle).Visit(clientData);
                }

                case CXCursorKind.CXCursor_PureAttr:
                {
                    return GetOrAddChild<PureAttr>(childHandle).Visit(clientData);
                }

                case CXCursorKind.CXCursor_ConstAttr:
                {
                    return GetOrAddChild<ConstAttr>(childHandle).Visit(clientData);
                }

                case CXCursorKind.CXCursor_VisibilityAttr:
                {
                    return GetOrAddChild<VisibilityAttr>(childHandle).Visit(clientData);
                }

                case CXCursorKind.CXCursor_DLLExport:
                {
                    var dllExport = GetOrAddChild<DLLExport>(childHandle);
                    DLLExport = dllExport;
                    return dllExport.Visit(clientData);
                }

                case CXCursorKind.CXCursor_DLLImport:
                {
                    var dllImport = GetOrAddChild<DLLImport>(childHandle);
                    DLLImport = dllImport;
                    return dllImport.Visit(clientData);
                }

                default:
                {
                    return base.VisitChildren(childHandle, handle, clientData);
                }
            }
        }
    }
}
