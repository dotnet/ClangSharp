// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Runtime.InteropServices;
using ClangSharp.Interop;

namespace ClangSharp.Abstractions;

internal struct FunctionOrDelegateDesc
{
    public AccessSpecifier AccessSpecifier { get; set; }
    public string NativeTypeName { get; set; }
    public string EscapedName { get; set; }
    public string EntryPoint { get; set; }
    public string ParentName { get; set; }
    public string? LibraryPath { get; set; }
    public string ReturnType { get; set; }
    public CallConv CallingConvention { get; set; }
    public FunctionOrDelegateFlags Flags { get; set; }
    public long? VtblIndex { get; set; }
    public CXSourceLocation? Location { get; set; }
    public bool HasBody { get; set; }
    public bool IsInherited { get; set; }
    public bool NeedsUnscopedRef { get; set; }

    public bool IsVirtual
    {
        readonly get
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
        readonly get
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

    public bool IsManualImport
    {
        readonly get
        {
            return (Flags & FunctionOrDelegateFlags.IsManualImport) != 0;
        }

        set
        {
            Flags = value
                  ? Flags | FunctionOrDelegateFlags.IsManualImport
                  : Flags & ~FunctionOrDelegateFlags.IsManualImport;
        }
    }

    public bool HasFnPtrCodeGen
    {
        readonly get
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
        readonly get
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
        readonly get
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
        readonly get
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
        readonly get
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
        readonly get
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
        readonly get
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
        readonly get
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
        readonly get
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
        readonly get
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
                null => Flags & ~FunctionOrDelegateFlags.IsStatic & ~FunctionOrDelegateFlags.IsNotStatic,
            };
        }
    }

    public bool NeedsReturnFixup
    {
        readonly get
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
        readonly get
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

    public Action<object> WriteCustomAttrs { get; set; }
    public object CustomAttrGeneratorData { get; set; }
}
