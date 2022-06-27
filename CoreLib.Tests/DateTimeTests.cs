using BassClefStudio.Core.Primitives;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CoreLib.Tests;

[TestClass]
public class DateTimeTests
{
    [TestMethod]
    public void TestSetUnion1()
    {
        DateTimeSet mySet = new DateTimeSet(new DateTimeSpan(DateTime.Today, DateTime.Today.AddDays(1)));
        Assert.AreEqual(1, mySet.Spans.Length);
        DateTimeSet secondSet = new DateTimeSet(new DateTimeSpan(DateTime.Today.AddDays(1), DateTime.Today.AddDays(2)));
        Assert.AreEqual(1, secondSet.Spans.Length);
        var newSet = mySet.Union(secondSet);
        Assert.AreEqual(1, newSet.Spans.Length);
        Assert.AreEqual(DateTime.Today, newSet.Spans[0].StartDate);
        Assert.AreEqual(DateTime.Today.AddDays(2), newSet.Spans[0].EndDate);
    }
}