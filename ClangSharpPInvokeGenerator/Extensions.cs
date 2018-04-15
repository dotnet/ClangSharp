namespace ClangSharpPInvokeGenerator
{
    using System.IO;
    using System.Text;
    using ClangSharp;

    internal static class Extensions
    {
        public static bool IsInSystemHeader(this CXCursor cursor)
        {
            return clang.Location_isInSystemHeader(clang.getCursorLocation(cursor)) != 0;
        }

        public static bool IsPtrToConstChar(this CXType type)
        {
            var pointee = clang.getPointeeType(type);

            if (clang.isConstQualifiedType(pointee) != 0)
            {
                switch (pointee.kind)
                {
                    case CXTypeKind.CXType_Char_S:
                        return true;
                }
            }

            return false;
        }

        public static string ToPlainTypeString(this CXType type, string unknownType = "UnknownType")
        {
            var canonical = clang.getCanonicalType(type);
            switch (type.kind)
            {
                case CXTypeKind.CXType_Bool:
                    return "bool";
                case CXTypeKind.CXType_UChar:
                case CXTypeKind.CXType_Char_U:
                    return "char";
                case CXTypeKind.CXType_SChar:
                case CXTypeKind.CXType_Char_S:
                    return "sbyte";
                case CXTypeKind.CXType_UShort:
                    return "ushort";
                case CXTypeKind.CXType_Short:
                    return "short";
                case CXTypeKind.CXType_Float:
                    return "float";
                case CXTypeKind.CXType_Double:
                    return "double";
                case CXTypeKind.CXType_Int:
                    return "int";
                case CXTypeKind.CXType_UInt:
                    return "uint";
                case CXTypeKind.CXType_Pointer:
                case CXTypeKind.CXType_NullPtr: // ugh, what else can I do?
                    return "IntPtr";
                case CXTypeKind.CXType_Long:
                    return "int";
                case CXTypeKind.CXType_ULong:
                    return "int";
                case CXTypeKind.CXType_LongLong:
                    return "long";
                case CXTypeKind.CXType_ULongLong:
                    return "ulong";
                case CXTypeKind.CXType_Void:
                    return "void";
                case CXTypeKind.CXType_Unexposed:
                    if (canonical.kind == CXTypeKind.CXType_Unexposed)
                    {
                        return clang.getTypeSpelling(canonical).ToString();
                    }
                    return canonical.ToPlainTypeString();
                default:
                    return unknownType;
            }
        }

        public static string ToMarshalString(this CXCursor cursor, string cursorSpelling)
        {
            var canonical = clang.getCanonicalType(clang.getCursorType(cursor));

            switch (canonical.kind)
            {
                case CXTypeKind.CXType_ConstantArray:
                    long arraySize = clang.getArraySize(canonical);
                    var elementType = clang.getCanonicalType(clang.getArrayElementType(canonical));

                    var sb = new StringBuilder();
                    for (int i = 0; i < arraySize; ++i)
                    {
                        sb.Append("public " + elementType.ToPlainTypeString() + " @" + cursorSpelling + i + "; ");
                    }

                    return sb.ToString();
                case CXTypeKind.CXType_Pointer:
                    var pointeeType = clang.getCanonicalType(clang.getPointeeType(canonical));
                    switch (pointeeType.kind)
                    {
                        case CXTypeKind.CXType_Char_S:
                            return "[MarshalAs(UnmanagedType.LPStr)] public string @" + cursorSpelling + ";";
                        case CXTypeKind.CXType_WChar:
                            return "[MarshalAs(UnmanagedType.LPWStr)] public string @" + cursorSpelling + ";";
                        default:
                            return "public IntPtr @" + cursorSpelling + ";";
                    }
                case CXTypeKind.CXType_Record:
                case CXTypeKind.CXType_Enum:
                    return "public " + clang.getTypeSpelling(canonical).ToString() + " @" + cursorSpelling + ";";
                default:
                    return "public " + canonical.ToPlainTypeString() + " @" + cursorSpelling + ";";
            }
        }

        public static void WriteFunctionInfoHelper(CXCursor cursor, TextWriter tw, string prefixStrip)
        {
            var functionType = clang.getCursorType(cursor);
            var functionName = clang.getCursorSpelling(cursor).ToString();
            var resultType = clang.getCursorResultType(cursor);

            tw.WriteLine("        [DllImport(libraryPath, EntryPoint = \"" + functionName + "\", CallingConvention = " + functionType.CallingConventionSpelling() + ")]");
            if(resultType.IsPtrToConstChar())
                tw.WriteLine("        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringReturnMarshaller))]");

            tw.Write("        public static extern ");

            ReturnTypeHelper(resultType, tw);

            if (functionName.StartsWith(prefixStrip))
            {
                functionName = functionName.Substring(prefixStrip.Length);
            }

            tw.Write(" " + functionName + "(");

            int numArgTypes = clang.getNumArgTypes(functionType);

            for (uint i = 0; i < numArgTypes; ++i)
            {
                ArgumentHelper(functionType, clang.Cursor_getArgument(cursor, i), tw, i);
            }

            tw.WriteLine(");");
            tw.WriteLine();
        }

        public static string CallingConventionSpelling(this CXType type)
        {
            var callingConvention = clang.getFunctionTypeCallingConv(type);
            switch (callingConvention)
            {
                case CXCallingConv.CXCallingConv_X86StdCall:
                case CXCallingConv.CXCallingConv_X86_64Win64:
                    return "CallingConvention.StdCall";
                default:
                    return "CallingConvention.Cdecl";
            }
        }

        public static void ReturnTypeHelper(CXType resultType, TextWriter tw)
        {
            switch (resultType.kind)
            {
                case CXTypeKind.CXType_Pointer:
                    tw.Write(resultType.IsPtrToConstChar() ? "string" : "IntPtr"); // const char* gets special treatment
                    break;
                default:
                    CommonTypeHandling(resultType, tw);
                    break;
            }
        }

        public static void ArgumentHelper(CXType functionType, CXCursor paramCursor, TextWriter tw, uint index)
        {
            var numArgTypes = clang.getNumArgTypes(functionType);
            var type = clang.getArgType(functionType, index);
            var cursorType = clang.getCursorType(paramCursor);

            var spelling = clang.getCursorSpelling(paramCursor).ToString();
            if (string.IsNullOrEmpty(spelling))
            {
                spelling = "param" + index;
            }

            switch (type.kind)
            {
                case CXTypeKind.CXType_Pointer:
                    var pointee = clang.getPointeeType(type);
                    switch (pointee.kind)
                    {
                        case CXTypeKind.CXType_Pointer:
                            tw.Write(pointee.IsPtrToConstChar() && clang.isConstQualifiedType(pointee) != 0 ? "string[]" : "out IntPtr");
                            break;
                        case CXTypeKind.CXType_FunctionProto:
                            tw.Write(clang.getTypeSpelling(cursorType).ToString());
                            break;
                        case CXTypeKind.CXType_Void:
                            tw.Write("IntPtr");
                            break;
                        case CXTypeKind.CXType_Char_S:
                            tw.Write(type.IsPtrToConstChar() ? "[MarshalAs(UnmanagedType.LPStr)] string" : "IntPtr"); // if it's not a const, it's best to go with IntPtr
                            break;
                        case CXTypeKind.CXType_WChar:
                            tw.Write(type.IsPtrToConstChar() ? "[MarshalAs(UnmanagedType.LPWStr)] string" : "IntPtr");
                            break;
                        default:
                            CommonTypeHandling(pointee, tw, "out ");
                            break;
                    }
                    break;
                default:
                    CommonTypeHandling(type, tw);
                    break;
            }

            tw.Write(" @");
            tw.Write(spelling);

            if (index != numArgTypes - 1)
            {
                tw.Write(", ");
            }
        }

        private static void CommonTypeHandling(CXType type, TextWriter tw, string outParam = "")
        {
            bool isConstQualifiedType = clang.isConstQualifiedType(type) != 0;
            string spelling;

            switch (type.kind)
            {
                // Need to unwrap elaborated types
                case CXTypeKind.CXType_Elaborated:
                    CommonTypeHandling(clang.Type_getNamedType(type), tw, outParam);
                    return;
                case CXTypeKind.CXType_Typedef:
                    var cursor = clang.getTypeDeclaration(type);
                    if (clang.Location_isInSystemHeader(clang.getCursorLocation(cursor)) != 0)
                    {
                        spelling = clang.getCanonicalType(type).ToPlainTypeString();
                    }
                    else
                    {
                        spelling = clang.getCursorSpelling(cursor).ToString();
                    }
                    break;
                case CXTypeKind.CXType_Record:
                case CXTypeKind.CXType_Enum:
                    spelling = clang.getTypeSpelling(type).ToString();
                    break;
                case CXTypeKind.CXType_IncompleteArray:
                    CommonTypeHandling(clang.getArrayElementType(type), tw);
                    spelling = "[]";
                    break;
                case CXTypeKind.CXType_Unexposed: // Often these are enums and canonical type gets you the enum spelling
                    var canonical = clang.getCanonicalType(type);
                    // unexposed decl which turns into a function proto seems to be an un-typedef'd fn pointer
                    if (canonical.kind == CXTypeKind.CXType_FunctionProto)
                    {
                        spelling = "IntPtr";
                    }
                    else
                    {
                        spelling = clang.getTypeSpelling(canonical).ToString();
                    }
                    break;
                default:
                    spelling = clang.getCanonicalType(type).ToPlainTypeString();
                    break;
            }

            if (isConstQualifiedType)
            {
                spelling = spelling.Replace("const ", string.Empty); // ugh
            }

            tw.Write(outParam + spelling);
        }
    }
}
