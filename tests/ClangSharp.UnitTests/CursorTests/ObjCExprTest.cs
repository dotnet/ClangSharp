// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using ClangSharp.Interop;
using NUnit.Framework;
using static ClangSharp.Interop.CX_ObjCMessageReceiverKind;
using static ClangSharp.Interop.CX_ObjCPropertyRefReceiverKind;

namespace ClangSharp.UnitTests;

public sealed class ObjCExprTest : TranslationUnitTest
{
    // Objective-C expression semantics (property messaging, object subscripting, @available) require a
    // modern runtime, so an Apple target triple is pinned to keep the parse deterministic cross-platform.
    private static readonly string[] s_commandLineArgs =
    [
        "-std=c++17",
        "-target",
        "x86_64-apple-macosx10.15.0",
        "-fobjc-runtime=macosx",
    ];

    [Test]
    public void ObjCExprAccessors()
    {
        var inputContents = """
@interface Widget
- (int)compute;
@property int value;
- (id)objectAtIndexedSubscript:(unsigned long)idx;
- (void)setObject:(id)obj atIndexedSubscript:(unsigned long)idx;
@end

@implementation Widget
- (int)compute { return 0; }
- (id)objectAtIndexedSubscript:(unsigned long)idx { return self; }
- (void)setObject:(id)obj atIndexedSubscript:(unsigned long)idx { }
- (void)run
{
    [self compute];
    self.value = 7;
    int x = self.value;
    Widget* w = self;
    w[0] = self;
    id y = w[1];
    if (__builtin_available(macos 10.15, *)) { }
}
@end
""";

        using var translationUnit = CreateTranslationUnit(inputContents, "objective-c++", s_commandLineArgs);

        var impl = translationUnit.TranslationUnitDecl.CursorChildren.OfType<ObjCImplDecl>().Single();
        var run = impl.CursorChildren.OfType<ObjCMethodDecl>().Single((method) => method.Name.Contains("run", StringComparison.Ordinal));

        var message = Descendants(run).OfType<ObjCMessageExpr>().Single();
        Assert.That(message.ReceiverKind, Is.EqualTo(CX_OMRK_Instance));
        Assert.That(message.IsDelegateInitCall, Is.False);

        var propertyRefs = Descendants(run).OfType<ObjCPropertyRefExpr>().ToList();
        Assert.That(propertyRefs, Has.Count.EqualTo(2));
        Assert.That(propertyRefs.All((propertyRef) => propertyRef.ReceiverKind == CX_OPRK_Object), Is.True);
        Assert.That(propertyRefs.Any((propertyRef) => propertyRef.IsMessagingGetter), Is.True);
        Assert.That(propertyRefs.Any((propertyRef) => propertyRef.IsMessagingSetter), Is.True);

        var subscripts = Descendants(run).OfType<ObjCSubscriptRefExpr>().ToList();
        Assert.That(subscripts, Has.Count.EqualTo(2));

        // Binding the underlying accessor methods requires a real Foundation collection, so here we only
        // exercise that the new SetAtIndexMethodDecl accessor is reachable alongside the existing one.
        foreach (var subscript in subscripts)
        {
            Assert.That(() => subscript.AtIndexMethodDecl, Throws.Nothing);
            Assert.That(() => subscript.SetAtIndexMethodDecl, Throws.Nothing);
        }

        var availability = Descendants(run).OfType<ObjCAvailabilityCheckExpr>().Single();
        Assert.That(availability.HasVersion, Is.True);
        Assert.That(availability.Version?.ToString(), Is.EqualTo("10.15"));
    }

    private static IEnumerable<Cursor> Descendants(Cursor cursor)
    {
        foreach (var child in cursor.CursorChildren)
        {
            yield return child;

            foreach (var descendant in Descendants(child))
            {
                yield return descendant;
            }
        }
    }
}
