using System;
using System.Runtime.InteropServices;
using Xunit;

namespace ClangSharp.Test
{
    public class ModuleMapDescriptor
    {
        [Fact]
        public void Basic()
        {
            var contents =
                "framework module TestFrame {\n"
                + "  umbrella header \"TestFrame.h\"\n"
                + "\n"
                + "  export *\n"
                + "  module * { export * }\n"
                + "}\n";

            CXModuleMapDescriptor mmd = clang.ModuleMapDescriptor_create(0);

            clang.ModuleMapDescriptor_setFrameworkModuleName(mmd, "TestFrame");
            clang.ModuleMapDescriptor_setUmbrellaHeader(mmd, "TestFrame.h");

            IntPtr bufPtr;
            uint bufSize = 0;
            clang.ModuleMapDescriptor_writeToBuffer(mmd, 0, out bufPtr, out bufSize);
            var bufStr = Marshal.PtrToStringAnsi(bufPtr, (int)bufSize);
            Assert.Equal(contents, bufStr);
            clang.free(bufPtr);
            clang.ModuleMapDescriptor_dispose(mmd);
        }
    }
}
