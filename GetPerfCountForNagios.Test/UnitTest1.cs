﻿using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace GetPerfCountForNagios.Test
{
    [TestFixture]
    public class UnitTest1
    {
        [SetUp]
        public void SetUp()
        {
            Program.Writer = new StringWriter();
        }

        #region Helper

        public string ConsoleOutput => Program.Writer.ToString();

        private static string[] CreateConsoleParameter(string parameter)
        {
            return parameter.Split(new[] {';'}, StringSplitOptions.RemoveEmptyEntries);
        }

        public static object GetPropValue(object src, string propName)
        {
            return src.GetType().GetProperty(propName).GetValue(src, null);
        }

        #endregion

        [TestCase("")]
        [TestCase("-h")]
        [TestCase("/h")]
        [TestCase("-help")]
        [TestCase("/help")]
        [TestCase("-?")]
        [TestCase("/?")]
        public void Intergration_ParameterHelp_Simple_Success(string parameter)
        {
            Program.Main(CreateConsoleParameter(parameter));

            Assert.AreEqual("Help:\n\nSyntax: GetPerfCountForNagios.exe", new string(this.ConsoleOutput.ToCharArray().Take(40).ToArray()));
        }

        [Test]
        public void Integration_Missing_Attributes_Succes()
        {
            var parameter = new[]
            {
                "-Name",
                @"\Processor Information(_Total)\% Processor Time",
            };

            Program.Perf = new MyPerformanceCounter();

            Program.Main(parameter);

            Console.Out.WriteLine("Out: " + this.ConsoleOutput);

            var expectedOutput = "Error, please use /h for help." + Environment.NewLine+
                                 "Error Message: " + Environment.NewLine +
                                 "Missing Attributes: " + Environment.NewLine +
                                 "-Label " + Environment.NewLine +
                                 "-Unit " + Environment.NewLine +
                                 "-Warning " + Environment.NewLine +
                                 "-Critical " + Environment.NewLine +
                                 "-Min " + Environment.NewLine +
                                 "-Max "+Environment.NewLine + Environment.NewLine;


            Assert.AreEqual(expectedOutput, new string(this.ConsoleOutput.ToCharArray().Take(128).ToArray()));
        }

        [Test]
        public void Intergration_Real_CPU_Success()
        {
            var parameter = new[]
            {
                "-Name",
                @"\Processor Information(_Total)\% Processor Time",
                "-Label",
                "CPU",
                "-Unit",
                "%",
                "-Warning",
                "90",
                "-Critical",
                "95",
                "-Min",
                "0",
                "-Max",
                "100"
            };

            Program.Perf = new MyPerformanceCounter();

            Program.Main(parameter);

            Console.Out.WriteLine("Out: " + this.ConsoleOutput);
            Assert.IsTrue(Regex.IsMatch(this.ConsoleOutput, "^\'CPU\'=\\d{1,2}(.\\d{1,})?\\[%\\];90;95;0;100"));
        }

        [Test]
        public void Intergration_Real_Memory_Success()
        {
            var parameter = new[]
            {
                "-Name",
                @"\Memory\Available MBytes",
                "-Label",
                "Mem",
                "-Unit",
                "MB",
                "-Warning",
                "7000",
                "-Critical",
                "8000",
                "-Min",
                "0",
                "-Max",
                "12000"
            };

            Program.Perf = new MyPerformanceCounter();

            Program.Main(parameter);

            Console.Out.WriteLine("Out: " + this.ConsoleOutput);
            Assert.IsTrue(Regex.IsMatch(this.ConsoleOutput, "^\'Mem\'=\\d{1,}\\[MB\\];7000;8000;0;12000$"));
        }

        [Test]
        public void Intergration_Mock_CPU_Success()
        {
            var parameter = new[]
            {
                "-Name",
                @"\Processor Information(_Total)\% Processor Time",
                "-Label",
                "CPU",
                "-Unit",
                "%",
                "-Warning",
                "90",
                "-Critical",
                "95",
                "-Min",
                "0",
                "-Max",
                "100"
            };
            Program.Perf = new MockPerformanceCounter {Value = 50.13f};

            Program.Main(parameter);

            Assert.AreEqual("'CPU'=50.13[%];90;95;0;100", this.ConsoleOutput);
        }

        [Test]
        public void Intergration_Mock_Memory_Success()
        {
            var parameter = new[]
            {
                "-Name",
                @"\Memory\Available MBytes",
                "-Label",
                "Mem",
                "-Unit",
                "MB",
                "-Warning",
                "7000",
                "-Critical",
                "8000",
                "-Min",
                "0",
                "-Max",
                "12000"
            };

            Program.Perf = new MockPerformanceCounter {Value = 5041};

            Program.Main(parameter);

            Assert.AreEqual("'Mem'=5041[MB];7000;8000;0;12000", this.ConsoleOutput);
        }

        [TestCase("CategoryName", "TestCategoryName")]
        [TestCase("InstanceName", "TestInstanceName")]
        [TestCase("CounterName", "TestCounterName")]
        [TestCase("Label", "TestLabel")]
        [TestCase("Unit", "TestUnit")]
        [TestCase("Warning", "100")]
        [TestCase("Critical", "200")]
        [TestCase("Min", "0")]
        [TestCase("Max", "1000")]
        public void ParseConfig_Success(string propName, string propValue)
        {
            var config = Config.ParseConfig(new[]
            {
                "-Name",
                "\\TestCategoryName(TestInstanceName)\\TestCounterName",
                "-Label",
                "TestLabel",
                "-Unit",
                "TestUnit",
                "-Warning",
                "100",
                "-Critical",
                "200",
                "-Min",
                "0",
                "-Max",
                "1000",
            });

            Assert.IsNotNull(config.Config, "Error must be not null");
            Assert.IsNull(config.Error, "Error must be null");

            Assert.AreEqual(propValue, GetPropValue(config.Config, propName));
        }

        [TestCase("CategoryName", "TestCategoryName")]
        [TestCase("InstanceName", "TestInstanceName")]
        [TestCase("CounterName", "TestCounterName")]
        [TestCase("Label", "TestLabel")]
        [TestCase("Unit", "TestUnit")]
        [TestCase("Warning", "100")]
        [TestCase("Critical", "200")]
        [TestCase("Min", "0")]
        [TestCase("Max", "1000")]
        public void ParseConfig_Unorderd_Success(string propName, string propValue)
        {
            var config = Config.ParseConfig(new[]
            {
                "-Unit",
                "TestUnit",
                "-Name",
                "\\TestCategoryName(TestInstanceName)\\TestCounterName",
                "-Critical",
                "200",
                "-Warning",
                "100",
                "-Min",
                "0",
                "-Label",
                "TestLabel",
                "-Max",
                "1000",
            });

            Assert.IsNotNull(config.Config, "Error must be not null");
            Assert.IsNull(config.Error, "Error must be null");

            Assert.AreEqual(propValue, GetPropValue(config.Config, propName));
        }
    }
}