// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using ClangSharp.Interop;

namespace ClangSharp;

public partial class PInvokeGenerator
{
    private void WriteDocCommentXml(CXComment comment)
    {
        if (!_config.GenerateDocComments)
        {
            return;
        }
        _outputBuilder!.WriteDocComment(in comment);
    }
}
