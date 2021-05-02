#region References

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sproto.Nmea.Messages;

#endregion

namespace Sproto.Tests.Nmea.Messages
{
	[TestClass]
	public class TxtMessageTests
	{
		#region Methods

		[TestMethod]
		public void TestMethodParse()
		{
			var m = "$GNTXT,01,01,02,u-blox AG - www.u-blox.com*4E";
			var n = new TxtMessage();

			n.Parse(m);

			Assert.AreEqual(n.Text, "u-blox AG - www.u-blox.com");
		}

		#endregion
	}
}