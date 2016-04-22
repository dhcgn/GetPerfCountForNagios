using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using GetPerfCountForNagios.Internal;

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

            SetMissingAttributesToDefaultValue(args, config);

            var error = !args.Contains("-Name") ? "Missing Counter Name" : null;

            return new ParseResult()
            {
                Config = config,
                Error = error
                //Error = missingAttributes
            };
        }

        private static void SetMissingAttributesToDefaultValue(string[] args, Config config)
        {
            if (!args.Contains($"-{nameof(Config.Label)}"))
            {
                config.Label = config.CounterName;
            }

            if (!args.Contains($"-{nameof(Config.Unit)}"))
            {
                config.Unit = config.CounterName.Contains("%") ? "%" : DefaultValues.DefaultValueUnit;

            }

            if (!args.Contains($"-{nameof(Config.Warning)}"))
            {
                config.Warning = DefaultValues.DefaultValueWarning;
            }

            if (!args.Contains($"-{nameof(Config.Critical)}"))
            {
                config.Critical = DefaultValues.DefaultValueCritical;
            }

            if (!args.Contains($"-{nameof(Config.Min)}"))
            {
                config.Min = DefaultValues.DefaultValueMin;
            }

            if (!args.Contains($"-{nameof(Config.Max)}"))
            {
                config.Max = DefaultValues.DefaultValueMax;
            }
        }

        private static string GetFollowedElement(string[] args, string name)
        {
            return args.Skip(Array.IndexOf(args, name) + 1).FirstOrDefault();
        }
    }
}