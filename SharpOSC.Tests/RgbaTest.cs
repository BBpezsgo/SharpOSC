using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SharpOSC.Tests;

[TestClass, ExcludeFromCodeCoverage]
public class RgbaTest
{
    [TestMethod]
    public void TestGeneralMethods()
    {
        RGBA rgba1 = new(1, 2, 3, 4);
        RGBA rgba2 = new(2, 3, 4, 5);

        Assert.IsTrue(rgba1.Equals(rgba1));
        Assert.IsFalse(rgba1.Equals(rgba2));

        Assert.IsTrue(rgba1.Equals((object)rgba1));
        Assert.IsFalse(rgba1.Equals((object)rgba2));
    }
}
