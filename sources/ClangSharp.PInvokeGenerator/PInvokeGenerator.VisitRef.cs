// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

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
