// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;

namespace ClangSharp.Interop
{
    public unsafe partial struct CXString : IDisposable
    {
        public static CXString ConstructUsr_ObjCClass(string className)
        {
            using var marshaledClassName = new MarshaledString(className);
            return clang.constructUSR_ObjCClass(marshaledClassName);
        }

        public static CXString ConstructUsr_ObjCCategory(string className, string categoryName)
        {
            using var marshaledClassName = new MarshaledString(className);
            using var marshaledCategoryName = new MarshaledString(categoryName);
            return clang.constructUSR_ObjCCategory(marshaledClassName, marshaledCategoryName);
        }

        public static CXString ConstructUsr_ObjCProtocol(string protocolName)
        {
            using var marshaledProtocolName = new MarshaledString(protocolName);
            return clang.constructUSR_ObjCProtocol(marshaledProtocolName);
        }

        public static CXString ConstructUsr_ObjCIvar(string name, CXString classUsr)
        {
            using var marshaledName = new MarshaledString(name);
            return clang.constructUSR_ObjCIvar(marshaledName, classUsr);
        }

        public static CXString ConstructUsr_ObjCMethod(string name, bool isInstanceMethod, CXString classUsr)
        {
            using var marshaledName = new MarshaledString(name);
            return clang.constructUSR_ObjCMethod(marshaledName, isInstanceMethod ? 1u : 0u, classUsr);
        }

        public static CXString ConstructUsr_ObjCProperty(string property, CXString classUsr)
        {
            using var marshaledProperty = new MarshaledString(property);
            return clang.constructUSR_ObjCProperty(marshaledProperty, classUsr);
        }

        public string CString
        {
            get
            {
                var pCString = clang.getCString(this);

                if (pCString is null)
                {
                    return string.Empty;
                }

                var span = new ReadOnlySpan<byte>(pCString, int.MaxValue);
                return span.Slice(0, span.IndexOf((byte)'\0')).AsString();
            }
        }

        public void Dispose() => clang.disposeString(this);

        public override string ToString() => CString;
    }
}
