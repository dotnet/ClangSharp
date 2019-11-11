// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

namespace ClangSharp.Interop
{
    public enum CXObjCDeclQualifierKind
    {
        CXObjCDeclQualifier_None = 0x0,
        CXObjCDeclQualifier_In = 0x1,
        CXObjCDeclQualifier_Inout = 0x2,
        CXObjCDeclQualifier_Out = 0x4,
        CXObjCDeclQualifier_Bycopy = 0x8,
        CXObjCDeclQualifier_Byref = 0x10,
        CXObjCDeclQualifier_Oneway = 0x20,
    }
}
