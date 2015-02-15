namespace ClangSharpPInvokeGenerator
{
    using System.IO;
    using System.Text;
    using ClangSharp;

    internal static class Extensions
    {
        public static bool IsInSystemHeader(this CXCursor cursor)
        {
            return Methods.clang_Location_isInSystemHeader(Methods.clang_getCursorLocation(cursor)) != 0;
        }

        public static bool IsPtrToConstChar(this CXType type)
        {
            var pointee = Methods.clang_getPointeeType(type);

            if (Methods.clang_isConstQualifiedType(pointee) != 0)
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
                    var canonical = Methods.clang_getCanonicalType(type);
                    if (canonical.kind == CXTypeKind.CXType_Unexposed)
                    {
                        return Methods.clang_getTypeSpelling(canonical).ToString();
                    }
                    return canonical.ToPlainTypeString();
                default:
                    return unknownType;
            }
        }

        public static string ToMarshalString(this CXCursor cursor, string cursorSpelling)
        {
            var canonical = Methods.clang_getCanonicalType(Methods.clang_getCursorType(cursor));

            switch (canonical.kind)
            {
                case CXTypeKind.CXType_ConstantArray:
                    long arraySize = Methods.clang_getArraySize(canonical);
                    var elementType = Methods.clang_getCanonicalType(Methods.clang_getArrayElementType(canonical));

                    var sb = new StringBuilder();
                    for (int i = 0; i < arraySize; ++i)
                    {
                        sb.Append("public " + elementType.ToPlainTypeString() + " @" + cursorSpelling + i + "; ");
                    }

                    return sb.ToString();
                case CXTypeKind.CXType_Pointer:
                    var pointeeType = Methods.clang_getCanonicalType(Methods.clang_getPointeeType(canonical));
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
                    return "public " + Methods.clang_getTypeSpelling(canonical).ToString() + " @" + cursorSpelling + ";";
                default:
                    return "public " + canonical.ToPlainTypeString() + " @"  + cursorSpelling + ";";
            }
        }

        public static void WriteFunctionInfoHelper(CXCursor cursor, TextWriter tw)
        {
            var functionType = Methods.clang_getCursorType(cursor);
            var functionName = Methods.clang_getCursorSpelling(cursor).ToString();
            var resultType = Methods.clang_getCursorResultType(cursor);

            tw.WriteLine("        [DllImport(libraryPath, CallingConvention = " + functionType.CallingConventionSpelling() + ")]");
            tw.Write("        public static extern ");

            ReturnTypeHelper(resultType, tw);

            tw.Write(" " + functionName + "(");

            int numArgTypes = Methods.clang_getNumArgTypes(functionType);

            for (uint i = 0; i < numArgTypes; ++i)
            {
                ArgumentHelper(functionType, Methods.clang_Cursor_getArgument(cursor, i), tw, i);
            }

            tw.WriteLine(");");
            tw.WriteLine();
        }

        public static string CallingConventionSpelling(this CXType type)
        {
            var callingConvention = Methods.clang_getFunctionTypeCallingConv(type);
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
            var numArgTypes = Methods.clang_getNumArgTypes(functionType);
            var type = Methods.clang_getArgType(functionType, index);
            var cursorType = Methods.clang_getCursorType(paramCursor);

            var spelling = Methods.clang_getCursorSpelling(paramCursor).ToString();
            if (string.IsNullOrEmpty(spelling))
            {
                spelling = "param" + index;
            }

            switch (type.kind)
            {
            case CXTypeKind.CXType_Pointer:
                var pointee = Methods.clang_getPointeeType(type);
                switch (pointee.kind)
                {
                case CXTypeKind.CXType_Pointer:
                    tw.Write(pointee.IsPtrToConstChar() && Methods.clang_isConstQualifiedType(pointee) != 0 ? "string[]" : "out IntPtr");
                    break;
                case CXTypeKind.CXType_FunctionProto:
                    tw.Write(Methods.clang_getTypeSpelling(cursorType).ToString());
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
            bool isConstQualifiedType = Methods.clang_isConstQualifiedType(type) != 0;
            string spelling;
            switch (type.kind)
            {
                case CXTypeKind.CXType_Typedef:
                    spelling = Methods.clang_getCursorSpelling(Methods.clang_getTypeDeclaration(type)).ToString();
                    break;
                case CXTypeKind.CXType_Record:
                case CXTypeKind.CXType_Enum:
                    spelling = Methods.clang_getTypeSpelling(type).ToString();
                    break;
                case CXTypeKind.CXType_IncompleteArray:
                    spelling = Methods.clang_getArrayElementType(type).ToPlainTypeString() + "[]";
                    break;
                case CXTypeKind.CXType_Unexposed: // Often these are enums and canonical type gets you the enum spelling
                    var canonical = Methods.clang_getCanonicalType(type);
                    // unexposed decl which turns into a function proto seems to be an un-typedef'd fn pointer
                    if (canonical.kind == CXTypeKind.CXType_FunctionProto)
                    {
                        spelling = "IntPtr";
                    }
                    else
                    {
                        spelling = Methods.clang_getTypeSpelling(canonical).ToString();
                    }    
                    break;
                default:
                    spelling = Methods.clang_getCanonicalType(type).ToPlainTypeString();
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