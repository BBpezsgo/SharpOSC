using System.Diagnostics.CodeAnalysis;
using System.Net.Sockets;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SharpOSC.Tests;

[TestClass, DoNotParallelize, ExcludeFromCodeCoverage]
public class ListenerTest
{
    /// <summary>
    /// Opens a listener on a specified port, then closes it and attempts to open another on the same port
    /// Opening the second listener will fail unless the first one has been properly closed.
    /// </summary>
    [TestMethod]
    public void CloseListener()
    {
        using (var listener = new UDPListener(55555))
        { listener.Receive(); }

        using (var listener = new UDPListener(55555))
        { listener.Receive(); }
    }

    /// <summary>
    /// Tries to open two listeners on the same port, results in an exception
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(SocketException))]
    public void CloseListenerException()
    {
        using UDPListener l1 = new UDPListener(55555);
        l1.Receive();

        using var l2 = new UDPListener(55555);
    }

    /// <summary>
    /// Single message receive
    /// </summary>
    [TestMethod]
    public void ListenerSingleMSG()
    {
        using var listener = new UDPListener(55555);

        using var sender = new UDPSender("127.0.0.1", 55555);

        var msg = new OscMessage("/test/", 23.42f);

        sender.Send(msg);

        while (true)
        {
            var pack = listener.Receive();
            if (pack == null) Thread.Sleep(1);
            else break;
        }
    }

    /// <summary>
    /// Bombard the listener with messages, check if they are all received
    /// </summary>
    [TestMethod]
    public void ListenerLoadTest()
    {
        using var listener = new UDPListener(55555);

        using var sender = new UDPSender("127.0.0.1", 55555);

        var msg = new OscMessage("/test/", 23.42f);

        for (int i = 0; i < 1000; i++)
            sender.Send(msg);

        for (int i = 0; i < 1000; i++)
        {
            var receivedMessage = listener.Receive();
            Assert.IsNotNull(receivedMessage);
        }
    }
}
