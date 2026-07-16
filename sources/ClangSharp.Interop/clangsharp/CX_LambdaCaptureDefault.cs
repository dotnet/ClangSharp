// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.
// Ported from https://github.com/dotnet/clangsharp/blob/main/sources/libClangSharp

namespace ClangSharp.Interop;

public enum CX_LambdaCaptureDefault
{
    CX_LCD_Invalid,
    CX_LCD_None = 1,
    CX_LCD_ByCopy = 2,
    CX_LCD_ByRef = 3,
}
