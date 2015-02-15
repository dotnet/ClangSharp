namespace ClangSharpPInvokeGenerator
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using ClangSharp;

    internal sealed class FunctionVisitor : ICXCursorVisitor
    {
        private readonly TextWriter tw;

        private readonly HashSet<string> visitedFunctions = new HashSet<string>();

        public FunctionVisitor(TextWriter tw, string libraryPath)
        {
            this.tw = tw;
            this.tw.WriteLine("        private const string libraryPath = \"" + libraryPath + "\";");
            this.tw.WriteLine();
        }

        public CXChildVisitResult Visit(CXCursor cursor, CXCursor parent, IntPtr data)
        {
            if (cursor.IsInSystemHeader())
            {
                return CXChildVisitResult.CXChildVisit_Continue;
            }

            CXCursorKind curKind = Methods.clang_getCursorKind(cursor);

            // look only at function decls
            if (curKind == CXCursorKind.CXCursor_FunctionDecl)
            {
                var functionName = Methods.clang_getCursorSpelling(cursor).ToString();

                if (this.visitedFunctions.Contains(functionName))
                {
                    return CXChildVisitResult.CXChildVisit_Continue;
                }

                this.visitedFunctions.Add(functionName);

                Extensions.WriteFunctionInfoHelper(cursor, this.tw);

                return CXChildVisitResult.CXChildVisit_Continue;
            }

            return CXChildVisitResult.CXChildVisit_Recurse;
        }
    }
}