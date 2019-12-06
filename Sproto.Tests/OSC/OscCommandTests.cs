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
		public void ToStringShouldNotException()
		{
			var command = new TestCommand { Name = "John", Age = 20, BirthDate = new DateTime(2000, 01, 15), Height = 5.11f, Weight = 164.32 };
			var actual = command.ToString();
			Assert.AreEqual("/test,\"John\",20,630834912000000000L,5.11f,164.32d", actual);
		}

		#endregion

		#region Classes

		public class TestCommand : OscCommand
		{
			#region Constants

			private const string _command = "/test";

			#endregion

			#region Constructors

			public TestCommand() : base(_command)
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
				Name = GetArgument(0, string.Empty);
				Age = GetArgument(1, 0);
				BirthDate = new DateTime(GetArgument(2, 0L));
				Height = GetArgument(3, 0.0f);
				Weight = GetArgument(4, 0.0);
			}

			protected override void UpdateMessage()
			{
				OscMessage = new OscMessage(_command, Name, Age, BirthDate.Ticks, Height, Weight);
			}

			#endregion
		}

		#endregion
	}
}