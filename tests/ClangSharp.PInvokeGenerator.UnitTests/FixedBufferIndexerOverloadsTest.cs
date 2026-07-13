// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Threading.Tasks;
using NUnit.Framework;

namespace ClangSharp.UnitTests;

/// <summary>
/// Tests for https://github.com/dotnet/ClangSharp/issues/450.
/// When <c>generate-fixed-buffer-indexer-overloads</c> is set, the generated <c>_e__FixedBuffer</c>
/// helper struct should expose additional <c>uint</c>, <c>nint</c>, and <c>nuint</c> indexers alongside
/// the default <c>int</c> one so callers can index without casting. Without the switch, only the
/// <c>int</c> indexer is generated.
/// </summary>
[Platform("win")]
public sealed class FixedBufferIndexerOverloadsTest : PInvokeGeneratorTest
{
    [Test]
    public Task GeneratesOverloadsWhenEnabled()
    {
        var inputContents = @"struct MyStruct
{
    int value;
};

struct MyOtherStruct
{
    MyStruct c[3];
};
";

        var expectedOutputContents = @"namespace ClangSharp.Test
{
    public partial struct MyStruct
    {
        public int value;
    }

    public partial struct MyOtherStruct
    {
        [NativeTypeName(""MyStruct[3]"")]
        public _c_e__FixedBuffer c;

        public partial struct _c_e__FixedBuffer
        {
            public MyStruct e0;
            public MyStruct e1;
            public MyStruct e2;

            public unsafe ref MyStruct this[int index]
            {
                get
                {
                    fixed (MyStruct* pThis = &e0)
                    {
                        return ref pThis[index];
                    }
                }
            }

            public unsafe ref MyStruct this[uint index]
            {
                get
                {
                    fixed (MyStruct* pThis = &e0)
                    {
                        return ref pThis[index];
                    }
                }
            }

            public unsafe ref MyStruct this[nint index]
            {
                get
                {
                    fixed (MyStruct* pThis = &e0)
                    {
                        return ref pThis[index];
                    }
                }
            }

            public unsafe ref MyStruct this[nuint index]
            {
                get
                {
                    fixed (MyStruct* pThis = &e0)
                    {
                        return ref pThis[index];
                    }
                }
            }
        }
    }
}
";

        return ValidateGeneratedCSharpCompatibleWindowsBindingsAsync(inputContents, expectedOutputContents, additionalConfigOptions: PInvokeGeneratorConfigurationOptions.GenerateFixedBufferIndexerOverloads);
    }

    [Test]
    public Task GeneratesOnlyIntIndexerWhenDisabled()
    {
        var inputContents = @"struct MyStruct
{
    int value;
};

struct MyOtherStruct
{
    MyStruct c[3];
};
";

        var expectedOutputContents = @"namespace ClangSharp.Test
{
    public partial struct MyStruct
    {
        public int value;
    }

    public partial struct MyOtherStruct
    {
        [NativeTypeName(""MyStruct[3]"")]
        public _c_e__FixedBuffer c;

        public partial struct _c_e__FixedBuffer
        {
            public MyStruct e0;
            public MyStruct e1;
            public MyStruct e2;

            public unsafe ref MyStruct this[int index]
            {
                get
                {
                    fixed (MyStruct* pThis = &e0)
                    {
                        return ref pThis[index];
                    }
                }
            }
        }
    }
}
";

        return ValidateGeneratedCSharpCompatibleWindowsBindingsAsync(inputContents, expectedOutputContents);
    }
}
