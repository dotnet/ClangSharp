// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class MacroDefinitionRecord : PreprocessingDirective
    {
        internal MacroDefinitionRecord(CXCursor handle) : base(handle, CXCursorKind.CXCursor_MacroDefinition)
        {
        }

        public bool IsFunctionLike => Handle.IsMacroFunctionLike;

        public bool IsBuiltinMacro => Handle.IsMacroBuiltIn;

        public string Name => Handle.Spelling.CString;
    }
}
