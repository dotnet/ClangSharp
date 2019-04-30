using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ClangSharp;

namespace ClangSharpPInvokeGenerator
{
    public class Program
    {
        private static RootCommand s_rootCommand;

        public static async Task<int> Main(string[] args)
        {
            s_rootCommand = new RootCommand();
            {
                s_rootCommand.Description = "ClangSharp P/Invoke Binding Generator";
                s_rootCommand.Handler = CommandHandler.Create(typeof(Program).GetMethod(nameof(Run)));

                AddAdditionalOption(s_rootCommand);
                AddConfigOption(s_rootCommand);
                AddDefineOption(s_rootCommand);
                AddExcludeFunctionOption(s_rootCommand);
                AddFileOption(s_rootCommand);
                AddIncludeOption(s_rootCommand);
                AddLibraryOption(s_rootCommand);
                AddMethodClassNameOption(s_rootCommand);
                AddNamespaceOption(s_rootCommand);
                AddOutputOption(s_rootCommand);
                AddPrefixStripOption(s_rootCommand);
            }
            return await s_rootCommand.InvokeAsync(args);
        }

        public static int Run(InvocationContext context)
        {
            var additionalArgs = context.ParseResult.ValueForOption<string[]>("additional");
            var config = new ConfigurationOptions(context.ParseResult.ValueForOption<string[]>("config"));
            var defines = context.ParseResult.ValueForOption<string[]>("define");
            config.ExcludedFunctions = context.ParseResult.ValueForOption<string[]>("excludeFunction");
            var files = context.ParseResult.ValueForOption<string[]>("file");
            var includeDirs = context.ParseResult.ValueForOption<string[]>("include");
            config.LibraryPath = context.ParseResult.ValueForOption<string>("libraryPath");
            config.MethodClassName = context.ParseResult.ValueForOption<string>("methodClassName");
            config.Namespace = context.ParseResult.ValueForOption<string>("namespace");
            var outputFile = context.ParseResult.ValueForOption<string>("output");
            config.MethodPrefixToStrip = context.ParseResult.ValueForOption<string>("prefixStrip");

            var errorList = new List<string>();

            if (!files.Any())
            {
                errorList.Add("Error: No input C/C++ files provided. Use --file or -f");
            }

            if (string.IsNullOrWhiteSpace(config.Namespace))
            {
                errorList.Add("Error: No namespace provided. Use --namespace or -n");
            }

            if (string.IsNullOrWhiteSpace(outputFile))
            {
                errorList.Add("Error: No output file location provided. Use --output or -o");
            }

            if (string.IsNullOrWhiteSpace(config.LibraryPath))
            {
                errorList.Add("Error: No library path location provided. Use --libraryPath or -l");
            }

            if (errorList.Any())
            {
                foreach (var error in errorList)
                {
                    context.Console.Error.WriteLine(error);
                }
                context.Console.Error.WriteLine();

                new HelpBuilder(context.Console).Write(s_rootCommand);
                return -1;
            }

                var createIndex = CXIndex.Create();
            string[] arr = { "-x", "c++" };

            arr = arr.Concat(includeDirs.Select(x => "-I" + x)).ToArray();
            arr = arr.Concat(defines.Select(x => "-D" + x)).ToArray();
            arr = arr.Concat(additionalArgs).ToArray();

            var translationUnits = new List<CXTranslationUnit>();

            foreach (var file in files)
            {
                CXTranslationUnit translationUnit;
                CXUnsavedFile[] unsavedFile = new CXUnsavedFile[0];
                var translationUnitError = CXTranslationUnit.Parse(createIndex, file, arr, unsavedFile, CXTranslationUnit_Flags.CXTranslationUnit_None, out translationUnit);

                if (translationUnitError != CXErrorCode.CXError_Success)
                {
                    Console.WriteLine($"Error: '{translationUnitError}' for '{file}'.");
                    var numDiagnostics = translationUnit.NumDiagnostics;

                    for (uint i = 0; i < numDiagnostics; ++i)
                    {
                        var diagnostic = translationUnit.GetDiagnostic(i);
                        Console.WriteLine(diagnostic.Spelling.ToString());
                        diagnostic.Dispose();
                    }
                }

                translationUnits.Add(translationUnit);
            }

            using (var sw = new StreamWriter(outputFile))
            {
                sw.NewLine = "\n";

                sw.WriteLine("namespace " + config.Namespace);
                sw.WriteLine("{");

                sw.WriteLine("    using System;");
                sw.WriteLine("    using System.Runtime.InteropServices;");
                sw.WriteLine();

                var writer = new CursorWriter(config, sw, indentation: 1, (cursor) => cursor.Kind != CXCursorKind.CXCursor_FunctionDecl);
                foreach (var tu in translationUnits)
                {
                    tu.Cursor.VisitChildren(writer.VisitTranslationUnit, clientData: default);
                }

                sw.Write("    public static ");

                if (config.GenerateUnsafeCode)
                {
                    sw.Write("unsafe ");
                }

                sw.WriteLine("partial class " + config.MethodClassName);
                sw.WriteLine("    {");
                {
                    var functionDeclWriter = new CursorWriter(config, sw, indentation: 2, (cursor) => cursor.Kind == CXCursorKind.CXCursor_FunctionDecl);

                    sw.WriteLine($"        private const string libraryPath = \"{config.LibraryPath}\";");
                    sw.WriteLine();

                    foreach (var tu in translationUnits)
                    {
                        tu.Cursor.VisitChildren(functionDeclWriter.VisitTranslationUnit, new CXClientData(IntPtr.Zero));
                    }
                }
                sw.WriteLine("    }");
                sw.WriteLine("}");
            }

            foreach (var tu in translationUnits)
            {
                tu.Dispose();
            }

            createIndex.Dispose();
            return 0;
        }

        private static void AddAdditionalOption(RootCommand rootCommand)
        {
            var argument = new Argument();
            argument.ArgumentType = typeof(string);
            argument.Arity = ArgumentArity.ZeroOrMore;
            argument.Name = "arg";
            argument.SetDefaultValue(Array.Empty<string>());

            var option = new Option("--additional", "An argument to pass to Clang when parsing the input files.", argument);
            option.AddAlias("--a");

            rootCommand.AddOption(option);
        }

        private static void AddConfigOption(RootCommand rootCommand)
        {
            var argument = new Argument();
            argument.ArgumentType = typeof(string);
            argument.Arity = ArgumentArity.ZeroOrMore;
            argument.Name = "config";
            argument.SetDefaultValue(Array.Empty<string>());

            var option = new Option("--config", "A configuration option that controls how the bindings are generated.", argument);
            option.AddAlias("--c");

            rootCommand.AddOption(option);
        }

        private static void AddDefineOption(RootCommand rootCommand)
        {
            var argument = new Argument();
            argument.ArgumentType = typeof(string);
            argument.Arity = ArgumentArity.ZeroOrMore;
            argument.Name = "macro";
            argument.SetDefaultValue(Array.Empty<string>());

            var option = new Option("--define", "A macro for Clang to define when parsing the input files.", argument);
            option.AddAlias("--d");

            rootCommand.AddOption(option);
        }

        private static void AddExcludeFunctionOption(RootCommand rootCommand)
        {
            var argument = new Argument();
            argument.ArgumentType = typeof(string);
            argument.Arity = ArgumentArity.ZeroOrMore;
            argument.Name = "name";
            argument.SetDefaultValue(Array.Empty<string>());

            var option = new Option("--excludeFunction", "A function to exclude from binding generation.", argument);
            option.AddAlias("--e");

            rootCommand.AddOption(option);
        }

        private static void AddFileOption(RootCommand rootCommand)
        {
            var argument = new Argument();
            argument.ArgumentType = typeof(string);
            argument.Arity = ArgumentArity.OneOrMore;
            argument.Name = "file";
            argument.SetDefaultValue(Array.Empty<string>());

            var option = new Option("--file", "A file to parse and generate bindings for.", argument);
            option.AddAlias("--f");

            rootCommand.AddOption(option);
        }

        private static void AddIncludeOption(RootCommand rootCommand)
        {
            var argument = new Argument();
            argument.ArgumentType = typeof(string);
            argument.Arity = ArgumentArity.ZeroOrMore;
            argument.Name = "directory";
            argument.SetDefaultValue(Array.Empty<string>());

            var option = new Option("--include", "A directory for clang to use when resolving #include directives.", argument);
            option.AddAlias("--i");

            rootCommand.AddOption(option);
        }

        private static void AddLibraryOption(RootCommand rootCommand)
        {
            var argument = new Argument();
            argument.ArgumentType = typeof(string);
            argument.Arity = ArgumentArity.ExactlyOne;
            argument.Name = "dllName";
            argument.SetDefaultValue(string.Empty);

            var option = new Option("--libraryPath", "The string to use in the DllImport attribute used when generating bindings.", argument);
            option.AddAlias("--l");

            rootCommand.AddOption(option);
        }

        private static void AddMethodClassNameOption(RootCommand rootCommand)
        {
            var argument = new Argument();
            argument.ArgumentType = typeof(string);
            argument.Arity = ArgumentArity.ZeroOrOne;
            argument.Name = "className";
            argument.SetDefaultValue("Methods");

            var option = new Option("--methodClassName", "The name of the static class that will contain the generated method bindings.", argument);
            option.AddAlias("--m");

            rootCommand.AddOption(option);
        }

        private static void AddNamespaceOption(RootCommand rootCommand)
        {
            var argument = new Argument();
            argument.ArgumentType = typeof(string);
            argument.Arity = ArgumentArity.ExactlyOne;
            argument.Name = "namespace";
            argument.SetDefaultValue(string.Empty);

            var option = new Option("--namespace", "The namespace in which to place the generated bindings.", argument);
            option.AddAlias("--n");

            rootCommand.AddOption(option);
        }

        private static void AddOutputOption(RootCommand rootCommand)
        {
            var argument = new Argument();
            argument.ArgumentType = typeof(string);
            argument.Arity = ArgumentArity.ExactlyOne;
            argument.Name = "file";
            argument.SetDefaultValue(string.Empty);

            var option = new Option("--output", "The output file to write the generated bindings to.", argument);
            option.AddAlias("--o");

            rootCommand.AddOption(option);
        }

        private static void AddPrefixStripOption(RootCommand rootCommand)
        {
            var argument = new Argument();
            argument.ArgumentType = typeof(string);
            argument.Arity = ArgumentArity.ExactlyOne;
            argument.Name = "prefix";
            argument.SetDefaultValue(string.Empty);

            var option = new Option("--prefixStrip", "The prefix to strip from the generated method bindings.", argument);
            option.AddAlias("--p");

            rootCommand.AddOption(option);
        }
    }
}
