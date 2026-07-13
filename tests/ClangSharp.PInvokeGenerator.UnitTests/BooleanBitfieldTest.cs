// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Threading.Tasks;
using NUnit.Framework;

namespace ClangSharp.UnitTests;

public sealed class BooleanBitfieldTest : PInvokeGeneratorTest
{
    // Regression test for https://github.com/dotnet/clangsharp/issues/508
    // A `bool` bitfield previously generated invalid C# (a `bool` backing field plus
    // `bool & int` arithmetic and `(bool)` casts). It should use an integer backing store
    // with the public accessor kept as `bool`, converting at the get/set boundary. An
    // adjacent non-bool bitfield must be unaffected.
    private const string InputContents = @"struct MyStruct
{
    bool a : 1;
    bool b : 1;
    int c : 2;
};
";

    [Test]
    public Task LatestTest()
    {
        var expectedOutputContents = @"namespace ClangSharp.Test
{
    public partial struct MyStruct
    {
        public byte _bitfield1;

        [NativeTypeName(""bool : 1"")]
        public bool a
        {
            readonly get
            {
                return (_bitfield1 & 0x1u) != 0;
            }

            set
            {
                _bitfield1 = (byte)((_bitfield1 & ~0x1u) | ((value ? 1 : 0) & 0x1u));
            }
        }

        [NativeTypeName(""bool : 1"")]
        public bool b
        {
            readonly get
            {
                return ((_bitfield1 >> 1) & 0x1u) != 0;
            }

            set
            {
                _bitfield1 = (byte)((_bitfield1 & ~(0x1u << 1)) | (((value ? 1 : 0) & 0x1u) << 1));
            }
        }

        public int _bitfield2;

        [NativeTypeName(""int : 2"")]
        public int c
        {
            readonly get
            {
                return (_bitfield2 << 30) >> 30;
            }

            set
            {
                _bitfield2 = (_bitfield2 & ~0x3) | (value & 0x3);
            }
        }
    }
}
";

        return ValidateGeneratedCSharpLatestWindowsBindingsAsync(InputContents, expectedOutputContents);
    }

    [Test]
    public Task CompatibleTest()
    {
        var expectedOutputContents = @"namespace ClangSharp.Test
{
    public partial struct MyStruct
    {
        public byte _bitfield1;

        [NativeTypeName(""bool : 1"")]
        public bool a
        {
            get
            {
                return (_bitfield1 & 0x1u) != 0;
            }

            set
            {
                _bitfield1 = (byte)((_bitfield1 & ~0x1u) | ((value ? 1 : 0) & 0x1u));
            }
        }

        [NativeTypeName(""bool : 1"")]
        public bool b
        {
            get
            {
                return ((_bitfield1 >> 1) & 0x1u) != 0;
            }

            set
            {
                _bitfield1 = (byte)((_bitfield1 & ~(0x1u << 1)) | (((value ? 1 : 0) & 0x1u) << 1));
            }
        }

        public int _bitfield2;

        [NativeTypeName(""int : 2"")]
        public int c
        {
            get
            {
                return (_bitfield2 << 30) >> 30;
            }

            set
            {
                _bitfield2 = (_bitfield2 & ~0x3) | (value & 0x3);
            }
        }
    }
}
";

        return ValidateGeneratedCSharpCompatibleWindowsBindingsAsync(InputContents, expectedOutputContents);
    }
}
