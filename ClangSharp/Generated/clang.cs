using System;
using System.Runtime.InteropServices;

namespace ClangSharp
{
    public static partial class clang
    {
        private const string libraryPath = "libclang";

        [DllImport(libraryPath, EntryPoint = "clang_createIndex", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXIndex createIndex(int excludeDeclarationsFromPCH, int displayDiagnostics);

        [DllImport(libraryPath, EntryPoint = "clang_disposeIndex", CallingConvention = CallingConvention.Cdecl)]
        public static extern void disposeIndex(CXIndex index);

        [DllImport(libraryPath, EntryPoint = "clang_CXIndex_setGlobalOptions", CallingConvention = CallingConvention.Cdecl)]
        public static extern void CXIndex_setGlobalOptions(CXIndex param0, uint options);

        [DllImport(libraryPath, EntryPoint = "clang_CXIndex_getGlobalOptions", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint CXIndex_getGlobalOptions(CXIndex param0);

        [DllImport(libraryPath, EntryPoint = "clang_CXIndex_setInvocationEmissionPathOption", CallingConvention = CallingConvention.Cdecl)]
        public static extern void CXIndex_setInvocationEmissionPathOption(CXIndex param0, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))] string Path);

        [DllImport(libraryPath, EntryPoint = "clang_getFileName", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString getFileName(CXFile SFile);

        [DllImport(libraryPath, EntryPoint = "clang_getFileTime", CallingConvention = CallingConvention.Cdecl)]
        public static extern long getFileTime(CXFile SFile);

        [DllImport(libraryPath, EntryPoint = "clang_getFileUniqueID", CallingConvention = CallingConvention.Cdecl)]
        public static extern int getFileUniqueID(CXFile file, out CXFileUniqueID outID);

        [DllImport(libraryPath, EntryPoint = "clang_isFileMultipleIncludeGuarded", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint isFileMultipleIncludeGuarded(CXTranslationUnit tu, CXFile file);

        [DllImport(libraryPath, EntryPoint = "clang_getFile", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXFile getFile(CXTranslationUnit tu, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))] string file_name);

        [DllImport(libraryPath, EntryPoint = "clang_getFileContents", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))]
        public static extern string getFileContents(CXTranslationUnit tu, CXFile file, out IntPtr size);

        [DllImport(libraryPath, EntryPoint = "clang_File_isEqual", CallingConvention = CallingConvention.Cdecl)]
        public static extern int File_isEqual(CXFile file1, CXFile file2);

        [DllImport(libraryPath, EntryPoint = "clang_File_tryGetRealPathName", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString File_tryGetRealPathName(CXFile file);

        [DllImport(libraryPath, EntryPoint = "clang_getNullLocation", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXSourceLocation getNullLocation();

        [DllImport(libraryPath, EntryPoint = "clang_equalLocations", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint equalLocations(CXSourceLocation loc1, CXSourceLocation loc2);

        [DllImport(libraryPath, EntryPoint = "clang_getLocation", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXSourceLocation getLocation(CXTranslationUnit tu, CXFile file, uint line, uint column);

        [DllImport(libraryPath, EntryPoint = "clang_getLocationForOffset", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXSourceLocation getLocationForOffset(CXTranslationUnit tu, CXFile file, uint offset);

        [DllImport(libraryPath, EntryPoint = "clang_Location_isInSystemHeader", CallingConvention = CallingConvention.Cdecl)]
        public static extern int Location_isInSystemHeader(CXSourceLocation location);

        [DllImport(libraryPath, EntryPoint = "clang_Location_isFromMainFile", CallingConvention = CallingConvention.Cdecl)]
        public static extern int Location_isFromMainFile(CXSourceLocation location);

        [DllImport(libraryPath, EntryPoint = "clang_getNullRange", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXSourceRange getNullRange();

        [DllImport(libraryPath, EntryPoint = "clang_getRange", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXSourceRange getRange(CXSourceLocation begin, CXSourceLocation end);

        [DllImport(libraryPath, EntryPoint = "clang_equalRanges", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint equalRanges(CXSourceRange range1, CXSourceRange range2);

        [DllImport(libraryPath, EntryPoint = "clang_Range_isNull", CallingConvention = CallingConvention.Cdecl)]
        public static extern int Range_isNull(CXSourceRange range);

        [DllImport(libraryPath, EntryPoint = "clang_getExpansionLocation", CallingConvention = CallingConvention.Cdecl)]
        public static extern void getExpansionLocation(CXSourceLocation location, out CXFile file, out uint line, out uint column, out uint offset);

        [DllImport(libraryPath, EntryPoint = "clang_getPresumedLocation", CallingConvention = CallingConvention.Cdecl)]
        public static extern void getPresumedLocation(CXSourceLocation location, out CXString filename, out uint line, out uint column);

        [DllImport(libraryPath, EntryPoint = "clang_getInstantiationLocation", CallingConvention = CallingConvention.Cdecl)]
        public static extern void getInstantiationLocation(CXSourceLocation location, out CXFile file, out uint line, out uint column, out uint offset);

        [DllImport(libraryPath, EntryPoint = "clang_getSpellingLocation", CallingConvention = CallingConvention.Cdecl)]
        public static extern void getSpellingLocation(CXSourceLocation location, out CXFile file, out uint line, out uint column, out uint offset);

        [DllImport(libraryPath, EntryPoint = "clang_getFileLocation", CallingConvention = CallingConvention.Cdecl)]
        public static extern void getFileLocation(CXSourceLocation location, out CXFile file, out uint line, out uint column, out uint offset);

        [DllImport(libraryPath, EntryPoint = "clang_getRangeStart", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXSourceLocation getRangeStart(CXSourceRange range);

        [DllImport(libraryPath, EntryPoint = "clang_getRangeEnd", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXSourceLocation getRangeEnd(CXSourceRange range);

        [DllImport(libraryPath, EntryPoint = "clang_getSkippedRanges", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr getSkippedRanges(CXTranslationUnit tu, CXFile file);

        [DllImport(libraryPath, EntryPoint = "clang_getAllSkippedRanges", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr getAllSkippedRanges(CXTranslationUnit tu);

        [DllImport(libraryPath, EntryPoint = "clang_getNumDiagnosticsInSet", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint getNumDiagnosticsInSet(CXDiagnosticSet Diags);

        [DllImport(libraryPath, EntryPoint = "clang_getDiagnosticInSet", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXDiagnostic getDiagnosticInSet(CXDiagnosticSet Diags, uint Index);

        [DllImport(libraryPath, EntryPoint = "clang_loadDiagnostics", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXDiagnosticSet loadDiagnostics([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))] string file, out CXLoadDiag_Error error, out CXString errorString);

        [DllImport(libraryPath, EntryPoint = "clang_disposeDiagnosticSet", CallingConvention = CallingConvention.Cdecl)]
        public static extern void disposeDiagnosticSet(CXDiagnosticSet Diags);

        [DllImport(libraryPath, EntryPoint = "clang_getChildDiagnostics", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXDiagnosticSet getChildDiagnostics(CXDiagnostic D);

        [DllImport(libraryPath, EntryPoint = "clang_getNumDiagnostics", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint getNumDiagnostics(CXTranslationUnit Unit);

        [DllImport(libraryPath, EntryPoint = "clang_getDiagnostic", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXDiagnostic getDiagnostic(CXTranslationUnit Unit, uint Index);

        [DllImport(libraryPath, EntryPoint = "clang_getDiagnosticSetFromTU", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXDiagnosticSet getDiagnosticSetFromTU(CXTranslationUnit Unit);

        [DllImport(libraryPath, EntryPoint = "clang_disposeDiagnostic", CallingConvention = CallingConvention.Cdecl)]
        public static extern void disposeDiagnostic(CXDiagnostic Diagnostic);

        [DllImport(libraryPath, EntryPoint = "clang_formatDiagnostic", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString formatDiagnostic(CXDiagnostic Diagnostic, uint Options);

        [DllImport(libraryPath, EntryPoint = "clang_defaultDiagnosticDisplayOptions", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint defaultDiagnosticDisplayOptions();

        [DllImport(libraryPath, EntryPoint = "clang_getDiagnosticSeverity", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXDiagnosticSeverity getDiagnosticSeverity(CXDiagnostic param0);

        [DllImport(libraryPath, EntryPoint = "clang_getDiagnosticLocation", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXSourceLocation getDiagnosticLocation(CXDiagnostic param0);

        [DllImport(libraryPath, EntryPoint = "clang_getDiagnosticSpelling", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString getDiagnosticSpelling(CXDiagnostic param0);

        [DllImport(libraryPath, EntryPoint = "clang_getDiagnosticOption", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString getDiagnosticOption(CXDiagnostic Diag, out CXString Disable);

        [DllImport(libraryPath, EntryPoint = "clang_getDiagnosticCategory", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint getDiagnosticCategory(CXDiagnostic param0);

        [DllImport(libraryPath, EntryPoint = "clang_getDiagnosticCategoryName", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString getDiagnosticCategoryName(uint Category);

        [DllImport(libraryPath, EntryPoint = "clang_getDiagnosticCategoryText", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString getDiagnosticCategoryText(CXDiagnostic param0);

        [DllImport(libraryPath, EntryPoint = "clang_getDiagnosticNumRanges", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint getDiagnosticNumRanges(CXDiagnostic param0);

        [DllImport(libraryPath, EntryPoint = "clang_getDiagnosticRange", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXSourceRange getDiagnosticRange(CXDiagnostic Diagnostic, uint Range);

        [DllImport(libraryPath, EntryPoint = "clang_getDiagnosticNumFixIts", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint getDiagnosticNumFixIts(CXDiagnostic Diagnostic);

        [DllImport(libraryPath, EntryPoint = "clang_getDiagnosticFixIt", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString getDiagnosticFixIt(CXDiagnostic Diagnostic, uint FixIt, out CXSourceRange ReplacementRange);

        [DllImport(libraryPath, EntryPoint = "clang_getTranslationUnitSpelling", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString getTranslationUnitSpelling(CXTranslationUnit CTUnit);

        [DllImport(libraryPath, EntryPoint = "clang_createTranslationUnit", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXTranslationUnit createTranslationUnit(CXIndex CIdx, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))] string ast_filename);

        [DllImport(libraryPath, EntryPoint = "clang_createTranslationUnit2", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXErrorCode createTranslationUnit2(CXIndex CIdx, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))] string ast_filename, out CXTranslationUnit out_TU);

        [DllImport(libraryPath, EntryPoint = "clang_defaultEditingTranslationUnitOptions", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint defaultEditingTranslationUnitOptions();

        [DllImport(libraryPath, EntryPoint = "clang_defaultSaveOptions", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint defaultSaveOptions(CXTranslationUnit TU);

        [DllImport(libraryPath, EntryPoint = "clang_saveTranslationUnit", CallingConvention = CallingConvention.Cdecl)]
        public static extern int saveTranslationUnit(CXTranslationUnit TU, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))] string FileName, uint options);

        [DllImport(libraryPath, EntryPoint = "clang_suspendTranslationUnit", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint suspendTranslationUnit(CXTranslationUnit param0);

        [DllImport(libraryPath, EntryPoint = "clang_disposeTranslationUnit", CallingConvention = CallingConvention.Cdecl)]
        public static extern void disposeTranslationUnit(CXTranslationUnit param0);

        [DllImport(libraryPath, EntryPoint = "clang_defaultReparseOptions", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint defaultReparseOptions(CXTranslationUnit TU);

        [DllImport(libraryPath, EntryPoint = "clang_getTUResourceUsageName", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))]
        public static extern string getTUResourceUsageName(CXTUResourceUsageKind kind);

        [DllImport(libraryPath, EntryPoint = "clang_getCXTUResourceUsage", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXTUResourceUsage getCXTUResourceUsage(CXTranslationUnit TU);

        [DllImport(libraryPath, EntryPoint = "clang_disposeCXTUResourceUsage", CallingConvention = CallingConvention.Cdecl)]
        public static extern void disposeCXTUResourceUsage(CXTUResourceUsage usage);

        [DllImport(libraryPath, EntryPoint = "clang_getTranslationUnitTargetInfo", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXTargetInfo getTranslationUnitTargetInfo(CXTranslationUnit CTUnit);

        [DllImport(libraryPath, EntryPoint = "clang_TargetInfo_dispose", CallingConvention = CallingConvention.Cdecl)]
        public static extern void TargetInfo_dispose(CXTargetInfo Info);

        [DllImport(libraryPath, EntryPoint = "clang_TargetInfo_getTriple", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString TargetInfo_getTriple(CXTargetInfo Info);

        [DllImport(libraryPath, EntryPoint = "clang_TargetInfo_getPointerWidth", CallingConvention = CallingConvention.Cdecl)]
        public static extern int TargetInfo_getPointerWidth(CXTargetInfo Info);

        [DllImport(libraryPath, EntryPoint = "clang_getNullCursor", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXCursor getNullCursor();

        [DllImport(libraryPath, EntryPoint = "clang_getTranslationUnitCursor", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXCursor getTranslationUnitCursor(CXTranslationUnit param0);

        [DllImport(libraryPath, EntryPoint = "clang_equalCursors", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint equalCursors(CXCursor param0, CXCursor param1);

        [DllImport(libraryPath, EntryPoint = "clang_Cursor_isNull", CallingConvention = CallingConvention.Cdecl)]
        public static extern int Cursor_isNull(CXCursor cursor);

        [DllImport(libraryPath, EntryPoint = "clang_hashCursor", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint hashCursor(CXCursor param0);

        [DllImport(libraryPath, EntryPoint = "clang_getCursorKind", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXCursorKind getCursorKind(CXCursor param0);

        [DllImport(libraryPath, EntryPoint = "clang_isDeclaration", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint isDeclaration(CXCursorKind param0);

        [DllImport(libraryPath, EntryPoint = "clang_isInvalidDeclaration", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint isInvalidDeclaration(CXCursor param0);

        [DllImport(libraryPath, EntryPoint = "clang_isReference", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint isReference(CXCursorKind param0);

        [DllImport(libraryPath, EntryPoint = "clang_isExpression", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint isExpression(CXCursorKind param0);

        [DllImport(libraryPath, EntryPoint = "clang_isStatement", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint isStatement(CXCursorKind param0);

        [DllImport(libraryPath, EntryPoint = "clang_isAttribute", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint isAttribute(CXCursorKind param0);

        [DllImport(libraryPath, EntryPoint = "clang_Cursor_hasAttrs", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint Cursor_hasAttrs(CXCursor C);

        [DllImport(libraryPath, EntryPoint = "clang_isInvalid", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint isInvalid(CXCursorKind param0);

        [DllImport(libraryPath, EntryPoint = "clang_isTranslationUnit", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint isTranslationUnit(CXCursorKind param0);

        [DllImport(libraryPath, EntryPoint = "clang_isPreprocessing", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint isPreprocessing(CXCursorKind param0);

        [DllImport(libraryPath, EntryPoint = "clang_isUnexposed", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint isUnexposed(CXCursorKind param0);

        [DllImport(libraryPath, EntryPoint = "clang_getCursorLinkage", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXLinkageKind getCursorLinkage(CXCursor cursor);

        [DllImport(libraryPath, EntryPoint = "clang_getCursorVisibility", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXVisibilityKind getCursorVisibility(CXCursor cursor);

        [DllImport(libraryPath, EntryPoint = "clang_getCursorAvailability", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXAvailabilityKind getCursorAvailability(CXCursor cursor);

        [DllImport(libraryPath, EntryPoint = "clang_getCursorLanguage", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXLanguageKind getCursorLanguage(CXCursor cursor);

        [DllImport(libraryPath, EntryPoint = "clang_getCursorTLSKind", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXTLSKind getCursorTLSKind(CXCursor cursor);

        [DllImport(libraryPath, EntryPoint = "clang_Cursor_getTranslationUnit", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXTranslationUnit Cursor_getTranslationUnit(CXCursor param0);

        [DllImport(libraryPath, EntryPoint = "clang_createCXCursorSet", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXCursorSet createCXCursorSet();

        [DllImport(libraryPath, EntryPoint = "clang_disposeCXCursorSet", CallingConvention = CallingConvention.Cdecl)]
        public static extern void disposeCXCursorSet(CXCursorSet cset);

        [DllImport(libraryPath, EntryPoint = "clang_CXCursorSet_contains", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint CXCursorSet_contains(CXCursorSet cset, CXCursor cursor);

        [DllImport(libraryPath, EntryPoint = "clang_CXCursorSet_insert", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint CXCursorSet_insert(CXCursorSet cset, CXCursor cursor);

        [DllImport(libraryPath, EntryPoint = "clang_getCursorSemanticParent", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXCursor getCursorSemanticParent(CXCursor cursor);

        [DllImport(libraryPath, EntryPoint = "clang_getCursorLexicalParent", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXCursor getCursorLexicalParent(CXCursor cursor);

        [DllImport(libraryPath, EntryPoint = "clang_getOverriddenCursors", CallingConvention = CallingConvention.Cdecl)]
        public static extern void getOverriddenCursors(CXCursor cursor, out CXCursor overridden, out uint num_overridden);

        [DllImport(libraryPath, EntryPoint = "clang_disposeOverriddenCursors", CallingConvention = CallingConvention.Cdecl)]
        public static extern void disposeOverriddenCursors(out CXCursor overridden);

        [DllImport(libraryPath, EntryPoint = "clang_getIncludedFile", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXFile getIncludedFile(CXCursor cursor);

        [DllImport(libraryPath, EntryPoint = "clang_getCursor", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXCursor getCursor(CXTranslationUnit param0, CXSourceLocation param1);

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
        public static extern long getEnumConstantDeclValue(CXCursor C);

        [DllImport(libraryPath, EntryPoint = "clang_getEnumConstantDeclUnsignedValue", CallingConvention = CallingConvention.Cdecl)]
        public static extern ulong getEnumConstantDeclUnsignedValue(CXCursor C);

        [DllImport(libraryPath, EntryPoint = "clang_getFieldDeclBitWidth", CallingConvention = CallingConvention.Cdecl)]
        public static extern int getFieldDeclBitWidth(CXCursor C);

        [DllImport(libraryPath, EntryPoint = "clang_Cursor_getNumArguments", CallingConvention = CallingConvention.Cdecl)]
        public static extern int Cursor_getNumArguments(CXCursor C);

        [DllImport(libraryPath, EntryPoint = "clang_Cursor_getArgument", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXCursor Cursor_getArgument(CXCursor C, uint i);

        [DllImport(libraryPath, EntryPoint = "clang_Cursor_getNumTemplateArguments", CallingConvention = CallingConvention.Cdecl)]
        public static extern int Cursor_getNumTemplateArguments(CXCursor C);

        [DllImport(libraryPath, EntryPoint = "clang_Cursor_getTemplateArgumentKind", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXTemplateArgumentKind Cursor_getTemplateArgumentKind(CXCursor C, uint I);

        [DllImport(libraryPath, EntryPoint = "clang_Cursor_getTemplateArgumentType", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXType Cursor_getTemplateArgumentType(CXCursor C, uint I);

        [DllImport(libraryPath, EntryPoint = "clang_Cursor_getTemplateArgumentValue", CallingConvention = CallingConvention.Cdecl)]
        public static extern long Cursor_getTemplateArgumentValue(CXCursor C, uint I);

        [DllImport(libraryPath, EntryPoint = "clang_Cursor_getTemplateArgumentUnsignedValue", CallingConvention = CallingConvention.Cdecl)]
        public static extern ulong Cursor_getTemplateArgumentUnsignedValue(CXCursor C, uint I);

        [DllImport(libraryPath, EntryPoint = "clang_equalTypes", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint equalTypes(CXType A, CXType B);

        [DllImport(libraryPath, EntryPoint = "clang_getCanonicalType", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXType getCanonicalType(CXType T);

        [DllImport(libraryPath, EntryPoint = "clang_isConstQualifiedType", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint isConstQualifiedType(CXType T);

        [DllImport(libraryPath, EntryPoint = "clang_Cursor_isMacroFunctionLike", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint Cursor_isMacroFunctionLike(CXCursor C);

        [DllImport(libraryPath, EntryPoint = "clang_Cursor_isMacroBuiltin", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint Cursor_isMacroBuiltin(CXCursor C);

        [DllImport(libraryPath, EntryPoint = "clang_Cursor_isFunctionInlined", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint Cursor_isFunctionInlined(CXCursor C);

        [DllImport(libraryPath, EntryPoint = "clang_isVolatileQualifiedType", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint isVolatileQualifiedType(CXType T);

        [DllImport(libraryPath, EntryPoint = "clang_isRestrictQualifiedType", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint isRestrictQualifiedType(CXType T);

        [DllImport(libraryPath, EntryPoint = "clang_getAddressSpace", CallingConvention = CallingConvention.Cdecl)]
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
        public static extern CXString getTypeKindSpelling(CXTypeKind K);

        [DllImport(libraryPath, EntryPoint = "clang_getFunctionTypeCallingConv", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXCallingConv getFunctionTypeCallingConv(CXType T);

        [DllImport(libraryPath, EntryPoint = "clang_getResultType", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXType getResultType(CXType T);

        [DllImport(libraryPath, EntryPoint = "clang_getExceptionSpecificationType", CallingConvention = CallingConvention.Cdecl)]
        public static extern int getExceptionSpecificationType(CXType T);

        [DllImport(libraryPath, EntryPoint = "clang_getNumArgTypes", CallingConvention = CallingConvention.Cdecl)]
        public static extern int getNumArgTypes(CXType T);

        [DllImport(libraryPath, EntryPoint = "clang_getArgType", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXType getArgType(CXType T, uint i);

        [DllImport(libraryPath, EntryPoint = "clang_Type_getObjCObjectBaseType", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXType Type_getObjCObjectBaseType(CXType T);

        [DllImport(libraryPath, EntryPoint = "clang_Type_getNumObjCProtocolRefs", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint Type_getNumObjCProtocolRefs(CXType T);

        [DllImport(libraryPath, EntryPoint = "clang_Type_getObjCProtocolDecl", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXCursor Type_getObjCProtocolDecl(CXType T, uint i);

        [DllImport(libraryPath, EntryPoint = "clang_Type_getNumObjCTypeArgs", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint Type_getNumObjCTypeArgs(CXType T);

        [DllImport(libraryPath, EntryPoint = "clang_Type_getObjCTypeArg", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXType Type_getObjCTypeArg(CXType T, uint i);

        [DllImport(libraryPath, EntryPoint = "clang_isFunctionTypeVariadic", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint isFunctionTypeVariadic(CXType T);

        [DllImport(libraryPath, EntryPoint = "clang_getCursorResultType", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXType getCursorResultType(CXCursor C);

        [DllImport(libraryPath, EntryPoint = "clang_getCursorExceptionSpecificationType", CallingConvention = CallingConvention.Cdecl)]
        public static extern int getCursorExceptionSpecificationType(CXCursor C);

        [DllImport(libraryPath, EntryPoint = "clang_isPODType", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint isPODType(CXType T);

        [DllImport(libraryPath, EntryPoint = "clang_getElementType", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXType getElementType(CXType T);

        [DllImport(libraryPath, EntryPoint = "clang_getNumElements", CallingConvention = CallingConvention.Cdecl)]
        public static extern long getNumElements(CXType T);

        [DllImport(libraryPath, EntryPoint = "clang_getArrayElementType", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXType getArrayElementType(CXType T);

        [DllImport(libraryPath, EntryPoint = "clang_getArraySize", CallingConvention = CallingConvention.Cdecl)]
        public static extern long getArraySize(CXType T);

        [DllImport(libraryPath, EntryPoint = "clang_Type_getNamedType", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXType Type_getNamedType(CXType T);

        [DllImport(libraryPath, EntryPoint = "clang_Type_isTransparentTagTypedef", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint Type_isTransparentTagTypedef(CXType T);

        [DllImport(libraryPath, EntryPoint = "clang_Type_getNullability", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXTypeNullabilityKind Type_getNullability(CXType T);

        [DllImport(libraryPath, EntryPoint = "clang_Type_getAlignOf", CallingConvention = CallingConvention.Cdecl)]
        public static extern long Type_getAlignOf(CXType T);

        [DllImport(libraryPath, EntryPoint = "clang_Type_getClassType", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXType Type_getClassType(CXType T);

        [DllImport(libraryPath, EntryPoint = "clang_Type_getSizeOf", CallingConvention = CallingConvention.Cdecl)]
        public static extern long Type_getSizeOf(CXType T);

        [DllImport(libraryPath, EntryPoint = "clang_Type_getOffsetOf", CallingConvention = CallingConvention.Cdecl)]
        public static extern long Type_getOffsetOf(CXType T, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))] string S);

        [DllImport(libraryPath, EntryPoint = "clang_Type_getModifiedType", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXType Type_getModifiedType(CXType T);

        [DllImport(libraryPath, EntryPoint = "clang_Cursor_getOffsetOfField", CallingConvention = CallingConvention.Cdecl)]
        public static extern long Cursor_getOffsetOfField(CXCursor C);

        [DllImport(libraryPath, EntryPoint = "clang_Cursor_isAnonymous", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint Cursor_isAnonymous(CXCursor C);

        [DllImport(libraryPath, EntryPoint = "clang_Type_getNumTemplateArguments", CallingConvention = CallingConvention.Cdecl)]
        public static extern int Type_getNumTemplateArguments(CXType T);

        [DllImport(libraryPath, EntryPoint = "clang_Type_getTemplateArgumentAsType", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXType Type_getTemplateArgumentAsType(CXType T, uint i);

        [DllImport(libraryPath, EntryPoint = "clang_Type_getCXXRefQualifier", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXRefQualifierKind Type_getCXXRefQualifier(CXType T);

        [DllImport(libraryPath, EntryPoint = "clang_Cursor_isBitField", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint Cursor_isBitField(CXCursor C);

        [DllImport(libraryPath, EntryPoint = "clang_isVirtualBase", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint isVirtualBase(CXCursor param0);

        [DllImport(libraryPath, EntryPoint = "clang_getCXXAccessSpecifier", CallingConvention = CallingConvention.Cdecl)]
        public static extern CX_CXXAccessSpecifier getCXXAccessSpecifier(CXCursor param0);

        [DllImport(libraryPath, EntryPoint = "clang_Cursor_getStorageClass", CallingConvention = CallingConvention.Cdecl)]
        public static extern CX_StorageClass Cursor_getStorageClass(CXCursor param0);

        [DllImport(libraryPath, EntryPoint = "clang_getNumOverloadedDecls", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint getNumOverloadedDecls(CXCursor cursor);

        [DllImport(libraryPath, EntryPoint = "clang_getOverloadedDecl", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXCursor getOverloadedDecl(CXCursor cursor, uint index);

        [DllImport(libraryPath, EntryPoint = "clang_getIBOutletCollectionType", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXType getIBOutletCollectionType(CXCursor param0);

        [DllImport(libraryPath, EntryPoint = "clang_visitChildren", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint visitChildren(CXCursor parent, CXCursorVisitor visitor, CXClientData client_data);

        [DllImport(libraryPath, EntryPoint = "clang_getCursorUSR", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString getCursorUSR(CXCursor param0);

        [DllImport(libraryPath, EntryPoint = "clang_constructUSR_ObjCClass", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString constructUSR_ObjCClass([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))] string class_name);

        [DllImport(libraryPath, EntryPoint = "clang_constructUSR_ObjCCategory", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString constructUSR_ObjCCategory([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))] string class_name, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))] string category_name);

        [DllImport(libraryPath, EntryPoint = "clang_constructUSR_ObjCProtocol", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString constructUSR_ObjCProtocol([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))] string protocol_name);

        [DllImport(libraryPath, EntryPoint = "clang_constructUSR_ObjCIvar", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString constructUSR_ObjCIvar([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))] string name, CXString classUSR);

        [DllImport(libraryPath, EntryPoint = "clang_constructUSR_ObjCMethod", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString constructUSR_ObjCMethod([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))] string name, uint isInstanceMethod, CXString classUSR);

        [DllImport(libraryPath, EntryPoint = "clang_constructUSR_ObjCProperty", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString constructUSR_ObjCProperty([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))] string property, CXString classUSR);

        [DllImport(libraryPath, EntryPoint = "clang_getCursorSpelling", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString getCursorSpelling(CXCursor param0);

        [DllImport(libraryPath, EntryPoint = "clang_Cursor_getSpellingNameRange", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXSourceRange Cursor_getSpellingNameRange(CXCursor param0, uint pieceIndex, uint options);

        [DllImport(libraryPath, EntryPoint = "clang_PrintingPolicy_getProperty", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint PrintingPolicy_getProperty(CXPrintingPolicy Policy, CXPrintingPolicyProperty Property);

        [DllImport(libraryPath, EntryPoint = "clang_PrintingPolicy_setProperty", CallingConvention = CallingConvention.Cdecl)]
        public static extern void PrintingPolicy_setProperty(CXPrintingPolicy Policy, CXPrintingPolicyProperty Property, uint Value);

        [DllImport(libraryPath, EntryPoint = "clang_getCursorPrintingPolicy", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXPrintingPolicy getCursorPrintingPolicy(CXCursor param0);

        [DllImport(libraryPath, EntryPoint = "clang_PrintingPolicy_dispose", CallingConvention = CallingConvention.Cdecl)]
        public static extern void PrintingPolicy_dispose(CXPrintingPolicy Policy);

        [DllImport(libraryPath, EntryPoint = "clang_getCursorPrettyPrinted", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString getCursorPrettyPrinted(CXCursor Cursor, CXPrintingPolicy Policy);

        [DllImport(libraryPath, EntryPoint = "clang_getCursorDisplayName", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString getCursorDisplayName(CXCursor param0);

        [DllImport(libraryPath, EntryPoint = "clang_getCursorReferenced", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXCursor getCursorReferenced(CXCursor param0);

        [DllImport(libraryPath, EntryPoint = "clang_getCursorDefinition", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXCursor getCursorDefinition(CXCursor param0);

        [DllImport(libraryPath, EntryPoint = "clang_isCursorDefinition", CallingConvention = CallingConvention.Cdecl)]
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
        public static extern uint Cursor_getObjCPropertyAttributes(CXCursor C, uint reserved);

        [DllImport(libraryPath, EntryPoint = "clang_Cursor_getObjCPropertyGetterName", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString Cursor_getObjCPropertyGetterName(CXCursor C);

        [DllImport(libraryPath, EntryPoint = "clang_Cursor_getObjCPropertySetterName", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString Cursor_getObjCPropertySetterName(CXCursor C);

        [DllImport(libraryPath, EntryPoint = "clang_Cursor_getObjCDeclQualifiers", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint Cursor_getObjCDeclQualifiers(CXCursor C);

        [DllImport(libraryPath, EntryPoint = "clang_Cursor_isObjCOptional", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint Cursor_isObjCOptional(CXCursor C);

        [DllImport(libraryPath, EntryPoint = "clang_Cursor_isVariadic", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint Cursor_isVariadic(CXCursor C);

        [DllImport(libraryPath, EntryPoint = "clang_Cursor_isExternalSymbol", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint Cursor_isExternalSymbol(CXCursor C, out CXString language, out CXString definedIn, out uint isGenerated);

        [DllImport(libraryPath, EntryPoint = "clang_Cursor_getCommentRange", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXSourceRange Cursor_getCommentRange(CXCursor C);

        [DllImport(libraryPath, EntryPoint = "clang_Cursor_getRawCommentText", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString Cursor_getRawCommentText(CXCursor C);

        [DllImport(libraryPath, EntryPoint = "clang_Cursor_getBriefCommentText", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString Cursor_getBriefCommentText(CXCursor C);

        [DllImport(libraryPath, EntryPoint = "clang_Cursor_getMangling", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString Cursor_getMangling(CXCursor param0);

        [DllImport(libraryPath, EntryPoint = "clang_Cursor_getCXXManglings", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr Cursor_getCXXManglings(CXCursor param0);

        [DllImport(libraryPath, EntryPoint = "clang_Cursor_getObjCManglings", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr Cursor_getObjCManglings(CXCursor param0);

        [DllImport(libraryPath, EntryPoint = "clang_Cursor_getModule", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXModule Cursor_getModule(CXCursor C);

        [DllImport(libraryPath, EntryPoint = "clang_getModuleForFile", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXModule getModuleForFile(CXTranslationUnit param0, CXFile param1);

        [DllImport(libraryPath, EntryPoint = "clang_Module_getASTFile", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXFile Module_getASTFile(CXModule Module);

        [DllImport(libraryPath, EntryPoint = "clang_Module_getParent", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXModule Module_getParent(CXModule Module);

        [DllImport(libraryPath, EntryPoint = "clang_Module_getName", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString Module_getName(CXModule Module);

        [DllImport(libraryPath, EntryPoint = "clang_Module_getFullName", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString Module_getFullName(CXModule Module);

        [DllImport(libraryPath, EntryPoint = "clang_Module_isSystem", CallingConvention = CallingConvention.Cdecl)]
        public static extern int Module_isSystem(CXModule Module);

        [DllImport(libraryPath, EntryPoint = "clang_Module_getNumTopLevelHeaders", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint Module_getNumTopLevelHeaders(CXTranslationUnit param0, CXModule Module);

        [DllImport(libraryPath, EntryPoint = "clang_Module_getTopLevelHeader", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXFile Module_getTopLevelHeader(CXTranslationUnit param0, CXModule Module, uint Index);

        [DllImport(libraryPath, EntryPoint = "clang_CXXConstructor_isConvertingConstructor", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint CXXConstructor_isConvertingConstructor(CXCursor C);

        [DllImport(libraryPath, EntryPoint = "clang_CXXConstructor_isCopyConstructor", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint CXXConstructor_isCopyConstructor(CXCursor C);

        [DllImport(libraryPath, EntryPoint = "clang_CXXConstructor_isDefaultConstructor", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint CXXConstructor_isDefaultConstructor(CXCursor C);

        [DllImport(libraryPath, EntryPoint = "clang_CXXConstructor_isMoveConstructor", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint CXXConstructor_isMoveConstructor(CXCursor C);

        [DllImport(libraryPath, EntryPoint = "clang_CXXField_isMutable", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint CXXField_isMutable(CXCursor C);

        [DllImport(libraryPath, EntryPoint = "clang_CXXMethod_isDefaulted", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint CXXMethod_isDefaulted(CXCursor C);

        [DllImport(libraryPath, EntryPoint = "clang_CXXMethod_isPureVirtual", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint CXXMethod_isPureVirtual(CXCursor C);

        [DllImport(libraryPath, EntryPoint = "clang_CXXMethod_isStatic", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint CXXMethod_isStatic(CXCursor C);

        [DllImport(libraryPath, EntryPoint = "clang_CXXMethod_isVirtual", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint CXXMethod_isVirtual(CXCursor C);

        [DllImport(libraryPath, EntryPoint = "clang_CXXRecord_isAbstract", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint CXXRecord_isAbstract(CXCursor C);

        [DllImport(libraryPath, EntryPoint = "clang_EnumDecl_isScoped", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint EnumDecl_isScoped(CXCursor C);

        [DllImport(libraryPath, EntryPoint = "clang_CXXMethod_isConst", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint CXXMethod_isConst(CXCursor C);

        [DllImport(libraryPath, EntryPoint = "clang_getTemplateCursorKind", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXCursorKind getTemplateCursorKind(CXCursor C);

        [DllImport(libraryPath, EntryPoint = "clang_getSpecializedCursorTemplate", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXCursor getSpecializedCursorTemplate(CXCursor C);

        [DllImport(libraryPath, EntryPoint = "clang_getCursorReferenceNameRange", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXSourceRange getCursorReferenceNameRange(CXCursor C, uint NameFlags, uint PieceIndex);

        [DllImport(libraryPath, EntryPoint = "clang_getToken", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr getToken(CXTranslationUnit TU, CXSourceLocation Location);

        [DllImport(libraryPath, EntryPoint = "clang_getTokenKind", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXTokenKind getTokenKind(CXToken param0);

        [DllImport(libraryPath, EntryPoint = "clang_getTokenSpelling", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString getTokenSpelling(CXTranslationUnit param0, CXToken param1);

        [DllImport(libraryPath, EntryPoint = "clang_getTokenLocation", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXSourceLocation getTokenLocation(CXTranslationUnit param0, CXToken param1);

        [DllImport(libraryPath, EntryPoint = "clang_getTokenExtent", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXSourceRange getTokenExtent(CXTranslationUnit param0, CXToken param1);

        [DllImport(libraryPath, EntryPoint = "clang_getCursorKindSpelling", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString getCursorKindSpelling(CXCursorKind Kind);

        [DllImport(libraryPath, EntryPoint = "clang_getDefinitionSpellingAndExtent", CallingConvention = CallingConvention.Cdecl)]
        public static extern void getDefinitionSpellingAndExtent(CXCursor param0, out IntPtr startBuf, out IntPtr endBuf, out uint startLine, out uint startColumn, out uint endLine, out uint endColumn);

        [DllImport(libraryPath, EntryPoint = "clang_enableStackTraces", CallingConvention = CallingConvention.Cdecl)]
        public static extern void enableStackTraces();

        [DllImport(libraryPath, EntryPoint = "clang_executeOnThread", CallingConvention = CallingConvention.Cdecl)]
        public static extern void executeOnThread(IntPtr fn, IntPtr user_data, uint stack_size);

        [DllImport(libraryPath, EntryPoint = "clang_getCompletionChunkKind", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXCompletionChunkKind getCompletionChunkKind(CXCompletionString completion_string, uint chunk_number);

        [DllImport(libraryPath, EntryPoint = "clang_getCompletionChunkText", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString getCompletionChunkText(CXCompletionString completion_string, uint chunk_number);

        [DllImport(libraryPath, EntryPoint = "clang_getCompletionChunkCompletionString", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXCompletionString getCompletionChunkCompletionString(CXCompletionString completion_string, uint chunk_number);

        [DllImport(libraryPath, EntryPoint = "clang_getNumCompletionChunks", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint getNumCompletionChunks(CXCompletionString completion_string);

        [DllImport(libraryPath, EntryPoint = "clang_getCompletionPriority", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint getCompletionPriority(CXCompletionString completion_string);

        [DllImport(libraryPath, EntryPoint = "clang_getCompletionAvailability", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXAvailabilityKind getCompletionAvailability(CXCompletionString completion_string);

        [DllImport(libraryPath, EntryPoint = "clang_getCompletionNumAnnotations", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint getCompletionNumAnnotations(CXCompletionString completion_string);

        [DllImport(libraryPath, EntryPoint = "clang_getCompletionAnnotation", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString getCompletionAnnotation(CXCompletionString completion_string, uint annotation_number);

        [DllImport(libraryPath, EntryPoint = "clang_getCompletionBriefComment", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString getCompletionBriefComment(CXCompletionString completion_string);

        [DllImport(libraryPath, EntryPoint = "clang_getCursorCompletionString", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXCompletionString getCursorCompletionString(CXCursor cursor);

        [DllImport(libraryPath, EntryPoint = "clang_getCompletionNumFixIts", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint getCompletionNumFixIts(out CXCodeCompleteResults results, uint completion_index);

        [DllImport(libraryPath, EntryPoint = "clang_getCompletionFixIt", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString getCompletionFixIt(out CXCodeCompleteResults results, uint completion_index, uint fixit_index, out CXSourceRange replacement_range);

        [DllImport(libraryPath, EntryPoint = "clang_defaultCodeCompleteOptions", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint defaultCodeCompleteOptions();

        [DllImport(libraryPath, EntryPoint = "clang_sortCodeCompletionResults", CallingConvention = CallingConvention.Cdecl)]
        public static extern void sortCodeCompletionResults(out CXCompletionResult Results, uint NumResults);

        [DllImport(libraryPath, EntryPoint = "clang_disposeCodeCompleteResults", CallingConvention = CallingConvention.Cdecl)]
        public static extern void disposeCodeCompleteResults(out CXCodeCompleteResults Results);

        [DllImport(libraryPath, EntryPoint = "clang_codeCompleteGetNumDiagnostics", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint codeCompleteGetNumDiagnostics(out CXCodeCompleteResults Results);

        [DllImport(libraryPath, EntryPoint = "clang_codeCompleteGetDiagnostic", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXDiagnostic codeCompleteGetDiagnostic(out CXCodeCompleteResults Results, uint Index);

        [DllImport(libraryPath, EntryPoint = "clang_codeCompleteGetContexts", CallingConvention = CallingConvention.Cdecl)]
        public static extern ulong codeCompleteGetContexts(out CXCodeCompleteResults Results);

        [DllImport(libraryPath, EntryPoint = "clang_codeCompleteGetContainerKind", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXCursorKind codeCompleteGetContainerKind(out CXCodeCompleteResults Results, out uint IsIncomplete);

        [DllImport(libraryPath, EntryPoint = "clang_codeCompleteGetContainerUSR", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString codeCompleteGetContainerUSR(out CXCodeCompleteResults Results);

        [DllImport(libraryPath, EntryPoint = "clang_codeCompleteGetObjCSelector", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString codeCompleteGetObjCSelector(out CXCodeCompleteResults Results);

        [DllImport(libraryPath, EntryPoint = "clang_getClangVersion", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString getClangVersion();

        [DllImport(libraryPath, EntryPoint = "clang_toggleCrashRecovery", CallingConvention = CallingConvention.Cdecl)]
        public static extern void toggleCrashRecovery(uint isEnabled);

        [DllImport(libraryPath, EntryPoint = "clang_getInclusions", CallingConvention = CallingConvention.Cdecl)]
        public static extern void getInclusions(CXTranslationUnit tu, CXInclusionVisitor visitor, CXClientData client_data);

        [DllImport(libraryPath, EntryPoint = "clang_Cursor_Evaluate", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXEvalResult Cursor_Evaluate(CXCursor C);

        [DllImport(libraryPath, EntryPoint = "clang_EvalResult_getKind", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXEvalResultKind EvalResult_getKind(CXEvalResult E);

        [DllImport(libraryPath, EntryPoint = "clang_EvalResult_getAsInt", CallingConvention = CallingConvention.Cdecl)]
        public static extern int EvalResult_getAsInt(CXEvalResult E);

        [DllImport(libraryPath, EntryPoint = "clang_EvalResult_getAsLongLong", CallingConvention = CallingConvention.Cdecl)]
        public static extern long EvalResult_getAsLongLong(CXEvalResult E);

        [DllImport(libraryPath, EntryPoint = "clang_EvalResult_isUnsignedInt", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint EvalResult_isUnsignedInt(CXEvalResult E);

        [DllImport(libraryPath, EntryPoint = "clang_EvalResult_getAsUnsigned", CallingConvention = CallingConvention.Cdecl)]
        public static extern ulong EvalResult_getAsUnsigned(CXEvalResult E);

        [DllImport(libraryPath, EntryPoint = "clang_EvalResult_getAsDouble", CallingConvention = CallingConvention.Cdecl)]
        public static extern double EvalResult_getAsDouble(CXEvalResult E);

        [DllImport(libraryPath, EntryPoint = "clang_EvalResult_getAsStr", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))]
        public static extern string EvalResult_getAsStr(CXEvalResult E);

        [DllImport(libraryPath, EntryPoint = "clang_EvalResult_dispose", CallingConvention = CallingConvention.Cdecl)]
        public static extern void EvalResult_dispose(CXEvalResult E);

        [DllImport(libraryPath, EntryPoint = "clang_getRemappings", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXRemapping getRemappings([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))] string path);

        [DllImport(libraryPath, EntryPoint = "clang_getRemappingsFromFileList", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXRemapping getRemappingsFromFileList(out IntPtr filePaths, uint numFiles);

        [DllImport(libraryPath, EntryPoint = "clang_remap_getNumFiles", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint remap_getNumFiles(CXRemapping param0);

        [DllImport(libraryPath, EntryPoint = "clang_remap_getFilenames", CallingConvention = CallingConvention.Cdecl)]
        public static extern void remap_getFilenames(CXRemapping param0, uint index, out CXString original, out CXString transformed);

        [DllImport(libraryPath, EntryPoint = "clang_remap_dispose", CallingConvention = CallingConvention.Cdecl)]
        public static extern void remap_dispose(CXRemapping param0);

        [DllImport(libraryPath, EntryPoint = "clang_findReferencesInFile", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXResult findReferencesInFile(CXCursor cursor, CXFile file, CXCursorAndRangeVisitor visitor);

        [DllImport(libraryPath, EntryPoint = "clang_findIncludesInFile", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXResult findIncludesInFile(CXTranslationUnit TU, CXFile file, CXCursorAndRangeVisitor visitor);

        [DllImport(libraryPath, EntryPoint = "clang_index_isEntityObjCContainerKind", CallingConvention = CallingConvention.Cdecl)]
        public static extern int index_isEntityObjCContainerKind(CXIdxEntityKind param0);

        [DllImport(libraryPath, EntryPoint = "clang_index_getObjCContainerDeclInfo", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr index_getObjCContainerDeclInfo(out CXIdxDeclInfo param0);

        [DllImport(libraryPath, EntryPoint = "clang_index_getObjCInterfaceDeclInfo", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr index_getObjCInterfaceDeclInfo(out CXIdxDeclInfo param0);

        [DllImport(libraryPath, EntryPoint = "clang_index_getObjCCategoryDeclInfo", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr index_getObjCCategoryDeclInfo(out CXIdxDeclInfo param0);

        [DllImport(libraryPath, EntryPoint = "clang_index_getObjCProtocolRefListInfo", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr index_getObjCProtocolRefListInfo(out CXIdxDeclInfo param0);

        [DllImport(libraryPath, EntryPoint = "clang_index_getObjCPropertyDeclInfo", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr index_getObjCPropertyDeclInfo(out CXIdxDeclInfo param0);

        [DllImport(libraryPath, EntryPoint = "clang_index_getIBOutletCollectionAttrInfo", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr index_getIBOutletCollectionAttrInfo(out CXIdxAttrInfo param0);

        [DllImport(libraryPath, EntryPoint = "clang_index_getCXXClassDeclInfo", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr index_getCXXClassDeclInfo(out CXIdxDeclInfo param0);

        [DllImport(libraryPath, EntryPoint = "clang_index_getClientContainer", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXIdxClientContainer index_getClientContainer(out CXIdxContainerInfo param0);

        [DllImport(libraryPath, EntryPoint = "clang_index_setClientContainer", CallingConvention = CallingConvention.Cdecl)]
        public static extern void index_setClientContainer(out CXIdxContainerInfo param0, CXIdxClientContainer param1);

        [DllImport(libraryPath, EntryPoint = "clang_IndexAction_create", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXIndexAction IndexAction_create(CXIndex CIdx);

        [DllImport(libraryPath, EntryPoint = "clang_IndexAction_dispose", CallingConvention = CallingConvention.Cdecl)]
        public static extern void IndexAction_dispose(CXIndexAction param0);

        [DllImport(libraryPath, EntryPoint = "clang_indexTranslationUnit", CallingConvention = CallingConvention.Cdecl)]
        public static extern int indexTranslationUnit(CXIndexAction param0, CXClientData client_data, out IndexerCallbacks index_callbacks, uint index_callbacks_size, uint index_options, CXTranslationUnit param5);

        [DllImport(libraryPath, EntryPoint = "clang_indexLoc_getFileLocation", CallingConvention = CallingConvention.Cdecl)]
        public static extern void indexLoc_getFileLocation(CXIdxLoc loc, out CXIdxClientFile indexFile, out CXFile file, out uint line, out uint column, out uint offset);

        [DllImport(libraryPath, EntryPoint = "clang_indexLoc_getCXSourceLocation", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXSourceLocation indexLoc_getCXSourceLocation(CXIdxLoc loc);

        [DllImport(libraryPath, EntryPoint = "clang_Type_visitFields", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint Type_visitFields(CXType T, CXFieldVisitor visitor, CXClientData client_data);

        [DllImport(libraryPath, EntryPoint = "clang_getCString", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))]
        public static extern string getCString(CXString @string);

        [DllImport(libraryPath, EntryPoint = "clang_disposeString", CallingConvention = CallingConvention.Cdecl)]
        public static extern void disposeString(CXString @string);

        [DllImport(libraryPath, EntryPoint = "clang_Cursor_getParsedComment", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXComment Cursor_getParsedComment(CXCursor C);

        [DllImport(libraryPath, EntryPoint = "clang_Comment_getKind", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXCommentKind Comment_getKind(CXComment Comment);

        [DllImport(libraryPath, EntryPoint = "clang_Comment_getNumChildren", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint Comment_getNumChildren(CXComment Comment);

        [DllImport(libraryPath, EntryPoint = "clang_Comment_getChild", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXComment Comment_getChild(CXComment Comment, uint ChildIdx);

        [DllImport(libraryPath, EntryPoint = "clang_Comment_isWhitespace", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint Comment_isWhitespace(CXComment Comment);

        [DllImport(libraryPath, EntryPoint = "clang_InlineContentComment_hasTrailingNewline", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint InlineContentComment_hasTrailingNewline(CXComment Comment);

        [DllImport(libraryPath, EntryPoint = "clang_TextComment_getText", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString TextComment_getText(CXComment Comment);

        [DllImport(libraryPath, EntryPoint = "clang_InlineCommandComment_getCommandName", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString InlineCommandComment_getCommandName(CXComment Comment);

        [DllImport(libraryPath, EntryPoint = "clang_InlineCommandComment_getRenderKind", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXCommentInlineCommandRenderKind InlineCommandComment_getRenderKind(CXComment Comment);

        [DllImport(libraryPath, EntryPoint = "clang_InlineCommandComment_getNumArgs", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint InlineCommandComment_getNumArgs(CXComment Comment);

        [DllImport(libraryPath, EntryPoint = "clang_InlineCommandComment_getArgText", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString InlineCommandComment_getArgText(CXComment Comment, uint ArgIdx);

        [DllImport(libraryPath, EntryPoint = "clang_HTMLTagComment_getTagName", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString HTMLTagComment_getTagName(CXComment Comment);

        [DllImport(libraryPath, EntryPoint = "clang_HTMLStartTagComment_isSelfClosing", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint HTMLStartTagComment_isSelfClosing(CXComment Comment);

        [DllImport(libraryPath, EntryPoint = "clang_HTMLStartTag_getNumAttrs", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint HTMLStartTag_getNumAttrs(CXComment Comment);

        [DllImport(libraryPath, EntryPoint = "clang_HTMLStartTag_getAttrName", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString HTMLStartTag_getAttrName(CXComment Comment, uint AttrIdx);

        [DllImport(libraryPath, EntryPoint = "clang_HTMLStartTag_getAttrValue", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString HTMLStartTag_getAttrValue(CXComment Comment, uint AttrIdx);

        [DllImport(libraryPath, EntryPoint = "clang_BlockCommandComment_getCommandName", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString BlockCommandComment_getCommandName(CXComment Comment);

        [DllImport(libraryPath, EntryPoint = "clang_BlockCommandComment_getNumArgs", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint BlockCommandComment_getNumArgs(CXComment Comment);

        [DllImport(libraryPath, EntryPoint = "clang_BlockCommandComment_getArgText", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString BlockCommandComment_getArgText(CXComment Comment, uint ArgIdx);

        [DllImport(libraryPath, EntryPoint = "clang_BlockCommandComment_getParagraph", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXComment BlockCommandComment_getParagraph(CXComment Comment);

        [DllImport(libraryPath, EntryPoint = "clang_ParamCommandComment_getParamName", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString ParamCommandComment_getParamName(CXComment Comment);

        [DllImport(libraryPath, EntryPoint = "clang_ParamCommandComment_isParamIndexValid", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint ParamCommandComment_isParamIndexValid(CXComment Comment);

        [DllImport(libraryPath, EntryPoint = "clang_ParamCommandComment_getParamIndex", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint ParamCommandComment_getParamIndex(CXComment Comment);

        [DllImport(libraryPath, EntryPoint = "clang_ParamCommandComment_isDirectionExplicit", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint ParamCommandComment_isDirectionExplicit(CXComment Comment);

        [DllImport(libraryPath, EntryPoint = "clang_ParamCommandComment_getDirection", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXCommentParamPassDirection ParamCommandComment_getDirection(CXComment Comment);

        [DllImport(libraryPath, EntryPoint = "clang_TParamCommandComment_getParamName", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString TParamCommandComment_getParamName(CXComment Comment);

        [DllImport(libraryPath, EntryPoint = "clang_TParamCommandComment_isParamPositionValid", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint TParamCommandComment_isParamPositionValid(CXComment Comment);

        [DllImport(libraryPath, EntryPoint = "clang_TParamCommandComment_getDepth", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint TParamCommandComment_getDepth(CXComment Comment);

        [DllImport(libraryPath, EntryPoint = "clang_TParamCommandComment_getIndex", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint TParamCommandComment_getIndex(CXComment Comment, uint Depth);

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
        public static extern ulong getBuildSessionTimestamp();

        [DllImport(libraryPath, EntryPoint = "clang_VirtualFileOverlay_create", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXVirtualFileOverlay VirtualFileOverlay_create(uint options);

        [DllImport(libraryPath, EntryPoint = "clang_VirtualFileOverlay_addFileMapping", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXErrorCode VirtualFileOverlay_addFileMapping(CXVirtualFileOverlay param0, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))] string virtualPath, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))] string realPath);

        [DllImport(libraryPath, EntryPoint = "clang_VirtualFileOverlay_setCaseSensitivity", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXErrorCode VirtualFileOverlay_setCaseSensitivity(CXVirtualFileOverlay param0, int caseSensitive);

        [DllImport(libraryPath, EntryPoint = "clang_VirtualFileOverlay_writeToBuffer", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXErrorCode VirtualFileOverlay_writeToBuffer(CXVirtualFileOverlay param0, uint options, out IntPtr out_buffer_ptr, out uint out_buffer_size);

        [DllImport(libraryPath, EntryPoint = "clang_free", CallingConvention = CallingConvention.Cdecl)]
        public static extern void free(IntPtr buffer);

        [DllImport(libraryPath, EntryPoint = "clang_VirtualFileOverlay_dispose", CallingConvention = CallingConvention.Cdecl)]
        public static extern void VirtualFileOverlay_dispose(CXVirtualFileOverlay param0);

        [DllImport(libraryPath, EntryPoint = "clang_ModuleMapDescriptor_create", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXModuleMapDescriptor ModuleMapDescriptor_create(uint options);

        [DllImport(libraryPath, EntryPoint = "clang_ModuleMapDescriptor_setFrameworkModuleName", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXErrorCode ModuleMapDescriptor_setFrameworkModuleName(CXModuleMapDescriptor param0, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))] string name);

        [DllImport(libraryPath, EntryPoint = "clang_ModuleMapDescriptor_setUmbrellaHeader", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXErrorCode ModuleMapDescriptor_setUmbrellaHeader(CXModuleMapDescriptor param0, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))] string name);

        [DllImport(libraryPath, EntryPoint = "clang_ModuleMapDescriptor_writeToBuffer", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXErrorCode ModuleMapDescriptor_writeToBuffer(CXModuleMapDescriptor param0, uint options, out IntPtr out_buffer_ptr, out uint out_buffer_size);

        [DllImport(libraryPath, EntryPoint = "clang_ModuleMapDescriptor_dispose", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ModuleMapDescriptor_dispose(CXModuleMapDescriptor param0);

        [DllImport(libraryPath, EntryPoint = "clang_CompilationDatabase_fromDirectory", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXCompilationDatabase CompilationDatabase_fromDirectory([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))] string BuildDir, out CXCompilationDatabase_Error ErrorCode);

        [DllImport(libraryPath, EntryPoint = "clang_CompilationDatabase_dispose", CallingConvention = CallingConvention.Cdecl)]
        public static extern void CompilationDatabase_dispose(CXCompilationDatabase param0);

        [DllImport(libraryPath, EntryPoint = "clang_CompilationDatabase_getCompileCommands", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXCompileCommands CompilationDatabase_getCompileCommands(CXCompilationDatabase param0, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))] string CompleteFileName);

        [DllImport(libraryPath, EntryPoint = "clang_CompilationDatabase_getAllCompileCommands", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXCompileCommands CompilationDatabase_getAllCompileCommands(CXCompilationDatabase param0);

        [DllImport(libraryPath, EntryPoint = "clang_CompileCommands_dispose", CallingConvention = CallingConvention.Cdecl)]
        public static extern void CompileCommands_dispose(CXCompileCommands param0);

        [DllImport(libraryPath, EntryPoint = "clang_CompileCommands_getSize", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint CompileCommands_getSize(CXCompileCommands param0);

        [DllImport(libraryPath, EntryPoint = "clang_CompileCommands_getCommand", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXCompileCommand CompileCommands_getCommand(CXCompileCommands param0, uint I);

        [DllImport(libraryPath, EntryPoint = "clang_CompileCommand_getDirectory", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString CompileCommand_getDirectory(CXCompileCommand param0);

        [DllImport(libraryPath, EntryPoint = "clang_CompileCommand_getFilename", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString CompileCommand_getFilename(CXCompileCommand param0);

        [DllImport(libraryPath, EntryPoint = "clang_CompileCommand_getNumArgs", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint CompileCommand_getNumArgs(CXCompileCommand param0);

        [DllImport(libraryPath, EntryPoint = "clang_CompileCommand_getArg", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString CompileCommand_getArg(CXCompileCommand param0, uint I);

        [DllImport(libraryPath, EntryPoint = "clang_CompileCommand_getNumMappedSources", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint CompileCommand_getNumMappedSources(CXCompileCommand param0);

        [DllImport(libraryPath, EntryPoint = "clang_CompileCommand_getMappedSourcePath", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString CompileCommand_getMappedSourcePath(CXCompileCommand param0, uint I);

        [DllImport(libraryPath, EntryPoint = "clang_CompileCommand_getMappedSourceContent", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString CompileCommand_getMappedSourceContent(CXCompileCommand param0, uint I);
    }
}
