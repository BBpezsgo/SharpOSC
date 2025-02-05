using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;

namespace SharpOSC;

public class OscValueComparer : IEqualityComparer<object?>
{
    const float VectorEqualityThreshold = 0.01f;
    public static readonly OscValueComparer Instance = new();

    public new bool Equals(object? x, object? y) => x switch
    {
        int v => y is int v2 && v == v2,
        float v => y is float v2 && v == v2,
        long v => y is long v2 && v == v2,
        ulong v => y is ulong v2 && v == v2,
        double v => y is double v2 && v == v2,
        char v => y is char v2 && v == v2,
        bool v => y is bool v2 && v == v2,
        string v => y is string v2 && v.Equals(v2),
        Timetag v => y is Timetag v2 && v.Equals(v2),
        Symbol v => y is Symbol v2 && v.Equals(v2),
        RGBA v => y is RGBA v2 && v.Equals(v2),
        Midi v => y is Midi v2 && v.Equals(v2),
        Vector2 v => y is Vector2 v2 && Vector2.DistanceSquared(v, v2) < (VectorEqualityThreshold * VectorEqualityThreshold),
        Vector3 v => y is Vector3 v2 && Vector3.DistanceSquared(v, v2) < (VectorEqualityThreshold * VectorEqualityThreshold),
        null => y is null,
        byte[] v => y is byte[] v2 && v.SequenceEqual(v2),
        object?[] v => y is IEnumerable<object?> v2 && v.SequenceEqual(v2, this),
        List<object?> v => y is IEnumerable<object?> v2 && v.SequenceEqual(v2, this),
        _ => throw new NotImplementedException(),
    };

    int IEqualityComparer<object?>.GetHashCode([DisallowNull] object? obj) => obj.GetHashCode();
}
