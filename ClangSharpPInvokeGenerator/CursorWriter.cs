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
                case CXCursorKind.CXCursor_FieldDecl:
                case CXCursorKind.CXCursor_EnumConstantDecl:
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

                case CXCursorKind.CXCursor_FunctionDecl:
                {
                    if (_attachedData.TryGetValue(cursor, out var data) && (data is AttachedFunctionDeclData functionDeclData))
                    {
                        Debug.Assert(functionDeclData.RemainingParmCount == 0);

                        WriteLine(");");
                        WriteLine();

                        _attachedData.Remove(cursor);
                    }
                    else if (!_config.ExcludedFunctions.Contains(cursor.GetFunctionDeclName()))
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
            Write(cursor.GetEnumConstantDeclName());
            Write(" = ");
            Write(cursor.GetEnumConstantDeclValue());
            WriteLine(',');

            return false;
        }

        private bool BeginHandleEnumDecl(CXCursor cursor, CXCursor parent)
        {
            Debug.Assert(cursor.Kind == CXCursorKind.CXCursor_EnumDecl);

            WriteIndented("public enum");
            Write(' ');
            Write(cursor.GetEnumDeclName());

            var integerTypeName = cursor.GetEnumDeclIntegerTypeName();

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

            var marshalAttribute = cursor.Type.GetMarshalAttribute(cursor);

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
                    Write(cursor.GetFieldDeclTypeName());
                    Write(' ');
                    Write(cursor.GetFieldDeclName());
                    Write(i);
                    Write(';');
                    Write(' ');
                }
            }

            Write("public");
            Write(' ');
            Write(cursor.GetFieldDeclTypeName());
            Write(' ');
            Write(cursor.GetFieldDeclName());

            if (lastElement != -1)
            {
                Write(lastElement);
            }
            WriteLine(';');

            return false;
        }

        private bool BeginHandleFunctionDecl(CXCursor cursor, CXCursor parent)
        {
            Debug.Assert(cursor.Kind == CXCursorKind.CXCursor_FunctionDecl);
            Debug.Assert(cursor.Type.kind == CXTypeKind.CXType_FunctionProto);

            var type = cursor.Type;
            var name = cursor.GetFunctionDeclName();

            if (_config.ExcludedFunctions.Contains(name))
            {
                return false;
            }

            _attachedData.Add(cursor, new AttachedFunctionDeclData(type.NumArgTypes));

            WriteIndented("[DllImport(libraryPath, EntryPoint = \"");
            Write(name);
            Write("\", CallingConvention = CallingConvention.");
            Write(type.GetFunctionProtoCallingConventionName(cursor));
            WriteLine(")]");

            var marshalAttribute = type.ResultType.GetMarshalAttribute(cursor);

            if (!string.IsNullOrWhiteSpace(marshalAttribute))
            {
                WriteIndented("[return: ");
                Write(marshalAttribute);
                Write(']');
                WriteLine();
            }

            WriteIndented("public static extern");
            Write(' ');
            Write(type.ResultType.GetName(cursor));
            Write(' ');
            Write(name.StartsWith(_config.MethodPrefixToStrip) ? name.Substring(_config.MethodPrefixToStrip.Length) : name);
            Write('(');

            return true;
        }

        private bool BeginHandleParmDecl(CXCursor cursor, CXCursor parent)
        {
            Debug.Assert(cursor.Kind == CXCursorKind.CXCursor_ParmDecl);

            if (_attachedData.TryGetValue(parent, out var data) && (data is AttachedFunctionDeclData functionDeclData))
            {
                var marshalAttribute = cursor.Type.GetMarshalAttribute(cursor);

                if (!string.IsNullOrWhiteSpace(marshalAttribute))
                {
                    Write("[");
                    Write(marshalAttribute);
                    Write(']');
                    Write(' ');
                }

                var parmModifier = cursor.Type.GetParmModifier(cursor);

                if (!string.IsNullOrWhiteSpace(parmModifier))
                {
                    Write(parmModifier);
                    Write(' ');
                }

                Write(cursor.GetParmDeclTypeName());
                Write(' ');
                Write(cursor.GetParmDeclName(functionDeclData.ParmCount - functionDeclData.RemainingParmCount));

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

            WriteIndented("public partial struct");
            Write(' ');
            WriteLine(cursor.GetStructDeclName());
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
                    WriteIndented("public partial struct");
                    Write(' ');
                    WriteLine(cursor.GetTypedefDeclName());
                    WriteBlockStart();
                    {
                        var typeName = underlyingType.GetName(cursor);

                        WriteIndented("public");
                        Write(' ');
                        Write(cursor.GetTypedefDeclName());
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
                    Unhandled(underlyingType, cursor);
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
                    WriteIndented("public partial struct");
                    Write(' ');
                    WriteLine(cursor.GetTypedefDeclName());
                    WriteBlockStart();
                    {
                        WriteIndented("public");
                        Write(' ');
                        Write(cursor.GetTypedefDeclName());
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
                    return true;
                }

                case CXTypeKind.CXType_FunctionProto:
                {
                    _attachedData.Add(cursor, new AttachedFunctionDeclData(pointeeType.NumArgTypes));

                    WriteIndented("[UnmanagedFunctionPointer(CallingConvention.");
                    Write(pointeeType.GetFunctionProtoCallingConventionName(cursor));
                    WriteLine(")]");
                    WriteIndented("public delegate");
                    Write(' ');
                    Write(pointeeType.ResultType.GetName(cursor));
                    Write(' ');
                    Write(cursor.GetTypedefDeclName());
                    Write('(');

                    return true;
                }

                case CXTypeKind.CXType_Elaborated:
                {
                    return BeginHandleTypedefDeclForPointer(cursor, parent, pointeeType.CanonicalType);
                }

                default:
                {
                    Unhandled(pointeeType, cursor);
                    return false;
                }
            }
        }

        private bool BeginHandleTypeRef(CXCursor cursor, CXCursor parent)
        {
            Debug.Assert(cursor.Kind == CXCursorKind.CXCursor_TypeRef);
            return true;
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
