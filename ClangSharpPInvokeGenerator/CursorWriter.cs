using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using ClangSharp;

namespace ClangSharpPInvokeGenerator
{
    internal class CursorWriter : CursorVisitor, IDisposable
    {
        private readonly Dictionary<CXCursor, object> _attachedData;
        private readonly Dictionary<string, OutputBuilder> _outputBuilders;
        private readonly Stack<CXCursor> _processingCursors;
        private readonly Stack<CXCursor> _predicatedCursors;
        private readonly HashSet<CXCursor> _visitedCursors;
        private readonly Func<CXCursor, bool> _predicate;
        private readonly ConfigurationOptions _config;

        private OutputBuilder _outputBuilder;

        public CursorWriter(ConfigurationOptions config, Func<CXCursor, bool> predicate = null)
        {
            _attachedData = new Dictionary<CXCursor, object>();
            _outputBuilders = new Dictionary<string, OutputBuilder>();
            _processingCursors = new Stack<CXCursor>();
            _predicatedCursors = new Stack<CXCursor>();
            _visitedCursors = new HashSet<CXCursor>();
            _predicate = predicate ?? ((cursor) => true);
            _config = config;
        }

        public void Dispose()
        {
            Debug.Assert(_outputBuilder is null);

            foreach (var outputBuilder in _outputBuilders)
            {
                Debug.Assert(outputBuilder.Key.Equals(outputBuilder.Value.OutputFile));
                outputBuilder.Value.Dispose();
            }

            _outputBuilders.Clear();

            Debug.Assert(_attachedData.Count == 0);
            Debug.Assert(_processingCursors.Count == 0);
            Debug.Assert(_predicatedCursors.Count == 0);

            _visitedCursors.Clear();
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
            bool clearOutputBuilder = false;

            if (!_processingCursors.TryPeek(out var processingCursor) || !cursor.Equals(processingCursor))
            {
                return;
            }

            _processingCursors.Pop();

            if (_predicatedCursors.TryPeek(out var activeCursor) && cursor.Equals(activeCursor))
            {
                _predicatedCursors.Pop();
                clearOutputBuilder = true;
            }

            switch (cursor.Kind)
            {
                case CXCursorKind.CXCursor_UnexposedDecl:
                case CXCursorKind.CXCursor_TypeRef:
                case CXCursorKind.CXCursor_UnexposedAttr:
                case CXCursorKind.CXCursor_DLLImport:
                {
                    clearOutputBuilder = false;
                    break;
                }

                case CXCursorKind.CXCursor_StructDecl:
                case CXCursorKind.CXCursor_EnumDecl:
                {
                    _outputBuilder.WriteBlockEnd();
                    break;
                }

                case CXCursorKind.CXCursor_FieldDecl:
                {
                    _outputBuilder.WriteLine(';');

                    clearOutputBuilder = false;
                    break;
                }

                case CXCursorKind.CXCursor_EnumConstantDecl:
                {
                    _outputBuilder.WriteLine(',');

                    clearOutputBuilder = false;
                    break;
                }

                case CXCursorKind.CXCursor_FunctionDecl:
                {
                    if (_attachedData.TryGetValue(cursor, out var data) && (data is AttachedFunctionDeclData functionDeclData))
                    {
                        Debug.Assert(functionDeclData.RemainingParmCount == 0);

                        _outputBuilder.WriteLine(");");

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
                            _outputBuilder.Write(", ");
                        }
                        _attachedData[parent] = functionDeclData;
                    }
                    else if (parent.Kind != CXCursorKind.CXCursor_ParmDecl)
                    {
                        Unhandled(cursor, parent);
                    }

                    clearOutputBuilder = false;
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

            if (clearOutputBuilder)
            {
                _outputBuilder = null;
            }
        }

        private bool BeginHandleEnumConstantDecl(CXCursor cursor, CXCursor parent)
        {
            Debug.Assert(cursor.Kind == CXCursorKind.CXCursor_EnumConstantDecl);

            var name = GetCursorName(cursor);

            _outputBuilder.WriteIndentation();
            _outputBuilder.Write(EscapeName(name));
            _outputBuilder.Write(" = ");
            _outputBuilder.Write(cursor.EnumConstantDeclValue);

            return false;
        }

        private bool BeginHandleEnumDecl(CXCursor cursor, CXCursor parent)
        {
            Debug.Assert(cursor.Kind == CXCursorKind.CXCursor_EnumDecl);

            var name = GetCursorName(cursor);
            InitializeOutputBuilder(name);

            _outputBuilder.WriteIndented("public enum");
            _outputBuilder.Write(' ');
            _outputBuilder.Write(EscapeName(name));

            var integerTypeName = GetTypeName(cursor, cursor.EnumDecl_IntegerType);

            if (!integerTypeName.Equals("int"))
            {
                _outputBuilder.Write(" : ");
                _outputBuilder.Write(integerTypeName);
            }

            _outputBuilder.WriteLine();
            _outputBuilder.WriteBlockStart();

            return true;
        }

        private bool BeginHandleFieldDecl(CXCursor cursor, CXCursor parent)
        {
            Debug.Assert(cursor.Kind == CXCursorKind.CXCursor_FieldDecl);

            _outputBuilder.WriteIndentation();

            var marshalAttribute = GetMarshalAttribute(cursor, cursor.Type);

            if (!string.IsNullOrWhiteSpace(marshalAttribute))
            {
                _outputBuilder.AddUsing("System.Runtime.InteropServices");

                _outputBuilder.Write('[');
                _outputBuilder.Write(marshalAttribute);
                _outputBuilder.Write(']');
                _outputBuilder.Write(' ');
            }

            long lastElement = -1;

            var name = GetCursorName(cursor);
            var escapedName = EscapeName(name);

            if (cursor.Type.kind == CXTypeKind.CXType_ConstantArray)
            {
                lastElement = cursor.Type.NumElements - 1;

                for (int i = 0; i < lastElement; i++)
                {
                    _outputBuilder.Write("public");
                    _outputBuilder.Write(' ');
                    _outputBuilder.Write(GetTypeName(cursor, cursor.Type));
                    _outputBuilder.Write(' ');
                    _outputBuilder.Write(escapedName);
                    _outputBuilder.Write(i);
                    _outputBuilder.Write(';');
                    _outputBuilder.Write(' ');
                }
            }

            _outputBuilder.Write("public");
            _outputBuilder.Write(' ');
            _outputBuilder.Write(GetTypeName(cursor, cursor.Type));
            _outputBuilder.Write(' ');
            _outputBuilder.Write(escapedName);

            if (lastElement != -1)
            {
                _outputBuilder.Write(lastElement);
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
            InitializeOutputBuilder(_config.MethodClassName);

            _attachedData.Add(cursor, new AttachedFunctionDeclData(type.NumArgTypes));

            _outputBuilder.AddUsing("System.Runtime.InteropServices");

            _outputBuilder.WriteIndented("[DllImport(libraryPath, EntryPoint = \"");
            _outputBuilder.Write(name);
            _outputBuilder.Write("\", CallingConvention = CallingConvention.");
            _outputBuilder.Write(GetCallingConventionName(cursor, type.FunctionTypeCallingConv));
            _outputBuilder.WriteLine(")]");

            var marshalAttribute = GetMarshalAttribute(cursor, type.ResultType);

            if (!string.IsNullOrWhiteSpace(marshalAttribute))
            {
                _outputBuilder.WriteIndented("[return: ");
                _outputBuilder.Write(marshalAttribute);
                _outputBuilder.Write(']');
                _outputBuilder.WriteLine();
            }

            if (name.StartsWith(_config.MethodPrefixToStrip))
            {
                name = name.Substring(_config.MethodPrefixToStrip.Length);
            }
            name = EscapeName(name);

            _outputBuilder.WriteIndented("public static extern");
            _outputBuilder.Write(' ');
            _outputBuilder.Write(GetTypeName(cursor, type.ResultType));
            _outputBuilder.Write(' ');
            _outputBuilder.Write(name);
            _outputBuilder.Write('(');

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
                    _outputBuilder.Write("[");
                    _outputBuilder.Write(marshalAttribute);
                    _outputBuilder.Write(']');
                    _outputBuilder.Write(' ');
                }

                var parmModifier = GetParmModifier(cursor, cursor.Type);

                if (!string.IsNullOrWhiteSpace(parmModifier))
                {
                    _outputBuilder.Write(parmModifier);
                    _outputBuilder.Write(' ');
                }

                _outputBuilder.Write(GetTypeName(cursor, cursor.Type));
                _outputBuilder.Write(' ');

                var name = GetCursorName(cursor);
                _outputBuilder.Write(EscapeName(name));

                if (name.Equals("param"))
                {
                    _outputBuilder.Write(functionDeclData.ParmCount - functionDeclData.RemainingParmCount);
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

            var name = GetCursorName(cursor);
            InitializeOutputBuilder(name);

            _outputBuilder.WriteIndented("public");

            if (_config.GenerateUnsafeCode)
            {
                _outputBuilder.Write(' ');
                _outputBuilder.Write("unsafe");
            }
            _outputBuilder.Write(' ');

            _outputBuilder.Write("partial struct");
            _outputBuilder.Write(' ');
            _outputBuilder.WriteLine(EscapeName(name));
            _outputBuilder.WriteBlockStart();

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
                        var name = GetCursorName(cursor);
                        var escapedName = EscapeName(name);

                        _outputBuilder.WriteIndented("public partial struct");
                        _outputBuilder.Write(' ');
                        _outputBuilder.WriteLine(escapedName);
                        _outputBuilder.WriteBlockStart();
                        {
                            var typeName = GetTypeName(cursor, underlyingType);

                            _outputBuilder.WriteIndented("public");
                            _outputBuilder.Write(' ');
                            _outputBuilder.Write(escapedName);
                            _outputBuilder.Write('(');
                            _outputBuilder.Write(typeName);
                            _outputBuilder.Write(' ');
                            _outputBuilder.Write("value");
                            _outputBuilder.WriteLine(')');
                            _outputBuilder.WriteBlockStart();
                            {
                                _outputBuilder.WriteIndentedLine("Value = value;");
                            }
                            _outputBuilder.WriteBlockEnd();
                            _outputBuilder.WriteLine();
                            _outputBuilder.WriteIndented("public");
                            _outputBuilder.Write(' ');
                            _outputBuilder.Write(typeName);
                            _outputBuilder.Write(' ');
                            _outputBuilder.Write("Value");
                            _outputBuilder.WriteLine(';');
                        }
                        _outputBuilder.WriteBlockEnd();
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
                        var name = GetCursorName(cursor);
                        InitializeOutputBuilder(name);

                        var escapedName = EscapeName(name);

                        _outputBuilder.AddUsing("System");

                        _outputBuilder.WriteIndented("public partial struct");
                        _outputBuilder.Write(' ');
                        _outputBuilder.WriteLine(escapedName);
                        _outputBuilder.WriteBlockStart();
                        {
                            _outputBuilder.WriteIndented("public");
                            _outputBuilder.Write(' ');
                            _outputBuilder.Write(escapedName);
                            _outputBuilder.WriteLine("(IntPtr pointer)");
                            _outputBuilder.WriteBlockStart();
                            {
                                _outputBuilder.WriteIndentedLine("Pointer = pointer;");
                            }
                            _outputBuilder.WriteBlockEnd();
                            _outputBuilder.WriteLine();
                            _outputBuilder.WriteIndentedLine("public IntPtr Pointer;");
                        }
                        _outputBuilder.WriteBlockEnd();
                    }
                    return true;
                }

                case CXTypeKind.CXType_FunctionProto:
                {
                    var name = GetCursorName(cursor);
                    InitializeOutputBuilder(name);

                    _attachedData.Add(cursor, new AttachedFunctionDeclData(pointeeType.NumArgTypes));
                    var escapedName = EscapeName(name);

                    _outputBuilder.AddUsing("System.Runtime.InteropServices");

                    _outputBuilder.WriteIndented("[UnmanagedFunctionPointer(CallingConvention.");
                    _outputBuilder.Write(GetCallingConventionName(cursor, pointeeType.FunctionTypeCallingConv));
                    _outputBuilder.WriteLine(")]");
                    _outputBuilder.WriteIndented("public delegate");
                    _outputBuilder.Write(' ');
                    _outputBuilder.Write(GetTypeName(cursor, pointeeType.ResultType));
                    _outputBuilder.Write(' ');
                    _outputBuilder.Write(escapedName);
                    _outputBuilder.Write('(');

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
                            _outputBuilder.AddUsing("System");
                            return "IntPtr";
                        }

                        case "size_t":
                        case "SIZE_T":
                        {
                            _outputBuilder.AddUsing("System");
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
                            _outputBuilder.AddUsing("System");
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
                    if (_config.GenerateUnsafeCode)
                    {
                        return "void*";
                    }

                    _outputBuilder.AddUsing("System");
                    return "IntPtr";
                }

                case CXTypeKind.CXType_FunctionProto:
                {
                    _outputBuilder.AddUsing("System");
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
                            else
                            {
                                _outputBuilder.AddUsing("System");
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
                                _outputBuilder.AddUsing("System");
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

        private void InitializeOutputBuilder(string name)
        {
            if (_outputBuilder != null)
            {
                return;
            }

            var outputFile = _config.OutputLocation;
            var isMethodClass = name.Equals(_config.MethodClassName);

            if (_config.GenerateMultipleFiles)
            {
                outputFile = Path.Combine(outputFile, $"{name}.cs");
            }
            else if (isMethodClass)
            {
                outputFile = Path.ChangeExtension(outputFile, $"{_config.MethodClassName}{Path.GetExtension(outputFile)}");
            }

            if (!_outputBuilders.TryGetValue(outputFile, out _outputBuilder))
            {
                _outputBuilder = new OutputBuilder(outputFile, _config, isMethodClass);
                _outputBuilders.Add(outputFile, _outputBuilder);
            }
            else
            {
                _outputBuilder.WriteLine();
            }

            Debug.Assert(outputFile.Equals(_outputBuilder.OutputFile));
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
    }
}
