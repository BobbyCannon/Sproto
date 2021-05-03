#region References

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sproto.Nmea;

#endregion

namespace Sproto.Tests.Nmea.Messages
{
	public abstract class BaseMessageTests : BaseTests
	{
		#region Methods

		protected void ProcessParseScenarios<T>((string sentance, T expected)[] scenarios)
			where T : NmeaMessage, new()
		{
			foreach (var scenario in scenarios)
			{
				scenario.expected.UpdateChecksum();
				scenario.expected.ToString().Dump();

				var actual = new T();
				actual.Parse(scenario.sentance);
				Extensions.AreEqual(scenario.expected, actual);

				scenario.expected.UpdateChecksum();
				Assert.AreEqual(scenario.expected.ToString(), actual.ToString());
			}
		}

		#endregion
	}
}