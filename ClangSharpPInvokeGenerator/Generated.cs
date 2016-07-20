namespace ClangSharp
{
    using System;
    using System.Runtime.InteropServices;

    public partial struct CXString
    {
        public IntPtr @data;
        public uint @private_flags;
    }

    public partial struct CXStringSet
    {
        public IntPtr @Strings;
        public uint @Count;
    }

    public partial struct CXVirtualFileOverlayImpl
    {
    }

    public partial struct CXModuleMapDescriptorImpl
    {
    }

    public partial struct CXTranslationUnitImpl
    {
    }

    public partial struct CXUnsavedFile
    {
        [MarshalAs(UnmanagedType.LPStr)] public string @Filename;
        [MarshalAs(UnmanagedType.LPStr)] public string @Contents;
        public int @Length;
    }

    public partial struct CXVersion
    {
        public int @Major;
        public int @Minor;
        public int @Subminor;
    }

    public partial struct CXFileUniqueID
    {
        public ulong @data0; public ulong @data1; public ulong @data2; 
    }

    public partial struct CXSourceLocation
    {
        public IntPtr @ptr_data0; public IntPtr @ptr_data1; 
        public uint @int_data;
    }

    public partial struct CXSourceRange
    {
        public IntPtr @ptr_data0; public IntPtr @ptr_data1; 
        public uint @begin_int_data;
        public uint @end_int_data;
    }

    public partial struct CXSourceRangeList
    {
        public uint @count;
        public IntPtr @ranges;
    }

    public partial struct CXTUResourceUsageEntry
    {
        public CXTUResourceUsageKind @kind;
        public int @amount;
    }

    public partial struct CXTUResourceUsage
    {
        public IntPtr @data;
        public uint @numEntries;
        public IntPtr @entries;
    }

    public partial struct CXCursor
    {
        public CXCursorKind @kind;
        public int @xdata;
        public IntPtr @data0; public IntPtr @data1; public IntPtr @data2; 
    }

    public partial struct CXPlatformAvailability
    {
        public CXString @Platform;
        public CXVersion @Introduced;
        public CXVersion @Deprecated;
        public CXVersion @Obsoleted;
        public int @Unavailable;
        public CXString @Message;
    }

    public partial struct CXCursorSetImpl
    {
    }

    public partial struct CXType
    {
        public CXTypeKind @kind;
        public IntPtr @data0; public IntPtr @data1; 
    }

    public partial struct CXToken
    {
        public uint @int_data0; public uint @int_data1; public uint @int_data2; public uint @int_data3; 
        public IntPtr @ptr_data;
    }

    public partial struct CXCompletionResult
    {
        public CXCursorKind @CursorKind;
        public IntPtr @CompletionString;
    }

    public partial struct CXCodeCompleteResults
    {
        public IntPtr @Results;
        public uint @NumResults;
    }

    public partial struct CXCursorAndRangeVisitor
    {
        public IntPtr @context;
        public IntPtr @visit;
    }

    public partial struct CXIdxLoc
    {
        public IntPtr @ptr_data0; public IntPtr @ptr_data1; 
        public uint @int_data;
    }

    public partial struct CXIdxIncludedFileInfo
    {
        public CXIdxLoc @hashLoc;
        [MarshalAs(UnmanagedType.LPStr)] public string @filename;
        public IntPtr @file;
        public int @isImport;
        public int @isAngled;
        public int @isModuleImport;
    }

    public partial struct CXIdxImportedASTFileInfo
    {
        public IntPtr @file;
        public IntPtr @module;
        public CXIdxLoc @loc;
        public int @isImplicit;
    }

    public partial struct CXIdxAttrInfo
    {
        public CXIdxAttrKind @kind;
        public CXCursor @cursor;
        public CXIdxLoc @loc;
    }

    public partial struct CXIdxEntityInfo
    {
        public CXIdxEntityKind @kind;
        public CXIdxEntityCXXTemplateKind @templateKind;
        public CXIdxEntityLanguage @lang;
        [MarshalAs(UnmanagedType.LPStr)] public string @name;
        [MarshalAs(UnmanagedType.LPStr)] public string @USR;
        public CXCursor @cursor;
        public IntPtr @attributes;
        public uint @numAttributes;
    }

    public partial struct CXIdxContainerInfo
    {
        public CXCursor @cursor;
    }

    public partial struct CXIdxIBOutletCollectionAttrInfo
    {
        public IntPtr @attrInfo;
        public IntPtr @objcClass;
        public CXCursor @classCursor;
        public CXIdxLoc @classLoc;
    }

    public partial struct CXIdxDeclInfo
    {
        public IntPtr @entityInfo;
        public CXCursor @cursor;
        public CXIdxLoc @loc;
        public IntPtr @semanticContainer;
        public IntPtr @lexicalContainer;
        public int @isRedeclaration;
        public int @isDefinition;
        public int @isContainer;
        public IntPtr @declAsContainer;
        public int @isImplicit;
        public IntPtr @attributes;
        public uint @numAttributes;
        public uint @flags;
    }

    public partial struct CXIdxObjCContainerDeclInfo
    {
        public IntPtr @declInfo;
        public CXIdxObjCContainerKind @kind;
    }

    public partial struct CXIdxBaseClassInfo
    {
        public IntPtr @base;
        public CXCursor @cursor;
        public CXIdxLoc @loc;
    }

    public partial struct CXIdxObjCProtocolRefInfo
    {
        public IntPtr @protocol;
        public CXCursor @cursor;
        public CXIdxLoc @loc;
    }

    public partial struct CXIdxObjCProtocolRefListInfo
    {
        public IntPtr @protocols;
        public uint @numProtocols;
    }

    public partial struct CXIdxObjCInterfaceDeclInfo
    {
        public IntPtr @containerInfo;
        public IntPtr @superInfo;
        public IntPtr @protocols;
    }

    public partial struct CXIdxObjCCategoryDeclInfo
    {
        public IntPtr @containerInfo;
        public IntPtr @objcClass;
        public CXCursor @classCursor;
        public CXIdxLoc @classLoc;
        public IntPtr @protocols;
    }

    public partial struct CXIdxObjCPropertyDeclInfo
    {
        public IntPtr @declInfo;
        public IntPtr @getter;
        public IntPtr @setter;
    }

    public partial struct CXIdxCXXClassDeclInfo
    {
        public IntPtr @declInfo;
        public IntPtr @bases;
        public uint @numBases;
    }

    public partial struct CXIdxEntityRefInfo
    {
        public CXIdxEntityRefKind @kind;
        public CXCursor @cursor;
        public CXIdxLoc @loc;
        public IntPtr @referencedEntity;
        public IntPtr @parentEntity;
        public IntPtr @container;
    }

    public partial struct IndexerCallbacks
    {
        public IntPtr @abortQuery;
        public IntPtr @diagnostic;
        public IntPtr @enteredMainFile;
        public IntPtr @ppIncludedFile;
        public IntPtr @importedASTFile;
        public IntPtr @startedTranslationUnit;
        public IntPtr @indexDeclaration;
        public IntPtr @indexEntityReference;
    }

    public partial struct CXComment
    {
        public IntPtr @ASTNode;
        public IntPtr @TranslationUnit;
    }

    public partial struct CXVirtualFileOverlay
    {
        public CXVirtualFileOverlay(IntPtr pointer)
        {
            this.Pointer = pointer;
        }

        public IntPtr Pointer;
    }

    public partial struct CXModuleMapDescriptor
    {
        public CXModuleMapDescriptor(IntPtr pointer)
        {
            this.Pointer = pointer;
        }

        public IntPtr Pointer;
    }

    public partial struct CXIndex
    {
        public CXIndex(IntPtr pointer)
        {
            this.Pointer = pointer;
        }

        public IntPtr Pointer;
    }

    public partial struct CXTranslationUnit
    {
        public CXTranslationUnit(IntPtr pointer)
        {
            this.Pointer = pointer;
        }

        public IntPtr Pointer;
    }

    public partial struct CXClientData
    {
        public CXClientData(IntPtr pointer)
        {
            this.Pointer = pointer;
        }

        public IntPtr Pointer;
    }

    public partial struct CXFile
    {
        public CXFile(IntPtr pointer)
        {
            this.Pointer = pointer;
        }

        public IntPtr Pointer;
    }

    public partial struct CXDiagnostic
    {
        public CXDiagnostic(IntPtr pointer)
        {
            this.Pointer = pointer;
        }

        public IntPtr Pointer;
    }

    public partial struct CXDiagnosticSet
    {
        public CXDiagnosticSet(IntPtr pointer)
        {
            this.Pointer = pointer;
        }

        public IntPtr Pointer;
    }

    public partial struct CXCursorSet
    {
        public CXCursorSet(IntPtr pointer)
        {
            this.Pointer = pointer;
        }

        public IntPtr Pointer;
    }

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate CXChildVisitResult CXCursorVisitor(CXCursor @cursor, CXCursor @parent, IntPtr @client_data);

    public partial struct CXModule
    {
        public CXModule(IntPtr pointer)
        {
            this.Pointer = pointer;
        }

        public IntPtr Pointer;
    }

    public partial struct CXCompletionString
    {
        public CXCompletionString(IntPtr pointer)
        {
            this.Pointer = pointer;
        }

        public IntPtr Pointer;
    }

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void CXInclusionVisitor(IntPtr @included_file, out CXSourceLocation @inclusion_stack, uint @include_len, IntPtr @client_data);

    public partial struct CXEvalResult
    {
        public CXEvalResult(IntPtr pointer)
        {
            this.Pointer = pointer;
        }

        public IntPtr Pointer;
    }

    public partial struct CXRemapping
    {
        public CXRemapping(IntPtr pointer)
        {
            this.Pointer = pointer;
        }

        public IntPtr Pointer;
    }

    public partial struct CXIdxClientFile
    {
        public CXIdxClientFile(IntPtr pointer)
        {
            this.Pointer = pointer;
        }

        public IntPtr Pointer;
    }

    public partial struct CXIdxClientEntity
    {
        public CXIdxClientEntity(IntPtr pointer)
        {
            this.Pointer = pointer;
        }

        public IntPtr Pointer;
    }

    public partial struct CXIdxClientContainer
    {
        public CXIdxClientContainer(IntPtr pointer)
        {
            this.Pointer = pointer;
        }

        public IntPtr Pointer;
    }

    public partial struct CXIdxClientASTFile
    {
        public CXIdxClientASTFile(IntPtr pointer)
        {
            this.Pointer = pointer;
        }

        public IntPtr Pointer;
    }

    public partial struct CXIndexAction
    {
        public CXIndexAction(IntPtr pointer)
        {
            this.Pointer = pointer;
        }

        public IntPtr Pointer;
    }

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate CXVisitorResult CXFieldVisitor(CXCursor @C, IntPtr @client_data);

    public partial struct CXCompilationDatabase
    {
        public CXCompilationDatabase(IntPtr pointer)
        {
            this.Pointer = pointer;
        }

        public IntPtr Pointer;
    }

    public partial struct CXCompileCommands
    {
        public CXCompileCommands(IntPtr pointer)
        {
            this.Pointer = pointer;
        }

        public IntPtr Pointer;
    }

    public partial struct CXCompileCommand
    {
        public CXCompileCommand(IntPtr pointer)
        {
            this.Pointer = pointer;
        }

        public IntPtr Pointer;
    }

    public enum CXErrorCode : int
    {
        @CXError_Success = 0,
        @CXError_Failure = 1,
        @CXError_Crashed = 2,
        @CXError_InvalidArguments = 3,
        @CXError_ASTReadError = 4,
    }

    public enum CXAvailabilityKind : int
    {
        @CXAvailability_Available = 0,
        @CXAvailability_Deprecated = 1,
        @CXAvailability_NotAvailable = 2,
        @CXAvailability_NotAccessible = 3,
    }

    public enum CXGlobalOptFlags : int
    {
        @CXGlobalOpt_None = 0,
        @CXGlobalOpt_ThreadBackgroundPriorityForIndexing = 1,
        @CXGlobalOpt_ThreadBackgroundPriorityForEditing = 2,
        @CXGlobalOpt_ThreadBackgroundPriorityForAll = 3,
    }

    public enum CXDiagnosticSeverity : int
    {
        @CXDiagnostic_Ignored = 0,
        @CXDiagnostic_Note = 1,
        @CXDiagnostic_Warning = 2,
        @CXDiagnostic_Error = 3,
        @CXDiagnostic_Fatal = 4,
    }

    public enum CXLoadDiag_Error : int
    {
        @CXLoadDiag_None = 0,
        @CXLoadDiag_Unknown = 1,
        @CXLoadDiag_CannotLoad = 2,
        @CXLoadDiag_InvalidFile = 3,
    }

    public enum CXDiagnosticDisplayOptions : int
    {
        @CXDiagnostic_DisplaySourceLocation = 1,
        @CXDiagnostic_DisplayColumn = 2,
        @CXDiagnostic_DisplaySourceRanges = 4,
        @CXDiagnostic_DisplayOption = 8,
        @CXDiagnostic_DisplayCategoryId = 16,
        @CXDiagnostic_DisplayCategoryName = 32,
    }

    public enum CXTranslationUnit_Flags : int
    {
        @CXTranslationUnit_None = 0,
        @CXTranslationUnit_DetailedPreprocessingRecord = 1,
        @CXTranslationUnit_Incomplete = 2,
        @CXTranslationUnit_PrecompiledPreamble = 4,
        @CXTranslationUnit_CacheCompletionResults = 8,
        @CXTranslationUnit_ForSerialization = 16,
        @CXTranslationUnit_CXXChainedPCH = 32,
        @CXTranslationUnit_SkipFunctionBodies = 64,
        @CXTranslationUnit_IncludeBriefCommentsInCodeCompletion = 128,
        @CXTranslationUnit_CreatePreambleOnFirstParse = 256,
        @CXTranslationUnit_KeepGoing = 512,
    }

    public enum CXSaveTranslationUnit_Flags : int
    {
        @CXSaveTranslationUnit_None = 0,
    }

    public enum CXSaveError : int
    {
        @CXSaveError_None = 0,
        @CXSaveError_Unknown = 1,
        @CXSaveError_TranslationErrors = 2,
        @CXSaveError_InvalidTU = 3,
    }

    public enum CXReparse_Flags : int
    {
        @CXReparse_None = 0,
    }

    public enum CXTUResourceUsageKind : int
    {
        @CXTUResourceUsage_AST = 1,
        @CXTUResourceUsage_Identifiers = 2,
        @CXTUResourceUsage_Selectors = 3,
        @CXTUResourceUsage_GlobalCompletionResults = 4,
        @CXTUResourceUsage_SourceManagerContentCache = 5,
        @CXTUResourceUsage_AST_SideTables = 6,
        @CXTUResourceUsage_SourceManager_Membuffer_Malloc = 7,
        @CXTUResourceUsage_SourceManager_Membuffer_MMap = 8,
        @CXTUResourceUsage_ExternalASTSource_Membuffer_Malloc = 9,
        @CXTUResourceUsage_ExternalASTSource_Membuffer_MMap = 10,
        @CXTUResourceUsage_Preprocessor = 11,
        @CXTUResourceUsage_PreprocessingRecord = 12,
        @CXTUResourceUsage_SourceManager_DataStructures = 13,
        @CXTUResourceUsage_Preprocessor_HeaderSearch = 14,
        @CXTUResourceUsage_MEMORY_IN_BYTES_BEGIN = 1,
        @CXTUResourceUsage_MEMORY_IN_BYTES_END = 14,
        @CXTUResourceUsage_First = 1,
        @CXTUResourceUsage_Last = 14,
    }

    public enum CXCursorKind : int
    {
        @CXCursor_UnexposedDecl = 1,
        @CXCursor_StructDecl = 2,
        @CXCursor_UnionDecl = 3,
        @CXCursor_ClassDecl = 4,
        @CXCursor_EnumDecl = 5,
        @CXCursor_FieldDecl = 6,
        @CXCursor_EnumConstantDecl = 7,
        @CXCursor_FunctionDecl = 8,
        @CXCursor_VarDecl = 9,
        @CXCursor_ParmDecl = 10,
        @CXCursor_ObjCInterfaceDecl = 11,
        @CXCursor_ObjCCategoryDecl = 12,
        @CXCursor_ObjCProtocolDecl = 13,
        @CXCursor_ObjCPropertyDecl = 14,
        @CXCursor_ObjCIvarDecl = 15,
        @CXCursor_ObjCInstanceMethodDecl = 16,
        @CXCursor_ObjCClassMethodDecl = 17,
        @CXCursor_ObjCImplementationDecl = 18,
        @CXCursor_ObjCCategoryImplDecl = 19,
        @CXCursor_TypedefDecl = 20,
        @CXCursor_CXXMethod = 21,
        @CXCursor_Namespace = 22,
        @CXCursor_LinkageSpec = 23,
        @CXCursor_Constructor = 24,
        @CXCursor_Destructor = 25,
        @CXCursor_ConversionFunction = 26,
        @CXCursor_TemplateTypeParameter = 27,
        @CXCursor_NonTypeTemplateParameter = 28,
        @CXCursor_TemplateTemplateParameter = 29,
        @CXCursor_FunctionTemplate = 30,
        @CXCursor_ClassTemplate = 31,
        @CXCursor_ClassTemplatePartialSpecialization = 32,
        @CXCursor_NamespaceAlias = 33,
        @CXCursor_UsingDirective = 34,
        @CXCursor_UsingDeclaration = 35,
        @CXCursor_TypeAliasDecl = 36,
        @CXCursor_ObjCSynthesizeDecl = 37,
        @CXCursor_ObjCDynamicDecl = 38,
        @CXCursor_CXXAccessSpecifier = 39,
        @CXCursor_FirstDecl = 1,
        @CXCursor_LastDecl = 39,
        @CXCursor_FirstRef = 40,
        @CXCursor_ObjCSuperClassRef = 40,
        @CXCursor_ObjCProtocolRef = 41,
        @CXCursor_ObjCClassRef = 42,
        @CXCursor_TypeRef = 43,
        @CXCursor_CXXBaseSpecifier = 44,
        @CXCursor_TemplateRef = 45,
        @CXCursor_NamespaceRef = 46,
        @CXCursor_MemberRef = 47,
        @CXCursor_LabelRef = 48,
        @CXCursor_OverloadedDeclRef = 49,
        @CXCursor_VariableRef = 50,
        @CXCursor_LastRef = 50,
        @CXCursor_FirstInvalid = 70,
        @CXCursor_InvalidFile = 70,
        @CXCursor_NoDeclFound = 71,
        @CXCursor_NotImplemented = 72,
        @CXCursor_InvalidCode = 73,
        @CXCursor_LastInvalid = 73,
        @CXCursor_FirstExpr = 100,
        @CXCursor_UnexposedExpr = 100,
        @CXCursor_DeclRefExpr = 101,
        @CXCursor_MemberRefExpr = 102,
        @CXCursor_CallExpr = 103,
        @CXCursor_ObjCMessageExpr = 104,
        @CXCursor_BlockExpr = 105,
        @CXCursor_IntegerLiteral = 106,
        @CXCursor_FloatingLiteral = 107,
        @CXCursor_ImaginaryLiteral = 108,
        @CXCursor_StringLiteral = 109,
        @CXCursor_CharacterLiteral = 110,
        @CXCursor_ParenExpr = 111,
        @CXCursor_UnaryOperator = 112,
        @CXCursor_ArraySubscriptExpr = 113,
        @CXCursor_BinaryOperator = 114,
        @CXCursor_CompoundAssignOperator = 115,
        @CXCursor_ConditionalOperator = 116,
        @CXCursor_CStyleCastExpr = 117,
        @CXCursor_CompoundLiteralExpr = 118,
        @CXCursor_InitListExpr = 119,
        @CXCursor_AddrLabelExpr = 120,
        @CXCursor_StmtExpr = 121,
        @CXCursor_GenericSelectionExpr = 122,
        @CXCursor_GNUNullExpr = 123,
        @CXCursor_CXXStaticCastExpr = 124,
        @CXCursor_CXXDynamicCastExpr = 125,
        @CXCursor_CXXReinterpretCastExpr = 126,
        @CXCursor_CXXConstCastExpr = 127,
        @CXCursor_CXXFunctionalCastExpr = 128,
        @CXCursor_CXXTypeidExpr = 129,
        @CXCursor_CXXBoolLiteralExpr = 130,
        @CXCursor_CXXNullPtrLiteralExpr = 131,
        @CXCursor_CXXThisExpr = 132,
        @CXCursor_CXXThrowExpr = 133,
        @CXCursor_CXXNewExpr = 134,
        @CXCursor_CXXDeleteExpr = 135,
        @CXCursor_UnaryExpr = 136,
        @CXCursor_ObjCStringLiteral = 137,
        @CXCursor_ObjCEncodeExpr = 138,
        @CXCursor_ObjCSelectorExpr = 139,
        @CXCursor_ObjCProtocolExpr = 140,
        @CXCursor_ObjCBridgedCastExpr = 141,
        @CXCursor_PackExpansionExpr = 142,
        @CXCursor_SizeOfPackExpr = 143,
        @CXCursor_LambdaExpr = 144,
        @CXCursor_ObjCBoolLiteralExpr = 145,
        @CXCursor_ObjCSelfExpr = 146,
        @CXCursor_OMPArraySectionExpr = 147,
        @CXCursor_ObjCAvailabilityCheckExpr = 148,
        @CXCursor_LastExpr = 148,
        @CXCursor_FirstStmt = 200,
        @CXCursor_UnexposedStmt = 200,
        @CXCursor_LabelStmt = 201,
        @CXCursor_CompoundStmt = 202,
        @CXCursor_CaseStmt = 203,
        @CXCursor_DefaultStmt = 204,
        @CXCursor_IfStmt = 205,
        @CXCursor_SwitchStmt = 206,
        @CXCursor_WhileStmt = 207,
        @CXCursor_DoStmt = 208,
        @CXCursor_ForStmt = 209,
        @CXCursor_GotoStmt = 210,
        @CXCursor_IndirectGotoStmt = 211,
        @CXCursor_ContinueStmt = 212,
        @CXCursor_BreakStmt = 213,
        @CXCursor_ReturnStmt = 214,
        @CXCursor_GCCAsmStmt = 215,
        @CXCursor_AsmStmt = 215,
        @CXCursor_ObjCAtTryStmt = 216,
        @CXCursor_ObjCAtCatchStmt = 217,
        @CXCursor_ObjCAtFinallyStmt = 218,
        @CXCursor_ObjCAtThrowStmt = 219,
        @CXCursor_ObjCAtSynchronizedStmt = 220,
        @CXCursor_ObjCAutoreleasePoolStmt = 221,
        @CXCursor_ObjCForCollectionStmt = 222,
        @CXCursor_CXXCatchStmt = 223,
        @CXCursor_CXXTryStmt = 224,
        @CXCursor_CXXForRangeStmt = 225,
        @CXCursor_SEHTryStmt = 226,
        @CXCursor_SEHExceptStmt = 227,
        @CXCursor_SEHFinallyStmt = 228,
        @CXCursor_MSAsmStmt = 229,
        @CXCursor_NullStmt = 230,
        @CXCursor_DeclStmt = 231,
        @CXCursor_OMPParallelDirective = 232,
        @CXCursor_OMPSimdDirective = 233,
        @CXCursor_OMPForDirective = 234,
        @CXCursor_OMPSectionsDirective = 235,
        @CXCursor_OMPSectionDirective = 236,
        @CXCursor_OMPSingleDirective = 237,
        @CXCursor_OMPParallelForDirective = 238,
        @CXCursor_OMPParallelSectionsDirective = 239,
        @CXCursor_OMPTaskDirective = 240,
        @CXCursor_OMPMasterDirective = 241,
        @CXCursor_OMPCriticalDirective = 242,
        @CXCursor_OMPTaskyieldDirective = 243,
        @CXCursor_OMPBarrierDirective = 244,
        @CXCursor_OMPTaskwaitDirective = 245,
        @CXCursor_OMPFlushDirective = 246,
        @CXCursor_SEHLeaveStmt = 247,
        @CXCursor_OMPOrderedDirective = 248,
        @CXCursor_OMPAtomicDirective = 249,
        @CXCursor_OMPForSimdDirective = 250,
        @CXCursor_OMPParallelForSimdDirective = 251,
        @CXCursor_OMPTargetDirective = 252,
        @CXCursor_OMPTeamsDirective = 253,
        @CXCursor_OMPTaskgroupDirective = 254,
        @CXCursor_OMPCancellationPointDirective = 255,
        @CXCursor_OMPCancelDirective = 256,
        @CXCursor_OMPTargetDataDirective = 257,
        @CXCursor_OMPTaskLoopDirective = 258,
        @CXCursor_OMPTaskLoopSimdDirective = 259,
        @CXCursor_OMPDistributeDirective = 260,
        @CXCursor_OMPTargetEnterDataDirective = 261,
        @CXCursor_OMPTargetExitDataDirective = 262,
        @CXCursor_OMPTargetParallelDirective = 263,
        @CXCursor_OMPTargetParallelForDirective = 264,
        @CXCursor_OMPTargetUpdateDirective = 265,
        @CXCursor_OMPDistributeParallelForDirective = 266,
        @CXCursor_OMPDistributeParallelForSimdDirective = 267,
        @CXCursor_OMPDistributeSimdDirective = 268,
        @CXCursor_OMPTargetParallelForSimdDirective = 269,
        @CXCursor_LastStmt = 269,
        @CXCursor_TranslationUnit = 300,
        @CXCursor_FirstAttr = 400,
        @CXCursor_UnexposedAttr = 400,
        @CXCursor_IBActionAttr = 401,
        @CXCursor_IBOutletAttr = 402,
        @CXCursor_IBOutletCollectionAttr = 403,
        @CXCursor_CXXFinalAttr = 404,
        @CXCursor_CXXOverrideAttr = 405,
        @CXCursor_AnnotateAttr = 406,
        @CXCursor_AsmLabelAttr = 407,
        @CXCursor_PackedAttr = 408,
        @CXCursor_PureAttr = 409,
        @CXCursor_ConstAttr = 410,
        @CXCursor_NoDuplicateAttr = 411,
        @CXCursor_CUDAConstantAttr = 412,
        @CXCursor_CUDADeviceAttr = 413,
        @CXCursor_CUDAGlobalAttr = 414,
        @CXCursor_CUDAHostAttr = 415,
        @CXCursor_CUDASharedAttr = 416,
        @CXCursor_VisibilityAttr = 417,
        @CXCursor_DLLExport = 418,
        @CXCursor_DLLImport = 419,
        @CXCursor_LastAttr = 419,
        @CXCursor_PreprocessingDirective = 500,
        @CXCursor_MacroDefinition = 501,
        @CXCursor_MacroExpansion = 502,
        @CXCursor_MacroInstantiation = 502,
        @CXCursor_InclusionDirective = 503,
        @CXCursor_FirstPreprocessing = 500,
        @CXCursor_LastPreprocessing = 503,
        @CXCursor_ModuleImportDecl = 600,
        @CXCursor_TypeAliasTemplateDecl = 601,
        @CXCursor_StaticAssert = 602,
        @CXCursor_FirstExtraDecl = 600,
        @CXCursor_LastExtraDecl = 602,
        @CXCursor_OverloadCandidate = 700,
    }

    public enum CXLinkageKind : int
    {
        @CXLinkage_Invalid = 0,
        @CXLinkage_NoLinkage = 1,
        @CXLinkage_Internal = 2,
        @CXLinkage_UniqueExternal = 3,
        @CXLinkage_External = 4,
    }

    public enum CXVisibilityKind : int
    {
        @CXVisibility_Invalid = 0,
        @CXVisibility_Hidden = 1,
        @CXVisibility_Protected = 2,
        @CXVisibility_Default = 3,
    }

    public enum CXLanguageKind : int
    {
        @CXLanguage_Invalid = 0,
        @CXLanguage_C = 1,
        @CXLanguage_ObjC = 2,
        @CXLanguage_CPlusPlus = 3,
    }

    public enum CXTypeKind : int
    {
        @CXType_Invalid = 0,
        @CXType_Unexposed = 1,
        @CXType_Void = 2,
        @CXType_Bool = 3,
        @CXType_Char_U = 4,
        @CXType_UChar = 5,
        @CXType_Char16 = 6,
        @CXType_Char32 = 7,
        @CXType_UShort = 8,
        @CXType_UInt = 9,
        @CXType_ULong = 10,
        @CXType_ULongLong = 11,
        @CXType_UInt128 = 12,
        @CXType_Char_S = 13,
        @CXType_SChar = 14,
        @CXType_WChar = 15,
        @CXType_Short = 16,
        @CXType_Int = 17,
        @CXType_Long = 18,
        @CXType_LongLong = 19,
        @CXType_Int128 = 20,
        @CXType_Float = 21,
        @CXType_Double = 22,
        @CXType_LongDouble = 23,
        @CXType_NullPtr = 24,
        @CXType_Overload = 25,
        @CXType_Dependent = 26,
        @CXType_ObjCId = 27,
        @CXType_ObjCClass = 28,
        @CXType_ObjCSel = 29,
        @CXType_Float128 = 30,
        @CXType_FirstBuiltin = 2,
        @CXType_LastBuiltin = 29,
        @CXType_Complex = 100,
        @CXType_Pointer = 101,
        @CXType_BlockPointer = 102,
        @CXType_LValueReference = 103,
        @CXType_RValueReference = 104,
        @CXType_Record = 105,
        @CXType_Enum = 106,
        @CXType_Typedef = 107,
        @CXType_ObjCInterface = 108,
        @CXType_ObjCObjectPointer = 109,
        @CXType_FunctionNoProto = 110,
        @CXType_FunctionProto = 111,
        @CXType_ConstantArray = 112,
        @CXType_Vector = 113,
        @CXType_IncompleteArray = 114,
        @CXType_VariableArray = 115,
        @CXType_DependentSizedArray = 116,
        @CXType_MemberPointer = 117,
        @CXType_Auto = 118,
        @CXType_Elaborated = 119,
    }

    public enum CXCallingConv : int
    {
        @CXCallingConv_Default = 0,
        @CXCallingConv_C = 1,
        @CXCallingConv_X86StdCall = 2,
        @CXCallingConv_X86FastCall = 3,
        @CXCallingConv_X86ThisCall = 4,
        @CXCallingConv_X86Pascal = 5,
        @CXCallingConv_AAPCS = 6,
        @CXCallingConv_AAPCS_VFP = 7,
        @CXCallingConv_IntelOclBicc = 9,
        @CXCallingConv_X86_64Win64 = 10,
        @CXCallingConv_X86_64SysV = 11,
        @CXCallingConv_X86VectorCall = 12,
        @CXCallingConv_Swift = 13,
        @CXCallingConv_PreserveMost = 14,
        @CXCallingConv_PreserveAll = 15,
        @CXCallingConv_Invalid = 100,
        @CXCallingConv_Unexposed = 200,
    }

    public enum CXTemplateArgumentKind : int
    {
        @CXTemplateArgumentKind_Null = 0,
        @CXTemplateArgumentKind_Type = 1,
        @CXTemplateArgumentKind_Declaration = 2,
        @CXTemplateArgumentKind_NullPtr = 3,
        @CXTemplateArgumentKind_Integral = 4,
        @CXTemplateArgumentKind_Template = 5,
        @CXTemplateArgumentKind_TemplateExpansion = 6,
        @CXTemplateArgumentKind_Expression = 7,
        @CXTemplateArgumentKind_Pack = 8,
        @CXTemplateArgumentKind_Invalid = 9,
    }

    public enum CXTypeLayoutError : int
    {
        @CXTypeLayoutError_Invalid = -1,
        @CXTypeLayoutError_Incomplete = -2,
        @CXTypeLayoutError_Dependent = -3,
        @CXTypeLayoutError_NotConstantSize = -4,
        @CXTypeLayoutError_InvalidFieldName = -5,
    }

    public enum CXRefQualifierKind : int
    {
        @CXRefQualifier_None = 0,
        @CXRefQualifier_LValue = 1,
        @CXRefQualifier_RValue = 2,
    }

    public enum CX_CXXAccessSpecifier : int
    {
        @CX_CXXInvalidAccessSpecifier = 0,
        @CX_CXXPublic = 1,
        @CX_CXXProtected = 2,
        @CX_CXXPrivate = 3,
    }

    public enum CX_StorageClass : int
    {
        @CX_SC_Invalid = 0,
        @CX_SC_None = 1,
        @CX_SC_Extern = 2,
        @CX_SC_Static = 3,
        @CX_SC_PrivateExtern = 4,
        @CX_SC_OpenCLWorkGroupLocal = 5,
        @CX_SC_Auto = 6,
        @CX_SC_Register = 7,
    }

    public enum CXChildVisitResult : int
    {
        @CXChildVisit_Break = 0,
        @CXChildVisit_Continue = 1,
        @CXChildVisit_Recurse = 2,
    }

    public enum CXObjCPropertyAttrKind : int
    {
        @CXObjCPropertyAttr_noattr = 0,
        @CXObjCPropertyAttr_readonly = 1,
        @CXObjCPropertyAttr_getter = 2,
        @CXObjCPropertyAttr_assign = 4,
        @CXObjCPropertyAttr_readwrite = 8,
        @CXObjCPropertyAttr_retain = 16,
        @CXObjCPropertyAttr_copy = 32,
        @CXObjCPropertyAttr_nonatomic = 64,
        @CXObjCPropertyAttr_setter = 128,
        @CXObjCPropertyAttr_atomic = 256,
        @CXObjCPropertyAttr_weak = 512,
        @CXObjCPropertyAttr_strong = 1024,
        @CXObjCPropertyAttr_unsafe_unretained = 2048,
        @CXObjCPropertyAttr_class = 4096,
    }

    public enum CXObjCDeclQualifierKind : int
    {
        @CXObjCDeclQualifier_None = 0,
        @CXObjCDeclQualifier_In = 1,
        @CXObjCDeclQualifier_Inout = 2,
        @CXObjCDeclQualifier_Out = 4,
        @CXObjCDeclQualifier_Bycopy = 8,
        @CXObjCDeclQualifier_Byref = 16,
        @CXObjCDeclQualifier_Oneway = 32,
    }

    public enum CXNameRefFlags : int
    {
        @CXNameRange_WantQualifier = 1,
        @CXNameRange_WantTemplateArgs = 2,
        @CXNameRange_WantSinglePiece = 4,
    }

    public enum CXTokenKind : int
    {
        @CXToken_Punctuation = 0,
        @CXToken_Keyword = 1,
        @CXToken_Identifier = 2,
        @CXToken_Literal = 3,
        @CXToken_Comment = 4,
    }

    public enum CXCompletionChunkKind : int
    {
        @CXCompletionChunk_Optional = 0,
        @CXCompletionChunk_TypedText = 1,
        @CXCompletionChunk_Text = 2,
        @CXCompletionChunk_Placeholder = 3,
        @CXCompletionChunk_Informative = 4,
        @CXCompletionChunk_CurrentParameter = 5,
        @CXCompletionChunk_LeftParen = 6,
        @CXCompletionChunk_RightParen = 7,
        @CXCompletionChunk_LeftBracket = 8,
        @CXCompletionChunk_RightBracket = 9,
        @CXCompletionChunk_LeftBrace = 10,
        @CXCompletionChunk_RightBrace = 11,
        @CXCompletionChunk_LeftAngle = 12,
        @CXCompletionChunk_RightAngle = 13,
        @CXCompletionChunk_Comma = 14,
        @CXCompletionChunk_ResultType = 15,
        @CXCompletionChunk_Colon = 16,
        @CXCompletionChunk_SemiColon = 17,
        @CXCompletionChunk_Equal = 18,
        @CXCompletionChunk_HorizontalSpace = 19,
        @CXCompletionChunk_VerticalSpace = 20,
    }

    public enum CXCodeComplete_Flags : int
    {
        @CXCodeComplete_IncludeMacros = 1,
        @CXCodeComplete_IncludeCodePatterns = 2,
        @CXCodeComplete_IncludeBriefComments = 4,
    }

    public enum CXCompletionContext : int
    {
        @CXCompletionContext_Unexposed = 0,
        @CXCompletionContext_AnyType = 1,
        @CXCompletionContext_AnyValue = 2,
        @CXCompletionContext_ObjCObjectValue = 4,
        @CXCompletionContext_ObjCSelectorValue = 8,
        @CXCompletionContext_CXXClassTypeValue = 16,
        @CXCompletionContext_DotMemberAccess = 32,
        @CXCompletionContext_ArrowMemberAccess = 64,
        @CXCompletionContext_ObjCPropertyAccess = 128,
        @CXCompletionContext_EnumTag = 256,
        @CXCompletionContext_UnionTag = 512,
        @CXCompletionContext_StructTag = 1024,
        @CXCompletionContext_ClassTag = 2048,
        @CXCompletionContext_Namespace = 4096,
        @CXCompletionContext_NestedNameSpecifier = 8192,
        @CXCompletionContext_ObjCInterface = 16384,
        @CXCompletionContext_ObjCProtocol = 32768,
        @CXCompletionContext_ObjCCategory = 65536,
        @CXCompletionContext_ObjCInstanceMessage = 131072,
        @CXCompletionContext_ObjCClassMessage = 262144,
        @CXCompletionContext_ObjCSelectorName = 524288,
        @CXCompletionContext_MacroName = 1048576,
        @CXCompletionContext_NaturalLanguage = 2097152,
        @CXCompletionContext_Unknown = 4194303,
    }

    public enum CXEvalResultKind : int
    {
        @CXEval_Int = 1,
        @CXEval_Float = 2,
        @CXEval_ObjCStrLiteral = 3,
        @CXEval_StrLiteral = 4,
        @CXEval_CFStr = 5,
        @CXEval_Other = 6,
        @CXEval_UnExposed = 0,
    }

    public enum CXVisitorResult : int
    {
        @CXVisit_Break = 0,
        @CXVisit_Continue = 1,
    }

    public enum CXResult : int
    {
        @CXResult_Success = 0,
        @CXResult_Invalid = 1,
        @CXResult_VisitBreak = 2,
    }

    public enum CXIdxEntityKind : int
    {
        @CXIdxEntity_Unexposed = 0,
        @CXIdxEntity_Typedef = 1,
        @CXIdxEntity_Function = 2,
        @CXIdxEntity_Variable = 3,
        @CXIdxEntity_Field = 4,
        @CXIdxEntity_EnumConstant = 5,
        @CXIdxEntity_ObjCClass = 6,
        @CXIdxEntity_ObjCProtocol = 7,
        @CXIdxEntity_ObjCCategory = 8,
        @CXIdxEntity_ObjCInstanceMethod = 9,
        @CXIdxEntity_ObjCClassMethod = 10,
        @CXIdxEntity_ObjCProperty = 11,
        @CXIdxEntity_ObjCIvar = 12,
        @CXIdxEntity_Enum = 13,
        @CXIdxEntity_Struct = 14,
        @CXIdxEntity_Union = 15,
        @CXIdxEntity_CXXClass = 16,
        @CXIdxEntity_CXXNamespace = 17,
        @CXIdxEntity_CXXNamespaceAlias = 18,
        @CXIdxEntity_CXXStaticVariable = 19,
        @CXIdxEntity_CXXStaticMethod = 20,
        @CXIdxEntity_CXXInstanceMethod = 21,
        @CXIdxEntity_CXXConstructor = 22,
        @CXIdxEntity_CXXDestructor = 23,
        @CXIdxEntity_CXXConversionFunction = 24,
        @CXIdxEntity_CXXTypeAlias = 25,
        @CXIdxEntity_CXXInterface = 26,
    }

    public enum CXIdxEntityLanguage : int
    {
        @CXIdxEntityLang_None = 0,
        @CXIdxEntityLang_C = 1,
        @CXIdxEntityLang_ObjC = 2,
        @CXIdxEntityLang_CXX = 3,
    }

    public enum CXIdxEntityCXXTemplateKind : int
    {
        @CXIdxEntity_NonTemplate = 0,
        @CXIdxEntity_Template = 1,
        @CXIdxEntity_TemplatePartialSpecialization = 2,
        @CXIdxEntity_TemplateSpecialization = 3,
    }

    public enum CXIdxAttrKind : int
    {
        @CXIdxAttr_Unexposed = 0,
        @CXIdxAttr_IBAction = 1,
        @CXIdxAttr_IBOutlet = 2,
        @CXIdxAttr_IBOutletCollection = 3,
    }

    public enum CXIdxDeclInfoFlags : int
    {
        @CXIdxDeclFlag_Skipped = 1,
    }

    public enum CXIdxObjCContainerKind : int
    {
        @CXIdxObjCContainer_ForwardRef = 0,
        @CXIdxObjCContainer_Interface = 1,
        @CXIdxObjCContainer_Implementation = 2,
    }

    public enum CXIdxEntityRefKind : int
    {
        @CXIdxEntityRef_Direct = 1,
        @CXIdxEntityRef_Implicit = 2,
    }

    public enum CXIndexOptFlags : int
    {
        @CXIndexOpt_None = 0,
        @CXIndexOpt_SuppressRedundantRefs = 1,
        @CXIndexOpt_IndexFunctionLocalSymbols = 2,
        @CXIndexOpt_IndexImplicitTemplateInstantiations = 4,
        @CXIndexOpt_SuppressWarnings = 8,
        @CXIndexOpt_SkipParsedBodiesInSession = 16,
    }

    public enum CXCommentKind : int
    {
        @CXComment_Null = 0,
        @CXComment_Text = 1,
        @CXComment_InlineCommand = 2,
        @CXComment_HTMLStartTag = 3,
        @CXComment_HTMLEndTag = 4,
        @CXComment_Paragraph = 5,
        @CXComment_BlockCommand = 6,
        @CXComment_ParamCommand = 7,
        @CXComment_TParamCommand = 8,
        @CXComment_VerbatimBlockCommand = 9,
        @CXComment_VerbatimBlockLine = 10,
        @CXComment_VerbatimLine = 11,
        @CXComment_FullComment = 12,
    }

    public enum CXCommentInlineCommandRenderKind : int
    {
        @CXCommentInlineCommandRenderKind_Normal = 0,
        @CXCommentInlineCommandRenderKind_Bold = 1,
        @CXCommentInlineCommandRenderKind_Monospaced = 2,
        @CXCommentInlineCommandRenderKind_Emphasized = 3,
    }

    public enum CXCommentParamPassDirection : int
    {
        @CXCommentParamPassDirection_In = 0,
        @CXCommentParamPassDirection_Out = 1,
        @CXCommentParamPassDirection_InOut = 2,
    }

    public enum CXCompilationDatabase_Error : int
    {
        @CXCompilationDatabase_NoError = 0,
        @CXCompilationDatabase_CanNotLoadDatabase = 1,
    }

    public static partial class clang
    {
        private const string libraryPath = "libclang";

        [DllImport(libraryPath, EntryPoint = "clang_getCString", CallingConvention = CallingConvention.Cdecl)]
        public static extern string getCString(CXString @string);

        [DllImport(libraryPath, EntryPoint = "clang_disposeString", CallingConvention = CallingConvention.Cdecl)]
        public static extern void disposeString(CXString @string);

        [DllImport(libraryPath, EntryPoint = "clang_disposeStringSet", CallingConvention = CallingConvention.Cdecl)]
        public static extern void disposeStringSet(ref CXStringSet @set);

        [DllImport(libraryPath, EntryPoint = "clang_getBuildSessionTimestamp", CallingConvention = CallingConvention.Cdecl)]
        public static extern ulong getBuildSessionTimestamp();

        [DllImport(libraryPath, EntryPoint = "clang_VirtualFileOverlay_create", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXVirtualFileOverlay VirtualFileOverlay_create(uint @options);

        [DllImport(libraryPath, EntryPoint = "clang_VirtualFileOverlay_addFileMapping", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXErrorCode VirtualFileOverlay_addFileMapping(CXVirtualFileOverlay @param0, [MarshalAs(UnmanagedType.LPStr)] string @virtualPath, [MarshalAs(UnmanagedType.LPStr)] string @realPath);

        [DllImport(libraryPath, EntryPoint = "clang_VirtualFileOverlay_setCaseSensitivity", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXErrorCode VirtualFileOverlay_setCaseSensitivity(CXVirtualFileOverlay @param0, int @caseSensitive);

        [DllImport(libraryPath, EntryPoint = "clang_VirtualFileOverlay_writeToBuffer", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXErrorCode VirtualFileOverlay_writeToBuffer(CXVirtualFileOverlay @param0, uint @options, out IntPtr @out_buffer_ptr, out uint @out_buffer_size);

        [DllImport(libraryPath, EntryPoint = "clang_free", CallingConvention = CallingConvention.Cdecl)]
        public static extern void free(IntPtr @buffer);

        [DllImport(libraryPath, EntryPoint = "clang_VirtualFileOverlay_dispose", CallingConvention = CallingConvention.Cdecl)]
        public static extern void VirtualFileOverlay_dispose(CXVirtualFileOverlay @param0);

        [DllImport(libraryPath, EntryPoint = "clang_ModuleMapDescriptor_create", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXModuleMapDescriptor ModuleMapDescriptor_create(uint @options);

        [DllImport(libraryPath, EntryPoint = "clang_ModuleMapDescriptor_setFrameworkModuleName", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXErrorCode ModuleMapDescriptor_setFrameworkModuleName(CXModuleMapDescriptor @param0, [MarshalAs(UnmanagedType.LPStr)] string @name);

        [DllImport(libraryPath, EntryPoint = "clang_ModuleMapDescriptor_setUmbrellaHeader", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXErrorCode ModuleMapDescriptor_setUmbrellaHeader(CXModuleMapDescriptor @param0, [MarshalAs(UnmanagedType.LPStr)] string @name);

        [DllImport(libraryPath, EntryPoint = "clang_ModuleMapDescriptor_writeToBuffer", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXErrorCode ModuleMapDescriptor_writeToBuffer(CXModuleMapDescriptor @param0, uint @options, out IntPtr @out_buffer_ptr, out uint @out_buffer_size);

        [DllImport(libraryPath, EntryPoint = "clang_ModuleMapDescriptor_dispose", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ModuleMapDescriptor_dispose(CXModuleMapDescriptor @param0);

        [DllImport(libraryPath, EntryPoint = "clang_createIndex", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXIndex createIndex(int @excludeDeclarationsFromPCH, int @displayDiagnostics);

        [DllImport(libraryPath, EntryPoint = "clang_disposeIndex", CallingConvention = CallingConvention.Cdecl)]
        public static extern void disposeIndex(CXIndex @index);

        [DllImport(libraryPath, EntryPoint = "clang_CXIndex_setGlobalOptions", CallingConvention = CallingConvention.Cdecl)]
        public static extern void CXIndex_setGlobalOptions(CXIndex @param0, uint @options);

        [DllImport(libraryPath, EntryPoint = "clang_CXIndex_getGlobalOptions", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint CXIndex_getGlobalOptions(CXIndex @param0);

        [DllImport(libraryPath, EntryPoint = "clang_getFileName", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString getFileName(CXFile @SFile);

        [DllImport(libraryPath, EntryPoint = "clang_getFileTime", CallingConvention = CallingConvention.Cdecl)]
        public static extern long getFileTime(CXFile @SFile);

        [DllImport(libraryPath, EntryPoint = "clang_getFileUniqueID", CallingConvention = CallingConvention.Cdecl)]
        public static extern int getFileUniqueID(CXFile @file, out CXFileUniqueID @outID);

        [DllImport(libraryPath, EntryPoint = "clang_isFileMultipleIncludeGuarded", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint isFileMultipleIncludeGuarded(CXTranslationUnit @tu, CXFile @file);

        [DllImport(libraryPath, EntryPoint = "clang_getFile", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXFile getFile(CXTranslationUnit @tu, [MarshalAs(UnmanagedType.LPStr)] string @file_name);

        [DllImport(libraryPath, EntryPoint = "clang_File_isEqual", CallingConvention = CallingConvention.Cdecl)]
        public static extern int File_isEqual(CXFile @file1, CXFile @file2);

        [DllImport(libraryPath, EntryPoint = "clang_getNullLocation", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXSourceLocation getNullLocation();

        [DllImport(libraryPath, EntryPoint = "clang_equalLocations", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint equalLocations(CXSourceLocation @loc1, CXSourceLocation @loc2);

        [DllImport(libraryPath, EntryPoint = "clang_getLocation", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXSourceLocation getLocation(CXTranslationUnit @tu, CXFile @file, uint @line, uint @column);

        [DllImport(libraryPath, EntryPoint = "clang_getLocationForOffset", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXSourceLocation getLocationForOffset(CXTranslationUnit @tu, CXFile @file, uint @offset);

        [DllImport(libraryPath, EntryPoint = "clang_Location_isInSystemHeader", CallingConvention = CallingConvention.Cdecl)]
        public static extern int Location_isInSystemHeader(CXSourceLocation @location);

        [DllImport(libraryPath, EntryPoint = "clang_Location_isFromMainFile", CallingConvention = CallingConvention.Cdecl)]
        public static extern int Location_isFromMainFile(CXSourceLocation @location);

        [DllImport(libraryPath, EntryPoint = "clang_getNullRange", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXSourceRange getNullRange();

        [DllImport(libraryPath, EntryPoint = "clang_getRange", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXSourceRange getRange(CXSourceLocation @begin, CXSourceLocation @end);

        [DllImport(libraryPath, EntryPoint = "clang_equalRanges", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint equalRanges(CXSourceRange @range1, CXSourceRange @range2);

        [DllImport(libraryPath, EntryPoint = "clang_Range_isNull", CallingConvention = CallingConvention.Cdecl)]
        public static extern int Range_isNull(CXSourceRange @range);

        [DllImport(libraryPath, EntryPoint = "clang_getExpansionLocation", CallingConvention = CallingConvention.Cdecl)]
        public static extern void getExpansionLocation(CXSourceLocation @location, out CXFile @file, out uint @line, out uint @column, out uint @offset);

        [DllImport(libraryPath, EntryPoint = "clang_getPresumedLocation", CallingConvention = CallingConvention.Cdecl)]
        public static extern void getPresumedLocation(CXSourceLocation @location, out CXString @filename, out uint @line, out uint @column);

        [DllImport(libraryPath, EntryPoint = "clang_getInstantiationLocation", CallingConvention = CallingConvention.Cdecl)]
        public static extern void getInstantiationLocation(CXSourceLocation @location, out CXFile @file, out uint @line, out uint @column, out uint @offset);

        [DllImport(libraryPath, EntryPoint = "clang_getSpellingLocation", CallingConvention = CallingConvention.Cdecl)]
        public static extern void getSpellingLocation(CXSourceLocation @location, out CXFile @file, out uint @line, out uint @column, out uint @offset);

        [DllImport(libraryPath, EntryPoint = "clang_getFileLocation", CallingConvention = CallingConvention.Cdecl)]
        public static extern void getFileLocation(CXSourceLocation @location, out CXFile @file, out uint @line, out uint @column, out uint @offset);

        [DllImport(libraryPath, EntryPoint = "clang_getRangeStart", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXSourceLocation getRangeStart(CXSourceRange @range);

        [DllImport(libraryPath, EntryPoint = "clang_getRangeEnd", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXSourceLocation getRangeEnd(CXSourceRange @range);

        [DllImport(libraryPath, EntryPoint = "clang_getSkippedRanges", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr getSkippedRanges(CXTranslationUnit @tu, CXFile @file);

        [DllImport(libraryPath, EntryPoint = "clang_disposeSourceRangeList", CallingConvention = CallingConvention.Cdecl)]
        public static extern void disposeSourceRangeList(ref CXSourceRangeList @ranges);

        [DllImport(libraryPath, EntryPoint = "clang_getNumDiagnosticsInSet", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint getNumDiagnosticsInSet(CXDiagnosticSet @Diags);

        [DllImport(libraryPath, EntryPoint = "clang_getDiagnosticInSet", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXDiagnostic getDiagnosticInSet(CXDiagnosticSet @Diags, uint @Index);

        [DllImport(libraryPath, EntryPoint = "clang_loadDiagnostics", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXDiagnosticSet loadDiagnostics([MarshalAs(UnmanagedType.LPStr)] string @file, out CXLoadDiag_Error @error, out CXString @errorString);

        [DllImport(libraryPath, EntryPoint = "clang_disposeDiagnosticSet", CallingConvention = CallingConvention.Cdecl)]
        public static extern void disposeDiagnosticSet(CXDiagnosticSet @Diags);

        [DllImport(libraryPath, EntryPoint = "clang_getChildDiagnostics", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXDiagnosticSet getChildDiagnostics(CXDiagnostic @D);

        [DllImport(libraryPath, EntryPoint = "clang_getNumDiagnostics", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint getNumDiagnostics(CXTranslationUnit @Unit);

        [DllImport(libraryPath, EntryPoint = "clang_getDiagnostic", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXDiagnostic getDiagnostic(CXTranslationUnit @Unit, uint @Index);

        [DllImport(libraryPath, EntryPoint = "clang_getDiagnosticSetFromTU", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXDiagnosticSet getDiagnosticSetFromTU(CXTranslationUnit @Unit);

        [DllImport(libraryPath, EntryPoint = "clang_disposeDiagnostic", CallingConvention = CallingConvention.Cdecl)]
        public static extern void disposeDiagnostic(CXDiagnostic @Diagnostic);

        [DllImport(libraryPath, EntryPoint = "clang_formatDiagnostic", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString formatDiagnostic(CXDiagnostic @Diagnostic, uint @Options);

        [DllImport(libraryPath, EntryPoint = "clang_defaultDiagnosticDisplayOptions", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint defaultDiagnosticDisplayOptions();

        [DllImport(libraryPath, EntryPoint = "clang_getDiagnosticSeverity", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXDiagnosticSeverity getDiagnosticSeverity(CXDiagnostic @param0);

        [DllImport(libraryPath, EntryPoint = "clang_getDiagnosticLocation", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXSourceLocation getDiagnosticLocation(CXDiagnostic @param0);

        [DllImport(libraryPath, EntryPoint = "clang_getDiagnosticSpelling", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString getDiagnosticSpelling(CXDiagnostic @param0);

        [DllImport(libraryPath, EntryPoint = "clang_getDiagnosticOption", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString getDiagnosticOption(CXDiagnostic @Diag, out CXString @Disable);

        [DllImport(libraryPath, EntryPoint = "clang_getDiagnosticCategory", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint getDiagnosticCategory(CXDiagnostic @param0);

        [DllImport(libraryPath, EntryPoint = "clang_getDiagnosticCategoryName", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString getDiagnosticCategoryName(uint @Category);

        [DllImport(libraryPath, EntryPoint = "clang_getDiagnosticCategoryText", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString getDiagnosticCategoryText(CXDiagnostic @param0);

        [DllImport(libraryPath, EntryPoint = "clang_getDiagnosticNumRanges", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint getDiagnosticNumRanges(CXDiagnostic @param0);

        [DllImport(libraryPath, EntryPoint = "clang_getDiagnosticRange", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXSourceRange getDiagnosticRange(CXDiagnostic @Diagnostic, uint @Range);

        [DllImport(libraryPath, EntryPoint = "clang_getDiagnosticNumFixIts", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint getDiagnosticNumFixIts(CXDiagnostic @Diagnostic);

        [DllImport(libraryPath, EntryPoint = "clang_getDiagnosticFixIt", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString getDiagnosticFixIt(CXDiagnostic @Diagnostic, uint @FixIt, out CXSourceRange @ReplacementRange);

        [DllImport(libraryPath, EntryPoint = "clang_getTranslationUnitSpelling", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString getTranslationUnitSpelling(CXTranslationUnit @CTUnit);

        [DllImport(libraryPath, EntryPoint = "clang_createTranslationUnit", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXTranslationUnit createTranslationUnit(CXIndex @CIdx, [MarshalAs(UnmanagedType.LPStr)] string @ast_filename);

        [DllImport(libraryPath, EntryPoint = "clang_createTranslationUnit2", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXErrorCode createTranslationUnit2(CXIndex @CIdx, [MarshalAs(UnmanagedType.LPStr)] string @ast_filename, out CXTranslationUnit @out_TU);

        [DllImport(libraryPath, EntryPoint = "clang_defaultEditingTranslationUnitOptions", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint defaultEditingTranslationUnitOptions();

        [DllImport(libraryPath, EntryPoint = "clang_defaultSaveOptions", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint defaultSaveOptions(CXTranslationUnit @TU);

        [DllImport(libraryPath, EntryPoint = "clang_saveTranslationUnit", CallingConvention = CallingConvention.Cdecl)]
        public static extern int saveTranslationUnit(CXTranslationUnit @TU, [MarshalAs(UnmanagedType.LPStr)] string @FileName, uint @options);

        [DllImport(libraryPath, EntryPoint = "clang_disposeTranslationUnit", CallingConvention = CallingConvention.Cdecl)]
        public static extern void disposeTranslationUnit(CXTranslationUnit @param0);

        [DllImport(libraryPath, EntryPoint = "clang_defaultReparseOptions", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint defaultReparseOptions(CXTranslationUnit @TU);

        [DllImport(libraryPath, EntryPoint = "clang_getTUResourceUsageName", CallingConvention = CallingConvention.Cdecl)]
        public static extern string getTUResourceUsageName(CXTUResourceUsageKind @kind);

        [DllImport(libraryPath, EntryPoint = "clang_getCXTUResourceUsage", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXTUResourceUsage getCXTUResourceUsage(CXTranslationUnit @TU);

        [DllImport(libraryPath, EntryPoint = "clang_disposeCXTUResourceUsage", CallingConvention = CallingConvention.Cdecl)]
        public static extern void disposeCXTUResourceUsage(CXTUResourceUsage @usage);

        [DllImport(libraryPath, EntryPoint = "clang_getNullCursor", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXCursor getNullCursor();

        [DllImport(libraryPath, EntryPoint = "clang_getTranslationUnitCursor", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXCursor getTranslationUnitCursor(CXTranslationUnit @param0);

        [DllImport(libraryPath, EntryPoint = "clang_equalCursors", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint equalCursors(CXCursor @param0, CXCursor @param1);

        [DllImport(libraryPath, EntryPoint = "clang_Cursor_isNull", CallingConvention = CallingConvention.Cdecl)]
        public static extern int Cursor_isNull(CXCursor @cursor);

        [DllImport(libraryPath, EntryPoint = "clang_hashCursor", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint hashCursor(CXCursor @param0);

        [DllImport(libraryPath, EntryPoint = "clang_getCursorKind", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXCursorKind getCursorKind(CXCursor @param0);

        [DllImport(libraryPath, EntryPoint = "clang_isDeclaration", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint isDeclaration(CXCursorKind @param0);

        [DllImport(libraryPath, EntryPoint = "clang_isReference", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint isReference(CXCursorKind @param0);

        [DllImport(libraryPath, EntryPoint = "clang_isExpression", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint isExpression(CXCursorKind @param0);

        [DllImport(libraryPath, EntryPoint = "clang_isStatement", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint isStatement(CXCursorKind @param0);

        [DllImport(libraryPath, EntryPoint = "clang_isAttribute", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint isAttribute(CXCursorKind @param0);

        [DllImport(libraryPath, EntryPoint = "clang_Cursor_hasAttrs", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint Cursor_hasAttrs(CXCursor @C);

        [DllImport(libraryPath, EntryPoint = "clang_isInvalid", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint isInvalid(CXCursorKind @param0);

        [DllImport(libraryPath, EntryPoint = "clang_isTranslationUnit", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint isTranslationUnit(CXCursorKind @param0);

        [DllImport(libraryPath, EntryPoint = "clang_isPreprocessing", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint isPreprocessing(CXCursorKind @param0);

        [DllImport(libraryPath, EntryPoint = "clang_isUnexposed", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint isUnexposed(CXCursorKind @param0);

        [DllImport(libraryPath, EntryPoint = "clang_getCursorLinkage", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXLinkageKind getCursorLinkage(CXCursor @cursor);

        [DllImport(libraryPath, EntryPoint = "clang_getCursorVisibility", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXVisibilityKind getCursorVisibility(CXCursor @cursor);

        [DllImport(libraryPath, EntryPoint = "clang_getCursorAvailability", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXAvailabilityKind getCursorAvailability(CXCursor @cursor);

        [DllImport(libraryPath, EntryPoint = "clang_getCursorPlatformAvailability", CallingConvention = CallingConvention.Cdecl)]
        public static extern int getCursorPlatformAvailability(CXCursor @cursor, out int @always_deprecated, out CXString @deprecated_message, out int @always_unavailable, out CXString @unavailable_message, out CXPlatformAvailability @availability, int @availability_size);

        [DllImport(libraryPath, EntryPoint = "clang_disposeCXPlatformAvailability", CallingConvention = CallingConvention.Cdecl)]
        public static extern void disposeCXPlatformAvailability(out CXPlatformAvailability @availability);

        [DllImport(libraryPath, EntryPoint = "clang_getCursorLanguage", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXLanguageKind getCursorLanguage(CXCursor @cursor);

        [DllImport(libraryPath, EntryPoint = "clang_Cursor_getTranslationUnit", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXTranslationUnit Cursor_getTranslationUnit(CXCursor @param0);

        [DllImport(libraryPath, EntryPoint = "clang_createCXCursorSet", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXCursorSet createCXCursorSet();

        [DllImport(libraryPath, EntryPoint = "clang_disposeCXCursorSet", CallingConvention = CallingConvention.Cdecl)]
        public static extern void disposeCXCursorSet(CXCursorSet @cset);

        [DllImport(libraryPath, EntryPoint = "clang_CXCursorSet_contains", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint CXCursorSet_contains(CXCursorSet @cset, CXCursor @cursor);

        [DllImport(libraryPath, EntryPoint = "clang_CXCursorSet_insert", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint CXCursorSet_insert(CXCursorSet @cset, CXCursor @cursor);

        [DllImport(libraryPath, EntryPoint = "clang_getCursorSemanticParent", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXCursor getCursorSemanticParent(CXCursor @cursor);

        [DllImport(libraryPath, EntryPoint = "clang_getCursorLexicalParent", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXCursor getCursorLexicalParent(CXCursor @cursor);

        [DllImport(libraryPath, EntryPoint = "clang_getOverriddenCursors", CallingConvention = CallingConvention.Cdecl)]
        public static extern void getOverriddenCursors(CXCursor @cursor, out IntPtr @overridden, out uint @num_overridden);

        [DllImport(libraryPath, EntryPoint = "clang_disposeOverriddenCursors", CallingConvention = CallingConvention.Cdecl)]
        public static extern void disposeOverriddenCursors(ref CXCursor @overridden);

        [DllImport(libraryPath, EntryPoint = "clang_getIncludedFile", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXFile getIncludedFile(CXCursor @cursor);

        [DllImport(libraryPath, EntryPoint = "clang_getCursor", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXCursor getCursor(CXTranslationUnit @param0, CXSourceLocation @param1);

        [DllImport(libraryPath, EntryPoint = "clang_getCursorLocation", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXSourceLocation getCursorLocation(CXCursor @param0);

        [DllImport(libraryPath, EntryPoint = "clang_getCursorExtent", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXSourceRange getCursorExtent(CXCursor @param0);

        [DllImport(libraryPath, EntryPoint = "clang_getCursorType", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXType getCursorType(CXCursor @C);

        [DllImport(libraryPath, EntryPoint = "clang_getTypeSpelling", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString getTypeSpelling(CXType @CT);

        [DllImport(libraryPath, EntryPoint = "clang_getTypedefDeclUnderlyingType", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXType getTypedefDeclUnderlyingType(CXCursor @C);

        [DllImport(libraryPath, EntryPoint = "clang_getEnumDeclIntegerType", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXType getEnumDeclIntegerType(CXCursor @C);

        [DllImport(libraryPath, EntryPoint = "clang_getEnumConstantDeclValue", CallingConvention = CallingConvention.Cdecl)]
        public static extern long getEnumConstantDeclValue(CXCursor @C);

        [DllImport(libraryPath, EntryPoint = "clang_getEnumConstantDeclUnsignedValue", CallingConvention = CallingConvention.Cdecl)]
        public static extern ulong getEnumConstantDeclUnsignedValue(CXCursor @C);

        [DllImport(libraryPath, EntryPoint = "clang_getFieldDeclBitWidth", CallingConvention = CallingConvention.Cdecl)]
        public static extern int getFieldDeclBitWidth(CXCursor @C);

        [DllImport(libraryPath, EntryPoint = "clang_Cursor_getNumArguments", CallingConvention = CallingConvention.Cdecl)]
        public static extern int Cursor_getNumArguments(CXCursor @C);

        [DllImport(libraryPath, EntryPoint = "clang_Cursor_getArgument", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXCursor Cursor_getArgument(CXCursor @C, uint @i);

        [DllImport(libraryPath, EntryPoint = "clang_Cursor_getNumTemplateArguments", CallingConvention = CallingConvention.Cdecl)]
        public static extern int Cursor_getNumTemplateArguments(CXCursor @C);

        [DllImport(libraryPath, EntryPoint = "clang_Cursor_getTemplateArgumentKind", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXTemplateArgumentKind Cursor_getTemplateArgumentKind(CXCursor @C, uint @I);

        [DllImport(libraryPath, EntryPoint = "clang_Cursor_getTemplateArgumentType", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXType Cursor_getTemplateArgumentType(CXCursor @C, uint @I);

        [DllImport(libraryPath, EntryPoint = "clang_Cursor_getTemplateArgumentValue", CallingConvention = CallingConvention.Cdecl)]
        public static extern long Cursor_getTemplateArgumentValue(CXCursor @C, uint @I);

        [DllImport(libraryPath, EntryPoint = "clang_Cursor_getTemplateArgumentUnsignedValue", CallingConvention = CallingConvention.Cdecl)]
        public static extern ulong Cursor_getTemplateArgumentUnsignedValue(CXCursor @C, uint @I);

        [DllImport(libraryPath, EntryPoint = "clang_equalTypes", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint equalTypes(CXType @A, CXType @B);

        [DllImport(libraryPath, EntryPoint = "clang_getCanonicalType", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXType getCanonicalType(CXType @T);

        [DllImport(libraryPath, EntryPoint = "clang_isConstQualifiedType", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint isConstQualifiedType(CXType @T);

        [DllImport(libraryPath, EntryPoint = "clang_Cursor_isMacroFunctionLike", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint Cursor_isMacroFunctionLike(CXCursor @C);

        [DllImport(libraryPath, EntryPoint = "clang_Cursor_isMacroBuiltin", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint Cursor_isMacroBuiltin(CXCursor @C);

        [DllImport(libraryPath, EntryPoint = "clang_Cursor_isFunctionInlined", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint Cursor_isFunctionInlined(CXCursor @C);

        [DllImport(libraryPath, EntryPoint = "clang_isVolatileQualifiedType", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint isVolatileQualifiedType(CXType @T);

        [DllImport(libraryPath, EntryPoint = "clang_isRestrictQualifiedType", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint isRestrictQualifiedType(CXType @T);

        [DllImport(libraryPath, EntryPoint = "clang_getPointeeType", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXType getPointeeType(CXType @T);

        [DllImport(libraryPath, EntryPoint = "clang_getTypeDeclaration", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXCursor getTypeDeclaration(CXType @T);

        [DllImport(libraryPath, EntryPoint = "clang_getDeclObjCTypeEncoding", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString getDeclObjCTypeEncoding(CXCursor @C);

        [DllImport(libraryPath, EntryPoint = "clang_Type_getObjCEncoding", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString Type_getObjCEncoding(CXType @type);

        [DllImport(libraryPath, EntryPoint = "clang_getTypeKindSpelling", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString getTypeKindSpelling(CXTypeKind @K);

        [DllImport(libraryPath, EntryPoint = "clang_getFunctionTypeCallingConv", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXCallingConv getFunctionTypeCallingConv(CXType @T);

        [DllImport(libraryPath, EntryPoint = "clang_getResultType", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXType getResultType(CXType @T);

        [DllImport(libraryPath, EntryPoint = "clang_getNumArgTypes", CallingConvention = CallingConvention.Cdecl)]
        public static extern int getNumArgTypes(CXType @T);

        [DllImport(libraryPath, EntryPoint = "clang_getArgType", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXType getArgType(CXType @T, uint @i);

        [DllImport(libraryPath, EntryPoint = "clang_isFunctionTypeVariadic", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint isFunctionTypeVariadic(CXType @T);

        [DllImport(libraryPath, EntryPoint = "clang_getCursorResultType", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXType getCursorResultType(CXCursor @C);

        [DllImport(libraryPath, EntryPoint = "clang_isPODType", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint isPODType(CXType @T);

        [DllImport(libraryPath, EntryPoint = "clang_getElementType", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXType getElementType(CXType @T);

        [DllImport(libraryPath, EntryPoint = "clang_getNumElements", CallingConvention = CallingConvention.Cdecl)]
        public static extern long getNumElements(CXType @T);

        [DllImport(libraryPath, EntryPoint = "clang_getArrayElementType", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXType getArrayElementType(CXType @T);

        [DllImport(libraryPath, EntryPoint = "clang_getArraySize", CallingConvention = CallingConvention.Cdecl)]
        public static extern long getArraySize(CXType @T);

        [DllImport(libraryPath, EntryPoint = "clang_Type_getNamedType", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXType Type_getNamedType(CXType @T);

        [DllImport(libraryPath, EntryPoint = "clang_Type_getAlignOf", CallingConvention = CallingConvention.Cdecl)]
        public static extern long Type_getAlignOf(CXType @T);

        [DllImport(libraryPath, EntryPoint = "clang_Type_getClassType", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXType Type_getClassType(CXType @T);

        [DllImport(libraryPath, EntryPoint = "clang_Type_getSizeOf", CallingConvention = CallingConvention.Cdecl)]
        public static extern long Type_getSizeOf(CXType @T);

        [DllImport(libraryPath, EntryPoint = "clang_Type_getOffsetOf", CallingConvention = CallingConvention.Cdecl)]
        public static extern long Type_getOffsetOf(CXType @T, [MarshalAs(UnmanagedType.LPStr)] string @S);

        [DllImport(libraryPath, EntryPoint = "clang_Cursor_getOffsetOfField", CallingConvention = CallingConvention.Cdecl)]
        public static extern long Cursor_getOffsetOfField(CXCursor @C);

        [DllImport(libraryPath, EntryPoint = "clang_Cursor_isAnonymous", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint Cursor_isAnonymous(CXCursor @C);

        [DllImport(libraryPath, EntryPoint = "clang_Type_getNumTemplateArguments", CallingConvention = CallingConvention.Cdecl)]
        public static extern int Type_getNumTemplateArguments(CXType @T);

        [DllImport(libraryPath, EntryPoint = "clang_Type_getTemplateArgumentAsType", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXType Type_getTemplateArgumentAsType(CXType @T, uint @i);

        [DllImport(libraryPath, EntryPoint = "clang_Type_getCXXRefQualifier", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXRefQualifierKind Type_getCXXRefQualifier(CXType @T);

        [DllImport(libraryPath, EntryPoint = "clang_Cursor_isBitField", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint Cursor_isBitField(CXCursor @C);

        [DllImport(libraryPath, EntryPoint = "clang_isVirtualBase", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint isVirtualBase(CXCursor @param0);

        [DllImport(libraryPath, EntryPoint = "clang_getCXXAccessSpecifier", CallingConvention = CallingConvention.Cdecl)]
        public static extern CX_CXXAccessSpecifier getCXXAccessSpecifier(CXCursor @param0);

        [DllImport(libraryPath, EntryPoint = "clang_Cursor_getStorageClass", CallingConvention = CallingConvention.Cdecl)]
        public static extern CX_StorageClass Cursor_getStorageClass(CXCursor @param0);

        [DllImport(libraryPath, EntryPoint = "clang_getNumOverloadedDecls", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint getNumOverloadedDecls(CXCursor @cursor);

        [DllImport(libraryPath, EntryPoint = "clang_getOverloadedDecl", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXCursor getOverloadedDecl(CXCursor @cursor, uint @index);

        [DllImport(libraryPath, EntryPoint = "clang_getIBOutletCollectionType", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXType getIBOutletCollectionType(CXCursor @param0);

        [DllImport(libraryPath, EntryPoint = "clang_visitChildren", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint visitChildren(CXCursor @parent, CXCursorVisitor @visitor, CXClientData @client_data);

        [DllImport(libraryPath, EntryPoint = "clang_getCursorUSR", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString getCursorUSR(CXCursor @param0);

        [DllImport(libraryPath, EntryPoint = "clang_constructUSR_ObjCClass", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString constructUSR_ObjCClass([MarshalAs(UnmanagedType.LPStr)] string @class_name);

        [DllImport(libraryPath, EntryPoint = "clang_constructUSR_ObjCCategory", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString constructUSR_ObjCCategory([MarshalAs(UnmanagedType.LPStr)] string @class_name, [MarshalAs(UnmanagedType.LPStr)] string @category_name);

        [DllImport(libraryPath, EntryPoint = "clang_constructUSR_ObjCProtocol", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString constructUSR_ObjCProtocol([MarshalAs(UnmanagedType.LPStr)] string @protocol_name);

        [DllImport(libraryPath, EntryPoint = "clang_constructUSR_ObjCIvar", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString constructUSR_ObjCIvar([MarshalAs(UnmanagedType.LPStr)] string @name, CXString @classUSR);

        [DllImport(libraryPath, EntryPoint = "clang_constructUSR_ObjCMethod", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString constructUSR_ObjCMethod([MarshalAs(UnmanagedType.LPStr)] string @name, uint @isInstanceMethod, CXString @classUSR);

        [DllImport(libraryPath, EntryPoint = "clang_constructUSR_ObjCProperty", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString constructUSR_ObjCProperty([MarshalAs(UnmanagedType.LPStr)] string @property, CXString @classUSR);

        [DllImport(libraryPath, EntryPoint = "clang_getCursorSpelling", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString getCursorSpelling(CXCursor @param0);

        [DllImport(libraryPath, EntryPoint = "clang_Cursor_getSpellingNameRange", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXSourceRange Cursor_getSpellingNameRange(CXCursor @param0, uint @pieceIndex, uint @options);

        [DllImport(libraryPath, EntryPoint = "clang_getCursorDisplayName", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString getCursorDisplayName(CXCursor @param0);

        [DllImport(libraryPath, EntryPoint = "clang_getCursorReferenced", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXCursor getCursorReferenced(CXCursor @param0);

        [DllImport(libraryPath, EntryPoint = "clang_getCursorDefinition", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXCursor getCursorDefinition(CXCursor @param0);

        [DllImport(libraryPath, EntryPoint = "clang_isCursorDefinition", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint isCursorDefinition(CXCursor @param0);

        [DllImport(libraryPath, EntryPoint = "clang_getCanonicalCursor", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXCursor getCanonicalCursor(CXCursor @param0);

        [DllImport(libraryPath, EntryPoint = "clang_Cursor_getObjCSelectorIndex", CallingConvention = CallingConvention.Cdecl)]
        public static extern int Cursor_getObjCSelectorIndex(CXCursor @param0);

        [DllImport(libraryPath, EntryPoint = "clang_Cursor_isDynamicCall", CallingConvention = CallingConvention.Cdecl)]
        public static extern int Cursor_isDynamicCall(CXCursor @C);

        [DllImport(libraryPath, EntryPoint = "clang_Cursor_getReceiverType", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXType Cursor_getReceiverType(CXCursor @C);

        [DllImport(libraryPath, EntryPoint = "clang_Cursor_getObjCPropertyAttributes", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint Cursor_getObjCPropertyAttributes(CXCursor @C, uint @reserved);

        [DllImport(libraryPath, EntryPoint = "clang_Cursor_getObjCDeclQualifiers", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint Cursor_getObjCDeclQualifiers(CXCursor @C);

        [DllImport(libraryPath, EntryPoint = "clang_Cursor_isObjCOptional", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint Cursor_isObjCOptional(CXCursor @C);

        [DllImport(libraryPath, EntryPoint = "clang_Cursor_isVariadic", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint Cursor_isVariadic(CXCursor @C);

        [DllImport(libraryPath, EntryPoint = "clang_Cursor_getCommentRange", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXSourceRange Cursor_getCommentRange(CXCursor @C);

        [DllImport(libraryPath, EntryPoint = "clang_Cursor_getRawCommentText", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString Cursor_getRawCommentText(CXCursor @C);

        [DllImport(libraryPath, EntryPoint = "clang_Cursor_getBriefCommentText", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString Cursor_getBriefCommentText(CXCursor @C);

        [DllImport(libraryPath, EntryPoint = "clang_Cursor_getMangling", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString Cursor_getMangling(CXCursor @param0);

        [DllImport(libraryPath, EntryPoint = "clang_Cursor_getCXXManglings", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr Cursor_getCXXManglings(CXCursor @param0);

        [DllImport(libraryPath, EntryPoint = "clang_Cursor_getModule", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXModule Cursor_getModule(CXCursor @C);

        [DllImport(libraryPath, EntryPoint = "clang_getModuleForFile", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXModule getModuleForFile(CXTranslationUnit @param0, CXFile @param1);

        [DllImport(libraryPath, EntryPoint = "clang_Module_getASTFile", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXFile Module_getASTFile(CXModule @Module);

        [DllImport(libraryPath, EntryPoint = "clang_Module_getParent", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXModule Module_getParent(CXModule @Module);

        [DllImport(libraryPath, EntryPoint = "clang_Module_getName", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString Module_getName(CXModule @Module);

        [DllImport(libraryPath, EntryPoint = "clang_Module_getFullName", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString Module_getFullName(CXModule @Module);

        [DllImport(libraryPath, EntryPoint = "clang_Module_isSystem", CallingConvention = CallingConvention.Cdecl)]
        public static extern int Module_isSystem(CXModule @Module);

        [DllImport(libraryPath, EntryPoint = "clang_Module_getNumTopLevelHeaders", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint Module_getNumTopLevelHeaders(CXTranslationUnit @param0, CXModule @Module);

        [DllImport(libraryPath, EntryPoint = "clang_Module_getTopLevelHeader", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXFile Module_getTopLevelHeader(CXTranslationUnit @param0, CXModule @Module, uint @Index);

        [DllImport(libraryPath, EntryPoint = "clang_CXXConstructor_isConvertingConstructor", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint CXXConstructor_isConvertingConstructor(CXCursor @C);

        [DllImport(libraryPath, EntryPoint = "clang_CXXConstructor_isCopyConstructor", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint CXXConstructor_isCopyConstructor(CXCursor @C);

        [DllImport(libraryPath, EntryPoint = "clang_CXXConstructor_isDefaultConstructor", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint CXXConstructor_isDefaultConstructor(CXCursor @C);

        [DllImport(libraryPath, EntryPoint = "clang_CXXConstructor_isMoveConstructor", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint CXXConstructor_isMoveConstructor(CXCursor @C);

        [DllImport(libraryPath, EntryPoint = "clang_CXXField_isMutable", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint CXXField_isMutable(CXCursor @C);

        [DllImport(libraryPath, EntryPoint = "clang_CXXMethod_isDefaulted", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint CXXMethod_isDefaulted(CXCursor @C);

        [DllImport(libraryPath, EntryPoint = "clang_CXXMethod_isPureVirtual", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint CXXMethod_isPureVirtual(CXCursor @C);

        [DllImport(libraryPath, EntryPoint = "clang_CXXMethod_isStatic", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint CXXMethod_isStatic(CXCursor @C);

        [DllImport(libraryPath, EntryPoint = "clang_CXXMethod_isVirtual", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint CXXMethod_isVirtual(CXCursor @C);

        [DllImport(libraryPath, EntryPoint = "clang_CXXMethod_isConst", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint CXXMethod_isConst(CXCursor @C);

        [DllImport(libraryPath, EntryPoint = "clang_getTemplateCursorKind", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXCursorKind getTemplateCursorKind(CXCursor @C);

        [DllImport(libraryPath, EntryPoint = "clang_getSpecializedCursorTemplate", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXCursor getSpecializedCursorTemplate(CXCursor @C);

        [DllImport(libraryPath, EntryPoint = "clang_getCursorReferenceNameRange", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXSourceRange getCursorReferenceNameRange(CXCursor @C, uint @NameFlags, uint @PieceIndex);

        [DllImport(libraryPath, EntryPoint = "clang_getTokenKind", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXTokenKind getTokenKind(CXToken @param0);

        [DllImport(libraryPath, EntryPoint = "clang_getTokenSpelling", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString getTokenSpelling(CXTranslationUnit @param0, CXToken @param1);

        [DllImport(libraryPath, EntryPoint = "clang_getTokenLocation", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXSourceLocation getTokenLocation(CXTranslationUnit @param0, CXToken @param1);

        [DllImport(libraryPath, EntryPoint = "clang_getTokenExtent", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXSourceRange getTokenExtent(CXTranslationUnit @param0, CXToken @param1);

        [DllImport(libraryPath, EntryPoint = "clang_tokenize", CallingConvention = CallingConvention.Cdecl)]
        public static extern void tokenize(CXTranslationUnit @TU, CXSourceRange @Range, out CXToken[] @Tokens, out uint @NumTokens);

        [DllImport(libraryPath, EntryPoint = "clang_annotateTokens", CallingConvention = CallingConvention.Cdecl)]
        public static extern void annotateTokens(CXTranslationUnit @TU, CXToken[] @Tokens, uint @NumTokens, out CXCursor @Cursors);

        [DllImport(libraryPath, EntryPoint = "clang_disposeTokens", CallingConvention = CallingConvention.Cdecl)]
        public static extern void disposeTokens(CXTranslationUnit @TU, CXToken[] @Tokens, uint @NumTokens);

        [DllImport(libraryPath, EntryPoint = "clang_getCursorKindSpelling", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString getCursorKindSpelling(CXCursorKind @Kind);

        [DllImport(libraryPath, EntryPoint = "clang_getDefinitionSpellingAndExtent", CallingConvention = CallingConvention.Cdecl)]
        public static extern void getDefinitionSpellingAndExtent(CXCursor @param0, out IntPtr @startBuf, out IntPtr @endBuf, out uint @startLine, out uint @startColumn, out uint @endLine, out uint @endColumn);

        [DllImport(libraryPath, EntryPoint = "clang_enableStackTraces", CallingConvention = CallingConvention.Cdecl)]
        public static extern void enableStackTraces();

        [DllImport(libraryPath, EntryPoint = "clang_executeOnThread", CallingConvention = CallingConvention.Cdecl)]
        public static extern void executeOnThread(out IntPtr @fn, IntPtr @user_data, uint @stack_size);

        [DllImport(libraryPath, EntryPoint = "clang_getCompletionChunkKind", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXCompletionChunkKind getCompletionChunkKind(CXCompletionString @completion_string, uint @chunk_number);

        [DllImport(libraryPath, EntryPoint = "clang_getCompletionChunkText", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString getCompletionChunkText(CXCompletionString @completion_string, uint @chunk_number);

        [DllImport(libraryPath, EntryPoint = "clang_getCompletionChunkCompletionString", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXCompletionString getCompletionChunkCompletionString(CXCompletionString @completion_string, uint @chunk_number);

        [DllImport(libraryPath, EntryPoint = "clang_getNumCompletionChunks", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint getNumCompletionChunks(CXCompletionString @completion_string);

        [DllImport(libraryPath, EntryPoint = "clang_getCompletionPriority", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint getCompletionPriority(CXCompletionString @completion_string);

        [DllImport(libraryPath, EntryPoint = "clang_getCompletionAvailability", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXAvailabilityKind getCompletionAvailability(CXCompletionString @completion_string);

        [DllImport(libraryPath, EntryPoint = "clang_getCompletionNumAnnotations", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint getCompletionNumAnnotations(CXCompletionString @completion_string);

        [DllImport(libraryPath, EntryPoint = "clang_getCompletionAnnotation", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString getCompletionAnnotation(CXCompletionString @completion_string, uint @annotation_number);

        [DllImport(libraryPath, EntryPoint = "clang_getCompletionParent", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString getCompletionParent(CXCompletionString @completion_string, out CXCursorKind @kind);

        [DllImport(libraryPath, EntryPoint = "clang_getCompletionBriefComment", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString getCompletionBriefComment(CXCompletionString @completion_string);

        [DllImport(libraryPath, EntryPoint = "clang_getCursorCompletionString", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXCompletionString getCursorCompletionString(CXCursor @cursor);

        [DllImport(libraryPath, EntryPoint = "clang_defaultCodeCompleteOptions", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint defaultCodeCompleteOptions();

        [DllImport(libraryPath, EntryPoint = "clang_sortCodeCompletionResults", CallingConvention = CallingConvention.Cdecl)]
        public static extern void sortCodeCompletionResults(CXCompletionResult[] @Results, uint @NumResults);

        [DllImport(libraryPath, EntryPoint = "clang_disposeCodeCompleteResults", CallingConvention = CallingConvention.Cdecl)]
        public static extern void disposeCodeCompleteResults(ref CXCodeCompleteResults @Results);

        [DllImport(libraryPath, EntryPoint = "clang_codeCompleteGetNumDiagnostics", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint codeCompleteGetNumDiagnostics(ref CXCodeCompleteResults @Results);

        [DllImport(libraryPath, EntryPoint = "clang_codeCompleteGetDiagnostic", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXDiagnostic codeCompleteGetDiagnostic(ref CXCodeCompleteResults @Results, uint @Index);

        [DllImport(libraryPath, EntryPoint = "clang_codeCompleteGetContexts", CallingConvention = CallingConvention.Cdecl)]
        public static extern ulong codeCompleteGetContexts(ref CXCodeCompleteResults @Results);

        [DllImport(libraryPath, EntryPoint = "clang_codeCompleteGetContainerKind", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXCursorKind codeCompleteGetContainerKind(ref CXCodeCompleteResults @Results, out uint @IsIncomplete);

        [DllImport(libraryPath, EntryPoint = "clang_codeCompleteGetContainerUSR", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString codeCompleteGetContainerUSR(ref CXCodeCompleteResults @Results);

        [DllImport(libraryPath, EntryPoint = "clang_codeCompleteGetObjCSelector", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString codeCompleteGetObjCSelector(ref CXCodeCompleteResults @Results);

        [DllImport(libraryPath, EntryPoint = "clang_getClangVersion", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString getClangVersion();

        [DllImport(libraryPath, EntryPoint = "clang_toggleCrashRecovery", CallingConvention = CallingConvention.Cdecl)]
        public static extern void toggleCrashRecovery(uint @isEnabled);

        [DllImport(libraryPath, EntryPoint = "clang_getInclusions", CallingConvention = CallingConvention.Cdecl)]
        public static extern void getInclusions(CXTranslationUnit @tu, CXInclusionVisitor @visitor, CXClientData @client_data);

        [DllImport(libraryPath, EntryPoint = "clang_Cursor_Evaluate", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXEvalResult Cursor_Evaluate(CXCursor @C);

        [DllImport(libraryPath, EntryPoint = "clang_EvalResult_getKind", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXEvalResultKind EvalResult_getKind(CXEvalResult @E);

        [DllImport(libraryPath, EntryPoint = "clang_EvalResult_getAsInt", CallingConvention = CallingConvention.Cdecl)]
        public static extern int EvalResult_getAsInt(CXEvalResult @E);

        [DllImport(libraryPath, EntryPoint = "clang_EvalResult_getAsDouble", CallingConvention = CallingConvention.Cdecl)]
        public static extern double EvalResult_getAsDouble(CXEvalResult @E);

        [DllImport(libraryPath, EntryPoint = "clang_EvalResult_getAsStr", CallingConvention = CallingConvention.Cdecl)]
        public static extern string EvalResult_getAsStr(CXEvalResult @E);

        [DllImport(libraryPath, EntryPoint = "clang_EvalResult_dispose", CallingConvention = CallingConvention.Cdecl)]
        public static extern void EvalResult_dispose(CXEvalResult @E);

        [DllImport(libraryPath, EntryPoint = "clang_getRemappings", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXRemapping getRemappings([MarshalAs(UnmanagedType.LPStr)] string @path);

        [DllImport(libraryPath, EntryPoint = "clang_getRemappingsFromFileList", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXRemapping getRemappingsFromFileList(out IntPtr @filePaths, uint @numFiles);

        [DllImport(libraryPath, EntryPoint = "clang_remap_getNumFiles", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint remap_getNumFiles(CXRemapping @param0);

        [DllImport(libraryPath, EntryPoint = "clang_remap_getFilenames", CallingConvention = CallingConvention.Cdecl)]
        public static extern void remap_getFilenames(CXRemapping @param0, uint @index, out CXString @original, out CXString @transformed);

        [DllImport(libraryPath, EntryPoint = "clang_remap_dispose", CallingConvention = CallingConvention.Cdecl)]
        public static extern void remap_dispose(CXRemapping @param0);

        [DllImport(libraryPath, EntryPoint = "clang_findReferencesInFile", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXResult findReferencesInFile(CXCursor @cursor, CXFile @file, CXCursorAndRangeVisitor @visitor);

        [DllImport(libraryPath, EntryPoint = "clang_findIncludesInFile", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXResult findIncludesInFile(CXTranslationUnit @TU, CXFile @file, CXCursorAndRangeVisitor @visitor);

        [DllImport(libraryPath, EntryPoint = "clang_index_isEntityObjCContainerKind", CallingConvention = CallingConvention.Cdecl)]
        public static extern int index_isEntityObjCContainerKind(CXIdxEntityKind @param0);

        [DllImport(libraryPath, EntryPoint = "clang_index_getObjCContainerDeclInfo", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr index_getObjCContainerDeclInfo(out CXIdxDeclInfo @param0);

        [DllImport(libraryPath, EntryPoint = "clang_index_getObjCInterfaceDeclInfo", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr index_getObjCInterfaceDeclInfo(out CXIdxDeclInfo @param0);

        [DllImport(libraryPath, EntryPoint = "clang_index_getObjCCategoryDeclInfo", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr index_getObjCCategoryDeclInfo(out CXIdxDeclInfo @param0);

        [DllImport(libraryPath, EntryPoint = "clang_index_getObjCProtocolRefListInfo", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr index_getObjCProtocolRefListInfo(out CXIdxDeclInfo @param0);

        [DllImport(libraryPath, EntryPoint = "clang_index_getObjCPropertyDeclInfo", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr index_getObjCPropertyDeclInfo(out CXIdxDeclInfo @param0);

        [DllImport(libraryPath, EntryPoint = "clang_index_getIBOutletCollectionAttrInfo", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr index_getIBOutletCollectionAttrInfo(out CXIdxAttrInfo @param0);

        [DllImport(libraryPath, EntryPoint = "clang_index_getCXXClassDeclInfo", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr index_getCXXClassDeclInfo(out CXIdxDeclInfo @param0);

        [DllImport(libraryPath, EntryPoint = "clang_index_getClientContainer", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXIdxClientContainer index_getClientContainer(out CXIdxContainerInfo @param0);

        [DllImport(libraryPath, EntryPoint = "clang_index_setClientContainer", CallingConvention = CallingConvention.Cdecl)]
        public static extern void index_setClientContainer(out CXIdxContainerInfo @param0, CXIdxClientContainer @param1);

        [DllImport(libraryPath, EntryPoint = "clang_IndexAction_create", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXIndexAction IndexAction_create(CXIndex @CIdx);

        [DllImport(libraryPath, EntryPoint = "clang_IndexAction_dispose", CallingConvention = CallingConvention.Cdecl)]
        public static extern void IndexAction_dispose(CXIndexAction @param0);

        [DllImport(libraryPath, EntryPoint = "clang_indexTranslationUnit", CallingConvention = CallingConvention.Cdecl)]
        public static extern int indexTranslationUnit(CXIndexAction @param0, CXClientData @client_data, out IndexerCallbacks @index_callbacks, uint @index_callbacks_size, uint @index_options, CXTranslationUnit @param5);

        [DllImport(libraryPath, EntryPoint = "clang_indexLoc_getFileLocation", CallingConvention = CallingConvention.Cdecl)]
        public static extern void indexLoc_getFileLocation(CXIdxLoc @loc, out CXIdxClientFile @indexFile, out CXFile @file, out uint @line, out uint @column, out uint @offset);

        [DllImport(libraryPath, EntryPoint = "clang_indexLoc_getCXSourceLocation", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXSourceLocation indexLoc_getCXSourceLocation(CXIdxLoc @loc);

        [DllImport(libraryPath, EntryPoint = "clang_Type_visitFields", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint Type_visitFields(CXType @T, CXFieldVisitor @visitor, CXClientData @client_data);

        [DllImport(libraryPath, EntryPoint = "clang_Cursor_getParsedComment", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXComment Cursor_getParsedComment(CXCursor @C);

        [DllImport(libraryPath, EntryPoint = "clang_Comment_getKind", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXCommentKind Comment_getKind(CXComment @Comment);

        [DllImport(libraryPath, EntryPoint = "clang_Comment_getNumChildren", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint Comment_getNumChildren(CXComment @Comment);

        [DllImport(libraryPath, EntryPoint = "clang_Comment_getChild", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXComment Comment_getChild(CXComment @Comment, uint @ChildIdx);

        [DllImport(libraryPath, EntryPoint = "clang_Comment_isWhitespace", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint Comment_isWhitespace(CXComment @Comment);

        [DllImport(libraryPath, EntryPoint = "clang_InlineContentComment_hasTrailingNewline", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint InlineContentComment_hasTrailingNewline(CXComment @Comment);

        [DllImport(libraryPath, EntryPoint = "clang_TextComment_getText", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString TextComment_getText(CXComment @Comment);

        [DllImport(libraryPath, EntryPoint = "clang_InlineCommandComment_getCommandName", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString InlineCommandComment_getCommandName(CXComment @Comment);

        [DllImport(libraryPath, EntryPoint = "clang_InlineCommandComment_getRenderKind", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXCommentInlineCommandRenderKind InlineCommandComment_getRenderKind(CXComment @Comment);

        [DllImport(libraryPath, EntryPoint = "clang_InlineCommandComment_getNumArgs", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint InlineCommandComment_getNumArgs(CXComment @Comment);

        [DllImport(libraryPath, EntryPoint = "clang_InlineCommandComment_getArgText", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString InlineCommandComment_getArgText(CXComment @Comment, uint @ArgIdx);

        [DllImport(libraryPath, EntryPoint = "clang_HTMLTagComment_getTagName", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString HTMLTagComment_getTagName(CXComment @Comment);

        [DllImport(libraryPath, EntryPoint = "clang_HTMLStartTagComment_isSelfClosing", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint HTMLStartTagComment_isSelfClosing(CXComment @Comment);

        [DllImport(libraryPath, EntryPoint = "clang_HTMLStartTag_getNumAttrs", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint HTMLStartTag_getNumAttrs(CXComment @Comment);

        [DllImport(libraryPath, EntryPoint = "clang_HTMLStartTag_getAttrName", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString HTMLStartTag_getAttrName(CXComment @Comment, uint @AttrIdx);

        [DllImport(libraryPath, EntryPoint = "clang_HTMLStartTag_getAttrValue", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString HTMLStartTag_getAttrValue(CXComment @Comment, uint @AttrIdx);

        [DllImport(libraryPath, EntryPoint = "clang_BlockCommandComment_getCommandName", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString BlockCommandComment_getCommandName(CXComment @Comment);

        [DllImport(libraryPath, EntryPoint = "clang_BlockCommandComment_getNumArgs", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint BlockCommandComment_getNumArgs(CXComment @Comment);

        [DllImport(libraryPath, EntryPoint = "clang_BlockCommandComment_getArgText", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString BlockCommandComment_getArgText(CXComment @Comment, uint @ArgIdx);

        [DllImport(libraryPath, EntryPoint = "clang_BlockCommandComment_getParagraph", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXComment BlockCommandComment_getParagraph(CXComment @Comment);

        [DllImport(libraryPath, EntryPoint = "clang_ParamCommandComment_getParamName", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString ParamCommandComment_getParamName(CXComment @Comment);

        [DllImport(libraryPath, EntryPoint = "clang_ParamCommandComment_isParamIndexValid", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint ParamCommandComment_isParamIndexValid(CXComment @Comment);

        [DllImport(libraryPath, EntryPoint = "clang_ParamCommandComment_getParamIndex", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint ParamCommandComment_getParamIndex(CXComment @Comment);

        [DllImport(libraryPath, EntryPoint = "clang_ParamCommandComment_isDirectionExplicit", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint ParamCommandComment_isDirectionExplicit(CXComment @Comment);

        [DllImport(libraryPath, EntryPoint = "clang_ParamCommandComment_getDirection", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXCommentParamPassDirection ParamCommandComment_getDirection(CXComment @Comment);

        [DllImport(libraryPath, EntryPoint = "clang_TParamCommandComment_getParamName", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString TParamCommandComment_getParamName(CXComment @Comment);

        [DllImport(libraryPath, EntryPoint = "clang_TParamCommandComment_isParamPositionValid", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint TParamCommandComment_isParamPositionValid(CXComment @Comment);

        [DllImport(libraryPath, EntryPoint = "clang_TParamCommandComment_getDepth", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint TParamCommandComment_getDepth(CXComment @Comment);

        [DllImport(libraryPath, EntryPoint = "clang_TParamCommandComment_getIndex", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint TParamCommandComment_getIndex(CXComment @Comment, uint @Depth);

        [DllImport(libraryPath, EntryPoint = "clang_VerbatimBlockLineComment_getText", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString VerbatimBlockLineComment_getText(CXComment @Comment);

        [DllImport(libraryPath, EntryPoint = "clang_VerbatimLineComment_getText", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString VerbatimLineComment_getText(CXComment @Comment);

        [DllImport(libraryPath, EntryPoint = "clang_HTMLTagComment_getAsString", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString HTMLTagComment_getAsString(CXComment @Comment);

        [DllImport(libraryPath, EntryPoint = "clang_FullComment_getAsHTML", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString FullComment_getAsHTML(CXComment @Comment);

        [DllImport(libraryPath, EntryPoint = "clang_FullComment_getAsXML", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString FullComment_getAsXML(CXComment @Comment);

        [DllImport(libraryPath, EntryPoint = "clang_CompilationDatabase_fromDirectory", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXCompilationDatabase CompilationDatabase_fromDirectory([MarshalAs(UnmanagedType.LPStr)] string @BuildDir, out CXCompilationDatabase_Error @ErrorCode);

        [DllImport(libraryPath, EntryPoint = "clang_CompilationDatabase_dispose", CallingConvention = CallingConvention.Cdecl)]
        public static extern void CompilationDatabase_dispose(CXCompilationDatabase @param0);

        [DllImport(libraryPath, EntryPoint = "clang_CompilationDatabase_getCompileCommands", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXCompileCommands CompilationDatabase_getCompileCommands(CXCompilationDatabase @param0, [MarshalAs(UnmanagedType.LPStr)] string @CompleteFileName);

        [DllImport(libraryPath, EntryPoint = "clang_CompilationDatabase_getAllCompileCommands", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXCompileCommands CompilationDatabase_getAllCompileCommands(CXCompilationDatabase @param0);

        [DllImport(libraryPath, EntryPoint = "clang_CompileCommands_dispose", CallingConvention = CallingConvention.Cdecl)]
        public static extern void CompileCommands_dispose(CXCompileCommands @param0);

        [DllImport(libraryPath, EntryPoint = "clang_CompileCommands_getSize", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint CompileCommands_getSize(CXCompileCommands @param0);

        [DllImport(libraryPath, EntryPoint = "clang_CompileCommands_getCommand", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXCompileCommand CompileCommands_getCommand(CXCompileCommands @param0, uint @I);

        [DllImport(libraryPath, EntryPoint = "clang_CompileCommand_getDirectory", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString CompileCommand_getDirectory(CXCompileCommand @param0);

        [DllImport(libraryPath, EntryPoint = "clang_CompileCommand_getFilename", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString CompileCommand_getFilename(CXCompileCommand @param0);

        [DllImport(libraryPath, EntryPoint = "clang_CompileCommand_getNumArgs", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint CompileCommand_getNumArgs(CXCompileCommand @param0);

        [DllImport(libraryPath, EntryPoint = "clang_CompileCommand_getArg", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString CompileCommand_getArg(CXCompileCommand @param0, uint @I);

        [DllImport(libraryPath, EntryPoint = "clang_CompileCommand_getNumMappedSources", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint CompileCommand_getNumMappedSources(CXCompileCommand @param0);

        [DllImport(libraryPath, EntryPoint = "clang_CompileCommand_getMappedSourcePath", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString CompileCommand_getMappedSourcePath(CXCompileCommand @param0, uint @I);

        [DllImport(libraryPath, EntryPoint = "clang_CompileCommand_getMappedSourceContent", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString CompileCommand_getMappedSourceContent(CXCompileCommand @param0, uint @I);

    }
}
