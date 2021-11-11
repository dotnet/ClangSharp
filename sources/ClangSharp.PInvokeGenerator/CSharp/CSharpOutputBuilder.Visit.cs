// Copyright (c) .NET Foundation and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using System.Runtime.CompilerServices;

namespace ClangSharp.CSharp
{
    internal partial class CSharpOutputBuilder
    {
        private bool _customAttrIsForParameter = false;
        public void WriteCustomAttribute(string attribute, Action callback = null)
        {
            if (attribute.Equals("Flags") || attribute.Equals("Obsolete"))
            {
                AddUsingDirective("System");
            }
            else if (attribute.Equals("EditorBrowsable") || attribute.StartsWith("EditorBrowsable("))
            {
                AddUsingDirective("System.ComponentModel");
            }
            else if (attribute.StartsWith("Guid(") || attribute.StartsWith("Optional, DefaultParameterValue("))
            {
                AddUsingDirective("System.Runtime.InteropServices");
            }
            else if (attribute.StartsWith("SupportedOSPlatform("))
            {
                AddUsingDirective("System.Runtime.Versioning");
            }

            if (!_customAttrIsForParameter)
            {
                WriteIndentation();
            }

            Write('[');
            Write(attribute);
            callback?.Invoke();
            Write(']');

            if (!_customAttrIsForParameter)
            {
                WriteNewline();
            }
            else
            {
                Write(' ');
            }
        }

        public void WriteDivider(bool force = false)
        {
            if (force)
            {
                WriteNewline();
            }
            else
            {
                NeedsNewline = true;
            }
        }

        public void SuppressDivider() => NeedsNewline = false;

        public void WriteIid(string name, Guid value)
        {
            if (_config.GenerateUnmanagedConstants)
            {
                AddUsingDirective("System");
                AddUsingDirective("System.Diagnostics");
                AddUsingDirective("System.Runtime.CompilerServices");
                AddUsingDirective("System.Runtime.InteropServices");

                WriteIndented("public static ref readonly Guid ");
                WriteLine(name);
                WriteBlockStart();

                WriteIndentedLine("get");
                WriteBlockStart();

                WriteIndentedLine("ReadOnlySpan<byte> data = new byte[] {");
                IncreaseIndentation();
                WriteIndentation();

                WriteValueAsBytes(Unsafe.As<Guid, uint>(ref value), 4);

                WriteLine(',');
                WriteIndentation();

                WriteValueAsBytes(Unsafe.As<Guid, ushort>(ref Unsafe.AddByteOffset(ref value, (nint)4)), 2);

                WriteLine(',');
                WriteIndentation();

                WriteValueAsBytes(Unsafe.As<Guid, ushort>(ref Unsafe.AddByteOffset(ref value, (nint)6)), 2);

                for (var i = 8; i < 16; i++)
                {
                    WriteLine(',');
                    WriteIndentation();

                    WriteValueAsBytes(Unsafe.As<Guid, ushort>(ref Unsafe.AddByteOffset(ref value, (nint)i)), 1);
                }

                WriteNewline();
                DecreaseIndentation();
                WriteIndentedLine("};");

                NeedsNewline = true;

                WriteIndentedLine("Debug.Assert(data.Length == Unsafe.SizeOf<Guid>());");
                WriteIndentedLine("return ref Unsafe.As<byte, Guid>(ref MemoryMarshal.GetReference(data));");

                WriteBlockEnd();
                WriteBlockEnd();
            }
            else
            {
                var valueString = value.ToString("X").ToUpperInvariant().Replace("{", "").Replace("}", "").Replace('X', 'x').Replace(",", ", ");
                WriteIid(name, valueString);
            }
        }

        public void WriteIid(string name, string value)
        {
            AddUsingDirective("System");
            WriteIndented("public static readonly Guid ");
            Write(name);
            Write(" = new Guid(");
            Write(value);
            Write(")");
            WriteSemicolon();
            WriteNewline();
        }

        public void EmitUsingDirective(string directive) => AddUsingDirective(directive);
    }
}
