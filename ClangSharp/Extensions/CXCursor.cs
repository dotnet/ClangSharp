using System;

namespace ClangSharp
{
    public partial struct CXCursor : IEquatable<CXCursor>
    {
        public static CXCursor Null => clang.getNullCursor();

        public CXSourceRange CommentRange => clang.Cursor_getCommentRange(this);

        public CXType EnumDeclIntegerType => clang.getEnumDeclIntegerType(this);

        public CXSourceRange Extent => clang.getCursorExtent(this);

        public CXType IBOutletCollectionType => clang.getIBOutletCollectionType(this);

        public bool IsNull => clang.Cursor_isNull(this) != 0;

        public CXCursorKind Kind => clang.getCursorKind(this);

        public CXSourceLocation Location => clang.getCursorLocation(this);

        public CXType TypedefDeclUnderlyingType => clang.getTypedefDeclUnderlyingType(this);

        public CXType RecieverType => clang.Cursor_getReceiverType(this);

        public CXType ResultType => clang.getCursorResultType(this);

        public CXString Spelling => clang.getCursorSpelling(this);

        public CXTranslationUnit TranslationUnit => clang.Cursor_getTranslationUnit(this);

        public CXType Type => clang.getCursorType(this);

        public override bool Equals(object obj) => (obj is CXCursor other) && Equals(other);

        public bool Equals(CXCursor other) => clang.equalCursors(this, other) != 0;

        public override int GetHashCode() => (int)clang.hashCursor(this);

        public CXSourceRange GetReferenceNameRange(CXNameRefFlags nameFlags, uint pieceIndex) => clang.getCursorReferenceNameRange(this, (uint)nameFlags, pieceIndex);

        public CXType GetTemplateArgumentType(uint i) => clang.Cursor_getTemplateArgumentType(this, i);

        public CXSourceRange GetSpellingNameRange(uint pieceIndex, uint options) => clang.Cursor_getSpellingNameRange(this, pieceIndex, options);

        public override string ToString() => Spelling.ToString();
    }
}
