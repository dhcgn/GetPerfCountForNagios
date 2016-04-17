namespace GetPerfCountForNagios
{
    internal interface IPerformanceCounter
    {
        void Set(string categoryName, string counterName, string instanceName);
        float NextValue();
    }
}