// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

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
