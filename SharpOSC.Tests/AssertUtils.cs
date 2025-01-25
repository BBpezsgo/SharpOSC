using System.Collections;
using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SharpOSC.Tests;

[ExcludeFromCodeCoverage]
internal static class AssertUtils
{
    public static void AreValueEquals(object? a, object? b)
    {
        if (a is null && b is null) return;
        if (a is null || b is null) throw new AssertFailedException();

        switch (a)
        {
            case byte v1:   { if (b is not byte v2)     throw new AssertFailedException(); Assert.AreEqual<byte>(v1, v2); break; }
            case sbyte v1:  { if (b is not sbyte v2)    throw new AssertFailedException(); Assert.AreEqual<sbyte>(v1, v2); break; }
            case ushort v1: { if (b is not ushort v2)   throw new AssertFailedException(); Assert.AreEqual<ushort>(v1, v2); break; }
            case short v1:  { if (b is not short v2)    throw new AssertFailedException(); Assert.AreEqual<short>(v1, v2); break; }
            case uint v1:   { if (b is not uint v2)     throw new AssertFailedException(); Assert.AreEqual<uint>(v1, v2); break; }
            case int v1:    { if (b is not int v2)      throw new AssertFailedException(); Assert.AreEqual<int>(v1, v2); break; }
            case ulong v1:  { if (b is not ulong v2)    throw new AssertFailedException(); Assert.AreEqual<ulong>(v1, v2); break; }
            case long v1:   { if (b is not long v2)     throw new AssertFailedException(); Assert.AreEqual<long>(v1, v2); break; }
            case float v1:  { if (b is not float v2)    throw new AssertFailedException(); Assert.AreEqual<float>(v1, v2); break; }
            case double v1: { if (b is not double v2)   throw new AssertFailedException(); Assert.AreEqual<double>(v1, v2); break; }
            case bool v1:   { if (b is not bool v2)     throw new AssertFailedException(); Assert.AreEqual<bool>(v1, v2); break; }
            case string v1: { if (b is not string v2)   throw new AssertFailedException(); Assert.AreEqual<string>(v1, v2); break; }
            case IEnumerable v1:
            {
                if (b is not IEnumerable v2) throw new AssertFailedException();
                var e1 = v1.GetEnumerator();
                var e2 = v2.GetEnumerator();
                while (e1.MoveNext())
                {
                    var i1 = e1.Current;
                    if (!e2.MoveNext()) throw new AssertFailedException();
                    var i2 = e2.Current;
                    AssertUtils.AreValueEquals(i1, i2);
                }
                if (e2.MoveNext()) throw new AssertFailedException();
                break;
            }
            default: { Assert.AreEqual(a, b); break; }
        }
    }
}
