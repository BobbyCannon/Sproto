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
			var message = new OscMessage(TestCommand.Command, "John", 20, new DateTime(2000, 01, 15), 5.11f, 164.32);
			var command = new TestCommand();
			command.Load(message);
			command.StartArgumentProcessing();
			Assert.AreEqual("John", command.GetArgumentAsString());
			Assert.AreEqual(20, command.GetArgumentAsInteger());
			Assert.AreEqual(new DateTime(2000, 01, 15), command.GetArgumentAsDateTime());
			Assert.AreEqual(5.11f, command.GetArgumentAsFloat());
			Assert.AreEqual(164.32, command.GetArgumentAsDouble());
		}

		[TestMethod]
		public void ToFromByteArray()
		{
			var command = new TestCommand { Name = "John", Age = 20, BirthDate = new DateTime(2000, 01, 15), Height = 5.11f, Weight = 164.32 };
			var expected = new byte[] { 0x2F, 0x74, 0x65, 0x73, 0x74, 0x00, 0x00, 0x00, 0x2C, 0x73, 0x69, 0x74, 0x66, 0x64, 0x00, 0x00, 0x4A, 0x6F, 0x68, 0x6E, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x14, 0xBC, 0x2A, 0x37, 0x00, 0x00, 0x00, 0x00, 0x00, 0x40, 0xA3, 0x85, 0x1F, 0x40, 0x64, 0x8A, 0x3D, 0x70, 0xA3, 0xD7, 0x0A };
			var actual = command.ToMessage().ToByteArray();
			Extensions.AreEqual(expected, actual);

			var actualMessage = OscMessage.Parse(actual) as OscMessage;
			Assert.IsNotNull(actualMessage, "Failed to parse the byte data.");

			var actualCommand = new TestCommand();
			actualCommand.Load(actualMessage);

			Assert.AreEqual(command.Name, actualCommand.Name);
			Assert.AreEqual(command.Age, actualCommand.Age);
			Assert.AreEqual(command.BirthDate, actualCommand.BirthDate);
			Assert.AreEqual(command.Height, actualCommand.Height);
			Assert.AreEqual(command.Weight, actualCommand.Weight);
		}

		[TestMethod]
		public void ToFromString()
		{
			var command = new TestCommand { Name = "John", Age = 20, BirthDate = new DateTime(2000, 01, 15), Height = 5.11f, Weight = 164.32 };
			var actual = command.ToString();
			Assert.AreEqual("/test,\"John\",20,{ Time: 2000-01-15T00:00:00.0000Z },5.11f,164.32d", actual);

			var actualMessage = OscMessage.Parse(actual) as OscMessage;
			Assert.IsNotNull(actualMessage);

			var actualCommand = new TestCommand();
			actualCommand.Load(actualMessage);

			Assert.AreEqual(command.Name, actualCommand.Name);
			Assert.AreEqual(command.Age, actualCommand.Age);
			Assert.AreEqual(command.BirthDate, actualCommand.BirthDate);
			Assert.AreEqual(command.Height, actualCommand.Height);
			Assert.AreEqual(command.Weight, actualCommand.Weight);
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

			public double Weight { get; set; }

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
			}

			protected override void UpdateMessage()
			{
				OscMessage = new OscMessage(Command, Name, Age, BirthDate, Height, Weight);
			}

			#endregion
		}

		#endregion
	}
}