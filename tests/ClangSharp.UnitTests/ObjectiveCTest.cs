// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ClangSharp.Interop;
using NUnit.Framework;

namespace ClangSharp.UnitTests;

[Platform("macosx")]
public sealed class ObjectiveCTest : TranslationUnitTest
{
    [Test]
    public void Attribute_ObjCRuntimeName()
    {
        var inputContents = $$"""
__attribute__((objc_runtime_name("MyRenamedClass")))
@interface MyClass
@end

__attribute__((objc_runtime_name("MyRenamedProtocol")))
@protocol MyProtocol
@end
""";
        using var translationUnit = CreateTranslationUnit(inputContents, "objective-c++");

        var classes = translationUnit.TranslationUnitDecl.Decls.OfType<ObjCInterfaceDecl>().ToList();

        var myClass = classes.SingleOrDefault(v => v.Name == "MyClass")!;
        Assert.That(myClass, Is.Not.Null, "MyClass");
        var myClassAttrs = myClass.Attrs;
        Assert.That(myClassAttrs.Count, Is.EqualTo(1), "myClassAttrs.Count");
        var runtimeNameAttr = myClassAttrs[0];
        Assert.That(runtimeNameAttr.Kind, Is.EqualTo(CX_AttrKind.CX_AttrKind_ObjCRuntimeName), "myClass Attr Kind");
        Assert.That(runtimeNameAttr.ObjCRuntimeNameMetadataName, Is.EqualTo("MyRenamedClass"), "myClass Attr ObjCRuntimeNameMetadataName");

        var protocols = translationUnit.TranslationUnitDecl.Decls.OfType<ObjCProtocolDecl>().ToList();
        var myProtocol = protocols.SingleOrDefault(v => v.Name == "MyProtocol")!;
        Assert.That(myProtocol, Is.Not.Null, "MyProtocol");
        var myProtocolAttrs = myProtocol.Attrs;
        Assert.That(myProtocolAttrs.Count, Is.EqualTo(1), "myProtocolAttrs.Count");
        runtimeNameAttr = myProtocolAttrs[0];
        Assert.That(runtimeNameAttr.Kind, Is.EqualTo(CX_AttrKind.CX_AttrKind_ObjCRuntimeName), "myProtocol Attr Kind");
        Assert.That(runtimeNameAttr.ObjCRuntimeNameMetadataName, Is.EqualTo("MyRenamedProtocol"), "myProtocol Attr ObjCRuntimeNameMetadataName");
    }

    [Test]
    public void Type_IsObjCInstanceType()
    {
        var inputContents = $@"
@interface MyClass
    -(instancetype) instanceMethod;
    +(MyClass*) staticMethod;
@end
";

        using var translationUnit = CreateTranslationUnit(inputContents, "objective-c++");

        var classes = translationUnit.TranslationUnitDecl.Decls.OfType<ObjCInterfaceDecl>().ToList();
        Assert.That(classes.Count, Is.GreaterThanOrEqualTo(1), $"At least one class");
        var myClass = classes.SingleOrDefault(v => v.Name == "MyClass")!;
        Assert.That(myClass, Is.Not.Null, "MyClass");

        var methodInstanceMethod = myClass.Methods.SingleOrDefault(v => v.Name == "instanceMethod")!;
        Assert.That(methodInstanceMethod, Is.Not.Null, "methodInstanceMethod");
        Assert.That(methodInstanceMethod.ReturnType.IsObjCInstanceType, Is.True, "methodInstanceMethod.ReturnType.IsObjCInstanceType");

        var methodStaticMethod = myClass.Methods.SingleOrDefault(v => v.Name == "staticMethod")!;
        Assert.That(methodStaticMethod, Is.Not.Null, "methodStaticMethod");
        Assert.That(methodStaticMethod.ReturnType.IsObjCInstanceType, Is.False, "methodStaticMethod.ReturnType.IsObjCInstanceType");
    }

    [Test]
    public void Method_Selector()
    {
        var inputContents = $@"
@interface MyClass
    @property int P1;
    -(void) instanceMethod;
    +(void) staticMethod;
@end
";

        using var translationUnit = CreateTranslationUnit(inputContents, "objective-c++");

        var classes = translationUnit.TranslationUnitDecl.Decls.OfType<ObjCInterfaceDecl>().ToList();
        Assert.That(classes.Count, Is.GreaterThanOrEqualTo(1), $"At least one class");
        var myClass = classes.SingleOrDefault(v => v.Name == "MyClass")!;
        Assert.That(myClass, Is.Not.Null, "MyClass");

        var methodP1 = myClass.Methods.SingleOrDefault(v => v.Name == "P1")!;
        Assert.That(methodP1, Is.Not.Null, "methodP1");
        Assert.That(methodP1.Selector, Is.EqualTo("P1"), "methodP1.Selector");

        var methodSetP1 = myClass.Methods.SingleOrDefault(v => v.Name == "setP1:")!;
        Assert.That(methodSetP1, Is.Not.Null, "methodSetP1");
        Assert.That(methodSetP1.Selector, Is.EqualTo("setP1:"), "methodSetP1.Selector");

        var methodInstanceMethod = myClass.Methods.SingleOrDefault(v => v.Name == "instanceMethod")!;
        Assert.That(methodInstanceMethod, Is.Not.Null, "methodInstanceMethod");
        Assert.That(methodInstanceMethod.Selector, Is.EqualTo("instanceMethod"), "methodInstanceMethod.Selector");

        var methodStaticMethod = myClass.Methods.SingleOrDefault(v => v.Name == "staticMethod")!;
        Assert.That(methodStaticMethod, Is.Not.Null, "methodStaticMethod");
        Assert.That(methodStaticMethod.Selector, Is.EqualTo("staticMethod"), "methodStaticMethod.Selector");
    }

    [Test]
    public void Method_Family()
    {
        var inputContents = $@"
@interface NSObject
    -(void) none;
    -(void) noneWithArgument:(int)arg;
    -(void) something;
    -(void) autoreleaseNone;
    -(void) deallocNone;
    -(void) finalizeNone;
    -(void) releaseNone;
    -(void) retainNone;
    -(unsigned long) retainCountNone;
    -(instancetype) selfNone;
    -(instancetype) initializeNone;
    -(void) performSelectorNone;

    +(instancetype) alloc;
    +(instancetype) allocWithZone:(void*)zone;
    +(instancetype) _alloc;
    +(instancetype) __alloc;
    +(instancetype) _allocWithZone:(void*)zone;

    -(instancetype) copy;
    -(instancetype) copyWithZone:(void*)zone;
    -(instancetype) _copy;
    -(instancetype) __copyWithZone:(void*)zone;

    -(instancetype) init;
    -(instancetype) initWithValue:(int)value;
    -(instancetype) _init;
    -(instancetype) __initWithValue:(int)value;

    -(instancetype) mutableCopy;
    -(instancetype) mutableCopyWithZone:(void*)zone;
    -(instancetype) _mutableCopy;
    -(instancetype) __mutableCopyWithZone:(void*)zone;

    +(instancetype) new;
    +(instancetype) newWithValue:(int)value;
    +(instancetype) _new;
    +(instancetype) __newWithValue:(int)value;

    -(void) autorelease;
    -(void) dealloc;
    -(void) finalize;
    -(void) release;
    -(void) retain;
    -(unsigned long) retainCount;
    -(instancetype) self;
    +(void) initialize;
    -(id) performSelector:(SEL)selector;
@end

@interface MyClass
    -(instancetype) instanceMethod;
    +(MyClass*) staticMethod;
@end
";

        using var translationUnit = CreateTranslationUnit(inputContents, "objective-c++");

        var classes = translationUnit.TranslationUnitDecl.Decls.OfType<ObjCInterfaceDecl>().ToList();
        Assert.That(classes.Count, Is.GreaterThanOrEqualTo(2), $"At least two classes");

        foreach (var cls in classes)
        {
            var methods = cls.Methods.ToList();
            if (methods.Count == 0)
            {
                continue;
            }

            var take = new Func<Func<string, bool>, ObjCMethodFamily, IEnumerable<ObjCMethodDecl>>((Func<string, bool> filter, ObjCMethodFamily family) => {
                var taken = new List<ObjCMethodDecl>();
                for (var v = methods.Count - 1; v >= 0; v--)
                {
                    var method = methods[v];
                    if (filter(method.Selector))
                    {
                        methods.RemoveAt(v);
                        taken.Add(method);
                    }
                }

                Assert.That(taken.Count, Is.GreaterThanOrEqualTo(0), $"family {family} not found, methods remaining: {string.Join(", ", methods.Select(p => p.Selector))}");
                return taken;
            });

            var assertFamily = new Action<Func<string, bool>, ObjCMethodFamily>((Func<string, bool> filter, ObjCMethodFamily family) => {
                var methodsInFamily = take(filter, family);
                foreach (var method in methodsInFamily)
                {
                    Assert.That(method.MethodFamily, Is.EqualTo(family), $"MethodFamily for {method.Selector}");
                }
            });

            var isSelectorFamily = new Func<string, string, bool>((string selector, string family) => {
                selector = selector.TrimStart('_');
                if (selector == family)
                {
                    return true;
                }
                if (!selector.StartsWith(family, StringComparison.Ordinal))
                {
                    return false;
                }
                var nextChar = selector[family.Length];
                if (nextChar == ':' || char.IsUpper(nextChar))
                {
                    return true;
                }
                return false;
            });

            Assert.Multiple(() => {
                assertFamily(s => isSelectorFamily(s, "alloc"), ObjCMethodFamily.Alloc);
                assertFamily(s => isSelectorFamily(s, "copy"), ObjCMethodFamily.Copy);
                assertFamily(s => isSelectorFamily(s, "init"), ObjCMethodFamily.Init);
                assertFamily(s => isSelectorFamily(s, "mutableCopy"), ObjCMethodFamily.MutableCopy);
                assertFamily(s => isSelectorFamily(s, "new"), ObjCMethodFamily.New);
                assertFamily(s => s == "autorelease", ObjCMethodFamily.Autorelease);
                assertFamily(s => s == "dealloc", ObjCMethodFamily.Dealloc);
                assertFamily(s => s == "finalize", ObjCMethodFamily.Finalize);
                assertFamily(s => s == "release", ObjCMethodFamily.Release);
                assertFamily(s => s == "retain", ObjCMethodFamily.Retain);
                assertFamily(s => s == "retainCount", ObjCMethodFamily.RetainCount);
                assertFamily(s => s == "self", ObjCMethodFamily.Self);
                assertFamily(s => s == "initialize", ObjCMethodFamily.Initialize);
                assertFamily(s => s.StartsWith("performSelector:", StringComparison.Ordinal), ObjCMethodFamily.PerformSelector);

                Assert.That(methods.Count, Is.GreaterThan(0), $"No remaining methods in {cls.Name}");

                // None of the remaining methods should belong to a family
                foreach (var method in methods)
                {
                    Assert.That(method.MethodFamily, Is.EqualTo(ObjCMethodFamily.None), $"MethodFamily for {method.Selector}");
                }
            });
        }
    }

    [Test]
    public void Category_TypeParamList()
    {
        var inputContents = $@"
@interface MyClass
@end
@interface MyClass (MyCategory)
@end
";

        using var translationUnit = CreateTranslationUnit(inputContents, "objective-c++");

        var categories = translationUnit.TranslationUnitDecl.Decls.OfType<ObjCCategoryDecl>().ToList ();
        Assert.That (categories.Count, Is.EqualTo (1), "Count");
        foreach (var c in categories) {
            Assert.That(c.TypeParamList.Count, Is.EqualTo (0), "TypeParamList.Count");
        }
    }

    [Test]
    public void ClassWithProtocols()
    {
        var inputContents = $@"
@protocol P1
@end
@protocol P2
@end

@interface MyClass <P1, P2>
@end
";

        using var translationUnit = CreateTranslationUnit(inputContents, "objective-c++");

        var classes = translationUnit.TranslationUnitDecl.Decls.OfType<ObjCInterfaceDecl>().ToList();
        Assert.That(classes.Count, Is.GreaterThanOrEqualTo(1), $"At least one class");
        var myClass = classes.SingleOrDefault(v => v.Name == "MyClass")!;
        Assert.That(myClass, Is.Not.Null, "MyClass");
        var protocols = myClass.Protocols.ToList();
        Assert.That(protocols.Count, Is.EqualTo(2), "protocols.Count");
        Assert.That(protocols[0].Name, Is.EqualTo("P1"), "protocols[0].Name");
        Assert.That(protocols[1].Name, Is.EqualTo("P2"), "protocols[1].Name");
    }

    [Test]
    public void Method_IsPropertyAccessor()
    {
        var inputContents = $@"
@interface MyClass
    @property int P1;
    -(void) instanceMethod;
    +(void) staticMethod;
@end
";

        using var translationUnit = CreateTranslationUnit(inputContents, "objective-c++");

        var classes = translationUnit.TranslationUnitDecl.Decls.OfType<ObjCInterfaceDecl>().ToList();
        Assert.That(classes.Count, Is.GreaterThanOrEqualTo(1), $"At least one class");
        var myClass = classes.SingleOrDefault(v => v.Name == "MyClass")!;
        Assert.That(myClass, Is.Not.Null, "MyClass");

        var methodP1 = myClass.Methods.SingleOrDefault(v => v.Name == "P1")!;
        Assert.That(methodP1, Is.Not.Null, "methodP1");
        Assert.That(methodP1.IsPropertyAccessor, Is.True, "methodP1.IsPropertyAccessor");

        var methodSetP1 = myClass.Methods.SingleOrDefault(v => v.Name == "setP1:")!;
        Assert.That(methodSetP1, Is.Not.Null, "methodSetP1");
        Assert.That(methodSetP1.IsPropertyAccessor, Is.True, "methodSetP1.IsPropertyAccessor");

        var methodInstanceMethod = myClass.Methods.SingleOrDefault(v => v.Name == "instanceMethod")!;
        Assert.That(methodInstanceMethod, Is.Not.Null, "methodInstanceMethod");
        Assert.That(methodInstanceMethod.IsPropertyAccessor, Is.False, "methodInstanceMethod.IsPropertyAccessor");

        var methodStaticMethod = myClass.Methods.SingleOrDefault(v => v.Name == "staticMethod")!;
        Assert.That(methodStaticMethod, Is.Not.Null, "methodStaticMethod");
        Assert.That(methodStaticMethod.IsPropertyAccessor, Is.False, "methodStaticMethod.IsPropertyAccessor");
    }

    [Test]
    public void TypeParams()
    {
        var inputContents = $$"""
@interface NSObject
@end

@interface MyClass<A, B> : NSObject
@end

@interface MyClass<T, Y> (MyCategory)
@end

@interface MyVariantClass<__contravariant T, __covariant U, V> : NSObject
@end

@interface MyBoundedClass<T : NSObject*> : NSObject
@end

""";
        using var translationUnit = CreateTranslationUnit(inputContents, "objective-c++");

        var classes = translationUnit.TranslationUnitDecl.Decls.OfType<ObjCInterfaceDecl>().ToList();

        var myClass = classes.SingleOrDefault(v => v.Name == "MyClass")!;
        Assert.That(myClass, Is.Not.Null, "MyClass");
        Assert.That(myClass.TypeParamList, Is.Not.Null, "myClass TypeParamList");
        var myClassTypeParams = myClass.TypeParamList.ToList();
        Assert.That(myClassTypeParams.Count, Is.EqualTo(2), "myClassTypeParams.Count");
        Assert.That(myClassTypeParams[0].Name, Is.EqualTo("A"), "myClassTypeParams[0].Name");
        Assert.That(myClassTypeParams[0].HasExplicitBound, Is.EqualTo(false), "myClassTypeParams[0].HasExplicitBound");
        Assert.That(myClassTypeParams[0].Variance, Is.EqualTo(ObjCTypeParamVariance.Invariant), "myClassTypeParams[0].Variance");
        Assert.That(myClassTypeParams[1].Name, Is.EqualTo("B"), "myClassTypeParams[1].Name");
        Assert.That(myClassTypeParams[1].HasExplicitBound, Is.EqualTo(false), "myClassTypeParams[1].HasExplicitBound");
        Assert.That(myClassTypeParams[1].Variance, Is.EqualTo(ObjCTypeParamVariance.Invariant), "myClassTypeParams[1].Variance");

        var categories = translationUnit.TranslationUnitDecl.Decls.OfType<ObjCCategoryDecl>().ToList();
        var myCategory = categories.SingleOrDefault(v => v.Name == "MyCategory")!;
        Assert.That(myCategory, Is.Not.Null, "MyCategory");
        Assert.That(myCategory.TypeParamList, Is.Not.Null, "myCategory TypeParamList");
        var myCategoryTypeParams = myCategory.TypeParamList.ToList();
        Assert.That(myCategoryTypeParams.Count, Is.EqualTo(2), "myCategoryTypeParams.Count");
        Assert.That(myCategoryTypeParams[0].Name, Is.EqualTo("T"), "myCategoryTypeParams[0].Name");
        Assert.That(myCategoryTypeParams[0].HasExplicitBound, Is.EqualTo(false), "myCategoryTypeParams[0].HasExplicitBound");
        Assert.That(myCategoryTypeParams[0].Variance, Is.EqualTo(ObjCTypeParamVariance.Invariant), "myCategoryTypeParams[0].Variance");
        Assert.That(myCategoryTypeParams[1].Name, Is.EqualTo("Y"), "myCategoryTypeParams[1].Name");
        Assert.That(myCategoryTypeParams[1].HasExplicitBound, Is.EqualTo(false), "myCategoryTypeParams[1].HasExplicitBound");
        Assert.That(myCategoryTypeParams[1].Variance, Is.EqualTo(ObjCTypeParamVariance.Invariant), "myCategoryTypeParams[1].Variance");

        var myVariantClass = classes.SingleOrDefault(v => v.Name == "MyVariantClass")!;
        Assert.That(myVariantClass, Is.Not.Null, "MyVariantClass");
        Assert.That(myVariantClass.TypeParamList, Is.Not.Null, "myVariantClass TypeParamList");
        var myVariantClassTypeParams = myVariantClass.TypeParamList.ToList();
        Assert.That(myVariantClassTypeParams.Count, Is.EqualTo(3), "myVariantClassTypeParams.Count");
        Assert.That(myVariantClassTypeParams[0].Name, Is.EqualTo("T"), "myVariantClassTypeParams[0].Name");
        Assert.That(myVariantClassTypeParams[1].Name, Is.EqualTo("U"), "myVariantClassTypeParams[1].Name");
        Assert.That(myVariantClassTypeParams[2].Name, Is.EqualTo("V"), "myVariantClassTypeParams[2].Name");
        Assert.That(myVariantClassTypeParams[0].HasExplicitBound, Is.EqualTo(false), "myVariantClassTypeParams[0].HasExplicitBound");
        Assert.That(myVariantClassTypeParams[1].HasExplicitBound, Is.EqualTo(false), "myVariantClassTypeParams[1].HasExplicitBound");
        Assert.That(myVariantClassTypeParams[2].HasExplicitBound, Is.EqualTo(false), "myBoundedClassTypeParams[2].HasExplicitBound");
        Assert.That(myVariantClassTypeParams[0].Variance, Is.EqualTo(ObjCTypeParamVariance.Contravariant), "myVariantClassTypeParams[0].Variance");
        Assert.That(myVariantClassTypeParams[1].Variance, Is.EqualTo(ObjCTypeParamVariance.Covariant), "myVariantClassTypeParams[1].Variance");
        Assert.That(myVariantClassTypeParams[2].Variance, Is.EqualTo(ObjCTypeParamVariance.Invariant), "myVariantClassTypeParams[2].Variance");

        var myBoundedClass = classes.SingleOrDefault(v => v.Name == "MyBoundedClass")!;
        Assert.That(myBoundedClass, Is.Not.Null, "MyBoundedClass");
        Assert.That(myBoundedClass.TypeParamList, Is.Not.Null, "myBoundedClass TypeParamList");
        var myBoundedClassTypeParams = myBoundedClass.TypeParamList.ToList();
        Assert.That(myBoundedClassTypeParams.Count, Is.EqualTo(1), "myBoundedClassTypeParams.Count");
        Assert.That(myBoundedClassTypeParams[0].Name, Is.EqualTo("T"), "myBoundedClassTypeParams[0].Name");
        Assert.That(myBoundedClassTypeParams[0].HasExplicitBound, Is.EqualTo(true), "myBoundedClassTypeParams[0].HasExplicitBound");
        Assert.That(myBoundedClassTypeParams[0].Variance, Is.EqualTo(ObjCTypeParamVariance.Invariant), "myBoundedClassTypeParams[0].Variance");
    }

    [Test]
    public void PointerTypes()
    {
        var inputContents = """
@interface MyClass
-(void)instanceMethod:(MyClass **)ptrToPtrToMyClass;
@end
""";
        using var translationUnit = CreateTranslationUnit(inputContents, "objective-c++");

        var classes = translationUnit.TranslationUnitDecl.Decls.OfType<ObjCInterfaceDecl>().ToList();

        var myClass = classes.SingleOrDefault(v => v.Name == "MyClass")!;
        Assert.That(myClass, Is.Not.Null, "MyClass");

        var instanceMethod = myClass.Methods.SingleOrDefault(v => v.Name == "instanceMethod:")!;
        Assert.That(instanceMethod, Is.Not.Null, "instanceMethod");
        var parameters = instanceMethod.Parameters.ToList();
        Assert.That(parameters.Count, Is.EqualTo(1), "parameters.Count");
        var parameter = parameters[0];
        Assert.That(parameter.Name, Is.EqualTo("ptrToPtrToMyClass"), "parameter.Name");
        var parameterType = parameter.Type;
        Assert.That(parameterType.Kind, Is.EqualTo(CXTypeKind.CXType_Pointer), "parameterType.Kind");
        var pointeeType = parameterType.PointeeType;
        Assert.That(pointeeType.Kind, Is.EqualTo(CXTypeKind.CXType_ObjCObjectPointer), "pointeeType.Kind");
        var pointee2Type = pointeeType.PointeeType;
        Assert.That(pointee2Type.Kind, Is.EqualTo(CXTypeKind.CXType_ObjCInterface), "pointee2Type.Kind");
    }

    [Test]
    public void BlockTypes()
    {
        var inputContents = $$"""
@interface MyClass
-(MyClass *(^)(id))instanceMethod1;
-(MyClass *(^)(SEL))instanceMethod2;
-(MyClass *(^)(Class))instanceMethod3;
@end
""";
        using var translationUnit = CreateTranslationUnit(inputContents, "objective-c++");

        var classes = translationUnit.TranslationUnitDecl.Decls.OfType<ObjCInterfaceDecl>().ToList();

        var myClass = classes.SingleOrDefault(v => v.Name == "MyClass")!;
        Assert.That(myClass, Is.Not.Null, "MyClass");

        var info = new[] {
            (Name: "instanceMethod1", Type: CXTypeKind.CXType_ObjCId),
            (Name: "instanceMethod2", Type: CXTypeKind.CXType_ObjCSel),
            (Name: "instanceMethod3", Type: CXTypeKind.CXType_ObjCClass),
        };

        foreach (var i in info)
        {
            var instanceMethod = myClass.Methods.SingleOrDefault(v => v.Name == i.Name)!;
            Assert.That(instanceMethod, Is.Not.Null, "instanceMethod");
            var returnType = instanceMethod.ReturnType;
            Assert.That(returnType.Kind, Is.EqualTo(CXTypeKind.CXType_BlockPointer), "returnType.Kind");
            var pointeeType = returnType.PointeeType;
            Assert.That(pointeeType.Kind, Is.EqualTo(CXTypeKind.CXType_FunctionProto), "pointeeType.Kind");
            var functionProtoType = (FunctionProtoType)pointeeType;
            Assert.That(functionProtoType.ParamTypes.Count, Is.EqualTo(1), "functionProtoType.ParamTypes.Count()");
            var paramType = functionProtoType.ParamTypes[0];
            Assert.That(paramType.Kind, Is.EqualTo(i.Type), "paramType.Kind");
        }
    }

    [Test]
    public void Attribute_AvailabilityAttributes()
    {
        var inputContents = $$"""
__attribute__((availability(ios,unavailable,message="Use another class")))
__attribute__((availability(tvos,introduced=10.0)))
__attribute__((availability(macosx,introduced=10.15.3)))
__attribute__((availability(maccatalyst,introduced=14.3.0)))
@interface MyClass
    @property int P1
        __attribute__((availability(ios,unavailable,message="Not this property")))
        __attribute__((availability(tvos,obsoleted=11.0,message="Obsoleted on tvOS")))
        __attribute__((availability(macosx,obsoleted=11.0,message="Obsoleted on macOS")))
        __attribute__((availability(maccatalyst,obsoleted=15.0,message="Obsoleted on Mac Catalyst")))
        ;

    -(void) instanceMethod
        __attribute__((availability(ios,unavailable,message="Not this instance method")))
        __attribute__((availability(tvos,deprecated=12.0,message="Deprecated on tvOS")))
        __attribute__((availability(macosx,deprecated=12.0,message="Deprecated on macOS")))
        __attribute__((availability(maccatalyst,deprecated=16.0,message="Deprecated on Mac Catalyst")))
        ;

    +(void) staticMethod __attribute__((unavailable("elsewhere")))
        __attribute__((availability(ios,unavailable,message="Not this static method")))
        __attribute__((availability(tvos,introduced=10.0,deprecated=11.0,obsoleted=12.0,message="Gone on tvOS")))
        __attribute__((availability(macosx,introduced=10.0.1,deprecated=11.0.2,obsoleted=12.0.3,message="Gone on macOS")))
        __attribute__((availability(maccatalyst,introduced=10.0.0,deprecated=11.0.0,obsoleted=12.0.0,message="Gone on Mac Catalyst")))
        ;
@end
""";
        using var translationUnit = CreateTranslationUnit(inputContents, "objective-c++");

        var classes = translationUnit.TranslationUnitDecl.Decls.OfType<ObjCInterfaceDecl>().ToList();
        Assert.That(classes.Count, Is.GreaterThanOrEqualTo(1), $"At least one class");
        var myClass = classes.SingleOrDefault(v => v.Name == "MyClass")!;
        Assert.That(myClass, Is.Not.Null, "MyClass");

        var assertVersionTuple = new Action<VersionTuple?, VersionTuple?, string>((VersionTuple? actual, VersionTuple? expected, string info) => {
            if (expected is null)
            {
                if (actual is not null)
                {
                    Assert.Fail($"Expected null VersionTuple, got: {actual} -- {info}");
                }
                return;
            }
            else
            {
                if (actual is null)
                {
                    Assert.Fail($"Expected non-null VersionTuple ({expected.Value}), got null -- {info}");
                    return;
                }
            }

            Assert.That(actual.ToString(), Is.EqualTo(expected.ToString()), info);
        });

        var assertAttribute = new Action<Attr, string?, string?, VersionTuple?, VersionTuple?, VersionTuple?, bool, string>((Attr attrib, string? message, string? platform, VersionTuple? introduced, VersionTuple? deprecated, VersionTuple? obsoleted, bool unavailable, string info) => {
            Assert.That(attrib, Is.Not.Null, $"attrib: {info}");

            info += $" (platform: {attrib.AvailabilityAttributePlatformIdentifierName})";
            Assert.That(attrib.AvailabilityAttributeMessage, Is.EqualTo(message), $"Message: {info}");
            Assert.That(attrib.AvailabilityAttributePlatformIdentifierName, Is.EqualTo(platform), $"PlatformIdentifierName: {info}");
            assertVersionTuple(attrib.AvailabilityAttributeIntroduced, introduced, $"Introduced: {info}");
            assertVersionTuple(attrib.AvailabilityAttributeDeprecated, deprecated, $"Deprecated: {info}");
            assertVersionTuple(attrib.AvailabilityAttributeObsoleted, obsoleted, $"Obsoleted: {info}");
            Assert.That(attrib.AvailabilityAttributeUnavailable, Is.EqualTo(unavailable), $"Unavailable: {info}");
        });

        Assert.Multiple(() => {
            var myClassAttrs = myClass.Attrs;
            Assert.That(myClassAttrs.Count, Is.EqualTo(4), "myClassAttrs.Count");
            assertAttribute(myClassAttrs[0], "Use another class", "ios", new VersionTuple(0), new VersionTuple(0), new VersionTuple(0), true, "myClass Attr - iOS");
            assertAttribute(myClassAttrs[1], "", "tvos", new VersionTuple(10, 0), new VersionTuple(0), new VersionTuple(0), false, "myClass Attr - tvOS");
            assertAttribute(myClassAttrs[2], "", "macos", new VersionTuple(10, 15, 3), new VersionTuple(0), new VersionTuple(0), false, "myClass Attr - macOS");
            assertAttribute(myClassAttrs[3], "", "maccatalyst", new VersionTuple(14, 3, 0), new VersionTuple(0), new VersionTuple(0), false, "myClass Attr - Mac Catalyst");

            var methodP1 = myClass.Methods.SingleOrDefault(v => v.Name == "P1")!;
            Assert.That(methodP1, Is.Not.Null, "methodP1");
            var methodP1Attrs = methodP1.Attrs;
            Assert.That(methodP1Attrs.Count, Is.EqualTo(4), "methodP1Attrs.Count");
            assertAttribute(methodP1Attrs[0], "Not this property", "ios", new VersionTuple(0), new VersionTuple(0), new VersionTuple(0), true, "methodP1 Attr - P1 - iOS");
            assertAttribute(methodP1Attrs[1], "Obsoleted on tvOS", "tvos", new VersionTuple(0), new VersionTuple(0), new VersionTuple(11, 0), false, "methodP1 Attr - tvOS");
            assertAttribute(methodP1Attrs[2], "Obsoleted on macOS", "macos", new VersionTuple(0), new VersionTuple(0), new VersionTuple(11, 0), false, "methodP1 Attr - macOS");
            assertAttribute(methodP1Attrs[3], "Obsoleted on Mac Catalyst", "maccatalyst", new VersionTuple(0), new VersionTuple(0), new VersionTuple(15, 0), false, "methodP1 Attr - Mac Catalyst");

            var methodSetP1 = myClass.Methods.SingleOrDefault(v => v.Name == "setP1:")!;
            Assert.That(methodSetP1, Is.Not.Null, "methodP1");
            var methodSetP1Attrs = methodSetP1.Attrs;
            Assert.That(methodSetP1Attrs.Count, Is.EqualTo(4), "methodSetP1Attrs.Count");
            assertAttribute(methodSetP1Attrs[0], "Not this property", "ios", new VersionTuple(0), new VersionTuple(0), new VersionTuple(0), true, "methodSetP1Attrs Attr - P1 - iOS");
            assertAttribute(methodSetP1Attrs[1], "Obsoleted on tvOS", "tvos", new VersionTuple(0), new VersionTuple(0), new VersionTuple(11, 0), false, "methodSetP1Attrs Attr - tvOS");
            assertAttribute(methodSetP1Attrs[2], "Obsoleted on macOS", "macos", new VersionTuple(0), new VersionTuple(0), new VersionTuple(11, 0), false, "methodSetP1Attrs Attr - macOS");
            assertAttribute(methodSetP1Attrs[3], "Obsoleted on Mac Catalyst", "maccatalyst", new VersionTuple(0), new VersionTuple(0), new VersionTuple(15, 0), false, "methodSetP1Attrs Attr - Mac Catalyst");

            var methodInstanceMethod = myClass.Methods.SingleOrDefault(v => v.Name == "instanceMethod")!;
            Assert.That(methodInstanceMethod, Is.Not.Null, "methodP1");
            var methodInstanceMethodAttrs = methodInstanceMethod.Attrs;
            Assert.That(methodInstanceMethodAttrs.Count, Is.EqualTo(4), "methodInstanceMethodAttrs.Count");
            assertAttribute(methodInstanceMethodAttrs[0], "Not this instance method", "ios", new VersionTuple(0), new VersionTuple(0), new VersionTuple(0), true, "methodInstanceMethodAttrs Attr - P1 - iOS");
            assertAttribute(methodInstanceMethodAttrs[1], "Deprecated on tvOS", "tvos", new VersionTuple(0), new VersionTuple(12, 0), new VersionTuple(0), false, "methodInstanceMethodAttrs Attr - tvOS");
            assertAttribute(methodInstanceMethodAttrs[2], "Deprecated on macOS", "macos", new VersionTuple(0), new VersionTuple(12, 0), new VersionTuple(0), false, "methodInstanceMethodAttrs Attr - macOS");
            assertAttribute(methodInstanceMethodAttrs[3], "Deprecated on Mac Catalyst", "maccatalyst", new VersionTuple(0), new VersionTuple(16, 0), new VersionTuple(0), false, "methodInstanceMethodAttrs Attr - Mac Catalyst");

            var methodStaticMethod = myClass.Methods.SingleOrDefault(v => v.Name == "staticMethod")!;
            Assert.That(methodStaticMethod, Is.Not.Null, "methodP1");
            var methodStaticMethodAttrs = methodStaticMethod.Attrs;
            Assert.That(methodStaticMethodAttrs.Count, Is.EqualTo(5), "methodStaticMethodAttrs.Count");
            assertAttribute(methodStaticMethodAttrs[0], "elsewhere", "", null, null, null, false, "methodStaticMethodAttrs Attr - P1 - unavailable");
            assertAttribute(methodStaticMethodAttrs[1], "Not this static method", "ios", new VersionTuple(0), new VersionTuple(0), new VersionTuple(0), true, "methodStaticMethodAttrs Attr - P1 - iOS");
            assertAttribute(methodStaticMethodAttrs[2], "Gone on tvOS", "tvos", new VersionTuple(10, 0), new VersionTuple(11, 0), new VersionTuple(12, 0), false, "methodStaticMethodAttrs Attr - tvOS");
            assertAttribute(methodStaticMethodAttrs[3], "Gone on macOS", "macos", new VersionTuple(10, 0, 1), new VersionTuple(11, 0, 2), new VersionTuple(12, 0, 3), false, "methodStaticMethodAttrs Attr - macOS");
            assertAttribute(methodStaticMethodAttrs[4], "Gone on Mac Catalyst", "maccatalyst", new VersionTuple(10, 0, 0), new VersionTuple(11, 0, 0), new VersionTuple(12, 0, 0), false, "methodStaticMethodAttrs Attr - Mac Catalyst");
        });
    }

    [Test]
    public void Attribute_PrettyPrint()
    {
        var inputContents = $@"
__attribute__((availability(ios,introduced=10.0)))
@interface MyClass
    @property int P1 __attribute__((availability(ios,introduced=11.0)));
    -(void) instanceMethod __attribute__((availability(ios,introduced=12.0)));
    +(void) staticMethod __attribute__((availability(ios,introduced=13.0)));
@end
";

        using var translationUnit = CreateTranslationUnit(inputContents, "objective-c++");

        var classes = translationUnit.TranslationUnitDecl.Decls.OfType<ObjCInterfaceDecl>().ToList();
        Assert.That(classes.Count, Is.GreaterThanOrEqualTo(1), $"At least one class");
        var myClass = classes.SingleOrDefault(v => v.Name == "MyClass")!;
        Assert.That(myClass, Is.Not.Null, "MyClass");
        var myClassAttrs = myClass.Attrs;
        Assert.That(myClassAttrs.Count, Is.EqualTo(1), "myClassAttrs.Count");
        Assert.That(myClassAttrs[0].PrettyPrint(), Is.EqualTo("__attribute__((availability(ios, introduced=10.0)))"), "myClass.Attr.PrettyPrint");

        var methodP1 = myClass.Methods.SingleOrDefault(v => v.Name == "P1")!;
        Assert.That(methodP1, Is.Not.Null, "methodP1");
        var methodP1Attrs = methodP1.Attrs;
        Assert.That(methodP1Attrs.Count, Is.EqualTo(1), "methodP1Attrs.Count");
        Assert.That(methodP1Attrs[0].PrettyPrint(), Is.EqualTo("__attribute__((availability(ios, introduced=11.0)))"), "methodP1.Attr.PrettyPrint");

        var methodSetP1 = myClass.Methods.SingleOrDefault(v => v.Name == "setP1:")!;
        Assert.That(methodSetP1, Is.Not.Null, "methodSetP1");
        var methodSetP1Attrs = methodSetP1.Attrs;
        Assert.That(methodSetP1Attrs.Count, Is.EqualTo(1), "methodSetP1Attrs.Count");
        Assert.That(methodSetP1Attrs[0].PrettyPrint(), Is.EqualTo("__attribute__((availability(ios, introduced=11.0)))"), "methodSetP1.Attr.PrettyPrint");

        var methodInstanceMethod = myClass.Methods.SingleOrDefault(v => v.Name == "instanceMethod")!;
        Assert.That(methodInstanceMethod, Is.Not.Null, "methodInstanceMethod");
        var methodInstanceMethodAttrs = methodInstanceMethod.Attrs;
        Assert.That(methodInstanceMethodAttrs.Count, Is.EqualTo(1), "methodInstanceMethodAttrs.Count");
        Assert.That(methodInstanceMethodAttrs[0].PrettyPrint(), Is.EqualTo("__attribute__((availability(ios, introduced=12.0)))"), "methodInstanceMethod.Attr.PrettyPrint");

        var methodStaticMethod = myClass.Methods.SingleOrDefault(v => v.Name == "staticMethod")!;
        Assert.That(methodStaticMethod, Is.Not.Null, "methodStaticMethod");
        var methodStaticMethodAttrs = methodStaticMethod.Attrs;
        Assert.That(methodStaticMethodAttrs.Count, Is.EqualTo(1), "methodStaticMethodAttrs.Count");
        Assert.That(methodStaticMethodAttrs[0].PrettyPrint(), Is.EqualTo("__attribute__((availability(ios, introduced=13.0)))"), "methodStaticMethod.Attr.PrettyPrint");
    }

    [Test]
    public void Property_PropertyAttributes()
    {
        var inputContents = $$"""
@class NSObject;

@interface MyClass
    @property NSObject* P_none;

    @property (readonly) NSObject* P_readonly;
    @property (getter=getGetter) NSObject* P_getter;
    @property (assign) NSObject* P_assign;
    @property (readwrite) NSObject* P_readwrite;
    @property (retain) NSObject* P_retain;
    @property (copy) NSObject* P_copy;
    @property (nonatomic) NSObject* P_nonatomic;
    @property (setter=setSetter:) NSObject* P_setter;
    @property (atomic) NSObject* P_atomic;
    @property (weak) NSObject* P_weak;
    @property (strong) NSObject* P_strong;
    @property (unsafe_unretained) NSObject* P_unsafe_unretained;
    @property (nonnull) NSObject* P_nonnull;
    @property (null_unspecified) NSObject* P_null_unspecified;
    @property (null_resettable) NSObject* P_null_resettable;
    @property (class) NSObject* P_class;
    @property (direct) NSObject* P_direct;

    // multiple
    @property (retain,readonly,class) NSObject* P_retain_readonly_class;
@end
""";

        using var translationUnit = CreateTranslationUnit(inputContents, "objective-c++");

        var classes = translationUnit.TranslationUnitDecl.Decls.OfType<ObjCInterfaceDecl>().ToList();
        Assert.That(classes.Count, Is.GreaterThanOrEqualTo(1), $"At least one class");
        var myClass = classes.SingleOrDefault(v => v.Name == "MyClass")!;
        Assert.That(myClass, Is.Not.Null, "MyClass");

        var properties = myClass.Properties.ToList();

        var take = new Func<string, ObjCPropertyDecl>((string propertyName) => {
            var idx = properties.FindIndex(v => v.Name == propertyName);
            Assert.That(idx, Is.GreaterThanOrEqualTo(0), $"property {propertyName} not found, properties remaining: {string.Join(", ", properties.Select(p => p.Name))}");
            var rv = properties[idx];
            properties.RemoveAt(idx);
            return rv;
        });

        var property_none = take("P_none");
        Assert.That(property_none.PropertyAttributes, Is.EqualTo(CXObjCPropertyAttrKind.CXObjCPropertyAttr_noattr), "property_none PropertyAttributes");
        Assert.That(property_none.GetPropertyAttributes(), Is.EqualTo(ObjCPropertyAttributeKind.Assign | ObjCPropertyAttributeKind.ReadWrite | ObjCPropertyAttributeKind.Atomic | ObjCPropertyAttributeKind.UnsafeUnretained), "property_none GetPropertyAttributes()");

        var property_readonly = take("P_readonly");
        Assert.That(property_readonly.PropertyAttributes, Is.EqualTo(CXObjCPropertyAttrKind.CXObjCPropertyAttr_readonly), "property_readonly PropertyAttributes");
        Assert.That(property_readonly.GetPropertyAttributes(), Is.EqualTo(ObjCPropertyAttributeKind.ReadOnly | ObjCPropertyAttributeKind.Atomic), "property_readonly GetPropertyAttributes()");

        var property_getter = take("P_getter");
        Assert.That(property_getter.PropertyAttributes, Is.EqualTo(CXObjCPropertyAttrKind.CXObjCPropertyAttr_getter), "property_getter PropertyAttributes");
        Assert.That(property_getter.GetPropertyAttributes(), Is.EqualTo(ObjCPropertyAttributeKind.Getter | ObjCPropertyAttributeKind.Assign | ObjCPropertyAttributeKind.ReadWrite | ObjCPropertyAttributeKind.Atomic | ObjCPropertyAttributeKind.UnsafeUnretained), "property_getter GetPropertyAttributes()");

        var property_assign = take("P_assign");
        Assert.That(property_assign.PropertyAttributes, Is.EqualTo(CXObjCPropertyAttrKind.CXObjCPropertyAttr_assign), "property_assign PropertyAttributes");
        Assert.That(property_assign.GetPropertyAttributes(), Is.EqualTo(ObjCPropertyAttributeKind.Assign | ObjCPropertyAttributeKind.ReadWrite | ObjCPropertyAttributeKind.Atomic | ObjCPropertyAttributeKind.UnsafeUnretained), "property_assign GetPropertyAttributes()");

        var property_readwrite = take("P_readwrite");
        Assert.That(property_readwrite.PropertyAttributes, Is.EqualTo(CXObjCPropertyAttrKind.CXObjCPropertyAttr_readwrite), "property_readwrite PropertyAttributes");
        Assert.That(property_readwrite.GetPropertyAttributes(), Is.EqualTo(ObjCPropertyAttributeKind.Assign | ObjCPropertyAttributeKind.ReadWrite | ObjCPropertyAttributeKind.Atomic | ObjCPropertyAttributeKind.UnsafeUnretained), "property_readwrite GetPropertyAttributes()");

        var property_retain = take("P_retain");
        Assert.That(property_retain.PropertyAttributes, Is.EqualTo(CXObjCPropertyAttrKind.CXObjCPropertyAttr_retain), "property_retain PropertyAttributes");
        Assert.That(property_retain.GetPropertyAttributes(), Is.EqualTo(ObjCPropertyAttributeKind.Retain | ObjCPropertyAttributeKind.ReadWrite | ObjCPropertyAttributeKind.Atomic), "property_retain GetPropertyAttributes()");

        var property_copy = take("P_copy");
        Assert.That(property_copy.PropertyAttributes, Is.EqualTo(CXObjCPropertyAttrKind.CXObjCPropertyAttr_copy), "property_copy PropertyAttributes");
        Assert.That(property_copy.GetPropertyAttributes(), Is.EqualTo(ObjCPropertyAttributeKind.Copy | ObjCPropertyAttributeKind.ReadWrite | ObjCPropertyAttributeKind.Atomic), "property_copy GetPropertyAttributes()");

        var property_nonatomic = take("P_nonatomic");
        Assert.That(property_nonatomic.PropertyAttributes, Is.EqualTo(CXObjCPropertyAttrKind.CXObjCPropertyAttr_nonatomic), "property_nonatomic PropertyAttributes");
        Assert.That(property_nonatomic.GetPropertyAttributes(), Is.EqualTo(ObjCPropertyAttributeKind.Assign | ObjCPropertyAttributeKind.ReadWrite | ObjCPropertyAttributeKind.NonAtomic | ObjCPropertyAttributeKind.UnsafeUnretained), "property_nonatomic GetPropertyAttributes()");

        var property_setter = take("P_setter");
        Assert.That(property_setter.PropertyAttributes, Is.EqualTo(CXObjCPropertyAttrKind.CXObjCPropertyAttr_setter), "property_setter PropertyAttributes");
        Assert.That(property_setter.GetPropertyAttributes(), Is.EqualTo(ObjCPropertyAttributeKind.Setter | ObjCPropertyAttributeKind.Assign | ObjCPropertyAttributeKind.ReadWrite | ObjCPropertyAttributeKind.Atomic | ObjCPropertyAttributeKind.UnsafeUnretained), "property_setter GetPropertyAttributes()");

        var property_atomic = take("P_atomic");
        Assert.That(property_atomic.PropertyAttributes, Is.EqualTo(CXObjCPropertyAttrKind.CXObjCPropertyAttr_atomic), "property_atomic PropertyAttributes");
        Assert.That(property_atomic.GetPropertyAttributes(), Is.EqualTo(ObjCPropertyAttributeKind.Assign | ObjCPropertyAttributeKind.ReadWrite | ObjCPropertyAttributeKind.Atomic | ObjCPropertyAttributeKind.UnsafeUnretained), "property_atomic GetPropertyAttributes()");

        var property_weak = take("P_weak");
        Assert.That(property_weak.PropertyAttributes, Is.EqualTo(CXObjCPropertyAttrKind.CXObjCPropertyAttr_weak), "property_weak PropertyAttributes");
        Assert.That(property_weak.GetPropertyAttributes(), Is.EqualTo(ObjCPropertyAttributeKind.Weak | ObjCPropertyAttributeKind.ReadWrite | ObjCPropertyAttributeKind.Atomic), "property_weak GetPropertyAttributes()");

        var property_strong = take("P_strong");
        Assert.That(property_strong.PropertyAttributes, Is.EqualTo(CXObjCPropertyAttrKind.CXObjCPropertyAttr_strong), "property_strong PropertyAttributes");
        Assert.That(property_strong.GetPropertyAttributes(), Is.EqualTo(ObjCPropertyAttributeKind.Strong | ObjCPropertyAttributeKind.ReadWrite | ObjCPropertyAttributeKind.Atomic), "property_strong GetPropertyAttributes()");

        var property_unsafe_unretained = take("P_unsafe_unretained");
        Assert.That(property_unsafe_unretained.PropertyAttributes, Is.EqualTo(CXObjCPropertyAttrKind.CXObjCPropertyAttr_unsafe_unretained), "property_unsafe_unretained PropertyAttributes");
        Assert.That(property_unsafe_unretained.GetPropertyAttributes(), Is.EqualTo(ObjCPropertyAttributeKind.Assign | ObjCPropertyAttributeKind.ReadWrite | ObjCPropertyAttributeKind.Atomic | ObjCPropertyAttributeKind.UnsafeUnretained), "property_unsafe_unretained GetPropertyAttributes()");

        var property_null_resettable = take("P_null_resettable");
        Assert.That(property_null_resettable.PropertyAttributes, Is.EqualTo(CXObjCPropertyAttrKind.CXObjCPropertyAttr_noattr), "property_null_resettable PropertyAttributes");
        Assert.That(property_null_resettable.GetPropertyAttributes(), Is.EqualTo(ObjCPropertyAttributeKind.Assign | ObjCPropertyAttributeKind.ReadWrite | ObjCPropertyAttributeKind.Atomic | ObjCPropertyAttributeKind.UnsafeUnretained | ObjCPropertyAttributeKind.Nullability | ObjCPropertyAttributeKind.NullResettable), "property_null_resettable GetPropertyAttributes()");

        var property_class = take("P_class");
        Assert.That(property_class.PropertyAttributes, Is.EqualTo(CXObjCPropertyAttrKind.CXObjCPropertyAttr_class), "property_class PropertyAttributes");
        Assert.That(property_class.GetPropertyAttributes(), Is.EqualTo(ObjCPropertyAttributeKind.Class | ObjCPropertyAttributeKind.Assign | ObjCPropertyAttributeKind.ReadWrite | ObjCPropertyAttributeKind.Atomic | ObjCPropertyAttributeKind.UnsafeUnretained), "property_class GetPropertyAttributes()");

        var property_direct = take("P_direct");
        Assert.That(property_direct.PropertyAttributes, Is.EqualTo(CXObjCPropertyAttrKind.CXObjCPropertyAttr_noattr), "property_direct PropertyAttributes");
        Assert.That(property_direct.GetPropertyAttributes(), Is.EqualTo(ObjCPropertyAttributeKind.Direct | ObjCPropertyAttributeKind.Assign | ObjCPropertyAttributeKind.ReadWrite | ObjCPropertyAttributeKind.Atomic | ObjCPropertyAttributeKind.UnsafeUnretained), "property_direct GetPropertyAttributes()");

        var property_nonnull = take("P_nonnull");
        Assert.That(property_nonnull.PropertyAttributes, Is.EqualTo(CXObjCPropertyAttrKind.CXObjCPropertyAttr_noattr), "property_nonnull PropertyAttributes");
        Assert.That(property_nonnull.GetPropertyAttributes(), Is.EqualTo(ObjCPropertyAttributeKind.Assign | ObjCPropertyAttributeKind.ReadWrite | ObjCPropertyAttributeKind.Atomic | ObjCPropertyAttributeKind.UnsafeUnretained | ObjCPropertyAttributeKind.Nullability), "property_nonnull GetPropertyAttributes()");

        var property_unspecified = take("P_null_unspecified");
        Assert.That(property_unspecified.PropertyAttributes, Is.EqualTo(CXObjCPropertyAttrKind.CXObjCPropertyAttr_noattr), "property_unspecified PropertyAttributes");
        Assert.That(property_unspecified.GetPropertyAttributes(), Is.EqualTo(ObjCPropertyAttributeKind.Assign | ObjCPropertyAttributeKind.ReadWrite | ObjCPropertyAttributeKind.Atomic | ObjCPropertyAttributeKind.UnsafeUnretained | ObjCPropertyAttributeKind.Nullability), "property_unspecified GetPropertyAttributes()");

        var property_retain_readonly_class = take("P_retain_readonly_class");
        Assert.That(property_retain_readonly_class.PropertyAttributes, Is.EqualTo(CXObjCPropertyAttrKind.CXObjCPropertyAttr_retain | CXObjCPropertyAttrKind.CXObjCPropertyAttr_readonly | CXObjCPropertyAttrKind.CXObjCPropertyAttr_class), "property_retain_readonly_class PropertyAttributes");
        Assert.That(property_retain_readonly_class.GetPropertyAttributes(), Is.EqualTo(ObjCPropertyAttributeKind.Retain | ObjCPropertyAttributeKind.ReadOnly | ObjCPropertyAttributeKind.Atomic | ObjCPropertyAttributeKind.Class), "property_retain_readonly_class GetPropertyAttributes()");

        Assert.That(properties, Is.Empty, "All properties processed");
    }

    [Test]
    public void Type_AttributedType_WithMismatchedCXTypeKind()
    {
        // Regression test: libclang may return a CXTypeKind (e.g. CXType_ObjCId) that
        // differs from what ClangSharp expects for a given CX_TypeClass (e.g. CX_TypeClass_Attributed
        // expects CXType_Attributed). This should not crash — TypeClass is the authoritative
        // classifier from libClangSharp and should be trusted when CXTypeKind is coarser.
        //
        // This pattern occurs with nullable/attributed ObjC pointer types in system framework
        // headers. The bug manifests with the iOS SDK where id types are attributed.

        var tmpDir = Path.Combine(Path.GetTempPath(), "clangsharp-test-" + Guid.NewGuid().ToString("N")[..8]);
        _ = Directory.CreateDirectory(tmpDir);
        var tmpFile = Path.Combine(tmpDir, "test.m");
        try
        {
            File.WriteAllText(tmpFile, """
#import <Foundation/Foundation.h>

@interface TestClass : NSObject
    // nullable id creates an AttributedType where libclang returns CXType_ObjCId
    // but libClangSharp classifies it as CX_TypeClass_Attributed
    -(void) doSomethingWith:(nullable id)value;
    @property (nullable, nonatomic, strong) NSObject *obj;
@end
""");

            using var translationUnit = CreateFoundationTranslationUnit(tmpFile, ["iphoneos", "macosx"]);
            if (translationUnit is null)
            {
                Assert.Ignore("No SDK with Foundation available — skipping test");
                return;
            }

            // Deep traversal of all declarations and their types — this is where the crash occurred.
            // PointerType.PointeeType would create an AttributedType with CXType_ObjCId,
            // and the Type constructor would throw ArgumentOutOfRangeException("handle").
            foreach (var decl in translationUnit.TranslationUnitDecl.Decls)
            {
                if (decl is ObjCContainerDecl containerDecl)
                {
                    foreach (var childDecl in containerDecl.Decls)
                    {
                        if (childDecl is ObjCMethodDecl methodDecl)
                        {
                            // Accessing ReturnType and parameter Types triggers type creation
                            _ = methodDecl.ReturnType;
                            foreach (var param in methodDecl.Parameters)
                            {
                                var paramType = param.Type;
                                // Walk the type chain to trigger PointeeType access
                                if (paramType is PointerType pointerType)
                                {
                                    _ = pointerType.PointeeType;
                                }
                                else if (paramType is AttributedType attributedType)
                                {
                                    _ = attributedType.ModifiedType;
                                    _ = attributedType.EquivalentType;
                                }
                            }
                        }

                        if (childDecl is ObjCPropertyDecl propertyDecl)
                        {
                            var propType = propertyDecl.Type;
                            if (propType is AttributedType attributedType)
                            {
                                _ = attributedType.ModifiedType;
                            }
                        }
                    }
                }
            }

            // Verify we found our test class
            var classes = translationUnit.TranslationUnitDecl.Decls.OfType<ObjCInterfaceDecl>().ToList();
            Assert.That(classes.Any(v => v.Name == "TestClass"), Is.True, "TestClass should be found");
        }
        finally
        {
            Directory.Delete(tmpDir, true);
        }
    }

    [Test]
    public void Type_FullFoundationTraversal_DoesNotCrash()
    {
        // Regression test: traversing ALL declarations from a translation unit that
        // includes <Foundation/Foundation.h> should not throw ArgumentOutOfRangeException.
        // This is the exact scenario that crashed before the fix.
        // The bug manifests with the iOS SDK where attributed types have CXType_ObjCId
        // instead of CXType_Attributed.

        var tmpDir = Path.Combine(Path.GetTempPath(), "clangsharp-test-" + Guid.NewGuid().ToString("N")[..8]);
        _ = Directory.CreateDirectory(tmpDir);
        var tmpFile = Path.Combine(tmpDir, "test.m");
        try
        {
            File.WriteAllText(tmpFile, """
#import <Foundation/Foundation.h>
@interface MyClass : NSObject
@property int val;
@end
""");

            using var translationUnit = CreateFoundationTranslationUnit(tmpFile, ["iphoneos", "macosx"]);
            if (translationUnit is null)
            {
                Assert.Ignore("No SDK with Foundation available — skipping test");
                return;
            }

            // Full recursive traversal — this crashed with ArgumentOutOfRangeException("handle")
            // because AttributedType constructor rejected CXType_ObjCId as a valid kind.
            // The crash path was: RecordDecl → FieldDecl → Type.PointeeType → AttributedType
            var topLevelCount = 0;
            var childCount = 0;
            foreach (var decl in translationUnit.TranslationUnitDecl.Decls)
            {
                topLevelCount++;

                // Visit ALL container types (ObjC interfaces, protocols, categories)
                if (decl is ObjCContainerDecl containerDecl)
                {
                    foreach (var childDecl in containerDecl.Decls)
                    {
                        childCount++;
                        VisitDeclTypes(childDecl);
                    }
                }

                // Visit RecordDecls (C structs) — this is the actual crash path.
                // Foundation headers contain structs with fields that have attributed
                // pointer types (e.g. nullable id), and accessing FieldDecl.Type.PointeeType
                // triggers the AttributedType constructor with mismatched CXTypeKind.
                if (decl is RecordDecl recordDecl)
                {
                    foreach (var field in recordDecl.Decls)
                    {
                        childCount++;
                        VisitDeclTypes(field);
                    }
                }
            }

            // Foundation should have thousands of declarations
            Assert.That(topLevelCount, Is.GreaterThan(100), "Should have many top-level declarations from Foundation");
            Assert.That(childCount, Is.GreaterThan(100), "Should have many child declarations");
        }
        finally
        {
            Directory.Delete(tmpDir, true);
        }
    }

    [Test]
    public void Type_DeepTraversal_DoesNotCrash()
    {
        // Regression test: traversing all declarations in an ObjC translation unit
        // and accessing their child declarations, method return types, parameter types,
        // and property types should not throw ArgumentOutOfRangeException("handle").
        //
        // This covers the scenario where a large number of ObjC declarations are parsed
        // and the binding generator needs to recursively visit all type information.
        var inputContents = """
@class NSObject;
@class NSString;
@class NSArray;
@class NSError;

@protocol Proto1
    @required
    -(void) requiredMethod;
    @optional
    -(void) optionalMethod;
    @property (nullable) NSString *optionalProp;
@end

@protocol Proto2 <Proto1>
    -(NSString*) stringMethod:(NSArray*)array error:(NSError**)error;
@end

@interface Base
    @property int baseValue;
@end

@interface Child : Base <Proto2>
    @property (nonatomic, copy) NSString *name;
    @property (nullable, nonatomic, strong) NSArray *items;
    -(instancetype) initWithName:(NSString *)name;
    -(void) doSomething:(nullable id)param;
    +(Child*) sharedInstance;
@end

@interface Child (Category)
    -(void) categoryMethod;
    @property (readonly) int categoryProp;
@end
""";
        using var translationUnit = CreateTranslationUnit(inputContents, "objective-c++");

        // Deep traversal: access all declarations, their children, and all types.
        // This should complete without any ArgumentOutOfRangeException.
        foreach (var decl in translationUnit.TranslationUnitDecl.Decls)
        {
            // Access cursor properties
            _ = decl.CursorKindSpelling;

            if (decl is NamedDecl namedDecl)
            {
                _ = namedDecl.Name;
            }

            // Recurse into containers
            if (decl is ObjCContainerDecl containerDecl)
            {
                foreach (var childDecl in containerDecl.Decls)
                {
                    _ = childDecl.CursorKindSpelling;

                    if (childDecl is ObjCMethodDecl methodDecl)
                    {
                        _ = methodDecl.ReturnType;
                        foreach (var param in methodDecl.Parameters)
                        {
                            _ = param.Type;
                            _ = param.Name;
                        }
                    }

                    if (childDecl is ObjCPropertyDecl propertyDecl)
                    {
                        _ = propertyDecl.Type;
                        _ = propertyDecl.Name;
                    }
                }
            }
        }

        // If we get here without throwing, the test passes
        var classes = translationUnit.TranslationUnitDecl.Decls.OfType<ObjCInterfaceDecl>().ToList();
        Assert.That(classes.Count, Is.GreaterThanOrEqualTo(2), "Should have at least Base and Child classes");
    }

    [Test]
    public void Decl_InvalidDeclKind_DoesNotCrash()
    {
        // Regression test: when libClangSharp returns CX_DeclKind_Invalid for a cursor
        // that libclang considers a valid declaration, Decl.Create() should create a
        // generic Decl wrapper instead of throwing ArgumentOutOfRangeException.
        //
        // This is tested indirectly by ensuring that complex ObjC code with diverse
        // declaration kinds can be fully traversed without crashes.
        var inputContents = """
@class NSObject;

@protocol DelegateProtocol
    @required
    -(void) didFinish;
    @optional
    -(void) didFail;
@end

@interface Manager
    @property (weak) id<DelegateProtocol> delegate;
    -(void) start;
@end

// Categories add diverse declaration kinds to the AST
@interface Manager (Extensions)
    -(void) reset;
@end

@interface Manager (MoreExtensions)
    @property (readonly) int state;
@end
""";
        using var translationUnit = CreateTranslationUnit(inputContents, "objective-c++");

        // Walk the entire AST including child decls — this exercises Decl.Create()
        // for diverse declaration kinds
        var declCount = 0;
        foreach (var decl in translationUnit.TranslationUnitDecl.Decls)
        {
            declCount++;
            foreach (var child in decl.CursorChildren)
            {
                _ = child.CursorKindSpelling;
            }
        }

        Assert.That(declCount, Is.GreaterThan(0), "Should have declarations");

        var classes = translationUnit.TranslationUnitDecl.Decls.OfType<ObjCInterfaceDecl>().ToList();
        Assert.That(classes.Any(v => v.Name == "Manager"), Is.True, "Manager class found");

        var protocols = translationUnit.TranslationUnitDecl.Decls.OfType<ObjCProtocolDecl>().ToList();
        Assert.That(protocols.Any(v => v.Name == "DelegateProtocol"), Is.True, "DelegateProtocol found");

        var categories = translationUnit.TranslationUnitDecl.Decls.OfType<ObjCCategoryDecl>().ToList();
        Assert.That(categories.Count, Is.GreaterThanOrEqualTo(2), "At least 2 categories");
    }

    private static string? GetMacOSSdkPath() => GetSdkPath("macosx");

    private static string? GetIPhoneOSSdkPath() => GetSdkPath("iphoneos");

    /// <summary>
    /// Create a TranslationUnit that imports Foundation, trying available SDKs.
    /// Returns null if no SDK can successfully compile the input.
    /// </summary>
    private static TranslationUnit? CreateFoundationTranslationUnit(string sourceFile, string[] sdksToTry)
    {
        var clangResourceDir = GetClangResourceDir();

        foreach (var sdk in sdksToTry)
        {
            var sdkPath = GetSdkPath(sdk);
            if (string.IsNullOrEmpty(sdkPath))
            {
                continue;
            }

            var index = CXIndex.Create();
            var args = new List<string> { "-isysroot", sdkPath, "-xobjective-c", "-fobjc-arc" };
            if (!string.IsNullOrEmpty(clangResourceDir))
            {
                args.AddRange(["-resource-dir", clangResourceDir]);
            }

            var errorCode = CXTranslationUnit.TryParse(index, sourceFile, args.ToArray(), [], DefaultTranslationUnitFlags, out var handle);

            if (errorCode != CXErrorCode.CXError_Success)
            {
                continue;
            }

            var tu = TranslationUnit.GetOrCreate(handle);

            // Check for fatal compilation errors (e.g. missing headers)
            var hasFatalErrors = false;
            for (uint i = 0; i < tu.Handle.NumDiagnostics; i++)
            {
                using var diag = tu.Handle.GetDiagnostic(i);
                if (diag.Severity >= CXDiagnosticSeverity.CXDiagnostic_Fatal)
                {
                    hasFatalErrors = true;
                    break;
                }
            }

            if (!hasFatalErrors)
            {
                return tu;
            }

            tu.Dispose();
        }

        return null;
    }

    private static string? GetClangResourceDir()
    {
        try
        {
            var psi = new System.Diagnostics.ProcessStartInfo
            {
                FileName = "xcrun",
                ArgumentList = { "clang", "--print-resource-dir" },
                RedirectStandardOutput = true,
                UseShellExecute = false,
            };
            using var process = System.Diagnostics.Process.Start(psi);
            var output = process?.StandardOutput.ReadToEnd().Trim();
            process?.WaitForExit();
            if (process?.ExitCode == 0 && !string.IsNullOrEmpty(output) && Directory.Exists(output))
            {
                return output;
            }
        }
        catch (InvalidOperationException)
        {
            // xcrun not available
        }

        return null;
    }

    private static string? GetSdkPath(string sdk)
    {
        try
        {
            var psi = new System.Diagnostics.ProcessStartInfo
            {
                FileName = "xcrun",
                ArgumentList = { "--show-sdk-path", "--sdk", sdk },
                RedirectStandardOutput = true,
                UseShellExecute = false,
            };
            using var process = System.Diagnostics.Process.Start(psi);
            var output = process?.StandardOutput.ReadToEnd().Trim();
            process?.WaitForExit();
            if (process?.ExitCode == 0 && !string.IsNullOrEmpty(output) && Directory.Exists(output))
            {
                return output;
            }
        }
        catch (InvalidOperationException)
        {
            // xcrun not available
        }

        return null;
    }

    /// <summary>
    /// Recursively visit a declaration's type information, including parameter types,
    /// return types, and property types. This exercises the ClangSharp type creation
    /// path that can crash with ArgumentOutOfRangeException("handle") when a type's
    /// CXTypeKind doesn't match what ClangSharp expects for its CX_TypeClass.
    /// </summary>
    private static void VisitDeclTypes(Decl decl)
    {
        if (decl is ObjCMethodDecl methodDecl)
        {
            _ = methodDecl.ReturnType;
            foreach (var param in methodDecl.Parameters)
            {
                WalkType(param.Type);
            }
        }
        else if (decl is ObjCPropertyDecl propertyDecl)
        {
            WalkType(propertyDecl.Type);
        }
        else if (decl is FieldDecl fieldDecl)
        {
            WalkType(fieldDecl.Type);
        }
        else if (decl is VarDecl varDecl)
        {
            WalkType(varDecl.Type);
        }
    }

    /// <summary>
    /// Walk a type's structure to force ClangSharp to materialize related types.
    /// This triggers PointeeType, ModifiedType, etc. which are lazily evaluated
    /// and can crash if CXTypeKind doesn't match the expected kind.
    /// </summary>
    private static void WalkType(Type type)
    {
        if (type is PointerType pointerType)
        {
            _ = pointerType.PointeeType;
        }
        else if (type is AttributedType attributedType)
        {
            _ = attributedType.ModifiedType;
            _ = attributedType.EquivalentType;
        }
        else if (type is ObjCObjectPointerType objcPointerType)
        {
            _ = objcPointerType.PointeeType;
        }
    }
}
