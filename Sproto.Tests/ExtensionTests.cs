#region References

using System.Collections.Generic;
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

		[TestMethod]
		public void EscapeString()
		{
			var items = new Dictionary<string, string>
			{
				{ "Hello \\\"aoeu\\\" foo.", "Hello \"aoeu\" foo." },
				{ "\\0", "\0" }
			};

			foreach (var item in items)
			{
				Assert.AreEqual(item.Key, item.Value.Escape());
			}
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
		public void UnescapeString()
		{
			var items = new Dictionary<string, string>
			{
				{ "Hello \\\"aoeu\\\" foo.", "Hello \"aoeu\" foo." },
				{ "\\0", "\0" }
			};

			foreach (var item in items)
			{
				Assert.AreEqual(item.Value, item.Key.Unescape());
			}
		}

		#endregion
	}
}