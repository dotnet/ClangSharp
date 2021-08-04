// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace ClangSharp.UnitTests
{
    public sealed class XmlLatestUnix_FunctionDeclarationDllImportTest : FunctionDeclarationDllImportTest
    {
        public override Task BasicTest()
        {
            var inputContents = @"extern ""C"" void MyFunction();";

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

            return ValidateGeneratedXmlLatestUnixBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task ArrayParameterTest()
        {
            var inputContents = @"extern ""C"" void MyFunction(const float color[4]);";

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

            return ValidateGeneratedXmlLatestUnixBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task FunctionPointerParameterTest()
        {
            var inputContents = @"extern ""C"" void MyFunction(void (*callback)());";

            var expectedOutputContents = @"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <class name=""Methods"" access=""public"" static=""true"">
      <function name=""MyFunction"" access=""public"" lib=""ClangSharpPInvokeGenerator"" convention=""Cdecl"" static=""true"" unsafe=""true"">
        <type>void</type>
        <param name=""callback"">
          <type>delegate* unmanaged[Cdecl]&lt;void&gt;</type>
        </param>
      </function>
    </class>
  </namespace>
</bindings>
";

            return ValidateGeneratedXmlLatestUnixBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task NamespaceTest()
        {
            var inputContents = @"namespace MyNamespace
{
    void MyFunction();
}";

            var entryPoint = RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ? "__ZN11MyNamespace10MyFunctionEv" : "_ZN11MyNamespace10MyFunctionEv";

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                entryPoint = "?MyFunction@MyNamespace@@YAXXZ";
            }

            var expectedOutputContents = $@"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <class name=""Methods"" access=""public"" static=""true"">
      <function name=""MyFunction"" access=""public"" lib=""ClangSharpPInvokeGenerator"" convention=""Cdecl"" entrypoint=""{entryPoint}"" static=""true"">
        <type>void</type>
      </function>
    </class>
  </namespace>
</bindings>
";

            return ValidateGeneratedXmlLatestUnixBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task TemplateParameterTest(string nativeType, bool expectedNativeTypeAttr, string expectedManagedType, string expectedUsingStatement)
        {
            var inputContents = @$"template <typename T> struct MyTemplate;

extern ""C"" void MyFunction(MyTemplate<{nativeType}> myStruct);";

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

            return ValidateGeneratedXmlLatestUnixBindingsAsync(inputContents, expectedOutputContents, excludedNames: new[] { "MyTemplate" });
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

            return ValidateGeneratedXmlLatestUnixBindingsAsync(inputContents, expectedOutputContents, excludedNames: new[] { "MyTemplate" });
        }

        public override Task NoLibraryPathTest()
        {
            var inputContents = @"extern ""C"" void MyFunction();";

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

            return ValidateGeneratedXmlLatestUnixBindingsAsync(inputContents, expectedOutputContents, libraryPath: string.Empty);
        }

        public override Task WithLibraryPathTest()
        {
            var inputContents = @"extern ""C"" void MyFunction();";

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
            return ValidateGeneratedXmlLatestUnixBindingsAsync(inputContents, expectedOutputContents, libraryPath: string.Empty, withLibraryPaths: withLibraryPaths);
        }

        public override Task WithLibraryPathStarTest()
        {
            var inputContents = @"extern ""C"" void MyFunction();";

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
            return ValidateGeneratedXmlLatestUnixBindingsAsync(inputContents, expectedOutputContents, libraryPath: string.Empty, withLibraryPaths: withLibraryPaths);
        }

        public override Task OptionalParameterTest(string nativeType, string nativeInit, bool expectedNativeTypeNameAttr, string expectedManagedType, string expectedManagedInit)
        {
            var inputContents = $@"extern ""C"" void MyFunction({nativeType} value = {nativeInit});";

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

            return ValidateGeneratedXmlLatestUnixBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task OptionalParameterUnsafeTest(string nativeType, string nativeInit, string expectedManagedType, string expectedManagedInit)
        {
            var inputContents = $@"extern ""C"" void MyFunction({nativeType} value = {nativeInit});";

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

            return ValidateGeneratedXmlLatestUnixBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task WithCallConvTest()
        {
            var inputContents = @"extern ""C"" void MyFunction1(int value); extern ""C"" void MyFunction2(int value);";

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
            return ValidateGeneratedXmlLatestUnixBindingsAsync(inputContents, expectedOutputContents, withCallConvs: withCallConvs);
        }

        public override Task WithCallConvStarTest()
        {
            var inputContents = @"extern ""C"" void MyFunction1(int value); extern ""C"" void MyFunction2(int value);";

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
            return ValidateGeneratedXmlLatestUnixBindingsAsync(inputContents, expectedOutputContents, withCallConvs: withCallConvs);
        }

        public override Task WithCallConvStarOverrideTest()
        {
            var inputContents = @"extern ""C"" void MyFunction1(int value); extern ""C"" void MyFunction2(int value);";

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
            return ValidateGeneratedXmlLatestUnixBindingsAsync(inputContents, expectedOutputContents, withCallConvs: withCallConvs);
        }

        public override Task WithSetLastErrorTest()
        {
            var inputContents = @"extern ""C"" void MyFunction1(int value); extern ""C"" void MyFunction2(int value);";

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
            return ValidateGeneratedXmlLatestUnixBindingsAsync(inputContents, expectedOutputContents, withSetLastErrors: withSetLastErrors);
        }

        public override Task WithSetLastErrorStarTest()
        {
            var inputContents = @"extern ""C"" void MyFunction1(int value); extern ""C"" void MyFunction2(int value);";

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
            return ValidateGeneratedXmlLatestUnixBindingsAsync(inputContents, expectedOutputContents, withSetLastErrors: withSetLastErrors);
        }

        public override Task SourceLocationTest()
        {
            const string InputContents = @"extern ""C"" void MyFunction(float value);";

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

            return ValidateGeneratedXmlLatestUnixBindingsAsync(InputContents, ExpectedOutputContents, PInvokeGeneratorConfigurationOptions.GenerateSourceLocationAttribute);
        }
    }
}
