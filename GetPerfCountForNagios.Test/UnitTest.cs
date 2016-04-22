using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace GetPerfCountForNagios.Test
{
    [TestFixture]
    public class UnitTest
    {
        [SetUp]
        public void SetUp()
        {
            Program.Writer = new StringWriter();
        }

        #region Helper

        public static object GetPropValue(object src, string propName)
        {
            return src.GetType().GetProperty(propName).GetValue(src, null);
        }

        #endregion

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