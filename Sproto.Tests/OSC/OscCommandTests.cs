#region References

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
			var command = new TestCommand();
			var actual = command.ToString();
			Assert.AreEqual("/test", actual);
		}

		#endregion

		#region Classes

		public class TestCommand : OscCommand
		{
			#region Constructors

			public TestCommand() : base("/test")
			{
			}

			#endregion

			#region Methods

			protected override void LoadMessage()
			{
			}

			protected override void UpdateMessage()
			{
			}

			#endregion
		}

		#endregion
	}
}