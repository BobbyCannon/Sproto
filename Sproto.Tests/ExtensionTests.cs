#region References

using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sproto;
using Sproto.OSC;

#endregion

namespace OSC.Tests
{
	[TestClass]
	public class ExtensionTests
	{
		#region Methods

		[TestMethod]
		public void CalculateCrc16()
		{
			var actual = Encoding.UTF8.GetBytes("123456789").CalculateCrc16();
			actual.ToString("X4").Dump();
			Assert.AreEqual(0x2189, actual);
		}

		//[TestMethod]
		public void OpenPortAtHighDataRate()
		{
			using (var serial = new OscSerial("COM4", 1000000))
			{
				serial.Open();
			}
		}

		[TestMethod]
		public void EscapeString()
		{
			Assert.AreEqual("Hello \\\"aoeu\\\" foo.", "Hello \"aoeu\" foo.".ToLiteral());
			Assert.AreEqual("\\0", "\0".ToLiteral());
			Assert.AreEqual("\\0", "\0".ToLiteral());
		}

		#endregion
	}
}