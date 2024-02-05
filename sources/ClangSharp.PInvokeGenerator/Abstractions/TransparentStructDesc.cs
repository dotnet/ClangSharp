// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

namespace ClangSharp.Abstractions;

internal struct TransparentStructDesc
{
    public string ParentName { get; set; }
    public string Name { get; set; }
    public string? NativeName { get; set; }
    public string Type { get; set; }
    public string? NativeType { get; set; }
    public PInvokeGeneratorTransparentStructKind Kind { get; set; }
}
