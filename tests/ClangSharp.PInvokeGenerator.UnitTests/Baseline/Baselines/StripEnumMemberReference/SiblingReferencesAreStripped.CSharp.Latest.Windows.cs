namespace ClangSharp.Test
{
    public enum WGPUInstanceBackend
    {
        Vulkan = 1 << 0,
        GL = 1 << 1,
        Metal = 1 << 2,
        Primary = Vulkan | Metal,
        Secondary = GL,
    }
}
