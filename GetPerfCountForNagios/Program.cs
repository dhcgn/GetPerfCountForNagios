using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace GetPerfCountForNagios
{
    internal class Program
    {
        internal static TextWriter Writer = Console.Out;
        internal static IPerformanceCounter Perf = new MyPerformanceCounter();

        internal static void Main(string[] args)
        {
            if (ContainsHelp(args))
            {
                PrintHelp();
                return;
            }

            var parseResult = Config.ParseConfig(args);

            if (!String.IsNullOrEmpty(parseResult.Error))
            {
                Writer.WriteLine("Error, please use /h for help.");
                Writer.WriteLine(parseResult.Error);
                return;
            }

            var value = GetPerformanceCounterValue(parseResult.Config.CategoryName, 
                                                    parseResult.Config.CounterName, 
                                                    parseResult.Config.InstanceName);

            if (value.StartsWith("Error: "))
            {
                Writer.WriteLine("Error, please use /h for help.");
                Writer.WriteLine($"Performance Coutner \"{parseResult.Config.CounterName}\" doesnt exist!");

                return;
            }

            var result = GetNagiosPerformanceDateString(value,
                                                        parseResult.Config.Label, 
                                                        parseResult.Config.Unit,
                                                        parseResult.Config.Warning,
                                                        parseResult.Config.Critical, 
                                                        parseResult.Config.Min, 
                                                        parseResult.Config.Max);

            Writer.Write(result);
        }

        private static string GetNagiosPerformanceDateString(string value, string label, string unit, string warning, string critical, string min, string max)
        {
            return $"'{label}'={value}[{unit}];{warning};{critical};{min};{max}";
        }

        private static string GetPerformanceCounterValue(string categoryName, string counterName, string instanceName)
        {
            Perf.Set(categoryName, counterName, instanceName);
            Perf.NextValue();

            Thread.Sleep(500);

            var result = Perf.NextValue();

            return result.ToString(CultureInfo.InvariantCulture); // Decimal must be "."
        }


        private static bool ContainsHelp(string[] args)
        {
            return args == null || !args.Any() || args.Any(s => new[] {"-h", "/h", "-help", "/help", "-?", "/?"}.Contains(s));
        }

        private static void PrintHelp()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = assembly.GetManifestResourceNames().Single(s => s.EndsWith("Help.txt"));

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                string result = reader.ReadToEnd();
                Writer.Write(result);
            }
        }
    }
}