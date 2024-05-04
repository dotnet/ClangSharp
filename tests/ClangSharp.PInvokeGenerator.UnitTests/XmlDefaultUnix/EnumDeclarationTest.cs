// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;

namespace ClangSharp.UnitTests;

[Platform("unix")]
public sealed class XmlDefaultUnix_EnumDeclarationTest : EnumDeclarationTest
{
    protected override Task BasicTestImpl()
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

        return ValidateGeneratedXmlDefaultUnixBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task BasicValueTestImpl()
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

        return ValidateGeneratedXmlDefaultUnixBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task ExcludeTestImpl()
    {
        var inputContents = @"enum MyEnum : int
{
    MyEnum_Value0,
    MyEnum_Value1,
    MyEnum_Value2,
};
";

        var expectedOutputContents = string.Empty;
        return ValidateGeneratedXmlDefaultUnixBindingsAsync(inputContents, expectedOutputContents, excludedNames: ExcludeTestExcludedNames);
    }

    protected override Task ExplicitTypedTestImpl(string nativeType, string expectedManagedType)
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

        return ValidateGeneratedXmlDefaultUnixBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task ExplicitTypedWithNativeTypeNameTestImpl(string nativeType, string expectedManagedType)
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

        return ValidateGeneratedXmlDefaultUnixBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task RemapTestImpl()
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
        return ValidateGeneratedXmlDefaultUnixBindingsAsync(inputContents, expectedOutputContents, remappedNames: remappedNames);
    }

    protected override Task WithAttributeTestImpl()
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
    <enumeration name=""MyEnum1"" access=""public"">
      <attribute>Flags</attribute>
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
            ["MyEnum1"] = ["Flags"]
        };
        return ValidateGeneratedXmlDefaultUnixBindingsAsync(inputContents, expectedOutputContents, withAttributes: withAttributes);
    }

    protected override Task WithNamespaceTestImpl()
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
            ["MyEnum1"] = ["static ClangSharp.Test.MyEnum1"]
        };
        return ValidateGeneratedXmlDefaultUnixBindingsAsync(inputContents, expectedOutputContents, withUsings: withNamespaces);
    }

    protected override Task WithNamespaceStarTestImpl()
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
            ["*"] = ["static ClangSharp.Test.MyEnum1"]
        };
        return ValidateGeneratedXmlDefaultUnixBindingsAsync(inputContents, expectedOutputContents, withUsings: withNamespaces);
    }

    protected override Task WithNamespaceStarPlusTestImpl()
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
            ["*"] = ["static ClangSharp.Test.MyEnum1"],
            ["MyEnum2"] = ["System"]
        };
        return ValidateGeneratedXmlDefaultUnixBindingsAsync(inputContents, expectedOutputContents, withUsings: withNamespaces);
    }

    protected override Task WithCastToEnumTypeImpl()
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

        return ValidateGeneratedXmlDefaultUnixBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task WithMultipleEnumsTestImpl()
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

        return ValidateGeneratedXmlDefaultUnixBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task WithImplicitConversionTestImpl()
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

        return ValidateGeneratedXmlDefaultUnixBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task WithTypeTestImpl()
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
        return ValidateGeneratedXmlDefaultUnixBindingsAsync(inputContents, expectedOutputContents, withTypes: withTypes);
    }

    protected override Task WithTypeAndImplicitConversionTestImpl()
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
        return ValidateGeneratedXmlDefaultUnixBindingsAsync(inputContents, expectedOutputContents, withTypes: withTypes);
    }

    protected override Task WithTypeStarTestImpl()
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
        return ValidateGeneratedXmlDefaultUnixBindingsAsync(inputContents, expectedOutputContents, withTypes: withTypes);
    }

    protected override Task WithTypeStarOverrideTestImpl()
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
        return ValidateGeneratedXmlDefaultUnixBindingsAsync(inputContents, expectedOutputContents, withTypes: withTypes);
    }

    protected override Task WithAnonymousEnumTestImpl()
    {
        var inputContents = @"enum
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
    <enumeration name=""MyEnum2"" access=""public"">
      <type>int</type>
      <enumerator name=""MyEnum2_Value1"" access=""public"">
        <type primitive=""False"">int</type>
        <value>
          <code>MyEnum1_Value1</code>
        </value>
      </enumerator>
    </enumeration>
    <class name=""Methods"" access=""public"" static=""true"">
      <constant name=""MyEnum1_Value1"" access=""public"">
        <type primitive=""True"">uint</type>
        <value>
          <code>1</code>
        </value>
      </constant>
    </class>
  </namespace>
</bindings>
";

        var diagnostics = new[] { new Diagnostic(DiagnosticLevel.Info, "Found anonymous enum: __AnonymousEnum_ClangUnsavedFile_L1_C1. Mapping values as constants in: Methods", "Line 1, Column 1 in ClangUnsavedFile.h") };
        return ValidateGeneratedXmlDefaultUnixBindingsAsync(inputContents, expectedOutputContents, expectedDiagnostics: diagnostics);
    }

    protected override Task WithReferenceToAnonymousEnumEnumeratorTestImpl()
    {
        var inputContents = @"enum
{
    MyEnum1_Value1 = 1,
};

const int MyEnum2_Value1 = MyEnum1_Value1 + 1;
";

        var expectedOutputContents = @"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <class name=""Methods"" access=""public"" static=""true"">
      <constant name=""MyEnum1_Value1"" access=""public"">
        <type primitive=""True"">uint</type>
        <value>
          <code>1</code>
        </value>
      </constant>
      <constant name=""MyEnum2_Value1"" access=""public"">
        <type primitive=""True"">int</type>
        <value>
          <code>(int)(<value>MyEnum1_Value1</value>) + 1</code>
        </value>
      </constant>
    </class>
  </namespace>
</bindings>
";
        var diagnostics = new[] { new Diagnostic(DiagnosticLevel.Info, "Found anonymous enum: __AnonymousEnum_ClangUnsavedFile_L1_C1. Mapping values as constants in: Methods", "Line 1, Column 1 in ClangUnsavedFile.h") };
        return ValidateGeneratedXmlDefaultUnixBindingsAsync(inputContents, expectedOutputContents, expectedDiagnostics: diagnostics);
    }
}
