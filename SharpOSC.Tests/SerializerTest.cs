using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SharpOSC.Tests;

[TestClass, ExcludeFromCodeCoverage]
public class SerializerTest
{
    [TestMethod]
    public void TestDouble()
    {
        double val = 1234567.2324521e36;

        var msg = new OscMessage("/test/1", val);
        var bytes = msg.Serialize();

        var msg2 = (OscMessage)OscPacket.Deserialize(bytes);
        AssertUtils.AreValueEquals(val, (double)msg2.Arguments[0]!);
    }

    [TestMethod]
    public void TestBlob()
    {
        var blob = new byte[] { 23, 65, 255, 12, 6 };

        var msg = new OscMessage("/test/1", blob);
        var bytes = msg.Serialize();

        var msg2 = (OscMessage)OscPacket.Deserialize(bytes);
        AssertUtils.AreValueEquals(blob, (byte[])msg2.Arguments[0]!);
    }

    [TestMethod]
    public void TestTimetag()
    {
        var val = DateTime.Now;
        var tag = new Timetag(val);

        var msg = new OscMessage("/test/1", tag);
        var bytes = msg.Serialize();

        var msg2 = (OscMessage)OscPacket.Deserialize(bytes);
        AssertUtils.AreValueEquals(tag.Tag, ((Timetag)msg2.Arguments[0]!).Tag);
    }

    [TestMethod]
    public void TestLong()
    {
        long num = 123456789012345;
        var msg = new OscMessage("/test/1", num);
        var bytes = msg.Serialize();

        var msg2 = (OscMessage)OscPacket.Deserialize(bytes);

        AssertUtils.AreValueEquals(num, msg2.Arguments[0]);
    }

    [TestMethod]
    public void TestArray()
    {
        var list = new List<object>() { 23, true, "hello world" };
        var msg = new OscMessage("/test/1", 9999, list, 24.24f);
        var bytes = msg.Serialize();

        var msg2 = (OscMessage)OscPacket.Deserialize(bytes);

        AssertUtils.AreValueEquals(9999, msg2.Arguments[0]);
        AssertUtils.AreValueEquals(list, msg2.Arguments[1]);
        AssertUtils.AreValueEquals(list.Count, ((object?[])msg2.Arguments[1]!).Length);
        AssertUtils.AreValueEquals(24.24f, msg2.Arguments[2]);
    }

    [TestMethod]
    public void TestNoAddress()
    {
        var msg = new OscMessage(string.Empty, 9999, 24.24f);
        var bytes = msg.Serialize();

        var msg2 = (OscMessage)OscPacket.Deserialize(bytes);

        AssertUtils.AreValueEquals(string.Empty, msg2.Address);
        AssertUtils.AreValueEquals(9999, msg2.Arguments[0]);
        AssertUtils.AreValueEquals(24.24f, msg2.Arguments[1]);
    }
}
