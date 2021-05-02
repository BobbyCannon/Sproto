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
				DataValid = "A",
				FixTaken = "143717.00",
				Latitude = new Location("4513.13795", "N"),
				Longitude = new Location("01859.19702", "E"),
				Checksum = "72",
				ModeIndicator = new ModeIndicator("Autonomous"),
				Prefix = NmeaMessagePrefix.GlobalNavigationSatelliteSystem
			};

			n.Parse(m);

			Extensions.AreEqual(e, n);
		}

		#endregion
	}
}