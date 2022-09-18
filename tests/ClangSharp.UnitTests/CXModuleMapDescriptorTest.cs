// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using ClangSharp.Interop;
using NUnit.Framework;

namespace ClangSharp.UnitTests;

public class CXModuleMapDescriptorTest
{
    [Test]
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
        Assert.AreEqual(contents, buffer.AsString());
        buffer.ClangFree();
    }
}
