// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System.Threading.Tasks;

namespace ClangSharp.UnitTests
{
    public sealed class XmlLatestWindows_FunctionPointerDeclarationTest : FunctionPointerDeclarationTest
    {
        public override Task BasicTest()
        {
            var inputContents = @"typedef void (*Callback)();

struct MyStruct {
    Callback _callback;
};
";

            var expectedOutputContents = @"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <struct name=""MyStruct"" access=""public"" unsafe=""true"">
      <field name=""_callback"" access=""public"">
        <type native=""Callback"">delegate* unmanaged[Cdecl]&lt;void&gt;</type>
      </field>
    </struct>
  </namespace>
</bindings>
";

            return ValidateGeneratedXmlLatestWindowsBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task CallconvTest()
        {
            var inputContents = @"typedef void (*Callback)() __attribute__((stdcall));

struct MyStruct {
    Callback _callback;
};
";

            var expectedOutputContents = @"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <struct name=""MyStruct"" access=""public"" unsafe=""true"">
      <field name=""_callback"" access=""public"">
        <type native=""Callback"">delegate* unmanaged[Stdcall]&lt;void&gt;</type>
      </field>
    </struct>
  </namespace>
</bindings>
";

            return ValidateGeneratedXmlLatestWindowsBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task PointerlessTypedefTest()
        {
            var inputContents = @"typedef void (Callback)();

struct MyStruct {
    Callback* _callback;
};
";

            var expectedOutputContents = @"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <struct name=""MyStruct"" access=""public"" unsafe=""true"">
      <field name=""_callback"" access=""public"">
        <type native=""Callback *"">delegate* unmanaged[Cdecl]&lt;void&gt;</type>
      </field>
    </struct>
  </namespace>
</bindings>
";

            return ValidateGeneratedXmlLatestWindowsBindingsAsync(inputContents, expectedOutputContents);
        }
    }
}
