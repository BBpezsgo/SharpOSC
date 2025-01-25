using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace SharpOSC;

public class OscMessage : OscPacket
{
    public readonly string Address;
    public readonly ImmutableArray<object?> Arguments;

    public OscMessage(string address, params object?[] args)
    {
        Address = address;
        Arguments = args.ToImmutableArray();
    }

    public OscMessage(string address, ImmutableArray<object?> args)
    {
        Address = address;
        Arguments = args;
    }

    static int FirstIndexAfter(ReadOnlySpan<byte> items, int start, Func<byte, bool> predicate)
    {
        int retVal = 0;
        foreach (var item in items)
        {
            if (retVal >= start && predicate(item)) return retVal;
            retVal++;
        }
        return -1;
    }

    /// <summary>
    /// Takes in an OSC bundle package in byte form and parses it into a more usable OscBundle object
    /// </summary>
    /// <returns>
    /// Message containing various arguments and an address
    /// </returns>
    public static void DeserializeArgument(ReadOnlySpan<byte> buffer, ReadOnlySpan<char> types, ref int typeIndex, ref int bufferIndex, List<object?> arguments)
    {
        char type = types[typeIndex++];
        switch (type)
        {
            case '\0':
                break;

            case 'i':
                arguments.Add(Deserializer.GetInt(buffer, bufferIndex));
                bufferIndex += sizeof(int);
                break;

            case 'f':
                arguments.Add(Deserializer.GetFloat(buffer, bufferIndex));
                bufferIndex += sizeof(int);
                break;

            case 's':
                string stringVal = Deserializer.GetString(buffer, bufferIndex)!;
                arguments.Add(stringVal);
                bufferIndex += stringVal.Length;
                break;

            case 'b':
                var blob = Deserializer.GetBlob(buffer, bufferIndex);
                arguments.Add(blob.ToArray());
                bufferIndex += sizeof(int) + blob.Length;
                break;

            case 'h':
                arguments.Add(Deserializer.GetLong(buffer, bufferIndex));
                bufferIndex += sizeof(long);
                break;

            case 't':
                arguments.Add(new Timetag(Deserializer.GetULong(buffer, bufferIndex)));
                bufferIndex += sizeof(ulong);
                break;

            case 'd':
                arguments.Add(Deserializer.GetDouble(buffer, bufferIndex));
                bufferIndex += sizeof(double);
                break;

            case 'S':
                string value = Deserializer.GetString(buffer, bufferIndex)!;
                arguments.Add(new Symbol(value));
                bufferIndex += value.Length;
                break;

            case 'c':
                arguments.Add(Deserializer.GetChar(buffer, bufferIndex));
                bufferIndex += 4;
                break;

            case 'r':
                arguments.Add(Deserializer.GetRGBA(buffer, bufferIndex));
                bufferIndex += 4;
                break;

            case 'm':
                arguments.Add(Deserializer.GetMidi(buffer, bufferIndex));
                bufferIndex += 4;
                break;

            case 'T':
                arguments.Add(true);
                break;

            case 'F':
                arguments.Add(false);
                break;

            case 'N':
                arguments.Add(null);
                break;

            case 'I':
                arguments.Add(double.PositiveInfinity);
                break;

            case '[':
                List<object?> _arguments = new();
                while (types[typeIndex] != ']')
                {
                    DeserializeArgument(buffer, types, ref typeIndex, ref bufferIndex, _arguments);
                }
                typeIndex++;
                arguments.Add(_arguments.ToArray());
                break;

            case ']':
                throw new Exception($"Unexpected OSC type tag '{type}'.");

            default:
                throw new Exception($"OSC type tag '{type}' is unknown.");
        }

        while (bufferIndex % Padding != 0) bufferIndex++;
    }

    /// <summary>
    /// Takes in an OSC bundle package in byte form and parses it into a more usable OscBundle object
    /// </summary>
    /// <returns>
    /// Message containing various arguments and an address
    /// </returns>
    public static OscMessage Deserialize(ReadOnlySpan<byte> buffer)
    {
        int bufferIndex = 0;

        string? address = null;
        List<object?> arguments = new();
        List<object?> mainArray = arguments; // used as a reference when we are parsing arrays to get the main array back

        // Get address
        address = Deserializer.GetAddress(buffer, bufferIndex);
        bufferIndex += FirstIndexAfter(buffer, address.Length, x => x == ',');

        if (bufferIndex % Padding != 0) throw new Exception($"Misaligned OSC Packet data. Address string is not padded correctly and does not align to {Padding} byte interval");

        // Get type tags
        int typesLength = Deserializer.GetTypesLength(buffer, bufferIndex);
        Span<char> types = stackalloc char[typesLength];
        typesLength = Deserializer.GetTypes(buffer, bufferIndex, types);
        types = types[..typesLength];
        bufferIndex += types.Length;

        while (bufferIndex % Padding != 0) bufferIndex++;

        int typeIndex = 0;

        if (types[0] == ',') typeIndex++;

        while (typeIndex < types.Length)
        {
            DeserializeArgument(buffer, types, ref typeIndex, ref bufferIndex, arguments);
        }

        return new OscMessage(address, arguments.ToArray());
    }

    static void SerializeArgument(object? argument, List<byte> buffer, StringBuilder types)
    {
        switch (argument)
        {
            case int v:
                types.Append('i');
                Serializer.SetInt(v, buffer);
                break;
            case float v:
                if (float.IsPositiveInfinity(v))
                {
                    types.Append('I');
                }
                else
                {
                    types.Append('f');
                    Serializer.SetFloat(v, buffer);
                }
                break;
            case string v:
                types.Append('s');
                Serializer.SetString(v, buffer);
                break;
            case byte[] v:
                types.Append('b');
                Serializer.SetBlob(v, buffer);
                break;
            case long v:
                types.Append('h');
                Serializer.SetLong(v, buffer);
                break;
            case ulong v:
                types.Append('t');
                Serializer.SetULong(v, buffer);
                break;
            case Timetag v:
                types.Append('t');
                Serializer.SetULong(v.Tag, buffer);
                break;
            case double v:
                if (double.IsPositiveInfinity(v))
                {
                    types.Append('I');
                }
                else
                {
                    types.Append('d');
                    Serializer.SetDouble(v, buffer);
                }
                break;

            case Symbol v:
                types.Append('S');
                Serializer.SetString(v.Value, buffer);
                break;

            case char v:
                types.Append('c');
                Serializer.SetChar(v, buffer);
                break;
            case RGBA v:
                types.Append('r');
                Serializer.SetRGBA(v, buffer);
                break;
            case Midi v:
                types.Append('m');
                Serializer.SetMidi(v, buffer);
                break;
            case bool v:
                types.Append(v ? "T" : "F");
                break;
            case null:
                types.Append('N');
                break;

            case object?[] v:
                types.Append('[');
                foreach (object? item in v)
                {
                    SerializeArgument(item, buffer, types);
                }
                types.Append(']');
                break;

            case List<object?> v:
                types.Append('[');
                foreach (object? item in v)
                {
                    SerializeArgument(item, buffer, types);
                }
                types.Append(']');
                break;

            default:
                throw new Exception($"Unable to transmit values of type {argument.GetType()}");
        }
    }

    public override byte[] Serialize()
    {
        List<byte> buffer = new();

        StringBuilder typesBuilder = new();
        typesBuilder.Append(',');

        foreach (object? argument in Arguments)
        {
            SerializeArgument(argument, buffer, typesBuilder);
        }

        string typesString = typesBuilder.ToString();

        int addressLength = (Address.Length == 0) ? 0 : Utils.AlignedStringLength(Address);
        int typeLength = Utils.AlignedStringLength(typesString);

        byte[] result = new byte[addressLength + typeLength + buffer.Count];
        int i = 0;

        Encoding.ASCII.GetBytes(Address, result.AsSpan()[i..]);
        i += addressLength;

        Encoding.ASCII.GetBytes(typesString, result.AsSpan()[i..]);
        i += typeLength;

        buffer.CopyTo(result, i);

        return result;
    }
}
