// Copyright (c) .NET Foundation and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using ClangSharp.Interop;

namespace ClangSharp.Abstractions
{
    internal struct ValueDesc
    {
        public AccessSpecifier AccessSpecifier { get; set; }
        public string TypeName { get; set; }
        public string EscapedName { get; set; }
        public string NativeTypeName { get; set; }
        public ValueKind Kind { get; set; }
        public ValueFlags Flags { get; set; }
        public CXSourceLocation? Location { get; set; }

        public bool HasInitializer => Flags.HasFlag(ValueFlags.Initializer);
        public bool IsArray => Flags.HasFlag(ValueFlags.Array);
        public bool IsConstant => Flags.HasFlag(ValueFlags.Constant);
        public bool IsCopy => Flags.HasFlag(ValueFlags.Copy);
    }
}
