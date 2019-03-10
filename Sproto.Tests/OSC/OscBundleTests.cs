﻿#region References

using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sproto.OSC;

#endregion

namespace OSC.Tests.OSC
{
	[TestClass]
	public class OscBundleTests
	{
		#region Methods

		[TestMethod]
		public void ParseBundle()
		{
			var time = new OscTimeTag(new DateTime(2019, 1, 20, 08, 50, 12, DateTimeKind.Local));
			var expected = new OscBundle(time, new OscMessage("/message", 123, "foo", true, null), new OscMessage("/delay", 321));
			var data = new byte[] { 0x23, 0x62, 0x75, 0x6E, 0x64, 0x6C, 0x65, 0x00, 0xDF, 0xEE, 0xB4, 0xC4, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x1C, 0x2F, 0x6D, 0x65, 0x73, 0x73, 0x61, 0x67, 0x65, 0x00, 0x00, 0x00, 0x00, 0x2C, 0x69, 0x73, 0x54, 0x4E, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x7B, 0x66, 0x6F, 0x6F, 0x00, 0x00, 0x00, 0x00, 0x10, 0x2F, 0x64, 0x65, 0x6C, 0x61, 0x79, 0x00, 0x00, 0x2C, 0x69, 0x00, 0x00, 0x00, 0x00, 0x01, 0x41 };
			var actual = OscPacket.GetPacket(data, data.Length);
			Extensions.AreEqual(expected, actual, false, null, nameof(OscTimeTag.Now), nameof(OscTimeTag.UtcNow));
		}

		[TestMethod]
		public void ParseExtendedBundle()
		{
			var time = new OscTimeTag(new DateTime(2019, 1, 20, 08, 50, 12, DateTimeKind.Local));
			var expected = new OscBundle(time.Value, new OscMessage("/message", 123, "bar", true, null), new OscMessage("/delay", 321)) { IsExtended = true };
			var data = new byte[] { 0x2B, 0x62, 0x75, 0x6E, 0x64, 0x6C, 0x65, 0x00, 0xDF, 0xEE, 0xB4, 0xC4, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x1C, 0x2F, 0x6D, 0x65, 0x73, 0x73, 0x61, 0x67, 0x65, 0x00, 0x00, 0x00, 0x00, 0x2C, 0x69, 0x73, 0x54, 0x4E, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x7B, 0x62, 0x61, 0x72, 0x00, 0x00, 0x00, 0x00, 0x10, 0x2F, 0x64, 0x65, 0x6C, 0x61, 0x79, 0x00, 0x00, 0x2C, 0x69, 0x00, 0x00, 0x00, 0x00, 0x01, 0x41, 0x45, 0x9C, 0x00, 0x00 };
			var actual = OscPacket.GetPacket(data, data.Length);
			Extensions.AreEqual(expected, actual, false, null, nameof(OscTimeTag.Now), nameof(OscTimeTag.UtcNow));
		}

		[TestMethod]
		public void ParseExtendedBundleShouldFailCrcCheck()
		{
			var random = new Random();
			var data = new byte[]
			{
				0x2B, 0x62, 0x75, 0x6E, 0x64, 0x6C, 0x65, 0x00, 0xDF, 0xEE, 
				0xB4, 0xC4, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x1C, 
				0x2F, 0x6D, 0x65, 0x73, 0x73, 0x61, 0x67, 0x65, 0x00, 0x00,
				0x00, 0x00, 0x2C, 0x69, 0x73, 0x54, 0x4E, 0x00, 0x00, 0x00,
				0x00, 0x00, 0x00, 0x7B, 0x62, 0x61, 0x72, 0x00, 0x00, 0x00,
				0x00, 0x10, 0x2F, 0x64, 0x65, 0x6C, 0x61, 0x79, 0x00, 0x00,
				0x2C, 0x69, 0x00, 0x00, 0x00, 0x00, 0x01, 0x41, 0x45, 0x9C,
				0x00, 0x00
			};

			var actual = OscPacket.GetPacket(data, data.Length);
			Assert.IsNotNull(actual);

			var randomOffset = random.Next(1, data.Length - 2);
			var randomBit = 1 << random.Next(0, 8);
			data[randomOffset].ToString("X2").Dump();
			data[randomOffset] = (byte) ((data[randomOffset] & randomBit) == randomBit ? data[randomOffset] ^ randomBit : data[randomOffset] | randomBit);
			var error = (OscError) OscPacket.GetPacket(data, data.Length);
			randomOffset.Dump();
			randomBit.Dump();
			data[randomOffset].ToString("X2").Dump();

			var possibleErrors = new[]
			{
				OscError.Message.InvalidBundle,
				OscError.Message.InvalidBundleCrc,
				OscError.Message.InvalidParsedMessage,
				OscError.Message.InvalidMessageAddressMisAligned,
				OscError.Message.UnknownTagType,
			};

			Assert.IsTrue(possibleErrors.Any(x => x.Equals(error.Code)), error.Code.ToString());
		}

		[TestMethod]
		public void ToBytes()
		{
			var time = new OscTimeTag(new DateTime(2019, 1, 20, 08, 50, 12, DateTimeKind.Local));
			var bundle = new OscBundle(time.Value, new OscMessage("/message", 123, "foo", true, null), new OscMessage("/delay", 321));
			var expected = new byte[] { 0x23, 0x62, 0x75, 0x6E, 0x64, 0x6C, 0x65, 0x00, 0xDF, 0xEE, 0xB4, 0xC4, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x1C, 0x2F, 0x6D, 0x65, 0x73, 0x73, 0x61, 0x67, 0x65, 0x00, 0x00, 0x00, 0x00, 0x2C, 0x69, 0x73, 0x54, 0x4E, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x7B, 0x66, 0x6F, 0x6F, 0x00, 0x00, 0x00, 0x00, 0x10, 0x2F, 0x64, 0x65, 0x6C, 0x61, 0x79, 0x00, 0x00, 0x2C, 0x69, 0x00, 0x00, 0x00, 0x00, 0x01, 0x41 };
			var actual = bundle.ToByteArray();
			actual.Dump();
			Extensions.AreEqual(expected, actual);
		}

		[TestMethod]
		public void ToBytesForExtendedBundle()
		{
			var time = new OscTimeTag(new DateTime(2019, 1, 20, 08, 50, 12, DateTimeKind.Local));
			var bundle = new OscBundle(time.Value, new OscMessage("/message", 123, "bar", true, null), new OscMessage("/delay", 321)) { IsExtended = true };
			var expected = new byte[] { 0x2B, 0x62, 0x75, 0x6E, 0x64, 0x6C, 0x65, 0x00, 0xDF, 0xEE, 0xB4, 0xC4, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x1C, 0x2F, 0x6D, 0x65, 0x73, 0x73, 0x61, 0x67, 0x65, 0x00, 0x00, 0x00, 0x00, 0x2C, 0x69, 0x73, 0x54, 0x4E, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x7B, 0x62, 0x61, 0x72, 0x00, 0x00, 0x00, 0x00, 0x10, 0x2F, 0x64, 0x65, 0x6C, 0x61, 0x79, 0x00, 0x00, 0x2C, 0x69, 0x00, 0x00, 0x00, 0x00, 0x01, 0x41, 0x45, 0x9C, 0x00, 0x00 };
			var data = bundle.ToByteArray();
			data.Dump();
			Extensions.AreEqual(expected, data);
		}

		#endregion
	}
}