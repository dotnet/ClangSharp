// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

namespace ClangSharp
{
    public partial class PInvokeGenerator
    {
        private void VisitRef(Ref @ref)
        {
            var name = GetRemappedCursorName(@ref.Referenced);
            StartCSharpCode().Write(name);
            StopCSharpCode();
        }
    }
}
