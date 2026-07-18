// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using ClangSharp.UnitTests.Baseline;
using NUnit.Framework;

namespace ClangSharp.UnitTests;

public sealed class CTest : StandaloneBaselineTest
{
    protected override string Area => "C";

    [Test]
    public Task BasicTest()
    {
        var inputContents = @"typedef enum MyEnum {
    MyEnum_Value0,
    MyEnum_Value1,
    MyEnum_Value2,
} enum_t;

typedef struct MyStruct {
    enum_t _field;
} struct_t;
";

        return ValidateGeneratedCSharpLatestHostBaselineAsync(inputContents, commandLineArgs: DefaultCClangCommandLineArgs, language: "c", languageStandard: DefaultCStandard);
    }

    [Test]
    public Task BooleanLogicalOperandsTest()
    {
        // In C, `&&`/`||` yield `int` and promote a `_Bool` operand to `int`; the equivalent C#
        // operators take `bool` operands directly, so the operands must be emitted as `bool` rather
        // than coerced with `? 1 : 0` (see https://github.com/dotnet/ClangSharp/issues/820).
        var inputContents = @"int LogicalAnd(_Bool a, _Bool b)
{
    return a && b;
}

int LogicalOr(_Bool a, _Bool b)
{
    return a || b;
}

int MixedOperands(_Bool a, int b)
{
    return a && (b < 3);
}

void LogicalLocal(_Bool a, _Bool b)
{
    int x = a || b;
}
";

        return ValidateGeneratedCSharpLatestHostBaselineAsync(inputContents, commandLineArgs: DefaultCClangCommandLineArgs, language: "c", languageStandard: DefaultCStandard);
    }

    [Test]
    public Task BooleanCoercionToIntegerTest()
    {
        // The inverse of #820: in C, relational and logical operators yield `int`, so their result
        // can be stored in or returned as an integer without a cast. The equivalent C# operators
        // yield `bool`, which has no implicit conversion to an integer, so a `? 1 : 0` coercion must
        // be inserted (see https://github.com/dotnet/ClangSharp/issues/820).
        var inputContents = @"
int ReturnFromComparison(int a, int b)
{
    return a < b;
}

int ReturnFromLogical(int a, int b)
{
    return a < b || a > b;
}

int ReturnFromNegation(int a)
{
    return !a;
}

void Locals(int a, int b)
{
    int x = a < b;
    int y = !a;
}
";

        return ValidateGeneratedCSharpLatestHostBaselineAsync(inputContents, commandLineArgs: DefaultCClangCommandLineArgs, language: "c", languageStandard: DefaultCStandard);
    }

    [Test]
    public Task BooleanReturnFromComparisonTest()
    {
        // In C, relational and logical operators yield `int`, so returning one as `_Bool`
        // inserts an `IntegralToBoolean` cast. The equivalent C# operators already yield `bool`,
        // so the `!= 0` coercion must be omitted (see https://github.com/dotnet/ClangSharp/issues/820).
        var inputContents = @"
#define bool _Bool

bool FromComparison(int a, int b)
{
    return a < b;
}

bool FromLogical(int a, int b)
{
    return a < b || a > b;
}

bool FromInteger(int a)
{
    return a;
}
";

        return ValidateGeneratedCSharpLatestHostBaselineAsync(inputContents, commandLineArgs: DefaultCClangCommandLineArgs, language: "c", languageStandard: DefaultCStandard);
    }

    [Test]
    public Task CompoundLiteralExprTest()
    {
        // C compound literals `(T){ ... }` project as the equivalent C# object initializer
        // (see https://github.com/dotnet/ClangSharp/issues/819).
        var inputContents = @"struct b3Vec3
{
    float x;
    float y;
    float z;
};

struct b3Vec3 b3Negate(struct b3Vec3 a)
{
    return (struct b3Vec3){ -a.x, -a.y, -a.z };
}
";

        return ValidateGeneratedCSharpLatestHostBaselineAsync(inputContents, commandLineArgs: DefaultCClangCommandLineArgs, language: "c", languageStandard: DefaultCStandard);
    }

    [Test]
    public Task EnumTest()
    {
        var inputContents = @"enum {
    VALUE1 = 0,
    VALUE2,
    VALUE3
};

struct MyStruct {
    enum {
        VALUEA = 0,
        VALUEB,
        VALUEC
    } field;
};
";

        var diagnostics = new[] {
            new Diagnostic(DiagnosticLevel.Info, "Found anonymous enum: __AnonymousEnum_ClangUnsavedFile_L1_C1. Mapping values as constants in: Methods", "Line 1, Column 1 in ClangUnsavedFile.h"),
            new Diagnostic(DiagnosticLevel.Info, "Found anonymous enum: __AnonymousEnum_ClangUnsavedFile_L8_C5. Mapping values as constants in: Methods", "Line 8, Column 5 in ClangUnsavedFile.h")
        };
        return ValidateGeneratedCSharpLatestHostBaselineAsync(inputContents, commandLineArgs: DefaultCClangCommandLineArgs, expectedDiagnostics: diagnostics, language: "c", languageStandard: DefaultCStandard);
    }

    [Test]
    public Task StructTest()
    {
        var inputContents = @"typedef struct _MyStruct
{
    int _field;
} MyStruct;

typedef struct _MyOtherStruct
{
    MyStruct _field1;
    MyStruct* _field2;
} MyOtherStruct;

typedef struct _MyStructWithAnonymousStruct
{
    struct {
        int _field;
    } _anonymousStructField1;
} MyStructWithAnonymousStruct;

typedef struct _MyStructWithAnonymousUnion
{
    union {
        int _field1;
        int* _field2;
    } union1;
} MyStructWithAnonymousUnion;
";
        return ValidateGeneratedCSharpLatestWindowsBaselineAsync(inputContents, commandLineArgs: DefaultCClangCommandLineArgs, language: "c", languageStandard: DefaultCStandard);
    }

    [Test]
    public Task UnionTest()
    {
        var inputContents = @"typedef union _MyUnion
{
    int _field;
} MyUnion;

typedef union _MyOtherUnion
{
    MyUnion _field1;
    MyUnion* _field2;
} MyOtherUnion;
";
        return ValidateGeneratedCSharpLatestWindowsBaselineAsync(inputContents, commandLineArgs: DefaultCClangCommandLineArgs, language: "c", languageStandard: DefaultCStandard);
    }

    [Test]
    public Task MacroTest()
    {
        var inputContents = @"#define MyMacro1 5
#define MyMacro2 MyMacro1";

        return ValidateGeneratedCSharpLatestWindowsBaselineAsync(inputContents, commandLineArgs: DefaultCClangCommandLineArgs, language: "c", languageStandard: DefaultCStandard);
    }

    [Test]
    public Task UnsignedIntBitshiftTest()
    {
        var inputContents = @"
const int Signed = 1;
const long SignedLong = 1;
const unsigned int Unsigned = 1;

const int ShiftSigned = 1 << Signed;
const int ShiftSignedLong = 1 << SignedLong;
const int ShiftUnsigned = 1 << Unsigned;

const int Char = 1 << 'a';

const int Byte = 1 << (signed char)1;
const int UByte = 1 << (unsigned char)1;

const int CInt = 1 << 1;
const int CUint = 1 << 1U;

const int Negative = 1 << -1;

const int OutOfRangePos = 1 << 10000000000;
const int OutOfRangeNeg = 1 << -10000000000;

const int IntMax = 1 << 2147483647;
const int IntMin = 1 << -2147483648;

const int LongMax = 1 << 9223372036854775807;
const int LongMin = 1 << -9223372036854775808;

const int ULongMax = 1 << 18446744073709551615;

const int Hexadecimal = 1 << 0x01;

#define Left 1 << 1U
#define Right 1 >> 1U

#define Int 1 << 1
#define Long 1 << 1L
#define LongLong 1 << 1LL
#define ULong 1 << 1UL
#define ULongLong 1 << 1ULL

#define Complex ((((unsigned int)(0)) << 29U) | (((unsigned int)(1)) << 22U) | (((unsigned int)(0)) << 12U) | ((unsigned int)(0)))
";

        // Non-ideal cases:
        // UByte
        // IntMin
        // ULongMax
        // Hexadecimal
        return ValidateGeneratedCSharpLatestWindowsBaselineAsync(inputContents, commandLineArgs: DefaultCClangCommandLineArgs, language: "c", languageStandard: DefaultCStandard);
    }


    [Test]
    public Task UnsignedIntBitshiftTestUnix()
    {
        var inputContents = @"
const long SignedLong = 1;
const int ShiftSignedLong = 1 << SignedLong;
";

        return ValidateGeneratedCSharpLatestUnixBaselineAsync(inputContents, commandLineArgs: DefaultCClangCommandLineArgs, language: "c", languageStandard: DefaultCStandard);
    }

    [Test]
    [Platform("unix")]
    public Task BitfieldEnumPropertySmallBackingTypeTestUnix()
    {
        // This test is here mainly to ensure that the sbyte case gets coverage
        if (RuntimeInformation.ProcessArchitecture != Architecture.X64)
        {
            Assert.Ignore("This test is only valid for Unix x64");
        }

        var inputContents = @"
typedef struct Bitfield {
    unsigned char bits1 : 8;
    char bits2 : 8;
    unsigned char bits3 : 8;
} Bitfield;
";

        return ValidateGeneratedCSharpLatestUnixBaselineAsync(inputContents, commandLineArgs: DefaultCClangCommandLineArgs, language: "c", languageStandard: DefaultCStandard);
    }

    [Test]
    [Platform("unix")]
    public Task BitfieldEnumPropertyBackingTypeTestUnix()
    {
        // This test is here mainly to ensure that the sbyte case gets coverage
        if (RuntimeInformation.ProcessArchitecture != Architecture.X64)
        {
            Assert.Ignore("This test is only valid for Unix x64");
        }

        var inputContents = @"
typedef struct IntBitfield {
    int bits : 8;
    unsigned int bits2 : 8;
} IntBitfield;

typedef struct UIntBitfield {
    unsigned int bits1 : 8;
    int bits2 : 8;
    unsigned char bits3 : 8;
    char bits4 : 8;
} UIntBitfield;
";

        return ValidateGeneratedCSharpLatestUnixBaselineAsync(inputContents, commandLineArgs: DefaultCClangCommandLineArgs, language: "c", languageStandard: DefaultCStandard);
    }

    [Test]
    [Platform("unix")]
    public Task BitfieldEnumPropertyTypeCastTestUnix()
    {
        var inputContents = @"
typedef enum Flags {
    Member = 0x7FFFFFFF
} Flags;

typedef struct Bitfield {
    unsigned int bits : 8;
    Flags flags : 8;
} Bitfield;
";

        return ValidateGeneratedCSharpLatestUnixBaselineAsync(inputContents, commandLineArgs: DefaultCClangCommandLineArgs, language: "c", languageStandard: DefaultCStandard);
    }

    [Test]
    [Platform("unix")]
    public Task BitfieldTypeDefTypeCastTestUnix()
    {
        var inputContents = @"
typedef unsigned int Number;

typedef struct Bitfield {
    Number bits : 8;
} Bitfield;
";

        return ValidateGeneratedCSharpLatestUnixBaselineAsync(inputContents, commandLineArgs: DefaultCClangCommandLineArgs, language: "c", languageStandard: DefaultCStandard);
    }

    [Test]
    [Platform("win")] // This test has slight platform-specific differences
    public Task BitfieldEnumPropertyTypeCastTestWindows()
    {
        var inputContents = @"
typedef enum Flags {
    Member = 0x7FFFFFFF
} Flags;

typedef struct Bitfield {
    unsigned int bits : 8;
    Flags flags : 8;
} Bitfield;
";

        return ValidateGeneratedCSharpLatestWindowsBaselineAsync(inputContents, commandLineArgs: DefaultCClangCommandLineArgs, language: "c", languageStandard: DefaultCStandard);
    }

    [Test]
    [Platform("unix")] // This test has slight platform-specific differences
    public Task BitfieldEnumTypeDefPropertyTypeCast()
    {
        var inputContents = @"
typedef enum FlagBits {
    Member = 0x7FFFFFFF
} FlagBits;
typedef unsigned int Flags;

typedef struct Bitfield {
    unsigned int bits : 8;
    Flags flags : 8;
} Bitfield;
";

        return ValidateGeneratedCSharpLatestUnixBaselineAsync(inputContents, commandLineArgs: DefaultCClangCommandLineArgs, language: "c", languageStandard: DefaultCStandard);
    }

    [Test]
    [Platform("unix")] // This test has slight platform-specific differences
    public Task BitfieldEnumTypeDefPropertyTypeCastWithRemappingTest()
    {
        var inputContents = @"
typedef enum FlagBits {
    Member = 0x7FFFFFFF
} FlagBits;
typedef unsigned int Flags;

typedef struct Bitfield {
    unsigned int bits : 8;
    Flags flags : 8;
} Bitfield;
";

        return ValidateGeneratedCSharpLatestUnixBaselineAsync(inputContents,
            commandLineArgs: DefaultCClangCommandLineArgs,
            language: "c",
            languageStandard: DefaultCStandard,
            remappedNames: new Dictionary<string, string>()
            {
                { "Flags", "FlagBits" }
            });
    }

    [Test]
    [Platform("unix")] // This test has slight platform-specific differences
    public Task BitfieldEnumTypeDefPropertyTypeCastWithSelfRemappingTest()
    {
        var inputContents = @"
typedef unsigned int Flags;

typedef struct Bitfield {
    unsigned int bits : 8;
    Flags flags : 8;
} Bitfield;
";

        return ValidateGeneratedCSharpLatestUnixBaselineAsync(inputContents,
            commandLineArgs: DefaultCClangCommandLineArgs,
            language: "c",
            languageStandard: DefaultCStandard,
            remappedNames: new Dictionary<string, string>()
            {
                { "Flags", "Flags" }
            });
    }

    [Test]
    [Platform("unix")] // This test has platform-specific differences (on Windows, __LONG_MAX__ is 32-bit)
    public Task CLongDefinesTestUnix()
    {
        // C longs differ based on platform
        // These values are taken from the Linux headers when using Clang
        var inputContents = @"
// stdint.h
#define SIZE_MAX (18446744073709551615UL)

// cl_ext.h from OpenCL
#define CL_IMPORT_MEMORY_WHOLE_ALLOCATION_ARM SIZE_MAX

// limits.h
#define LONG_MAX  __LONG_MAX__
#define ULONG_MAX (__LONG_MAX__ *2UL+1UL)

// These expressions never exceed the allowed range, so should remain const
#define CONST_IN_RANGE_S1 ((long)2147483647)
#define CONST_IN_RANGE_U1 ((unsigned long)4294967295)

// These expressions exceed the allowed range, so should become static readonly
#define READONLY_OUT_OF_RANGE_S1 ((long)4294967295)
#define READONLY_OUT_OF_RANGE_S2 ((long)4294967296)
#define READONLY_OUT_OF_RANGE_U1 ((unsigned long)4294967296)
";

        // We use "static readonly" instead of "const" because nint/nuint differ on 32/64-bit platforms
        return ValidateGeneratedCSharpLatestUnixBaselineAsync(inputContents, commandLineArgs: DefaultCClangCommandLineArgs, language: "c", languageStandard: DefaultCStandard);
    }

    [Test]
    [Platform("unix")]
    public Task CLongDefinesRegressionTestUnix()
    {
        // This test is to catch a potential regression when changing how native integers are handled
        // Specifically, (18446744073709551615U) must not be wrapped in an unnecessary unchecked scope

        // Macro values are taken from the Linux headers when using Clang
        // Some values are substituted for simplicity
        var inputContents = @"
// __stddef_size_t.h
typedef __SIZE_TYPE__ size_t;

// stdbool.h
#define bool _Bool
#define true 1
#define false 0

// SDL_stdinc.h from SDL3
bool SDL_size_add_check_overflow(size_t a, size_t b, size_t *ret)
{
    if (b > (18446744073709551615UL) - a) {
        return false;
    }
    *ret = a + b;
    return true;
}
";

        // The unnecessary unchecked scope around the `ulong` literal is now elided; see
        // https://github.com/dotnet/ClangSharp/issues/709
        return ValidateGeneratedCSharpLatestUnixBaselineAsync(inputContents, commandLineArgs: DefaultCClangCommandLineArgs, language: "c", languageStandard: DefaultCStandard);
    }

    [Test]
    [Platform("win")] // This test has platform-specific differences
    public Task CLongDefinesTestWindows()
    {
        // C longs differ based on platform
        // These values are taken from the Windows headers when using MSVC
        var inputContents = @"
// limits.h
#define SIZE_MAX 0xffffffffffffffffui64

// cl_ext.h from OpenCL
#define CL_IMPORT_MEMORY_WHOLE_ALLOCATION_ARM SIZE_MAX

// limits.h
#define LONG_MAX 2147483647L
#define ULONG_MAX 0xffffffffUL
";

        return ValidateGeneratedCSharpLatestWindowsBaselineAsync(inputContents, commandLineArgs: DefaultCClangCommandLineArgs, language: "c", languageStandard: DefaultCStandard);
    }

    [Test]
    public Task IntptrDefineTest()
    {
        string inputContents;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            // These values are taken from the Windows headers when using MSVC
            inputContents = @"
// vcruntime.h
typedef __int64 intptr_t;

// cl_ext.h from OpenCL
#define CL_ICD2_TAG_KHR ((intptr_t)0x4F50454E434C3331)
";
        }
        else
        {
            // These values are taken from the Linux headers when using Clang
            inputContents = @"
// stdint.h
typedef long int intptr_t;

// cl_ext.h from OpenCL
#define CL_ICD2_TAG_KHR ((intptr_t)0x4F50454E434C3331)
";
        }

        var expectedOutputContents = @"namespace ClangSharp.Test
{
    public static partial class Methods
    {
        [NativeTypeName(""#define CL_ICD2_TAG_KHR ((intptr_t)0x4F50454E434C3331)"")]
        public static readonly nint CL_ICD2_TAG_KHR = unchecked((nint)(0x4F50454E434C3331));
    }
}
";

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return ValidateGeneratedCSharpLatestWindowsBindingsAsync(inputContents, expectedOutputContents, commandLineArgs: DefaultCClangCommandLineArgs, language: "c", languageStandard: DefaultCStandard);
        }
        else
        {
            return ValidateGeneratedCSharpLatestUnixBindingsAsync(inputContents, expectedOutputContents, commandLineArgs: DefaultCClangCommandLineArgs, language: "c", languageStandard: DefaultCStandard);
        }
    }

    [Test]
    public Task FixedWidthIntTypedefTest()
    {
        // The exact-width stdint types are architecture-independent, but their underlying
        // typedef can decay to a platform-specific integer (e.g. `unsigned long` on Unix,
        // which would otherwise map to `nuint`). Ensure they resolve to the fixed-width type.

        string inputContents;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            inputContents = @"typedef long long int64_t;
typedef unsigned long long uint64_t;

struct MyStruct
{
    int64_t s64;
    uint64_t u64;
};
";
        }
        else
        {
            inputContents = @"typedef long int64_t;
typedef unsigned long uint64_t;

struct MyStruct
{
    int64_t s64;
    uint64_t u64;
};
";
        }

        var expectedOutputContents = @"namespace ClangSharp.Test
{
    public partial struct MyStruct
    {
        [NativeTypeName(""int64_t"")]
        public long s64;

        [NativeTypeName(""uint64_t"")]
        public ulong u64;
    }
}
";

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return ValidateGeneratedCSharpLatestWindowsBindingsAsync(inputContents, expectedOutputContents, commandLineArgs: DefaultCClangCommandLineArgs, language: "c", languageStandard: DefaultCStandard);
        }
        else
        {
            return ValidateGeneratedCSharpLatestUnixBindingsAsync(inputContents, expectedOutputContents, commandLineArgs: DefaultCClangCommandLineArgs, language: "c", languageStandard: DefaultCStandard);
        }
    }

    [Test]
    [Platform("unix")] // ssize_t is a POSIX type and not defined by MSVC
    public Task SSizeTypedefTestUnix()
    {
        // ssize_t is pointer-width, so it should map to nint just like ptrdiff_t/intptr_t.

        var inputContents = @"typedef long ssize_t;

struct MyStruct
{
    ssize_t count;
};
";

        var expectedOutputContents = @"namespace ClangSharp.Test
{
    public partial struct MyStruct
    {
        [NativeTypeName(""ssize_t"")]
        public nint count;
    }
}
";

        return ValidateGeneratedCSharpLatestUnixBindingsAsync(inputContents, expectedOutputContents, commandLineArgs: DefaultCClangCommandLineArgs, language: "c", languageStandard: DefaultCStandard);
    }

    [Test]
    [Platform("win")] // The pointer-width Windows types are only remapped when targeting Windows
    public Task WindowsPointerWidthTypedefTestWindows()
    {
        // The pointer-width Windows SDK types are architecture-independent (pointer-width by
        // definition), but their underlying typedef (e.g. __int64) would otherwise decay to a
        // fixed-width type. Ensure they resolve to nint/nuint just like intptr_t/size_t.

        var inputContents = @"typedef __int64 INT_PTR;
typedef unsigned __int64 UINT_PTR;

struct MyStruct
{
    INT_PTR s;
    UINT_PTR u;
};
";

        var expectedOutputContents = @"namespace ClangSharp.Test
{
    public partial struct MyStruct
    {
        [NativeTypeName(""INT_PTR"")]
        public nint s;

        [NativeTypeName(""UINT_PTR"")]
        public nuint u;
    }
}
";

        return ValidateGeneratedCSharpLatestWindowsBindingsAsync(inputContents, expectedOutputContents, commandLineArgs: DefaultCClangCommandLineArgs, language: "c", languageStandard: DefaultCStandard);
    }

    [Test]
    [Platform("win")] // _GUID is only remapped to System.Guid when targeting Windows
    public Task GuidTypedefTestWindows()
    {
        // _GUID matches the layout of System.Guid and is remapped to it. Its definition
        // normally lives in a system header that isn't traversed, so exclude it here and
        // ensure references resolve to Guid.

        var inputContents = @"typedef struct _GUID
{
    unsigned long Data1;
    unsigned short Data2;
    unsigned short Data3;
    unsigned char Data4[8];
} GUID;

struct MyStruct
{
    GUID id;
};
";

        var expectedOutputContents = @"using System;

namespace ClangSharp.Test
{
    public partial struct MyStruct
    {
        public Guid id;
    }
}
";

        return ValidateGeneratedCSharpLatestWindowsBindingsAsync(inputContents, expectedOutputContents, excludedNames: ["_GUID"], commandLineArgs: DefaultCClangCommandLineArgs, language: "c", languageStandard: DefaultCStandard);
    }

    [Test]
    public Task VoidPointerArithmeticTest()
    {
        // C/C++ allow pointer arithmetic on `void*` (treating it as byte-sized), but C# does not,
        // so the generator must insert an explicit `(byte*)` cast on the `void*` operand.
        var inputContents = @"void* MyFunction(void *buf, unsigned long long size) {
    return buf + size;
}
";

        return ValidateGeneratedCSharpLatestWindowsBaselineAsync(inputContents, commandLineArgs: DefaultCClangCommandLineArgs, language: "c", languageStandard: DefaultCStandard);
    }
}
