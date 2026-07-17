// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Collections.Generic;
using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;

namespace ClangSharp;

public sealed class MacroDefinitionRecord : PreprocessingDirective
{
    private readonly LazyList<string> _parameterNames;
    private readonly LazyList<string> _tokens;

    internal unsafe MacroDefinitionRecord(CXCursor handle) : base(handle, CXCursor_MacroDefinition)
    {
        _parameterNames = LazyList.Create<string>(this, Math.Max(0, Handle.NumMacroParams), &ParameterNamesFactory);
        _tokens = LazyList.Create<string>(this, Math.Max(0, Handle.NumMacroTokens), &TokensFactory);
    }

    public bool IsBuiltinMacro => Handle.IsMacroBuiltIn;

    public bool IsC99Varargs => Handle.IsMacroC99Varargs;

    public bool IsFunctionLike => Handle.IsMacroFunctionLike;

    public bool IsGNUVarargs => Handle.IsMacroGNUVarargs;

    public bool IsVariadic => Handle.IsMacroVariadic;

    public string Name => Handle.Spelling.CString;

    public int NumParameters => Handle.NumMacroParams;

    public int NumTokens => Handle.NumMacroTokens;

    public IReadOnlyList<string> ParameterNames => _parameterNames;

    public IReadOnlyList<string> Tokens => _tokens;

    private static string ParameterNamesFactory(object self, int i) => ((MacroDefinitionRecord)self).Handle.GetMacroParamName(unchecked((uint)i)).CString;

    private static string TokensFactory(object self, int i) => ((MacroDefinitionRecord)self).Handle.GetMacroTokenSpelling(unchecked((uint)i)).CString;
}
