// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Runtime.InteropServices;

namespace ClangSharp.Interop;

// The UTF-8 path is null-terminated and is only valid for the duration of the callback.
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public unsafe delegate void CXModuleCachePruneCallback([NativeTypeName("const char *")] sbyte* path, [NativeTypeName("void *")] void* userData);
