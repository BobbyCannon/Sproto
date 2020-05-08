#region References

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OSC.Tests.Samples;
using Sproto.OSC;

#endregion

namespace OSC.Tests.OSC
{
	[TestClass]
	public class SampleOscCommandTests
	{
		#region Methods

		[TestMethod]
		public void DowngradeCommand()
		{
			var data = "/sample,3,\"Bob\",{ Time: 2020-02-14T11:36:15.000Z },True,{ SampleValue: 1,2,3 }";
			var parser = new OscArgumentParser<SampleCustomValue>();
			var message = OscPacket.Parse(data, parser) as OscMessage;
			Assert.IsNotNull(message);

			var command = OscCommand.FromMessage<SampleOscCommand>(message);
			command.Version = 2;
			var expected = "/sample,2,\"Bob\",{ Time: 2020-02-14T11:36:15.000Z },True";
			var actual = command.ToString();
			Assert.AreEqual(expected, actual);

			command = OscCommand.FromMessage<SampleOscCommand>(message);
			command.Version = 1;
			expected = "/sample,1,\"Bob\"";
			actual = command.ToString();
			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void ParseWithoutArgumentParser()
		{
			var data = "/sample,3,\"Bob\",{ Time: 2020-02-14T11:36:15.000Z },True,{ SampleValue: 1,2,3 }";
			var message = OscPacket.Parse(data) as OscMessage;
			Assert.IsNotNull(message);

			Extensions.ExpectedException<InvalidCastException>(() => OscCommand.FromMessage<SampleOscCommand>(message), "Specified cast is not valid");
		}

		[TestMethod]
		public void Version1()
		{
			var command = new SampleOscCommand { Version = 1, Name = "Bob" };
			var expected = "/sample,1,\"Bob\"";
			var actual = command.ToString();
			Assert.AreEqual(expected, actual);

			var expected2 = new byte[] { 0x2F, 0x73, 0x61, 0x6D, 0x70, 0x6C, 0x65, 0x00, 0x2C, 0x69, 0x73, 0x00, 0x00, 0x00, 0x00, 0x01, 0x42, 0x6F, 0x62, 0x00 };
			var actual2 = command.ToMessage().ToByteArray();
			actual2.Dump();
			Extensions.AreEqual(expected2, actual2);
			Assert.AreEqual(0, actual2.Length % 4);

			var actualCommand = OscCommand.FromMessage<SampleOscCommand>((OscMessage) OscPacket.Parse(expected));
			Extensions.AreEqual(command, actualCommand, false, null, nameof(SampleOscCommand.Time), nameof(OscCommand.HasBeenRead));
			actualCommand = OscCommand.FromMessage<SampleOscCommand>((OscMessage) OscPacket.Parse(expected2));
			Extensions.AreEqual(command, actualCommand, false, null, nameof(SampleOscCommand.Time), nameof(OscCommand.HasBeenRead));
		}

		[TestMethod]
		public void Version2()
		{
			var command = new SampleOscCommand { Version = 2, Name = "Bob", BirthDate = new DateTime(2020, 02, 14, 11, 36, 15, DateTimeKind.Utc), Enabled = true };
			var expected = "/sample,2,\"Bob\",{ Time: 2020-02-14T11:36:15.000Z },True";
			var actual = command.ToString();
			Assert.AreEqual(expected, actual);

			var expected2 = new byte[] { 0x2F, 0x73, 0x61, 0x6D, 0x70, 0x6C, 0x65, 0x00, 0x2C, 0x69, 0x73, 0x74, 0x54, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x02, 0x42, 0x6F, 0x62, 0x00, 0xE1, 0xF1, 0x04, 0xAF, 0x00, 0x00, 0x00, 0x00 };
			var actual2 = command.ToMessage().ToByteArray();
			actual2.Dump();
			Extensions.AreEqual(expected2, actual2);
			Assert.AreEqual(0, actual2.Length % 4);

			var actualCommand = OscCommand.FromMessage<SampleOscCommand>((OscMessage) OscPacket.Parse(expected));
			Extensions.AreEqual(command, actualCommand, false, null, nameof(SampleOscCommand.Time), nameof(OscCommand.HasBeenRead));
			actualCommand = OscCommand.FromMessage<SampleOscCommand>((OscMessage) OscPacket.Parse(expected2));
			Extensions.AreEqual(command, actualCommand, false, null, nameof(SampleOscCommand.Time), nameof(OscCommand.HasBeenRead));
		}

		[TestMethod]
		public void Version3()
		{
			var command = new SampleOscCommand { Version = 3, Name = "Bob", BirthDate = new DateTime(2020, 02, 14, 11, 36, 15, DateTimeKind.Utc), Enabled = true, Value = new SampleCustomValue(1, 2, 3) };
			var expected = "/sample,3,\"Bob\",{ Time: 2020-02-14T11:36:15.000Z },True,{ SampleValue: 1,2,3 }";
			var actual = command.ToString();
			Assert.AreEqual(expected, actual);

			var expected2 = new byte[] { 0x2F, 0x73, 0x61, 0x6D, 0x70, 0x6C, 0x65, 0x00, 0x2C, 0x69, 0x73, 0x74, 0x54, 0x61, 0x00, 0x00, 0x00, 0x00, 0x00, 0x03, 0x42, 0x6F, 0x62, 0x00, 0xE1, 0xF1, 0x04, 0xAF, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x02, 0x00, 0x00, 0x00, 0x03 };
			var actual2 = command.ToMessage().ToByteArray();
			actual2.Dump();
			Extensions.AreEqual(expected2, actual2);
			Assert.AreEqual(0, actual2.Length % 4);

			var parser = new OscArgumentParser<SampleCustomValue>();
			var actualCommand = OscCommand.FromMessage<SampleOscCommand>((OscMessage) OscPacket.Parse(expected, parser));
			Extensions.AreEqual(command, actualCommand, false, null, nameof(SampleOscCommand.Time), nameof(OscCommand.HasBeenRead));
			actualCommand = OscCommand.FromMessage<SampleOscCommand>((OscMessage) OscPacket.Parse(expected2, parser));
			Extensions.AreEqual(command, actualCommand, false, null, nameof(SampleOscCommand.Time), nameof(OscCommand.HasBeenRead));
		}

		#endregion
	}
}