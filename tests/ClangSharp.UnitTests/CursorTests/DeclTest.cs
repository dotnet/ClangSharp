// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System.Linq;
using ClangSharp.Interop;
using Xunit;

namespace ClangSharp.UnitTests
{
    public sealed class DeclTest : TranslationUnitTest
    {
        [Theory]
        [InlineData("private", CX_CXXAccessSpecifier.CX_CXXPrivate)]
        [InlineData("protected", CX_CXXAccessSpecifier.CX_CXXProtected)]
        [InlineData("public", CX_CXXAccessSpecifier.CX_CXXPublic)]
        public void AccessSpecDeclTest(string accessSpecifier, CX_CXXAccessSpecifier expectedAccessSpecifier)
        {
            var inputContents = $@"struct MyStruct
{{
{accessSpecifier}:
}};
";

            using var translationUnit = CreateTranslationUnit(inputContents);

            var recordDecl = translationUnit.TranslationUnitDecl.Decls.OfType<RecordDecl>().Single();
            var accessSpecDecl = recordDecl.Decls.OfType<AccessSpecDecl>().Single();

            Assert.Equal(expectedAccessSpecifier, accessSpecDecl.Access);
        }
    }
}
