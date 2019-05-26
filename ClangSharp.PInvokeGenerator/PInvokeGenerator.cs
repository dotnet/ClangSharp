using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace ClangSharp
{
    public sealed class PInvokeGenerator : IDisposable
    {
        private const int DefaultStreamWriterBufferSize = 1024;
        private static readonly Encoding DefaultStreamWriterEncoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: true);

        private readonly CXIndex _index;
        private readonly OutputBuilderFactory _outputBuilderFactory;
        private readonly Func<string, (Stream stream, bool leaveOpen)> _outputStreamFactory;
        private readonly HashSet<Cursor> _visitedCursors;
        private readonly List<Diagnostic> _diagnostics;
        private readonly PInvokeGeneratorConfiguration _config;

        private OutputBuilder _outputBuilder;
        private int _outputBuilderUsers;
        private bool _disposed;

        public PInvokeGenerator(PInvokeGeneratorConfiguration config, Func<string, (Stream stream, bool leaveOpen)> outputStreamFactory = null)
        {
            if (config is null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            _index = CXIndex.Create();
            _outputBuilderFactory = new OutputBuilderFactory();
            _outputStreamFactory = outputStreamFactory ?? ((path) => {
                var directoryPath = Path.GetDirectoryName(path);
                Directory.CreateDirectory(directoryPath);
                return (new FileStream(path, FileMode.Create), leaveOpen: false);
            });
            _visitedCursors = new HashSet<Cursor>();
            _diagnostics = new List<Diagnostic>();
            _config = config;
        }

        ~PInvokeGenerator()
        {
            Dispose(isDisposing: false);
        }

        public PInvokeGeneratorConfiguration Config => _config;

        public IReadOnlyList<Diagnostic> Diagnostics => _diagnostics;

        public CXIndex IndexHandle => _index;

        public void Close()
        {
            OutputBuilder methodClassOutputBuilder = null;
            bool emitNamespaceDeclaration = true;

            foreach (var outputBuilder in _outputBuilderFactory.OutputBuilders)
            {
                var outputPath = _config.OutputLocation;
                var isMethodClass = _config.MethodClassName.Equals(outputBuilder.Name);

                if (_config.GenerateMultipleFiles)
                {
                    outputPath = Path.Combine(outputPath, $"{outputBuilder.Name}.cs");
                    emitNamespaceDeclaration = true;
                }
                else if (isMethodClass)
                {
                    methodClassOutputBuilder = outputBuilder;
                    continue;
                }

                var (stream, leaveStreamOpen) = _outputStreamFactory(outputPath);
                CloseOutputBuilder(stream, outputBuilder, isMethodClass, leaveStreamOpen, emitNamespaceDeclaration);
                emitNamespaceDeclaration = false;
            }

            if (!_config.GenerateMultipleFiles)
            {
                var outputPath = _config.OutputLocation;
                var (stream, leaveStreamOpen) = _outputStreamFactory(outputPath);

                if (methodClassOutputBuilder != null)
                {
                    CloseOutputBuilder(stream, methodClassOutputBuilder, isMethodClass: true, leaveStreamOpen: true, emitNamespaceDeclaration);
                }

                using (var sw = new StreamWriter(stream, DefaultStreamWriterEncoding, DefaultStreamWriterBufferSize, leaveStreamOpen))
                {
                    sw.WriteLine('}');
                }
            }

            _diagnostics.Clear();
            _outputBuilderFactory.Clear();
            _visitedCursors.Clear();
        }

        public void Dispose()
        {
            Dispose(isDisposing: true);
            GC.SuppressFinalize(this);
        }

        public void GenerateBindings(CXTranslationUnit translationUnitHandle)
        {
            Debug.Assert(_outputBuilder is null);

            if (translationUnitHandle.NumDiagnostics != 0)
            {
                var errorDiagnostics = new StringBuilder();
                errorDiagnostics.AppendLine($"The provided {nameof(CXTranslationUnit)} has the following diagnostics which prevent its use:");
                var invalidTranslationUnitHandle = false;

                for (uint i = 0; i < translationUnitHandle.NumDiagnostics; ++i)
                {
                    using (var diagnostic = translationUnitHandle.GetDiagnostic(i))
                    {
                        if ((diagnostic.Severity == CXDiagnosticSeverity.CXDiagnostic_Error) || (diagnostic.Severity == CXDiagnosticSeverity.CXDiagnostic_Fatal))
                        {
                            invalidTranslationUnitHandle = true;
                            errorDiagnostics.Append(' ', 4);
                            errorDiagnostics.AppendLine(diagnostic.Format(CXDiagnosticDisplayOptions.CXDiagnostic_DisplayOption).ToString());
                        }
                    }
                }

                if (invalidTranslationUnitHandle)
                {
                    throw new ArgumentOutOfRangeException(nameof(translationUnitHandle), errorDiagnostics.ToString());
                }
            }

            var translationUnit = new TranslationUnit(translationUnitHandle.Cursor);
            translationUnit.Visit(clientData: default);

            _visitedCursors.Add(translationUnit);

            foreach (var child in translationUnit.Children)
            {
                if (!child.IsFromMainFile)
                {
                    continue;
                }

                Visit(child, translationUnit);
            }
        }

        private void AddDiagnostic(DiagnosticLevel level, string message, Cursor cursor)
        {
            var diagnostic = new Diagnostic(level, message, cursor.Location);
            _diagnostics.Add(diagnostic);

            if (level != DiagnosticLevel.Info)
            {
                Debugger.Break();
            }
        }

        private void CloseOutputBuilder(Stream stream, OutputBuilder outputBuilder, bool isMethodClass, bool leaveStreamOpen, bool emitNamespaceDeclaration)
        {
            if (stream is null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            if (outputBuilder is null)
            {
                throw new ArgumentNullException(nameof(outputBuilder));
            }

            using (var sw = new StreamWriter(stream, DefaultStreamWriterEncoding, DefaultStreamWriterBufferSize, leaveStreamOpen))
            {
                if (outputBuilder.UsingDirectives.Any())
                {
                    foreach (var usingDirective in outputBuilder.UsingDirectives)
                    {
                        sw.Write("using");
                        sw.Write(' ');
                        sw.Write(usingDirective);
                        sw.WriteLine(';');
                    }

                    sw.WriteLine();
                }

                var indentationString = outputBuilder.IndentationString;

                if (emitNamespaceDeclaration)
                {
                    sw.Write("namespace");
                    sw.Write(' ');
                    sw.WriteLine(Config.Namespace);
                    sw.WriteLine('{');
                }
                else
                {
                    sw.WriteLine();
                }

                if (isMethodClass)
                {
                    sw.Write(indentationString);
                    sw.Write("public static");
                    sw.Write(' ');

                    if (Config.GenerateUnsafeCode)
                    {
                        sw.Write("unsafe");
                        sw.Write(' ');
                    }

                    sw.Write("partial class");
                    sw.Write(' ');
                    sw.WriteLine(Config.MethodClassName);
                    sw.Write(indentationString);
                    sw.WriteLine('{');

                    indentationString += outputBuilder.IndentationString;

                    sw.Write(indentationString);
                    sw.Write("private const string libraryPath = ");
                    sw.Write('"');
                    sw.Write(Config.LibraryPath);
                    sw.Write('"');
                    sw.WriteLine(';');
                    sw.WriteLine();
                }

                foreach (var line in outputBuilder.Contents)
                {
                    if (string.IsNullOrWhiteSpace(line))
                    {
                        sw.WriteLine();
                    }
                    else
                    {
                        sw.Write(indentationString);
                        sw.WriteLine(line);
                    }
                }

                if (isMethodClass)
                {
                    sw.Write(outputBuilder.IndentationString);
                    sw.WriteLine('}');
                }

                if (_config.GenerateMultipleFiles)
                {
                    sw.WriteLine('}');
                }
            }
        }

        private void Dispose(bool isDisposing)
        {
            Debug.Assert(_outputBuilder is null);

            if (_disposed)
            {
                return;
            }
            _disposed = true;

            if (isDisposing)
            {
                Close();
            }

            _index.Dispose();
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

        private string GetCallingConventionName(Cursor cursor, CXCallingConv callingConvention)
        {
            switch (callingConvention)
            {
                case CXCallingConv.CXCallingConv_C:
                {
                    return "Cdecl";
                }

                case CXCallingConv.CXCallingConv_X86StdCall:
                {
                    return "StdCall";
                }

                case CXCallingConv.CXCallingConv_X86FastCall:
                {
                    return "FastCall";
                }

                case CXCallingConv.CXCallingConv_X86ThisCall:
                {
                    return "ThisCall";
                }

                case CXCallingConv.CXCallingConv_Win64:
                {
                    return "Winapi";
                }

                default:
                {
                    var name = "WinApi";
                    AddDiagnostic(DiagnosticLevel.Info, $"Unsupported calling convention: '{callingConvention}'. Falling back to '{name}'.", cursor);
                    return name;
                }
            }
        }

        private string GetCursorName(Decl decl)
        {
            switch (decl.Kind)
            {
                case CXCursorKind.CXCursor_StructDecl:
                case CXCursorKind.CXCursor_UnionDecl:
                case CXCursorKind.CXCursor_EnumDecl:
                {
                    var name = decl.Spelling;

                    if (string.IsNullOrWhiteSpace(name))
                    {
                        if (decl.IsAnonymous)
                        {
                            decl.Location.GetFileLocation(out var file, out var _, out var _, out var offset);
                            var fileName = Path.GetFileNameWithoutExtension(file.Name.ToString());
                            name = $"__Anonymous{decl.Type.KindSpelling}_{fileName}_{offset}";
                            AddDiagnostic(DiagnosticLevel.Info, $"Anonymous declaration found in '{nameof(GetCursorName)}'. Falling back to '{name}'.'", decl);
                        }
                        else
                        {
                            name = GetTypeName(decl, decl.Type);
                        }
                    }

                    Debug.Assert(!string.IsNullOrWhiteSpace(name));
                    return name;
                }

                case CXCursorKind.CXCursor_FieldDecl:
                case CXCursorKind.CXCursor_EnumConstantDecl:
                case CXCursorKind.CXCursor_FunctionDecl:
                {
                    var name = decl.Spelling;
                    Debug.Assert(!string.IsNullOrWhiteSpace(name));
                    return name;
                }

                case CXCursorKind.CXCursor_ParmDecl:
                {
                    var name = decl.Spelling;

                    if (string.IsNullOrWhiteSpace(name))
                    {
                        name = "param";
                    }

                    Debug.Assert(!string.IsNullOrWhiteSpace(name));
                    return name;
                }

                case CXCursorKind.CXCursor_TypedefDecl:
                {
                    switch (decl.Spelling)
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
                            _outputBuilder.AddUsingDirective("System");
                            return "IntPtr";
                        }

                        case "size_t":
                        case "SIZE_T":
                        {
                            _outputBuilder.AddUsingDirective("System");
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
                            _outputBuilder.AddUsingDirective("System");
                            return "UIntPtr";
                        }

                        default:
                        {
                            var typedefDecl = (TypedefDecl)decl;
                            return GetCursorName(typedefDecl, typedefDecl.UnderlyingType);
                        }
                    }
                }

                default:
                {
                    var name = decl.Spelling;
                    AddDiagnostic(DiagnosticLevel.Warning, $"Unsupported declaration: '{decl.KindSpelling}'. Falling back to '{name}'.", decl);
                    return name;
                }
            }
        }

        private string GetCursorName(TypedefDecl typedefDecl, Type underlyingType)
        {
            switch (underlyingType.Kind)
            {
                case CXTypeKind.CXType_Bool:
                case CXTypeKind.CXType_Char_U:
                case CXTypeKind.CXType_UChar:
                case CXTypeKind.CXType_UShort:
                case CXTypeKind.CXType_UInt:
                case CXTypeKind.CXType_ULong:
                case CXTypeKind.CXType_ULongLong:
                case CXTypeKind.CXType_Char_S:
                case CXTypeKind.CXType_SChar:
                case CXTypeKind.CXType_WChar:
                case CXTypeKind.CXType_Short:
                case CXTypeKind.CXType_Int:
                case CXTypeKind.CXType_Long:
                case CXTypeKind.CXType_LongLong:
                case CXTypeKind.CXType_Float:
                case CXTypeKind.CXType_Double:
                case CXTypeKind.CXType_Pointer:
                {
                    var name = typedefDecl.Spelling;

                    if (_config.GenerateUnsafeCode || string.IsNullOrWhiteSpace(name))
                    {
                        name = GetTypeName(typedefDecl, underlyingType);
                    }

                    Debug.Assert(!string.IsNullOrWhiteSpace(name));
                    return name;
                }

                case CXTypeKind.CXType_Record:
                case CXTypeKind.CXType_Enum:
                {
                    var name = GetTypeName(typedefDecl, underlyingType);
                    Debug.Assert(!string.IsNullOrWhiteSpace(name));
                    return name;
                }

                case CXTypeKind.CXType_Typedef:
                case CXTypeKind.CXType_Elaborated:
                {
                    return GetCursorName(typedefDecl, underlyingType.CanonicalType);
                }

                default:
                {
                    var name = typedefDecl.Spelling;
                    AddDiagnostic(DiagnosticLevel.Warning, $"Unsupported underlying type: '{underlyingType.KindSpelling}'. Falling back to '{name}'.", typedefDecl);
                    return name;
                }
            }
        }

        private string GetMarshalAttribute(Decl decl, Type type)
        {
            if (_config.GenerateUnsafeCode)
            {
                return string.Empty;
            }

            switch (type.Kind)
            {
                case CXTypeKind.CXType_Void:
                case CXTypeKind.CXType_Char_U:
                case CXTypeKind.CXType_UChar:
                case CXTypeKind.CXType_UShort:
                case CXTypeKind.CXType_UInt:
                case CXTypeKind.CXType_ULong:
                case CXTypeKind.CXType_ULongLong:
                case CXTypeKind.CXType_Char_S:
                case CXTypeKind.CXType_SChar:
                case CXTypeKind.CXType_Short:
                case CXTypeKind.CXType_Int:
                case CXTypeKind.CXType_Long:
                case CXTypeKind.CXType_LongLong:
                case CXTypeKind.CXType_Float:
                case CXTypeKind.CXType_Double:
                case CXTypeKind.CXType_Record:
                case CXTypeKind.CXType_Enum:
                case CXTypeKind.CXType_Typedef:
                case CXTypeKind.CXType_ConstantArray:
                case CXTypeKind.CXType_IncompleteArray:
                {
                    return string.Empty;
                }

                case CXTypeKind.CXType_Bool:
                {
                    return "MarshalAs(UnmanagedType.U1)";
                }

                case CXTypeKind.CXType_Pointer:
                {
                    return GetMarshalAttributeForPointeeType(decl, type.PointeeType);
                }

                case CXTypeKind.CXType_Elaborated:
                {
                    return GetMarshalAttribute(decl, type.CanonicalType);
                }

                default:
                {
                    AddDiagnostic(DiagnosticLevel.Warning, $"Unsupported type: '{type.KindSpelling}'. Falling back to no marshalling.", decl);
                    return string.Empty;
                }
            }
        }

        private string GetMarshalAttributeForPointeeType(Decl decl, Type pointeeType)
        {
            Debug.Assert(!_config.GenerateUnsafeCode);

            switch (pointeeType.Kind)
            {
                case CXTypeKind.CXType_Void:
                case CXTypeKind.CXType_Bool:
                case CXTypeKind.CXType_UChar:
                case CXTypeKind.CXType_UShort:
                case CXTypeKind.CXType_UInt:
                case CXTypeKind.CXType_ULong:
                case CXTypeKind.CXType_ULongLong:
                case CXTypeKind.CXType_SChar:
                case CXTypeKind.CXType_Short:
                case CXTypeKind.CXType_Int:
                case CXTypeKind.CXType_Long:
                case CXTypeKind.CXType_LongLong:
                case CXTypeKind.CXType_Float:
                case CXTypeKind.CXType_Double:
                case CXTypeKind.CXType_Pointer:
                case CXTypeKind.CXType_Record:
                case CXTypeKind.CXType_Enum:
                case CXTypeKind.CXType_Typedef:
                case CXTypeKind.CXType_FunctionProto:
                {
                    return string.Empty;
                }

                case CXTypeKind.CXType_Char_U:
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
                    return GetMarshalAttributeForPointeeType(decl, pointeeType.CanonicalType);
                }

                default:
                {
                    AddDiagnostic(DiagnosticLevel.Warning, $"Unsupported pointee type: '{pointeeType.KindSpelling}'. Falling back to no marshalling.", decl);
                    return string.Empty;
                }
            }
        }

        private string GetParmModifier(Decl decl, Type type)
        {
            if (_config.GenerateUnsafeCode)
            {
                return string.Empty;
            }

            switch (type.Kind)
            {
                case CXTypeKind.CXType_Void:
                case CXTypeKind.CXType_Bool:
                case CXTypeKind.CXType_Char_U:
                case CXTypeKind.CXType_UChar:
                case CXTypeKind.CXType_UShort:
                case CXTypeKind.CXType_UInt:
                case CXTypeKind.CXType_ULong:
                case CXTypeKind.CXType_ULongLong:
                case CXTypeKind.CXType_Char_S:
                case CXTypeKind.CXType_SChar:
                case CXTypeKind.CXType_WChar:
                case CXTypeKind.CXType_Short:
                case CXTypeKind.CXType_Int:
                case CXTypeKind.CXType_Long:
                case CXTypeKind.CXType_LongLong:
                case CXTypeKind.CXType_Float:
                case CXTypeKind.CXType_Double:
                case CXTypeKind.CXType_Record:
                case CXTypeKind.CXType_Enum:
                case CXTypeKind.CXType_Typedef:
                {
                    return string.Empty;
                }

                case CXTypeKind.CXType_Pointer:
                {
                    return GetParmModifierForPointeeType(decl, type.PointeeType);
                }

                case CXTypeKind.CXType_ConstantArray:
                case CXTypeKind.CXType_IncompleteArray:
                {
                    return "out";
                }

                case CXTypeKind.CXType_Elaborated:
                {
                    return GetParmModifier(decl, type.CanonicalType);
                }

                default:
                {
                    AddDiagnostic(DiagnosticLevel.Warning, $"Unsupported type: '{type.KindSpelling}'. Falling back to no parameter modifier.", decl);
                    return string.Empty;
                }
            }
        }

        private string GetParmModifierForPointeeType(Decl decl, Type pointeeType)
        {
            Debug.Assert(!_config.GenerateUnsafeCode);

            switch (pointeeType.Kind)
            {
                case CXTypeKind.CXType_Void:
                case CXTypeKind.CXType_Char_U:
                case CXTypeKind.CXType_Char_S:
                case CXTypeKind.CXType_WChar:
                case CXTypeKind.CXType_FunctionProto:
                {
                    return string.Empty;
                }

                case CXTypeKind.CXType_Bool:
                case CXTypeKind.CXType_UChar:
                case CXTypeKind.CXType_UShort:
                case CXTypeKind.CXType_UInt:
                case CXTypeKind.CXType_ULong:
                case CXTypeKind.CXType_ULongLong:
                case CXTypeKind.CXType_SChar:
                case CXTypeKind.CXType_Short:
                case CXTypeKind.CXType_Int:
                case CXTypeKind.CXType_Long:
                case CXTypeKind.CXType_LongLong:
                case CXTypeKind.CXType_Float:
                case CXTypeKind.CXType_Double:
                case CXTypeKind.CXType_Pointer:
                case CXTypeKind.CXType_Record:
                case CXTypeKind.CXType_Enum:
                {
                    return "out";
                }

                case CXTypeKind.CXType_Typedef:
                case CXTypeKind.CXType_Elaborated:
                {
                    return GetParmModifierForPointeeType(decl, pointeeType.CanonicalType);
                }

                default:
                {
                    AddDiagnostic(DiagnosticLevel.Warning, $"Unsupported pointee type: '{pointeeType.KindSpelling}'. Falling back to no parameter modifier.", decl);
                    return string.Empty;
                }
            }
        }

        private string GetTypeName(Decl decl, Type type)
        {
            switch (type.Kind)
            {
                case CXTypeKind.CXType_Void:
                {
                    return "void";
                }

                case CXTypeKind.CXType_Bool:
                {
                    return "bool";
                }

                case CXTypeKind.CXType_Char_U:
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

                case CXTypeKind.CXType_Char_S:
                case CXTypeKind.CXType_SChar:
                {
                    return "sbyte";
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
                    return GetTypeNameForPointeeType(decl, type.PointeeType);
                }

                case CXTypeKind.CXType_Record:
                case CXTypeKind.CXType_Enum:
                case CXTypeKind.CXType_FunctionProto:
                {
                    var name = type.Spelling;
                    Debug.Assert(!string.IsNullOrWhiteSpace(name));
                    return name;
                }

                case CXTypeKind.CXType_Typedef:
                {
                    return GetCursorName(type.DeclarationCursor);
                }

                case CXTypeKind.CXType_Elaborated:
                {
                    return GetTypeName(decl, type.CanonicalType);
                }

                case CXTypeKind.CXType_ConstantArray:
                case CXTypeKind.CXType_IncompleteArray:
                {
                    return GetTypeName(decl, type.ElementType);
                }

                default:
                {
                    var name = type.Spelling;
                    AddDiagnostic(DiagnosticLevel.Warning, $"Unsupported type: '{type.KindSpelling}'. Falling back '{name}'.", decl);
                    return name;
                }
            }
        }

        private string GetTypeNameForPointeeType(Decl decl, Type pointeeType)
        {
            switch (pointeeType.Kind)
            {
                case CXTypeKind.CXType_Void:
                {
                    if (_config.GenerateUnsafeCode)
                    {
                        return "void*";
                    }

                    _outputBuilder.AddUsingDirective("System");
                    return "IntPtr";
                }

                case CXTypeKind.CXType_UChar:
                case CXTypeKind.CXType_UShort:
                case CXTypeKind.CXType_UInt:
                case CXTypeKind.CXType_ULong:
                case CXTypeKind.CXType_ULongLong:
                case CXTypeKind.CXType_SChar:
                case CXTypeKind.CXType_Short:
                case CXTypeKind.CXType_Int:
                case CXTypeKind.CXType_Long:
                case CXTypeKind.CXType_LongLong:
                case CXTypeKind.CXType_Float:
                case CXTypeKind.CXType_Double:
                case CXTypeKind.CXType_Pointer:
                case CXTypeKind.CXType_Record:
                case CXTypeKind.CXType_Enum:
                case CXTypeKind.CXType_Typedef:
                {
                    switch (decl.Kind)
                    {
                        case CXCursorKind.CXCursor_FieldDecl:
                        case CXCursorKind.CXCursor_FunctionDecl:
                        {
                            var name = "IntPtr";

                            if (_config.GenerateUnsafeCode)
                            {
                                name = GetTypeName(decl, pointeeType);
                                name += '*';
                            }
                            else
                            {
                                _outputBuilder.AddUsingDirective("System");
                            }

                            return name;
                        }

                        case CXCursorKind.CXCursor_ParmDecl:
                        {
                            var name = GetTypeName(decl, pointeeType);

                            if (_config.GenerateUnsafeCode)
                            {
                                name += '*';
                            }
                            return name;
                        }

                        case CXCursorKind.CXCursor_TypedefDecl:
                        {
                            var typedefDecl = (TypedefDecl)decl;
                            var underlyingType = typedefDecl.UnderlyingType;

                            if ((underlyingType.Kind == CXTypeKind.CXType_Pointer) && (underlyingType.PointeeType.Kind == CXTypeKind.CXType_FunctionProto))
                            {
                                goto case CXCursorKind.CXCursor_FunctionDecl;
                            }

                            var name = GetCursorName(pointeeType.DeclarationCursor);

                            if (_config.GenerateUnsafeCode)
                            {
                                name += '*';
                            }
                            return name;
                        }

                        default:
                        {
                            var name = "IntPtr";
                            AddDiagnostic(DiagnosticLevel.Warning, $"Unsupported declaration: '{decl.KindSpelling}'. Falling back '{name}'.", decl);
                            return string.Empty;
                        }
                    }
                }

                case CXTypeKind.CXType_Char_U:
                case CXTypeKind.CXType_Char_S:
                {
                    switch (decl.Kind)
                    {
                        case CXCursorKind.CXCursor_FieldDecl:
                        case CXCursorKind.CXCursor_FunctionDecl:
                        {
                            return _config.GenerateUnsafeCode ? "byte*" : "string";
                        }

                        case CXCursorKind.CXCursor_ParmDecl:
                        {
                            if (GetParmModifier(decl, decl.Type).Equals("out"))
                            {
                                Debug.Assert(!_config.GenerateUnsafeCode);
                                _outputBuilder.AddUsingDirective("System");
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
                            var name = "IntPtr";
                            AddDiagnostic(DiagnosticLevel.Warning, $"Unsupported declaration: '{decl.KindSpelling}'. Falling back '{name}'.", decl);
                            return string.Empty;
                        }
                    }
                }

                case CXTypeKind.CXType_FunctionProto:
                {
                    _outputBuilder.AddUsingDirective("System");
                    return "IntPtr";
                }

                case CXTypeKind.CXType_Elaborated:
                {
                    return GetTypeNameForPointeeType(decl, pointeeType.CanonicalType);
                }

                case CXTypeKind.CXType_Attributed:
                {
                    return GetTypeNameForPointeeType(decl, pointeeType.ModifierType);
                }

                default:
                {
                    var name = "IntPtr";
                    AddDiagnostic(DiagnosticLevel.Warning, $"Unsupported pointee type: '{pointeeType.KindSpelling}'. Falling back '{name}'.", decl);
                    return string.Empty;
                }
            }
        }

        private void StartUsingOutputBuilder(string name)
        {
            if (_outputBuilder != null)
            {
                Debug.Assert(_outputBuilderUsers >= 1);
                _outputBuilderUsers++;
                return;
            }

            Debug.Assert(_outputBuilderUsers == 0);

            if (!_outputBuilderFactory.TryGetOutputBuilder(name, out _outputBuilder))
            {
                _outputBuilder = _outputBuilderFactory.Create(name);
            }
            else
            {
                _outputBuilder.WriteLine();
            }
            _outputBuilderUsers++;
        }

        private void StopUsingOutputBuilder()
        {
            if (_outputBuilderUsers == 1)
            {
                _outputBuilder = null;
            }
            _outputBuilderUsers--;
        }

        private void Visit(Cursor cursor, Cursor parent)
        {
            Debug.Assert(_visitedCursors.Contains(parent));

            if (_visitedCursors.Contains(cursor))
            {
                return;
            }

            _visitedCursors.Add(cursor);

            switch (cursor.Kind)
            {
                case CXCursorKind.CXCursor_UnexposedDecl:
                case CXCursorKind.CXCursor_UnexposedExpr:
                case CXCursorKind.CXCursor_UnexposedAttr:
                {
                    VisitChildren(cursor);
                    break;
                }

                case CXCursorKind.CXCursor_StructDecl:
                {
                    var structDecl = (StructDecl)cursor;
                    VisitStructDecl(structDecl, parent);
                    break;
                }

                case CXCursorKind.CXCursor_EnumDecl:
                {
                    var enumDecl = (EnumDecl)cursor;
                    VisitEnumDecl(enumDecl, parent);
                    break;
                }

                case CXCursorKind.CXCursor_FieldDecl:
                {
                    var fieldDecl = (FieldDecl)cursor;
                    VisitFieldDecl(fieldDecl, parent);
                    break;
                }

                case CXCursorKind.CXCursor_EnumConstantDecl:
                {
                    var enumConstantDecl = (EnumConstantDecl)cursor;
                    VisitEnumConstantDecl(enumConstantDecl, parent);
                    break;
                }

                case CXCursorKind.CXCursor_FunctionDecl:
                {
                    var functionDecl = (FunctionDecl)cursor;
                    VisitFunctionDecl(functionDecl, parent);
                    break;
                }

                case CXCursorKind.CXCursor_ParmDecl:
                {
                    var parmDecl = (ParmDecl)cursor;
                    VisitParmDecl(parmDecl, parent);
                    break;
                }

                case CXCursorKind.CXCursor_TypedefDecl:
                {
                    var typedefDecl = (TypedefDecl)cursor;
                    VisitTypedefDecl(typedefDecl, parent, typedefDecl.UnderlyingType);
                    break;
                }

                case CXCursorKind.CXCursor_TypeRef:
                {
                    var typeRef = (TypeRef)cursor;
                    VisitTypeRef(typeRef, parent);
                    break;
                }

                case CXCursorKind.CXCursor_DeclRefExpr:
                {
                    var declRefExpr = (DeclRefExpr)cursor;
                    VisitDeclRefExpr(declRefExpr, parent);
                    break;
                }

                case CXCursorKind.CXCursor_IntegerLiteral:
                {
                    var integerLiteral = (IntegerLiteral)cursor;
                    VisitIntegerLiteral(integerLiteral, parent);
                    break;
                }

                case CXCursorKind.CXCursor_ParenExpr:
                {
                    var parenExpr = (ParenExpr)cursor;
                    VisitParenExpr(parenExpr, parent);
                    break;
                }

                case CXCursorKind.CXCursor_UnaryOperator:
                {
                    var unaryOperator = (UnaryOperator)cursor;
                    VisitUnaryOperator(unaryOperator, parent);
                    break;
                }

                case CXCursorKind.CXCursor_BinaryOperator:
                {
                    var binaryOperator = (BinaryOperator)cursor;
                    VisitBinaryOperator(binaryOperator, parent);
                    break;
                }

                default:
                {
                    AddDiagnostic(DiagnosticLevel.Error, $"Unsupported cursor: '{cursor.KindSpelling}'. Generated bindings may be incomplete.", cursor);
                    break;
                }
            }
        }

        private void VisitBinaryOperator(BinaryOperator binaryOperator, Cursor parent)
        {
            Debug.Assert((parent is EnumConstantDecl) || (parent is Expr));

            Visit(binaryOperator.LhsExpr, binaryOperator);
            _outputBuilder.Write(' ');
            _outputBuilder.Write(binaryOperator.Operator);
            _outputBuilder.Write(' ');
            Visit(binaryOperator.RhsExpr, binaryOperator);

            Debug.Assert(binaryOperator.Children.Count == 2);
        }

        private void VisitChildren(Cursor parent)
        {
            foreach (var child in parent.Children)
            {
                Visit(child, parent);
            }
        }

        private void VisitDeclRefExpr(DeclRefExpr declRefExpr, Cursor parent)
        {
            Debug.Assert((parent is EnumConstantDecl) || (parent is Expr));
            _outputBuilder.Write(declRefExpr.Identifier);
            Debug.Assert(declRefExpr.Children.Count == 0);
        }

        private void VisitEnumConstantDecl(EnumConstantDecl enumConstantDecl, Cursor parent)
        {
            var name = GetCursorName(enumConstantDecl);

            _outputBuilder.WriteIndentation();
            _outputBuilder.Write(EscapeName(name));

            if (enumConstantDecl.Expr != null)
            {
                _outputBuilder.Write(" = ");
                Visit(enumConstantDecl.Expr, enumConstantDecl);
            }
            VisitChildren(enumConstantDecl);

            _outputBuilder.WriteLine(',');
        }

        private void VisitEnumDecl(EnumDecl enumDecl, Cursor parent)
        {
            var name = GetCursorName(enumDecl);
            StartUsingOutputBuilder(name);
            {
                _outputBuilder.WriteIndented("public enum");
                _outputBuilder.Write(' ');
                _outputBuilder.Write(EscapeName(name));

                var integerTypeName = GetTypeName(enumDecl, enumDecl.IntegerType);

                if (!integerTypeName.Equals("int"))
                {
                    _outputBuilder.Write(" : ");
                    _outputBuilder.Write(integerTypeName);
                }

                _outputBuilder.WriteLine();
                _outputBuilder.WriteBlockStart();

                foreach (var enumConstantDecl in enumDecl.EnumConstantDecls)
                {
                    Visit(enumConstantDecl, enumDecl);
                }
                VisitChildren(enumDecl);

                _outputBuilder.WriteBlockEnd();
            }
            StopUsingOutputBuilder();
        }

        private void VisitFieldDecl(FieldDecl fieldDecl, Cursor parent)
        {
            _outputBuilder.WriteIndentation();

            var marshalAttribute = GetMarshalAttribute(fieldDecl, fieldDecl.Type);

            if (!string.IsNullOrWhiteSpace(marshalAttribute))
            {
                _outputBuilder.AddUsingDirective("System.Runtime.InteropServices");

                _outputBuilder.Write('[');
                _outputBuilder.Write(marshalAttribute);
                _outputBuilder.Write(']');
                _outputBuilder.Write(' ');
            }

            long lastElement = -1;

            var name = GetCursorName(fieldDecl);
            var escapedName = EscapeName(name);

            if (fieldDecl.Type.Kind == CXTypeKind.CXType_ConstantArray)
            {
                lastElement = fieldDecl.Type.NumElements - 1;

                for (int i = 0; i < lastElement; i++)
                {
                    _outputBuilder.Write("public");
                    _outputBuilder.Write(' ');
                    _outputBuilder.Write(GetTypeName(fieldDecl, fieldDecl.Type));
                    _outputBuilder.Write(' ');
                    _outputBuilder.Write(escapedName);
                    _outputBuilder.Write(i);
                    _outputBuilder.Write(';');
                    _outputBuilder.Write(' ');
                }
            }

            _outputBuilder.Write("public");
            _outputBuilder.Write(' ');
            _outputBuilder.Write(GetTypeName(fieldDecl, fieldDecl.Type));
            _outputBuilder.Write(' ');
            _outputBuilder.Write(escapedName);

            if (lastElement != -1)
            {
                _outputBuilder.Write(lastElement);
            }
            VisitChildren(fieldDecl);

            _outputBuilder.WriteLine(';');
        }

        private void VisitFunctionDecl(FunctionDecl functionDecl, Cursor parent)
        {
            var type = functionDecl.Type;
            var name = GetCursorName(functionDecl);

            if (_config.ExcludedFunctions.Contains(name))
            {
                foreach (var child in functionDecl.Children)
                {
                    _visitedCursors.Add(child);
                }
                return;
            }

            StartUsingOutputBuilder(_config.MethodClassName);
            {
                _outputBuilder.AddUsingDirective("System.Runtime.InteropServices");

                _outputBuilder.WriteIndented("[DllImport(libraryPath, EntryPoint = \"");
                _outputBuilder.Write(name);
                _outputBuilder.Write("\", CallingConvention = CallingConvention.");
                _outputBuilder.Write(GetCallingConventionName(functionDecl, type.CallingConv));
                _outputBuilder.WriteLine(")]");

                var marshalAttribute = GetMarshalAttribute(functionDecl, type.ResultType);

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
                _outputBuilder.Write(GetTypeName(functionDecl, type.ResultType));
                _outputBuilder.Write(' ');
                _outputBuilder.Write(name);
                _outputBuilder.Write('(');

                foreach (var parmDecl in functionDecl.ParmDecls)
                {
                    Visit(parmDecl, functionDecl);
                }
                VisitChildren(functionDecl);

                _outputBuilder.WriteLine(");");
            }
            StopUsingOutputBuilder();
        }

        private void VisitIntegerLiteral(IntegerLiteral integerLiteral, Cursor parent)
        {
            if (parent is FieldDecl)
            {
                return;
            }

            Debug.Assert((parent is EnumConstantDecl) || (parent is Expr));
            _outputBuilder.Write(integerLiteral.RawValue);
            Debug.Assert(integerLiteral.Children.Count == 0);
        }

        private void VisitParenExpr(ParenExpr parenExpr, Cursor parent)
        {
            Debug.Assert((parent is EnumConstantDecl) || (parent is Expr));

            _outputBuilder.Write('(');
            VisitChildren(parenExpr);
            _outputBuilder.Write(')');

            Debug.Assert(parenExpr.Children.Count == 1);
        }

        private void VisitParmDecl(ParmDecl parmDecl, Cursor parent)
        {
            int lastIndex = -1;

            if (parent is FunctionDecl functionDecl)
            {
                lastIndex = functionDecl.ParmDecls.Count - 1;
            }
            else if (parent is TypedefDecl typedefDecl)
            {
                lastIndex = typedefDecl.ParmDecls.Count - 1;
            }

            if (lastIndex != -1)
            {
                var marshalAttribute = GetMarshalAttribute(parmDecl, parmDecl.Type);

                if (!string.IsNullOrWhiteSpace(marshalAttribute))
                {
                    _outputBuilder.Write("[");
                    _outputBuilder.Write(marshalAttribute);
                    _outputBuilder.Write(']');
                    _outputBuilder.Write(' ');
                }

                var parmModifier = GetParmModifier(parmDecl, parmDecl.Type);

                if (!string.IsNullOrWhiteSpace(parmModifier))
                {
                    _outputBuilder.Write(parmModifier);
                    _outputBuilder.Write(' ');
                }

                _outputBuilder.Write(GetTypeName(parmDecl, parmDecl.Type));
                _outputBuilder.Write(' ');

                var name = GetCursorName(parmDecl);
                _outputBuilder.Write(EscapeName(name));

                if (name.Equals("param"))
                {
                    _outputBuilder.Write(parmDecl.Index);
                }
                VisitChildren(parmDecl);

                if (parmDecl.Index != lastIndex)
                {
                    _outputBuilder.Write(", ");
                }
            }
            else if ((parent is FieldDecl) || (parent is ParmDecl))
            {
                // TODO: We should properly handle inline function pointers for fields and method parameters
                VisitChildren(parmDecl);
            }
            else
            {
                AddDiagnostic(DiagnosticLevel.Error, $"Unsupported '{parmDecl.KindSpelling}' parent: '{parent.KindSpelling}'. Generated bindings may be incomplete.", parmDecl);
            }
        }

        private void VisitStructDecl(StructDecl structDecl, Cursor parent)
        {
            var name = GetCursorName(structDecl);

            StartUsingOutputBuilder(name);
            {
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

                foreach (var fieldDecl in structDecl.FieldDecls)
                {
                    Visit(fieldDecl, structDecl);
                }
                VisitChildren(structDecl);

                _outputBuilder.WriteBlockEnd();
            }
            StopUsingOutputBuilder();
        }

        private void VisitTypedefDecl(TypedefDecl typedefDecl, Cursor parent, Type underlyingType)
        {
            switch (underlyingType.Kind)
            {
                case CXTypeKind.CXType_Bool:
                case CXTypeKind.CXType_Char_U:
                case CXTypeKind.CXType_UChar:
                case CXTypeKind.CXType_UShort:
                case CXTypeKind.CXType_UInt:
                case CXTypeKind.CXType_ULong:
                case CXTypeKind.CXType_ULongLong:
                case CXTypeKind.CXType_Char_S:
                case CXTypeKind.CXType_SChar:
                case CXTypeKind.CXType_WChar:
                case CXTypeKind.CXType_Short:
                case CXTypeKind.CXType_Int:
                case CXTypeKind.CXType_Long:
                case CXTypeKind.CXType_LongLong:
                case CXTypeKind.CXType_Float:
                case CXTypeKind.CXType_Double:
                {
                    if (!_config.GenerateUnsafeCode)
                    {
                        var name = GetCursorName(typedefDecl);

                        StartUsingOutputBuilder(name);
                        {

                            var escapedName = EscapeName(name);

                            _outputBuilder.WriteIndented("public partial struct");
                            _outputBuilder.Write(' ');
                            _outputBuilder.WriteLine(escapedName);
                            _outputBuilder.WriteBlockStart();
                            {
                                var typeName = GetTypeName(typedefDecl, underlyingType);

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
                        StopUsingOutputBuilder();
                    }
                    break;
                }

                case CXTypeKind.CXType_Pointer:
                {
                    VisitTypedefDeclForPointer(typedefDecl, parent, underlyingType.PointeeType);
                    break;
                }

                case CXTypeKind.CXType_Record:
                case CXTypeKind.CXType_Enum:
                {
                    // We recurse the struct and record declarations directly
                    break;
                }

                case CXTypeKind.CXType_Typedef:
                case CXTypeKind.CXType_Elaborated:
                {
                    VisitTypedefDecl(typedefDecl, parent, underlyingType.CanonicalType);
                    break;
                }

                default:
                {
                    AddDiagnostic(DiagnosticLevel.Error, $"Unsupported underlying type: '{underlyingType.KindSpelling}'. Generating bindings may be incomplete.", typedefDecl);
                    break;
                }
            }

            VisitChildren(typedefDecl);
        }

        private void VisitTypedefDeclForPointer(TypedefDecl typedefDecl, Cursor parent, Type pointeeType)
        {
            switch (pointeeType.Kind)
            {
                case CXTypeKind.CXType_Void:
                case CXTypeKind.CXType_Record:
                {
                    if (!_config.GenerateUnsafeCode)
                    {
                        var name = GetCursorName(typedefDecl);
                        StartUsingOutputBuilder(name);
                        {
                            var escapedName = EscapeName(name);

                            _outputBuilder.AddUsingDirective("System");

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
                        StopUsingOutputBuilder();
                    }
                    break;
                }

                case CXTypeKind.CXType_FunctionProto:
                {
                    var name = GetCursorName(typedefDecl);
                    StartUsingOutputBuilder(name);
                    {
                        var escapedName = EscapeName(name);

                        _outputBuilder.AddUsingDirective("System.Runtime.InteropServices");

                        _outputBuilder.WriteIndented("[UnmanagedFunctionPointer(CallingConvention.");
                        _outputBuilder.Write(GetCallingConventionName(typedefDecl, pointeeType.CallingConv));
                        _outputBuilder.WriteLine(")]");
                        _outputBuilder.WriteIndented("public delegate");
                        _outputBuilder.Write(' ');
                        _outputBuilder.Write(GetTypeName(typedefDecl, pointeeType.ResultType));
                        _outputBuilder.Write(' ');
                        _outputBuilder.Write(escapedName);
                        _outputBuilder.Write('(');

                        foreach (var parmDecl in typedefDecl.ParmDecls)
                        {
                            Visit(parmDecl, typedefDecl);
                        }
                        VisitChildren(typedefDecl);

                        _outputBuilder.WriteLine(");");
                    }
                    StopUsingOutputBuilder();
                    break;
                }

                case CXTypeKind.CXType_Elaborated:
                {
                    VisitTypedefDeclForPointer(typedefDecl, parent, pointeeType.CanonicalType);
                    break;
                }

                default:
                {
                    AddDiagnostic(DiagnosticLevel.Error, $"Unsupported pointee type: '{pointeeType.KindSpelling}'. Generating bindings may be incomplete.", typedefDecl);
                    break;
                }
            }
        }

        private void VisitTypeRef(TypeRef typeRef, Cursor parent)
        {
            Debug.Assert(typeRef.Children.Count == 0);
            Debug.Assert((parent is FieldDecl) || (parent is FunctionDecl) || (parent is ParmDecl) || (parent is TypedefDecl));
        }

        private void VisitUnaryOperator(UnaryOperator unaryOperator, Cursor parent)
        {
            Debug.Assert((parent is EnumConstantDecl) || (parent is Expr));

            if (unaryOperator.IsPrefix)
            {
                _outputBuilder.Write(unaryOperator.Operator);
                Visit(unaryOperator.Expr, unaryOperator);
            }
            else
            {
                Visit(unaryOperator.Expr, unaryOperator);
                _outputBuilder.Write(unaryOperator.Operator);
            }


            Debug.Assert(unaryOperator.Children.Count == 1);
        }
    }
}
