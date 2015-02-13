namespace ClangSharpPInvokeGenerator
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using ClangSharp;

    internal sealed class StructVisitor : ICXCursorVisitor
    {
        private readonly ISet<string> visitedStructs = new HashSet<string>();

        private readonly TextWriter tw;

        private int indentLevel = 1;

        private const int indentMultiplier = 4;

        public StructVisitor(TextWriter tw)
        {
            this.tw = tw;
        }

        public CXChildVisitResult Visit(CXCursor cursor, CXCursor parent, IntPtr data)
        {
            CXCursorKind curKind = Methods.clang_getCursorKind(cursor);
            if (curKind == CXCursorKind.CXCursor_StructDecl)
            {
                var structName = Methods.clang_getCursorSpelling(cursor).String();

                // struct names can be empty, and so we visit its sibling to find the name
                if (string.IsNullOrEmpty(structName))
                {
                    var forwardDeclaringVisitor = new ForwardDeclarationVisitor(cursor);
                    Methods.clang_visitChildren(Methods.clang_getCursorSemanticParent(cursor), forwardDeclaringVisitor.Visit, new CXClientData(IntPtr.Zero));
                    structName = Methods.clang_getCursorSpelling(forwardDeclaringVisitor.ForwardDeclarationCursor).String();

                    if (string.IsNullOrEmpty(structName))
                    {
                        structName = "_";
                    }
                }

                if (!this.visitedStructs.Contains(structName))
                {
                    this.IndentedWriteLine("public partial struct " + structName);
                    this.IndentedWriteLine("{");

                    this.indentLevel++;
                    Methods.clang_visitChildren(cursor, this.Visit, new CXClientData(IntPtr.Zero));
                    this.indentLevel--;

                    this.IndentedWriteLine("}");
                    this.tw.WriteLine();

                    this.visitedStructs.Add(structName);
                }

                return CXChildVisitResult.CXChildVisit_Continue;
            }

            if (curKind == CXCursorKind.CXCursor_FieldDecl)
            {
                this.IndentedWriteLine(cursor.ToMarshalString());
                return CXChildVisitResult.CXChildVisit_Continue;
            }

            return CXChildVisitResult.CXChildVisit_Recurse;
        }

        private void IndentedWriteLine(string s)
        {
            for (int i = 0; i < indentMultiplier * indentLevel; ++i)
            {
                this.tw.Write(" ");
            }

            this.tw.WriteLine(s);
        }
    }
}