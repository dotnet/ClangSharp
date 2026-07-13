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

public partial class PInvokeGenerator
{
    public void Close()
    {
        Stream? stream = null;
        Stream? testStream = null;

        var methodClassOutputBuilders = new Dictionary<string, IOutputBuilder>(StringComparer.Ordinal);
        var methodClassTestOutputBuilders = new Dictionary<string, IOutputBuilder>(StringComparer.Ordinal);
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
            GenerateNativeAnnotationAttribute(this, stream, leaveStreamOpen);
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

        static void GenerateNativeAnnotationAttribute(PInvokeGenerator generator, Stream? stream, bool leaveStreamOpen)
        {
            const string AttributeName = "NativeAnnotationAttribute";
            var config = generator.Config;

            var ns = generator.GetNamespace(AttributeName);
            if (config.ExcludedNames.Contains(AttributeName) || config.ExcludedNames.Contains($"{ns}.{AttributeName}"))
            {
                return;
            }

            if (stream is null)
            {
                var outputPath = Path.Combine(config.OutputLocation, $"{AttributeName}.cs");
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
            sw.Write(ns);

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
            sw.WriteLine("/// <summary>Defines the annotation found in a native declaration.</summary>");
            sw.Write(indentString);
            sw.WriteLine("[AttributeUsage(AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.ReturnValue, AllowMultiple = true, Inherited = false)]");
            sw.Write(indentString);
            sw.WriteLine("[Conditional(\"DEBUG\")]");
            sw.Write(indentString);
            sw.WriteLine($"internal sealed partial class {AttributeName} : Attribute");
            sw.Write(indentString);
            sw.WriteLine('{');
            sw.Write(indentString);
            sw.WriteLine("    private readonly string _annotation;");
            sw.WriteLine();
            sw.Write(indentString);
            sw.WriteLine($"    /// <summary>Initializes a new instance of the <see cref=\"{AttributeName}\" /> class.</summary>");
            sw.Write(indentString);
            sw.WriteLine("    /// <param name=\"annotation\">The annotation that was used in the native declaration.</param>");
            sw.Write(indentString);
            sw.WriteLine($"    public {AttributeName}(string annotation)");
            sw.Write(indentString);
            sw.WriteLine("    {");
            sw.Write(indentString);
            sw.WriteLine("        _annotation = annotation;");
            sw.Write(indentString);
            sw.WriteLine("    }");
            sw.WriteLine();
            sw.Write(indentString);
            sw.WriteLine("    /// <summary>Gets the annotation that was used in the native declaration.</summary>");
            sw.Write(indentString);
            sw.WriteLine("    public string Annotation => _annotation;");
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
                        sw.WriteLine(")(value ? 1u : 0u));");
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
                sw.Write("        if (obj is ");
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
                return type.Contains('*', StringComparison.Ordinal)
                     ? (8, 4, +1)
                     : type switch {
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
}
