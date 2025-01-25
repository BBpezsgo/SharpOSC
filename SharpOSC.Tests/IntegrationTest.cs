using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SharpOSC.Tests;

[TestClass, DoNotParallelize, ExcludeFromCodeCoverage]
public class IntegrationTest
{
  [TestMethod]
  public void TestMessage()
  {
    using var listener = new UDPListener(55555);

    using var sender = new UDPSender("127.0.0.1", 55555);

    // Test every message type (except Symbol)
    var msg1 = new OscMessage(
        "/test/address",

        23,
        42.42f,
        "hello world",
        new byte[3] { 2, 3, 4 },
        -123456789123L,
        new Timetag(DateTime.Now.Date).Tag,
        new Timetag(DateTime.Now.Date.AddMonths(1)),
        1234567.890d,
        new Symbol("wut wut"),
        'x',
        new RGBA(20, 40, 60, 255),
        new Midi(3, 110, 55, 66),
        true,
        false,
        null,
        double.PositiveInfinity
    );

    sender.Send(msg1);
    Thread.Sleep(50);
    OscMessage? msgRevc = (OscMessage?)listener.Receive();
    Assert.IsNotNull(msgRevc);

    Assert.AreEqual("/test/address", msgRevc.Address);
    Assert.AreEqual(16, msgRevc.Arguments.Length);

    AssertUtils.AreValueEquals(23, msgRevc.Arguments[0]);
    AssertUtils.AreValueEquals(42.42f, msgRevc.Arguments[1]);
    AssertUtils.AreValueEquals("hello world", msgRevc.Arguments[2]);
    AssertUtils.AreValueEquals(new byte[3] { 2, 3, 4 }, msgRevc.Arguments[3]);
    AssertUtils.AreValueEquals(-123456789123L, msgRevc.Arguments[4]);
    AssertUtils.AreValueEquals(new Timetag(DateTime.Now.Date), msgRevc.Arguments[5]);
    AssertUtils.AreValueEquals(new Timetag(DateTime.Now.Date.AddMonths(1)), msgRevc.Arguments[6]);
    AssertUtils.AreValueEquals(1234567.890d, msgRevc.Arguments[7]);
    AssertUtils.AreValueEquals(new Symbol("wut wut"), msgRevc.Arguments[8]);
    AssertUtils.AreValueEquals('x', msgRevc.Arguments[9]);
    AssertUtils.AreValueEquals(new RGBA(20, 40, 60, 255), msgRevc.Arguments[10]);
    AssertUtils.AreValueEquals(new Midi(3, 110, 55, 66), msgRevc.Arguments[11]);
    AssertUtils.AreValueEquals(true, msgRevc.Arguments[12]);
    AssertUtils.AreValueEquals(false, msgRevc.Arguments[13]);
    AssertUtils.AreValueEquals(null, msgRevc.Arguments[14]);
    AssertUtils.AreValueEquals(double.PositiveInfinity, msgRevc.Arguments[15]);
  }

  [TestMethod]
  public void TestBundle()
  {
    using var listener = new UDPListener(55555);

    using var sender1 = new UDPSender("127.0.0.1", 55555);
    var msg1 = new OscMessage("/test/address1", 23, 42.42f, "hello world", new byte[3] { 2, 3, 4 });
    var msg2 = new OscMessage("/test/address2", 34, 24.24f, "hello again", new byte[5] { 5, 6, 7, 8, 9 });
    var dt = DateTime.Now;
    var bundle = new OscBundle(new Timetag(dt).Tag, msg1, msg2);

    sender1.Send(bundle);
    sender1.Send(bundle);
    sender1.Send(bundle);
    Thread.Sleep(50);

    var recv = (OscBundle?)listener.Receive();
    Assert.IsNotNull(recv);

    recv = (OscBundle?)listener.Receive();
    Assert.IsNotNull(recv);

    recv = (OscBundle?)listener.Receive();
    Assert.IsNotNull(recv);

    Assert.AreEqual(dt.Date, recv.Timestamp.Date);
    Assert.AreEqual(dt.Hour, recv.Timestamp.Hour);
    Assert.AreEqual(dt.Minute, recv.Timestamp.Minute);
    Assert.AreEqual(dt.Second, recv.Timestamp.Second);
    //Assert.AreEqual(dt.Millisecond, recv.DateTime.Millisecond); Ventus not accurate enough

    AssertUtils.AreValueEquals("/test/address1", recv.Messages[0].Address);
    AssertUtils.AreValueEquals(4, recv.Messages[0].Arguments.Length);
    AssertUtils.AreValueEquals(23, recv.Messages[0].Arguments[0]);
    AssertUtils.AreValueEquals(42.42f, recv.Messages[0].Arguments[1]);
    AssertUtils.AreValueEquals("hello world", recv.Messages[0].Arguments[2]);
    AssertUtils.AreValueEquals(new byte[3] { 2, 3, 4 }, recv.Messages[0].Arguments[3]);

    AssertUtils.AreValueEquals("/test/address2", recv.Messages[1].Address);
    AssertUtils.AreValueEquals(4, recv.Messages[1].Arguments.Length);
    AssertUtils.AreValueEquals(34, recv.Messages[1].Arguments[0]);
    AssertUtils.AreValueEquals(24.24f, recv.Messages[1].Arguments[1]);
    AssertUtils.AreValueEquals("hello again", recv.Messages[1].Arguments[2]);
    AssertUtils.AreValueEquals(new byte[5] { 5, 6, 7, 8, 9 }, recv.Messages[1].Arguments[3]);
  }
}
