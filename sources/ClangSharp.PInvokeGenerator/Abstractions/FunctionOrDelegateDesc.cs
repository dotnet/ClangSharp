// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
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
    public string[]? ParameterTypes { get; set; }

    public bool IsVirtual
    {
        readonly get
        {
            return GetFlag(FunctionOrDelegateFlags.IsVirtual);
        }

        set
        {
            SetFlag(FunctionOrDelegateFlags.IsVirtual, value);
        }
    }

    public bool IsDllImport
    {
        readonly get
        {
            return GetFlag(FunctionOrDelegateFlags.IsDllImport);
        }

        set
        {
            SetFlag(FunctionOrDelegateFlags.IsDllImport, value);
        }
    }

    public bool IsManualImport
    {
        readonly get
        {
            return GetFlag(FunctionOrDelegateFlags.IsManualImport);
        }

        set
        {
            SetFlag(FunctionOrDelegateFlags.IsManualImport, value);
        }
    }

    public bool IsReadOnly
    {
        readonly get
        {
            return GetFlag(FunctionOrDelegateFlags.IsReadOnly);
        }

        set
        {
            SetFlag(FunctionOrDelegateFlags.IsReadOnly, value);
        }
    }

    public bool HasFnPtrCodeGen
    {
        readonly get
        {
            return GetFlag(FunctionOrDelegateFlags.HasFnPtrCodeGen);
        }

        set
        {
            SetFlag(FunctionOrDelegateFlags.HasFnPtrCodeGen, value);
        }
    }

    public bool IsAggressivelyInlined
    {
        readonly get
        {
            return GetFlag(FunctionOrDelegateFlags.IsAggressivelyInlined);
        }

        set
        {
            SetFlag(FunctionOrDelegateFlags.IsAggressivelyInlined, value);
        }
    }

    public bool SetLastError
    {
        readonly get
        {
            return GetFlag(FunctionOrDelegateFlags.SetLastError);
        }

        set
        {
            SetFlag(FunctionOrDelegateFlags.SetLastError, value);
        }
    }

    public bool IsCxx
    {
        readonly get
        {
            return GetFlag(FunctionOrDelegateFlags.IsCxx);
        }

        set
        {
            SetFlag(FunctionOrDelegateFlags.IsCxx, value);
        }
    }

    public bool NeedsNewKeyword
    {
        readonly get
        {
            return GetFlag(FunctionOrDelegateFlags.NeedsNewKeyword);
        }

        set
        {
            SetFlag(FunctionOrDelegateFlags.NeedsNewKeyword, value);
        }
    }

    public bool IsUnsafe
    {
        readonly get
        {
            return GetFlag(FunctionOrDelegateFlags.IsUnsafe);
        }

        set
        {
            SetFlag(FunctionOrDelegateFlags.IsUnsafe, value);
        }
    }

    public bool IsCtxCxxRecord
    {
        readonly get
        {
            return GetFlag(FunctionOrDelegateFlags.IsCtxCxxRecord);
        }

        set
        {
            SetFlag(FunctionOrDelegateFlags.IsCtxCxxRecord, value);
        }
    }

    public bool IsCxxRecordCtxUnsafe
    {
        readonly get
        {
            return GetFlag(FunctionOrDelegateFlags.IsCxxRecordCtxUnsafe);
        }

        set
        {
            SetFlag(FunctionOrDelegateFlags.IsCxxRecordCtxUnsafe, value);
        }
    }

    public bool IsMemberFunction
    {
        readonly get
        {
            return GetFlag(FunctionOrDelegateFlags.IsMemberFunction);
        }

        set
        {
            SetFlag(FunctionOrDelegateFlags.IsMemberFunction, value);
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
            return GetFlag(FunctionOrDelegateFlags.NeedsReturnFixup);
        }

        set
        {
            SetFlag(FunctionOrDelegateFlags.NeedsReturnFixup, value);
        }
    }

    public bool IsCxxConstructor
    {
        readonly get
        {
            return GetFlag(FunctionOrDelegateFlags.IsCxxConstructor);
        }

        set
        {
            SetFlag(FunctionOrDelegateFlags.IsCxxConstructor, value);
        }
    }

    private readonly bool GetFlag(FunctionOrDelegateFlags flag) => (Flags & flag) != 0;

    private void SetFlag(FunctionOrDelegateFlags flag, bool value) => Flags = value ? Flags | flag : Flags & ~flag;

    public Action<object> WriteCustomAttrs { get; set; }
    public object CustomAttrGeneratorData { get; set; }
}
