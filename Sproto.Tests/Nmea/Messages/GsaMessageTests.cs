#region References

using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sproto.Nmea.Messages;

#endregion

namespace Sproto.Tests.Nmea.Messages
{
	[TestClass]
	public class GsaMessageTests
	{
		#region Methods

		[TestMethod]
		public void TestMethodParse()
		{
			var m = "$GNGSA,A,3,01,18,32,08,11,,,,,,,,6.16,1.86,5.88*16";
			var n = new GsaMessage();

			n.Parse(m);

			Assert.AreEqual("A", n.AutoSelection);
			Assert.AreEqual("3", n.Fix3D);
			Assert.AreEqual(5, n.PrnsOfSatellitesUsedForFix.Count); //"01,18,32,08,11,,,,,,,");
			Assert.AreEqual(6.16, n.PositionDilutionOfPrecision);
			Assert.AreEqual(1.86, n.HorizontalDilutionOfPrecision);
			Assert.AreEqual(5.88, n.VerticalDilutionOfPrecision);

			m = "$GPGSA,A,1,,,,,,,,,,,,,,,*1E";
			n.Parse(m);

			Assert.AreEqual("A", n.AutoSelection);
			Assert.AreEqual("1", n.Fix3D);
			Assert.AreEqual(0, n.HorizontalDilutionOfPrecision);
			Assert.AreEqual("1E", n.Checksum);
			Assert.AreEqual(0, n.PositionDilutionOfPrecision);
			Extensions.AreEqual(new List<int>(), n.PrnsOfSatellitesUsedForFix);
			Assert.AreEqual(DateTime.MinValue, n.TimestampUtc);
			Assert.AreEqual(0, n.VerticalDilutionOfPrecision);
		}

		#endregion
	}
}