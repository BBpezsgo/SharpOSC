namespace SharpOSC;

public class Utils
{
    public static int AlignedStringLength(string value)
    {
        int len = value.Length + (IOscPacket.Padding - value.Length % IOscPacket.Padding);
        if (len <= value.Length) len += IOscPacket.Padding;

        return len;
    }
}
