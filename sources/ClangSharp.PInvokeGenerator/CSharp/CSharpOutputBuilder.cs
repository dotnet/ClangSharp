// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using ClangSharp.Abstractions;

namespace ClangSharp.CSharp;

internal sealed partial class CSharpOutputBuilder(string name, PInvokeGenerator generator, string indentationString = CSharpOutputBuilder.DefaultIndentationString,
                                                  bool isTestOutput = false, MarkerMode markerMode = MarkerMode.None,
                                                  bool writeSourceLocation = false)
{
    public const string DefaultIndentationString = "    ";

    private readonly string _name = name;
    private readonly PInvokeGenerator _generator = generator;
    private readonly List<string> _contents = [];
    private readonly StringBuilder _currentLine = new StringBuilder();
    private readonly SortedSet<string> _usingDirectives = new SortedSet<string>(StringComparer.Ordinal);
    private readonly SortedSet<string> _staticUsingDirectives = new SortedSet<string>(StringComparer.Ordinal);
    private readonly string _indentationString = indentationString;
    private readonly bool _isTestOutput = isTestOutput;

    private int _indentationLevel;
    private bool _isInMarkerInterface;
    private readonly MarkerMode _markerMode = markerMode;
    private readonly bool _writeSourceLocation = writeSourceLocation;

    public IEnumerable<string> Contents => _contents;

    public bool HasPendingLine => _currentLine.Length > 0;

    public string IndentationString => _indentationString;

    public bool IsUncheckedContext { get; private set; }

    public bool IsTestOutput => _isTestOutput;

    public string Name => _name;

    public string Extension { get; } = ".cs";

    public bool NeedsNewline { get; set; }

    public bool NeedsSemicolon { get; set; }

    public IEnumerable<string> StaticUsingDirectives => _staticUsingDirectives;

    public IEnumerable<string> UsingDirectives => _usingDirectives;

    public void AddUsingDirective(string namespaceName) => _ = namespaceName.StartsWith("static ", StringComparison.Ordinal) ? _staticUsingDirectives.Add(namespaceName) : _usingDirectives.Add(namespaceName);

    public void DecreaseIndentation()
    {
        Debug.Assert(_indentationLevel > 0);
        _indentationLevel--;
    }

    public void IncreaseIndentation() => _indentationLevel++;

    public void WriteBlockStart()
    {
        WriteIndentedLine('{');
        IncreaseIndentation();
    }

    public void WriteBlockEnd()
    {
        // We don't need a newline if immediately closing the scope
        NeedsNewline = false;

        // We don't need a semicolon if immediately closing the scope
        NeedsSemicolon = false;

        DecreaseIndentation();
        WriteIndentedLine('}');
    }

    public void Write<T>(T value) => _ = _currentLine.Append(value);

    public void Write(ReadOnlySpan<char> value) => _ = _currentLine.Append(value);

    public void WriteIndentation()
    {
        WriteNewlineIfNeeded();

        for (var i = 0; i < _indentationLevel; i++)
        {
            _ = _currentLine.Append(_indentationString);
        }
    }

    public void WriteIndented<T>(T value)
    {
        WriteIndentation();
        Write(value);
    }

    public void WriteIndentedLine<T>(T value)
    {
        WriteIndentation();
        WriteLine(value);
    }

    public void WriteLabel(string name)
    {
        if (_currentLine.Length >= _indentationString.Length)
        {
            var match = true;

            for (var i = 0; i < _indentationString.Length; i++)
            {
                if (_currentLine[_currentLine.Length - i - 1] != _indentationString[_indentationString.Length - 1 - i])
                {
                    match = false;
                    break;
                }
            }

            if (match)
            {
                _ = _currentLine.Remove(_currentLine.Length - _indentationString.Length, _indentationString.Length);
            }
        }

        Write(name);
        WriteLine(':');
    }

    public void WriteNumberLiteral(string value) => Write(value.Replace('\'', '_'));

    public void WriteLine<T>(T value)
    {
        Write(value);
        WriteNewline();
    }

    public void WriteNewline()
    {
        _contents.Add(_currentLine.ToString());
        _ = _currentLine.Clear();
        NeedsNewline = false;
    }

    public void WriteNewlineIfNeeded()
    {
        if (NeedsNewline)
        {
            WriteNewline();
        }
    }

    public void WritePendingLine()
    {
        if (HasPendingLine)
        {
            WriteNewline();
        }
    }

    public void WriteSemicolon()
    {
        Write(';');
        NeedsSemicolon = false;
        NeedsNewline = true;
    }

    public void WriteSemicolonIfNeeded()
    {
        if (NeedsSemicolon)
        {
            WriteSemicolon();
        }
    }

    public void WriteValueAsBytes(ulong value, int sizeInChars)
    {
        Write("0x");
        Write(((byte)value).ToString("X2", CultureInfo.InvariantCulture));

        for (var i = 1; i < sizeInChars; i++)
        {
            Write(", ");
            value >>= 8;

            Write("0x");
            Write(((byte)value).ToString("X2", CultureInfo.InvariantCulture));
        }
    }

    public void BeginMarker(string kind, params KeyValuePair<string, object>[] attributes)
    {
        if (_markerMode != MarkerMode.Xml)
        {
            return;
        }

        Write("/*M*/<");
        Write(kind);
        foreach (var kvp in attributes)
        {
            Write(' ');
            Write(kvp.Key);
            Write('=');
            Write('"');
            Write(kvp.Value);
            Write('"');
        }

        Write("/*M*/>");
    }

    public void EndMarker(string kind)
    {
        if (_markerMode != MarkerMode.Xml)
        {
            return;
        }

        Write("/*M*/</");
        Write(kind);
        Write("/*M*/>");
    }

    private void AddCppAttributes(IEnumerable<string> attrs, string? prefix = null, string? postfix = null)
    {
        var attributeList = string.Join("^", attrs);
        if (string.IsNullOrWhiteSpace(attributeList))
        {
            return;
        }

        if (prefix is null)
        {
            WriteIndentation();
        }
        else
        {
            WriteNewlineIfNeeded();
            Write(prefix);
        }

        Write("[CppAttributeList(\"");
        Write(attributeList);
        Write("\")]");

        if (postfix is null)
        {
            NeedsNewline = true;
        }
        else
        {
            Write(postfix);
        }
    }

    private void AddNativeInheritanceAttribute(string inheritedFromName, string? prefix = null, string? postfix = null, string? attributePrefix = null)
    {
        if (prefix is null)
        {
            WriteIndentation();
        }
        else
        {
            WriteNewlineIfNeeded();
            Write(prefix);
        }

        Write('[');

        if (attributePrefix != null)
        {
            Write(attributePrefix);
        }

        Write("NativeInheritance");
        Write('(');
        Write('"');
        Write(PInvokeGenerator.EscapeString(inheritedFromName));
        Write('"');
        Write(')');
        Write(']');

        if (postfix is null)
        {
            NeedsNewline = true;
        }
        else
        {
            Write(postfix);
        }
    }

    private void AddVtblIndexAttribute(long vtblIndex, string? prefix = null, string? postfix = null, string? attributePrefix = null)
    {
        if (prefix is null)
        {
            WriteIndentation();
        }
        else
        {
            WriteNewlineIfNeeded();
            Write(prefix);
        }

        Write('[');

        if (attributePrefix != null)
        {
            Write(attributePrefix);
        }

        Write("VtblIndex");
        Write('(');
        Write(vtblIndex);
        Write(')');
        Write(']');

        if (postfix is null)
        {
            NeedsNewline = true;
        }
        else
        {
            Write(postfix);
        }
    }

    private void AddNativeTypeNameAttribute(string nativeTypeName, string? prefix = null, string? postfix = null, string? attributePrefix = null)
    {
        foreach (var entry in _generator.Config.NativeTypeNamesToStrip)
        {
            nativeTypeName = nativeTypeName.Replace(entry, "", StringComparison.Ordinal);
        }

        if (string.IsNullOrWhiteSpace(nativeTypeName))
        {
            return;
        }

        if (prefix is null)
        {
            WriteIndentation();
        }
        else
        {
            WriteNewlineIfNeeded();
            Write(prefix);
        }

        Write('[');

        if (attributePrefix != null)
        {
            Write(attributePrefix);
        }

        Write("NativeTypeName(\"");
        Write(PInvokeGenerator.EscapeString(nativeTypeName));
        Write("\")]");

        if (postfix is null)
        {
            NeedsNewline = true;
        }
        else
        {
            Write(postfix);
        }
    }

    public override string ToString()
    {
        var result = new StringBuilder();

        foreach (var line in _contents)
        {
            _ = result.Append(line);
            _ = result.Append('\n');
        }

        _ = result.Append(_currentLine);
        return result.ToString();
    }
}
