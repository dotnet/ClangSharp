// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class NonTypeTemplateParmDecl : DeclaratorDecl, ITemplateParmPosition
    {
        internal NonTypeTemplateParmDecl(CXCursor handle) : base(handle, CXCursorKind.CXCursor_NonTypeTemplateParameter, CX_DeclKind.CX_DeclKind_NonTypeTemplateParm)
        {
        }
    }
}
