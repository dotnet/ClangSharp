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

        private readonly string prefixStrip;

        public FunctionVisitor(TextWriter tw, string libraryPath, string prefixStrip, string[] excludeFunctionsArray)
        {
            this.prefixStrip = prefixStrip;
            this.tw = tw;
            this.tw.WriteLine("        private const string libraryPath = \"" + libraryPath + "\";");
            this.tw.WriteLine();

            if(excludeFunctionsArray != null)
            {
                //For all the functions we're excluding add them to the visitedFunctions set so they're ignored
                foreach(var func in excludeFunctionsArray)
                {
                    visitedFunctions.Add(func);
                }
            }
        }

        public CXChildVisitResult Visit(CXCursor cursor, CXCursor parent, IntPtr data)
        {
            if (cursor.IsInSystemHeader())
            {
                return CXChildVisitResult.CXChildVisit_Continue;
            }

            CXCursorKind curKind = clang.getCursorKind(cursor);

            // look only at function decls
            if (curKind == CXCursorKind.CXCursor_FunctionDecl)
            {
                var functionName = clang.getCursorSpelling(cursor).ToString();

                if (this.visitedFunctions.Contains(functionName))
                {
                    return CXChildVisitResult.CXChildVisit_Continue;
                }

                this.visitedFunctions.Add(functionName);

                Extensions.WriteFunctionInfoHelper(cursor, this.tw, this.prefixStrip);

                return CXChildVisitResult.CXChildVisit_Continue;
            }

            return CXChildVisitResult.CXChildVisit_Recurse;
        }
    }
}
