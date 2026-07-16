// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.
// Ported from https://github.com/dotnet/clangsharp/blob/main/sources/libClangSharp

namespace ClangSharp.Interop;

public enum CX_PredefinedIdentKind
{
    CX_PIK_Invalid,
    CX_PIK_Func = 1,
    CX_PIK_Function = 2,
    CX_PIK_LFunction = 3,
    CX_PIK_FuncDName = 4,
    CX_PIK_FuncSig = 5,
    CX_PIK_LFuncSig = 6,
    CX_PIK_PrettyFunction = 7,
    CX_PIK_PrettyFunctionNoVirtual = 8,
}
