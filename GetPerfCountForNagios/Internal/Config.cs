using System;
using System.Linq;
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

            return new ParseResult()
            {
                Config = config,
                // Todo Error handling
                Error = null,
            };
        }

        private static string GetFollowedElement(string[] args, string name)
        {
            return args.Skip(Array.IndexOf(args, name) + 1).FirstOrDefault();
        }
    }
}