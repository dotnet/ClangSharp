// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Linq;
using NUnit.Framework;

namespace ClangSharp.UnitTests;

public sealed class TypePredicateTest : TranslationUnitTest
{
    // Objective-C lightweight generics and __kindof require a modern runtime, so an Apple target triple
    // is pinned to keep the parse deterministic cross-platform.
    private static readonly string[] s_objCCommandLineArgs =
    [
        "-target",
        "x86_64-apple-macosx10.15.0",
        "-fobjc-runtime=macosx",
    ];

    [Test]
    public void TemplateTypeParmTypeIsParameterPackTest()
    {
        var inputContents = """
template <typename T, typename... Ts>
struct S
{
    T first;
    void rest(Ts... args);
};
""";

        using var translationUnit = CreateTranslationUnit(inputContents);

        var classTemplate = translationUnit.TranslationUnitDecl.Decls.OfType<ClassTemplateDecl>().Single();
        var record = (CXXRecordDecl)classTemplate.TemplatedDecl;

        var first = record.Decls.OfType<FieldDecl>().Single();
        var firstType = (TemplateTypeParmType)first.Type;
        Assert.That(firstType.IsParameterPack, Is.False);

        var rest = record.Decls.OfType<CXXMethodDecl>().Single((method) => method.Name.Equals("rest", StringComparison.Ordinal));
        var packExpansion = (PackExpansionType)rest.Parameters.Single().Type;
        var packParm = (TemplateTypeParmType)packExpansion.Pattern;
        Assert.That(packParm.IsParameterPack, Is.True);
    }

    [Test]
    public void ObjCTypePredicatesTest()
    {
        var inputContents = """
__attribute__((objc_root_class))
@interface Base
@end

@protocol Flying
@end

@interface Bird : Base <Flying>
@end

@interface Cage<T> : Base
- (T)occupant;
@end

@interface Keeper<U> : Base
- (U<Flying>)charge;
@end

@interface Api : Base
- (id)anyObject;
- (Class)anyClass;
- (id<Flying>)flyer;
- (Class<Flying>)flyerClass;
- (__kindof Bird *)someBird;
- (Cage<Bird *> *)birdCage;
@end
""";

        using var translationUnit = CreateTranslationUnit(inputContents, "objective-c", s_objCCommandLineArgs);

        var api = translationUnit.TranslationUnitDecl.CursorChildren.OfType<ObjCInterfaceDecl>().Single((decl) => decl.Name.Equals("Api", StringComparison.Ordinal));

        Type Return(string selector)
        {
            return api.CursorChildren.OfType<ObjCMethodDecl>().Single((method) => method.Selector.Equals(selector, StringComparison.Ordinal)).ReturnType;
        }

        Assert.That(Return("anyObject").IsObjCIdType, Is.True);
        Assert.That(Return("anyClass").IsObjCClassType, Is.True);
        Assert.That(Return("flyer").IsObjCQualifiedIdType, Is.True);
        Assert.That(Return("flyerClass").IsObjCQualifiedClassType, Is.True);

        var someBird = (ObjCObjectPointerType)Return("someBird").CanonicalType;
        Assert.That(someBird.IsKindOfType, Is.True);
        Assert.That(someBird.ObjectType.IsKindOfType, Is.True);
        Assert.That(someBird.ObjectType.IsKindOfTypeAsWritten, Is.True);

        var birdCage = (ObjCObjectPointerType)Return("birdCage").CanonicalType;
        Assert.That(birdCage.IsSpecialized, Is.True);
        Assert.That(birdCage.ObjectType.IsSpecialized, Is.True);
        Assert.That(birdCage.ObjectType.IsSpecializedAsWritten, Is.True);

        var keeper = translationUnit.TranslationUnitDecl.CursorChildren.OfType<ObjCInterfaceDecl>().Single((decl) => decl.Name.Equals("Keeper", StringComparison.Ordinal));
        var charge = (ObjCTypeParamType)keeper.CursorChildren.OfType<ObjCMethodDecl>().Single((method) => method.Selector.Equals("charge", StringComparison.Ordinal)).ReturnType;
        Assert.That(charge.Protocols, Has.Count.EqualTo(1));
        Assert.That(charge.Protocols[0].Name, Is.EqualTo("Flying"));
    }
}
