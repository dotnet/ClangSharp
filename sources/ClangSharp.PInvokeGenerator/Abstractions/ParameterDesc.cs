// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

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
