using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpOSC;

public class OscBundle : OscPacket
{
    readonly Timetag Timetag;
    public readonly List<OscMessage> Messages;
    public DateTime Timestamp => Timetag.Timestamp;

    public OscBundle(ulong timetag, params OscMessage[] args)
    {
        Timetag = new Timetag(timetag);
        Messages = new(args);
    }

    public override byte[] Serialize()
    {
        string bundle = "#bundle";
        int bundleTagLen = Utils.AlignedStringLength(bundle);
        var tag = new List<byte>();
        Serializer.SetULong(Timetag.Tag, tag);

        byte[][] outMessages = new byte[Messages.Count][];
        for (int i = 0; i < Messages.Count; i++)
        {
            OscMessage msg = Messages[i];
            outMessages[i] = msg.Serialize();
        }

        int len = bundleTagLen + tag.Count + outMessages.Sum(x => x.Length + 4);

        int length = 0;
        byte[] output = new byte[len];
        Encoding.ASCII.GetBytes(bundle).CopyTo(output, length);
        length += bundleTagLen;
        tag.CopyTo(output, length);
        length += tag.Count;

        for (int i = 0; i < outMessages.Length; i++)
        {
            var size = new List<byte>();
            Serializer.SetInt(outMessages[i].Length, size);
            size.CopyTo(output, length);
            length += size.Count;

            outMessages[i].CopyTo(output, length);
            length += outMessages[i].Length; // msg size is always a multiple of 4
        }

        return output;
    }

    /// <summary>
    /// Takes in an OSC bundle package in byte form and parses it into a more usable OscBundle object
    /// </summary>
    /// <returns>
    /// Bundle containing elements and a timetag
    /// </returns>
    public static OscBundle Deserialize(ReadOnlySpan<byte> buffer)
    {
        string bundleTag = Encoding.ASCII.GetString(buffer[..8]);
        if (bundleTag != "#bundle\0") throw new Exception("Not a bundle");

        ulong timetag;
        List<OscMessage> messages = new();

        int index = 8;

        timetag = Deserializer.GetULong(buffer, index);
        index += 8;

        while (index < buffer.Length)
        {
            int size = Deserializer.GetInt(buffer, index);
            index += 4;

            ReadOnlySpan<byte> messageBytes = buffer.Slice(index, size);
            var message = OscMessage.Deserialize(messageBytes);

            messages.Add(message);

            index += size;
            while (index % 4 != 0) index++;
        }

        OscBundle output = new OscBundle(timetag, messages.ToArray());
        return output;
    }
}
