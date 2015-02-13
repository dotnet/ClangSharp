namespace ClangSharpPInvokeGenerator
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using ClangSharp;

    public class Program
    {
        public static void Main(string[] args)
        {
            Regex re = new Regex(@"(?<switch>-{1,2}\S*)(?:[=:]?|\s+)(?<value>[^-\s].*?)?(?=\s+[-\/]|$)");
            List<KeyValuePair<string, string>> matches = (from match in re.Matches(string.Join(" ", args)).Cast<Match>()
                select new KeyValuePair<string, string>(match.Groups["switch"].Value, match.Groups["value"].Value))
                .ToList();

            var files = new List<string>();
            var includeDirs = new List<string>();
            string outputFile = string.Empty;
            string @namespace = string.Empty;
            string libraryPath = string.Empty;

            foreach (KeyValuePair<string, string> match in matches)
            {
                if (string.Equals(match.Key, "--n") || string.Equals(match.Key, "--namespace"))
                {
                    @namespace = match.Value;
                }

                if (string.Equals(match.Key, "--l") || string.Equals(match.Key, "--libraryPath"))
                {
                    libraryPath = match.Value;
                }

                if (string.Equals(match.Key, "--i") || string.Equals(match.Key, "--include"))
                {
                    includeDirs.Add(match.Value);
                }

                if (string.Equals(match.Key, "--o") || string.Equals(match.Key, "--output"))
                {
                    outputFile = match.Value;
                }

                if (string.Equals(match.Key, "--f") || string.Equals(match.Key, "--file"))
                {
                    files.Add(match.Value);
                }
            }

            var errorList = new List<string>();
            if (!files.Any())
            {
                errorList.Add("Error: No input C/C++ files provided. Use --file or --f");
            }

            if (string.IsNullOrWhiteSpace(@namespace))
            {
                errorList.Add("Error: No namespace provided. Use --namespace or --n");
            }

            if (string.IsNullOrWhiteSpace(outputFile))
            {
                errorList.Add("Error: No output file location provided. Use --output or --o");
            }

            if (string.IsNullOrWhiteSpace(libraryPath))
            {
                errorList.Add("Error: No library path location provided. Use --libraryPath or --l");
            }

            if (errorList.Any())
            {
                Console.WriteLine("Usage: ClangPInvokeGenerator --file [fileLocation] --libraryPath [library.dll] --output [output.cs] --namespace [Namespace] --include [headerFileIncludeDirs]");
                foreach (var error in errorList)
                {
                    Console.WriteLine(error);
                }
            }

            var createIndex = Methods.clang_createIndex(0, 0);
            string[] arr = { "-x", "c++" };
            
            arr = arr.Concat(includeDirs.Select(x => "-I" + x)).ToArray();

            List<CXTranslationUnit> translationUnits = new List<CXTranslationUnit>();

            foreach (var file in files)
            {
                CXTranslationUnit translationUnit;
                CXUnsavedFile unsavedFile;
                var translationUnitError = Methods.clang_parseTranslationUnit2(createIndex, file, arr, 3, out unsavedFile, 0, 0, out translationUnit);
                
                if (translationUnitError != CXErrorCode.CXError_Success)
                {
                    Console.WriteLine("Error: " + translationUnitError);
                    var numDiagnostics = Methods.clang_getNumDiagnostics(translationUnit);

                    for (uint i = 0; i < numDiagnostics; ++i)
                    {
                        var diagnostic = Methods.clang_getDiagnostic(translationUnit, i);
                        Console.WriteLine(Methods.clang_getDiagnosticSpelling(diagnostic).ToString());
                        Methods.clang_disposeDiagnostic(diagnostic);
                    }
                }

                translationUnits.Add(translationUnit);
            }

            using (var sw = new StreamWriter(outputFile))
            {
                sw.WriteLine("namespace " + @namespace);
                sw.WriteLine("{");

                sw.WriteLine("    using System;");
                sw.WriteLine("    using System.Runtime.InteropServices;");
                sw.WriteLine();

                var structVisitor = new StructVisitor(sw);
                foreach (var tu in translationUnits)
                {
                    Methods.clang_visitChildren(Methods.clang_getTranslationUnitCursor(tu), structVisitor.Visit, new CXClientData(IntPtr.Zero));
                }

                var typeDefVisitor = new TypeDefVisitor(sw);
                foreach (var tu in translationUnits)
                {
                    Methods.clang_visitChildren(Methods.clang_getTranslationUnitCursor(tu), typeDefVisitor.Visit, new CXClientData(IntPtr.Zero));
                }
                
                var enumVisitor = new EnumVisitor(sw);
                foreach (var tu in translationUnits)
                {
                    Methods.clang_visitChildren(Methods.clang_getTranslationUnitCursor(tu), enumVisitor.Visit, new CXClientData(IntPtr.Zero));
                }

                sw.WriteLine("    public static partial class Methods");
                sw.WriteLine("    {");
                {
                    var functionVisitor = new FunctionVisitor(sw, libraryPath);
                    foreach (var tu in translationUnits)
                    {
                        Methods.clang_visitChildren(Methods.clang_getTranslationUnitCursor(tu), functionVisitor.Visit, new CXClientData(IntPtr.Zero));
                    }    
                }
                sw.WriteLine("    }");
                sw.WriteLine("}");
            }

            foreach (var tu in translationUnits)
            {
                Methods.clang_disposeTranslationUnit(tu);
            }

            Methods.clang_disposeIndex(createIndex);
        }
    }
}