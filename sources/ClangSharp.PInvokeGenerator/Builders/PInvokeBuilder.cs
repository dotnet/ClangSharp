using System;
using ClangSharp.Interop;
using Microsoft.CodeAnalysis;

using NativeType = ClangSharp.Type;
using ManagedType = System.Type;

namespace ClangSharp
{
    public sealed class PInvokeBuilder : IDisposable
    {
        private readonly PInvokeGeneratorConfiguration _config;
        private readonly Index _index;
        private readonly Workspace _workspace;

        public string LibraryName { get; set; }

        public CXIndex IndexHandle => _index.Handle;
        public Workspace Workspace => _workspace;

        public PInvokeBuilder(PInvokeGeneratorConfiguration configuration)
        {
            _config = configuration;
            _index = Index.Create();
            _workspace = new AdhocWorkspace();
        }

        /*public PInvokeDocumentBuilder AddFile(string path)
        {
            var error = CXTranslationUnit.TryParse(IndexHandle, path, )
            TranslationUnit.GetOrCreate


            return this;
        }*/

        public void Dispose()
        {
            _workspace?.Dispose();
            _index?.Dispose();
        }
    }
}
