#region References

using System.Collections.Generic;
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

		[TestMethod]
		public void ParseTime()
		{
			var values = new Dictionary<string, OscTimeTag>
			{
				{ "2019-04-05T00:00:59.1234Z", OscTimeTag.Parse("2019-04-05T00:00:59.1234Z") },
				{ "2019-04-05T00:00:59Z", OscTimeTag.Parse("2019-04-05T00:00:59Z") },
				{ "2019-04-05", OscTimeTag.Parse("2019-04-05") }
			};

			foreach (var e in values)
			{
				var command = $"/system/time, {{ Time: {e.Key} }}";
				var message = (OscMessage) OscMessage.Parse(command);
				Assert.AreEqual("/system/time", message.Address);
				Assert.AreEqual(1, message.Arguments.Count);
				Assert.AreEqual(e.Value, ((OscTimeTag) message.Arguments[0]).Value);
			}
		}

		#endregion
	}
}