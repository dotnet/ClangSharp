using System.Diagnostics;
using System.Text;
using ClangSharp.Abstractions;
using ClangSharp.CSharp;

namespace ClangSharp.XML
{
    internal partial class XmlOutputBuilder
    {
        private StringBuilder _sb = new();
        public void BeginInnerValue() => _sb.Append("<value>");
        public void EndInnerValue() => _sb.Append("</value>");

        public void BeginInnerCast() => _sb.Append("<cast>");
        public void WriteCastType(string targetTypeName) => _sb.Append(targetTypeName);
        public void EndInnerCast() => _sb.Append("</cast>");

        public void BeginUnchecked() => _sb.Append("<unchecked>");
        public void EndUnchecked() => _sb.Append("</unchecked>");

        public void BeginConstant(in ConstantDesc desc)
        {
            _sb.Append((desc.Kind & ConstantKind.Enumerator) == 0
                ? $"<constant name=\"{desc.EscapedName}\" access=\"{desc.AccessSpecifier}\">"
                : $"<enumerator name=\"{desc.EscapedName}\" access=\"{desc.AccessSpecifier}\">");
            _sb.Append($"<type primitive=\"{(desc.Kind & ConstantKind.PrimitiveConstant) != 0}\">");
            _sb.Append(desc.TypeName.Replace("<", "&lt;").Replace(">", "&gt;"));
            _sb.Append("</type>");
        }

        public void BeginConstantValue(bool isGetOnlyProperty = false) => _sb.Append("<value>");
        public void WriteConstantValue(long value) => _sb.Append(value);
        public void WriteConstantValue(ulong value) => _sb.Append(value);
        public void EndConstantValue() => _sb.Append("</value>");
        public void EndConstant(bool isConstant) => _sb.Append(isConstant ? "</constant>" : "</enumerator>");

        public void BeginEnum(string accessSpecifier, string typeName, string escapedName, string nativeTypeName)
            => _sb.Append($"<enumeration name=\"{escapedName}\" access=\"{accessSpecifier}\"><type>{typeName}</type>");

        public void EndEnum() => _sb.Append("</enumeration>");

        public void BeginField(string accessSpecifier, string nativeTypeName, string escapedName, int? offset,
            bool needsNewKeyword, string inheritedFrom = null)
        {
            _sb.Append($"<field name=\"{escapedName}\" access=\"{accessSpecifier}\"");
            if (inheritedFrom is not null)
            {
                _sb.Append($" inherited=\"{inheritedFrom}\"");
            }

            if (offset is not null)
            {
                _sb.Append($" offset=\"{offset}\"");
            }

            _sb.Append('>');
            _sb.Append($"<type native=\"{nativeTypeName}\"");
        }

        public void WriteFixedCountField(string typeName, string escapedName, string fixedName, string count)
            => _sb.Append($" count=\"{count}\" fixed=\"{fixedName}\">" +
                          $"{typeName.Replace("<", "&lt;").Replace(">", "&gt;")}</type>");

        public void WriteRegularField(string typeName, string escapedName)
            => _sb.Append($">{typeName.Replace("<", "&lt;").Replace(">", "&gt;")}</type>");
        public void EndField(bool isBodyless = true) => _sb.Append("</field>");
        public void BeginFunctionOrDelegate<TCustomAttrGeneratorData>(
            in FunctionOrDelegateDesc<TCustomAttrGeneratorData> desc, ref bool isMethodClassUnsafe)
        {
            if (desc.IsVirtual)
            {
                Debug.Assert(!desc.HasFnPtrCodeGen);
                _sb.Append($"<delegate name=\"{desc.EscapedName}\" access=\"{desc.AccessSpecifier}\"");
                if (desc.CallingConventionName != "Winapi")
                {
                    _sb.Append($" convention=\"{desc.CallingConventionName}\"");
                }
            }
            else if (desc.IsDllImport)
            {
                _sb.Append($"<function name=\"{desc.EscapedName}\" access=\"{desc.AccessSpecifier}\"");
                _sb.Append($" lib=\"{desc.LibraryPath}\"");
                if (desc.CallingConventionName != "Winapi")
                {
                    _sb.Append($" convention=\"{desc.CallingConventionName}\"");
                }

                if (desc.EntryPoint != desc.EscapedName)
                {
                    _sb.Append($" entrypoint=\"{desc.EntryPoint}\"");
                }

                if (desc.SetLastError)
                {
                    _sb.Append(" setlasterror=\"true\"");
                }
            }
            else
            {
                _sb.Append($"<function name=\"{desc.EscapedName}\" access=\"{desc.AccessSpecifier}\"");
            }

            if (!desc.IsMemberFunction && (desc.IsStatic ?? (desc.IsDllImport || !desc.IsCxx)))
            {
                _sb.Append(" static=\"true\"");
            }

            if (!desc.IsUnsafe)
            {
                _sb.Append(" unsafe=\"true\"");
            }

            _sb.Append('>');

            desc.WriteCustomAttrs(desc.CustomAttrGeneratorData);
            _sb.Append("<type");
            if (!string.IsNullOrWhiteSpace(desc.NativeTypeName))
            {
                _sb.Append(" native=\"");
                _sb.Append(desc.NativeTypeName);
                _sb.Append('"');
            }

            _sb.Append('>');
        }

        public void WriteReturnType(string typeString)
        {
            _sb.Append(typeString.Replace("<", "&lt;").Replace(">", "&gt;"));
            _sb.Append("</type>");
        }

        public void BeginFunctionInnerPrototype(string escapedName)
        {
            // nop, only used in C#
        }

        public void BeginParameter<TCustomAttrGeneratorData>(in ParameterDesc<TCustomAttrGeneratorData> info)
        {
            _sb.Append($"<param name=\"{info.Name}\">");
            info.WriteCustomAttrs(info.CustomAttrGeneratorData);
            _sb.Append("<type>");
            _sb.Append(info.Type.Replace("<", "&lt;").Replace(">", "&gt;"));
            _sb.Append("</type>");
        }

        public void BeginParameterDefault()
        {
            _sb.Append("<init>");
        }

        public void EndParameterDefault()
        {
            _sb.Append("</init>");
        }

        public void EndParameter()
        {
            _sb.Append("</param>");
        }

        public void WriteParameterSeparator()
        {
            // nop, used only in C#
        }

        public void EndFunctionInnerPrototype()
        {
            // nop, used only in C#
        }

        public void BeginBody(bool isExpressionBody = false)
        {
            // nop, used only by C#
        }

        public void BeginConstructorInitializers()
        {
            // nop, method only exists for consistency and/or future use
        }

        public void BeginConstructorInitializer(string memberRefName, string memberInitName)
        {
            // "hint" is the name we're initializing using, but should only be used as a "hint" rather than a definitive
            // value, which is contained within the init block.
            _sb.Append($"<init field=\"{memberRefName}\" hint=\"{memberInitName}\">");
        }

        public void EndConstructorInitializer() => _sb.Append("</init>");

        public void EndConstructorInitializers()
        {
            // nop, method only exists for consistency and/or future use
        }

        public void BeginInnerFunctionBody()
        {
            _sb.Append("<body>");
        }

        public void EndInnerFunctionBody()
        {
            _sb.Append("</body>");
        }

        public void EndBody(bool isExpressionBody = false)
        {
            // nop, used only by C#
        }

        public void EndFunctionOrDelegate(bool isVirtual, bool _)
            => _sb.Append(isVirtual ? "</delegate>" : "</function>");

        public void BeginStruct<TCustomAttrGeneratorData>(in StructDesc<TCustomAttrGeneratorData> info)
        {
            _sb.Append("<struct name=\"");
            _sb.Append(info.EscapedName);
            _sb.Append("\" access=\"");
            _sb.Append(info.AccessSpecifier);
            _sb.Append('"');
            if (info.NativeType is not null)
            {
                _sb.Append(" native=\"");
                _sb.Append(info.NativeType);
                _sb.Append('"');
            }

            if (info.NativeInheritance is not null)
            {
                _sb.Append(" parent=\"");
                _sb.Append(info.NativeInheritance);
                _sb.Append('"');
            }

            if (info.Uuid is not null)
            {
                _sb.Append(" uuid=\"");
                _sb.Append(info.Uuid.Value);
                _sb.Append('"');
            }

            if (info.HasVtbl)
            {
                _sb.Append(" vtbl=\"true\"");
            }

            if (info.IsUnsafe)
            {
                _sb.Append(" unsafe=\"true\"");
            }

            if (info.Layout is not null)
            {
                _sb.Append(" layout=\"");
                _sb.Append(info.Layout.Value);
                _sb.Append('"');
                if (info.Layout.Pack != default)
                {
                    _sb.Append(" pack=\"");
                    _sb.Append(info.Layout.Pack);
                    _sb.Append('"');
                }
            }

            _sb.Append('>');
            info.WriteCustomAttrs(info.CustomAttrGeneratorData);
        }

        public void BeginExplicitVtbl() => _sb.Append("<vtbl>");
        public void EndExplicitVtbl() => _sb.Append("</vtbl>");

        public void EndStruct() => _sb.Append("</struct>");

        public void EmitCompatibleCodeSupport()
        {
            // nop, used only by C#
        }

        public void EmitFnPtrSupport()
        {
            // nop, used only by C#
        }

        public void EmitSystemSupport()
        {
            // nop, used only by C#
        }

        public CSharpOutputBuilder BeginCSharpCode()
        {
            _sb.Append("<code>");
            return new CSharpOutputBuilder("__Internal", markerMode: MarkerMode.Xml);
        }

        public void EndCSharpCode(CSharpOutputBuilder output)
        {
            output.WritePendingLine();
            foreach (var s in output.Contents)
            {
                _sb.AppendLine(s.Replace("&", "&amp;")
                    .Replace("<", "&lt;")
                    .Replace(">", "&gt;")
                    .Replace("/*M*/&lt;", "<")
                    .Replace("/*M*/&gt;", ">"));
            }

            _sb.Append("</code>");
        }

        public void BeginGetter(bool aggressivelyInlined)
        {
            _sb.Append("<get");
            if (aggressivelyInlined)
            {
                _sb.Append(" inlining=\"aggressive\"");
            }

            _sb.Append('>');
        }

        public void EndGetter()
        {
            _sb.Append("</get>");
        }

        public void BeginSetter(bool aggressivelyInlined)
        {
            _sb.Append("<set");
            if (aggressivelyInlined)
            {
                _sb.Append(" inlining=\"aggressive\"");
            }

            _sb.Append('>');
        }

        public void EndSetter()
        {
            _sb.Append("</set>");
        }

        public void BeginIndexer(string accessSpecifier, bool isUnsafe)
        {
            _sb.Append("<indexer access=\"");
            _sb.Append(accessSpecifier);
            _sb.Append(isUnsafe ? "\" unsafe=\"true\">" : "\">");
        }

        public void WriteIndexer(string typeName)
        {
            _sb.Append("<type>");
            _sb.Append(typeName.Replace("<", "&lt;").Replace(">", "&gt;"));
            _sb.Append("</type>");
        }

        public void BeginIndexerParameters()
        {
            // nop, used only by C#
        }

        public void EndIndexerParameters()
        {
            // nop, used only by C#
        }

        public void EndIndexer() => _sb.Append("</indexer>");

        public void BeginDereference() => _sb.Append("<deref>");

        public void EndDereference() => _sb.Append("</deref>");
    }
}
