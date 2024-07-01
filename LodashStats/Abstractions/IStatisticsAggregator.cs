namespace LodashStats.Abstractions;

public interface IStatisticsAggregator
{
    void ExtractStatistics(string content);
    IOrderedEnumerable<KeyValuePair<char, int>> GetOrderedStatistics();
}