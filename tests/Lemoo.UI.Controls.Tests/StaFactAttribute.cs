using System;
using System.Threading;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Lemoo.UI.Controls.Tests;

/// <summary>
/// Helper class for running WPF control creation on STA threads.
/// </summary>
public static class StaThreadHelper
{
    /// <summary>
    /// Executes an action on an STA thread.
    /// </summary>
    public static void RunOnStaThread(Action action)
    {
        Exception? exception = null;
        var thread = new Thread(() =>
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                exception = ex;
            }
        });
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        thread.Join();

        if (exception != null)
        {
            throw exception;
        }
    }

    /// <summary>
    /// Executes a function on an STA thread and returns the result.
    /// </summary>
    public static T RunOnStaThread<T>(Func<T> func)
    {
        T? result = default;
        Exception? exception = null;
        var thread = new Thread(() =>
        {
            try
            {
                result = func();
            }
            catch (Exception ex)
            {
                exception = ex;
            }
        });
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        thread.Join();

        if (exception != null)
        {
            throw exception;
        }

        return result!;
    }
}

/// <summary>
/// Marks a test to run on an STA thread, which is required for WPF controls.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
[XunitTestCaseDiscoverer("Lemoo.UI.Controls.Tests.StaFactDiscoverer", "Lemoo.UI.Controls.Tests")]
public sealed class StaFactAttribute : Xunit.FactAttribute
{
}

/// <summary>
/// Marks a theory test to run on an STA thread, which is required for WPF controls.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
[XunitTestCaseDiscoverer("Lemoo.UI.Controls.Tests.StaTheoryDiscoverer", "Lemoo.UI.Controls.Tests")]
public sealed class StaTheoryAttribute : Xunit.TheoryAttribute
{
}

public class StaFactDiscoverer : IXunitTestCaseDiscoverer
{
    private readonly IMessageSink _diagnosticMessageSink;

    public StaFactDiscoverer(IMessageSink diagnosticMessageSink)
    {
        _diagnosticMessageSink = diagnosticMessageSink;
    }

    public IEnumerable<IXunitTestCase> Discover(
        ITestFrameworkDiscoveryOptions discoveryOptions,
        ITestMethod testMethod,
        IAttributeInfo factAttribute)
    {
        yield return new StaTestCase(_diagnosticMessageSink, discoveryOptions, testMethod, factAttribute);
    }
}

public class StaTheoryDiscoverer : IXunitTestCaseDiscoverer
{
    private readonly IMessageSink _diagnosticMessageSink;
    private readonly TheoryDiscoverer _theoryDiscoverer;

    public StaTheoryDiscoverer(IMessageSink diagnosticMessageSink)
    {
        _diagnosticMessageSink = diagnosticMessageSink;
        _theoryDiscoverer = new TheoryDiscoverer(diagnosticMessageSink);
    }

    public IEnumerable<IXunitTestCase> Discover(
        ITestFrameworkDiscoveryOptions discoveryOptions,
        ITestMethod testMethod,
        IAttributeInfo theoryAttribute)
    {
        foreach (var testCase in _theoryDiscoverer.Discover(discoveryOptions, testMethod, theoryAttribute))
        {
            yield return new StaTestCase(
                _diagnosticMessageSink,
                discoveryOptions,
                testCase.TestMethod,
                theoryAttribute,
                testCase.TestMethodArguments);
        }
    }
}

public class StaTestCase : XunitTestCase
{
    public StaTestCase()
    {
    }

    public StaTestCase(
        IMessageSink diagnosticMessageSink,
        ITestFrameworkDiscoveryOptions discoveryOptions,
        ITestMethod testMethod,
        IAttributeInfo factAttribute)
        : base(diagnosticMessageSink, discoveryOptions.MethodDisplayOrDefault(), testMethod, testMethodArguments: new object[] { })
    {
    }

    public StaTestCase(
        IMessageSink diagnosticMessageSink,
        ITestFrameworkDiscoveryOptions discoveryOptions,
        ITestMethod testMethod,
        IAttributeInfo factAttribute,
        object[] testMethodArguments)
        : base(diagnosticMessageSink, discoveryOptions.MethodDisplayOrDefault(), testMethod, testMethodArguments)
    {
    }

    public override Task<RunSummary> RunAsync(
        IMessageSink diagnosticMessageSink,
        IMessageBus messageBus,
        object[] constructorArguments,
        ExceptionAggregator aggregator,
        CancellationTokenSource cancellationTokenSource)
    {
        var tcs = new TaskCompletionSource<RunSummary>();
        RunSummary? result = null;
        Exception? exception = null;

        var thread = new Thread(() =>
        {
            try
            {
                result = base.RunAsync(
                    diagnosticMessageSink,
                    messageBus,
                    constructorArguments,
                    aggregator,
                    cancellationTokenSource).GetAwaiter().GetResult();
                tcs.SetResult(result);
            }
            catch (Exception ex)
            {
                exception = ex;
                tcs.SetException(ex);
            }
        });
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        thread.Join();

        return tcs.Task;
    }

    public override void Serialize(IXunitSerializationInfo info)
    {
        base.Serialize(info);
    }

    public new static IXunitTestCase Deserialize(IXunitSerializationInfo info)
    {
        throw new NotImplementedException();
    }
}
