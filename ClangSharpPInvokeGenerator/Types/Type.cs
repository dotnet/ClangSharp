using System;
using System.Diagnostics;
using ClangSharp;

namespace ClangSharpPInvokeGenerator
{
    internal sealed class Type
    {
        public static Type Create(CXType handle, TranslationUnit translationUnit)
        {
            Debug.Assert(handle.kind != CXTypeKind.CXType_Invalid);
            return new Type(handle, translationUnit);
        }

        private readonly Lazy<Type> _canonicalType;
        private readonly Lazy<Type> _elementType;
        private readonly Lazy<Type> _modifierType;
        private readonly Lazy<Type> _pointeeType;
        private readonly Lazy<Type> _resultType;

        private readonly Lazy<Decl> _declarationCursor;

        private Type(CXType handle, TranslationUnit translationUnit)
        {
            Debug.Assert(translationUnit != null);

            Handle = handle;
            TranslationUnit = translationUnit;

            translationUnit.AddVisitedType(this);

            _canonicalType = new Lazy<Type>(() => translationUnit.GetOrCreateType(handle.CanonicalType, () => Create(handle.CanonicalType, translationUnit)));
            _elementType = new Lazy<Type>(() => translationUnit.GetOrCreateType(handle.ElementType, () => Create(handle.ElementType, translationUnit)));
            _modifierType = new Lazy<Type>(() => translationUnit.GetOrCreateType(handle.ModifierType, () => Create(handle.ModifierType, translationUnit)));
            _pointeeType = new Lazy<Type>(() => translationUnit.GetOrCreateType(handle.PointeeType, () => Create(handle.PointeeType, translationUnit)));
            _resultType = new Lazy<Type>(() => translationUnit.GetOrCreateType(handle.ResultType, () => Create(handle.ResultType, translationUnit)));

            _declarationCursor = new Lazy<Decl>(() => {
                var cursor = translationUnit.GetOrCreateCursor(handle.Declaration, () => Cursor.Create(handle.Declaration, translationUnit));
                cursor?.Visit(clientData: default);
                return (Decl)cursor;
            });
        }

        public CXCallingConv CallingConv => Handle.FunctionTypeCallingConv;

        public Type CanonicalType => _canonicalType.Value;

        public Decl DeclarationCursor => _declarationCursor.Value;

        public Type ElementType => _elementType.Value;

        public CXType Handle { get; }

        public CXTypeKind Kind => Handle.kind;

        public string KindSpelling => Handle.KindSpelling.ToString();

        public Type ModifierType => _modifierType.Value;

        public long NumElements => Handle.NumElements;

        public Type PointeeType => _pointeeType.Value;

        public Type ResultType => _resultType.Value;

        public TranslationUnit TranslationUnit { get; }

        public string Spelling => Handle.Spelling.ToString();
    }
}
