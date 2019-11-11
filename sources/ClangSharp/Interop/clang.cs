// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using System.Runtime.InteropServices;

namespace ClangSharp.Interop
{
    public static unsafe partial class clang
    {
        private const string libraryPath = "libclang";

        [DllImport(libraryPath, EntryPoint = "clang_createIndex", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("CXIndex")]
        public static extern void* createIndex(int excludeDeclarationsFromPCH, int displayDiagnostics);

        [DllImport(libraryPath, EntryPoint = "clang_disposeIndex", CallingConvention = CallingConvention.Cdecl)]
        public static extern void disposeIndex([NativeTypeName("CXIndex")] void* index);

        [DllImport(libraryPath, EntryPoint = "clang_CXIndex_setGlobalOptions", CallingConvention = CallingConvention.Cdecl)]
        public static extern void CXIndex_setGlobalOptions([NativeTypeName("CXIndex")] void* param0, [NativeTypeName("unsigned int")] uint options);

        [DllImport(libraryPath, EntryPoint = "clang_CXIndex_getGlobalOptions", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint CXIndex_getGlobalOptions([NativeTypeName("CXIndex")] void* param0);

        [DllImport(libraryPath, EntryPoint = "clang_CXIndex_setInvocationEmissionPathOption", CallingConvention = CallingConvention.Cdecl)]
        public static extern void CXIndex_setInvocationEmissionPathOption([NativeTypeName("CXIndex")] void* param0, [NativeTypeName("const char *")] sbyte* Path);

        [DllImport(libraryPath, EntryPoint = "clang_getFileName", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString getFileName([NativeTypeName("CXFile")] void* SFile);

        [DllImport(libraryPath, EntryPoint = "clang_getFileTime", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("time_t")]
        public static extern long getFileTime([NativeTypeName("CXFile")] void* SFile);

        [DllImport(libraryPath, EntryPoint = "clang_getFileUniqueID", CallingConvention = CallingConvention.Cdecl)]
        public static extern int getFileUniqueID([NativeTypeName("CXFile")] void* file, [NativeTypeName("CXFileUniqueID *")] CXFileUniqueID* outID);

        [DllImport(libraryPath, EntryPoint = "clang_isFileMultipleIncludeGuarded", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint isFileMultipleIncludeGuarded([NativeTypeName("CXTranslationUnit")] CXTranslationUnitImpl* tu, [NativeTypeName("CXFile")] void* file);

        [DllImport(libraryPath, EntryPoint = "clang_getFile", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("CXFile")]
        public static extern void* getFile([NativeTypeName("CXTranslationUnit")] CXTranslationUnitImpl* tu, [NativeTypeName("const char *")] sbyte* file_name);

        [DllImport(libraryPath, EntryPoint = "clang_getFileContents", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("const char *")]
        public static extern sbyte* getFileContents([NativeTypeName("CXTranslationUnit")] CXTranslationUnitImpl* tu, [NativeTypeName("CXFile")] void* file, [NativeTypeName("size_t *")] UIntPtr* size);

        [DllImport(libraryPath, EntryPoint = "clang_File_isEqual", CallingConvention = CallingConvention.Cdecl)]
        public static extern int File_isEqual([NativeTypeName("CXFile")] void* file1, [NativeTypeName("CXFile")] void* file2);

        [DllImport(libraryPath, EntryPoint = "clang_File_tryGetRealPathName", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString File_tryGetRealPathName([NativeTypeName("CXFile")] void* file);

        [DllImport(libraryPath, EntryPoint = "clang_getNullLocation", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXSourceLocation getNullLocation();

        [DllImport(libraryPath, EntryPoint = "clang_equalLocations", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint equalLocations(CXSourceLocation loc1, CXSourceLocation loc2);

        [DllImport(libraryPath, EntryPoint = "clang_getLocation", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXSourceLocation getLocation([NativeTypeName("CXTranslationUnit")] CXTranslationUnitImpl* tu, [NativeTypeName("CXFile")] void* file, [NativeTypeName("unsigned int")] uint line, [NativeTypeName("unsigned int")] uint column);

        [DllImport(libraryPath, EntryPoint = "clang_getLocationForOffset", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXSourceLocation getLocationForOffset([NativeTypeName("CXTranslationUnit")] CXTranslationUnitImpl* tu, [NativeTypeName("CXFile")] void* file, [NativeTypeName("unsigned int")] uint offset);

        [DllImport(libraryPath, EntryPoint = "clang_Location_isInSystemHeader", CallingConvention = CallingConvention.Cdecl)]
        public static extern int Location_isInSystemHeader(CXSourceLocation location);

        [DllImport(libraryPath, EntryPoint = "clang_Location_isFromMainFile", CallingConvention = CallingConvention.Cdecl)]
        public static extern int Location_isFromMainFile(CXSourceLocation location);

        [DllImport(libraryPath, EntryPoint = "clang_getNullRange", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXSourceRange getNullRange();

        [DllImport(libraryPath, EntryPoint = "clang_getRange", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXSourceRange getRange(CXSourceLocation begin, CXSourceLocation end);

        [DllImport(libraryPath, EntryPoint = "clang_equalRanges", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint equalRanges(CXSourceRange range1, CXSourceRange range2);

        [DllImport(libraryPath, EntryPoint = "clang_Range_isNull", CallingConvention = CallingConvention.Cdecl)]
        public static extern int Range_isNull(CXSourceRange range);

        [DllImport(libraryPath, EntryPoint = "clang_getExpansionLocation", CallingConvention = CallingConvention.Cdecl)]
        public static extern void getExpansionLocation(CXSourceLocation location, [NativeTypeName("CXFile *")] void** file, [NativeTypeName("unsigned int *")] uint* line, [NativeTypeName("unsigned int *")] uint* column, [NativeTypeName("unsigned int *")] uint* offset);

        [DllImport(libraryPath, EntryPoint = "clang_getPresumedLocation", CallingConvention = CallingConvention.Cdecl)]
        public static extern void getPresumedLocation(CXSourceLocation location, [NativeTypeName("CXString *")] CXString* filename, [NativeTypeName("unsigned int *")] uint* line, [NativeTypeName("unsigned int *")] uint* column);

        [DllImport(libraryPath, EntryPoint = "clang_getInstantiationLocation", CallingConvention = CallingConvention.Cdecl)]
        public static extern void getInstantiationLocation(CXSourceLocation location, [NativeTypeName("CXFile *")] void** file, [NativeTypeName("unsigned int *")] uint* line, [NativeTypeName("unsigned int *")] uint* column, [NativeTypeName("unsigned int *")] uint* offset);

        [DllImport(libraryPath, EntryPoint = "clang_getSpellingLocation", CallingConvention = CallingConvention.Cdecl)]
        public static extern void getSpellingLocation(CXSourceLocation location, [NativeTypeName("CXFile *")] void** file, [NativeTypeName("unsigned int *")] uint* line, [NativeTypeName("unsigned int *")] uint* column, [NativeTypeName("unsigned int *")] uint* offset);

        [DllImport(libraryPath, EntryPoint = "clang_getFileLocation", CallingConvention = CallingConvention.Cdecl)]
        public static extern void getFileLocation(CXSourceLocation location, [NativeTypeName("CXFile *")] void** file, [NativeTypeName("unsigned int *")] uint* line, [NativeTypeName("unsigned int *")] uint* column, [NativeTypeName("unsigned int *")] uint* offset);

        [DllImport(libraryPath, EntryPoint = "clang_getRangeStart", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXSourceLocation getRangeStart(CXSourceRange range);

        [DllImport(libraryPath, EntryPoint = "clang_getRangeEnd", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXSourceLocation getRangeEnd(CXSourceRange range);

        [DllImport(libraryPath, EntryPoint = "clang_getSkippedRanges", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("CXSourceRangeList *")]
        public static extern CXSourceRangeList* getSkippedRanges([NativeTypeName("CXTranslationUnit")] CXTranslationUnitImpl* tu, [NativeTypeName("CXFile")] void* file);

        [DllImport(libraryPath, EntryPoint = "clang_getAllSkippedRanges", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("CXSourceRangeList *")]
        public static extern CXSourceRangeList* getAllSkippedRanges([NativeTypeName("CXTranslationUnit")] CXTranslationUnitImpl* tu);

        [DllImport(libraryPath, EntryPoint = "clang_disposeSourceRangeList", CallingConvention = CallingConvention.Cdecl)]
        public static extern void disposeSourceRangeList([NativeTypeName("CXSourceRangeList *")] CXSourceRangeList* ranges);

        [DllImport(libraryPath, EntryPoint = "clang_getNumDiagnosticsInSet", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint getNumDiagnosticsInSet([NativeTypeName("CXDiagnosticSet")] void* Diags);

        [DllImport(libraryPath, EntryPoint = "clang_getDiagnosticInSet", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("CXDiagnostic")]
        public static extern void* getDiagnosticInSet([NativeTypeName("CXDiagnosticSet")] void* Diags, [NativeTypeName("unsigned int")] uint Index);

        [DllImport(libraryPath, EntryPoint = "clang_loadDiagnostics", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("CXDiagnosticSet")]
        public static extern void* loadDiagnostics([NativeTypeName("const char *")] sbyte* file, [NativeTypeName("enum CXLoadDiag_Error *")] CXLoadDiag_Error* error, [NativeTypeName("CXString *")] CXString* errorString);

        [DllImport(libraryPath, EntryPoint = "clang_disposeDiagnosticSet", CallingConvention = CallingConvention.Cdecl)]
        public static extern void disposeDiagnosticSet([NativeTypeName("CXDiagnosticSet")] void* Diags);

        [DllImport(libraryPath, EntryPoint = "clang_getChildDiagnostics", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("CXDiagnosticSet")]
        public static extern void* getChildDiagnostics([NativeTypeName("CXDiagnostic")] void* D);

        [DllImport(libraryPath, EntryPoint = "clang_getNumDiagnostics", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint getNumDiagnostics([NativeTypeName("CXTranslationUnit")] CXTranslationUnitImpl* Unit);

        [DllImport(libraryPath, EntryPoint = "clang_getDiagnostic", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("CXDiagnostic")]
        public static extern void* getDiagnostic([NativeTypeName("CXTranslationUnit")] CXTranslationUnitImpl* Unit, [NativeTypeName("unsigned int")] uint Index);

        [DllImport(libraryPath, EntryPoint = "clang_getDiagnosticSetFromTU", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("CXDiagnosticSet")]
        public static extern void* getDiagnosticSetFromTU([NativeTypeName("CXTranslationUnit")] CXTranslationUnitImpl* Unit);

        [DllImport(libraryPath, EntryPoint = "clang_disposeDiagnostic", CallingConvention = CallingConvention.Cdecl)]
        public static extern void disposeDiagnostic([NativeTypeName("CXDiagnostic")] void* Diagnostic);

        [DllImport(libraryPath, EntryPoint = "clang_formatDiagnostic", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString formatDiagnostic([NativeTypeName("CXDiagnostic")] void* Diagnostic, [NativeTypeName("unsigned int")] uint Options);

        [DllImport(libraryPath, EntryPoint = "clang_defaultDiagnosticDisplayOptions", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint defaultDiagnosticDisplayOptions();

        [DllImport(libraryPath, EntryPoint = "clang_getDiagnosticSeverity", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("enum CXDiagnosticSeverity")]
        public static extern CXDiagnosticSeverity getDiagnosticSeverity([NativeTypeName("CXDiagnostic")] void* param0);

        [DllImport(libraryPath, EntryPoint = "clang_getDiagnosticLocation", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXSourceLocation getDiagnosticLocation([NativeTypeName("CXDiagnostic")] void* param0);

        [DllImport(libraryPath, EntryPoint = "clang_getDiagnosticSpelling", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString getDiagnosticSpelling([NativeTypeName("CXDiagnostic")] void* param0);

        [DllImport(libraryPath, EntryPoint = "clang_getDiagnosticOption", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString getDiagnosticOption([NativeTypeName("CXDiagnostic")] void* Diag, [NativeTypeName("CXString *")] CXString* Disable);

        [DllImport(libraryPath, EntryPoint = "clang_getDiagnosticCategory", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint getDiagnosticCategory([NativeTypeName("CXDiagnostic")] void* param0);

        [DllImport(libraryPath, EntryPoint = "clang_getDiagnosticCategoryName", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString getDiagnosticCategoryName([NativeTypeName("unsigned int")] uint Category);

        [DllImport(libraryPath, EntryPoint = "clang_getDiagnosticCategoryText", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString getDiagnosticCategoryText([NativeTypeName("CXDiagnostic")] void* param0);

        [DllImport(libraryPath, EntryPoint = "clang_getDiagnosticNumRanges", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint getDiagnosticNumRanges([NativeTypeName("CXDiagnostic")] void* param0);

        [DllImport(libraryPath, EntryPoint = "clang_getDiagnosticRange", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXSourceRange getDiagnosticRange([NativeTypeName("CXDiagnostic")] void* Diagnostic, [NativeTypeName("unsigned int")] uint Range);

        [DllImport(libraryPath, EntryPoint = "clang_getDiagnosticNumFixIts", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint getDiagnosticNumFixIts([NativeTypeName("CXDiagnostic")] void* Diagnostic);

        [DllImport(libraryPath, EntryPoint = "clang_getDiagnosticFixIt", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString getDiagnosticFixIt([NativeTypeName("CXDiagnostic")] void* Diagnostic, [NativeTypeName("unsigned int")] uint FixIt, [NativeTypeName("CXSourceRange *")] CXSourceRange* ReplacementRange);

        [DllImport(libraryPath, EntryPoint = "clang_getTranslationUnitSpelling", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString getTranslationUnitSpelling([NativeTypeName("CXTranslationUnit")] CXTranslationUnitImpl* CTUnit);

        [DllImport(libraryPath, EntryPoint = "clang_createTranslationUnitFromSourceFile", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("CXTranslationUnit")]
        public static extern CXTranslationUnitImpl* createTranslationUnitFromSourceFile([NativeTypeName("CXIndex")] void* CIdx, [NativeTypeName("const char *")] sbyte* source_filename, int num_clang_command_line_args, [NativeTypeName("const char *const *")] sbyte** clang_command_line_args, [NativeTypeName("unsigned int")] uint num_unsaved_files, [NativeTypeName("struct CXUnsavedFile *")] CXUnsavedFile* unsaved_files);

        [DllImport(libraryPath, EntryPoint = "clang_createTranslationUnit", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("CXTranslationUnit")]
        public static extern CXTranslationUnitImpl* createTranslationUnit([NativeTypeName("CXIndex")] void* CIdx, [NativeTypeName("const char *")] sbyte* ast_filename);

        [DllImport(libraryPath, EntryPoint = "clang_createTranslationUnit2", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("enum CXErrorCode")]
        public static extern CXErrorCode createTranslationUnit2([NativeTypeName("CXIndex")] void* CIdx, [NativeTypeName("const char *")] sbyte* ast_filename, [NativeTypeName("CXTranslationUnit *")] CXTranslationUnitImpl** out_TU);

        [DllImport(libraryPath, EntryPoint = "clang_defaultEditingTranslationUnitOptions", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint defaultEditingTranslationUnitOptions();

        [DllImport(libraryPath, EntryPoint = "clang_parseTranslationUnit", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("CXTranslationUnit")]
        public static extern CXTranslationUnitImpl* parseTranslationUnit([NativeTypeName("CXIndex")] void* CIdx, [NativeTypeName("const char *")] sbyte* source_filename, [NativeTypeName("const char *const *")] sbyte** command_line_args, int num_command_line_args, [NativeTypeName("struct CXUnsavedFile *")] CXUnsavedFile* unsaved_files, [NativeTypeName("unsigned int")] uint num_unsaved_files, [NativeTypeName("unsigned int")] uint options);

        [DllImport(libraryPath, EntryPoint = "clang_parseTranslationUnit2", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("enum CXErrorCode")]
        public static extern CXErrorCode parseTranslationUnit2([NativeTypeName("CXIndex")] void* CIdx, [NativeTypeName("const char *")] sbyte* source_filename, [NativeTypeName("const char *const *")] sbyte** command_line_args, int num_command_line_args, [NativeTypeName("struct CXUnsavedFile *")] CXUnsavedFile* unsaved_files, [NativeTypeName("unsigned int")] uint num_unsaved_files, [NativeTypeName("unsigned int")] uint options, [NativeTypeName("CXTranslationUnit *")] CXTranslationUnitImpl** out_TU);

        [DllImport(libraryPath, EntryPoint = "clang_parseTranslationUnit2FullArgv", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("enum CXErrorCode")]
        public static extern CXErrorCode parseTranslationUnit2FullArgv([NativeTypeName("CXIndex")] void* CIdx, [NativeTypeName("const char *")] sbyte* source_filename, [NativeTypeName("const char *const *")] sbyte** command_line_args, int num_command_line_args, [NativeTypeName("struct CXUnsavedFile *")] CXUnsavedFile* unsaved_files, [NativeTypeName("unsigned int")] uint num_unsaved_files, [NativeTypeName("unsigned int")] uint options, [NativeTypeName("CXTranslationUnit *")] CXTranslationUnitImpl** out_TU);

        [DllImport(libraryPath, EntryPoint = "clang_defaultSaveOptions", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint defaultSaveOptions([NativeTypeName("CXTranslationUnit")] CXTranslationUnitImpl* TU);

        [DllImport(libraryPath, EntryPoint = "clang_saveTranslationUnit", CallingConvention = CallingConvention.Cdecl)]
        public static extern int saveTranslationUnit([NativeTypeName("CXTranslationUnit")] CXTranslationUnitImpl* TU, [NativeTypeName("const char *")] sbyte* FileName, [NativeTypeName("unsigned int")] uint options);

        [DllImport(libraryPath, EntryPoint = "clang_suspendTranslationUnit", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint suspendTranslationUnit([NativeTypeName("CXTranslationUnit")] CXTranslationUnitImpl* param0);

        [DllImport(libraryPath, EntryPoint = "clang_disposeTranslationUnit", CallingConvention = CallingConvention.Cdecl)]
        public static extern void disposeTranslationUnit([NativeTypeName("CXTranslationUnit")] CXTranslationUnitImpl* param0);

        [DllImport(libraryPath, EntryPoint = "clang_defaultReparseOptions", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint defaultReparseOptions([NativeTypeName("CXTranslationUnit")] CXTranslationUnitImpl* TU);

        [DllImport(libraryPath, EntryPoint = "clang_reparseTranslationUnit", CallingConvention = CallingConvention.Cdecl)]
        public static extern int reparseTranslationUnit([NativeTypeName("CXTranslationUnit")] CXTranslationUnitImpl* TU, [NativeTypeName("unsigned int")] uint num_unsaved_files, [NativeTypeName("struct CXUnsavedFile *")] CXUnsavedFile* unsaved_files, [NativeTypeName("unsigned int")] uint options);

        [DllImport(libraryPath, EntryPoint = "clang_getTUResourceUsageName", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("const char *")]
        public static extern sbyte* getTUResourceUsageName([NativeTypeName("enum CXTUResourceUsageKind")] CXTUResourceUsageKind kind);

        [DllImport(libraryPath, EntryPoint = "clang_getCXTUResourceUsage", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXTUResourceUsage getCXTUResourceUsage([NativeTypeName("CXTranslationUnit")] CXTranslationUnitImpl* TU);

        [DllImport(libraryPath, EntryPoint = "clang_disposeCXTUResourceUsage", CallingConvention = CallingConvention.Cdecl)]
        public static extern void disposeCXTUResourceUsage(CXTUResourceUsage usage);

        [DllImport(libraryPath, EntryPoint = "clang_getTranslationUnitTargetInfo", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("CXTargetInfo")]
        public static extern CXTargetInfoImpl* getTranslationUnitTargetInfo([NativeTypeName("CXTranslationUnit")] CXTranslationUnitImpl* CTUnit);

        [DllImport(libraryPath, EntryPoint = "clang_TargetInfo_dispose", CallingConvention = CallingConvention.Cdecl)]
        public static extern void TargetInfo_dispose([NativeTypeName("CXTargetInfo")] CXTargetInfoImpl* Info);

        [DllImport(libraryPath, EntryPoint = "clang_TargetInfo_getTriple", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString TargetInfo_getTriple([NativeTypeName("CXTargetInfo")] CXTargetInfoImpl* Info);

        [DllImport(libraryPath, EntryPoint = "clang_TargetInfo_getPointerWidth", CallingConvention = CallingConvention.Cdecl)]
        public static extern int TargetInfo_getPointerWidth([NativeTypeName("CXTargetInfo")] CXTargetInfoImpl* Info);

        [DllImport(libraryPath, EntryPoint = "clang_getNullCursor", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXCursor getNullCursor();

        [DllImport(libraryPath, EntryPoint = "clang_getTranslationUnitCursor", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXCursor getTranslationUnitCursor([NativeTypeName("CXTranslationUnit")] CXTranslationUnitImpl* param0);

        [DllImport(libraryPath, EntryPoint = "clang_equalCursors", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint equalCursors(CXCursor param0, CXCursor param1);

        [DllImport(libraryPath, EntryPoint = "clang_Cursor_isNull", CallingConvention = CallingConvention.Cdecl)]
        public static extern int Cursor_isNull(CXCursor cursor);

        [DllImport(libraryPath, EntryPoint = "clang_hashCursor", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint hashCursor(CXCursor param0);

        [DllImport(libraryPath, EntryPoint = "clang_getCursorKind", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("enum CXCursorKind")]
        public static extern CXCursorKind getCursorKind(CXCursor param0);

        [DllImport(libraryPath, EntryPoint = "clang_isDeclaration", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint isDeclaration([NativeTypeName("enum CXCursorKind")] CXCursorKind param0);

        [DllImport(libraryPath, EntryPoint = "clang_isInvalidDeclaration", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint isInvalidDeclaration(CXCursor param0);

        [DllImport(libraryPath, EntryPoint = "clang_isReference", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint isReference([NativeTypeName("enum CXCursorKind")] CXCursorKind param0);

        [DllImport(libraryPath, EntryPoint = "clang_isExpression", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint isExpression([NativeTypeName("enum CXCursorKind")] CXCursorKind param0);

        [DllImport(libraryPath, EntryPoint = "clang_isStatement", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint isStatement([NativeTypeName("enum CXCursorKind")] CXCursorKind param0);

        [DllImport(libraryPath, EntryPoint = "clang_isAttribute", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint isAttribute([NativeTypeName("enum CXCursorKind")] CXCursorKind param0);

        [DllImport(libraryPath, EntryPoint = "clang_Cursor_hasAttrs", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint Cursor_hasAttrs(CXCursor C);

        [DllImport(libraryPath, EntryPoint = "clang_isInvalid", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint isInvalid([NativeTypeName("enum CXCursorKind")] CXCursorKind param0);

        [DllImport(libraryPath, EntryPoint = "clang_isTranslationUnit", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint isTranslationUnit([NativeTypeName("enum CXCursorKind")] CXCursorKind param0);

        [DllImport(libraryPath, EntryPoint = "clang_isPreprocessing", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint isPreprocessing([NativeTypeName("enum CXCursorKind")] CXCursorKind param0);

        [DllImport(libraryPath, EntryPoint = "clang_isUnexposed", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint isUnexposed([NativeTypeName("enum CXCursorKind")] CXCursorKind param0);

        [DllImport(libraryPath, EntryPoint = "clang_getCursorLinkage", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("enum CXLinkageKind")]
        public static extern CXLinkageKind getCursorLinkage(CXCursor cursor);

        [DllImport(libraryPath, EntryPoint = "clang_getCursorVisibility", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("enum CXVisibilityKind")]
        public static extern CXVisibilityKind getCursorVisibility(CXCursor cursor);

        [DllImport(libraryPath, EntryPoint = "clang_getCursorAvailability", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("enum CXAvailabilityKind")]
        public static extern CXAvailabilityKind getCursorAvailability(CXCursor cursor);

        [DllImport(libraryPath, EntryPoint = "clang_getCursorPlatformAvailability", CallingConvention = CallingConvention.Cdecl)]
        public static extern int getCursorPlatformAvailability(CXCursor cursor, [NativeTypeName("int *")] int* always_deprecated, [NativeTypeName("CXString *")] CXString* deprecated_message, [NativeTypeName("int *")] int* always_unavailable, [NativeTypeName("CXString *")] CXString* unavailable_message, [NativeTypeName("CXPlatformAvailability *")] CXPlatformAvailability* availability, int availability_size);

        [DllImport(libraryPath, EntryPoint = "clang_disposeCXPlatformAvailability", CallingConvention = CallingConvention.Cdecl)]
        public static extern void disposeCXPlatformAvailability([NativeTypeName("CXPlatformAvailability *")] CXPlatformAvailability* availability);

        [DllImport(libraryPath, EntryPoint = "clang_getCursorLanguage", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("enum CXLanguageKind")]
        public static extern CXLanguageKind getCursorLanguage(CXCursor cursor);

        [DllImport(libraryPath, EntryPoint = "clang_getCursorTLSKind", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("enum CXTLSKind")]
        public static extern CXTLSKind getCursorTLSKind(CXCursor cursor);

        [DllImport(libraryPath, EntryPoint = "clang_Cursor_getTranslationUnit", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("CXTranslationUnit")]
        public static extern CXTranslationUnitImpl* Cursor_getTranslationUnit(CXCursor param0);

        [DllImport(libraryPath, EntryPoint = "clang_createCXCursorSet", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("CXCursorSet")]
        public static extern CXCursorSetImpl* createCXCursorSet();

        [DllImport(libraryPath, EntryPoint = "clang_disposeCXCursorSet", CallingConvention = CallingConvention.Cdecl)]
        public static extern void disposeCXCursorSet([NativeTypeName("CXCursorSet")] CXCursorSetImpl* cset);

        [DllImport(libraryPath, EntryPoint = "clang_CXCursorSet_contains", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint CXCursorSet_contains([NativeTypeName("CXCursorSet")] CXCursorSetImpl* cset, CXCursor cursor);

        [DllImport(libraryPath, EntryPoint = "clang_CXCursorSet_insert", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint CXCursorSet_insert([NativeTypeName("CXCursorSet")] CXCursorSetImpl* cset, CXCursor cursor);

        [DllImport(libraryPath, EntryPoint = "clang_getCursorSemanticParent", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXCursor getCursorSemanticParent(CXCursor cursor);

        [DllImport(libraryPath, EntryPoint = "clang_getCursorLexicalParent", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXCursor getCursorLexicalParent(CXCursor cursor);

        [DllImport(libraryPath, EntryPoint = "clang_getOverriddenCursors", CallingConvention = CallingConvention.Cdecl)]
        public static extern void getOverriddenCursors(CXCursor cursor, [NativeTypeName("CXCursor **")] CXCursor** overridden, [NativeTypeName("unsigned int *")] uint* num_overridden);

        [DllImport(libraryPath, EntryPoint = "clang_disposeOverriddenCursors", CallingConvention = CallingConvention.Cdecl)]
        public static extern void disposeOverriddenCursors([NativeTypeName("CXCursor *")] CXCursor* overridden);

        [DllImport(libraryPath, EntryPoint = "clang_getIncludedFile", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("CXFile")]
        public static extern void* getIncludedFile(CXCursor cursor);

        [DllImport(libraryPath, EntryPoint = "clang_getCursor", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXCursor getCursor([NativeTypeName("CXTranslationUnit")] CXTranslationUnitImpl* param0, CXSourceLocation param1);

        [DllImport(libraryPath, EntryPoint = "clang_getCursorLocation", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXSourceLocation getCursorLocation(CXCursor param0);

        [DllImport(libraryPath, EntryPoint = "clang_getCursorExtent", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXSourceRange getCursorExtent(CXCursor param0);

        [DllImport(libraryPath, EntryPoint = "clang_getCursorType", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXType getCursorType(CXCursor C);

        [DllImport(libraryPath, EntryPoint = "clang_getTypeSpelling", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString getTypeSpelling(CXType CT);

        [DllImport(libraryPath, EntryPoint = "clang_getTypedefDeclUnderlyingType", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXType getTypedefDeclUnderlyingType(CXCursor C);

        [DllImport(libraryPath, EntryPoint = "clang_getEnumDeclIntegerType", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXType getEnumDeclIntegerType(CXCursor C);

        [DllImport(libraryPath, EntryPoint = "clang_getEnumConstantDeclValue", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("long long")]
        public static extern long getEnumConstantDeclValue(CXCursor C);

        [DllImport(libraryPath, EntryPoint = "clang_getEnumConstantDeclUnsignedValue", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("unsigned long long")]
        public static extern ulong getEnumConstantDeclUnsignedValue(CXCursor C);

        [DllImport(libraryPath, EntryPoint = "clang_getFieldDeclBitWidth", CallingConvention = CallingConvention.Cdecl)]
        public static extern int getFieldDeclBitWidth(CXCursor C);

        [DllImport(libraryPath, EntryPoint = "clang_Cursor_getNumArguments", CallingConvention = CallingConvention.Cdecl)]
        public static extern int Cursor_getNumArguments(CXCursor C);

        [DllImport(libraryPath, EntryPoint = "clang_Cursor_getArgument", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXCursor Cursor_getArgument(CXCursor C, [NativeTypeName("unsigned int")] uint i);

        [DllImport(libraryPath, EntryPoint = "clang_Cursor_getNumTemplateArguments", CallingConvention = CallingConvention.Cdecl)]
        public static extern int Cursor_getNumTemplateArguments(CXCursor C);

        [DllImport(libraryPath, EntryPoint = "clang_Cursor_getTemplateArgumentKind", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("enum CXTemplateArgumentKind")]
        public static extern CXTemplateArgumentKind Cursor_getTemplateArgumentKind(CXCursor C, [NativeTypeName("unsigned int")] uint I);

        [DllImport(libraryPath, EntryPoint = "clang_Cursor_getTemplateArgumentType", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXType Cursor_getTemplateArgumentType(CXCursor C, [NativeTypeName("unsigned int")] uint I);

        [DllImport(libraryPath, EntryPoint = "clang_Cursor_getTemplateArgumentValue", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("long long")]
        public static extern long Cursor_getTemplateArgumentValue(CXCursor C, [NativeTypeName("unsigned int")] uint I);

        [DllImport(libraryPath, EntryPoint = "clang_Cursor_getTemplateArgumentUnsignedValue", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("unsigned long long")]
        public static extern ulong Cursor_getTemplateArgumentUnsignedValue(CXCursor C, [NativeTypeName("unsigned int")] uint I);

        [DllImport(libraryPath, EntryPoint = "clang_equalTypes", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint equalTypes(CXType A, CXType B);

        [DllImport(libraryPath, EntryPoint = "clang_getCanonicalType", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXType getCanonicalType(CXType T);

        [DllImport(libraryPath, EntryPoint = "clang_isConstQualifiedType", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint isConstQualifiedType(CXType T);

        [DllImport(libraryPath, EntryPoint = "clang_Cursor_isMacroFunctionLike", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint Cursor_isMacroFunctionLike(CXCursor C);

        [DllImport(libraryPath, EntryPoint = "clang_Cursor_isMacroBuiltin", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint Cursor_isMacroBuiltin(CXCursor C);

        [DllImport(libraryPath, EntryPoint = "clang_Cursor_isFunctionInlined", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint Cursor_isFunctionInlined(CXCursor C);

        [DllImport(libraryPath, EntryPoint = "clang_isVolatileQualifiedType", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint isVolatileQualifiedType(CXType T);

        [DllImport(libraryPath, EntryPoint = "clang_isRestrictQualifiedType", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint isRestrictQualifiedType(CXType T);

        [DllImport(libraryPath, EntryPoint = "clang_getAddressSpace", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint getAddressSpace(CXType T);

        [DllImport(libraryPath, EntryPoint = "clang_getTypedefName", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString getTypedefName(CXType CT);

        [DllImport(libraryPath, EntryPoint = "clang_getPointeeType", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXType getPointeeType(CXType T);

        [DllImport(libraryPath, EntryPoint = "clang_getTypeDeclaration", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXCursor getTypeDeclaration(CXType T);

        [DllImport(libraryPath, EntryPoint = "clang_getDeclObjCTypeEncoding", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString getDeclObjCTypeEncoding(CXCursor C);

        [DllImport(libraryPath, EntryPoint = "clang_Type_getObjCEncoding", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString Type_getObjCEncoding(CXType type);

        [DllImport(libraryPath, EntryPoint = "clang_getTypeKindSpelling", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString getTypeKindSpelling([NativeTypeName("enum CXTypeKind")] CXTypeKind K);

        [DllImport(libraryPath, EntryPoint = "clang_getFunctionTypeCallingConv", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("enum CXCallingConv")]
        public static extern CXCallingConv getFunctionTypeCallingConv(CXType T);

        [DllImport(libraryPath, EntryPoint = "clang_getResultType", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXType getResultType(CXType T);

        [DllImport(libraryPath, EntryPoint = "clang_getExceptionSpecificationType", CallingConvention = CallingConvention.Cdecl)]
        public static extern int getExceptionSpecificationType(CXType T);

        [DllImport(libraryPath, EntryPoint = "clang_getNumArgTypes", CallingConvention = CallingConvention.Cdecl)]
        public static extern int getNumArgTypes(CXType T);

        [DllImport(libraryPath, EntryPoint = "clang_getArgType", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXType getArgType(CXType T, [NativeTypeName("unsigned int")] uint i);

        [DllImport(libraryPath, EntryPoint = "clang_Type_getObjCObjectBaseType", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXType Type_getObjCObjectBaseType(CXType T);

        [DllImport(libraryPath, EntryPoint = "clang_Type_getNumObjCProtocolRefs", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint Type_getNumObjCProtocolRefs(CXType T);

        [DllImport(libraryPath, EntryPoint = "clang_Type_getObjCProtocolDecl", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXCursor Type_getObjCProtocolDecl(CXType T, [NativeTypeName("unsigned int")] uint i);

        [DllImport(libraryPath, EntryPoint = "clang_Type_getNumObjCTypeArgs", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint Type_getNumObjCTypeArgs(CXType T);

        [DllImport(libraryPath, EntryPoint = "clang_Type_getObjCTypeArg", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXType Type_getObjCTypeArg(CXType T, [NativeTypeName("unsigned int")] uint i);

        [DllImport(libraryPath, EntryPoint = "clang_isFunctionTypeVariadic", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint isFunctionTypeVariadic(CXType T);

        [DllImport(libraryPath, EntryPoint = "clang_getCursorResultType", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXType getCursorResultType(CXCursor C);

        [DllImport(libraryPath, EntryPoint = "clang_getCursorExceptionSpecificationType", CallingConvention = CallingConvention.Cdecl)]
        public static extern int getCursorExceptionSpecificationType(CXCursor C);

        [DllImport(libraryPath, EntryPoint = "clang_isPODType", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint isPODType(CXType T);

        [DllImport(libraryPath, EntryPoint = "clang_getElementType", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXType getElementType(CXType T);

        [DllImport(libraryPath, EntryPoint = "clang_getNumElements", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("long long")]
        public static extern long getNumElements(CXType T);

        [DllImport(libraryPath, EntryPoint = "clang_getArrayElementType", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXType getArrayElementType(CXType T);

        [DllImport(libraryPath, EntryPoint = "clang_getArraySize", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("long long")]
        public static extern long getArraySize(CXType T);

        [DllImport(libraryPath, EntryPoint = "clang_Type_getNamedType", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXType Type_getNamedType(CXType T);

        [DllImport(libraryPath, EntryPoint = "clang_Type_isTransparentTagTypedef", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint Type_isTransparentTagTypedef(CXType T);

        [DllImport(libraryPath, EntryPoint = "clang_Type_getNullability", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("enum CXTypeNullabilityKind")]
        public static extern CXTypeNullabilityKind Type_getNullability(CXType T);

        [DllImport(libraryPath, EntryPoint = "clang_Type_getAlignOf", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("long long")]
        public static extern long Type_getAlignOf(CXType T);

        [DllImport(libraryPath, EntryPoint = "clang_Type_getClassType", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXType Type_getClassType(CXType T);

        [DllImport(libraryPath, EntryPoint = "clang_Type_getSizeOf", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("long long")]
        public static extern long Type_getSizeOf(CXType T);

        [DllImport(libraryPath, EntryPoint = "clang_Type_getOffsetOf", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("long long")]
        public static extern long Type_getOffsetOf(CXType T, [NativeTypeName("const char *")] sbyte* S);

        [DllImport(libraryPath, EntryPoint = "clang_Type_getModifiedType", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXType Type_getModifiedType(CXType T);

        [DllImport(libraryPath, EntryPoint = "clang_Cursor_getOffsetOfField", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("long long")]
        public static extern long Cursor_getOffsetOfField(CXCursor C);

        [DllImport(libraryPath, EntryPoint = "clang_Cursor_isAnonymous", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint Cursor_isAnonymous(CXCursor C);

        [DllImport(libraryPath, EntryPoint = "clang_Cursor_isAnonymousRecordDecl", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint Cursor_isAnonymousRecordDecl(CXCursor C);

        [DllImport(libraryPath, EntryPoint = "clang_Cursor_isInlineNamespace", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint Cursor_isInlineNamespace(CXCursor C);

        [DllImport(libraryPath, EntryPoint = "clang_Type_getNumTemplateArguments", CallingConvention = CallingConvention.Cdecl)]
        public static extern int Type_getNumTemplateArguments(CXType T);

        [DllImport(libraryPath, EntryPoint = "clang_Type_getTemplateArgumentAsType", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXType Type_getTemplateArgumentAsType(CXType T, [NativeTypeName("unsigned int")] uint i);

        [DllImport(libraryPath, EntryPoint = "clang_Type_getCXXRefQualifier", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("enum CXRefQualifierKind")]
        public static extern CXRefQualifierKind Type_getCXXRefQualifier(CXType T);

        [DllImport(libraryPath, EntryPoint = "clang_Cursor_isBitField", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint Cursor_isBitField(CXCursor C);

        [DllImport(libraryPath, EntryPoint = "clang_isVirtualBase", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint isVirtualBase(CXCursor param0);

        [DllImport(libraryPath, EntryPoint = "clang_getCXXAccessSpecifier", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("enum CX_CXXAccessSpecifier")]
        public static extern CX_CXXAccessSpecifier getCXXAccessSpecifier(CXCursor param0);

        [DllImport(libraryPath, EntryPoint = "clang_Cursor_getStorageClass", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("enum CX_StorageClass")]
        public static extern CX_StorageClass Cursor_getStorageClass(CXCursor param0);

        [DllImport(libraryPath, EntryPoint = "clang_getNumOverloadedDecls", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint getNumOverloadedDecls(CXCursor cursor);

        [DllImport(libraryPath, EntryPoint = "clang_getOverloadedDecl", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXCursor getOverloadedDecl(CXCursor cursor, [NativeTypeName("unsigned int")] uint index);

        [DllImport(libraryPath, EntryPoint = "clang_getIBOutletCollectionType", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXType getIBOutletCollectionType(CXCursor param0);

        [DllImport(libraryPath, EntryPoint = "clang_visitChildren", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint visitChildren(CXCursor parent, [NativeTypeName("CXCursorVisitor")] IntPtr visitor, [NativeTypeName("CXClientData")] void* client_data);

        [DllImport(libraryPath, EntryPoint = "clang_getCursorUSR", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString getCursorUSR(CXCursor param0);

        [DllImport(libraryPath, EntryPoint = "clang_constructUSR_ObjCClass", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString constructUSR_ObjCClass([NativeTypeName("const char *")] sbyte* class_name);

        [DllImport(libraryPath, EntryPoint = "clang_constructUSR_ObjCCategory", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString constructUSR_ObjCCategory([NativeTypeName("const char *")] sbyte* class_name, [NativeTypeName("const char *")] sbyte* category_name);

        [DllImport(libraryPath, EntryPoint = "clang_constructUSR_ObjCProtocol", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString constructUSR_ObjCProtocol([NativeTypeName("const char *")] sbyte* protocol_name);

        [DllImport(libraryPath, EntryPoint = "clang_constructUSR_ObjCIvar", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString constructUSR_ObjCIvar([NativeTypeName("const char *")] sbyte* name, CXString classUSR);

        [DllImport(libraryPath, EntryPoint = "clang_constructUSR_ObjCMethod", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString constructUSR_ObjCMethod([NativeTypeName("const char *")] sbyte* name, [NativeTypeName("unsigned int")] uint isInstanceMethod, CXString classUSR);

        [DllImport(libraryPath, EntryPoint = "clang_constructUSR_ObjCProperty", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString constructUSR_ObjCProperty([NativeTypeName("const char *")] sbyte* property, CXString classUSR);

        [DllImport(libraryPath, EntryPoint = "clang_getCursorSpelling", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString getCursorSpelling(CXCursor param0);

        [DllImport(libraryPath, EntryPoint = "clang_Cursor_getSpellingNameRange", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXSourceRange Cursor_getSpellingNameRange(CXCursor param0, [NativeTypeName("unsigned int")] uint pieceIndex, [NativeTypeName("unsigned int")] uint options);

        [DllImport(libraryPath, EntryPoint = "clang_PrintingPolicy_getProperty", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint PrintingPolicy_getProperty([NativeTypeName("CXPrintingPolicy")] void* Policy, [NativeTypeName("enum CXPrintingPolicyProperty")] CXPrintingPolicyProperty Property);

        [DllImport(libraryPath, EntryPoint = "clang_PrintingPolicy_setProperty", CallingConvention = CallingConvention.Cdecl)]
        public static extern void PrintingPolicy_setProperty([NativeTypeName("CXPrintingPolicy")] void* Policy, [NativeTypeName("enum CXPrintingPolicyProperty")] CXPrintingPolicyProperty Property, [NativeTypeName("unsigned int")] uint Value);

        [DllImport(libraryPath, EntryPoint = "clang_getCursorPrintingPolicy", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("CXPrintingPolicy")]
        public static extern void* getCursorPrintingPolicy(CXCursor param0);

        [DllImport(libraryPath, EntryPoint = "clang_PrintingPolicy_dispose", CallingConvention = CallingConvention.Cdecl)]
        public static extern void PrintingPolicy_dispose([NativeTypeName("CXPrintingPolicy")] void* Policy);

        [DllImport(libraryPath, EntryPoint = "clang_getCursorPrettyPrinted", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString getCursorPrettyPrinted(CXCursor Cursor, [NativeTypeName("CXPrintingPolicy")] void* Policy);

        [DllImport(libraryPath, EntryPoint = "clang_getCursorDisplayName", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString getCursorDisplayName(CXCursor param0);

        [DllImport(libraryPath, EntryPoint = "clang_getCursorReferenced", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXCursor getCursorReferenced(CXCursor param0);

        [DllImport(libraryPath, EntryPoint = "clang_getCursorDefinition", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXCursor getCursorDefinition(CXCursor param0);

        [DllImport(libraryPath, EntryPoint = "clang_isCursorDefinition", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint isCursorDefinition(CXCursor param0);

        [DllImport(libraryPath, EntryPoint = "clang_getCanonicalCursor", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXCursor getCanonicalCursor(CXCursor param0);

        [DllImport(libraryPath, EntryPoint = "clang_Cursor_getObjCSelectorIndex", CallingConvention = CallingConvention.Cdecl)]
        public static extern int Cursor_getObjCSelectorIndex(CXCursor param0);

        [DllImport(libraryPath, EntryPoint = "clang_Cursor_isDynamicCall", CallingConvention = CallingConvention.Cdecl)]
        public static extern int Cursor_isDynamicCall(CXCursor C);

        [DllImport(libraryPath, EntryPoint = "clang_Cursor_getReceiverType", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXType Cursor_getReceiverType(CXCursor C);

        [DllImport(libraryPath, EntryPoint = "clang_Cursor_getObjCPropertyAttributes", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint Cursor_getObjCPropertyAttributes(CXCursor C, [NativeTypeName("unsigned int")] uint reserved);

        [DllImport(libraryPath, EntryPoint = "clang_Cursor_getObjCPropertyGetterName", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString Cursor_getObjCPropertyGetterName(CXCursor C);

        [DllImport(libraryPath, EntryPoint = "clang_Cursor_getObjCPropertySetterName", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString Cursor_getObjCPropertySetterName(CXCursor C);

        [DllImport(libraryPath, EntryPoint = "clang_Cursor_getObjCDeclQualifiers", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint Cursor_getObjCDeclQualifiers(CXCursor C);

        [DllImport(libraryPath, EntryPoint = "clang_Cursor_isObjCOptional", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint Cursor_isObjCOptional(CXCursor C);

        [DllImport(libraryPath, EntryPoint = "clang_Cursor_isVariadic", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint Cursor_isVariadic(CXCursor C);

        [DllImport(libraryPath, EntryPoint = "clang_Cursor_isExternalSymbol", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint Cursor_isExternalSymbol(CXCursor C, [NativeTypeName("CXString *")] CXString* language, [NativeTypeName("CXString *")] CXString* definedIn, [NativeTypeName("unsigned int *")] uint* isGenerated);

        [DllImport(libraryPath, EntryPoint = "clang_Cursor_getCommentRange", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXSourceRange Cursor_getCommentRange(CXCursor C);

        [DllImport(libraryPath, EntryPoint = "clang_Cursor_getRawCommentText", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString Cursor_getRawCommentText(CXCursor C);

        [DllImport(libraryPath, EntryPoint = "clang_Cursor_getBriefCommentText", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString Cursor_getBriefCommentText(CXCursor C);

        [DllImport(libraryPath, EntryPoint = "clang_Cursor_getMangling", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString Cursor_getMangling(CXCursor param0);

        [DllImport(libraryPath, EntryPoint = "clang_Cursor_getCXXManglings", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("CXStringSet *")]
        public static extern CXStringSet* Cursor_getCXXManglings(CXCursor param0);

        [DllImport(libraryPath, EntryPoint = "clang_Cursor_getObjCManglings", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("CXStringSet *")]
        public static extern CXStringSet* Cursor_getObjCManglings(CXCursor param0);

        [DllImport(libraryPath, EntryPoint = "clang_Cursor_getModule", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("CXModule")]
        public static extern void* Cursor_getModule(CXCursor C);

        [DllImport(libraryPath, EntryPoint = "clang_getModuleForFile", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("CXModule")]
        public static extern void* getModuleForFile([NativeTypeName("CXTranslationUnit")] CXTranslationUnitImpl* param0, [NativeTypeName("CXFile")] void* param1);

        [DllImport(libraryPath, EntryPoint = "clang_Module_getASTFile", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("CXFile")]
        public static extern void* Module_getASTFile([NativeTypeName("CXModule")] void* Module);

        [DllImport(libraryPath, EntryPoint = "clang_Module_getParent", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("CXModule")]
        public static extern void* Module_getParent([NativeTypeName("CXModule")] void* Module);

        [DllImport(libraryPath, EntryPoint = "clang_Module_getName", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString Module_getName([NativeTypeName("CXModule")] void* Module);

        [DllImport(libraryPath, EntryPoint = "clang_Module_getFullName", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString Module_getFullName([NativeTypeName("CXModule")] void* Module);

        [DllImport(libraryPath, EntryPoint = "clang_Module_isSystem", CallingConvention = CallingConvention.Cdecl)]
        public static extern int Module_isSystem([NativeTypeName("CXModule")] void* Module);

        [DllImport(libraryPath, EntryPoint = "clang_Module_getNumTopLevelHeaders", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint Module_getNumTopLevelHeaders([NativeTypeName("CXTranslationUnit")] CXTranslationUnitImpl* param0, [NativeTypeName("CXModule")] void* Module);

        [DllImport(libraryPath, EntryPoint = "clang_Module_getTopLevelHeader", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("CXFile")]
        public static extern void* Module_getTopLevelHeader([NativeTypeName("CXTranslationUnit")] CXTranslationUnitImpl* param0, [NativeTypeName("CXModule")] void* Module, [NativeTypeName("unsigned int")] uint Index);

        [DllImport(libraryPath, EntryPoint = "clang_CXXConstructor_isConvertingConstructor", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint CXXConstructor_isConvertingConstructor(CXCursor C);

        [DllImport(libraryPath, EntryPoint = "clang_CXXConstructor_isCopyConstructor", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint CXXConstructor_isCopyConstructor(CXCursor C);

        [DllImport(libraryPath, EntryPoint = "clang_CXXConstructor_isDefaultConstructor", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint CXXConstructor_isDefaultConstructor(CXCursor C);

        [DllImport(libraryPath, EntryPoint = "clang_CXXConstructor_isMoveConstructor", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint CXXConstructor_isMoveConstructor(CXCursor C);

        [DllImport(libraryPath, EntryPoint = "clang_CXXField_isMutable", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint CXXField_isMutable(CXCursor C);

        [DllImport(libraryPath, EntryPoint = "clang_CXXMethod_isDefaulted", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint CXXMethod_isDefaulted(CXCursor C);

        [DllImport(libraryPath, EntryPoint = "clang_CXXMethod_isPureVirtual", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint CXXMethod_isPureVirtual(CXCursor C);

        [DllImport(libraryPath, EntryPoint = "clang_CXXMethod_isStatic", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint CXXMethod_isStatic(CXCursor C);

        [DllImport(libraryPath, EntryPoint = "clang_CXXMethod_isVirtual", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint CXXMethod_isVirtual(CXCursor C);

        [DllImport(libraryPath, EntryPoint = "clang_CXXRecord_isAbstract", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint CXXRecord_isAbstract(CXCursor C);

        [DllImport(libraryPath, EntryPoint = "clang_EnumDecl_isScoped", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint EnumDecl_isScoped(CXCursor C);

        [DllImport(libraryPath, EntryPoint = "clang_CXXMethod_isConst", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint CXXMethod_isConst(CXCursor C);

        [DllImport(libraryPath, EntryPoint = "clang_getTemplateCursorKind", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("enum CXCursorKind")]
        public static extern CXCursorKind getTemplateCursorKind(CXCursor C);

        [DllImport(libraryPath, EntryPoint = "clang_getSpecializedCursorTemplate", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXCursor getSpecializedCursorTemplate(CXCursor C);

        [DllImport(libraryPath, EntryPoint = "clang_getCursorReferenceNameRange", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXSourceRange getCursorReferenceNameRange(CXCursor C, [NativeTypeName("unsigned int")] uint NameFlags, [NativeTypeName("unsigned int")] uint PieceIndex);

        [DllImport(libraryPath, EntryPoint = "clang_getToken", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("CXToken *")]
        public static extern CXToken* getToken([NativeTypeName("CXTranslationUnit")] CXTranslationUnitImpl* TU, CXSourceLocation Location);

        [DllImport(libraryPath, EntryPoint = "clang_getTokenKind", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXTokenKind getTokenKind(CXToken param0);

        [DllImport(libraryPath, EntryPoint = "clang_getTokenSpelling", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString getTokenSpelling([NativeTypeName("CXTranslationUnit")] CXTranslationUnitImpl* param0, CXToken param1);

        [DllImport(libraryPath, EntryPoint = "clang_getTokenLocation", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXSourceLocation getTokenLocation([NativeTypeName("CXTranslationUnit")] CXTranslationUnitImpl* param0, CXToken param1);

        [DllImport(libraryPath, EntryPoint = "clang_getTokenExtent", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXSourceRange getTokenExtent([NativeTypeName("CXTranslationUnit")] CXTranslationUnitImpl* param0, CXToken param1);

        [DllImport(libraryPath, EntryPoint = "clang_tokenize", CallingConvention = CallingConvention.Cdecl)]
        public static extern void tokenize([NativeTypeName("CXTranslationUnit")] CXTranslationUnitImpl* TU, CXSourceRange Range, [NativeTypeName("CXToken **")] CXToken** Tokens, [NativeTypeName("unsigned int *")] uint* NumTokens);

        [DllImport(libraryPath, EntryPoint = "clang_annotateTokens", CallingConvention = CallingConvention.Cdecl)]
        public static extern void annotateTokens([NativeTypeName("CXTranslationUnit")] CXTranslationUnitImpl* TU, [NativeTypeName("CXToken *")] CXToken* Tokens, [NativeTypeName("unsigned int")] uint NumTokens, [NativeTypeName("CXCursor *")] CXCursor* Cursors);

        [DllImport(libraryPath, EntryPoint = "clang_disposeTokens", CallingConvention = CallingConvention.Cdecl)]
        public static extern void disposeTokens([NativeTypeName("CXTranslationUnit")] CXTranslationUnitImpl* TU, [NativeTypeName("CXToken *")] CXToken* Tokens, [NativeTypeName("unsigned int")] uint NumTokens);

        [DllImport(libraryPath, EntryPoint = "clang_getCursorKindSpelling", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString getCursorKindSpelling([NativeTypeName("enum CXCursorKind")] CXCursorKind Kind);

        [DllImport(libraryPath, EntryPoint = "clang_getDefinitionSpellingAndExtent", CallingConvention = CallingConvention.Cdecl)]
        public static extern void getDefinitionSpellingAndExtent(CXCursor param0, [NativeTypeName("const char **")] sbyte** startBuf, [NativeTypeName("const char **")] sbyte** endBuf, [NativeTypeName("unsigned int *")] uint* startLine, [NativeTypeName("unsigned int *")] uint* startColumn, [NativeTypeName("unsigned int *")] uint* endLine, [NativeTypeName("unsigned int *")] uint* endColumn);

        [DllImport(libraryPath, EntryPoint = "clang_enableStackTraces", CallingConvention = CallingConvention.Cdecl)]
        public static extern void enableStackTraces();

        [DllImport(libraryPath, EntryPoint = "clang_executeOnThread", CallingConvention = CallingConvention.Cdecl)]
        public static extern void executeOnThread([NativeTypeName("void (*)(void *)")] IntPtr fn, [NativeTypeName("void *")] void* user_data, [NativeTypeName("unsigned int")] uint stack_size);

        [DllImport(libraryPath, EntryPoint = "clang_getCompletionChunkKind", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("enum CXCompletionChunkKind")]
        public static extern CXCompletionChunkKind getCompletionChunkKind([NativeTypeName("CXCompletionString")] void* completion_string, [NativeTypeName("unsigned int")] uint chunk_number);

        [DllImport(libraryPath, EntryPoint = "clang_getCompletionChunkText", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString getCompletionChunkText([NativeTypeName("CXCompletionString")] void* completion_string, [NativeTypeName("unsigned int")] uint chunk_number);

        [DllImport(libraryPath, EntryPoint = "clang_getCompletionChunkCompletionString", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("CXCompletionString")]
        public static extern void* getCompletionChunkCompletionString([NativeTypeName("CXCompletionString")] void* completion_string, [NativeTypeName("unsigned int")] uint chunk_number);

        [DllImport(libraryPath, EntryPoint = "clang_getNumCompletionChunks", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint getNumCompletionChunks([NativeTypeName("CXCompletionString")] void* completion_string);

        [DllImport(libraryPath, EntryPoint = "clang_getCompletionPriority", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint getCompletionPriority([NativeTypeName("CXCompletionString")] void* completion_string);

        [DllImport(libraryPath, EntryPoint = "clang_getCompletionAvailability", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("enum CXAvailabilityKind")]
        public static extern CXAvailabilityKind getCompletionAvailability([NativeTypeName("CXCompletionString")] void* completion_string);

        [DllImport(libraryPath, EntryPoint = "clang_getCompletionNumAnnotations", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint getCompletionNumAnnotations([NativeTypeName("CXCompletionString")] void* completion_string);

        [DllImport(libraryPath, EntryPoint = "clang_getCompletionAnnotation", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString getCompletionAnnotation([NativeTypeName("CXCompletionString")] void* completion_string, [NativeTypeName("unsigned int")] uint annotation_number);

        [DllImport(libraryPath, EntryPoint = "clang_getCompletionParent", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString getCompletionParent([NativeTypeName("CXCompletionString")] void* completion_string, [NativeTypeName("enum CXCursorKind *")] CXCursorKind* kind);

        [DllImport(libraryPath, EntryPoint = "clang_getCompletionBriefComment", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString getCompletionBriefComment([NativeTypeName("CXCompletionString")] void* completion_string);

        [DllImport(libraryPath, EntryPoint = "clang_getCursorCompletionString", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("CXCompletionString")]
        public static extern void* getCursorCompletionString(CXCursor cursor);

        [DllImport(libraryPath, EntryPoint = "clang_getCompletionNumFixIts", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint getCompletionNumFixIts([NativeTypeName("CXCodeCompleteResults *")] CXCodeCompleteResults* results, [NativeTypeName("unsigned int")] uint completion_index);

        [DllImport(libraryPath, EntryPoint = "clang_getCompletionFixIt", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString getCompletionFixIt([NativeTypeName("CXCodeCompleteResults *")] CXCodeCompleteResults* results, [NativeTypeName("unsigned int")] uint completion_index, [NativeTypeName("unsigned int")] uint fixit_index, [NativeTypeName("CXSourceRange *")] CXSourceRange* replacement_range);

        [DllImport(libraryPath, EntryPoint = "clang_defaultCodeCompleteOptions", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint defaultCodeCompleteOptions();

        [DllImport(libraryPath, EntryPoint = "clang_codeCompleteAt", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("CXCodeCompleteResults *")]
        public static extern CXCodeCompleteResults* codeCompleteAt([NativeTypeName("CXTranslationUnit")] CXTranslationUnitImpl* TU, [NativeTypeName("const char *")] sbyte* complete_filename, [NativeTypeName("unsigned int")] uint complete_line, [NativeTypeName("unsigned int")] uint complete_column, [NativeTypeName("struct CXUnsavedFile *")] CXUnsavedFile* unsaved_files, [NativeTypeName("unsigned int")] uint num_unsaved_files, [NativeTypeName("unsigned int")] uint options);

        [DllImport(libraryPath, EntryPoint = "clang_sortCodeCompletionResults", CallingConvention = CallingConvention.Cdecl)]
        public static extern void sortCodeCompletionResults([NativeTypeName("CXCompletionResult *")] CXCompletionResult* Results, [NativeTypeName("unsigned int")] uint NumResults);

        [DllImport(libraryPath, EntryPoint = "clang_disposeCodeCompleteResults", CallingConvention = CallingConvention.Cdecl)]
        public static extern void disposeCodeCompleteResults([NativeTypeName("CXCodeCompleteResults *")] CXCodeCompleteResults* Results);

        [DllImport(libraryPath, EntryPoint = "clang_codeCompleteGetNumDiagnostics", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint codeCompleteGetNumDiagnostics([NativeTypeName("CXCodeCompleteResults *")] CXCodeCompleteResults* Results);

        [DllImport(libraryPath, EntryPoint = "clang_codeCompleteGetDiagnostic", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("CXDiagnostic")]
        public static extern void* codeCompleteGetDiagnostic([NativeTypeName("CXCodeCompleteResults *")] CXCodeCompleteResults* Results, [NativeTypeName("unsigned int")] uint Index);

        [DllImport(libraryPath, EntryPoint = "clang_codeCompleteGetContexts", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("unsigned long long")]
        public static extern ulong codeCompleteGetContexts([NativeTypeName("CXCodeCompleteResults *")] CXCodeCompleteResults* Results);

        [DllImport(libraryPath, EntryPoint = "clang_codeCompleteGetContainerKind", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("enum CXCursorKind")]
        public static extern CXCursorKind codeCompleteGetContainerKind([NativeTypeName("CXCodeCompleteResults *")] CXCodeCompleteResults* Results, [NativeTypeName("unsigned int *")] uint* IsIncomplete);

        [DllImport(libraryPath, EntryPoint = "clang_codeCompleteGetContainerUSR", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString codeCompleteGetContainerUSR([NativeTypeName("CXCodeCompleteResults *")] CXCodeCompleteResults* Results);

        [DllImport(libraryPath, EntryPoint = "clang_codeCompleteGetObjCSelector", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString codeCompleteGetObjCSelector([NativeTypeName("CXCodeCompleteResults *")] CXCodeCompleteResults* Results);

        [DllImport(libraryPath, EntryPoint = "clang_getClangVersion", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString getClangVersion();

        [DllImport(libraryPath, EntryPoint = "clang_toggleCrashRecovery", CallingConvention = CallingConvention.Cdecl)]
        public static extern void toggleCrashRecovery([NativeTypeName("unsigned int")] uint isEnabled);

        [DllImport(libraryPath, EntryPoint = "clang_getInclusions", CallingConvention = CallingConvention.Cdecl)]
        public static extern void getInclusions([NativeTypeName("CXTranslationUnit")] CXTranslationUnitImpl* tu, [NativeTypeName("CXInclusionVisitor")] IntPtr visitor, [NativeTypeName("CXClientData")] void* client_data);

        [DllImport(libraryPath, EntryPoint = "clang_Cursor_Evaluate", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("CXEvalResult")]
        public static extern void* Cursor_Evaluate(CXCursor C);

        [DllImport(libraryPath, EntryPoint = "clang_EvalResult_getKind", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXEvalResultKind EvalResult_getKind([NativeTypeName("CXEvalResult")] void* E);

        [DllImport(libraryPath, EntryPoint = "clang_EvalResult_getAsInt", CallingConvention = CallingConvention.Cdecl)]
        public static extern int EvalResult_getAsInt([NativeTypeName("CXEvalResult")] void* E);

        [DllImport(libraryPath, EntryPoint = "clang_EvalResult_getAsLongLong", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("long long")]
        public static extern long EvalResult_getAsLongLong([NativeTypeName("CXEvalResult")] void* E);

        [DllImport(libraryPath, EntryPoint = "clang_EvalResult_isUnsignedInt", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint EvalResult_isUnsignedInt([NativeTypeName("CXEvalResult")] void* E);

        [DllImport(libraryPath, EntryPoint = "clang_EvalResult_getAsUnsigned", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("unsigned long long")]
        public static extern ulong EvalResult_getAsUnsigned([NativeTypeName("CXEvalResult")] void* E);

        [DllImport(libraryPath, EntryPoint = "clang_EvalResult_getAsDouble", CallingConvention = CallingConvention.Cdecl)]
        public static extern double EvalResult_getAsDouble([NativeTypeName("CXEvalResult")] void* E);

        [DllImport(libraryPath, EntryPoint = "clang_EvalResult_getAsStr", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("const char *")]
        public static extern sbyte* EvalResult_getAsStr([NativeTypeName("CXEvalResult")] void* E);

        [DllImport(libraryPath, EntryPoint = "clang_EvalResult_dispose", CallingConvention = CallingConvention.Cdecl)]
        public static extern void EvalResult_dispose([NativeTypeName("CXEvalResult")] void* E);

        [DllImport(libraryPath, EntryPoint = "clang_getRemappings", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("CXRemapping")]
        public static extern void* getRemappings([NativeTypeName("const char *")] sbyte* path);

        [DllImport(libraryPath, EntryPoint = "clang_getRemappingsFromFileList", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("CXRemapping")]
        public static extern void* getRemappingsFromFileList([NativeTypeName("const char **")] sbyte** filePaths, [NativeTypeName("unsigned int")] uint numFiles);

        [DllImport(libraryPath, EntryPoint = "clang_remap_getNumFiles", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint remap_getNumFiles([NativeTypeName("CXRemapping")] void* param0);

        [DllImport(libraryPath, EntryPoint = "clang_remap_getFilenames", CallingConvention = CallingConvention.Cdecl)]
        public static extern void remap_getFilenames([NativeTypeName("CXRemapping")] void* param0, [NativeTypeName("unsigned int")] uint index, [NativeTypeName("CXString *")] CXString* original, [NativeTypeName("CXString *")] CXString* transformed);

        [DllImport(libraryPath, EntryPoint = "clang_remap_dispose", CallingConvention = CallingConvention.Cdecl)]
        public static extern void remap_dispose([NativeTypeName("CXRemapping")] void* param0);

        [DllImport(libraryPath, EntryPoint = "clang_findReferencesInFile", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXResult findReferencesInFile(CXCursor cursor, [NativeTypeName("CXFile")] void* file, CXCursorAndRangeVisitor visitor);

        [DllImport(libraryPath, EntryPoint = "clang_findIncludesInFile", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXResult findIncludesInFile([NativeTypeName("CXTranslationUnit")] CXTranslationUnitImpl* TU, [NativeTypeName("CXFile")] void* file, CXCursorAndRangeVisitor visitor);

        [DllImport(libraryPath, EntryPoint = "clang_index_isEntityObjCContainerKind", CallingConvention = CallingConvention.Cdecl)]
        public static extern int index_isEntityObjCContainerKind(CXIdxEntityKind param0);

        [DllImport(libraryPath, EntryPoint = "clang_index_getObjCContainerDeclInfo", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("const CXIdxObjCContainerDeclInfo *")]
        public static extern CXIdxObjCContainerDeclInfo* index_getObjCContainerDeclInfo([NativeTypeName("const CXIdxDeclInfo *")] CXIdxDeclInfo* param0);

        [DllImport(libraryPath, EntryPoint = "clang_index_getObjCInterfaceDeclInfo", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("const CXIdxObjCInterfaceDeclInfo *")]
        public static extern CXIdxObjCInterfaceDeclInfo* index_getObjCInterfaceDeclInfo([NativeTypeName("const CXIdxDeclInfo *")] CXIdxDeclInfo* param0);

        [DllImport(libraryPath, EntryPoint = "clang_index_getObjCCategoryDeclInfo", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("const CXIdxObjCCategoryDeclInfo *")]
        public static extern CXIdxObjCCategoryDeclInfo* index_getObjCCategoryDeclInfo([NativeTypeName("const CXIdxDeclInfo *")] CXIdxDeclInfo* param0);

        [DllImport(libraryPath, EntryPoint = "clang_index_getObjCProtocolRefListInfo", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("const CXIdxObjCProtocolRefListInfo *")]
        public static extern CXIdxObjCProtocolRefListInfo* index_getObjCProtocolRefListInfo([NativeTypeName("const CXIdxDeclInfo *")] CXIdxDeclInfo* param0);

        [DllImport(libraryPath, EntryPoint = "clang_index_getObjCPropertyDeclInfo", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("const CXIdxObjCPropertyDeclInfo *")]
        public static extern CXIdxObjCPropertyDeclInfo* index_getObjCPropertyDeclInfo([NativeTypeName("const CXIdxDeclInfo *")] CXIdxDeclInfo* param0);

        [DllImport(libraryPath, EntryPoint = "clang_index_getIBOutletCollectionAttrInfo", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("const CXIdxIBOutletCollectionAttrInfo *")]
        public static extern CXIdxIBOutletCollectionAttrInfo* index_getIBOutletCollectionAttrInfo([NativeTypeName("const CXIdxAttrInfo *")] CXIdxAttrInfo* param0);

        [DllImport(libraryPath, EntryPoint = "clang_index_getCXXClassDeclInfo", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("const CXIdxCXXClassDeclInfo *")]
        public static extern CXIdxCXXClassDeclInfo* index_getCXXClassDeclInfo([NativeTypeName("const CXIdxDeclInfo *")] CXIdxDeclInfo* param0);

        [DllImport(libraryPath, EntryPoint = "clang_index_getClientContainer", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("CXIdxClientContainer")]
        public static extern void* index_getClientContainer([NativeTypeName("const CXIdxContainerInfo *")] CXIdxContainerInfo* param0);

        [DllImport(libraryPath, EntryPoint = "clang_index_setClientContainer", CallingConvention = CallingConvention.Cdecl)]
        public static extern void index_setClientContainer([NativeTypeName("const CXIdxContainerInfo *")] CXIdxContainerInfo* param0, [NativeTypeName("CXIdxClientContainer")] void* param1);

        [DllImport(libraryPath, EntryPoint = "clang_index_getClientEntity", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("CXIdxClientEntity")]
        public static extern void* index_getClientEntity([NativeTypeName("const CXIdxEntityInfo *")] CXIdxEntityInfo* param0);

        [DllImport(libraryPath, EntryPoint = "clang_index_setClientEntity", CallingConvention = CallingConvention.Cdecl)]
        public static extern void index_setClientEntity([NativeTypeName("const CXIdxEntityInfo *")] CXIdxEntityInfo* param0, [NativeTypeName("CXIdxClientEntity")] void* param1);

        [DllImport(libraryPath, EntryPoint = "clang_IndexAction_create", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("CXIndexAction")]
        public static extern void* IndexAction_create([NativeTypeName("CXIndex")] void* CIdx);

        [DllImport(libraryPath, EntryPoint = "clang_IndexAction_dispose", CallingConvention = CallingConvention.Cdecl)]
        public static extern void IndexAction_dispose([NativeTypeName("CXIndexAction")] void* param0);

        [DllImport(libraryPath, EntryPoint = "clang_indexSourceFile", CallingConvention = CallingConvention.Cdecl)]
        public static extern int indexSourceFile([NativeTypeName("CXIndexAction")] void* param0, [NativeTypeName("CXClientData")] void* client_data, [NativeTypeName("IndexerCallbacks *")] IndexerCallbacks* index_callbacks, [NativeTypeName("unsigned int")] uint index_callbacks_size, [NativeTypeName("unsigned int")] uint index_options, [NativeTypeName("const char *")] sbyte* source_filename, [NativeTypeName("const char *const *")] sbyte** command_line_args, int num_command_line_args, [NativeTypeName("struct CXUnsavedFile *")] CXUnsavedFile* unsaved_files, [NativeTypeName("unsigned int")] uint num_unsaved_files, [NativeTypeName("CXTranslationUnit *")] CXTranslationUnitImpl** out_TU, [NativeTypeName("unsigned int")] uint TU_options);

        [DllImport(libraryPath, EntryPoint = "clang_indexSourceFileFullArgv", CallingConvention = CallingConvention.Cdecl)]
        public static extern int indexSourceFileFullArgv([NativeTypeName("CXIndexAction")] void* param0, [NativeTypeName("CXClientData")] void* client_data, [NativeTypeName("IndexerCallbacks *")] IndexerCallbacks* index_callbacks, [NativeTypeName("unsigned int")] uint index_callbacks_size, [NativeTypeName("unsigned int")] uint index_options, [NativeTypeName("const char *")] sbyte* source_filename, [NativeTypeName("const char *const *")] sbyte** command_line_args, int num_command_line_args, [NativeTypeName("struct CXUnsavedFile *")] CXUnsavedFile* unsaved_files, [NativeTypeName("unsigned int")] uint num_unsaved_files, [NativeTypeName("CXTranslationUnit *")] CXTranslationUnitImpl** out_TU, [NativeTypeName("unsigned int")] uint TU_options);

        [DllImport(libraryPath, EntryPoint = "clang_indexTranslationUnit", CallingConvention = CallingConvention.Cdecl)]
        public static extern int indexTranslationUnit([NativeTypeName("CXIndexAction")] void* param0, [NativeTypeName("CXClientData")] void* client_data, [NativeTypeName("IndexerCallbacks *")] IndexerCallbacks* index_callbacks, [NativeTypeName("unsigned int")] uint index_callbacks_size, [NativeTypeName("unsigned int")] uint index_options, [NativeTypeName("CXTranslationUnit")] CXTranslationUnitImpl* param5);

        [DllImport(libraryPath, EntryPoint = "clang_indexLoc_getFileLocation", CallingConvention = CallingConvention.Cdecl)]
        public static extern void indexLoc_getFileLocation(CXIdxLoc loc, [NativeTypeName("CXIdxClientFile *")] void** indexFile, [NativeTypeName("CXFile *")] void** file, [NativeTypeName("unsigned int *")] uint* line, [NativeTypeName("unsigned int *")] uint* column, [NativeTypeName("unsigned int *")] uint* offset);

        [DllImport(libraryPath, EntryPoint = "clang_indexLoc_getCXSourceLocation", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXSourceLocation indexLoc_getCXSourceLocation(CXIdxLoc loc);

        [DllImport(libraryPath, EntryPoint = "clang_Type_visitFields", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint Type_visitFields(CXType T, [NativeTypeName("CXFieldVisitor")] IntPtr visitor, [NativeTypeName("CXClientData")] void* client_data);

        [DllImport(libraryPath, EntryPoint = "clang_getCString", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("const char *")]
        public static extern sbyte* getCString(CXString @string);

        [DllImport(libraryPath, EntryPoint = "clang_disposeString", CallingConvention = CallingConvention.Cdecl)]
        public static extern void disposeString(CXString @string);

        [DllImport(libraryPath, EntryPoint = "clang_disposeStringSet", CallingConvention = CallingConvention.Cdecl)]
        public static extern void disposeStringSet([NativeTypeName("CXStringSet *")] CXStringSet* set);

        [DllImport(libraryPath, EntryPoint = "clang_Cursor_getParsedComment", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXComment Cursor_getParsedComment(CXCursor C);

        [DllImport(libraryPath, EntryPoint = "clang_Comment_getKind", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("enum CXCommentKind")]
        public static extern CXCommentKind Comment_getKind(CXComment Comment);

        [DllImport(libraryPath, EntryPoint = "clang_Comment_getNumChildren", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint Comment_getNumChildren(CXComment Comment);

        [DllImport(libraryPath, EntryPoint = "clang_Comment_getChild", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXComment Comment_getChild(CXComment Comment, [NativeTypeName("unsigned int")] uint ChildIdx);

        [DllImport(libraryPath, EntryPoint = "clang_Comment_isWhitespace", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint Comment_isWhitespace(CXComment Comment);

        [DllImport(libraryPath, EntryPoint = "clang_InlineContentComment_hasTrailingNewline", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint InlineContentComment_hasTrailingNewline(CXComment Comment);

        [DllImport(libraryPath, EntryPoint = "clang_TextComment_getText", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString TextComment_getText(CXComment Comment);

        [DllImport(libraryPath, EntryPoint = "clang_InlineCommandComment_getCommandName", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString InlineCommandComment_getCommandName(CXComment Comment);

        [DllImport(libraryPath, EntryPoint = "clang_InlineCommandComment_getRenderKind", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("enum CXCommentInlineCommandRenderKind")]
        public static extern CXCommentInlineCommandRenderKind InlineCommandComment_getRenderKind(CXComment Comment);

        [DllImport(libraryPath, EntryPoint = "clang_InlineCommandComment_getNumArgs", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint InlineCommandComment_getNumArgs(CXComment Comment);

        [DllImport(libraryPath, EntryPoint = "clang_InlineCommandComment_getArgText", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString InlineCommandComment_getArgText(CXComment Comment, [NativeTypeName("unsigned int")] uint ArgIdx);

        [DllImport(libraryPath, EntryPoint = "clang_HTMLTagComment_getTagName", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString HTMLTagComment_getTagName(CXComment Comment);

        [DllImport(libraryPath, EntryPoint = "clang_HTMLStartTagComment_isSelfClosing", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint HTMLStartTagComment_isSelfClosing(CXComment Comment);

        [DllImport(libraryPath, EntryPoint = "clang_HTMLStartTag_getNumAttrs", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint HTMLStartTag_getNumAttrs(CXComment Comment);

        [DllImport(libraryPath, EntryPoint = "clang_HTMLStartTag_getAttrName", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString HTMLStartTag_getAttrName(CXComment Comment, [NativeTypeName("unsigned int")] uint AttrIdx);

        [DllImport(libraryPath, EntryPoint = "clang_HTMLStartTag_getAttrValue", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString HTMLStartTag_getAttrValue(CXComment Comment, [NativeTypeName("unsigned int")] uint AttrIdx);

        [DllImport(libraryPath, EntryPoint = "clang_BlockCommandComment_getCommandName", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString BlockCommandComment_getCommandName(CXComment Comment);

        [DllImport(libraryPath, EntryPoint = "clang_BlockCommandComment_getNumArgs", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint BlockCommandComment_getNumArgs(CXComment Comment);

        [DllImport(libraryPath, EntryPoint = "clang_BlockCommandComment_getArgText", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString BlockCommandComment_getArgText(CXComment Comment, [NativeTypeName("unsigned int")] uint ArgIdx);

        [DllImport(libraryPath, EntryPoint = "clang_BlockCommandComment_getParagraph", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXComment BlockCommandComment_getParagraph(CXComment Comment);

        [DllImport(libraryPath, EntryPoint = "clang_ParamCommandComment_getParamName", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString ParamCommandComment_getParamName(CXComment Comment);

        [DllImport(libraryPath, EntryPoint = "clang_ParamCommandComment_isParamIndexValid", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint ParamCommandComment_isParamIndexValid(CXComment Comment);

        [DllImport(libraryPath, EntryPoint = "clang_ParamCommandComment_getParamIndex", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint ParamCommandComment_getParamIndex(CXComment Comment);

        [DllImport(libraryPath, EntryPoint = "clang_ParamCommandComment_isDirectionExplicit", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint ParamCommandComment_isDirectionExplicit(CXComment Comment);

        [DllImport(libraryPath, EntryPoint = "clang_ParamCommandComment_getDirection", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("enum CXCommentParamPassDirection")]
        public static extern CXCommentParamPassDirection ParamCommandComment_getDirection(CXComment Comment);

        [DllImport(libraryPath, EntryPoint = "clang_TParamCommandComment_getParamName", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString TParamCommandComment_getParamName(CXComment Comment);

        [DllImport(libraryPath, EntryPoint = "clang_TParamCommandComment_isParamPositionValid", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint TParamCommandComment_isParamPositionValid(CXComment Comment);

        [DllImport(libraryPath, EntryPoint = "clang_TParamCommandComment_getDepth", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint TParamCommandComment_getDepth(CXComment Comment);

        [DllImport(libraryPath, EntryPoint = "clang_TParamCommandComment_getIndex", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint TParamCommandComment_getIndex(CXComment Comment, [NativeTypeName("unsigned int")] uint Depth);

        [DllImport(libraryPath, EntryPoint = "clang_VerbatimBlockLineComment_getText", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString VerbatimBlockLineComment_getText(CXComment Comment);

        [DllImport(libraryPath, EntryPoint = "clang_VerbatimLineComment_getText", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString VerbatimLineComment_getText(CXComment Comment);

        [DllImport(libraryPath, EntryPoint = "clang_HTMLTagComment_getAsString", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString HTMLTagComment_getAsString(CXComment Comment);

        [DllImport(libraryPath, EntryPoint = "clang_FullComment_getAsHTML", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString FullComment_getAsHTML(CXComment Comment);

        [DllImport(libraryPath, EntryPoint = "clang_FullComment_getAsXML", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString FullComment_getAsXML(CXComment Comment);

        [DllImport(libraryPath, EntryPoint = "clang_getBuildSessionTimestamp", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("unsigned long long")]
        public static extern ulong getBuildSessionTimestamp();

        [DllImport(libraryPath, EntryPoint = "clang_VirtualFileOverlay_create", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("CXVirtualFileOverlay")]
        public static extern CXVirtualFileOverlayImpl* VirtualFileOverlay_create([NativeTypeName("unsigned int")] uint options);

        [DllImport(libraryPath, EntryPoint = "clang_VirtualFileOverlay_addFileMapping", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("enum CXErrorCode")]
        public static extern CXErrorCode VirtualFileOverlay_addFileMapping([NativeTypeName("CXVirtualFileOverlay")] CXVirtualFileOverlayImpl* param0, [NativeTypeName("const char *")] sbyte* virtualPath, [NativeTypeName("const char *")] sbyte* realPath);

        [DllImport(libraryPath, EntryPoint = "clang_VirtualFileOverlay_setCaseSensitivity", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("enum CXErrorCode")]
        public static extern CXErrorCode VirtualFileOverlay_setCaseSensitivity([NativeTypeName("CXVirtualFileOverlay")] CXVirtualFileOverlayImpl* param0, int caseSensitive);

        [DllImport(libraryPath, EntryPoint = "clang_VirtualFileOverlay_writeToBuffer", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("enum CXErrorCode")]
        public static extern CXErrorCode VirtualFileOverlay_writeToBuffer([NativeTypeName("CXVirtualFileOverlay")] CXVirtualFileOverlayImpl* param0, [NativeTypeName("unsigned int")] uint options, [NativeTypeName("char **")] sbyte** out_buffer_ptr, [NativeTypeName("unsigned int *")] uint* out_buffer_size);

        [DllImport(libraryPath, EntryPoint = "clang_free", CallingConvention = CallingConvention.Cdecl)]
        public static extern void free([NativeTypeName("void *")] void* buffer);

        [DllImport(libraryPath, EntryPoint = "clang_VirtualFileOverlay_dispose", CallingConvention = CallingConvention.Cdecl)]
        public static extern void VirtualFileOverlay_dispose([NativeTypeName("CXVirtualFileOverlay")] CXVirtualFileOverlayImpl* param0);

        [DllImport(libraryPath, EntryPoint = "clang_ModuleMapDescriptor_create", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("CXModuleMapDescriptor")]
        public static extern CXModuleMapDescriptorImpl* ModuleMapDescriptor_create([NativeTypeName("unsigned int")] uint options);

        [DllImport(libraryPath, EntryPoint = "clang_ModuleMapDescriptor_setFrameworkModuleName", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("enum CXErrorCode")]
        public static extern CXErrorCode ModuleMapDescriptor_setFrameworkModuleName([NativeTypeName("CXModuleMapDescriptor")] CXModuleMapDescriptorImpl* param0, [NativeTypeName("const char *")] sbyte* name);

        [DllImport(libraryPath, EntryPoint = "clang_ModuleMapDescriptor_setUmbrellaHeader", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("enum CXErrorCode")]
        public static extern CXErrorCode ModuleMapDescriptor_setUmbrellaHeader([NativeTypeName("CXModuleMapDescriptor")] CXModuleMapDescriptorImpl* param0, [NativeTypeName("const char *")] sbyte* name);

        [DllImport(libraryPath, EntryPoint = "clang_ModuleMapDescriptor_writeToBuffer", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("enum CXErrorCode")]
        public static extern CXErrorCode ModuleMapDescriptor_writeToBuffer([NativeTypeName("CXModuleMapDescriptor")] CXModuleMapDescriptorImpl* param0, [NativeTypeName("unsigned int")] uint options, [NativeTypeName("char **")] sbyte** out_buffer_ptr, [NativeTypeName("unsigned int *")] uint* out_buffer_size);

        [DllImport(libraryPath, EntryPoint = "clang_ModuleMapDescriptor_dispose", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ModuleMapDescriptor_dispose([NativeTypeName("CXModuleMapDescriptor")] CXModuleMapDescriptorImpl* param0);

        [DllImport(libraryPath, EntryPoint = "clang_CompilationDatabase_fromDirectory", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("CXCompilationDatabase")]
        public static extern void* CompilationDatabase_fromDirectory([NativeTypeName("const char *")] sbyte* BuildDir, [NativeTypeName("CXCompilationDatabase_Error *")] CXCompilationDatabase_Error* ErrorCode);

        [DllImport(libraryPath, EntryPoint = "clang_CompilationDatabase_dispose", CallingConvention = CallingConvention.Cdecl)]
        public static extern void CompilationDatabase_dispose([NativeTypeName("CXCompilationDatabase")] void* param0);

        [DllImport(libraryPath, EntryPoint = "clang_CompilationDatabase_getCompileCommands", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("CXCompileCommands")]
        public static extern void* CompilationDatabase_getCompileCommands([NativeTypeName("CXCompilationDatabase")] void* param0, [NativeTypeName("const char *")] sbyte* CompleteFileName);

        [DllImport(libraryPath, EntryPoint = "clang_CompilationDatabase_getAllCompileCommands", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("CXCompileCommands")]
        public static extern void* CompilationDatabase_getAllCompileCommands([NativeTypeName("CXCompilationDatabase")] void* param0);

        [DllImport(libraryPath, EntryPoint = "clang_CompileCommands_dispose", CallingConvention = CallingConvention.Cdecl)]
        public static extern void CompileCommands_dispose([NativeTypeName("CXCompileCommands")] void* param0);

        [DllImport(libraryPath, EntryPoint = "clang_CompileCommands_getSize", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint CompileCommands_getSize([NativeTypeName("CXCompileCommands")] void* param0);

        [DllImport(libraryPath, EntryPoint = "clang_CompileCommands_getCommand", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("CXCompileCommand")]
        public static extern void* CompileCommands_getCommand([NativeTypeName("CXCompileCommands")] void* param0, [NativeTypeName("unsigned int")] uint I);

        [DllImport(libraryPath, EntryPoint = "clang_CompileCommand_getDirectory", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString CompileCommand_getDirectory([NativeTypeName("CXCompileCommand")] void* param0);

        [DllImport(libraryPath, EntryPoint = "clang_CompileCommand_getFilename", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString CompileCommand_getFilename([NativeTypeName("CXCompileCommand")] void* param0);

        [DllImport(libraryPath, EntryPoint = "clang_CompileCommand_getNumArgs", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint CompileCommand_getNumArgs([NativeTypeName("CXCompileCommand")] void* param0);

        [DllImport(libraryPath, EntryPoint = "clang_CompileCommand_getArg", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString CompileCommand_getArg([NativeTypeName("CXCompileCommand")] void* param0, [NativeTypeName("unsigned int")] uint I);

        [DllImport(libraryPath, EntryPoint = "clang_CompileCommand_getNumMappedSources", CallingConvention = CallingConvention.Cdecl)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint CompileCommand_getNumMappedSources([NativeTypeName("CXCompileCommand")] void* param0);

        [DllImport(libraryPath, EntryPoint = "clang_CompileCommand_getMappedSourcePath", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString CompileCommand_getMappedSourcePath([NativeTypeName("CXCompileCommand")] void* param0, [NativeTypeName("unsigned int")] uint I);

        [DllImport(libraryPath, EntryPoint = "clang_CompileCommand_getMappedSourceContent", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString CompileCommand_getMappedSourceContent([NativeTypeName("CXCompileCommand")] void* param0, [NativeTypeName("unsigned int")] uint I);
    }
}
