using System;

namespace ClangSharp.Abstractions
{
    public struct ParameterDesc<TCustomAttrGeneratorData>
    {
        public string Type;
        public string Name;
        public Action<TCustomAttrGeneratorData> WriteCustomAttrs;
        public TCustomAttrGeneratorData CustomAttrGeneratorData;
    }
}
