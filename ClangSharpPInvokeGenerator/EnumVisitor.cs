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

                // Cross-plat hack:
                // For whatever reason, libclang detects untyped enums (i.e. your average 'enum X { A, B }')
                // as uints on Linux and ints on Windows.
                // Since we want to have the same generated code everywhere, we try to force 'int'
                // if it doesn't change semantics, i.e. if all enum values are in the right range.
                // Remember that 2's complement ints use the same binary representation as uints for positive numbers.
                if (inheritedEnumType == "uint")
                {
                    bool hasOneValue = false;
                    long minValue = long.MaxValue;
                    long maxValue = long.MinValue;
                    clang.visitChildren(cursor, (cxCursor, _, __) =>
                    {
                        hasOneValue = true;

                        long value = clang.getEnumConstantDeclValue(cxCursor);
                        minValue = Math.Min(minValue, value);
                        maxValue = Math.Max(maxValue, value);

                        return CXChildVisitResult.CXChildVisit_Continue;
                    }, new CXClientData(IntPtr.Zero));

                    if (hasOneValue && minValue >= 0 && maxValue <= int.MaxValue)
                    {
                        inheritedEnumType = "int";
                    }
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