using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SharpOSC.Tests;

[TestClass, ExcludeFromCodeCoverage]
public class MidiTest
{
    [TestMethod]
    public void TestGeneralMethods()
    {
        Midi midi1 = new(1, 2, 3, 4);
        Midi midi2 = new(2, 3, 4, 5);

        Assert.IsTrue(midi1.Equals(midi1));
        Assert.IsFalse(midi1.Equals(midi2));

        Assert.IsTrue(midi1.Equals((object)midi1));
        Assert.IsFalse(midi1.Equals((object)midi2));
    }
}
