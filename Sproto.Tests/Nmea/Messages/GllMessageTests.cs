#region References

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sproto.Nmea;
using Sproto.Nmea.Messages;

#endregion

namespace Sproto.Tests.Nmea.Messages
{
	[TestClass]
	public class GllMessageTests
	{
		#region Methods

		[TestMethod]
		public void TestMethodParse()
		{
			var m = "$GNGLL,4513.13795,N,01859.19702,E,143717.00,A,A*72";
			var n = new GllMessage();
			var e = new GllMessage
			{
				Prefix = NmeaMessagePrefix.GlobalNavigationSatelliteSystem,
				Latitude = new Location("4513.13795", "N"),
				Longitude = new Location("01859.19702", "E"),
				FixTaken = "143717.00",
				DataValid = "A",
				ModeIndicator = new ModeIndicator("Autonomous"),
				Checksum = "72"
			};

			n.Parse(m);

			Extensions.AreEqual(e, n);
		}

		#endregion
	}
}