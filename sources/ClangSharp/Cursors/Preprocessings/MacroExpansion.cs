// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class MacroExpansion : PreprocessedEntity
    {
        private readonly Lazy<MacroDefinitionRecord> _definition;

        internal MacroExpansion(CXCursor handle) : base(handle, CXCursorKind.CXCursor_MacroExpansion)
        {
            _definition = new Lazy<MacroDefinitionRecord>(() => TranslationUnit.GetOrCreate<MacroDefinitionRecord>(handle.Referenced));
        }

        public MacroDefinitionRecord Definition => _definition.Value;

        public bool IsBuiltinMacro => Handle.IsMacroBuiltIn;

        public string Name => Handle.Spelling.CString;
    }
}
