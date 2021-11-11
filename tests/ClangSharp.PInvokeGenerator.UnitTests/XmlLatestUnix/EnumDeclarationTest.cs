// Copyright (c) .NET Foundation and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClangSharp.UnitTests
{
    public sealed class XmlLatestUnix_EnumDeclarationTest : EnumDeclarationTest
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

            var expectedOutputContents = @"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <enumeration name=""MyEnum"" access=""public"">
      <type>int</type>
      <enumerator name=""MyEnum_Value0"" access=""public"">
        <type primitive=""False"">int</type>
      </enumerator>
      <enumerator name=""MyEnum_Value1"" access=""public"">
        <type primitive=""False"">int</type>
      </enumerator>
      <enumerator name=""MyEnum_Value2"" access=""public"">
        <type primitive=""False"">int</type>
      </enumerator>
    </enumeration>
  </namespace>
</bindings>
";

            return ValidateGeneratedXmlLatestUnixBindingsAsync(inputContents, expectedOutputContents);
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

            var expectedOutputContents = @"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <enumeration name=""MyEnum"" access=""public"">
      <type>int</type>
      <enumerator name=""MyEnum_Value1"" access=""public"">
        <type primitive=""False"">int</type>
        <value>
          <code>1</code>
        </value>
      </enumerator>
      <enumerator name=""MyEnum_Value2"" access=""public"">
        <type primitive=""False"">int</type>
      </enumerator>
      <enumerator name=""MyEnum_Value3"" access=""public"">
        <type primitive=""False"">int</type>
      </enumerator>
    </enumeration>
  </namespace>
</bindings>
";

            return ValidateGeneratedXmlLatestUnixBindingsAsync(inputContents, expectedOutputContents);
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
            return ValidateGeneratedXmlLatestUnixBindingsAsync(inputContents, expectedOutputContents, excludedNames: excludedNames);
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

            var expectedOutputContents = $@"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <enumeration name=""MyEnum"" access=""public"">
      <type>{EscapeXml(expectedManagedType)}</type>
      <enumerator name=""MyEnum_Value0"" access=""public"">
        <type primitive=""False"">{EscapeXml(expectedManagedType)}</type>
      </enumerator>
      <enumerator name=""MyEnum_Value1"" access=""public"">
        <type primitive=""False"">{EscapeXml(expectedManagedType)}</type>
      </enumerator>
      <enumerator name=""MyEnum_Value2"" access=""public"">
        <type primitive=""False"">{EscapeXml(expectedManagedType)}</type>
      </enumerator>
    </enumeration>
  </namespace>
</bindings>
";

            return ValidateGeneratedXmlLatestUnixBindingsAsync(inputContents, expectedOutputContents);
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

            var expectedOutputContents = $@"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <enumeration name=""MyEnum"" access=""public"">
      <type>{EscapeXml(expectedManagedType)}</type>
      <enumerator name=""MyEnum_Value0"" access=""public"">
        <type primitive=""False"">{EscapeXml(expectedManagedType)}</type>
      </enumerator>
      <enumerator name=""MyEnum_Value1"" access=""public"">
        <type primitive=""False"">{EscapeXml(expectedManagedType)}</type>
      </enumerator>
      <enumerator name=""MyEnum_Value2"" access=""public"">
        <type primitive=""False"">{EscapeXml(expectedManagedType)}</type>
      </enumerator>
    </enumeration>
  </namespace>
</bindings>
";

            return ValidateGeneratedXmlLatestUnixBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task RemapTest()
        {
            var inputContents = @"typedef enum _MyEnum : int
{
    MyEnum_Value1,
    MyEnum_Value2,
    MyEnum_Value3,
} MyEnum;
";

            var expectedOutputContents = @"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <enumeration name=""MyEnum"" access=""public"">
      <type>int</type>
      <enumerator name=""MyEnum_Value1"" access=""public"">
        <type primitive=""False"">int</type>
      </enumerator>
      <enumerator name=""MyEnum_Value2"" access=""public"">
        <type primitive=""False"">int</type>
      </enumerator>
      <enumerator name=""MyEnum_Value3"" access=""public"">
        <type primitive=""False"">int</type>
      </enumerator>
    </enumeration>
  </namespace>
</bindings>
";

            var remappedNames = new Dictionary<string, string> { ["_MyEnum"] = "MyEnum" };
            return ValidateGeneratedXmlLatestUnixBindingsAsync(inputContents, expectedOutputContents, remappedNames: remappedNames);
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

            var expectedOutputContents = @"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <attribute>Flags</attribute>
    <enumeration name=""MyEnum1"" access=""public"">
      <type>int</type>
      <enumerator name=""MyEnum1_Value1"" access=""public"">
        <type primitive=""False"">int</type>
        <value>
          <code>1</code>
        </value>
      </enumerator>
    </enumeration>
    <enumeration name=""MyEnum2"" access=""public"">
      <type>int</type>
      <enumerator name=""MyEnum2_Value1"" access=""public"">
        <type primitive=""False"">int</type>
        <value>
          <code>1</code>
        </value>
      </enumerator>
    </enumeration>
  </namespace>
</bindings>
";

            var withAttributes = new Dictionary<string, IReadOnlyList<string>>
            {
                ["MyEnum1"] = new List<string>() { "Flags" }
            };
            return ValidateGeneratedXmlLatestUnixBindingsAsync(inputContents, expectedOutputContents, withAttributes: withAttributes);
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

            var expectedOutputContents = @"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <attribute>Flags</attribute>
    <enumeration name=""MyEnum1"" access=""public"">
      <type>int</type>
      <enumerator name=""MyEnum1_Value1"" access=""public"">
        <type primitive=""False"">int</type>
        <value>
          <code>1</code>
        </value>
      </enumerator>
    </enumeration>
    <attribute>Flags</attribute>
    <enumeration name=""MyEnum2"" access=""public"">
      <type>int</type>
      <enumerator name=""MyEnum2_Value1"" access=""public"">
        <type primitive=""False"">int</type>
        <value>
          <code>1</code>
        </value>
      </enumerator>
    </enumeration>
  </namespace>
</bindings>
";

            var withAttributes = new Dictionary<string, IReadOnlyList<string>>
            {
                ["*"] = new List<string>() { "Flags" }
            };
            return ValidateGeneratedXmlLatestUnixBindingsAsync(inputContents, expectedOutputContents, withAttributes: withAttributes);
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

            var expectedOutputContents = @"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <attribute>Flags</attribute>
    <enumeration name=""MyEnum1"" access=""public"">
      <type>int</type>
      <enumerator name=""MyEnum1_Value1"" access=""public"">
        <type primitive=""False"">int</type>
        <value>
          <code>1</code>
        </value>
      </enumerator>
    </enumeration>
    <attribute>Flags</attribute>
    <attribute>EditorBrowsable(EditorBrowsableState.Never)</attribute>
    <enumeration name=""MyEnum2"" access=""public"">
      <type>int</type>
      <enumerator name=""MyEnum2_Value1"" access=""public"">
        <type primitive=""False"">int</type>
        <value>
          <code>1</code>
        </value>
      </enumerator>
    </enumeration>
  </namespace>
</bindings>
";

            var withAttributes = new Dictionary<string, IReadOnlyList<string>>
            {
                ["*"] = new List<string>() { "Flags" },
                ["MyEnum2"] = new List<string>() { "EditorBrowsable(EditorBrowsableState.Never)" }
            };
            return ValidateGeneratedXmlLatestUnixBindingsAsync(inputContents, expectedOutputContents, withAttributes: withAttributes);
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

            var expectedOutputContents = @"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <enumeration name=""MyEnum1"" access=""public"">
      <type>int</type>
      <enumerator name=""MyEnum1_Value1"" access=""public"">
        <type primitive=""False"">int</type>
        <value>
          <code>1</code>
        </value>
      </enumerator>
    </enumeration>
    <enumeration name=""MyEnum2"" access=""public"">
      <type>int</type>
      <enumerator name=""MyEnum2_Value1"" access=""public"">
        <type primitive=""False"">int</type>
        <value>
          <code>MyEnum1_Value1</code>
        </value>
      </enumerator>
    </enumeration>
  </namespace>
</bindings>
";

            var withNamespaces = new Dictionary<string, IReadOnlyList<string>>
            {
                ["MyEnum1"] = new List<string>() { "static ClangSharp.Test.MyEnum1" }
            };
            return ValidateGeneratedXmlLatestUnixBindingsAsync(inputContents, expectedOutputContents, withUsings: withNamespaces);
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

            var expectedOutputContents = @"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <enumeration name=""MyEnum1"" access=""public"">
      <type>int</type>
      <enumerator name=""MyEnum1_Value1"" access=""public"">
        <type primitive=""False"">int</type>
        <value>
          <code>1</code>
        </value>
      </enumerator>
    </enumeration>
    <enumeration name=""MyEnum2"" access=""public"">
      <type>int</type>
      <enumerator name=""MyEnum2_Value1"" access=""public"">
        <type primitive=""False"">int</type>
        <value>
          <code>MyEnum1_Value1</code>
        </value>
      </enumerator>
    </enumeration>
  </namespace>
</bindings>
";

            var withNamespaces = new Dictionary<string, IReadOnlyList<string>>
            {
                ["*"] = new List<string>() { "static ClangSharp.Test.MyEnum1" }
            };
            return ValidateGeneratedXmlLatestUnixBindingsAsync(inputContents, expectedOutputContents, withUsings: withNamespaces);
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

            var expectedOutputContents = @"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <enumeration name=""MyEnum1"" access=""public"">
      <type>int</type>
      <enumerator name=""MyEnum1_Value1"" access=""public"">
        <type primitive=""False"">int</type>
        <value>
          <code>1</code>
        </value>
      </enumerator>
    </enumeration>
    <enumeration name=""MyEnum2"" access=""public"">
      <type>int</type>
      <enumerator name=""MyEnum2_Value1"" access=""public"">
        <type primitive=""False"">int</type>
        <value>
          <code>MyEnum1_Value1</code>
        </value>
      </enumerator>
    </enumeration>
  </namespace>
</bindings>
";

            var withNamespaces = new Dictionary<string, IReadOnlyList<string>>
            {
                ["*"] = new List<string>() { "static ClangSharp.Test.MyEnum1" },
                ["MyEnum2"] = new List<string>() { "System" }
            };
            return ValidateGeneratedXmlLatestUnixBindingsAsync(inputContents, expectedOutputContents, withUsings: withNamespaces);
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

            var expectedOutputContents = @"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <enumeration name=""MyEnum"" access=""public"">
      <type>int</type>
      <enumerator name=""MyEnum_Value0"" access=""public"">
        <type primitive=""False"">int</type>
        <value>
          <code>(int)(MyEnum)(<value>10</value>)</code>
        </value>
      </enumerator>
      <enumerator name=""MyEnum_Value1"" access=""public"">
        <type primitive=""False"">int</type>
        <value>
          <code>(int)(MyEnum)(<value>MyEnum_Value0</value>)</code>
        </value>
      </enumerator>
      <enumerator name=""MyEnum_Value2"" access=""public"">
        <type primitive=""False"">int</type>
        <value>
          <code>((int)(MyEnum)(<value>10</value>)) + MyEnum_Value1</code>
        </value>
      </enumerator>
    </enumeration>
  </namespace>
</bindings>
";

            return ValidateGeneratedXmlLatestUnixBindingsAsync(inputContents, expectedOutputContents);
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

            var expectedOutputContents = @"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <enumeration name=""MyEnum1"" access=""public"">
      <type>int</type>
      <enumerator name=""MyEnum1_Value0"" access=""public"">
        <type primitive=""False"">int</type>
        <value>
          <code>10</code>
        </value>
      </enumerator>
    </enumeration>
    <enumeration name=""MyEnum2"" access=""public"">
      <type>int</type>
      <enumerator name=""MyEnum2_Value0"" access=""public"">
        <type primitive=""False"">int</type>
        <value>
          <code>MyEnum1_Value0</code>
        </value>
      </enumerator>
      <enumerator name=""MyEnum2_Value1"" access=""public"">
        <type primitive=""False"">int</type>
        <value>
          <code>MyEnum1_Value0 + (int)(MyEnum1)(<value>10</value>)</code>
        </value>
      </enumerator>
    </enumeration>
  </namespace>
</bindings>
";

            return ValidateGeneratedXmlLatestUnixBindingsAsync(inputContents, expectedOutputContents);
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

            var expectedOutputContents = @"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <enumeration name=""MyEnum"" access=""public"">
      <type>int</type>
      <enumerator name=""MyEnum_Value0"" access=""public"">
        <type primitive=""False"">int</type>
      </enumerator>
      <enumerator name=""MyEnum_Value1"" access=""public"">
        <type primitive=""False"">int</type>
      </enumerator>
      <enumerator name=""MyEnum_Value2"" access=""public"">
        <type primitive=""False"">int</type>
        <value>
          <unchecked>
            <value>
              <cast>int</cast>
              <value>
                <code>0x80000000</code>
              </value>
            </value>
          </unchecked>
        </value>
      </enumerator>
    </enumeration>
  </namespace>
</bindings>
";

            return ValidateGeneratedXmlLatestUnixBindingsAsync(inputContents, expectedOutputContents);
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

            var expectedOutputContents = @"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <enumeration name=""MyEnum"" access=""public"">
      <type>uint</type>
      <enumerator name=""MyEnum_Value0"" access=""public"">
        <type primitive=""False"">uint</type>
      </enumerator>
      <enumerator name=""MyEnum_Value1"" access=""public"">
        <type primitive=""False"">uint</type>
      </enumerator>
      <enumerator name=""MyEnum_Value2"" access=""public"">
        <type primitive=""False"">uint</type>
      </enumerator>
    </enumeration>
  </namespace>
</bindings>
";

            var withTypes = new Dictionary<string, string> {
                ["MyEnum"] = "uint"
            };
            return ValidateGeneratedXmlLatestUnixBindingsAsync(inputContents, expectedOutputContents, withTypes: withTypes);
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

            var expectedOutputContents = @"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <enumeration name=""MyEnum"" access=""public"">
      <type>uint</type>
      <enumerator name=""MyEnum_Value0"" access=""public"">
        <type primitive=""False"">uint</type>
      </enumerator>
      <enumerator name=""MyEnum_Value1"" access=""public"">
        <type primitive=""False"">uint</type>
      </enumerator>
      <enumerator name=""MyEnum_Value2"" access=""public"">
        <type primitive=""False"">uint</type>
        <value>
          <code>0x80000000</code>
        </value>
      </enumerator>
    </enumeration>
  </namespace>
</bindings>
";

            var withTypes = new Dictionary<string, string>
            {
                ["MyEnum"] = "uint"
            };
            return ValidateGeneratedXmlLatestUnixBindingsAsync(inputContents, expectedOutputContents, withTypes: withTypes);
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

            var expectedOutputContents = @"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <enumeration name=""MyEnum1"" access=""public"">
      <type>uint</type>
      <enumerator name=""MyEnum1_Value0"" access=""public"">
        <type primitive=""False"">uint</type>
      </enumerator>
    </enumeration>
    <enumeration name=""MyEnum2"" access=""public"">
      <type>uint</type>
      <enumerator name=""MyEnum2_Value0"" access=""public"">
        <type primitive=""False"">uint</type>
      </enumerator>
    </enumeration>
  </namespace>
</bindings>
";

            var withTypes = new Dictionary<string, string>
            {
                ["*"] = "uint"
            };
            return ValidateGeneratedXmlLatestUnixBindingsAsync(inputContents, expectedOutputContents, withTypes: withTypes);
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

            var expectedOutputContents = @"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <enumeration name=""MyEnum1"" access=""public"">
      <type>int</type>
      <enumerator name=""MyEnum1_Value0"" access=""public"">
        <type primitive=""False"">int</type>
      </enumerator>
    </enumeration>
    <enumeration name=""MyEnum2"" access=""public"">
      <type>uint</type>
      <enumerator name=""MyEnum2_Value0"" access=""public"">
        <type primitive=""False"">uint</type>
      </enumerator>
    </enumeration>
  </namespace>
</bindings>
";

            var withTypes = new Dictionary<string, string>
            {
                ["*"] = "uint",
                ["MyEnum1"] = "int",
            };
            return ValidateGeneratedXmlLatestUnixBindingsAsync(inputContents, expectedOutputContents, withTypes: withTypes);
        }
    }
}
