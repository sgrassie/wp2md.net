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
            Action<string> logger = msg => Console.WriteLine(msg);
            var parser = CreateArgsParser();
            var results = parser.Parse(args);

            if (results.HasErrors)
            {
                Console.WriteLine(results.ErrorText);
                Environment.Exit(-2);
            }

            new Application(logger).Execute(parser.Object);
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
                .Setup(arg => arg.Verbose)
                .As('v', "verbose")
                .SetDefault(false)
                .WithDescription("Whether to output log messages or not. (All or nothing).");

            return parser;
        }
    }

    class Options
    {
        public string SourceFile { get; set; }

        public bool Verbose { get; set; }

        public string OutputPath { get; set; }
    }
}
