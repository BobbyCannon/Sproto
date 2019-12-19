#region References

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sproto.OSC;

#endregion

namespace OSC.Tests.OSC
{
	[TestClass]
	public class OscCommandTests
	{
		#region Methods

		[TestMethod]
		public void SequentialProcessingOfArguments()
		{
			var message = new OscMessage(TestCommand.Command, "John", 20, new DateTime(2000, 01, 15), 5.11f, 164.32, 4, new byte[] { 0, 1, 1, 2, 3, 5, 8, 13 });
			var command = new TestCommand();
			command.Load(message);
			command.StartArgumentProcessing();
			Assert.AreEqual("John", command.GetArgumentAsString());
			Assert.AreEqual(20, command.GetArgumentAsInteger());
			Assert.AreEqual(new DateTime(2000, 01, 15), command.GetArgumentAsDateTime());
			Assert.AreEqual(5.11f, command.GetArgumentAsFloat());
			Assert.AreEqual(164.32, command.GetArgumentAsDouble());
			Assert.AreEqual((byte) 4, command.GetArgumentAsByte());
			Extensions.AreEqual(new byte[] { 0, 1, 1, 2, 3, 5, 8, 13 }, command.GetArgumentAsBlob());
		}

		[TestMethod]
		public void ToFromByteArray()
		{
			var command = GetTestCommand();
			var expected = new byte[] { 0x2F, 0x74, 0x65, 0x73, 0x74, 0x00, 0x00, 0x00, 0x2C, 0x73, 0x69, 0x74, 0x66, 0x64, 0x63, 0x62, 0x00, 0x00, 0x00, 0x00, 0x4A, 0x6F, 0x68, 0x6E, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x14, 0xBC, 0x2A, 0x37, 0x00, 0x00, 0x00, 0x00, 0x00, 0x40, 0xA3, 0x85, 0x1F, 0x40, 0x64, 0x8A, 0x3D, 0x70, 0xA3, 0xD7, 0x0A, 0x00, 0x00, 0x00, 0x04, 0x00, 0x00, 0x00, 0x07, 0x00, 0x01, 0x01, 0x03, 0x05, 0x08, 0x0D, 0x00 };
			var actual = command.ToMessage().ToByteArray();
			actual.Dump();
			Extensions.AreEqual(expected, actual);

			var actualMessage = OscMessage.Parse(actual) as OscMessage;
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
			Assert.AreEqual("/test,\"John\",20,{ Time: 2000-01-15T00:00:00.0000Z },5.11f,164.32d,'',{ Blob: 0x0001010305080D }", actual);

			var actualMessage = OscMessage.Parse(actual) as OscMessage;
			Assert.IsNotNull(actualMessage);

			var actualCommand = new TestCommand();
			actualCommand.Load(actualMessage);

			Extensions.AreEqual(command, actualCommand, membersToIgnore: new[] { nameof(OscCommand.HasBeenRead) });
		}

		private static TestCommand GetTestCommand()
		{
			return new TestCommand { Name = "John", Age = 20, BirthDate = new DateTime(2000, 01, 15), Height = 5.11f, Weight = 164.32, Rating = 4, Values = new byte[] { 0, 1, 1, 3, 5, 8, 13 } };
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

			public float Height { get; set; }

			public string Name { get; set; }

			public byte Rating { get; set; }

			public double Weight { get; set; }

			public byte[] Values { get; set; }

			#endregion

			#region Methods

			protected override void LoadMessage()
			{
				StartArgumentProcessing();
				Name = GetArgumentAsString();
				Age = GetArgumentAsInteger();
				BirthDate = GetArgumentAsDateTime();
				Height = GetArgumentAsFloat();
				Weight = GetArgumentAsDouble();
				Rating = GetArgumentAsByte();
				Values = GetArgumentAsBlob();
			}

			protected override void UpdateMessage()
			{
				OscMessage = new OscMessage(Command, Name, Age, BirthDate, Height, Weight, Rating, Values);
			}

			#endregion
		}

		#endregion
	}
}