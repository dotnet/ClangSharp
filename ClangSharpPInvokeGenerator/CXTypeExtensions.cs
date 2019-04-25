using System.Diagnostics;
using ClangSharp;

namespace ClangSharpPInvokeGenerator
{
    internal static class CXTypeExtensions
    {
        public static string GetFunctionProtoCallingConventionName(this CXType type, CXCursor cursor)
        {
            Debug.Assert(type.kind == CXTypeKind.CXType_FunctionProto);

            var callingConvention = type.FunctionTypeCallingConv;

            switch (callingConvention)
            {
                case CXCallingConv.CXCallingConv_C:
                {
                    return "Cdecl";
                }

                default:
                {
                    Debug.WriteLine($"Unhandled calling convention: {callingConvention} in {type.KindSpelling} in {cursor.KindSpelling}.");
                    Debugger.Break();
                    return string.Empty;
                }
            }
        }

        public static string GetMarshalAttribute(this CXType type, CXCursor cursor)
        {
            switch (type.kind)
            {
                case CXTypeKind.CXType_Void:
                case CXTypeKind.CXType_UShort:
                case CXTypeKind.CXType_UInt:
                case CXTypeKind.CXType_ULong:
                case CXTypeKind.CXType_ULongLong:
                case CXTypeKind.CXType_Short:
                case CXTypeKind.CXType_Int:
                case CXTypeKind.CXType_Long:
                case CXTypeKind.CXType_LongLong:
                case CXTypeKind.CXType_Double:
                case CXTypeKind.CXType_Enum:
                case CXTypeKind.CXType_Typedef:
                case CXTypeKind.CXType_ConstantArray:
                {
                    return string.Empty;
                }

                case CXTypeKind.CXType_Pointer:
                {
                    return type.GetMarshalAttributeForPointeeType(cursor, type.PointeeType);
                }

                case CXTypeKind.CXType_Elaborated:
                {
                    return type.CanonicalType.GetMarshalAttribute(cursor);
                }

                default:
                {
                    Unhandled(type, cursor);
                    return string.Empty;
                }
            }
        }

        public static string GetMarshalAttributeForPointeeType(this CXType type, CXCursor cursor, CXType pointeeType)
        {
            switch (pointeeType.kind)
            {
                case CXTypeKind.CXType_Void:
                case CXTypeKind.CXType_UShort:
                case CXTypeKind.CXType_UInt:
                case CXTypeKind.CXType_ULong:
                case CXTypeKind.CXType_ULongLong:
                case CXTypeKind.CXType_Short:
                case CXTypeKind.CXType_Int:
                case CXTypeKind.CXType_Long:
                case CXTypeKind.CXType_LongLong:
                case CXTypeKind.CXType_Double:
                case CXTypeKind.CXType_Pointer:
                case CXTypeKind.CXType_Enum:
                case CXTypeKind.CXType_Typedef:
                case CXTypeKind.CXType_FunctionProto:
                {
                    return string.Empty;
                }

                case CXTypeKind.CXType_Char_S:
                {
                    return "MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))";
                }

                case CXTypeKind.CXType_WChar:
                {
                    return "MarshalAs(UnmanagedType.LPWStr)";
                }

                case CXTypeKind.CXType_Elaborated:
                {
                    return type.GetMarshalAttributeForPointeeType(cursor, pointeeType.CanonicalType);
                }

                default:
                {
                    Unhandled(pointeeType, cursor);
                    return string.Empty;
                }
            }
        }

        public static string GetName(this CXType type, CXCursor cursor)
        {
            switch (type.kind)
            {
                case CXTypeKind.CXType_Void:
                {
                    return "void";
                }

                case CXTypeKind.CXType_UShort:
                {
                    return "ushort";
                }

                case CXTypeKind.CXType_UInt:
                {
                    return "uint";
                }

                case CXTypeKind.CXType_ULong:
                {
                    return "uint";
                }

                case CXTypeKind.CXType_ULongLong:
                {
                    return "ulong";
                }

                case CXTypeKind.CXType_Short:
                {
                    return "short";
                }

                case CXTypeKind.CXType_Int:
                {
                    return "int";
                }

                case CXTypeKind.CXType_Long:
                {
                    return "int";
                }

                case CXTypeKind.CXType_LongLong:
                {
                    return "long";
                }

                case CXTypeKind.CXType_Double:
                {
                    return "double";
                }

                case CXTypeKind.CXType_Pointer:
                {
                    return type.GetNameForPointeeType(cursor, type.PointeeType);
                }

                case CXTypeKind.CXType_Record:
                case CXTypeKind.CXType_Enum:
                case CXTypeKind.CXType_FunctionProto:
                {
                    var name = type.Spelling.ToString();
                    Debug.Assert(!string.IsNullOrWhiteSpace(name));
                    return name;
                }

                case CXTypeKind.CXType_Typedef:
                {
                    return type.Declaration.GetTypedefDeclName();
                }

                case CXTypeKind.CXType_Elaborated:
                {
                    return type.CanonicalType.GetName(cursor);
                }

                case CXTypeKind.CXType_ConstantArray:
                {
                    return type.GetNameForElementType(cursor, type.ElementType);
                }

                default:
                {
                    Unhandled(type, cursor);
                    return string.Empty;
                }
            }
        }

        public static string GetNameForPointeeType(this CXType type, CXCursor cursor, CXType pointeeType)
        {
            switch (pointeeType.kind)
            {
                case CXTypeKind.CXType_Void:
                case CXTypeKind.CXType_FunctionProto:
                {
                    return "IntPtr";
                }

                case CXTypeKind.CXType_UShort:
                case CXTypeKind.CXType_UInt:
                case CXTypeKind.CXType_ULong:
                case CXTypeKind.CXType_ULongLong:
                case CXTypeKind.CXType_Short:
                case CXTypeKind.CXType_Int:
                case CXTypeKind.CXType_Long:
                case CXTypeKind.CXType_LongLong:
                case CXTypeKind.CXType_Pointer:
                case CXTypeKind.CXType_Record:
                case CXTypeKind.CXType_Enum:
                {
                    switch (cursor.Kind)
                    {
                        case CXCursorKind.CXCursor_FieldDecl:
                        {
                            return "IntPtr";
                        }

                        case CXCursorKind.CXCursor_ParmDecl:
                        {
                            return pointeeType.GetName(cursor);
                        }

                        case CXCursorKind.CXCursor_TypedefDecl:
                        {
                            return cursor.GetTypedefDeclName();
                        }

                        default:
                        {
                            Unhandled(pointeeType, cursor);
                            return string.Empty;
                        }
                    }
                }

                case CXTypeKind.CXType_Char_S:
                {
                    switch (cursor.Kind)
                    {
                        case CXCursorKind.CXCursor_FieldDecl:
                        case CXCursorKind.CXCursor_FunctionDecl:
                        {
                            return "string";
                        }

                        case CXCursorKind.CXCursor_ParmDecl:
                        {
                            if (cursor.Type.GetParmModifier(cursor).Equals("out"))
                            {
                                return "IntPtr";
                            }

                            return "string";
                        }

                        default:
                        {
                            Unhandled(pointeeType, cursor);
                            return string.Empty;
                        }
                    }
                }

                case CXTypeKind.CXType_Typedef:
                {
                    switch (cursor.Kind)
                    {
                        case CXCursorKind.CXCursor_FieldDecl:
                        case CXCursorKind.CXCursor_FunctionDecl:
                        {
                            return "IntPtr";
                        }

                        case CXCursorKind.CXCursor_ParmDecl:
                        {
                            return pointeeType.Declaration.GetTypedefDeclName();
                        }

                        default:
                        {
                            Unhandled(pointeeType, cursor);
                            return string.Empty;
                        }
                    }
                }

                case CXTypeKind.CXType_Elaborated:
                {
                    return type.GetNameForPointeeType(cursor, pointeeType.CanonicalType);
                }

                default:
                {
                    Unhandled(pointeeType, cursor);
                    return string.Empty;
                }
            }
        }

        public static string GetNameForElementType(this CXType type, CXCursor cursor, CXType elementType)
        {
            return $"{elementType.GetName(cursor)}";
        }

        public static string GetParmModifier(this CXType type, CXCursor cursor)
        {
            switch (type.kind)
            {
                case CXTypeKind.CXType_Void:
                case CXTypeKind.CXType_UShort:
                case CXTypeKind.CXType_UInt:
                case CXTypeKind.CXType_ULong:
                case CXTypeKind.CXType_ULongLong:
                case CXTypeKind.CXType_Short:
                case CXTypeKind.CXType_Int:
                case CXTypeKind.CXType_Long:
                case CXTypeKind.CXType_LongLong:
                case CXTypeKind.CXType_Double:
                case CXTypeKind.CXType_Enum:
                case CXTypeKind.CXType_Typedef:
                case CXTypeKind.CXType_ConstantArray:
                {
                    return string.Empty;
                }

                case CXTypeKind.CXType_Pointer:
                {
                    return type.GetParmModifierForPointeeType(cursor, type.PointeeType);
                }

                case CXTypeKind.CXType_Elaborated:
                {
                    return type.CanonicalType.GetParmModifier(cursor);
                }

                default:
                {
                    Unhandled(type, cursor);
                    return string.Empty;
                }
            }
        }

        public static string GetParmModifierForPointeeType(this CXType type, CXCursor cursor, CXType pointeeType)
        {
            switch (pointeeType.kind)
            {
                case CXTypeKind.CXType_Void:
                case CXTypeKind.CXType_Char_S:
                case CXTypeKind.CXType_WChar:
                case CXTypeKind.CXType_FunctionProto:
                {
                    return string.Empty;
                }

                case CXTypeKind.CXType_UShort:
                case CXTypeKind.CXType_UInt:
                case CXTypeKind.CXType_ULong:
                case CXTypeKind.CXType_ULongLong:
                case CXTypeKind.CXType_Short:
                case CXTypeKind.CXType_Int:
                case CXTypeKind.CXType_Long:
                case CXTypeKind.CXType_LongLong:
                case CXTypeKind.CXType_Double:
                case CXTypeKind.CXType_Record:
                case CXTypeKind.CXType_Enum:
                case CXTypeKind.CXType_Pointer:
                {
                    return "out";
                }

                case CXTypeKind.CXType_Typedef:
                case CXTypeKind.CXType_Elaborated:
                {
                    return type.GetParmModifierForPointeeType(cursor, pointeeType.CanonicalType);
                }

                default:
                {
                    Unhandled(pointeeType, cursor);
                    return string.Empty;
                }
            }
        }

        public static void Unhandled(CXType type, CXCursor cursor)
        {
            Debug.WriteLine($"Unhandled type kind: {type.KindSpelling} in {cursor.KindSpelling}.");
            Debugger.Break();
        }
    }
}
