using System.Diagnostics;

namespace ClangSharp
{
    internal sealed class ConversionFunction : Decl
    {
        public ConversionFunction(CXCursor handle, Cursor parent) : base(handle, parent)
        {
            Debug.Assert(handle.Kind == CXCursorKind.CXCursor_ConversionFunction);
        }

        protected override CXChildVisitResult VisitChildren(CXCursor childHandle, CXCursor handle, CXClientData clientData)
        {
            ValidateVisit(ref handle);

            switch (childHandle.Kind)
            {
                case CXCursorKind.CXCursor_TypeRef:
                {
                    return GetOrAddChild<TypeRef>(childHandle).Visit(clientData);
                }

                case CXCursorKind.CXCursor_CompoundStmt:
                {
                    return GetOrAddChild<CompoundStmt>(childHandle).Visit(clientData);
                }

                case CXCursorKind.CXCursor_DLLExport:
                {
                    return GetOrAddChild<DLLExport>(childHandle).Visit(clientData);
                }

                case CXCursorKind.CXCursor_DLLImport:
                {
                    return GetOrAddChild<DLLImport>(childHandle).Visit(clientData);
                }

                default:
                {
                    return base.VisitChildren(childHandle, handle, clientData);
                }
            }
        }
    }
}
