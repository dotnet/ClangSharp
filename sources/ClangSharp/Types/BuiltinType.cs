// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using ClangSharp.Interop;

namespace ClangSharp;

public sealed class BuiltinType : Type
{

    internal BuiltinType(CXType handle) : base(handle, handle.kind, CX_TypeClass.CX_TypeClass_Builtin)
    {
    }
}
