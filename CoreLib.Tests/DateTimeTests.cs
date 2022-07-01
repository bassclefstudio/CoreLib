using BassClefStudio.Core.Primitives;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace CoreLib.Tests;

[TestClass]
public class DateTimeTests
{
    [TestMethod]
    public void TestNormalize1()
    {
        var today = DateTime.Today;
        DateTimeSpan[] spans = new[]
        {
            new DateTimeSpan(today, today),
            new DateTimeSpan(today.AddDays(1), today.AddDays(1)),
            new DateTimeSpan(today.AddDays(1), today.AddDays(2))
        };
        var normalized = spans.Normalize().ToArray();
        var expected = new[] { spans[2] };
        Console.WriteLine(string.Join(",", expected));
        Console.WriteLine(string.Join(",", normalized));
        Assert.IsTrue(expected.SequenceEqual(normalized));
    }

    [TestMethod]
    public void TestNormalize2()
    {
        var today = DateTime.Today;
        DateTimeSpan[] spans = new[]
        {
            new DateTimeSpan(today, today),
            new DateTimeSpan(today.AddDays(2), today.AddDays(3)),
            new DateTimeSpan(today.AddDays(1), today.AddDays(2))
        };
        var normalized = spans.Normalize().ToArray();
        var expected = new[] { new DateTimeSpan(today.AddDays(1), today.AddDays(3)) };
        Console.WriteLine(string.Join(",", expected));
        Console.WriteLine(string.Join(",", normalized));
        Assert.IsTrue(expected.SequenceEqual(normalized));
    }

    [TestMethod]
    public void TestNormalize3()
    {
        var today = DateTime.Today;
        DateTimeSpan[] spans = new[]
        {
            new DateTimeSpan(today, today.AddDays(1)),
            new DateTimeSpan(today.AddDays(1), today.AddDays(2)),
            new DateTimeSpan(today, today.AddDays(3))
        };
        var normalized = spans.Normalize().ToArray();
        var expected = new[] { spans[2] };
        Console.WriteLine(string.Join(",", expected));
        Console.WriteLine(string.Join(",", normalized));
        Assert.IsTrue(expected.SequenceEqual(normalized));
    }

    [TestMethod]
    public void TestSetUnion1()
    {
        var today = DateTime.Today;
        DateTimeSet mySet = new DateTimeSet(new DateTimeSpan(today, today.AddDays(1)));
        Assert.AreEqual(1, mySet.Spans.Length);
        DateTimeSet secondSet = new DateTimeSet(new DateTimeSpan(today.AddDays(1), today.AddDays(2)));
        Assert.AreEqual(1, secondSet.Spans.Length);
        var newSet = mySet.Union(secondSet);
        Assert.AreEqual(1, newSet.Spans.Length);
        Assert.AreEqual(today, newSet.Spans[0].StartDate);
        Assert.AreEqual(today.AddDays(2), newSet.Spans[0].EndDate);
    }

    [TestMethod]
    public void TestSetUnion2()
    {
        DateTimeSet mySet = new DateTimeSet(new DateTimeSpan(DateTime.Today, DateTime.Today.AddDays(1)));
        Assert.AreEqual(1, mySet.Spans.Length);
        DateTimeSet secondSet = new DateTimeSet(new DateTimeSpan(DateTime.Today.AddDays(2), DateTime.Today.AddDays(3)));
        Assert.AreEqual(1, secondSet.Spans.Length);
        var newSet = mySet.Union(secondSet);
        Assert.AreEqual(2, newSet.Spans.Length);
        Assert.AreEqual(DateTime.Today, newSet.Spans[0].StartDate);
        Assert.AreEqual(DateTime.Today.AddDays(1), newSet.Spans[0].EndDate);
        Assert.AreEqual(DateTime.Today.AddDays(2), newSet.Spans[1].StartDate);
        Assert.AreEqual(DateTime.Today.AddDays(3), newSet.Spans[1].EndDate);
    }

    [TestMethod]
    public void TestSetUnion3()
    {
        DateTimeSet mySet = DateTimeSet.Empty;
        Assert.AreEqual(0, mySet.Spans.Length);
        DateTimeSet secondSet = new DateTimeSet(new DateTimeSpan(DateTime.Today.AddDays(2), DateTime.Today.AddDays(3)));
        Assert.AreEqual(1, secondSet.Spans.Length);
        var newSet = mySet.Union(secondSet);
        Assert.AreEqual(1, newSet.Spans.Length);
        Assert.IsTrue(newSet.Spans.SequenceEqual(secondSet.Spans));
    }
}