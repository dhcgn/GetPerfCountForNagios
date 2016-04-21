using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;

namespace GetPerfCountForNagios
{
    internal class Config
    {
        public string CategoryName { get; private set; }
        public string CounterName { get; private set; }
        public string InstanceName { get; private set; }
        public string Label { get; private set; }
        public string Unit { get; private set; }
        public string Warning { get; private set; }
        public string Critical { get; private set; }
        public string Min { get; private set; }
        public string Max { get; private set; }

        public class ParseResult
        {
            public Config Config { get; internal set; }
            public string Error { get; internal set; }
        }

        internal static ParseResult ParseConfig(string[] args)
        {
            var name = GetFollowedElement(args, "-Name");
            var match = Regex.Match(name ?? String.Empty, @"\\(?<Category>[^\(]*)(\((?<Instance>.*.)\))?\\(?<Name>.*.)");

            Config config = new Config()
            {
                CategoryName = match.Groups["Category"].Value,
                CounterName = match.Groups["Name"].Value,
                InstanceName = match.Groups["Instance"].Value,
                Label = GetFollowedElement(args, $"-{nameof(config.Label)}"),
                Unit = GetFollowedElement(args, $"-{nameof(config.Unit)}"),
                Warning = GetFollowedElement(args, $"-{nameof(config.Warning)}"),
                Critical = GetFollowedElement(args, $"-{nameof(config.Critical)}"),
                Min = GetFollowedElement(args, $"-{nameof(config.Min)}"),
                Max = GetFollowedElement(args, $"-{nameof(config.Max)}"),
            };
            var missingAttributes = CheckForMissingAttributes(args);

            if (!String.IsNullOrEmpty(missingAttributes))
            {
                missingAttributes = "Error Message: " + Environment.NewLine + "Missing Attributes: " + Environment.NewLine + missingAttributes;
            }
            else
            {
                missingAttributes = null;
            }


            return new ParseResult()
            {
                Config = config,
                Error = missingAttributes,
            };
        }

        private static string CheckForMissingAttributes(string[] args)
        {
            var sb = new StringBuilder();

            FindMissingAttributes(args, nameof(Config.Label), sb);
            FindMissingAttributes(args, nameof(Config.Unit), sb);
            FindMissingAttributes(args, nameof(Config.Warning), sb);
            FindMissingAttributes(args, nameof(Config.Critical), sb);
            FindMissingAttributes(args, nameof(Config.Min), sb);
            FindMissingAttributes(args, nameof(Config.Max), sb);

            return sb.ToString();
        }

        private static void FindMissingAttributes(string[] args, string attribute, StringBuilder sb)
        {
            if (!args.Contains($"-{attribute}"))
            {
                sb.AppendLine($"-{attribute} ");
                return;
            }
        }


        private static string GetFollowedElement(string[] args, string name)
        {
            return args.Skip(Array.IndexOf(args, name) + 1).FirstOrDefault();
        }
    }
}