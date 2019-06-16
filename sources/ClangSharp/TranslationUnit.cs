using System;
using System.Collections.Generic;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class TranslationUnit : IDisposable, IEquatable<TranslationUnit>
    {
        private static readonly Dictionary<CXTranslationUnit, TranslationUnit> _createdTranslationUnits = new Dictionary<CXTranslationUnit, TranslationUnit>();
        private static readonly object _createTranslationUnitLock = new object();

        private readonly Dictionary<CXCursor, Cursor> _createdCursors;
        private readonly Dictionary<CXType, Type> _createdTypes;

        private bool _isDisposed;

        private TranslationUnit(CXTranslationUnit handle)
        {
            Handle = handle;
            TranslationUnitDecl = new TranslationUnitDecl(Handle.Cursor);

            _createdCursors = new Dictionary<CXCursor, Cursor>();
            _createdTypes = new Dictionary<CXType, Type>();
        }

        ~TranslationUnit()
        {
            Dispose(isDisposing: false);
        }

        public CXTranslationUnit Handle { get; }

        public TranslationUnitDecl TranslationUnitDecl { get; }

        public static bool operator ==(TranslationUnit left, TranslationUnit right) => (left is object) ? ((right is object) && (left.Handle == right.Handle)) : (right is null);

        public static bool operator !=(TranslationUnit left, TranslationUnit right) => (left is object) ? ((right is null) || (left.Handle != right.Handle)) : (right is object);

        public static TranslationUnit GetOrCreate(CXTranslationUnit handle)
        {
            TranslationUnit translationUnit;

            if (handle == null)
            {
                translationUnit = null;
            }
            else if (!_createdTranslationUnits.TryGetValue(handle, out translationUnit))
            {
                lock (_createTranslationUnitLock)
                {
                    if (!_createdTranslationUnits.TryGetValue(handle, out translationUnit))
                    {
                        translationUnit = new TranslationUnit(handle);
                        _createdTranslationUnits.Add(handle, translationUnit);
                    }
                    else
                    {
                        System.Diagnostics.Debugger.Break();
                    }
                }
            }
            else
            {
                System.Diagnostics.Debugger.Break();
            }

            return translationUnit;
        }

        public void Dispose()
        {
            Dispose(isDisposing: true);
            GC.SuppressFinalize(this);
        }

        public override bool Equals(object obj) => (obj is TranslationUnit other) && Equals(other);

        public bool Equals(TranslationUnit other) => this == other;

        public override int GetHashCode() => Handle.GetHashCode();

        internal TCursor GetOrCreate<TCursor>(CXCursor handle)
            where TCursor : Cursor
        {
            Cursor cursor;

            if (handle.IsNull)
            {
                cursor = null;
            }
            else if (!_createdCursors.TryGetValue(handle, out cursor))
            {
                cursor = Cursor.Create(handle);
                _createdCursors.Add(handle, cursor);
            }

            return (TCursor)cursor;
        }

        internal TType GetOrCreate<TType>(CXType handle)
            where TType : Type
        {
            Type type;

            if (handle.kind == CXTypeKind.CXType_Invalid)
            {
                type = null;
            }
            else if (!_createdTypes.TryGetValue(handle, out type))
            {
                type = Type.Create(handle);
                _createdTypes.Add(handle, type);
            }

            return (TType)type;
        }

        private void Dispose(bool isDisposing)
        {
            if (_isDisposed)
            {
                return;
            }
            _isDisposed = true;

            if (Handle != default)
            {
                Handle.Dispose();
            }
            _createdTranslationUnits.Remove(Handle);
        }
    }
}
