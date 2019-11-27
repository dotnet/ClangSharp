// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using ClangSharp.Interop;

namespace ClangSharp
{
    public class Program
    {
        private static RootCommand s_rootCommand;

        public static async Task<int> Main(params string[] args)
        {
            s_rootCommand = new RootCommand("ClangSharp P/Invoke Binding Generator")
            {
                Handler = CommandHandler.Create(typeof(Program).GetMethod(nameof(Run)))
            };

            AddAdditionalOption(s_rootCommand);
            AddConfigOption(s_rootCommand);
            AddDefineMacroOption(s_rootCommand);
            AddExcludeOption(s_rootCommand);
            AddFileOption(s_rootCommand);
            AddHeaderOption(s_rootCommand);
            AddIncludeDirectoryOption(s_rootCommand);
            AddLanguageOption(s_rootCommand);
            AddLibraryOption(s_rootCommand);
            AddMethodClassNameOption(s_rootCommand);
            AddNamespaceOption(s_rootCommand);
            AddOutputOption(s_rootCommand);
            AddPrefixStripOption(s_rootCommand);
            AddRemapOption(s_rootCommand);
            AddStdOption(s_rootCommand);
            AddTraverseOption(s_rootCommand);

            return await s_rootCommand.InvokeAsync(args);
        }

        public static int Run(InvocationContext context)
        {
            var additionalArgs = context.ParseResult.ValueForOption<string[]>("additional");
            var configSwitches = context.ParseResult.ValueForOption<string[]>("config");
            var defineMacros = context.ParseResult.ValueForOption<string[]>("define-macro");
            var excludedNames = context.ParseResult.ValueForOption<string[]>("exclude");
            var files = context.ParseResult.ValueForOption<string[]>("file");
            var headerFile = context.ParseResult.ValueForOption<string>("headerFile");
            var includeDirectories = context.ParseResult.ValueForOption<string[]>("include-directory");
            var language = context.ParseResult.ValueForOption<string>("language");
            var libraryPath = context.ParseResult.ValueForOption<string>("libraryPath");
            var methodClassName = context.ParseResult.ValueForOption<string>("methodClassName");
            var methodPrefixToStrip = context.ParseResult.ValueForOption<string>("prefixStrip");
            var namespaceName = context.ParseResult.ValueForOption<string>("namespace");
            var outputLocation = context.ParseResult.ValueForOption<string>("output");
            var remappedNameValuePairs = context.ParseResult.ValueForOption<string[]>("remap");
            var std = context.ParseResult.ValueForOption<string>("std");
            var traversalNames = context.ParseResult.ValueForOption<string[]>("traverse");

            var errorList = new List<string>();

            if (!files.Any())
            {
                errorList.Add("Error: No input C/C++ files provided. Use --file or -f");
            }

            if (string.IsNullOrWhiteSpace(libraryPath))
            {
                errorList.Add("Error: No library path location provided. Use --libraryPath or -l");
            }

            if (string.IsNullOrWhiteSpace(namespaceName))
            {
                errorList.Add("Error: No namespace provided. Use --namespace or -n");
            }

            if (string.IsNullOrWhiteSpace(outputLocation))
            {
                errorList.Add("Error: No output file location provided. Use --output or -o");
            }

            var remappedNames = new Dictionary<string, string>();

            foreach (var remappedNameValuePair in remappedNameValuePairs)
            {
                var parts = remappedNameValuePair.Split('=');

                if (parts.Length != 2)
                {
                    errorList.Add($"Error: Invalid remap argument: {remappedNameValuePair}. Expected 'name=value'");
                    continue;
                }

                remappedNames[parts[0].TrimEnd()] = parts[1].TrimStart();
            }

            var configOptions = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? PInvokeGeneratorConfigurationOptions.None : PInvokeGeneratorConfigurationOptions.GenerateUnixTypes;

            foreach (var configSwitch in configSwitches)
            {
                switch (configSwitch)
                {
                    case "compatible-codegen":
                    {
                        configOptions |= PInvokeGeneratorConfigurationOptions.GenerateCompatibleCode;
                        break;
                    }

                    case "default-remappings":
                    {
                        configOptions &= ~PInvokeGeneratorConfigurationOptions.NoDefaultRemappings;
                        break;
                    }

                    case "latest-codegen":
                    {
                        configOptions &= ~PInvokeGeneratorConfigurationOptions.GenerateCompatibleCode;
                        break;
                    }

                    case "multi-file":
                    {
                        configOptions |= PInvokeGeneratorConfigurationOptions.GenerateMultipleFiles;
                        break;
                    }

                    case "no-default-remappings":
                    {
                        configOptions |= PInvokeGeneratorConfigurationOptions.NoDefaultRemappings;
                        break;
                    }

                    case "single-file":
                    {
                        configOptions &= ~PInvokeGeneratorConfigurationOptions.GenerateMultipleFiles;
                        break;
                    }

                    case "unix-types":
                    {
                        configOptions |= PInvokeGeneratorConfigurationOptions.GenerateUnixTypes;
                        break;
                    }

                    case "windows-types":
                    {
                        configOptions &= ~PInvokeGeneratorConfigurationOptions.GenerateUnixTypes;
                        break;
                    }

                    default:
                    {
                        errorList.Add($"Error: Unrecognized config switch: {configSwitch}.");
                        break;
                    }
                }
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

            var clangCommandLineArgs = new string[]
            {
                $"--language={language}",               // Treat subsequent input files as having type <language>
                $"--std={std}",                         // Language standard to compile for
                "-Wno-pragma-once-outside-header"       // We are processing files which may be header files
            };

            clangCommandLineArgs = clangCommandLineArgs.Concat(includeDirectories.Select(x => "--include-directory=" + x)).ToArray();
            clangCommandLineArgs = clangCommandLineArgs.Concat(defineMacros.Select(x => "--define-macro=" + x)).ToArray();
            clangCommandLineArgs = clangCommandLineArgs.Concat(additionalArgs).ToArray();

            var translationFlags = CXTranslationUnit_Flags.CXTranslationUnit_None;

            translationFlags |= CXTranslationUnit_Flags.CXTranslationUnit_IncludeAttributedTypes;               // Include attributed types in CXType
            translationFlags |= CXTranslationUnit_Flags.CXTranslationUnit_VisitImplicitAttributes;              // Implicit attributes should be visited

            var config = new PInvokeGeneratorConfiguration(libraryPath, namespaceName, outputLocation, configOptions, excludedNames, headerFile, methodClassName, methodPrefixToStrip, remappedNames, traversalNames);

            int exitCode = 0;

            using (var pinvokeGenerator = new PInvokeGenerator(config))
            {
                foreach (var file in files)
                {
                    var translationUnitError = CXTranslationUnit.TryParse(pinvokeGenerator.IndexHandle, file, clangCommandLineArgs, Array.Empty<CXUnsavedFile>(), translationFlags, out CXTranslationUnit handle);
                    var skipProcessing = false;

                    if (translationUnitError != CXErrorCode.CXError_Success)
                    {
                        Console.WriteLine($"Error: Parsing failed for '{file}' due to '{translationUnitError}'.");
                        skipProcessing = true;
                    }
                    else if (handle.NumDiagnostics != 0)
                    {
                        Console.WriteLine($"Diagnostics for '{file}':");

                        for (uint i = 0; i < handle.NumDiagnostics; ++i)
                        {
                            using var diagnostic = handle.GetDiagnostic(i);

                            Console.Write("    ");
                            Console.WriteLine(diagnostic.Format(CXDiagnostic.DefaultDisplayOptions).ToString());

                            skipProcessing |= (diagnostic.Severity == CXDiagnosticSeverity.CXDiagnostic_Error);
                            skipProcessing |= (diagnostic.Severity == CXDiagnosticSeverity.CXDiagnostic_Fatal);
                        }
                    }

                    if (skipProcessing)
                    {
                        Console.WriteLine($"Skipping '{file}' due to one or more errors listed above.");
                        Console.WriteLine();

                        exitCode = -1;
                        continue;
                    }

                    using var translationUnit = TranslationUnit.GetOrCreate(handle);

                    Console.WriteLine($"Processing '{file}'");
                    pinvokeGenerator.GenerateBindings(translationUnit);
                }

                if (pinvokeGenerator.Diagnostics.Count != 0)
                {
                    Console.WriteLine("Diagnostics for binding generation:");

                    foreach (var diagnostic in pinvokeGenerator.Diagnostics)
                    {
                        Console.Write("    ");
                        Console.WriteLine(diagnostic);

                        if (diagnostic.Level == DiagnosticLevel.Warning)
                        {
                            if (exitCode >= 0)
                            {
                                exitCode++;
                            }
                        }
                        else if (diagnostic.Level == DiagnosticLevel.Error)
                        {
                            if (exitCode >= 0)
                            {
                                exitCode = -1;
                            }
                            else
                            {
                                exitCode--;
                            }
                        }
                    }
                }
            }

            return exitCode;
        }

        private static void AddAdditionalOption(RootCommand rootCommand)
        {
            var option = new Option(new string[] { "--additional", "-a" }, "An argument to pass to Clang when parsing the input files.")
            {
                Argument = new Argument("<arg>")
                {
                    ArgumentType = typeof(string),
                    Arity = ArgumentArity.OneOrMore,
                }
            };
            option.Argument.SetDefaultValue(Array.Empty<string>());

            rootCommand.AddOption(option);
        }

        private static void AddConfigOption(RootCommand rootCommand)
        {
            var option = new Option(new string[] { "--config", "-c" }, "A configuration option that controls how the bindings are generated.")
            {
                Argument = new Argument("<arg>")
                {
                    ArgumentType = typeof(string),
                    Arity = ArgumentArity.OneOrMore,
                }
            };
            option.Argument.SetDefaultValue(Array.Empty<string>());

            rootCommand.AddOption(option);
        }

        private static void AddDefineMacroOption(RootCommand rootCommand)
        {
            var option = new Option(new string[] { "--define-macro", "-D" }, "Define <macro> to <value> (or 1 if <value> omitted).")
            {
                Argument = new Argument("<macro>=<value>")
                {
                    ArgumentType = typeof(string),
                    Arity = ArgumentArity.OneOrMore,
                }
            };
            option.Argument.SetDefaultValue(Array.Empty<string>());

            rootCommand.AddOption(option);
        }

        private static void AddExcludeOption(RootCommand rootCommand)
        {
            var option = new Option(new string[] { "--exclude", "-e" }, "A declaration name to exclude from binding generation.")
            {
                Argument = new Argument("<name>")
                {
                    ArgumentType = typeof(string),
                    Arity = ArgumentArity.OneOrMore,
                }
            };
            option.Argument.SetDefaultValue(Array.Empty<string>());

            rootCommand.AddOption(option);
        }

        private static void AddFileOption(RootCommand rootCommand)
        {
            var option = new Option(new string[] { "--file", "-f" }, "A file to parse and generate bindings for.")
            {
                Argument = new Argument("<file>")
                {
                    ArgumentType = typeof(string),
                    Arity = ArgumentArity.OneOrMore,
                }
            };
            option.Argument.SetDefaultValue(Array.Empty<string>());

            rootCommand.AddOption(option);
        }

        private static void AddHeaderOption(RootCommand rootCommand)
        {
            var option = new Option(new string[] { "--headerFile", "-h" }, "A file which contains the header to prefix every generated file with.")
            {
                Argument = new Argument("<file>")
                {
                    ArgumentType = typeof(string),
                    Arity = ArgumentArity.ExactlyOne,
                }
            };
            option.Argument.SetDefaultValue(string.Empty);

            rootCommand.AddOption(option);
        }

        private static void AddIncludeDirectoryOption(RootCommand rootCommand)
        {
            var option = new Option(new string[] { "--include-directory", "-I" }, "Add directory to include search path.")
            {
                Argument = new Argument("<arg>")
                {
                    ArgumentType = typeof(string),
                    Arity = ArgumentArity.OneOrMore,
                }
            };
            option.Argument.SetDefaultValue(Array.Empty<string>());

            rootCommand.AddOption(option);
        }

        private static void AddLanguageOption(RootCommand rootCommand)
        {
            var option = new Option(new string[] { "--language", "-x" }, "Treat subsequent input files as having type <language>.")
            {
                Argument = new Argument("<arg>")
                {
                    ArgumentType = typeof(string),
                    Arity = ArgumentArity.ExactlyOne,
                }
            };
            option.Argument.SetDefaultValue("c++");

            rootCommand.AddOption(option);
        }

        private static void AddLibraryOption(RootCommand rootCommand)
        {
            var option = new Option(new string[] { "--libraryPath", "-l" }, "The string to use in the DllImport attribute used when generating bindings.")
            {
                Argument = new Argument("<dllName>")
                {
                    ArgumentType = typeof(string),
                    Arity = ArgumentArity.ExactlyOne,
                }
            };
            option.Argument.SetDefaultValue(string.Empty);

            rootCommand.AddOption(option);
        }

        private static void AddMethodClassNameOption(RootCommand rootCommand)
        {
            var option = new Option(new string[] { "--methodClassName", "-m" }, "The name of the static class that will contain the generated method bindings.")
            {
                Argument = new Argument("<className>")
                {
                    ArgumentType = typeof(string),
                    Arity = ArgumentArity.ExactlyOne,
                }
            };
            option.Argument.SetDefaultValue("Methods");

            rootCommand.AddOption(option);
        }

        private static void AddNamespaceOption(RootCommand rootCommand)
        {
            var option = new Option(new string[] { "--namespace", "-n" }, "The namespace in which to place the generated bindings.")
            {
                Argument = new Argument("<namespace>")
                {
                    ArgumentType = typeof(string),
                    Arity = ArgumentArity.ExactlyOne,
                }
            };
            option.Argument.SetDefaultValue(string.Empty);

            rootCommand.AddOption(option);
        }

        private static void AddOutputOption(RootCommand rootCommand)
        {
            var option = new Option(new string[] { "--output", "-o" }, "The output location to write the generated bindings to.")
            {
                Argument = new Argument("<file>")
                {
                    ArgumentType = typeof(string),
                    Arity = ArgumentArity.ExactlyOne,
                }
            };
            option.Argument.SetDefaultValue(string.Empty);

            rootCommand.AddOption(option);
        }

        private static void AddPrefixStripOption(RootCommand rootCommand)
        {
            var option = new Option(new string[] { "--prefixStrip", "-p" }, "The prefix to strip from the generated method bindings.")
            {
                Argument = new Argument("<prefix>")
                {
                    ArgumentType = typeof(string),
                    Arity = ArgumentArity.ExactlyOne,
                }
            };
            option.Argument.SetDefaultValue(string.Empty);

            rootCommand.AddOption(option);
        }

        private static void AddRemapOption(RootCommand rootCommand)
        {
            var option = new Option(new string[] { "--remap", "-r" }, "A declaration name to be remapped to another name during binding generation.")
            {
                Argument = new Argument("<name>=<value>")
                {
                    ArgumentType = typeof(string),
                    Arity = ArgumentArity.OneOrMore,
                }
            };
            option.Argument.SetDefaultValue(Array.Empty<string>());

            rootCommand.AddOption(option);
        }

        private static void AddStdOption(RootCommand rootCommand)
        {
            var option = new Option(new string[] { "--std", "-std" }, "Language standard to compile for.")
            {
                Argument = new Argument("<arg>")
                {
                    ArgumentType = typeof(string),
                    Arity = ArgumentArity.ExactlyOne,
                }
            };
            option.Argument.SetDefaultValue("c++17");

            rootCommand.AddOption(option);
        }

        private static void AddTraverseOption(RootCommand rootCommand)
        {
            var option = new Option(new string[] { "--traverse", "-t" }, "A file name included either directly or indirectly by -f that should be traversed during binding generation.")
            {
                Argument = new Argument("<name>")
                {
                    ArgumentType = typeof(string),
                    Arity = ArgumentArity.OneOrMore,
                }
            };
            option.Argument.SetDefaultValue(Array.Empty<string>());

            rootCommand.AddOption(option);
        }
    }
}
