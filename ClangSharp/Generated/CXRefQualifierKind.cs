namespace ClangSharp
{
    using System;
    using System.Runtime.InteropServices;

    public enum CXRefQualifierKind
    {
        CXRefQualifier_None = 0,
        CXRefQualifier_LValue = 1,
        CXRefQualifier_RValue = 2,
    }
}
