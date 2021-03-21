// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System.Threading.Tasks;

namespace ClangSharp.UnitTests
{
    public sealed class XmlLatestUnix_FunctionPointerDeclarationTest : FunctionPointerDeclarationTest
    {
        public override Task BasicTest()
        {
            var inputContents = @"typedef void (*Callback)();";

            var expectedOutputContents = @"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <delegate name=""Callback"" access=""public"" convention=""Cdecl"" static=""true"">
      <type>void</type>
    </delegate>
  </namespace>
</bindings>
";

            return ValidateGeneratedXmlLatestUnixBindingsAsync(inputContents, expectedOutputContents);
        }
    }
}
