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
        private readonly Func<string, Stream> _outputStreamFactory;
        private readonly HashSet<Cursor> _visitedCursors;
        private readonly List<Diagnostic> _diagnostics;
        private readonly PInvokeGeneratorConfiguration _config;

        private OutputBuilder _outputBuilder;
        private int _outputBuilderUsers;
        private bool _disposed;

        public PInvokeGenerator(PInvokeGeneratorConfiguration config, Func<string, Stream> outputStreamFactory = null)
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
                return new FileStream(path, FileMode.Create);
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
            Stream stream = null;
            OutputBuilder methodClassOutputBuilder = null;
            bool emitNamespaceDeclaration = true;
            bool leaveStreamOpen = false;

            if (!_config.GenerateMultipleFiles)
            {
                var outputPath = _config.OutputLocation;
                stream = _outputStreamFactory(outputPath);
                leaveStreamOpen = true;

                var usingDirectives = Enumerable.Empty<string>();

                foreach (var outputBuilder in _outputBuilderFactory.OutputBuilders)
                {
                    usingDirectives = usingDirectives.Concat(outputBuilder.UsingDirectives);
                }

                usingDirectives = usingDirectives.Distinct()
                                                 .OrderBy((usingDirective) => usingDirective);

                if (usingDirectives.Any())
                {
                    using (var sw = new StreamWriter(stream, DefaultStreamWriterEncoding, DefaultStreamWriterBufferSize, leaveStreamOpen))
                    {
                        sw.NewLine = "\n";

                        foreach (var usingDirective in usingDirectives)
                        {
                            sw.Write("using");
                            sw.Write(' ');
                            sw.Write(usingDirective);
                            sw.WriteLine(';');
                        }

                        sw.WriteLine();
                    }
                }
            }

            foreach (var outputBuilder in _outputBuilderFactory.OutputBuilders)
            {
                var outputPath = _config.OutputLocation;
                var isMethodClass = _config.MethodClassName.Equals(outputBuilder.Name);

                if (_config.GenerateMultipleFiles)
                {
                    outputPath = Path.Combine(outputPath, $"{outputBuilder.Name}.cs");
                    stream = _outputStreamFactory(outputPath);
                    emitNamespaceDeclaration = true;
                }
                else if (isMethodClass)
                {
                    methodClassOutputBuilder = outputBuilder;
                    continue;
                }

                CloseOutputBuilder(stream, outputBuilder, isMethodClass, leaveStreamOpen, emitNamespaceDeclaration);
                emitNamespaceDeclaration = false;
            }

            if (leaveStreamOpen && _outputBuilderFactory.OutputBuilders.Any())
            {
                if (methodClassOutputBuilder != null)
                {
                    CloseOutputBuilder(stream, methodClassOutputBuilder, isMethodClass: true, leaveStreamOpen, emitNamespaceDeclaration);
                }

                using (var sw = new StreamWriter(stream, DefaultStreamWriterEncoding, DefaultStreamWriterBufferSize, leaveStreamOpen))
                {
                    sw.NewLine = "\n";
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

            foreach (var declaration in translationUnit.Declarations)
            {
                if (!declaration.IsFromMainFile)
                {
                    continue;
                }

                Visit(declaration, translationUnit);
            }
        }

        private void AddDiagnostic(DiagnosticLevel level, string message, Cursor cursor)
        {
            var diagnostic = new Diagnostic(level, message, cursor.Location);

            if (_diagnostics.Contains(diagnostic))
            {
                return;
            }

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
                sw.NewLine = "\n";

                if (outputBuilder.UsingDirectives.Any() && _config.GenerateMultipleFiles)
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

        private string EscapeAndStripName(string name)
        {
            if (name.StartsWith(_config.MethodPrefixToStrip))
            {
                name = name.Substring(_config.MethodPrefixToStrip.Length);
            }

            return EscapeName(name);
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

        private string GetArtificalFixedSizedBufferName(FieldDecl fieldDecl)
        {
            var name = GetRemappedCursorName(fieldDecl);
            return $"_{name}_e__FixedBuffer";
        }

        private string GetCursorName(NamedDecl namedDecl)
        {
            var name = string.Empty;

            if (namedDecl is TagDecl tagDecl)
            {
                name = namedDecl.Spelling;

                if (tagDecl.IsAnonymous)
                {
                    namedDecl.Location.GetFileLocation(out var file, out var _, out var _, out var offset);
                    var fileName = Path.GetFileNameWithoutExtension(file.Name.ToString());
                    name = $"__Anonymous{tagDecl.Type.KindSpelling}_{fileName}_{offset}";
                    AddDiagnostic(DiagnosticLevel.Info, $"Anonymous declaration found in '{nameof(GetCursorName)}'. Falling back to '{name}'.'", namedDecl);
                }
                else if (string.IsNullOrWhiteSpace(name))
                {
                    name = GetTypeName(namedDecl, tagDecl.Type);
                }
            }
            else
            {
                name = namedDecl.Spelling;

                if ((namedDecl is ParmVarDecl parmVarDecl) && string.IsNullOrWhiteSpace(name))
                {
                    name = "param";
                }
                else if (namedDecl is TypedefDecl typedefDecl)
                {
                    switch (name)
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
                            return GetCursorName(typedefDecl, typedefDecl.UnderlyingType);
                        }
                    }
                }
            }

            Debug.Assert(!string.IsNullOrWhiteSpace(name));
            return name;
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

        private string GetRemappedCursorName(NamedDecl namedDecl)
        {
            var name = GetCursorName(namedDecl);

            if (!_config.RemappedNames.TryGetValue(name, out string remappedName))
            {
                remappedName = name;
            }

            return remappedName;
        }

        private string GetRemappedTypeName(Decl decl, Type type)
        {
            var name = GetTypeName(decl, type);

            if (!_config.RemappedNames.TryGetValue(name, out string remappedName))
            {
                remappedName = name;
            }

            return remappedName;
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
                    return GetCursorName((NamedDecl)type.DeclarationCursor);
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

                            var name = GetRemappedCursorName((NamedDecl)pointeeType.DeclarationCursor);

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
                            if (GetParmModifier(decl, ((ParmVarDecl)decl).Type).Equals("out"))
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

        private bool IsSupportedFixedSizedBufferType(string typeName)
        {
            switch (typeName)
            {
                case "bool":
                case "byte":
                case "char":
                case "double":
                case "float":
                case "int":
                case "long":
                case "sbyte":
                case "short":
                case "ushort":
                case "uint":
                case "ulong":
                {
                    return true;
                }

                default:
                {
                    return false;
                }
            }
        }

        private void StartUsingOutputBuilder(string name)
        {
            if (_outputBuilder != null)
            {
                Debug.Assert(_outputBuilderUsers >= 1);
                _outputBuilderUsers++;

                _outputBuilder.WriteLine();
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

            if (cursor is Attr attr)
            {
                VisitAttr(attr, parent);
            }
            else if (cursor is Decl decl)
            {
                VisitDecl(decl, parent);
            }
            else if (cursor is Ref @ref)
            {
                VisitRef(@ref, parent);
            }
            else if (cursor is Stmt stmt)
            {
                VisitStmt(stmt, parent);
            }
            else
            {
                AddDiagnostic(DiagnosticLevel.Error, $"Unsupported cursor: '{cursor.KindSpelling}'. Generated bindings may be incomplete.", cursor);
            }
        }

        private void VisitAttr(Attr attr, Cursor parent)
        {
            // We don't consider most attributes particularly important and so we do nothing
        }

        private void VisitBinaryOperator(BinaryOperator binaryOperator, Cursor parent)
        {
            Visit(binaryOperator.LHS, binaryOperator);
            _outputBuilder.Write(' ');
            _outputBuilder.Write(binaryOperator.Opcode);
            _outputBuilder.Write(' ');
            Visit(binaryOperator.RHS, binaryOperator);
        }

        private void VisitCallExpr(CallExpr callExpr, Cursor parent)
        {
            var calleeDecl = callExpr.CalleeDecl;

            if (calleeDecl is FunctionDecl functionDecl)
            {
                var name = GetRemappedCursorName(functionDecl);

                _outputBuilder.WriteIndented(EscapeAndStripName(name));
                _outputBuilder.Write('(');

                foreach (var argument in callExpr.Arguments)
                {
                    Visit(argument, callExpr);
                }

                _outputBuilder.WriteLine(");");
            }
            else
            {
                AddDiagnostic(DiagnosticLevel.Error, $"Unsupported callee declaration: '{calleeDecl.KindSpelling}'. Generated bindings may be incomplete.", calleeDecl);
            }
        }

        private void VisitCompoundStmt(CompoundStmt compoundStmt, Cursor parent)
        {
            _outputBuilder.WriteBlockStart();

            foreach (var stmt in compoundStmt.Body)
            {
                Visit(stmt, parent);
            }

            _outputBuilder.WriteBlockEnd();
        }

        private void VisitDecl(Decl decl, Cursor parent)
        {
            if (decl is AccessSpecDecl accessSpecDecl)
            {
                // Access specifications are also exposed as a queryable property
                // on the declarations they impact, so we don't need to do anything
            }
            else if (decl is NamedDecl namedDecl)
            {
                VisitNamedDecl(namedDecl, parent);
            }
            else if (decl is UnexposedDecl unexposedDecl)
            {
                VisitUnexposedDecl(unexposedDecl, parent);
            }
            else
            {
                AddDiagnostic(DiagnosticLevel.Error, $"Unsupported declaration: '{decl.KindSpelling}'. Generated bindings may be incomplete.", decl);
            }
        }

        private void VisitDeclaratorDecl(DeclaratorDecl declaratorDecl, Cursor parent)
        {
            if (declaratorDecl is FieldDecl fieldDecl)
            {
                VisitFieldDecl(fieldDecl, parent);
            }
            else if (declaratorDecl is FunctionDecl functionDecl)
            {
                VisitFunctionDecl(functionDecl, parent);
            }
            else if (declaratorDecl is VarDecl varDecl)
            {
                VisitVarDecl(varDecl, parent);
            }
            else
            {
                AddDiagnostic(DiagnosticLevel.Error, $"Unsupported declarator declaration: '{declaratorDecl.KindSpelling}'. Generated bindings may be incomplete.", declaratorDecl);
            }
        }

        private void VisitDeclRefExpr(DeclRefExpr declRefExpr, Cursor parent)
        {
            var name = GetRemappedCursorName(declRefExpr.Decl);
            _outputBuilder.Write(EscapeName(name));
        }

        private void VisitEnumConstantDecl(EnumConstantDecl enumConstantDecl, Cursor parent)
        {
            var name = GetRemappedCursorName(enumConstantDecl);

            _outputBuilder.WriteIndentation();
            _outputBuilder.Write(EscapeName(name));

            if (enumConstantDecl.InitExpr != null)
            {
                _outputBuilder.Write(" = ");
                Visit(enumConstantDecl.InitExpr, enumConstantDecl);
            }

            _outputBuilder.WriteLine(',');
        }

        private void VisitEnumDecl(EnumDecl enumDecl, Cursor parent)
        {
            var name = GetRemappedCursorName(enumDecl);

            StartUsingOutputBuilder(name);
            {
                _outputBuilder.WriteIndented("public enum");
                _outputBuilder.Write(' ');
                _outputBuilder.Write(EscapeName(name));

                var integerTypeName = GetRemappedTypeName(enumDecl, enumDecl.IntegerType);

                if (!integerTypeName.Equals("int"))
                {
                    _outputBuilder.Write(" : ");
                    _outputBuilder.Write(integerTypeName);
                }

                _outputBuilder.WriteLine();
                _outputBuilder.WriteBlockStart();

                foreach (var enumerator in enumDecl.Enumerators)
                {
                    Visit(enumerator, enumDecl);
                }

                foreach (var declaration in enumDecl.Declarations)
                {
                    Visit(declaration, enumDecl);
                }

                _outputBuilder.WriteBlockEnd();
            }
            StopUsingOutputBuilder();
        }

        private void VisitExpr(Expr expr, Cursor parent)
        {
            if (expr is BinaryOperator binaryOperator)
            {
                VisitBinaryOperator(binaryOperator, parent);
            }
            else if (expr is CallExpr callExpr)
            {
                VisitCallExpr(callExpr, parent);
            }
            else if (expr is DeclRefExpr declRefExpr)
            {
                VisitDeclRefExpr(declRefExpr, parent);
            }
            else if (expr is IntegerLiteral integerLiteral)
            {
                VisitIntegerLiteral(integerLiteral, parent);
            }
            else if (expr is ParenExpr parenExpr)
            {
                VisitParenExpr(parenExpr, parent);
            }
            else if (expr is UnaryOperator unaryOperator)
            {
                VisitUnaryOperator(unaryOperator, parent);
            }
            else if (expr is UnexposedExpr unexposedExpr)
            {
                VisitUnexposedExpr(unexposedExpr, parent);
            }
            else
            {
                AddDiagnostic(DiagnosticLevel.Error, $"Unsupported expression: '{expr.KindSpelling}'. Generated bindings may be incomplete.", expr);
            }
        }

        private void VisitFieldDecl(FieldDecl fieldDecl, Cursor parent)
        {
            _outputBuilder.WriteIndentation();

            var type = fieldDecl.Type;
            var marshalAttribute = GetMarshalAttribute(fieldDecl, type);

            if (!string.IsNullOrWhiteSpace(marshalAttribute))
            {
                _outputBuilder.AddUsingDirective("System.Runtime.InteropServices");

                _outputBuilder.Write('[');
                _outputBuilder.Write(marshalAttribute);
                _outputBuilder.Write(']');
                _outputBuilder.Write(' ');
            }

            var name = GetRemappedCursorName(fieldDecl);
            var escapedName = EscapeName(name);
            var typeName = GetRemappedTypeName(fieldDecl, type);

            if (type.Kind == CXTypeKind.CXType_ConstantArray)
            {
                if (_config.GenerateUnsafeCode)
                {
                    if (IsSupportedFixedSizedBufferType(typeName))
                    {
                        _outputBuilder.Write("public fixed");
                        _outputBuilder.Write(' ');
                        _outputBuilder.Write(typeName);
                        _outputBuilder.Write(' ');
                        _outputBuilder.Write(escapedName);
                        _outputBuilder.Write('[');
                        _outputBuilder.Write(type.NumElements);
                        _outputBuilder.Write(']');
                    }
                    else
                    {
                        _outputBuilder.Write("public");
                        _outputBuilder.Write(' ');
                        _outputBuilder.Write(GetArtificalFixedSizedBufferName(fieldDecl));
                        _outputBuilder.Write(' ');
                        _outputBuilder.Write(escapedName);
                    }
                }
                else
                {
                    long lastElement = type.NumElements - 1;

                    for (int i = 0; i < lastElement; i++)
                    {
                        _outputBuilder.Write("public");
                        _outputBuilder.Write(' ');
                        _outputBuilder.Write(typeName);
                        _outputBuilder.Write(' ');
                        _outputBuilder.Write(escapedName);
                        _outputBuilder.Write(i);
                        _outputBuilder.Write(';');
                        _outputBuilder.Write(' ');
                    }

                    _outputBuilder.Write("public");
                    _outputBuilder.Write(' ');
                    _outputBuilder.Write(typeName);
                    _outputBuilder.Write(' ');
                    _outputBuilder.Write(escapedName);
                    _outputBuilder.Write(lastElement);
                }
            }
            else
            {
                _outputBuilder.Write("public");
                _outputBuilder.Write(' ');
                _outputBuilder.Write(typeName);
                _outputBuilder.Write(' ');
                _outputBuilder.Write(escapedName);
            }

            _outputBuilder.WriteLine(';');
        }

        private void VisitFunctionDecl(FunctionDecl functionDecl, Cursor parent)
        {
            var name = GetRemappedCursorName(functionDecl);

            StartUsingOutputBuilder(_config.MethodClassName);
            {
                var type = functionDecl.Type;
                var returnType = functionDecl.ReturnType;
                var body = functionDecl.Body;

                if (body is null)
                {
                    _outputBuilder.AddUsingDirective("System.Runtime.InteropServices");

                    _outputBuilder.WriteIndented("[DllImport(libraryPath, EntryPoint = \"");
                    _outputBuilder.Write(name);
                    _outputBuilder.Write("\", CallingConvention = CallingConvention.");
                    _outputBuilder.Write(GetCallingConventionName(functionDecl, type.CallingConv));
                    _outputBuilder.WriteLine(")]");

                    var marshalAttribute = GetMarshalAttribute(functionDecl, returnType);

                    if (!string.IsNullOrWhiteSpace(marshalAttribute))
                    {
                        _outputBuilder.WriteIndented("[return: ");
                        _outputBuilder.Write(marshalAttribute);
                        _outputBuilder.Write(']');
                        _outputBuilder.WriteLine();
                    }
                }

                _outputBuilder.WriteIndented("public static");
                _outputBuilder.Write(' ');

                if (body is null)
                {
                    _outputBuilder.Write("extern");
                    _outputBuilder.Write(' ');
                }

                _outputBuilder.Write(GetRemappedTypeName(functionDecl, returnType));
                _outputBuilder.Write(' ');
                _outputBuilder.Write(EscapeAndStripName(name));
                _outputBuilder.Write('(');

                foreach (var parmVarDecl in functionDecl.Parameters)
                {
                    Visit(parmVarDecl, functionDecl);
                }

                _outputBuilder.Write(")");

                if (body is null)
                {
                    _outputBuilder.WriteLine(';');
                }
                else
                {
                    _outputBuilder.WriteLine();

                    if (body is CompoundStmt)
                    {
                        Visit(body, functionDecl);
                    }
                    else
                    {
                        _outputBuilder.WriteBlockStart();
                        Visit(body, functionDecl);
                        _outputBuilder.WriteBlockEnd();
                    }
                }

                foreach (var declaration in functionDecl.Declarations)
                {
                    Visit(declaration, functionDecl);
                }
            }
            StopUsingOutputBuilder();
        }

        private void VisitIntegerLiteral(IntegerLiteral integerLiteral, Cursor parent)
        {
            _outputBuilder.Write(integerLiteral.Value);
        }

        private void VisitNamedDecl(NamedDecl namedDecl, Cursor parent)
        {
            // We get the non-remapped name for the purpose of exclusion
            // checks to ensure that users can remove no-definition declarations
            // in favor of remapped anonymous declarations.
            var name = GetCursorName(namedDecl);

            if (_config.ExcludedNames.Contains(name))
            {
                return;
            }

            if (namedDecl is TypeDecl typeDecl)
            {
                VisitTypeDecl(typeDecl, parent);
            }
            else if (namedDecl is ValueDecl valueDecl)
            {
                VisitValueDecl(valueDecl, parent);
            }
            else
            {
                AddDiagnostic(DiagnosticLevel.Error, $"Unsupported named declaration: '{namedDecl.KindSpelling}'. Generated bindings may be incomplete.", namedDecl);
            }
        }

        private void VisitParenExpr(ParenExpr parenExpr, Cursor parent)
        {
            _outputBuilder.Write('(');
            Visit(parenExpr.SubExpr, parenExpr);
            _outputBuilder.Write(')');
        }

        private void VisitParmVarDecl(ParmVarDecl parmVarDecl, Cursor parent)
        {
            if (parent is FunctionDecl functionDecl)
            {
                VisitParmVarDecl(parmVarDecl, functionDecl);
            }
            else if (parent is TypedefDecl typedefDecl)
            {
                VisitParmVarDecl(parmVarDecl, typedefDecl);
            }
            else
            {
                AddDiagnostic(DiagnosticLevel.Error, $"Unsupported parameter variable declaration parent: '{parent.KindSpelling}'. Generated bindings may be incomplete.", parent);
            }
        }

        private void VisitParmVarDecl(ParmVarDecl parmVarDecl, FunctionDecl functionDecl)
        {
            if (functionDecl.Body is null)
            {
                var marshalAttribute = GetMarshalAttribute(parmVarDecl, parmVarDecl.Type);

                if (!string.IsNullOrWhiteSpace(marshalAttribute))
                {
                    _outputBuilder.Write("[");
                    _outputBuilder.Write(marshalAttribute);
                    _outputBuilder.Write(']');
                    _outputBuilder.Write(' ');
                }
            }

            var parmModifier = GetParmModifier(parmVarDecl, parmVarDecl.Type);

            if (!string.IsNullOrWhiteSpace(parmModifier))
            {
                _outputBuilder.Write(parmModifier);
                _outputBuilder.Write(' ');
            }

            _outputBuilder.Write(GetRemappedTypeName(parmVarDecl, parmVarDecl.Type));
            _outputBuilder.Write(' ');

            var name = GetRemappedCursorName(parmVarDecl);
            _outputBuilder.Write(EscapeName(name));

            var parameters = functionDecl.Parameters;
            var index = parameters.IndexOf(parmVarDecl);
            var lastIndex = parameters.Count - 1;

            if (name.Equals("param"))
            {
                _outputBuilder.Write(index);
            }

            if (index != lastIndex)
            {
                _outputBuilder.Write(", ");
            }
        }

        private void VisitParmVarDecl(ParmVarDecl parmVarDecl, TypedefDecl typedefDecl)
        {
            var marshalAttribute = GetMarshalAttribute(parmVarDecl, parmVarDecl.Type);

            if (!string.IsNullOrWhiteSpace(marshalAttribute))
            {
                _outputBuilder.Write("[");
                _outputBuilder.Write(marshalAttribute);
                _outputBuilder.Write(']');
                _outputBuilder.Write(' ');
            }

            var parmModifier = GetParmModifier(parmVarDecl, parmVarDecl.Type);

            if (!string.IsNullOrWhiteSpace(parmModifier))
            {
                _outputBuilder.Write(parmModifier);
                _outputBuilder.Write(' ');
            }

            _outputBuilder.Write(GetRemappedTypeName(parmVarDecl, parmVarDecl.Type));
            _outputBuilder.Write(' ');

            var name = GetRemappedCursorName(parmVarDecl);
            _outputBuilder.Write(EscapeName(name));

            var parameters = typedefDecl.Parameters;
            var index = parameters.IndexOf(parmVarDecl);
            var lastIndex = parameters.Count - 1;

            if (name.Equals("param"))
            {
                _outputBuilder.Write(index);
            }

            if (index != lastIndex)
            {
                _outputBuilder.Write(", ");
            }
        }

        private void VisitRecordDecl(RecordDecl recordDecl, Cursor parent)
        {
            var name = GetRemappedCursorName(recordDecl);

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

                foreach (var field in recordDecl.Fields)
                {
                    Visit(field, recordDecl);
                }

                foreach (var declaration in recordDecl.Declarations)
                {
                    Visit(declaration, recordDecl);
                }

                if (_config.GenerateUnsafeCode)
                {
                    foreach (var constantArray in recordDecl.ConstantArrays)
                    {
                        var type = constantArray.Type;
                        var typeName = GetRemappedTypeName(constantArray, constantArray.Type);

                        if (IsSupportedFixedSizedBufferType(typeName))
                        {
                            continue;
                        }

                        _outputBuilder.WriteLine();
                        _outputBuilder.WriteIndented("public partial struct");
                        _outputBuilder.Write(' ');
                        _outputBuilder.WriteLine(GetArtificalFixedSizedBufferName(constantArray));
                        _outputBuilder.WriteBlockStart();

                        for (int i = 0; i < type.NumElements; i++)
                        {
                            _outputBuilder.WriteIndented("private");
                            _outputBuilder.Write(' ');
                            _outputBuilder.Write(typeName);
                            _outputBuilder.Write(' ');
                            _outputBuilder.Write('e');
                            _outputBuilder.Write(i);
                            _outputBuilder.WriteLine(';');
                        }

                        _outputBuilder.AddUsingDirective("System.Runtime.InteropServices");

                        _outputBuilder.WriteLine();
                        _outputBuilder.WriteIndented("public ref");
                        _outputBuilder.Write(' ');
                        _outputBuilder.Write(typeName);
                        _outputBuilder.Write(' ');
                        _outputBuilder.Write("this[int index] => ref MemoryMarshal.CreateSpan(ref e0, ");
                        _outputBuilder.Write(type.NumElements);
                        _outputBuilder.WriteLine(")[index];");
                        _outputBuilder.WriteBlockEnd();
                    }
                }

                _outputBuilder.WriteBlockEnd();
            }
            StopUsingOutputBuilder();
        }

        private void VisitRef(Ref @ref, Cursor parent)
        {
            AddDiagnostic(DiagnosticLevel.Error, $"Unsupported reference: '{@ref.KindSpelling}'. Generated bindings may be incomplete.", @ref);
        }

        private void VisitReturnStmt(ReturnStmt returnStmt, Cursor parent)
        {
            Debug.Assert(returnStmt.RetValue != null);

            _outputBuilder.WriteIndented("return");
            _outputBuilder.Write(' ');

            Visit(returnStmt.RetValue, returnStmt);
            _outputBuilder.WriteLine(';');
        }

        private void VisitStmt(Stmt stmt, Cursor parent)
        {
            if (stmt is CompoundStmt compoundStmt)
            {
                VisitCompoundStmt(compoundStmt, parent);
            }
            else if (stmt is ReturnStmt returnStmt)
            {
                VisitReturnStmt(returnStmt, parent);
            }
            else if (stmt is ValueStmt valueStmt)
            {
                VisitValueStmt(valueStmt, parent);
            }
            else
            {
                AddDiagnostic(DiagnosticLevel.Error, $"Unsupported statement: '{stmt.KindSpelling}'. Generated bindings may be incomplete.", stmt);
            }
        }

        private void VisitTagDecl(TagDecl tagDecl, Cursor parent)
        {
            if (!tagDecl.IsDefinition && (tagDecl.Definition != null))
            {
                // We don't want to generate bindings for anything
                // that is not itself a definition and that has a
                // definition that can be resolved. This ensures we
                // still generate bindings for things which are used
                // as opaque handles, but which aren't ever defined.

                return;
            }

            if (tagDecl is RecordDecl recordDecl)
            {
                VisitRecordDecl(recordDecl, parent);
            }
            else if (tagDecl is EnumDecl enumDecl)
            {
                VisitEnumDecl(enumDecl, parent);
            }
            else
            {
                AddDiagnostic(DiagnosticLevel.Error, $"Unsupported tag declaration: '{tagDecl.KindSpelling}'. Generated bindings may be incomplete.", tagDecl);
            }
        }

        private void VisitTypeDecl(TypeDecl typeDecl, Cursor parent)
        {
            if (typeDecl is TagDecl tagDecl)
            {
                VisitTagDecl(tagDecl, parent);
            }
            else if (typeDecl is TypedefNameDecl typedefNameDecl)
            {
                VisitTypedefNameDecl(typedefNameDecl, parent);
            }
            else
            {
                AddDiagnostic(DiagnosticLevel.Error, $"Unsupported type declaration: '{typeDecl.KindSpelling}'. Generated bindings may be incomplete.", typeDecl);
            }
        }

        private void VisitTypedefNameDecl(TypedefNameDecl typedefNameDecl, Cursor parent)
        {
            if (typedefNameDecl is TypedefDecl typedefDecl)
            {
                VisitTypedefDecl(typedefDecl, parent);
            }
            else
            {
                AddDiagnostic(DiagnosticLevel.Error, $"Unsupported typedef name declaration: '{typedefNameDecl.KindSpelling}'. Generated bindings may be incomplete.", typedefNameDecl);
            }
        }

        private void VisitTypedefDecl(TypedefDecl typedefDecl, Cursor parent)
        {
            VisitTypedefDecl(typedefDecl, parent, typedefDecl.UnderlyingType);
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
                        var name = GetRemappedCursorName(typedefDecl);

                        StartUsingOutputBuilder(name);
                        {

                            var escapedName = EscapeName(name);

                            _outputBuilder.WriteIndented("public partial struct");
                            _outputBuilder.Write(' ');
                            _outputBuilder.WriteLine(escapedName);
                            _outputBuilder.WriteBlockStart();
                            {
                                var typeName = GetRemappedTypeName(typedefDecl, underlyingType);

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
                        var name = GetRemappedCursorName(typedefDecl);
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
                    var name = GetRemappedCursorName(typedefDecl);
                    StartUsingOutputBuilder(name);
                    {
                        var escapedName = EscapeName(name);

                        _outputBuilder.AddUsingDirective("System.Runtime.InteropServices");

                        _outputBuilder.WriteIndented("[UnmanagedFunctionPointer(CallingConvention.");
                        _outputBuilder.Write(GetCallingConventionName(typedefDecl, pointeeType.CallingConv));
                        _outputBuilder.WriteLine(")]");
                        _outputBuilder.WriteIndented("public delegate");
                        _outputBuilder.Write(' ');
                        _outputBuilder.Write(GetRemappedTypeName(typedefDecl, pointeeType.ResultType));
                        _outputBuilder.Write(' ');
                        _outputBuilder.Write(escapedName);
                        _outputBuilder.Write('(');

                        foreach (var parmVarDecl in typedefDecl.Parameters)
                        {
                            Visit(parmVarDecl, typedefDecl);
                        }

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

        private void VisitUnaryOperator(UnaryOperator unaryOperator, Cursor parent)
        {
            if (unaryOperator.IsPrefix)
            {
                _outputBuilder.Write(unaryOperator.Opcode);
                Visit(unaryOperator.SubExpr, unaryOperator);
            }
            else
            {
                Visit(unaryOperator.SubExpr, unaryOperator);
                _outputBuilder.Write(unaryOperator.Opcode);
            }
        }

        private void VisitValueDecl(ValueDecl valueDecl, Cursor parent)
        {
            if (valueDecl is DeclaratorDecl declaratorDecl)
            {
                VisitDeclaratorDecl(declaratorDecl, parent);
            }
            else if (valueDecl is EnumConstantDecl enumConstantDecl)
            {
                VisitEnumConstantDecl(enumConstantDecl, parent);
            }
            else
            {
                AddDiagnostic(DiagnosticLevel.Error, $"Unsupported value declaration: '{valueDecl.KindSpelling}'. Generated bindings may be incomplete.", valueDecl);
            }
        }

        private void VisitValueStmt(ValueStmt valueStmt, Cursor parent)
        {
            if (valueStmt is Expr expr)
            {
                VisitExpr(expr, parent);
            }
            else
            {
                AddDiagnostic(DiagnosticLevel.Error, $"Unsupported value statement: '{valueStmt.KindSpelling}'. Generated bindings may be incomplete.", valueStmt);
            }
        }

        private void VisitVarDecl(VarDecl varDecl, Cursor parent)
        {
            if (varDecl is ParmVarDecl parmVarDecl)
            {
                VisitParmVarDecl(parmVarDecl, parent);
            }
            else
            {
                var name = GetRemappedCursorName(varDecl);

                StartUsingOutputBuilder(_config.MethodClassName);
                {
                    _outputBuilder.WriteIndented("// public static extern");
                    _outputBuilder.Write(' ');
                    _outputBuilder.Write(GetRemappedTypeName(varDecl, varDecl.Type));
                    _outputBuilder.Write(' ');
                    _outputBuilder.Write(EscapeName(name));
                    _outputBuilder.WriteLine(';');
                }
                StopUsingOutputBuilder();
            }
        }

        private void VisitUnexposedDecl(UnexposedDecl unexposedDecl, Cursor parent)
        {
            foreach (var declaration in unexposedDecl.Declarations)
            {
                Visit(declaration, unexposedDecl);
            }
        }

        private void VisitUnexposedExpr(UnexposedExpr unexposedExpr, Cursor parent)
        {
            foreach (var child in unexposedExpr.Children)
            {
                Visit(child, unexposedExpr);
            }
        }
    }
}
