// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.
// Ported from https://github.com/microsoft/ClangSharp/blob/master/sources/libClangSharp

using System.Runtime.InteropServices;

namespace ClangSharp.Interop
{
    public static unsafe partial class clangsharp
    {
        private const string LibraryPath = "libClangSharp";

        [DllImport(LibraryPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getAttrKind", ExactSpelling = true)]
        public static extern CX_AttrKind Cursor_getAttrKind(CXCursor C);

        [DllImport(LibraryPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getBinaryOpcode", ExactSpelling = true)]
        public static extern CX_BinaryOperatorKind Cursor_getBinaryOpcode(CXCursor C);

        [DllImport(LibraryPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getBinaryOpcodeSpelling", ExactSpelling = true)]
        public static extern CXString Cursor_getBinaryOpcodeSpelling(CX_BinaryOperatorKind Op);

        [DllImport(LibraryPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getCastKind", ExactSpelling = true)]
        public static extern CX_CastKind Cursor_getCastKind(CXCursor C);

        [DllImport(LibraryPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getDeclKind", ExactSpelling = true)]
        public static extern CX_DeclKind Cursor_getDeclKind(CXCursor C);

        [DllImport(LibraryPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getStmtClass", ExactSpelling = true)]
        public static extern CX_StmtClass Cursor_getStmtClass(CXCursor C);

        [DllImport(LibraryPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getUnaryOpcode", ExactSpelling = true)]
        public static extern CX_UnaryOperatorKind Cursor_getUnaryOpcode(CXCursor C);

        [DllImport(LibraryPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getUnaryOpcodeSpelling", ExactSpelling = true)]
        public static extern CXString Cursor_getUnaryOpcodeSpelling(CX_UnaryOperatorKind Op);

        [DllImport(LibraryPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_getCursorExtent", ExactSpelling = true)]
        public static extern CXSourceRange getCursorExtent(CXCursor C);

        [DllImport(LibraryPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_getSpellingLocation", ExactSpelling = true)]
        public static extern void getSpellingLocation(CXSourceLocation location, [NativeTypeName("CXFile *")] void** file, [NativeTypeName("unsigned int *")] uint* line, [NativeTypeName("unsigned int *")] uint* column, [NativeTypeName("unsigned int *")] uint* offset);

        [DllImport(LibraryPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Type_getTypeClass", ExactSpelling = true)]
        public static extern CX_TypeClass Type_getTypeClass(CXType CT);
    }
}
