#region References

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sproto.Nmea;

#endregion

namespace Sproto.Tests.Nmea
{
	[TestClass]
	public class GngsaMessageTests

	{
		#region Methods

		[TestMethod]
		public void TestMethodParse()
		{
			var m = "$GNGSA,A,3,01,18,32,08,11,,,,,,,,6.16,1.86,5.88*16";
			var n = new GngsaMessage();

			n.Parse(m);

			Assert.AreEqual("A", n.AutoSelection);
			Assert.AreEqual("3", n.Fix3D);
			Assert.AreEqual(5, n.PrnsOfSatellitesUsedForFix.Count); //"01,18,32,08,11,,,,,,,");
			Assert.AreEqual(6.16, n.PositionDilutionOfPrecision);
			Assert.AreEqual(1.86, n.HorizontalDilutionOfPrecision);
			Assert.AreEqual(5.88, n.VerticalDilutionOfPrecision);
		}

		#endregion
	}
}