using System.Diagnostics;

namespace ClangSharp
{
    internal sealed class FriendDecl : Decl
    {
        public FriendDecl(CXCursor handle, Cursor parent) : base(handle, parent)
        {
            Debug.Assert(handle.Kind == CXCursorKind.CXCursor_FriendDecl);
        }

        protected override CXChildVisitResult VisitChildren(CXCursor childHandle, CXCursor handle, CXClientData clientData)
        {
            ValidateVisit(ref handle);

            switch (childHandle.Kind)
            {
                case CXCursorKind.CXCursor_FunctionDecl:
                {
                    return GetOrAddChild<FunctionDecl>(childHandle).Visit(clientData);
                }

                case CXCursorKind.CXCursor_FunctionTemplate:
                {
                    return GetOrAddChild<FunctionTemplate>(childHandle).Visit(clientData);
                }

                case CXCursorKind.CXCursor_ClassTemplate:
                {
                    return GetOrAddChild<ClassTemplate>(childHandle).Visit(clientData);
                }

                case CXCursorKind.CXCursor_TypeRef:
                {
                    return GetOrAddChild<TypeRef>(childHandle).Visit(clientData);
                }

                case CXCursorKind.CXCursor_TemplateRef:
                {
                    return GetOrAddChild<TemplateRef>(childHandle).Visit(clientData);
                }

                default:
                {
                    return base.VisitChildren(childHandle, handle, clientData);
                }
            }
        }
    }
}
