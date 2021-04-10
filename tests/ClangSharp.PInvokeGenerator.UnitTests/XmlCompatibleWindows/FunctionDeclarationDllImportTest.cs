// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClangSharp.UnitTests
{
    public sealed class XmlCompatibleWindows_FunctionDeclarationDllImportTest : FunctionDeclarationDllImportTest
    {
        public override Task BasicTest()
        {
            var inputContents = @"void MyFunction();";

            var expectedOutputContents = @"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <class name=""Methods"" access=""public"" static=""true"">
      <function name=""MyFunction"" access=""public"" lib=""ClangSharpPInvokeGenerator"" convention=""Cdecl"" static=""true"">
        <type>void</type>
      </function>
    </class>
  </namespace>
</bindings>
";

            return ValidateGeneratedXmlCompatibleWindowsBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task ArrayParameterTest()
        {
            var inputContents = @"void MyFunction(const float color[4]);";

            var expectedOutputContents = @"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <class name=""Methods"" access=""public"" static=""true"">
      <function name=""MyFunction"" access=""public"" lib=""ClangSharpPInvokeGenerator"" convention=""Cdecl"" static=""true"" unsafe=""true"">
        <type>void</type>
        <param name=""color"">
          <type>float*</type>
        </param>
      </function>
    </class>
  </namespace>
</bindings>
";

            return ValidateGeneratedXmlCompatibleWindowsBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task FunctionPointerParameterTest()
        {
            var inputContents = @"void MyFunction(void (*callback)());";

            var expectedOutputContents = @"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <class name=""Methods"" access=""public"" static=""true"">
      <function name=""MyFunction"" access=""public"" lib=""ClangSharpPInvokeGenerator"" convention=""Cdecl"" static=""true"">
        <type>void</type>
        <param name=""callback"">
          <type>IntPtr</type>
        </param>
      </function>
    </class>
  </namespace>
</bindings>
";

            return ValidateGeneratedXmlCompatibleWindowsBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task TemplateParameterTest(string nativeType, bool expectedNativeTypeAttr, string expectedManagedType, string expectedUsingStatement)
        {
            var inputContents = @$"template <typename T> struct MyTemplate;

void MyFunction(MyTemplate<{nativeType}> myStruct);";

            var expectedOutputContents = $@"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <class name=""Methods"" access=""public"" static=""true"">
      <function name=""MyFunction"" access=""public"" lib=""ClangSharpPInvokeGenerator"" convention=""Cdecl"" static=""true"">
        <type>void</type>
        <param name=""myStruct"">
          <type>MyTemplate&lt;{expectedManagedType}&gt;</type>
        </param>
      </function>
    </class>
  </namespace>
</bindings>
";

            return ValidateGeneratedXmlCompatibleWindowsBindingsAsync(inputContents, expectedOutputContents, excludedNames: new[] { "MyTemplate" });
        }

        public override Task TemplateMemberTest()
        {
            var inputContents = @$"template <typename T> struct MyTemplate
{{
}};

struct MyStruct
{{
    MyTemplate<float*> a;
}};
";

            var expectedOutputContents = $@"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <struct name=""MyStruct"" access=""public"">
      <field name=""a"" access=""public"">
        <type native=""MyTemplate&lt;float *&gt;"">MyTemplate&lt;IntPtr&gt;</type>
      </field>
    </struct>
  </namespace>
</bindings>
";

            return ValidateGeneratedXmlCompatibleWindowsBindingsAsync(inputContents, expectedOutputContents, excludedNames: new[] { "MyTemplate" });
        }

        public override Task NoLibraryPathTest()
        {
            var inputContents = @"void MyFunction();";

            var expectedOutputContents = @"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <class name=""Methods"" access=""public"" static=""true"">
      <function name=""MyFunction"" access=""public"" lib="""" convention=""Cdecl"" static=""true"">
        <type>void</type>
      </function>
    </class>
  </namespace>
</bindings>
";

            return ValidateGeneratedXmlCompatibleWindowsBindingsAsync(inputContents, expectedOutputContents, libraryPath: string.Empty);
        }

        public override Task WithLibraryPathTest()
        {
            var inputContents = @"void MyFunction();";

            var expectedOutputContents = @"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <class name=""Methods"" access=""public"" static=""true"">
      <function name=""MyFunction"" access=""public"" lib=""ClangSharpPInvokeGenerator"" convention=""Cdecl"" static=""true"">
        <type>void</type>
      </function>
    </class>
  </namespace>
</bindings>
";

            var withLibraryPaths = new Dictionary<string, string>
            {
                ["MyFunction"] = "ClangSharpPInvokeGenerator"
            };
            return ValidateGeneratedXmlCompatibleWindowsBindingsAsync(inputContents, expectedOutputContents, libraryPath: string.Empty, withLibraryPaths: withLibraryPaths);
        }

        public override Task WithLibraryPathStarTest()
        {
            var inputContents = @"void MyFunction();";

            var expectedOutputContents = @"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <class name=""Methods"" access=""public"" static=""true"">
      <function name=""MyFunction"" access=""public"" lib=""ClangSharpPInvokeGenerator"" convention=""Cdecl"" static=""true"">
        <type>void</type>
      </function>
    </class>
  </namespace>
</bindings>
";

            var withLibraryPaths = new Dictionary<string, string>
            {
                ["*"] = "ClangSharpPInvokeGenerator"
            };
            return ValidateGeneratedXmlCompatibleWindowsBindingsAsync(inputContents, expectedOutputContents, libraryPath: string.Empty, withLibraryPaths: withLibraryPaths);
        }

        public override Task OptionalParameterTest(string nativeType, string nativeInit, bool expectedNativeTypeNameAttr, string expectedManagedType, string expectedManagedInit)
        {
            var inputContents = $@"void MyFunction({nativeType} value = {nativeInit});";

            var expectedOutputContents = $@"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <class name=""Methods"" access=""public"" static=""true"">
      <function name=""MyFunction"" access=""public"" lib=""ClangSharpPInvokeGenerator"" convention=""Cdecl"" static=""true"">
        <type>void</type>
        <param name=""value"">
          <type>{expectedManagedType}</type>
          <init>
            <code>{expectedManagedInit}</code>
          </init>
        </param>
      </function>
    </class>
  </namespace>
</bindings>
";

            return ValidateGeneratedXmlCompatibleWindowsBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task OptionalParameterUnsafeTest(string nativeType, string nativeInit, string expectedManagedType, string expectedManagedInit)
        {
            var inputContents = $@"void MyFunction({nativeType} value = {nativeInit});";

            var expectedOutputContents = $@"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <class name=""Methods"" access=""public"" static=""true"">
      <function name=""MyFunction"" access=""public"" lib=""ClangSharpPInvokeGenerator"" convention=""Cdecl"" static=""true"" unsafe=""true"">
        <type>void</type>
        <param name=""value"">
          <type>{expectedManagedType}</type>
          <init>
            <code>{expectedManagedInit}</code>
          </init>
        </param>
      </function>
    </class>
  </namespace>
</bindings>
";

            return ValidateGeneratedXmlCompatibleWindowsBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task WithCallConvTest()
        {
            var inputContents = @"void MyFunction1(int value); void MyFunction2(int value);";

            var expectedOutputContents = @"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <class name=""Methods"" access=""public"" static=""true"">
      <function name=""MyFunction1"" access=""public"" lib=""ClangSharpPInvokeGenerator"" static=""true"">
        <type>void</type>
        <param name=""value"">
          <type>int</type>
        </param>
      </function>
      <function name=""MyFunction2"" access=""public"" lib=""ClangSharpPInvokeGenerator"" convention=""Cdecl"" static=""true"">
        <type>void</type>
        <param name=""value"">
          <type>int</type>
        </param>
      </function>
    </class>
  </namespace>
</bindings>
";

            var withCallConvs = new Dictionary<string, string> {
                ["MyFunction1"] = "Winapi"
            };
            return ValidateGeneratedXmlCompatibleWindowsBindingsAsync(inputContents, expectedOutputContents, withCallConvs: withCallConvs);
        }

        public override Task WithCallConvStarTest()
        {
            var inputContents = @"void MyFunction1(int value); void MyFunction2(int value);";

            var expectedOutputContents = @"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <class name=""Methods"" access=""public"" static=""true"">
      <function name=""MyFunction1"" access=""public"" lib=""ClangSharpPInvokeGenerator"" static=""true"">
        <type>void</type>
        <param name=""value"">
          <type>int</type>
        </param>
      </function>
      <function name=""MyFunction2"" access=""public"" lib=""ClangSharpPInvokeGenerator"" static=""true"">
        <type>void</type>
        <param name=""value"">
          <type>int</type>
        </param>
      </function>
    </class>
  </namespace>
</bindings>
";

            var withCallConvs = new Dictionary<string, string>
            {
                ["*"] = "Winapi"
            };
            return ValidateGeneratedXmlCompatibleWindowsBindingsAsync(inputContents, expectedOutputContents, withCallConvs: withCallConvs);
        }

        public override Task WithCallConvStarOverrideTest()
        {
            var inputContents = @"void MyFunction1(int value); void MyFunction2(int value);";

            var expectedOutputContents = @"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <class name=""Methods"" access=""public"" static=""true"">
      <function name=""MyFunction1"" access=""public"" lib=""ClangSharpPInvokeGenerator"" static=""true"">
        <type>void</type>
        <param name=""value"">
          <type>int</type>
        </param>
      </function>
      <function name=""MyFunction2"" access=""public"" lib=""ClangSharpPInvokeGenerator"" convention=""StdCall"" static=""true"">
        <type>void</type>
        <param name=""value"">
          <type>int</type>
        </param>
      </function>
    </class>
  </namespace>
</bindings>
";

            var withCallConvs = new Dictionary<string, string>
            {
                ["*"] = "Winapi",
                ["MyFunction2"] = "StdCall"
            };
            return ValidateGeneratedXmlCompatibleWindowsBindingsAsync(inputContents, expectedOutputContents, withCallConvs: withCallConvs);
        }

        public override Task WithSetLastErrorTest()
        {
            var inputContents = @"void MyFunction1(int value); void MyFunction2(int value);";

            var expectedOutputContents = @"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <class name=""Methods"" access=""public"" static=""true"">
      <function name=""MyFunction1"" access=""public"" lib=""ClangSharpPInvokeGenerator"" convention=""Cdecl"" setlasterror=""true"" static=""true"">
        <type>void</type>
        <param name=""value"">
          <type>int</type>
        </param>
      </function>
      <function name=""MyFunction2"" access=""public"" lib=""ClangSharpPInvokeGenerator"" convention=""Cdecl"" static=""true"">
        <type>void</type>
        <param name=""value"">
          <type>int</type>
        </param>
      </function>
    </class>
  </namespace>
</bindings>
";

            var withSetLastErrors = new string[]
            {
                "MyFunction1"
            };
            return ValidateGeneratedXmlCompatibleWindowsBindingsAsync(inputContents, expectedOutputContents, withSetLastErrors: withSetLastErrors);
        }

        public override Task WithSetLastErrorStarTest()
        {
            var inputContents = @"void MyFunction1(int value); void MyFunction2(int value);";

            var expectedOutputContents = @"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <class name=""Methods"" access=""public"" static=""true"">
      <function name=""MyFunction1"" access=""public"" lib=""ClangSharpPInvokeGenerator"" convention=""Cdecl"" setlasterror=""true"" static=""true"">
        <type>void</type>
        <param name=""value"">
          <type>int</type>
        </param>
      </function>
      <function name=""MyFunction2"" access=""public"" lib=""ClangSharpPInvokeGenerator"" convention=""Cdecl"" setlasterror=""true"" static=""true"">
        <type>void</type>
        <param name=""value"">
          <type>int</type>
        </param>
      </function>
    </class>
  </namespace>
</bindings>
";

            var withSetLastErrors = new string[]
            {
                "*"
            };
            return ValidateGeneratedXmlCompatibleWindowsBindingsAsync(inputContents, expectedOutputContents, withSetLastErrors: withSetLastErrors);
        }

        public override Task SourceLocationTest()
        {
            const string InputContents = @"void MyFunction(float value);";

            const string ExpectedOutputContents = @"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <class name=""Methods"" access=""public"" static=""true"">
      <function name=""MyFunction"" access=""public"" lib=""ClangSharpPInvokeGenerator"" convention=""Cdecl"" static=""true"">
        <type>void</type>
        <param name=""value"">
          <type>float</type>
        </param>
      </function>
    </class>
  </namespace>
</bindings>
";

            return ValidateGeneratedXmlCompatibleWindowsBindingsAsync(InputContents, ExpectedOutputContents, PInvokeGeneratorConfigurationOptions.GenerateSourceLocationAttribute);
        }
    }
}
