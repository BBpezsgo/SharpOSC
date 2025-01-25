namespace SharpOSC;

public class Utils
{
    public static int AlignedStringLength(string value)
    {
        int len = value.Length + (OscPacket.Padding - value.Length % OscPacket.Padding);
        if (len <= value.Length) len += OscPacket.Padding;

        return len;
    }
}
