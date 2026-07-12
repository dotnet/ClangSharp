// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;

namespace ClangSharp;

public sealed class MacroExpansion : PreprocessedEntity
{
    private ValueLazy<MacroExpansion, MacroDefinitionRecord> _definition;

    internal unsafe MacroExpansion(CXCursor handle) : base(handle, CXCursor_MacroExpansion)
    {
        _definition = new ValueLazy<MacroExpansion, MacroDefinitionRecord>(&DefinitionFactory);
    }

    public MacroDefinitionRecord Definition => _definition.GetValue(this);

    public bool IsBuiltinMacro => Handle.IsMacroBuiltIn;

    public string Name => Handle.Spelling.CString;

    private static unsafe MacroDefinitionRecord DefinitionFactory(MacroExpansion self) => self.TranslationUnit.GetOrCreate<MacroDefinitionRecord>(self.Handle.Referenced);
}
