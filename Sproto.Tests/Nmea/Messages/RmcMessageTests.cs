#region References

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sproto.Nmea;
using Sproto.Nmea.Messages;

#endregion

namespace Sproto.Tests.Nmea.Messages
{
	[TestClass]
	public class RmcMessageTests
	{
		#region Methods

		[TestMethod]
		public void TestMethodParse()
		{
			var scenarios = new (string sentance, RmcMessage expected)[]
			{
				(
					"$GNRMC,143718.00,A,4513.13793,N,01859.19704,E,0.050,,290719,,,A*65",
					new RmcMessage
					{
						Prefix = NmeaMessagePrefix.GlobalNavigationSatelliteSystem,
						TimeOfFix = "143718.00",
						Status = "A",
						Latitude = new Location("4513.13793", "N"),
						Longitude = new Location("01859.19704", "E"),
						Speed = "0.050",
						Course = "",
						DateOfFix = "290719",
						MagneticVariation = "",
						MagneticVariationUnit = "",
						ModeIndicator = new ModeIndicator("A"),
						Checksum = "65"
					}
				),
				(
					"$GPRMC,002959.00,A,4253.65205,N,07852.11902,W,0.022,,020521,,,D*63",
					new RmcMessage
					{
						Prefix = NmeaMessagePrefix.GlobalPositioningSystem,
						TimeOfFix = "002959.00",
						Status = "A",
						Latitude = new Location("4253.65205", "N"),
						Longitude = new Location("07852.11902", "W"),
						Speed = "0.022",
						Course = "",
						DateOfFix = "020521",
						MagneticVariation = "",
						MagneticVariationUnit = "",
						ModeIndicator = new ModeIndicator("D"),
						Checksum = "63"
					}
				)
			};

			foreach (var scenario in scenarios)
			{
				scenario.expected.UpdateChecksum();
				scenario.expected.ToString().Dump();

				var actual = new RmcMessage();
				actual.Parse(scenario.sentance);

				Extensions.AreEqual(scenario.expected, actual);
				Assert.AreEqual(scenario.expected.ToString(), actual.ToString());
			}
		}

		#endregion
	}
}