// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

namespace ClangSharp.Interop
{
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
