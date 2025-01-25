using System;
using System.Diagnostics.CodeAnalysis;

namespace SharpOSC;

public readonly struct RGBA(byte red, byte green, byte blue, byte alpha) : IEquatable<RGBA>
{
    public readonly byte R = red;
    public readonly byte G = green;
    public readonly byte B = blue;
    public readonly byte A = alpha;

    public override bool Equals([NotNullWhen(true)] object? obj) => obj is RGBA other && Equals(other);

    public static bool operator ==(RGBA a, RGBA b) => a.Equals(b);
    public static bool operator !=(RGBA a, RGBA b) => !a.Equals(b);

    public override int GetHashCode() => (R << 24) + (G << 16) + (B << 8) + (A);
    public override string ToString() => $"({R}, {G}, {B}, {A})";

    public bool Equals(RGBA other) =>
        R == other.R &&
        G == other.G &&
        B == other.B &&
        A == other.A;
}
