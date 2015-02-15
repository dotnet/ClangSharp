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

        private int fieldPosition;

        private const int indentMultiplier = 4;

        public StructVisitor(TextWriter tw)
        {
            this.tw = tw;
        }

        public CXChildVisitResult Visit(CXCursor cursor, CXCursor parent, IntPtr data)
        {
            if (cursor.IsInSystemHeader())
            {
                return CXChildVisitResult.CXChildVisit_Continue;
            }

            CXCursorKind curKind = clang.getCursorKind(cursor);
            if (curKind == CXCursorKind.CXCursor_StructDecl)
            {
                this.fieldPosition = 0;
                var structName = clang.getCursorSpelling(cursor).ToString();

                // struct names can be empty, and so we visit its sibling to find the name
                if (string.IsNullOrEmpty(structName))
                {
                    var forwardDeclaringVisitor = new ForwardDeclarationVisitor(cursor);
                    clang.visitChildren(clang.getCursorSemanticParent(cursor), forwardDeclaringVisitor.Visit, new CXClientData(IntPtr.Zero));
                    structName = clang.getCursorSpelling(forwardDeclaringVisitor.ForwardDeclarationCursor).ToString();

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
                    clang.visitChildren(cursor, this.Visit, new CXClientData(IntPtr.Zero));
                    this.indentLevel--;

                    this.IndentedWriteLine("}");
                    this.tw.WriteLine();

                    this.visitedStructs.Add(structName);
                }

                return CXChildVisitResult.CXChildVisit_Continue;
            }

            if (curKind == CXCursorKind.CXCursor_FieldDecl)
            {
                var fieldName = clang.getCursorSpelling(cursor).ToString();
                if (string.IsNullOrEmpty(fieldName))
                {
                    fieldName = "field" + this.fieldPosition; // what if they have fields called field*? :)
                }

                this.fieldPosition++;
                this.IndentedWriteLine(cursor.ToMarshalString(fieldName));
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