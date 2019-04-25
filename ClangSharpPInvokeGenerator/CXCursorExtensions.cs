using System.Diagnostics;
using ClangSharp;

namespace ClangSharpPInvokeGenerator
{
    internal static class CXCursorExtensions
    {
        public static string EscapeName(string name)
        {
            switch (name)
            {
                case "abstract":
                case "as":
                case "base":
                case "bool":
                case "break":
                case "byte":
                case "case":
                case "catch":
                case "char":
                case "checked":
                case "class":
                case "const":
                case "continue":
                case "decimal":
                case "default":
                case "delegate":
                case "do":
                case "double":
                case "else":
                case "enum":
                case "event":
                case "explicit":
                case "extern":
                case "false":
                case "finally":
                case "fixed":
                case "float":
                case "for":
                case "foreach":
                case "goto":
                case "if":
                case "implicit":
                case "in":
                case "int":
                case "interface":
                case "internal":
                case "is":
                case "lock":
                case "long":
                case "namespace":
                case "new":
                case "null":
                case "object":
                case "operator":
                case "out":
                case "override":
                case "params":
                case "private":
                case "protected":
                case "public":
                case "readonly":
                case "ref":
                case "return":
                case "sbyte":
                case "sealed":
                case "short":
                case "sizeof":
                case "stackalloc":
                case "static":
                case "string":
                case "struct":
                case "switch":
                case "this":
                case "throw":
                case "true":
                case "try":
                case "typeof":
                case "uint":
                case "ulong":
                case "unchecked":
                case "unsafe":
                case "ushort":
                case "using":
                case "using static":
                case "virtual":
                case "void":
                case "volatile":
                case "while":
                {
                    return $"@{name}";
                }

                default:
                {
                    return name;
                }
            }
        }

        public static string GetEnumConstantDeclName(this CXCursor cursor)
        {
            Debug.Assert(cursor.Kind == CXCursorKind.CXCursor_EnumConstantDecl);

            var name = cursor.Spelling.ToString();
            Debug.Assert(!string.IsNullOrWhiteSpace(name));
            return EscapeName(name);
        }

        public static string GetEnumConstantDeclValue(this CXCursor cursor)
        {
            Debug.Assert(cursor.Kind == CXCursorKind.CXCursor_EnumConstantDecl);
            return cursor.EnumConstantDeclValue.ToString();
        }

        public static string GetEnumDeclIntegerTypeName(this CXCursor cursor)
        {
            Debug.Assert(cursor.Kind == CXCursorKind.CXCursor_EnumDecl);
            return cursor.EnumDecl_IntegerType.GetName(cursor);
        }

        public static string GetEnumDeclName(this CXCursor cursor)
        {
            Debug.Assert(cursor.Kind == CXCursorKind.CXCursor_EnumDecl);

            var name = cursor.Spelling.ToString();

            if (string.IsNullOrWhiteSpace(name))
            {
                name = cursor.Type.GetName(cursor);
            }

            Debug.Assert(!string.IsNullOrWhiteSpace(name));
            return EscapeName(name);
        }

        public static string GetFieldDeclName(this CXCursor cursor)
        {
            Debug.Assert(cursor.Kind == CXCursorKind.CXCursor_FieldDecl);

            var name = cursor.Spelling.ToString();
            Debug.Assert(!string.IsNullOrWhiteSpace(name));
            return EscapeName(name);
        }

        public static string GetFieldDeclTypeName(this CXCursor cursor)
        {
            Debug.Assert(cursor.Kind == CXCursorKind.CXCursor_FieldDecl);
            return cursor.Type.GetName(cursor);
        }

        public static string GetFunctionDeclName(this CXCursor cursor)
        {
            Debug.Assert(cursor.Kind == CXCursorKind.CXCursor_FunctionDecl);

            var name = cursor.Spelling.ToString();
            Debug.Assert(!string.IsNullOrWhiteSpace(name));
            return EscapeName(name);
        }

        public static string GetParmDeclName(this CXCursor cursor, int index)
        {
            Debug.Assert(cursor.Kind == CXCursorKind.CXCursor_ParmDecl);

            var name = cursor.Spelling.ToString();

            if (string.IsNullOrWhiteSpace(name))
            {
                name = $"param{index}";
            }

            Debug.Assert(!string.IsNullOrWhiteSpace(name));
            return EscapeName(name);
        }

        public static string GetParmDeclTypeName(this CXCursor cursor)
        {
            Debug.Assert(cursor.Kind == CXCursorKind.CXCursor_ParmDecl);

            var name = cursor.Type.GetName(cursor);
            Debug.Assert(!string.IsNullOrWhiteSpace(name));
            return name;
        }

        public static string GetStructDeclName(this CXCursor cursor)
        {
            Debug.Assert(cursor.Kind == CXCursorKind.CXCursor_StructDecl);

            var name = cursor.Spelling.ToString();

            if (string.IsNullOrWhiteSpace(name))
            {
                name = cursor.Type.GetName(cursor);
            }

            Debug.Assert(!string.IsNullOrWhiteSpace(name));
            return EscapeName(name);
        }

        public static string GetTypedefDeclName(this CXCursor cursor)
        {
            Debug.Assert(cursor.Kind == CXCursorKind.CXCursor_TypedefDecl);

            var name = cursor.Spelling.ToString();

            switch (name)
            {
                case "size_t":
                {
                    return "IntPtr";
                }

                case "time_t":
                {
                    return "long";
                }
            }

            Debug.Assert(!string.IsNullOrWhiteSpace(name));
            return EscapeName(name);
        }

        public static void Unhandled(CXCursor cursor, CXCursor parent)
        {
            Debug.WriteLine($"Unhandled cursor kind: {cursor.KindSpelling} in {parent.KindSpelling}.");
            Debugger.Break();
        }
    }
}
