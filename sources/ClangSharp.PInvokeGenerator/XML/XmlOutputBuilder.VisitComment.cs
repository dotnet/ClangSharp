// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using ClangSharp.Interop;

namespace ClangSharp.XML;

internal partial class XmlOutputBuilder
{
    public void WriteDocComment(in CXComment comment)
    {
        // Not implemented for XML
    }
}
