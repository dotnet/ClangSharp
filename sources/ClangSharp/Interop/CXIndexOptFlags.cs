// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

namespace ClangSharp.Interop
{
    public enum CXIndexOptFlags
    {
        CXIndexOpt_None = 0x0,
        CXIndexOpt_SuppressRedundantRefs = 0x1,
        CXIndexOpt_IndexFunctionLocalSymbols = 0x2,
        CXIndexOpt_IndexImplicitTemplateInstantiations = 0x4,
        CXIndexOpt_SuppressWarnings = 0x8,
        CXIndexOpt_SkipParsedBodiesInSession = 0x10,
    }
}
