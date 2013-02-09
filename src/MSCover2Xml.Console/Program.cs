using System;
using System.IO;
using System.Xml;
using CommandLine;

namespace MSCover2Xml.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var options = new Options();
            if (!CommandLineParser.Default.ParseArguments(args, options))
                return;
            
            var symbolPaths = (options.SymbolPaths ?? Path.GetDirectoryName(options.InputFile)).Split(new[] {';'}, StringSplitOptions.RemoveEmptyEntries);
            var executablePaths = (options.ExecutablePaths ?? Path.GetDirectoryName(options.InputFile)).Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

            System.Console.WriteLine("Generating coverage report from {0} ...", options.InputFile);
            PrintPaths("Symbols", symbolPaths);
            PrintPaths("Executables", executablePaths);

            try
            {
                var coverageReport = CoverageReportGenerator.Create(options.InputFile, executablePaths, symbolPaths);

                System.Console.WriteLine("Generating XML at {0} ...", options.OutputFile);

                coverageReport.SaveToFile(options.OutputFile);

                System.Console.WriteLine("Done");
            }
            catch (Exception ex)
            {
                System.Console.Error.WriteLine("ERROR: " + ex.Message);
            }
        }

        private static void PrintPaths(string pathName, string[] paths)
        {
            System.Console.Write(pathName + ": ");

            if (paths.Length == 0)
                System.Console.WriteLine("/");
            foreach (var path in paths)
                System.Console.WriteLine("\t" + path);
        }
    }
}
