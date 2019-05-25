using System;
using System.Diagnostics;
using ClangSharp;

namespace ClangSharpPInvokeGenerator
{
    internal class Stmt : Cursor
    {
        protected Stmt(CXCursor handle, Cursor parent) : base(handle, parent)
        {
            Debug.Assert(handle.IsStatement);
        }

        protected override void ValidateVisit(ref CXCursor handle)
        {
            // Clang currently uses the PostChildrenVisitor which clears data0

            var modifiedHandle = Handle;
            modifiedHandle.data0 = IntPtr.Zero;

            Debug.Assert(handle.Equals(modifiedHandle));
            handle = Handle;
        }
    }
}
