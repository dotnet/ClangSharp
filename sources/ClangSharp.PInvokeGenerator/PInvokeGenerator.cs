// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class PInvokeGenerator : IDisposable
    {
        private const int DefaultStreamWriterBufferSize = 1024;
        private static readonly Encoding defaultStreamWriterEncoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: true);

        private readonly CXIndex _index;
        private readonly OutputBuilderFactory _outputBuilderFactory;
        private readonly Func<string, Stream> _outputStreamFactory;
        private readonly HashSet<Cursor> _visitedCursors;
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
            _visitedCursors.Clear();
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
                        errorDiagnostics.AppendLine(diagnostic.Format(CXDiagnosticDisplayOptions.CXDiagnostic_DisplayOption).ToString());
                    }
                }

                if (invalidTranslationUnitHandle)
                {
                    throw new ArgumentOutOfRangeException(nameof(translationUnit), errorDiagnostics.ToString());
                }
            }

            var translationUnitDecl = translationUnit.TranslationUnitDecl;
            _visitedCursors.Add(translationUnitDecl);

            foreach (var decl in translationUnitDecl.Decls)
            {
                if (_config.TraversalNames.Length == 0)
                {
                    if (!decl.Location.IsFromMainFile)
                    {
                        continue;
                    }
                }
                else
                {
                    decl.Location.GetFileLocation(out CXFile file, out _, out _, out _);
                    var fileName = file.Name.ToString();

                    if (!_config.TraversalNames.Contains(fileName))
                    {
                        continue;
                    }
                }
                Visit(decl);
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
                _outputBuilder.Write(prefix);
            }

            _outputBuilder.Write('[');

            if (attributePrefix != null)
            {
                _outputBuilder.Write(attributePrefix);
            }

            _outputBuilder.Write("NativeTypeName(");
            _outputBuilder.Write('"');
            _outputBuilder.Write(nativeTypeName);
            _outputBuilder.Write('"');
            _outputBuilder.Write(")]");

            if (postfix is null)
            {
                _outputBuilder.WriteLine();
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

                sw.Write(indentationString);
                sw.Write("private const string libraryPath =");
                sw.Write(' ');
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

        private string GetArtificalFixedSizedBufferName(FieldDecl fieldDecl)
        {
            var name = GetRemappedCursorName(fieldDecl);
            return $"_{name}_e__FixedBuffer";
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

        private string GetCursorName(NamedDecl namedDecl)
        {
            var name = namedDecl.Name;

            if (string.IsNullOrWhiteSpace(name))
            {
                if (namedDecl is TypeDecl typeDecl)
                {
                    if ((typeDecl is TagDecl tagDecl) && tagDecl.Handle.IsAnonymous)
                    {
                        namedDecl.Location.GetFileLocation(out var file, out var _, out var _, out var offset);
                        var fileName = Path.GetFileNameWithoutExtension(file.Name.ToString());
                        name = $"__Anonymous{tagDecl.TypeForDecl.KindSpelling}_{fileName}_{offset}";
                        AddDiagnostic(DiagnosticLevel.Info, $"Anonymous declaration found in '{nameof(GetCursorName)}'. Falling back to '{name}'.'", namedDecl);
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
                else
                {
                    AddDiagnostic(DiagnosticLevel.Error, $"Unsupported anonymous named declaration: '{namedDecl.KindSpelling}'.", namedDecl);
                }
            }

            Debug.Assert(!string.IsNullOrWhiteSpace(name));
            return name;
        }

        private string GetRemappedCursorName(NamedDecl namedDecl)
        {
            var name = GetCursorName(namedDecl);
            return GetRemappedName(name);
        }

        private string GetRemappedName(string name)
        {
            if (!_config.RemappedNames.TryGetValue(name, out string remappedName))
            {
                remappedName = name;
            }

            if (remappedName.Equals("IntPtr") || remappedName.Equals("UIntPtr"))
            {
                _outputBuilder.AddUsingDirective("System");
            }

            return remappedName;
        }

        private string GetRemappedTypeName(NamedDecl namedDecl, Type type, out string nativeTypeName)
        {
            var name = GetTypeName(namedDecl, type, out nativeTypeName);
            return GetRemappedName(name);
        }

        private string GetTypeName(NamedDecl namedDecl, Type type, out string nativeTypeName)
        {
            var name = type.AsString;
            nativeTypeName = name;

            if (type is ArrayType arrayType)
            {
                name = GetTypeName(namedDecl, arrayType.ElementType, out var nativeElementTypeName);
            }
            else if (type is AttributedType attributedType)
            {
                name = GetTypeName(namedDecl, attributedType.ModifiedType, out var nativeModifiedTypeName);
            }
            else if (type is BuiltinType)
            {
                switch (type.Kind)
                {
                    case CXTypeKind.CXType_Void:
                    {
                        name = "void";
                        break;
                    }

                    case CXTypeKind.CXType_Bool:
                    {
                        name = "bool";
                        break;
                    }

                    case CXTypeKind.CXType_Char_U:
                    case CXTypeKind.CXType_UChar:
                    {
                        name = "byte";
                        break;
                    }

                    case CXTypeKind.CXType_UShort:
                    {
                        name = "ushort";
                        break;
                    }

                    case CXTypeKind.CXType_UInt:
                    {
                        name = "uint";
                        break;
                    }

                    case CXTypeKind.CXType_ULong:
                    {
                        name = _config.GenerateUnixTypes ? "UIntPtr" : "uint";
                        break;
                    }

                    case CXTypeKind.CXType_ULongLong:
                    {
                        name = "ulong";
                        break;
                    }

                    case CXTypeKind.CXType_Char_S:
                    case CXTypeKind.CXType_SChar:
                    {
                        name = "sbyte";
                        break;
                    }

                    case CXTypeKind.CXType_WChar:
                    {
                        name = _config.GenerateUnixTypes ? "int" : "char";
                        break;
                    }

                    case CXTypeKind.CXType_Short:
                    {
                        name = "short";
                        break;
                    }

                    case CXTypeKind.CXType_Int:
                    {
                        name = "int";
                        break;
                    }

                    case CXTypeKind.CXType_Long:
                    {
                        name = _config.GenerateUnixTypes ? "IntPtr" : "int";
                        break;
                    }

                    case CXTypeKind.CXType_LongLong:
                    {
                        name = "long";
                        break;
                    }

                    case CXTypeKind.CXType_Float:
                    {
                        name = "float";
                        break;
                    }

                    case CXTypeKind.CXType_Double:
                    {
                        name = "double";
                        break;
                    }

                    default:
                    {
                        AddDiagnostic(DiagnosticLevel.Warning, $"Unsupported builtin type: '{type.KindSpelling}'. Falling back '{name}'.", namedDecl);
                        break;
                    }
                }
            }
            else if (type is ElaboratedType elaboratedType)
            {
                name = GetTypeName(namedDecl, elaboratedType.NamedType, out var nativeNamedTypeName);
            }
            else if (type is PointerType pointerType)
            {
                name = GetTypeNameForPointeeType(namedDecl, pointerType.PointeeType, out var nativePointeeTypeName);
            }
            else if (type is ReferenceType referenceType)
            {
                name = GetTypeNameForPointeeType(namedDecl, referenceType.PointeeType, out var nativePointeeTypeName);
            }
            else if (type is TypedefType typedefType)
            {
                // We check remapped names here so that types that have variable sizes
                // can be treated correctly. Otherwise, they will resolve to a particular
                // platform size, based on whatever parameters were passed into clang.

                if (_config.RemappedNames.TryGetValue(name, out string remappedName))
                {
                    name = remappedName;
                }
                else
                {
                    name = GetTypeName(namedDecl, typedefType.Decl.UnderlyingType, out var nativeUnderlyingTypeName);
                }
            }
            else if (!(type is FunctionType) && !(type is TagType))
            {
                AddDiagnostic(DiagnosticLevel.Warning, $"Unsupported type: '{type.KindSpelling}'. Falling back '{name}'.", namedDecl);
            }

            Debug.Assert(!string.IsNullOrWhiteSpace(name));
            Debug.Assert(!string.IsNullOrWhiteSpace(nativeTypeName));

            if (nativeTypeName.Equals(name))
            {
                nativeTypeName = string.Empty;
            }
            return name;
        }

        private string GetTypeNameForPointeeType(NamedDecl namedDecl, Type pointeeType, out string nativePointeeTypeName)
        {
            var name = pointeeType.AsString;
            nativePointeeTypeName = name;

            if (pointeeType is AttributedType attributedType)
            {
                name = GetTypeNameForPointeeType(namedDecl, attributedType.ModifiedType, out var nativeModifiedTypeName);
            }
            else if (pointeeType is FunctionType)
            {
                name = "IntPtr";
            }
            else
            {
                name = GetTypeName(namedDecl, pointeeType, out nativePointeeTypeName);
                name += '*';
            }

            return name;
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
            return false;
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

        private void Visit(Cursor cursor)
        {
            if (_visitedCursors.Contains(cursor))
            {
                return;
            }

            _visitedCursors.Add(cursor);

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
                AddDiagnostic(DiagnosticLevel.Error, $"Unsupported cursor: '{cursor.KindSpelling}'. Generated bindings may be incomplete.", cursor);
            }
        }

        private void VisitAttr(Attr attr)
        {
            // We don't consider most attributes particularly important and so we do nothing
        }

        private void VisitBinaryOperator(BinaryOperator binaryOperator)
        {
            Visit(binaryOperator.LHS);
            _outputBuilder.Write(' ');
            _outputBuilder.Write(binaryOperator.Opcode);
            _outputBuilder.Write(' ');
            Visit(binaryOperator.RHS);
        }

        private void VisitCallExpr(CallExpr callExpr)
        {
            var calleeDecl = callExpr.CalleeDecl;

            if (calleeDecl is FunctionDecl functionDecl)
            {
                var name = GetRemappedCursorName(functionDecl);

                _outputBuilder.WriteIndented(EscapeAndStripName(name));
                _outputBuilder.Write('(');

                foreach (var argument in callExpr.Args)
                {
                    Visit(argument);
                }

                _outputBuilder.WriteLine(");");
            }
            else
            {
                AddDiagnostic(DiagnosticLevel.Error, $"Unsupported callee declaration: '{calleeDecl.KindSpelling}'. Generated bindings may be incomplete.", calleeDecl);
            }
        }

        private void VisitCompoundStmt(CompoundStmt compoundStmt)
        {
            _outputBuilder.WriteBlockStart();

            foreach (var stmt in compoundStmt.Body)
            {
                Visit(stmt);
            }

            _outputBuilder.WriteBlockEnd();
        }

        private void VisitDecl(Decl decl)
        {
            if (decl is AccessSpecDecl)
            {
                // Access specifications are also exposed as a queryable property
                // on the declarations they impact, so we don't need to do anything
            }
            else if (decl is NamedDecl namedDecl)
            {
                VisitNamedDecl(namedDecl);
            }
            else if (decl.Kind == CXCursorKind.CXCursor_UnexposedDecl)
            {
                VisitUnexposedDecl(decl);
            }
            else
            {
                AddDiagnostic(DiagnosticLevel.Error, $"Unsupported declaration: '{decl.KindSpelling}'. Generated bindings may be incomplete.", decl);
            }
        }

        private void VisitDeclaratorDecl(DeclaratorDecl declaratorDecl)
        {
            if (declaratorDecl is FieldDecl fieldDecl)
            {
                VisitFieldDecl(fieldDecl);
            }
            else if (declaratorDecl is FunctionDecl functionDecl)
            {
                VisitFunctionDecl(functionDecl);
            }
            else if (declaratorDecl is VarDecl varDecl)
            {
                VisitVarDecl(varDecl);
            }
            else
            {
                AddDiagnostic(DiagnosticLevel.Error, $"Unsupported declarator declaration: '{declaratorDecl.KindSpelling}'. Generated bindings may be incomplete.", declaratorDecl);
            }
        }

        private void VisitDeclRefExpr(DeclRefExpr declRefExpr)
        {
            var name = GetRemappedCursorName(declRefExpr.Decl);
            _outputBuilder.Write(EscapeName(name));
        }

        private void VisitEnumConstantDecl(EnumConstantDecl enumConstantDecl)
        {
            var name = GetRemappedCursorName(enumConstantDecl);

            _outputBuilder.WriteIndentation();
            _outputBuilder.Write(EscapeName(name));

            if (enumConstantDecl.InitExpr != null)
            {
                _outputBuilder.Write(' ');
                _outputBuilder.Write('=');
                _outputBuilder.Write(' ');
                Visit(enumConstantDecl.InitExpr);
            }

            _outputBuilder.WriteLine(',');
        }

        private void VisitEnumDecl(EnumDecl enumDecl)
        {
            var name = GetRemappedCursorName(enumDecl);

            StartUsingOutputBuilder(name);
            {
                var integerTypeName = GetRemappedTypeName(enumDecl, enumDecl.IntegerType, out var nativeTypeName);
                AddNativeTypeNameAttribute(nativeTypeName);

                _outputBuilder.WriteIndented(GetAccessSpecifierName(enumDecl));
                _outputBuilder.Write(' ');
                _outputBuilder.Write("enum");
                _outputBuilder.Write(' ');
                _outputBuilder.Write(EscapeName(name));

                if (!integerTypeName.Equals("int"))
                {
                    _outputBuilder.Write(' ');
                    _outputBuilder.Write(':');
                    _outputBuilder.Write(' ');
                    _outputBuilder.Write(integerTypeName);
                }

                _outputBuilder.WriteLine();
                _outputBuilder.WriteBlockStart();

                foreach (var enumerator in enumDecl.Enumerators)
                {
                    Visit(enumerator);
                }

                foreach (var declaration in enumDecl.Decls)
                {
                    Visit(declaration);
                }

                _outputBuilder.WriteBlockEnd();
            }
            StopUsingOutputBuilder();
        }

        private void VisitExpr(Expr expr)
        {
            if (expr is BinaryOperator binaryOperator)
            {
                VisitBinaryOperator(binaryOperator);
            }
            else if (expr is CallExpr callExpr)
            {
                VisitCallExpr(callExpr);
            }
            else if (expr is DeclRefExpr declRefExpr)
            {
                VisitDeclRefExpr(declRefExpr);
            }
            else if (expr is IntegerLiteral integerLiteral)
            {
                VisitIntegerLiteral(integerLiteral);
            }
            else if (expr is ParenExpr parenExpr)
            {
                VisitParenExpr(parenExpr);
            }
            else if (expr is UnaryOperator unaryOperator)
            {
                VisitUnaryOperator(unaryOperator);
            }
            else if (expr.Kind == CXCursorKind.CXCursor_UnexposedExpr)
            {
                VisitUnexposedExpr(expr);
            }
            else
            {
                AddDiagnostic(DiagnosticLevel.Error, $"Unsupported expression: '{expr.KindSpelling}'. Generated bindings may be incomplete.", expr);
            }
        }

        private void VisitFieldDecl(FieldDecl fieldDecl)
        {
            var name = GetRemappedCursorName(fieldDecl);
            var escapedName = EscapeName(name);

            var type = fieldDecl.Type;
            var typeName = GetRemappedTypeName(fieldDecl, type, out var nativeTypeName);

            if (fieldDecl.Parent.IsUnion)
            {
                _outputBuilder.WriteIndentedLine("[FieldOffset(0)]");
            }
            AddNativeTypeNameAttribute(nativeTypeName);

            _outputBuilder.WriteIndented(GetAccessSpecifierName(fieldDecl));
            _outputBuilder.Write(' ');

            if (type is ConstantArrayType constantArrayType)
            {
                if (IsSupportedFixedSizedBufferType(typeName))
                {
                    _outputBuilder.Write("fixed");
                    _outputBuilder.Write(' ');
                    _outputBuilder.Write(typeName);
                    _outputBuilder.Write(' ');
                    _outputBuilder.Write(escapedName);
                    _outputBuilder.Write('[');
                    _outputBuilder.Write(constantArrayType.Size);
                    _outputBuilder.Write(']');
                }
                else
                {
                    _outputBuilder.Write(GetArtificalFixedSizedBufferName(fieldDecl));
                    _outputBuilder.Write(' ');
                    _outputBuilder.Write(escapedName);
                }
            }
            else
            {
                if (fieldDecl.IsBitField)
                {
                    AddDiagnostic(DiagnosticLevel.Warning, "Unsupported field declaration kind: 'BitField'. Generated bindings may be incomplete.", fieldDecl);
                }

                _outputBuilder.Write(typeName);
                _outputBuilder.Write(' ');
                _outputBuilder.Write(escapedName);
            }

            _outputBuilder.WriteLine(';');
        }

        private void VisitFunctionDecl(FunctionDecl functionDecl)
        {
            var name = GetRemappedCursorName(functionDecl);

            StartUsingOutputBuilder(_config.MethodClassName);
            {
                var type = functionDecl.Type;

                if (type is AttributedType attributedType)
                {
                    type = attributedType.ModifiedType;
                }
                var functionType = (FunctionType)type;

                var body = functionDecl.Body;

                if (body is null)
                {
                    _outputBuilder.AddUsingDirective("System.Runtime.InteropServices");

                    _outputBuilder.WriteIndented("[DllImport(libraryPath, EntryPoint = \"");
                    _outputBuilder.Write(name);
                    _outputBuilder.Write("\", CallingConvention = CallingConvention.");
                    _outputBuilder.Write(GetCallingConventionName(functionDecl, functionType.CallConv));
                    _outputBuilder.WriteLine(")]");
                }

                var returnType = functionDecl.ReturnType;
                var returnTypeName = GetRemappedTypeName(functionDecl, returnType, out var nativeTypeName);
                AddNativeTypeNameAttribute(nativeTypeName, attributePrefix: "return: ");

                _outputBuilder.WriteIndented(GetAccessSpecifierName(functionDecl));
                _outputBuilder.Write(' ');
                _outputBuilder.Write("static");
                _outputBuilder.Write(' ');

                if (body is null)
                {
                    _outputBuilder.Write("extern");
                    _outputBuilder.Write(' ');
                }

                if (IsUnsafe(functionDecl))
                {
                    _isMethodClassUnsafe = true;
                }

                _outputBuilder.Write(returnTypeName);
                _outputBuilder.Write(' ');
                _outputBuilder.Write(EscapeAndStripName(name));
                _outputBuilder.Write('(');

                foreach (var parmVarDecl in functionDecl.Parameters)
                {
                    Visit(parmVarDecl);
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
                        Visit(body);
                    }
                    else
                    {
                        _outputBuilder.WriteBlockStart();
                        Visit(body);
                        _outputBuilder.WriteBlockEnd();
                    }
                }

                foreach (var declaration in functionDecl.Decls)
                {
                    Visit(declaration);
                }
            }
            StopUsingOutputBuilder();
        }

        private void VisitIntegerLiteral(IntegerLiteral integerLiteral)
        {
            _outputBuilder.Write(integerLiteral.Value);
        }

        private void VisitNamedDecl(NamedDecl namedDecl)
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
                VisitTypeDecl(typeDecl);
            }
            else if (namedDecl is ValueDecl valueDecl)
            {
                VisitValueDecl(valueDecl);
            }
            else
            {
                AddDiagnostic(DiagnosticLevel.Error, $"Unsupported named declaration: '{namedDecl.KindSpelling}'. Generated bindings may be incomplete.", namedDecl);
            }
        }

        private void VisitParenExpr(ParenExpr parenExpr)
        {
            _outputBuilder.Write('(');
            Visit(parenExpr.SubExpr);
            _outputBuilder.Write(')');
        }

        private void VisitParmVarDecl(ParmVarDecl parmVarDecl)
        {
            var cursorParent = parmVarDecl.CursorParent;

            if (cursorParent is FunctionDecl functionDecl)
            {
                VisitParmVarDecl(parmVarDecl, functionDecl);
            }
            else if (cursorParent is TypedefDecl typedefDecl)
            {
                VisitParmVarDecl(parmVarDecl, typedefDecl);
            }
            else
            {
                AddDiagnostic(DiagnosticLevel.Error, $"Unsupported parameter variable declaration parent: '{cursorParent.KindSpelling}'. Generated bindings may be incomplete.", cursorParent);
            }
        }

        private void VisitParmVarDecl(ParmVarDecl parmVarDecl, FunctionDecl functionDecl)
        {
            var type = parmVarDecl.Type;
            var typeName = GetRemappedTypeName(parmVarDecl, type, out var nativeTypeName);
            AddNativeTypeNameAttribute(nativeTypeName, prefix: "", postfix: " ");

            _outputBuilder.Write(typeName);
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
                _outputBuilder.Write(',');
                _outputBuilder.Write(' ');
            }
        }

        private void VisitParmVarDecl(ParmVarDecl parmVarDecl, TypedefDecl typedefDecl)
        {
            var type = parmVarDecl.Type;
            var typeName = GetRemappedTypeName(parmVarDecl, type, out var nativeTypeName);
            AddNativeTypeNameAttribute(nativeTypeName, prefix: "", postfix: " ");

            _outputBuilder.Write(typeName);
            _outputBuilder.Write(' ');

            var name = GetRemappedCursorName(parmVarDecl);
            _outputBuilder.Write(EscapeName(name));

            var parameters = typedefDecl.CursorChildren.OfType<ParmVarDecl>().ToList();
            var index = parameters.IndexOf(parmVarDecl);
            var lastIndex = parameters.Count - 1;

            if (name.Equals("param"))
            {
                _outputBuilder.Write(index);
            }

            if (index != lastIndex)
            {
                _outputBuilder.Write(',');
                _outputBuilder.Write(' ');
            }
        }

        private void VisitRecordDecl(RecordDecl recordDecl)
        {
            var name = GetRemappedCursorName(recordDecl);

            StartUsingOutputBuilder(name);
            {
                if (recordDecl.IsUnion)
                {
                    _outputBuilder.AddUsingDirective("System.Runtime.InteropServices");
                    _outputBuilder.WriteIndentedLine("[StructLayout(LayoutKind.Explicit)]");
                }

                _outputBuilder.WriteIndented(GetAccessSpecifierName(recordDecl));
                _outputBuilder.Write(' ');

                if (IsUnsafe(recordDecl))
                {
                    _outputBuilder.Write("unsafe");
                    _outputBuilder.Write(' ');
                }

                _outputBuilder.Write("partial struct");
                _outputBuilder.Write(' ');
                _outputBuilder.WriteLine(EscapeName(name));
                _outputBuilder.WriteBlockStart();

                var fields = recordDecl.Fields;

                if (fields.Count != 0)
                {
                    Visit(fields[0]);

                    for (int i = 1; i < fields.Count; i++)
                    {
                        _outputBuilder.WriteLine();
                        Visit(fields[i]);
                    }
                }

                foreach (var declaration in recordDecl.Decls)
                {
                    Visit(declaration);
                }

                foreach (var constantArray in recordDecl.Fields.Where((field) => field.Type is ConstantArrayType))
                {
                    var type = (ConstantArrayType)constantArray.Type;
                    var typeName = GetRemappedTypeName(constantArray, constantArray.Type, out _);

                    if (IsSupportedFixedSizedBufferType(typeName))
                    {
                        continue;
                    }
                    bool isUnsafe = typeName.Contains('*');

                    _outputBuilder.WriteLine();
                    _outputBuilder.WriteIndented(GetAccessSpecifierName(constantArray));
                    _outputBuilder.Write(' ');

                    if (isUnsafe)
                    {
                        _outputBuilder.Write("unsafe");
                        _outputBuilder.Write(' ');
                    }

                    _outputBuilder.Write("partial struct");
                    _outputBuilder.Write(' ');
                    _outputBuilder.WriteLine(GetArtificalFixedSizedBufferName(constantArray));
                    _outputBuilder.WriteBlockStart();

                    for (int i = 0; i < type.Size; i++)
                    {
                        _outputBuilder.WriteIndented("internal");
                        _outputBuilder.Write(' ');
                        _outputBuilder.Write(typeName);
                        _outputBuilder.Write(' ');
                        _outputBuilder.Write('e');
                        _outputBuilder.Write(i);
                        _outputBuilder.WriteLine(';');
                    }

                    _outputBuilder.WriteLine();
                    _outputBuilder.WriteIndented("public");
                    _outputBuilder.Write(' ');

                    if (!isUnsafe)
                    {
                        _outputBuilder.Write("unsafe");
                        _outputBuilder.Write(' ');
                    }

                    _outputBuilder.Write("ref");
                    _outputBuilder.Write(' ');
                    _outputBuilder.Write(typeName);
                    _outputBuilder.Write(' ');
                    _outputBuilder.WriteLine("this[int index]");
                    _outputBuilder.WriteBlockStart();
                    _outputBuilder.WriteIndentedLine("get");
                    _outputBuilder.WriteBlockStart();
                    _outputBuilder.WriteIndented("fixed (");
                    _outputBuilder.Write(typeName);
                    _outputBuilder.WriteLine("* pThis = &e0)");
                    _outputBuilder.WriteBlockStart();
                    _outputBuilder.WriteIndentedLine("return ref pThis[index];");
                    _outputBuilder.WriteBlockEnd();
                    _outputBuilder.WriteBlockEnd();
                    _outputBuilder.WriteBlockEnd();
                    _outputBuilder.WriteBlockEnd();
                }

                _outputBuilder.WriteBlockEnd();
            }
            StopUsingOutputBuilder();
        }

        private void VisitRef(Ref @ref)
        {
            AddDiagnostic(DiagnosticLevel.Error, $"Unsupported reference: '{@ref.KindSpelling}'. Generated bindings may be incomplete.", @ref);
        }

        private void VisitReturnStmt(ReturnStmt returnStmt)
        {
            Debug.Assert(returnStmt.RetValue != null);

            _outputBuilder.WriteIndented("return");
            _outputBuilder.Write(' ');

            Visit(returnStmt.RetValue);
            _outputBuilder.WriteLine(';');
        }

        private void VisitStmt(Stmt stmt)
        {
            if (stmt is CompoundStmt compoundStmt)
            {
                VisitCompoundStmt(compoundStmt);
            }
            else if (stmt is ReturnStmt returnStmt)
            {
                VisitReturnStmt(returnStmt);
            }
            else if (stmt is ValueStmt valueStmt)
            {
                VisitValueStmt(valueStmt);
            }
            else
            {
                AddDiagnostic(DiagnosticLevel.Error, $"Unsupported statement: '{stmt.KindSpelling}'. Generated bindings may be incomplete.", stmt);
            }
        }

        private void VisitTagDecl(TagDecl tagDecl)
        {
            if ((tagDecl.Definition != tagDecl) && (tagDecl.Definition != null))
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
                VisitRecordDecl(recordDecl);
            }
            else if (tagDecl is EnumDecl enumDecl)
            {
                VisitEnumDecl(enumDecl);
            }
            else
            {
                AddDiagnostic(DiagnosticLevel.Error, $"Unsupported tag declaration: '{tagDecl.KindSpelling}'. Generated bindings may be incomplete.", tagDecl);
            }
        }

        private void VisitTypeDecl(TypeDecl typeDecl)
        {
            if (typeDecl is TagDecl tagDecl)
            {
                VisitTagDecl(tagDecl);
            }
            else if (typeDecl is TypedefNameDecl typedefNameDecl)
            {
                VisitTypedefNameDecl(typedefNameDecl);
            }
            else
            {
                AddDiagnostic(DiagnosticLevel.Error, $"Unsupported type declaration: '{typeDecl.KindSpelling}'. Generated bindings may be incomplete.", typeDecl);
            }
        }

        private void VisitTypedefNameDecl(TypedefNameDecl typedefNameDecl)
        {
            if (typedefNameDecl is TypedefDecl typedefDecl)
            {
                VisitTypedefDecl(typedefDecl, typedefDecl.UnderlyingType);
            }
            else
            {
                AddDiagnostic(DiagnosticLevel.Error, $"Unsupported typedef name declaration: '{typedefNameDecl.KindSpelling}'. Generated bindings may be incomplete.", typedefNameDecl);
            }
        }

        private void VisitTypedefDecl(TypedefDecl typedefDecl, Type underlyingType)
        {
            if (underlyingType is ElaboratedType elaboratedType)
            {
                VisitTypedefDecl(typedefDecl, elaboratedType.NamedType);
            }
            else if (underlyingType is PointerType pointerType)
            {
                VisitTypedefDeclForPointeeType(typedefDecl, pointerType.PointeeType);
            }
            else if (underlyingType is TypedefType typedefType)
            {
                VisitTypedefDecl(typedefDecl, typedefType.Decl.UnderlyingType);
            }
            else if (!(underlyingType is BuiltinType) && !(underlyingType is IncompleteArrayType) && !(underlyingType is TagType))
            {
                AddDiagnostic(DiagnosticLevel.Error, $"Unsupported underlying type: '{underlyingType.KindSpelling}'. Generating bindings may be incomplete.", typedefDecl);
            }
            return;
        }

        private void VisitTypedefDeclForPointeeType(TypedefDecl typedefDecl, Type pointeeType)
        {
            if (pointeeType is ElaboratedType elaboratedType)
            {
                VisitTypedefDeclForPointeeType(typedefDecl, elaboratedType.NamedType);
            }
            else if (pointeeType is FunctionProtoType functionProtoType)
            {
                var name = GetRemappedCursorName(typedefDecl);

                StartUsingOutputBuilder(name);
                {
                    _outputBuilder.AddUsingDirective("System.Runtime.InteropServices");

                    _outputBuilder.WriteIndented("[UnmanagedFunctionPointer(CallingConvention.");
                    _outputBuilder.Write(GetCallingConventionName(typedefDecl, functionProtoType.CallConv));
                    _outputBuilder.WriteLine(")]");

                    var returnType = functionProtoType.ReturnType;
                    var returnTypeName = GetRemappedTypeName(typedefDecl, returnType, out var nativeTypeName);
                    AddNativeTypeNameAttribute(nativeTypeName, attributePrefix: "return: ");

                    _outputBuilder.WriteIndented(GetAccessSpecifierName(typedefDecl));
                    _outputBuilder.Write(' ');

                    if (IsUnsafe(typedefDecl, functionProtoType))
                    {
                        _outputBuilder.Write("unsafe");
                        _outputBuilder.Write(' ');
                    }

                    _outputBuilder.Write("delegate");
                    _outputBuilder.Write(' ');
                    _outputBuilder.Write(returnTypeName);
                    _outputBuilder.Write(' ');
                    _outputBuilder.Write(EscapeName(name));
                    _outputBuilder.Write('(');

                    foreach (var parmVarDecl in typedefDecl.CursorChildren.OfType<ParmVarDecl>())
                    {
                        Visit(parmVarDecl);
                    }

                    _outputBuilder.WriteLine(");");
                }
                StopUsingOutputBuilder();
            }
            else if (pointeeType is PointerType pointerType)
            {
                VisitTypedefDeclForPointeeType(typedefDecl, pointerType.PointeeType);
            }
            else if (pointeeType is TypedefType typedefType)
            {
                VisitTypedefDeclForPointeeType(typedefDecl, typedefType.Decl.UnderlyingType);
            }
            else if (!(pointeeType is BuiltinType) && !(pointeeType is TagType))
            {
                AddDiagnostic(DiagnosticLevel.Error, $"Unsupported pointee type: '{pointeeType.KindSpelling}'. Generating bindings may be incomplete.", typedefDecl);
            }
         }

        private void VisitUnaryOperator(UnaryOperator unaryOperator)
        {
            if (unaryOperator.IsPrefix)
            {
                _outputBuilder.Write(unaryOperator.Opcode);
                Visit(unaryOperator.SubExpr);
            }
            else
            {
                Visit(unaryOperator.SubExpr);
                _outputBuilder.Write(unaryOperator.Opcode);
            }
        }

        private void VisitValueDecl(ValueDecl valueDecl)
        {
            if (valueDecl is DeclaratorDecl declaratorDecl)
            {
                VisitDeclaratorDecl(declaratorDecl);
            }
            else if (valueDecl is EnumConstantDecl enumConstantDecl)
            {
                VisitEnumConstantDecl(enumConstantDecl);
            }
            else
            {
                AddDiagnostic(DiagnosticLevel.Error, $"Unsupported value declaration: '{valueDecl.KindSpelling}'. Generated bindings may be incomplete.", valueDecl);
            }
        }

        private void VisitValueStmt(ValueStmt valueStmt)
        {
            if (valueStmt is Expr expr)
            {
                VisitExpr(expr);
            }
            else
            {
                AddDiagnostic(DiagnosticLevel.Error, $"Unsupported value statement: '{valueStmt.KindSpelling}'. Generated bindings may be incomplete.", valueStmt);
            }
        }

        private void VisitVarDecl(VarDecl varDecl)
        {
            if (varDecl is ParmVarDecl parmVarDecl)
            {
                VisitParmVarDecl(parmVarDecl);
            }
            else
            {
                var name = GetRemappedCursorName(varDecl);

                StartUsingOutputBuilder(_config.MethodClassName);
                {
                    var type = varDecl.Type;
                    var typeName = GetRemappedTypeName(varDecl, type, out var nativeTypeName);
                    AddNativeTypeNameAttribute(nativeTypeName, prefix: "// ");

                    _outputBuilder.WriteIndented("// public static extern");
                    _outputBuilder.Write(' ');
                    _outputBuilder.Write(typeName);
                    _outputBuilder.Write(' ');
                    _outputBuilder.Write(EscapeName(name));
                    _outputBuilder.WriteLine(';');
                }
                StopUsingOutputBuilder();
            }
        }

        private void VisitUnexposedDecl(Decl unexposedDecl)
        {
            Debug.Assert(unexposedDecl.Kind == CXCursorKind.CXCursor_UnexposedDecl);

            foreach (var decl in unexposedDecl.CursorChildren.OfType<Decl>())
            {
                Visit(decl);
            }
        }

        private void VisitUnexposedExpr(Expr unexposedExpr)
        {
            Debug.Assert(unexposedExpr.Kind == CXCursorKind.CXCursor_UnexposedExpr);

            foreach (var stmt in unexposedExpr.Children)
            {
                Visit(stmt);
            }
        }
    }
}
