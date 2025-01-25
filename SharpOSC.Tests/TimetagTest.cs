using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SharpOSC.Tests;

[TestClass, ExcludeFromCodeCoverage]
public class TimetagTest
{
    [TestMethod]
    public void TestTimetag()
    {
        ulong time = (ulong)60 * (ulong)60 * (ulong)24 * (ulong)365 * (ulong)108;
        time = time << 32;
        time = time + (ulong)(Math.Pow(2, 32) / 2);
        var date = new Timetag(time).Timestamp;

        Assert.AreEqual(DateTime.Parse("2007-12-06 00:00:00.500"), date);
    }

    [TestMethod]
    public void TestDateTimeToTimetag()
    {
        var dt = DateTime.Now;

        var l = new Timetag(dt);
        var dtBack = l.Timestamp;

        Assert.AreEqual(dt.Date, dtBack.Date);
        Assert.AreEqual(dt.Hour, dtBack.Hour);
        Assert.AreEqual(dt.Minute, dtBack.Minute);
        Assert.AreEqual(dt.Second, dtBack.Second);
        Assert.AreEqual(dt.Millisecond, dtBack.Millisecond + 1); // WTF???
    }

    [TestMethod]
    public void TestGeneralMethods()
    {
        DateTime now = DateTime.Now;
        Timetag timetag1 = new(now);
        Timetag timetag2 = new(now.AddMonths(1));

        Assert.IsTrue(timetag1.Equals(timetag1));
        Assert.IsFalse(timetag1.Equals(timetag2));

        Assert.IsTrue(timetag1.Equals((object)timetag1));
        Assert.IsFalse(timetag1.Equals((object)timetag2));

        Assert.IsTrue(timetag1.Equals(timetag1.Tag));
        Assert.IsFalse(timetag1.Equals(timetag2.Tag));
    }
}
