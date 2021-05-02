#region References

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sproto.Nmea;
using Sproto.Nmea.Messages;

#endregion

namespace Sproto.Tests.Nmea.Messages
{
	[TestClass]
	public class GnsMessageTests : BaseTests
	{
		#region Methods

		[TestMethod]
		public void ShouldParse()
		{
			var m = "$GPGNS,003000.00,4253.65208,N,07852.11903,W,DA,14,0.76,253.0,-35.4,,0000*46";
			var n = new GnsMessage();

			n.Parse(m);

			Assert.AreEqual(NmeaMessagePrefix.GlobalPositioningSystem, n.Prefix);
			Assert.AreEqual(NmeaMessageType.GnssFixInformation, n.Type);
		}

		#endregion
	}
}