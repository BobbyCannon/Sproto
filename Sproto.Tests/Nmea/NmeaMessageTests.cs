#region References

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sproto.Nmea.Messages;

#endregion

namespace Sproto.Tests.Nmea
{
	[TestClass]
	public class NmeaMessageTests
	{
		#region Methods

		[TestMethod]
		public void TestMethodExtractChecksum()
		{
			var n = new RmcMessage();
			var m = "$GNRMC,143718.00,A,4513.13793,N,01859.19704,E,0.050,,290719,,,A*65";
			var c = n.ExtractChecksum(m);
			Assert.AreEqual("65", c);
		}

		[TestMethod]
		public void TestMethodExtractChecksum_NoStar()
		{
			var n = new RmcMessage();
			var m = "$GNRMC,143718.00,A,4513.13793,N,01859.19704,E,0.050,,290719,,,A";
			var c = n.ExtractChecksum(m);
			Assert.AreEqual(string.Empty, c);
		}

		[TestMethod]
		public void TestMethodParseChecksum()
		{
			var n = new RmcMessage();
			var m = "$GNRMC,143718.00,A,4513.13793,N,01859.19704,E,0.050,,290719,,,A*65";
			n.ParseChecksum(m);
			Assert.AreEqual("65", n.Checksum);
		}

		#endregion
	}
}