// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

namespace ClangSharp;

public partial class PInvokeGenerator
{
    private void VisitRef(Ref @ref)
    {
        if (_outputBuilder is null)
        {
            // A reference visited without an active output builder (e.g. a stray top-level
            // Objective-C class/protocol ref hanging off the translation unit) has nowhere to be
            // written; there is nothing meaningful to emit for it.
            return;
        }

        var name = GetRemappedCursorName(@ref.Referenced);
        StartCSharpCode().Write(name);
        StopCSharpCode();
    }
}
