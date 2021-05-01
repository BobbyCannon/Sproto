#region References

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sproto.Nmea;

#endregion

namespace Sproto.Tests.Nmea
{
	[TestClass]
	public class GnvtgMessageTests

	{
		#region Methods

		[TestMethod]
		public void TestMethodParse()
		{
			var m = "$GNVTG,,T,,M,0.050,N,0.092,K,A*33";
			var n = new GnvtgMessage();

			n.Parse(m);

			Assert.AreEqual("T", n.TrueTrackMadeGood);
			Assert.AreEqual("M", n.MagneticTrackMadeGood);
			Assert.AreEqual("0.050N", n.GroundSpeedKnots);
			Assert.AreEqual("0.092K", n.GroundSpeedKilometersPerHour);
			Assert.AreEqual("Autonomous", n.ModeIndicator.Mode);
		}

		#endregion
	}
}