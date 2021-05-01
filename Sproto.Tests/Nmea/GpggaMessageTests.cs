#region References

using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sproto.Nmea;

#endregion

namespace Sproto.Tests.Nmea
{
	[TestClass]
	public class GpggaMessageTests
	{
		#region Methods

		[TestMethod]
		public void MessageWithEmptyValuesShouldParse()
		{
			var m = "$GPGGA,014349.357,,,,,0,00,,,M,0.0,M,,0000*5C";
			var n = new GpggaMessage();

			n.Parse(m);

			Assert.AreEqual(14349.357, n.FixTaken);
			Assert.AreEqual("-1.00000000", n.Latitude.ToString());
			Assert.AreEqual("-1.00000000", n.Longitude.ToString());
			Assert.AreEqual("Invalid", n.FixQuality);
			Assert.AreEqual(0, n.NumberOfSatellites);
			Assert.AreEqual(0.0, n.HorizontalDilutionOfPrecision);
			Assert.AreEqual(0.0, n.Altitude);
			Assert.AreEqual(0.0, n.HeightOfGeoid);
			Assert.AreEqual(string.Empty, n.SecondsSinceLastUpdateDgps);
			Assert.AreEqual("0000", n.StationIdNumberDgps);
		}

		#endregion
	}

	[TestClass]
	public class GpgsaMessageTests : BaseTests
	{
		#region Methods

		[TestMethod]
		public void MessageShouldParse()
		{
			var m = "$GPGSA,A,1,,,,,,,,,,,,,,,*1E";
			var n = new GpgsaMessage();

			n.Parse(m);

			Assert.AreEqual("A", n.AutoSelection);
			Assert.AreEqual("1", n.Fix3D);
			Assert.AreEqual(0, n.HorizontalDilutionOfPrecision);
			Assert.AreEqual("1E", n.MandatoryChecksum);
			Assert.AreEqual(0, n.PositionDilutionOfPrecision);
			Extensions.AreEqual(new List<int>(), n.PrnsOfSatellitesUsedForFix);
			Assert.AreEqual(DateTime.MinValue, n.TimestampUtc);
			Assert.AreEqual(0, n.VerticalDilutionOfPrecision);
		}

		#endregion
	}
}