// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Collections.Generic;
using ClangSharp.Interop;

namespace ClangSharp.Abstractions
{
    internal struct ParameterDesc<TCustomAttrGeneratorData>
    {
        public string Type { get; set; }
        public string Name { get; set; }
        public string NativeTypeName { get; set; }
        public IEnumerable<string> CppAttributes { get; set; }
        public Action<TCustomAttrGeneratorData> WriteCustomAttrs { get; set; }
        public TCustomAttrGeneratorData CustomAttrGeneratorData { get; set; }
        public CXSourceLocation? Location { get; set; }
    }
}
