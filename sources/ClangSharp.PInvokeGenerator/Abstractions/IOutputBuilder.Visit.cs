// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;

namespace ClangSharp.Abstractions
{
    internal partial interface IOutputBuilder
    {
        void WriteDivider(bool force = false);
        void SuppressDivider();

        void WriteCustomAttribute(string attribute, Action callback = null);
        void WriteIid(string name, Guid value);
        void EmitUsingDirective(string directive);
    }
}
