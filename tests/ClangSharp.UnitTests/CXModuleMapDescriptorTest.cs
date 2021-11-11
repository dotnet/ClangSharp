// Copyright (c) .NET Foundation and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using ClangSharp.Interop;
using Xunit;

namespace ClangSharp.UnitTests
{
    public class CXModuleMapDescriptorTest
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

            using var mmd = CXModuleMapDescriptor.Create(options: 0);
            _ = mmd.SetFrameworkModuleName("TestFrame");
            _ = mmd.SetUmbrellaHeader("TestFrame.h");

            var buffer = mmd.WriteToBuffer(options: 0, errorCode: out _);
            Assert.Equal(contents, buffer.AsString());
            buffer.ClangFree();
        }
    }
}
