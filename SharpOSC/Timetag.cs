using System;
using System.Diagnostics.CodeAnalysis;

namespace SharpOSC;

public readonly struct Timetag : IEquatable<Timetag>, IEquatable<ulong>
{
    public readonly ulong Tag;

    public DateTime Timestamp
    {
        get
        {
            if (Tag == 1) return DateTime.Now;

            uint seconds = (uint)(Tag >> 32);
            var time = DateTime.Parse("1900-01-01 00:00:00");
            time = time.AddSeconds(seconds);

            double fraction = (double)(Tag & 0x00000000FFFFFFFF) / (double)0xFFFFFFFF;
            time = time.AddSeconds(fraction);

            return time;
        }
    }

    /// <summary>
    /// Gets or sets the fraction of a second in the timestamp. the double precision number is multiplied by 2^32
    /// giving an accuracy down to about 230 picoseconds ( 1/(2^32) of a second)
    /// </summary>
    public double Fraction
    {
        get
        {
            if (Tag == 1) return 0d;
            uint seconds = (uint)(Tag & 0x00000000FFFFFFFF);
            double fraction = (double)seconds / (uint)0xFFFFFFFF;
            return fraction;
        }
    }

    public Timetag(ulong value)
    {
        Tag = value;
    }

    public Timetag(DateTime value)
    {
        ulong seconds = (uint)(value - DateTime.Parse("1900-01-01 00:00:00.000")).TotalSeconds;
        ulong fraction = (uint)(0xFFFFFFFF * ((double)value.Millisecond / 1000));

        ulong output = (seconds << 32) + fraction;
        Tag = output;
    }

    public Timetag AddFraction(double value) => new((Tag & 0xFFFFFFFF00000000) + (uint)(value * 0xFFFFFFFF));

    public override bool Equals([NotNullWhen(true)] object? obj) => obj switch
    {
        null => false,
        Timetag v => Tag == v.Tag,
        ulong v => Tag == v,
        _ => false
    };

    public static bool operator ==(Timetag a, Timetag b) => a.Equals(b);
    public static bool operator !=(Timetag a, Timetag b) => a.Equals(b);

    public override int GetHashCode() => (int)(((uint)(Tag >> 32) + (uint)(Tag & 0x00000000FFFFFFFF)) / 2);
    public override string ToString() => $"{Timestamp} ({Tag})";

    public bool Equals(Timetag other) => Tag == other.Tag;
    public bool Equals(ulong other) => Tag == other;
}
