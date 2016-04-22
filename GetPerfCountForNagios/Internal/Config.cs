using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace GetPerfCountForNagios
{
    internal class Config
    {
        public class Defaults
        {
            internal const string DefaultValueLabel = "Performance Counter";
            internal const string DefaultValueUnit = "N";
            internal const string DefaultValueWarning = "85";
            internal const string DefaultValueCritical = "95";
            internal const string DefaultValueMin = "0";
            internal const string DefaultValueMax = "100";
        }

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

            SetMissingAttributesToDefaultValue(args, config);

            if (config.InstanceName == "*")
            {
                config.InstanceName = "_Total";
            }

            var error = !args.Contains("-Name") ? "Missing Counter Name" : null;

            return new ParseResult()
            {
                Config = config,
                Error = error
            };
        }

        private static void SetMissingAttributesToDefaultValue(string[] args, Config config)
        {
            if (!args.Contains($"-{nameof(Config.Label)}"))
                config.Label = config.CounterName;

            if (!args.Contains($"-{nameof(Config.Unit)}"))
                config.Unit = config.CounterName.Contains("%") ? "%" : Defaults.DefaultValueUnit;

            if (!args.Contains($"-{nameof(Config.Warning)}"))
                config.Warning = Defaults.DefaultValueWarning;

            if (!args.Contains($"-{nameof(Config.Critical)}"))
                config.Critical = Defaults.DefaultValueCritical;

            if (!args.Contains($"-{nameof(Config.Min)}"))
                config.Min = Defaults.DefaultValueMin;

            if (!args.Contains($"-{nameof(Config.Max)}"))
                config.Max = Defaults.DefaultValueMax;
        }

        private static string GetFollowedElement(string[] args, string name)
        {
            return args.Skip(Array.IndexOf(args, name) + 1).FirstOrDefault();
        }
    }
}