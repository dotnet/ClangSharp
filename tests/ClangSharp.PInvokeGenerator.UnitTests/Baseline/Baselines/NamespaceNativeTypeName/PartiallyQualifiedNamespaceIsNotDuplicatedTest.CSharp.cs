namespace ClangSharp.Test
{
    public partial struct PropertyValue
    {
        public int value;
    }

    public partial struct EffectSource
    {
        public int source;
    }

    public unsafe partial struct Interop
    {
        [NativeTypeName("Abi::Windows::Foundation::PropertyValue **")]
        public PropertyValue** partiallyQualified;

        [NativeTypeName("Abi::Windows::Foundation::PropertyValue **")]
        public PropertyValue** fullyQualified;

        [NativeTypeName("Abi::Windows::Graphics::Effects::EffectSource **")]
        public EffectSource** sameNamespace;
    }
}
