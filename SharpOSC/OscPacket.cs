using System;

namespace SharpOSC;

public interface IOscPacket
{
    public const int Padding = 4;

    public static IOscPacket Deserialize(ReadOnlySpan<byte> buffer)
    {
        if (buffer[0] == '#') return OscBundle.Deserialize(buffer);
        else return OscMessage.Deserialize(buffer);
    }

    public abstract byte[] Serialize();
}
