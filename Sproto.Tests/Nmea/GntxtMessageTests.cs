#region References

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sproto.Nmea;

#endregion

namespace Sproto.Tests.Nmea
{
	[TestClass]
	public class GntxtMessageTests
	{
		#region Methods

		[TestMethod]
		public void TestMethodParse()
		{
			var m = "$GNTXT,01,01,02,u-blox AG - www.u-blox.com*4E";
			var n = new GntxtMessage();

			n.Parse(m);

			Assert.AreEqual(n.Text, "u-blox AG - www.u-blox.com");
		}

		#endregion
	}
}