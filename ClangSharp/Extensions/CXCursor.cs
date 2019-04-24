﻿using System;

namespace ClangSharp
{
    public partial struct CXCursor : IEquatable<CXCursor>
    {
        public static CXCursor Null => clang.getNullCursor();

        public CXAvailabilityKind Availability => clang.getCursorAvailability(this);

        public CXString BriefCommentText => clang.Cursor_getBriefCommentText(this);

        public CXCursor CanonicalCursor => clang.getCanonicalCursor(this);

        public CXSourceRange CommentRange => clang.Cursor_getCommentRange(this);

        public CXCompletionString CompletionString => clang.getCursorCompletionString(this);

        public CX_CXXAccessSpecifier CXXAccessSpecifier => clang.getCXXAccessSpecifier(this);

        public bool CXXConstructor_IsConvertingConstructor => clang.CXXConstructor_isConvertingConstructor(this) != 0;

        public bool CXXConstructor_IsCopyConstructor => clang.CXXConstructor_isCopyConstructor(this) != 0;

        public bool CXXConstructor_IsDefaultConstructor => clang.CXXConstructor_isDefaultConstructor(this) != 0;

        public bool CXXConstructor_IsMoveConstructor => clang.CXXConstructor_isMoveConstructor(this) != 0;

        public bool CXXField_IsMutable => clang.CXXField_isMutable(this) != 0;

        public unsafe ref CXStringSet CXXManglings => ref *(CXStringSet*)clang.Cursor_getCXXManglings(this);

        public bool CXXMethod_IsConst => clang.CXXMethod_isConst(this) != 0;

        public bool CXXMethod_IsDefaulted => clang.CXXMethod_isDefaulted(this) != 0;

        public bool CXXMethod_IsPureVirtual => clang.CXXMethod_isPureVirtual(this) != 0;

        public bool CXXMethod_IsStatic => clang.CXXMethod_isStatic(this) != 0;

        public bool CXXMethod_IsVirtual => clang.CXXMethod_isVirtual(this) != 0;

        public bool CXXRecord_IsAbstract => clang.CXXRecord_isAbstract(this) != 0;

        public CXString DeclObjCTypeEncoding => clang.getDeclObjCTypeEncoding(this);

        public CXCursor Definition => clang.getCursorDefinition(this);

        public CXString DisplayName => clang.getCursorDisplayName(this);

        public ulong EnumConstantDeclUnsignedValue => clang.getEnumConstantDeclUnsignedValue(this);

        public long EnumConstantDeclValue => clang.getEnumConstantDeclValue(this);

        public CXType EnumDecl_IntegerType => clang.getEnumDeclIntegerType(this);

        public bool EnumDecl_IsScoped => clang.EnumDecl_isScoped(this) != 0;

        public CXEvalResult Evaluate => clang.Cursor_Evaluate(this);

        public int ExceptionSpecificationType => clang.getCursorExceptionSpecificationType(this);

        public CXSourceRange Extent => clang.getCursorExtent(this);

        public int FieldDeclBitWidth => clang.getFieldDeclBitWidth(this);

        public bool HasAttrs => clang.Cursor_hasAttrs(this) != 0;

        public CXType IBOutletCollectionType => clang.getIBOutletCollectionType(this);

        public CXFile IncludedFile => clang.getIncludedFile(this);

        public bool IsAnonymous => clang.Cursor_isAnonymous(this) != 0;

        public bool IsAttribute => clang.isAttribute(Kind) != 0;

        public bool IsBitField => clang.Cursor_isBitField(this) != 0;

        public bool IsCanonical => this.Equals(CanonicalCursor);

        public bool IsDeclaration => clang.isDeclaration(Kind) != 0;

        public bool IsDefinition => clang.isCursorDefinition(this) != 0;

        public bool IsDynamicCall => clang.Cursor_isDynamicCall(this) != 0;

        public bool IsExpression => clang.isExpression(Kind) != 0;

        public bool IsFunctionInlined => clang.Cursor_isFunctionInlined(this) != 0;

        public bool IsInvalid => clang.isInvalid(Kind) != 0;

        public bool IsInvalidDeclaration => clang.isInvalidDeclaration(this) != 0;

        public bool IsNull => clang.Cursor_isNull(this) != 0;

        public bool IsMacroBuiltIn => clang.Cursor_isMacroBuiltin(this) != 0;

        public bool IsMacroFunctionLike => clang.Cursor_isMacroFunctionLike(this) != 0;

        public bool IsObjCOptional => clang.Cursor_isObjCOptional(this) != 0;

        public bool IsPreprocessing => clang.isPreprocessing(Kind) != 0;

        public bool IsReference => clang.isReference(Kind) != 0;

        public bool IsStatement => clang.isStatement(Kind) != 0;

        public bool IsTranslationUnit => clang.isTranslationUnit(Kind) != 0;

        public bool IsUnexposed => clang.isUnexposed(Kind) != 0;

        public bool IsVariadic => clang.Cursor_isVariadic(this) != 0;

        public bool IsVirtualBase => clang.isVirtualBase(this) != 0;

        public CXCursorKind Kind => clang.getCursorKind(this);

        public CXString KindSpelling => clang.getCursorKindSpelling(Kind);

        public CXLanguageKind Language => clang.getCursorLanguage(this);

        public CXCursor LexicalParent => clang.getCursorLexicalParent(this);

        public CXLinkageKind Linkage => clang.getCursorLinkage(this);

        public CXSourceLocation Location => clang.getCursorLocation(this);

        public CXString Mangling => clang.Cursor_getMangling(this);

        public CXModule Module => clang.Cursor_getModule(this);

        public int NumArguments => clang.Cursor_getNumArguments(this);

        public uint NumOverloadedDecls => clang.getNumOverloadedDecls(this);

        public int NumTemplateArguments => clang.Cursor_getNumTemplateArguments(this);

        public CXObjCDeclQualifierKind ObjCDeclQualifiers => (CXObjCDeclQualifierKind)clang.Cursor_getObjCDeclQualifiers(this);

        public unsafe ref CXStringSet ObjCManglings => ref *(CXStringSet*)clang.Cursor_getObjCManglings(this);

        public CXString ObjCPropertyGetterName => clang.Cursor_getObjCPropertyGetterName(this);

        public CXString ObjCPropertySetterName => clang.Cursor_getObjCPropertySetterName(this);

        public int ObjCSelectorIndex => clang.Cursor_getObjCSelectorIndex(this);

        public long OffsetOfField => clang.Cursor_getOffsetOfField(this);

        public CXComment ParsedComment => clang.Cursor_getParsedComment(this);

        public CXString RawCommentText => clang.Cursor_getRawCommentText(this);

        public CXCursor Referenced => clang.getCursorReferenced(this);

        public CXType ResultType => clang.getCursorResultType(this);

        public CXCursor SemanticParent => clang.getCursorSemanticParent(this);

        public CXCursor SpecializedCursorTemplate => clang.getSpecializedCursorTemplate(this);

        public CXString Spelling => clang.getCursorSpelling(this);

        public CX_StorageClass StorageClass => clang.Cursor_getStorageClass(this);

        public CXCursorKind TemplateCursorKind => clang.getTemplateCursorKind(this);

        public CXTLSKind TlsKind => clang.getCursorTLSKind(this);

        public CXTranslationUnit TranslationUnit => clang.Cursor_getTranslationUnit(this);

        public CXType Type => clang.getCursorType(this);

        public CXType TypedefDeclUnderlyingType => clang.getTypedefDeclUnderlyingType(this);

        public CXString UnifiedSymbolResolution => clang.getCursorUSR(this);

        public CXVisibilityKind Visibility => clang.getCursorVisibility(this);

        public override bool Equals(object obj) => (obj is CXCursor other) && Equals(other);

        public bool Equals(CXCursor other) => clang.equalCursors(this, other) != 0;

        public CXResult FindReferenceInFile(CXFile file, CXCursorAndRangeVisitor visitor) => clang.findReferencesInFile(this, file, visitor);

        public CXCursor GetArgument(uint index) => clang.Cursor_getArgument(this, index);

        public override int GetHashCode() => (int)clang.hashCursor(this);

        public bool GetIsExternalSymbol(out CXString language, out CXString definedIn, out bool isGenerated)
        {
            var result = clang.Cursor_isExternalSymbol(this, out language, out definedIn, out uint isGeneratedOut);
            isGenerated = isGeneratedOut != 0;
            return result != 0;
        }

        public CXObjCPropertyAttrKind GetObjCPropertyAttributes(uint reserved) => (CXObjCPropertyAttrKind)clang.Cursor_getObjCPropertyAttributes(this, reserved);

        public CXCursor GetOverloadedDecl(uint index) => clang.getOverloadedDecl(this, index);

        public int GetPlatformAvailability(out bool alwaysDeprecated, out CXString deprecatedMessage, out bool alwaysUnavailable, out CXString unavailableMessage, CXPlatformAvailability[] availability) => clang.getCursorPlatformAvailability(this, out alwaysDeprecated, out deprecatedMessage, out alwaysUnavailable, out unavailableMessage, availability, availability.Length);

        public CXType GetRecieverType() => clang.Cursor_getReceiverType(this);

        public CXSourceRange GetReferenceNameRange(CXNameRefFlags nameFlags, uint pieceIndex) => clang.getCursorReferenceNameRange(this, (uint)nameFlags, pieceIndex);

        public CXSourceRange GetSpellingNameRange(uint pieceIndex, uint options) => clang.Cursor_getSpellingNameRange(this, pieceIndex, options);

        public CXTemplateArgumentKind GetTemplateArgumentKind(uint i) => clang.Cursor_getTemplateArgumentKind(this, i);

        public CXType GetTemplateArgumentType(uint i) => clang.Cursor_getTemplateArgumentType(this, i);

        public ulong GetTemplateArgumentUnsignedValue(uint i) => clang.Cursor_getTemplateArgumentUnsignedValue(this, i);

        public long GetTemplateArgumentValue(uint i) => clang.Cursor_getTemplateArgumentValue(this, i);

        public override string ToString() => Spelling.ToString();

        public CXChildVisitResult VisitChildren(CXCursorVisitor visitor, CXClientData clientData) => (CXChildVisitResult)clang.visitChildren(this, visitor, clientData);
    }
}
