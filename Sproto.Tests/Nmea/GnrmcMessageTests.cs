#region References

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sproto.Nmea;

#endregion

namespace Sproto.Tests.Nmea
{
	[TestClass]
	public class GnrmcMessageTests
	{
		#region Methods

		[TestMethod]
		public void TestMethodParse()
		{
			var m = "$GNRMC,143718.00,A,4513.13793,N,01859.19704,E,0.050,,290719,,,A*65";
			var n = new GnrmcMessage();

			n.Parse(m);

			Assert.AreEqual("143718.00", n.TimeOfFix);
			Assert.AreEqual("OK", n.NavigationReceiverWarning);
			Assert.AreEqual("45.21896550", n.Latitude.ToString());
			Assert.AreEqual("18.98661733", n.Longitude.ToString());
			Assert.AreEqual("0.050", n.SpeedOverGround);
			Assert.AreEqual(string.Empty, n.CourseMadeGood);
			Assert.AreEqual("290719", n.DateOfFix);
			Assert.AreEqual(string.Empty, n.MagneticVariation);
			Assert.AreEqual("Autonomous", n.ModeIndicator.Mode);
		}

		#endregion
	}
}