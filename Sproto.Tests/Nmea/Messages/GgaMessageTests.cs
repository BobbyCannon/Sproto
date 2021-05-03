#region References

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sproto.Nmea.Messages;

#endregion

namespace Sproto.Tests.Nmea.Messages
{
	[TestClass]
	public class GgaMessageTests
	{
		#region Methods

		[TestMethod]
		public void TestMethodParse()
		{
			var m = "$GNGGA,143718.00,4513.13793,N,01859.19704,E,1,05,1.86,108.1,M,38.1,M,,*40";
			var n = new GgaMessage();

			n.Parse(m);

			Assert.AreEqual(143718.00, n.FixTaken);
			Assert.AreEqual("45.21896550", n.Latitude.ToString());
			Assert.AreEqual("18.98661733", n.Longitude.ToString());
			Assert.AreEqual("1", n.FixQuality);
			Assert.AreEqual(5, n.NumberOfSatellites);
			Assert.AreEqual(1.86, n.HorizontalDilutionOfPrecision);
			Assert.AreEqual(108.1, n.Altitude);
			Assert.AreEqual(38.1, n.HeightOfGeoid);
			Assert.AreEqual(string.Empty, n.SecondsSinceLastUpdateDgps);
			Assert.AreEqual(string.Empty, n.StationIdNumberDgps);

			m = "$GPGGA,014349.357,,,,,0,00,,,M,0.0,M,,0000*5C";
			n.Parse(m);

			Assert.AreEqual(14349.357, n.FixTaken);
			Assert.AreEqual("-1.00000000", n.Latitude.ToString());
			Assert.AreEqual("-1.00000000", n.Longitude.ToString());
			Assert.AreEqual("0", n.FixQuality);
			Assert.AreEqual(0, n.NumberOfSatellites);
			Assert.AreEqual(0.0, n.HorizontalDilutionOfPrecision);
			Assert.AreEqual(0.0, n.Altitude);
			Assert.AreEqual(0.0, n.HeightOfGeoid);
			Assert.AreEqual(string.Empty, n.SecondsSinceLastUpdateDgps);
			Assert.AreEqual("0000", n.StationIdNumberDgps);
		}

		#endregion
	}
}