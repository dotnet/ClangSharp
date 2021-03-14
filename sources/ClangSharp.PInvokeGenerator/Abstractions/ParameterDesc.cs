using System;
using System.Collections.Generic;

namespace ClangSharp.Abstractions
{
    internal struct ParameterDesc<TCustomAttrGeneratorData>
    {
        public string Type;
        public string Name;
        public string NativeTypeName;
        public IEnumerable<string> CppAttributes;
        public Action<TCustomAttrGeneratorData> WriteCustomAttrs;
        public TCustomAttrGeneratorData CustomAttrGeneratorData;
    }
}
