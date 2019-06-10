using System;
using System.Runtime.CompilerServices;
using Xunit;

namespace ClangSharp.UnitTests
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

            using (var mmd = CXModuleMapDescriptor.Create(options: 0))
            {
                mmd.SetFrameworkModuleName("TestFrame");
                mmd.SetUmbrellaHeader("TestFrame.h");

                Span<byte> buffer = mmd.WriteToBuffer(options: 0, errorCode: out _);
                Assert.Equal(contents, buffer.AsString());
                buffer.ClangFree();
            }
        }
    }
}
