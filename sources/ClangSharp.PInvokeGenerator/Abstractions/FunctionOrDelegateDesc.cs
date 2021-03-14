﻿using System;

namespace ClangSharp.Abstractions
{
    internal struct FunctionOrDelegateDesc<TCustomAttrGeneratorData>
    {
        public string AccessSpecifier;
        public string NativeTypeName;
        public string EscapedName;
        public string EntryPoint;
        public string CallingConventionName;
        public string LibraryPath;
        public bool IsVirtual;
        public bool IsDllImport;
        public bool HasFnPtrCodeGen;
        public bool IsAggressivelyInlined;
        public bool SetLastError;
        public bool IsCxx;
        public bool? IsStatic; // true - static, false - not static, null - infer
        public bool NeedsNewKeyword;
        public bool IsUnsafe;
        public bool IsCtxCxxRecord;
        public bool IsCxxRecordCtxUnsafe;
        public bool IsMemberFunction;
        public Action<TCustomAttrGeneratorData> WriteCustomAttrs;
        public TCustomAttrGeneratorData CustomAttrGeneratorData;
    }
}