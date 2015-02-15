namespace ClangSharpPInvokeGenerator
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using ClangSharp;

    internal sealed class EnumVisitor : ICXCursorVisitor
    {
        private readonly ISet<string> printedEnums = new HashSet<string>();

        private readonly TextWriter tw;

        public EnumVisitor(TextWriter sw)
        {
            this.tw = sw;
        }

        public CXChildVisitResult Visit(CXCursor cursor, CXCursor parent, IntPtr data)
        {
            if (cursor.IsInSystemHeader())
            {
                return CXChildVisitResult.CXChildVisit_Continue;
            }

            CXCursorKind curKind = clang.getCursorKind(cursor);
            if (curKind == CXCursorKind.CXCursor_EnumDecl)
            {
                string inheritedEnumType;
                CXTypeKind kind = clang.getEnumDeclIntegerType(cursor).kind;

                switch (kind)
                {
                    case CXTypeKind.CXType_Int:
                        inheritedEnumType = "int";
                        break;
                    case CXTypeKind.CXType_UInt:
                        inheritedEnumType = "uint";
                        break;
                    case CXTypeKind.CXType_Short:
                        inheritedEnumType = "short";
                        break;
                    case CXTypeKind.CXType_UShort:
                        inheritedEnumType = "ushort";
                        break;
                    case CXTypeKind.CXType_LongLong:
                        inheritedEnumType = "long";
                        break;
                    case CXTypeKind.CXType_ULongLong:
                        inheritedEnumType = "ulong";
                        break;
                    default:
                        inheritedEnumType = "int";
                        break;
                }

                var enumName = clang.getCursorSpelling(cursor).ToString();

                // enumName can be empty because of typedef enum { .. } enumName;
                // so we have to find the sibling, and this is the only way I've found
                // to do with libclang, maybe there is a better way?
                if (string.IsNullOrEmpty(enumName))
                {
                    var forwardDeclaringVisitor = new ForwardDeclarationVisitor(cursor);
                    clang.visitChildren(clang.getCursorLexicalParent(cursor), forwardDeclaringVisitor.Visit, new CXClientData(IntPtr.Zero));
                    enumName = clang.getCursorSpelling(forwardDeclaringVisitor.ForwardDeclarationCursor).ToString();

                    if (string.IsNullOrEmpty(enumName))
                    {
                        enumName = "_";
                    }
                }

                // if we've printed these previously, skip them
                if (this.printedEnums.Contains(enumName))
                {
                    return CXChildVisitResult.CXChildVisit_Continue;
                }

                this.printedEnums.Add(enumName);

                this.tw.WriteLine("    public enum " + enumName + " : " + inheritedEnumType);
                this.tw.WriteLine("    {");

                // visit all the enum values
                clang.visitChildren(cursor, (cxCursor, _, __) =>
                {
                    this.tw.WriteLine("        @" + clang.getCursorSpelling(cxCursor).ToString() + " = " + clang.getEnumConstantDeclValue(cxCursor) + ",");
                    return CXChildVisitResult.CXChildVisit_Continue;
                }, new CXClientData(IntPtr.Zero));

                this.tw.WriteLine("    }");
                this.tw.WriteLine();
            }

            return CXChildVisitResult.CXChildVisit_Recurse;
        }
    }
}