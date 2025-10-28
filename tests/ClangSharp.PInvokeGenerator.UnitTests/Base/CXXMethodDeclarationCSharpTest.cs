// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Threading.Tasks;

namespace ClangSharp.UnitTests;

public abstract class CXXMethodDeclarationCSharpTest : CXXMethodDeclarationTest
{
    protected override Task ConversionTestImpl()
    {
        var inputContents = @"struct MyStruct
{
    int value;
    int* pointer;
    int** pointer2;

    operator int()
    {
        return value;
    }

    operator int*()
    {
        return pointer;
    }

    operator int**()
    {
        return pointer2;
    }
};
";

        var expectedOutputContents = @"namespace ClangSharp.Test
{
    public unsafe partial struct MyStruct
    {
        public int value;

        public int* pointer;

        public int** pointer2;

        public int ToInt32()
        {
            return value;
        }

        public int* ToInt32Pointer()
        {
            return pointer;
        }

        public int** ToInt32Pointer2()
        {
            return pointer2;
        }
    }
}
";

        return ValidateBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task MacrosExpansionTestImpl()
    {
        var inputContents = @"typedef struct
{
    unsigned char *buf;
    int size;
} context_t;

int buf_close(void *pContext)
{
    ((context_t*)pContext)->buf=0;
    return 0;
}
";

        var expectedOutputContents = @"namespace ClangSharp.Test
{
    public unsafe partial struct context_t
    {
        [NativeTypeName(""unsigned char *"")]
        public byte* buf;

        public int size;
    }

    public static unsafe partial class Methods
    {
        public static int buf_close(void* pContext)
        {
            ((context_t*)(pContext))->buf = null;
            return 0;
        }
    }
}
";

        return ValidateBindingsAsync(inputContents, expectedOutputContents);
    }
}
