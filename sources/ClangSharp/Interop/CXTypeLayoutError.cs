// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

namespace ClangSharp.Interop
{
    public enum CXTypeLayoutError
    {
        CXTypeLayoutError_Invalid = -1,
        CXTypeLayoutError_Incomplete = -2,
        CXTypeLayoutError_Dependent = -3,
        CXTypeLayoutError_NotConstantSize = -4,
        CXTypeLayoutError_InvalidFieldName = -5,
        CXTypeLayoutError_Undeduced = -6,
    }
}
