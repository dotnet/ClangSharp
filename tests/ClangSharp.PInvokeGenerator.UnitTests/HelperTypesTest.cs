// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Threading.Tasks;
using NUnit.Framework;

namespace ClangSharp.UnitTests;

/// <summary>
/// Regression tests for https://github.com/dotnet/ClangSharp/issues/657 and
/// https://github.com/dotnet/ClangSharp/issues/429.
/// When <c>generate-helper-types</c> is used with a single output file, the helper types must be
/// emitted inside the shared namespace with their usings hoisted to the top of the file, rather than
/// each helper emitting its own <c>using</c> directives and <c>namespace</c> wrapper (which produced
/// invalid C# with nested namespaces, usings after a namespace, and a stray trailing brace). The
/// helper types must also be emitted at the bottom of the shared namespace, after the deferred method
/// class, rather than in the middle of the generated output.
/// </summary>
[Platform("win")]
public sealed class HelperTypesTest : PInvokeGeneratorTest
{
    [Test]
    public Task HelperTypesEmittedInSharedNamespace()
    {
        var inputContents = @"struct SRC_DATA
{
    const float *data_in;
    long input_frames;
};
";

        var expectedOutputContents = @"using System;
using System.Diagnostics;

namespace ClangSharp.Test
{
    public unsafe partial struct SRC_DATA
    {
        [NativeTypeName(""const float *"")]
        public float* data_in;

        [NativeTypeName(""long"")]
        public int input_frames;
    }

    /// <summary>Defines the type of a member as it was used in the native signature.</summary>
    [AttributeUsage(AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.ReturnValue, AllowMultiple = false, Inherited = true)]
    [Conditional(""DEBUG"")]
    internal sealed partial class NativeTypeNameAttribute : Attribute
    {
        private readonly string _name;

        /// <summary>Initializes a new instance of the <see cref=""NativeTypeNameAttribute"" /> class.</summary>
        /// <param name=""name"">The name of the type that was used in the native signature.</param>
        public NativeTypeNameAttribute(string name)
        {
            _name = name;
        }

        /// <summary>Gets the name of the type that was used in the native signature.</summary>
        public string Name => _name;
    }

    /// <summary>Defines the annotation found in a native declaration.</summary>
    [AttributeUsage(AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.ReturnValue, AllowMultiple = true, Inherited = false)]
    [Conditional(""DEBUG"")]
    internal sealed partial class NativeAnnotationAttribute : Attribute
    {
        private readonly string _annotation;

        /// <summary>Initializes a new instance of the <see cref=""NativeAnnotationAttribute"" /> class.</summary>
        /// <param name=""annotation"">The annotation that was used in the native declaration.</param>
        public NativeAnnotationAttribute(string annotation)
        {
            _annotation = annotation;
        }

        /// <summary>Gets the annotation that was used in the native declaration.</summary>
        public string Annotation => _annotation;
    }
}
";

        return ValidateGeneratedCSharpLatestWindowsBindingsAsync(inputContents, expectedOutputContents, additionalConfigOptions: PInvokeGeneratorConfigurationOptions.GenerateHelperTypes);
    }

    [Test]
    public Task HelperTypesEmittedInSharedFileScopedNamespace()
    {
        var inputContents = @"struct SRC_DATA
{
    const float *data_in;
    long input_frames;
};
";

        var expectedOutputContents = @"using System;
using System.Diagnostics;

namespace ClangSharp.Test;

public unsafe partial struct SRC_DATA
{
    [NativeTypeName(""const float *"")]
    public float* data_in;

    [NativeTypeName(""long"")]
    public int input_frames;
}

/// <summary>Defines the type of a member as it was used in the native signature.</summary>
[AttributeUsage(AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.ReturnValue, AllowMultiple = false, Inherited = true)]
[Conditional(""DEBUG"")]
internal sealed partial class NativeTypeNameAttribute : Attribute
{
    private readonly string _name;

    /// <summary>Initializes a new instance of the <see cref=""NativeTypeNameAttribute"" /> class.</summary>
    /// <param name=""name"">The name of the type that was used in the native signature.</param>
    public NativeTypeNameAttribute(string name)
    {
        _name = name;
    }

    /// <summary>Gets the name of the type that was used in the native signature.</summary>
    public string Name => _name;
}

/// <summary>Defines the annotation found in a native declaration.</summary>
[AttributeUsage(AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.ReturnValue, AllowMultiple = true, Inherited = false)]
[Conditional(""DEBUG"")]
internal sealed partial class NativeAnnotationAttribute : Attribute
{
    private readonly string _annotation;

    /// <summary>Initializes a new instance of the <see cref=""NativeAnnotationAttribute"" /> class.</summary>
    /// <param name=""annotation"">The annotation that was used in the native declaration.</param>
    public NativeAnnotationAttribute(string annotation)
    {
        _annotation = annotation;
    }

    /// <summary>Gets the annotation that was used in the native declaration.</summary>
    public string Annotation => _annotation;
}
";

        return ValidateGeneratedCSharpLatestWindowsBindingsAsync(inputContents, expectedOutputContents, additionalConfigOptions: PInvokeGeneratorConfigurationOptions.GenerateHelperTypes | PInvokeGeneratorConfigurationOptions.GenerateFileScopedNamespaces);
    }

    [Test]
    public Task HelperTypesEmittedAfterMethodClass()
    {
        var inputContents = @"struct SRC_DATA
{
    const float *data_in;
    long input_frames;
};

extern ""C"" void MyFunction();
";

        var expectedOutputContents = @"using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace ClangSharp.Test
{
    public unsafe partial struct SRC_DATA
    {
        [NativeTypeName(""const float *"")]
        public float* data_in;

        [NativeTypeName(""long"")]
        public int input_frames;
    }

    public static partial class Methods
    {
        [DllImport(""ClangSharpPInvokeGenerator"", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void MyFunction();
    }

    /// <summary>Defines the type of a member as it was used in the native signature.</summary>
    [AttributeUsage(AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.ReturnValue, AllowMultiple = false, Inherited = true)]
    [Conditional(""DEBUG"")]
    internal sealed partial class NativeTypeNameAttribute : Attribute
    {
        private readonly string _name;

        /// <summary>Initializes a new instance of the <see cref=""NativeTypeNameAttribute"" /> class.</summary>
        /// <param name=""name"">The name of the type that was used in the native signature.</param>
        public NativeTypeNameAttribute(string name)
        {
            _name = name;
        }

        /// <summary>Gets the name of the type that was used in the native signature.</summary>
        public string Name => _name;
    }

    /// <summary>Defines the annotation found in a native declaration.</summary>
    [AttributeUsage(AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.ReturnValue, AllowMultiple = true, Inherited = false)]
    [Conditional(""DEBUG"")]
    internal sealed partial class NativeAnnotationAttribute : Attribute
    {
        private readonly string _annotation;

        /// <summary>Initializes a new instance of the <see cref=""NativeAnnotationAttribute"" /> class.</summary>
        /// <param name=""annotation"">The annotation that was used in the native declaration.</param>
        public NativeAnnotationAttribute(string annotation)
        {
            _annotation = annotation;
        }

        /// <summary>Gets the annotation that was used in the native declaration.</summary>
        public string Annotation => _annotation;
    }
}
";

        return ValidateGeneratedCSharpLatestWindowsBindingsAsync(inputContents, expectedOutputContents, additionalConfigOptions: PInvokeGeneratorConfigurationOptions.GenerateHelperTypes);
    }

    [Test]
    public Task HelperTypesEmittedAfterMethodClassFileScoped()
    {
        var inputContents = @"struct SRC_DATA
{
    const float *data_in;
    long input_frames;
};

extern ""C"" void MyFunction();
";

        var expectedOutputContents = @"using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace ClangSharp.Test;

public unsafe partial struct SRC_DATA
{
    [NativeTypeName(""const float *"")]
    public float* data_in;

    [NativeTypeName(""long"")]
    public int input_frames;
}

public static partial class Methods
{
    [DllImport(""ClangSharpPInvokeGenerator"", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern void MyFunction();
}

/// <summary>Defines the type of a member as it was used in the native signature.</summary>
[AttributeUsage(AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.ReturnValue, AllowMultiple = false, Inherited = true)]
[Conditional(""DEBUG"")]
internal sealed partial class NativeTypeNameAttribute : Attribute
{
    private readonly string _name;

    /// <summary>Initializes a new instance of the <see cref=""NativeTypeNameAttribute"" /> class.</summary>
    /// <param name=""name"">The name of the type that was used in the native signature.</param>
    public NativeTypeNameAttribute(string name)
    {
        _name = name;
    }

    /// <summary>Gets the name of the type that was used in the native signature.</summary>
    public string Name => _name;
}

/// <summary>Defines the annotation found in a native declaration.</summary>
[AttributeUsage(AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.ReturnValue, AllowMultiple = true, Inherited = false)]
[Conditional(""DEBUG"")]
internal sealed partial class NativeAnnotationAttribute : Attribute
{
    private readonly string _annotation;

    /// <summary>Initializes a new instance of the <see cref=""NativeAnnotationAttribute"" /> class.</summary>
    /// <param name=""annotation"">The annotation that was used in the native declaration.</param>
    public NativeAnnotationAttribute(string annotation)
    {
        _annotation = annotation;
    }

    /// <summary>Gets the annotation that was used in the native declaration.</summary>
    public string Annotation => _annotation;
}
";

        return ValidateGeneratedCSharpLatestWindowsBindingsAsync(inputContents, expectedOutputContents, additionalConfigOptions: PInvokeGeneratorConfigurationOptions.GenerateHelperTypes | PInvokeGeneratorConfigurationOptions.GenerateFileScopedNamespaces);
    }
}
