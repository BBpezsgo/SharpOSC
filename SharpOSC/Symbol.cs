using System;
using System.Diagnostics.CodeAnalysis;

namespace SharpOSC;

public readonly struct Symbol(string value) : IEquatable<Symbol>, IEquatable<string>
{
    public readonly string Value = value;

    public override bool Equals([NotNullWhen(true)] object? obj) => obj switch
    {
        null => false,
        Symbol v => Value.Equals(v.Value),
        string v => Value.Equals(v),
        _ => false
    };

    public static bool operator ==(Symbol a, Symbol b) => a.Equals(b);
    public static bool operator !=(Symbol a, Symbol b) => !a.Equals(b);

    public override int GetHashCode() => Value.GetHashCode();
    public override string ToString() => Value;
    public bool Equals(Symbol other) => Value.Equals(other.Value);
    public bool Equals(string? other) => Value.Equals(other);
}
