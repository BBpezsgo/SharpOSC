using System;
using System.Diagnostics.CodeAnalysis;

namespace SharpOSC;

public readonly struct Midi(byte port, byte status, byte data1, byte data2) : IEquatable<Midi>
{
    public readonly byte Port = port;
    public readonly byte Status = status;
    public readonly byte Data1 = data1;
    public readonly byte Data2 = data2;

    public override bool Equals([NotNullWhen(true)] object? obj) => obj is Midi other && Equals(other);

    public static bool operator ==(Midi a, Midi b) => a.Equals(b);
    public static bool operator !=(Midi a, Midi b) => !a.Equals(b);

    public override int GetHashCode() => (Port << 24) + (Status << 16) + (Data1 << 8) + (Data2);
    public override string ToString() => $"Midi{{ Port: {Port} Status: {Status} Data: [{Data1}, {Data2}] }}";

    public bool Equals(Midi other) =>
        Port == other.Port &&
        Status == other.Status &&
        Data1 == other.Data1 &&
        Data2 == other.Data2;
}
