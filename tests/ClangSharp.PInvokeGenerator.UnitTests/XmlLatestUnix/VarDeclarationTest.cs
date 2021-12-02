// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClangSharp.UnitTests
{
    public sealed class XmlLatestUnix_VarDeclarationTest : VarDeclarationTest
    {
        protected override Task BasicTestImpl(string nativeType, string expectedManagedType)
        {
            var inputContents = $@"{nativeType} MyVariable = 0;";

            var expectedOutputContents = $@"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <class name=""Methods"" access=""public"" static=""true"">
      <constant name=""MyVariable"" access=""public"">
        <type primitive=""True"">{expectedManagedType}</type>
        <value>
          <code>0</code>
        </value>
      </constant>
    </class>
  </namespace>
</bindings>
";

            return ValidateGeneratedXmlLatestUnixBindingsAsync(inputContents, expectedOutputContents);
        }

        protected override Task BasicWithNativeTypeNameTestImpl(string nativeType, string expectedManagedType)
        {
            var inputContents = $@"{nativeType} MyVariable = 0;";

            var expectedOutputContents = $@"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <class name=""Methods"" access=""public"" static=""true"">
      <constant name=""MyVariable"" access=""public"">
        <type primitive=""True"">{expectedManagedType}</type>
        <value>
          <code>0</code>
        </value>
      </constant>
    </class>
  </namespace>
</bindings>
";

            return ValidateGeneratedXmlLatestUnixBindingsAsync(inputContents, expectedOutputContents);
        }

        protected override Task GuidMacroTestImpl()
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

            return ValidateGeneratedXmlLatestUnixBindingsAsync(inputContents, expectedOutputContents, excludedNames: excludedNames, remappedNames: remappedNames);
        }

        protected override Task MacroTestImpl(string nativeValue, string expectedManagedType, string expectedManagedValue)
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

            return ValidateGeneratedXmlLatestUnixBindingsAsync(inputContents, expectedOutputContents);
        }

        protected override Task MultilineMacroTestImpl()
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

            return ValidateGeneratedXmlLatestUnixBindingsAsync(inputContents, expectedOutputContents);
        }

        protected override Task NoInitializerTestImpl(string nativeType)
        {
            var inputContents = $@"{nativeType} MyVariable;";
            var expectedOutputContents = "";
            return ValidateGeneratedXmlLatestUnixBindingsAsync(inputContents, expectedOutputContents);
        }

        protected override Task Utf8StringLiteralMacroTestImpl()
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

            return ValidateGeneratedXmlLatestUnixBindingsAsync(inputContents, expectedOutputContents);
        }

        protected override Task Utf16StringLiteralMacroTestImpl()
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

            return ValidateGeneratedXmlLatestUnixBindingsAsync(inputContents, expectedOutputContents);
        }

        protected override Task WideStringLiteralConstTestImpl() =>
            // Unsupported string literal kind: 'CX_CLK_Wide'
            Task.CompletedTask;

        protected override Task StringLiteralConstTestImpl()
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

            return ValidateGeneratedXmlLatestUnixBindingsAsync(inputContents, expectedOutputContents);
        }

        protected override Task UncheckedConversionMacroTestImpl()
        {
            var inputContents = $@"#define MyMacro1 (long)0x80000000L
#define MyMacro2 (int)0x80000000";

            var expectedOutputContents = $@"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <class name=""Methods"" access=""public"" static=""true"">
      <constant name=""MyMacro1"" access=""public"">
        <type primitive=""True"">nint</type>
        <value>
          <unchecked>
            <value>
              <code>(nint)(<value>0x80000000</value>)</code>
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

            return ValidateGeneratedXmlLatestUnixBindingsAsync(inputContents, expectedOutputContents);
        }

        protected override Task UncheckedFunctionLikeCastMacroTestImpl()
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

            return ValidateGeneratedXmlLatestUnixBindingsAsync(inputContents, expectedOutputContents);
        }

        protected override Task UncheckedConversionMacroTest2Impl()
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
            <code>((int)(((nuint)(1) &lt;&lt; 31) | ((nuint)(2) &lt;&lt; 16) | ((nuint)(3))))</code>
          </unchecked>
        </value>
      </constant>
    </class>
  </namespace>
</bindings>
";

            var excludedNames = new string[] { "MyMacro1", "MyMacro2" };
            return ValidateGeneratedXmlLatestUnixBindingsAsync(inputContents, expectedOutputContents, excludedNames: excludedNames);
        }

        protected override Task UncheckedPointerMacroTestImpl()
        {
            var inputContents = $@"#define Macro1 ((int*) -1)";

            var expectedOutputContents = $@"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <class name=""Methods"" access=""public"" static=""true"" unsafe=""true"">
      <constant name=""Macro1"" access=""public"">
        <type primitive=""True"">int*</type>
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

            return ValidateGeneratedXmlLatestUnixBindingsAsync(inputContents, expectedOutputContents);
        }

        protected override Task UncheckedReinterpretCastMacroTestImpl()
        {
            var inputContents = $@"#define Macro1 reinterpret_cast<int*>(-1)";

            var expectedOutputContents = $@"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <class name=""Methods"" access=""public"" static=""true"" unsafe=""true"">
      <constant name=""Macro1"" access=""public"">
        <type primitive=""True"">int*</type>
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

            return ValidateGeneratedXmlLatestUnixBindingsAsync(inputContents, expectedOutputContents);
        }

        protected override Task MultidimensionlArrayTestImpl()
        {
            var inputContents = $@"const int MyArray[2][2] = {{ {{ 0, 1 }}, {{ 2, 3 }} }};";

            var expectedOutputContents = $@"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <class name=""Methods"" access=""public"" static=""true"">
      <constant name=""MyArray"" access=""public"">
        <type primitive=""False"">int[][]</type>
        <value>
          <code>new int[2][]
      {{
          new int[2]
          {{
              0,
              1,
          }},
          new int[2]
          {{
              2,
              3,
          }},
      }}</code>
        </value>
      </constant>
    </class>
  </namespace>
</bindings>
";

            return ValidateGeneratedXmlLatestUnixBindingsAsync(inputContents, expectedOutputContents);
        }

        protected override Task ConditionalDefineConstTestImpl()
        {
            var inputContents = @"typedef int TESTRESULT;
#define TESTRESULT_FROM_WIN32(x) ((TESTRESULT)(x) <= 0 ? ((TESTRESULT)(x)) : ((TESTRESULT) (((x) & 0x0000FFFF) | (7 << 16) | 0x80000000)))
#define ADDRESS_IN_USE TESTRESULT_FROM_WIN32(10048)";

            var expectedOutputContents = $@"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <class name=""Methods"" access=""public"" static=""true"">
      <constant name=""ADDRESS_IN_USE"" access=""public"">
        <type primitive=""True"">int</type>
        <value>
          <unchecked>
            <code>((int)(10048) &lt;= 0 ? ((int)(10048)) : ((int)(((10048) &amp; 0x0000FFFF) | (7 &lt;&lt; 16) | 0x80000000)))</code>
          </unchecked>
        </value>
      </constant>
    </class>
  </namespace>
</bindings>
";
            var diagnostics = new Diagnostic[] { new Diagnostic(DiagnosticLevel.Warning, "Function like macro definition records are not supported: 'TESTRESULT_FROM_WIN32'. Generated bindings may be incomplete.", "Line 2, Column 9 in ClangUnsavedFile.h") };

            return ValidateGeneratedXmlLatestUnixBindingsAsync(inputContents, expectedOutputContents, expectedDiagnostics: diagnostics);
        }
    }
}
