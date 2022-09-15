// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace ClangSharp.UnitTests;

public sealed class XmlCompatibleWindows_FunctionDeclarationDllImportTest : FunctionDeclarationDllImportTest
{
    protected override Task BasicTestImpl()
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

        return ValidateGeneratedXmlCompatibleWindowsBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task ArrayParameterTestImpl()
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

        return ValidateGeneratedXmlCompatibleWindowsBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task FunctionPointerParameterTestImpl()
    {
        var inputContents = @"extern ""C"" void MyFunction(void (*callback)());";

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

    protected override Task NamespaceTestImpl()
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

        return ValidateGeneratedXmlCompatibleWindowsBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task TemplateParameterTestImpl(string nativeType, bool expectedNativeTypeAttr, string expectedManagedType, string expectedUsingStatement)
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

        return ValidateGeneratedXmlCompatibleWindowsBindingsAsync(inputContents, expectedOutputContents, excludedNames: new[] { "MyTemplate" });
    }

    protected override Task TemplateMemberTestImpl()
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

    protected override Task NoLibraryPathTestImpl()
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

        return ValidateGeneratedXmlCompatibleWindowsBindingsAsync(inputContents, expectedOutputContents, libraryPath: string.Empty);
    }

    protected override Task WithLibraryPathTestImpl()
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
        return ValidateGeneratedXmlCompatibleWindowsBindingsAsync(inputContents, expectedOutputContents, libraryPath: string.Empty, withLibraryPaths: withLibraryPaths);
    }

    protected override Task WithLibraryPathStarTestImpl()
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
        return ValidateGeneratedXmlCompatibleWindowsBindingsAsync(inputContents, expectedOutputContents, libraryPath: string.Empty, withLibraryPaths: withLibraryPaths);
    }

    protected override Task OptionalParameterTestImpl(string nativeType, string nativeInit, bool expectedNativeTypeNameAttr, string expectedManagedType, string expectedManagedInit)
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

        return ValidateGeneratedXmlCompatibleWindowsBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task OptionalParameterUnsafeTestImpl(string nativeType, string nativeInit, string expectedManagedType, string expectedManagedInit)
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

        return ValidateGeneratedXmlCompatibleWindowsBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task WithCallConvTestImpl()
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
        return ValidateGeneratedXmlCompatibleWindowsBindingsAsync(inputContents, expectedOutputContents, withCallConvs: withCallConvs);
    }

    protected override Task WithCallConvStarTestImpl()
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
        return ValidateGeneratedXmlCompatibleWindowsBindingsAsync(inputContents, expectedOutputContents, withCallConvs: withCallConvs);
    }

    protected override Task WithCallConvStarOverrideTestImpl()
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
        return ValidateGeneratedXmlCompatibleWindowsBindingsAsync(inputContents, expectedOutputContents, withCallConvs: withCallConvs);
    }

    protected override Task WithSetLastErrorTestImpl()
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
        return ValidateGeneratedXmlCompatibleWindowsBindingsAsync(inputContents, expectedOutputContents, withSetLastErrors: withSetLastErrors);
    }

    protected override Task WithSetLastErrorStarTestImpl()
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
        return ValidateGeneratedXmlCompatibleWindowsBindingsAsync(inputContents, expectedOutputContents, withSetLastErrors: withSetLastErrors);
    }

    protected override Task SourceLocationTestImpl()
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

        return ValidateGeneratedXmlCompatibleWindowsBindingsAsync(InputContents, ExpectedOutputContents, PInvokeGeneratorConfigurationOptions.GenerateSourceLocationAttribute);
    }

    protected override Task VarargsTestImpl() => Task.CompletedTask;
}
