using System.Diagnostics;

namespace GetPerfCountForNagios
{
    internal class MyPerformanceCounter : IPerformanceCounter
    {
        private readonly PerformanceCounter performanceCounter;

        public MyPerformanceCounter()
        {
            this.performanceCounter = new PerformanceCounter();
        }

        public void Set(string categoryName, string counterName, string instanceName)
        {
            this.performanceCounter.InstanceName = instanceName;
            this.performanceCounter.CategoryName = categoryName;
            this.performanceCounter.CounterName = counterName;
        }

        public float NextValue()
        {
            return this.performanceCounter.NextValue();
        }
    }
}