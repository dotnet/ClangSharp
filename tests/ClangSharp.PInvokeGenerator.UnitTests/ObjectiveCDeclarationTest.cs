// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Threading.Tasks;
using NUnit.Framework;

namespace ClangSharp.UnitTests;

/// <summary>
/// Covers Objective-C binding generation (issue https://github.com/dotnet/ClangSharp/issues/254):
/// an opt-in <c>--generate-objective-c-bindings</c> flag that maps an <c>@protocol</c> or
/// <c>@interface</c> onto a layout-faithful struct (first field <c>isa</c>, mirroring
/// <c>struct objc_object</c>) plus a raw <c>Selectors</c> cache and friendly <c>objc_msgSend</c>
/// members, alongside a small <c>libobjc</c> support surface. Instance members dispatch on <c>this</c>;
/// class (<c>+</c>) members are <c>static</c> and dispatch on the backing <c>Class</c> object.
/// <c>@optional</c> protocol members are flagged, and only scalar/pointer/void signatures are in
/// scope; anything outside that subset is diagnosed rather than emitted with a wrong ABI.
/// </summary>
public sealed class ObjectiveCDeclarationTest : PInvokeGeneratorTest
{
    private const string ProtocolInputContents = @"@protocol Greeter
- (void)hello;
- (int)addLeft:(int)left right:(int)right;
@end
";

    private const string ProtocolExpectedOutputContents = @"using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace ClangSharp.Test
{
    [NativeTypeName(""@protocol Greeter"")]
    public unsafe partial struct Greeter
    {
        [NativeTypeName(""Class"")]
        public void* isa;

        public static class Selectors
        {
            public static readonly SEL hello = ObjectiveC.sel_registerName(""hello"");
            public static readonly SEL addLeft_right_ = ObjectiveC.sel_registerName(""addLeft:right:"");
        }

        public void hello() => ((delegate* unmanaged<Greeter*, SEL, void>)ObjectiveC.objc_msgSend)((Greeter*)Unsafe.AsPointer(ref this), Selectors.hello);

        public int addLeft_right_(int left, int right) => ((delegate* unmanaged<Greeter*, SEL, int, int, int>)ObjectiveC.objc_msgSend)((Greeter*)Unsafe.AsPointer(ref this), Selectors.addLeft_right_, left, right);
    }

    /// <summary>Represents an Objective-C selector (<c>SEL</c>).</summary>
    public unsafe partial struct SEL : IEquatable<SEL>
    {
        public void* Value;

        public SEL(void* value)
        {
            Value = value;
        }

        public static bool operator ==(SEL left, SEL right) => left.Value == right.Value;

        public static bool operator !=(SEL left, SEL right) => left.Value != right.Value;

        public override bool Equals(object? obj) => (obj is SEL other) && Equals(other);

        public bool Equals(SEL other) => Value == other.Value;

        public override int GetHashCode() => ((nuint)Value).GetHashCode();
    }

    /// <summary>Provides access to the Objective-C runtime (<c>libobjc</c>).</summary>
    public static unsafe partial class ObjectiveC
    {
        private const string LibObjC = ""/usr/lib/libobjc.A.dylib"";

        /// <summary>The raw <c>objc_msgSend</c> entry point, cast per call site to the selector's signature.</summary>
        public static readonly void* objc_msgSend = (void*)NativeLibrary.GetExport(NativeLibrary.Load(LibObjC), ""objc_msgSend"");

        [DllImport(LibObjC, EntryPoint = ""sel_registerName"", ExactSpelling = true)]
        public static extern SEL sel_registerName([MarshalAs(UnmanagedType.LPUTF8Str)] string name);

        [DllImport(LibObjC, EntryPoint = ""objc_getClass"", ExactSpelling = true)]
        public static extern void* objc_getClass([MarshalAs(UnmanagedType.LPUTF8Str)] string name);
    }
}
";

    private static readonly string[] s_objectiveCCommandLineArgs = ["-xobjective-c"];

    [Test]
    public Task ProtocolWindowsTest()
    {
        return ValidateGeneratedCSharpLatestWindowsBindingsAsync(ProtocolInputContents, ProtocolExpectedOutputContents, additionalConfigOptions: PInvokeGeneratorConfigurationOptions.GenerateObjectiveCBindings, commandLineArgs: s_objectiveCCommandLineArgs, language: "objective-c", languageStandard: DefaultCStandard);
    }

    [Test]
    public Task ProtocolUnixTest()
    {
        // The first-slice output uses only platform-independent types (`void*`, `int`) and the same
        // `objc_msgSend` entry point on every target, so the Windows and Unix bindings are identical.
        return ValidateGeneratedCSharpLatestUnixBindingsAsync(ProtocolInputContents, ProtocolExpectedOutputContents, additionalConfigOptions: PInvokeGeneratorConfigurationOptions.GenerateObjectiveCBindings, commandLineArgs: s_objectiveCCommandLineArgs, language: "objective-c", languageStandard: DefaultCStandard);
    }

    [Test]
    public Task UnsupportedMethodIsDiagnosedTest()
    {
        var inputContents = @"@protocol Logger
- (void)logValue:(int)value, ...;
- (int)count;
@end
";

        // The variadic method is outside the v1 subset, so it is diagnosed and skipped; the in-subset
        // `count` method is still emitted.
        var expectedOutputContents = @"using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace ClangSharp.Test
{
    [NativeTypeName(""@protocol Logger"")]
    public unsafe partial struct Logger
    {
        [NativeTypeName(""Class"")]
        public void* isa;

        public static class Selectors
        {
            public static readonly SEL count = ObjectiveC.sel_registerName(""count"");
        }

        public int count() => ((delegate* unmanaged<Logger*, SEL, int>)ObjectiveC.objc_msgSend)((Logger*)Unsafe.AsPointer(ref this), Selectors.count);
    }

    /// <summary>Represents an Objective-C selector (<c>SEL</c>).</summary>
    public unsafe partial struct SEL : IEquatable<SEL>
    {
        public void* Value;

        public SEL(void* value)
        {
            Value = value;
        }

        public static bool operator ==(SEL left, SEL right) => left.Value == right.Value;

        public static bool operator !=(SEL left, SEL right) => left.Value != right.Value;

        public override bool Equals(object? obj) => (obj is SEL other) && Equals(other);

        public bool Equals(SEL other) => Value == other.Value;

        public override int GetHashCode() => ((nuint)Value).GetHashCode();
    }

    /// <summary>Provides access to the Objective-C runtime (<c>libobjc</c>).</summary>
    public static unsafe partial class ObjectiveC
    {
        private const string LibObjC = ""/usr/lib/libobjc.A.dylib"";

        /// <summary>The raw <c>objc_msgSend</c> entry point, cast per call site to the selector's signature.</summary>
        public static readonly void* objc_msgSend = (void*)NativeLibrary.GetExport(NativeLibrary.Load(LibObjC), ""objc_msgSend"");

        [DllImport(LibObjC, EntryPoint = ""sel_registerName"", ExactSpelling = true)]
        public static extern SEL sel_registerName([MarshalAs(UnmanagedType.LPUTF8Str)] string name);

        [DllImport(LibObjC, EntryPoint = ""objc_getClass"", ExactSpelling = true)]
        public static extern void* objc_getClass([MarshalAs(UnmanagedType.LPUTF8Str)] string name);
    }
}
";

        var expectedDiagnostics = new[] {
            new Diagnostic(DiagnosticLevel.Warning, "Objective-C method 'logValue:' on '@protocol Logger' is not supported: variadic methods are not yet supported. It was skipped.", "Line 2, Column 9 in ClangUnsavedFile.h")
        };

        return ValidateGeneratedCSharpLatestWindowsBindingsAsync(inputContents, expectedOutputContents, additionalConfigOptions: PInvokeGeneratorConfigurationOptions.GenerateObjectiveCBindings, expectedDiagnostics: expectedDiagnostics, commandLineArgs: s_objectiveCCommandLineArgs, language: "objective-c", languageStandard: DefaultCStandard);
    }

    [Test]
    public Task PropertyTest()
    {
        var inputContents = @"@protocol Counter
@property (readonly) int count;
@property int step;
@end
";

        // A `readonly` property maps to an expression-bodied getter; a read-write property maps to a
        // `get`/`set` block whose setter dispatches the synthesized `setStep:` selector.
        var expectedOutputContents = @"using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace ClangSharp.Test
{
    [NativeTypeName(""@protocol Counter"")]
    public unsafe partial struct Counter
    {
        [NativeTypeName(""Class"")]
        public void* isa;

        public static class Selectors
        {
            public static readonly SEL count = ObjectiveC.sel_registerName(""count"");
            public static readonly SEL step = ObjectiveC.sel_registerName(""step"");
            public static readonly SEL setStep_ = ObjectiveC.sel_registerName(""setStep:"");
        }

        public int count => ((delegate* unmanaged<Counter*, SEL, int>)ObjectiveC.objc_msgSend)((Counter*)Unsafe.AsPointer(ref this), Selectors.count);

        public int step
        {
            get => ((delegate* unmanaged<Counter*, SEL, int>)ObjectiveC.objc_msgSend)((Counter*)Unsafe.AsPointer(ref this), Selectors.step);
            set => ((delegate* unmanaged<Counter*, SEL, int, void>)ObjectiveC.objc_msgSend)((Counter*)Unsafe.AsPointer(ref this), Selectors.setStep_, value);
        }
    }

    /// <summary>Represents an Objective-C selector (<c>SEL</c>).</summary>
    public unsafe partial struct SEL : IEquatable<SEL>
    {
        public void* Value;

        public SEL(void* value)
        {
            Value = value;
        }

        public static bool operator ==(SEL left, SEL right) => left.Value == right.Value;

        public static bool operator !=(SEL left, SEL right) => left.Value != right.Value;

        public override bool Equals(object? obj) => (obj is SEL other) && Equals(other);

        public bool Equals(SEL other) => Value == other.Value;

        public override int GetHashCode() => ((nuint)Value).GetHashCode();
    }

    /// <summary>Provides access to the Objective-C runtime (<c>libobjc</c>).</summary>
    public static unsafe partial class ObjectiveC
    {
        private const string LibObjC = ""/usr/lib/libobjc.A.dylib"";

        /// <summary>The raw <c>objc_msgSend</c> entry point, cast per call site to the selector's signature.</summary>
        public static readonly void* objc_msgSend = (void*)NativeLibrary.GetExport(NativeLibrary.Load(LibObjC), ""objc_msgSend"");

        [DllImport(LibObjC, EntryPoint = ""sel_registerName"", ExactSpelling = true)]
        public static extern SEL sel_registerName([MarshalAs(UnmanagedType.LPUTF8Str)] string name);

        [DllImport(LibObjC, EntryPoint = ""objc_getClass"", ExactSpelling = true)]
        public static extern void* objc_getClass([MarshalAs(UnmanagedType.LPUTF8Str)] string name);
    }
}
";

        return ValidateGeneratedCSharpLatestWindowsBindingsAsync(inputContents, expectedOutputContents, additionalConfigOptions: PInvokeGeneratorConfigurationOptions.GenerateObjectiveCBindings, commandLineArgs: s_objectiveCCommandLineArgs, language: "objective-c", languageStandard: DefaultCStandard);
    }

    [Test]
    public Task InterfaceTest()
    {
        var inputContents = @"@interface Base
- (void)baseMethod;
@end

@interface Widget : Base
@property (class, readonly) int sharedCount;
@property int width;
+ (int)defaultWidth;
- (void)render;
@end
";

        // An `@interface` maps to the same opaque `{ void* isa }` struct as an `@protocol` (Objective-C
        // uses non-fragile ivars, so there is no stable field layout to mirror), plus a cached `Class`
        // object. Class (`+`) members are `static` and dispatch on `Class`; the superclass is recorded
        // in the `[NativeTypeName]` since every struct shares the same layout and can be reinterpreted.
        var expectedOutputContents = @"using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace ClangSharp.Test
{
    [NativeTypeName(""@interface Base"")]
    public unsafe partial struct Base
    {
        [NativeTypeName(""Class"")]
        public void* isa;

        public static readonly void* Class = ObjectiveC.objc_getClass(""Base"");

        public static class Selectors
        {
            public static readonly SEL baseMethod = ObjectiveC.sel_registerName(""baseMethod"");
        }

        public void baseMethod() => ((delegate* unmanaged<Base*, SEL, void>)ObjectiveC.objc_msgSend)((Base*)Unsafe.AsPointer(ref this), Selectors.baseMethod);
    }

    [NativeTypeName(""@interface Widget : Base"")]
    public unsafe partial struct Widget
    {
        [NativeTypeName(""Class"")]
        public void* isa;

        public static readonly void* Class = ObjectiveC.objc_getClass(""Widget"");

        public static class Selectors
        {
            public static readonly SEL render = ObjectiveC.sel_registerName(""render"");
            public static readonly SEL width = ObjectiveC.sel_registerName(""width"");
            public static readonly SEL setWidth_ = ObjectiveC.sel_registerName(""setWidth:"");
            public static readonly SEL defaultWidth = ObjectiveC.sel_registerName(""defaultWidth"");
            public static readonly SEL sharedCount = ObjectiveC.sel_registerName(""sharedCount"");
        }

        public void render() => ((delegate* unmanaged<Widget*, SEL, void>)ObjectiveC.objc_msgSend)((Widget*)Unsafe.AsPointer(ref this), Selectors.render);

        public static int defaultWidth() => ((delegate* unmanaged<void*, SEL, int>)ObjectiveC.objc_msgSend)(Class, Selectors.defaultWidth);

        public int width
        {
            get => ((delegate* unmanaged<Widget*, SEL, int>)ObjectiveC.objc_msgSend)((Widget*)Unsafe.AsPointer(ref this), Selectors.width);
            set => ((delegate* unmanaged<Widget*, SEL, int, void>)ObjectiveC.objc_msgSend)((Widget*)Unsafe.AsPointer(ref this), Selectors.setWidth_, value);
        }

        public static int sharedCount => ((delegate* unmanaged<void*, SEL, int>)ObjectiveC.objc_msgSend)(Class, Selectors.sharedCount);
    }

    /// <summary>Represents an Objective-C selector (<c>SEL</c>).</summary>
    public unsafe partial struct SEL : IEquatable<SEL>
    {
        public void* Value;

        public SEL(void* value)
        {
            Value = value;
        }

        public static bool operator ==(SEL left, SEL right) => left.Value == right.Value;

        public static bool operator !=(SEL left, SEL right) => left.Value != right.Value;

        public override bool Equals(object? obj) => (obj is SEL other) && Equals(other);

        public bool Equals(SEL other) => Value == other.Value;

        public override int GetHashCode() => ((nuint)Value).GetHashCode();
    }

    /// <summary>Provides access to the Objective-C runtime (<c>libobjc</c>).</summary>
    public static unsafe partial class ObjectiveC
    {
        private const string LibObjC = ""/usr/lib/libobjc.A.dylib"";

        /// <summary>The raw <c>objc_msgSend</c> entry point, cast per call site to the selector's signature.</summary>
        public static readonly void* objc_msgSend = (void*)NativeLibrary.GetExport(NativeLibrary.Load(LibObjC), ""objc_msgSend"");

        [DllImport(LibObjC, EntryPoint = ""sel_registerName"", ExactSpelling = true)]
        public static extern SEL sel_registerName([MarshalAs(UnmanagedType.LPUTF8Str)] string name);

        [DllImport(LibObjC, EntryPoint = ""objc_getClass"", ExactSpelling = true)]
        public static extern void* objc_getClass([MarshalAs(UnmanagedType.LPUTF8Str)] string name);
    }
}
";

        return ValidateGeneratedCSharpLatestWindowsBindingsAsync(inputContents, expectedOutputContents, additionalConfigOptions: PInvokeGeneratorConfigurationOptions.GenerateObjectiveCBindings, commandLineArgs: s_objectiveCCommandLineArgs, language: "objective-c", languageStandard: DefaultCStandard);
    }

    [Test]
    public Task OptionalMemberTest()
    {
        var inputContents = @"@protocol Named
@required
- (int)identifier;
@optional
- (void)greet;
@property (readonly) int badge;
@end
";

        // Optional members are still emitted, but flagged: a conforming object may not implement them,
        // so callers must guard with `respondsToSelector:` before sending.
        var expectedOutputContents = @"using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace ClangSharp.Test
{
    [NativeTypeName(""@protocol Named"")]
    public unsafe partial struct Named
    {
        [NativeTypeName(""Class"")]
        public void* isa;

        public static class Selectors
        {
            public static readonly SEL identifier = ObjectiveC.sel_registerName(""identifier"");
            public static readonly SEL greet = ObjectiveC.sel_registerName(""greet"");
            public static readonly SEL badge = ObjectiveC.sel_registerName(""badge"");
        }

        public int identifier() => ((delegate* unmanaged<Named*, SEL, int>)ObjectiveC.objc_msgSend)((Named*)Unsafe.AsPointer(ref this), Selectors.identifier);

        // @optional; guard with respondsToSelector: before sending.
        public void greet() => ((delegate* unmanaged<Named*, SEL, void>)ObjectiveC.objc_msgSend)((Named*)Unsafe.AsPointer(ref this), Selectors.greet);

        // @optional; guard with respondsToSelector: before sending.
        public int badge => ((delegate* unmanaged<Named*, SEL, int>)ObjectiveC.objc_msgSend)((Named*)Unsafe.AsPointer(ref this), Selectors.badge);
    }

    /// <summary>Represents an Objective-C selector (<c>SEL</c>).</summary>
    public unsafe partial struct SEL : IEquatable<SEL>
    {
        public void* Value;

        public SEL(void* value)
        {
            Value = value;
        }

        public static bool operator ==(SEL left, SEL right) => left.Value == right.Value;

        public static bool operator !=(SEL left, SEL right) => left.Value != right.Value;

        public override bool Equals(object? obj) => (obj is SEL other) && Equals(other);

        public bool Equals(SEL other) => Value == other.Value;

        public override int GetHashCode() => ((nuint)Value).GetHashCode();
    }

    /// <summary>Provides access to the Objective-C runtime (<c>libobjc</c>).</summary>
    public static unsafe partial class ObjectiveC
    {
        private const string LibObjC = ""/usr/lib/libobjc.A.dylib"";

        /// <summary>The raw <c>objc_msgSend</c> entry point, cast per call site to the selector's signature.</summary>
        public static readonly void* objc_msgSend = (void*)NativeLibrary.GetExport(NativeLibrary.Load(LibObjC), ""objc_msgSend"");

        [DllImport(LibObjC, EntryPoint = ""sel_registerName"", ExactSpelling = true)]
        public static extern SEL sel_registerName([MarshalAs(UnmanagedType.LPUTF8Str)] string name);

        [DllImport(LibObjC, EntryPoint = ""objc_getClass"", ExactSpelling = true)]
        public static extern void* objc_getClass([MarshalAs(UnmanagedType.LPUTF8Str)] string name);
    }
}
";

        return ValidateGeneratedCSharpLatestWindowsBindingsAsync(inputContents, expectedOutputContents, additionalConfigOptions: PInvokeGeneratorConfigurationOptions.GenerateObjectiveCBindings, commandLineArgs: s_objectiveCCommandLineArgs, language: "objective-c", languageStandard: DefaultCStandard);
    }
}
