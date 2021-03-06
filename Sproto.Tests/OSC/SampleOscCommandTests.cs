﻿#region References

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Speedy;
using Sproto.Osc;
using Sproto.Tests.Samples;

#endregion

namespace Sproto.Tests.Osc
{
	[TestClass]
	public class SampleOscCommandTests : BaseTests
	{
		#region Methods

		[TestMethod]
		public void DateTimeMinMaxTests()
		{
			var originalZone = TimeZoneHelper.GetSystemTimeZone();

			try
			{
				var timeZones = new[] { "Pacific Standard Time", "Central Standard Time", "Eastern Standard Time" };

				foreach (var zone in timeZones)
				{
					zone.Dump();

					TimeZoneHelper.SetSystemTimeZone(zone);

					var command = new SampleOscCommand { BirthDate = DateTime.MinValue, Timestamp = OscTimeTag.MinValue };
					var expected = "/sample,3,null,{ Time: 1900-01-01T00:00:00.000Z },False,{ SampleValue: 0,0,0 },{ Time: 1900-01-01T00:00:00.000Z }";
					var actual = command.ToMessage().ToString();
					Assert.AreEqual(expected, actual);

					var actualMessage = OscPacket.Parse(expected, new OscArgumentParser<SampleCustomValue>()) as OscMessage;
					var actualCommand = OscCommand.FromMessage<SampleOscCommand>(actualMessage);
					Assert.AreEqual(DateTime.MinValue, actualCommand.BirthDate);

					command = new SampleOscCommand { BirthDate = DateTime.MaxValue, Timestamp = OscTimeTag.MaxValue };
					expected = "/sample,3,null,{ Time: 2036-02-07T06:28:16.000Z },False,{ SampleValue: 0,0,0 },{ Time: 2036-02-07T06:28:16.000Z }";
					actual = command.ToMessage().ToString();
					Assert.AreEqual(expected, actual);

					actualMessage = OscPacket.Parse(expected, new OscArgumentParser<SampleCustomValue>()) as OscMessage;
					actualCommand = OscCommand.FromMessage<SampleOscCommand>(actualMessage);
					Assert.AreEqual(DateTime.MaxValue, actualCommand.BirthDate);
				}
			}
			finally
			{
				TimeZoneHelper.SetSystemTimeZone(originalZone);
			}
		}

		[TestMethod]
		public void DateTimeSwitch()
		{
			var command = new SampleOscCommand { BirthDate = new DateTime(2021, 02, 17, 08, 56, 32, 123, DateTimeKind.Local) };
			var expected = "/sample,3,null,{ Time: 2021-02-17T13:56:32.123Z },False,{ SampleValue: 0,0,0 },{ Time: 1900-01-01T00:00:00.000Z }";
			var actual = command.ToString();

			Assert.AreEqual(expected, actual);

			command.Timestamp = command.BirthDate.ToOscTimeTag();
			expected = "/sample,3,null,{ Time: 2021-02-17T13:56:32.123Z },False,{ SampleValue: 0,0,0 },{ Time: 2021-02-17T13:56:32.123Z }";
			actual = command.ToString();

			Assert.AreEqual(expected, actual);

			command = OscCommand.FromMessage<SampleOscCommand>(expected, new OscArgumentParser<SampleCustomValue>());
			Assert.AreEqual(new DateTime(2021, 02, 17, 13, 56, 32, 123, DateTimeKind.Utc), command.BirthDate);
			Assert.AreEqual(new OscTimeTag(new DateTime(2021, 02, 17, 13, 56, 32, 123, DateTimeKind.Utc)), command.Timestamp);

			command.Timestamp = new OscTimeTag(new DateTime(2021, 02, 17, 01, 13, 12, 762, DateTimeKind.Utc));
			expected = "/sample,3,null,{ Time: 2021-02-17T13:56:32.123Z },False,{ SampleValue: 0,0,0 },{ Time: 2021-02-17T01:13:12.762Z }";
			actual = command.ToString();

			Assert.AreEqual(expected, actual);

			command = OscCommand.FromMessage<SampleOscCommand>(expected, new OscArgumentParser<SampleCustomValue>());
			Assert.AreEqual(new DateTime(2021, 02, 17, 13, 56, 32, 123, DateTimeKind.Utc), command.BirthDate);
			Assert.AreEqual(new OscTimeTag(new DateTime(2021, 02, 17, 01, 13, 12, 762, DateTimeKind.Utc)), command.Timestamp);

			command.BirthDate = command.Timestamp.ToDateTime();
			expected = "/sample,3,null,{ Time: 2021-02-17T01:13:12.762Z },False,{ SampleValue: 0,0,0 },{ Time: 2021-02-17T01:13:12.762Z }";
			actual = command.ToString();

			Assert.AreEqual(expected, actual);

			command = OscCommand.FromMessage<SampleOscCommand>(expected, new OscArgumentParser<SampleCustomValue>());
			Assert.AreEqual(new DateTime(2021, 02, 17, 01, 13, 12, 762, DateTimeKind.Utc), command.BirthDate);
			Assert.AreEqual(new OscTimeTag(new DateTime(2021, 02, 17, 01, 13, 12, 762, DateTimeKind.Utc)), command.Timestamp);
		}

		[TestMethod]
		public void DateTimeZoneTest()
		{
			var originalZone = TimeZoneHelper.GetSystemTimeZone();

			try
			{
				TimeZoneHelper.SetSystemTimeZone("Pacific Standard Time");

				var command = new SampleOscCommand
				{
					BirthDate = new DateTime(1970, 01, 02, 0, 0, 0, DateTimeKind.Local),
					Timestamp = new OscTimeTag(new DateTime(1970, 01, 02, 0, 0, 0, DateTimeKind.Local))
				};
				var expected = "/sample,3,null,{ Time: 1970-01-02T08:00:00.000Z },False,{ SampleValue: 0,0,0 },{ Time: 1970-01-02T08:00:00.000Z }";
				var actual = command.ToMessage().ToString();
				Assert.AreEqual(expected, actual);

				TimeZoneHelper.SetSystemTimeZone("Central Standard Time");
				var value = TimeService.Now.IsDaylightSavingTime() ? 3 : 2;

				command = new SampleOscCommand
				{
					BirthDate = new DateTime(1970, 01, 02, value, 0, 0, DateTimeKind.Local),
					Timestamp = new OscTimeTag(new DateTime(1970, 01, 02, value, 0, 0, DateTimeKind.Local))
				};
				actual = command.ToMessage().ToString();
				Assert.AreEqual(expected, actual);

				TimeZoneHelper.SetSystemTimeZone("Eastern Standard Time");
				value = TimeService.Now.IsDaylightSavingTime() ? 4 : 3;

				command = new SampleOscCommand
				{
					BirthDate = new DateTime(1970, 01, 02, value, 0, 0, DateTimeKind.Local),
					Timestamp = new OscTimeTag(new DateTime(1970, 01, 02, value, 0, 0, DateTimeKind.Local))
				};
				actual = command.ToMessage().ToString();
				Assert.AreEqual(expected, actual);
			}
			finally
			{
				TimeZoneHelper.SetSystemTimeZone(originalZone);
			}
		}

		[TestMethod]
		public void DowngradeCommand()
		{
			var data = "/sample,3,\"Bob\",{ Time: 2020-02-14T11:36:15.000Z },True,{ SampleValue: 1,2,3 },{ Time: 2020-02-14T11:36:16.123Z }";
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
		public void ParseWithArgumentParser()
		{
			var data = "/sample,3,\"Bob\",{ Time: 2020-02-14T11:36:15.000Z },True,{ SampleValue: 1,2,3 }";
			var message = OscPacket.Parse(data, new OscArgumentParser<SampleCustomValue>()) as OscMessage;
			Assert.IsNotNull(message);

			var actual = OscCommand.FromMessage<SampleOscCommand>(message);
			Assert.AreEqual(new DateTime(2020, 02, 14, 11, 36, 15, DateTimeKind.Utc), actual.BirthDate);
			Assert.AreEqual(new SampleCustomValue(1, 2, 3), actual.Value);
		}

		[TestMethod]
		public void ParseWithoutArgumentParser()
		{
			var data = "/sample,3,\"Bob\",{ Time: 2020-02-14T11:36:15.000Z },True,{ SampleValue: 1,2,3 }";
			var message = OscPacket.Parse(data) as OscMessage;
			Assert.IsNotNull(message);

			Extensions.ExpectedException<InvalidCastException>(() => OscCommand.FromMessage<SampleOscCommand>(message),
				"Unable to cast object of type 'System.String' to type 'Sproto.Tests.Samples.SampleCustomValue'.");
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
			var command = new SampleOscCommand { Version = 3, Name = "Bob", BirthDate = new DateTime(2020, 02, 14, 11, 36, 15, 321, DateTimeKind.Utc), Enabled = true, Timestamp = new OscTimeTag(new DateTime(2020, 02, 14, 11, 36, 15, 456, DateTimeKind.Utc)), Value = new SampleCustomValue(1, 2, 3) };
			var expected = "/sample,3,\"Bob\",{ Time: 2020-02-14T11:36:15.321Z },True,{ SampleValue: 1,2,3 },{ Time: 2020-02-14T11:36:15.456Z }";
			var actual = command.ToString();
			Assert.AreEqual(expected, actual);

			var expected2 = new byte[] { 0x2F, 0x73, 0x61, 0x6D, 0x70, 0x6C, 0x65, 0x00, 0x2C, 0x69, 0x73, 0x74, 0x54, 0x61, 0x74, 0x00, 0x00, 0x00, 0x00, 0x03, 0x42, 0x6F, 0x62, 0x00, 0xE1, 0xF1, 0x04, 0xAF, 0x52, 0x2D, 0x0E, 0x55, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x02, 0x00, 0x00, 0x00, 0x03, 0xE1, 0xF1, 0x04, 0xAF, 0x74, 0xBC, 0x6A, 0x7E };
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