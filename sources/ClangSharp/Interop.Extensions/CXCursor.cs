// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using System.Runtime.InteropServices;

namespace ClangSharp.Interop
{
    public unsafe partial struct CXCursor : IEquatable<CXCursor>
    {
        public static CXCursor Null => clang.getNullCursor();

        public CX_AttrKind AttrKind => clangsharp.Cursor_getAttrKind(this);

        public CXAvailabilityKind Availability => clang.getCursorAvailability(this);

        public CX_BinaryOperatorKind BinaryOperatorKind => clangsharp.Cursor_getBinaryOpcode(this);

        public CXString BinaryOperatorKindSpelling => clangsharp.Cursor_getBinaryOpcodeSpelling(BinaryOperatorKind);

        public CXString BriefCommentText => clang.Cursor_getBriefCommentText(this);

        public CXCursor CanonicalCursor => clang.getCanonicalCursor(this);

        public CXSourceRange CommentRange => clang.Cursor_getCommentRange(this);

        public CXCompletionString CompletionString => (CXCompletionString)clang.getCursorCompletionString(this);

        public CX_CXXAccessSpecifier CXXAccessSpecifier => clang.getCXXAccessSpecifier(this);

        public bool CXXConstructor_IsConvertingConstructor => clang.CXXConstructor_isConvertingConstructor(this) != 0;

        public bool CXXConstructor_IsCopyConstructor => clang.CXXConstructor_isCopyConstructor(this) != 0;

        public bool CXXConstructor_IsDefaultConstructor => clang.CXXConstructor_isDefaultConstructor(this) != 0;

        public bool CXXConstructor_IsMoveConstructor => clang.CXXConstructor_isMoveConstructor(this) != 0;

        public bool CXXField_IsMutable => clang.CXXField_isMutable(this) != 0;

        public CXStringSet* CXXManglings => clang.Cursor_getCXXManglings(this);

        public bool CXXMethod_IsConst => clang.CXXMethod_isConst(this) != 0;

        public bool CXXMethod_IsDefaulted => clang.CXXMethod_isDefaulted(this) != 0;

        public bool CXXMethod_IsPureVirtual => clang.CXXMethod_isPureVirtual(this) != 0;

        public bool CXXMethod_IsStatic => clang.CXXMethod_isStatic(this) != 0;

        public bool CXXMethod_IsVirtual => clang.CXXMethod_isVirtual(this) != 0;

        public bool CXXRecord_IsAbstract => clang.CXXRecord_isAbstract(this) != 0;

        public CX_DeclKind DeclKind => clangsharp.Cursor_getDeclKind(this);

        public CXString DeclObjCTypeEncoding => clang.getDeclObjCTypeEncoding(this);

        public CXCursor Definition => clang.getCursorDefinition(this);

        public CXString DisplayName => clang.getCursorDisplayName(this);

        public ulong EnumConstantDeclUnsignedValue => clang.getEnumConstantDeclUnsignedValue(this);

        public long EnumConstantDeclValue => clang.getEnumConstantDeclValue(this);

        public CXType EnumDecl_IntegerType => clang.getEnumDeclIntegerType(this);

        public bool EnumDecl_IsScoped => clang.EnumDecl_isScoped(this) != 0;

        public CXEvalResult Evaluate => (CXEvalResult)clang.Cursor_Evaluate(this);

        public int ExceptionSpecificationType => clang.getCursorExceptionSpecificationType(this);

        public CXSourceRange Extent => clangsharp.getCursorExtent(this);

        public int FieldDeclBitWidth => clang.getFieldDeclBitWidth(this);

        public bool HasAttrs => clang.Cursor_hasAttrs(this) != 0;

        public uint Hash => clang.hashCursor(this);

        public CXType IBOutletCollectionType => clang.getIBOutletCollectionType(this);

        public CXFile IncludedFile => (CXFile)clang.getIncludedFile(this);

        public bool IsAnonymous => clang.Cursor_isAnonymous(this) != 0;

        public bool IsAnonymousRecordDecl => clang.Cursor_isAnonymousRecordDecl(this) != 0;

        public bool IsAttribute => clang.isAttribute(Kind) != 0;

        public bool IsBitField => clang.Cursor_isBitField(this) != 0;

        public bool IsCanonical => Equals(CanonicalCursor);

        public bool IsDeclaration => clang.isDeclaration(Kind) != 0;

        public bool IsDefinition => clang.isCursorDefinition(this) != 0;

        public bool IsDynamicCall => clang.Cursor_isDynamicCall(this) != 0;

        public bool IsExpression => clang.isExpression(Kind) != 0;

        public bool IsFunctionInlined => clang.Cursor_isFunctionInlined(this) != 0;

        public bool IsInlineNamespace => clang.Cursor_isInlineNamespace(this) != 0;

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

        public CXModule Module => (CXModule)clang.Cursor_getModule(this);

        public int NumArguments => clang.Cursor_getNumArguments(this);

        public uint NumOverloadedDecls => clang.getNumOverloadedDecls(this);

        public int NumTemplateArguments => clang.Cursor_getNumTemplateArguments(this);

        public CXObjCDeclQualifierKind ObjCDeclQualifiers => (CXObjCDeclQualifierKind)clang.Cursor_getObjCDeclQualifiers(this);

        public CXStringSet* ObjCManglings => clang.Cursor_getObjCManglings(this);

        public CXString ObjCPropertyGetterName => clang.Cursor_getObjCPropertyGetterName(this);

        public CXString ObjCPropertySetterName => clang.Cursor_getObjCPropertySetterName(this);

        public int ObjCSelectorIndex => clang.Cursor_getObjCSelectorIndex(this);

        public long OffsetOfField => clang.Cursor_getOffsetOfField(this);

        public ReadOnlySpan<CXCursor> OverriddenCursors
        {
            get
            {
                CXCursor* overridden;
                uint numOverridden;

                clang.getOverriddenCursors(this, &overridden, &numOverridden);
                return new ReadOnlySpan<CXCursor>(overridden, (int)numOverridden);
            }
        }

        public CXComment ParsedComment => clang.Cursor_getParsedComment(this);

        public CXPrintingPolicy PrintingPolicy => (CXPrintingPolicy)clang.getCursorPrintingPolicy(this);

        public CXString RawCommentText => clang.Cursor_getRawCommentText(this);

        public CXType RecieverType => !IsExpression ? default : clang.Cursor_getReceiverType(this);

        public CXCursor Referenced => clang.getCursorReferenced(this);

        public CXType ResultType => clang.getCursorResultType(this);

        public CXCursor SemanticParent => clang.getCursorSemanticParent(this);

        public CXCursor SpecializedCursorTemplate => clang.getSpecializedCursorTemplate(this);

        public CXString Spelling => clang.getCursorSpelling(this);

        public CX_StmtClass StmtClass => clangsharp.Cursor_getStmtClass(this);

        public CX_StorageClass StorageClass => clang.Cursor_getStorageClass(this);

        public CXCursorKind TemplateCursorKind => clang.getTemplateCursorKind(this);

        public CXTLSKind TlsKind => clang.getCursorTLSKind(this);

        public CXTranslationUnit TranslationUnit => clang.Cursor_getTranslationUnit(this);

        public CXType Type => clang.getCursorType(this);

        public CXType TypedefDeclUnderlyingType => clang.getTypedefDeclUnderlyingType(this);

        public CX_UnaryOperatorKind UnaryOperatorKind => clangsharp.Cursor_getUnaryOpcode(this);

        public CXString UnaryOperatorKindSpelling => clangsharp.Cursor_getUnaryOpcodeSpelling(UnaryOperatorKind);

        public CXString Usr => clang.getCursorUSR(this);

        public CXVisibilityKind Visibility => clang.getCursorVisibility(this);

        public static bool operator ==(CXCursor left, CXCursor right) => clang.equalCursors(left, right) != 0;

        public static bool operator !=(CXCursor left, CXCursor right) => clang.equalCursors(left, right) == 0;

        public void DisposeOverriddenCursors(ReadOnlySpan<CXCursor> overridden)
        {
            fixed (CXCursor* pOverridden = overridden)
            {
                clang.disposeOverriddenCursors(pOverridden);
            }
        }

        public override bool Equals(object obj) => (obj is CXCursor other) && Equals(other);

        public bool Equals(CXCursor other) => this == other;

        public CXResult FindReferencesInFile(CXFile file, CXCursorAndRangeVisitor visitor) => clang.findReferencesInFile(this, file, visitor);

        public CXCursor GetArgument(uint index) => clang.Cursor_getArgument(this, index);

        public void GetDefinitionSpellingAndExtent(out string spelling, out uint startLine, out uint startColumn, out uint endLine, out uint endColumn)
        {
            fixed (uint* pStartLine = &startLine)
            fixed (uint* pStartColumn = &startColumn)
            fixed (uint* pEndLine = &endLine)
            fixed (uint* pEndColumn = &endColumn)
            {
                sbyte* startBuf;
                sbyte* endBuf;
                clang.getDefinitionSpellingAndExtent(this, &startBuf, &endBuf, pStartLine, pStartColumn, pEndLine, pEndColumn);
                spelling = new ReadOnlySpan<byte>(startBuf, (int)(endBuf - startBuf)).AsString();
            }
        }

        public override int GetHashCode() => (int)Hash;

        public bool GetIsExternalSymbol(out CXString language, out CXString definedIn, out bool isGenerated)
        {
            fixed (CXString* pLanguage = &language)
            fixed (CXString* pDefinedIn = &definedIn)
            {
                uint isGeneratedOut;
                var result = clang.Cursor_isExternalSymbol(this, pLanguage, pDefinedIn, &isGeneratedOut);
                isGenerated = isGeneratedOut != 0;
                return result != 0;
            }
        }

        public CXObjCPropertyAttrKind GetObjCPropertyAttributes(uint reserved) => (CXObjCPropertyAttrKind)clang.Cursor_getObjCPropertyAttributes(this, reserved);

        public CXCursor GetOverloadedDecl(uint index) => clang.getOverloadedDecl(this, index);

        public int GetPlatformAvailability(out bool alwaysDeprecated, out CXString deprecatedMessage, out bool alwaysUnavailable, out CXString unavailableMessage, Span<CXPlatformAvailability> availability)
        {
            fixed (CXString* pDeprecatedMessage = &deprecatedMessage)
            fixed (CXString* pUnavailableMessage = &unavailableMessage)
            fixed (CXPlatformAvailability* pAvailability = availability)
            {
                int alwaysDeprecatedOut;
                int alwaysUnavailableOut;
                var result = clang.getCursorPlatformAvailability(this, &alwaysDeprecatedOut, pDeprecatedMessage, &alwaysUnavailableOut, pUnavailableMessage, pAvailability, availability.Length);
                alwaysDeprecated = alwaysDeprecatedOut != 0;
                alwaysUnavailable = alwaysUnavailableOut != 0;
                return result;
            }
        }

        public CXString GetPrettyPrinted(CXPrintingPolicy policy) => clang.getCursorPrettyPrinted(this, policy);

        public CXSourceRange GetReferenceNameRange(CXNameRefFlags nameFlags, uint pieceIndex) => clang.getCursorReferenceNameRange(this, (uint)nameFlags, pieceIndex);

        public CXSourceRange GetSpellingNameRange(uint pieceIndex, uint options) => clang.Cursor_getSpellingNameRange(this, pieceIndex, options);

        public CXTemplateArgumentKind GetTemplateArgumentKind(uint i) => clang.Cursor_getTemplateArgumentKind(this, i);

        public CXType GetTemplateArgumentType(uint i) => clang.Cursor_getTemplateArgumentType(this, i);

        public ulong GetTemplateArgumentUnsignedValue(uint i) => clang.Cursor_getTemplateArgumentUnsignedValue(this, i);

        public long GetTemplateArgumentValue(uint i) => clang.Cursor_getTemplateArgumentValue(this, i);

        public override string ToString() => Spelling.ToString();

        public CXChildVisitResult VisitChildren(CXCursorVisitor visitor, CXClientData clientData)
        {
            var pVisitor = Marshal.GetFunctionPointerForDelegate(visitor);
            var result = (CXChildVisitResult)clang.visitChildren(this, pVisitor, clientData);

            GC.KeepAlive(visitor);
            return result;
        }
    }
}
