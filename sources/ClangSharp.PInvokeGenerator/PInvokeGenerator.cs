// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using ClangSharp.Abstractions;
using ClangSharp.CSharp;
using ClangSharp.Interop;
using ClangSharp.XML;
using static ClangSharp.Interop.CX_AttrKind;
using static ClangSharp.Interop.CX_CXXAccessSpecifier;
using static ClangSharp.Interop.CX_StmtClass;
using static ClangSharp.Interop.CX_UnaryExprOrTypeTrait;
using static ClangSharp.Interop.CXBinaryOperatorKind;
using static ClangSharp.Interop.CXCallingConv;
using static ClangSharp.Interop.CXDiagnosticSeverity;
using static ClangSharp.Interop.CXEvalResultKind;
using static ClangSharp.Interop.CXTemplateArgumentKind;
using static ClangSharp.Interop.CXTranslationUnit_Flags;
using static ClangSharp.Interop.CXTypeKind;
using static ClangSharp.Interop.CXUnaryOperatorKind;

namespace ClangSharp;

public sealed partial class PInvokeGenerator : IDisposable
{
    private const int DefaultStreamWriterBufferSize = 1024;

    private const string AnonymousNamePrefix = "__Anonymous";
    private const string AnonymousBasePrefix = $"{AnonymousNamePrefix}Base_";
    private const string AnonymousEnumPrefix = $"{AnonymousNamePrefix}Enum_";
    private const string AnonymousFieldDeclPrefix = $"{AnonymousNamePrefix}FieldDecl_";
    private const string AnonymousRecordPrefix = $"{AnonymousNamePrefix}Record_";
    private const string AnonymousTypeKindTag = "_e__";

    private static readonly Encoding s_defaultStreamWriterEncoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: true);
    private static readonly string[] s_doubleColonSeparator = ["::"];
    private static readonly char[] s_doubleQuoteSeparator = ['"'];
    private static readonly SearchValues<char> s_qualifiedNameSeparatorChars = SearchValues.Create(".:");

    private static string ExpectedClangVersion => $"version {clang.MajorVersion}.{clang.MinorVersion}";
    private static string ExpectedClangSharpVersion => ExpectedClangVersion; // change if necessary

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
    private readonly Dictionary<CXFile, (nuint Address, nuint Length)> _fileContents;
    private readonly Dictionary<CXFile, (string Name, string FullName)> _fileNames;
    private readonly HashSet<string> _topLevelClassNames;
    private readonly HashSet<string> _usedRemappings;
    private readonly HashSet<RecordDecl> _declashedRecordNames;
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
            _visitedFiles = new HashSet<string>(StringComparer.Ordinal);
            _diagnostics = [];
            _context = new LinkedList<(Cursor, object?)>();
            _uuidsToGenerate = new Dictionary<string, Guid>(StringComparer.Ordinal);
            _generatedUuids = new HashSet<string>(StringComparer.Ordinal);
            _cursorNames = [];
            _cursorQualifiedNames = [];
            _typeNames = [];
            _allValidNameRemappings = new Dictionary<string, HashSet<string>>(QualifiedNameComparer.Default) {
                ["intptr_t"] = ["IntPtr", "nint"],
                ["ptrdiff_t"] = ["IntPtr", "nint"],
                ["size_t"] = ["UIntPtr", "nuint"],
                ["uintptr_t"] = ["UIntPtr", "nuint"],
                ["_GUID"] = ["Guid"],
            };
            _traversedValidNameRemappings = new Dictionary<string, HashSet<string>>(StringComparer.Ordinal);
            _overloadIndices = [];
            _isExcluded = [];
            _topLevelClassHasGuidMember = new Dictionary<string, bool>(StringComparer.Ordinal);
            _topLevelClassIsUnsafe = new Dictionary<string, bool>(StringComparer.Ordinal);
            _topLevelClassNames = new HashSet<string>(StringComparer.Ordinal);
            _topLevelClassAttributes = new Dictionary<string, List<string>>(StringComparer.Ordinal);
            _fileContents = [];
            _fileNames = [];
            _topLevelClassUsings = new Dictionary<string, HashSet<string>>(StringComparer.Ordinal);
            _usedRemappings = new HashSet<string>(StringComparer.Ordinal);
            _declashedRecordNames = [];
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

        // We need to clear any cached state from a previous translation unit as
        // native handle IDs or other info may have been reused if it was disposed.

        _context.Clear();
        _cursorNames.Clear();
        _cursorQualifiedNames.Clear();
        _typeNames.Clear();
        _overloadIndices.Clear();
        _isExcluded.Clear();
        _fileContents.Clear();
        _fileNames.Clear();

        if (translationUnit.Handle.NumDiagnostics != 0)
        {
            var errorDiagnostics = new StringBuilder();
            _ = errorDiagnostics.AppendLine($"The provided {nameof(CXTranslationUnit)} has the following diagnostics which prevent its use:");
            var invalidTranslationUnitHandle = false;

            for (uint i = 0; i < translationUnit.Handle.NumDiagnostics; ++i)
            {
                using var diagnostic = translationUnit.Handle.GetDiagnostic(i);

                if (diagnostic.Severity is CXDiagnostic_Error or CXDiagnostic_Fatal)
                {
                    invalidTranslationUnitHandle = true;
                    _ = errorDiagnostics.Append(' ', 4);
                    _ = errorDiagnostics.AppendLine(diagnostic.Format(CXDiagnostic.DefaultDisplayOptions).ToString());
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
                var fileContentsBuilder = _fileContentsBuilder;

                foreach (var cursor in translationUnit.TranslationUnitDecl.CursorChildren)
                {
                    if (cursor is PreprocessedEntity preprocessedEntity)
                    {
                        VisitPreprocessedEntity(preprocessedEntity);
                    }
                }

                var unsavedFileContents = fileContentsBuilder.ToString();
                _ = fileContentsBuilder.Clear();

                var translationUnitHandle = translationUnit.Handle;
                var file = translationUnitHandle.GetFile(_filePath);

                using var unsavedFile = CXUnsavedFile.Create(_filePath, translationUnitHandle, file, unsavedFileContents);
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
                    var name = kvp.Key.AsSpan();
                    var remappings = kvp.Value;

                    if (name.Contains('<'))
                    {
                        var parts = name.Split('<');

                        if (parts.MoveNext())
                        {
                            var part = parts.Current;

                            if (parts.MoveNext())
                            {
                                name = name[part];
                            }
                        }
                    }

                    var remappedNamesLookup = _config._remappedNames.GetAlternateLookup<ReadOnlySpan<char>>();

                    if (!remappedNamesLookup.TryGetValue(name, out _))
                    {
                        var addDiag = true;

                        var smlName = name;
                        var lastSeparatorIndex = smlName.LastIndexOfAny(s_qualifiedNameSeparatorChars);

                        if (lastSeparatorIndex != -1)
                        {
                            smlName = smlName[(lastSeparatorIndex + 1)..];
                            addDiag = false;
                        }

                        if (!addDiag && !remappedNamesLookup.TryGetValue(smlName, out _))
                        {
                            addDiag = true;
                        }

                        if (addDiag)
                        {
                            var remappingsLookup = remappings.GetAlternateLookup<ReadOnlySpan<char>>();

                            if (!remappingsLookup.Contains(name) && !remappingsLookup.Contains(smlName))
                            {
                                AddDiagnostic(DiagnosticLevel.Info, $"Potential missing remapping '{name}'. {GetFoundRemappingString(name, remappings)}");
                            }
                        }
                    }
                }

                foreach (var kvp in _allValidNameRemappings)
                {
                    var name = kvp.Key;
                    var remappings = kvp.Value;

                    var remappedNamesLookup = _config._remappedNames.GetAlternateLookup<ReadOnlySpan<char>>();

                    if (remappedNamesLookup.TryGetValue(name, out var remappedName) && !remappings.Contains(remappedName) && (name != remappedName) && !_config.ForceRemappedNames.Contains(name))
                    {
                        var addDiag = true;

                        var smlName = name;
                        var lastSeparatorIndex = smlName.AsSpan().LastIndexOfAny(s_qualifiedNameSeparatorChars);

                        if (lastSeparatorIndex != -1)
                        {
                            smlName = smlName[(lastSeparatorIndex + 1)..];
                            addDiag = false;
                        }

                        if (!addDiag && remappedNamesLookup.TryGetValue(smlName, out remappedName) && !remappings.Contains(remappedName) && (smlName != remappedName) && !_config.ForceRemappedNames.Contains(smlName))
                        {
                            addDiag = true;
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

                    var allValidNameRemappingsLookup = _allValidNameRemappings.GetAlternateLookup<ReadOnlySpan<char>>();

                    if (!allValidNameRemappingsLookup.ContainsKey(name) && (name != remappedName) && !_config.ForceRemappedNames.Contains(name))
                    {
                        var addDiag = true;

                        var smlName = name;
                        var lastSeparatorIndex = smlName.AsSpan().LastIndexOfAny(s_qualifiedNameSeparatorChars);

                        if (lastSeparatorIndex != -1)
                        {
                            smlName = smlName[(lastSeparatorIndex + 1)..];
                            addDiag = false;
                        }

                        if (!addDiag && !allValidNameRemappingsLookup.ContainsKey(smlName) && (smlName != remappedName) && !_config.ForceRemappedNames.Contains(smlName))
                        {
                            addDiag = true;
                        }

                        if (addDiag)
                        {
                            AddDiagnostic(DiagnosticLevel.Info, $"Potential invalid remapping '{name}={remappedName}'. No remappings were found.");
                        }
                    }
                }

                static ReadOnlySpan<char> GetFoundRemappingString(ReadOnlySpan<char> name, HashSet<string> remappings)
                {
                    var recommendedRemappingString = "";
                    var recommendedRemapping = recommendedRemappingString.AsSpan();

                    if (remappings.Count == 1)
                    {
                        recommendedRemappingString = remappings.Single();
                        recommendedRemapping = recommendedRemappingString;
                    }

                    var remappingsLookup = remappings.GetAlternateLookup<ReadOnlySpan<char>>();

                    if (recommendedRemapping.IsWhiteSpace() && name.StartsWith('_'))
                    {
                        var remapping = name[1..];

                        if (remappingsLookup.Contains(remapping))
                        {
                            recommendedRemapping = remapping;
                            recommendedRemappingString = null;
                        }
                    }

                    if (recommendedRemapping.IsWhiteSpace() && name.StartsWith("tag", StringComparison.Ordinal))
                    {
                        var remapping = name[3..];

                        if (remappingsLookup.Contains(remapping))
                        {
                            recommendedRemapping = remapping;
                            recommendedRemappingString = null;
                        }
                    }

                    if (recommendedRemapping.IsWhiteSpace() && name.EndsWith('_'))
                    {
                        var remapping = name[..^1];

                        if (remappingsLookup.Contains(remapping))
                        {
                            recommendedRemapping = remapping;
                            recommendedRemappingString = null;
                        }
                    }

                    if (recommendedRemapping.IsWhiteSpace() && name.EndsWith("tag", StringComparison.Ordinal))
                    {
                        var remapping = name[..^3];

                        if (remappingsLookup.Contains(remapping))
                        {
                            recommendedRemapping = remapping;
                            recommendedRemappingString = null;
                        }
                    }

                    if (recommendedRemapping.IsWhiteSpace())
                    {
                        var remapping = name.ToString().ToUpperInvariant();

                        if (remappingsLookup.Contains(remapping))
                        {
                            recommendedRemappingString = remapping;
                            recommendedRemapping = recommendedRemappingString;
                        }
                    }

                    var result = "";
                    var remainingRemappings = (IEnumerable<string>)remappings;
                    var remainingString = "Found";

                    if (!recommendedRemapping.IsWhiteSpace())
                    {
                        result += $"Recommended remapping: '{name}={recommendedRemapping}'.";

                        if (remappings.Count == 1)
                        {
                            remainingRemappings = [];
                        }
                        else
                        {
                            result += ' ';
                            remainingRemappings = remappings.Except([recommendedRemappingString ?? recommendedRemapping.ToString()]);
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
                    var nonTestName = outputBuilder.IsTestOutput ? outputBuilder.Name.AsSpan()[..^5] : outputBuilder.Name;

                    if (_topLevelClassUsings.GetAlternateLookup<ReadOnlySpan<char>>().TryGetValue(nonTestName, out var withUsings))
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

        void ForCSharp(CSharpOutputBuilder outputBuilder)
        {
            var indentationString = Config.GenerateFileScopedNamespaces ? "" : outputBuilder.IndentationString;
            var nonTestName = outputBuilder.IsTestOutput ? outputBuilder.Name.AsSpan()[..^5] : outputBuilder.Name;

            if (emitNamespaceDeclaration)
            {
                sw.Write("namespace ");
                sw.Write(GetNamespace(nonTestName));

                if (outputBuilder.IsTestOutput)
                {
                    sw.Write(".UnitTests");
                }

                if (Config.GenerateFileScopedNamespaces)
                {
                    sw.WriteLine(';');
                    sw.WriteLine();
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
                var isTopLevelStruct = _config._withTypes.GetAlternateLookup<ReadOnlySpan<char>>().TryGetValue(nonTestName, out var withType) && withType.Equals("struct", StringComparison.Ordinal);

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

                if (_topLevelClassAttributes.GetAlternateLookup<ReadOnlySpan<char>>().TryGetValue(nonTestName, out var withAttributes))
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

                if ((_topLevelClassIsUnsafe.GetAlternateLookup<ReadOnlySpan<char>>().TryGetValue(nonTestName, out var isUnsafe) && isUnsafe) || (outputBuilder.IsTestOutput && isTopLevelStruct))
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

                if ((!outputBuilder.IsTestOutput && !isTopLevelStruct) || !string.IsNullOrEmpty(outputBuilder.Contents.First()))
                {
                    sw.WriteLine();
                }

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
                indentationString = indentationString[..^outputBuilder.IndentationString.Length];

                sw.Write(indentationString);
                sw.WriteLine('}');
            }

            if (_config.GenerateMultipleFiles && !Config.GenerateFileScopedNamespaces)
            {
                sw.WriteLine('}');
            }
        }

        void ForXml(XmlOutputBuilder outputBuilder)
        {
            const string Indent = "  ";
            var indentationString = Indent;

            if (emitNamespaceDeclaration)
            {
                sw.Write(indentationString);
                sw.Write("<namespace name=\"");
                sw.Write(GetNamespace(outputBuilder.Name));
                sw.WriteLine("\">");
            }

            indentationString += Indent;

            if (isMethodClass)
            {
                sw.Write(indentationString);
                sw.Write("<class name=\"");
                sw.Write(outputBuilder.Name);
                sw.Write("\" access=\"public\" static=\"true\"");

                if (_topLevelClassIsUnsafe.TryGetValue(outputBuilder.Name, out var isUnsafe) && isUnsafe)
                {
                    sw.Write(" unsafe=\"true\"");
                }

                sw.WriteLine('>');
                indentationString += Indent;
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

    private string EscapeAndStripMethodName(string name)
    {
        name = PrefixAndStrip(name, _config.MethodPrefixToStrip);
        return EscapeName(name);
    }

    private string EscapeAndStripEnumMemberName(string name, string enumTypeName)
    {
        if (Config.StripEnumMemberTypeName)
        {
            var escapedName = PrefixAndStrip(name, enumTypeName, trimChar: '_');
            if (escapedName.Length > 0 && char.IsAsciiDigit(escapedName[0]))
            {
                escapedName = '_' + escapedName;
            }
            return escapedName;
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
        if (!TryGetRemappedValue(namedDecl, _config._withAccessSpecifiers, out var accessSpecifier, matchStar) || (accessSpecifier == AccessSpecifier.None))
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
        return $"{AnonymousNamePrefix}{kind}_{fileName}_L{line}_C{column}";
    }

    private static bool IsAnonymousEnum(string name) => name.StartsWith(AnonymousEnumPrefix, StringComparison.Ordinal);

    private static bool IsAnonymousRecord(string name) => name.StartsWith(AnonymousRecordPrefix, StringComparison.Ordinal);

    private string GetArtificialFixedSizedBufferName(FieldDecl fieldDecl)
    {
        var name = GetRemappedCursorName(fieldDecl);
        return $"_{name}{AnonymousTypeKindTag}FixedBuffer";
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
            if (TryGetRemappedValue(namedDecl, _config._withCallConvs, out var callConv, matchStar: true))
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

    private unsafe ReadOnlySpan<byte> GetFileContents(CXTranslationUnit translationUnit, CXFile file)
    {
        if (!_fileContents.TryGetValue(file, out var fileContentsMetadata))
        {
            var fileContents = translationUnit.GetFileContents(file, out _);
            fileContentsMetadata = ((nuint)Unsafe.AsPointer(ref MemoryMarshal.GetReference(fileContents)), (uint)fileContents.Length);
            _fileContents[file] = fileContentsMetadata;
        }

        return new ReadOnlySpan<byte>((byte*)fileContentsMetadata.Address, (int)fileContentsMetadata.Length);
    }

    private string GetSourceRangeContents(CXTranslationUnit translationUnit, CXSourceRange sourceRange)
    {
        sourceRange.Start.GetFileLocation(out var startFile, out _, out _, out var startOffset);
        sourceRange.End.GetFileLocation(out var endFile, out _, out _, out var endOffset);

        if (startFile != endFile)
        {
            return string.Empty;
        }

        var contents1 = GetFileContents(translationUnit, startFile);
        var contents = contents1.Slice(unchecked((int)startOffset), unchecked((int)(endOffset - startOffset)));
        return Encoding.UTF8.GetString(contents);
    }

    private bool HasSuppressGCTransition(Cursor? cursor)
        => (cursor is NamedDecl namedDecl) && HasRemapping(namedDecl, _config._withSuppressGCTransitions);

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
        var hasVtbl = cxxRecordDecl.Methods.Any((method) => method.IsVirtual && (method.OverriddenMethods.Count == 0));
        hasBaseVtbl = false;

        var indirectVtblCount = 0;

        foreach (var cxxBaseSpecifier in cxxRecordDecl.Bases)
        {
            var baseCxxRecordDecl = GetRecordDecl(cxxBaseSpecifier);

            if ((HasVtbl(baseCxxRecordDecl, out var baseHasBaseVtbl) || baseHasBaseVtbl) && !HasField(baseCxxRecordDecl))
            {
                indirectVtblCount++;
            }
        }

        // Multiple virtual bases require a distinct vtable pointer per base subobject, but the
        // generated bindings only model a single, flattened `lpVtbl`. This is true even when the
        // derived type introduces its own virtuals (`hasVtbl`), so the warning must fire regardless.
        if (indirectVtblCount > 1)
        {
            AddDiagnostic(DiagnosticLevel.Warning, "Unsupported cxx record declaration: 'multiple virtual bases'. Generated bindings may be incomplete.", cxxRecordDecl);
        }

        if (!hasVtbl)
        {
            hasBaseVtbl = indirectVtblCount != 0;
        }

        return hasVtbl;
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
        return name.Equals("Equals", StringComparison.Ordinal)
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

    /// <summary>
    /// Checks whether the specified name starts with a prefix, optionally trims a separator character following the prefix and returns the remainder.
    /// </summary>
    /// <param name="name">The name to strip.</param>
    /// <param name="prefix">The prefix to strip from <paramref name="name"/>.</param>
    /// <param name="trimChar">Additional separator that may follow <paramref name="prefix"/> which should also be trimmed away.</param>
    /// <returns>
    /// <paramref name="name"/> if it does not start with <paramref name="prefix"/>;
    /// otherwise,
    /// the remainder of <paramref name="name"/> after stripping <paramref name="prefix"/> and all immediate following occurences of <paramref name="trimChar"/> from it beginning.
    /// </returns>
    private static string PrefixAndStrip(string name, string prefix, char trimChar = '\0')
    {
        var nameSpan = name.AsSpan();
        if (nameSpan.StartsWith(prefix, StringComparison.Ordinal))
        {
            nameSpan = nameSpan[prefix.Length..];
            nameSpan = nameSpan.TrimStart(trimChar);
            return nameSpan.ToString();
        }
        return name;
    }

    private string PrefixAndStripMethodName(string name, uint overloadIndex)
    {
        name = PrefixAndStrip(name, _config.MethodPrefixToStrip);
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
        if (TryGetRemappedValue(recordDecl, _config._withGuids, out var guid))
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
        var uuidTextParts = uuidAttrText.Split(s_doubleQuoteSeparator, StringSplitOptions.RemoveEmptyEntries);

        if (uuidTextParts.Length < 2)
        {
            AddDiagnostic(DiagnosticLevel.Warning, $"Failed to parse uuid attr text '{uuidAttrText}'.", recordDecl);
            uuid = Guid.Empty;
            return false;
        }

        var uuidText = uuidTextParts[1];

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
            var pointerIndirectionLevel = 0;
            while (returnType is PointerType pointerType)
            {
                pointerIndirectionLevel++;
                returnType = pointerType.PointeeType;
            }
            var returnTypeName = GetRemappedTypeName(cursor: null, context: null, returnType, out _, skipUsing: true);

            name = pointerIndirectionLevel switch {
                0 => $"To{returnTypeName}",
                1 => $"To{returnTypeName}Pointer",
                2 => $"To{returnTypeName}PointerPointer",
                _ => $"To{returnTypeName}{string.Concat(Enumerable.Repeat("Pointer", pointerIndirectionLevel))}"
            };
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

            // Cast to output type if out of range of the corresponding 32-bit integer type
            if (IsPrevContextDecl<VarDecl>(out _, out _) && !IsStmtAsWritten<CastExpr>(stmt, out _, removeParens: true)
                && ((targetTypeName is "nuint" or "UIntPtr" && stmt.Handle.Evaluate.AsUnsigned > uint.MaxValue)
                    || (targetTypeName is "nint" or "IntPtr" && stmt.Handle.Evaluate.AsLongLong is < int.MinValue or > int.MaxValue)))
            {
                needsCast = true;
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

    private static IEnumerable<Attr> GetAttributesFor(NamedDecl namedDecl)
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

        return declAttrs;
    }

    private void WithAttributes(NamedDecl namedDecl, bool onlySupportedOSPlatform = false, bool isTestOutput = false)
    {
        var outputBuilder = isTestOutput ? _testOutputBuilder : _outputBuilder;
        Debug.Assert(outputBuilder is not null);

        if (TryGetRemappedValue(namedDecl, _config._withAttributes, out var attributes, matchStar: true))
        {
            foreach (var attribute in attributes.Where((a) => !onlySupportedOSPlatform || a.StartsWith("SupportedOSPlatform(", StringComparison.Ordinal)))
            {
                outputBuilder.WriteCustomAttribute(attribute);
            }
        }

        if (!isTestOutput)
        {
            var obsoleteEmitted = false;

            foreach (var attr in GetAttributesFor(namedDecl))
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
                        var message = GetDeprecatedMessage(attrText);

                        if (!string.IsNullOrEmpty(message))
                        {
                            outputBuilder.WriteCustomAttribute($"Obsolete(\"{message}\")");
                        }
                        else
                        {
                            outputBuilder.WriteCustomAttribute($"Obsolete");
                        }
                        obsoleteEmitted = true;
                        break;
                    }

                    case CX_AttrKind_Annotate:
                    {
                        var annotationText = EscapeString(attr.Spelling);
                        outputBuilder.WriteCustomAttribute($"""NativeAnnotation("{annotationText}")""");
                        break;
                    }

                    case CX_AttrKind_Format:
                    case CX_AttrKind_FormatArg:
                    case CX_AttrKind_MSNoVTable:
                    case CX_AttrKind_MSAllocator:
                    case CX_AttrKind_MaxFieldAlignment:
                    case CX_AttrKind_NoInline:
                    case CX_AttrKind_NoThrow:
                    case CX_AttrKind_SelectAny:
                    case CX_AttrKind_TypeVisibility:
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

    private static string? GetDeprecatedMessage(string attrText)
    {
        // The attribute source looks like `deprecated("message")`, but C allows the message to be
        // written as adjacent string literals (e.g. `deprecated("part1" "part2")`), which the
        // compiler concatenates into a single value. Gather the contents of every literal and join
        // them so the emitted `Obsolete("...")` is a single, valid C# string.

        var builder = new StringBuilder();
        var hasLiteral = false;
        var index = 0;

        while (index < attrText.Length)
        {
            if (attrText[index] != '"')
            {
                index++;
                continue;
            }

            hasLiteral = true;
            index++;

            while ((index < attrText.Length) && (attrText[index] != '"'))
            {
                // Preserve escape sequences verbatim so an escaped quote isn't treated as the end
                // of the literal and the contents are emitted unchanged (as before).
                if ((attrText[index] == '\\') && ((index + 1) < attrText.Length))
                {
                    _ = builder.Append(attrText[index]);
                    index++;
                }

                _ = builder.Append(attrText[index]);
                index++;
            }

            index++;
        }

        return hasLiteral ? builder.ToString() : null;
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

    private bool TryGetClass(ReadOnlySpan<char> remappedName, [MaybeNullWhen(false)] out string className, bool disallowPrefixMatch = false)
    {
        var index = remappedName.IndexOf('*');

        if (index != -1)
        {
            remappedName = remappedName[..index];
        }

        if (_config._withClasses.GetAlternateLookup<ReadOnlySpan<char>>().TryGetValue(remappedName, out className))
        {
            _ = _topLevelClassNames.Add(className);
            _ = _topLevelClassNames.Add($"{className}Tests");
            return true;
        }

        if (disallowPrefixMatch)
        {
            return false;
        }

        foreach (var withClass in _config._withClasses)
        {
            if (!withClass.Key.EndsWith('*'))
            {
                continue;
            }

            var prefix = withClass.Key.AsSpan()[..^1];

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

    private string GetNamespace(ReadOnlySpan<char> remappedName, NamedDecl? namedDecl = null)
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

    private bool TryGetNamespace(ReadOnlySpan<char> remappedName, [MaybeNullWhen(false)] out string namespaceName)
    {
        var index = remappedName.IndexOf('*');

        if (index != -1)
        {
            remappedName = remappedName[..index];
        }

        return _config._withNamespaces.GetAlternateLookup<ReadOnlySpan<char>>().TryGetValue(remappedName, out namespaceName);
    }

    private bool GetSetLastError(NamedDecl namedDecl) => HasRemapping(namedDecl, _config._withSetLastErrors, matchStar: true);

    private bool HasRemapping(NamedDecl namedDecl, HashSet<string> entries, bool matchStar = false)
    {
        var name = GetCursorQualifiedName(namedDecl).AsSpan();

        if (name.StartsWith("ClangSharpMacro_", StringComparison.Ordinal))
        {
            name = name["ClangSharpMacro_".Length..];
        }

        var entriesLookup = entries.GetAlternateLookup<ReadOnlySpan<char>>();

        if (entriesLookup.Contains(name))
        {
            return true;
        }

        name = GetCursorQualifiedName(namedDecl, truncateParameters: true);

        if (name.StartsWith("ClangSharpMacro_", StringComparison.Ordinal))
        {
            name = name["ClangSharpMacro_".Length..];
        }

        if (entriesLookup.Contains(name))
        {
            return true;
        }

        name = GetRemappedCursorName(namedDecl);

        if (name.StartsWith("ClangSharpMacro_", StringComparison.Ordinal))
        {
            name = name["ClangSharpMacro_".Length..];
        }

        return entriesLookup.Contains(name) || (matchStar && entriesLookup.Contains("*"));
    }

    private bool TryGetRemappedValue<T>(NamedDecl namedDecl, Dictionary<string, T> remappings, [MaybeNullWhen(false)] out T value, bool matchStar = false)
    {
        var name = GetCursorQualifiedName(namedDecl).AsSpan();

        if (name.StartsWith("ClangSharpMacro_", StringComparison.Ordinal))
        {
            name = name["ClangSharpMacro_".Length..];
        }

        var remappingsLookup = remappings.GetAlternateLookup<ReadOnlySpan<char>>();

        if (remappingsLookup.TryGetValue(name, out value))
        {
            return true;
        }

        name = GetCursorQualifiedName(namedDecl, truncateParameters: true);

        if (name.StartsWith("ClangSharpMacro_", StringComparison.Ordinal))
        {
            name = name["ClangSharpMacro_".Length..];
        }

        if (remappingsLookup.TryGetValue(name, out value))
        {
            return true;
        }

        name = GetRemappedCursorName(namedDecl);

        if (name.StartsWith("ClangSharpMacro_", StringComparison.Ordinal))
        {
            name = name["ClangSharpMacro_".Length..];
        }

        if (remappingsLookup.TryGetValue(name, out value))
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

    private void WithType(NamedDecl namedDecl, ref string integerTypeName, ref string nativeTypeName, bool matchStar = true)
    {
        if (TryGetRemappedValue(namedDecl, _config._withTypes, out var type, matchStar))
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

        if (TryGetRemappedValue(namedDecl, _config._withUsings, out var usings, matchStar: true))
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
