#region References

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sproto.Nmea;

#endregion

namespace Sproto.Tests.Nmea
{
	[TestClass]
	public class NmeaParserTests : BaseTests
	{
		#region Methods

		[TestMethod]
		public void ParserShouldParse()
		{
			var parser = new NmeaParser();
			var m = "$GNGGA,143718.00,4513.13793,N,01859.19704,E,1,05,1.86,108.1,M,38.1,M,,*40";
			var n = parser.Parse(m) as GnggaMessage;

			Assert.IsNotNull(n);
			Assert.AreEqual(143718.00, n.FixTaken);
			Assert.AreEqual("45.21896550", n.Latitude.ToString());
			Assert.AreEqual("18.98661733", n.Longitude.ToString());
			Assert.AreEqual("GPS fix", n.FixQuality);
			Assert.AreEqual(5, n.NumberOfSatellites);
			Assert.AreEqual(1.86, n.HorizontalDilutionOfPrecision);
			Assert.AreEqual(108.1, n.Altitude);
			Assert.AreEqual(38.1, n.HeightOfGeoid);
			Assert.AreEqual(string.Empty, n.SecondsSinceLastUpdateDgps);
			Assert.AreEqual(string.Empty, n.StationIdNumberDgps);
		}

		#endregion
	}
}