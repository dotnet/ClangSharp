using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using ClangSharp;

namespace ClangSharpPInvokeGenerator
{
    internal class CursorWriter : CursorVisitor
    {
        private readonly Dictionary<CXCursor, object> _attachedData;
        private readonly Stack<CXCursor> _processingCursors;
        private readonly Stack<CXCursor> _predicatedCursors;
        private readonly HashSet<CXCursor> _visitedCursors;
        private readonly Func<CXCursor, bool> _predicate;
        private readonly TextWriter _tw;
        private readonly ConfigurationOptions _config;

        private int _indentation;

        public CursorWriter(ConfigurationOptions config, TextWriter tw, int indentation = 0, Func<CXCursor, bool> predicate = null)
        {
            _attachedData = new Dictionary<CXCursor, object>();
            _processingCursors = new Stack<CXCursor>();
            _predicatedCursors = new Stack<CXCursor>();
            _visitedCursors = new HashSet<CXCursor>();
            _predicate = predicate ?? ((cursor) => true);
            _tw = tw;
            _config = config;

            _indentation = indentation;
        }

        protected override bool BeginHandle(CXCursor cursor, CXCursor parent)
        {
            if (_predicate(cursor))
            {
                _predicatedCursors.Push(cursor);
            }
            else if ((_predicatedCursors.Count == 0) && (cursor.Kind != CXCursorKind.CXCursor_UnexposedDecl))
            {
                return false;
            }

            _processingCursors.Push(cursor);
            _visitedCursors.Add(cursor);

            switch (cursor.Kind)
            {
                case CXCursorKind.CXCursor_UnexposedDecl:
                {
                    if (_predicatedCursors.TryPeek(out var activeCursor) && activeCursor.Equals(cursor))
                    {
                        _predicatedCursors.Pop();
                    }
                    return true;
                }

                case CXCursorKind.CXCursor_StructDecl:
                {
                    return BeginHandleStructDecl(cursor, parent);
                }

                case CXCursorKind.CXCursor_EnumDecl:
                {
                    return BeginHandleEnumDecl(cursor, parent);
                }

                case CXCursorKind.CXCursor_FieldDecl:
                {
                    return BeginHandleFieldDecl(cursor, parent);
                }

                case CXCursorKind.CXCursor_EnumConstantDecl:
                {
                    return BeginHandleEnumConstantDecl(cursor, parent);
                }

                case CXCursorKind.CXCursor_FunctionDecl:
                {
                    return BeginHandleFunctionDecl(cursor, parent);
                }

                case CXCursorKind.CXCursor_ParmDecl:
                {
                    return BeginHandleParmDecl(cursor, parent);
                }

                case CXCursorKind.CXCursor_TypedefDecl:
                {
                    return BeginHandleTypedefDecl(cursor, parent, cursor.TypedefDeclUnderlyingType);
                }

                case CXCursorKind.CXCursor_TypeRef:
                {
                    return BeginHandleTypeRef(cursor, parent);
                }

                case CXCursorKind.CXCursor_UnexposedAttr:
                case CXCursorKind.CXCursor_DLLImport:
                {
                    return false;
                }

                default:
                {
                    Unhandled(cursor, parent);
                    return base.BeginHandle(cursor, parent);
                }
            }
        }

        protected override void EndHandle(CXCursor cursor, CXCursor parent)
        {
            if (!_processingCursors.TryPeek(out var processingCursor) || !cursor.Equals(processingCursor))
            {
                return;
            }

            _processingCursors.Pop();

            if (_predicatedCursors.TryPeek(out var activeCursor) && cursor.Equals(activeCursor))
            {
                _predicatedCursors.Pop();
            }

            switch (cursor.Kind)
            {
                case CXCursorKind.CXCursor_UnexposedDecl:
                case CXCursorKind.CXCursor_TypeRef:
                case CXCursorKind.CXCursor_UnexposedAttr:
                case CXCursorKind.CXCursor_DLLImport:
                {
                    break;
                }

                case CXCursorKind.CXCursor_StructDecl:
                case CXCursorKind.CXCursor_EnumDecl:
                {
                    WriteBlockEnd();
                    WriteLine();
                    break;
                }

                case CXCursorKind.CXCursor_FieldDecl:
                {
                    WriteLine(';');
                    break;
                }

                case CXCursorKind.CXCursor_EnumConstantDecl:
                {
                    WriteLine(',');
                    break;
                }

                case CXCursorKind.CXCursor_FunctionDecl:
                {
                    if (_attachedData.TryGetValue(cursor, out var data) && (data is AttachedFunctionDeclData functionDeclData))
                    {
                        Debug.Assert(functionDeclData.RemainingParmCount == 0);

                        WriteLine(");");
                        WriteLine();

                        _attachedData.Remove(cursor);
                    }
                    else if (!_config.ExcludedFunctions.Contains(GetCursorName(cursor)))
                    {
                        Unhandled(cursor, parent);
                    }
                    break;
                }

                case CXCursorKind.CXCursor_ParmDecl:
                {
                    if (_attachedData.TryGetValue(parent, out var data) && (data is AttachedFunctionDeclData functionDeclData))
                    {
                        functionDeclData.RemainingParmCount -= 1;;

                        if (functionDeclData.RemainingParmCount != 0)
                        {
                            Write(", ");
                        }
                        _attachedData[parent] = functionDeclData;
                    }
                    else if (parent.Kind != CXCursorKind.CXCursor_ParmDecl)
                    {
                        Unhandled(cursor, parent);
                    }
                    break;
                }

                case CXCursorKind.CXCursor_TypedefDecl:
                {
                    if (_attachedData.TryGetValue(cursor, out var data) && (data is AttachedFunctionDeclData functionDeclData))
                    {
                        goto case CXCursorKind.CXCursor_FunctionDecl;
                    }
                    break;
                }

                default:
                {
                    Unhandled(cursor, parent);
                    break;
                }
            }
        }

        private bool BeginHandleEnumConstantDecl(CXCursor cursor, CXCursor parent)
        {
            Debug.Assert(cursor.Kind == CXCursorKind.CXCursor_EnumConstantDecl);

            WriteIndentation();
            Write(GetEscapedCursorName(cursor));
            Write(" = ");
            Write(cursor.EnumConstantDeclValue);

            return false;
        }

        private bool BeginHandleEnumDecl(CXCursor cursor, CXCursor parent)
        {
            Debug.Assert(cursor.Kind == CXCursorKind.CXCursor_EnumDecl);

            WriteIndented("public enum");
            Write(' ');
            Write(GetEscapedCursorName(cursor));

            var integerTypeName = GetTypeName(cursor, cursor.EnumDecl_IntegerType);

            if (!integerTypeName.Equals("int"))
            {
                Write(" : ");
                Write(integerTypeName);
            }

            WriteLine();
            WriteBlockStart();

            return true;
        }

        private bool BeginHandleFieldDecl(CXCursor cursor, CXCursor parent)
        {
            Debug.Assert(cursor.Kind == CXCursorKind.CXCursor_FieldDecl);

            WriteIndentation();

            var marshalAttribute = GetMarshalAttribute(cursor, cursor.Type);

            if (!string.IsNullOrWhiteSpace(marshalAttribute))
            {
                Write('[');
                Write(marshalAttribute);
                Write(']');
                Write(' ');
            }

            long lastElement = -1;

            if (cursor.Type.kind == CXTypeKind.CXType_ConstantArray)
            {
                lastElement = cursor.Type.NumElements - 1;

                for (int i = 0; i < lastElement; i++)
                {
                    Write("public");
                    Write(' ');
                    Write(GetTypeName(cursor, cursor.Type));
                    Write(' ');
                    Write(GetEscapedCursorName(cursor));
                    Write(i);
                    Write(';');
                    Write(' ');
                }
            }

            Write("public");
            Write(' ');
            Write(GetTypeName(cursor, cursor.Type));
            Write(' ');
            Write(GetEscapedCursorName(cursor));

            if (lastElement != -1)
            {
                Write(lastElement);
            }
            return false;
        }

        private bool BeginHandleFunctionDecl(CXCursor cursor, CXCursor parent)
        {
            Debug.Assert(cursor.Kind == CXCursorKind.CXCursor_FunctionDecl);
            Debug.Assert(cursor.Type.kind == CXTypeKind.CXType_FunctionProto);

            var type = cursor.Type;
            var name = GetCursorName(cursor);

            if (_config.ExcludedFunctions.Contains(name))
            {
                return false;
            }

            _attachedData.Add(cursor, new AttachedFunctionDeclData(type.NumArgTypes));

            WriteIndented("[DllImport(libraryPath, EntryPoint = \"");
            Write(name);
            Write("\", CallingConvention = CallingConvention.");
            Write(GetCallingConventionName(cursor, type.FunctionTypeCallingConv));
            WriteLine(")]");

            var marshalAttribute = GetMarshalAttribute(cursor, type.ResultType);

            if (!string.IsNullOrWhiteSpace(marshalAttribute))
            {
                WriteIndented("[return: ");
                Write(marshalAttribute);
                Write(']');
                WriteLine();
            }

            if (name.StartsWith(_config.MethodPrefixToStrip))
            {
                name = name.Substring(_config.MethodPrefixToStrip.Length);
            }
            name = EscapeName(name);

            WriteIndented("public static extern");
            Write(' ');
            Write(GetTypeName(cursor, type.ResultType));
            Write(' ');
            Write(name);
            Write('(');

            return true;
        }

        private bool BeginHandleParmDecl(CXCursor cursor, CXCursor parent)
        {
            Debug.Assert(cursor.Kind == CXCursorKind.CXCursor_ParmDecl);

            if (_attachedData.TryGetValue(parent, out var data) && (data is AttachedFunctionDeclData functionDeclData))
            {
                var marshalAttribute = GetMarshalAttribute(cursor, cursor.Type);

                if (!string.IsNullOrWhiteSpace(marshalAttribute))
                {
                    Write("[");
                    Write(marshalAttribute);
                    Write(']');
                    Write(' ');
                }

                var parmModifier = GetParmModifier(cursor, cursor.Type);

                if (!string.IsNullOrWhiteSpace(parmModifier))
                {
                    Write(parmModifier);
                    Write(' ');
                }

                Write(GetTypeName(cursor, cursor.Type));
                Write(' ');

                var name = GetEscapedCursorName(cursor);
                Write(name);

                if (name.Equals("param"))
                {
                    Write(functionDeclData.ParmCount - functionDeclData.RemainingParmCount);
                }
                return true;
            }
            else if (parent.Kind != CXCursorKind.CXCursor_ParmDecl)
            {
                Unhandled(cursor, parent);
            }

            return false;
        }

        private bool BeginHandleStructDecl(CXCursor cursor, CXCursor parent)
        {
            Debug.Assert(cursor.Kind == CXCursorKind.CXCursor_StructDecl);

            WriteIndented("public");

            if (_config.GenerateUnsafeCode)
            {
                Write(' ');
                Write("unsafe");
            }
            Write(' ');

            Write("partial struct");
            Write(' ');
            WriteLine(GetEscapedCursorName(cursor));
            WriteBlockStart();

            return true;
        }

        private bool BeginHandleTypedefDecl(CXCursor cursor, CXCursor parent, CXType underlyingType)
        {
            Debug.Assert(cursor.Kind == CXCursorKind.CXCursor_TypedefDecl);

            switch (underlyingType.kind)
            {
                case CXTypeKind.CXType_Bool:
                case CXTypeKind.CXType_UShort:
                case CXTypeKind.CXType_UInt:
                case CXTypeKind.CXType_ULong:
                case CXTypeKind.CXType_ULongLong:
                case CXTypeKind.CXType_Short:
                case CXTypeKind.CXType_Int:
                case CXTypeKind.CXType_Long:
                case CXTypeKind.CXType_LongLong:
                case CXTypeKind.CXType_Double:
                {
                    if (!_config.GenerateUnsafeCode)
                    {
                        var escapedName = GetEscapedCursorName(cursor);

                        WriteIndented("public partial struct");
                        Write(' ');
                        WriteLine(escapedName);
                        WriteBlockStart();
                        {
                            var typeName = GetTypeName(cursor, underlyingType);

                            WriteIndented("public");
                            Write(' ');
                            Write(escapedName);
                            Write('(');
                            Write(typeName);
                            Write(' ');
                            Write("value");
                            WriteLine(')');
                            WriteBlockStart();
                            {
                                WriteIndentedLine("Value = value;");
                            }
                            WriteBlockEnd();
                            WriteLine();
                            WriteIndented("public");
                            Write(' ');
                            Write(typeName);
                            Write(' ');
                            Write("Value");
                            WriteLine(';');
                        }
                        WriteBlockEnd();
                        WriteLine();
                    }
                    return true;
                }

                case CXTypeKind.CXType_Pointer:
                {
                    return BeginHandleTypedefDeclForPointer(cursor, parent, underlyingType.PointeeType);
                }

                case CXTypeKind.CXType_Record:
                case CXTypeKind.CXType_Enum:
                {
                    return false;
                }

                case CXTypeKind.CXType_Typedef:
                case CXTypeKind.CXType_Elaborated:
                {
                    return BeginHandleTypedefDecl(cursor, parent, underlyingType.CanonicalType);
                }

                default:
                {
                    Unhandled(cursor, underlyingType);
                    return false;
                }
            }
        }

        private bool BeginHandleTypedefDeclForPointer(CXCursor cursor, CXCursor parent, CXType pointeeType)
        {
            Debug.Assert(cursor.Kind == CXCursorKind.CXCursor_TypedefDecl);

            switch (pointeeType.kind)
            {
                case CXTypeKind.CXType_Void:
                case CXTypeKind.CXType_Record:
                {
                    if (!_config.GenerateUnsafeCode)
                    {
                        WriteIndented("public partial struct");
                        Write(' ');
                        WriteLine(GetEscapedCursorName(cursor));
                        WriteBlockStart();
                        {
                            WriteIndented("public");
                            Write(' ');
                            Write(GetEscapedCursorName(cursor));
                            WriteLine("(IntPtr pointer)");
                            WriteBlockStart();
                            {
                                WriteIndentedLine("Pointer = pointer;");
                            }
                            WriteBlockEnd();
                            WriteLine();
                            WriteIndentedLine("public IntPtr Pointer;");
                        }
                        WriteBlockEnd();
                        WriteLine();
                    }
                    return true;
                }

                case CXTypeKind.CXType_FunctionProto:
                {
                    var name = GetCursorName(cursor);
                    _attachedData.Add(cursor, new AttachedFunctionDeclData(pointeeType.NumArgTypes));
                    name = EscapeName(name);

                    WriteIndented("[UnmanagedFunctionPointer(CallingConvention.");
                    Write(GetCallingConventionName(cursor, pointeeType.FunctionTypeCallingConv));
                    WriteLine(")]");
                    WriteIndented("public delegate");
                    Write(' ');
                    Write(GetTypeName(cursor, pointeeType.ResultType));
                    Write(' ');
                    Write(name);
                    Write('(');

                    return true;
                }

                case CXTypeKind.CXType_Elaborated:
                {
                    return BeginHandleTypedefDeclForPointer(cursor, parent, pointeeType.CanonicalType);
                }

                default:
                {
                    Unhandled(cursor, pointeeType);
                    return false;
                }
            }
        }

        private bool BeginHandleTypeRef(CXCursor cursor, CXCursor parent)
        {
            Debug.Assert(cursor.Kind == CXCursorKind.CXCursor_TypeRef);
            return true;
        }

        private string EscapeName(string name)
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

        private string GetCallingConventionName(CXCursor cursor, CXCallingConv callingConvention)
        {
            switch (callingConvention)
            {
                case CXCallingConv.CXCallingConv_C:
                {
                    return "Cdecl";
                }

                default:
                {
                    Debug.WriteLine($"Unhandled calling convention: {callingConvention} in {cursor.KindSpelling}.");
                    Debugger.Break();
                    return string.Empty;
                }
            }
        }

        private string GetCursorName(CXCursor cursor)
        {
            switch (cursor.Kind)
            {
                case CXCursorKind.CXCursor_StructDecl:
                case CXCursorKind.CXCursor_UnionDecl:
                case CXCursorKind.CXCursor_EnumDecl:
                {
                    var name = cursor.Spelling.ToString();

                    if (string.IsNullOrWhiteSpace(name))
                    {
                        name = GetTypeName(cursor, cursor.Type);
                    }

                    Debug.Assert(!string.IsNullOrWhiteSpace(name));
                    return name;
                }

                case CXCursorKind.CXCursor_FieldDecl:
                case CXCursorKind.CXCursor_EnumConstantDecl:
                case CXCursorKind.CXCursor_FunctionDecl:
                {
                    var name = cursor.Spelling.ToString();
                    Debug.Assert(!string.IsNullOrWhiteSpace(name));
                    return name;
                }

                case CXCursorKind.CXCursor_ParmDecl:
                {
                    var name = cursor.Spelling.ToString();

                    if (string.IsNullOrWhiteSpace(name))
                    {
                        name = "param";
                    }

                    Debug.Assert(!string.IsNullOrWhiteSpace(name));
                    return name;
                }

                case CXCursorKind.CXCursor_TypedefDecl:
                {
                    switch (cursor.Spelling.ToString())
                    {
                        case "int8_t":
                        {
                            return "sbyte";
                        }

                        case "int16_t":
                        {
                            return "short";
                        }

                        case "int32_t":
                        {
                            return "int";
                        }

                        case "int64_t":
                        {
                            return "long";
                        }

                        case "intptr_t":
                        {
                            return "IntPtr";
                        }

                        case "size_t":
                        case "SIZE_T":
                        {
                            return "IntPtr";
                        }

                        case "time_t":
                        {
                            return "long";
                        }

                        case "uint8_t":
                        {
                            return "byte";
                        }

                        case "uint16_t":
                        {
                            return "ushort";
                        }

                        case "uint32_t":
                        {
                            return "uint";
                        }

                        case "uint64_t":
                        {
                            return "ulong";
                        }

                        case "uintptr_t":
                        {
                            return "UIntPtr";
                        }

                        default:
                        {
                            return GetCursorNameForTypedefDecl(cursor, cursor.TypedefDeclUnderlyingType);
                        }
                    }
                }

                default:
                {
                    Unhandled(cursor);
                    return string.Empty;
                }
            }
        }

        private string GetCursorNameForTypedefDecl(CXCursor cursor, CXType underlyingType)
        {
            Debug.Assert(cursor.Kind == CXCursorKind.CXCursor_TypedefDecl);

            switch (underlyingType.kind)
            {
                case CXTypeKind.CXType_Bool:
                case CXTypeKind.CXType_UChar:
                case CXTypeKind.CXType_UShort:
                case CXTypeKind.CXType_UInt:
                case CXTypeKind.CXType_ULong:
                case CXTypeKind.CXType_ULongLong:
                case CXTypeKind.CXType_WChar:
                case CXTypeKind.CXType_Short:
                case CXTypeKind.CXType_Int:
                case CXTypeKind.CXType_Long:
                case CXTypeKind.CXType_LongLong:
                case CXTypeKind.CXType_Float:
                case CXTypeKind.CXType_Pointer:
                {
                    var name = cursor.Spelling.ToString();

                    if (_config.GenerateUnsafeCode || string.IsNullOrWhiteSpace(name))
                    {
                        name = GetTypeName(cursor, underlyingType);
                    }

                    Debug.Assert(!string.IsNullOrWhiteSpace(name));
                    return name;
                }

                case CXTypeKind.CXType_Record:
                case CXTypeKind.CXType_Enum:
                {
                    var name = GetTypeName(cursor, underlyingType);
                    Debug.Assert(!string.IsNullOrWhiteSpace(name));
                    return name;
                }

                case CXTypeKind.CXType_Typedef:
                case CXTypeKind.CXType_Elaborated:
                {
                    return GetCursorNameForTypedefDecl(cursor, underlyingType.CanonicalType);
                }

                default:
                {
                    Unhandled(cursor, underlyingType);
                    return string.Empty;
                }
            }
        }

        private string GetEscapedCursorName(CXCursor cursor)
        {
            var name = GetCursorName(cursor);
            return EscapeName(name);
        }

        private string GetMarshalAttribute(CXCursor cursor, CXType type)
        {
            if (_config.GenerateUnsafeCode)
            {
                return string.Empty;
            }

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
                case CXTypeKind.CXType_Record:
                case CXTypeKind.CXType_Enum:
                case CXTypeKind.CXType_Typedef:
                case CXTypeKind.CXType_ConstantArray:
                case CXTypeKind.CXType_IncompleteArray:
                {
                    return string.Empty;
                }

                case CXTypeKind.CXType_Pointer:
                {
                    return GetMarshalAttributeForPointeeType(cursor, type.PointeeType);
                }

                case CXTypeKind.CXType_Elaborated:
                {
                    return GetMarshalAttribute(cursor, type.CanonicalType);
                }

                default:
                {
                    Unhandled(cursor, type);
                    return string.Empty;
                }
            }
        }

        private string GetMarshalAttributeForPointeeType(CXCursor cursor, CXType pointeeType)
        {
            Debug.Assert(!_config.GenerateUnsafeCode);

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
                case CXTypeKind.CXType_Record:
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
                    return GetMarshalAttributeForPointeeType(cursor, pointeeType.CanonicalType);
                }

                default:
                {
                    Unhandled(cursor, pointeeType);
                    return string.Empty;
                }
            }
        }

        private string GetParmModifier(CXCursor cursor, CXType type)
        {
            if (_config.GenerateUnsafeCode)
            {
                return string.Empty;
            }

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
                {
                    return string.Empty;
                }

                case CXTypeKind.CXType_Pointer:
                {
                    return GetParmModifierForPointeeType(cursor, type.PointeeType);
                }

                case CXTypeKind.CXType_ConstantArray:
                case CXTypeKind.CXType_IncompleteArray:
                {
                    return "out";
                }

                case CXTypeKind.CXType_Elaborated:
                {
                    return GetParmModifier(cursor, type.CanonicalType);
                }

                default:
                {
                    Unhandled(cursor, type);
                    return string.Empty;
                }
            }
        }

        private string GetParmModifierForPointeeType(CXCursor cursor, CXType pointeeType)
        {
            Debug.Assert(!_config.GenerateUnsafeCode);

            switch (pointeeType.kind)
            {
                case CXTypeKind.CXType_Void:
                case CXTypeKind.CXType_Char_S:
                case CXTypeKind.CXType_WChar:
                case CXTypeKind.CXType_FunctionProto:
                {
                    return string.Empty;
                }

                case CXTypeKind.CXType_UChar:
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
                    return GetParmModifierForPointeeType(cursor, pointeeType.CanonicalType);
                }

                default:
                {
                    Unhandled(cursor, pointeeType);
                    return string.Empty;
                }
            }
        }

        private string GetTypeName(CXCursor cursor, CXType type)
        {
            switch (type.kind)
            {
                case CXTypeKind.CXType_Void:
                {
                    return "void";
                }

                case CXTypeKind.CXType_Bool:
                {
                    return "bool";
                }

                case CXTypeKind.CXType_UChar:
                {
                    return "byte";
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

                case CXTypeKind.CXType_WChar:
                {
                    return "char";
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

                case CXTypeKind.CXType_Float:
                {
                    return "float";
                }

                case CXTypeKind.CXType_Double:
                {
                    return "double";
                }

                case CXTypeKind.CXType_Pointer:
                case CXTypeKind.CXType_LValueReference:
                {
                    return GetTypeNameForPointeeType(cursor, type.PointeeType);
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
                    return GetCursorName(type.Declaration);
                }

                case CXTypeKind.CXType_Elaborated:
                {
                    return GetTypeName(cursor, type.CanonicalType);
                }

                case CXTypeKind.CXType_ConstantArray:
                case CXTypeKind.CXType_IncompleteArray:
                {
                    return GetTypeName(cursor, type.ElementType);
                }

                default:
                {
                    Unhandled(cursor, type);
                    return string.Empty;
                }
            }
        }

        private string GetTypeNameForPointeeType(CXCursor cursor, CXType pointeeType)
        {
            switch (pointeeType.kind)
            {
                case CXTypeKind.CXType_Void:
                {
                    return _config.GenerateUnsafeCode ? "void*" : "IntPtr";
                }

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
                case CXTypeKind.CXType_Typedef:
                {
                    switch (cursor.Kind)
                    {
                        case CXCursorKind.CXCursor_FieldDecl:
                        case CXCursorKind.CXCursor_FunctionDecl:
                        {
                            var name = "IntPtr";

                            if (_config.GenerateUnsafeCode)
                            {
                                name = GetTypeName(cursor, pointeeType);
                                name += '*';
                            }
                            return name;
                        }

                        case CXCursorKind.CXCursor_ParmDecl:
                        {
                            var name = GetTypeName(cursor, pointeeType);

                            if (_config.GenerateUnsafeCode)
                            {
                                name += '*';
                            }
                            return name;
                        }

                        case CXCursorKind.CXCursor_TypedefDecl:
                        {
                            if (_attachedData.TryGetValue(cursor, out var data) && (data is AttachedFunctionDeclData functionDeclData))
                            {
                                goto case CXCursorKind.CXCursor_FunctionDecl;
                            }

                            var name = GetCursorName(pointeeType.Declaration);

                            if (_config.GenerateUnsafeCode)
                            {
                                name += '*';
                            }
                            return name;
                        }

                        default:
                        {
                            Unhandled(cursor, pointeeType);
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
                            return _config.GenerateUnsafeCode ? "byte*" : "string";
                        }

                        case CXCursorKind.CXCursor_ParmDecl:
                        {
                            if (GetParmModifier(cursor, cursor.Type).Equals("out"))
                            {
                                Debug.Assert(!_config.GenerateUnsafeCode);
                                return "IntPtr";
                            }

                            return _config.GenerateUnsafeCode ? "byte*" : "string";
                        }

                        case CXCursorKind.CXCursor_TypedefDecl:
                        {
                            return _config.GenerateUnsafeCode ? "byte*" : "string";
                        }

                        default:
                        {
                            Unhandled(cursor, pointeeType);
                            return string.Empty;
                        }
                    }
                }

                case CXTypeKind.CXType_Elaborated:
                {
                    return GetTypeNameForPointeeType(cursor, pointeeType.CanonicalType);
                }

                default:
                {
                    Unhandled(cursor, pointeeType);
                    return string.Empty;
                }
            }
        }

        private void Unhandled(CXCursor cursor)
        {
            Debug.WriteLine($"Unhandled cursor kind: {cursor.KindSpelling}");
            Debugger.Break();
        }

        private CXChildVisitResult Unhandled(CXCursor cursor, CXType type)
        {
            Debug.WriteLine($"Unhandled type kind: {type.KindSpelling} in {cursor.KindSpelling}.");
            Debugger.Break();
            return CXChildVisitResult.CXChildVisit_Break;
        }

        private void WriteBlockStart()
        {
            WriteIndentedLine('{');
            _indentation++;
        }

        private void WriteBlockEnd()
        {
            _indentation--;
            WriteIndentedLine('}');
        }

        private void Write<T>(T value)
        {
            _tw.Write(value);
        }

        private void WriteIndentation()
        {
            for (var i = 0; i < _indentation; i++)
            {
                _tw.Write("    ");
            }
        }

        private void WriteIndented<T>(T value)
        {
            WriteIndentation();
            Write(value);
        }

        private void WriteIndentedLine<T>(T value)
        {
            WriteIndentation();
            WriteLine(value);
        }

        private void WriteLine<T>(T value)
        {
            Write(value);
            WriteLine();
        }

        private void WriteLine()
        {
            _tw.WriteLine();
        }
    }
}
