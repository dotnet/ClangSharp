# The XML binding format

`ClangSharpPInvokeGenerator` can emit its bindings as XML instead of C# by passing
`--output-mode Xml` (`-om Xml`). This document describes the shape of that XML.

The XML output is a **structured serialization of the same information the C# emitter produces** — the
generator's final decisions about names, types, access, layout, calling conventions, and so on — not a
dump of the raw Clang AST. It exists so that downstream tooling can consume, diff, post-process, or
re-emit the bindings without having to parse C#.

> [!NOTE]
> This format is specific to this generator and mirrors the C# emitter. There is no formal schema
> (XSD), and the element/attribute shape can change alongside the C# output. Treat it as a convenience
> for tooling built against this repo rather than a stable, versioned contract. The examples below are
> taken directly from the generator's own baseline tests, so they reflect exactly what it emits.

## Producing XML output

The mode is selected on the command line and is otherwise driven by the same options and response
files as C# generation:

```
ClangSharpPInvokeGenerator -om Xml -n MyNamespace -m Methods -o bindings.xml -f header.h
```

Everything that shapes the C# output — `--methodClassName`, remappings, `--config` options, etc. —
applies identically here; only the serialization differs.

## Document structure

A generated file is a single `<bindings>` document. The declared namespace and, for free functions,
the static method class are represented as nesting `<namespace>` and `<class>` elements:

```xml
<?xml version="1.0" encoding="UTF-8" standalone="yes" ?>
<bindings>
  <namespace name="ClangSharp.Test">
    <class name="Methods" access="public" static="true">
      <function name="MyFunction" access="public" lib="ClangSharpPInvokeGenerator" convention="Cdecl" static="true" unsafe="true">
        <type>void</type>
        <param name="color">
          <type>float*</type>
        </param>
      </function>
    </class>
  </namespace>
</bindings>
```

* `<bindings>` — the document root.
* `<comment>` — emitted immediately inside `<bindings>` when a `--headerFile` / header text is
  configured; it carries that verbatim header text.
* `<namespace name="...">` — the output namespace (from `-n`).
* `<class name="..." access="public" static="true">` — the static holder for free functions (from
  `-m` / `--methodClassName`). Types such as structs and enums are emitted directly under
  `<namespace>` rather than inside this class. It gains `unsafe="true"` when the class contains unsafe
  members.

In single-file mode one `<bindings>` wraps the whole output; with `--multi-file`, each generated file
is its own complete `<bindings>` document.

## Types (`<type>`)

Almost every declaration carries a `<type>` child describing the emitted C# type as text. When the
original C/C++ spelling differs from the emitted type, it is preserved on the `native` attribute:

```xml
<field name="r" access="public">
  <type native="unsigned int">uint</type>
</field>
```

In value contexts (enumerators, constants) `<type>` also carries `primitive="True|False"` indicating
whether the value is a primitive.

## Structs (`<struct>`)

```xml
<struct name="MyStruct2" access="public" unsafe="true" layout="Sequential" pack="4">
  <field name="Field1" access="public">
    <type native="unsigned int">uint</type>
  </field>
  <field name="Field2" access="public">
    <type>void*</type>
  </field>
</struct>
```

`<struct>` attributes (each emitted only when applicable):

* `name`, `access` — the escaped name and access specifier.
* `native` — the original native type name, when it differs.
* `parent` — the native base type, for inherited layouts.
* `uuid` — the associated GUID, when present.
* `vtbl="true"` — the struct has a vtable.
* `unsafe="true"` — the struct requires an unsafe context.
* `layout` / `pack` — the `StructLayout` kind and packing, when explicitly emitted.

`<field>` elements carry `name`, `access`, and optionally `inherited` (the type a field is inherited
from) and `offset`. A fixed-size buffer adds `count` and `fixed` attributes to its `<type>`.

## Enums (`<enumeration>`)

```xml
<enumeration name="MyEnum" access="public">
  <type>int</type>
  <enumerator name="MyEnum_Value1" access="public">
    <type primitive="False">int</type>
    <value>
      <code>1</code>
    </value>
  </enumerator>
  <enumerator name="MyEnum_Value2" access="public">
    <type primitive="False">int</type>
  </enumerator>
</enumeration>
```

The `<enumeration>` `<type>` is the underlying integral type. Each `<enumerator>` optionally contains a
`<value>` wrapping the initializer; the initializer expression itself is emitted inside `<code>` (see
[Embedded C# expressions](#embedded-c-expressions)).

## Constants and fields (`<constant>` / `<field>`)

Value declarations are emitted as `<constant>` when constant, or `<field>` otherwise, following the
same `<type>` + optional `<value>` shape:

```xml
<constant name="x" access="private">
  <type primitive="True">float</type>
  <value>
    <code>1_024.0</code>
  </value>
</constant>
```

## Functions and delegates (`<function>` / `<delegate>`)

Non-virtual functions are `<function>`; virtual methods (function-pointer/vtable slots) are
`<delegate>`. The `<type>` immediately inside is the return type; parameters follow as `<param>`:

```xml
<function name="MyFunction" access="public" lib="clang" convention="Cdecl" static="true" unsafe="true">
  <type>void</type>
  <param name="index">
    <type>int</type>
  </param>
</function>
```

Common attributes:

* `name`, `access` — as elsewhere.
* `lib` — the P/Invoke library, for `DllImport` functions.
* `convention` — the calling convention, when it is not the default `Winapi`.
* `entrypoint` — the native entry point, when it differs from `name`.
* `setlasterror="true"` — sets `SetLastError`.
* `static="true"`, `readonly="true"`, `unsafe="true"` — as applicable.
* `vtblindex` — the vtable slot index, for virtual methods.

`<param>` elements carry a `name` and a `<type>`; a default argument is emitted inside `<init>`. A
method body, when generated, is emitted inside `<body>`.

## Properties and indexers

Property accessors and indexers reuse the surrounding declaration and add accessor elements:

* `<get>` / `<set>` — accessor bodies; `inlining="aggressive"` marks aggressive inlining.
* `<indexer access="..." unsafe="...">` with a `<type>` for the element type.

## Embedded C# expressions (`<code>`)

Where the binding requires C# that has no structured XML representation — initializer expressions,
generated helper bodies, and similar — the generator falls back to emitting the raw C# text inside a
`<code>` element (as seen in the enum and constant examples above).

## Other elements

* `<comment>` — the configured header text, at the top of the document.
* `<attribute>` — a custom attribute attached to the following declaration.
* `<iid name="..." value="..." />` — an interface IID.
* `<vtbl>` — an explicit vtable struct.
* `<value>`, `<cast>`, `<deref>`, `<unchecked>` — expression-shaping wrappers used within values and
  bodies.

## When to use it

Prefer the default C# output unless you specifically need a machine-readable description of the
bindings — for example to drive a custom code generator, produce documentation, or diff the generator's
decisions across runs. For everything else, see
[Generating bindings: best practices](generating-bindings-best-practices.md).
