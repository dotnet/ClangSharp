// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

namespace System.Diagnostics.CodeAnalysis;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
internal sealed class MemberNotNullWhenAttribute(bool returnValue, params string[] members) : Attribute
{
    public bool ReturnValue { get; } = returnValue;

    public string[] Members { get; } = members;
}
