#region References

using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Sproto;
using Sproto.OSC;

#endregion

namespace OSC.Tests.OSC
{
	[TestClass]
	public class OscMessageTests
	{
		#region Methods

		[TestMethod]
		public void AddressEmpty()
		{
			var command = "/";
			var message = (OscError) OscMessage.Parse(command);
			Assert.AreEqual(OscError.Message.InvalidMessageAddress, message.Code);
		}

		[TestMethod]
		public void FromBytes()
		{
			var actual = new byte[] { 0x62, 0x75, 0x6E, 0x64, 0x00, 0x79, 0x00, 0x61, 0x00, 0x6C, 0x69, 0x00, 0x00, 0xBB, 0xC0 };
			var slip = new OscSlip();
			var start = 0;
			var actualMessage = (OscError) slip.ProcessBytes(actual, ref start, actual.Length);
			actualMessage.Code.Dump();
			actualMessage.Description.Dump();
		}

		[TestMethod]
		public void MissingAddressPrefix()
		{
			var command = "name";
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

		[TestMethod]
		public void ParseWithStringInnerQuotes()
		{
			var data = "/update,\"This is \\\"a quote\\\". -John\"";
			var expected = new OscMessage("/update", "This is \"a quote\". -John");
			var actual = (OscMessage) OscPacket.Parse(data);

			Assert.AreEqual(expected.Address, actual.Address);
			Assert.AreEqual(expected.Arguments.Count, actual.Arguments.Count);
			Assert.AreEqual(expected.Arguments[0], actual.Arguments[0]);
		}

		[TestMethod]
		public void ToBytes()
		{
			var message = OscMessage.Parse("/ahoy,\"Flare Lite\"");
			var actual = OscSlip.EncodePacket(message);
			actual.Dump();

			var slip = new OscSlip();
			var start = 0;
			var actualMessage = (OscMessage) slip.ProcessBytes(actual, ref start, actual.Length);
			actualMessage.Address.Dump();
			actualMessage.Arguments[0].Dump();
		}

		[TestMethod]
		public void ToStringWithStringContainingSpecialCharacters()
		{
			var itemsToTest = new Dictionary<string, string>
			{
				{ "This is \"a quote\". -John", "/update,\"This is \\\"a quote\\\". -John\"" },
				{ "\0", "/update,\"\\0\"" },
				{ "\a", "/update,\"\\a\"" },
				{ "\b", "/update,\"\\b\"" },
				{ "\f", "/update,\"\\f\"" },
				{ "\n", "/update,\"\\n\"" },
				{ "\r", "/update,\"\\r\"" },
				{ "\t", "/update,\"\\t\"" },
				{ "\v", "/update,\"\\v\"" },
				{ "\\", "/update,\"\\\\\"" }
			};

			foreach (var item in itemsToTest)
			{
				var message = new OscMessage("/update", item.Key); 
				var actual = message.ToString();
				Assert.AreEqual(item.Value, actual);

				var actualMessage = OscPacket.Parse(item.Value) as OscMessage;
				Assert.IsNotNull(actualMessage);
				Assert.AreEqual(1, actualMessage.Arguments.Count);
				Assert.AreEqual(item.Key, actualMessage.Arguments[0]);
			}
		}

		[TestMethod]
		public void TestWithJsonSerializedObject()
		{
			var time = OscTimeTag.Now;
			var json = JsonConvert.SerializeObject(time);
			var message = new OscMessage("/object", json);
			var actual = message.ToString();
			actual.Escape().Dump();

			var actualMessage = OscPacket.Parse(actual) as OscMessage;
			Assert.IsNotNull(actualMessage);
			actualMessage.Arguments[0].Dump();

			var actualTime = JsonConvert.DeserializeObject<OscTimeTag>(actualMessage.GetArgument<string>(0));
			Assert.AreEqual(time, actualTime);
		}

		#endregion
	}
}