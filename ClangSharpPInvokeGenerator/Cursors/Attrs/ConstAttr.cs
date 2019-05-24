using System.Diagnostics;
using ClangSharp;

namespace ClangSharpPInvokeGenerator
{
    internal sealed class ConstAttr : Attr
    {
        public ConstAttr(CXCursor handle, Cursor parent) : base(handle, parent)
        {
            Debug.Assert(handle.Kind == CXCursorKind.CXCursor_ConstAttr);
        }

        protected override CXChildVisitResult VisitChildren(CXCursor childHandle, CXCursor handle, CXClientData clientData)
        {
            ValidateVisit(ref handle);

            switch (childHandle.Kind)
            {
                default:
                {
                    return base.VisitChildren(childHandle, handle, clientData);
                }
            }
        }
    }
}
