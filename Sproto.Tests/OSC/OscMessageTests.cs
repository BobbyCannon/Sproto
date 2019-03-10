#region References

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sproto.OSC;

#endregion

namespace OSC.Tests.OSC
{
	[TestClass]
	public class OscMessageTests
	{
		#region Methods

		[TestMethod]
		public void InvalidAddressDelimiter()
		{
			var command = "name";
			var message = (OscError) OscMessage.Parse(command);
			Assert.AreEqual(OscError.Message.InvalidMessageAddress, message.Code);
		}

		[TestMethod]
		public void InvalidAddressEmpty()
		{
			var command = "/";
			var message = (OscError) OscMessage.Parse(command);
			Assert.AreEqual(OscError.Message.InvalidMessageAddress, message.Code);
		}
		
		[TestMethod]
		public void Parse()
		{
			var command = "/name";
			var message = (OscMessage) OscMessage.Parse(command);
			Assert.AreEqual("/name", message.Address);
		}

		#endregion
	}
}