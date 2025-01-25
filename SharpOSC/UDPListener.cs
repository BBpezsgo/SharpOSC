using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace SharpOSC;

public delegate void HandleOscPacket(OscPacket? packet);
public delegate void HandleBytePacket(byte[] packet);

public class UDPListener : IDisposable
{
    readonly object _callbackLock;

    readonly UdpClient _receivingUdpClient;
    IPEndPoint? _remoteEP;

    readonly HandleBytePacket? _bytePacketCallback;
    readonly HandleOscPacket? _oscPacketCallback;
    readonly ManualResetEvent _closingEvent;

    readonly Queue<byte[]> _queue;

    public UDPListener(int port)
    {
        _queue = new Queue<byte[]>();
        _closingEvent = new ManualResetEvent(false);
        _callbackLock = new object();

        const int MaxRetries = 10;

        // try to open the port 10 times, else fail
        for (int retry = 0; retry < MaxRetries; retry++)
        {
            try
            {
                _receivingUdpClient = new UdpClient(port);
                break;
            }
            catch (Exception)
            {
                // Failed in ten tries, throw the exception and give up
                if (retry >= MaxRetries - 1) throw;

                Thread.Sleep(50);
            }
        }
        _remoteEP = new IPEndPoint(IPAddress.Any, 0);

        // setup first async event
        AsyncCallback callBack = new(ReceiveCallback);
        _receivingUdpClient!.BeginReceive(callBack, null);
    }

    public UDPListener(int port, HandleOscPacket callback) : this(port)
    {
        _oscPacketCallback = callback;
    }

    public UDPListener(int port, HandleBytePacket callback) : this(port)
    {
        _bytePacketCallback = callback;
    }

    void ReceiveCallback(IAsyncResult result)
    {
        Monitor.Enter(_callbackLock);
        byte[]? bytes = null;

        try
        {
            bytes = _receivingUdpClient.EndReceive(result, ref _remoteEP);
        }
        catch (ObjectDisposedException)
        {
            // Ignore if disposed. This happens when closing the listener
        }

        // Process bytes
        if (bytes != null && bytes.Length > 0)
        {
            if (_bytePacketCallback != null)
            {
                _bytePacketCallback(bytes);
            }
            else if (_oscPacketCallback != null)
            {
                OscPacket? packet = null;
                try
                {
                    packet = OscPacket.Deserialize(bytes);
                }
                catch (Exception)
                {
                    // If there is an error reading the packet, null is sent to the callback
                }

                _oscPacketCallback(packet);
            }
            else
            {
                lock (_queue)
                {
                    _queue.Enqueue(bytes);
                }
            }
        }

        if (_closing)
        {
            _closingEvent.Set();
        }
        else
        {
            // Setup next async event
            AsyncCallback callBack = new(ReceiveCallback);
            _receivingUdpClient.BeginReceive(callBack, null);
        }
        Monitor.Exit(_callbackLock);
    }

    bool _closing = false;
    public void Dispose()
    {
        lock (_callbackLock)
        {
            _closingEvent.Reset();
            _closing = true;
            _receivingUdpClient.Dispose();
        }
        _closingEvent.WaitOne();
    }

    public OscPacket? Receive()
    {
        byte[]? bytes = ReceiveBytes();
        if (bytes is not null) return OscPacket.Deserialize(bytes);
        return null;
    }

    public byte[]? ReceiveBytes()
    {
        ObjectDisposedException.ThrowIf(_closing, this);

        lock (_queue)
        {
            return _queue.TryDequeue(out byte[]? bytes) ? bytes : null;
        }
    }
}
