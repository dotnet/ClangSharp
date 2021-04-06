// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using ClangSharp.Interop;

namespace ClangSharp.Abstractions
{
    internal struct FieldDesc
    {
        public AccessSpecifier AccessSpecifier { get; set; }
        public string NativeTypeName { get; set; }
        public string EscapedName { get; set; }
        public int? Offset { get; set; }
        public bool NeedsNewKeyword { get; set; }
        public string InheritedFrom { get; set; }
        public CXSourceLocation? Location { get; set; }
    }
}
