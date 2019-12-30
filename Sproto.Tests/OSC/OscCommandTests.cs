#region References

using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sproto;
using Sproto.OSC;

#endregion

namespace OSC.Tests.OSC
{
	[TestClass]
	public class OscCommandTests
	{
		#region Methods

		[TestMethod]
		public void GetArgumentWithAllTypes()
		{
		}

		[TestMethod]
		public void SequentialProcessingOfArguments()
		{
			var message = new OscMessage(TestCommand.Command, 1, (ulong) 23, "John", 20, new DateTime(2000, 01, 15), 5.11f, 164.32,
				(byte) 4, new byte[] { 0, 1, 1, 2, 3, 5, 8, 13 }, true, Guid.Parse("E3966202-40FA-443D-B21F-E1528A1E6DFE"),
				uint.MaxValue, long.MaxValue
			);

			var command = new TestCommand();
			command.Load(message);
			command.StartArgumentProcessing();
			Assert.AreEqual(1, command.GetArgumentAsInteger());
			Assert.AreEqual((ulong) 23, command.GetArgumentAsUnsignedLong());
			Assert.AreEqual("John", command.GetArgumentAsString());
			Assert.AreEqual(20, command.GetArgumentAsInteger());
			Assert.AreEqual(new DateTime(2000, 01, 15), command.GetArgumentAsDateTime());
			Assert.AreEqual(5.11f, command.GetArgumentAsFloat());
			Assert.AreEqual(164.32, command.GetArgumentAsDouble());
			Assert.AreEqual((byte) 4, command.GetArgumentAsByte());
			Extensions.AreEqual(new byte[] { 0, 1, 1, 2, 3, 5, 8, 13 }, command.GetArgumentAsBlob());
			Assert.AreEqual(true, command.GetArgumentAsBoolean());
			Assert.AreEqual(Guid.Parse("E3966202-40FA-443D-B21F-E1528A1E6DFE"), command.GetArgumentAsGuid());
			Assert.AreEqual(uint.MaxValue, command.GetArgumentAsUnsignedInteger());
			Assert.AreEqual(long.MaxValue, command.GetArgumentAsLong());
		}

		[TestMethod]
		public void ToFromByteArray()
		{
			var command = GetTestCommand();
			var expected = new byte[] { 0x2F, 0x74, 0x65, 0x73, 0x74, 0x00, 0x00, 0x00, 0x2C, 0x69, 0x48, 0x73, 0x69, 0x74, 0x66, 0x64, 0x63, 0x62, 0x54, 0x73, 0x75, 0x68, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x17, 0x4A, 0x6F, 0x68, 0x6E, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x14, 0xBC, 0x2A, 0x37, 0x00, 0x00, 0x00, 0x00, 0x00, 0x40, 0xA3, 0x85, 0x1F, 0x40, 0x64, 0x8A, 0x3D, 0x70, 0xA3, 0xD7, 0x0A, 0x00, 0x00, 0x00, 0x04, 0x00, 0x00, 0x00, 0x08, 0x00, 0x01, 0x01, 0x02, 0x03, 0x05, 0x08, 0x0D, 0x65, 0x33, 0x39, 0x36, 0x36, 0x32, 0x30, 0x32, 0x2D, 0x34, 0x30, 0x66, 0x61, 0x2D, 0x34, 0x34, 0x33, 0x64, 0x2D, 0x62, 0x32, 0x31, 0x66, 0x2D, 0x65, 0x31, 0x35, 0x32, 0x38, 0x61, 0x31, 0x65, 0x36, 0x64, 0x66, 0x65, 0x00, 0x00, 0x00, 0x00, 0xFF, 0xFF, 0xFF, 0xFF, 0x7F, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };
			var actual = command.ToMessage().ToByteArray();
			actual.Dump();
			Extensions.AreEqual(expected, actual);

			var actualMessage = OscPacket.Parse(command.Time, actual) as OscMessage;
			Assert.IsNotNull(actualMessage, "Failed to parse the byte data.");

			var actualCommand = new TestCommand();
			actualCommand.Load(actualMessage);

			Extensions.AreEqual(command, actualCommand, membersToIgnore: new[] { nameof(OscCommand.HasBeenRead) });
		}

		[TestMethod]
		public void ToFromString()
		{
			var command = GetTestCommand();
			var actual = command.ToString();
			actual.Escape().Dump();
			Assert.AreEqual("/test,0,23U,\"John\",20,{ Time: 2000-01-15T00:00:00.0000Z },5.11f,164.32d,\'\u0004\',{ Blob: 0x000101020305080D },True,\"e3966202-40fa-443d-b21f-e1528a1e6dfe\",4294967295u,9223372036854775807L", actual);

			var actualMessage = OscPacket.Parse(command.Time, actual) as OscMessage;
			Assert.IsNotNull(actualMessage);

			var actualCommand = new TestCommand();
			actualCommand.Load(actualMessage);

			Extensions.AreEqual(command, actualCommand, membersToIgnore: new[] { nameof(OscCommand.HasBeenRead) });
		}

		[TestMethod]
		public void ToStringShouldUpdate()
		{
			var command = GetTestCommand();
			var expectedTime = command.Time;

			command.Version = 1;
			command.Name = "Johnny";
			command.Age = 21;

			var expected = "/test,1,23U,\"Johnny\",21,{ Time: 2000-01-15T00:00:00.0000Z },5.11f,164.32d,'',{ Blob: 0x000101020305080D },True,\"e3966202-40fa-443d-b21f-e1528a1e6dfe\",4294967295u,9223372036854775807L";
			var actual = command.ToString();
			actual.Dump();
			Assert.AreEqual(expectedTime, command.Time);
			Assert.AreEqual(expected, actual);
			
			command = GetTestCommand();
			command.Version = 2;
			command.Name = "Johnny";
			command.Age = 21;

			expected = "/test,2,23U,\"Johnny\",21,{ Time: 2000-01-15T00:00:00.0000Z },5.11f,164.32d,'',{ Blob: 0x000101020305080D },True,\"e3966202-40fa-443d-b21f-e1528a1e6dfe\",4294967295u,9223372036854775807L";
			actual = command.ToString();
			actual.Dump();
			Assert.AreEqual(expectedTime, command.Time);
			Assert.AreEqual(expected, actual);

			expected = "/test";
			actual = command.ToMessage(false).ToString();
			Assert.AreEqual(expectedTime, command.Time);
			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void ToMessageTimeShouldBeTimeOfInstantiation()
		{
			var command = GetTestCommand();
			var expected = command.Time;
			Thread.Sleep(100);
			var actual = command.ToMessage().Time;
			expected.Dump();
			actual.Dump();
			Assert.AreEqual(expected, actual);
		}

		private static TestCommand GetTestCommand()
		{
			return new TestCommand
			{
				Name = "John",
				Age = 20,
				BirthDate = new DateTime(2000, 01, 15),
				Enable = true,
				Height = 5.11f,
				Id = 23,
				SyncId = Guid.Parse("E3966202-40FA-443D-B21F-E1528A1E6DFE"),
				Time = new OscTimeTag(new DateTime(2000, 01, 15, 11, 15, 56, DateTimeKind.Utc)),
				Rating = 4,
				Values = new byte[] { 0, 1, 1, 2, 3, 5, 8, 13 },
				Visits = uint.MaxValue,
				VoteId = long.MaxValue,
				Weight = 164.32,
			};
		}

		#endregion

		#region Classes

		public class TestCommand : OscCommand
		{
			#region Constants

			public const string Command = "/test";

			#endregion

			#region Constructors

			public TestCommand() : base(Command)
			{
			}

			#endregion

			#region Properties

			public int Age { get; set; }

			public DateTime BirthDate { get; set; }

			public bool Enable { get; set; }

			public float Height { get; set; }
			
			public int Version { get;  set; }

			public ulong Id { get; set; }

			public string Name { get; set; }

			public byte Rating { get; set; }

			public Guid SyncId { get; set; }

			public byte[] Values { get; set; }

			public uint Visits { get; set; }

			public long VoteId { get; set; }

			public double Weight { get; set; }
			
			#endregion

			#region Methods

			protected override void LoadMessage()
			{
				StartArgumentProcessing();

				Version = GetArgument<int>();
				Id = GetArgument<ulong>();
				Name = GetArgument<string>();
				Age = GetArgument<int>();
				BirthDate = GetArgument<DateTime>();
				Height = GetArgument<float>();
				Weight = GetArgument<double>();
				Rating = GetArgument<byte>();
				Values = GetArgument<byte[]>();
				Enable = GetArgument<bool>();
				SyncId = GetArgument<Guid>();
				Visits = GetArgument<uint>();
				VoteId = GetArgument<long>();
			}

			protected override void UpdateMessage()
			{
				switch (Version)
				{
					case 1:
						OscMessage = new OscMessage(Time, Command, Version, Id, Name, Age, BirthDate, Height, Weight, Rating, Values, Enable, SyncId, Visits, VoteId);
						break;
					
					default:
						SetArguments(Version, Id, Name, Age, BirthDate, Height, Weight, Rating, Values, Enable, SyncId, Visits, VoteId);
						break;
				}
			}

			#endregion
		}

		#endregion
	}
}