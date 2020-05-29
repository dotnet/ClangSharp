// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;

namespace ClangSharp
{
    [Flags]
    public enum PInvokeGeneratorConfigurationOptions
    {
        None = 0x00000000,

        GenerateMultipleFiles = 0x00000001,

        GenerateUnixTypes = 0x00000002,

        NoDefaultRemappings = 0x00000004,

        GenerateCompatibleCode = 0x00000008,

        GeneratePreviewCodeNint = 0x00000010,

        GeneratePreviewCodeFnptr = 0x00000020,

        GeneratePreviewCode = GeneratePreviewCodeNint | GeneratePreviewCodeFnptr,

        LogExclusions = 0x00000040,
    }
}
