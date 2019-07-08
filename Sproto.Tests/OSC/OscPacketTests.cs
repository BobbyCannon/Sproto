#region References

using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sproto.OSC;

#endregion

namespace OSC.Tests.OSC
{
	[TestClass]
	public class OscPacketTests
	{
		#region Methods

		[TestMethod]
		public void FromBytes()
		{
			var data = new byte[] { 0x2F, 0x74, 0x65, 0x73, 0x74, 0x00, 0x00, 0x00, 0x2C, 0x54, 0x00, 0x00 };
			var actual = (OscMessage) OscPacket.GetPacket(data, data.Length);

			Assert.AreEqual("/test", actual.Address);
			Assert.AreEqual(1, actual.Arguments.Count);
			Assert.AreEqual(true, actual.Arguments[0]);
		}

		[TestMethod]
		public void FromBytesOfDoubleNegativeInfinity()
		{
			var data = new byte[] { 0x2F, 0x74, 0x65, 0x73, 0x74, 0x00, 0x00, 0x00, 0x2C, 0x49, 0x00, 0x00 };
			var actual = (OscMessage) OscPacket.GetPacket(data, data.Length);

			Assert.AreEqual("/test", actual.Address);
			Assert.AreEqual(1, actual.Arguments.Count);
			Assert.AreEqual(double.PositiveInfinity, actual.Arguments[0]);
		}

		[TestMethod]
		public void FromBytesOfFloatNegativeInfinity()
		{
			var data = new byte[] { 0x2F, 0x74, 0x65, 0x73, 0x74, 0x00, 0x00, 0x00, 0x2C, 0x49, 0x00, 0x00 };
			var actual = (OscMessage) OscPacket.GetPacket(data, data.Length);

			Assert.AreEqual("/test", actual.Address);
			Assert.AreEqual(1, actual.Arguments.Count);
			Assert.AreEqual(double.PositiveInfinity, actual.Arguments[0]);
		}

		[TestMethod]
		public void GetBytes()
		{
			var message = new OscMessage(OscTimeTag.UtcNow, "/test");
			message.Arguments.Add(true);
			message.Arguments.Add(123);

			var actual = message.ToByteArray();
			var expected = new byte[] { 0x2F, 0x74, 0x65, 0x73, 0x74, 0x00, 0x00, 0x00, 0x2C, 0x54, 0x69, 0x00, 0x00, 0x00, 0x00, 0x7B };

			actual.Dump();

			Extensions.AreEqual(expected, actual);
		}

		[TestMethod]
		public void GetBytesForAllInfinity()
		{
			var expected = new byte[] { 0x2F, 0x74, 0x65, 0x73, 0x74, 0x00, 0x00, 0x00, 0x2C, 0x49, 0x00, 0x00 };
			var message = new OscMessage(OscTimeTag.UtcNow, "/test");
			message.Arguments.Add(float.PositiveInfinity);
			var actual = message.ToByteArray();
			Extensions.AreEqual(expected, actual);

			message = new OscMessage(OscTimeTag.UtcNow, "/test");
			message.Arguments.Add(float.NegativeInfinity);
			actual = message.ToByteArray();
			Extensions.AreEqual(expected, actual);

			message = new OscMessage(OscTimeTag.UtcNow, "/test");
			message.Arguments.Add(double.PositiveInfinity);
			actual = message.ToByteArray();
			Extensions.AreEqual(expected, actual);

			message = new OscMessage(OscTimeTag.UtcNow, "/test");
			message.Arguments.Add(double.NegativeInfinity);
			actual = message.ToByteArray();
			Extensions.AreEqual(expected, actual);
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
				var message = (OscMessage) OscPacket.Parse(command);
				Assert.AreEqual("/system/time", message.Address);
				Assert.AreEqual(1, message.Arguments.Count);
				Assert.AreEqual(e.Value, ((OscTimeTag) message.Arguments[0]).Value);
			}
		}

		#endregion
	}
}