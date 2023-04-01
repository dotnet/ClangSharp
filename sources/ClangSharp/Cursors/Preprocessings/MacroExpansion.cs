// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;

namespace ClangSharp;

public sealed class MacroExpansion : PreprocessedEntity
{
    private readonly Lazy<MacroDefinitionRecord> _definition;

    internal MacroExpansion(CXCursor handle) : base(handle, CXCursor_MacroExpansion)
    {
        _definition = new Lazy<MacroDefinitionRecord>(() => TranslationUnit.GetOrCreate<MacroDefinitionRecord>(handle.Referenced));
    }

    public MacroDefinitionRecord Definition => _definition.Value;

    public bool IsBuiltinMacro => Handle.IsMacroBuiltIn;

    public string Name => Handle.Spelling.CString;
}
