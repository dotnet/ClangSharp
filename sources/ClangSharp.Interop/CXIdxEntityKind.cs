// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

// Ported from https://github.com/llvm/llvm-project/tree/llvmorg-13.0.0/clang/include/clang-c
// Original source is Copyright (c) the LLVM Project and Contributors. Licensed under the Apache License v2.0 with LLVM Exceptions. See NOTICE.txt in the project root for license information.

namespace ClangSharp.Interop
{
    public enum CXIdxEntityKind
    {
        CXIdxEntity_Unexposed = 0,
        CXIdxEntity_Typedef = 1,
        CXIdxEntity_Function = 2,
        CXIdxEntity_Variable = 3,
        CXIdxEntity_Field = 4,
        CXIdxEntity_EnumConstant = 5,
        CXIdxEntity_ObjCClass = 6,
        CXIdxEntity_ObjCProtocol = 7,
        CXIdxEntity_ObjCCategory = 8,
        CXIdxEntity_ObjCInstanceMethod = 9,
        CXIdxEntity_ObjCClassMethod = 10,
        CXIdxEntity_ObjCProperty = 11,
        CXIdxEntity_ObjCIvar = 12,
        CXIdxEntity_Enum = 13,
        CXIdxEntity_Struct = 14,
        CXIdxEntity_Union = 15,
        CXIdxEntity_CXXClass = 16,
        CXIdxEntity_CXXNamespace = 17,
        CXIdxEntity_CXXNamespaceAlias = 18,
        CXIdxEntity_CXXStaticVariable = 19,
        CXIdxEntity_CXXStaticMethod = 20,
        CXIdxEntity_CXXInstanceMethod = 21,
        CXIdxEntity_CXXConstructor = 22,
        CXIdxEntity_CXXDestructor = 23,
        CXIdxEntity_CXXConversionFunction = 24,
        CXIdxEntity_CXXTypeAlias = 25,
        CXIdxEntity_CXXInterface = 26,
    }
}
