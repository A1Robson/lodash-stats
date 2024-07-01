using LodashStats.Abstractions;

namespace LodashStats;

public class StatisticsAggregator : IStatisticsAggregator
{
    private readonly Dictionary<char, int> _characterDictionary = [];

    public void ExtractStatistics(string content)
    {
        foreach (char charInFile in content)
        {
            if (_characterDictionary.TryGetValue(charInFile, out int charFrequency))
            {
                charFrequency++;
            }
            else
            {
                charFrequency = 1;
            }

            _characterDictionary[charInFile] = charFrequency;
        }
    }

    public IOrderedEnumerable<KeyValuePair<char, int>> GetOrderedStatistics()
    {
        return _characterDictionary.OrderByDescending(kv => kv.Value);
    }
}