// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

namespace ClangSharp.Abstractions
{
    internal partial interface IOutputBuilder
    {
        void WriteDivider(bool force = false);
        void SuppressDivider();

        void WriteCustomAttribute(string attribute);
        void WriteIid(string iidName, string iidValue);
        void EmitUsingDirective(string directive);
    }
}
