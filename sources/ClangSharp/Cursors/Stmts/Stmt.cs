using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ClangSharp.Interop;

namespace ClangSharp
{
    public class Stmt : Cursor
    {
        private readonly Lazy<IReadOnlyList<Stmt>> _children;

        private protected Stmt(CXCursor handle, CXCursorKind expectedKind) : base(handle, expectedKind)
        {
            _children = new Lazy<IReadOnlyList<Stmt>>(() => CursorChildren.Where((cursor) => cursor is Stmt).Cast<Stmt>().ToList());
        }

        public IReadOnlyList<Stmt> Children => _children.Value;

        internal static new Stmt Create(CXCursor handle)
        {
            Stmt result;

            switch (handle.Kind)
            {
                case CXCursorKind.CXCursor_CompoundStmt:
                {
                    result = new CompoundStmt(handle);
                    break;
                }

                case CXCursorKind.CXCursor_IfStmt:
                {
                    result = new IfStmt(handle);
                    break;
                }

                case CXCursorKind.CXCursor_DoStmt:
                {
                    result = new DoStmt(handle);
                    break;
                }

                case CXCursorKind.CXCursor_ForStmt:
                {
                    result = new ForStmt(handle);
                    break;
                }

                case CXCursorKind.CXCursor_BreakStmt:
                {
                    result = new BreakStmt(handle);
                    break;
                }

                case CXCursorKind.CXCursor_ReturnStmt:
                {
                    result = new ReturnStmt(handle);
                    break;
                }

                case CXCursorKind.CXCursor_NullStmt:
                {
                    result = new NullStmt(handle);
                    break;
                }

                case CXCursorKind.CXCursor_DeclStmt:
                {
                    result = new DeclStmt(handle);
                    break;
                }

                default:
                {
                    Debug.WriteLine($"Unhandled statement kind: {handle.KindSpelling}.");
                    Debugger.Break();

                    result = new Stmt(handle, handle.Kind);
                    break;
                }
            }

            return result;
        }
    }
}
