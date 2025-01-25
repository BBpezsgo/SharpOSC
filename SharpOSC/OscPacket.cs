namespace SharpOSC;

public abstract class OscPacket
{
    public const int Padding = 4;

    public static OscPacket Deserialize(byte[] buffer)
    {
        if (buffer[0] == '#') return OscBundle.Deserialize(buffer);
        else return OscMessage.Deserialize(buffer);
    }

    public abstract byte[] Serialize();
}
