// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

namespace ClangSharp.Interop
{
    public enum CXNameRefFlags
    {
        CXNameRange_WantQualifier = 0x1,
        CXNameRange_WantTemplateArgs = 0x2,
        CXNameRange_WantSinglePiece = 0x4,
    }
}
