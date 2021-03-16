using System;

namespace ClangSharp.Abstractions
{
    internal struct FunctionOrDelegateDesc<TCustomAttrGeneratorData>
    {
        public string AccessSpecifier { get; set; }
        public string NativeTypeName { get; set; }
        public string EscapedName { get; set; }
        public string EntryPoint { get; set; }
        public string CallingConventionName { get; set; }
        public string LibraryPath { get; set; }
        public FunctionOrDelegateFlags Flags { get; set; }

        public bool IsVirtual
        {
            get => (Flags & FunctionOrDelegateFlags.IsVirtual) != 0;
            set => Flags =
                value ? Flags | FunctionOrDelegateFlags.IsVirtual : Flags & ~FunctionOrDelegateFlags.IsVirtual;
        }

        public bool IsDllImport
        {
            get => (Flags & FunctionOrDelegateFlags.IsDllImport) != 0;
            set => Flags = value
                ? Flags | FunctionOrDelegateFlags.IsDllImport
                : Flags & ~FunctionOrDelegateFlags.IsDllImport;
        }

        public bool HasFnPtrCodeGen
        {
            get => (Flags & FunctionOrDelegateFlags.HasFnPtrCodeGen) != 0;
            set => Flags = value
                ? Flags | FunctionOrDelegateFlags.HasFnPtrCodeGen
                : Flags & ~FunctionOrDelegateFlags.HasFnPtrCodeGen;
        }

        public bool IsAggressivelyInlined
        {
            get => (Flags & FunctionOrDelegateFlags.IsAggressivelyInlined) != 0;
            set => Flags = value
                ? Flags | FunctionOrDelegateFlags.IsAggressivelyInlined
                : Flags & ~FunctionOrDelegateFlags.IsAggressivelyInlined;
        }

        public bool SetLastError
        {
            get => (Flags & FunctionOrDelegateFlags.SetLastError) != 0;
            set => Flags = value
                ? Flags | FunctionOrDelegateFlags.SetLastError
                : Flags & ~FunctionOrDelegateFlags.SetLastError;
        }

        public bool IsCxx
        {
            get => (Flags & FunctionOrDelegateFlags.IsCxx) != 0;
            set => Flags = value ? Flags | FunctionOrDelegateFlags.IsCxx : Flags & ~FunctionOrDelegateFlags.IsCxx;
        }

        public bool NeedsNewKeyword
        {
            get => (Flags & FunctionOrDelegateFlags.NeedsNewKeyword) != 0;
            set => Flags = value
                ? Flags | FunctionOrDelegateFlags.NeedsNewKeyword
                : Flags & ~FunctionOrDelegateFlags.NeedsNewKeyword;
        }

        public bool IsUnsafe
        {
            get => (Flags & FunctionOrDelegateFlags.IsUnsafe) != 0;
            set => Flags = value ? Flags | FunctionOrDelegateFlags.IsUnsafe : Flags & ~FunctionOrDelegateFlags.IsUnsafe;
        }

        public bool IsCtxCxxRecord
        {
            get => (Flags & FunctionOrDelegateFlags.IsCtxCxxRecord) != 0;
            set => Flags = value
                ? Flags | FunctionOrDelegateFlags.IsCtxCxxRecord
                : Flags & ~FunctionOrDelegateFlags.IsCtxCxxRecord;
        }

        public bool IsCxxRecordCtxUnsafe
        {
            get => (Flags & FunctionOrDelegateFlags.IsCxxRecordCtxUnsafe) != 0;
            set => Flags = value
                ? Flags | FunctionOrDelegateFlags.IsCxxRecordCtxUnsafe
                : Flags & ~FunctionOrDelegateFlags.IsCxxRecordCtxUnsafe;
        }

        public bool IsMemberFunction
        {
            get => (Flags & FunctionOrDelegateFlags.IsMemberFunction) != 0;
            set => Flags = value
                ? Flags | FunctionOrDelegateFlags.IsMemberFunction
                : Flags & ~FunctionOrDelegateFlags.IsMemberFunction;
        }

        public bool? IsStatic
        {
            get => (Flags & FunctionOrDelegateFlags.IsStatic) != 0 ? true :
                (Flags & FunctionOrDelegateFlags.IsNotStatic) != 0 ? false : null;
            set => Flags = value switch
            {
                // true - static, false - not static, null - infer
                true => Flags | FunctionOrDelegateFlags.IsStatic & ~FunctionOrDelegateFlags.IsNotStatic,
                false => Flags & ~FunctionOrDelegateFlags.IsStatic | FunctionOrDelegateFlags.IsNotStatic,
                null => Flags & ~FunctionOrDelegateFlags.IsStatic & ~FunctionOrDelegateFlags.IsNotStatic
            };
        }

        public Action<TCustomAttrGeneratorData> WriteCustomAttrs { get; set; }
        public TCustomAttrGeneratorData CustomAttrGeneratorData { get; set; }
    }
}
