// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClangSharp.UnitTests
{
    public sealed class CSharpLatestWindows_EnumDeclarationTest : EnumDeclarationTest
    {
        public override Task BasicTest()
        {
            var inputContents = @"enum MyEnum : int
{
    MyEnum_Value0,
    MyEnum_Value1,
    MyEnum_Value2,
};
";

            var expectedOutputContents = @"namespace ClangSharp.Test
{
    public enum MyEnum
    {
        MyEnum_Value0,
        MyEnum_Value1,
        MyEnum_Value2,
    }
}
";

            return ValidateGeneratedCSharpLatestWindowsBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task BasicValueTest()
        {
            var inputContents = @"enum MyEnum : int
{
    MyEnum_Value1 = 1,
    MyEnum_Value2,
    MyEnum_Value3,
};
";

            var expectedOutputContents = @"namespace ClangSharp.Test
{
    public enum MyEnum
    {
        MyEnum_Value1 = 1,
        MyEnum_Value2,
        MyEnum_Value3,
    }
}
";

            return ValidateGeneratedCSharpLatestWindowsBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task ExcludeTest()
        {
            var inputContents = @"enum MyEnum : int
{
    MyEnum_Value0,
    MyEnum_Value1,
    MyEnum_Value2,
};
";

            var expectedOutputContents = string.Empty;

            var excludedNames = new string[] { "MyEnum" };
            return ValidateGeneratedCSharpLatestWindowsBindingsAsync(inputContents, expectedOutputContents, excludedNames: excludedNames);
        }

        public override Task ExplicitTypedTest(string nativeType, string expectedManagedType)
        {
            var inputContents = $@"enum MyEnum : {nativeType}
{{
    MyEnum_Value0,
    MyEnum_Value1,
    MyEnum_Value2,
}};
";

            var expectedOutputContents = $@"namespace ClangSharp.Test
{{
    public enum MyEnum : {expectedManagedType}
    {{
        MyEnum_Value0,
        MyEnum_Value1,
        MyEnum_Value2,
    }}
}}
";

            return ValidateGeneratedCSharpLatestWindowsBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task ExplicitTypedWithNativeTypeNameTest(string nativeType, string expectedManagedType)
        {
            var inputContents = $@"enum MyEnum : {nativeType}
{{
    MyEnum_Value0,
    MyEnum_Value1,
    MyEnum_Value2,
}};
";

            var expectedOutputContents = $@"namespace ClangSharp.Test
{{
    [NativeTypeName(""{nativeType}"")]
    public enum MyEnum : {expectedManagedType}
    {{
        MyEnum_Value0,
        MyEnum_Value1,
        MyEnum_Value2,
    }}
}}
";

            return ValidateGeneratedCSharpLatestWindowsBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task RemapTest()
        {
            var inputContents = @"typedef enum _MyEnum1 : int
{
    MyEnum1_Value1,
    MyEnum1_Value2,
    MyEnum1_Value3,
} MyEnum1;

namespace Namespace1
{
    namespace Namespace2
    {
        typedef enum _MyEnum2 : int
        {
            MyEnum2_Value1,
            MyEnum2_Value2,
            MyEnum2_Value3,
        } MyEnum2;

        typedef enum _MyEnum3 : int
        {
            MyEnum3_Value1,
            MyEnum3_Value2,
            MyEnum3_Value3,
        } MyEnum3;

        typedef enum _MyEnum4 : int
        {
            MyEnum4_Value1,
            MyEnum4_Value2,
            MyEnum4_Value3,
        } MyEnum4;
    }
}
";

            var expectedOutputContents = @"namespace ClangSharp.Test
{
    public enum MyEnum1
    {
        MyEnum1_Value1,
        MyEnum1_Value2,
        MyEnum1_Value3,
    }

    public enum MyEnum2
    {
        MyEnum2_Value1,
        MyEnum2_Value2,
        MyEnum2_Value3,
    }

    public enum MyEnum3
    {
        MyEnum3_Value1,
        MyEnum3_Value2,
        MyEnum3_Value3,
    }

    public enum MyEnum4
    {
        MyEnum4_Value1,
        MyEnum4_Value2,
        MyEnum4_Value3,
    }
}
";

            var remappedNames = new Dictionary<string, string> { ["_MyEnum1"] = "MyEnum1", ["Namespace1.Namespace2._MyEnum2"] = "MyEnum2", ["_MyEnum3"] = "MyEnum3", ["Namespace1::Namespace2::_MyEnum4"] = "MyEnum4" };
            return ValidateGeneratedCSharpLatestWindowsBindingsAsync(inputContents, expectedOutputContents, remappedNames: remappedNames);
        }

        public override Task WithAttributeTest()
        {
            var inputContents = @"enum MyEnum1 : int
{
    MyEnum1_Value1 = 1,
};

enum MyEnum2 : int
{
    MyEnum2_Value1 = 1,
};
";

            var expectedOutputContents = @"using System;

namespace ClangSharp.Test
{
    [Flags]
    public enum MyEnum1
    {
        MyEnum1_Value1 = 1,
    }

    public enum MyEnum2
    {
        MyEnum2_Value1 = 1,
    }
}
";

            var withAttributes = new Dictionary<string, IReadOnlyList<string>>
            {
                ["MyEnum1"] = new List<string>() { "Flags" }
            };
            return ValidateGeneratedCSharpLatestWindowsBindingsAsync(inputContents, expectedOutputContents, withAttributes: withAttributes);
        }

        public override Task WithAttributeStarTest()
        {
            var inputContents = @"enum MyEnum1 : int
{
    MyEnum1_Value1 = 1,
};

enum MyEnum2 : int
{
    MyEnum2_Value1 = 1,
};
";

            var expectedOutputContents = @"using System;

namespace ClangSharp.Test
{
    [Flags]
    public enum MyEnum1
    {
        MyEnum1_Value1 = 1,
    }

    [Flags]
    public enum MyEnum2
    {
        MyEnum2_Value1 = 1,
    }
}
";

            var withAttributes = new Dictionary<string, IReadOnlyList<string>>
            {
                ["*"] = new List<string>() { "Flags" }
            };
            return ValidateGeneratedCSharpLatestWindowsBindingsAsync(inputContents, expectedOutputContents, withAttributes: withAttributes);
        }

        public override Task WithAttributeStarPlusTest()
        {
            var inputContents = @"enum MyEnum1 : int
{
    MyEnum1_Value1 = 1,
};

enum MyEnum2 : int
{
    MyEnum2_Value1 = 1,
};
";

            var expectedOutputContents = @"using System;
using System.ComponentModel;

namespace ClangSharp.Test
{
    [Flags]
    public enum MyEnum1
    {
        MyEnum1_Value1 = 1,
    }

    [Flags]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public enum MyEnum2
    {
        MyEnum2_Value1 = 1,
    }
}
";

            var withAttributes = new Dictionary<string, IReadOnlyList<string>>
            {
                ["*"] = new List<string>() { "Flags" },
                ["MyEnum2"] = new List<string>() { "EditorBrowsable(EditorBrowsableState.Never)" }
            };
            return ValidateGeneratedCSharpLatestWindowsBindingsAsync(inputContents, expectedOutputContents, withAttributes: withAttributes);
        }

        public override Task WithNamespaceTest()
        {
            var inputContents = @"enum MyEnum1 : int
{
    MyEnum1_Value1 = 1,
};

enum MyEnum2 : int
{
    MyEnum2_Value1 = MyEnum1_Value1,
};
";

            var expectedOutputContents = @"using static ClangSharp.Test.MyEnum1;

namespace ClangSharp.Test
{
    public enum MyEnum1
    {
        MyEnum1_Value1 = 1,
    }

    public enum MyEnum2
    {
        MyEnum2_Value1 = MyEnum1_Value1,
    }
}
";

            var withNamespaces = new Dictionary<string, IReadOnlyList<string>>
            {
                ["MyEnum1"] = new List<string>() { "static ClangSharp.Test.MyEnum1" }
            };
            return ValidateGeneratedCSharpLatestWindowsBindingsAsync(inputContents, expectedOutputContents, withUsings: withNamespaces);
        }

        public override Task WithNamespaceStarTest()
        {
            var inputContents = @"enum MyEnum1 : int
{
    MyEnum1_Value1 = 1,
};

enum MyEnum2 : int
{
    MyEnum2_Value1 = MyEnum1_Value1,
};
";

            var expectedOutputContents = @"using static ClangSharp.Test.MyEnum1;

namespace ClangSharp.Test
{
    public enum MyEnum1
    {
        MyEnum1_Value1 = 1,
    }

    public enum MyEnum2
    {
        MyEnum2_Value1 = MyEnum1_Value1,
    }
}
";

            var withNamespaces = new Dictionary<string, IReadOnlyList<string>>
            {
                ["*"] = new List<string>() { "static ClangSharp.Test.MyEnum1" }
            };
            return ValidateGeneratedCSharpLatestWindowsBindingsAsync(inputContents, expectedOutputContents, withUsings: withNamespaces);
        }

        public override Task WithNamespaceStarPlusTest()
        {
            var inputContents = @"enum MyEnum1 : int
{
    MyEnum1_Value1 = 1,
};

enum MyEnum2 : int
{
    MyEnum2_Value1 = MyEnum1_Value1,
};
";

            var expectedOutputContents = @"using System;
using static ClangSharp.Test.MyEnum1;

namespace ClangSharp.Test
{
    public enum MyEnum1
    {
        MyEnum1_Value1 = 1,
    }

    public enum MyEnum2
    {
        MyEnum2_Value1 = MyEnum1_Value1,
    }
}
";

            var withNamespaces = new Dictionary<string, IReadOnlyList<string>>
            {
                ["*"] = new List<string>() { "static ClangSharp.Test.MyEnum1" },
                ["MyEnum2"] = new List<string>() { "System" }
            };
            return ValidateGeneratedCSharpLatestWindowsBindingsAsync(inputContents, expectedOutputContents, withUsings: withNamespaces);
        }

        public override Task WithCastToEnumType()
        {
            var inputContents = @"enum MyEnum : int
{
    MyEnum_Value0 = (MyEnum) 10,
    MyEnum_Value1 = (MyEnum) MyEnum_Value0,
    MyEnum_Value2 = ((MyEnum) 10) + MyEnum_Value1,
};
";

            var expectedOutputContents = @"namespace ClangSharp.Test
{
    public enum MyEnum
    {
        MyEnum_Value0 = (int)(MyEnum)(10),
        MyEnum_Value1 = (int)(MyEnum)(MyEnum_Value0),
        MyEnum_Value2 = ((int)(MyEnum)(10)) + MyEnum_Value1,
    }
}
";

            return ValidateGeneratedCSharpLatestWindowsBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task WithMultipleEnumsTest()
        {
            var inputContents = @"enum MyEnum1 : int
{
    MyEnum1_Value0 = 10,
};

enum MyEnum2 : int
{
    MyEnum2_Value0 = MyEnum1_Value0,
    MyEnum2_Value1 = MyEnum1_Value0 + (MyEnum1) 10,
};
";

            var expectedOutputContents = @"using static ClangSharp.Test.MyEnum1;

namespace ClangSharp.Test
{
    public enum MyEnum1
    {
        MyEnum1_Value0 = 10,
    }

    public enum MyEnum2
    {
        MyEnum2_Value0 = MyEnum1_Value0,
        MyEnum2_Value1 = MyEnum1_Value0 + (int)(MyEnum1)(10),
    }
}
";

            return ValidateGeneratedCSharpLatestWindowsBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task WithImplicitConversionTest()
        {
            var inputContents = @"enum MyEnum : int
{
    MyEnum_Value0,
    MyEnum_Value1,
    MyEnum_Value2 = 0x80000000,
};
";

            var expectedOutputContents = @"namespace ClangSharp.Test
{
    public enum MyEnum
    {
        MyEnum_Value0,
        MyEnum_Value1,
        MyEnum_Value2 = unchecked((int)(0x80000000)),
    }
}
";

            return ValidateGeneratedCSharpLatestWindowsBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task WithTypeTest()
        {
            var inputContents = @"enum MyEnum : int
{
    MyEnum_Value0,
    MyEnum_Value1,
    MyEnum_Value2,
};
";

            var expectedOutputContents = @"namespace ClangSharp.Test
{
    [NativeTypeName(""int"")]
    public enum MyEnum : uint
    {
        MyEnum_Value0,
        MyEnum_Value1,
        MyEnum_Value2,
    }
}
";

            var withTypes = new Dictionary<string, string> {
                ["MyEnum"] = "uint"
            };
            return ValidateGeneratedCSharpLatestWindowsBindingsAsync(inputContents, expectedOutputContents, withTypes: withTypes);
        }

        public override Task WithTypeAndImplicitConversionTest()
        {
            var inputContents = @"enum MyEnum : int
{
    MyEnum_Value0,
    MyEnum_Value1,
    MyEnum_Value2 = 0x80000000,
};
";

            var expectedOutputContents = @"namespace ClangSharp.Test
{
    [NativeTypeName(""int"")]
    public enum MyEnum : uint
    {
        MyEnum_Value0,
        MyEnum_Value1,
        MyEnum_Value2 = 0x80000000,
    }
}
";

            var withTypes = new Dictionary<string, string>
            {
                ["MyEnum"] = "uint"
            };
            return ValidateGeneratedCSharpLatestWindowsBindingsAsync(inputContents, expectedOutputContents, withTypes: withTypes);
        }

        public override Task WithTypeStarTest()
        {
            var inputContents = @"enum MyEnum1 : int
{
    MyEnum1_Value0
};

enum MyEnum2 : int
{
    MyEnum2_Value0
};
";

            var expectedOutputContents = @"namespace ClangSharp.Test
{
    [NativeTypeName(""int"")]
    public enum MyEnum1 : uint
    {
        MyEnum1_Value0,
    }

    [NativeTypeName(""int"")]
    public enum MyEnum2 : uint
    {
        MyEnum2_Value0,
    }
}
";

            var withTypes = new Dictionary<string, string>
            {
                ["*"] = "uint"
            };
            return ValidateGeneratedCSharpLatestWindowsBindingsAsync(inputContents, expectedOutputContents, withTypes: withTypes);
        }

        public override Task WithTypeStarOverrideTest()
        {
            var inputContents = @"enum MyEnum1 : int
{
    MyEnum1_Value0
};

enum MyEnum2 : int
{
    MyEnum2_Value0
};
";

            var expectedOutputContents = @"namespace ClangSharp.Test
{
    public enum MyEnum1
    {
        MyEnum1_Value0,
    }

    [NativeTypeName(""int"")]
    public enum MyEnum2 : uint
    {
        MyEnum2_Value0,
    }
}
";

            var withTypes = new Dictionary<string, string>
            {
                ["*"] = "uint",
                ["MyEnum1"] = "int",
            };
            return ValidateGeneratedCSharpLatestWindowsBindingsAsync(inputContents, expectedOutputContents, withTypes: withTypes);
        }
    }
}
