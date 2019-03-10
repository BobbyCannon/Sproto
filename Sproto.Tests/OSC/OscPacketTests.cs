#region References

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
			var message = new OscMessage("/test");
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
			var message = new OscMessage("/test");
			message.Arguments.Add(float.PositiveInfinity);
			var actual = message.ToByteArray();
			Extensions.AreEqual(expected, actual);

			message = new OscMessage("/test");
			message.Arguments.Add(float.NegativeInfinity);
			actual = message.ToByteArray();
			Extensions.AreEqual(expected, actual);

			message = new OscMessage("/test");
			message.Arguments.Add(double.PositiveInfinity);
			actual = message.ToByteArray();
			Extensions.AreEqual(expected, actual);

			message = new OscMessage("/test");
			message.Arguments.Add(double.NegativeInfinity);
			actual = message.ToByteArray();
			Extensions.AreEqual(expected, actual);
		}

		[TestMethod]
		public void Parse()
		{

		}

		#endregion
	}
}