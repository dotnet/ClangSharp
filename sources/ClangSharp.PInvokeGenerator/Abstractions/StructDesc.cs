// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using ClangSharp.Interop;

namespace ClangSharp.Abstractions;

internal struct StructDesc
{
    public AccessSpecifier AccessSpecifier { get; set; }
    public string EscapedName { get; set; }
    public string NativeType { get; set; }
    public string NativeInheritance { get; set; }
    public LayoutDesc Layout { get; set; }
    public Guid? Uuid { get; set; }
    public StructFlags Flags { get; set; }
    public CXSourceLocation? Location { get; set; }

    public bool IsNested
    {
        get
        {
            return (Flags & StructFlags.Nested) != 0;
        }

        set
        {
            Flags = value ? Flags | StructFlags.Nested : Flags & ~StructFlags.Nested;
        }
    }

    public bool IsUnsafe
    {
        get
        {
            return (Flags & StructFlags.Unsafe) != 0;
        }

        set
        {
            Flags = value ? Flags | StructFlags.Unsafe : Flags & ~StructFlags.Unsafe;
        }
    }

    public bool HasVtbl
    {
        get
        {
            return (Flags & StructFlags.Vtbl) != 0;
        }

        set
        {
            Flags = value ? Flags | StructFlags.Vtbl : Flags & ~StructFlags.Vtbl;
        }
    }

    public bool IsUnion
    {
        get
        {
            return (Flags & StructFlags.Union) != 0;
        }

        set
        {
            Flags = value ? Flags | StructFlags.Union : Flags & ~StructFlags.Union;
        }
    }

    public Action<object> WriteCustomAttrs { get; set; }
    public object CustomAttrGeneratorData { get; set; }

    public StructLayoutAttribute LayoutAttribute
    {
        get
        {
            var layout = Layout;

            if (IsUnion)
            {
                Debug.Assert(layout.Kind == LayoutKind.Explicit);

                var attribute = new StructLayoutAttribute(layout.Kind);

                if (layout.Pack != 0)
                {
                    attribute.Pack = (int)layout.Pack;
                }

                return attribute;
            }

            if (layout.Pack != 0)
            {
                return new StructLayoutAttribute(layout.Kind) {
                    Pack = (int)layout.Pack
                };
            }

            return null;
        }
    }

    public struct LayoutDesc
    {
        public long Alignment32 { get; set; }
        public long Alignment64 { get; set; }
        public long Size32 { get; set; }
        public long Size64 { get; set; }
        public long Pack { get; set; }
        public long MaxFieldAlignment { get; set; }
        public LayoutKind Kind { get; set; }
    }
}
