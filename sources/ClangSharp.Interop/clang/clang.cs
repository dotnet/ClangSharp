// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

// Ported from https://github.com/llvm/llvm-project/tree/llvmorg-16.0.6/clang/include/clang-c
// Original source is Copyright (c) the LLVM Project and Contributors. Licensed under the Apache License v2.0 with LLVM Exceptions. See NOTICE.txt in the project root for license information.

using System;
using System.Runtime.InteropServices;

namespace ClangSharp.Interop;

public static unsafe partial class @clang
{
    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_getBuildSessionTimestamp", ExactSpelling = true)]
    [return: NativeTypeName("unsigned long long")]
    public static extern ulong getBuildSessionTimestamp();

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_VirtualFileOverlay_create", ExactSpelling = true)]
    [return: NativeTypeName("CXVirtualFileOverlay")]
    public static extern CXVirtualFileOverlayImpl* VirtualFileOverlay_create([NativeTypeName("unsigned int")] uint options);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_VirtualFileOverlay_addFileMapping", ExactSpelling = true)]
    [return: NativeTypeName("enum CXErrorCode")]
    public static extern CXErrorCode VirtualFileOverlay_addFileMapping([NativeTypeName("CXVirtualFileOverlay")] CXVirtualFileOverlayImpl* param0, [NativeTypeName("const char *")] sbyte* virtualPath, [NativeTypeName("const char *")] sbyte* realPath);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_VirtualFileOverlay_setCaseSensitivity", ExactSpelling = true)]
    [return: NativeTypeName("enum CXErrorCode")]
    public static extern CXErrorCode VirtualFileOverlay_setCaseSensitivity([NativeTypeName("CXVirtualFileOverlay")] CXVirtualFileOverlayImpl* param0, int caseSensitive);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_VirtualFileOverlay_writeToBuffer", ExactSpelling = true)]
    [return: NativeTypeName("enum CXErrorCode")]
    public static extern CXErrorCode VirtualFileOverlay_writeToBuffer([NativeTypeName("CXVirtualFileOverlay")] CXVirtualFileOverlayImpl* param0, [NativeTypeName("unsigned int")] uint options, [NativeTypeName("char **")] sbyte** out_buffer_ptr, [NativeTypeName("unsigned int *")] uint* out_buffer_size);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_free", ExactSpelling = true)]
    public static extern void free(void* buffer);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_VirtualFileOverlay_dispose", ExactSpelling = true)]
    public static extern void VirtualFileOverlay_dispose([NativeTypeName("CXVirtualFileOverlay")] CXVirtualFileOverlayImpl* param0);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_ModuleMapDescriptor_create", ExactSpelling = true)]
    [return: NativeTypeName("CXModuleMapDescriptor")]
    public static extern CXModuleMapDescriptorImpl* ModuleMapDescriptor_create([NativeTypeName("unsigned int")] uint options);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_ModuleMapDescriptor_setFrameworkModuleName", ExactSpelling = true)]
    [return: NativeTypeName("enum CXErrorCode")]
    public static extern CXErrorCode ModuleMapDescriptor_setFrameworkModuleName([NativeTypeName("CXModuleMapDescriptor")] CXModuleMapDescriptorImpl* param0, [NativeTypeName("const char *")] sbyte* name);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_ModuleMapDescriptor_setUmbrellaHeader", ExactSpelling = true)]
    [return: NativeTypeName("enum CXErrorCode")]
    public static extern CXErrorCode ModuleMapDescriptor_setUmbrellaHeader([NativeTypeName("CXModuleMapDescriptor")] CXModuleMapDescriptorImpl* param0, [NativeTypeName("const char *")] sbyte* name);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_ModuleMapDescriptor_writeToBuffer", ExactSpelling = true)]
    [return: NativeTypeName("enum CXErrorCode")]
    public static extern CXErrorCode ModuleMapDescriptor_writeToBuffer([NativeTypeName("CXModuleMapDescriptor")] CXModuleMapDescriptorImpl* param0, [NativeTypeName("unsigned int")] uint options, [NativeTypeName("char **")] sbyte** out_buffer_ptr, [NativeTypeName("unsigned int *")] uint* out_buffer_size);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_ModuleMapDescriptor_dispose", ExactSpelling = true)]
    public static extern void ModuleMapDescriptor_dispose([NativeTypeName("CXModuleMapDescriptor")] CXModuleMapDescriptorImpl* param0);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_CompilationDatabase_fromDirectory", ExactSpelling = true)]
    [return: NativeTypeName("CXCompilationDatabase")]
    public static extern void* CompilationDatabase_fromDirectory([NativeTypeName("const char *")] sbyte* BuildDir, CXCompilationDatabase_Error* ErrorCode);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_CompilationDatabase_dispose", ExactSpelling = true)]
    public static extern void CompilationDatabase_dispose([NativeTypeName("CXCompilationDatabase")] void* param0);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_CompilationDatabase_getCompileCommands", ExactSpelling = true)]
    [return: NativeTypeName("CXCompileCommands")]
    public static extern void* CompilationDatabase_getCompileCommands([NativeTypeName("CXCompilationDatabase")] void* param0, [NativeTypeName("const char *")] sbyte* CompleteFileName);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_CompilationDatabase_getAllCompileCommands", ExactSpelling = true)]
    [return: NativeTypeName("CXCompileCommands")]
    public static extern void* CompilationDatabase_getAllCompileCommands([NativeTypeName("CXCompilationDatabase")] void* param0);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_CompileCommands_dispose", ExactSpelling = true)]
    public static extern void CompileCommands_dispose([NativeTypeName("CXCompileCommands")] void* param0);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_CompileCommands_getSize", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint CompileCommands_getSize([NativeTypeName("CXCompileCommands")] void* param0);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_CompileCommands_getCommand", ExactSpelling = true)]
    [return: NativeTypeName("CXCompileCommand")]
    public static extern void* CompileCommands_getCommand([NativeTypeName("CXCompileCommands")] void* param0, [NativeTypeName("unsigned int")] uint I);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_CompileCommand_getDirectory", ExactSpelling = true)]
    public static extern CXString CompileCommand_getDirectory([NativeTypeName("CXCompileCommand")] void* param0);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_CompileCommand_getFilename", ExactSpelling = true)]
    public static extern CXString CompileCommand_getFilename([NativeTypeName("CXCompileCommand")] void* param0);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_CompileCommand_getNumArgs", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint CompileCommand_getNumArgs([NativeTypeName("CXCompileCommand")] void* param0);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_CompileCommand_getArg", ExactSpelling = true)]
    public static extern CXString CompileCommand_getArg([NativeTypeName("CXCompileCommand")] void* param0, [NativeTypeName("unsigned int")] uint I);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_CompileCommand_getNumMappedSources", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint CompileCommand_getNumMappedSources([NativeTypeName("CXCompileCommand")] void* param0);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_CompileCommand_getMappedSourcePath", ExactSpelling = true)]
    public static extern CXString CompileCommand_getMappedSourcePath([NativeTypeName("CXCompileCommand")] void* param0, [NativeTypeName("unsigned int")] uint I);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_CompileCommand_getMappedSourceContent", ExactSpelling = true)]
    public static extern CXString CompileCommand_getMappedSourceContent([NativeTypeName("CXCompileCommand")] void* param0, [NativeTypeName("unsigned int")] uint I);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_getCString", ExactSpelling = true)]
    [return: NativeTypeName("const char *")]
    public static extern sbyte* getCString(CXString @string);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_disposeString", ExactSpelling = true)]
    public static extern void disposeString(CXString @string);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_disposeStringSet", ExactSpelling = true)]
    public static extern void disposeStringSet(CXStringSet* set);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_Cursor_getParsedComment", ExactSpelling = true)]
    public static extern CXComment Cursor_getParsedComment(CXCursor C);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_Comment_getKind", ExactSpelling = true)]
    [return: NativeTypeName("enum CXCommentKind")]
    public static extern CXCommentKind Comment_getKind(CXComment Comment);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_Comment_getNumChildren", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Comment_getNumChildren(CXComment Comment);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_Comment_getChild", ExactSpelling = true)]
    public static extern CXComment Comment_getChild(CXComment Comment, [NativeTypeName("unsigned int")] uint ChildIdx);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_Comment_isWhitespace", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Comment_isWhitespace(CXComment Comment);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_InlineContentComment_hasTrailingNewline", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint InlineContentComment_hasTrailingNewline(CXComment Comment);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_TextComment_getText", ExactSpelling = true)]
    public static extern CXString TextComment_getText(CXComment Comment);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_InlineCommandComment_getCommandName", ExactSpelling = true)]
    public static extern CXString InlineCommandComment_getCommandName(CXComment Comment);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_InlineCommandComment_getRenderKind", ExactSpelling = true)]
    [return: NativeTypeName("enum CXCommentInlineCommandRenderKind")]
    public static extern CXCommentInlineCommandRenderKind InlineCommandComment_getRenderKind(CXComment Comment);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_InlineCommandComment_getNumArgs", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint InlineCommandComment_getNumArgs(CXComment Comment);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_InlineCommandComment_getArgText", ExactSpelling = true)]
    public static extern CXString InlineCommandComment_getArgText(CXComment Comment, [NativeTypeName("unsigned int")] uint ArgIdx);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_HTMLTagComment_getTagName", ExactSpelling = true)]
    public static extern CXString HTMLTagComment_getTagName(CXComment Comment);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_HTMLStartTagComment_isSelfClosing", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint HTMLStartTagComment_isSelfClosing(CXComment Comment);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_HTMLStartTag_getNumAttrs", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint HTMLStartTag_getNumAttrs(CXComment Comment);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_HTMLStartTag_getAttrName", ExactSpelling = true)]
    public static extern CXString HTMLStartTag_getAttrName(CXComment Comment, [NativeTypeName("unsigned int")] uint AttrIdx);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_HTMLStartTag_getAttrValue", ExactSpelling = true)]
    public static extern CXString HTMLStartTag_getAttrValue(CXComment Comment, [NativeTypeName("unsigned int")] uint AttrIdx);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_BlockCommandComment_getCommandName", ExactSpelling = true)]
    public static extern CXString BlockCommandComment_getCommandName(CXComment Comment);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_BlockCommandComment_getNumArgs", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint BlockCommandComment_getNumArgs(CXComment Comment);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_BlockCommandComment_getArgText", ExactSpelling = true)]
    public static extern CXString BlockCommandComment_getArgText(CXComment Comment, [NativeTypeName("unsigned int")] uint ArgIdx);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_BlockCommandComment_getParagraph", ExactSpelling = true)]
    public static extern CXComment BlockCommandComment_getParagraph(CXComment Comment);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_ParamCommandComment_getParamName", ExactSpelling = true)]
    public static extern CXString ParamCommandComment_getParamName(CXComment Comment);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_ParamCommandComment_isParamIndexValid", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint ParamCommandComment_isParamIndexValid(CXComment Comment);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_ParamCommandComment_getParamIndex", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint ParamCommandComment_getParamIndex(CXComment Comment);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_ParamCommandComment_isDirectionExplicit", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint ParamCommandComment_isDirectionExplicit(CXComment Comment);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_ParamCommandComment_getDirection", ExactSpelling = true)]
    [return: NativeTypeName("enum CXCommentParamPassDirection")]
    public static extern CXCommentParamPassDirection ParamCommandComment_getDirection(CXComment Comment);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_TParamCommandComment_getParamName", ExactSpelling = true)]
    public static extern CXString TParamCommandComment_getParamName(CXComment Comment);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_TParamCommandComment_isParamPositionValid", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint TParamCommandComment_isParamPositionValid(CXComment Comment);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_TParamCommandComment_getDepth", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint TParamCommandComment_getDepth(CXComment Comment);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_TParamCommandComment_getIndex", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint TParamCommandComment_getIndex(CXComment Comment, [NativeTypeName("unsigned int")] uint Depth);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_VerbatimBlockLineComment_getText", ExactSpelling = true)]
    public static extern CXString VerbatimBlockLineComment_getText(CXComment Comment);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_VerbatimLineComment_getText", ExactSpelling = true)]
    public static extern CXString VerbatimLineComment_getText(CXComment Comment);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_HTMLTagComment_getAsString", ExactSpelling = true)]
    public static extern CXString HTMLTagComment_getAsString(CXComment Comment);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_FullComment_getAsHTML", ExactSpelling = true)]
    public static extern CXString FullComment_getAsHTML(CXComment Comment);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_FullComment_getAsXML", ExactSpelling = true)]
    public static extern CXString FullComment_getAsXML(CXComment Comment);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_install_aborting_llvm_fatal_error_handler", ExactSpelling = true)]
    public static extern void install_aborting_llvm_fatal_error_handler();

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_uninstall_llvm_fatal_error_handler", ExactSpelling = true)]
    public static extern void uninstall_llvm_fatal_error_handler();

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_createIndex", ExactSpelling = true)]
    [return: NativeTypeName("CXIndex")]
    public static extern void* createIndex(int excludeDeclarationsFromPCH, int displayDiagnostics);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_disposeIndex", ExactSpelling = true)]
    public static extern void disposeIndex([NativeTypeName("CXIndex")] void* index);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_CXIndex_setGlobalOptions", ExactSpelling = true)]
    public static extern void CXIndex_setGlobalOptions([NativeTypeName("CXIndex")] void* param0, [NativeTypeName("unsigned int")] uint options);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_CXIndex_getGlobalOptions", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint CXIndex_getGlobalOptions([NativeTypeName("CXIndex")] void* param0);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_CXIndex_setInvocationEmissionPathOption", ExactSpelling = true)]
    public static extern void CXIndex_setInvocationEmissionPathOption([NativeTypeName("CXIndex")] void* param0, [NativeTypeName("const char *")] sbyte* Path);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_getFileName", ExactSpelling = true)]
    public static extern CXString getFileName([NativeTypeName("CXFile")] void* SFile);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_getFileTime", ExactSpelling = true)]
    [return: NativeTypeName("time_t")]
    public static extern long getFileTime([NativeTypeName("CXFile")] void* SFile);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_getFileUniqueID", ExactSpelling = true)]
    public static extern int getFileUniqueID([NativeTypeName("CXFile")] void* file, CXFileUniqueID* outID);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_isFileMultipleIncludeGuarded", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint isFileMultipleIncludeGuarded([NativeTypeName("CXTranslationUnit")] CXTranslationUnitImpl* tu, [NativeTypeName("CXFile")] void* file);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_getFile", ExactSpelling = true)]
    [return: NativeTypeName("CXFile")]
    public static extern void* getFile([NativeTypeName("CXTranslationUnit")] CXTranslationUnitImpl* tu, [NativeTypeName("const char *")] sbyte* file_name);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_getFileContents", ExactSpelling = true)]
    [return: NativeTypeName("const char *")]
    public static extern sbyte* getFileContents([NativeTypeName("CXTranslationUnit")] CXTranslationUnitImpl* tu, [NativeTypeName("CXFile")] void* file, [NativeTypeName("size_t *")] nuint* size);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_File_isEqual", ExactSpelling = true)]
    public static extern int File_isEqual([NativeTypeName("CXFile")] void* file1, [NativeTypeName("CXFile")] void* file2);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_File_tryGetRealPathName", ExactSpelling = true)]
    public static extern CXString File_tryGetRealPathName([NativeTypeName("CXFile")] void* file);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_getNullLocation", ExactSpelling = true)]
    public static extern CXSourceLocation getNullLocation();

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_equalLocations", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint equalLocations(CXSourceLocation loc1, CXSourceLocation loc2);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_getLocation", ExactSpelling = true)]
    public static extern CXSourceLocation getLocation([NativeTypeName("CXTranslationUnit")] CXTranslationUnitImpl* tu, [NativeTypeName("CXFile")] void* file, [NativeTypeName("unsigned int")] uint line, [NativeTypeName("unsigned int")] uint column);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_getLocationForOffset", ExactSpelling = true)]
    public static extern CXSourceLocation getLocationForOffset([NativeTypeName("CXTranslationUnit")] CXTranslationUnitImpl* tu, [NativeTypeName("CXFile")] void* file, [NativeTypeName("unsigned int")] uint offset);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_Location_isInSystemHeader", ExactSpelling = true)]
    public static extern int Location_isInSystemHeader(CXSourceLocation location);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_Location_isFromMainFile", ExactSpelling = true)]
    public static extern int Location_isFromMainFile(CXSourceLocation location);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_getNullRange", ExactSpelling = true)]
    public static extern CXSourceRange getNullRange();

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_getRange", ExactSpelling = true)]
    public static extern CXSourceRange getRange(CXSourceLocation begin, CXSourceLocation end);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_equalRanges", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint equalRanges(CXSourceRange range1, CXSourceRange range2);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_Range_isNull", ExactSpelling = true)]
    public static extern int Range_isNull(CXSourceRange range);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_getExpansionLocation", ExactSpelling = true)]
    public static extern void getExpansionLocation(CXSourceLocation location, [NativeTypeName("CXFile *")] void** file, [NativeTypeName("unsigned int *")] uint* line, [NativeTypeName("unsigned int *")] uint* column, [NativeTypeName("unsigned int *")] uint* offset);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_getPresumedLocation", ExactSpelling = true)]
    public static extern void getPresumedLocation(CXSourceLocation location, CXString* filename, [NativeTypeName("unsigned int *")] uint* line, [NativeTypeName("unsigned int *")] uint* column);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_getInstantiationLocation", ExactSpelling = true)]
    public static extern void getInstantiationLocation(CXSourceLocation location, [NativeTypeName("CXFile *")] void** file, [NativeTypeName("unsigned int *")] uint* line, [NativeTypeName("unsigned int *")] uint* column, [NativeTypeName("unsigned int *")] uint* offset);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_getSpellingLocation", ExactSpelling = true)]
    public static extern void getSpellingLocation(CXSourceLocation location, [NativeTypeName("CXFile *")] void** file, [NativeTypeName("unsigned int *")] uint* line, [NativeTypeName("unsigned int *")] uint* column, [NativeTypeName("unsigned int *")] uint* offset);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_getFileLocation", ExactSpelling = true)]
    public static extern void getFileLocation(CXSourceLocation location, [NativeTypeName("CXFile *")] void** file, [NativeTypeName("unsigned int *")] uint* line, [NativeTypeName("unsigned int *")] uint* column, [NativeTypeName("unsigned int *")] uint* offset);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_getRangeStart", ExactSpelling = true)]
    public static extern CXSourceLocation getRangeStart(CXSourceRange range);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_getRangeEnd", ExactSpelling = true)]
    public static extern CXSourceLocation getRangeEnd(CXSourceRange range);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_getSkippedRanges", ExactSpelling = true)]
    public static extern CXSourceRangeList* getSkippedRanges([NativeTypeName("CXTranslationUnit")] CXTranslationUnitImpl* tu, [NativeTypeName("CXFile")] void* file);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_getAllSkippedRanges", ExactSpelling = true)]
    public static extern CXSourceRangeList* getAllSkippedRanges([NativeTypeName("CXTranslationUnit")] CXTranslationUnitImpl* tu);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_disposeSourceRangeList", ExactSpelling = true)]
    public static extern void disposeSourceRangeList(CXSourceRangeList* ranges);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_getNumDiagnosticsInSet", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint getNumDiagnosticsInSet([NativeTypeName("CXDiagnosticSet")] void* Diags);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_getDiagnosticInSet", ExactSpelling = true)]
    [return: NativeTypeName("CXDiagnostic")]
    public static extern void* getDiagnosticInSet([NativeTypeName("CXDiagnosticSet")] void* Diags, [NativeTypeName("unsigned int")] uint Index);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_loadDiagnostics", ExactSpelling = true)]
    [return: NativeTypeName("CXDiagnosticSet")]
    public static extern void* loadDiagnostics([NativeTypeName("const char *")] sbyte* file, [NativeTypeName("enum CXLoadDiag_Error *")] CXLoadDiag_Error* error, CXString* errorString);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_disposeDiagnosticSet", ExactSpelling = true)]
    public static extern void disposeDiagnosticSet([NativeTypeName("CXDiagnosticSet")] void* Diags);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_getChildDiagnostics", ExactSpelling = true)]
    [return: NativeTypeName("CXDiagnosticSet")]
    public static extern void* getChildDiagnostics([NativeTypeName("CXDiagnostic")] void* D);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_getNumDiagnostics", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint getNumDiagnostics([NativeTypeName("CXTranslationUnit")] CXTranslationUnitImpl* Unit);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_getDiagnostic", ExactSpelling = true)]
    [return: NativeTypeName("CXDiagnostic")]
    public static extern void* getDiagnostic([NativeTypeName("CXTranslationUnit")] CXTranslationUnitImpl* Unit, [NativeTypeName("unsigned int")] uint Index);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_getDiagnosticSetFromTU", ExactSpelling = true)]
    [return: NativeTypeName("CXDiagnosticSet")]
    public static extern void* getDiagnosticSetFromTU([NativeTypeName("CXTranslationUnit")] CXTranslationUnitImpl* Unit);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_disposeDiagnostic", ExactSpelling = true)]
    public static extern void disposeDiagnostic([NativeTypeName("CXDiagnostic")] void* Diagnostic);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_formatDiagnostic", ExactSpelling = true)]
    public static extern CXString formatDiagnostic([NativeTypeName("CXDiagnostic")] void* Diagnostic, [NativeTypeName("unsigned int")] uint Options);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_defaultDiagnosticDisplayOptions", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint defaultDiagnosticDisplayOptions();

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_getDiagnosticSeverity", ExactSpelling = true)]
    [return: NativeTypeName("enum CXDiagnosticSeverity")]
    public static extern CXDiagnosticSeverity getDiagnosticSeverity([NativeTypeName("CXDiagnostic")] void* param0);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_getDiagnosticLocation", ExactSpelling = true)]
    public static extern CXSourceLocation getDiagnosticLocation([NativeTypeName("CXDiagnostic")] void* param0);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_getDiagnosticSpelling", ExactSpelling = true)]
    public static extern CXString getDiagnosticSpelling([NativeTypeName("CXDiagnostic")] void* param0);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_getDiagnosticOption", ExactSpelling = true)]
    public static extern CXString getDiagnosticOption([NativeTypeName("CXDiagnostic")] void* Diag, CXString* Disable);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_getDiagnosticCategory", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint getDiagnosticCategory([NativeTypeName("CXDiagnostic")] void* param0);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_getDiagnosticCategoryName", ExactSpelling = true)]
    [Obsolete]
    public static extern CXString getDiagnosticCategoryName([NativeTypeName("unsigned int")] uint Category);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_getDiagnosticCategoryText", ExactSpelling = true)]
    public static extern CXString getDiagnosticCategoryText([NativeTypeName("CXDiagnostic")] void* param0);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_getDiagnosticNumRanges", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint getDiagnosticNumRanges([NativeTypeName("CXDiagnostic")] void* param0);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_getDiagnosticRange", ExactSpelling = true)]
    public static extern CXSourceRange getDiagnosticRange([NativeTypeName("CXDiagnostic")] void* Diagnostic, [NativeTypeName("unsigned int")] uint Range);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_getDiagnosticNumFixIts", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint getDiagnosticNumFixIts([NativeTypeName("CXDiagnostic")] void* Diagnostic);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_getDiagnosticFixIt", ExactSpelling = true)]
    public static extern CXString getDiagnosticFixIt([NativeTypeName("CXDiagnostic")] void* Diagnostic, [NativeTypeName("unsigned int")] uint FixIt, CXSourceRange* ReplacementRange);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_getTranslationUnitSpelling", ExactSpelling = true)]
    public static extern CXString getTranslationUnitSpelling([NativeTypeName("CXTranslationUnit")] CXTranslationUnitImpl* CTUnit);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_createTranslationUnitFromSourceFile", ExactSpelling = true)]
    [return: NativeTypeName("CXTranslationUnit")]
    public static extern CXTranslationUnitImpl* createTranslationUnitFromSourceFile([NativeTypeName("CXIndex")] void* CIdx, [NativeTypeName("const char *")] sbyte* source_filename, int num_clang_command_line_args, [NativeTypeName("const char *const *")] sbyte** clang_command_line_args, [NativeTypeName("unsigned int")] uint num_unsaved_files, [NativeTypeName("struct CXUnsavedFile *")] CXUnsavedFile* unsaved_files);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_createTranslationUnit", ExactSpelling = true)]
    [return: NativeTypeName("CXTranslationUnit")]
    public static extern CXTranslationUnitImpl* createTranslationUnit([NativeTypeName("CXIndex")] void* CIdx, [NativeTypeName("const char *")] sbyte* ast_filename);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_createTranslationUnit2", ExactSpelling = true)]
    [return: NativeTypeName("enum CXErrorCode")]
    public static extern CXErrorCode createTranslationUnit2([NativeTypeName("CXIndex")] void* CIdx, [NativeTypeName("const char *")] sbyte* ast_filename, [NativeTypeName("CXTranslationUnit *")] CXTranslationUnitImpl** out_TU);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_defaultEditingTranslationUnitOptions", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint defaultEditingTranslationUnitOptions();

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_parseTranslationUnit", ExactSpelling = true)]
    [return: NativeTypeName("CXTranslationUnit")]
    public static extern CXTranslationUnitImpl* parseTranslationUnit([NativeTypeName("CXIndex")] void* CIdx, [NativeTypeName("const char *")] sbyte* source_filename, [NativeTypeName("const char *const *")] sbyte** command_line_args, int num_command_line_args, [NativeTypeName("struct CXUnsavedFile *")] CXUnsavedFile* unsaved_files, [NativeTypeName("unsigned int")] uint num_unsaved_files, [NativeTypeName("unsigned int")] uint options);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_parseTranslationUnit2", ExactSpelling = true)]
    [return: NativeTypeName("enum CXErrorCode")]
    public static extern CXErrorCode parseTranslationUnit2([NativeTypeName("CXIndex")] void* CIdx, [NativeTypeName("const char *")] sbyte* source_filename, [NativeTypeName("const char *const *")] sbyte** command_line_args, int num_command_line_args, [NativeTypeName("struct CXUnsavedFile *")] CXUnsavedFile* unsaved_files, [NativeTypeName("unsigned int")] uint num_unsaved_files, [NativeTypeName("unsigned int")] uint options, [NativeTypeName("CXTranslationUnit *")] CXTranslationUnitImpl** out_TU);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_parseTranslationUnit2FullArgv", ExactSpelling = true)]
    [return: NativeTypeName("enum CXErrorCode")]
    public static extern CXErrorCode parseTranslationUnit2FullArgv([NativeTypeName("CXIndex")] void* CIdx, [NativeTypeName("const char *")] sbyte* source_filename, [NativeTypeName("const char *const *")] sbyte** command_line_args, int num_command_line_args, [NativeTypeName("struct CXUnsavedFile *")] CXUnsavedFile* unsaved_files, [NativeTypeName("unsigned int")] uint num_unsaved_files, [NativeTypeName("unsigned int")] uint options, [NativeTypeName("CXTranslationUnit *")] CXTranslationUnitImpl** out_TU);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_defaultSaveOptions", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint defaultSaveOptions([NativeTypeName("CXTranslationUnit")] CXTranslationUnitImpl* TU);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_saveTranslationUnit", ExactSpelling = true)]
    public static extern int saveTranslationUnit([NativeTypeName("CXTranslationUnit")] CXTranslationUnitImpl* TU, [NativeTypeName("const char *")] sbyte* FileName, [NativeTypeName("unsigned int")] uint options);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_suspendTranslationUnit", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint suspendTranslationUnit([NativeTypeName("CXTranslationUnit")] CXTranslationUnitImpl* param0);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_disposeTranslationUnit", ExactSpelling = true)]
    public static extern void disposeTranslationUnit([NativeTypeName("CXTranslationUnit")] CXTranslationUnitImpl* param0);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_defaultReparseOptions", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint defaultReparseOptions([NativeTypeName("CXTranslationUnit")] CXTranslationUnitImpl* TU);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_reparseTranslationUnit", ExactSpelling = true)]
    public static extern int reparseTranslationUnit([NativeTypeName("CXTranslationUnit")] CXTranslationUnitImpl* TU, [NativeTypeName("unsigned int")] uint num_unsaved_files, [NativeTypeName("struct CXUnsavedFile *")] CXUnsavedFile* unsaved_files, [NativeTypeName("unsigned int")] uint options);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_getTUResourceUsageName", ExactSpelling = true)]
    [return: NativeTypeName("const char *")]
    public static extern sbyte* getTUResourceUsageName([NativeTypeName("enum CXTUResourceUsageKind")] CXTUResourceUsageKind kind);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_getCXTUResourceUsage", ExactSpelling = true)]
    public static extern CXTUResourceUsage getCXTUResourceUsage([NativeTypeName("CXTranslationUnit")] CXTranslationUnitImpl* TU);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_disposeCXTUResourceUsage", ExactSpelling = true)]
    public static extern void disposeCXTUResourceUsage(CXTUResourceUsage usage);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_getTranslationUnitTargetInfo", ExactSpelling = true)]
    [return: NativeTypeName("CXTargetInfo")]
    public static extern CXTargetInfoImpl* getTranslationUnitTargetInfo([NativeTypeName("CXTranslationUnit")] CXTranslationUnitImpl* CTUnit);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_TargetInfo_dispose", ExactSpelling = true)]
    public static extern void TargetInfo_dispose([NativeTypeName("CXTargetInfo")] CXTargetInfoImpl* Info);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_TargetInfo_getTriple", ExactSpelling = true)]
    public static extern CXString TargetInfo_getTriple([NativeTypeName("CXTargetInfo")] CXTargetInfoImpl* Info);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_TargetInfo_getPointerWidth", ExactSpelling = true)]
    public static extern int TargetInfo_getPointerWidth([NativeTypeName("CXTargetInfo")] CXTargetInfoImpl* Info);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_getNullCursor", ExactSpelling = true)]
    public static extern CXCursor getNullCursor();

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_getTranslationUnitCursor", ExactSpelling = true)]
    public static extern CXCursor getTranslationUnitCursor([NativeTypeName("CXTranslationUnit")] CXTranslationUnitImpl* param0);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_equalCursors", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint equalCursors(CXCursor param0, CXCursor param1);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_Cursor_isNull", ExactSpelling = true)]
    public static extern int Cursor_isNull(CXCursor cursor);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_hashCursor", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint hashCursor(CXCursor param0);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_getCursorKind", ExactSpelling = true)]
    [return: NativeTypeName("enum CXCursorKind")]
    public static extern CXCursorKind getCursorKind(CXCursor param0);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_isDeclaration", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint isDeclaration([NativeTypeName("enum CXCursorKind")] CXCursorKind param0);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_isInvalidDeclaration", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint isInvalidDeclaration(CXCursor param0);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_isReference", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint isReference([NativeTypeName("enum CXCursorKind")] CXCursorKind param0);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_isExpression", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint isExpression([NativeTypeName("enum CXCursorKind")] CXCursorKind param0);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_isStatement", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint isStatement([NativeTypeName("enum CXCursorKind")] CXCursorKind param0);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_isAttribute", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint isAttribute([NativeTypeName("enum CXCursorKind")] CXCursorKind param0);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_Cursor_hasAttrs", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Cursor_hasAttrs(CXCursor C);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_isInvalid", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint isInvalid([NativeTypeName("enum CXCursorKind")] CXCursorKind param0);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_isTranslationUnit", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint isTranslationUnit([NativeTypeName("enum CXCursorKind")] CXCursorKind param0);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_isPreprocessing", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint isPreprocessing([NativeTypeName("enum CXCursorKind")] CXCursorKind param0);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_isUnexposed", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint isUnexposed([NativeTypeName("enum CXCursorKind")] CXCursorKind param0);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_getCursorLinkage", ExactSpelling = true)]
    [return: NativeTypeName("enum CXLinkageKind")]
    public static extern CXLinkageKind getCursorLinkage(CXCursor cursor);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_getCursorVisibility", ExactSpelling = true)]
    [return: NativeTypeName("enum CXVisibilityKind")]
    public static extern CXVisibilityKind getCursorVisibility(CXCursor cursor);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_getCursorAvailability", ExactSpelling = true)]
    [return: NativeTypeName("enum CXAvailabilityKind")]
    public static extern CXAvailabilityKind getCursorAvailability(CXCursor cursor);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_getCursorPlatformAvailability", ExactSpelling = true)]
    public static extern int getCursorPlatformAvailability(CXCursor cursor, int* always_deprecated, CXString* deprecated_message, int* always_unavailable, CXString* unavailable_message, CXPlatformAvailability* availability, int availability_size);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_disposeCXPlatformAvailability", ExactSpelling = true)]
    public static extern void disposeCXPlatformAvailability(CXPlatformAvailability* availability);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_Cursor_getVarDeclInitializer", ExactSpelling = true)]
    public static extern CXCursor Cursor_getVarDeclInitializer(CXCursor cursor);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_Cursor_hasVarDeclGlobalStorage", ExactSpelling = true)]
    public static extern int Cursor_hasVarDeclGlobalStorage(CXCursor cursor);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_Cursor_hasVarDeclExternalStorage", ExactSpelling = true)]
    public static extern int Cursor_hasVarDeclExternalStorage(CXCursor cursor);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_getCursorLanguage", ExactSpelling = true)]
    [return: NativeTypeName("enum CXLanguageKind")]
    public static extern CXLanguageKind getCursorLanguage(CXCursor cursor);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_getCursorTLSKind", ExactSpelling = true)]
    [return: NativeTypeName("enum CXTLSKind")]
    public static extern CXTLSKind getCursorTLSKind(CXCursor cursor);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_Cursor_getTranslationUnit", ExactSpelling = true)]
    [return: NativeTypeName("CXTranslationUnit")]
    public static extern CXTranslationUnitImpl* Cursor_getTranslationUnit(CXCursor param0);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_createCXCursorSet", ExactSpelling = true)]
    [return: NativeTypeName("CXCursorSet")]
    public static extern CXCursorSetImpl* createCXCursorSet();

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_disposeCXCursorSet", ExactSpelling = true)]
    public static extern void disposeCXCursorSet([NativeTypeName("CXCursorSet")] CXCursorSetImpl* cset);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_CXCursorSet_contains", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint CXCursorSet_contains([NativeTypeName("CXCursorSet")] CXCursorSetImpl* cset, CXCursor cursor);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_CXCursorSet_insert", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint CXCursorSet_insert([NativeTypeName("CXCursorSet")] CXCursorSetImpl* cset, CXCursor cursor);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_getCursorSemanticParent", ExactSpelling = true)]
    public static extern CXCursor getCursorSemanticParent(CXCursor cursor);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_getCursorLexicalParent", ExactSpelling = true)]
    public static extern CXCursor getCursorLexicalParent(CXCursor cursor);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_getOverriddenCursors", ExactSpelling = true)]
    public static extern void getOverriddenCursors(CXCursor cursor, CXCursor** overridden, [NativeTypeName("unsigned int *")] uint* num_overridden);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_disposeOverriddenCursors", ExactSpelling = true)]
    public static extern void disposeOverriddenCursors(CXCursor* overridden);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_getIncludedFile", ExactSpelling = true)]
    [return: NativeTypeName("CXFile")]
    public static extern void* getIncludedFile(CXCursor cursor);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_getCursor", ExactSpelling = true)]
    public static extern CXCursor getCursor([NativeTypeName("CXTranslationUnit")] CXTranslationUnitImpl* param0, CXSourceLocation param1);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_getCursorLocation", ExactSpelling = true)]
    public static extern CXSourceLocation getCursorLocation(CXCursor param0);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_getCursorExtent", ExactSpelling = true)]
    public static extern CXSourceRange getCursorExtent(CXCursor param0);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_getCursorType", ExactSpelling = true)]
    public static extern CXType getCursorType(CXCursor C);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_getTypeSpelling", ExactSpelling = true)]
    public static extern CXString getTypeSpelling(CXType CT);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_getTypedefDeclUnderlyingType", ExactSpelling = true)]
    public static extern CXType getTypedefDeclUnderlyingType(CXCursor C);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_getEnumDeclIntegerType", ExactSpelling = true)]
    public static extern CXType getEnumDeclIntegerType(CXCursor C);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_getEnumConstantDeclValue", ExactSpelling = true)]
    [return: NativeTypeName("long long")]
    public static extern long getEnumConstantDeclValue(CXCursor C);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_getEnumConstantDeclUnsignedValue", ExactSpelling = true)]
    [return: NativeTypeName("unsigned long long")]
    public static extern ulong getEnumConstantDeclUnsignedValue(CXCursor C);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_getFieldDeclBitWidth", ExactSpelling = true)]
    public static extern int getFieldDeclBitWidth(CXCursor C);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_Cursor_getNumArguments", ExactSpelling = true)]
    public static extern int Cursor_getNumArguments(CXCursor C);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_Cursor_getArgument", ExactSpelling = true)]
    public static extern CXCursor Cursor_getArgument(CXCursor C, [NativeTypeName("unsigned int")] uint i);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_Cursor_getNumTemplateArguments", ExactSpelling = true)]
    public static extern int Cursor_getNumTemplateArguments(CXCursor C);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_Cursor_getTemplateArgumentKind", ExactSpelling = true)]
    [return: NativeTypeName("enum CXTemplateArgumentKind")]
    public static extern CXTemplateArgumentKind Cursor_getTemplateArgumentKind(CXCursor C, [NativeTypeName("unsigned int")] uint I);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_Cursor_getTemplateArgumentType", ExactSpelling = true)]
    public static extern CXType Cursor_getTemplateArgumentType(CXCursor C, [NativeTypeName("unsigned int")] uint I);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_Cursor_getTemplateArgumentValue", ExactSpelling = true)]
    [return: NativeTypeName("long long")]
    public static extern long Cursor_getTemplateArgumentValue(CXCursor C, [NativeTypeName("unsigned int")] uint I);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_Cursor_getTemplateArgumentUnsignedValue", ExactSpelling = true)]
    [return: NativeTypeName("unsigned long long")]
    public static extern ulong Cursor_getTemplateArgumentUnsignedValue(CXCursor C, [NativeTypeName("unsigned int")] uint I);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_equalTypes", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint equalTypes(CXType A, CXType B);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_getCanonicalType", ExactSpelling = true)]
    public static extern CXType getCanonicalType(CXType T);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_isConstQualifiedType", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint isConstQualifiedType(CXType T);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_Cursor_isMacroFunctionLike", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Cursor_isMacroFunctionLike(CXCursor C);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_Cursor_isMacroBuiltin", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Cursor_isMacroBuiltin(CXCursor C);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_Cursor_isFunctionInlined", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Cursor_isFunctionInlined(CXCursor C);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_isVolatileQualifiedType", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint isVolatileQualifiedType(CXType T);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_isRestrictQualifiedType", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint isRestrictQualifiedType(CXType T);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_getAddressSpace", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint getAddressSpace(CXType T);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_getTypedefName", ExactSpelling = true)]
    public static extern CXString getTypedefName(CXType CT);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_getPointeeType", ExactSpelling = true)]
    public static extern CXType getPointeeType(CXType T);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_getTypeDeclaration", ExactSpelling = true)]
    public static extern CXCursor getTypeDeclaration(CXType T);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_getDeclObjCTypeEncoding", ExactSpelling = true)]
    public static extern CXString getDeclObjCTypeEncoding(CXCursor C);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_Type_getObjCEncoding", ExactSpelling = true)]
    public static extern CXString Type_getObjCEncoding(CXType type);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_getTypeKindSpelling", ExactSpelling = true)]
    public static extern CXString getTypeKindSpelling([NativeTypeName("enum CXTypeKind")] CXTypeKind K);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_getFunctionTypeCallingConv", ExactSpelling = true)]
    [return: NativeTypeName("enum CXCallingConv")]
    public static extern CXCallingConv getFunctionTypeCallingConv(CXType T);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_getResultType", ExactSpelling = true)]
    public static extern CXType getResultType(CXType T);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_getExceptionSpecificationType", ExactSpelling = true)]
    public static extern int getExceptionSpecificationType(CXType T);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_getNumArgTypes", ExactSpelling = true)]
    public static extern int getNumArgTypes(CXType T);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_getArgType", ExactSpelling = true)]
    public static extern CXType getArgType(CXType T, [NativeTypeName("unsigned int")] uint i);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_Type_getObjCObjectBaseType", ExactSpelling = true)]
    public static extern CXType Type_getObjCObjectBaseType(CXType T);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_Type_getNumObjCProtocolRefs", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Type_getNumObjCProtocolRefs(CXType T);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_Type_getObjCProtocolDecl", ExactSpelling = true)]
    public static extern CXCursor Type_getObjCProtocolDecl(CXType T, [NativeTypeName("unsigned int")] uint i);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_Type_getNumObjCTypeArgs", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Type_getNumObjCTypeArgs(CXType T);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_Type_getObjCTypeArg", ExactSpelling = true)]
    public static extern CXType Type_getObjCTypeArg(CXType T, [NativeTypeName("unsigned int")] uint i);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_isFunctionTypeVariadic", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint isFunctionTypeVariadic(CXType T);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_getCursorResultType", ExactSpelling = true)]
    public static extern CXType getCursorResultType(CXCursor C);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_getCursorExceptionSpecificationType", ExactSpelling = true)]
    public static extern int getCursorExceptionSpecificationType(CXCursor C);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_isPODType", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint isPODType(CXType T);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_getElementType", ExactSpelling = true)]
    public static extern CXType getElementType(CXType T);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_getNumElements", ExactSpelling = true)]
    [return: NativeTypeName("long long")]
    public static extern long getNumElements(CXType T);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_getArrayElementType", ExactSpelling = true)]
    public static extern CXType getArrayElementType(CXType T);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_getArraySize", ExactSpelling = true)]
    [return: NativeTypeName("long long")]
    public static extern long getArraySize(CXType T);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_Type_getNamedType", ExactSpelling = true)]
    public static extern CXType Type_getNamedType(CXType T);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_Type_isTransparentTagTypedef", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Type_isTransparentTagTypedef(CXType T);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_Type_getNullability", ExactSpelling = true)]
    [return: NativeTypeName("enum CXTypeNullabilityKind")]
    public static extern CXTypeNullabilityKind Type_getNullability(CXType T);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_Type_getAlignOf", ExactSpelling = true)]
    [return: NativeTypeName("long long")]
    public static extern long Type_getAlignOf(CXType T);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_Type_getClassType", ExactSpelling = true)]
    public static extern CXType Type_getClassType(CXType T);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_Type_getSizeOf", ExactSpelling = true)]
    [return: NativeTypeName("long long")]
    public static extern long Type_getSizeOf(CXType T);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_Type_getOffsetOf", ExactSpelling = true)]
    [return: NativeTypeName("long long")]
    public static extern long Type_getOffsetOf(CXType T, [NativeTypeName("const char *")] sbyte* S);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_Type_getModifiedType", ExactSpelling = true)]
    public static extern CXType Type_getModifiedType(CXType T);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_Type_getValueType", ExactSpelling = true)]
    public static extern CXType Type_getValueType(CXType CT);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_Cursor_getOffsetOfField", ExactSpelling = true)]
    [return: NativeTypeName("long long")]
    public static extern long Cursor_getOffsetOfField(CXCursor C);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_Cursor_isAnonymous", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Cursor_isAnonymous(CXCursor C);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_Cursor_isAnonymousRecordDecl", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Cursor_isAnonymousRecordDecl(CXCursor C);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_Cursor_isInlineNamespace", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Cursor_isInlineNamespace(CXCursor C);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_Type_getNumTemplateArguments", ExactSpelling = true)]
    public static extern int Type_getNumTemplateArguments(CXType T);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_Type_getTemplateArgumentAsType", ExactSpelling = true)]
    public static extern CXType Type_getTemplateArgumentAsType(CXType T, [NativeTypeName("unsigned int")] uint i);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_Type_getCXXRefQualifier", ExactSpelling = true)]
    [return: NativeTypeName("enum CXRefQualifierKind")]
    public static extern CXRefQualifierKind Type_getCXXRefQualifier(CXType T);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_Cursor_isBitField", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Cursor_isBitField(CXCursor C);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_isVirtualBase", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint isVirtualBase(CXCursor param0);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_getCXXAccessSpecifier", ExactSpelling = true)]
    [return: NativeTypeName("enum CX_CXXAccessSpecifier")]
    public static extern CX_CXXAccessSpecifier getCXXAccessSpecifier(CXCursor param0);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_Cursor_getStorageClass", ExactSpelling = true)]
    [return: NativeTypeName("enum CX_StorageClass")]
    public static extern CX_StorageClass Cursor_getStorageClass(CXCursor param0);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_getNumOverloadedDecls", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint getNumOverloadedDecls(CXCursor cursor);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_getOverloadedDecl", ExactSpelling = true)]
    public static extern CXCursor getOverloadedDecl(CXCursor cursor, [NativeTypeName("unsigned int")] uint index);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_getIBOutletCollectionType", ExactSpelling = true)]
    public static extern CXType getIBOutletCollectionType(CXCursor param0);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_visitChildren", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint visitChildren(CXCursor parent, [NativeTypeName("CXCursorVisitor")] delegate* unmanaged[Cdecl]<CXCursor, CXCursor, void*, CXChildVisitResult> visitor, [NativeTypeName("CXClientData")] void* client_data);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_getCursorUSR", ExactSpelling = true)]
    public static extern CXString getCursorUSR(CXCursor param0);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_constructUSR_ObjCClass", ExactSpelling = true)]
    public static extern CXString constructUSR_ObjCClass([NativeTypeName("const char *")] sbyte* class_name);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_constructUSR_ObjCCategory", ExactSpelling = true)]
    public static extern CXString constructUSR_ObjCCategory([NativeTypeName("const char *")] sbyte* class_name, [NativeTypeName("const char *")] sbyte* category_name);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_constructUSR_ObjCProtocol", ExactSpelling = true)]
    public static extern CXString constructUSR_ObjCProtocol([NativeTypeName("const char *")] sbyte* protocol_name);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_constructUSR_ObjCIvar", ExactSpelling = true)]
    public static extern CXString constructUSR_ObjCIvar([NativeTypeName("const char *")] sbyte* name, CXString classUSR);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_constructUSR_ObjCMethod", ExactSpelling = true)]
    public static extern CXString constructUSR_ObjCMethod([NativeTypeName("const char *")] sbyte* name, [NativeTypeName("unsigned int")] uint isInstanceMethod, CXString classUSR);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_constructUSR_ObjCProperty", ExactSpelling = true)]
    public static extern CXString constructUSR_ObjCProperty([NativeTypeName("const char *")] sbyte* property, CXString classUSR);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_getCursorSpelling", ExactSpelling = true)]
    public static extern CXString getCursorSpelling(CXCursor param0);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_Cursor_getSpellingNameRange", ExactSpelling = true)]
    public static extern CXSourceRange Cursor_getSpellingNameRange(CXCursor param0, [NativeTypeName("unsigned int")] uint pieceIndex, [NativeTypeName("unsigned int")] uint options);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_PrintingPolicy_getProperty", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint PrintingPolicy_getProperty([NativeTypeName("CXPrintingPolicy")] void* Policy, [NativeTypeName("enum CXPrintingPolicyProperty")] CXPrintingPolicyProperty Property);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_PrintingPolicy_setProperty", ExactSpelling = true)]
    public static extern void PrintingPolicy_setProperty([NativeTypeName("CXPrintingPolicy")] void* Policy, [NativeTypeName("enum CXPrintingPolicyProperty")] CXPrintingPolicyProperty Property, [NativeTypeName("unsigned int")] uint Value);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_getCursorPrintingPolicy", ExactSpelling = true)]
    [return: NativeTypeName("CXPrintingPolicy")]
    public static extern void* getCursorPrintingPolicy(CXCursor param0);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_PrintingPolicy_dispose", ExactSpelling = true)]
    public static extern void PrintingPolicy_dispose([NativeTypeName("CXPrintingPolicy")] void* Policy);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_getCursorPrettyPrinted", ExactSpelling = true)]
    public static extern CXString getCursorPrettyPrinted(CXCursor Cursor, [NativeTypeName("CXPrintingPolicy")] void* Policy);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_getCursorDisplayName", ExactSpelling = true)]
    public static extern CXString getCursorDisplayName(CXCursor param0);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_getCursorReferenced", ExactSpelling = true)]
    public static extern CXCursor getCursorReferenced(CXCursor param0);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_getCursorDefinition", ExactSpelling = true)]
    public static extern CXCursor getCursorDefinition(CXCursor param0);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_isCursorDefinition", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint isCursorDefinition(CXCursor param0);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_getCanonicalCursor", ExactSpelling = true)]
    public static extern CXCursor getCanonicalCursor(CXCursor param0);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_Cursor_getObjCSelectorIndex", ExactSpelling = true)]
    public static extern int Cursor_getObjCSelectorIndex(CXCursor param0);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_Cursor_isDynamicCall", ExactSpelling = true)]
    public static extern int Cursor_isDynamicCall(CXCursor C);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_Cursor_getReceiverType", ExactSpelling = true)]
    public static extern CXType Cursor_getReceiverType(CXCursor C);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_Cursor_getObjCPropertyAttributes", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Cursor_getObjCPropertyAttributes(CXCursor C, [NativeTypeName("unsigned int")] uint reserved);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_Cursor_getObjCPropertyGetterName", ExactSpelling = true)]
    public static extern CXString Cursor_getObjCPropertyGetterName(CXCursor C);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_Cursor_getObjCPropertySetterName", ExactSpelling = true)]
    public static extern CXString Cursor_getObjCPropertySetterName(CXCursor C);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_Cursor_getObjCDeclQualifiers", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Cursor_getObjCDeclQualifiers(CXCursor C);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_Cursor_isObjCOptional", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Cursor_isObjCOptional(CXCursor C);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_Cursor_isVariadic", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Cursor_isVariadic(CXCursor C);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_Cursor_isExternalSymbol", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Cursor_isExternalSymbol(CXCursor C, CXString* language, CXString* definedIn, [NativeTypeName("unsigned int *")] uint* isGenerated);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_Cursor_getCommentRange", ExactSpelling = true)]
    public static extern CXSourceRange Cursor_getCommentRange(CXCursor C);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_Cursor_getRawCommentText", ExactSpelling = true)]
    public static extern CXString Cursor_getRawCommentText(CXCursor C);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_Cursor_getBriefCommentText", ExactSpelling = true)]
    public static extern CXString Cursor_getBriefCommentText(CXCursor C);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_Cursor_getMangling", ExactSpelling = true)]
    public static extern CXString Cursor_getMangling(CXCursor param0);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_Cursor_getCXXManglings", ExactSpelling = true)]
    public static extern CXStringSet* Cursor_getCXXManglings(CXCursor param0);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_Cursor_getObjCManglings", ExactSpelling = true)]
    public static extern CXStringSet* Cursor_getObjCManglings(CXCursor param0);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_Cursor_getModule", ExactSpelling = true)]
    [return: NativeTypeName("CXModule")]
    public static extern void* Cursor_getModule(CXCursor C);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_getModuleForFile", ExactSpelling = true)]
    [return: NativeTypeName("CXModule")]
    public static extern void* getModuleForFile([NativeTypeName("CXTranslationUnit")] CXTranslationUnitImpl* param0, [NativeTypeName("CXFile")] void* param1);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_Module_getASTFile", ExactSpelling = true)]
    [return: NativeTypeName("CXFile")]
    public static extern void* Module_getASTFile([NativeTypeName("CXModule")] void* Module);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_Module_getParent", ExactSpelling = true)]
    [return: NativeTypeName("CXModule")]
    public static extern void* Module_getParent([NativeTypeName("CXModule")] void* Module);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_Module_getName", ExactSpelling = true)]
    public static extern CXString Module_getName([NativeTypeName("CXModule")] void* Module);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_Module_getFullName", ExactSpelling = true)]
    public static extern CXString Module_getFullName([NativeTypeName("CXModule")] void* Module);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_Module_isSystem", ExactSpelling = true)]
    public static extern int Module_isSystem([NativeTypeName("CXModule")] void* Module);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_Module_getNumTopLevelHeaders", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Module_getNumTopLevelHeaders([NativeTypeName("CXTranslationUnit")] CXTranslationUnitImpl* param0, [NativeTypeName("CXModule")] void* Module);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_Module_getTopLevelHeader", ExactSpelling = true)]
    [return: NativeTypeName("CXFile")]
    public static extern void* Module_getTopLevelHeader([NativeTypeName("CXTranslationUnit")] CXTranslationUnitImpl* param0, [NativeTypeName("CXModule")] void* Module, [NativeTypeName("unsigned int")] uint Index);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_CXXConstructor_isConvertingConstructor", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint CXXConstructor_isConvertingConstructor(CXCursor C);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_CXXConstructor_isCopyConstructor", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint CXXConstructor_isCopyConstructor(CXCursor C);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_CXXConstructor_isDefaultConstructor", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint CXXConstructor_isDefaultConstructor(CXCursor C);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_CXXConstructor_isMoveConstructor", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint CXXConstructor_isMoveConstructor(CXCursor C);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_CXXField_isMutable", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint CXXField_isMutable(CXCursor C);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_CXXMethod_isDefaulted", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint CXXMethod_isDefaulted(CXCursor C);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_CXXMethod_isPureVirtual", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint CXXMethod_isPureVirtual(CXCursor C);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_CXXMethod_isStatic", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint CXXMethod_isStatic(CXCursor C);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_CXXMethod_isVirtual", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint CXXMethod_isVirtual(CXCursor C);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_CXXRecord_isAbstract", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint CXXRecord_isAbstract(CXCursor C);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_EnumDecl_isScoped", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint EnumDecl_isScoped(CXCursor C);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_CXXMethod_isConst", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint CXXMethod_isConst(CXCursor C);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_getTemplateCursorKind", ExactSpelling = true)]
    [return: NativeTypeName("enum CXCursorKind")]
    public static extern CXCursorKind getTemplateCursorKind(CXCursor C);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_getSpecializedCursorTemplate", ExactSpelling = true)]
    public static extern CXCursor getSpecializedCursorTemplate(CXCursor C);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_getCursorReferenceNameRange", ExactSpelling = true)]
    public static extern CXSourceRange getCursorReferenceNameRange(CXCursor C, [NativeTypeName("unsigned int")] uint NameFlags, [NativeTypeName("unsigned int")] uint PieceIndex);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_getToken", ExactSpelling = true)]
    public static extern CXToken* getToken([NativeTypeName("CXTranslationUnit")] CXTranslationUnitImpl* TU, CXSourceLocation Location);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_getTokenKind", ExactSpelling = true)]
    public static extern CXTokenKind getTokenKind(CXToken param0);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_getTokenSpelling", ExactSpelling = true)]
    public static extern CXString getTokenSpelling([NativeTypeName("CXTranslationUnit")] CXTranslationUnitImpl* param0, CXToken param1);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_getTokenLocation", ExactSpelling = true)]
    public static extern CXSourceLocation getTokenLocation([NativeTypeName("CXTranslationUnit")] CXTranslationUnitImpl* param0, CXToken param1);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_getTokenExtent", ExactSpelling = true)]
    public static extern CXSourceRange getTokenExtent([NativeTypeName("CXTranslationUnit")] CXTranslationUnitImpl* param0, CXToken param1);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_tokenize", ExactSpelling = true)]
    public static extern void tokenize([NativeTypeName("CXTranslationUnit")] CXTranslationUnitImpl* TU, CXSourceRange Range, CXToken** Tokens, [NativeTypeName("unsigned int *")] uint* NumTokens);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_annotateTokens", ExactSpelling = true)]
    public static extern void annotateTokens([NativeTypeName("CXTranslationUnit")] CXTranslationUnitImpl* TU, CXToken* Tokens, [NativeTypeName("unsigned int")] uint NumTokens, CXCursor* Cursors);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_disposeTokens", ExactSpelling = true)]
    public static extern void disposeTokens([NativeTypeName("CXTranslationUnit")] CXTranslationUnitImpl* TU, CXToken* Tokens, [NativeTypeName("unsigned int")] uint NumTokens);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_getCursorKindSpelling", ExactSpelling = true)]
    public static extern CXString getCursorKindSpelling([NativeTypeName("enum CXCursorKind")] CXCursorKind Kind);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_getDefinitionSpellingAndExtent", ExactSpelling = true)]
    public static extern void getDefinitionSpellingAndExtent(CXCursor param0, [NativeTypeName("const char **")] sbyte** startBuf, [NativeTypeName("const char **")] sbyte** endBuf, [NativeTypeName("unsigned int *")] uint* startLine, [NativeTypeName("unsigned int *")] uint* startColumn, [NativeTypeName("unsigned int *")] uint* endLine, [NativeTypeName("unsigned int *")] uint* endColumn);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_enableStackTraces", ExactSpelling = true)]
    public static extern void enableStackTraces();

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_executeOnThread", ExactSpelling = true)]
    public static extern void executeOnThread([NativeTypeName("void (*)(void *)")] delegate* unmanaged[Cdecl]<void*, void> fn, void* user_data, [NativeTypeName("unsigned int")] uint stack_size);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_getCompletionChunkKind", ExactSpelling = true)]
    [return: NativeTypeName("enum CXCompletionChunkKind")]
    public static extern CXCompletionChunkKind getCompletionChunkKind([NativeTypeName("CXCompletionString")] void* completion_string, [NativeTypeName("unsigned int")] uint chunk_number);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_getCompletionChunkText", ExactSpelling = true)]
    public static extern CXString getCompletionChunkText([NativeTypeName("CXCompletionString")] void* completion_string, [NativeTypeName("unsigned int")] uint chunk_number);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_getCompletionChunkCompletionString", ExactSpelling = true)]
    [return: NativeTypeName("CXCompletionString")]
    public static extern void* getCompletionChunkCompletionString([NativeTypeName("CXCompletionString")] void* completion_string, [NativeTypeName("unsigned int")] uint chunk_number);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_getNumCompletionChunks", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint getNumCompletionChunks([NativeTypeName("CXCompletionString")] void* completion_string);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_getCompletionPriority", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint getCompletionPriority([NativeTypeName("CXCompletionString")] void* completion_string);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_getCompletionAvailability", ExactSpelling = true)]
    [return: NativeTypeName("enum CXAvailabilityKind")]
    public static extern CXAvailabilityKind getCompletionAvailability([NativeTypeName("CXCompletionString")] void* completion_string);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_getCompletionNumAnnotations", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint getCompletionNumAnnotations([NativeTypeName("CXCompletionString")] void* completion_string);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_getCompletionAnnotation", ExactSpelling = true)]
    public static extern CXString getCompletionAnnotation([NativeTypeName("CXCompletionString")] void* completion_string, [NativeTypeName("unsigned int")] uint annotation_number);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_getCompletionParent", ExactSpelling = true)]
    public static extern CXString getCompletionParent([NativeTypeName("CXCompletionString")] void* completion_string, [NativeTypeName("enum CXCursorKind *")] CXCursorKind* kind);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_getCompletionBriefComment", ExactSpelling = true)]
    public static extern CXString getCompletionBriefComment([NativeTypeName("CXCompletionString")] void* completion_string);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_getCursorCompletionString", ExactSpelling = true)]
    [return: NativeTypeName("CXCompletionString")]
    public static extern void* getCursorCompletionString(CXCursor cursor);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_getCompletionNumFixIts", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint getCompletionNumFixIts(CXCodeCompleteResults* results, [NativeTypeName("unsigned int")] uint completion_index);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_getCompletionFixIt", ExactSpelling = true)]
    public static extern CXString getCompletionFixIt(CXCodeCompleteResults* results, [NativeTypeName("unsigned int")] uint completion_index, [NativeTypeName("unsigned int")] uint fixit_index, CXSourceRange* replacement_range);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_defaultCodeCompleteOptions", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint defaultCodeCompleteOptions();

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_codeCompleteAt", ExactSpelling = true)]
    public static extern CXCodeCompleteResults* codeCompleteAt([NativeTypeName("CXTranslationUnit")] CXTranslationUnitImpl* TU, [NativeTypeName("const char *")] sbyte* complete_filename, [NativeTypeName("unsigned int")] uint complete_line, [NativeTypeName("unsigned int")] uint complete_column, [NativeTypeName("struct CXUnsavedFile *")] CXUnsavedFile* unsaved_files, [NativeTypeName("unsigned int")] uint num_unsaved_files, [NativeTypeName("unsigned int")] uint options);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_sortCodeCompletionResults", ExactSpelling = true)]
    public static extern void sortCodeCompletionResults(CXCompletionResult* Results, [NativeTypeName("unsigned int")] uint NumResults);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_disposeCodeCompleteResults", ExactSpelling = true)]
    public static extern void disposeCodeCompleteResults(CXCodeCompleteResults* Results);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_codeCompleteGetNumDiagnostics", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint codeCompleteGetNumDiagnostics(CXCodeCompleteResults* Results);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_codeCompleteGetDiagnostic", ExactSpelling = true)]
    [return: NativeTypeName("CXDiagnostic")]
    public static extern void* codeCompleteGetDiagnostic(CXCodeCompleteResults* Results, [NativeTypeName("unsigned int")] uint Index);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_codeCompleteGetContexts", ExactSpelling = true)]
    [return: NativeTypeName("unsigned long long")]
    public static extern ulong codeCompleteGetContexts(CXCodeCompleteResults* Results);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_codeCompleteGetContainerKind", ExactSpelling = true)]
    [return: NativeTypeName("enum CXCursorKind")]
    public static extern CXCursorKind codeCompleteGetContainerKind(CXCodeCompleteResults* Results, [NativeTypeName("unsigned int *")] uint* IsIncomplete);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_codeCompleteGetContainerUSR", ExactSpelling = true)]
    public static extern CXString codeCompleteGetContainerUSR(CXCodeCompleteResults* Results);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_codeCompleteGetObjCSelector", ExactSpelling = true)]
    public static extern CXString codeCompleteGetObjCSelector(CXCodeCompleteResults* Results);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_getClangVersion", ExactSpelling = true)]
    public static extern CXString getClangVersion();

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_toggleCrashRecovery", ExactSpelling = true)]
    public static extern void toggleCrashRecovery([NativeTypeName("unsigned int")] uint isEnabled);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_getInclusions", ExactSpelling = true)]
    public static extern void getInclusions([NativeTypeName("CXTranslationUnit")] CXTranslationUnitImpl* tu, [NativeTypeName("CXInclusionVisitor")] delegate* unmanaged[Cdecl]<void*, CXSourceLocation*, uint, void*, void> visitor, [NativeTypeName("CXClientData")] void* client_data);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_Cursor_Evaluate", ExactSpelling = true)]
    [return: NativeTypeName("CXEvalResult")]
    public static extern void* Cursor_Evaluate(CXCursor C);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_EvalResult_getKind", ExactSpelling = true)]
    public static extern CXEvalResultKind EvalResult_getKind([NativeTypeName("CXEvalResult")] void* E);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_EvalResult_getAsInt", ExactSpelling = true)]
    public static extern int EvalResult_getAsInt([NativeTypeName("CXEvalResult")] void* E);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_EvalResult_getAsLongLong", ExactSpelling = true)]
    [return: NativeTypeName("long long")]
    public static extern long EvalResult_getAsLongLong([NativeTypeName("CXEvalResult")] void* E);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_EvalResult_isUnsignedInt", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint EvalResult_isUnsignedInt([NativeTypeName("CXEvalResult")] void* E);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_EvalResult_getAsUnsigned", ExactSpelling = true)]
    [return: NativeTypeName("unsigned long long")]
    public static extern ulong EvalResult_getAsUnsigned([NativeTypeName("CXEvalResult")] void* E);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_EvalResult_getAsDouble", ExactSpelling = true)]
    public static extern double EvalResult_getAsDouble([NativeTypeName("CXEvalResult")] void* E);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_EvalResult_getAsStr", ExactSpelling = true)]
    [return: NativeTypeName("const char *")]
    public static extern sbyte* EvalResult_getAsStr([NativeTypeName("CXEvalResult")] void* E);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_EvalResult_dispose", ExactSpelling = true)]
    public static extern void EvalResult_dispose([NativeTypeName("CXEvalResult")] void* E);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_getRemappings", ExactSpelling = true)]
    [return: NativeTypeName("CXRemapping")]
    public static extern void* getRemappings([NativeTypeName("const char *")] sbyte* path);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_getRemappingsFromFileList", ExactSpelling = true)]
    [return: NativeTypeName("CXRemapping")]
    public static extern void* getRemappingsFromFileList([NativeTypeName("const char **")] sbyte** filePaths, [NativeTypeName("unsigned int")] uint numFiles);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_remap_getNumFiles", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint remap_getNumFiles([NativeTypeName("CXRemapping")] void* param0);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_remap_getFilenames", ExactSpelling = true)]
    public static extern void remap_getFilenames([NativeTypeName("CXRemapping")] void* param0, [NativeTypeName("unsigned int")] uint index, CXString* original, CXString* transformed);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_remap_dispose", ExactSpelling = true)]
    public static extern void remap_dispose([NativeTypeName("CXRemapping")] void* param0);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_findReferencesInFile", ExactSpelling = true)]
    public static extern CXResult findReferencesInFile(CXCursor cursor, [NativeTypeName("CXFile")] void* file, CXCursorAndRangeVisitor visitor);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_findIncludesInFile", ExactSpelling = true)]
    public static extern CXResult findIncludesInFile([NativeTypeName("CXTranslationUnit")] CXTranslationUnitImpl* TU, [NativeTypeName("CXFile")] void* file, CXCursorAndRangeVisitor visitor);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_index_isEntityObjCContainerKind", ExactSpelling = true)]
    public static extern int index_isEntityObjCContainerKind(CXIdxEntityKind param0);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_index_getObjCContainerDeclInfo", ExactSpelling = true)]
    [return: NativeTypeName("const CXIdxObjCContainerDeclInfo *")]
    public static extern CXIdxObjCContainerDeclInfo* index_getObjCContainerDeclInfo([NativeTypeName("const CXIdxDeclInfo *")] CXIdxDeclInfo* param0);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_index_getObjCInterfaceDeclInfo", ExactSpelling = true)]
    [return: NativeTypeName("const CXIdxObjCInterfaceDeclInfo *")]
    public static extern CXIdxObjCInterfaceDeclInfo* index_getObjCInterfaceDeclInfo([NativeTypeName("const CXIdxDeclInfo *")] CXIdxDeclInfo* param0);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_index_getObjCCategoryDeclInfo", ExactSpelling = true)]
    [return: NativeTypeName("const CXIdxObjCCategoryDeclInfo *")]
    public static extern CXIdxObjCCategoryDeclInfo* index_getObjCCategoryDeclInfo([NativeTypeName("const CXIdxDeclInfo *")] CXIdxDeclInfo* param0);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_index_getObjCProtocolRefListInfo", ExactSpelling = true)]
    [return: NativeTypeName("const CXIdxObjCProtocolRefListInfo *")]
    public static extern CXIdxObjCProtocolRefListInfo* index_getObjCProtocolRefListInfo([NativeTypeName("const CXIdxDeclInfo *")] CXIdxDeclInfo* param0);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_index_getObjCPropertyDeclInfo", ExactSpelling = true)]
    [return: NativeTypeName("const CXIdxObjCPropertyDeclInfo *")]
    public static extern CXIdxObjCPropertyDeclInfo* index_getObjCPropertyDeclInfo([NativeTypeName("const CXIdxDeclInfo *")] CXIdxDeclInfo* param0);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_index_getIBOutletCollectionAttrInfo", ExactSpelling = true)]
    [return: NativeTypeName("const CXIdxIBOutletCollectionAttrInfo *")]
    public static extern CXIdxIBOutletCollectionAttrInfo* index_getIBOutletCollectionAttrInfo([NativeTypeName("const CXIdxAttrInfo *")] CXIdxAttrInfo* param0);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_index_getCXXClassDeclInfo", ExactSpelling = true)]
    [return: NativeTypeName("const CXIdxCXXClassDeclInfo *")]
    public static extern CXIdxCXXClassDeclInfo* index_getCXXClassDeclInfo([NativeTypeName("const CXIdxDeclInfo *")] CXIdxDeclInfo* param0);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_index_getClientContainer", ExactSpelling = true)]
    [return: NativeTypeName("CXIdxClientContainer")]
    public static extern void* index_getClientContainer([NativeTypeName("const CXIdxContainerInfo *")] CXIdxContainerInfo* param0);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_index_setClientContainer", ExactSpelling = true)]
    public static extern void index_setClientContainer([NativeTypeName("const CXIdxContainerInfo *")] CXIdxContainerInfo* param0, [NativeTypeName("CXIdxClientContainer")] void* param1);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_index_getClientEntity", ExactSpelling = true)]
    [return: NativeTypeName("CXIdxClientEntity")]
    public static extern void* index_getClientEntity([NativeTypeName("const CXIdxEntityInfo *")] CXIdxEntityInfo* param0);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_index_setClientEntity", ExactSpelling = true)]
    public static extern void index_setClientEntity([NativeTypeName("const CXIdxEntityInfo *")] CXIdxEntityInfo* param0, [NativeTypeName("CXIdxClientEntity")] void* param1);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_IndexAction_create", ExactSpelling = true)]
    [return: NativeTypeName("CXIndexAction")]
    public static extern void* IndexAction_create([NativeTypeName("CXIndex")] void* CIdx);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_IndexAction_dispose", ExactSpelling = true)]
    public static extern void IndexAction_dispose([NativeTypeName("CXIndexAction")] void* param0);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_indexSourceFile", ExactSpelling = true)]
    public static extern int indexSourceFile([NativeTypeName("CXIndexAction")] void* param0, [NativeTypeName("CXClientData")] void* client_data, IndexerCallbacks* index_callbacks, [NativeTypeName("unsigned int")] uint index_callbacks_size, [NativeTypeName("unsigned int")] uint index_options, [NativeTypeName("const char *")] sbyte* source_filename, [NativeTypeName("const char *const *")] sbyte** command_line_args, int num_command_line_args, [NativeTypeName("struct CXUnsavedFile *")] CXUnsavedFile* unsaved_files, [NativeTypeName("unsigned int")] uint num_unsaved_files, [NativeTypeName("CXTranslationUnit *")] CXTranslationUnitImpl** out_TU, [NativeTypeName("unsigned int")] uint TU_options);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_indexSourceFileFullArgv", ExactSpelling = true)]
    public static extern int indexSourceFileFullArgv([NativeTypeName("CXIndexAction")] void* param0, [NativeTypeName("CXClientData")] void* client_data, IndexerCallbacks* index_callbacks, [NativeTypeName("unsigned int")] uint index_callbacks_size, [NativeTypeName("unsigned int")] uint index_options, [NativeTypeName("const char *")] sbyte* source_filename, [NativeTypeName("const char *const *")] sbyte** command_line_args, int num_command_line_args, [NativeTypeName("struct CXUnsavedFile *")] CXUnsavedFile* unsaved_files, [NativeTypeName("unsigned int")] uint num_unsaved_files, [NativeTypeName("CXTranslationUnit *")] CXTranslationUnitImpl** out_TU, [NativeTypeName("unsigned int")] uint TU_options);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_indexTranslationUnit", ExactSpelling = true)]
    public static extern int indexTranslationUnit([NativeTypeName("CXIndexAction")] void* param0, [NativeTypeName("CXClientData")] void* client_data, IndexerCallbacks* index_callbacks, [NativeTypeName("unsigned int")] uint index_callbacks_size, [NativeTypeName("unsigned int")] uint index_options, [NativeTypeName("CXTranslationUnit")] CXTranslationUnitImpl* param5);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_indexLoc_getFileLocation", ExactSpelling = true)]
    public static extern void indexLoc_getFileLocation(CXIdxLoc loc, [NativeTypeName("CXIdxClientFile *")] void** indexFile, [NativeTypeName("CXFile *")] void** file, [NativeTypeName("unsigned int *")] uint* line, [NativeTypeName("unsigned int *")] uint* column, [NativeTypeName("unsigned int *")] uint* offset);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_indexLoc_getCXSourceLocation", ExactSpelling = true)]
    public static extern CXSourceLocation indexLoc_getCXSourceLocation(CXIdxLoc loc);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_Type_visitFields", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Type_visitFields(CXType T, [NativeTypeName("CXFieldVisitor")] delegate* unmanaged[Cdecl]<CXCursor, void*, CXVisitorResult> visitor, [NativeTypeName("CXClientData")] void* client_data);

    [NativeTypeName("#define CINDEX_VERSION_MAJOR 0")]
    public const int CINDEX_VERSION_MAJOR = 0;

    [NativeTypeName("#define CINDEX_VERSION_MINOR 62")]
    public const int CINDEX_VERSION_MINOR = 62;

    [NativeTypeName("#define CINDEX_VERSION CINDEX_VERSION_ENCODE(CINDEX_VERSION_MAJOR, CINDEX_VERSION_MINOR)")]
    public const int CINDEX_VERSION = (((0) * 10000) + ((62) * 1));

    [NativeTypeName("#define CINDEX_VERSION_STRING CINDEX_VERSION_STRINGIZE(CINDEX_VERSION_MAJOR, CINDEX_VERSION_MINOR)")]
    public static ReadOnlySpan<byte> CINDEX_VERSION_STRING => "0.62"u8;

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_CXRewriter_create", ExactSpelling = true)]
    [return: NativeTypeName("CXRewriter")]
    public static extern void* CXRewriter_create([NativeTypeName("CXTranslationUnit")] CXTranslationUnitImpl* TU);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_CXRewriter_insertTextBefore", ExactSpelling = true)]
    public static extern void CXRewriter_insertTextBefore([NativeTypeName("CXRewriter")] void* Rew, CXSourceLocation Loc, [NativeTypeName("const char *")] sbyte* Insert);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_CXRewriter_replaceText", ExactSpelling = true)]
    public static extern void CXRewriter_replaceText([NativeTypeName("CXRewriter")] void* Rew, CXSourceRange ToBeReplaced, [NativeTypeName("const char *")] sbyte* Replacement);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_CXRewriter_removeText", ExactSpelling = true)]
    public static extern void CXRewriter_removeText([NativeTypeName("CXRewriter")] void* Rew, CXSourceRange ToBeRemoved);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_CXRewriter_overwriteChangedFiles", ExactSpelling = true)]
    public static extern int CXRewriter_overwriteChangedFiles([NativeTypeName("CXRewriter")] void* Rew);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_CXRewriter_writeMainFileToStdOut", ExactSpelling = true)]
    public static extern void CXRewriter_writeMainFileToStdOut([NativeTypeName("CXRewriter")] void* Rew);

    [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clang_CXRewriter_dispose", ExactSpelling = true)]
    public static extern void CXRewriter_dispose([NativeTypeName("CXRewriter")] void* Rew);
}
