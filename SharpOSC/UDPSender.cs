using System;
using System.Net.Sockets;
using System.Net;

namespace SharpOSC;

public readonly struct UDPSender : IDisposable
{
    readonly IPEndPoint _remoteEP;
    readonly Socket _socket;

    public UDPSender(string address, int port)
    {
        _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        var addresses = Dns.GetHostAddresses(address);
        if (addresses.Length == 0) throw new Exception($"Unable to find IP address for {address}");

        _remoteEP = new IPEndPoint(addresses[0], port);
    }

    public void Send(byte[] message) => _socket.SendTo(message, _remoteEP);

    public void Send(IOscPacket packet) => Send(packet.Serialize());

    public void Dispose() => _socket.Dispose();
}
