using Quartz.Core;

namespace Quartz.Tests.Unit.Core;

[TestFixture]
public class QuartzSchedulerResourcesTest
{
    private QuartzSchedulerResources _resources;

    [SetUp]
    public void SetUp()
    {
        _resources = new QuartzSchedulerResources();
    }

    [Test]
    public void DefaultCtor()
    {
        var resources = new QuartzSchedulerResources();
        Assert.That(resources.BatchTimeWindow, Is.EqualTo(TimeSpan.Zero));
        Assert.IsNull(resources.InstanceId);
        Assert.IsFalse(resources.InterruptJobsOnShutdown);
        Assert.IsFalse(resources.InterruptJobsOnShutdownWithWait);
        Assert.IsNull(resources.JobRunShellFactory);
        Assert.IsNull(resources.JobStore);
        Assert.IsFalse(resources.MakeSchedulerThreadDaemon);
        Assert.That(resources.MaxBatchSize, Is.EqualTo(1));
        Assert.IsNull(resources.Name);
        Assert.IsNotNull(resources.SchedulerPlugins);
        Assert.IsEmpty(resources.SchedulerPlugins);
        Assert.IsNull(resources.ThreadName);
        Assert.IsNull(resources.ThreadPool);
    }

    [Test]
    public void IdleWaitTime_ValidValues([ValueSource(nameof(ValidIdleWaitTimes))] TimeSpan idleWaitTime)
    {
        _resources.IdleWaitTime = idleWaitTime;
        Assert.That(_resources.IdleWaitTime, Is.EqualTo(idleWaitTime));
    }

    [Test]
    public void IdleWaitTime_InvalidValues([ValueSource(nameof(InvalidIdleWaitTimes))] TimeSpan idleWaitTime)
    {
        try
        {
            _resources.IdleWaitTime = idleWaitTime;
            Assert.Fail();
        }
        catch (ArgumentOutOfRangeException ex)
        {
            Assert.That(ex.ParamName, Is.EqualTo("value"));
        }
    }

    [Test]
    public void BatchTimeWindow_ValidValues([ValueSource(nameof(ValidBatchTimeWindows))] TimeSpan batchTimeWindow)
    {
        _resources.BatchTimeWindow = batchTimeWindow;
        Assert.That(_resources.BatchTimeWindow, Is.EqualTo(batchTimeWindow));
    }

    [Test]
    public void BatchTimeWindow_InvalidValues([ValueSource(nameof(InvalidBatchTimeWindows))] TimeSpan batchTimeWindow)
    {
        try
        {
            _resources.BatchTimeWindow = batchTimeWindow;
            Assert.Fail();
        }
        catch (ArgumentOutOfRangeException ex)
        {
            Assert.That(ex.ParamName, Is.EqualTo("value"));
        }
    }

    [Test]
    public void MaxBatchSize_ValidValues([ValueSource(nameof(ValidMaxBatchSizes))] int maxBatchSize)
    {
        _resources.MaxBatchSize = maxBatchSize;
        Assert.That(_resources.MaxBatchSize, Is.EqualTo(maxBatchSize));
    }

    [Test]
    public void MaxBatchSize_InvalidValues([ValueSource(nameof(InvalidMaxBatchSizes))] int maxBatchSize)
    {
        try
        {
            _resources.MaxBatchSize = maxBatchSize;
            Assert.Fail();
        }
        catch (ArgumentOutOfRangeException ex)
        {
            Assert.That(ex.ParamName, Is.EqualTo("value"));
        }
    }

    internal static IEnumerable<TimeSpan> ValidIdleWaitTimes()
    {
        yield return TimeSpan.Zero;
        yield return TimeSpan.FromTicks(1);
        yield return TimeSpan.MaxValue;
    }

    internal static IEnumerable<TimeSpan> InvalidIdleWaitTimes()
    {
        yield return TimeSpan.FromTicks(-1);
        yield return TimeSpan.FromDays(-30);
        yield return TimeSpan.MinValue;
    }

    internal static IEnumerable<int> ValidMaxBatchSizes()
    {
        yield return 1;
        yield return 8;
        yield return int.MaxValue;
    }

    internal static IEnumerable<int> InvalidMaxBatchSizes()
    {
        yield return 0;
        yield return -1;
        yield return int.MinValue;
    }

    internal static IEnumerable<TimeSpan> ValidBatchTimeWindows()
    {
        yield return TimeSpan.Zero;
        yield return TimeSpan.FromTicks(1);
        yield return TimeSpan.FromDays(30);
    }

    internal static IEnumerable<TimeSpan> InvalidBatchTimeWindows()
    {
        yield return TimeSpan.FromTicks(-1);
        yield return TimeSpan.FromDays(-30);
        yield return TimeSpan.MinValue;
    }

}