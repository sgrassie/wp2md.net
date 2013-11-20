using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fclp;

namespace wp2md
{
    class Program
    {
        static void Main(string[] args)
        {
            var parser = CreateArgsParser();
            var results = parser.Parse(args);

            if (results.HasErrors)
            {
                Console.WriteLine(results.ErrorText);
                Environment.Exit(-2);
            }

            var options = parser.Object;
        }

        static FluentCommandLineBuilder<Options> CreateArgsParser()
        {
            var parser = new FluentCommandLineBuilder<Options>();
            parser
                .SetupHelp("?", "h", "help")
                .UseForEmptyArgs()
                .Callback(help => Console.WriteLine(help));

            parser
                .Setup(arg => arg.SourceFile).As('s', "source")
                .Required()
                .WithDescription("WordPress eXtended RSS source file.");

            parser
                .Setup(arg => arg.OutputPath)
                .As('o', "outputPath")
                .Required()
                .WithDescription("The output folder for generated files.");

            parser
                .Setup(arg => arg.LogToFile)
                .As('l', "logfile")
                .SetDefault(string.Empty)
                .WithDescription("If given, logs to the specified file, instead of outputting to console");

            parser
                .Setup(arg => arg.Verbosity)
                .As('v', "verbosity")
                .SetDefault(0)
                .WithDescription("Log verbosity level. 0 for no logging, 1 for only vaguely interesting stuff, 2 for... everything");

            return parser;
        }
    }

    class Options
    {
        public string SourceFile { get; set; }

        public string LogToFile { get; set; }

        public int Verbosity { get; set; }

        public string OutputPath { get; set; }
    }
}
