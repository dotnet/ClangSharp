// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed partial class PInvokeGenerator : IDisposable
    {
        private const int DefaultStreamWriterBufferSize = 1024;
        private static readonly Encoding defaultStreamWriterEncoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: true);

        private readonly CXIndex _index;
        private readonly OutputBuilderFactory _outputBuilderFactory;
        private readonly Func<string, Stream> _outputStreamFactory;
        private readonly StringBuilder _fileContentsBuilder;
        private readonly HashSet<string> _visitedFiles;
        private readonly List<Diagnostic> _diagnostics;
        private readonly LinkedList<Cursor> _context;
        private readonly Dictionary<string, Guid> _uuidsToGenerate;
        private readonly HashSet<string> _generatedUuids;
        private readonly PInvokeGeneratorConfiguration _config;

        private string _filePath;
        private string[] _clangCommandLineArgs;
        private CXTranslationUnit_Flags _translationFlags;

        private OutputBuilder _outputBuilder;
        private OutputBuilder _testOutputBuilder;
        private int _outputBuilderUsers;
        private bool _disposed;
        private bool _isMethodClassUnsafe;

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
            _fileContentsBuilder = new StringBuilder();
            _visitedFiles = new HashSet<string>();
            _diagnostics = new List<Diagnostic>();
            _context = new LinkedList<Cursor>();
            _config = config;
            _uuidsToGenerate = new Dictionary<string, Guid>();
            _generatedUuids = new HashSet<string>();
        }

        ~PInvokeGenerator()
        {
            Dispose(isDisposing: false);
        }

        public PInvokeGeneratorConfiguration Config => _config;

        public Cursor CurrentContext => _context.Last.Value;

        public IReadOnlyList<Diagnostic> Diagnostics => _diagnostics;

        public CXIndex IndexHandle => _index;

        public Cursor PreviousContext => _context.Last.Previous.Value;

        public void Close()
        {
            Stream stream = null;
            OutputBuilder methodClassOutputBuilder = null;
            bool emitNamespaceDeclaration = true;
            bool leaveStreamOpen = false;

            foreach (var foundUuid in _uuidsToGenerate)
            {
                var iidName = foundUuid.Key;

                if (_generatedUuids.Contains(iidName))
                {
                    continue;
                }

                var iidValue = foundUuid.Value.ToString("X").ToUpperInvariant().Replace("{", "").Replace("}", "").Replace('X', 'x').Replace(",", ", ");

                StartUsingOutputBuilder(_config.MethodClassName);

                _outputBuilder.AddUsingDirective("System");

                _outputBuilder.WriteIndented("public static readonly Guid ");
                _outputBuilder.Write(iidName);
                _outputBuilder.Write(" = new Guid(");
                _outputBuilder.Write(iidValue);
                _outputBuilder.Write(")");
                _outputBuilder.WriteSemicolon();
                _outputBuilder.WriteNewline();

                StopUsingOutputBuilder();
            }

            if (!_config.GenerateMultipleFiles)
            {
                var outputPath = _config.OutputLocation;
                stream = _outputStreamFactory(outputPath);
                leaveStreamOpen = true;

                var usingDirectives = Enumerable.Empty<string>();
                var staticUsingDirectives = Enumerable.Empty<string>();

                foreach (var outputBuilder in _outputBuilderFactory.OutputBuilders)
                {
                    usingDirectives = usingDirectives.Concat(outputBuilder.UsingDirectives);
                    staticUsingDirectives = staticUsingDirectives.Concat(outputBuilder.StaticUsingDirectives);
                }

                usingDirectives = usingDirectives.Distinct()
                                                 .OrderBy((usingDirective) => usingDirective);

                staticUsingDirectives = staticUsingDirectives.Distinct()
                                                 .OrderBy((staticUsingDirective) => staticUsingDirective);

                usingDirectives = usingDirectives.Concat(staticUsingDirectives);

                if (usingDirectives.Any())
                {
                    using var sw = new StreamWriter(stream, defaultStreamWriterEncoding, DefaultStreamWriterBufferSize, leaveStreamOpen);
                    {
                        sw.NewLine = "\n";
                        sw.Write(_config.HeaderText);

                        foreach (var usingDirective in usingDirectives)
                        {
                            sw.Write("using ");
                            sw.Write(usingDirective);
                            sw.WriteLine(';');
                        }

                        sw.WriteLine();
                    }
                }
            }

            foreach (var outputBuilder in _outputBuilderFactory.OutputBuilders)
            {
                var outputPath = outputBuilder.IsTestOutput ? _config.TestOutputLocation : _config.OutputLocation;

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

                using var sw = new StreamWriter(stream, defaultStreamWriterEncoding, DefaultStreamWriterBufferSize, leaveStreamOpen);
                sw.NewLine = "\n";

                sw.WriteLine('}');
            }

            _context.Clear();
            _diagnostics.Clear();
            _fileContentsBuilder.Clear();
            _generatedUuids.Clear();
            _outputBuilderFactory.Clear();
            _uuidsToGenerate.Clear();
            _visitedFiles.Clear();
        }

        public void Dispose()
        {
            Dispose(isDisposing: true);
            GC.SuppressFinalize(this);
        }

        public void GenerateBindings(TranslationUnit translationUnit, string filePath, string[] clangCommandLineArgs, CXTranslationUnit_Flags translationFlags)
        {
            Debug.Assert(_outputBuilder is null);

            _filePath = filePath;
            _clangCommandLineArgs = clangCommandLineArgs;
            _translationFlags = translationFlags;

            if (translationUnit.Handle.NumDiagnostics != 0)
            {
                var errorDiagnostics = new StringBuilder();
                errorDiagnostics.AppendLine($"The provided {nameof(CXTranslationUnit)} has the following diagnostics which prevent its use:");
                var invalidTranslationUnitHandle = false;

                for (uint i = 0; i < translationUnit.Handle.NumDiagnostics; ++i)
                {
                    using var diagnostic = translationUnit.Handle.GetDiagnostic(i);

                    if ((diagnostic.Severity == CXDiagnosticSeverity.CXDiagnostic_Error) || (diagnostic.Severity == CXDiagnosticSeverity.CXDiagnostic_Fatal))
                    {
                        invalidTranslationUnitHandle = true;
                        errorDiagnostics.Append(' ', 4);
                        errorDiagnostics.AppendLine(diagnostic.Format(CXDiagnostic.DefaultDisplayOptions).ToString());
                    }
                }

                if (invalidTranslationUnitHandle)
                {
                    throw new ArgumentOutOfRangeException(nameof(translationUnit), errorDiagnostics.ToString());
                }
            }

            if (_config.GenerateMacroBindings)
            {
                var translationUnitHandle = translationUnit.Handle;

                var file = translationUnitHandle.GetFile(_filePath);
                var fileContents = translationUnitHandle.GetFileContents(file, out var size);

#if NETCOREAPP
                _fileContentsBuilder.Append(Encoding.UTF8.GetString(fileContents));
#else
                _fileContentsBuilder.Append(Encoding.UTF8.GetString(fileContents.ToArray()));
#endif

                foreach (var cursor in translationUnit.TranslationUnitDecl.CursorChildren)
                {
                    if (cursor is PreprocessedEntity preprocessedEntity)
                    {
                        VisitPreprocessedEntity(preprocessedEntity);
                    }
                }

                var unsavedFileContents = _fileContentsBuilder.ToString();
                _fileContentsBuilder.Clear();

                using var unsavedFile = CXUnsavedFile.Create(_filePath, unsavedFileContents);
                var unsavedFiles = new CXUnsavedFile[] { unsavedFile };

                translationFlags = _translationFlags & ~CXTranslationUnit_Flags.CXTranslationUnit_DetailedPreprocessingRecord;
                var handle = CXTranslationUnit.Parse(IndexHandle, _filePath, _clangCommandLineArgs, unsavedFiles, translationFlags);

                using var nestedTranslationUnit = TranslationUnit.GetOrCreate(handle);
                Visit(nestedTranslationUnit.TranslationUnitDecl);
            }
            else
            {
                Visit(translationUnit.TranslationUnitDecl);
            }
        }

        private void AddDiagnostic(DiagnosticLevel level, string message)
        {
            var diagnostic = new Diagnostic(level, message);

            if (_diagnostics.Contains(diagnostic))
            {
                return;
            }

            _diagnostics.Add(diagnostic);
        }

        private void AddDiagnostic(DiagnosticLevel level, string message, Cursor cursor)
        {
            var diagnostic = new Diagnostic(level, message, (cursor?.Location).GetValueOrDefault());

            if (_diagnostics.Contains(diagnostic))
            {
                return;
            }

            _diagnostics.Add(diagnostic);
        }

        private void AddCppAttributes(ParmVarDecl parmVarDecl, string prefix = null, string postfix = null)
        {
            if (!_config.GenerateCppAttributes)
            {
                return;
            }

            if (parmVarDecl.Attrs.Count == 0)
            {
                return;
            }

            if (prefix is null)
            {
                _outputBuilder.WriteIndentation();
            }
            else
            {
                _outputBuilder.WriteNewlineIfNeeded();
                _outputBuilder.Write(prefix);
            }

            _outputBuilder.Write($"[CppAttributeList(\"");

            for (int i = 0; i < parmVarDecl.Attrs.Count; i++)
            {
                var attr = EscapeString(parmVarDecl.Attrs[i].Spelling);
                if (i != 0)
                {
                    _outputBuilder.Write('^');
                }

                _outputBuilder.Write(attr);
            }

            _outputBuilder.Write($"\")]");

            if (postfix is null)
            {
                _outputBuilder.NeedsNewline = true;
            }
            else
            {
                _outputBuilder.Write(postfix);
            }
        }

        private void AddNativeInheritanceAttribute(string inheritedFromName, string prefix = null, string postfix = null, string attributePrefix = null)
        {
            if (prefix is null)
            {
                _outputBuilder.WriteIndentation();
            }
            else
            {
                _outputBuilder.WriteNewlineIfNeeded();
                _outputBuilder.Write(prefix);
            }

            _outputBuilder.Write('[');

            if (attributePrefix != null)
            {
                _outputBuilder.Write(attributePrefix);
            }

            _outputBuilder.Write("NativeInheritance");
            _outputBuilder.Write('(');

            _outputBuilder.Write('"');
            _outputBuilder.Write(EscapeString(inheritedFromName));
            _outputBuilder.Write('"');
            _outputBuilder.Write(')');
            _outputBuilder.Write(']');

            if (postfix is null)
            {
                _outputBuilder.NeedsNewline = true;
            }
            else
            {
                _outputBuilder.Write(postfix);
            }
        }

        private void AddNativeTypeNameAttribute(string nativeTypeName, string prefix = null, string postfix = null, string attributePrefix = null)
        {
            if (string.IsNullOrWhiteSpace(nativeTypeName))
            {
                return;
            }

            if (prefix is null)
            {
                _outputBuilder.WriteIndentation();
            }
            else
            {
                _outputBuilder.WriteNewlineIfNeeded();
                _outputBuilder.Write(prefix);
            }

            _outputBuilder.Write('[');

            if (attributePrefix != null)
            {
                _outputBuilder.Write(attributePrefix);
            }

            _outputBuilder.Write("NativeTypeName(\"");
            _outputBuilder.Write(EscapeString(nativeTypeName));
            _outputBuilder.Write("\")]");

            if (postfix is null)
            {
                _outputBuilder.NeedsNewline = true;
            }
            else
            {
                _outputBuilder.Write(postfix);
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

            using var sw = new StreamWriter(stream, defaultStreamWriterEncoding, DefaultStreamWriterBufferSize, leaveStreamOpen);
            sw.NewLine = "\n";

            if (_config.GenerateMultipleFiles)
            {
                if (_config.HeaderText != string.Empty)
                {
                    sw.WriteLine(_config.HeaderText);
                }

                var usingDirectives = outputBuilder.UsingDirectives.Concat(outputBuilder.StaticUsingDirectives);

                if (usingDirectives.Any())
                {
                    foreach(var usingDirective in usingDirectives)
                    {
                        sw.Write("using ");
                        sw.Write(usingDirective);
                        sw.WriteLine(';');
                    }

                    sw.WriteLine();
                }
            }

            var indentationString = outputBuilder.IndentationString;

            if (emitNamespaceDeclaration)
            {
                sw.Write("namespace ");
                sw.Write(Config.Namespace);

                if (outputBuilder.IsTestOutput)
                {
                    sw.Write(".UnitTests");
                }

                sw.WriteLine();
                sw.WriteLine('{');
            }
            else
            {
                sw.WriteLine();
            }

            if (isMethodClass)
            {
                sw.Write(indentationString);
                sw.Write("public static ");

                if (_isMethodClassUnsafe)
                {
                    sw.Write("unsafe ");
                }

                sw.Write("partial class ");
                sw.WriteLine(Config.MethodClassName);
                sw.Write(indentationString);
                sw.WriteLine('{');

                indentationString += outputBuilder.IndentationString;
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

        private string EscapeCharacter(char value) => EscapeString(value.ToString());

        private string EscapeString(string value) => value.Replace("\\", "\\\\")
                                                          .Replace("\r", "\\r")
                                                          .Replace("\n", "\\n")
                                                          .Replace("\t", "\\t")
                                                          .Replace("\"", "\\\"");

        private string GetAccessSpecifierName(NamedDecl namedDecl)
        {
            string name;

            switch (namedDecl.Access)
            {
                case CX_CXXAccessSpecifier.CX_CXXInvalidAccessSpecifier:
                {
                    // Top level declarations will have an invalid access specifier
                    name = "public";
                    break;
                }

                case CX_CXXAccessSpecifier.CX_CXXPublic:
                {
                    name = "public";
                    break;
                }

                case CX_CXXAccessSpecifier.CX_CXXProtected:
                {
                    name = "protected";
                    break;
                }

                case CX_CXXAccessSpecifier.CX_CXXPrivate:
                {
                    name = "private";
                    break;
                }

                default:
                {
                    name = "internal";
                    AddDiagnostic(DiagnosticLevel.Warning, $"Unknown access specifier: '{namedDecl.Access}'. Falling back to '{name}'.", namedDecl);
                    break;
                }
            }

            return name;
        }

        private string GetAnonymousName(Cursor cursor, string kind)
        {
            cursor.Location.GetFileLocation(out var file, out var line, out var column, out _);
            var fileName = Path.GetFileNameWithoutExtension(file.Name.ToString());
            return $"__Anonymous{kind}_{fileName}_L{line}_C{column}";
        }

        private string GetArtificialFixedSizedBufferName(FieldDecl fieldDecl)
        {
            var name = GetRemappedCursorName(fieldDecl);
            return $"_{name}_e__FixedBuffer";
        }

        private Type[] GetBitfieldCount(RecordDecl recordDecl)
        {
            var types = new List<Type>(recordDecl.Fields.Count);

            var count = 0;
            var previousSize = 0L;
            var remainingBits = 0L;

            foreach (var fieldDecl in recordDecl.Fields)
            {
                if (!fieldDecl.IsBitField)
                {
                    previousSize = 0;
                    remainingBits = 0;
                    continue;
                }

                var currentSize = fieldDecl.Type.Handle.SizeOf;

                if ((!_config.GenerateUnixTypes && (currentSize != previousSize)) || (fieldDecl.BitWidthValue > remainingBits))
                {
                    count++;
                    remainingBits = currentSize * 8;
                    previousSize = 0;

                    var type = fieldDecl.Type;

                    if (type.CanonicalType is EnumType enumType)
                    {
                        type = enumType.Decl.IntegerType;
                    }

                    types.Add(type);
                }
                else if (_config.GenerateUnixTypes && (currentSize > previousSize))
                {
                    remainingBits += (currentSize - previousSize) * 8;

                    var type = fieldDecl.Type;

                    if (type.CanonicalType is EnumType enumType)
                    {
                        type = enumType.Decl.IntegerType;
                    }

                    types[types.Count - 1] = type;
                }

                remainingBits -= fieldDecl.BitWidthValue;
                previousSize = Math.Max(previousSize, currentSize);
            }

            return types.ToArray();
        }

        private string GetCallingConventionName(Cursor cursor, CXCallingConv callingConvention, string remappedName, bool isForFnptr)
        {
            if (_config.WithCallConvs.TryGetValue(remappedName, out string callConv) || _config.WithCallConvs.TryGetValue("*", out callConv))
            {
                return callConv;
            }

            switch (callingConvention)
            {
                case CXCallingConv.CXCallingConv_C:
                {
                    return "Cdecl";
                }

                case CXCallingConv.CXCallingConv_X86StdCall:
                {
                    return isForFnptr ? "Stdcall" : "StdCall";
                }

                case CXCallingConv.CXCallingConv_X86FastCall:
                {
                    return isForFnptr ? "Fastcall" : "FastCall";
                }

                case CXCallingConv.CXCallingConv_X86ThisCall:
                {
                    return isForFnptr ? "Thiscall" : "ThisCall";
                }

                case CXCallingConv.CXCallingConv_Win64:
                {
                    return "Winapi";
                }

                default:
                {
                    var name = "Winapi";
                    AddDiagnostic(DiagnosticLevel.Warning, $"Unsupported calling convention: '{callingConvention}'. Falling back to '{name}'.", cursor);
                    return name;
                }
            }
        }

        private string GetCursorName(NamedDecl namedDecl)
        {
            var name = namedDecl.Name.Replace('\\', '/');

            if (string.IsNullOrWhiteSpace(name))
            {
                if (namedDecl is TypeDecl typeDecl)
                {
                    if ((typeDecl is TagDecl tagDecl) && tagDecl.Handle.IsAnonymous)
                    {
                        name = GetAnonymousName(tagDecl, tagDecl.TypeForDecl.KindSpelling);

                    }
                    else
                    {
                        name = GetTypeName(namedDecl, context: null, typeDecl.TypeForDecl, out var nativeTypeName);
                        Debug.Assert(string.IsNullOrWhiteSpace(nativeTypeName));
                    }
                }
                else if (namedDecl is ParmVarDecl)
                {
                    name = "param";
                }
                else if (namedDecl is FieldDecl fieldDecl)
                {
                    name = GetAnonymousName(fieldDecl, fieldDecl.CursorKindSpelling);
                }
                else
                {
                    AddDiagnostic(DiagnosticLevel.Error, $"Unsupported anonymous named declaration: '{namedDecl.Kind}'.", namedDecl);
                }
            }
            else if (namedDecl is CXXDestructorDecl)
            {
                name = "Finalize";
            }

            Debug.Assert(!string.IsNullOrWhiteSpace(name));
            return name;
        }

        private string GetCursorQualifiedName(NamedDecl namedDecl, bool truncateFunctionParameters = false)
        {
            var parts = new Stack<NamedDecl>();

            for (Decl decl = namedDecl; decl.DeclContext != null; decl = (Decl)decl.DeclContext)
            {
                if (decl is NamedDecl)
                {
                    parts.Push((NamedDecl)decl);
                }
            }

            var qualifiedName = new StringBuilder();

            NamedDecl part = parts.Pop();

            while (parts.Count != 0)
            {
                AppendNamedDecl(part, GetCursorName(part), qualifiedName);
                qualifiedName.Append('.');
                part = parts.Pop();
            }

            AppendNamedDecl(part, GetCursorName(part), qualifiedName);

            return qualifiedName.ToString();

            void AppendFunctionParameters(CXType functionType, StringBuilder qualifiedName)
            {
                if (truncateFunctionParameters)
                {
                    return;
                }

                qualifiedName.Append('(');

                if (functionType.NumArgTypes != 0)
                {
                    qualifiedName.Append(functionType.GetArgType(0).Spelling);

                    for (uint i = 1; i < functionType.NumArgTypes; i++)
                    {
                        qualifiedName.Append(',');
                        qualifiedName.Append(' ');
                        qualifiedName.Append(functionType.GetArgType(i).Spelling);
                    }
                }

                qualifiedName.Append(')');
                qualifiedName.Append(':');

                qualifiedName.Append(functionType.ResultType.Spelling);

                if (functionType.ExceptionSpecificationType == CXCursor_ExceptionSpecificationKind.CXCursor_ExceptionSpecificationKind_NoThrow)
                {
                    qualifiedName.Append(' ');
                    qualifiedName.Append("nothrow");
                }
            }

            void AppendNamedDecl(NamedDecl namedDecl, string name, StringBuilder qualifiedName)
            {
                qualifiedName.Append(name);

                if (namedDecl is FunctionDecl functionDecl)
                {
                    AppendFunctionParameters(functionDecl.Type.Handle, qualifiedName);
                }
                else if (namedDecl is TemplateDecl templateDecl)
                {
                    AppendTemplateParameters(templateDecl, qualifiedName);

                    if (namedDecl is FunctionTemplateDecl functionTemplateDecl)
                    {
                        AppendFunctionParameters(functionTemplateDecl.Handle.Type, qualifiedName);
                    }
                }
                else if (namedDecl is ClassTemplateSpecializationDecl classTemplateSpecializationDecl)
                {
                    AppendTemplateArguments(classTemplateSpecializationDecl, qualifiedName);
                }
            }

            void AppendTemplateArgument(TemplateArgument templateArgument, Decl parentDecl, StringBuilder qualifiedName)
            {
                switch (templateArgument.Kind)
                {
                    case CXTemplateArgumentKind.CXTemplateArgumentKind_Type:
                    {
                        qualifiedName.Append(templateArgument.AsType.AsString);
                        break;
                    }

                    case CXTemplateArgumentKind.CXTemplateArgumentKind_Integral:
                    {
                        qualifiedName.Append(templateArgument.AsIntegral);
                        break;
                    }

                    default:
                    {
                        qualifiedName.Append('?');
                        break;
                    }
                }
            }

            void AppendTemplateArguments(ClassTemplateSpecializationDecl classTemplateSpecializationDecl, StringBuilder qualifiedName)
            {
                qualifiedName.Append('<');

                var templateArgs = classTemplateSpecializationDecl.TemplateArgs;

                if (templateArgs.Any())
                {
                    AppendTemplateArgument(templateArgs[0], classTemplateSpecializationDecl, qualifiedName);

                    for (int i = 1; i < templateArgs.Count; i++)
                    {
                        qualifiedName.Append(',');
                        qualifiedName.Append(' ');
                        AppendTemplateArgument(templateArgs[i], classTemplateSpecializationDecl, qualifiedName);
                    }
                }

                qualifiedName.Append('>');
            }

            void AppendTemplateParameters(TemplateDecl templateDecl, StringBuilder qualifiedName)
            {
                qualifiedName.Append('<');

                var templateParameters = templateDecl.TemplateParameters;

                if (templateParameters.Any())
                {
                    qualifiedName.Append(templateParameters[0].Name);

                    for (int i = 1; i < templateParameters.Count; i++)
                    {
                        qualifiedName.Append(',');
                        qualifiedName.Append(' ');
                        qualifiedName.Append(templateParameters[i].Name);
                    }
                }

                qualifiedName.Append('>');
            }
        }

        private Expr GetExprAsWritten(Expr expr, bool removeParens)
        {
            do
            {
                if (expr is ImplicitCastExpr implicitCastExpr)
                {
                    expr = implicitCastExpr.SubExprAsWritten;
                }
                else if (removeParens && (expr is ParenExpr parenExpr))
                {
                    expr = parenExpr.SubExpr;
                }
                else
                {
                    return expr;
                }
            }
            while (true);
        }

        private static CXXRecordDecl GetRecordDeclForBaseSpecifier(CXXBaseSpecifier cxxBaseSpecifier)
        {
            Type baseType = cxxBaseSpecifier.Type;

            while (!(baseType is RecordType))
            {
                if (baseType is AttributedType attributedType)
                {
                    baseType = attributedType.ModifiedType;
                }
                else if (baseType is ElaboratedType elaboratedType)
                {
                    baseType = elaboratedType.CanonicalType;
                }
                else if (baseType is TypedefType typedefType)
                {
                    baseType = typedefType.Decl.UnderlyingType;
                }
                else
                {
                    break;
                }
            }

            var baseRecordType = (RecordType)baseType;
            return (CXXRecordDecl)baseRecordType.Decl;
        }

        private string GetRemappedCursorName(NamedDecl namedDecl)
        {
            var name = GetCursorQualifiedName(namedDecl);
            var remappedName = GetRemappedName(name, namedDecl, tryRemapOperatorName: true);

            if (remappedName != name)
            {
                return remappedName;
            }

            name = GetCursorQualifiedName(namedDecl, truncateFunctionParameters: true);
            remappedName = GetRemappedName(name, namedDecl, tryRemapOperatorName: true);

            if (remappedName != name)
            {
                return remappedName;
            }

            name = GetCursorName(namedDecl);
            remappedName = GetRemappedName(name, namedDecl, tryRemapOperatorName: true);

            if (remappedName != name)
            {
                return remappedName;
            }

            if ((namedDecl is FieldDecl fieldDecl) && name.StartsWith("__AnonymousField_"))
            {
                remappedName = "Anonymous";

                if (fieldDecl.Parent.AnonymousDecls.Count > 1)
                {
                    var index = fieldDecl.Parent.AnonymousDecls.IndexOf(fieldDecl) + 1;
                    remappedName += index.ToString();
                }
            }
            else if ((namedDecl is RecordDecl recordDecl) && name.StartsWith("__AnonymousRecord_"))
            {
                remappedName = "_Anonymous";

                if (recordDecl.Parent is RecordDecl parentRecordDecl)
                {
                    var matchingField = parentRecordDecl.Fields.Where((fieldDecl) => fieldDecl.Type.CanonicalType == recordDecl.TypeForDecl.CanonicalType).FirstOrDefault();

                    if (matchingField != null)
                    {
                        remappedName = "_";
                        remappedName += GetRemappedCursorName(matchingField);
                    }
                    else if (parentRecordDecl.AnonymousDecls.Count > 1)
                    {
                        var index = parentRecordDecl.AnonymousDecls.IndexOf(recordDecl) + 1;
                        remappedName += index.ToString();
                    }
                }
                remappedName += $"_e__{(recordDecl.IsUnion ? "Union" : "Struct")}";
            }

            return remappedName;
        }

        private string GetRemappedName(string name, Cursor cursor, bool tryRemapOperatorName)
        {
            if (_config.RemappedNames.TryGetValue(name, out string remappedName))
            {
                return AddUsingDirectiveIfNeeded(remappedName);
            }

            if (name.StartsWith("const ") && _config.RemappedNames.TryGetValue(name.Substring(6), out remappedName))
            {
                return AddUsingDirectiveIfNeeded(remappedName);
            }

            remappedName = name;

            if ((cursor is FunctionDecl functionDecl) && tryRemapOperatorName && TryRemapOperatorName(ref remappedName, functionDecl))
            {
                return AddUsingDirectiveIfNeeded(remappedName);
            }

            return AddUsingDirectiveIfNeeded(remappedName);

            string AddUsingDirectiveIfNeeded(string remappedName)
            {
                if (remappedName.Equals("Guid") || remappedName.Equals("IntPtr") || remappedName.Equals("UIntPtr"))
                {
                    _outputBuilder?.AddUsingDirective("System");
                }

                return remappedName;
            }
        }

        private string GetRemappedTypeName(Cursor cursor, Cursor context, Type type, out string nativeTypeName)
        {
            var name = GetTypeName(cursor, context, type, out nativeTypeName);
            name = GetRemappedName(name, cursor, tryRemapOperatorName: false);

            if (name.Contains("::"))
            {
                name = name.Split(new string[] { "::" }, StringSplitOptions.RemoveEmptyEntries).Last();
                name = GetRemappedName(name, cursor, tryRemapOperatorName: false);
            }

            var canonicalType = type.CanonicalType;

            if ((canonicalType is ConstantArrayType constantArrayType) && (constantArrayType.ElementType is RecordType))
            {
                canonicalType = constantArrayType.ElementType;
            }

            if ((canonicalType is RecordType recordType) && name.StartsWith("__AnonymousRecord_"))
            {
                var recordDecl = recordType.Decl;
                name = "_Anonymous";

                if (recordDecl.Parent is RecordDecl parentRecordDecl)
                {
                    var matchingField = parentRecordDecl.Fields.Where((fieldDecl) => fieldDecl.Type.CanonicalType == recordType).FirstOrDefault();

                    if (matchingField != null)
                    {
                        name = "_";
                        name += GetRemappedCursorName(matchingField);
                    }
                    else if (parentRecordDecl.AnonymousDecls.Count > 1)
                    {
                        var index = parentRecordDecl.AnonymousDecls.IndexOf(cursor) + 1;
                        name += index.ToString();
                    }
                }

                name += $"_e__{(recordDecl.IsUnion ? "Union" : "Struct")}";
            }
            else if (cursor is EnumDecl enumDecl)
            {
                var enumDeclName = GetRemappedCursorName(enumDecl);

                if (enumDecl.Enumerators.Any((enumConstantDecl) => IsForceDwordOrForceUInt(enumDeclName, enumConstantDecl)))
                {
                    name = "uint";
                }

                WithType("*", ref name, ref nativeTypeName);
                WithType(enumDeclName, ref name, ref nativeTypeName);
            }

            if (nativeTypeName.Equals(name))
            {
                nativeTypeName = string.Empty;
            }
            return name;

            bool IsForceDwordOrForceUInt(string enumDeclName, EnumConstantDecl enumConstantDecl)
            {
                var enumConstantDeclName = GetRemappedCursorName(enumConstantDecl);
                return (enumConstantDeclName == $"{enumDeclName}_FORCE_DWORD") || (enumConstantDeclName == $"{enumDeclName}_FORCE_UINT");
            }
        }

        private string GetSourceRangeContents(CXTranslationUnit translationUnit, CXSourceRange sourceRange)
        {
            sourceRange.Start.GetFileLocation(out var startFile, out var startLine, out var startColumn, out var startOffset);
            sourceRange.End.GetFileLocation(out var endFile, out var endLine, out var endColumn, out var endOffset);

            if (startFile != endFile)
            {
                return string.Empty;
            }

            var fileContents = translationUnit.GetFileContents(startFile, out var fileSize);
            fileContents = fileContents.Slice(unchecked((int)startOffset), unchecked((int)(endOffset - startOffset)));

#if NETCOREAPP
            return Encoding.UTF8.GetString(fileContents);
#else
            return Encoding.UTF8.GetString(fileContents.ToArray());
#endif
        }

        private string GetTypeName(Cursor cursor, Cursor context, Type type, out string nativeTypeName)
        {
            var name = type.AsString.Replace('\\', '/');
            nativeTypeName = name;

            if (type is ArrayType arrayType)
            {
                name = GetTypeName(cursor, context, arrayType.ElementType, out var nativeElementTypeName);

                if ((cursor is FunctionDecl) || (cursor is ParmVarDecl))
                {
                    name = GetRemappedName(name, cursor, tryRemapOperatorName: false);
                    name += '*';
                }
            }
            else if (type is AttributedType attributedType)
            {
                name = GetTypeName(cursor, context, attributedType.ModifiedType, out var nativeModifiedTypeName);
            }
            else if (type is BuiltinType)
            {
                switch (type.Kind)
                {
                    case CXTypeKind.CXType_Void:
                    {
                        name = (cursor is null) ? "Void" : "void";
                        break;
                    }

                    case CXTypeKind.CXType_Bool:
                    {
                        name = (cursor is null) ? "Boolean" : "bool";
                        break;
                    }

                    case CXTypeKind.CXType_Char_U:
                    case CXTypeKind.CXType_UChar:
                    {
                        name = (cursor is null) ? "Byte" : "byte";
                        break;
                    }

                    case CXTypeKind.CXType_Char16:
                    case CXTypeKind.CXType_UShort:
                    {
                        name = (cursor is null) ? "UInt16" : "ushort";
                        break;
                    }

                    case CXTypeKind.CXType_UInt:
                    {
                        name = (cursor is null) ? "UInt32" : "uint";
                        break;
                    }

                    case CXTypeKind.CXType_ULong:
                    {
                        if (_config.GenerateUnixTypes)
                        {
                            name = _config.GeneratePreviewCodeNint ? "nuint" : "UIntPtr";
                        }
                        else
                        {
                            goto case CXTypeKind.CXType_UInt;
                        }
                        break;
                    }

                    case CXTypeKind.CXType_ULongLong:
                    {
                        name = (cursor is null) ? "UInt64" : "ulong";
                        break;
                    }

                    case CXTypeKind.CXType_Char_S:
                    case CXTypeKind.CXType_SChar:
                    {
                        name = (cursor is null) ? "SByte" : "sbyte";
                        break;
                    }

                    case CXTypeKind.CXType_WChar:
                    {
                        if (_config.GenerateUnixTypes)
                        {
                            goto case CXTypeKind.CXType_Int;
                        }
                        else
                        {
                            goto case CXTypeKind.CXType_UShort;
                        }
                    }

                    case CXTypeKind.CXType_Short:
                    {
                        name = (cursor is null) ? "Int16" : "short";
                        break;
                    }

                    case CXTypeKind.CXType_Int:
                    {
                        name = (cursor is null) ? "Int32" : "int";
                        break;
                    }

                    case CXTypeKind.CXType_Long:
                    {
                        if (_config.GenerateUnixTypes)
                        {
                            name = _config.GeneratePreviewCodeNint ? "nint" : "IntPtr";
                        }
                        else
                        {
                            goto case CXTypeKind.CXType_Int;
                        }
                        break;
                    }

                    case CXTypeKind.CXType_LongLong:
                    {
                        name = (cursor is null) ? "Int64" : "long";
                        break;
                    }

                    case CXTypeKind.CXType_Float:
                    {
                        name = (cursor is null) ? "Single" : "float";
                        break;
                    }

                    case CXTypeKind.CXType_Double:
                    {
                        name = (cursor is null) ? "Double" : "double";
                        break;
                    }

                    default:
                    {
                        AddDiagnostic(DiagnosticLevel.Warning, $"Unsupported builtin type: '{type.TypeClass}'. Falling back '{name}'.", cursor);
                        break;
                    }
                }
            }
            else if (type is DeducedType deducedType)
            {
                name = GetTypeName(cursor, context, deducedType.CanonicalType, out var nativeDeducedTypeName);
            }
            else if (type is ElaboratedType elaboratedType)
            {
                name = GetTypeName(cursor, context, elaboratedType.NamedType, out var nativeNamedTypeName);
            }
            else if (type is FunctionType functionType)
            {
                name = GetTypeNameForPointeeType(cursor, context, functionType, out var nativeFunctionTypeName);
            }
            else if (type is PointerType pointerType)
            {
                name = GetTypeNameForPointeeType(cursor, context, pointerType.PointeeType, out var nativePointeeTypeName);
            }
            else if (type is ReferenceType referenceType)
            {
                name = GetTypeNameForPointeeType(cursor, context, referenceType.PointeeType, out var nativePointeeTypeName);
            }
            else if (type is TagType tagType)
            {
                if (tagType.Decl.Handle.IsAnonymous)
                {
                    name = GetAnonymousName(tagType.Decl, tagType.KindSpelling);
                }
                else if (tagType.Handle.IsConstQualified)
                {
                    name = GetTypeName(cursor, context, tagType.Decl.TypeForDecl, out var nativeDeclTypeName);
                }
                else
                {
                    // The default name should be correct
                }
            }
            else if (type is TypedefType typedefType)
            {
                // We check remapped names here so that types that have variable sizes
                // can be treated correctly. Otherwise, they will resolve to a particular
                // platform size, based on whatever parameters were passed into clang.

                var remappedName = GetRemappedName(name, cursor, tryRemapOperatorName: false);

                if (remappedName.Equals(name))
                {
                    name = GetTypeName(cursor, context, typedefType.Decl.UnderlyingType, out var nativeUnderlyingTypeName);
                }
                else
                {
                    name = remappedName;
                }
            }
            else
            {
                AddDiagnostic(DiagnosticLevel.Warning, $"Unsupported type: '{type.TypeClass}'. Falling back '{name}'.", cursor);
            }

            Debug.Assert(!string.IsNullOrWhiteSpace(name));
            Debug.Assert(!string.IsNullOrWhiteSpace(nativeTypeName));

            if (nativeTypeName.Equals(name))
            {
                nativeTypeName = string.Empty;
            }
            return name;
        }

        private string GetTypeNameForPointeeType(Cursor cursor, Cursor context, Type pointeeType, out string nativePointeeTypeName)
        {
            var name = pointeeType.AsString;
            nativePointeeTypeName = name;

            if (pointeeType is AttributedType attributedType)
            {
                name = GetTypeNameForPointeeType(cursor, context, attributedType.ModifiedType, out var nativeModifiedTypeName);
            }
            else if (pointeeType is FunctionType functionType)
            {
                if (_config.GeneratePreviewCodeFnptr && (functionType is FunctionProtoType functionProtoType))
                {
                    var remappedName = GetRemappedName(name, cursor, tryRemapOperatorName: false);
                    var callConv = GetCallingConventionName(cursor, functionType.CallConv, remappedName, isForFnptr: true);

                    var needsReturnFixup = false;
                    var returnTypeName = GetRemappedTypeName(cursor, context: null, functionType.ReturnType, out _);

                    if (returnTypeName == "bool")
                    {
                        // bool is not blittable, so we shouldn't use it for P/Invoke signatures
                        returnTypeName = "byte";
                    }

                    var nameBuilder = new StringBuilder();
                    nameBuilder.Append("delegate");
                    nameBuilder.Append('*');

                    var isMacroDefinitionRecord = (cursor is VarDecl varDecl) && GetCursorName(varDecl).StartsWith("ClangSharpMacro_");

                    if (!isMacroDefinitionRecord)
                    {
                        nameBuilder.Append(" unmanaged");

                        if (callConv != "Winapi")
                        {
                            nameBuilder.Append('[');
                            nameBuilder.Append(callConv);
                            nameBuilder.Append(']');
                        }
                    }

                    nameBuilder.Append('<');

                    if ((cursor is CXXMethodDecl cxxMethodDecl) && (context is CXXRecordDecl cxxRecordDecl))
                    {
                        var cxxRecordDeclName = GetRemappedCursorName(cxxRecordDecl);
                        needsReturnFixup = cxxMethodDecl.IsVirtual && NeedsReturnFixup(cxxMethodDecl);

                        nameBuilder.Append(EscapeName(cxxRecordDeclName));
                        nameBuilder.Append('*');
                        nameBuilder.Append(',');
                        nameBuilder.Append(' ');

                        if (needsReturnFixup)
                        {
                            nameBuilder.Append(returnTypeName);
                            nameBuilder.Append('*');
                            nameBuilder.Append(',');
                            nameBuilder.Append(' ');
                        }
                    }

                    IEnumerable<Type> paramTypes = functionProtoType.ParamTypes;

                    if (isMacroDefinitionRecord)
                    {
                        varDecl = (VarDecl)cursor;

                        if (IsStmtAsWritten(varDecl.Init, out DeclRefExpr declRefExpr, removeParens: true) && (declRefExpr.Decl is FunctionDecl functionDecl))
                        {
                            cursor = functionDecl;
                            paramTypes = functionDecl.Parameters.Select((param) => param.Type);
                            returnTypeName = GetRemappedTypeName(cursor, context: null, functionDecl.ReturnType, out _);
                        }
                    }

                    foreach (var paramType in paramTypes)
                    {
                        var typeName = GetRemappedTypeName(cursor, context: null, paramType, out _);

                        if (typeName == "bool")
                        {
                            // bool is not blittable, so we shouldn't use it for P/Invoke signatures
                            typeName = "byte";
                        }

                        nameBuilder.Append(typeName);
                        nameBuilder.Append(',');
                        nameBuilder.Append(' ');
                    }

                    nameBuilder.Append(returnTypeName);

                    if (needsReturnFixup)
                    {
                        nameBuilder.Append('*');
                    }

                    nameBuilder.Append('>');
                    name = nameBuilder.ToString();
                }
                else
                {
                    name = "IntPtr";
                }
            }
            else
            {
                name = GetTypeName(cursor, context, pointeeType, out nativePointeeTypeName);
                name = GetRemappedName(name, cursor, tryRemapOperatorName: false);
                name += '*';
            }

            return name;
        }

        private void GetTypeSize(Cursor cursor, Type type, ref long alignment32, ref long alignment64, out long size32, out long size64)
        {
            size32 = 0;
            size64 = 0;

            if (type is ArrayType arrayType)
            {
                if (type is ConstantArrayType constantArrayType)
                {
                    GetTypeSize(cursor, arrayType.ElementType, ref alignment32, ref alignment64, out var elementSize32, out var elementSize64);

                    size32 = elementSize32 * Math.Max(constantArrayType.Size, 1);
                    size64 = elementSize64 * Math.Max(constantArrayType.Size, 1);

                    if (alignment32 == -1)
                    {
                        alignment32 = elementSize32;
                    }

                    if (alignment64 == -1)
                    {
                        alignment64 = elementSize64;
                    }
                }
                else if (type is IncompleteArrayType incompleteArrayType)
                {
                    GetTypeSize(cursor, arrayType.ElementType, ref alignment32, ref alignment64, out var elementSize32, out var elementSize64);

                    size32 = elementSize32;
                    size64 = elementSize64;

                    if (alignment32 == -1)
                    {
                        alignment32 = elementSize32;
                    }

                    if (alignment64 == -1)
                    {
                        alignment64 = elementSize64;
                    }
                }
                else
                {
                    size32 = 4;
                    size64 = 8;

                    if (alignment32 == -1)
                    {
                        alignment32 = 4;
                    }

                    if (alignment64 == -1)
                    {
                        alignment64 = 8;
                    }
                }
            }
            else if (type is AttributedType attributedType)
            {
                GetTypeSize(cursor, attributedType.ModifiedType, ref alignment32, ref alignment64, out size32, out size64);
            }
            else if (type is BuiltinType)
            {
                switch (type.Kind)
                {
                    case CXTypeKind.CXType_Bool:
                    case CXTypeKind.CXType_Char_U:
                    case CXTypeKind.CXType_UChar:
                    case CXTypeKind.CXType_Char_S:
                    case CXTypeKind.CXType_SChar:
                    {
                        size32 = 1;
                        size64 = 1;
                        break;
                    }

                    case CXTypeKind.CXType_UShort:
                    case CXTypeKind.CXType_Short:
                    {
                        size32 = 2;
                        size64 = 2;
                        break;
                    }

                    case CXTypeKind.CXType_UInt:
                    case CXTypeKind.CXType_Int:
                    case CXTypeKind.CXType_Float:
                    {
                        size32 = 4;
                        size64 = 4;
                        break;
                    }

                    case CXTypeKind.CXType_ULong:
                    case CXTypeKind.CXType_Long:
                    {
                        if (_config.GenerateUnixTypes)
                        {
                            size32 = 4;
                            size64 = 8;

                            if (alignment64 == -1)
                            {
                                alignment64 = 8;
                            }
                        }
                        else
                        {
                            goto case CXTypeKind.CXType_UInt;
                        }
                        break;
                    }

                    case CXTypeKind.CXType_ULongLong:
                    case CXTypeKind.CXType_LongLong:
                    case CXTypeKind.CXType_Double:
                    {
                        size32 = 8;
                        size64 = 8;

                        if (alignment64 == -1)
                        {
                            alignment64 = 8;
                        }
                        break;
                    }

                    case CXTypeKind.CXType_WChar:
                    {
                        if (_config.GenerateUnixTypes)
                        {
                            goto case CXTypeKind.CXType_Int;
                        }
                        else
                        {
                            goto case CXTypeKind.CXType_UShort;
                        }
                    }

                    default:
                    {
                        AddDiagnostic(DiagnosticLevel.Error, $"Unsupported builtin type: '{type.TypeClass}.", cursor);
                        break;
                    }
                }
            }
            else if (type is ElaboratedType elaboratedType)
            {
                GetTypeSize(cursor, elaboratedType.NamedType, ref alignment32, ref alignment64, out size32, out size64);
            }
            else if (type is EnumType enumType)
            {
                GetTypeSize(cursor, enumType.Decl.IntegerType, ref alignment32, ref alignment64, out size32, out size64);
            }
            else if ((type is FunctionType functionType) || (type is PointerType) || (type is ReferenceType))
            {
                size32 = 4;
                size64 = 8;

                if (alignment64 == -1)
                {
                    alignment64 = 8;
                }
            }
            else if (type is RecordType recordType)
            {
                if (alignment32 == -1)
                {
                    alignment32 = recordType.Handle.AlignOf;
                    alignment64 = recordType.Handle.AlignOf;
                }

                long maxFieldAlignment32 = -1;
                long maxFieldAlignment64 = -1;

                long maxFieldSize32 = 0;
                long maxFieldSize64 = 0;

                if (recordType.Decl is CXXRecordDecl cxxRecordDecl)
                {
                    if (HasVtbl(cxxRecordDecl))
                    {
                        size32 += 4;
                        size64 += 8;

                        if (alignment64 == 4)
                        {
                            alignment64 = 8;
                        }

                        maxFieldSize32 = 4;
                        maxFieldSize64 = 8;

                        maxFieldAlignment32 = alignment32;
                        maxFieldAlignment64 = alignment64;
                    }
                    else
                    {
                        foreach (var baseCXXRecordDecl in cxxRecordDecl.Bases)
                        {
                            long fieldSize32;
                            long fieldSize64;

                            long fieldAlignment32 = -1;
                            long fieldAlignment64 = -1;

                            GetTypeSize(baseCXXRecordDecl, baseCXXRecordDecl.Type, ref fieldAlignment32, ref fieldAlignment64, out fieldSize32, out fieldSize64);

                            if ((fieldAlignment32 == -1) || (alignment32 < 4))
                            {
                                fieldAlignment32 = Math.Min(alignment32, fieldSize32);
                            }

                            if ((fieldAlignment64 == -1) || (alignment64 < 4))
                            {
                                fieldAlignment64 = Math.Min(alignment64, fieldSize64);
                            }

                            if ((size32 % fieldAlignment32) != 0)
                            {
                                size32 += fieldAlignment32 - (size32 % fieldAlignment32);
                            }

                            if ((size64 % fieldAlignment64) != 0)
                            {
                                size64 += fieldAlignment64 - (size64 % fieldAlignment64);
                            }

                            size32 += fieldSize32;
                            size64 += fieldSize64;

                            maxFieldAlignment32 = Math.Max(maxFieldAlignment32, fieldAlignment32);
                            maxFieldAlignment64 = Math.Max(maxFieldAlignment64, fieldAlignment64);

                            maxFieldSize32 = Math.Max(maxFieldSize32, fieldSize32);
                            maxFieldSize64 = Math.Max(maxFieldSize64, fieldSize64);
                        }
                    }
                }

                var bitfieldPreviousSize = 0L;
                var bitfieldRemainingBits = 0L;

                foreach (var declaration in recordType.Decl.Decls)
                {
                    long fieldSize32;
                    long fieldSize64;

                    long fieldAlignment32 = -1;
                    long fieldAlignment64 = -1;

                    if (declaration is FieldDecl fieldDecl)
                    {
                        GetTypeSize(fieldDecl, fieldDecl.Type, ref fieldAlignment32, ref fieldAlignment64, out fieldSize32, out fieldSize64);

                        if (fieldDecl.IsBitField)
                        {
                            if ((fieldSize32 != bitfieldPreviousSize) || (fieldDecl.BitWidthValue > bitfieldRemainingBits))
                            {
                                bitfieldRemainingBits = fieldSize32 * 8;
                                bitfieldPreviousSize = fieldSize32;
                                bitfieldRemainingBits -= fieldDecl.BitWidthValue;
                            }
                            else
                            {
                                bitfieldPreviousSize = fieldSize32;
                                bitfieldRemainingBits -= fieldDecl.BitWidthValue;
                                continue;
                            }
                        }
                    }
                    else if ((declaration is RecordDecl nestedRecordDecl) && nestedRecordDecl.IsAnonymousStructOrUnion)
                    {
                        GetTypeSize(nestedRecordDecl, nestedRecordDecl.TypeForDecl, ref fieldAlignment32, ref fieldAlignment64, out fieldSize32, out fieldSize64);
                    }
                    else
                    {
                        continue;
                    }

                    if ((fieldAlignment32 == -1) || (alignment32 < 4))
                    {
                        fieldAlignment32 = Math.Min(alignment32, fieldSize32);
                    }

                    if ((fieldAlignment64 == -1) || (alignment64 < 4))
                    {
                        fieldAlignment64 = Math.Min(alignment64, fieldSize64);
                    }

                    if ((size32 % fieldAlignment32) != 0)
                    {
                        size32 += fieldAlignment32 - (size32 % fieldAlignment32);
                    }

                    if ((size64 % fieldAlignment64) != 0)
                    {
                        size64 += fieldAlignment64 - (size64 % fieldAlignment64);
                    }

                    size32 += fieldSize32;
                    size64 += fieldSize64;

                    maxFieldAlignment32 = Math.Max(maxFieldAlignment32, fieldAlignment32);
                    maxFieldAlignment64 = Math.Max(maxFieldAlignment64, fieldAlignment64);

                    maxFieldSize32 = Math.Max(maxFieldSize32, fieldSize32);
                    maxFieldSize64 = Math.Max(maxFieldSize64, fieldSize64);
                }

                if (alignment32 == 8)
                {
                    alignment32 = Math.Min(alignment32, maxFieldAlignment32);
                }

                if (alignment64 == 4)
                {
                    alignment64 = Math.Max(alignment64, maxFieldAlignment64);
                }

                if (recordType.Decl.IsUnion)
                {
                    size32 = maxFieldSize32;
                    size64 = maxFieldSize64;
                }

                if ((size32 % alignment32) != 0)
                {
                    size32 += alignment32 - (size32 % alignment32);
                }

                if ((size64 % alignment64) != 0)
                {
                    size64 += alignment64 - (size64 % alignment64);
                }
            }
            else if (type is TypedefType typedefType)
            {
                // We check remapped names here so that types that have variable sizes
                // can be treated correctly. Otherwise, they will resolve to a particular
                // platform size, based on whatever parameters were passed into clang.

                var name = GetTypeName(cursor, context: null, type, out _);

                if (!_config.RemappedNames.TryGetValue(name, out string remappedName))
                {
                    remappedName = name;
                }

                if (remappedName.Equals("IntPtr") || remappedName.Equals("nint") || remappedName.Equals("nuint") || remappedName.Equals("UIntPtr") || remappedName.EndsWith("*"))
                {
                    size32 = 4;
                    size64 = 8;

                    if (alignment64 == -1)
                    {
                        alignment64 = 8;
                    }
                }
                else
                {
                    GetTypeSize(cursor, typedefType.Decl.UnderlyingType, ref alignment32, ref alignment64, out size32, out size64);
                }
            }
            else
            {
                AddDiagnostic(DiagnosticLevel.Error, $"Unsupported type: '{type.TypeClass}'.", cursor);
            }
        }

        private bool HasVtbl(CXXRecordDecl cxxRecordDecl)
        {
            var hasDirectVtbl = cxxRecordDecl.Methods.Any((method) => method.IsVirtual);
            var indirectVtblCount = 0;

            foreach (var cxxBaseSpecifier in cxxRecordDecl.Bases)
            {
                var baseCxxRecordDecl = GetRecordDeclForBaseSpecifier(cxxBaseSpecifier);

                if (HasVtbl(baseCxxRecordDecl))
                {
                    indirectVtblCount++;
                }
            }

            if (indirectVtblCount > 1)
            {
                AddDiagnostic(DiagnosticLevel.Warning, "Unsupported cxx record declaration: 'multiple virtual bases'. Generated bindings may be incomplete.", cxxRecordDecl);
            }

            return hasDirectVtbl || (indirectVtblCount != 0);
        }

        private bool IsEnumOperator(FunctionDecl functionDecl, string name)
        {
            if (name.StartsWith("operator") && ((functionDecl.Parameters.Count == 1) || (functionDecl.Parameters.Count == 2)))
            {
                var parmVarDecl1 = functionDecl.Parameters[0];
                var parmVarDecl1Type = parmVarDecl1.Type.CanonicalType;

                if (parmVarDecl1Type is PointerType pointerType1)
                {
                    parmVarDecl1Type = pointerType1.PointeeType.CanonicalType;
                }
                else if (parmVarDecl1Type is ReferenceType referenceType1)
                {
                    parmVarDecl1Type = referenceType1.PointeeType.CanonicalType;
                }

                if (functionDecl.Parameters.Count == 1)
                {
                    return (parmVarDecl1Type.Kind == CXTypeKind.CXType_Enum);
                }

                var parmVarDecl2 = functionDecl.Parameters[1];
                var parmVarDecl2Type = parmVarDecl2.Type.CanonicalType;

                if (parmVarDecl2Type is PointerType pointerType2)
                {
                    parmVarDecl2Type = pointerType2.PointeeType.CanonicalType;
                }
                else if (parmVarDecl2Type is ReferenceType referenceType2)
                {
                    parmVarDecl2Type = referenceType2.PointeeType.CanonicalType;
                }

                if ((parmVarDecl1Type == parmVarDecl2Type) && (parmVarDecl2Type.Kind == CXTypeKind.CXType_Enum))
                {
                    return true;
                }
            }
            return false;
        }

        private bool IsExcluded(Cursor cursor) => IsExcluded(cursor, out _);

        private bool IsExcluded(Cursor cursor, out bool isExcludedByConflictingDefinition)
        {
            isExcludedByConflictingDefinition = false;

            if (IsAlwaysIncluded(cursor))
            {
                return false;
            }

            if (_config.ExcludeFunctionsWithBody &&
                cursor is FunctionDecl functionDecl &&
                cursor.CursorKind == CXCursorKind.CXCursor_FunctionDecl &&
                functionDecl.HasBody)
            {
                return true;
            }

            return IsExcludedByFile(cursor) || IsExcludedByName(cursor, out isExcludedByConflictingDefinition);

            bool IsAlwaysIncluded(Cursor cursor)
            {
                return (cursor is TranslationUnitDecl) || (cursor is LinkageSpecDecl) || ((cursor is VarDecl varDecl) && varDecl.Name.StartsWith("ClangSharpMacro_"));
            }

            bool IsExcludedByFile(Cursor cursor)
            {
                if (_outputBuilder != null)
                {
                    // We don't want to exclude  by fileif we already have an active output builder as we
                    // are likely processing members of an already included type but those members may
                    // indirectly exist or be defined in a non-traversed file.
                    return false;
                }

                var declLocation = cursor.Location;
                declLocation.GetFileLocation(out CXFile file, out uint line, out uint column, out _);

                if (IsIncludedFileOrLocation(cursor, file, declLocation))
                {
                    return false;
                }

                // It is not uncommon for some declarations to be done using macros, which are themselves
                // defined in an imported header file. We want to also check if the expansion location is
                // in the main file to catch these cases and ensure we still generate bindings for them.

                declLocation.GetExpansionLocation(out CXFile expansionFile, out uint expansionLine, out uint expansionColumn, out _);

                if ((expansionFile == file) && (expansionLine == line) && (expansionColumn == column) && (_config.TraversalNames.Length != 0))
                {
                    // clang_getLocation is a very expensive call, so exit early if the expansion file is the same
                    // However, if we are not explicitly specifying traversal names, its possible the expansion location
                    // is the same, but IsMainFile is now marked as true, in which case we can't exit early.

                    return true;
                }

                var expansionLocation = cursor.TranslationUnit.Handle.GetLocation(expansionFile, expansionLine, expansionColumn);

                if (IsIncludedFileOrLocation(cursor, file, expansionLocation))
                {
                    return false;
                }

                return true;
            }

            bool IsExcludedByName(Cursor cursor, out bool isExcludedByConflictingDefinition)
            {
                var isExcludedByConfigOption = false;
                isExcludedByConflictingDefinition = false;

                string qualifiedName;
                string name;
                string kind;

                if (cursor is NamedDecl namedDecl)
                {
                    // We get the non-remapped name for the purpose of exclusion checks to ensure that users
                    // can remove no-definition declarations in favor of remapped anonymous declarations.

                    qualifiedName = GetCursorQualifiedName(namedDecl);
                    name = GetCursorName(namedDecl);
                    kind = $"{namedDecl.DeclKindName} declaration";

                    if ((namedDecl is TagDecl tagDecl) && (tagDecl.Definition != tagDecl) && (tagDecl.Definition != null))
                    {
                        // We don't want to generate bindings for anything
                        // that is not itself a definition and that has a
                        // definition that can be resolved. This ensures we
                        // still generate bindings for things which are used
                        // as opaque handles, but which aren't ever defined.

                        if (_config.LogExclusions)
                        {
                            AddDiagnostic(DiagnosticLevel.Info, $"Excluded {kind} '{qualifiedName}' by as it is not a definition.");
                        }
                        return true;
                    }
                }
                else if (cursor is MacroDefinitionRecord macroDefinitionRecord)
                {
                    qualifiedName = macroDefinitionRecord.Name;
                    name = macroDefinitionRecord.Name;
                    kind = macroDefinitionRecord.CursorKindSpelling;
                }
                else
                {
                    return false;
                }
                if (cursor is RecordDecl recordDecl)
                {
                    if (_config.ExcludeEmptyRecords && IsEmptyRecord(recordDecl))
                    {
                        isExcludedByConfigOption = true;
                    }
                }
                else if (cursor is FunctionDecl functionDecl)
                {
                    if (_config.ExcludeComProxies && IsComProxy(functionDecl, name))
                    {
                        isExcludedByConfigOption = true;
                    }
                    else if (_config.ExcludeEnumOperators && IsEnumOperator(functionDecl, name))
                    {
                        isExcludedByConfigOption = true;
                    }
                    else if ((functionDecl is CXXMethodDecl cxxMethodDecl) && IsConflictingMethodDecl(cxxMethodDecl, cxxMethodDecl.Parent))
                    {
                        isExcludedByConflictingDefinition = true;
                    }
                }

                if (_config.ExcludedNames.Contains(qualifiedName))
                {
                    if (_config.LogExclusions)
                    {
                        var message = $"Excluded {kind} '{qualifiedName}' by exact match";

                        if (isExcludedByConfigOption)
                        {
                            message += "; Exclusion is unnecessary due to a config option";
                        }
                        else if (isExcludedByConflictingDefinition)
                        {
                            message += "; Exclusion is unnecessary due to a conflicting definition";
                        }

                        AddDiagnostic(DiagnosticLevel.Info, message);
                    }
                    return true;
                }

                if (_config.ExcludedNames.Contains(name))
                {
                    if (_config.LogExclusions)
                    {
                        var message = $"Excluded {kind} '{qualifiedName}' by partial match against {name}";

                        if (isExcludedByConfigOption)
                        {
                            message += "; Exclusion is unnecessary due to a config option";
                        }
                        else if (isExcludedByConflictingDefinition)
                        {
                            message += "; Exclusion is unnecessary due to a conflicting definition";
                        }

                        AddDiagnostic(DiagnosticLevel.Info, message);
                    }
                    return true;
                }

                if (isExcludedByConfigOption)
                {
                    if (_config.LogExclusions)
                    {
                        AddDiagnostic(DiagnosticLevel.Info, $"Excluded {kind} '{qualifiedName}' by config option");
                    }
                    return true;
                }

                if (isExcludedByConflictingDefinition)
                {
                    if (_config.LogExclusions)
                    {
                        AddDiagnostic(DiagnosticLevel.Info, $"Excluded {kind} '{qualifiedName}' by conflicting definition");
                    }
                    return true;
                }

                return false;
            }

            bool IsIncludedFileOrLocation(Cursor cursor, CXFile file, CXSourceLocation location)
            {
                // Normalize paths to be '/' for comparison
                var fileName = file.Name.ToString().Replace('\\', '/');

                if (_visitedFiles.Add(fileName) && _config.LogVisitedFiles)
                {
                    AddDiagnostic(DiagnosticLevel.Info, $"Visiting {fileName}");
                }

                // Use case insensitive comparison on Windows
                var equalityComparer = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal;
                if (_config.TraversalNames.Contains(fileName, equalityComparer))
                {
                    return true;
                }
                else if ((_config.TraversalNames.Length == 0) && location.IsFromMainFile)
                {
                    return true;
                }

                return false;
            }

            bool IsComProxy(FunctionDecl functionDecl, string name)
            {
                var parmVarDecl = null as ParmVarDecl;

                if (name.EndsWith("_UserFree") || name.EndsWith("_UserFree64") ||
                    name.EndsWith("_UserMarshal") || name.EndsWith("_UserMarshal64") ||
                    name.EndsWith("_UserSize") || name.EndsWith("_UserSize64") ||
                    name.EndsWith("_UserUnmarshal") || name.EndsWith("_UserUnmarshal64"))
                {
                    parmVarDecl = functionDecl.Parameters.LastOrDefault();
                }
                else if (name.EndsWith("_Proxy") || name.EndsWith("_Stub"))
                {
                    parmVarDecl = functionDecl.Parameters.FirstOrDefault();
                }

                if ((parmVarDecl != null) && (parmVarDecl.Type is PointerType pointerType))
                {
                    var typeName = GetTypeName(parmVarDecl, context: null, pointerType.PointeeType, out string nativeTypeName);
                    return name.StartsWith($"{nativeTypeName}_") || name.StartsWith($"{typeName}_") || (typeName == "IRpcStubBuffer");
                }
                return false;
            }

            bool IsConflictingMethodDecl(CXXMethodDecl cxxMethodDecl, CXXRecordDecl cxxRecordDecl)
            {
                var cxxMethodName = GetRemappedCursorName(cxxMethodDecl);
                var cxxMethodDeclIndex = -1;

                for (int i = 0; i < cxxRecordDecl.Methods.Count; i++)
                {
                    var methodDecl = cxxRecordDecl.Methods[i];
                    var methodName = GetRemappedCursorName(methodDecl);

                    if (cxxMethodName != methodName)
                    {
                        continue;
                    }

                    if (cxxMethodDecl == methodDecl)
                    {
                        cxxMethodDeclIndex = i;
                        continue;
                    }

                    if (cxxMethodDecl.Parameters.Count != methodDecl.Parameters.Count)
                    {
                        continue;
                    }

                    var allMatch = true;

                    for (int n = 0; n < cxxMethodDecl.Parameters.Count; n++)
                    {
                        var baseParameterType = cxxMethodDecl.Parameters[n].Type.CanonicalType;
                        var thisParameterType = methodDecl.Parameters[n].Type.CanonicalType;

                        if (baseParameterType == thisParameterType)
                        {
                            continue;
                        }

                        if ((baseParameterType is PointerType basePointerType) &&
                            (thisParameterType is ReferenceType thisReferenceType) &&
                            (basePointerType.PointeeType.CanonicalType == thisReferenceType.PointeeType.CanonicalType))
                        {
                            continue;
                        }

                        if ((baseParameterType is ReferenceType baseReferenceType) &&
                            (thisParameterType is PointerType thisPointerType) &&
                            (baseReferenceType.PointeeType.CanonicalType == thisPointerType.PointeeType.CanonicalType))
                        {
                            continue;
                        }

                        allMatch = false;
                        break;
                    }

                    if (allMatch)
                    {
                        // An index of -1 means we found a conflict before encountering
                        // ourselves. Since we generally want to prefer the first declaration,
                        // we want to classify ourselves as the conflicting instance.
                        return (cxxMethodDeclIndex == -1);
                    }
                }

                foreach (var @base in cxxRecordDecl.Bases)
                {
                    CXXRecordDecl baseRecordDecl;

                    if (@base.Referenced is CXXRecordDecl)
                    {
                        baseRecordDecl = (CXXRecordDecl)@base.Referenced;
                    }
                    else if (@base.Referenced is TypedefDecl typedefDecl)
                    {
                        baseRecordDecl = (CXXRecordDecl)((RecordType)typedefDecl.TypeForDecl.CanonicalType).Decl;
                    }
                    else
                    {
                        continue;
                    }

                    if (IsConflictingMethodDecl(cxxMethodDecl, baseRecordDecl))
                    {
                        return true;
                    }
                }

                return false;
            }

            bool IsEmptyRecord(RecordDecl recordDecl)
            {
                if (recordDecl.Fields.Count != 0)
                {
                    if (!GetCursorName(recordDecl).EndsWith("__") || (recordDecl.Fields.Count != 1))
                    {
                        return false;
                    }

                    var field = recordDecl.Fields.First();

                    if ((GetCursorName(field) != "unused") || (field.Type.CanonicalType.Kind != CXTypeKind.CXType_Int))
                    {
                        return false;
                    }
                }

                foreach (var decl in recordDecl.Decls)
                {
                    if ((decl is RecordDecl nestedRecordDecl) && nestedRecordDecl.IsAnonymousStructOrUnion && !IsEmptyRecord(nestedRecordDecl))
                    {
                        return false;
                    }

                    if ((decl is CXXMethodDecl cxxMethodDecl) && cxxMethodDecl.IsVirtual)
                    {
                        return false;
                    }
                }

                if (recordDecl is CXXRecordDecl cxxRecordDecl)
                {
                    foreach (var cxxBaseSpecifier in cxxRecordDecl.Bases)
                    {
                        var baseCxxRecordDecl = GetRecordDeclForBaseSpecifier(cxxBaseSpecifier);

                        if (!IsEmptyRecord(baseCxxRecordDecl))
                        {
                            return false;
                        }
                    }
                }

                return !TryGetUuid(recordDecl, out _);
            }
        }

        private bool IsFixedSize(Cursor cursor, Type type)
        {
            if (type is ArrayType)
            {
                return false;
            }
            else if (type is AttributedType attributedType)
            {
                return IsFixedSize(cursor, attributedType.ModifiedType);
            }
            else if (type is BuiltinType)
            {
                return true;
            }
            else if (type is ElaboratedType elaboratedType)
            {
                return IsFixedSize(cursor, elaboratedType.NamedType);
            }
            else if (type is EnumType enumType)
            {
                return IsFixedSize(cursor, enumType.Decl.IntegerType);
            }
            else if (type is FunctionType)
            {
                return false;
            }
            else if (type is PointerType)
            {
                return false;
            }
            else if (type is RecordType recordType)
            {
                var recordDecl = recordType.Decl;

                return recordDecl.Fields.All((fieldDecl) => IsFixedSize(fieldDecl, fieldDecl.Type))
                    && (!(recordDecl is CXXRecordDecl cxxRecordDecl) || cxxRecordDecl.Methods.All((cxxMethodDecl) => !cxxMethodDecl.IsVirtual));
            }
            else if (type is ReferenceType)
            {
                return false;
            }
            else if (type is TypedefType typedefType)
            {
                var name = GetTypeName(cursor, context: null, type, out _);

                if (!_config.RemappedNames.TryGetValue(name, out string remappedName))
                {
                    remappedName = name;
                }

                return (remappedName != "IntPtr")
                    && (remappedName != "nint")
                    && (remappedName != "nuint")
                    && (remappedName != "UIntPtr")
                    && IsFixedSize(cursor, typedefType.Decl.UnderlyingType);
            }
            else
            {
                AddDiagnostic(DiagnosticLevel.Warning, $"Unsupported type: '{type.TypeClass}'. Assuming unfixed size.", cursor);
                return false;
            }
        }

        private bool IsPrevContextDecl<T>(out T value)
            where T : Decl
        {
            var previousContext = _context.Last.Previous;

            while (!(previousContext.Value is Decl))
            {
                previousContext = previousContext.Previous;
            }

            if (previousContext.Value is T)
            {
                value = (T)previousContext.Value;
                return true;
            }
            else
            {
                value = null;
                return false;
            }
        }

        private bool IsPrevContextStmt<T>(out T value)
            where T : Stmt
        {
            var previousContext = _context.Last.Previous;

            while ((previousContext.Value is ParenExpr) || (previousContext.Value is ImplicitCastExpr))
            {
                previousContext = previousContext.Previous;
            }

            if (previousContext.Value is T)
            {
                value = (T)previousContext.Value;
                return true;
            }
            else
            {
                value = null;
                return false;
            }
        }

        private bool IsStmtAsWritten<T>(Cursor cursor, out T value, bool removeParens = false)
            where T : Stmt
        {
            if (cursor is Expr expr)
            {
                cursor = GetExprAsWritten(expr, removeParens);
            }

            if (cursor is T)
            {
                value = (T)cursor;
                return true;
            }
            else
            {
                value = null;
                return false;
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

        private bool IsUnchecked(string targetTypeName, Stmt stmt)
        {
            switch (stmt.StmtClass)
            {
                // case CX_StmtClass.CX_StmtClass_BinaryConditionalOperator:

                case CX_StmtClass.CX_StmtClass_ConditionalOperator:
                {
                    var conditionalOperator = (ConditionalOperator)stmt;
                    return IsUnchecked(targetTypeName, conditionalOperator.LHS)
                        || IsUnchecked(targetTypeName, conditionalOperator.RHS)
                        || IsUnchecked(targetTypeName, conditionalOperator.Handle.Evaluate);
                }

                // case CX_StmtClass.CX_StmtClass_AddrLabelExpr:
                // case CX_StmtClass.CX_StmtClass_ArrayInitIndexExpr:
                // case CX_StmtClass.CX_StmtClass_ArrayInitLoopExpr:
                // case CX_StmtClass.CX_StmtClass_ArraySubscriptExpr:
                // case CX_StmtClass.CX_StmtClass_ArrayTypeTraitExpr:
                // case CX_StmtClass.CX_StmtClass_AsTypeExpr:
                // case CX_StmtClass.CX_StmtClass_AtomicExpr:

                case CX_StmtClass.CX_StmtClass_BinaryOperator:
                {
                    var binaryOperator = (BinaryOperator)stmt;
                    return IsUnchecked(targetTypeName, binaryOperator.LHS)
                        || IsUnchecked(targetTypeName, binaryOperator.RHS)
                        || IsUnchecked(targetTypeName, binaryOperator.Handle.Evaluate)
                        || IsOverflow(binaryOperator);
                }

                // case CX_StmtClass.CX_StmtClass_CompoundAssignOperator:
                // case CX_StmtClass.CX_StmtClass_BlockExpr:
                // case CX_StmtClass.CX_StmtClass_CXXBindTemporaryExpr:

                case CX_StmtClass.CX_StmtClass_CXXBoolLiteralExpr:
                {
                    return false;
                }

                case CX_StmtClass.CX_StmtClass_CXXConstructExpr:
                {
                    return false;
                }

                // case CX_StmtClass.CX_StmtClass_CXXTemporaryObjectExpr:
                // case CX_StmtClass.CX_StmtClass_CXXDefaultArgExpr:
                // case CX_StmtClass.CX_StmtClass_CXXDefaultInitExpr:
                // case CX_StmtClass.CX_StmtClass_CXXDeleteExpr:
                // case CX_StmtClass.CX_StmtClass_CXXDependentScopeMemberExpr:
                // case CX_StmtClass.CX_StmtClass_CXXFoldExpr:
                // case CX_StmtClass.CX_StmtClass_CXXInheritedCtorInitExpr:
                // case CX_StmtClass.CX_StmtClass_CXXNewExpr:
                // case CX_StmtClass.CX_StmtClass_CXXNoexceptExpr:

                case CX_StmtClass.CX_StmtClass_CXXNullPtrLiteralExpr:
                {
                    return false;
                }

                // case CX_StmtClass.CX_StmtClass_CXXPseudoDestructorExpr:
                // case CX_StmtClass.CX_StmtClass_CXXRewrittenBinaryOperator:
                // case CX_StmtClass.CX_StmtClass_CXXScalarValueInitExpr:
                // case CX_StmtClass.CX_StmtClass_CXXStdInitializerListExpr:
                // case CX_StmtClass.CX_StmtClass_CXXThisExpr:
                // case CX_StmtClass.CX_StmtClass_CXXThrowExpr:
                // case CX_StmtClass.CX_StmtClass_CXXTypeidExpr:
                // case CX_StmtClass.CX_StmtClass_CXXUnresolvedConstructExpr:
                // case CX_StmtClass.CX_StmtClass_CXXUuidofExpr:

                case CX_StmtClass.CX_StmtClass_CallExpr:
                {
                    return false;
                }

                // case CX_StmtClass.CX_StmtClass_CUDAKernelCallExpr:

                case CX_StmtClass.CX_StmtClass_CXXMemberCallExpr:
                {
                    return false;
                }

                case CX_StmtClass.CX_StmtClass_CXXOperatorCallExpr:
                {
                    return false;
                }

                // case CX_StmtClass.CX_StmtClass_UserDefinedLiteral:
                // case CX_StmtClass.CX_StmtClass_BuiltinBitCastExpr:

                case CX_StmtClass.CX_StmtClass_CStyleCastExpr:
                case CX_StmtClass.CX_StmtClass_CXXStaticCastExpr:
                {
                    var explicitCastExpr = (ExplicitCastExpr)stmt;
                    var explicitCastExprTypeName = GetRemappedTypeName(explicitCastExpr, context: null, explicitCastExpr.Type, out _);

                    return IsUnchecked(targetTypeName, explicitCastExpr.SubExprAsWritten)
                        || IsUnchecked(targetTypeName, explicitCastExpr.Handle.Evaluate)
                        || (IsUnsigned(targetTypeName) != IsUnsigned(explicitCastExprTypeName));
                }

                // case CX_StmtClass.CX_StmtClass_CXXFunctionalCastExpr:
                // case CX_StmtClass.CX_StmtClass_CXXConstCastExpr:
                // case CX_StmtClass.CX_StmtClass_CXXDynamicCastExpr:
                // case CX_StmtClass.CX_StmtClass_CXXReinterpretCastExpr:

                // case CX_StmtClass.CX_StmtClass_ObjCBridgedCastExpr:

                case CX_StmtClass.CX_StmtClass_ImplicitCastExpr:
                {
                    var implicitCastExpr = (ImplicitCastExpr)stmt;
                    var implicitCastExprTypeName = GetRemappedTypeName(implicitCastExpr, context: null, implicitCastExpr.Type, out _);

                    return IsUnchecked(targetTypeName, implicitCastExpr.SubExprAsWritten)
                        || IsUnchecked(targetTypeName, implicitCastExpr.Handle.Evaluate);
                }

                case CX_StmtClass.CX_StmtClass_CharacterLiteral:
                {
                    return false;
                }

                // case CX_StmtClass.CX_StmtClass_ChooseExpr:
                // case CX_StmtClass.CX_StmtClass_CompoundLiteralExpr:
                // case CX_StmtClass.CX_StmtClass_ConceptSpecializationExpr:
                // case CX_StmtClass.CX_StmtClass_ConvertVectorExpr:
                // case CX_StmtClass.CX_StmtClass_CoawaitExpr:
                // case CX_StmtClass.CX_StmtClass_CoyieldExpr:

                case CX_StmtClass.CX_StmtClass_DeclRefExpr:
                {
                    var declRefExpr = (DeclRefExpr)stmt;
                    return (declRefExpr.Decl is VarDecl varDecl) && varDecl.HasInit && IsUnchecked(targetTypeName, varDecl.Init);
                }

                // case CX_StmtClass.CX_StmtClass_DependentCoawaitExpr:
                // case CX_StmtClass.CX_StmtClass_DependentScopeDeclRefExpr:
                // case CX_StmtClass.CX_StmtClass_DesignatedInitExpr:
                // case CX_StmtClass.CX_StmtClass_DesignatedInitUpdateExpr:
                // case CX_StmtClass.CX_StmtClass_ExpressionTraitExpr:
                // case CX_StmtClass.CX_StmtClass_ExtVectorElementExpr:
                // case CX_StmtClass.CX_StmtClass_FixedPointLiteral:

                case CX_StmtClass.CX_StmtClass_FloatingLiteral:
                {
                    return false;
                }

                // case CX_StmtClass.CX_StmtClass_ConstantExpr:
                // case CX_StmtClass.CX_StmtClass_ExprWithCleanups:
                // case CX_StmtClass.CX_StmtClass_FunctionParmPackExpr:
                // case CX_StmtClass.CX_StmtClass_GNUNullExpr:
                // case CX_StmtClass.CX_StmtClass_GenericSelectionExpr:
                // case CX_StmtClass.CX_StmtClass_ImaginaryLiteral:
                // case CX_StmtClass.CX_StmtClass_ImplicitValueInitExpr:

                case CX_StmtClass.CX_StmtClass_InitListExpr:
                {
                    return false;
                }

                case CX_StmtClass.CX_StmtClass_IntegerLiteral:
                {
                    var integerLiteral = (IntegerLiteral)stmt;
                    var signedValue = integerLiteral.Value;
                    return IsUnchecked(targetTypeName, signedValue, integerLiteral.IsNegative, isHex: integerLiteral.ValueString.StartsWith("0x"));
                }

                // case CX_StmtClass.CX_StmtClass_LambdaExpr:
                // case CX_StmtClass.CX_StmtClass_MSPropertyRefExpr:
                // case CX_StmtClass.CX_StmtClass_MSPropertySubscriptExpr:
                // case CX_StmtClass.CX_StmtClass_MaterializeTemporaryExpr:

                case CX_StmtClass.CX_StmtClass_MemberExpr:
                {
                    return false;
                }

                // case CX_StmtClass.CX_StmtClass_NoInitExpr:
                // case CX_StmtClass.CX_StmtClass_OMPArraySectionExpr:
                // case CX_StmtClass.CX_StmtClass_ObjCArrayLiteral:
                // case CX_StmtClass.CX_StmtClass_ObjCAvailabilityCheckExpr:
                // case CX_StmtClass.CX_StmtClass_ObjCBoolLiteralExpr:
                // case CX_StmtClass.CX_StmtClass_ObjCBoxedExpr:
                // case CX_StmtClass.CX_StmtClass_ObjCDictionaryLiteral:
                // case CX_StmtClass.CX_StmtClass_ObjCEncodeExpr:
                // case CX_StmtClass.CX_StmtClass_ObjCIndirectCopyRestoreExpr:
                // case CX_StmtClass.CX_StmtClass_ObjCIsaExpr:
                // case CX_StmtClass.CX_StmtClass_ObjCIvarRefExpr:
                // case CX_StmtClass.CX_StmtClass_ObjCMessageExpr:
                // case CX_StmtClass.CX_StmtClass_ObjCPropertyRefExpr:
                // case CX_StmtClass.CX_StmtClass_ObjCProtocolExpr:
                // case CX_StmtClass.CX_StmtClass_ObjCSelectorExpr:
                // case CX_StmtClass.CX_StmtClass_ObjCStringLiteral:
                // case CX_StmtClass.CX_StmtClass_ObjCSubscriptRefExpr:
                // case CX_StmtClass.CX_StmtClass_OffsetOfExpr:
                // case CX_StmtClass.CX_StmtClass_OpaqueValueExpr:
                // case CX_StmtClass.CX_StmtClass_UnresolvedLookupExpr:
                // case CX_StmtClass.CX_StmtClass_UnresolvedMemberExpr:
                // case CX_StmtClass.CX_StmtClass_PackExpansionExpr:

                case CX_StmtClass.CX_StmtClass_ParenExpr:
                {
                    var parenExpr = (ParenExpr)stmt;
                    return IsUnchecked(targetTypeName, parenExpr.SubExpr)
                        || IsUnchecked(targetTypeName, parenExpr.Handle.Evaluate);
                }

                // case CX_StmtClass.CX_StmtClass_ParenListExpr:
                // case CX_StmtClass.CX_StmtClass_PredefinedExpr:
                // case CX_StmtClass.CX_StmtClass_PseudoObjectExpr:
                // case CX_StmtClass.CX_StmtClass_RequiresExpr:
                // case CX_StmtClass.CX_StmtClass_ShuffleVectorExpr:
                // case CX_StmtClass.CX_StmtClass_SizeOfPackExpr:
                // case CX_StmtClass.CX_StmtClass_SourceLocExpr:
                // case CX_StmtClass.CX_StmtClass_StmtExpr:

                case CX_StmtClass.CX_StmtClass_StringLiteral:
                {
                    return false;
                }

                // case CX_StmtClass.CX_StmtClass_SubstNonTypeTemplateParmExpr:
                // case CX_StmtClass.CX_StmtClass_SubstNonTypeTemplateParmPackExpr:
                // case CX_StmtClass.CX_StmtClass_TypeTraitExpr:
                // case CX_StmtClass.CX_StmtClass_TypoExpr:

                case CX_StmtClass.CX_StmtClass_UnaryExprOrTypeTraitExpr:
                {
                    var unaryExprOrTypeTraitExpr = (UnaryExprOrTypeTraitExpr)stmt;

                    var argumentType = unaryExprOrTypeTraitExpr.TypeOfArgument;

                    long size32;
                    long size64;

                    long alignment32 = -1;
                    long alignment64 = -1;

                    GetTypeSize(unaryExprOrTypeTraitExpr, argumentType, ref alignment32, ref alignment64, out size32, out size64);

                    switch (unaryExprOrTypeTraitExpr.Kind)
                    {
                        case CX_UnaryExprOrTypeTrait.CX_UETT_SizeOf:
                        {
                            switch (targetTypeName)
                            {
                                case "byte":
                                case "Byte":
                                case "ushort":
                                case "UInt16":
                                case "uint":
                                case "UInt32":
                                case "nuint":
                                case "sbyte":
                                case "SByte":
                                case "short":
                                case "Int16":
                                {
                                    return (size32 != size64) || !IsPrevContextDecl<VarDecl>(out _);
                                }

                                case "ulong":
                                case "UInt64":
                                case "int":
                                case "Int32":
                                case "nint":
                                case "long":
                                case "Int64":
                                {
                                    return false;
                                }

                                default:
                                {
                                    return false;
                                }
                            }
                        }

                        default:
                        {
                            return false;
                        }
                    }
                }

                case CX_StmtClass.CX_StmtClass_UnaryOperator:
                {
                    var unaryOperator = (UnaryOperator)stmt;
                    return IsUnchecked(targetTypeName, unaryOperator.SubExpr)
                        || IsUnchecked(targetTypeName, unaryOperator.Handle.Evaluate)
                        || ((unaryOperator.Opcode == CX_UnaryOperatorKind.CX_UO_Minus) && IsUnsigned(targetTypeName));
                }

                // case CX_StmtClass.CX_StmtClass_VAArgExpr:

                default:
                {
                    AddDiagnostic(DiagnosticLevel.Warning, $"Unsupported statement class: '{stmt.StmtClassName}'. Generated bindings may not be unchecked.", stmt);
                    return false;
                }
            }

            bool IsOverflow(BinaryOperator binaryOperator)
            {
                var lhs = binaryOperator.LHS;
                var rhs = binaryOperator.RHS;

                if (!IsStmtAsWritten<IntegerLiteral>(lhs, out var lhsIntegerLiteral, removeParens: true) || !IsStmtAsWritten<IntegerLiteral>(rhs, out var rhsIntegerLiteral, removeParens: true))
                {
                    return false;
                }

                var targetTypeName = GetRemappedTypeName(binaryOperator, context: null, binaryOperator.Type, out _);
                var isUnsigned = IsUnsigned(targetTypeName);

                switch (binaryOperator.Opcode)
                {
                    case CX_BinaryOperatorKind.CX_BO_Add:
                    {
                        if (isUnsigned)
                        {
                            return unchecked((ulong)lhsIntegerLiteral.Value + (ulong)rhsIntegerLiteral.Value < (ulong)lhsIntegerLiteral.Value);
                        }
                        else
                        {
                            return unchecked(lhsIntegerLiteral.Value + rhsIntegerLiteral.Value < lhsIntegerLiteral.Value);
                        }
                    }

                    case CX_BinaryOperatorKind.CX_BO_Sub:
                    {
                        if (isUnsigned)
                        {
                            return unchecked((ulong)lhsIntegerLiteral.Value - (ulong)rhsIntegerLiteral.Value > (ulong)lhsIntegerLiteral.Value);
                        }
                        else
                        {
                            return unchecked(lhsIntegerLiteral.Value - rhsIntegerLiteral.Value > lhsIntegerLiteral.Value);
                        }
                    }

                    default:
                    {
                        return false;
                    }
                }
            }
        }

        private bool IsUnchecked(string typeName, CXEvalResult evalResult)
        {
            if (evalResult.Kind != CXEvalResultKind.CXEval_Int)
            {
                return false;
            }

            var signedValue = evalResult.AsLongLong;
            return IsUnchecked(typeName, signedValue, (signedValue < 0), isHex: false);
        }

        private bool IsUnchecked(string typeName, long signedValue, bool isNegative, bool isHex)
        {
            switch (typeName)
            {
                case "byte":
                case "Byte":
                {
                    var unsignedValue = unchecked((uint)signedValue);
                    return (unsignedValue < byte.MinValue) || (byte.MaxValue < unsignedValue);
                }

                case "ushort":
                case "UInt16":
                {
                    var unsignedValue = unchecked((uint)signedValue);
                    return (unsignedValue < ushort.MinValue) || (ushort.MaxValue < unsignedValue);
                }

                case "uint":
                case "UInt32":
                case "nuint":
                {
                    var unsignedValue = unchecked((uint)signedValue);
                    return (unsignedValue < uint.MinValue) || (uint.MaxValue < unsignedValue);
                }

                case "ulong":
                case "UInt64":
                {
                    var unsignedValue = unchecked((ulong)signedValue);
                    return (unsignedValue < ulong.MinValue) || (ulong.MaxValue < unsignedValue);
                }

                case "sbyte":
                case "SByte":
                {
                    return (signedValue < sbyte.MinValue) || (sbyte.MaxValue < signedValue) || (isNegative && isHex);
                }

                case "short":
                case "Int16":
                {
                    return (signedValue < short.MinValue) || (short.MaxValue < signedValue) || (isNegative && isHex);
                }

                case "int":
                case "Int32":
                case "nint":
                {
                    return (signedValue < int.MinValue) || (int.MaxValue < signedValue) || (isNegative && isHex);
                }

                case "long":
                case "Int64":
                {
                    return (signedValue < long.MinValue) || (long.MaxValue < signedValue) || (isNegative && isHex);
                }

                default:
                {
                    return false;
                }
            }
        }

        private bool IsUnsafe(FieldDecl fieldDecl)
        {
            var type = fieldDecl.Type;

            if (type.CanonicalType is ConstantArrayType)
            {
                var name = GetTypeName(fieldDecl, context: null, type, out _);

                if (!_config.RemappedNames.TryGetValue(name, out string remappedName))
                {
                    remappedName = name;
                }

                return IsSupportedFixedSizedBufferType(remappedName);
            }

            return IsUnsafe(fieldDecl, type);
        }

        private bool IsUnsafe(FunctionDecl functionDecl)
        {
            if (IsUnsafe(functionDecl, functionDecl.ReturnType))
            {
                return true;
            }

            foreach (var parmVarDecl in functionDecl.Parameters)
            {
                if (IsUnsafe(parmVarDecl))
                {
                    return true;
                }
            }

            return false;
        }

        private bool IsUnsafe(ParmVarDecl parmVarDecl)
        {
            var type = parmVarDecl.Type;
            return IsUnsafe(parmVarDecl, type);
        }

        private bool IsUnsafe(RecordDecl recordDecl)
        {
            foreach (var decl in recordDecl.Decls)
            {
                if ((decl is FieldDecl fieldDecl) && IsUnsafe(fieldDecl))
                {
                    return true;
                }
                else if ((decl is RecordDecl nestedRecordDecl) && nestedRecordDecl.IsAnonymousStructOrUnion && IsUnsafe(nestedRecordDecl))
                {
                    return true;
                }
            }
            return (recordDecl is CXXRecordDecl cxxRecordDecl) && HasVtbl(cxxRecordDecl);
        }

        private bool IsUnsafe(TypedefDecl typedefDecl, FunctionProtoType functionProtoType)
        {
            var returnType = functionProtoType.ReturnType;

            if (IsUnsafe(typedefDecl, returnType))
            {
                return true;
            }

            foreach (var paramType in functionProtoType.ParamTypes)
            {
                if (IsUnsafe(typedefDecl, paramType))
                {
                    return true;
                }
            }

            return false;
        }

        private bool IsUnsafe(NamedDecl namedDecl, Type type)
        {
            var name = GetTypeName(namedDecl, context: null, type, out _);

            if (!_config.RemappedNames.TryGetValue(name, out string remappedName))
            {
                remappedName = name;
            }

            return remappedName.Contains('*');
        }

        private static bool IsUnsigned(string typeName)
        {
            switch (typeName)
            {
                case "byte":
                case "Byte":
                case "UInt16":
                case "nuint":
                case "uint":
                case "UInt32":
                case "ulong":
                case "UInt64":
                case "ushort":
                case var _ when typeName.EndsWith("*"):
                {
                    return true;
                }

                case "Int16":
                case "int":
                case "Int32":
                case "long":
                case "Int64":
                case "nint":
                case "sbyte":
                case "SByte":
                case "short":
                {
                    return false;
                }

                default:
                {
                    return false;
                }
            }
        }

        private bool NeedsReturnFixup(CXXMethodDecl cxxMethodDecl)
        {
            Debug.Assert(cxxMethodDecl != null);

            var needsReturnFixup = false;

            if (cxxMethodDecl.IsVirtual)
            {
                var canonicalReturnType = cxxMethodDecl.ReturnType.CanonicalType;

                switch (canonicalReturnType.TypeClass)
                {
                    case CX_TypeClass.CX_TypeClass_Builtin:
                    case CX_TypeClass.CX_TypeClass_Enum:
                    case CX_TypeClass.CX_TypeClass_Pointer:
                    {
                        break;
                    }

                    case CX_TypeClass.CX_TypeClass_Record:
                    {
                        needsReturnFixup = true;
                        break;
                    }

                    default:
                    {
                        AddDiagnostic(DiagnosticLevel.Error, $"Unsupported return type for abstract method: '{canonicalReturnType.TypeClass}'. Generated bindings may be incomplete.", cxxMethodDecl);
                        break;
                    }
                }
            }

            return needsReturnFixup;
        }

        private bool NeedsNewKeyword(string name)
        {
            return name.Equals("Equals")
                || name.Equals("GetHashCode")
                || name.Equals("GetType")
                || name.Equals("MemberwiseClone")
                || name.Equals("ReferenceEquals")
                || name.Equals("ToString");
        }

        private bool NeedsNewKeyword(string name, IReadOnlyList<ParmVarDecl> parmVarDecls)
        {
            if (name.Equals("GetHashCode")
                || name.Equals("GetType")
                || name.Equals("MemberwiseClone")
                || name.Equals("ToString"))
            {
                return parmVarDecls.Count == 0;
            }

            return false;
        }

        private void ParenthesizeStmt(Stmt stmt)
        {
            if (IsStmtAsWritten<ParenExpr>(stmt, out _))
            {
                Visit(stmt);
            }
            else
            {
                _outputBuilder.Write('(');
                Visit(stmt);
                _outputBuilder.Write(')');
            }
        }

        private string PrefixAndStripName(string name)
        {
            if (name.StartsWith(_config.MethodPrefixToStrip))
            {
                name = name.Substring(_config.MethodPrefixToStrip.Length);
            }

            return '_' + name;
        }

        private void StartUsingOutputBuilder(string name, bool includeTestOutput = false)
        {
            if (_outputBuilder != null)
            {
                Debug.Assert(_outputBuilderUsers >= 1);
                _outputBuilderUsers++;

                _outputBuilder.NeedsNewline = true;

                if (includeTestOutput && !string.IsNullOrWhiteSpace(_config.TestOutputLocation))
                {
                    _testOutputBuilder.NeedsNewline = true;
                }
                return;
            }

            Debug.Assert(_outputBuilderUsers == 0);

            if (!_outputBuilderFactory.TryGetOutputBuilder(name, out _outputBuilder))
            {
                _outputBuilder = _outputBuilderFactory.Create(name);

                WithAttributes("*");
                WithAttributes(name);

                WithUsings("*");
                WithUsings(name);

                if (includeTestOutput && !string.IsNullOrWhiteSpace(_config.TestOutputLocation))
                {
                    _testOutputBuilder = _outputBuilderFactory.Create($"{name}Tests", isTestOutput: true);

                    _testOutputBuilder.AddUsingDirective("System.Runtime.InteropServices");

                    if (_config.GenerateTestsNUnit)
                    {
                        _testOutputBuilder.AddUsingDirective("NUnit.Framework");
                    }
                    else if (_config.GenerateTestsXUnit)
                    {
                        _testOutputBuilder.AddUsingDirective("Xunit");
                    }
                }
            }
            else
            {
                _outputBuilder.NeedsNewline = true;

                if (includeTestOutput && !string.IsNullOrWhiteSpace(_config.TestOutputLocation))
                {
                    _testOutputBuilder = _outputBuilderFactory.GetOutputBuilder($"{name}Tests");
                    Debug.Assert(_testOutputBuilder.IsTestOutput);
                    _testOutputBuilder.NeedsNewline = true;
                }
            }
            _outputBuilderUsers++;
        }

        private void StopUsingOutputBuilder()
        {
            if (_outputBuilderUsers == 1)
            {
                _outputBuilder = null;
                _testOutputBuilder = null;
            }
            _outputBuilderUsers--;
        }

        private bool TryGetUuid(RecordDecl recordDecl, out Guid uuid)
        {
            var uuidAttrs = recordDecl.Attrs.Where((attr) => attr.Kind == CX_AttrKind.CX_AttrKind_Uuid);

            if (uuidAttrs.Count() == 0)
            {
                uuid = Guid.Empty;
                return false;
            }
            else if (uuidAttrs.Count() != 1)
            {
                AddDiagnostic(DiagnosticLevel.Warning, $"Multiply uuid attributes for {recordDecl.Name}. Falling back to first attribute.", recordDecl);
            }

            var uuidAttr = uuidAttrs.First();
            var uuidAttrText = GetSourceRangeContents(recordDecl.TranslationUnit.Handle, uuidAttr.Extent);
            var uuidText = uuidAttrText.Split(new char[] { '"' }, StringSplitOptions.RemoveEmptyEntries)[1];

            if (!Guid.TryParse(uuidText, out uuid))
            {
                AddDiagnostic(DiagnosticLevel.Warning, $"Failed to parse uuid attr text '{uuidAttrText}'. Extracted portion: '{uuidText}'.", recordDecl);
                return false;
            }
            return true;
        }

        private bool TryRemapOperatorName(ref string name, FunctionDecl functionDecl)
        {
            var numArgs = functionDecl.Parameters.Count;

            if (functionDecl.DeclContext is CXXRecordDecl)
            {
                numArgs++;
            }

            if (functionDecl is CXXConversionDecl)
            {
                var returnType = functionDecl.ReturnType;
                var returnTypeName = GetRemappedTypeName(cursor: null, context: null, returnType, out _);

                name = $"To{returnTypeName}";
                return true;
            }

            switch (name)
            {
                case "operator+":
                {
                    name = (numArgs == 1) ? "Plus" : "Add";
                    return true;
                }

                case "operator-":
                {
                    name = (numArgs == 1) ? "Negate" : "Subtract";
                    return true;
                }

                case "operator!":
                {
                    name = "Not";
                    return true;
                }

                case "operator~":
                {
                    name = "OnesComplement";
                    return true;
                }

                case "operator++":
                {
                    name = "Increment";
                    return true;
                }

                case "operator--":
                {
                    name = "Decrement";
                    return true;
                }

                case "operator*":
                {
                    name = "Multiply";
                    return true;
                }

                case "operator/":
                {
                    name = "Divide";
                    return true;
                }

                case "operator%":
                {
                    name = "Modulus";
                    return true;
                }

                case "operator&":
                {
                    name = "And";
                    return true;
                }

                case "operator|":
                {
                    name = "Or";
                    return true;
                }

                case "operator^":
                {
                    name = "Xor";
                    return true;
                }

                case "operator<<":
                {
                    name = "LeftShift";
                    return true;
                }

                case "operator>>":
                {
                    name = "RightShift";
                    return true;
                }

                case "operator==":
                {
                    name = "Equals";
                    return true;
                }

                case "operator!=":
                {
                    name = "NotEquals";
                    return true;
                }

                case "operator<":
                {
                    name = "LessThan";
                    return true;
                }

                case "operator>":
                {
                    name = "GreaterThan";
                    return true;
                }

                case "operator<=":
                {
                    name = "LessThanOrEquals";
                    return true;
                }

                case "operator>=":
                {
                    name = "GreaterThanOrEquals";
                    return true;
                }

                default:
                {
                    return false;
                }
            }
        }

        private void UncheckStmt(string targetTypeName, Stmt stmt)
        {
            if (IsUnchecked(targetTypeName, stmt))
            {
                _outputBuilder.Write("unchecked");

                var needsCast = IsStmtAsWritten<IntegerLiteral>(stmt, out _, removeParens: true) && (stmt.DeclContext is EnumDecl);

                if (IsStmtAsWritten<UnaryExprOrTypeTraitExpr>(stmt, out var unaryExprOrTypeTraitExpr, removeParens: true) && ((CurrentContext is VarDecl) || IsPrevContextDecl<VarDecl>(out _)))
                {
                    var argumentType = unaryExprOrTypeTraitExpr.TypeOfArgument;

                    long size32;
                    long size64;

                    long alignment32 = -1;
                    long alignment64 = -1;

                    GetTypeSize(unaryExprOrTypeTraitExpr, argumentType, ref alignment32, ref alignment64, out size32, out size64);

                    switch (unaryExprOrTypeTraitExpr.Kind)
                    {
                        case CX_UnaryExprOrTypeTrait.CX_UETT_SizeOf:
                        {
                            needsCast |= (size32 != size64);
                            break;
                        }

                        case CX_UnaryExprOrTypeTrait.CX_UETT_AlignOf:
                        case CX_UnaryExprOrTypeTrait.CX_UETT_PreferredAlignOf:
                        {
                            needsCast |= (alignment32 != alignment64);
                            break;
                        }

                        default:
                        {
                            break;
                        }
                    }
                }

                if (needsCast)
                {
                    _outputBuilder.Write("((");
                    _outputBuilder.Write(targetTypeName);
                    _outputBuilder.Write(')');
                }

                ParenthesizeStmt(stmt);

                if (needsCast)
                {
                    _outputBuilder.Write(')');
                }
            }
            else
            {
                Visit(stmt);
            }
        }

        private void Visit(Cursor cursor)
        {
            var currentContext = _context.AddLast(cursor);

            if (cursor is Attr attr)
            {
                VisitAttr(attr);
            }
            else if (cursor is Decl decl)
            {
                VisitDecl(decl);
            }
            else if (cursor is Ref @ref)
            {
                VisitRef(@ref);
            }
            else if (cursor is Stmt stmt)
            {
                VisitStmt(stmt);
            }
            else
            {
                AddDiagnostic(DiagnosticLevel.Error, $"Unsupported cursor: '{cursor.CursorKindSpelling}'. Generated bindings may be incomplete.", cursor);
            }

            Debug.Assert(_context.Last == currentContext);
            _context.RemoveLast();
        }

        private void Visit(IEnumerable<Cursor> cursors)
        {
            foreach (var cursor in cursors)
            {
                Visit(cursor);
            }
        }

        private void Visit(IEnumerable<Cursor> cursors, IEnumerable<Cursor> excludedCursors)
        {
            Visit(cursors.Except(excludedCursors));
        }

        private void WithAttributes(string remappedName)
        {
            if (_config.WithAttributes.TryGetValue(remappedName, out IReadOnlyList<string> attributes))
            {
                foreach (var attribute in attributes)
                {
                    if (attribute.Equals("Flags") || attribute.Equals("Obsolete"))
                    {
                        _outputBuilder.AddUsingDirective("System");
                    }
                    else if (attribute.Equals("EditorBrowsable") || attribute.StartsWith("EditorBrowsable("))
                    {
                        _outputBuilder.AddUsingDirective("System.ComponentModel");
                    }
                    else if (attribute.StartsWith("Guid("))
                    {
                        _outputBuilder.AddUsingDirective("System.Runtime.InteropServices");
                    }

                    _outputBuilder.WriteIndented('[');
                    _outputBuilder.Write(attribute);
                    _outputBuilder.WriteLine(']');
                }
            }
        }

        private void WithLibraryPath(string remappedName)
        {
            if (!_config.WithLibraryPaths.TryGetValue(remappedName, out string libraryPath) && !_config.WithLibraryPaths.TryGetValue("*", out libraryPath))
            {
                _outputBuilder.Write(_config.LibraryPath);
            }
            else
            {
                _outputBuilder.Write('"');
                _outputBuilder.Write(libraryPath);
                _outputBuilder.Write('"');
            }
        }

        private void WithSetLastError(string remappedName)
        {
            if (_config.WithSetLastErrors.Contains("*") || _config.WithSetLastErrors.Contains(remappedName))
            {
                _outputBuilder.Write(',');
                _outputBuilder.Write(' ');
                _outputBuilder.Write("SetLastError");
                _outputBuilder.Write(' ');
                _outputBuilder.Write('=');
                _outputBuilder.Write(' ');
                _outputBuilder.Write("true");
            }
        }

        private void WithTestAttribute()
        {
            if (_config.GenerateTestsNUnit)
            {
                _testOutputBuilder.WriteIndentedLine("[Test]");
            }
            else if (_config.GenerateTestsXUnit)
            {
                _testOutputBuilder.WriteIndentedLine("[Fact]");
            }
        }

        private void WithTestAssertEqual(string expected, string actual)
        {
            if (_config.GenerateTestsNUnit)
            {
                _testOutputBuilder.WriteIndented("Assert.That");
                _testOutputBuilder.Write('(');
                _testOutputBuilder.Write(actual);
                _testOutputBuilder.Write(',');
                _testOutputBuilder.Write(' ');
                _testOutputBuilder.Write("Is.EqualTo");
                _testOutputBuilder.Write('(');
                _testOutputBuilder.Write(expected);
                _testOutputBuilder.Write(')');
                _testOutputBuilder.Write(')');
                _testOutputBuilder.WriteSemicolon();
                _testOutputBuilder.WriteNewline();
            }
            else if (_config.GenerateTestsXUnit)
            {
                _testOutputBuilder.WriteIndented("Assert.Equal");
                _testOutputBuilder.Write('(');
                _testOutputBuilder.Write(expected);
                _testOutputBuilder.Write(',');
                _testOutputBuilder.Write(' ');
                _testOutputBuilder.Write(actual);
                _testOutputBuilder.Write(')');
                _testOutputBuilder.WriteSemicolon();
                _testOutputBuilder.WriteNewline();
            }
        }

        private void WithTestAssertTrue(string actual)
        {
            if (_config.GenerateTestsNUnit)
            {
                _testOutputBuilder.WriteIndented("Assert.That");
                _testOutputBuilder.Write('(');
                _testOutputBuilder.Write(actual);
                _testOutputBuilder.Write(',');
                _testOutputBuilder.Write(' ');
                _testOutputBuilder.Write("Is.True");
                _testOutputBuilder.Write(')');
                _testOutputBuilder.WriteSemicolon();
                _testOutputBuilder.WriteNewline();
            }
            else if (_config.GenerateTestsXUnit)
            {
                _testOutputBuilder.WriteIndented("Assert.True");
                _testOutputBuilder.Write('(');
                _testOutputBuilder.Write(actual);
                _testOutputBuilder.Write(')');
                _testOutputBuilder.WriteSemicolon();
                _testOutputBuilder.WriteNewline();
            }
        }

        private void WithType(string remappedName, ref string integerTypeName, ref string nativeTypeName)
        {
            if (_config.WithTypes.TryGetValue(remappedName, out string type))
            {
                if (string.IsNullOrWhiteSpace(nativeTypeName))
                {
                    nativeTypeName = integerTypeName;
                }

                integerTypeName = type;

                if (nativeTypeName.Equals(type))
                {
                    nativeTypeName = string.Empty;
                }
            }
        }

        private void WithUsings(string remappedName)
        {
            if (_config.WithUsings.TryGetValue(remappedName, out IReadOnlyList<string> usings))
            {
                foreach (var @using in usings)
                {
                    _outputBuilder.AddUsingDirective(@using);
                }
            }
        }
    }
}
