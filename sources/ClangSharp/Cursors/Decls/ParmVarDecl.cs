// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class ParmVarDecl : VarDecl
    {
        internal ParmVarDecl(CXCursor handle) : base(handle, CXCursorKind.CXCursor_ParmDecl, CX_DeclKind.CX_DeclKind_ParmVar)
        {
        }

        public Expr DefaultArg
        {
            get
            {
                Expr defaultArg = Init;

                if (defaultArg is FullExpr fullExpr)
                {
                    defaultArg = fullExpr.SubExpr;
                }

                return defaultArg;
            }
        }

        public bool HasDefaultArg => DefaultArg != null;
    }
}
