namespace ClangSharp
{
    using System;
    using System.Runtime.InteropServices;

    public partial struct __crt_locale_data_public
    {
        public IntPtr @_locale_pctype;
        public int @_locale_mb_cur_max;
        public uint @_locale_lc_codepage;
    }

    public partial struct __crt_locale_pointers
    {
        public partial struct __crt_locale_data
        {
        }

        public IntPtr @locinfo;
        public partial struct __crt_multibyte_data
        {
        }

        public IntPtr @mbcinfo;
    }

    public partial struct _Mbstatet
    {
        public int @_Wchar;
        public ushort @_Byte;
        public ushort @_State;
    }

    public partial struct tm
    {
        public int @tm_sec;
        public int @tm_min;
        public int @tm_hour;
        public int @tm_mday;
        public int @tm_mon;
        public int @tm_year;
        public int @tm_wday;
        public int @tm_yday;
        public int @tm_isdst;
    }

    public partial struct _timespec32
    {
        public int @tv_sec;
        public int @tv_nsec;
    }

    public partial struct _timespec64
    {
        public long @tv_sec;
        public int @tv_nsec;
    }

    public partial struct timespec
    {
        public long @tv_sec;
        public int @tv_nsec;
    }

    public partial struct CXString
    {
        public IntPtr @data;
        public uint @private_flags;
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

    public partial struct uintptr_t
    {
        public ulong Value;
    }

    public partial struct va_list
    {
        public IntPtr Value;
    }

    public partial struct size_t
    {
        public ulong Value;
    }

    public partial struct ptrdiff_t
    {
        public long Value;
    }

    public partial struct intptr_t
    {
        public long Value;
    }

    public partial struct errno_t
    {
        public int Value;
    }

    public partial struct wint_t
    {
        public ushort Value;
    }

    public partial struct wctype_t
    {
        public ushort Value;
    }

    public partial struct __time32_t
    {
        public int Value;
    }

    public partial struct __time64_t
    {
        public long Value;
    }

    public partial struct _locale_t
    {
        public _locale_t(IntPtr pointer)
        {
            this.Pointer = pointer;
        }

        public IntPtr Pointer;
    }

    public partial struct time_t
    {
        public long Value;
    }

    public partial struct rsize_t
    {
        public ulong Value;
    }

    public partial struct clock_t
    {
        public int Value;
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

    public enum _ : int
    {
        @__the_value = 0,
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
        @CXCursor_LastExpr = 146,
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
        @CXCursor_LastStmt = 247,
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
        @CXCursor_LastAttr = 415,
        @CXCursor_PreprocessingDirective = 500,
        @CXCursor_MacroDefinition = 501,
        @CXCursor_MacroExpansion = 502,
        @CXCursor_MacroInstantiation = 502,
        @CXCursor_InclusionDirective = 503,
        @CXCursor_FirstPreprocessing = 500,
        @CXCursor_LastPreprocessing = 503,
        @CXCursor_ModuleImportDecl = 600,
        @CXCursor_FirstExtraDecl = 600,
        @CXCursor_LastExtraDecl = 600,
    }

    public enum CXLinkageKind : int
    {
        @CXLinkage_Invalid = 0,
        @CXLinkage_NoLinkage = 1,
        @CXLinkage_Internal = 2,
        @CXLinkage_UniqueExternal = 3,
        @CXLinkage_External = 4,
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
        @CXCallingConv_PnaclCall = 8,
        @CXCallingConv_IntelOclBicc = 9,
        @CXCallingConv_X86_64Win64 = 10,
        @CXCallingConv_X86_64SysV = 11,
        @CXCallingConv_Invalid = 100,
        @CXCallingConv_Unexposed = 200,
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

    public static partial class Methods
    {
        private const string libraryPath = "libclang.dll";

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern void __va_start(out va_list @param0);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern void __security_init_cookie();

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern void __security_check_cookie(uintptr_t @_StackCookie);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern void __report_gsfailure(uintptr_t @_StackCookie);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern void _invalid_parameter_noinfo();

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern void _invalid_parameter_noinfo_noreturn();

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern void _invoke_watson(IntPtr @param0, IntPtr @param1, IntPtr @param2, uint @param3, uintptr_t @param4);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr _wasctime(out tm @_Tm);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern errno_t _wasctime_s(IntPtr @_Buffer, size_t @_SizeInWords, out tm @_Tm);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern size_t wcsftime(IntPtr @_Buffer, size_t @_SizeInWords, IntPtr @_Format, out tm @_Tm);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern size_t _wcsftime_l(IntPtr @_Buffer, size_t @_SizeInWords, IntPtr @_Format, out tm @_Tm, _locale_t @_Locale);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr _wctime32(out __time32_t @_Time);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern errno_t _wctime32_s(IntPtr @_Buffer, size_t @_SizeInWords, out __time32_t @_Time);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr _wctime64(out __time64_t @_Time);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern errno_t _wctime64_s(IntPtr @_Buffer, size_t @_SizeInWords, out __time64_t @_Time);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern errno_t _wstrdate_s(IntPtr @_Buffer, size_t @_SizeInWords);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr _wstrdate(IntPtr @_Buffer);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern errno_t _wstrtime_s(IntPtr @_Buffer, size_t @_SizeInWords);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr _wstrtime(IntPtr @_Buffer);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr _wctime(out time_t @_Time);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern errno_t _wctime_s(IntPtr @_Buffer, size_t @_SizeInWords, out time_t @_Time);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr __daylight();

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr __dstbias();

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr __timezone();

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr __tzname();

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern errno_t _get_daylight(out int @_Daylight);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern errno_t _get_dstbias(out int @_DaylightSavingsBias);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern errno_t _get_timezone(out int @_TimeZone);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern errno_t _get_tzname(out size_t @_ReturnValue, IntPtr @_Buffer, size_t @_SizeInBytes, int @_Index);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr asctime(out tm @_Tm);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern errno_t asctime_s(IntPtr @_Buffer, size_t @_SizeInBytes, out tm @_Tm);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern clock_t clock();

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr _ctime32(out __time32_t @_Time);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern errno_t _ctime32_s(IntPtr @_Buffer, size_t @_SizeInBytes, out __time32_t @_Time);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr _ctime64(out __time64_t @_Time);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern errno_t _ctime64_s(IntPtr @_Buffer, size_t @_SizeInBytes, out __time64_t @_Time);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern double _difftime32(__time32_t @_Time1, __time32_t @_Time2);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern double _difftime64(__time64_t @_Time1, __time64_t @_Time2);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr _gmtime32(out __time32_t @_Time);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern errno_t _gmtime32_s(out tm @_Tm, out __time32_t @_Time);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr _gmtime64(out __time64_t @_Time);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern errno_t _gmtime64_s(out tm @_Tm, out __time64_t @_Time);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr _localtime32(out __time32_t @_Time);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern errno_t _localtime32_s(out tm @_Tm, out __time32_t @_Time);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr _localtime64(out __time64_t @_Time);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern errno_t _localtime64_s(out tm @_Tm, out __time64_t @_Time);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern __time32_t _mkgmtime32(out tm @_Tm);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern __time64_t _mkgmtime64(out tm @_Tm);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern __time32_t _mktime32(out tm @_Tm);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern __time64_t _mktime64(out tm @_Tm);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern size_t strftime(IntPtr @_Buffer, size_t @_SizeInBytes, [MarshalAs(UnmanagedType.LPStr)] string @_Format, out tm @_Tm);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern size_t _strftime_l(IntPtr @_Buffer, size_t @_MaxSize, [MarshalAs(UnmanagedType.LPStr)] string @_Format, out tm @_Tm, _locale_t @_Locale);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern errno_t _strdate_s(IntPtr @_Buffer, size_t @_SizeInBytes);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr _strdate(IntPtr @_Buffer);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern errno_t _strtime_s(IntPtr @_Buffer, size_t @_SizeInBytes);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr _strtime(IntPtr @_Buffer);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern __time32_t _time32(out __time32_t @_Time);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern __time64_t _time64(out __time64_t @_Time);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern int _timespec32_get(out _timespec32 @_Ts, int @_Base);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern int _timespec64_get(out _timespec64 @_Ts, int @_Base);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern void _tzset();

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint _getsystime(out tm @_Tm);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint _setsystime(out tm @_Tm, uint @_Milliseconds);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr ctime(out time_t @_Time);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern double difftime(time_t @_Time1, time_t @_Time2);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr gmtime(out time_t @_Time);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr localtime(out time_t @_Time);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern time_t _mkgmtime(out tm @_Tm);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern time_t mktime(out tm @_Tm);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern time_t time(out time_t @_Time);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern int timespec_get(out timespec @_Ts, int @_Base);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern errno_t ctime_s(IntPtr @_Buffer, size_t @_SizeInBytes, out time_t @_Time);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern errno_t gmtime_s(out tm @_Tm, out time_t @_Time);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern errno_t localtime_s(out tm @_Tm, out time_t @_Time);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern void tzset();

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern string clang_getCString(CXString @string);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern void clang_disposeString(CXString @string);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern ulong clang_getBuildSessionTimestamp();

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern CXVirtualFileOverlay clang_VirtualFileOverlay_create(uint @options);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern CXErrorCode clang_VirtualFileOverlay_addFileMapping(CXVirtualFileOverlay @param0, [MarshalAs(UnmanagedType.LPStr)] string @virtualPath, [MarshalAs(UnmanagedType.LPStr)] string @realPath);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern CXErrorCode clang_VirtualFileOverlay_setCaseSensitivity(CXVirtualFileOverlay @param0, int @caseSensitive);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern CXErrorCode clang_VirtualFileOverlay_writeToBuffer(CXVirtualFileOverlay @param0, uint @options, out IntPtr @out_buffer_ptr, out uint @out_buffer_size);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern void clang_VirtualFileOverlay_dispose(CXVirtualFileOverlay @param0);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern CXModuleMapDescriptor clang_ModuleMapDescriptor_create(uint @options);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern CXErrorCode clang_ModuleMapDescriptor_setFrameworkModuleName(CXModuleMapDescriptor @param0, [MarshalAs(UnmanagedType.LPStr)] string @name);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern CXErrorCode clang_ModuleMapDescriptor_setUmbrellaHeader(CXModuleMapDescriptor @param0, [MarshalAs(UnmanagedType.LPStr)] string @name);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern CXErrorCode clang_ModuleMapDescriptor_writeToBuffer(CXModuleMapDescriptor @param0, uint @options, out IntPtr @out_buffer_ptr, out uint @out_buffer_size);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern void clang_ModuleMapDescriptor_dispose(CXModuleMapDescriptor @param0);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern CXIndex clang_createIndex(int @excludeDeclarationsFromPCH, int @displayDiagnostics);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern void clang_disposeIndex(CXIndex @index);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern void clang_CXIndex_setGlobalOptions(CXIndex @param0, uint @options);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint clang_CXIndex_getGlobalOptions(CXIndex @param0);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString clang_getFileName(CXFile @SFile);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern time_t clang_getFileTime(CXFile @SFile);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern int clang_getFileUniqueID(CXFile @file, out CXFileUniqueID @outID);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint clang_isFileMultipleIncludeGuarded(CXTranslationUnit @tu, CXFile @file);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern CXFile clang_getFile(CXTranslationUnit @tu, [MarshalAs(UnmanagedType.LPStr)] string @file_name);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern CXSourceLocation clang_getNullLocation();

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint clang_equalLocations(CXSourceLocation @loc1, CXSourceLocation @loc2);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern CXSourceLocation clang_getLocation(CXTranslationUnit @tu, CXFile @file, uint @line, uint @column);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern CXSourceLocation clang_getLocationForOffset(CXTranslationUnit @tu, CXFile @file, uint @offset);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern int clang_Location_isInSystemHeader(CXSourceLocation @location);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern int clang_Location_isFromMainFile(CXSourceLocation @location);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern CXSourceRange clang_getNullRange();

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern CXSourceRange clang_getRange(CXSourceLocation @begin, CXSourceLocation @end);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint clang_equalRanges(CXSourceRange @range1, CXSourceRange @range2);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern int clang_Range_isNull(CXSourceRange @range);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern void clang_getExpansionLocation(CXSourceLocation @location, out CXFile @file, out uint @line, out uint @column, out uint @offset);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern void clang_getPresumedLocation(CXSourceLocation @location, out CXString @filename, out uint @line, out uint @column);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern void clang_getInstantiationLocation(CXSourceLocation @location, out CXFile @file, out uint @line, out uint @column, out uint @offset);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern void clang_getSpellingLocation(CXSourceLocation @location, out CXFile @file, out uint @line, out uint @column, out uint @offset);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern void clang_getFileLocation(CXSourceLocation @location, out CXFile @file, out uint @line, out uint @column, out uint @offset);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern CXSourceLocation clang_getRangeStart(CXSourceRange @range);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern CXSourceLocation clang_getRangeEnd(CXSourceRange @range);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr clang_getSkippedRanges(CXTranslationUnit @tu, CXFile @file);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern void clang_disposeSourceRangeList(out CXSourceRangeList @ranges);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint clang_getNumDiagnosticsInSet(CXDiagnosticSet @Diags);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern CXDiagnostic clang_getDiagnosticInSet(CXDiagnosticSet @Diags, uint @Index);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern CXDiagnosticSet clang_loadDiagnostics([MarshalAs(UnmanagedType.LPStr)] string @file, out CXLoadDiag_Error @error, out CXString @errorString);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern void clang_disposeDiagnosticSet(CXDiagnosticSet @Diags);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern CXDiagnosticSet clang_getChildDiagnostics(CXDiagnostic @D);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint clang_getNumDiagnostics(CXTranslationUnit @Unit);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern CXDiagnostic clang_getDiagnostic(CXTranslationUnit @Unit, uint @Index);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern CXDiagnosticSet clang_getDiagnosticSetFromTU(CXTranslationUnit @Unit);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern void clang_disposeDiagnostic(CXDiagnostic @Diagnostic);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString clang_formatDiagnostic(CXDiagnostic @Diagnostic, uint @Options);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint clang_defaultDiagnosticDisplayOptions();

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern CXDiagnosticSeverity clang_getDiagnosticSeverity(CXDiagnostic @param0);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern CXSourceLocation clang_getDiagnosticLocation(CXDiagnostic @param0);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString clang_getDiagnosticSpelling(CXDiagnostic @param0);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString clang_getDiagnosticOption(CXDiagnostic @Diag, out CXString @Disable);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint clang_getDiagnosticCategory(CXDiagnostic @param0);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString clang_getDiagnosticCategoryName(uint @Category);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString clang_getDiagnosticCategoryText(CXDiagnostic @param0);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint clang_getDiagnosticNumRanges(CXDiagnostic @param0);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern CXSourceRange clang_getDiagnosticRange(CXDiagnostic @Diagnostic, uint @Range);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint clang_getDiagnosticNumFixIts(CXDiagnostic @Diagnostic);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString clang_getDiagnosticFixIt(CXDiagnostic @Diagnostic, uint @FixIt, out CXSourceRange @ReplacementRange);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString clang_getTranslationUnitSpelling(CXTranslationUnit @CTUnit);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern CXTranslationUnit clang_createTranslationUnitFromSourceFile(CXIndex @CIdx, [MarshalAs(UnmanagedType.LPStr)] string @source_filename, int @num_clang_command_line_args, string[] @clang_command_line_args, uint @num_unsaved_files, out CXUnsavedFile @unsaved_files);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern CXTranslationUnit clang_createTranslationUnit(CXIndex @CIdx, [MarshalAs(UnmanagedType.LPStr)] string @ast_filename);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern CXErrorCode clang_createTranslationUnit2(CXIndex @CIdx, [MarshalAs(UnmanagedType.LPStr)] string @ast_filename, out CXTranslationUnit @out_TU);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint clang_defaultEditingTranslationUnitOptions();

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern CXTranslationUnit clang_parseTranslationUnit(CXIndex @CIdx, [MarshalAs(UnmanagedType.LPStr)] string @source_filename, string[] @command_line_args, int @num_command_line_args, out CXUnsavedFile @unsaved_files, uint @num_unsaved_files, uint @options);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern CXErrorCode clang_parseTranslationUnit2(CXIndex @CIdx, [MarshalAs(UnmanagedType.LPStr)] string @source_filename, string[] @command_line_args, int @num_command_line_args, out CXUnsavedFile @unsaved_files, uint @num_unsaved_files, uint @options, out CXTranslationUnit @out_TU);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint clang_defaultSaveOptions(CXTranslationUnit @TU);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern int clang_saveTranslationUnit(CXTranslationUnit @TU, [MarshalAs(UnmanagedType.LPStr)] string @FileName, uint @options);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern void clang_disposeTranslationUnit(CXTranslationUnit @param0);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint clang_defaultReparseOptions(CXTranslationUnit @TU);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern int clang_reparseTranslationUnit(CXTranslationUnit @TU, uint @num_unsaved_files, out CXUnsavedFile @unsaved_files, uint @options);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern string clang_getTUResourceUsageName(CXTUResourceUsageKind @kind);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern CXTUResourceUsage clang_getCXTUResourceUsage(CXTranslationUnit @TU);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern void clang_disposeCXTUResourceUsage(CXTUResourceUsage @usage);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern CXCursor clang_getNullCursor();

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern CXCursor clang_getTranslationUnitCursor(CXTranslationUnit @param0);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint clang_equalCursors(CXCursor @param0, CXCursor @param1);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern int clang_Cursor_isNull(CXCursor @cursor);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint clang_hashCursor(CXCursor @param0);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern CXCursorKind clang_getCursorKind(CXCursor @param0);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint clang_isDeclaration(CXCursorKind @param0);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint clang_isReference(CXCursorKind @param0);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint clang_isExpression(CXCursorKind @param0);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint clang_isStatement(CXCursorKind @param0);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint clang_isAttribute(CXCursorKind @param0);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint clang_isInvalid(CXCursorKind @param0);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint clang_isTranslationUnit(CXCursorKind @param0);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint clang_isPreprocessing(CXCursorKind @param0);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint clang_isUnexposed(CXCursorKind @param0);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern CXLinkageKind clang_getCursorLinkage(CXCursor @cursor);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern CXAvailabilityKind clang_getCursorAvailability(CXCursor @cursor);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern int clang_getCursorPlatformAvailability(CXCursor @cursor, out int @always_deprecated, out CXString @deprecated_message, out int @always_unavailable, out CXString @unavailable_message, out CXPlatformAvailability @availability, int @availability_size);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern void clang_disposeCXPlatformAvailability(out CXPlatformAvailability @availability);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern CXLanguageKind clang_getCursorLanguage(CXCursor @cursor);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern CXTranslationUnit clang_Cursor_getTranslationUnit(CXCursor @param0);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern CXCursorSet clang_createCXCursorSet();

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern void clang_disposeCXCursorSet(CXCursorSet @cset);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint clang_CXCursorSet_contains(CXCursorSet @cset, CXCursor @cursor);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint clang_CXCursorSet_insert(CXCursorSet @cset, CXCursor @cursor);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern CXCursor clang_getCursorSemanticParent(CXCursor @cursor);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern CXCursor clang_getCursorLexicalParent(CXCursor @cursor);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern void clang_getOverriddenCursors(CXCursor @cursor, out IntPtr @overridden, out uint @num_overridden);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern void clang_disposeOverriddenCursors(out CXCursor @overridden);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern CXFile clang_getIncludedFile(CXCursor @cursor);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern CXCursor clang_getCursor(CXTranslationUnit @param0, CXSourceLocation @param1);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern CXSourceLocation clang_getCursorLocation(CXCursor @param0);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern CXSourceRange clang_getCursorExtent(CXCursor @param0);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern CXType clang_getCursorType(CXCursor @C);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString clang_getTypeSpelling(CXType @CT);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern CXType clang_getTypedefDeclUnderlyingType(CXCursor @C);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern CXType clang_getEnumDeclIntegerType(CXCursor @C);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern long clang_getEnumConstantDeclValue(CXCursor @C);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern ulong clang_getEnumConstantDeclUnsignedValue(CXCursor @C);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern int clang_getFieldDeclBitWidth(CXCursor @C);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern int clang_Cursor_getNumArguments(CXCursor @C);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern CXCursor clang_Cursor_getArgument(CXCursor @C, uint @i);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint clang_equalTypes(CXType @A, CXType @B);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern CXType clang_getCanonicalType(CXType @T);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint clang_isConstQualifiedType(CXType @T);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint clang_isVolatileQualifiedType(CXType @T);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint clang_isRestrictQualifiedType(CXType @T);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern CXType clang_getPointeeType(CXType @T);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern CXCursor clang_getTypeDeclaration(CXType @T);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString clang_getDeclObjCTypeEncoding(CXCursor @C);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString clang_getTypeKindSpelling(CXTypeKind @K);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern CXCallingConv clang_getFunctionTypeCallingConv(CXType @T);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern CXType clang_getResultType(CXType @T);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern int clang_getNumArgTypes(CXType @T);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern CXType clang_getArgType(CXType @T, uint @i);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint clang_isFunctionTypeVariadic(CXType @T);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern CXType clang_getCursorResultType(CXCursor @C);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint clang_isPODType(CXType @T);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern CXType clang_getElementType(CXType @T);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern long clang_getNumElements(CXType @T);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern CXType clang_getArrayElementType(CXType @T);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern long clang_getArraySize(CXType @T);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern long clang_Type_getAlignOf(CXType @T);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern CXType clang_Type_getClassType(CXType @T);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern long clang_Type_getSizeOf(CXType @T);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern long clang_Type_getOffsetOf(CXType @T, [MarshalAs(UnmanagedType.LPStr)] string @S);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern int clang_Type_getNumTemplateArguments(CXType @T);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern CXType clang_Type_getTemplateArgumentAsType(CXType @T, uint @i);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern CXRefQualifierKind clang_Type_getCXXRefQualifier(CXType @T);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint clang_Cursor_isBitField(CXCursor @C);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint clang_isVirtualBase(CXCursor @param0);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern CX_CXXAccessSpecifier clang_getCXXAccessSpecifier(CXCursor @param0);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint clang_getNumOverloadedDecls(CXCursor @cursor);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern CXCursor clang_getOverloadedDecl(CXCursor @cursor, uint @index);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern CXType clang_getIBOutletCollectionType(CXCursor @param0);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint clang_visitChildren(CXCursor @parent, CXCursorVisitor @visitor, CXClientData @client_data);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString clang_getCursorUSR(CXCursor @param0);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString clang_constructUSR_ObjCClass([MarshalAs(UnmanagedType.LPStr)] string @class_name);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString clang_constructUSR_ObjCCategory([MarshalAs(UnmanagedType.LPStr)] string @class_name, [MarshalAs(UnmanagedType.LPStr)] string @category_name);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString clang_constructUSR_ObjCProtocol([MarshalAs(UnmanagedType.LPStr)] string @protocol_name);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString clang_constructUSR_ObjCIvar([MarshalAs(UnmanagedType.LPStr)] string @name, CXString @classUSR);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString clang_constructUSR_ObjCMethod([MarshalAs(UnmanagedType.LPStr)] string @name, uint @isInstanceMethod, CXString @classUSR);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString clang_constructUSR_ObjCProperty([MarshalAs(UnmanagedType.LPStr)] string @property, CXString @classUSR);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString clang_getCursorSpelling(CXCursor @param0);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern CXSourceRange clang_Cursor_getSpellingNameRange(CXCursor @param0, uint @pieceIndex, uint @options);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString clang_getCursorDisplayName(CXCursor @param0);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern CXCursor clang_getCursorReferenced(CXCursor @param0);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern CXCursor clang_getCursorDefinition(CXCursor @param0);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint clang_isCursorDefinition(CXCursor @param0);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern CXCursor clang_getCanonicalCursor(CXCursor @param0);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern int clang_Cursor_getObjCSelectorIndex(CXCursor @param0);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern int clang_Cursor_isDynamicCall(CXCursor @C);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern CXType clang_Cursor_getReceiverType(CXCursor @C);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint clang_Cursor_getObjCPropertyAttributes(CXCursor @C, uint @reserved);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint clang_Cursor_getObjCDeclQualifiers(CXCursor @C);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint clang_Cursor_isObjCOptional(CXCursor @C);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint clang_Cursor_isVariadic(CXCursor @C);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern CXSourceRange clang_Cursor_getCommentRange(CXCursor @C);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString clang_Cursor_getRawCommentText(CXCursor @C);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString clang_Cursor_getBriefCommentText(CXCursor @C);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern CXModule clang_Cursor_getModule(CXCursor @C);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern CXModule clang_getModuleForFile(CXTranslationUnit @param0, CXFile @param1);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern CXFile clang_Module_getASTFile(CXModule @Module);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern CXModule clang_Module_getParent(CXModule @Module);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString clang_Module_getName(CXModule @Module);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString clang_Module_getFullName(CXModule @Module);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern int clang_Module_isSystem(CXModule @Module);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint clang_Module_getNumTopLevelHeaders(CXTranslationUnit @param0, CXModule @Module);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern CXFile clang_Module_getTopLevelHeader(CXTranslationUnit @param0, CXModule @Module, uint @Index);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint clang_CXXMethod_isPureVirtual(CXCursor @C);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint clang_CXXMethod_isStatic(CXCursor @C);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint clang_CXXMethod_isVirtual(CXCursor @C);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint clang_CXXMethod_isConst(CXCursor @C);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern CXCursorKind clang_getTemplateCursorKind(CXCursor @C);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern CXCursor clang_getSpecializedCursorTemplate(CXCursor @C);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern CXSourceRange clang_getCursorReferenceNameRange(CXCursor @C, uint @NameFlags, uint @PieceIndex);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern CXTokenKind clang_getTokenKind(CXToken @param0);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString clang_getTokenSpelling(CXTranslationUnit @param0, CXToken @param1);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern CXSourceLocation clang_getTokenLocation(CXTranslationUnit @param0, CXToken @param1);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern CXSourceRange clang_getTokenExtent(CXTranslationUnit @param0, CXToken @param1);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern void clang_tokenize(CXTranslationUnit @TU, CXSourceRange @Range, out IntPtr @Tokens, out uint @NumTokens);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern void clang_annotateTokens(CXTranslationUnit @TU, out CXToken @Tokens, uint @NumTokens, out CXCursor @Cursors);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern void clang_disposeTokens(CXTranslationUnit @TU, out CXToken @Tokens, uint @NumTokens);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString clang_getCursorKindSpelling(CXCursorKind @Kind);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern void clang_getDefinitionSpellingAndExtent(CXCursor @param0, out IntPtr @startBuf, out IntPtr @endBuf, out uint @startLine, out uint @startColumn, out uint @endLine, out uint @endColumn);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern void clang_enableStackTraces();

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern void clang_executeOnThread(out IntPtr @fn, IntPtr @user_data, uint @stack_size);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern CXCompletionChunkKind clang_getCompletionChunkKind(CXCompletionString @completion_string, uint @chunk_number);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString clang_getCompletionChunkText(CXCompletionString @completion_string, uint @chunk_number);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern CXCompletionString clang_getCompletionChunkCompletionString(CXCompletionString @completion_string, uint @chunk_number);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint clang_getNumCompletionChunks(CXCompletionString @completion_string);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint clang_getCompletionPriority(CXCompletionString @completion_string);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern CXAvailabilityKind clang_getCompletionAvailability(CXCompletionString @completion_string);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint clang_getCompletionNumAnnotations(CXCompletionString @completion_string);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString clang_getCompletionAnnotation(CXCompletionString @completion_string, uint @annotation_number);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString clang_getCompletionParent(CXCompletionString @completion_string, out CXCursorKind @kind);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString clang_getCompletionBriefComment(CXCompletionString @completion_string);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern CXCompletionString clang_getCursorCompletionString(CXCursor @cursor);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint clang_defaultCodeCompleteOptions();

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr clang_codeCompleteAt(CXTranslationUnit @TU, [MarshalAs(UnmanagedType.LPStr)] string @complete_filename, uint @complete_line, uint @complete_column, out CXUnsavedFile @unsaved_files, uint @num_unsaved_files, uint @options);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern void clang_sortCodeCompletionResults(out CXCompletionResult @Results, uint @NumResults);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern void clang_disposeCodeCompleteResults(out CXCodeCompleteResults @Results);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint clang_codeCompleteGetNumDiagnostics(out CXCodeCompleteResults @Results);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern CXDiagnostic clang_codeCompleteGetDiagnostic(out CXCodeCompleteResults @Results, uint @Index);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern ulong clang_codeCompleteGetContexts(out CXCodeCompleteResults @Results);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern CXCursorKind clang_codeCompleteGetContainerKind(out CXCodeCompleteResults @Results, out uint @IsIncomplete);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString clang_codeCompleteGetContainerUSR(out CXCodeCompleteResults @Results);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString clang_codeCompleteGetObjCSelector(out CXCodeCompleteResults @Results);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString clang_getClangVersion();

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern void clang_toggleCrashRecovery(uint @isEnabled);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern void clang_getInclusions(CXTranslationUnit @tu, CXInclusionVisitor @visitor, CXClientData @client_data);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern CXRemapping clang_getRemappings([MarshalAs(UnmanagedType.LPStr)] string @path);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern CXRemapping clang_getRemappingsFromFileList(out IntPtr @filePaths, uint @numFiles);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint clang_remap_getNumFiles(CXRemapping @param0);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern void clang_remap_getFilenames(CXRemapping @param0, uint @index, out CXString @original, out CXString @transformed);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern void clang_remap_dispose(CXRemapping @param0);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern CXResult clang_findReferencesInFile(CXCursor @cursor, CXFile @file, CXCursorAndRangeVisitor @visitor);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern CXResult clang_findIncludesInFile(CXTranslationUnit @TU, CXFile @file, CXCursorAndRangeVisitor @visitor);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern int clang_index_isEntityObjCContainerKind(CXIdxEntityKind @param0);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr clang_index_getObjCContainerDeclInfo(out CXIdxDeclInfo @param0);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr clang_index_getObjCInterfaceDeclInfo(out CXIdxDeclInfo @param0);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr clang_index_getObjCCategoryDeclInfo(out CXIdxDeclInfo @param0);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr clang_index_getObjCProtocolRefListInfo(out CXIdxDeclInfo @param0);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr clang_index_getObjCPropertyDeclInfo(out CXIdxDeclInfo @param0);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr clang_index_getIBOutletCollectionAttrInfo(out CXIdxAttrInfo @param0);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr clang_index_getCXXClassDeclInfo(out CXIdxDeclInfo @param0);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern CXIdxClientContainer clang_index_getClientContainer(out CXIdxContainerInfo @param0);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern void clang_index_setClientContainer(out CXIdxContainerInfo @param0, CXIdxClientContainer @param1);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern CXIdxClientEntity clang_index_getClientEntity(out CXIdxEntityInfo @param0);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern void clang_index_setClientEntity(out CXIdxEntityInfo @param0, CXIdxClientEntity @param1);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern CXIndexAction clang_IndexAction_create(CXIndex @CIdx);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern void clang_IndexAction_dispose(CXIndexAction @param0);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern int clang_indexSourceFile(CXIndexAction @param0, CXClientData @client_data, out IndexerCallbacks @index_callbacks, uint @index_callbacks_size, uint @index_options, [MarshalAs(UnmanagedType.LPStr)] string @source_filename, string[] @command_line_args, int @num_command_line_args, out CXUnsavedFile @unsaved_files, uint @num_unsaved_files, out CXTranslationUnit @out_TU, uint @TU_options);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern int clang_indexTranslationUnit(CXIndexAction @param0, CXClientData @client_data, out IndexerCallbacks @index_callbacks, uint @index_callbacks_size, uint @index_options, CXTranslationUnit @param5);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern void clang_indexLoc_getFileLocation(CXIdxLoc @loc, out CXIdxClientFile @indexFile, out CXFile @file, out uint @line, out uint @column, out uint @offset);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern CXSourceLocation clang_indexLoc_getCXSourceLocation(CXIdxLoc @loc);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern CXComment clang_Cursor_getParsedComment(CXCursor @C);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern CXCommentKind clang_Comment_getKind(CXComment @Comment);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint clang_Comment_getNumChildren(CXComment @Comment);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern CXComment clang_Comment_getChild(CXComment @Comment, uint @ChildIdx);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint clang_Comment_isWhitespace(CXComment @Comment);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint clang_InlineContentComment_hasTrailingNewline(CXComment @Comment);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString clang_TextComment_getText(CXComment @Comment);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString clang_InlineCommandComment_getCommandName(CXComment @Comment);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern CXCommentInlineCommandRenderKind clang_InlineCommandComment_getRenderKind(CXComment @Comment);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint clang_InlineCommandComment_getNumArgs(CXComment @Comment);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString clang_InlineCommandComment_getArgText(CXComment @Comment, uint @ArgIdx);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString clang_HTMLTagComment_getTagName(CXComment @Comment);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint clang_HTMLStartTagComment_isSelfClosing(CXComment @Comment);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint clang_HTMLStartTag_getNumAttrs(CXComment @Comment);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString clang_HTMLStartTag_getAttrName(CXComment @Comment, uint @AttrIdx);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString clang_HTMLStartTag_getAttrValue(CXComment @Comment, uint @AttrIdx);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString clang_BlockCommandComment_getCommandName(CXComment @Comment);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint clang_BlockCommandComment_getNumArgs(CXComment @Comment);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString clang_BlockCommandComment_getArgText(CXComment @Comment, uint @ArgIdx);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern CXComment clang_BlockCommandComment_getParagraph(CXComment @Comment);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString clang_ParamCommandComment_getParamName(CXComment @Comment);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint clang_ParamCommandComment_isParamIndexValid(CXComment @Comment);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint clang_ParamCommandComment_getParamIndex(CXComment @Comment);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint clang_ParamCommandComment_isDirectionExplicit(CXComment @Comment);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern CXCommentParamPassDirection clang_ParamCommandComment_getDirection(CXComment @Comment);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString clang_TParamCommandComment_getParamName(CXComment @Comment);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint clang_TParamCommandComment_isParamPositionValid(CXComment @Comment);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint clang_TParamCommandComment_getDepth(CXComment @Comment);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint clang_TParamCommandComment_getIndex(CXComment @Comment, uint @Depth);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString clang_VerbatimBlockLineComment_getText(CXComment @Comment);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString clang_VerbatimLineComment_getText(CXComment @Comment);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString clang_HTMLTagComment_getAsString(CXComment @Comment);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString clang_FullComment_getAsHTML(CXComment @Comment);

        [DllImport(libraryPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString clang_FullComment_getAsXML(CXComment @Comment);

    }
}
