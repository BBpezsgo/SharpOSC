using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SharpOSC.Tests;

[TestClass, DoNotParallelize, ExcludeFromCodeCoverage]
public class CallbackTest
{
    [TestMethod]
    public void TestCallback()
    {
        bool cbCalled = false;

        void _callback(OscPacket? packet)
        {
            cbCalled = true;
            Assert.IsNotNull(packet);
            Assert.IsInstanceOfType<OscMessage>(packet);
            OscMessage msg = (OscMessage)packet;
            AssertUtils.AreValueEquals(2, msg.Arguments.Length);
            AssertUtils.AreValueEquals(23, msg.Arguments[0]);
            AssertUtils.AreValueEquals("hello world", msg.Arguments[1]);
        }

        using var l1 = new UDPListener(55555, _callback);

        using var sender = new UDPSender("127.0.0.1", 55555);
        var msg1 = new OscMessage("/test/address", 23, "hello world");
        sender.Send(msg1);

        // Wait until callback processes its message
        var start = DateTime.Now;
        while (cbCalled == false && start.AddSeconds(2) > DateTime.Now)
            Thread.Sleep(1);

        Assert.IsTrue(cbCalled);
    }

    [TestMethod]
    public void TestByteCallback()
    {
        bool cbCalled = false;

        void _callback(byte[] packet)
        {
            var msg = (OscMessage)OscPacket.Deserialize(packet);
            AssertUtils.AreValueEquals(2, msg.Arguments.Length);
            AssertUtils.AreValueEquals(23, msg.Arguments[0]);
            AssertUtils.AreValueEquals("hello world", msg.Arguments[1]);
            cbCalled = true;
        }

        using var l1 = new UDPListener(55555, _callback);

        using var sender = new UDPSender("127.0.0.1", 55555);
        var msg1 = new OscMessage("/test/address", 23, "hello world");
        sender.Send(msg1);

        // Wait until callback processes its message
        var start = DateTime.Now;
        while (cbCalled == false && start.AddSeconds(2) > DateTime.Now)
            Thread.Sleep(1);

        Assert.IsTrue(cbCalled);
    }
}
