using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace GetPerfCountForNagios.Test
{
    public class IntegrationTest
    {
        [SetUp]
        public void SetUp()
        {
            Program.Writer = new StringWriter();
        }

        #region Helper

        public string ConsoleOutput => Program.Writer.ToString();

        #endregion

        private static string[] CreateConsoleParameter(string parameter)
        {
            return parameter.Split(new[] {';'}, StringSplitOptions.RemoveEmptyEntries);
        }

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
        public void Intergration_PerformanceCounter_Failure()
        {
            var parameter = new[]
            {
                "-Name",
                @"\Whatever(_Total)\TestCounter",
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

            Assert.Catch(typeof(System.InvalidOperationException), () => Program.Main(parameter));
        }

        [Test]
        public void Intergration_Real_CPU_InstanceAutomaticallySetToTotal_Success()
        {
            var parameter = new[]
            {
                "-Name",
                @"\Processor Information(*)\% Processor Time",
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

            Assert.IsTrue(Regex.IsMatch(this.ConsoleOutput, "^\'CPU\'=\\d{1,2}(.\\d{1,})?\\[%\\];90;95;0;100"));
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

            Assert.IsTrue(Regex.IsMatch(this.ConsoleOutput, "^\'% Processor Time\'=\\d{1,2}(.\\d{1,})?\\[%\\];85;95;0;100"));
        }

        [Test]
        public void Intergration_Name_Missing_Success()
        {
            var parameter = new[]
            {
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

            var expectedOutput = "Error, please use /h for help." + Environment.NewLine +
                                 "Missing Counter Name" + Environment.NewLine;

            Assert.AreEqual(expectedOutput, this.ConsoleOutput);
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

            Assert.IsTrue(Regex.IsMatch(this.ConsoleOutput, "^\'CPU\'=\\d{1,2}(.\\d{1,})?\\[%\\];90;95;0;100"));
        }

        [Test]
        [Category("IgnoreTravis")]
        public void Intergration_Real_CPU_InitValue_Success()
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

            var isMatch = Regex.Match(this.ConsoleOutput, "^\'CPU\'=(?<value>\\d{1,2}(.\\d{1,})?)\\[%\\];90;95;0;100");
            var value = isMatch.Groups["value"].Value;

            Assert.IsNotEmpty(value, "Get value from nagios output");

            float valueFloat;
            Assert.IsTrue(float.TryParse(value, out valueFloat), "Parse to float");

            Assert.AreNotEqual(0.0f, valueFloat);
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
    }
}