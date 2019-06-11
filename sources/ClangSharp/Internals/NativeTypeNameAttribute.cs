using System;
using System.Diagnostics;

namespace ClangSharp
{
    [Conditional("DEBUG")]
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.ReturnValue, AllowMultiple = false, Inherited = true)]
    internal sealed class NativeTypeNameAttribute : Attribute
    {
        public NativeTypeNameAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }
}
