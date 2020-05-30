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
        private readonly HashSet<Decl> _visitedDecls;
        private readonly HashSet<string> _visitedFiles;
        private readonly List<Diagnostic> _diagnostics;
        private readonly PInvokeGeneratorConfiguration _config;

        private OutputBuilder _outputBuilder;
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
            _visitedDecls = new HashSet<Decl>();
            _visitedFiles = new HashSet<string>();
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

                using var sw = new StreamWriter(stream, defaultStreamWriterEncoding, DefaultStreamWriterBufferSize, leaveStreamOpen);
                sw.NewLine = "\n";

                sw.WriteLine('}');
            }

            _diagnostics.Clear();
            _outputBuilderFactory.Clear();
            _visitedDecls.Clear();
        }

        public void Dispose()
        {
            Dispose(isDisposing: true);
            GC.SuppressFinalize(this);
        }

        public void GenerateBindings(TranslationUnit translationUnit)
        {
            Debug.Assert(_outputBuilder is null);

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

            Visit(translationUnit.TranslationUnitDecl);
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

            _outputBuilder.Write("NativeTypeName(");
            _outputBuilder.Write('"');
            _outputBuilder.Write(nativeTypeName.Replace('\\', '/'));
            _outputBuilder.Write('"');
            _outputBuilder.Write(")]");

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
                        sw.Write("using");
                        sw.Write(' ');
                        sw.Write(usingDirective);
                        sw.WriteLine(';');
                    }

                    sw.WriteLine();
                }
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

                if (_isMethodClassUnsafe)
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

        private string GetArtificalFixedSizedBufferName(FieldDecl fieldDecl)
        {
            var name = GetRemappedCursorName(fieldDecl);
            return $"_{name}_e__FixedBuffer";
        }

        private string GetCallingConventionName(Cursor cursor, CXCallingConv callingConvention, string remappedName)
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
                    var name = "Winapi";
                    AddDiagnostic(DiagnosticLevel.Warning, $"Unsupported calling convention: '{callingConvention}'. Falling back to '{name}'.", cursor);
                    return name;
                }
            }
        }

        private string GetCursorName(NamedDecl namedDecl)
        {
            var name = namedDecl.Name;

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
                        name = GetTypeName(namedDecl, typeDecl.TypeForDecl, out var nativeTypeName);
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

        private string GetCursorQualifiedName(NamedDecl namedDecl)
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

            static void AppendFunctionParameters(CXType functionType, StringBuilder qualifiedName)
            {
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

            static void AppendTemplateParameters(TemplateDecl templateDecl, StringBuilder qualifiedName)
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

        private string GetRemappedAnonymousName(Cursor cursor, string kind)
        {
            var name = GetAnonymousName(cursor, kind);
            return GetRemappedName(name, cursor, tryRemapOperatorName: true);
        }

        private string GetRemappedCursorName(NamedDecl namedDecl)
        {
            var name = GetCursorName(namedDecl);
            return GetRemappedName(name, namedDecl, tryRemapOperatorName: true);
        }

        private string GetRemappedName(string name, Cursor cursor, bool tryRemapOperatorName)
        {
            if (!_config.RemappedNames.TryGetValue(name, out string remappedName))
            {
                if ((cursor is FunctionDecl functionDecl) && tryRemapOperatorName)
                {
                    TryRemapOperatorName(ref name, functionDecl);
                }
                remappedName = name;
            }

            if (remappedName.Equals("Guid") || remappedName.Equals("IntPtr") || remappedName.Equals("UIntPtr"))
            {
                _outputBuilder.AddUsingDirective("System");
            }

            return remappedName;
        }

        private string GetRemappedTypeName(Cursor cursor, Type type, out string nativeTypeName)
        {
            var name = GetTypeName(cursor, type, out nativeTypeName);
            name = GetRemappedName(name, cursor, tryRemapOperatorName: false);

            if (nativeTypeName.Equals(name))
            {
                nativeTypeName = string.Empty;
            }
            return name;
        }

        private string GetTypeName(Cursor cursor, Type type, out string nativeTypeName)
        {
            var name = type.AsString;
            nativeTypeName = name;

            if (type is ArrayType arrayType)
            {
                name = GetTypeName(cursor, arrayType.ElementType, out var nativeElementTypeName);
            }
            else if (type is AttributedType attributedType)
            {
                name = GetTypeName(cursor, attributedType.ModifiedType, out var nativeModifiedTypeName);
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
            else if (type is ElaboratedType elaboratedType)
            {
                name = GetTypeName(cursor, elaboratedType.NamedType, out var nativeNamedTypeName);
            }
            else if (type is FunctionType functionType)
            {
                name = GetTypeNameForPointeeType(cursor, functionType, out var nativeFunctionTypeName);
            }
            else if (type is PointerType pointerType)
            {
                name = GetTypeNameForPointeeType(cursor, pointerType.PointeeType, out var nativePointeeTypeName);
            }
            else if (type is ReferenceType referenceType)
            {
                name = GetTypeNameForPointeeType(cursor, referenceType.PointeeType, out var nativePointeeTypeName);
            }
            else if (type is TagType tagType)
            {
                if (tagType.Decl.Handle.IsAnonymous)
                {
                    name = GetAnonymousName(tagType.Decl, tagType.KindSpelling);
                }
                else if (tagType.Handle.IsConstQualified)
                {
                    name = GetTypeName(cursor, tagType.Decl.TypeForDecl, out var nativeDeclTypeName);
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
                    name = GetTypeName(cursor, typedefType.Decl.UnderlyingType, out var nativeUnderlyingTypeName);
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

        private string GetTypeNameForPointeeType(Cursor cursor, Type pointeeType, out string nativePointeeTypeName)
        {
            var name = pointeeType.AsString;
            nativePointeeTypeName = name;

            if (pointeeType is AttributedType attributedType)
            {
                name = GetTypeNameForPointeeType(cursor, attributedType.ModifiedType, out var nativeModifiedTypeName);
            }
            else if (pointeeType is FunctionType functionType)
            {
                if (_config.GeneratePreviewCodeFnptr && (functionType is FunctionProtoType functionProtoType))
                {
                    var remappedName = GetRemappedName(name, cursor, tryRemapOperatorName: false);
                    var callConv = GetCallingConventionName(cursor, functionType.CallConv, remappedName).ToLower();

                    var nameBuilder = new StringBuilder();
                    nameBuilder.Append("delegate");
                    nameBuilder.Append('*');
                    nameBuilder.Append(' ');
                    nameBuilder.Append((callConv != "winapi") ? callConv : "unmanaged");
                    nameBuilder.Append('<');

                    foreach (var paramType in functionProtoType.ParamTypes)
                    {
                        nameBuilder.Append(GetRemappedTypeName(cursor, paramType, out _));
                        nameBuilder.Append(',');
                        nameBuilder.Append(' ');
                    }

                    nameBuilder.Append(GetRemappedTypeName(cursor, functionType.ReturnType, out _));
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
                name = GetTypeName(cursor, pointeeType, out nativePointeeTypeName);
                name = GetRemappedName(name, cursor, tryRemapOperatorName: false);
                name += '*';
            }

            return name;
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

        private bool IsExcluded(Decl decl)
        {
            if (IsAlwaysIncluded())
            {
                return false;
            }

            return IsExcludedByFile(decl) || IsExcludedByName(decl);

            bool IsAlwaysIncluded()
            {
                return (decl is TranslationUnitDecl) || (decl is LinkageSpecDecl);
            }

            bool IsExcludedByFile(Decl decl)
            {
                if (_outputBuilder != null)
                {
                    // We don't want to exclude  by fileif we already have an active output builder as we
                    // are likely processing members of an already included type but those members may
                    // indirectly exist or be defined in a non-traversed file.
                    return false;
                }

                var declLocation = decl.Location;
                declLocation.GetFileLocation(out CXFile file, out _, out _, out _);

                if (IsIncludedFileOrLocation(decl, file, declLocation))
                {
                    return false;
                }

                // It is not uncommon for some declarations to be done using macros, which are themselves
                // defined in an imported header file. We want to also check if the expansion location is
                // in the main file to catch these cases and ensure we still generate bindings for them.

                declLocation.GetExpansionLocation(out file, out uint line, out uint column, out _);
                declLocation = decl.TranslationUnit.Handle.GetLocation(file, line, column);

                if (IsIncludedFileOrLocation(decl, file, declLocation))
                {
                    return false;
                }

                return true;
            }

            bool IsExcludedByName(Decl decl)
            {
                if (!(decl is NamedDecl))
                {
                    return false;
                }

                var namedDecl = (NamedDecl)decl;

                // We get the non-remapped name for the purpose of exclusion checks to ensure that users
                // can remove no-definition declarations in favor of remapped anonymous declarations.

                var qualifiedName = GetCursorQualifiedName(namedDecl);

                if (_config.ExcludedNames.Contains(qualifiedName))
                {
                    if (_config.LogExclusions)
                    {
                        AddDiagnostic(DiagnosticLevel.Info, $"Excluded by exact match {qualifiedName}");
                    }
                    return true;
                }

                var name = GetCursorName(namedDecl);

                if (_config.ExcludedNames.Contains(name))
                {
                    if (_config.LogExclusions)
                    {
                        AddDiagnostic(DiagnosticLevel.Info, $"Excluded {qualifiedName} by partial match against {name}");
                    }
                    return true;
                }

                if (namedDecl is TagDecl tagDecl)
                {
                    if ((tagDecl.Definition != tagDecl) && (tagDecl.Definition != null))
                    {
                        // We don't want to generate bindings for anything
                        // that is not itself a definition and that has a
                        // definition that can be resolved. This ensures we
                        // still generate bindings for things which are used
                        // as opaque handles, but which aren't ever defined.

                        return true;
                    }
                }

                return false;
            }

            bool IsIncludedFileOrLocation(Decl decl, CXFile file, CXSourceLocation location)
            {
                // Use case insensitive comparison on Windows
                var equalityComparer = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal;

                // Normalize paths to be '/' for comparison
                var fileName = file.Name.ToString().Replace('\\', '/');

                if (_visitedFiles.Add(fileName) && _config.LogVisitedFiles)
                {
                    AddDiagnostic(DiagnosticLevel.Info, $"Visiting {fileName}");
                }

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
            else if (type is ReferenceType)
            {
                return false;
            }
            else if (type is RecordType recordType)
            {
                var recordDecl = recordType.Decl;

                return recordDecl.Fields.All((fieldDecl) => IsFixedSize(fieldDecl, fieldDecl.Type))
                    && (!(recordDecl is CXXRecordDecl cxxRecordDecl) || cxxRecordDecl.Methods.All((cxxMethodDecl) => !cxxMethodDecl.IsVirtual));
            }
            else if (type is TypedefType typedefType)
            {
                var remappedName = GetRemappedTypeName(cursor, typedefType, out _);

                return (remappedName == "IntPtr")
                    || (remappedName == "nint")
                    || (remappedName == "nuint")
                    || (remappedName == "UIntPtr")
                    || IsFixedSize(cursor, typedefType.Decl.UnderlyingType);
            }
            else
            {
                AddDiagnostic(DiagnosticLevel.Warning, $"Unsupported type: '{type.TypeClass}'. Assuming unfixed size.", cursor);
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

        private bool IsUnsafe(FieldDecl fieldDecl)
        {
            var type = fieldDecl.Type;

            if (type is ConstantArrayType)
            {
                var typeName = GetRemappedTypeName(fieldDecl, type, out _);
                return IsSupportedFixedSizedBufferType(typeName);
            }

            return IsUnsafe(fieldDecl, type);
        }

        private bool IsUnsafe(FunctionDecl functionDecl)
        {
            var returnType = functionDecl.ReturnType;
            var returnTypeName = GetRemappedTypeName(functionDecl, returnType, out _);

            if (returnTypeName.Contains('*'))
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
            foreach (var fieldDecl in recordDecl.Fields)
            {
                if (IsUnsafe(fieldDecl))
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
            var typeName = GetRemappedTypeName(namedDecl, type, out _);
            return typeName.Contains('*');
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

        private string PrefixAndStripName(string name)
        {
            if (name.StartsWith(_config.MethodPrefixToStrip))
            {
                name = name.Substring(_config.MethodPrefixToStrip.Length);
            }

            return '_' + name;
        }

        private void StartUsingOutputBuilder(string name)
        {
            if (_outputBuilder != null)
            {
                Debug.Assert(_outputBuilderUsers >= 1);
                _outputBuilderUsers++;

                _outputBuilder.NeedsNewline = true;
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
            }
            else
            {
                _outputBuilder.NeedsNewline = true;
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
                var returnTypeName = GetRemappedTypeName(cursor: null, returnType, out _);

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

        private void Visit(Cursor cursor)
        {
            if (cursor is Attr attr)
            {
                VisitAttr(attr);
            }
            else if (cursor is Decl decl)
            {
                VisitDecl(decl, ignorePriorVisit: false);
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
                _outputBuilder.Write(", SetLastError = true");
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
