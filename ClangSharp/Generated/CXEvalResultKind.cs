namespace ClangSharp
{
    using System;
    using System.Runtime.InteropServices;

    public enum CXEvalResultKind
    {
        CXEval_Int = 1,
        CXEval_Float = 2,
        CXEval_ObjCStrLiteral = 3,
        CXEval_StrLiteral = 4,
        CXEval_CFStr = 5,
        CXEval_Other = 6,
        CXEval_UnExposed = 0,
    }
}
