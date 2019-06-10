using System.Diagnostics;

namespace ClangSharp
{
    internal class Stmt : Cursor
    {
        public static new Stmt Create(CXCursor handle, Cursor parent)
        {
            if (handle.IsExpression)
            {
                return Expr.Create(handle, parent);
            }

            switch (handle.Kind)
            {
                case CXCursorKind.CXCursor_CompoundStmt:
                {
                    return new CompoundStmt(handle, parent);
                }

                case CXCursorKind.CXCursor_IfStmt:
                {
                    return new IfStmt(handle, parent);
                }

                case CXCursorKind.CXCursor_DoStmt:
                {
                    return new DoStmt(handle, parent);
                }

                case CXCursorKind.CXCursor_ForStmt:
                {
                    return new ForStmt(handle, parent);
                }

                case CXCursorKind.CXCursor_BreakStmt:
                {
                    return new BreakStmt(handle, parent);
                }

                case CXCursorKind.CXCursor_ReturnStmt:
                {
                    return new ReturnStmt(handle, parent);
                }

                case CXCursorKind.CXCursor_NullStmt:
                {
                    return new NullStmt(handle, parent);
                }

                case CXCursorKind.CXCursor_DeclStmt:
                {
                    return new DeclStmt(handle, parent);
                }

                default:
                {
                    Debug.WriteLine($"Unhandled statement kind: {handle.KindSpelling}.");
                    Debugger.Break();
                    return new Stmt(handle, parent);
                }
            }
        }

        protected Stmt(CXCursor handle, Cursor parent) : base(handle, parent)
        {
            Debug.Assert(handle.IsStatement || (this is ValueStmt));
        }

        protected unsafe override void ValidateVisit(ref CXCursor handle)
        {
            // Clang currently uses the PostChildrenVisitor which clears data0

            var modifiedHandle = Handle;
            modifiedHandle.data[0] = null;

            Debug.Assert(handle.Equals(modifiedHandle));
            handle = Handle;
        }
    }
}
