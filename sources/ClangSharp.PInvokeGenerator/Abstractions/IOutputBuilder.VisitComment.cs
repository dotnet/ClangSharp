// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using ClangSharp.Interop;

namespace ClangSharp.Abstractions;

internal partial interface IOutputBuilder
{
    void WriteDocComment(in CXComment comment);
}
