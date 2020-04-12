// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

#ifndef LIBCLANGSHARP_CLANGSHARP_H
#define LIBCLANGSHARP_CLANGSHARP_H

#include <clang-c/Index.h>

#ifdef __cplusplus
#define EXTERN_C extern "C"
#else
#define EXTERN_C
#endif

#ifdef _MSC_VER
#ifdef CLANGSHARP_LINKAGE
#define CLANGSHARP_LINKAGE EXTERN_C __declspec(dllexport)
#else
#define CLANGSHARP_LINKAGE EXTERN_C __declspec(dllimport)
#endif
#else
#define CLANGSHARP_LINKAGE EXTERN_C
#endif

enum CX_AttrKind {
    CX_AttrKind_Invalid,
#define ATTR(X) CX_AttrKind_##X,
#define ATTR_RANGE(CLASS, FIRST_NAME, LAST_NAME) CX_AttrKind_First##CLASS = CX_AttrKind_##FIRST_NAME, CX_AttrKind_Last##CLASS = CX_AttrKind_##LAST_NAME,
#include <clang/Basic/AttrList.inc>
};

enum CX_BinaryOperatorKind {
    CX_BO_Invalid,
#define BINARY_OPERATION(Name, Spelling) CX_BO_##Name,
#include <clang/AST/OperationKinds.def>
};

enum CX_CastKind {
    CX_CK_Invalid,
#define CAST_OPERATION(Name) CX_CK_##Name,
#include <clang/AST/OperationKinds.def>
};

enum CX_DeclKind {
    CX_DeclKind_Invalid,
#define DECL(DERIVED, BASE) CX_DeclKind_##DERIVED,
#define DECL_RANGE(BASE, START, END) CX_DeclKind_First##BASE = CX_DeclKind_##START, CX_DeclKind_Last##BASE = CX_DeclKind_##END,
#define LAST_DECL_RANGE(BASE, START, END) CX_DeclKind_First##BASE = CX_DeclKind_##START, CX_DeclKind_Last##BASE = CX_DeclKind_##END
#define ABSTRACT_DECL(DECL)
#include <clang/AST/DeclNodes.inc>
};

enum CX_StmtClass {
    CX_StmtClass_Invalid,
#define STMT(CLASS, PARENT) CX_StmtClass_##CLASS,
#define STMT_RANGE(BASE, FIRST, LAST) CX_StmtClass_First##BASE = CX_StmtClass_##FIRST, CX_StmtClass_Last##BASE = CX_StmtClass_##LAST,
#define LAST_STMT_RANGE(BASE, FIRST, LAST) CX_StmtClass_First##BASE = CX_StmtClass_##FIRST, CX_StmtClass_Last##BASE = CX_StmtClass_##LAST
#define ABSTRACT_STMT(STMT)
#include <clang/AST/StmtNodes.inc>
};

enum CX_TypeClass {
    CX_TypeClass_Invalid,
#define TYPE(Class, Base) CX_TypeClass_##Class,
#define LAST_TYPE(Class) CX_TypeClass_TypeLast = CX_TypeClass_##Class,
#define ABSTRACT_TYPE(Class, Base)
#include <clang/AST/TypeNodes.inc>
    CX_TypeClass_TagFirst = CX_TypeClass_Record, CX_TypeClass_TagLast = CX_TypeClass_Enum
};

enum CX_UnaryOperatorKind {
    CX_UO_Invalid,
#define UNARY_OPERATION(Name, Spelling) CX_UO_##Name,
#include <clang/AST/OperationKinds.def>
};

CLANGSHARP_LINKAGE CX_AttrKind clangsharp_Cursor_getAttrKind(CXCursor C);

CLANGSHARP_LINKAGE CX_BinaryOperatorKind clangsharp_Cursor_getBinaryOpcode(CXCursor C);

CLANGSHARP_LINKAGE CXString clangsharp_Cursor_getBinaryOpcodeSpelling(CX_BinaryOperatorKind Op);

CLANGSHARP_LINKAGE CX_CastKind clangsharp_Cursor_getCastKind(CXCursor C);

CLANGSHARP_LINKAGE CX_DeclKind clangsharp_Cursor_getDeclKind(CXCursor C);

CLANGSHARP_LINKAGE CX_StmtClass clangsharp_Cursor_getStmtClass(CXCursor C);

CLANGSHARP_LINKAGE CX_UnaryOperatorKind clangsharp_Cursor_getUnaryOpcode(CXCursor C);

CLANGSHARP_LINKAGE CXString clangsharp_Cursor_getUnaryOpcodeSpelling(CX_UnaryOperatorKind Op);

/**
 * Retrieve the physical extent of the source construct referenced by
 * the given cursor.
 *
 * The extent of a cursor starts with the file/line/column pointing at the
 * first character within the source construct that the cursor refers to and
 * ends with the last character within that source construct. For a
 * declaration, the extent covers the declaration itself. For a reference,
 * the extent covers the location of the reference (e.g., where the referenced
 * entity was actually used).
 */
CLANGSHARP_LINKAGE CXSourceRange clangsharp_getCursorExtent(CXCursor C);

/**
 * Retrieve the file, line, column, and offset represented by
 * the given source location.
 *
 * If the location refers into a macro instantiation, return where the
 * location was originally spelled in the source file.
 *
 * \param location the location within a source file that will be decomposed
 * into its parts.
 *
 * \param file [out] if non-NULL, will be set to the file to which the given
 * source location points.
 *
 * \param line [out] if non-NULL, will be set to the line to which the given
 * source location points.
 *
 * \param column [out] if non-NULL, will be set to the column to which the given
 * source location points.
 *
 * \param offset [out] if non-NULL, will be set to the offset into the
 * buffer to which the given source location points.
 */
CLANGSHARP_LINKAGE void clangsharp_getSpellingLocation(CXSourceLocation location, CXFile* file, unsigned* line, unsigned* column, unsigned* offset);

CLANGSHARP_LINKAGE CX_TypeClass clangsharp_Type_getTypeClass(CXType CT);

#endif
