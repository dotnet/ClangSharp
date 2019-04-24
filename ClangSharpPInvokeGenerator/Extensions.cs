namespace ClangSharpPInvokeGenerator
{
    using System;
    using System.IO;
    using System.Text;
    using ClangSharp;

    internal static class Extensions
    {
        public static bool IsInSystemHeader(this CXCursor cursor)
        {
            return cursor.Location.IsInSystemHeader;
        }

        public static bool IsPtrToConstChar(this CXType type)
        {
            var pointee = type.PointeeType;
            return pointee.IsConstQualified && (pointee.kind == CXTypeKind.CXType_Char_S);
        }

        public static string ToPlainTypeString(this CXType type, string unknownType = "UnknownType")
        {
            var canonical = type.CanonicalType;;
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
                        return canonical.Spelling.ToString();
                    }
                    return canonical.ToPlainTypeString();
                default:
                    return unknownType;
            }
        }

        public static string ToMarshalString(this CXCursor cursor, string cursorSpelling)
        {
            var canonical = cursor.Type.CanonicalType;

            switch (canonical.kind)
            {
                case CXTypeKind.CXType_ConstantArray:
                    long arraySize = canonical.ArraySize;
                    var elementType = canonical.ArrayElementType.CanonicalType;

                    var sb = new StringBuilder();
                    for (int i = 0; i < arraySize; ++i)
                    {
                        sb.Append("public " + elementType.ToPlainTypeString() + " @" + cursorSpelling + i + "; ");
                    }

                    return sb.ToString().TrimEnd();
                case CXTypeKind.CXType_Pointer:
                    var pointeeType = canonical.PointeeType.CanonicalType;
                    switch (pointeeType.kind)
                    {
                        case CXTypeKind.CXType_Char_S:
                            return "[MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))] public string @" + cursorSpelling + ";";
                        case CXTypeKind.CXType_WChar:
                            return "[MarshalAs(UnmanagedType.LPWStr)] public string @" + cursorSpelling + ";";
                        default:
                            return "public IntPtr @" + cursorSpelling + ";";
                    }
                case CXTypeKind.CXType_Record:
                case CXTypeKind.CXType_Enum:
                    return "public " + canonical.Spelling.ToString() + " @" + cursorSpelling + ";";
                default:
                    return "public " + canonical.ToPlainTypeString() + " @" + cursorSpelling + ";";
            }
        }

        public static void WriteFunctionInfoHelper(CXCursor cursor, TextWriter tw, string prefixStrip)
        {
            var functionType = cursor.Type;
            var functionName = cursor.Spelling.ToString();
            var resultType = cursor.ResultType;

            tw.WriteLine("        [DllImport(libraryPath, EntryPoint = \"" + functionName + "\", CallingConvention = " + functionType.CallingConventionSpelling() + ")]");
            if (resultType.IsPtrToConstChar())
                tw.WriteLine("        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))]");

            tw.Write("        public static extern ");

            ReturnTypeHelper(resultType, tw);

            if (functionName.StartsWith(prefixStrip))
            {
                functionName = functionName.Substring(prefixStrip.Length);
            }

            tw.Write(" " + functionName + "(");

            int numArgTypes = functionType.NumArgTypes;

            for (uint i = 0; i < numArgTypes; ++i)
            {
                ArgumentHelper(functionType, cursor.GetArgument(i), tw, i);
            }

            tw.WriteLine(");");
            tw.WriteLine();
        }

        public static string CallingConventionSpelling(this CXType type)
        {
            var callingConvention = type.FunctionTypeCallingConv;
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
            var numArgTypes = functionType.NumArgTypes;
            var type = functionType.GetArgType(index);
            var cursorType = paramCursor.Type;

            var spelling = paramCursor.Spelling.ToString();
            if (string.IsNullOrEmpty(spelling))
            {
                spelling = "param" + index;
            }

            switch (type.kind)
            {
                case CXTypeKind.CXType_Pointer:
                    var pointee = type.PointeeType;
                    switch (pointee.kind)
                    {
                        case CXTypeKind.CXType_Pointer:
                            tw.Write(pointee.IsPtrToConstChar() && pointee.IsConstQualified ? "string[]" : "out IntPtr");
                            break;
                        case CXTypeKind.CXType_FunctionProto:
                            tw.Write(cursorType.Spelling.ToString());
                            break;
                        case CXTypeKind.CXType_Void:
                            tw.Write("IntPtr");
                            break;
                        case CXTypeKind.CXType_Char_S:
                            tw.Write(type.IsPtrToConstChar() ? "[MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))] string" : "IntPtr"); // if it's not a const, it's best to go with IntPtr
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
            bool isConstQualifiedType = type.IsConstQualified;
            string spelling;

            switch (type.kind)
            {
                // Need to unwrap elaborated types
                case CXTypeKind.CXType_Elaborated:
                    CommonTypeHandling(type.NamedType, tw, outParam);
                    return;
                case CXTypeKind.CXType_Typedef:
                    var cursor = type.Declaration;
                    var location = cursor.Location;

                    // For some reason size_t isn't considered as within a system header.
                    // We work around this by asking for the file name - if it's unknown, probably it's a system header
                    var isInSystemHeader = cursor.IsInSystemHeader();
                    cursor.Location.GetPresumedLocation(out CXString @filename, out uint @line, out uint @column);
                    isInSystemHeader |= filename.ToString() == string.Empty;

                    if (isInSystemHeader)
                    {
                        // Cross-plat:
                        // Getting the actual type of a typedef is painful, since platforms don't even agree on the meaning of types;
                        // 64-bit is "long long" on Windows but "long" on Linux, for historical reasons.
                        // The easiest way is to just get the size & signed-ness and write the type ourselves
                        var size = type.SizeOf;
                        var signed = !cursor.TypedefDeclUnderlyingType.ToString().Contains("unsigned");
                        spelling = GetTypeName(size, signed);
                    }
                    else
                    {
                        spelling = cursor.Spelling.ToString();
                    }
                    break;
                case CXTypeKind.CXType_Record:
                case CXTypeKind.CXType_Enum:
                    spelling = type.Spelling.ToString();
                    break;
                case CXTypeKind.CXType_IncompleteArray:
                    CommonTypeHandling(type.ArrayElementType, tw);
                    spelling = "[]";
                    break;
                case CXTypeKind.CXType_Unexposed: // Often these are enums and canonical type gets you the enum spelling
                    var canonical = type.CanonicalType;
                    // unexposed decl which turns into a function proto seems to be an un-typedef'd fn pointer
                    if (canonical.kind == CXTypeKind.CXType_FunctionProto)
                    {
                        spelling = "IntPtr";
                    }
                    else
                    {
                        spelling = canonical.Spelling.ToString();
                    }
                    break;
                default:
                    spelling = type.CanonicalType.ToPlainTypeString();
                    break;
            }

            if (isConstQualifiedType)
            {
                spelling = spelling.Replace("const ", string.Empty); // ugh
            }

            tw.Write(outParam + spelling);
        }

        private static string GetTypeName(long size, bool signed)
        {
            if (signed)
            {
                switch (size)
                {
                    case 1:
                        return "sbyte";

                    case 2:
                        return "short";

                    case 4:
                        return "int";

                    case 8:
                        return "long";
                }
            }
            else
            {

                switch (size)
                {
                    case 1:
                        return "byte";

                    case 2:
                        return "ushort";

                    case 4:
                        return "uint";

                    case 8:
                        return "ulong";
                }
            }

            throw new Exception("Unknown size.");
        }
    }
}
