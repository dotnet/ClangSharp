// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClangSharp.UnitTests
{
    public sealed class XmlLatestWindows_VarDeclarationTest : VarDeclarationTest
    {
        public override Task BasicTest(string nativeType, string expectedManagedType)
        {
            var inputContents = $@"{nativeType} MyVariable = 0;";

            var expectedOutputContents = $@"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <class name=""Methods"" access=""public"" static=""true"">
      <constant name=""MyVariable"" access=""public"">
        <type primitive=""False"">{expectedManagedType}</type>
        <value>
          <code>0</code>
        </value>
      </constant>
    </class>
  </namespace>
</bindings>
";

            return ValidateGeneratedXmlLatestWindowsBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task BasicWithNativeTypeNameTest(string nativeType, string expectedManagedType)
        {
            var inputContents = $@"{nativeType} MyVariable = 0;";

            var expectedOutputContents = $@"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <class name=""Methods"" access=""public"" static=""true"">
      <constant name=""MyVariable"" access=""public"">
        <type primitive=""False"">{expectedManagedType}</type>
        <value>
          <code>0</code>
        </value>
      </constant>
    </class>
  </namespace>
</bindings>
";

            return ValidateGeneratedXmlLatestWindowsBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task GuidMacroTest()
        {
            var inputContents = $@"struct GUID {{
    unsigned long  Data1;
    unsigned short Data2;
    unsigned short Data3;
    unsigned char  Data4[8];
}};

const GUID IID_IUnknown = {{ 0x00000000, 0x0000, 0x0000, {{ 0xC0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x46 }} }};
";

            var expectedOutputContents = $@"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <class name=""Methods"" access=""public"" static=""true"">
      <constant name=""IID_IUnknown"" access=""public"">
        <type primitive=""False"">Guid</type>
        <value>
          <code>new Guid(0x00000000, 0x0000, 0x0000, 0xC0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x46)</code>
        </value>
      </constant>
    </class>
  </namespace>
</bindings>
";
            var excludedNames = new string[] { "GUID" };
            var remappedNames = new Dictionary<string, string> { ["GUID"] = "Guid" };

            return ValidateGeneratedXmlLatestWindowsBindingsAsync(inputContents, expectedOutputContents, excludedNames: excludedNames, remappedNames: remappedNames);
        }

        public override Task MacroTest(string nativeValue, string expectedManagedType, string expectedManagedValue)
        {
            var inputContents = $@"#define MyMacro1 {nativeValue}
#define MyMacro2 MyMacro1";

            var expectedOutputContents = $@"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <class name=""Methods"" access=""public"" static=""true"">
      <constant name=""MyMacro1"" access=""public"">
        <type primitive=""True"">{expectedManagedType}</type>
        <value>
          <code>{expectedManagedValue}</code>
        </value>
      </constant>
      <constant name=""MyMacro2"" access=""public"">
        <type primitive=""True"">{expectedManagedType}</type>
        <value>
          <code>{expectedManagedValue}</code>
        </value>
      </constant>
    </class>
  </namespace>
</bindings>
";

            return ValidateGeneratedXmlLatestWindowsBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task MultilineMacroTest()
        {
            var inputContents = $@"#define MyMacro1 0 + \
1";

            var expectedOutputContents = $@"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <class name=""Methods"" access=""public"" static=""true"">
      <constant name=""MyMacro1"" access=""public"">
        <type primitive=""True"">int</type>
        <value>
          <code>0 + 1</code>
        </value>
      </constant>
    </class>
  </namespace>
</bindings>
";

            return ValidateGeneratedXmlLatestWindowsBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task NoInitializerTest(string nativeType)
        {
            var inputContents = $@"{nativeType} MyVariable;";
            var expectedOutputContents = "";
            return ValidateGeneratedXmlLatestWindowsBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task Utf8StringLiteralMacroTest()
        {
            var inputContents = $@"#define MyMacro1 ""Test""";

            var expectedOutputContents = $@"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <class name=""Methods"" access=""public"" static=""true"">
      <constant name=""MyMacro1"" access=""public"">
        <type primitive=""False"">ReadOnlySpan&lt;byte&gt;</type>
        <value>
          <code>new byte[] {{ 0x54, 0x65, 0x73, 0x74, 0x00 }}</code>
        </value>
      </constant>
    </class>
  </namespace>
</bindings>
";

            return ValidateGeneratedXmlLatestWindowsBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task Utf16StringLiteralMacroTest()
        {
            var inputContents = $@"#define MyMacro1 u""Test""";

            var expectedOutputContents = $@"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <class name=""Methods"" access=""public"" static=""true"">
      <constant name=""MyMacro1"" access=""public"">
        <type primitive=""True"">string</type>
        <value>
          <code>""Test""</code>
        </value>
      </constant>
    </class>
  </namespace>
</bindings>
";

            return ValidateGeneratedXmlLatestWindowsBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task WideStringLiteralConstTest()
        {
            var inputContents = $@"const wchar_t MyConst1[] = L""Test"";";

            var expectedOutputContents = $@"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <class name=""Methods"" access=""public"" static=""true"">
      <constant name=""MyConst1"" access=""public"">
        <type primitive=""True"">string</type>
        <value>
          <code>""Test""</code>
        </value>
      </constant>
    </class>
  </namespace>
</bindings>
";

            return ValidateGeneratedXmlLatestWindowsBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task StringLiteralConstTest()
        {
            var inputContents = $@"const char MyConst1[] = ""Test"";";

            var expectedOutputContents = $@"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <class name=""Methods"" access=""public"" static=""true"">
      <constant name=""MyConst1"" access=""public"">
        <type primitive=""False"">ReadOnlySpan&lt;byte&gt;</type>
        <value>
          <code>new byte[] {{ 0x54, 0x65, 0x73, 0x74, 0x00 }}</code>
        </value>
      </constant>
    </class>
  </namespace>
</bindings>
";

            return ValidateGeneratedXmlLatestWindowsBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task UncheckedConversionMacroTest()
        {
            var inputContents = $@"#define MyMacro1 (long)0x80000000L
#define MyMacro2 (int)0x80000000";

            var expectedOutputContents = $@"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <class name=""Methods"" access=""public"" static=""true"">
      <constant name=""MyMacro1"" access=""public"">
        <type primitive=""True"">int</type>
        <value>
          <unchecked>
            <value>
              <code>(int)(<value>0x80000000</value>)</code>
            </value>
          </unchecked>
        </value>
      </constant>
      <constant name=""MyMacro2"" access=""public"">
        <type primitive=""True"">int</type>
        <value>
          <unchecked>
            <value>
              <code>(int)(<value>0x80000000</value>)</code>
            </value>
          </unchecked>
        </value>
      </constant>
    </class>
  </namespace>
</bindings>
";

            return ValidateGeneratedXmlLatestWindowsBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task UncheckedFunctionLikeCastMacroTest()
        {
            var inputContents = $@"#define MyMacro1 unsigned(-1)";

            var expectedOutputContents = $@"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <class name=""Methods"" access=""public"" static=""true"">
      <constant name=""MyMacro1"" access=""public"">
        <type primitive=""True"">uint</type>
        <value>
          <unchecked>
            <value>
              <code>(uint)(<value>-1</value>)</code>
            </value>
          </unchecked>
        </value>
      </constant>
    </class>
  </namespace>
</bindings>
";

            return ValidateGeneratedXmlLatestWindowsBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task UncheckedConversionMacroTest2()
        {
            var inputContents = $@"#define MyMacro1(x, y, z) ((int)(((unsigned long)(x)<<31) | ((unsigned long)(y)<<16) | ((unsigned long)(z))))
#define MyMacro2(n) MyMacro1(1, 2, n)
#define MyMacro3 MyMacro2(3)";

            var expectedOutputContents = $@"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <class name=""Methods"" access=""public"" static=""true"">
      <constant name=""MyMacro3"" access=""public"">
        <type primitive=""True"">int</type>
        <value>
          <unchecked>
            <code>((int)(((uint)(1) &lt;&lt; 31) | ((uint)(2) &lt;&lt; 16) | ((uint)(3))))</code>
          </unchecked>
        </value>
      </constant>
    </class>
  </namespace>
</bindings>
";

            var excludedNames = new string[] { "MyMacro1", "MyMacro2" };
            return ValidateGeneratedXmlLatestWindowsBindingsAsync(inputContents, expectedOutputContents, excludedNames: excludedNames);
        }

        public override Task UncheckedPointerMacroTest()
        {
            var inputContents = $@"#define Macro1 ((int*) -1)";

            var expectedOutputContents = $@"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <class name=""Methods"" access=""public"" static=""true"" unsafe=""true"">
      <constant name=""Macro1"" access=""public"">
        <type primitive=""False"">int*</type>
        <value>
          <unchecked>
            <code>((int*)(<value>-1</value>))</code>
          </unchecked>
        </value>
      </constant>
    </class>
  </namespace>
</bindings>
";

            return ValidateGeneratedXmlLatestWindowsBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task UncheckedReinterpretCastMacroTest()
        {
            var inputContents = $@"#define Macro1 reinterpret_cast<int*>(-1)";

            var expectedOutputContents = $@"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <class name=""Methods"" access=""public"" static=""true"" unsafe=""true"">
      <constant name=""Macro1"" access=""public"">
        <type primitive=""False"">int*</type>
        <value>
          <unchecked>
            <value>
              <code>(int*)(<value>-1</value>)</code>
            </value>
          </unchecked>
        </value>
      </constant>
    </class>
  </namespace>
</bindings>
";

            return ValidateGeneratedXmlLatestWindowsBindingsAsync(inputContents, expectedOutputContents);
        }
    }
}
