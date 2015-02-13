namespace ClangSharpPInvokeGenerator
{
    using System;
    using ClangSharp;

    internal interface ICXCursorVisitor
    {
        CXChildVisitResult Visit(CXCursor cursor, CXCursor parent, IntPtr data);
    }
}