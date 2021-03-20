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

            var recordDecl = translationUnit.TranslationUnitDecl.Decls.OfType<RecordDecl>().Where((recordDecl) => recordDecl.Name == "MyStruct").Single();
            var accessSpecDecl = recordDecl.Decls.OfType<AccessSpecDecl>().Single();

            Assert.Equal(expectedAccessSpecifier, accessSpecDecl.Access);
        }

        [Fact]
        public void ClassTemplateDeclTest()
        {
            var inputContents = $@"template<class T>
class MyClass
{{
    T value;
}};
";

            using var translationUnit = CreateTranslationUnit(inputContents);

            var classTemplateDecl = translationUnit.TranslationUnitDecl.Decls.OfType<ClassTemplateDecl>().Single();
            Assert.Equal("MyClass", classTemplateDecl.Name);

            var templateParameter = classTemplateDecl.TemplateParameters.Single();
            Assert.Equal("T", templateParameter.Name);
        }

        [Fact]
        public void ClassTemplatePartialSpecializationDeclTest()
        {
            var inputContents = $@"template<class T, class U>
class MyClass
{{
    T value1;
    U value2;
}};

template<class U>
class MyClass<int, U>
{{
    int value1;
    U value2;
}};
";

            using var translationUnit = CreateTranslationUnit(inputContents);

            var classTemplatePartialSpecializationDecl = translationUnit.TranslationUnitDecl.Decls.OfType<ClassTemplatePartialSpecializationDecl>().Single();
            Assert.Equal("MyClass", classTemplatePartialSpecializationDecl.Name);

            var templateParameter = classTemplatePartialSpecializationDecl.TemplateParameters.Single();
            Assert.Equal("U", templateParameter.Name);
        }
    }
}
