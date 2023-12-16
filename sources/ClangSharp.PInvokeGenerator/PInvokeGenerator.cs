// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using ClangSharp.Abstractions;
using ClangSharp.CSharp;
using ClangSharp.Interop;
using ClangSharp.XML;
using static ClangSharp.Interop.CX_AttrKind;
using static ClangSharp.Interop.CXBinaryOperatorKind;
using static ClangSharp.Interop.CX_CXXAccessSpecifier;
using static ClangSharp.Interop.CX_StmtClass;
using static ClangSharp.Interop.CX_UnaryExprOrTypeTrait;
using static ClangSharp.Interop.CXUnaryOperatorKind;
using static ClangSharp.Interop.CXCallingConv;
using static ClangSharp.Interop.CXDiagnosticSeverity;
using static ClangSharp.Interop.CXEvalResultKind;
using static ClangSharp.Interop.CXTemplateArgumentKind;
using static ClangSharp.Interop.CXTranslationUnit_Flags;
using static ClangSharp.Interop.CXTypeKind;

namespace ClangSharp;

public sealed partial class PInvokeGenerator : IDisposable
{
    private const int DefaultStreamWriterBufferSize = 1024;

    private static readonly Encoding s_defaultStreamWriterEncoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: true);
    private static readonly string[] s_doubleColonSeparator = ["::"];
    private static readonly char[] s_doubleQuoteSeparator = ['"'];

    private const string ExpectedClangVersion = "version 17.0";
    private const string ExpectedClangSharpVersion = "version 17.0";

    private readonly CXIndex _index;
    private readonly OutputBuilderFactory _outputBuilderFactory;
    private readonly Func<string, Stream> _outputStreamFactory;
    private readonly StringBuilder _fileContentsBuilder;
    private readonly HashSet<string> _visitedFiles;
    private readonly List<Diagnostic> _diagnostics;
    private readonly LinkedList<(Cursor Cursor, object? UserData)> _context;
    private readonly Dictionary<string, Guid> _uuidsToGenerate;
    private readonly HashSet<string> _generatedUuids;
    private readonly PInvokeGeneratorConfiguration _config;
    private readonly Dictionary<NamedDecl, string> _cursorNames;
    private readonly Dictionary<(NamedDecl namedDecl, bool truncateForFunctionParameters), string> _cursorQualifiedNames;
    private readonly Dictionary<(Cursor? cursor, Cursor? context, Type type), (string typeName, string nativeTypeName)> _typeNames;
    private readonly Dictionary<string, HashSet<string>> _allValidNameRemappings;
    private readonly Dictionary<string, HashSet<string>> _traversedValidNameRemappings;
    private readonly Dictionary<CXXMethodDecl, uint> _overloadIndices;
    private readonly Dictionary<Cursor, uint> _isExcluded;
    private readonly Dictionary<string, bool> _topLevelClassHasGuidMember;
    private readonly Dictionary<string, bool> _topLevelClassIsUnsafe;
    private readonly Dictionary<string, HashSet<string>> _topLevelClassUsings;
    private readonly Dictionary<string, List<string>> _topLevelClassAttributes;
    private readonly HashSet<string> _topLevelClassNames;
    private readonly HashSet<string> _usedRemappings;
    private readonly string _placeholderMacroType;

    private string _filePath;
    private string[] _clangCommandLineArgs;
    private CXTranslationUnit_Flags _translationFlags;
    private string? _currentClass;
    private string? _currentNamespace;
    private IOutputBuilder? _outputBuilder;
    private CSharpOutputBuilder? _testOutputBuilder;
    private CSharpOutputBuilder? _stmtOutputBuilder;
    private CSharpOutputBuilder? _testStmtOutputBuilder;
    private int _stmtOutputBuilderUsers;
    private int _testStmtOutputBuilderUsers;
    private int _outputBuilderUsers;
    private CXXRecordDecl? _cxxRecordDeclContext;
    private bool _disposed;

    public PInvokeGenerator(PInvokeGeneratorConfiguration config, Func<string, Stream>? outputStreamFactory = null)
    {
        ArgumentNullException.ThrowIfNull(config);

        var clangVersion = string.Empty;

        try
        {
            clangVersion = clang.getClangVersion().ToString();
        }
        catch
        {
            Console.WriteLine();
            Console.WriteLine("*****IMPORTANT*****");
            Console.WriteLine($"Failed to resolve libClang.");
            Console.WriteLine("If you are running as a dotnet tool, you may need to manually copy the appropriate DLLs from NuGet due to limitations in the dotnet tool support. Please see https://github.com/dotnet/clangsharp for more details.");
            Console.WriteLine("*****IMPORTANT*****");
            Console.WriteLine();

            throw;
        }

        if (clangVersion.Contains(ExpectedClangVersion, StringComparison.Ordinal))
        {
            var clangSharpVersion = string.Empty;

            try
            {
                clangSharpVersion = clangsharp.getVersion().ToString();
            }
            catch
            {
                Console.WriteLine();
                Console.WriteLine("*****IMPORTANT*****");
                Console.WriteLine($"Failed to resolve libClangSharp.");
                Console.WriteLine("If you are running as a dotnet tool, you may need to manually copy the appropriate DLLs from NuGet due to limitations in the dotnet tool support. Please see https://github.com/dotnet/clangsharp for more details.");
                Console.WriteLine("*****IMPORTANT*****");
                Console.WriteLine();

                throw;
            }

            if (!clangSharpVersion.Contains(ExpectedClangSharpVersion, StringComparison.Ordinal))
            {
                throw new InvalidOperationException($"Invalid libClang version. Returned string '{clangSharpVersion}' does not contain '{ExpectedClangSharpVersion}'");
            }

            _config = config;
            _index = CXIndex.Create();
            _outputBuilderFactory = new OutputBuilderFactory(this);
            _outputStreamFactory = outputStreamFactory ?? ((path) => {
                var directoryPath = Path.GetDirectoryName(path) ?? "";
                _ = Directory.CreateDirectory(directoryPath);
                return new FileStream(path, FileMode.Create);
            });
            _fileContentsBuilder = new StringBuilder();
            _visitedFiles = [];
            _diagnostics = [];
            _context = new LinkedList<(Cursor, object?)>();
            _uuidsToGenerate = [];
            _generatedUuids = [];
            _cursorNames = [];
            _cursorQualifiedNames = new Dictionary<(NamedDecl, bool), string>();
            _typeNames = new Dictionary<(Cursor?, Cursor?, Type), (string, string)>();
            _allValidNameRemappings = new Dictionary<string, HashSet<string>>() {
                ["intptr_t"] = ["IntPtr", "nint"],
                ["ptrdiff_t"] = ["IntPtr", "nint"],
                ["size_t"] = ["UIntPtr", "nuint"],
                ["uintptr_t"] = ["UIntPtr", "nuint"],
                ["_GUID"] = ["Guid"],
            };
            _traversedValidNameRemappings = [];
            _overloadIndices = [];
            _isExcluded = [];
            _topLevelClassHasGuidMember = [];
            _topLevelClassIsUnsafe = [];
            _topLevelClassNames = [];
            _topLevelClassAttributes = [];
            _topLevelClassUsings = [];
            _usedRemappings = [];
            _filePath = "";
            _clangCommandLineArgs = [];
            _placeholderMacroType = GetPlaceholderMacroType();
        }
        else
        {
            throw new InvalidOperationException($"Invalid libClang version. Returned string '{clangVersion}' does not contain '{ExpectedClangVersion}'");
        }
    }

    ~PInvokeGenerator()
    {
        Dispose(isDisposing: false);
    }

    public PInvokeGeneratorConfiguration Config => _config;

    public (Cursor Cursor, object? UserData) CurrentContext
    {
        get
        {
            var currentContext = _context.Last;
            Debug.Assert(currentContext is not null);
            return currentContext.Value;
        }
    }

    public IReadOnlyList<Diagnostic> Diagnostics => _diagnostics;

    public string FilePath => _filePath;

    public CXIndex IndexHandle => _index;

    public (Cursor Cursor, object? UserData) PreviousContext
    {
        get
        {
            var previousContext = _context.Last;
            Debug.Assert(previousContext is not null);

            previousContext = previousContext.Previous;
            Debug.Assert(previousContext is not null);

            return previousContext.Value;
        }
    }

    public void Close()
    {
        Stream? stream = null;
        Stream? testStream = null;

        var methodClassOutputBuilders = new Dictionary<string, IOutputBuilder>();
        var methodClassTestOutputBuilders = new Dictionary<string, IOutputBuilder>();
        var emitNamespaceDeclaration = true;
        var leaveStreamOpen = false;

        foreach (var foundUuid in _uuidsToGenerate)
        {
            var iidName = foundUuid.Key;

            if (_generatedUuids.Contains(iidName) || _config.ExcludedNames.Contains(iidName))
            {
                continue;
            }

            StartUsingOutputBuilder(GetClass(iidName));

            Debug.Assert(_outputBuilder is not null);
            _outputBuilder.WriteIid(iidName, foundUuid.Value);

            StopUsingOutputBuilder();
        }

        if (!_config.GenerateMultipleFiles)
        {
            var outputPath = _config.OutputLocation;
            stream = _outputStreamFactory(outputPath);

            leaveStreamOpen = true;

            var usingDirectives = new SortedSet<string>(StringComparer.Ordinal);
            var staticUsingDirectives = new SortedSet<string>(StringComparer.Ordinal);
            var hasAnyContents = false;
            var testHasAnyContents = false;

            foreach (var outputBuilder in _outputBuilderFactory.OutputBuilders)
            {
                if (outputBuilder is CSharpOutputBuilder csharpOutputBuilder)
                {
                    foreach (var usingDirective in csharpOutputBuilder.UsingDirectives)
                    {
                        _ = usingDirectives.Add(usingDirective);
                    }

                    foreach (var staticUsingDirective in csharpOutputBuilder.StaticUsingDirectives)
                    {
                        _ = staticUsingDirectives.Add(staticUsingDirective);
                    }

                    if (csharpOutputBuilder.IsTestOutput)
                    {
                        testHasAnyContents |= csharpOutputBuilder.Contents.Any();
                    }
                    else
                    {
                        hasAnyContents |= csharpOutputBuilder.Contents.Any();
                    }
                }
                else if (outputBuilder is XmlOutputBuilder xmlOutputBuilder)
                {
                    Debug.Assert(!xmlOutputBuilder.IsTestOutput);
                    hasAnyContents |= xmlOutputBuilder.Contents.Any();
                }
            }

            foreach (var staticUsingDirective in staticUsingDirectives)
            {
                _ = usingDirectives.Add(staticUsingDirective);
            }

            if (hasAnyContents)
            {
                using var sw = new StreamWriter(stream, s_defaultStreamWriterEncoding, DefaultStreamWriterBufferSize, leaveStreamOpen);
                sw.NewLine = "\n";

                if (_config.OutputMode == PInvokeGeneratorOutputMode.CSharp)
                {
                    if (!string.IsNullOrEmpty(_config.HeaderText))
                    {
                        sw.WriteLine(_config.HeaderText);
                    }

                    if (usingDirectives.Count != 0)
                    {
                        foreach (var usingDirective in usingDirectives)
                        {
                            sw.Write("using ");
                            sw.Write(usingDirective);
                            sw.WriteLine(';');
                        }

                        sw.WriteLine();
                    }
                }
                else if (_config.OutputMode == PInvokeGeneratorOutputMode.Xml)
                {
                    sw.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\" ?>");
                    sw.WriteLine("<bindings>");

                    if (!string.IsNullOrEmpty(_config.HeaderText))
                    {
                        sw.WriteLine("  <comment>");

                        if (!string.IsNullOrEmpty(_config.HeaderText))
                        {
                            foreach (var ln in _config.HeaderText.Split('\n'))
                            {
                                sw.Write("    ");
                                sw.WriteLine(ln);
                            }
                        }

                        sw.WriteLine("  </comment>");
                    }
                }
            }

            if (testHasAnyContents)
            {
                var testOutputPath = _config.TestOutputLocation;
                testStream = _outputStreamFactory(testOutputPath);

                using var sw = new StreamWriter(testStream, s_defaultStreamWriterEncoding, DefaultStreamWriterBufferSize, leaveStreamOpen);
                sw.NewLine = "\n";

                if (_config.OutputMode == PInvokeGeneratorOutputMode.CSharp)
                {
                    if (!string.IsNullOrEmpty(_config.HeaderText))
                    {
                        sw.WriteLine(_config.HeaderText);
                    }

                    if (usingDirectives.Count != 0)
                    {
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
            else
            {
                Debug.Assert(testStream is null);
            }
        }

        foreach (var outputBuilder in _outputBuilderFactory.OutputBuilders)
        {
            var outputPath = outputBuilder.IsTestOutput ? _config.TestOutputLocation : _config.OutputLocation;
            var isMethodClass = _topLevelClassNames.Contains(outputBuilder.Name);

            if (outputBuilder is CSharpOutputBuilder csharpOutputBuilder)
            {
                if (!csharpOutputBuilder.Contents.Any())
                {
                    continue;
                }
            }
            else if (outputBuilder is XmlOutputBuilder xmlOutputBuilder)
            {
                if (!xmlOutputBuilder.Contents.Any())
                {
                    continue;
                }
            }

            if (_config.GenerateMultipleFiles)
            {
                outputPath = Path.Combine(outputPath, $"{outputBuilder.Name}{outputBuilder.Extension}");
                stream = _outputStreamFactory(outputPath);
                emitNamespaceDeclaration = true;
            }
            else if (isMethodClass)
            {
                if (outputBuilder.IsTestOutput)
                {
                    methodClassTestOutputBuilders.Add(outputBuilder.Name, outputBuilder);
                }
                else
                {
                    methodClassOutputBuilders.Add(outputBuilder.Name, outputBuilder);
                }
                continue;
            }

            Debug.Assert(stream is not null);
            CloseOutputBuilder(stream, outputBuilder, isMethodClass, leaveStreamOpen, emitNamespaceDeclaration);

            if (testStream is not null)
            {
                CloseOutputBuilder(testStream, outputBuilder, isMethodClass, leaveStreamOpen, emitNamespaceDeclaration);
            }

            emitNamespaceDeclaration = false;

            if (_config.GenerateMultipleFiles)
            {
                stream = null;
                Debug.Assert(testStream is null);
            }
        }

        if (_config.GenerateHelperTypes && (_config.OutputMode == PInvokeGeneratorOutputMode.CSharp))
        {
            if (_config.GenerateMultipleFiles)
            {
                Debug.Assert(stream is null);
                Debug.Assert(leaveStreamOpen is false);
            }
            else
            {
                Debug.Assert(stream is not null);
                Debug.Assert(leaveStreamOpen is true);
            }

            GenerateDisableRuntimeMarshallingAttribute(this, stream, leaveStreamOpen);
            GenerateNativeBitfieldAttribute(this, stream, leaveStreamOpen);
            GenerateNativeInheritanceAttribute(this, stream, leaveStreamOpen);
            GenerateNativeTypeNameAttribute(this, stream, leaveStreamOpen);
            GenerateSetsLastSystemErrorAttribute(this, stream, leaveStreamOpen);
            GenerateVtblIndexAttribute(this, stream, leaveStreamOpen);
            GenerateTransparentStructs(this, stream, leaveStreamOpen);
        }

        if (leaveStreamOpen && _outputBuilderFactory.OutputBuilders.Any())
        {
            Debug.Assert(stream is not null);

            foreach (var entry in methodClassOutputBuilders)
            {
                CloseOutputBuilder(stream, entry.Value, isMethodClass: true, leaveStreamOpen, emitNamespaceDeclaration);
            }

            foreach (var entry in methodClassTestOutputBuilders)
            {
                CloseOutputBuilder(testStream ?? stream, entry.Value, isMethodClass: true, leaveStreamOpen, emitNamespaceDeclaration);
            }

            using var sw = new StreamWriter(stream, s_defaultStreamWriterEncoding, DefaultStreamWriterBufferSize, leaveStreamOpen);
            sw.NewLine = "\n";

            if (_config.OutputMode == PInvokeGeneratorOutputMode.CSharp)
            {
                sw.WriteLine('}');
            }
            else if (_config.OutputMode == PInvokeGeneratorOutputMode.Xml)
            {
                sw.WriteLine("  </namespace>");
                sw.WriteLine("</bindings>");
            }

            if (testStream is not null)
            {
                using var tsw = new StreamWriter(testStream, s_defaultStreamWriterEncoding, DefaultStreamWriterBufferSize, leaveStreamOpen);
                tsw.NewLine = "\n";

                if (_config.OutputMode == PInvokeGeneratorOutputMode.CSharp)
                {
                    tsw.WriteLine('}');
                }
            }
        }

        _context.Clear();
        _diagnostics.Clear();
        _ = _fileContentsBuilder.Clear();
        _generatedUuids.Clear();
        _outputBuilderFactory.Clear();
        _uuidsToGenerate.Clear();
        _visitedFiles.Clear();

        static void GenerateDisableRuntimeMarshallingAttribute(PInvokeGenerator generator, Stream? stream, bool leaveStreamOpen)
        {
            var config = generator.Config;

            if (!config.GenerateDisableRuntimeMarshalling)
            {
                return;
            }

            if (stream is null)
            {
                var outputPath = Path.Combine(config.OutputLocation, "DisableRuntimeMarshalling.cs");
                stream = generator._outputStreamFactory(outputPath);
            }

            using var sw = new StreamWriter(stream, s_defaultStreamWriterEncoding, DefaultStreamWriterBufferSize, leaveStreamOpen);
            sw.NewLine = "\n";

            if (!string.IsNullOrEmpty(config.HeaderText))
            {
                sw.WriteLine(config.HeaderText);
            }

            sw.WriteLine("using System.Runtime.CompilerServices;");
            sw.WriteLine();
            sw.WriteLine("[assembly: DisableRuntimeMarshalling]");
            
            if (!leaveStreamOpen)
            {
                stream = null;
            }
        }

        static void GenerateNativeBitfieldAttribute(PInvokeGenerator generator, Stream? stream, bool leaveStreamOpen)
        {
            var config = generator.Config;

            if (!config.GenerateNativeBitfieldAttribute)
            {
                return;
            }

            if (stream is null)
            {
                var outputPath = Path.Combine(config.OutputLocation, "NativeBitfieldAttribute.cs");
                stream = generator._outputStreamFactory(outputPath);
            }

            using var sw = new StreamWriter(stream, s_defaultStreamWriterEncoding, DefaultStreamWriterBufferSize, leaveStreamOpen);
            sw.NewLine = "\n";

            if (!string.IsNullOrEmpty(config.HeaderText))
            {
                sw.WriteLine(config.HeaderText);
            }

            var indentString = "    ";

            sw.WriteLine("using System;");
            sw.WriteLine("using System.Diagnostics;");
            sw.WriteLine();

            sw.Write("namespace ");
            sw.Write(generator.GetNamespace("NativeBitfieldAttribute"));

            if (generator.Config.GenerateFileScopedNamespaces)
            {
                sw.WriteLine(';');
                sw.WriteLine();
                indentString = "";
            }
            else
            {
                sw.WriteLine();
                sw.WriteLine('{');
            }

            sw.Write(indentString);
            sw.WriteLine("/// <summary>Defines the layout of a bitfield as it was used in the native signature.</summary>");
            sw.Write(indentString);
            sw.WriteLine("[AttributeUsage(AttributeTargets.Field, AllowMultiple = true, Inherited = true)]");
            sw.Write(indentString);
            sw.WriteLine("[Conditional(\"DEBUG\")]");
            sw.Write(indentString);
            sw.WriteLine("internal sealed partial class NativeBitfieldAttribute : Attribute");
            sw.Write(indentString);
            sw.WriteLine('{');
            sw.Write(indentString);
            sw.WriteLine("    private readonly string _name;");
            sw.WriteLine("    private readonly int _offset;");
            sw.WriteLine("    private readonly int _length;");
            sw.WriteLine();
            sw.Write(indentString);
            sw.WriteLine("    /// <summary>Initializes a new instance of the <see cref=\"NativeBitfieldAttribute\" /> class.</summary>");
            sw.Write(indentString);
            sw.WriteLine("    /// <param name=\"name\">The name of the bitfield that was used in the native signature.</param>");
            sw.WriteLine("    /// <param name=\"offset\">The offset of the bitfield that was used in the native signature.</param>");
            sw.WriteLine("    /// <param name=\"length\">The length of the bitfield that was used in the native signature.</param>");
            sw.Write(indentString);
            sw.WriteLine("    public NativeBitfieldAttribute(string name, int offset, int length)");
            sw.Write(indentString);
            sw.WriteLine("    {");
            sw.Write(indentString);
            sw.WriteLine("        _name = name;");
            sw.WriteLine("        _offset = offset;");
            sw.WriteLine("        _length = length;");
            sw.Write(indentString);
            sw.WriteLine("    }");
            sw.WriteLine();
            sw.Write(indentString);
            sw.WriteLine("    /// <summary>Gets the length of the bitfield that was used in the native signature.</summary>");
            sw.Write(indentString);
            sw.WriteLine("    public int Length => _length;");
            sw.WriteLine();
            sw.Write(indentString);
            sw.WriteLine("    /// <summary>Gets the name of the bitfield that was used in the native signature.</summary>");
            sw.Write(indentString);
            sw.WriteLine("    public string Name => _name;");
            sw.WriteLine();
            sw.Write(indentString);
            sw.WriteLine("    /// <summary>Gets the offset of the bitfield that was used in the native signature.</summary>");
            sw.Write(indentString);
            sw.WriteLine("    public int Offset => _offset;");
            sw.Write(indentString);
            sw.WriteLine('}');

            if (!generator.Config.GenerateFileScopedNamespaces)
            {
                sw.WriteLine('}');
            }

            if (!leaveStreamOpen)
            {
                stream = null;
            }
        }

        static void GenerateNativeInheritanceAttribute(PInvokeGenerator generator, Stream? stream, bool leaveStreamOpen)
        {
            var config = generator.Config;

            if (!config.GenerateNativeInheritanceAttribute)
            {
                return;
            }

            if (stream is null)
            {
                var outputPath = Path.Combine(config.OutputLocation, "NativeInheritanceAttribute.cs");
                stream = generator._outputStreamFactory(outputPath);
            }

            using var sw = new StreamWriter(stream, s_defaultStreamWriterEncoding, DefaultStreamWriterBufferSize, leaveStreamOpen);
            sw.NewLine = "\n";

            if (!string.IsNullOrEmpty(config.HeaderText))
            {
                sw.WriteLine(config.HeaderText);
            }

            var indentString = "    ";

            sw.WriteLine("using System;");
            sw.WriteLine("using System.Diagnostics;");
            sw.WriteLine();

            sw.Write("namespace ");
            sw.Write(generator.GetNamespace("NativeInheritanceAttribute"));

            if (generator.Config.GenerateFileScopedNamespaces)
            {
                sw.WriteLine(';');
                sw.WriteLine();
                indentString = "";
            }
            else
            {
                sw.WriteLine();
                sw.WriteLine('{');
            }

            sw.Write(indentString);
            sw.WriteLine("/// <summary>Defines the base type of a struct as it was in the native signature.</summary>");
            sw.Write(indentString);
            sw.WriteLine("[AttributeUsage(AttributeTargets.Struct, AllowMultiple = false, Inherited = true)]");
            sw.Write(indentString);
            sw.WriteLine("[Conditional(\"DEBUG\")]");
            sw.Write(indentString);
            sw.WriteLine("internal sealed partial class NativeInheritanceAttribute : Attribute");
            sw.Write(indentString);
            sw.WriteLine('{');
            sw.Write(indentString);
            sw.WriteLine("    private readonly string _name;");
            sw.WriteLine();
            sw.Write(indentString);
            sw.WriteLine("    /// <summary>Initializes a new instance of the <see cref=\"NativeInheritanceAttribute\" /> class.</summary>");
            sw.Write(indentString);
            sw.WriteLine("    /// <param name=\"name\">The name of the base type that was inherited from in the native signature.</param>");
            sw.Write(indentString);
            sw.WriteLine("    public NativeInheritanceAttribute(string name)");
            sw.Write(indentString);
            sw.WriteLine("    {");
            sw.Write(indentString);
            sw.WriteLine("        _name = name;");
            sw.Write(indentString);
            sw.WriteLine("    }");
            sw.WriteLine();
            sw.Write(indentString);
            sw.WriteLine("    /// <summary>Gets the name of the base type that was inherited from in the native signature.</summary>");
            sw.Write(indentString);
            sw.WriteLine("    public string Name => _name;");
            sw.Write(indentString);
            sw.WriteLine('}');

            if (!generator.Config.GenerateFileScopedNamespaces)
            {
                sw.WriteLine('}');
            }

            if (!leaveStreamOpen)
            {
                stream = null;
            }
        }

        static void GenerateNativeTypeNameAttribute(PInvokeGenerator generator, Stream? stream, bool leaveStreamOpen)
        {
            var config = generator.Config;

            if (stream is null)
            {
                var outputPath = Path.Combine(config.OutputLocation, "NativeTypeNameAttribute.cs");
                stream = generator._outputStreamFactory(outputPath);
            }

            using var sw = new StreamWriter(stream, s_defaultStreamWriterEncoding, DefaultStreamWriterBufferSize, leaveStreamOpen);
            sw.NewLine = "\n";

            if (!string.IsNullOrEmpty(config.HeaderText))
            {
                sw.WriteLine(config.HeaderText);
            }

            var indentString = "    ";

            sw.WriteLine("using System;");
            sw.WriteLine("using System.Diagnostics;");
            sw.WriteLine();

            sw.Write("namespace ");
            sw.Write(generator.GetNamespace("NativeTypeNameAttribute"));

            if (generator.Config.GenerateFileScopedNamespaces)
            {
                sw.WriteLine(';');
                sw.WriteLine();
                indentString = "";
            }
            else
            {
                sw.WriteLine();
                sw.WriteLine('{');
            }

            sw.Write(indentString);
            sw.WriteLine("/// <summary>Defines the type of a member as it was used in the native signature.</summary>");
            sw.Write(indentString);
            sw.WriteLine("[AttributeUsage(AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.ReturnValue, AllowMultiple = false, Inherited = true)]");
            sw.Write(indentString);
            sw.WriteLine("[Conditional(\"DEBUG\")]");
            sw.Write(indentString);
            sw.WriteLine("internal sealed partial class NativeTypeNameAttribute : Attribute");
            sw.Write(indentString);
            sw.WriteLine('{');
            sw.Write(indentString);
            sw.WriteLine("    private readonly string _name;");
            sw.WriteLine();
            sw.Write(indentString);
            sw.WriteLine("    /// <summary>Initializes a new instance of the <see cref=\"NativeTypeNameAttribute\" /> class.</summary>");
            sw.Write(indentString);
            sw.WriteLine("    /// <param name=\"name\">The name of the type that was used in the native signature.</param>");
            sw.Write(indentString);
            sw.WriteLine("    public NativeTypeNameAttribute(string name)");
            sw.Write(indentString);
            sw.WriteLine("    {");
            sw.Write(indentString);
            sw.WriteLine("        _name = name;");
            sw.Write(indentString);
            sw.WriteLine("    }");
            sw.WriteLine();
            sw.Write(indentString);
            sw.WriteLine("    /// <summary>Gets the name of the type that was used in the native signature.</summary>");
            sw.Write(indentString);
            sw.WriteLine("    public string Name => _name;");
            sw.Write(indentString);
            sw.WriteLine('}');

            if (!generator.Config.GenerateFileScopedNamespaces)
            {
                sw.WriteLine('}');
            }

            if (!leaveStreamOpen)
            {
                stream = null;
            }
        }

        static void GenerateSetsLastSystemErrorAttribute(PInvokeGenerator generator, Stream? stream, bool leaveStreamOpen)
        {
            var config = generator.Config;

            if (!config.GenerateSetsLastSystemErrorAttribute)
            {
                return;
            }

            if (stream is null)
            {
                Debug.Assert(stream is null);
                var outputPath = Path.Combine(config.OutputLocation, "SetsLastSystemErrorAttribute.cs");
                stream = generator._outputStreamFactory(outputPath);
            }

            using var sw = new StreamWriter(stream, s_defaultStreamWriterEncoding, DefaultStreamWriterBufferSize, leaveStreamOpen);
            sw.NewLine = "\n";

            if (!string.IsNullOrEmpty(config.HeaderText))
            {
                sw.WriteLine(config.HeaderText);
            }

            var indentString = "    ";

            sw.WriteLine("using System;");
            sw.WriteLine("using System.Diagnostics;");
            sw.WriteLine("using System.Runtime.InteropServices;");
            sw.WriteLine();

            sw.Write("namespace ");
            sw.Write(generator.GetNamespace("SetsLastSystemErrorAttribute"));

            if (generator.Config.GenerateFileScopedNamespaces)
            {
                sw.WriteLine(';');
                sw.WriteLine();
                indentString = "";
            }
            else
            {
                sw.WriteLine();
                sw.WriteLine('{');
            }

            sw.Write(indentString);
            sw.WriteLine("/// <summary>Specifies that the given method sets the last system error and it can be retrieved via <see cref=\"Marshal.GetLastSystemError\" />.</summary>");
            sw.Write(indentString);
            sw.WriteLine("[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]");
            sw.Write(indentString);
            sw.WriteLine("[Conditional(\"DEBUG\")]");
            sw.Write(indentString);
            sw.WriteLine("internal sealed partial class SetsLastSystemErrorAttribute : Attribute");
            sw.Write(indentString);
            sw.WriteLine('{');
            sw.Write(indentString);
            sw.WriteLine("    /// <summary>Initializes a new instance of the <see cref=\"SetsLastSystemErrorAttribute\" /> class.</summary>");
            sw.Write(indentString);
            sw.WriteLine("    public SetsLastSystemErrorAttribute()");
            sw.Write(indentString);
            sw.WriteLine("    {");
            sw.Write(indentString);
            sw.WriteLine("    }");
            sw.Write(indentString);
            sw.WriteLine('}');

            if (!generator.Config.GenerateFileScopedNamespaces)
            {
                sw.WriteLine('}');
            }

            if (!leaveStreamOpen)
            {
                stream = null;
            }
        }

        static void GenerateVtblIndexAttribute(PInvokeGenerator generator, Stream? stream, bool leaveStreamOpen)
        {
            var config = generator.Config;

            if (!config.GenerateVtblIndexAttribute)
            {
                return;
            }

            if (stream is null)
            {
                Debug.Assert(stream is null);
                var outputPath = Path.Combine(config.OutputLocation, "VtblIndexAttribute.cs");
                stream = generator._outputStreamFactory(outputPath);
            }

            using var sw = new StreamWriter(stream, s_defaultStreamWriterEncoding, DefaultStreamWriterBufferSize, leaveStreamOpen);
            sw.NewLine = "\n";

            if (!string.IsNullOrEmpty(config.HeaderText))
            {
                sw.WriteLine(config.HeaderText);
            }

            var indentString = "    ";

            sw.WriteLine("using System;");
            sw.WriteLine("using System.Diagnostics;");
            sw.WriteLine();

            sw.Write("namespace ");
            sw.Write(generator.GetNamespace("VtblIndexAttribute"));

            if (generator.Config.GenerateFileScopedNamespaces)
            {
                sw.WriteLine(';');
                sw.WriteLine();
                indentString = "";
            }
            else
            {
                sw.WriteLine();
                sw.WriteLine('{');
            }

            sw.Write(indentString);
            sw.WriteLine("/// <summary>Defines the vtbl index of a method as it was in the native signature.</summary>");
            sw.Write(indentString);
            sw.WriteLine("[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]");
            sw.Write(indentString);
            sw.WriteLine("[Conditional(\"DEBUG\")]");
            sw.Write(indentString);
            sw.WriteLine("internal sealed partial class VtblIndexAttribute : Attribute");
            sw.Write(indentString);
            sw.WriteLine('{');
            sw.Write(indentString);
            sw.WriteLine("    private readonly uint _index;");
            sw.WriteLine();
            sw.Write(indentString);
            sw.WriteLine("    /// <summary>Initializes a new instance of the <see cref=\"VtblIndexAttribute\" /> class.</summary>");
            sw.Write(indentString);
            sw.WriteLine("    /// <param name=\"index\">The vtbl index of a method as it was in the native signature.</param>");
            sw.Write(indentString);
            sw.WriteLine("    public VtblIndexAttribute(uint index)");
            sw.Write(indentString);
            sw.WriteLine("    {");
            sw.Write(indentString);
            sw.WriteLine("        _index = index;");
            sw.Write(indentString);
            sw.WriteLine("    }");
            sw.WriteLine();
            sw.Write(indentString);
            sw.WriteLine("    /// <summary>Gets the vtbl index of a method as it was in the native signature.</summary>");
            sw.Write(indentString);
            sw.WriteLine("    public uint Index => _index;");
            sw.Write(indentString);
            sw.WriteLine('}');

            if (!generator.Config.GenerateFileScopedNamespaces)
            {
                sw.WriteLine('}');
            }

            if (!leaveStreamOpen)
            {
                stream = null;
            }
        }

        static void GenerateTransparentStructs(PInvokeGenerator generator, Stream? stream, bool leaveStreamOpen)
        {
            var config = generator.Config;

            foreach (var transparentStruct in config.WithTransparentStructs)
            {
                var name = transparentStruct.Key;
                var type = transparentStruct.Value.Name;
                var kind = transparentStruct.Value.Kind;

                var isTypePointer = type.Contains('*', StringComparison.Ordinal);

                if (stream is null)
                {
                    var outputPath = Path.Combine(config.OutputLocation, $"{name}.cs");
                    stream = generator._outputStreamFactory(outputPath);
                }

                using var sw = new StreamWriter(stream, s_defaultStreamWriterEncoding, DefaultStreamWriterBufferSize, leaveStreamOpen);
                sw.NewLine = "\n";

                if (!string.IsNullOrEmpty(config.HeaderText))
                {
                    sw.WriteLine(config.HeaderText);
                }

                var indentString = "    ";
                var targetNamespace = generator.GetNamespace(name);

                sw.WriteLine("using System;");

                if (kind == PInvokeGeneratorTransparentStructKind.HandleWin32)
                {
                    var handleNamespace = generator.GetNamespace("HANDLE");

                    if (targetNamespace != handleNamespace)
                    {
                        sw.Write("using ");
                        sw.Write(handleNamespace);
                        sw.WriteLine(';');
                    }
                }

                sw.WriteLine();

                sw.Write("namespace ");
                sw.Write(targetNamespace);

                if (generator.Config.GenerateFileScopedNamespaces)
                {
                    sw.WriteLine(';');
                    sw.WriteLine();
                    indentString = "";
                }
                else
                {
                    sw.WriteLine();
                    sw.WriteLine('{');
                }

                sw.Write(indentString);
                sw.Write("public readonly ");

                if (isTypePointer || IsTransparentStructHexBased(kind))
                {
                    sw.Write("unsafe ");
                }

                sw.Write("partial struct ");
                sw.Write(name);
                sw.Write(" : IComparable, IComparable<");
                sw.Write(name);
                sw.Write(">, IEquatable<");
                sw.Write(name);
                sw.WriteLine(">, IFormattable");

                sw.Write(indentString);
                sw.WriteLine('{');

                sw.Write(indentString);
                sw.Write("    public readonly ");
                sw.Write(type);
                sw.WriteLine(" Value;");
                sw.WriteLine();

                // All transparent structs be created directly from the underlying type

                sw.Write(indentString);
                sw.Write("    public ");
                sw.Write(name);
                sw.Write('(');
                sw.Write(type);
                sw.WriteLine(" value)");
                sw.Write(indentString);
                sw.WriteLine("    {");
                sw.Write(indentString);
                sw.WriteLine("        Value = value;");
                sw.Write(indentString);
                sw.WriteLine("    }");
                sw.WriteLine();

                if (IsTransparentStructHandle(kind) || (kind == PInvokeGeneratorTransparentStructKind.HandleVulkan))
                {
                    // Handle like transparent structs define a NULL member

                    if (kind == PInvokeGeneratorTransparentStructKind.HandleWin32)
                    {
                        sw.Write(indentString);
                        sw.Write("    public static ");
                        sw.Write(name);
                        sw.Write(" INVALID_VALUE => new ");
                        sw.Write(name);

                        if (isTypePointer)
                        {
                            sw.Write("((");
                            sw.Write(type);
                            sw.WriteLine(")(-1));");
                        }
                        else
                        {
                            sw.WriteLine("(-1);");
                        }

                        sw.WriteLine();
                    }

                    sw.Write(indentString);
                    sw.Write("    public static ");
                    sw.Write(name);
                    sw.Write(" NULL => new ");
                    sw.Write(name);

                    if (isTypePointer)
                    {
                        sw.WriteLine("(null);");
                    }
                    else
                    {
                        sw.WriteLine("(0);");
                    }

                    sw.WriteLine();
                }
                else if (IsTransparentStructBoolean(kind))
                {
                    // Boolean like transparent structs define FALSE and TRUE members

                    sw.Write(indentString);
                    sw.Write("    public static ");
                    sw.Write(name);
                    sw.Write(" FALSE => new ");
                    sw.Write(name);
                    sw.WriteLine("(0);");
                    sw.WriteLine();

                    sw.Write(indentString);
                    sw.Write("    public static ");
                    sw.Write(name);
                    sw.Write(" TRUE => new ");
                    sw.Write(name);
                    sw.WriteLine("(1);");
                    sw.WriteLine();
                }

                // All transparent structs support equality and relational comparisons with themselves

                sw.Write(indentString);
                sw.Write("    public static bool operator ==(");
                sw.Write(name);
                sw.Write(" left, ");
                sw.Write(name);
                sw.WriteLine(" right) => left.Value == right.Value;");
                sw.WriteLine();

                sw.Write(indentString);
                sw.Write("    public static bool operator !=(");
                sw.Write(name);
                sw.Write(" left, ");
                sw.Write(name);
                sw.WriteLine(" right) => left.Value != right.Value;");
                sw.WriteLine();

                sw.Write(indentString);
                sw.Write("    public static bool operator <(");
                sw.Write(name);
                sw.Write(" left, ");
                sw.Write(name);
                sw.WriteLine(" right) => left.Value < right.Value;");
                sw.WriteLine();

                sw.Write(indentString);
                sw.Write("    public static bool operator <=(");
                sw.Write(name);
                sw.Write(" left, ");
                sw.Write(name);
                sw.WriteLine(" right) => left.Value <= right.Value;");
                sw.WriteLine();

                sw.Write(indentString);
                sw.Write("    public static bool operator >(");
                sw.Write(name);
                sw.Write(" left, ");
                sw.Write(name);
                sw.WriteLine(" right) => left.Value > right.Value;");
                sw.WriteLine();

                sw.Write(indentString);
                sw.Write("    public static bool operator >=(");
                sw.Write(name);
                sw.Write(" left, ");
                sw.Write(name);
                sw.WriteLine(" right) => left.Value >= right.Value;");
                sw.WriteLine();

                if (IsTransparentStructHandle(kind))
                {
                    // Handle like transparent structs can be cast to/from void*

                    sw.Write(indentString);
                    sw.Write("    public static explicit operator ");
                    sw.Write(name);
                    sw.Write("(void* value) => new ");
                    sw.Write(name);

                    if (type.Equals("void*", StringComparison.Ordinal))
                    {
                        sw.WriteLine("(value);");
                    }
                    else
                    {
                        if (!IsUnsigned(type))
                        {
                            sw.Write("unchecked");
                        }

                        sw.Write("((");
                        sw.Write(type);
                        sw.WriteLine(")(value));");
                    }
                    sw.WriteLine();

                    sw.Write(indentString);
                    sw.Write("    public static implicit operator void*(");
                    sw.Write(name);

                    if (isTypePointer)
                    {
                        sw.WriteLine(" value) => value.Value;");
                    }
                    else
                    {
                        var isUnchecked = !IsUnsigned(type);
                        sw.Write(" value) => ");

                        if (isUnchecked)
                        {
                            sw.Write("unchecked(");
                        }
                        sw.Write("(void*)(value.Value)");

                        if (isUnchecked)
                        {
                            sw.Write(")");
                        }
                        sw.WriteLine();
                    }

                    sw.WriteLine();

                    if ((kind == PInvokeGeneratorTransparentStructKind.HandleWin32) && !name.Equals("HANDLE", StringComparison.Ordinal))
                    {
                        // Win32 handle like transparent structs can also be cast to/from HANDLE

                        sw.Write(indentString);
                        sw.Write("    public static explicit operator ");
                        sw.Write(name);
                        sw.Write("(HANDLE value) => new ");
                        sw.Write(name);
                        sw.WriteLine("(value);");
                        sw.WriteLine();

                        sw.Write(indentString);
                        sw.Write("    public static implicit operator HANDLE(");
                        sw.Write(name);
                        sw.WriteLine(" value) => new HANDLE(value.Value);");
                        sw.WriteLine();
                    }
                }
                else if (IsTransparentStructBoolean(kind))
                {
                    // Boolean like transparent structs define conversion to/from bool
                    // and support for usage in bool like scenarios.

                    sw.Write(indentString);
                    sw.Write("    public static implicit operator bool(");
                    sw.Write(name);
                    sw.WriteLine(" value) => value.Value != 0;");
                    sw.WriteLine();

                    sw.Write(indentString);
                    sw.Write("    public static implicit operator ");
                    sw.Write(name);
                    sw.Write("(bool value) => new ");
                    sw.Write(name);

                    if (type.Equals("int", StringComparison.Ordinal))
                    {
                        sw.WriteLine("(value ? 1 : 0);");
                    }
                    else if (type.Equals("uint", StringComparison.Ordinal))
                    {
                        sw.WriteLine("(value ? 1u : 0u);");
                    }
                    else
                    {
                        sw.Write("((");
                        sw.Write(type);
                        sw.WriteLine(")(value ? 1u : 0u);");
                    }

                    sw.WriteLine();

                    sw.Write(indentString);
                    sw.Write("    public static bool operator false(");
                    sw.Write(name);
                    sw.WriteLine(" value) => value.Value == 0;");
                    sw.WriteLine();

                    sw.Write(indentString);
                    sw.Write("    public static bool operator true(");
                    sw.Write(name);
                    sw.WriteLine(" value) => value.Value != 0;");
                    sw.WriteLine();
                }

                // All transparent structs define casts to/from the various integer types

                OutputConversions(sw, indentString, name, type, kind, "byte");
                OutputConversions(sw, indentString, name, type, kind, "short");
                OutputConversions(sw, indentString, name, type, kind, "int");
                OutputConversions(sw, indentString, name, type, kind, "long");
                OutputConversions(sw, indentString, name, type, kind, "nint");
                OutputConversions(sw, indentString, name, type, kind, "sbyte");
                OutputConversions(sw, indentString, name, type, kind, "ushort");
                OutputConversions(sw, indentString, name, type, kind, "uint");
                OutputConversions(sw, indentString, name, type, kind, "ulong");
                OutputConversions(sw, indentString, name, type, kind, "nuint");

                // All transparent structs override CompareTo, Equals, GetHashCode, and ToString

                sw.Write(indentString);
                sw.WriteLine("    public int CompareTo(object? obj)");
                sw.Write(indentString);
                sw.WriteLine("    {");
                sw.Write(indentString);
                sw.Write("            if (obj is ");
                sw.Write(name);
                sw.WriteLine(" other)");
                sw.Write(indentString);
                sw.WriteLine("        {");
                sw.Write(indentString);
                sw.WriteLine("            return CompareTo(other);");
                sw.Write(indentString);
                sw.WriteLine("        }");
                sw.WriteLine();
                sw.Write(indentString);
                sw.Write("        return (obj is null) ? 1 : throw new ArgumentException(\"obj is not an instance of ");
                sw.Write(name);
                sw.WriteLine(".\");");
                sw.Write(indentString);
                sw.WriteLine("    }");
                sw.WriteLine();

                sw.Write(indentString);
                sw.Write("    public int CompareTo(");
                sw.Write(name);

                if (isTypePointer)
                {
                    sw.WriteLine(" other) => ((nuint)(Value)).CompareTo((nuint)(other.Value));");
                }
                else
                {
                    sw.WriteLine(" other) => Value.CompareTo(other.Value);");
                }

                sw.WriteLine();

                sw.Write(indentString);
                sw.Write("    public override bool Equals(object? obj) => (obj is ");
                sw.Write(name);
                sw.WriteLine(" other) && Equals(other);");
                sw.WriteLine();

                sw.Write(indentString);
                sw.Write("    public bool Equals(");
                sw.Write(name);

                if (isTypePointer)
                {
                    sw.WriteLine(" other) => ((nuint)(Value)).Equals((nuint)(other.Value));");
                }
                else
                {
                    sw.WriteLine(" other) => Value.Equals(other.Value);");
                }

                sw.WriteLine();

                sw.Write(indentString);
                sw.Write("    public override int GetHashCode() => ");

                if (isTypePointer)
                {
                    sw.Write("((nuint)(Value))");
                }
                else
                {
                    sw.Write("Value");
                }

                sw.WriteLine(".GetHashCode();");
                sw.WriteLine();

                sw.Write(indentString);
                sw.Write("    public override string ToString() => ");

                if (isTypePointer)
                {
                    sw.Write("((nuint)(Value))");
                }
                else
                {
                    sw.Write("Value");
                }

                sw.Write(".ToString(");

                if (IsTransparentStructHexBased(kind))
                {
                    var (typeSrcSize, typeDstSize, typeSign) = GetSizeAndSignOf(type);

                    if (typeSrcSize != typeDstSize)
                    {
                        sw.Write("(sizeof(nint) == 4) ? \"X8\" : \"X16\"");
                    }
                    else
                    {
                        sw.Write('"');
                        sw.Write('X');
                        sw.Write(typeSrcSize * 2);
                        sw.Write('"');
                    }
                }

                sw.WriteLine(");");
                sw.WriteLine();

                sw.Write(indentString);
                sw.Write("    public string ToString(string? format, IFormatProvider? formatProvider) => ");

                if (isTypePointer)
                {
                    sw.Write("((nuint)(Value))");
                }
                else
                {
                    sw.Write("Value");
                }

                sw.WriteLine(".ToString(format, formatProvider);");

                sw.Write(indentString);
                sw.WriteLine('}');

                if (!generator.Config.GenerateFileScopedNamespaces)
                {
                    sw.WriteLine('}');
                }

                if (!leaveStreamOpen)
                {
                    stream = null;
                }
            }

            static (int srcSize, int dstSize, int sign) GetSizeAndSignOf(string type)
            {
                if (type.Contains('*', StringComparison.Ordinal))
                {
                    return (8, 4, +1);
                }

                return type switch {
                    "sbyte" => (1, 1, -1),
                    "byte" => (1, 1, +1),
                    "short" => (2, 2, -1),
                    "ushort" => (2, 2, +1),
                    "int" => (4, 4, -1),
                    "uint" => (4, 4, +1),
                    "nint" => (8, 4, -1),
                    "nuint" => (8, 4, +1),
                    "long" => (8, 8, -1),
                    "ulong" => (8, 8, +1),
                    _ => (0, 0, 0),
                };
            }

            static void OutputConversions(StreamWriter sw, string indentString, string name, string type, PInvokeGeneratorTransparentStructKind kind, string target)
            {
                var (typeSrcSize, typeDstSize, typeSign) = GetSizeAndSignOf(type);
                var (targetSrcSize, targetDstSize, targetSign) = GetSizeAndSignOf(target);

                var isTypePointer = type.Contains('*', StringComparison.Ordinal);
                var isPointerToNativeCast = (isTypePointer && target.Equals("nint", StringComparison.Ordinal)) || (isTypePointer && target.Equals("nuint", StringComparison.Ordinal));

                // public static castFromKind operator name(target value) => new name((type)(value));

                var castFromKind = "implicit";
                var areEquivalentTypeAndTarget = (type == target) || isPointerToNativeCast
                    || (type.Equals("nint", StringComparison.Ordinal) && target.Equals("int", StringComparison.Ordinal))
                    || (type.Equals("nuint", StringComparison.Ordinal) && target.Equals("uint", StringComparison.Ordinal))
                    || (type.Equals("long", StringComparison.Ordinal) && target.Equals("nint", StringComparison.Ordinal))
                    || (type.Equals("ulong", StringComparison.Ordinal) && target.Equals("nuint", StringComparison.Ordinal));

                if (((typeDstSize <= targetSrcSize) && !areEquivalentTypeAndTarget) || ((targetSign == -1) && (typeSign == +1)) || IsTransparentStructHandle(kind))
                {
                    castFromKind = "explicit";
                }

                sw.Write(indentString);
                sw.Write("    public static ");
                sw.Write(castFromKind);
                sw.Write(" operator ");
                sw.Write(name);
                sw.Write('(');
                sw.Write(target);
                sw.Write(" value) => new ");
                sw.Write(name);
                sw.Write('(');

                if (castFromKind.Equals("explicit", StringComparison.Ordinal) || isPointerToNativeCast)
                {
                    sw.Write("unchecked((");
                    sw.Write(type);
                    sw.Write(")(");
                }

                sw.Write("value");

                if (castFromKind.Equals("explicit", StringComparison.Ordinal) || isPointerToNativeCast)
                {
                    sw.Write("))");
                }

                sw.WriteLine(");");
                sw.WriteLine();

                // public static castToKind operator target(name value) => ((target)(value.Value));

                var castToKind = "implicit";
                areEquivalentTypeAndTarget = (type == target) || isPointerToNativeCast
                    || (type.Equals("int", StringComparison.Ordinal) && target.Equals("nint", StringComparison.Ordinal))
                    || (type.Equals("uint", StringComparison.Ordinal) && target.Equals("nuint", StringComparison.Ordinal))
                    || (type.Equals("nint", StringComparison.Ordinal) && target.Equals("long", StringComparison.Ordinal))
                    || (type.Equals("nuint", StringComparison.Ordinal) && target.Equals("ulong", StringComparison.Ordinal));

                if (((targetDstSize <= typeSrcSize) && !areEquivalentTypeAndTarget) || ((typeSign == -1) && (targetSign == +1)))
                {
                    castToKind = "explicit";
                }

                sw.Write(indentString);
                sw.Write("    public static ");
                sw.Write(castToKind);
                sw.Write(" operator ");
                sw.Write(target);
                sw.Write('(');
                sw.Write(name);
                sw.Write(" value) => ");

                if (castToKind.Equals("explicit", StringComparison.Ordinal) || isPointerToNativeCast)
                {
                    sw.Write('(');
                    sw.Write(target);
                    sw.Write(")(");
                }

                sw.Write("value.Value");

                if (castToKind.Equals("explicit", StringComparison.Ordinal) || isPointerToNativeCast)
                {
                    sw.Write(')');
                }

                sw.WriteLine(';');
                sw.WriteLine();
            }
        }
    }

    public void Dispose()
    {
        Dispose(isDisposing: true);
        GC.SuppressFinalize(this);
    }

    public void GenerateBindings(TranslationUnit translationUnit, string filePath, string[] clangCommandLineArgs, CXTranslationUnit_Flags translationFlags)
    {
        ArgumentNullException.ThrowIfNull(translationUnit);
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

                if (diagnostic.Severity is CXDiagnostic_Error or CXDiagnostic_Fatal)
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

#pragma warning disable CA1031 // Do not catch general exception types

        try
        {
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

                translationFlags = _translationFlags & ~CXTranslationUnit_DetailedPreprocessingRecord;
                var handle = CXTranslationUnit.Parse(IndexHandle, _filePath, _clangCommandLineArgs, unsavedFiles, translationFlags);

                using var nestedTranslationUnit = TranslationUnit.GetOrCreate(handle);
                Visit(nestedTranslationUnit.TranslationUnitDecl);
            }
            else
            {
                Visit(translationUnit.TranslationUnitDecl);
            }

            if (_config.LogPotentialTypedefRemappings)
            {
                foreach (var kvp in _traversedValidNameRemappings)
                {
                    var name = kvp.Key;
                    var remappings = kvp.Value;

                    if (!_config.RemappedNames.TryGetValue(name, out _))
                    {
                        var addDiag = false;

                        var altName = name;
                        var smlName = name;

                        if (name.Contains("::", StringComparison.Ordinal))
                        {
                            altName = name.Replace("::", ".", StringComparison.Ordinal);
                            smlName = altName.Split('.')[^1];
                        }
                        else if (name.Contains('.', StringComparison.Ordinal))
                        {
                            altName = name.Replace(".", "::", StringComparison.Ordinal);
                            smlName = altName.Split("::")[^1];
                        }
                        else
                        {
                            addDiag = true;
                        }

                        if (!addDiag && !_config.RemappedNames.TryGetValue(altName, out _))
                        {
                            if (!_config.RemappedNames.TryGetValue(smlName, out _))
                            {
                                addDiag = true;
                            }
                        }

                        if (addDiag && !remappings.Contains(altName) && !remappings.Contains(smlName))
                        {
                            AddDiagnostic(DiagnosticLevel.Info, $"Potential missing remapping '{name}'. {GetFoundRemappingString(name, remappings)}");
                        }
                    }
                }

                foreach (var kvp in _allValidNameRemappings)
                {
                    var name = kvp.Key;
                    var remappings = kvp.Value;

                    if (_config.RemappedNames.TryGetValue(name, out var remappedName) && !remappings.Contains(remappedName) && (name != remappedName) && !_config.ForceRemappedNames.Contains(name))
                    {
                        var addDiag = false;

                        var altName = name;
                        var smlName = name;

                        if (name.Contains("::", StringComparison.Ordinal))
                        {
                            altName = name.Replace("::", ".", StringComparison.Ordinal);
                            smlName = altName.Split('.')[^1];
                        }
                        else if (name.Contains('.', StringComparison.Ordinal))
                        {
                            altName = name.Replace(".", "::", StringComparison.Ordinal);
                            smlName = altName.Split("::")[^1];
                        }
                        else
                        {
                            addDiag = true;
                        }

                        if (!addDiag && _config.RemappedNames.TryGetValue(altName, out remappedName) && !remappings.Contains(remappedName) && (altName != remappedName) && !_config.ForceRemappedNames.Contains(altName))
                        {
                            if (_config.RemappedNames.TryGetValue(smlName, out remappedName) && !remappings.Contains(remappedName) && (smlName != remappedName) && !_config.ForceRemappedNames.Contains(smlName))
                            {
                                addDiag = true;
                            }
                        }

                        if (addDiag)
                        {
                            AddDiagnostic(DiagnosticLevel.Info, $"Potential invalid remapping '{name}={remappedName}'. {GetFoundRemappingString(name, remappings)}");
                        }
                    }
                }

                foreach (var name in _usedRemappings)
                {
                    var remappedName = _config.RemappedNames[name];

                    if (!_allValidNameRemappings.ContainsKey(name) && (name != remappedName) && !_config.ForceRemappedNames.Contains(name))
                    {
                        var addDiag = false;

                        var altName = name;
                        var smlName = name;

                        if (name.Contains("::", StringComparison.Ordinal))
                        {
                            altName = name.Replace("::", ".", StringComparison.Ordinal);
                            smlName = altName.Split('.')[^1];
                        }
                        else if (name.Contains('.', StringComparison.Ordinal))
                        {
                            altName = name.Replace(".", "::", StringComparison.Ordinal);
                            smlName = altName.Split("::")[^1];
                        }
                        else
                        {
                            addDiag = true;
                        }

                        if (!addDiag && !_allValidNameRemappings.ContainsKey(altName) && (altName != remappedName) && !_config.ForceRemappedNames.Contains(altName))
                        {
                            if (!_allValidNameRemappings.ContainsKey(smlName) && (smlName != remappedName) && !_config.ForceRemappedNames.Contains(smlName))
                            {
                                addDiag = true;
                            }
                        }

                        if (addDiag)
                        {
                            AddDiagnostic(DiagnosticLevel.Info, $"Potential invalid remapping '{name}={remappedName}'. No remappings were found.");
                        }
                    }
                }

                static string GetFoundRemappingString(string name, HashSet<string> remappings)
                {
                    var recommendedRemapping = "";

                    if (remappings.Count == 1)
                    {
                        recommendedRemapping = remappings.Single();
                    }

                    if (string.IsNullOrEmpty(recommendedRemapping) && name.StartsWith('_'))
                    {
                        var remapping = name[1..];

                        if (remappings.Contains(remapping))
                        {
                            recommendedRemapping = remapping;
                        }
                    }

                    if (string.IsNullOrEmpty(recommendedRemapping) && name.StartsWith("tag", StringComparison.Ordinal))
                    {
                        var remapping = name[3..];

                        if (remappings.Contains(remapping))
                        {
                            recommendedRemapping = remapping;
                        }
                    }

                    if (string.IsNullOrEmpty(recommendedRemapping) && name.EndsWith('_'))
                    {
                        var remapping = name[0..^1];

                        if (remappings.Contains(remapping))
                        {
                            recommendedRemapping = remapping;
                        }
                    }

                    if (string.IsNullOrEmpty(recommendedRemapping) && name.EndsWith("tag", StringComparison.Ordinal))
                    {
                        var remapping = name[0..^3];

                        if (remappings.Contains(remapping))
                        {
                            recommendedRemapping = remapping;
                        }
                    }

                    if (string.IsNullOrEmpty(recommendedRemapping))
                    {
                        var remapping = name.ToUpperInvariant();

                        if (remappings.Contains(remapping))
                        {
                            recommendedRemapping = remapping;
                        }
                    }

                    var result = "";
                    var remainingRemappings = (IEnumerable<string>)remappings;
                    var remainingString = "Found";

                    if (!string.IsNullOrEmpty(recommendedRemapping))
                    {
                        result += $"Recommended remapping: '{name}={recommendedRemapping}'.";

                        if (remappings.Count == 1)
                        {
                            remainingRemappings = Array.Empty<string>();
                        }
                        else
                        {
                            result += ' ';
                            remainingRemappings = remappings.Except([recommendedRemapping]);
                            remainingString = "Other";
                        }
                    }

                    if (remainingRemappings.Any())
                    {
                        result += $"{remainingString} typedefs: {string.Join("; ", remainingRemappings)}";
                    }

                    return result;
                }
            }
        }
        catch (Exception e)
        {
            var diagnostic = new Diagnostic(DiagnosticLevel.Error, e.ToString());
            _diagnostics.Add(diagnostic);
        }

#pragma warning restore CA1031 // Do not catch general exception types

        GC.KeepAlive(translationUnit);
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

    private void AddDiagnostic(DiagnosticLevel level, string message, Cursor? cursor)
    {
        var diagnostic = new Diagnostic(level, message, (cursor?.Location).GetValueOrDefault());

        if (_diagnostics.Contains(diagnostic))
        {
            return;
        }

        _diagnostics.Add(diagnostic);
    }

    private void AddUsingDirective(IOutputBuilder? outputBuilder, string namespaceName)
    {
        if (outputBuilder is null)
        {
            return;
        }

        var needsUsing = false;

        if (_currentNamespace is not null)
        {
            if (!_currentNamespace.StartsWith(namespaceName, StringComparison.Ordinal))
            {
                needsUsing = true;
            }
            else if ((_currentNamespace.Length > namespaceName.Length) && (_currentNamespace[namespaceName.Length] != '.'))
            {
                needsUsing = true;
            }
        }

        if (needsUsing)
        {
            outputBuilder.EmitUsingDirective(namespaceName);
        }
    }

    private void CloseOutputBuilder(Stream stream, IOutputBuilder outputBuilder, bool isMethodClass, bool leaveStreamOpen, bool emitNamespaceDeclaration)
    {
        ArgumentNullException.ThrowIfNull(stream);
        ArgumentNullException.ThrowIfNull(outputBuilder);

        using var sw = new StreamWriter(stream, s_defaultStreamWriterEncoding, DefaultStreamWriterBufferSize, leaveStreamOpen);
        sw.NewLine = "\n";

        if (_config.GenerateMultipleFiles)
        {
            if (outputBuilder is CSharpOutputBuilder csharpOutputBuilder)
            {
                if (!string.IsNullOrEmpty(_config.HeaderText))
                {
                    sw.WriteLine(_config.HeaderText);
                }

                if (isMethodClass)
                {
                    var nonTestName = outputBuilder.IsTestOutput ? outputBuilder.Name[0..^5] : outputBuilder.Name;

                    if (_topLevelClassUsings.TryGetValue(nonTestName, out var withUsings))
                    {
                        foreach (var withUsing in withUsings)
                        {
                            csharpOutputBuilder.AddUsingDirective(withUsing);
                        }
                    }
                }

                var usingDirectives = new SortedSet<string>(csharpOutputBuilder.UsingDirectives, StringComparer.Ordinal);

                foreach (var staticUsingDirective in csharpOutputBuilder.StaticUsingDirectives)
                {
                    _ = usingDirectives.Add(staticUsingDirective);
                }

                if (usingDirectives.Count != 0)
                {
                    foreach (var usingDirective in usingDirectives)
                    {
                        sw.Write("using ");
                        sw.Write(usingDirective);
                        sw.WriteLine(';');
                    }

                    sw.WriteLine();
                }
            }
            else if (outputBuilder is XmlOutputBuilder xmlOutputBuilder)
            {
                sw.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\" ?>");
                sw.WriteLine("<bindings>");

                if (!string.IsNullOrEmpty(_config.HeaderText))
                {
                    sw.WriteLine("  <comment>");

                    foreach (var ln in _config.HeaderText.Split('\n'))
                    {
                        sw.Write("    ");
                        sw.WriteLine(ln);
                    }

                    sw.WriteLine("  </comment>");
                }
            }
        }

        if (outputBuilder is CSharpOutputBuilder csOutputBuilder)
        {
            if (csOutputBuilder.Contents.Any())
            {
                ForCSharp(csOutputBuilder);
            }
        }
        else if (outputBuilder is XmlOutputBuilder xmlOutputBuilder)
        {
            if (xmlOutputBuilder.Contents.Any())
            {
                ForXml(xmlOutputBuilder);
            }
        }

        void ForCSharp(CSharpOutputBuilder csharpOutputBuilder)
        {
            var indentationString = csharpOutputBuilder.IndentationString;
            var nonTestName = outputBuilder.IsTestOutput ? outputBuilder.Name[0..^5] : outputBuilder.Name;

            if (emitNamespaceDeclaration)
            {
                sw.Write("namespace ");
                sw.Write(GetNamespace(nonTestName));

                if (csharpOutputBuilder.IsTestOutput)
                {
                    sw.Write(".UnitTests");
                }

                if (Config.GenerateFileScopedNamespaces)
                {
                    sw.WriteLine(';');
                    sw.WriteLine();
                    indentationString = "";
                }
                else
                {
                    sw.WriteLine();
                    sw.WriteLine('{');
                }
            }
            else
            {
                sw.WriteLine();
            }

            if (isMethodClass)
            {
                var isTopLevelStruct = _config.WithTypes.TryGetValue(nonTestName, out var withType) && withType.Equals("struct", StringComparison.Ordinal);

                if (outputBuilder.IsTestOutput)
                {
                    sw.Write(indentationString);
                    sw.Write("/// <summary>Provides validation of the <see cref=\"");
                    sw.Write(nonTestName);
                    sw.Write("\" /> ");

                    if (isTopLevelStruct)
                    {
                        sw.Write("struct");
                    }
                    else
                    {
                        sw.Write("class");
                    }

                    sw.WriteLine(".</summary>");
                }

                if (_topLevelClassAttributes.TryGetValue(nonTestName, out var withAttributes))
                {
                    if (withAttributes.Count != 0)
                    {
                        foreach (var attribute in withAttributes)
                        {
                            if (outputBuilder.IsTestOutput && !attribute.StartsWith("SupportedOSPlatform(", StringComparison.Ordinal))
                            {
                                continue;
                            }

                            sw.Write(indentationString);
                            sw.Write('[');
                            sw.Write(attribute);
                            sw.WriteLine(']');
                        }
                    }
                }

                sw.Write(indentationString);
                sw.Write(GetMethodClassAccessSpecifier(outputBuilder.Name).AsString());
                sw.Write(' ');

                if (outputBuilder.IsTestOutput || !isTopLevelStruct)
                {
                    sw.Write("static ");
                }

                if ((_topLevelClassIsUnsafe.TryGetValue(nonTestName, out var isUnsafe) && isUnsafe) || (outputBuilder.IsTestOutput && isTopLevelStruct))
                {
                    sw.Write("unsafe ");
                }

                sw.Write("partial ");

                if (!outputBuilder.IsTestOutput && isTopLevelStruct)
                {
                    sw.Write("struct ");
                }
                else
                {
                    sw.Write("class ");
                }

                sw.Write(outputBuilder.Name);

                if (_topLevelClassHasGuidMember.TryGetValue(outputBuilder.Name, out var hasGuidMember) && hasGuidMember)
                {
                    sw.Write(" : INativeGuid");
                }

                sw.WriteLine();
                sw.Write(indentationString);
                sw.Write('{');

                if ((!outputBuilder.IsTestOutput && !isTopLevelStruct) || !string.IsNullOrEmpty(csharpOutputBuilder.Contents.First()))
                {
                    sw.WriteLine();
                }

                indentationString += csharpOutputBuilder.IndentationString;
            }

            foreach (var line in csharpOutputBuilder.Contents)
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
                indentationString = indentationString[..^csharpOutputBuilder.IndentationString.Length];

                sw.Write(indentationString);
                sw.WriteLine('}');
            }

            if (_config.GenerateMultipleFiles && !Config.GenerateFileScopedNamespaces)
            {
                sw.WriteLine('}');
            }
        }

        void ForXml(XmlOutputBuilder xmlOutputBuilder)
        {
            const string Indent = "  ";
            var indentationString = Indent;

            if (emitNamespaceDeclaration)
            {
                sw.Write(indentationString);
                sw.Write("<namespace name=\"");
                sw.Write(GetNamespace(xmlOutputBuilder.Name));
                sw.WriteLine("\">");
            }

            indentationString += Indent;

            if (isMethodClass)
            {
                sw.Write(indentationString);
                sw.Write("<class name=\"");
                sw.Write(xmlOutputBuilder.Name);
                sw.Write("\" access=\"public\" static=\"true\"");

                if (_topLevelClassIsUnsafe.TryGetValue(xmlOutputBuilder.Name, out var isUnsafe) && isUnsafe)
                {
                    sw.Write(" unsafe=\"true\"");
                }

                sw.WriteLine('>');
                indentationString += Indent;
            }

            foreach (var line in xmlOutputBuilder.Contents)
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
                indentationString = indentationString[..^Indent.Length];
                sw.Write(indentationString);
                sw.WriteLine("</class>");
            }

            if (_config.GenerateMultipleFiles)
            {
                indentationString = indentationString[..^Indent.Length];
                sw.Write(indentationString);
                sw.WriteLine("</namespace>");
                sw.WriteLine("</bindings>");
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

    private static string EscapeName(string name)
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
        if (name.StartsWith(_config.MethodPrefixToStrip, StringComparison.Ordinal))
        {
            name = name[_config.MethodPrefixToStrip.Length..];
        }

        return EscapeName(name);
    }

    internal static string EscapeCharacter(char value) => value switch {
        '\0' => @"\0",
        '\\' => @"\\",
        '\r' => @"\r",
        '\n' => @"\n",
        '\t' => @"\t",
        '\'' => @"\'",
        _ => value.ToString(),
    };

    // We first replace already escaped characters with their raw counterpart
    // We then re-escape any raw characters. This ensures we don't end up with double escaped backslashes
    internal static string EscapeString(string value) => value.Replace(@"\0", "\0", StringComparison.Ordinal)
                                                              .Replace(@"\r", "\r", StringComparison.Ordinal)
                                                              .Replace(@"\n", "\n", StringComparison.Ordinal)
                                                              .Replace(@"\t", "\t", StringComparison.Ordinal)
                                                              .Replace(@"\""", "\"", StringComparison.Ordinal)
                                                              .Replace("\\", @"\\", StringComparison.Ordinal)
                                                              .Replace("\0", @"\0", StringComparison.Ordinal)
                                                              .Replace("\r", @"\r", StringComparison.Ordinal)
                                                              .Replace("\n", @"\n", StringComparison.Ordinal)
                                                              .Replace("\t", @"\t", StringComparison.Ordinal)
                                                              .Replace("\"", @"\""", StringComparison.Ordinal);

    private AccessSpecifier GetAccessSpecifier(NamedDecl namedDecl, bool matchStar)
    {
        if (!TryGetRemappedValue(namedDecl, _config.WithAccessSpecifiers, out var accessSpecifier, matchStar) || (accessSpecifier == AccessSpecifier.None))
        {
            switch (namedDecl.Access)
            {
                case CX_CXXInvalidAccessSpecifier:
                {
                    // Top level declarations will have an invalid access specifier
                    accessSpecifier = AccessSpecifier.Public;
                    break;
                }

                case CX_CXXPublic:
                {
                    accessSpecifier = AccessSpecifier.Public;
                    break;
                }

                case CX_CXXProtected:
                {
                    accessSpecifier = AccessSpecifier.Protected;
                    break;
                }

                case CX_CXXPrivate:
                {
                    accessSpecifier = AccessSpecifier.Private;
                    break;
                }

                default:
                {
                    accessSpecifier = AccessSpecifier.Internal;
                    AddDiagnostic(DiagnosticLevel.Warning, $"Unknown access specifier: '{namedDecl.Access}'. Falling back to '{accessSpecifier}'.", namedDecl);
                    break;
                }
            }
        }

        return accessSpecifier;
    }

    private AccessSpecifier GetMethodClassAccessSpecifier(string name)
    {
        var accessMap = _config.WithAccessSpecifiers;
        return (accessMap.TryGetValue(name, out var accessSpecifier) || accessMap.TryGetValue("*", out accessSpecifier))
             ? accessSpecifier
             : AccessSpecifier.None;
    }

    private static string GetAnonymousName(Cursor cursor, string kind)
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

    private BitfieldDesc[] GetBitfieldDescs(RecordDecl recordDecl)
    {
        var bitfieldDescs = new List<BitfieldDesc>(recordDecl.Fields.Count);

        var backingFieldIndex = -1;
        var previousSize = 0L;
        var remainingBits = 0L;
        var currentBits = 0L;

        foreach (var fieldDecl in recordDecl.Fields)
        {
            if (!fieldDecl.IsBitField)
            {
                previousSize = 0;
                remainingBits = 0;
                continue;
            }

            var type = fieldDecl.Type;
            var currentSize = type.Handle.SizeOf;

            if ((!_config.GenerateUnixTypes && (currentSize != previousSize)) || (fieldDecl.BitWidthValue > remainingBits))
            {
                backingFieldIndex++;
                currentBits = currentSize * 8;
                remainingBits = currentBits;
                previousSize = 0;

                var typeBacking = type;

                if (IsType<EnumType>(fieldDecl, type, out var enumType))
                {
                    typeBacking = enumType.Decl.IntegerType;
                }

                var bitfieldDesc = new BitfieldDesc {
                    TypeBacking = typeBacking,
                    Regions = [
                        new BitfieldRegion {
                            Name = GetRemappedCursorName(fieldDecl),
                            Offset = 0,
                            Length = fieldDecl.BitWidthValue
                        },
                    ]
                };
                bitfieldDescs.Add(bitfieldDesc);
            }
            else
            {
                var bitfieldDesc = bitfieldDescs[^1];

                if (_config.GenerateUnixTypes && (currentSize > previousSize))
                {
                    remainingBits += (currentSize - previousSize) * 8;
                    currentBits += (currentSize - previousSize) * 8;

                    var typeBacking = type;

                    if (IsType<EnumType>(fieldDecl, type, out var enumType))
                    {
                        typeBacking = enumType.Decl.IntegerType;
                    }

                    bitfieldDescs[^1] = new BitfieldDesc {
                        TypeBacking = typeBacking,
                        Regions = bitfieldDesc.Regions,
                    };
                }

                var bitfieldRegion = new BitfieldRegion {
                    Name = GetRemappedCursorName(fieldDecl),
                    Offset = currentBits - remainingBits,
                    Length = fieldDecl.BitWidthValue
                };
                bitfieldDesc.Regions.Add(bitfieldRegion);
            }

            remainingBits -= fieldDecl.BitWidthValue;
            previousSize = Math.Max(previousSize, currentSize);
        }

        return [.. bitfieldDescs];
    }

    private CallConv GetCallingConvention(Cursor? cursor, Cursor? context, Type type)
    {
        if (cursor is FunctionDecl functionDecl)
        {
            if (functionDecl.IsVariadic)
            {
                return CallConv.Cdecl;
            }

            if (_config.GenerateCallConvMemberFunction)
            {
                if ((cursor is CXXMethodDecl cxxMethodDecl) && cxxMethodDecl.IsInstance && (context is not null))
                {
                    return CallConv.MemberFunction;
                }
            }
        }

        if (cursor is NamedDecl namedDecl)
        {
            if (TryGetRemappedValue(namedDecl, _config.WithCallConvs, out var callConv, matchStar: true))
            {
                if (Enum.TryParse<CallConv>(callConv, true, out var remappedCallingConvention))
                {
                    return remappedCallingConvention;
                }
                AddDiagnostic(DiagnosticLevel.Warning, $"Unsupported manually-specified calling convention: '{callConv}'. Determining convention from cursor.", cursor);
            }
        }

        var wasRemapped = false;
        return GetCallingConvention(cursor, context, type, ref wasRemapped);
    }

    private CallConv GetCallingConvention(Cursor? cursor, Cursor? context, Type type, ref bool wasRemapped)
    {
        var remappedName = GetRemappedTypeName(cursor, context, type, out _, ignoreTransparentStructsWhereRequired: false, skipUsing: true);

        if (_config.WithCallConvs.TryGetValue(remappedName, out var callConv) || _config.WithCallConvs.TryGetValue("*", out callConv))
        {
            if (Enum.TryParse<CallConv>(callConv, true, out var remappedCallingConvention))
            {
                if (_config.GenerateCallConvMemberFunction)
                {
                    if ((cursor is CXXMethodDecl cxxMethodDecl) && cxxMethodDecl.IsInstance)
                    {
                        return CallConv.MemberFunction;
                    }
                }

                wasRemapped = true;
                return remappedCallingConvention;
            }
            AddDiagnostic(DiagnosticLevel.Warning, $"Unsupported manually-specified calling convention: '{callConv}'. Determining convention from cursor.", cursor);
        }

        if (IsType<AttributedType>(cursor, type, out var attributedType))
        {
            var callingConvention = GetCallingConvention(cursor, context, attributedType.ModifiedType, ref wasRemapped);

            if (wasRemapped)
            {
                return callingConvention;
            }

            switch (attributedType.AttrKind)
            {
                case CX_AttrKind_MSABI:
                case CX_AttrKind_SysVABI:
                {
                    return CallConv.Winapi;
                }

                case CX_AttrKind_CDecl:
                {
                    return CallConv.Cdecl;
                }

                case CX_AttrKind_FastCall:
                {
                    return CallConv.FastCall;
                }

                case CX_AttrKind_StdCall:
                {
                    return CallConv.StdCall;
                }

                case CX_AttrKind_ThisCall:
                {
                    return _config.GenerateCallConvMemberFunction ? CallConv.MemberFunction : CallConv.ThisCall;
                }

                case CX_AttrKind_AArch64VectorPcs:
                case CX_AttrKind_Pcs:
                case CX_AttrKind_PreserveAll:
                case CX_AttrKind_PreserveMost:
                case CX_AttrKind_RegCall:
                case CX_AttrKind_VectorCall:
                {
                    AddDiagnostic(DiagnosticLevel.Warning, $"Unsupported calling convention: '{attributedType.AttrKind}'.", cursor);
                    return callingConvention;
                }

                default:
                {
                    return callingConvention;
                }
            }
        }
        else if (IsType<FunctionType>(cursor, type, out var functionType))
        {
            var callingConv = functionType.CallConv;

            switch (callingConv)
            {
                case CXCallingConv_C:
                {
                    return ((cursor is CXXMethodDecl cxxMethodDecl) && cxxMethodDecl.IsInstance)
                         ? (_config.GenerateCallConvMemberFunction ? CallConv.MemberFunction : CallConv.ThisCall)
                         : CallConv.Cdecl;
                }

                case CXCallingConv_X86StdCall:
                {
                    return CallConv.StdCall;
                }

                case CXCallingConv_X86FastCall:
                {
                    return CallConv.FastCall;
                }

                case CXCallingConv_X86ThisCall:
                {
                    return _config.GenerateCallConvMemberFunction ? CallConv.MemberFunction : CallConv.ThisCall;
                }

                case CXCallingConv_Win64:
                {
                    return CallConv.Winapi;
                }

                default:
                {
                    const CallConv Name = CallConv.Winapi;
                    AddDiagnostic(DiagnosticLevel.Warning, $"Unsupported calling convention: '{callingConv}'. Falling back to '{Name}'.", cursor);
                    return Name;
                }
            }
        }
        else if (IsType<PointerType>(cursor, type, out var pointerType))
        {
            return GetCallingConvention(cursor, context, pointerType.PointeeType, ref wasRemapped);
        }
        else
        {
            AddDiagnostic(DiagnosticLevel.Warning, $"Unsupported type: '{type.TypeClass}'. Falling back to canonical type.", cursor);
            return GetCallingConvention(cursor, context, type.CanonicalType, ref wasRemapped);
        }
    }

    private string GetCursorName(NamedDecl namedDecl)
    {
        if (!_cursorNames.TryGetValue(namedDecl, out var name))
        {
            name = namedDecl.Name.NormalizePath();

            // strip the prefix
            if (name.StartsWith("enum ", StringComparison.Ordinal))
            {
                name = name[5..];
            }
            else if (name.StartsWith("struct ", StringComparison.Ordinal))
            {
                name = name[7..];
            }
            else if (name.StartsWith("union ", StringComparison.Ordinal))
            {
                name = name[6..];
            }

            if (namedDecl is CXXConstructorDecl cxxConstructorDecl)
            {
                var parent = cxxConstructorDecl.Parent;
                Debug.Assert(parent is not null);
                name = GetCursorName(parent);
            }
            else if (namedDecl is CXXDestructorDecl)
            {
                name = "Dispose";
            }
            else if (string.IsNullOrWhiteSpace(name) || name.StartsWith('('))
            {
#if DEBUG
                if (name.StartsWith('('))
                {
                    Debug.Assert(name.StartsWith("(anonymous enum at ", StringComparison.Ordinal) ||
                                 name.StartsWith("(anonymous struct at ", StringComparison.Ordinal) ||
                                 name.StartsWith("(anonymous union at ", StringComparison.Ordinal) ||
                                 name.StartsWith("(unnamed enum at ", StringComparison.Ordinal) ||
                                 name.StartsWith("(unnamed struct at ", StringComparison.Ordinal) ||
                                 name.StartsWith("(unnamed union at ", StringComparison.Ordinal) ||
                                 name.StartsWith("(unnamed at ", StringComparison.Ordinal));
                    Debug.Assert(name.EndsWith(')'));
                }
#endif

                if (namedDecl is TypeDecl typeDecl)
                {
                    name = (typeDecl is TagDecl tagDecl) && tagDecl.Handle.IsAnonymous
                         ? GetAnonymousName(tagDecl, tagDecl.TypeForDecl.KindSpelling)
                         : GetTypeName(namedDecl, context: null, typeDecl.TypeForDecl, ignoreTransparentStructsWhereRequired: false, out _);
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
                    AddDiagnostic(DiagnosticLevel.Error, $"Unsupported anonymous named declaration: '{namedDecl.DeclKindName}'.", namedDecl);
                }
            }

            _cursorNames[namedDecl] = name;
        }

        Debug.Assert(!string.IsNullOrWhiteSpace(name));
        return name;
    }

    private string GetCursorQualifiedName(NamedDecl namedDecl, bool truncateParameters = false)
    {
        if (!_cursorQualifiedNames.TryGetValue((namedDecl, truncateParameters), out var qualifiedName))
        {
            var parts = new Stack<NamedDecl>();
            Decl? decl = namedDecl;

            do
            {
                if (decl is NamedDecl parentNamedDecl)
                {
                    parts.Push(parentNamedDecl);
                }

                if ((decl.DeclContext is null) && (decl is CXXMethodDecl cxxMethodDecl))
                {
                    var cxxRecordDecl = cxxMethodDecl.ThisObjectType.AsCXXRecordDecl;
                    Debug.Assert(cxxRecordDecl is not null);
                    decl = cxxRecordDecl;
                }
                else
                {
                    decl = (Decl?)decl.DeclContext;
                }
            }
            while (decl is not null);

            var qualifiedNameBuilder = new StringBuilder();

            var part = parts.Pop();

            while (parts.Count != 0)
            {
                AppendNamedDecl(part, GetCursorName(part), qualifiedNameBuilder);
                _ = qualifiedNameBuilder.Append("::");
                part = parts.Pop();
            }

            AppendNamedDecl(part, GetCursorName(part), qualifiedNameBuilder);

            qualifiedName = qualifiedNameBuilder.ToString();
            _cursorQualifiedNames[(namedDecl, truncateParameters)] = qualifiedName;
        }

        Debug.Assert(!string.IsNullOrWhiteSpace(qualifiedName));
        return qualifiedName;

        void AppendFunctionParameters(CXType functionType, StringBuilder qualifiedName)
        {
            if (truncateParameters)
            {
                return;
            }

            _ = qualifiedName.Append('(');

            if (functionType.NumArgTypes != 0)
            {
                _ = qualifiedName.Append(functionType.GetArgType(0).Spelling);

                for (uint i = 1; i < functionType.NumArgTypes; i++)
                {
                    _ = qualifiedName.Append(',');
                    _ = qualifiedName.Append(' ');
                    _ = qualifiedName.Append(functionType.GetArgType(i).Spelling);
                }
            }

            _ = qualifiedName.Append(')');
            _ = qualifiedName.Append(':');

            _ = qualifiedName.Append(functionType.ResultType.Spelling);

            if (functionType.ExceptionSpecificationType == CXCursor_ExceptionSpecificationKind.CXCursor_ExceptionSpecificationKind_NoThrow)
            {
                _ = qualifiedName.Append(' ');
                _ = qualifiedName.Append("nothrow");
            }
        }

        void AppendNamedDecl(NamedDecl namedDecl, string name, StringBuilder qualifiedName)
        {
            _ = qualifiedName.Append(name);

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

        void AppendTemplateArgument(TemplateArgument templateArgument, StringBuilder qualifiedName)
        {
            switch (templateArgument.Kind)
            {
                case CXTemplateArgumentKind_Type:
                {
                    _ = qualifiedName.Append(templateArgument.AsType.AsString);
                    break;
                }

                case CXTemplateArgumentKind_Integral:
                {
                    _ = qualifiedName.Append(templateArgument.AsIntegral);
                    break;
                }

                default:
                {
                    _ = qualifiedName.Append('?');
                    break;
                }
            }
        }

        void AppendTemplateArguments(ClassTemplateSpecializationDecl classTemplateSpecializationDecl, StringBuilder qualifiedName)
        {
            if (truncateParameters)
            {
                return;
            }

            _ = qualifiedName.Append('<');

            var templateArgs = classTemplateSpecializationDecl.TemplateArgs;

            if (templateArgs.Any())
            {
                AppendTemplateArgument(templateArgs[0], qualifiedName);

                for (var i = 1; i < templateArgs.Count; i++)
                {
                    _ = qualifiedName.Append(',');
                    _ = qualifiedName.Append(' ');
                    AppendTemplateArgument(templateArgs[i], qualifiedName);
                }
            }

            _ = qualifiedName.Append('>');
        }

        void AppendTemplateParameters(TemplateDecl templateDecl, StringBuilder qualifiedName)
        {
            if (truncateParameters)
            {
                return;
            }

            _ = qualifiedName.Append('<');

            var templateParameters = templateDecl.TemplateParameters;

            if (templateParameters.Any())
            {
                _ = qualifiedName.Append(templateParameters[0].Name);

                for (var i = 1; i < templateParameters.Count; i++)
                {
                    _ = qualifiedName.Append(',');
                    _ = qualifiedName.Append(' ');
                    _ = qualifiedName.Append(templateParameters[i].Name);
                }
            }

            _ = qualifiedName.Append('>');
        }
    }

    private static Expr GetExprAsWritten(Expr expr, bool removeParens)
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

    private uint GetOverloadIndex(CXXMethodDecl cxxMethodDeclToMatch)
    {
        if (!_overloadIndices.TryGetValue(cxxMethodDeclToMatch, out var index))
        {
            var parent = cxxMethodDeclToMatch.Parent;
            Debug.Assert(parent is not null);

            index = GetOverloadIndex(cxxMethodDeclToMatch, parent, baseIndex: 0);
            _overloadIndices.Add(cxxMethodDeclToMatch, index);
        }
        return index;

        uint GetOverloadIndex(CXXMethodDecl cxxMethodDeclToMatch, CXXRecordDecl cxxRecordDecl, uint baseIndex)
        {
            var index = baseIndex;

            foreach (var cxxBaseSpecifier in cxxRecordDecl.Bases)
            {
                var baseCxxRecordDecl = GetRecordDecl(cxxBaseSpecifier);
                index = GetOverloadIndex(cxxMethodDeclToMatch, baseCxxRecordDecl, index);
            }

            foreach (var cxxMethodDecl in cxxRecordDecl.Methods.OrderBy((cxxmd) => cxxmd.VtblIndex))
            {
                if (IsExcluded(cxxMethodDecl))
                {
                    continue;
                }
                else if (cxxMethodDecl == cxxMethodDeclToMatch)
                {
                    break;
                }
                else if (cxxMethodDecl.Name == cxxMethodDeclToMatch.Name)
                {
                    index++;
                }
            }

            return index;
        }
    }

    private CXXRecordDecl GetRecordDecl(CXXBaseSpecifier cxxBaseSpecifier)
    {
        var baseType = cxxBaseSpecifier.Type;

        if (IsType<RecordType>(cxxBaseSpecifier, baseType, out var recordType))
        {
            return (CXXRecordDecl)recordType.Decl;
        }

        AddDiagnostic(DiagnosticLevel.Error, "Failed to retrieve record type for CXX base specifier. Falling back to referenced type.", cxxBaseSpecifier);
        return (CXXRecordDecl)cxxBaseSpecifier.Referenced;
    }

    private string GetRemappedCursorName(NamedDecl namedDecl) => GetRemappedCursorName(namedDecl, out _, skipUsing: false);

    private string GetRemappedCursorName(NamedDecl namedDecl, out string nativeTypeName, bool skipUsing)
    {
        nativeTypeName = GetCursorQualifiedName(namedDecl);

        var name = nativeTypeName;
        var remappedName = GetRemappedName(name, namedDecl, tryRemapOperatorName: true, out var wasRemapped, skipUsing);

        if (wasRemapped)
        {
            return remappedName;
        }

        name = name.Replace("::", ".", StringComparison.Ordinal);
        remappedName = GetRemappedName(name, namedDecl, tryRemapOperatorName: true, out wasRemapped, skipUsing);

        if (wasRemapped)
        {
            return remappedName;
        }


        name = GetCursorQualifiedName(namedDecl, truncateParameters: true);
        remappedName = GetRemappedName(name, namedDecl, tryRemapOperatorName: true, out wasRemapped, skipUsing);

        if (wasRemapped)
        {
            return remappedName;
        }

        name = name.Replace("::", ".", StringComparison.Ordinal);
        remappedName = GetRemappedName(name, namedDecl, tryRemapOperatorName: true, out wasRemapped, skipUsing);

        if (wasRemapped)
        {
            return remappedName;
        }

        name = GetCursorName(namedDecl);
        remappedName = GetRemappedName(name, namedDecl, tryRemapOperatorName: true, out wasRemapped, skipUsing);

        if (wasRemapped)
        {
            return remappedName;
        }

        if (namedDecl is CXXConstructorDecl cxxConstructorDecl)
        {
            var parent = cxxConstructorDecl.Parent;
            Debug.Assert(parent is not null);
            remappedName = GetRemappedCursorName(parent);
        }
        else if (namedDecl is FieldDecl fieldDecl)
        {
            if (name.StartsWith("__AnonymousFieldDecl_", StringComparison.Ordinal))
            {
                remappedName = "Anonymous";

                var parent = fieldDecl.Parent;
                Debug.Assert(parent is not null);

                if (parent.AnonymousFields.Count > 1)
                {
                    var index = parent.AnonymousFields.IndexOf(fieldDecl) + 1;
                    remappedName += index.ToString(CultureInfo.InvariantCulture);
                }
            }
        }
        else if ((namedDecl is RecordDecl recordDecl) && name.StartsWith("__AnonymousRecord_", StringComparison.Ordinal))
        {
            if (recordDecl.Parent is RecordDecl parentRecordDecl)
            {
                remappedName = "_Anonymous";

                var matchingField = parentRecordDecl.Fields.Where((fieldDecl) => fieldDecl.Type.CanonicalType == recordDecl.TypeForDecl.CanonicalType).FirstOrDefault();

                if (matchingField is not null)
                {
                    remappedName = "_";
                    remappedName += GetRemappedCursorName(matchingField);
                }
                else if (parentRecordDecl.AnonymousRecords.Count > 1)
                {
                    var index = parentRecordDecl.AnonymousRecords.IndexOf(recordDecl) + 1;
                    remappedName += index.ToString(CultureInfo.InvariantCulture);
                }

                remappedName += $"_e__{(recordDecl.IsUnion ? "Union" : "Struct")}";
            }
        }

        return remappedName;
    }

    private string GetRemappedName(string name, Cursor? cursor, bool tryRemapOperatorName, out bool wasRemapped, bool skipUsing = false)
        => GetRemappedName(name, cursor, tryRemapOperatorName, out wasRemapped, skipUsing, skipUsingIfNotRemapped: skipUsing);

    private string GetRemappedName(string name, Cursor? cursor, bool tryRemapOperatorName, out bool wasRemapped, bool skipUsing, bool skipUsingIfNotRemapped)
    {
        if (_config.RemappedNames.TryGetValue(name, out var remappedName))
        {
            wasRemapped = true;
            _ = _usedRemappings.Add(name);
            return AddUsingDirectiveIfNeeded(_outputBuilder, remappedName, skipUsing);
        }

        if (name.StartsWith("const ", StringComparison.Ordinal))
        {
            var tmpName = name[6..];

            if (_config.RemappedNames.TryGetValue(tmpName, out remappedName))
            {

                wasRemapped = true;
                _ = _usedRemappings.Add(tmpName);
                return AddUsingDirectiveIfNeeded(_outputBuilder, remappedName, skipUsing);
            }
        }

        remappedName = name;

        if ((cursor is FunctionDecl functionDecl) && tryRemapOperatorName && TryRemapOperatorName(ref remappedName, functionDecl))
        {
            wasRemapped = true;
            // We don't track remapped operators in _usedRemappings
            return AddUsingDirectiveIfNeeded(_outputBuilder, remappedName, skipUsing);
        }

        if ((cursor is CXXBaseSpecifier cxxBaseSpecifier) && remappedName.StartsWith("__AnonymousBase_", StringComparison.Ordinal))
        {
            Debug.Assert(_cxxRecordDeclContext is not null);
            remappedName = "Base";

            if (_cxxRecordDeclContext.Bases.Count > 1)
            {
                var index = _cxxRecordDeclContext.Bases.IndexOf(cxxBaseSpecifier) + 1;
                remappedName += index.ToString(CultureInfo.InvariantCulture);
            }

            wasRemapped = true;
            return AddUsingDirectiveIfNeeded(_outputBuilder, remappedName, skipUsing);
        }

        wasRemapped = false;
        return AddUsingDirectiveIfNeeded(_outputBuilder, remappedName, skipUsingIfNotRemapped);

        string AddUsingDirectiveIfNeeded(IOutputBuilder? outputBuilder, string remappedName, bool skipUsing)
        {
            if (!skipUsing)
            {
                if (NeedsSystemSupportRegex().IsMatch(remappedName))
                {
                    outputBuilder?.EmitSystemSupport();
                }

                var namespaceName = GetNamespace(remappedName);
                AddUsingDirective(outputBuilder, namespaceName);
            }

            return remappedName;
        }
    }

    private string GetRemappedTypeName(Cursor? cursor, Cursor? context, Type type, out string nativeTypeName, bool skipUsing = false, bool ignoreTransparentStructsWhereRequired = false)
    {
        var name = GetTypeName(cursor, context, type, ignoreTransparentStructsWhereRequired, out nativeTypeName);

        var nameToCheck = nativeTypeName;
        var remappedName = GetRemappedName(nameToCheck, cursor, tryRemapOperatorName: false, out var wasRemapped, skipUsing, skipUsingIfNotRemapped: true);

        if (!wasRemapped)
        {
            nameToCheck = nameToCheck.Replace("::", ".", StringComparison.Ordinal);
            remappedName = GetRemappedName(nameToCheck, cursor, tryRemapOperatorName: false, out wasRemapped, skipUsing, skipUsingIfNotRemapped: true);

            if (!wasRemapped)
            {
                nameToCheck = name;
                remappedName = GetRemappedName(nameToCheck, cursor, tryRemapOperatorName: false, out wasRemapped, skipUsing);

                if (!wasRemapped)
                {
                    if (IsTypeConstantOrIncompleteArray(cursor, type, out var arrayType) && IsType<RecordType>(cursor, arrayType.ElementType))
                    {
                        type = arrayType.ElementType;
                    }

                    if (IsType<RecordType>(cursor, type, out var recordType) && remappedName.StartsWith("__AnonymousRecord_", StringComparison.Ordinal))
                    {
                        var recordDecl = recordType.Decl;
                        remappedName = "_Anonymous";

                        if (recordDecl.Parent is RecordDecl parentRecordDecl)
                        {
                            var matchingField = parentRecordDecl.Fields.Where((fieldDecl) => fieldDecl.Type.CanonicalType == recordType).FirstOrDefault();

                            if (matchingField is not null)
                            {
                                remappedName = "_";
                                remappedName += GetRemappedCursorName(matchingField);
                            }
                            else
                            {
                                var index = 0;

                                if (parentRecordDecl.AnonymousRecords.Count > 1)
                                {
                                    index = parentRecordDecl.AnonymousRecords.IndexOf(cursor) + 1;
                                }

                                while (parentRecordDecl.IsAnonymousStructOrUnion && (parentRecordDecl.IsUnion == recordType.Decl.IsUnion))
                                {
                                    index += 1;

                                    if (parentRecordDecl.Parent is RecordDecl parentRecordDeclParent)
                                    {
                                        if (parentRecordDeclParent.AnonymousRecords.Count > 0)
                                        {
                                            index += parentRecordDeclParent.AnonymousRecords.Count - 1;
                                        }
                                        parentRecordDecl = parentRecordDeclParent;
                                    }
                                }

                                if (index != 0)
                                {
                                    remappedName += index.ToString(CultureInfo.InvariantCulture);
                                }
                            }
                        }

                        remappedName += $"_e__{(recordDecl.IsUnion ? "Union" : "Struct")}";
                    }
                    else if (IsType<EnumType>(cursor, type, out var enumType) && remappedName.StartsWith("__AnonymousEnum_", StringComparison.Ordinal))
                    {
                        remappedName = GetRemappedTypeName(enumType.Decl, context: null, enumType.Decl.IntegerType, out _, skipUsing);
                    }
                    else if (cursor is EnumDecl enumDecl)
                    {
                        // Even though some types have entries with names like *_FORCE_DWORD or *_FORCE_UINT
                        // MSVC and Clang both still treat this as "signed" values and thus we don't want
                        // to specially handle it as uint, as that can break ABI handling on some platforms.

                        WithType(enumDecl, ref remappedName, ref nativeTypeName);
                    }
                }
            }
        }

        if (string.IsNullOrWhiteSpace(nativeTypeName))
        {
            // When we have an empty native type name, it means the original
            // name is the same as the native type name and no adjustments
            // were made. In order to ensure things are correctly preserved
            // we need to ensure its propagated back here so the below comparison
            // works and we don't end up comparing "empty" vs "remapped"
            nativeTypeName = name;
        }

        if (IsNativeTypeNameEquivalent(nativeTypeName, remappedName))
        {
            // Empty the native type name if its equivalent to the new name
            nativeTypeName = string.Empty;
        }

        return remappedName;
    }

    private static string GetSourceRangeContents(CXTranslationUnit translationUnit, CXSourceRange sourceRange)
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

    private string GetTargetTypeName(Cursor cursor, out string nativeTypeName)
    {
        var targetTypeName = "";
        nativeTypeName = "";

        if (cursor is Decl decl)
        {
            if (decl is EnumConstantDecl enumConstantDecl)
            {
                targetTypeName = enumConstantDecl.DeclContext is EnumDecl enumDecl
                               ? GetRemappedTypeName(enumDecl, context: null, enumDecl.IntegerType, out nativeTypeName)
                               : GetRemappedTypeName(enumConstantDecl, context: null, enumConstantDecl.Type, out nativeTypeName);
            }
            else if (decl is TypeDecl previousTypeDecl)
            {
                targetTypeName = GetRemappedTypeName(previousTypeDecl, context: null, previousTypeDecl.TypeForDecl, out nativeTypeName);
            }
            else if (decl is VarDecl varDecl)
            {
                if (varDecl is ParmVarDecl parmVarDecl)
                {
                    targetTypeName = GetRemappedTypeName(parmVarDecl, context: null, parmVarDecl.Type, out nativeTypeName);

                    if (!_config.GenerateDisableRuntimeMarshalling && (parmVarDecl.ParentFunctionOrMethod is FunctionDecl functionDecl) && (((functionDecl is CXXMethodDecl cxxMethodDecl) && cxxMethodDecl.IsVirtual) || (functionDecl.Body is null)) && targetTypeName.Equals("bool", StringComparison.Ordinal))
                    {
                        // bool is not blittable when DisableRuntimeMarshalling is not specified, so we shouldn't use it for P/Invoke signatures
                        targetTypeName = "byte";
                        nativeTypeName = string.IsNullOrWhiteSpace(nativeTypeName) ? "bool" : nativeTypeName;
                    }
                }
                else
                {
                    var type = varDecl.Type;
                    var cursorName = GetCursorName(varDecl);

                    if (cursorName.StartsWith("ClangSharpMacro_", StringComparison.Ordinal))
                    {
                        cursorName = cursorName["ClangSharpMacro_".Length..];

                        if (_config.WithTypes.TryGetValue(cursorName, out targetTypeName))
                        {
                            return targetTypeName;
                        }

                        type = varDecl.Init.Type;
                    }

                    targetTypeName = GetRemappedTypeName(varDecl, context: null, type, out nativeTypeName);
                }
            }

        }
        else if ((cursor is Expr expr) && (expr is not MemberExpr))
        {
            targetTypeName = GetRemappedTypeName(expr, context: null, expr.Type, out nativeTypeName);
        }

        return targetTypeName;
    }

    private string GetTypeName(Cursor? cursor, Cursor? context, Type type, bool ignoreTransparentStructsWhereRequired, out string nativeTypeName)
    {
        if (_typeNames.TryGetValue((cursor, context, type), out var result))
        {
            nativeTypeName = result.nativeTypeName;
            return result.typeName;
        }
        else if (IsType<TagType>(cursor, type, out var tagType) && tagType.Decl.Handle.IsAnonymous)
        {
            // In order to avoid minor path differences, casing, and other deltas across different
            // invocations of the tool, we want to use the "built" anonymous name so we get a more
            // minimal but still accurate set of information embedded in the output.

            result.typeName = GetAnonymousName(tagType.Decl, tagType.KindSpelling);
            result.nativeTypeName = result.typeName;

            _typeNames[(cursor, context, type)] = result;

            nativeTypeName = result.nativeTypeName;
            return result.typeName;
        }
        else
        {
            return GetTypeName(cursor, context, type, type, ignoreTransparentStructsWhereRequired, out nativeTypeName);
        }
    }

    private string GetTypeName(Cursor? cursor, Cursor? context, Type rootType, Type type, bool ignoreTransparentStructsWhereRequired, out string nativeTypeName)
    {
        if (!_typeNames.TryGetValue((cursor, context, type), out var result))
        {
            result.typeName = type.AsString.NormalizePath()
                                           .Replace("unnamed enum at", "anonymous enum at", StringComparison.Ordinal)
                                           .Replace("unnamed struct at", "anonymous struct at", StringComparison.Ordinal)
                                           .Replace("unnamed union at", "anonymous union at", StringComparison.Ordinal);

            result.nativeTypeName = result.typeName;

            // We don't want to handle these using IsType because we need to specially
            // handle cases like TypedefType at each level of the type hierarchy

            if (type is ArrayType arrayType)
            {
                result.typeName = GetRemappedTypeName(cursor, context, arrayType.ElementType, out _, skipUsing: true, ignoreTransparentStructsWhereRequired);

                if (cursor is FunctionDecl or ParmVarDecl)
                {
                    result.typeName += '*';
                }
            }
            else if (type is AttributedType attributedType)
            {
                result.typeName = GetTypeName(cursor, context, rootType, attributedType.ModifiedType, ignoreTransparentStructsWhereRequired, out _);
            }
            else if (type is BuiltinType)
            {
                switch (type.Kind)
                {
                    case CXType_Void:
                    {
                        result.typeName = (cursor is null) ? "Void" : "void";
                        break;
                    }

                    case CXType_Bool:
                    {
                        result.typeName = (cursor is null) ? "Boolean" : "bool";
                        break;
                    }

                    case CXType_Char_U:
                    case CXType_UChar:
                    {
                        result.typeName = (cursor is null) ? "Byte" : "byte";
                        break;
                    }

                    case CXType_Char16:
                    {
                        if (_config.GenerateDisableRuntimeMarshalling)
                        {
                            result.typeName = (cursor is null) ? "Char" : "char";
                            break;
                        }
                        goto case CXType_UShort;
                    }

                    case CXType_UShort:
                    {
                        result.typeName = (cursor is null) ? "UInt16" : "ushort";
                        break;
                    }

                    case CXType_UInt:
                    {
                        result.typeName = (cursor is null) ? "UInt32" : "uint";
                        break;
                    }

                    case CXType_ULong:
                    {
                        if (_config.GenerateUnixTypes)
                        {
                            result.typeName = _config.ExcludeNIntCodegen ? "UIntPtr" : "nuint";
                        }
                        else
                        {
                            goto case CXType_UInt;
                        }
                        break;
                    }

                    case CXType_ULongLong:
                    {
                        result.typeName = (cursor is null) ? "UInt64" : "ulong";
                        break;
                    }

                    case CXType_Char_S:
                    case CXType_SChar:
                    {
                        result.typeName = (cursor is null) ? "SByte" : "sbyte";
                        break;
                    }

                    case CXType_WChar:
                    {
                        if (_config.GenerateUnixTypes)
                        {
                            goto case CXType_UInt;
                        }
                        else
                        {
                            goto case CXType_Char16;
                        }
                    }

                    case CXType_Short:
                    {
                        result.typeName = (cursor is null) ? "Int16" : "short";
                        break;
                    }

                    case CXType_Int:
                    {
                        result.typeName = (cursor is null) ? "Int32" : "int";
                        break;
                    }

                    case CXType_Long:
                    {
                        if (_config.GenerateUnixTypes)
                        {
                            result.typeName = _config.ExcludeNIntCodegen ? "IntPtr" : "nint";
                        }
                        else
                        {
                            goto case CXType_Int;
                        }
                        break;
                    }

                    case CXType_LongLong:
                    {
                        result.typeName = (cursor is null) ? "Int64" : "long";
                        break;
                    }

                    case CXType_Float:
                    {
                        result.typeName = (cursor is null) ? "Single" : "float";
                        break;
                    }

                    case CXType_Double:
                    {
                        result.typeName = (cursor is null) ? "Double" : "double";
                        break;
                    }

                    case CXType_NullPtr:
                    {
                        result.typeName = "null";
                        break;
                    }

                    default:
                    {
                        AddDiagnostic(DiagnosticLevel.Warning, $"Unsupported builtin type: '{type.KindSpelling}'. Falling back '{result.typeName}'.", cursor);
                        break;
                    }
                }
            }
            else if (type is DecltypeType decltypeType)
            {
                result.typeName = GetTypeName(cursor, context, rootType, decltypeType.UnderlyingType, ignoreTransparentStructsWhereRequired, out _);
            }
            else if (type is DeducedType deducedType)
            {
                result.typeName = GetTypeName(cursor, context, rootType, deducedType.GetDeducedType, ignoreTransparentStructsWhereRequired, out _);
            }
            else if (type is DependentNameType dependentNameType)
            {
                if (dependentNameType.IsSugared)
                {
                    result.typeName = GetTypeName(cursor, context, rootType, dependentNameType.Desugar, ignoreTransparentStructsWhereRequired, out _);
                }
                else
                {
                    // The default name should be correct
                }
            }
            else if (type is ElaboratedType elaboratedType)
            {
                result.typeName = GetTypeName(cursor, context, rootType, elaboratedType.NamedType, ignoreTransparentStructsWhereRequired, out var nativeNamedTypeName);

                if (!string.IsNullOrWhiteSpace(nativeNamedTypeName) &&
                    !result.nativeTypeName.StartsWith("const ", StringComparison.Ordinal) &&
                    !result.nativeTypeName.StartsWith("enum ", StringComparison.Ordinal) &&
                    !result.nativeTypeName.StartsWith("struct ", StringComparison.Ordinal) &&
                    !result.nativeTypeName.StartsWith("union ", StringComparison.Ordinal))
                {
                    result.nativeTypeName = nativeNamedTypeName;
                }
            }
            else if (type is FunctionType functionType)
            {
                result.typeName = GetTypeNameForPointeeType(cursor, context, rootType, functionType, ignoreTransparentStructsWhereRequired, out _, out _);
            }
            else if (type is InjectedClassNameType injectedClassNameType)
            {
                result.typeName = GetTypeName(cursor, context, rootType, injectedClassNameType.InjectedTST, ignoreTransparentStructsWhereRequired, out _);
            }
            else if (type is PackExpansionType packExpansionType)
            {
                result.typeName = GetTypeName(cursor, context, rootType, packExpansionType.Pattern, ignoreTransparentStructsWhereRequired, out _);
            }
            else if (type is PointerType pointerType)
            {
                result.typeName = GetTypeNameForPointeeType(cursor, context, rootType, pointerType.PointeeType, ignoreTransparentStructsWhereRequired, out var nativePointeeTypeName, out var isAdjusted);

                if (isAdjusted)
                {
                    result.nativeTypeName = $"{nativePointeeTypeName} *";
                }
            }
            else if (type is ReferenceType referenceType)
            {
                result.typeName = GetTypeNameForPointeeType(cursor, context, rootType, referenceType.PointeeType, ignoreTransparentStructsWhereRequired, out var nativePointeeTypeName, out var isAdjusted);

                if (isAdjusted)
                {
                    result.nativeTypeName = $"{nativePointeeTypeName} &";
                }
            }
            else if (type is SubstTemplateTypeParmType substTemplateTypeParmType)
            {
                result.typeName = GetTypeName(cursor, context, rootType, substTemplateTypeParmType.ReplacementType, ignoreTransparentStructsWhereRequired, out _);
            }
            else if (type is TagType tagType)
            {
                if (tagType.Decl.Handle.IsAnonymous)
                {
                    // In order to avoid minor path differences, casing, and other deltas across different
                    // invocations of the tool, we want to use the "built" anonymous name so we get a more
                    // minimal but still accurate set of information embedded in the output.

                    result.typeName = GetAnonymousName(tagType.Decl, tagType.KindSpelling);
                    result.nativeTypeName = result.typeName;
                }
                else if (tagType.Handle.IsConstQualified)
                {
                    result.typeName = GetTypeName(cursor, context, rootType, tagType.Decl.TypeForDecl, ignoreTransparentStructsWhereRequired, out _);
                }
                else
                {
                    // The default name should be correct for C++, but C may have a prefix we need to strip

                    if (result.typeName.StartsWith("enum ", StringComparison.Ordinal))
                    {
                        result.typeName = result.typeName[5..];
                    }
                    else if (result.typeName.StartsWith("struct ", StringComparison.Ordinal))
                    {
                        result.typeName = result.typeName[7..];
                    }
                    else if (result.typeName.StartsWith("union ", StringComparison.Ordinal))
                    {
                        result.typeName = result.typeName[6..];
                    }
                }

                if (result.typeName.Contains("::", StringComparison.Ordinal))
                {
                    result.typeName = result.typeName.Split(s_doubleColonSeparator, StringSplitOptions.RemoveEmptyEntries).Last();
                    result.typeName = GetRemappedName(result.typeName, cursor, tryRemapOperatorName: false, out _, skipUsing: true);
                }
            }
            else if (type is TemplateSpecializationType templateSpecializationType)
            {
                var nameBuilder = new StringBuilder();

                var templateTypeDecl = IsType<RecordType>(cursor, templateSpecializationType, out var recordType)
                                     ? recordType.Decl
                                     : (NamedDecl)templateSpecializationType.TemplateName.AsTemplateDecl;

                var templateTypeDeclName = GetRemappedCursorName(templateTypeDecl, out _, skipUsing: true);
                var isStdAtomic = false;

                if (templateTypeDeclName.Equals("atomic", StringComparison.Ordinal))
                {
                    isStdAtomic = (templateTypeDecl.Parent is NamespaceDecl namespaceDecl) && namespaceDecl.IsStdNamespace;
                }

                if (!isStdAtomic)
                {
                    _ = nameBuilder.Append(templateTypeDeclName);
                    _ = nameBuilder.Append('<');
                }
                else
                {
                    _ = nameBuilder.Append("volatile ");
                }

                var shouldWritePrecedingComma = false;

                foreach (var arg in templateSpecializationType.Args)
                {
                    if (shouldWritePrecedingComma)
                    {
                        _ = nameBuilder.Append(',');
                        _ = nameBuilder.Append(' ');
                    }

                    var typeName = "";

                    switch (arg.Kind)
                    {
                        case CXTemplateArgumentKind_Type:
                        {
                            typeName = GetRemappedTypeName(cursor, context: null, arg.AsType, out var nativeAsTypeName, skipUsing: true);
                            break;
                        }

                        case CXTemplateArgumentKind_Expression:
                        {
                            var oldOutputBuilder = _outputBuilder;
                            _outputBuilder = new CSharpOutputBuilder("ClangSharp_TemplateSpecializationType_AsExpr", this);

                            Visit(arg.AsExpr);
                            typeName = _outputBuilder.ToString() ?? "";

                            _outputBuilder = oldOutputBuilder;
                            break;
                        }

                        default:
                        {
                            typeName = result.typeName;
                            AddDiagnostic(DiagnosticLevel.Warning, $"Unsupported template argument kind: '{arg.Kind}'. Falling back '{result.typeName}'.", cursor);
                            break;
                        }
                    }

                    if (!_config.GenerateDisableRuntimeMarshalling && typeName.Equals("bool", StringComparison.Ordinal))
                    {
                        // bool is not blittable when DisableRuntimeMarshalling is not specified, so we shouldn't use it for P/Invoke signatures
                        typeName = "byte";
                    }

                    if (typeName.EndsWith('*') || typeName.Contains("delegate*", StringComparison.Ordinal))
                    {
                        // Pointers are not yet supported as generic arguments; remap to IntPtr
                        typeName = "IntPtr";
                        _outputBuilder?.EmitSystemSupport();
                    }

                    _ = nameBuilder.Append(typeName);

                    shouldWritePrecedingComma = true;
                }

                if (!isStdAtomic)
                {
                    _ = nameBuilder.Append('>');
                }

                result.typeName = nameBuilder.ToString();
            }
            else if (type is TemplateTypeParmType templateTypeParmType)
            {
                if (templateTypeParmType.IsSugared)
                {
                    result.typeName = GetTypeName(cursor, context, rootType, templateTypeParmType.Desugar, ignoreTransparentStructsWhereRequired, out _);
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

                var remappedName = GetRemappedName(result.typeName, cursor, tryRemapOperatorName: false, out var wasRemapped, skipUsing: true);
                result.typeName = wasRemapped ? remappedName : GetTypeName(cursor, context, rootType, typedefType.Decl.UnderlyingType, ignoreTransparentStructsWhereRequired, out _);
            }
            else if (type is UsingType usingType)
            {
                result.typeName = GetTypeName(cursor, context, rootType, usingType.Desugar, ignoreTransparentStructsWhereRequired, out _);
            }
            else
            {
                AddDiagnostic(DiagnosticLevel.Warning, $"Unsupported type: '{type.TypeClass}'. Falling back '{result.typeName}'.", cursor);
            }

            Debug.Assert(!string.IsNullOrWhiteSpace(result.typeName));
            Debug.Assert(!string.IsNullOrWhiteSpace(result.nativeTypeName));

            if (IsNativeTypeNameEquivalent(result.nativeTypeName, result.typeName))
            {
                result.nativeTypeName = string.Empty;
            }

            _typeNames[(cursor, context, type)] = result;
        }

        nativeTypeName = result.nativeTypeName;
        return result.typeName;
    }

    private string GetTypeNameForPointeeType(Cursor? cursor, Cursor? context, Type rootType, Type pointeeType, bool ignoreTransparentStructsWhereRequired, out string nativePointeeTypeName, out bool isAdjusted)
    {
        var name = pointeeType.AsString;

        nativePointeeTypeName = name;
        isAdjusted = false;

        // We don't want to handle these using IsType because we need to specially
        // handle cases like TypedefType at each level of the type hierarchy

        if (pointeeType is AttributedType attributedType)
        {
            name = GetTypeNameForPointeeType(cursor, context, rootType, attributedType.ModifiedType, ignoreTransparentStructsWhereRequired, out var nativeModifiedTypeName, out isAdjusted);
        }
        else if (pointeeType is ElaboratedType elaboratedType)
        {
            name = GetTypeNameForPointeeType(cursor, context, rootType, elaboratedType.NamedType, ignoreTransparentStructsWhereRequired, out var nativeNamedTypeName, out isAdjusted);

            if (!string.IsNullOrWhiteSpace(nativeNamedTypeName) &&
                !nativePointeeTypeName.StartsWith("const ", StringComparison.Ordinal) &&
                !nativePointeeTypeName.StartsWith("enum ", StringComparison.Ordinal) &&
                !nativePointeeTypeName.StartsWith("struct ", StringComparison.Ordinal) &&
                !nativePointeeTypeName.StartsWith("union ", StringComparison.Ordinal))
            {
                nativePointeeTypeName = nativeNamedTypeName;
                isAdjusted = true;
            }
        }
        else if (pointeeType is FunctionType functionType)
        {
            if (!_config.ExcludeFnptrCodegen && IsType<FunctionProtoType>(cursor, functionType, out var functionProtoType))
            {
                _config.ExcludeFnptrCodegen = true;
                var callConv = GetCallingConvention(cursor, context, rootType);
                _config.ExcludeFnptrCodegen = false;

                var needsReturnFixup = false;
                var returnTypeName = GetRemappedTypeName(cursor, context: null, functionType.ReturnType, out _, skipUsing: true);

                if (!_config.GenerateDisableRuntimeMarshalling && returnTypeName.Equals("bool", StringComparison.Ordinal))
                {
                    // bool is not blittable when DisableRuntimeMarshalling is not specified, so we shouldn't use it for P/Invoke signatures
                    returnTypeName = "byte";
                }

                var nameBuilder = new StringBuilder();
                _ = nameBuilder.Append("delegate");
                _ = nameBuilder.Append('*');

                var isMacroDefinitionRecord = (cursor is VarDecl varDecl) && GetCursorName(varDecl).StartsWith("ClangSharpMacro_", StringComparison.Ordinal);

                if (!isMacroDefinitionRecord)
                {
                    _ = nameBuilder.Append(" unmanaged");
                    var hasSuppressGCTransition = HasSuppressGCTransition(cursor);

                    if (callConv != CallConv.Winapi)
                    {
                        _ = nameBuilder.Append('[');
                        _ = nameBuilder.Append(callConv.AsString(true));

                        if (hasSuppressGCTransition)
                        {
                            _ = nameBuilder.Append(", SuppressGCTransition");
                        }
                        _ = nameBuilder.Append(']');
                    }
                    else if (hasSuppressGCTransition)
                    {
                        _ = nameBuilder.Append("[SuppressGCTransition]");
                    }
                }

                _ = nameBuilder.Append('<');

                if ((cursor is CXXMethodDecl cxxMethodDecl) && (context is CXXRecordDecl cxxRecordDecl))
                {
                    var cxxRecordDeclName = GetRemappedCursorName(cxxRecordDecl);
                    needsReturnFixup = cxxMethodDecl.IsVirtual && NeedsReturnFixup(cxxMethodDecl);

                    _ = nameBuilder.Append(EscapeName(cxxRecordDeclName));
                    _ = nameBuilder.Append('*');
                    _ = nameBuilder.Append(',');
                    _ = nameBuilder.Append(' ');

                    if (needsReturnFixup)
                    {
                        _ = nameBuilder.Append(returnTypeName);
                        _ = nameBuilder.Append('*');
                        _ = nameBuilder.Append(',');
                        _ = nameBuilder.Append(' ');
                    }
                }

                IEnumerable<Type> paramTypes = functionProtoType.ParamTypes;

                if (isMacroDefinitionRecord)
                {
                    Debug.Assert(cursor is not null);
                    varDecl = (VarDecl)cursor;

                    if (IsStmtAsWritten<DeclRefExpr>(varDecl.Init, out var declRefExpr, removeParens: true) && (declRefExpr.Decl is FunctionDecl functionDecl))
                    {
                        cursor = functionDecl;
                        paramTypes = functionDecl.Parameters.Select((param) => param.Type);
                        returnTypeName = GetRemappedTypeName(cursor, context: null, functionDecl.ReturnType, out _, skipUsing: true);
                    }
                }

                foreach (var paramType in paramTypes)
                {
                    var typeName = GetRemappedTypeName(cursor, context: null, paramType, out _, skipUsing: true);

                    if (!_config.GenerateDisableRuntimeMarshalling && typeName.Equals("bool", StringComparison.Ordinal))
                    {
                        // bool is not blittable when DisableRuntimeMarshalling is not specified, so we shouldn't use it for P/Invoke signatures
                        typeName = "byte";
                    }

                    _ = nameBuilder.Append(typeName);
                    _ = nameBuilder.Append(',');
                    _ = nameBuilder.Append(' ');
                }

                if (!needsReturnFixup && ignoreTransparentStructsWhereRequired && _config.WithTransparentStructs.TryGetValue(returnTypeName, out var transparentStruct))
                {
                    _ = nameBuilder.Append(transparentStruct.Name);
                }
                else
                {
                    _ = nameBuilder.Append(returnTypeName);

                    if (needsReturnFixup)
                    {
                        _ = nameBuilder.Append('*');
                    }
                }

                _ = nameBuilder.Append('>');
                name = nameBuilder.ToString();
            }
            else
            {
                name = "IntPtr";
            }
        }
        else if (pointeeType is TypedefType typedefType)
        {
            // We check remapped names here so that types that have variable sizes
            // can be treated correctly. Otherwise, they will resolve to a particular
            // platform size, based on whatever parameters were passed into clang.

            var remappedName = GetRemappedName(name, cursor, tryRemapOperatorName: false, out var wasRemapped, skipUsing: true);

            if (wasRemapped)
            {
                name = remappedName;
                name += '*';
            }
            else
            {
                name = GetTypeNameForPointeeType(cursor, context, rootType, typedefType.Decl.UnderlyingType, ignoreTransparentStructsWhereRequired, out var nativeUnderlyingTypeName, out isAdjusted);
            }
        }
        else
        {
            // Otherwise fields that point at anonymous structs get the wrong name
            name = GetRemappedTypeName(cursor, context, pointeeType, out nativePointeeTypeName, skipUsing: true);
            name += '*';
        }

        return name;
    }

    private void GetTypeSize(Cursor cursor, Type type, ref long alignment32, ref long alignment64, out long size32, out long size64)
    {
        var has8BytePrimitiveField = false;
        GetTypeSize(cursor, type, ref alignment32, ref alignment64, ref has8BytePrimitiveField, out size32, out size64);
    }

    private void GetTypeSize(Cursor cursor, Type type, ref long alignment32, ref long alignment64, ref bool has8BytePrimitiveField, out long size32, out long size64)
    {
        size32 = 0;
        size64 = 0;

        // We don't want to handle these using IsType because we need to specially
        // handle cases like TypedefType at each level of the type hierarchy

        if (type is ArrayType arrayType)
        {
            if (IsTypeConstantOrIncompleteArray(cursor, type))
            {
                var count = Math.Max((arrayType as ConstantArrayType)?.Size ?? 0, 1);
                GetTypeSize(cursor, arrayType.ElementType, ref alignment32, ref alignment64, ref has8BytePrimitiveField, out var elementSize32, out var elementSize64);

                size32 = elementSize32 * Math.Max(count, 1);
                size64 = elementSize64 * Math.Max(count, 1);

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
            GetTypeSize(cursor, attributedType.ModifiedType, ref alignment32, ref alignment64, ref has8BytePrimitiveField, out size32, out size64);
        }
        else if (type is BuiltinType)
        {
            switch (type.Kind)
            {
                case CXType_Bool:
                case CXType_Char_U:
                case CXType_UChar:
                case CXType_Char_S:
                case CXType_SChar:
                {
                    size32 = 1;
                    size64 = 1;
                    break;
                }

                case CXType_UShort:
                case CXType_Short:
                {
                    size32 = 2;
                    size64 = 2;
                    break;
                }

                case CXType_UInt:
                case CXType_Int:
                case CXType_Float:
                {
                    size32 = 4;
                    size64 = 4;
                    break;
                }

                case CXType_ULong:
                case CXType_Long:
                {
                    if (_config.GenerateUnixTypes)
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
                    else
                    {
                        goto case CXType_UInt;
                    }
                    break;
                }

                case CXType_ULongLong:
                case CXType_LongLong:
                case CXType_Double:
                {
                    size32 = 8;
                    size64 = 8;

                    if (alignment32 == -1)
                    {
                        alignment32 = 8;
                    }

                    if (alignment64 == -1)
                    {
                        alignment64 = 8;
                    }

                    has8BytePrimitiveField = true;
                    break;
                }

                case CXType_WChar:
                {
                    if (_config.GenerateUnixTypes)
                    {
                        goto case CXType_Int;
                    }
                    else
                    {
                        goto case CXType_UShort;
                    }
                }

                default:
                {
                    AddDiagnostic(DiagnosticLevel.Error, $"Unsupported builtin type: '{type.KindSpelling}.", cursor);
                    break;
                }
            }
        }
        else if (type is DecltypeType decltypeType)
        {
            GetTypeSize(cursor, decltypeType.UnderlyingType, ref alignment32, ref alignment64, ref has8BytePrimitiveField, out size32, out size64);
        }
        else if (type is ElaboratedType elaboratedType)
        {
            GetTypeSize(cursor, elaboratedType.NamedType, ref alignment32, ref alignment64, ref has8BytePrimitiveField, out size32, out size64);
        }
        else if (type is EnumType enumType)
        {
            GetTypeSize(cursor, enumType.Decl.IntegerType, ref alignment32, ref alignment64, ref has8BytePrimitiveField, out size32, out size64);
        }
        else if (type is FunctionType or PointerType or ReferenceType)
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
        else if (type is InjectedClassNameType)
        {
            // Nothing to handle
        }
        else if (type is RecordType recordType)
        {
            var recordTypeAlignOf = Math.Min(recordType.Handle.AlignOf, 8);

            if (alignment32 == -1)
            {
                alignment32 = recordTypeAlignOf;
            }

            if (alignment64 == -1)
            {
                alignment64 = recordTypeAlignOf;
            }

            long maxFieldAlignment32 = -1;
            long maxFieldAlignment64 = -1;

            long maxFieldSize32 = 0;
            long maxFieldSize64 = 0;

            var anyFieldIs8BytePrimitive = false;

            if (recordType.Decl is CXXRecordDecl cxxRecordDecl)
            {
                if (HasVtbl(cxxRecordDecl, out _))
                {
                    size32 += 4;
                    size64 += 8;

                    if (alignment32 < 4)
                    {
                        alignment32 = Math.Max(Math.Min(alignment32, 4), 1);
                    }

                    if (alignment64 < 4)
                    {
                        alignment64 = Math.Max(Math.Min(alignment32, 8), 1);
                    }

                    maxFieldSize32 = Math.Max(maxFieldSize32, 4);
                    maxFieldSize64 = Math.Max(maxFieldSize64, 8);

                    maxFieldAlignment32 = Math.Max(maxFieldSize32, 4);
                    maxFieldAlignment64 = Math.Max(maxFieldSize64, 8);
                }
                else
                {
                    foreach (var baseCXXRecordDecl in cxxRecordDecl.Bases)
                    {
                        long fieldAlignment32 = -1;
                        long fieldAlignment64 = -1;

                        GetTypeSize(baseCXXRecordDecl, baseCXXRecordDecl.Type, ref fieldAlignment32, ref fieldAlignment64, ref anyFieldIs8BytePrimitive, out var fieldSize32, out var fieldSize64);

                        if ((fieldAlignment32 == -1) || (alignment32 < 4))
                        {
                            fieldAlignment32 = Math.Max(Math.Min(alignment32, fieldSize32), 1);
                        }

                        if ((fieldAlignment64 == -1) || (alignment64 < 4))
                        {
                            fieldAlignment64 = Math.Max(Math.Min(alignment64, fieldSize64), 1);
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

            var bitfieldPreviousSize32 = 0L;
            var bitfieldPreviousSize64 = 0L;
            var bitfieldRemainingBits32 = 0L;
            var bitfieldRemainingBits64 = 0L;

            foreach (var fieldDecl in recordType.Decl.Fields)
            {
                long fieldAlignment32 = -1;
                long fieldAlignment64 = -1;

                GetTypeSize(fieldDecl, fieldDecl.Type, ref fieldAlignment32, ref fieldAlignment64, ref anyFieldIs8BytePrimitive, out var fieldSize32, out var fieldSize64);

                var ignoreFieldSize32 = false;
                var ignoreFieldSize64 = false;

                if (fieldDecl.IsBitField)
                {
                    if (fieldSize32 != bitfieldPreviousSize32)
                    {
                        bitfieldRemainingBits32 = fieldSize32 * 8;
                        bitfieldPreviousSize32 = fieldSize32;
                        bitfieldRemainingBits32 -= fieldDecl.BitWidthValue;
                    }
                    else if (fieldDecl.BitWidthValue > bitfieldRemainingBits32)
                    {
                        if (bitfieldRemainingBits32 != bitfieldRemainingBits64)
                        {
                            ignoreFieldSize32 = true;
                        }

                        bitfieldRemainingBits32 = fieldSize32 * 8;
                        bitfieldPreviousSize32 = fieldSize32;
                        bitfieldRemainingBits32 -= fieldDecl.BitWidthValue;
                    }
                    else
                    {
                        bitfieldPreviousSize32 = fieldSize32;
                        bitfieldRemainingBits32 -= fieldDecl.BitWidthValue;
                        ignoreFieldSize32 = true;
                    }

                    if ((fieldSize64 != bitfieldPreviousSize64) || (fieldDecl.BitWidthValue > bitfieldRemainingBits64))
                    {
                        bitfieldRemainingBits64 = fieldSize64 * 8;
                        bitfieldPreviousSize64 = fieldSize64;
                        bitfieldRemainingBits64 -= fieldDecl.BitWidthValue;
                    }
                    else
                    {
                        bitfieldPreviousSize64 = fieldSize64;
                        bitfieldRemainingBits64 -= fieldDecl.BitWidthValue;
                        ignoreFieldSize64 = true;
                    }
                }

                if (!ignoreFieldSize32)
                {
                    if ((fieldAlignment32 == -1) || (alignment32 < 4))
                    {
                        fieldAlignment32 = Math.Max(Math.Min(alignment32, fieldSize32), 1);
                    }

                    if ((size32 % fieldAlignment32) != 0)
                    {
                        size32 += fieldAlignment32 - (size32 % fieldAlignment32);
                    }

                    size32 += fieldSize32;
                    maxFieldAlignment32 = Math.Max(maxFieldAlignment32, fieldAlignment32);
                    maxFieldSize32 = Math.Max(maxFieldSize32, fieldSize32);
                }

                if (!ignoreFieldSize64)
                {
                    if ((fieldAlignment64 == -1) || (alignment64 < 4))
                    {
                        fieldAlignment64 = Math.Max(Math.Min(alignment64, fieldSize64), 1);
                    }

                    if ((size64 % fieldAlignment64) != 0)
                    {
                        size64 += fieldAlignment64 - (size64 % fieldAlignment64);
                    }

                    size64 += fieldSize64;
                    maxFieldAlignment64 = Math.Max(maxFieldAlignment64, fieldAlignment64);
                    maxFieldSize64 = Math.Max(maxFieldSize64, fieldSize64);
                }
            }

            if ((alignment32 == 8) && !anyFieldIs8BytePrimitive)
            {
                alignment32 = Math.Min(alignment32, maxFieldAlignment32);
            }

            if ((alignment64 == 4) && !anyFieldIs8BytePrimitive)
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

            has8BytePrimitiveField |= anyFieldIs8BytePrimitive;
        }
        else if (type is TypedefType typedefType)
        {
            // We check remapped names here so that types that have variable sizes
            // can be treated correctly. Otherwise, they will resolve to a particular
            // platform size, based on whatever parameters were passed into clang.

            var name = GetTypeName(cursor, context: null, type, ignoreTransparentStructsWhereRequired: false, out _);
            var remappedName = GetRemappedTypeName(cursor, context: null, type, out _, skipUsing: true, ignoreTransparentStructsWhereRequired: false);

            if ((remappedName == name) && _config.WithTransparentStructs.TryGetValue(remappedName, out var transparentStruct) && (transparentStruct.Name.Equals("long", StringComparison.Ordinal) || transparentStruct.Name.Equals("ulong", StringComparison.Ordinal)))
            {
                size32 = 8;
                size64 = 8;

                if (alignment32 == -1)
                {
                    alignment32 = 8;
                }

                if (alignment64 == -1)
                {
                    alignment64 = 8;
                }

                has8BytePrimitiveField = true;
            }
            else if (remappedName.Equals("IntPtr", StringComparison.Ordinal) ||
                     remappedName.Equals("nint", StringComparison.Ordinal) ||
                     remappedName.Equals("nuint", StringComparison.Ordinal) ||
                     remappedName.Equals("UIntPtr", StringComparison.Ordinal) ||
                     remappedName.EndsWith('*'))
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
            else
            {
                GetTypeSize(cursor, typedefType.Decl.UnderlyingType, ref alignment32, ref alignment64, ref has8BytePrimitiveField, out size32, out size64);
            }
        }
        else if (type is SubstTemplateTypeParmType substTemplateTypeParmType)
        {
            GetTypeSize(cursor, substTemplateTypeParmType.ReplacementType, ref alignment32, ref alignment64, ref has8BytePrimitiveField, out size32, out size64);
        }
        else if (type is TemplateSpecializationType templateSpecializationType)
        {
            if (templateSpecializationType.IsTypeAlias)
            {
                GetTypeSize(cursor, templateSpecializationType.AliasedType, ref alignment32, ref alignment64, ref has8BytePrimitiveField, out size32, out size64);
            }
            else if (templateSpecializationType.IsSugared)
            {
                GetTypeSize(cursor, templateSpecializationType.Desugar, ref alignment32, ref alignment64, ref has8BytePrimitiveField, out size32, out size64);
            }
            else if (templateSpecializationType.TemplateName.AsTemplateDecl is TemplateDecl templateDecl)
            {
                if (templateDecl.TemplatedDecl is TypeDecl typeDecl)
                {
                    GetTypeSize(cursor, typeDecl.TypeForDecl, ref alignment32, ref alignment64, ref has8BytePrimitiveField, out size32, out size64);
                }
                else
                {
                    AddDiagnostic(DiagnosticLevel.Error, $"Unsupported template specialization declaration kind: '{templateDecl.TemplatedDecl.DeclKindName}'.", cursor);
                }
            }
            else
            {
                AddDiagnostic(DiagnosticLevel.Error, $"Unsupported template specialization type: '{templateSpecializationType}'.", cursor);
            }
        }
        else if (type is TemplateTypeParmType)
        {
            // Nothing to handle
        }
        else
        {
            AddDiagnostic(DiagnosticLevel.Error, $"Unsupported type: '{type.TypeClass}'.", cursor);
        }
    }

    private bool HasSuppressGCTransition(Cursor? cursor)
        => (cursor is NamedDecl namedDecl) && HasRemapping(namedDecl, _config.WithSuppressGCTransitions);

    private bool HasBaseField(CXXRecordDecl cxxRecordDecl)
    {
        var hasBaseField = false;

        foreach (var cxxBaseSpecifier in cxxRecordDecl.Bases)
        {
            var baseCxxRecordDecl = GetRecordDecl(cxxBaseSpecifier);

            if (HasField(baseCxxRecordDecl))
            {
                hasBaseField = true;
                break;
            }
        }

        return hasBaseField;
    }

    private bool HasField(RecordDecl recordDecl)
    {
        var hasField = recordDecl.Fields.Any() || recordDecl.Decls.Any((decl) => (decl is RecordDecl nestedRecordDecl) && nestedRecordDecl.IsAnonymousStructOrUnion && HasField(nestedRecordDecl));

        if (!hasField && (recordDecl is CXXRecordDecl cxxRecordDecl))
        {
            hasField = HasBaseField(cxxRecordDecl);
        }

        return hasField;
    }

    private bool HasUnsafeMethod(CXXRecordDecl cxxRecordDecl)
    {
        var hasUnsafeMethod = cxxRecordDecl.Methods.Any((method) => method.IsUserProvided && IsUnsafe(method) && !IsExcluded(method));

        if (!hasUnsafeMethod)
        {
            foreach (var cxxBaseSpecifier in cxxRecordDecl.Bases)
            {
                var baseCxxRecordDecl = GetRecordDecl(cxxBaseSpecifier);

                if (HasUnsafeMethod(baseCxxRecordDecl))
                {
                    hasUnsafeMethod = true;
                    break;
                }
            }
        }

        return hasUnsafeMethod;
    }

    private bool HasVtbl(CXXRecordDecl cxxRecordDecl, out bool hasBaseVtbl)
    {
        var hasVtbl = cxxRecordDecl.Methods.Any((method) => method.IsVirtual && method.IsVirtual && (method.OverriddenMethods.Count == 0));
        hasBaseVtbl = false;

        if (!hasVtbl)
        {
            var indirectVtblCount = 0;

            foreach (var cxxBaseSpecifier in cxxRecordDecl.Bases)
            {
                var baseCxxRecordDecl = GetRecordDecl(cxxBaseSpecifier);

                if ((HasVtbl(baseCxxRecordDecl, out var baseHasBaseVtbl) || baseHasBaseVtbl) && !HasField(baseCxxRecordDecl))
                {
                    indirectVtblCount++;
                }
            }

            if (indirectVtblCount > 1)
            {
                AddDiagnostic(DiagnosticLevel.Warning, "Unsupported cxx record declaration: 'multiple virtual bases'. Generated bindings may be incomplete.", cxxRecordDecl);
            }

            hasBaseVtbl = indirectVtblCount != 0;
        }

        return hasVtbl;
    }

    private bool IsEnumOperator(FunctionDecl functionDecl, string name)
    {
        if (name.StartsWith("operator", StringComparison.Ordinal) && ((functionDecl.Parameters.Count == 1) || (functionDecl.Parameters.Count == 2)))
        {
            var parmVarDecl1 = functionDecl.Parameters[0];
            var parmVarDecl1Type = parmVarDecl1.Type;

            if (IsType<PointerType>(parmVarDecl1, parmVarDecl1Type, out var pointerType1))
            {
                parmVarDecl1Type = pointerType1.PointeeType;
            }
            else if (IsType<ReferenceType>(parmVarDecl1, parmVarDecl1Type, out var referenceType1))
            {
                parmVarDecl1Type = referenceType1.PointeeType;
            }

            if (functionDecl.Parameters.Count == 1)
            {
                return IsType<EnumType>(parmVarDecl1);
            }

            var parmVarDecl2 = functionDecl.Parameters[1];
            var parmVarDecl2Type = parmVarDecl2.Type;

            if (IsType<PointerType>(parmVarDecl2, parmVarDecl2Type, out var pointerType2))
            {
                parmVarDecl2Type = pointerType2.PointeeType;
            }
            else if (IsType<ReferenceType>(parmVarDecl2, parmVarDecl2Type, out var referenceType2))
            {
                parmVarDecl2Type = referenceType2.PointeeType;
            }

            if ((parmVarDecl1Type.CanonicalType == parmVarDecl2Type.CanonicalType) && IsType<EnumType>(parmVarDecl2))
            {
                return true;
            }
        }
        return false;
    }

    private bool IsExcluded(Cursor cursor) => IsExcluded(cursor, out _);

    private bool IsExcluded(Cursor cursor, out bool isExcludedByConflictingDefinition)
    {
        if (!_isExcluded.TryGetValue(cursor, out var isExcludedValue))
        {
            isExcludedValue |= (!IsAlwaysIncluded(cursor) && (IsExcludedByConfig(cursor) || IsExcludedByFile(cursor) || IsExcludedByName(cursor, ref isExcludedValue))) ? 0b01u : 0b00u;
            _isExcluded.Add(cursor, isExcludedValue);
        }
        isExcludedByConflictingDefinition = (isExcludedValue & 0b10) != 0;
        return (isExcludedValue & 0b01) != 0;

        bool IsAlwaysIncluded(Cursor cursor)
        {
            return (cursor is TranslationUnitDecl) || (cursor is LinkageSpecDecl) || (cursor is NamespaceDecl) || ((cursor is VarDecl varDecl) && varDecl.Name.StartsWith("ClangSharpMacro_", StringComparison.Ordinal));
        }

        bool IsExcludedByConfig(Cursor cursor)
        {
            return (_config.ExcludeFunctionsWithBody && (cursor is FunctionDecl functionDecl) && functionDecl.HasBody)
                || (!_config.GenerateTemplateBindings && (cursor is TemplateDecl));
        }

        bool IsExcludedByFile(Cursor cursor)
        {
            if (_outputBuilder != null)
            {
                // We don't want to exclude by file if we already have an active output builder as we
                // are likely processing members of an already included type but those members may
                // indirectly exist or be defined in a non-traversed file.
                return false;
            }

            var declLocation = cursor.Location;
            declLocation.GetFileLocation(out var file, out var line, out var column, out _);

            if (IsIncludedFileOrLocation(cursor, file, declLocation))
            {
                return false;
            }

            // It is not uncommon for some declarations to be done using macros, which are themselves
            // defined in an imported header file. We want to also check if the expansion location is
            // in the main file to catch these cases and ensure we still generate bindings for them.

            declLocation.GetExpansionLocation(out var expansionFile, out var expansionLine, out var expansionColumn, out _);

            if ((expansionFile == file) && (expansionLine == line) && (expansionColumn == column) && _config.TraversalNames.Count != 0)
            {
                // clang_getLocation is a very expensive call, so exit early if the expansion file is the same
                // However, if we are not explicitly specifying traversal names, its possible the expansion location
                // is the same, but IsMainFile is now marked as true, in which case we can't exit early.

                return true;
            }

            var expansionLocation = cursor.TranslationUnit.Handle.GetLocation(expansionFile, expansionLine, expansionColumn);

            return !IsIncludedFileOrLocation(cursor, file, expansionLocation);
        }

        bool IsExcludedByName(Cursor cursor, ref uint isExcludedValue)
        {
            var isExcludedByConfigOption = false;
            var qualifiedNameWithoutParameters = "";

            string qualifiedName;
            string name;
            string kind;

            if (cursor is NamedDecl namedDecl)
            {
                // We get the non-remapped name for the purpose of exclusion checks to ensure that users
                // can remove no-definition declarations in favor of remapped anonymous declarations.

                qualifiedName = GetCursorQualifiedName(namedDecl);

                if (namedDecl is FunctionDecl)
                {
                    qualifiedNameWithoutParameters = GetCursorQualifiedName(namedDecl, truncateParameters: true);
                }

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

            if (qualifiedName.Contains("ClangSharpMacro_", StringComparison.Ordinal))
            {
                qualifiedName = qualifiedName.Replace("ClangSharpMacro_", "", StringComparison.Ordinal);
            }

            if (name.Contains("ClangSharpMacro_", StringComparison.Ordinal))
            {
                name = name.Replace("ClangSharpMacro_", "", StringComparison.Ordinal);
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
                else if (functionDecl is CXXMethodDecl cxxMethodDecl)
                {
                    var parent = cxxMethodDecl.Parent;
                    Debug.Assert(parent is not null);

                    if (IsConflictingMethodDecl(cxxMethodDecl, parent))
                    {
                        isExcludedValue |= 0b10;
                    }
                }

                if (_config.GenerateDisableRuntimeMarshalling && functionDecl.IsVariadic)
                {
                    isExcludedByConfigOption = true;
                }
            }

            var dottedQualifiedName = qualifiedName.Replace("::", ".", StringComparison.Ordinal);

            if (_config.ExcludedNames.Contains(qualifiedName) || _config.ExcludedNames.Contains(dottedQualifiedName))
            {
                if (_config.LogExclusions)
                {
                    var message = $"Excluded {kind} '{qualifiedName}' by exact match";

                    if (isExcludedByConfigOption)
                    {
                        message += "; Exclusion is unnecessary due to a config option";
                    }
                    else if ((isExcludedValue & 0b10) != 0)
                    {
                        message += "; Exclusion is unnecessary due to a conflicting definition";
                    }

                    AddDiagnostic(DiagnosticLevel.Info, message);
                }
                return true;
            }

            var dottedQualifiedNameWithoutParameters = qualifiedNameWithoutParameters.Replace("::", ".", StringComparison.Ordinal);

            if (_config.ExcludedNames.Contains(qualifiedNameWithoutParameters) || _config.ExcludedNames.Contains(dottedQualifiedNameWithoutParameters) || _config.ExcludedNames.Contains(name))
            {
                if (_config.LogExclusions)
                {
                    var message = $"Excluded {kind} '{qualifiedName}' by partial match against {name}";

                    if (isExcludedByConfigOption)
                    {
                        message += "; Exclusion is unnecessary due to a config option";
                    }
                    else if ((isExcludedValue & 0b10) != 0)
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

            if (_config.IncludedNames.Count != 0 && !_config.IncludedNames.Contains(qualifiedName)
                                                 && !_config.IncludedNames.Contains(dottedQualifiedName)
                                                 && !_config.IncludedNames.Contains(qualifiedNameWithoutParameters)
                                                 && !_config.IncludedNames.Contains(dottedQualifiedNameWithoutParameters)
                                                 && !_config.IncludedNames.Contains(name))
            {
                var semanticParentCursor = cursor.SemanticParentCursor;

                if ((semanticParentCursor is null) || IsExcluded(semanticParentCursor) || IsAlwaysIncluded(semanticParentCursor))
                {
                    if (_config.LogExclusions)
                    {
                        AddDiagnostic(DiagnosticLevel.Info, $"Excluded {kind} '{qualifiedName}' as it was not in the include list");
                    }
                    return true;
                }
            }

            if ((isExcludedValue & 0b10) != 0)
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
            // Use case insensitive comparison on Windows
            var equalityComparer = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal;

            // Normalize paths to be '/' for comparison
            var fileName = file.Name.ToString().NormalizePath();

            if (_visitedFiles.Add(fileName) && _config.LogVisitedFiles)
            {
                AddDiagnostic(DiagnosticLevel.Info, $"Visiting {fileName}");
            }

            if (_config.TraversalNames.Contains(fileName, equalityComparer))
            {
                return true;
            }
            else if (_config.TraversalNames.Contains(fileName.NormalizeFullPath(), equalityComparer))
            {
                return true;
            }
            else if (_config.TraversalNames.Count == 0 && location.IsFromMainFile)
            {
                return true;
            }

            return false;
        }

        bool IsComProxy(FunctionDecl functionDecl, string name)
        {
            var parmVarDecl = null as ParmVarDecl;

            if (name.EndsWith("_UserFree", StringComparison.Ordinal) || name.EndsWith("_UserFree64", StringComparison.Ordinal) ||
                name.EndsWith("_UserMarshal", StringComparison.Ordinal) || name.EndsWith("_UserMarshal64", StringComparison.Ordinal) ||
                name.EndsWith("_UserSize", StringComparison.Ordinal) || name.EndsWith("_UserSize64", StringComparison.Ordinal) ||
                name.EndsWith("_UserUnmarshal", StringComparison.Ordinal) || name.EndsWith("_UserUnmarshal64", StringComparison.Ordinal))
            {
                var parameters = functionDecl.Parameters;
                parmVarDecl = (parameters.Count != 0) ? parameters[^1] : null;
            }
            else if (name.EndsWith("_Proxy", StringComparison.Ordinal) || name.EndsWith("_Stub", StringComparison.Ordinal))
            {
                var parameters = functionDecl.Parameters;
                parmVarDecl = (parameters.Count != 0) ? parameters[0] : null;
            }

            if ((parmVarDecl is not null) && IsType<PointerType>(parmVarDecl, out var pointerType))
            {
                var typeName = GetTypeName(parmVarDecl, context: null, pointerType.PointeeType, ignoreTransparentStructsWhereRequired: false, out var nativeTypeName);
                return name.StartsWith($"{nativeTypeName}_", StringComparison.Ordinal) || name.StartsWith($"{typeName}_", StringComparison.Ordinal) || typeName.Equals("IRpcStubBuffer", StringComparison.Ordinal);
            }
            return false;
        }

        bool IsConflictingMethodDecl(CXXMethodDecl cxxMethodDeclToMatch, CXXRecordDecl cxxRecordDecl)
        {
            var cxxMethodDeclToMatchName = GetRemappedCursorName(cxxMethodDeclToMatch);
            var foundCxxMethodDeclToMatch = false;

            foreach (var cxxBaseSpecifier in cxxRecordDecl.Bases)
            {
                var baseCxxRecordDecl = GetRecordDecl(cxxBaseSpecifier);

                if (ContainsConflictingMethodDecl(cxxMethodDeclToMatch, cxxRecordDecl, baseCxxRecordDecl, cxxMethodDeclToMatchName, ref foundCxxMethodDeclToMatch))
                {
                    return true;
                }
            }

            return ContainsConflictingMethodDecl(cxxMethodDeclToMatch, cxxRecordDecl, cxxRecordDecl, cxxMethodDeclToMatchName, ref foundCxxMethodDeclToMatch);

            bool ContainsConflictingMethodDecl(CXXMethodDecl cxxMethodDeclToMatch, CXXRecordDecl rootCxxRecordDecl, CXXRecordDecl cxxRecordDecl, string cxxMethodDeclToMatchName, ref bool foundCxxMethodDeclToMatch)
            {
                var cxxMethodDecls = cxxRecordDecl.Methods;

                if (cxxMethodDecls.Count != 0)
                {
                    foreach (var cxxMethodDecl in cxxMethodDecls.OrderBy((cxxmd) => cxxmd.VtblIndex))
                    {
                        if (IsConflictingMethodDecl(cxxMethodDeclToMatch, cxxMethodDecl, rootCxxRecordDecl, cxxRecordDecl, cxxMethodDeclToMatchName, ref foundCxxMethodDeclToMatch))
                        {
                            return true;
                        }
                    }
                }

                return false;
            }

            bool IsConflictingMethodDecl(CXXMethodDecl cxxMethodDeclToMatch, CXXMethodDecl cxxMethodDecl, CXXRecordDecl rootCxxRecordDecl, CXXRecordDecl cxxRecordDecl, string cxxMethodDeclToMatchName, ref bool foundCxxMethodDeclToMatch)
            {
                var methodName = GetRemappedCursorName(cxxMethodDecl);

                if (cxxMethodDeclToMatchName != methodName)
                {
                    return false;
                }

                if (cxxMethodDecl == cxxMethodDeclToMatch)
                {
                    foundCxxMethodDeclToMatch = true;
                    return false;
                }

                if (cxxMethodDecl.Parameters.Count != cxxMethodDeclToMatch.Parameters.Count)
                {
                    return false;
                }

                var allMatch = true;

                for (var n = 0; n < cxxMethodDeclToMatch.Parameters.Count; n++)
                {
                    var parameterTypeToMatch = cxxMethodDeclToMatch.Parameters[n].Type;
                    var parameterType = cxxMethodDecl.Parameters[n].Type;

                    if (parameterType.CanonicalType == parameterTypeToMatch.CanonicalType)
                    {
                        continue;
                    }

                    if (IsType<PointerType>(cursor, parameterTypeToMatch, out var pointerTypeToMatch) &&
                        IsType<ReferenceType>(cursor, parameterType, out var referenceType) &&
                        (referenceType.PointeeType.CanonicalType == pointerTypeToMatch.PointeeType.CanonicalType))
                    {
                        continue;
                    }

                    if (IsType<ReferenceType>(cursor, parameterTypeToMatch, out var referenceTypeToMatch) &&
                        IsType<PointerType>(cursor, parameterType, out var pointerType) &&
                        (pointerType.PointeeType.CanonicalType == referenceTypeToMatch.PointeeType.CanonicalType))
                    {
                        continue;
                    }

                    allMatch = false;
                    break;
                }

                if (!allMatch)
                {
                    return false;
                }

                if (cxxMethodDecl.IsVirtual)
                {
                    if (cxxMethodDeclToMatch.IsVirtual)
                    {
                        if (rootCxxRecordDecl != cxxRecordDecl)
                        {
                            // The found declaration and declaration to match are both virtual
                            // We want to treat the one from the base declaration as non-conflicting
                            // So return true to report the declaration to match as the conflict
                            return true;
                        }
                        else if (cxxMethodDeclToMatch.IsThisDeclarationADefinition != cxxMethodDecl.IsThisDeclarationADefinition)
                        {
                            return false;
                        }
                        else
                        {
                            AddDiagnostic(DiagnosticLevel.Error, "Found conflicting method definitions for two virtual methods.", cxxMethodDeclToMatch);
                        }
                    }
                    else
                    {
                        // The found declaration is virtual while the declaration to match is not
                        // We want to treat the virtual declaration as non-conflicting
                        // So return true to report the declaration to match as the conflict
                        return true;
                    }
                }
                else if (cxxMethodDeclToMatch.IsVirtual)
                {
                    // The declaration to match is virtual while the found declaration is not
                    // We want to treat the virtual declaration as non-conflicting
                    // So treat the declaration as non-conflicting and continue searching
                    return false;
                }
                else
                {
                    // Neither the declaration nor the declaration to match are virtual
                    // We want to pick whichever declaration appears first
                    // So return true or false based on if we already encountered the declaration to match
                    return !foundCxxMethodDeclToMatch;
                }

                return false;
            }
        }

        bool IsEmptyRecord(RecordDecl recordDecl)
        {
            if (recordDecl.Fields.Count != 0)
            {
                if (!GetCursorName(recordDecl).EndsWith("__", StringComparison.Ordinal) || (recordDecl.Fields.Count != 1))
                {
                    return false;
                }

                var field = recordDecl.Fields[0];

                if (!GetCursorName(field).Equals("unused", StringComparison.Ordinal) || !IsType<BuiltinType>(field, out var builtinType) || (builtinType.Kind != CXType_Int))
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
                    var baseCxxRecordDecl = GetRecordDecl(cxxBaseSpecifier);

                    if (!IsEmptyRecord(baseCxxRecordDecl))
                    {
                        return false;
                    }
                }
            }

            return !TryGetUuid(recordDecl, out _);
        }
    }

    private bool IsBaseExcluded(CXXRecordDecl cxxRecordDecl, CXXRecordDecl baseCxxRecordDecl, CXXBaseSpecifier cxxBaseSpecifier, out string baseFieldName)
    {
        baseFieldName = GetAnonymousName(cxxBaseSpecifier, "Base");
        baseFieldName = GetRemappedName(baseFieldName, cxxBaseSpecifier, tryRemapOperatorName: true, out _, skipUsing: true);

        var qualifiedName = $"{GetCursorQualifiedName(cxxRecordDecl)}::{baseFieldName}";
        var dottedQualifiedName = qualifiedName.Replace("::", ".", StringComparison.Ordinal);

        return _config.ExcludedNames.Contains(qualifiedName) || _config.ExcludedNames.Contains(dottedQualifiedName);
    }

    private bool IsFixedSize(Cursor cursor, Type type)
    {
        // We don't want to handle these using IsType because we need to specially
        // handle cases like TypedefType at each level of the type hierarchy

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
        else if (type is DecltypeType decltypeType)
        {
            return IsFixedSize(cursor, decltypeType.UnderlyingType);
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
                && (recordDecl is not CXXRecordDecl cxxRecordDecl || cxxRecordDecl.Methods.All((cxxMethodDecl) => !cxxMethodDecl.IsVirtual));
        }
        else if (type is ReferenceType)
        {
            return false;
        }
        else if (type is TypedefType typedefType)
        {
            var name = GetTypeName(cursor, context: null, type, ignoreTransparentStructsWhereRequired: false, out _);
            var remappedName = GetRemappedTypeName(cursor, context: null, type, out _, skipUsing: true, ignoreTransparentStructsWhereRequired: false);

            return !remappedName.Equals("IntPtr", StringComparison.Ordinal)
                && !remappedName.Equals("nint", StringComparison.Ordinal)
                && !remappedName.Equals("nuint", StringComparison.Ordinal)
                && !remappedName.Equals("UIntPtr", StringComparison.Ordinal)
                && IsFixedSize(cursor, typedefType.Decl.UnderlyingType);
        }
        else
        {
            AddDiagnostic(DiagnosticLevel.Warning, $"Unsupported type: '{type.TypeClass}'. Assuming unfixed size.", cursor);
            return false;
        }
    }

    private static bool IsNativeTypeNameEquivalent(string nativeTypeName, string typeName)
    {
        return nativeTypeName.Equals(typeName, StringComparison.OrdinalIgnoreCase)
            || nativeTypeName.Replace(" ", "", StringComparison.Ordinal).Equals(typeName, StringComparison.OrdinalIgnoreCase);
    }

    private bool IsPrevContextDecl<T>([MaybeNullWhen(false)] out T cursor, out object? userData, bool includeLast = false)
        where T : Decl
    {
        var previousContext = _context.Last;
        Debug.Assert(previousContext != null);

        if (!includeLast)
        {
            previousContext = previousContext.Previous;
            Debug.Assert(previousContext != null);
        }

        while (previousContext.Value.Cursor is not Decl)
        {
            previousContext = previousContext.Previous;
            Debug.Assert(previousContext != null);
        }

        var value = previousContext.Value;

        if (value.Cursor is T t)
        {
            cursor = t;
            userData = value.UserData;
            return true;
        }
        else
        {
            cursor = null;
            userData = null;
            return false;
        }
    }

    private bool IsPrevContextStmt<T>([MaybeNullWhen(false)] out T cursor, out object? userData, bool preserveParen = false, bool preserveImplicitCast = false)
        where T : Stmt
    {
        var previousContext = _context.Last;
        Debug.Assert(previousContext != null);

        do
        {
            previousContext = previousContext.Previous;
            Debug.Assert(previousContext is not null);
        }
        while ((!preserveParen && (previousContext.Value.Cursor is ParenExpr)) || (!preserveImplicitCast && (previousContext.Value.Cursor is ImplicitCastExpr)));

        var value = previousContext.Value;

        if (value.Cursor is T t)
        {
            cursor = t;
            userData = value.UserData;
            return true;
        }
        else
        {
            cursor = null;
            userData = null;
            return false;
        }
    }

    private static bool IsStmtAsWritten<T>(Cursor cursor, [MaybeNullWhen(false)] out T value, bool removeParens = false)
        where T : Stmt
    {
        if (cursor is Expr expr)
        {
            cursor = GetExprAsWritten(expr, removeParens);
        }

        if (cursor is T t)
        {
            value = t;
            return true;
        }
        else
        {
            value = null;
            return false;
        }
    }

    private static bool IsStmtAsWritten(Stmt stmt, Stmt expectedStmt, bool removeParens = false)
    {
        if (stmt == expectedStmt)
        {
            return true;
        }

        if (stmt is not Expr expr)
        {
            return false;
        }

        expr = GetExprAsWritten(expr, removeParens);
        return expr == expectedStmt;
    }

    private bool IsType<T>(Expr expr)
        where T : Type => IsType<T>(expr, out _);

    private bool IsType<T>(Expr expr, [MaybeNullWhen(false)] out T value)
        where T : Type => IsType(expr, expr.Type, out value);

    private bool IsType<T>(ValueDecl valueDecl)
        where T : Type => IsType<T>(valueDecl, out _);

    private bool IsType<T>(ValueDecl typeDecl, [MaybeNullWhen(false)] out T value)
        where T : Type => IsType(typeDecl, typeDecl.Type, out value);

    private bool IsType<T>(Cursor? cursor, Type type)
        where T : Type => IsType<T>(cursor, type, out _);

    private bool IsType<T>(Cursor? cursor, Type type, [MaybeNullWhen(false)] out T value)
        where T : Type
    {
        if (type is T t)
        {
            value = t;
            return true;
        }
        else if (type is AttributedType attributedType)
        {
            return IsType(cursor, attributedType.ModifiedType, out value);
        }
        else if (type is DecltypeType decltypeType)
        {
            return IsType(cursor, decltypeType.UnderlyingType, out value);
        }
        else if (type is DeducedType deducedType)
        {
            return IsType(cursor, deducedType.GetDeducedType, out value);
        }
        else if (type is DependentNameType dependentNameType)
        {
            if (dependentNameType.IsSugared)
            {
                return IsType(cursor, dependentNameType.Desugar, out value);
            }
        }
        else if (type is ElaboratedType elaboratedType)
        {
            return IsType(cursor, elaboratedType.NamedType, out value);
        }
        else if (type is InjectedClassNameType injectedClassNameType)
        {
            return IsType(cursor, injectedClassNameType.InjectedTST, out value);
        }
        else if (type is PackExpansionType packExpansionType)
        {
            return IsType(cursor, packExpansionType.Pattern, out value);
        }
        else if (type is SubstTemplateTypeParmType substTemplateTypeParmType)
        {
            return IsType(cursor, substTemplateTypeParmType.ReplacementType, out value);
        }
        else if (type is TemplateSpecializationType templateSpecializationType)
        {
            if (templateSpecializationType.IsTypeAlias)
            {
                return IsType(cursor, templateSpecializationType.AliasedType, out value);
            }
            else if (templateSpecializationType.IsSugared)
            {
                return IsType(cursor, templateSpecializationType.Desugar, out value);
            }
            else if (templateSpecializationType.TemplateName.AsTemplateDecl is TemplateDecl templateDecl)
            {
                if (templateDecl.TemplatedDecl is TypeDecl typeDecl)
                {
                    return IsType(cursor, typeDecl.TypeForDecl, out value);
                }
            }
        }
        else if (type is TemplateTypeParmType templateTypeParmType)
        {
            if (templateTypeParmType.IsSugared)
            {
                return IsType(cursor, templateTypeParmType.Decl.TypeForDecl, out value);
            }
        }
        else if (type is TypedefType typedefType)
        {
            return IsType(cursor, typedefType.Decl.UnderlyingType, out value);
        }
        else if (type is UsingType usingType)
        {
            if (usingType.IsSugared)
            {
                return IsType(cursor, usingType.Desugar, out value);
            }
        }

        value = default;
        return false;
    }

    private bool IsTypeConstantOrIncompleteArray(Expr expr)
         => IsTypeConstantOrIncompleteArray(expr, out _);

    private bool IsTypeConstantOrIncompleteArray(Expr expr, [MaybeNullWhen(false)] out ArrayType arrayType)
         => IsTypeConstantOrIncompleteArray(expr, expr.Type, out arrayType);

    private bool IsTypeConstantOrIncompleteArray(ValueDecl valueDecl)
         => IsTypeConstantOrIncompleteArray(valueDecl, out _);

    private bool IsTypeConstantOrIncompleteArray(ValueDecl valueDecl, [MaybeNullWhen(false)] out ArrayType arrayType)
         => IsTypeConstantOrIncompleteArray(valueDecl, valueDecl.Type, out arrayType);

    private bool IsTypeConstantOrIncompleteArray(Cursor? cursor, Type type)
         => IsTypeConstantOrIncompleteArray(cursor, type, out _);

    private bool IsTypeConstantOrIncompleteArray(Cursor? cursor, Type type, [MaybeNullWhen(false)] out ArrayType arrayType)
         => IsType(cursor, type, out arrayType)
         && (arrayType is ConstantArrayType or IncompleteArrayType);

    private bool IsTypePointerOrReference(Expr expr)
         => IsTypePointerOrReference(expr, expr.Type);

    private bool IsTypePointerOrReference(ValueDecl valueDecl)
        => IsTypePointerOrReference(valueDecl, valueDecl.Type);

    private bool IsTypePointerOrReference(Cursor? cursor, Type type)
        => IsType<PointerType>(cursor, type)
        || IsType<ReferenceType>(cursor, type);

    private bool IsTypeVoid(Cursor? cursor, Type type)
         => IsType<BuiltinType>(cursor, type, out var builtinType)
         && (builtinType.Kind == CXType_Void);

    internal bool IsSupportedFixedSizedBufferType(string typeName)
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
                // We want to prefer InlineArray in modern code, as it is safer and supports more features
                return !Config.GenerateLatestCode;
            }

            default:
            {
                return false;
            }
        }
    }

    private static bool IsTransparentStructBoolean(PInvokeGeneratorTransparentStructKind kind)
        => kind is PInvokeGeneratorTransparentStructKind.Boolean;

    private static bool IsTransparentStructHandle(PInvokeGeneratorTransparentStructKind kind)
         =>  kind is PInvokeGeneratorTransparentStructKind.Handle
                  or PInvokeGeneratorTransparentStructKind.HandleWin32;

    private static bool IsTransparentStructHexBased(PInvokeGeneratorTransparentStructKind kind)
         => IsTransparentStructHandle(kind)
         || (kind == PInvokeGeneratorTransparentStructKind.TypedefHex);

    private bool IsUnchecked(string targetTypeName, Stmt stmt)
    {
        if (IsPrevContextDecl<VarDecl>(out var parentVarDecl, out _))
        {
            var cursorName = GetCursorName(parentVarDecl);

            if (cursorName.StartsWith("ClangSharpMacro_", StringComparison.Ordinal) && _config.WithTransparentStructs.TryGetValue(targetTypeName, out var transparentStruct))
            {
                targetTypeName = transparentStruct.Name;
            }
        }

        switch (stmt.StmtClass)
        {
            // case CX_StmtClass_BinaryConditionalOperator:

            case CX_StmtClass_ConditionalOperator:
            {
                var conditionalOperator = (ConditionalOperator)stmt;
                return IsUnchecked(targetTypeName, conditionalOperator.LHS)
                    || IsUnchecked(targetTypeName, conditionalOperator.RHS)
                    || IsUnchecked(targetTypeName, conditionalOperator.Handle.Evaluate);
            }

            // case CX_StmtClass_AddrLabelExpr:
            // case CX_StmtClass_ArrayInitIndexExpr:
            // case CX_StmtClass_ArrayInitLoopExpr:

            case CX_StmtClass_ArraySubscriptExpr:
            {
                var arraySubscriptExpr = (ArraySubscriptExpr)stmt;
                return IsUnchecked(targetTypeName, arraySubscriptExpr.LHS)
                    || IsUnchecked(targetTypeName, arraySubscriptExpr.RHS);
            }

            // case CX_StmtClass_ArrayTypeTraitExpr:
            // case CX_StmtClass_AsTypeExpr:
            // case CX_StmtClass_AtomicExpr:

            case CX_StmtClass_BinaryOperator:
            {
                var binaryOperator = (BinaryOperator)stmt;
                return IsUnchecked(targetTypeName, binaryOperator.LHS)
                    || IsUnchecked(targetTypeName, binaryOperator.RHS)
                    || IsUnchecked(targetTypeName, binaryOperator.Handle.Evaluate)
                    || IsOverflow(binaryOperator);
            }

            // case CX_StmtClass_CompoundAssignOperator:
            // case CX_StmtClass_BlockExpr:
            // case CX_StmtClass_CXXBindTemporaryExpr:

            case CX_StmtClass_CXXBoolLiteralExpr:
            {
                return false;
            }

            case CX_StmtClass_CXXConstructExpr:
            {
                return false;
            }

            case CX_StmtClass_CXXTemporaryObjectExpr:
            {
                return false;
            }

            case CX_StmtClass_CXXDefaultArgExpr:
            {
                return false;
            }

            case CX_StmtClass_CXXDefaultInitExpr:
            {
                return false;
            }

            // case CX_StmtClass_CXXDeleteExpr:

            case CX_StmtClass_CXXDependentScopeMemberExpr:
            {
                return false;
            }

            // case CX_StmtClass_CXXFoldExpr:
            // case CX_StmtClass_CXXInheritedCtorInitExpr:

            case CX_StmtClass_CXXNewExpr:
            {
                return false;
            }

            // case CX_StmtClass_CXXNoexceptExpr:

            case CX_StmtClass_CXXNullPtrLiteralExpr:
            {
                return false;
            }

            // case CX_StmtClass_CXXPseudoDestructorExpr:
            // case CX_StmtClass_CXXRewrittenBinaryOperator:
            // case CX_StmtClass_CXXScalarValueInitExpr:
            // case CX_StmtClass_CXXStdInitializerListExpr:

            case CX_StmtClass_CXXThisExpr:
            {
                return false;
            }

            // case CX_StmtClass_CXXThrowExpr:
            // case CX_StmtClass_CXXTypeidExpr:
            // case CX_StmtClass_CXXUnresolvedConstructExpr:

            case CX_StmtClass_CXXUuidofExpr:
            {
                return false;
            }

            case CX_StmtClass_CallExpr:
            {
                return false;
            }

            // case CX_StmtClass_CUDAKernelCallExpr:

            case CX_StmtClass_CXXMemberCallExpr:
            {
                return false;
            }

            case CX_StmtClass_CXXOperatorCallExpr:
            {
                return false;
            }

            // case CX_StmtClass_UserDefinedLiteral:
            // case CX_StmtClass_BuiltinBitCastExpr:

            case CX_StmtClass_CStyleCastExpr:
            case CX_StmtClass_CXXStaticCastExpr:
            case CX_StmtClass_CXXFunctionalCastExpr:
            {
                var explicitCastExpr = (ExplicitCastExpr)stmt;
                var explicitCastExprTypeName = GetRemappedTypeName(explicitCastExpr, context: null, explicitCastExpr.Type, out _);

                return IsUnchecked(targetTypeName, explicitCastExpr.SubExprAsWritten)
                    || IsUnchecked(targetTypeName, explicitCastExpr.Handle.Evaluate)
                    || (IsUnsigned(targetTypeName) != IsUnsigned(explicitCastExprTypeName));
            }

            case CX_StmtClass_CXXConstCastExpr:
            case CX_StmtClass_CXXDynamicCastExpr:
            case CX_StmtClass_CXXReinterpretCastExpr:
            {
                var namedCastExpr = (CXXNamedCastExpr)stmt;

                return IsUnchecked(targetTypeName, namedCastExpr.SubExprAsWritten)
                    || IsUnchecked(targetTypeName, namedCastExpr.Handle.Evaluate);
            }

            // case CX_StmtClass_ObjCBridgedCastExpr:

            case CX_StmtClass_ImplicitCastExpr:
            {
                var implicitCastExpr = (ImplicitCastExpr)stmt;

                return IsUnchecked(targetTypeName, implicitCastExpr.SubExprAsWritten)
                    || IsUnchecked(targetTypeName, implicitCastExpr.Handle.Evaluate);
            }

            case CX_StmtClass_CharacterLiteral:
            {
                return false;
            }

            // case CX_StmtClass_ChooseExpr:
            // case CX_StmtClass_CompoundLiteralExpr:
            // case CX_StmtClass_ConceptSpecializationExpr:
            // case CX_StmtClass_ConvertVectorExpr:
            // case CX_StmtClass_CoawaitExpr:
            // case CX_StmtClass_CoyieldExpr:

            case CX_StmtClass_DeclRefExpr:
            {
                var declRefExpr = (DeclRefExpr)stmt;
                return (declRefExpr.Decl is VarDecl varDecl) && varDecl.HasInit && IsUnchecked(targetTypeName, varDecl.Init);
            }

            // case CX_StmtClass_DependentCoawaitExpr:
            // case CX_StmtClass_DependentScopeDeclRefExpr:
            // case CX_StmtClass_DesignatedInitExpr:
            // case CX_StmtClass_DesignatedInitUpdateExpr:
            // case CX_StmtClass_ExpressionTraitExpr:
            // case CX_StmtClass_ExtVectorElementExpr:
            // case CX_StmtClass_FixedPointLiteral:

            case CX_StmtClass_FloatingLiteral:
            {
                return false;
            }

            // case CX_StmtClass_ConstantExpr:

            case CX_StmtClass_ExprWithCleanups:
            {
                var exprWithCleanups = (ExprWithCleanups)stmt;
                return IsUnchecked(targetTypeName, exprWithCleanups.SubExpr);
            }

            // case CX_StmtClass_FunctionParmPackExpr:
            // case CX_StmtClass_GNUNullExpr:
            // case CX_StmtClass_GenericSelectionExpr:
            // case CX_StmtClass_ImaginaryLiteral:
            // case CX_StmtClass_ImplicitValueInitExpr:

            case CX_StmtClass_InitListExpr:
            {
                return false;
            }

            case CX_StmtClass_IntegerLiteral:
            {
                var integerLiteral = (IntegerLiteral)stmt;
                var signedValue = integerLiteral.Value;
                return IsUnchecked(targetTypeName, signedValue, integerLiteral.IsNegative, isHex: integerLiteral.ValueString.StartsWith("0x", StringComparison.Ordinal));
            }

            case CX_StmtClass_LambdaExpr:
            {
                return false;
            }

            // case CX_StmtClass_MSPropertyRefExpr:
            // case CX_StmtClass_MSPropertySubscriptExpr:

            case CX_StmtClass_MaterializeTemporaryExpr:
            {
                return false;
            }

            case CX_StmtClass_MemberExpr:
            {
                return false;
            }

            // case CX_StmtClass_NoInitExpr:
            // case CX_StmtClass_OMPArraySectionExpr:
            // case CX_StmtClass_ObjCArrayLiteral:
            // case CX_StmtClass_ObjCAvailabilityCheckExpr:
            // case CX_StmtClass_ObjCBoolLiteralExpr:
            // case CX_StmtClass_ObjCBoxedExpr:
            // case CX_StmtClass_ObjCDictionaryLiteral:
            // case CX_StmtClass_ObjCEncodeExpr:
            // case CX_StmtClass_ObjCIndirectCopyRestoreExpr:
            // case CX_StmtClass_ObjCIsaExpr:
            // case CX_StmtClass_ObjCIvarRefExpr:
            // case CX_StmtClass_ObjCMessageExpr:
            // case CX_StmtClass_ObjCPropertyRefExpr:
            // case CX_StmtClass_ObjCProtocolExpr:
            // case CX_StmtClass_ObjCSelectorExpr:
            // case CX_StmtClass_ObjCStringLiteral:
            // case CX_StmtClass_ObjCSubscriptRefExpr:

            case CX_StmtClass_OffsetOfExpr:
            {
                return false;
            }

            // case CX_StmtClass_OpaqueValueExpr:
            // case CX_StmtClass_UnresolvedLookupExpr:
            // case CX_StmtClass_UnresolvedMemberExpr:
            // case CX_StmtClass_PackExpansionExpr:

            case CX_StmtClass_ParenExpr:
            {
                var parenExpr = (ParenExpr)stmt;
                return IsUnchecked(targetTypeName, parenExpr.SubExpr)
                    || IsUnchecked(targetTypeName, parenExpr.Handle.Evaluate);
            }

            case CX_StmtClass_ParenListExpr:
            {
                var parenListExpr = (ParenListExpr)stmt;

                foreach (var expr in parenListExpr.Exprs)
                {
                    if (IsUnchecked(targetTypeName, expr) || IsUnchecked(targetTypeName, expr.Handle.Evaluate))
                    {
                        return true;
                    }
                }

                return false;
            }

            // case CX_StmtClass_PredefinedExpr:
            // case CX_StmtClass_PseudoObjectExpr:
            // case CX_StmtClass_RequiresExpr:
            // case CX_StmtClass_ShuffleVectorExpr:
            // case CX_StmtClass_SizeOfPackExpr:
            // case CX_StmtClass_SourceLocExpr:
            // case CX_StmtClass_StmtExpr:

            case CX_StmtClass_StringLiteral:
            {
                return false;
            }

            case CX_StmtClass_SubstNonTypeTemplateParmExpr:
            {
                return false;
            }

            // case CX_StmtClass_SubstNonTypeTemplateParmPackExpr:
            // case CX_StmtClass_TypeTraitExpr:
            // case CX_StmtClass_TypoExpr:

            case CX_StmtClass_UnaryExprOrTypeTraitExpr:
            {
                var unaryExprOrTypeTraitExpr = (UnaryExprOrTypeTraitExpr)stmt;

                var argumentType = unaryExprOrTypeTraitExpr.TypeOfArgument;

                long alignment32 = -1;
                long alignment64 = -1;

                GetTypeSize(unaryExprOrTypeTraitExpr, argumentType, ref alignment32, ref alignment64, out var size32, out var size64);

                switch (unaryExprOrTypeTraitExpr.Kind)
                {
                    case CX_UETT_SizeOf:
                    {
                        switch (targetTypeName)
                        {
                            case "bool":
                            case "Boolean":
                            case "byte":
                            case "Byte":
                            case "char":
                            case "Char":
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
                                return (size32 != size64) || !IsPrevContextDecl<VarDecl>(out _, out _);
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

            case CX_StmtClass_UnaryOperator:
            {
                var unaryOperator = (UnaryOperator)stmt;

                if (IsUnchecked(targetTypeName, unaryOperator.SubExpr))
                {
                    return true;
                }

                var evaluation = unaryOperator.Handle.Evaluate;

                if (IsUnchecked(targetTypeName, evaluation))
                {
                    return true;
                }

                var sourceTypeName = GetTypeName(stmt, context: null, unaryOperator.SubExpr.Type, ignoreTransparentStructsWhereRequired: false, out _);

                switch (unaryOperator.Opcode)
                {
                    case CXUnaryOperator_Minus:
                    {
                        return IsUnsigned(targetTypeName);
                    }

                    case CXUnaryOperator_Not:
                    {
                        return IsUnsigned(targetTypeName) != IsUnsigned(sourceTypeName);
                    }

                    default:
                    {
                        return false;
                    }
                }
            }

            // case CX_StmtClass_VAArgExpr:

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

            long lhsValue, rhsValue;

            if (IsStmtAsWritten<IntegerLiteral>(lhs, out var lhsIntegerLiteral, removeParens: true))
            {
                lhsValue = lhsIntegerLiteral.Value;
            }
            else
            {
                var lhsEvaluation = lhs.Handle.Evaluate;

                if (lhsEvaluation.Kind == CXEval_Int)
                {
                    lhsValue = lhsEvaluation.AsInt;
                }
                else
                {
                    return false;
                }
            }

            if (IsStmtAsWritten<IntegerLiteral>(rhs, out var rhsIntegerLiteral, removeParens: true))
            {
                rhsValue = rhsIntegerLiteral.Value;
            }
            else
            {
                var rhsEvaluation = rhs.Handle.Evaluate;

                if (rhsEvaluation.Kind == CXEval_Int)
                {
                    rhsValue = rhsEvaluation.AsInt;
                }
                else
                {
                    return false;
                }
            }

            var targetTypeName = GetRemappedTypeName(binaryOperator, context: null, binaryOperator.Type, out _, skipUsing: true);
            var isUnsigned = IsUnsigned(targetTypeName);

            switch (binaryOperator.Opcode)
            {
                case CXBinaryOperator_Add:
                {
                    return isUnsigned
                        ? (ulong)lhsValue + (ulong)rhsValue < (ulong)lhsValue
                        : lhsValue + rhsValue < lhsValue;
                }

                case CXBinaryOperator_Sub:
                {
                    return isUnsigned
                        ? (ulong)lhsValue - (ulong)rhsValue > (ulong)lhsValue
                        : lhsValue - rhsValue > lhsValue;
                }

                default:
                {
                    return false;
                }
            }
        }
    }

    private static bool IsUnchecked(string typeName, CXEvalResult evalResult)
    {
        if (evalResult.Kind != CXEval_Int)
        {
            return false;
        }

        var signedValue = evalResult.AsLongLong;
        return IsUnchecked(typeName, signedValue, signedValue < 0, isHex: false);
    }

    private static bool IsUnchecked(string typeName, long signedValue, bool isNegative, bool isHex)
    {
        switch (typeName)
        {
            case "byte":
            case "Byte":
            {
                var unsignedValue = unchecked((ulong)signedValue);
                return unsignedValue is < byte.MinValue or > byte.MaxValue;
            }

            case "char":
            case "Char":
            {
                var unsignedValue = unchecked((ulong)signedValue);
                return unsignedValue is < char.MinValue or > char.MaxValue;
            }

            case "ushort":
            case "UInt16":
            {
                var unsignedValue = unchecked((ulong)signedValue);
                return unsignedValue is < ushort.MinValue or > ushort.MaxValue;
            }

            case "uint":
            case "UInt32":
            case "nuint":
            case "UIntPtr":
            {
                return false;
            }

            case "ulong":
            case "UInt64":
            {
                return false;
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
            case "IntPtr":
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

        if (IsType<ArrayType>(fieldDecl, out _) && IsTypeConstantOrIncompleteArray(fieldDecl, type))
        {
            var remappedName = GetRemappedTypeName(fieldDecl, context: null, type, out _, skipUsing: true, ignoreTransparentStructsWhereRequired: false);
            return IsSupportedFixedSizedBufferType(remappedName);
        }

        return IsUnsafe(fieldDecl, type);
    }

    private bool IsUnsafe(FunctionDecl functionDecl)
    {
        var name = GetRemappedCursorName(functionDecl);

        if (_config.WithManualImports.Contains(name))
        {
            return true;
        }

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
            else if ((decl is RecordDecl nestedRecordDecl) && nestedRecordDecl.IsAnonymousStructOrUnion && (IsUnsafe(nestedRecordDecl) || Config.GenerateCompatibleCode))
            {
                return true;
            }
        }
        return (recordDecl is CXXRecordDecl cxxRecordDecl) && (HasVtbl(cxxRecordDecl, out var hasBaseVtbl) || hasBaseVtbl || HasUnsafeMethod(cxxRecordDecl));
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
        var remappedName = GetRemappedTypeName(namedDecl, context: null, type, out _, skipUsing: true, ignoreTransparentStructsWhereRequired: false);
        return remappedName.Contains('*', StringComparison.Ordinal);
    }

    private static bool IsUnsigned(string typeName)
    {
        switch (typeName)
        {
            case "byte":
            case "Byte":
            case "char":
            case "Char":
            case "nuint":
            case "UInt16":
            case "uint":
            case "UInt32":
            case "ulong":
            case "UInt64":
            case "UIntPtr":
            case "ushort":
            case var _ when typeName.EndsWith('*'):
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
            if (IsType<BuiltinType>(cxxMethodDecl, cxxMethodDecl.ReturnType))
            {
                // No fixup needed
            }
            else if (IsType<EnumType>(cxxMethodDecl, cxxMethodDecl.ReturnType))
            {
                // No fixup needed
            }
            else if (IsType<PointerType>(cxxMethodDecl, cxxMethodDecl.ReturnType))
            {
                // No fixup needed
            }
            else if (IsType<RecordType>(cxxMethodDecl, cxxMethodDecl.ReturnType))
            {
                needsReturnFixup = !_config.GenerateCallConvMemberFunction;
            }
            else
            {
                AddDiagnostic(DiagnosticLevel.Error, $"Unsupported return type for abstract method: '{cxxMethodDecl.ReturnType.TypeClass}'. Generated bindings may be incomplete.", cxxMethodDecl);
            }
        }

        return needsReturnFixup;
    }

    private static bool NeedsNewKeyword(string name)
    {
        return name.Equals("Equals",StringComparison.Ordinal)
            || name.Equals("GetHashCode", StringComparison.Ordinal)
            || name.Equals("GetType", StringComparison.Ordinal)
            || name.Equals("MemberwiseClone", StringComparison.Ordinal)
            || name.Equals("ReferenceEquals", StringComparison.Ordinal)
            || name.Equals("ToString", StringComparison.Ordinal);
    }

    private static bool NeedsNewKeyword(string name, IReadOnlyList<ParmVarDecl> parmVarDecls)
    {
        return (parmVarDecls.Count == 0) && (name.Equals("GetHashCode", StringComparison.Ordinal)
            || name.Equals("GetType", StringComparison.Ordinal)
            || name.Equals("MemberwiseClone", StringComparison.Ordinal)
            || name.Equals("ToString", StringComparison.Ordinal));
    }

    private void ParenthesizeStmt(Stmt stmt)
    {
        if (IsStmtAsWritten<ParenExpr>(stmt, out _))
        {
            Visit(stmt);
        }
        else
        {
            if (_stmtOutputBuilderUsers > 0)
            {
                Debug.Assert(_stmtOutputBuilder is not null);

                _stmtOutputBuilder.Write('(');
                _stmtOutputBuilder.BeginMarker("value");
                Visit(stmt);
                _stmtOutputBuilder.EndMarker("value");
                _stmtOutputBuilder.Write(')');
            }
            else
            {
                Debug.Assert(_outputBuilder is not null);

                _outputBuilder.BeginInnerValue();
                Visit(stmt);
                _outputBuilder.EndInnerValue();
            }
        }
    }

    private string PrefixAndStripName(string name, uint overloadIndex)
    {
        if (name.StartsWith(_config.MethodPrefixToStrip, StringComparison.Ordinal))
        {
            name = name[_config.MethodPrefixToStrip.Length..];
        }

        return $"_{name}{((overloadIndex != 0) ? overloadIndex.ToString(CultureInfo.InvariantCulture) : "")}";
    }

    private void StartUsingOutputBuilder(string name, bool includeTestOutput = false)
    {
        var nameTests = $"{name}Tests";

        // Set the current namespace so subsequent type lookups add the right using
        _currentNamespace = GetNamespace(name);
        _currentClass = name;

        if (_outputBuilder != null)
        {
            Debug.Assert(_outputBuilderUsers >= 1);
            _outputBuilderUsers++;

            _outputBuilder.WriteDivider();

            if (includeTestOutput && !string.IsNullOrWhiteSpace(_config.TestOutputLocation))
            {
                if (_testOutputBuilder is null)
                {
                    if (!_outputBuilderFactory.TryGetOutputBuilder(nameTests, out var testOutputBuilder))
                    {
                        CreateTestOutputBuilder(name, nameTests);
                    }
                    else
                    {
                        _testOutputBuilder = (CSharpOutputBuilder)testOutputBuilder;
                    }
                }

                Debug.Assert(_testOutputBuilder is not null);
                _testOutputBuilder.NeedsNewline = true;
            }
            return;
        }

        Debug.Assert(_outputBuilderUsers == 0);

        if (!_outputBuilderFactory.TryGetOutputBuilder(name, out _outputBuilder))
        {
            _outputBuilder = _outputBuilderFactory.Create(name);

            if (includeTestOutput && !string.IsNullOrWhiteSpace(_config.TestOutputLocation))
            {
                CreateTestOutputBuilder(name, nameTests);
            }
        }
        else
        {
            _outputBuilder.WriteDivider();

            if (includeTestOutput && !string.IsNullOrWhiteSpace(_config.TestOutputLocation))
            {
                if (!_outputBuilderFactory.TryGetOutputBuilder(nameTests, out var testOutputBuilder))
                {
                    CreateTestOutputBuilder(name, nameTests);
                }
                else
                {
                    _testOutputBuilder = (CSharpOutputBuilder)testOutputBuilder;
                }

                Debug.Assert(_testOutputBuilder is not null);
                _testOutputBuilder.NeedsNewline = true;
            }
        }
        _outputBuilderUsers++;

        void CreateTestOutputBuilder(string name, string nameTests)
        {
            _testOutputBuilder = _outputBuilderFactory.CreateTests(nameTests);

            var isTopLevelStruct = _config.WithTypes.TryGetValue(name, out var withType) && withType.Equals("struct", StringComparison.Ordinal);

            if (!_topLevelClassNames.Contains(name) || isTopLevelStruct)
            {
                _testOutputBuilder.AddUsingDirective("System.Runtime.InteropServices");
            }

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

    private void StopUsingOutputBuilder()
    {
        if (_outputBuilderUsers == 1)
        {
            _currentClass = null;
            _currentNamespace = null;
            _outputBuilder = null;
            _testOutputBuilder = null;
        }
        _outputBuilderUsers--;
    }

    private bool TryGetUuid(RecordDecl recordDecl, out Guid uuid)
    {
        if (TryGetRemappedValue(recordDecl, _config.WithGuids, out var guid))
        {
            uuid = guid;
            return true;
        }

        var uuidAttrs = recordDecl.Attrs.Where((attr) => attr.Kind == CX_AttrKind_Uuid).ToArray();

        if (uuidAttrs.Length == 0)
        {
            uuid = Guid.Empty;
            return false;
        }
        else if (uuidAttrs.Length != 1)
        {
            AddDiagnostic(DiagnosticLevel.Warning, $"Multiply uuid attributes for {recordDecl.Name}. Falling back to first attribute.", recordDecl);
        }

        var uuidAttr = uuidAttrs[0];
        var uuidAttrText = GetSourceRangeContents(recordDecl.TranslationUnit.Handle, uuidAttr.Extent);
        var uuidText = uuidAttrText.Split(s_doubleQuoteSeparator, StringSplitOptions.RemoveEmptyEntries)[1];

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

        if (functionDecl.IsInstance)
        {
            numArgs++;
        }

        if (functionDecl is CXXConversionDecl)
        {
            var returnType = functionDecl.ReturnType;
            var returnTypeName = GetRemappedTypeName(cursor: null, context: null, returnType, out _, skipUsing: true);

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
        Debug.Assert(_outputBuilder is not null);

        if (!_outputBuilder.IsUncheckedContext && IsUnchecked(targetTypeName, stmt))
        {
            _outputBuilder.BeginUnchecked();

            var needsCast = false;

            if (stmt.DeclContext is EnumDecl)
            {
                if (IsStmtAsWritten<IntegerLiteral>(stmt, out _, removeParens: true))
                {
                    needsCast = true;
                }
                else if (stmt is ImplicitCastExpr)
                {
                    needsCast = true;
                }
            }

            if (IsStmtAsWritten<UnaryExprOrTypeTraitExpr>(stmt, out var unaryExprOrTypeTraitExpr, removeParens: true) && ((CurrentContext.Cursor is VarDecl) || IsPrevContextDecl<VarDecl>(out _, out _)))
            {
                var argumentType = unaryExprOrTypeTraitExpr.TypeOfArgument;

                long alignment32 = -1;
                long alignment64 = -1;

                GetTypeSize(unaryExprOrTypeTraitExpr, argumentType, ref alignment32, ref alignment64, out var size32, out var size64);

                switch (unaryExprOrTypeTraitExpr.Kind)
                {
                    case CX_UETT_SizeOf:
                    {
                        needsCast |= size32 != size64;
                        break;
                    }

                    case CX_UETT_AlignOf:
                    case CX_UETT_PreferredAlignOf:
                    {
                        needsCast |= alignment32 != alignment64;
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
                _outputBuilder.BeginInnerValue();
                _outputBuilder.BeginInnerCast();
                _outputBuilder.WriteCastType(targetTypeName);
                _outputBuilder.EndInnerCast();
            }

            ParenthesizeStmt(stmt);

            if (needsCast)
            {
                _outputBuilder.EndInnerValue();
            }

            _outputBuilder.EndUnchecked();
        }
        else
        {
            VisitStmt(stmt);
        }
    }

    private void Visit(Cursor cursor)
    {
        var currentContext = _context.AddLast((cursor, null));
        var currentStmtUsers = _stmtOutputBuilder is not null ? (int?)_stmtOutputBuilderUsers : null;

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
            var targetTypeName = GetTargetTypeName(PreviousContext.Cursor, out _);

            if (!string.IsNullOrEmpty(targetTypeName))
            {
                UncheckStmt(targetTypeName, stmt);
            }
            else
            {
                VisitStmt(stmt);
            }
        }
        else
        {
            AddDiagnostic(DiagnosticLevel.Error, $"Unsupported cursor: '{cursor.CursorKindSpelling}'. Generated bindings may be incomplete.", cursor);
        }

        Debug.Assert(currentStmtUsers == (_stmtOutputBuilder is not null ? (int?)_stmtOutputBuilderUsers : null));
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

    private void Visit(IEnumerable<Cursor> cursors, IEnumerable<Cursor> excludedCursors) => Visit(cursors.Except(excludedCursors));

    private void WithAttributes(NamedDecl namedDecl, bool onlySupportedOSPlatform = false, bool isTestOutput = false)
    {
        var outputBuilder = isTestOutput ? _testOutputBuilder : _outputBuilder;
        Debug.Assert(outputBuilder is not null);

        if (TryGetRemappedValue(namedDecl, _config.WithAttributes, out var attributes))
        {
            foreach (var attribute in attributes.Where((a) => !onlySupportedOSPlatform || a.StartsWith("SupportedOSPlatform(", StringComparison.Ordinal)))
            {
                outputBuilder.WriteCustomAttribute(attribute);
            }
        }

        if (!isTestOutput)
        {
            var declAttrs = namedDecl.HasAttrs
                ? namedDecl.Attrs
                : Enumerable.Empty<Attr>();

            if (namedDecl is FieldDecl fieldDecl)
            {
                if (IsType<TypedefType>(fieldDecl, out var typedefType))
                {
                    declAttrs = declAttrs.Concat(typedefType.Decl.Attrs);
                }
            }
            else if (namedDecl is RecordDecl recordDecl)
            {
                var typedefName = recordDecl.TypedefNameForAnonDecl;

                if ((typedefName is not null) && typedefName.HasAttrs)
                {
                    declAttrs = declAttrs.Concat(typedefName.Attrs);
                }
            }

            var obsoleteEmitted = false;

            foreach (var attr in declAttrs)
            {
                switch (attr.Kind)
                {
                    case CX_AttrKind_Aligned:
                    case CX_AttrKind_AlwaysInline:
                    case CX_AttrKind_DLLExport:
                    case CX_AttrKind_DLLImport:
                    {
                        // Nothing to handle
                        break;
                    }

                    case CX_AttrKind_Deprecated:
                    {
                        if (obsoleteEmitted)
                        {
                            break;
                        }

                        var attrText = GetSourceRangeContents(namedDecl.TranslationUnit.Handle, attr.Extent);

                        var textStart = attrText.IndexOf('"', StringComparison.Ordinal);
                        var textLength = attrText.LastIndexOf('"') - textStart;

                        if (textLength > 1)
                        {
                            var text = attrText.AsSpan(textStart + 1, textLength - 1);
                            outputBuilder.WriteCustomAttribute($"Obsolete(\"{text}\")");
                        }
                        else
                        {
                            outputBuilder.WriteCustomAttribute($"Obsolete");
                        }
                        obsoleteEmitted = true;
                        break;
                    }

                    case CX_AttrKind_Format:
                    case CX_AttrKind_FormatArg:
                    case CX_AttrKind_MSNoVTable:
                    case CX_AttrKind_MSAllocator:
                    case CX_AttrKind_MaxFieldAlignment:
                    case CX_AttrKind_SelectAny:
                    case CX_AttrKind_Uuid:
                    {
                        // Nothing to handle
                        break;
                    }

                    default:
                    {
                        AddDiagnostic(DiagnosticLevel.Warning, $"Unsupported attribute: '{attr.KindSpelling}'. Generated bindings may be incomplete.", namedDecl);
                        break;
                    }
                }
            }
        }
    }

    private string GetLibraryPath(string remappedName)
    {
        return !_config.WithLibraryPaths.TryGetValue(remappedName, out var libraryPath) && !_config.WithLibraryPaths.TryGetValue("*", out libraryPath)
            ? _config.LibraryPath
            : libraryPath;
    }

    private string GetClass(string remappedName, bool disallowPrefixMatch = false)
    {
        if (!TryGetClass(remappedName, out var className, disallowPrefixMatch))
        {
            className = _config.DefaultClass;
            _ = _topLevelClassNames.Add(className);
            _ = _topLevelClassNames.Add($"{className}Tests");
        }
        return className;
    }

    private bool TryGetClass(string remappedName, [MaybeNullWhen(false)] out string className, bool disallowPrefixMatch = false)
    {
        var index = remappedName.IndexOf('*', StringComparison.Ordinal);

        if (index != -1)
        {
            remappedName = remappedName[..index];
        }

        if (_config.WithClasses.TryGetValue(remappedName, out className))
        {
            _ = _topLevelClassNames.Add(className);
            _ = _topLevelClassNames.Add($"{className}Tests");
            return true;
        }

        if (disallowPrefixMatch)
        {
            return false;
        }

        foreach (var withClass in _config.WithClasses)
        {
            if (!withClass.Key.EndsWith('*'))
            {
                continue;
            }

            var prefix = withClass.Key[0..^1];

            if (remappedName.StartsWith(prefix, StringComparison.Ordinal))
            {
                className = withClass.Value;
                _ = _topLevelClassNames.Add(className);
                _ = _topLevelClassNames.Add($"{className}Tests");
                return true;
            }
        }
        return false;
    }

    private string GetNamespace(string remappedName, NamedDecl? namedDecl = null)
    {
        if (!TryGetNamespace(remappedName, out var namespaceName))
        {
            if ((namedDecl is not null) && (namedDecl.Parent is TypeDecl parentTypeDecl))
            {
                var parentName = GetRemappedCursorName(parentTypeDecl);
                namespaceName = $"{GetNamespace(parentName, parentTypeDecl)}.{parentName}";
            }
            else
            {
                namespaceName = NeedsSystemSupportRegex().IsMatch(remappedName) ? "System" : _config.DefaultNamespace;
            }
        }
        return namespaceName;
    }

    private bool TryGetNamespace(string remappedName, [MaybeNullWhen(false)] out string namespaceName)
    {
        var index = remappedName.IndexOf('*', StringComparison.Ordinal);

        if (index != -1)
        {
            remappedName = remappedName[..index];
        }

        return _config.WithNamespaces.TryGetValue(remappedName, out namespaceName);
    }

    private bool GetSetLastError(NamedDecl namedDecl) => HasRemapping(namedDecl, _config.WithSetLastErrors, matchStar: true);

    private bool HasRemapping(NamedDecl namedDecl, IReadOnlyCollection<string> entries, bool matchStar = false)
    {
        var name = GetCursorQualifiedName(namedDecl);

        if (name.StartsWith("ClangSharpMacro_", StringComparison.Ordinal))
        {
            name = name["ClangSharpMacro_".Length..];
        }

        if (entries.Contains(name))
        {
            return true;
        }

        name = name.Replace("::", ".", StringComparison.Ordinal);

        if (entries.Contains(name))
        {
            return true;
        }

        name = GetCursorQualifiedName(namedDecl, truncateParameters: true);

        if (name.StartsWith("ClangSharpMacro_", StringComparison.Ordinal))
        {
            name = name["ClangSharpMacro_".Length..];
        }

        if (entries.Contains(name))
        {
            return true;
        }

        name = name.Replace("::", ".", StringComparison.Ordinal);

        if (entries.Contains(name))
        {
            return true;
        }

        name = GetRemappedCursorName(namedDecl);

        if (name.StartsWith("ClangSharpMacro_", StringComparison.Ordinal))
        {
            name = name["ClangSharpMacro_".Length..];
        }

        return entries.Contains(name) || (matchStar && entries.Contains("*"));
    }

    private bool TryGetRemappedValue<T>(NamedDecl namedDecl, IReadOnlyDictionary<string, T> remappings, [MaybeNullWhen(false)] out T value, bool matchStar = false)
    {
        var name = GetCursorQualifiedName(namedDecl);

        if (name.StartsWith("ClangSharpMacro_", StringComparison.Ordinal))
        {
            name = name["ClangSharpMacro_".Length..];
        }

        if (remappings.TryGetValue(name, out value))
        {
            return true;
        }

        name = name.Replace("::", ".", StringComparison.Ordinal);

        if (remappings.TryGetValue(name, out value))
        {
            return true;
        }

        name = GetCursorQualifiedName(namedDecl, truncateParameters: true);

        if (name.StartsWith("ClangSharpMacro_", StringComparison.Ordinal))
        {
            name = name["ClangSharpMacro_".Length..];
        }

        if (remappings.TryGetValue(name, out value))
        {
            return true;
        }

        name = name.Replace("::", ".", StringComparison.Ordinal);

        if (remappings.TryGetValue(name, out value))
        {
            return true;
        }

        name = GetRemappedCursorName(namedDecl);

        if (name.StartsWith("ClangSharpMacro_", StringComparison.Ordinal))
        {
            name = name["ClangSharpMacro_".Length..];
        }

        if (remappings.TryGetValue(name, out value))
        {
            return true;
        }

        if (matchStar && remappings.TryGetValue("*", out value))
        {
            return true;
        }

        value = default;
        return false;
    }

    private void WithTestAttribute()
    {
        if (_config.GenerateTestsNUnit)
        {
            Debug.Assert(_testOutputBuilder is not null);
            _testOutputBuilder.WriteIndentedLine("[Test]");
        }
        else if (_config.GenerateTestsXUnit)
        {
            Debug.Assert(_testOutputBuilder is not null);
            _testOutputBuilder.WriteIndentedLine("[Fact]");
        }
    }

    private void WithTestAssertEqual(string expected, string actual)
    {
        if (_config.GenerateTestsNUnit)
        {
            Debug.Assert(_testOutputBuilder is not null);

            _testOutputBuilder.WriteIndented("Assert.That");
            _testOutputBuilder.Write('(');
            _testOutputBuilder.Write(actual);
            _testOutputBuilder.Write(", Is.EqualTo(");
            _testOutputBuilder.Write(expected);
            _testOutputBuilder.Write("))");
            _testOutputBuilder.WriteSemicolon();
            _testOutputBuilder.WriteNewline();
        }
        else if (_config.GenerateTestsXUnit)
        {
            Debug.Assert(_testOutputBuilder is not null);

            _testOutputBuilder.WriteIndented("Assert.Equal");
            _testOutputBuilder.Write('(');
            _testOutputBuilder.Write(expected);
            _testOutputBuilder.Write(", ");
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
            Debug.Assert(_testOutputBuilder is not null);

            _testOutputBuilder.WriteIndented("Assert.That");
            _testOutputBuilder.Write('(');
            _testOutputBuilder.Write(actual);
            _testOutputBuilder.Write(", Is.True)");
            _testOutputBuilder.WriteSemicolon();
            _testOutputBuilder.WriteNewline();
        }
        else if (_config.GenerateTestsXUnit)
        {
            Debug.Assert(_testOutputBuilder is not null);

            _testOutputBuilder.WriteIndented("Assert.True");
            _testOutputBuilder.Write('(');
            _testOutputBuilder.Write(actual);
            _testOutputBuilder.Write(')');
            _testOutputBuilder.WriteSemicolon();
            _testOutputBuilder.WriteNewline();
        }
    }

    private void WithType(NamedDecl namedDecl, ref string integerTypeName, ref string nativeTypeName)
    {
        if (TryGetRemappedValue(namedDecl, _config.WithTypes, out var type, matchStar: true))
        {
            if (string.IsNullOrWhiteSpace(nativeTypeName))
            {
                nativeTypeName = integerTypeName;
            }

            integerTypeName = type;

            if (IsNativeTypeNameEquivalent(nativeTypeName, type))
            {
                nativeTypeName = string.Empty;
            }
        }
    }

    private void WithUsings(NamedDecl namedDecl)
    {
        Debug.Assert(_outputBuilder is not null);

        if (TryGetRemappedValue(namedDecl, _config.WithUsings, out var usings))
        {
            foreach (var @using in usings)
            {
                _outputBuilder.EmitUsingDirective(@using);
            }
        }
    }

    private CSharpOutputBuilder StartCSharpCode()
    {
        if ((_outputBuilder == _testOutputBuilder) && (_testStmtOutputBuilder is null))
        {
            _testStmtOutputBuilder = _stmtOutputBuilder;
            _testStmtOutputBuilderUsers = _stmtOutputBuilderUsers;

            _stmtOutputBuilder = null;
            _stmtOutputBuilderUsers = 0;
        }

        if (_stmtOutputBuilder is null)
        {
            Debug.Assert(_outputBuilder is not null);
            _stmtOutputBuilder = _outputBuilder.BeginCSharpCode();
            _stmtOutputBuilderUsers = 1;
        }
        else
        {
            _stmtOutputBuilderUsers++;
        }

        return _stmtOutputBuilder;
    }

    private void StopCSharpCode()
    {
        _stmtOutputBuilderUsers--;
        if (_stmtOutputBuilderUsers <= 0)
        {
            Debug.Assert(_outputBuilder is not null);
            Debug.Assert(_stmtOutputBuilder is not null);

            _outputBuilder.EndCSharpCode(_stmtOutputBuilder);

            if (_testStmtOutputBuilder is not null)
            {
                _stmtOutputBuilder = _testStmtOutputBuilder;
                _stmtOutputBuilderUsers = _testStmtOutputBuilderUsers;

                _testStmtOutputBuilder = null;
                _testStmtOutputBuilderUsers = 0;
            }
            else
            {
                _stmtOutputBuilder = null;
            }
        }
    }

    private string GetPlaceholderMacroType()
    {
        const string Cxx11AutoType = "auto";
        const string GnuAutoType = "__auto_type";

        if (_config.Language is "c++")
        {
            if (string.IsNullOrEmpty(_config.LanguageStandard))
            {
                return Cxx11AutoType; // Nowadays, the default standard within clang will always be above c++11
            }

            var cxxStandard = ParseCxxStandard(_config.LanguageStandard);
            return (cxxStandard is not -1 and not 98 and >= 11) ? Cxx11AutoType : GnuAutoType;
        }
        else
        {
            return GnuAutoType;
        }
    }

    [GeneratedRegex(@"\b(?:Guid|IntPtr|UIntPtr)\b")]
    private static partial Regex NeedsSystemSupportRegex();

    private static int ParseCxxStandard(string standard)
    {
        const string CxxStandard = "c++";
        const string GnuxxStandard = "gnu++";

        return standard.StartsWith(CxxStandard, StringComparison.Ordinal) ? ParseCxxStandardVersion(standard.AsSpan()[CxxStandard.Length..])
             : standard.StartsWith(GnuxxStandard, StringComparison.Ordinal) ? ParseCxxStandardVersion(standard.AsSpan()[GnuxxStandard.Length..])
             : -1;
    }

    private static int ParseCxxStandardVersion(ReadOnlySpan<char> version)
    {
        // Maybe in the future we'll need to check for more c++ standard drafts, but atm this should work

        if (version.EndsWith("a") || version.EndsWith("b"))
        {
            if (int.TryParse(version[..^1], out var draftStd))
            {
                return draftStd * 10;
            }
        }
        else if (int.TryParse(version, out var std))
        {
            return std;
        }

        return -1;
    }
}
