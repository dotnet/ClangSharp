namespace ClangSharp
{
    using System;
    using System.Runtime.InteropServices;

    public enum CXObjCDeclQualifierKind
    {
        CXObjCDeclQualifier_None = 0,
        CXObjCDeclQualifier_In = 1,
        CXObjCDeclQualifier_Inout = 2,
        CXObjCDeclQualifier_Out = 4,
        CXObjCDeclQualifier_Bycopy = 8,
        CXObjCDeclQualifier_Byref = 16,
        CXObjCDeclQualifier_Oneway = 32,
    }
}
