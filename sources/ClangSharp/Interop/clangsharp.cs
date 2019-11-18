// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

// Ported from https://github.com/llvm/llvm-project/tree/llvmorg-9.0.0/clang/include/clang-c
// Original source is Copyright (c) the LLVM Project and Contributors. Licensed under the Apache License v2.0 with LLVM Exceptions. See NOTICE.txt in the project root for license information.

using System.Runtime.InteropServices;

namespace ClangSharp.Interop
{
    public static unsafe partial class clangsharp
    {
        private const string libraryPath = "libClangSharp";

        [DllImport(libraryPath, EntryPoint = "clangsharp_Cursor_getAttrKind", CallingConvention = CallingConvention.Cdecl)]
        public static extern CX_AttrKind Cursor_getAttrKind(CXCursor C);

        [DllImport(libraryPath, EntryPoint = "clangsharp_Cursor_getBinaryOpcode", CallingConvention = CallingConvention.Cdecl)]
        public static extern CX_BinaryOperatorKind Cursor_getBinaryOpcode(CXCursor C);

        [DllImport(libraryPath, EntryPoint = "clangsharp_Cursor_getBinaryOpcodeSpelling", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString Cursor_getBinaryOpcodeSpelling(CX_BinaryOperatorKind Op);

        [DllImport(libraryPath, EntryPoint = "clangsharp_Cursor_getDeclKind", CallingConvention = CallingConvention.Cdecl)]
        public static extern CX_DeclKind Cursor_getDeclKind(CXCursor C);

        [DllImport(libraryPath, EntryPoint = "clangsharp_Cursor_getStmtClass", CallingConvention = CallingConvention.Cdecl)]
        public static extern CX_StmtClass Cursor_getStmtClass(CXCursor C);

        [DllImport(libraryPath, EntryPoint = "clangsharp_Cursor_getUnaryOpcode", CallingConvention = CallingConvention.Cdecl)]
        public static extern CX_UnaryOperatorKind Cursor_getUnaryOpcode(CXCursor C);

        [DllImport(libraryPath, EntryPoint = "clangsharp_Cursor_getUnaryOpcodeSpelling", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString Cursor_getUnaryOpcodeSpelling(CX_UnaryOperatorKind Op);

        [DllImport(libraryPath, EntryPoint = "clangsharp_getCursorExtent", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXSourceRange getCursorExtent(CXCursor param0);

        [DllImport(libraryPath, EntryPoint = "clangsharp_getNullRange", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXSourceRange getNullRange();

        [DllImport(libraryPath, EntryPoint = "clangsharp_getRange", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXSourceRange getRange(CXSourceLocation begin, CXSourceLocation end);

        [DllImport(libraryPath, EntryPoint = "clangsharp_getSpellingLocation", CallingConvention = CallingConvention.Cdecl)]
        public static extern void getSpellingLocation(CXSourceLocation location, [NativeTypeName("CXFile *")] void** file, [NativeTypeName("unsigned int *")] uint* line, [NativeTypeName("unsigned int *")] uint* column, [NativeTypeName("unsigned int *")] uint* offset);

        [DllImport(libraryPath, EntryPoint = "clangsharp_isAttribute", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint isAttribute([NativeTypeName("CXCursorKind")] CXCursorKind param0);

        [DllImport(libraryPath, EntryPoint = "clangsharp_isDeclaration", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint isDeclaration([NativeTypeName("CXCursorKind")] CXCursorKind param0);

        [DllImport(libraryPath, EntryPoint = "clangsharp_isExpression", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint isExpression([NativeTypeName("CXCursorKind")] CXCursorKind param0);

        [DllImport(libraryPath, EntryPoint = "clangsharp_isReference", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint isReference([NativeTypeName("CXCursorKind")] CXCursorKind param0);

        [DllImport(libraryPath, EntryPoint = "clangsharp_isStatement", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint isStatement([NativeTypeName("CXCursorKind")] CXCursorKind param0);

        [DllImport(libraryPath, EntryPoint = "clangsharp_isTranslationUnit", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint isTranslationUnit([NativeTypeName("CXCursorKind")] CXCursorKind param0);

        [DllImport(libraryPath, EntryPoint = "clangsharp_Type_getTypeClass", CallingConvention = CallingConvention.Cdecl)]
        public static extern CX_TypeClass Type_getTypeClass(CXType CT);
    }
}
