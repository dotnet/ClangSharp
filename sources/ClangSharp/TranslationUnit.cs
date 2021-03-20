// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using System.Collections.Generic;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class TranslationUnit : IDisposable, IEquatable<TranslationUnit>
    {
        private static readonly Dictionary<CXTranslationUnit, WeakReference<TranslationUnit>> _createdTranslationUnits = new Dictionary<CXTranslationUnit, WeakReference<TranslationUnit>>();
        private static readonly object _createTranslationUnitLock = new object();

        private readonly Dictionary<CXCursor, WeakReference<Cursor>> _createdCursors;
        private readonly Dictionary<CXType, WeakReference<Type>> _createdTypes;
        private readonly Lazy<TranslationUnitDecl> _translationUnitDecl;

        private bool _isDisposed;

        private TranslationUnit(CXTranslationUnit handle)
        {
            Handle = handle;

            _createdCursors = new Dictionary<CXCursor, WeakReference<Cursor>>();
            _createdTypes = new Dictionary<CXType, WeakReference<Type>>();

            _translationUnitDecl = new Lazy<TranslationUnitDecl>(() => GetOrCreate<TranslationUnitDecl>(Handle.Cursor));
        }

        ~TranslationUnit()
        {
            Dispose(isDisposing: false);
        }

        public CXTranslationUnit Handle { get; }

        public TranslationUnitDecl TranslationUnitDecl => _translationUnitDecl.Value;

        public static bool operator ==(TranslationUnit left, TranslationUnit right) => (left is object) ? ((right is object) && (left.Handle == right.Handle)) : (right is null);

        public static bool operator !=(TranslationUnit left, TranslationUnit right) => (left is object) ? ((right is null) || (left.Handle != right.Handle)) : (right is object);

        public static TranslationUnit GetOrCreate(CXTranslationUnit handle)
        {
            WeakReference<TranslationUnit> translationUnitRef;

            if (handle == null)
            {
                return null;
            }
            else if (!_createdTranslationUnits.TryGetValue(handle, out translationUnitRef))
            {
                lock (_createTranslationUnitLock)
                {
                    if (!_createdTranslationUnits.TryGetValue(handle, out translationUnitRef))
                    {
                        translationUnitRef = new WeakReference<TranslationUnit>(null);
                        _createdTranslationUnits.Add(handle, translationUnitRef);
                    }
                }
            }

            if (!translationUnitRef.TryGetTarget(out TranslationUnit translationUnit))
            {
                lock (_createTranslationUnitLock)
                {
                    if (!translationUnitRef.TryGetTarget(out translationUnit))
                    {
                        translationUnit = new TranslationUnit(handle);
                        translationUnitRef.SetTarget(translationUnit);
                    }
                }
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
            WeakReference<Cursor> cursorRef;

            if (handle.IsNull)
            {
                return null;
            }
            else if (!_createdCursors.TryGetValue(handle, out cursorRef))
            {
                cursorRef = new WeakReference<Cursor>(null);
                _createdCursors.Add(handle, cursorRef);
            }

            if (!cursorRef.TryGetTarget(out Cursor cursor))
            {
                cursor = Cursor.Create(handle);
                cursorRef.SetTarget(cursor);
            }
            return (TCursor)cursor;
        }

        internal TType GetOrCreate<TType>(CXType handle)
            where TType : Type
        {
            WeakReference<Type> typeRef;

            if (handle.kind == CXTypeKind.CXType_Invalid)
            {
                return null;
            }
            else if (!_createdTypes.TryGetValue(handle, out typeRef))
            {
                typeRef = new WeakReference<Type>(null);
                _createdTypes.Add(handle, typeRef);
            }

            if (!typeRef.TryGetTarget(out Type type))
            {
                type = Type.Create(handle);
                typeRef.SetTarget(type);
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

            lock (_createTranslationUnitLock)
            {
                _createdTranslationUnits.Remove(Handle);
            }
        }
    }
}
