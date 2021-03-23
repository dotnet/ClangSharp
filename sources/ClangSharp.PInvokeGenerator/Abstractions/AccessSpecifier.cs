// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

namespace ClangSharp.Abstractions
{
    public enum AccessSpecifier : byte
    {
        None,
        Public,
        Protected,
        ProtectedInternal,
        Internal,
        PrivateProtected,
        Private
    }
}
