namespace ClangSharp.Test
{
    public enum abc_backend
    {
        vulkan = 1 << 0,
        gl = 1 << 1,
        metal = 1 << 2,
        primary = vulkan | metal,
        secondary = gl,
    }
}
