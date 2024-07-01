using LodashStats;

namespace LodashStatsTests;

public class StatisticsAggregatorTests
{
    private StatisticsAggregator _statisticsAggregator;

    [SetUp]
    public void Setup()
    {
        _statisticsAggregator = new StatisticsAggregator();
    }

    [Test]
    public void WhenNoStatisticsAreExtractedCountIsZero()
    {
        Assert.That(_statisticsAggregator.GetOrderedStatistics().Count(), Is.EqualTo(0));
    }

    [Test]
    public void WhenUniqueStatisticsAreExtractedCountIsMatching()
    {
        _statisticsAggregator.ExtractStatistics("abc");

        Assert.That(_statisticsAggregator.GetOrderedStatistics().Count(), Is.EqualTo(3));
    }

    [Test]
    public void WhenDuplicatedStatisticsAreExtractedCountIsMatching()
    {
        _statisticsAggregator.ExtractStatistics("abc");
        _statisticsAggregator.ExtractStatistics("abc");

        Assert.That(_statisticsAggregator.GetOrderedStatistics().Count(), Is.EqualTo(3));
    }

    [Test]
    public void WhenUniqueStatisticsAreExtractedFrequencyIsMatching()
    {
        _statisticsAggregator.ExtractStatistics("abc");
        Dictionary<char, int> frequencyDict = _statisticsAggregator.GetOrderedStatistics().ToDictionary();

        Assert.Multiple(() =>
        {
            Assert.That(frequencyDict['a'], Is.EqualTo(1));
            Assert.That(frequencyDict['b'], Is.EqualTo(1));
            Assert.That(frequencyDict['c'], Is.EqualTo(1));
        });
    }

    [Test]
    public void WhenSpecialCharactersAreExtractedFrequencyAndCountAreMatching()
    {
        _statisticsAggregator.ExtractStatistics("abc\r\n\r\t ");
        Dictionary<char, int> frequencyDict = _statisticsAggregator.GetOrderedStatistics().ToDictionary();

        Assert.That(_statisticsAggregator.GetOrderedStatistics().Count(), Is.EqualTo(7));

        Assert.Multiple(() =>
        {
            Assert.That(frequencyDict['a'], Is.EqualTo(1));
            Assert.That(frequencyDict['b'], Is.EqualTo(1));
            Assert.That(frequencyDict['c'], Is.EqualTo(1));
            Assert.That(frequencyDict['\r'], Is.EqualTo(2));
            Assert.That(frequencyDict['\n'], Is.EqualTo(1));
            Assert.That(frequencyDict['\t'], Is.EqualTo(1));
            Assert.That(frequencyDict[' '], Is.EqualTo(1));
        });
    }

    [Test]
    public void WhenDuplicatedStatisticsAreExtractedFrequencyIsMatching()
    {
        _statisticsAggregator.ExtractStatistics("abc");
        _statisticsAggregator.ExtractStatistics("abc");
        Dictionary<char, int> frequencyDict = _statisticsAggregator.GetOrderedStatistics().ToDictionary();

        Assert.Multiple(() =>
        {
            Assert.That(frequencyDict['a'], Is.EqualTo(2));
            Assert.That(frequencyDict['b'], Is.EqualTo(2));
            Assert.That(frequencyDict['c'], Is.EqualTo(2));
        });
    }

    [Test]
    public void WhenVariedStatisticsAreExtractedFrequencyAndCountAreMatching()
    {
        _statisticsAggregator.ExtractStatistics("abc");
        _statisticsAggregator.ExtractStatistics("abc1234");
        _statisticsAggregator.ExtractStatistics("xyz");
        Dictionary<char, int> frequencyDict = _statisticsAggregator.GetOrderedStatistics().ToDictionary();

        Assert.That(_statisticsAggregator.GetOrderedStatistics().Count(), Is.EqualTo(10));

        Assert.Multiple(() =>
        {
            Assert.That(frequencyDict['a'], Is.EqualTo(2));
            Assert.That(frequencyDict['b'], Is.EqualTo(2));
            Assert.That(frequencyDict['c'], Is.EqualTo(2));
            Assert.That(frequencyDict['1'], Is.EqualTo(1));
            Assert.That(frequencyDict['2'], Is.EqualTo(1));
            Assert.That(frequencyDict['3'], Is.EqualTo(1));
            Assert.That(frequencyDict['4'], Is.EqualTo(1));
            Assert.That(frequencyDict['x'], Is.EqualTo(1));
            Assert.That(frequencyDict['y'], Is.EqualTo(1));
            Assert.That(frequencyDict['z'], Is.EqualTo(1));
        });
    }
}