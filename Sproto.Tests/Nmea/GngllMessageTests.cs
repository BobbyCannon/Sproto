#region References

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sproto.Nmea;

#endregion

namespace Sproto.Tests.Nmea
{
	[TestClass]
	public class GngllMessageTests
	{
		#region Methods

		[TestMethod]
		public void TestMethodParse()
		{
			var m = "$GNGLL,4513.13795,N,01859.19702,E,143717.00,A,A*72";
			var n = new GngllMessage();

			n.Parse(m);

			Assert.AreEqual("45.21896583", n.Latitude.ToString());
			Assert.AreEqual("18.98661700", n.Longitude.ToString());
			Assert.AreEqual("143717.00", n.FixTaken);
			Assert.AreEqual("A", n.DataValid);
			Assert.AreEqual("Autonomous", n.ModeIndicator.Mode);
		}

		#endregion
	}
}