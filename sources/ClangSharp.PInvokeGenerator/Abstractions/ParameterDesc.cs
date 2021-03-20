using System;
using System.Collections.Generic;

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
    }
}
