// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using System.Runtime.InteropServices;
using ClangSharp.Interop;

namespace ClangSharp.Abstractions
{
    internal struct FunctionOrDelegateDesc<TCustomAttrGeneratorData>
    {
        public AccessSpecifier AccessSpecifier { get; set; }
        public string NativeTypeName { get; set; }
        public string EscapedName { get; set; }
        public string EntryPoint { get; set; }
        public string LibraryPath { get; set; }
        public string ReturnType { get; set; }
        public CallingConvention CallingConvention { get; set; }
        public FunctionOrDelegateFlags Flags { get; set; }
        public long? VtblIndex { get; set; }
        public CXSourceLocation? Location { get; set; }

        public bool IsVirtual
        {
            get
            {
                return (Flags & FunctionOrDelegateFlags.IsVirtual) != 0;
            }

            set
            {
                Flags = value ? Flags | FunctionOrDelegateFlags.IsVirtual : Flags & ~FunctionOrDelegateFlags.IsVirtual;
            }
        }

        public bool IsDllImport
        {
            get
            {
                return (Flags & FunctionOrDelegateFlags.IsDllImport) != 0;
            }

            set
            {
                Flags = value
                      ? Flags | FunctionOrDelegateFlags.IsDllImport
                      : Flags & ~FunctionOrDelegateFlags.IsDllImport;
            }
        }

        public bool HasFnPtrCodeGen
        {
            get
            {
                return (Flags & FunctionOrDelegateFlags.HasFnPtrCodeGen) != 0;
            }

            set
            {
                Flags = value
                      ? Flags | FunctionOrDelegateFlags.HasFnPtrCodeGen
                      : Flags & ~FunctionOrDelegateFlags.HasFnPtrCodeGen;
            }
        }

        public bool IsAggressivelyInlined
        {
            get
            {
                return (Flags & FunctionOrDelegateFlags.IsAggressivelyInlined) != 0;
            }

            set
            {
                Flags = value
                      ? Flags | FunctionOrDelegateFlags.IsAggressivelyInlined
                      : Flags & ~FunctionOrDelegateFlags.IsAggressivelyInlined;
            }
        }

        public bool SetLastError
        {
            get
            {
                return (Flags & FunctionOrDelegateFlags.SetLastError) != 0;
            }

            set
            {
                Flags = value
                      ? Flags | FunctionOrDelegateFlags.SetLastError
                      : Flags & ~FunctionOrDelegateFlags.SetLastError;
            }
        }

        public bool IsCxx
        {
            get
            {
                return (Flags & FunctionOrDelegateFlags.IsCxx) != 0;
            }

            set
            {
                Flags = value ? Flags | FunctionOrDelegateFlags.IsCxx : Flags & ~FunctionOrDelegateFlags.IsCxx;
            }
        }

        public bool NeedsNewKeyword
        {
            get
            {
                return (Flags & FunctionOrDelegateFlags.NeedsNewKeyword) != 0;
            }

            set
            {
                Flags = value
                      ? Flags | FunctionOrDelegateFlags.NeedsNewKeyword
                      : Flags & ~FunctionOrDelegateFlags.NeedsNewKeyword;
            }
        }

        public bool IsUnsafe
        {
            get
            {
                return (Flags & FunctionOrDelegateFlags.IsUnsafe) != 0;
            }

            set
            {
                Flags = value ? Flags | FunctionOrDelegateFlags.IsUnsafe : Flags & ~FunctionOrDelegateFlags.IsUnsafe;
            }
        }

        public bool IsCtxCxxRecord
        {
            get
            {
                return (Flags & FunctionOrDelegateFlags.IsCtxCxxRecord) != 0;
            }

            set
            {
                Flags = value
                      ? Flags | FunctionOrDelegateFlags.IsCtxCxxRecord
                      : Flags & ~FunctionOrDelegateFlags.IsCtxCxxRecord;
            }
        }

        public bool IsCxxRecordCtxUnsafe
        {
            get
            {
                return (Flags & FunctionOrDelegateFlags.IsCxxRecordCtxUnsafe) != 0;
            }

            set
            {
                Flags = value
                      ? Flags | FunctionOrDelegateFlags.IsCxxRecordCtxUnsafe
                      : Flags & ~FunctionOrDelegateFlags.IsCxxRecordCtxUnsafe;
            }
        }

        public bool IsMemberFunction
        {
            get
            {
                return (Flags & FunctionOrDelegateFlags.IsMemberFunction) != 0;
            }

            set
            {
                Flags = value
                      ? Flags | FunctionOrDelegateFlags.IsMemberFunction
                      : Flags & ~FunctionOrDelegateFlags.IsMemberFunction;
            }
        }

        public bool? IsStatic
        {
            get
            {
                return (Flags & FunctionOrDelegateFlags.IsStatic) != 0
                     ? true
                     : (Flags & FunctionOrDelegateFlags.IsNotStatic) != 0 ? false : null;
            }

            set
            {
                Flags = value switch {
                    // true - static, false - not static, null - infer
                    true => Flags | (FunctionOrDelegateFlags.IsStatic & ~FunctionOrDelegateFlags.IsNotStatic),
                    false => (Flags & ~FunctionOrDelegateFlags.IsStatic) | FunctionOrDelegateFlags.IsNotStatic,
                    null => Flags & ~FunctionOrDelegateFlags.IsStatic & ~FunctionOrDelegateFlags.IsNotStatic
                };
            }
        }

        public bool NeedsReturnFixup
        {
            get
            {
                return (Flags & FunctionOrDelegateFlags.NeedsReturnFixup) != 0;
            }

            set
            {
                Flags = value
                      ? Flags | FunctionOrDelegateFlags.NeedsReturnFixup
                      : Flags & ~FunctionOrDelegateFlags.NeedsReturnFixup;
            }
        }

        public bool IsCxxConstructor
        {
            get
            {
                return (Flags & FunctionOrDelegateFlags.IsCxxConstructor) != 0;
            }

            set
            {
                Flags = value
                      ? Flags | FunctionOrDelegateFlags.IsCxxConstructor
                      : Flags & ~FunctionOrDelegateFlags.IsCxxConstructor;
            }
        }

        public Action<TCustomAttrGeneratorData> WriteCustomAttrs { get; set; }
        public TCustomAttrGeneratorData CustomAttrGeneratorData { get; set; }
    }
}
