using System.Diagnostics;

namespace ClangSharp
{
    internal sealed class TemplateTemplateParameter : Decl
    {
        public TemplateTemplateParameter(CXCursor handle, Cursor parent) : base(handle, parent)
        {
            Debug.Assert(handle.Kind == CXCursorKind.CXCursor_TemplateTemplateParameter);
        }

        protected override CXChildVisitResult VisitChildren(CXCursor childHandle, CXCursor handle, CXClientData clientData)
        {
            ValidateVisit(ref handle);

            switch (childHandle.Kind)
            {
                case CXCursorKind.CXCursor_TemplateTypeParameter:
                {
                    return GetOrAddChild<TemplateTypeParameter>(childHandle).Visit(clientData);
                }

                default:
                {
                    return base.VisitChildren(childHandle, handle, clientData);
                }
            }
        }
    }
}
