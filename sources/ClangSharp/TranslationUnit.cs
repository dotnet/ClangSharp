// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed unsafe class TranslationUnit : IDisposable, IEquatable<TranslationUnit>
    {
        private static readonly ConcurrentDictionary<CXTranslationUnit, WeakReference<TranslationUnit>> s_createdTranslationUnits = new ConcurrentDictionary<CXTranslationUnit, WeakReference<TranslationUnit>>();
        private static readonly object s_createTranslationUnitLock = new object();

        private readonly Dictionary<CXCursor, WeakReference<Cursor>> _createdCursors;
        private readonly Dictionary<CX_TemplateArgument, WeakReference<TemplateArgument>> _createdTemplateArguments;
        private readonly Dictionary<CX_TemplateArgumentLoc, WeakReference<TemplateArgumentLoc>> _createdTemplateArgumentLocs;
        private readonly Dictionary<CX_TemplateName, WeakReference<TemplateName>> _createdTemplateNames;
        private readonly Dictionary<CXType, WeakReference<Type>> _createdTypes;
        private readonly Lazy<TranslationUnitDecl> _translationUnitDecl;

        private bool _isDisposed;

        private TranslationUnit(CXTranslationUnit handle)
        {
            Handle = handle;

            _createdCursors = new Dictionary<CXCursor, WeakReference<Cursor>>();
            _createdTemplateArguments = new Dictionary<CX_TemplateArgument, WeakReference<TemplateArgument>>();
            _createdTemplateArgumentLocs = new Dictionary<CX_TemplateArgumentLoc, WeakReference<TemplateArgumentLoc>>();
            _createdTemplateNames = new Dictionary<CX_TemplateName, WeakReference<TemplateName>>();
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
            if (handle == null)
            {
                return null;
            }

            var translationUnitRef = s_createdTranslationUnits.GetOrAdd(handle, (handle) => new WeakReference<TranslationUnit>(null));

            if (!translationUnitRef.TryGetTarget(out var translationUnit))
            {
                lock (s_createTranslationUnitLock)
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

            if (!cursorRef.TryGetTarget(out var cursor))
            {
                cursor = Cursor.Create(handle);
                cursorRef.SetTarget(cursor);
            }
            return (TCursor)cursor;
        }

        internal TemplateArgument GetOrCreate(CX_TemplateArgument handle)
        {
            WeakReference<TemplateArgument> templateArgumentRef;

            if (handle.kind == CXTemplateArgumentKind.CXTemplateArgumentKind_Invalid)
            {
                return null;
            }
            else if (!_createdTemplateArguments.TryGetValue(handle, out templateArgumentRef))
            {
                templateArgumentRef = new WeakReference<TemplateArgument>(null);
                _createdTemplateArguments.Add(handle, templateArgumentRef);
            }

            if (!templateArgumentRef.TryGetTarget(out var templateArgument))
            {
                templateArgument = new TemplateArgument(handle);
                templateArgumentRef.SetTarget(templateArgument);
}
            return templateArgument;
        }

        internal TemplateArgumentLoc GetOrCreate(CX_TemplateArgumentLoc handle)
        {
            WeakReference<TemplateArgumentLoc> templateArgumentLocRef;

            if (handle.value == null)
            {
                return null;
            }
            else if (!_createdTemplateArgumentLocs.TryGetValue(handle, out templateArgumentLocRef))
            {
                templateArgumentLocRef = new WeakReference<TemplateArgumentLoc>(null);
                _createdTemplateArgumentLocs.Add(handle, templateArgumentLocRef);
            }

            if (!templateArgumentLocRef.TryGetTarget(out var templateArgumentLoc))
            {
                templateArgumentLoc = new TemplateArgumentLoc(handle);
                templateArgumentLocRef.SetTarget(templateArgumentLoc);
            }
            return templateArgumentLoc;
        }

        internal TemplateName GetOrCreate(CX_TemplateName handle)
        {
            WeakReference<TemplateName> templateNameRef;

            if (handle.kind == CX_TemplateNameKind.CX_TNK_Invalid)
            {
                return null;
            }
            else if (!_createdTemplateNames.TryGetValue(handle, out templateNameRef))
            {
                templateNameRef = new WeakReference<TemplateName>(null);
                _createdTemplateNames.Add(handle, templateNameRef);
            }

            if (!templateNameRef.TryGetTarget(out var templateName))
            {
                templateName = new TemplateName(handle);
                templateNameRef.SetTarget(templateName);
            }
            return templateName;
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

            if (!typeRef.TryGetTarget(out var type))
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

            _ = s_createdTranslationUnits.TryRemove(Handle, out _);
        }
    }
}
