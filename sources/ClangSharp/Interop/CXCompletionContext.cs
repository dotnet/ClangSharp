// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

// Ported from https://github.com/llvm/llvm-project/tree/llvmorg-12.0.0/clang/include/clang-c
// Original source is Copyright (c) the LLVM Project and Contributors. Licensed under the Apache License v2.0 with LLVM Exceptions. See NOTICE.txt in the project root for license information.

namespace ClangSharp.Interop
{
    public enum CXCompletionContext
    {
        CXCompletionContext_Unexposed = 0,
        CXCompletionContext_AnyType = 1 << 0,
        CXCompletionContext_AnyValue = 1 << 1,
        CXCompletionContext_ObjCObjectValue = 1 << 2,
        CXCompletionContext_ObjCSelectorValue = 1 << 3,
        CXCompletionContext_CXXClassTypeValue = 1 << 4,
        CXCompletionContext_DotMemberAccess = 1 << 5,
        CXCompletionContext_ArrowMemberAccess = 1 << 6,
        CXCompletionContext_ObjCPropertyAccess = 1 << 7,
        CXCompletionContext_EnumTag = 1 << 8,
        CXCompletionContext_UnionTag = 1 << 9,
        CXCompletionContext_StructTag = 1 << 10,
        CXCompletionContext_ClassTag = 1 << 11,
        CXCompletionContext_Namespace = 1 << 12,
        CXCompletionContext_NestedNameSpecifier = 1 << 13,
        CXCompletionContext_ObjCInterface = 1 << 14,
        CXCompletionContext_ObjCProtocol = 1 << 15,
        CXCompletionContext_ObjCCategory = 1 << 16,
        CXCompletionContext_ObjCInstanceMessage = 1 << 17,
        CXCompletionContext_ObjCClassMessage = 1 << 18,
        CXCompletionContext_ObjCSelectorName = 1 << 19,
        CXCompletionContext_MacroName = 1 << 20,
        CXCompletionContext_NaturalLanguage = 1 << 21,
        CXCompletionContext_IncludedFile = 1 << 22,
        CXCompletionContext_Unknown = ((1 << 23) - 1),
    }
}
