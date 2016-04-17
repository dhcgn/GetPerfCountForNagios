namespace GetPerfCountForNagios
{
    internal class MockPerformanceCounter : IPerformanceCounter
    {
        public float Value { get; set; }

        public MockPerformanceCounter()
        {
        }

        public void Set(string categoryName, string counterName, string instanceName)
        {
            
        }

        public float NextValue()
        {
            return this.Value;
        }
    }
}