#region References

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sproto.Nmea;

#endregion

namespace Sproto.Tests.Nmea
{
	[TestClass]
	public class GpgsvMessageTests

	{
		#region Methods

		[TestMethod]
		public void TestMethodParse()
		{
			var m = "$GPGSV,3,1,10,01,50,304,26,03,24,245,16,08,56,204,28,10,21,059,20*77";
			var n = new GpgsvMessage();

			n.Parse(m);

			Assert.AreEqual(3, n.NumberOfSentences);
			Assert.AreEqual(1, n.SentenceNr);
			Assert.AreEqual(10, n.NumberOfSatellitesInView);
			Assert.AreEqual(n.Satellites.Count, 4);
		}

		#endregion
	}
}