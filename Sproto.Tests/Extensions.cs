#region References

using System;
using System.Linq;
using System.Text;
using KellermanSoftware.CompareNetObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#endregion

namespace OSC.Tests
{
	public static class Extensions
	{
		#region Methods

		public static void AreEqual<T>(T expected, T actual, bool ignoreCollectionOrder = false, Action<bool> process = null, params string[] membersToIgnore)
		{
			var configuration = new ComparisonConfig { IgnoreObjectTypes = true, MaxDifferences = int.MaxValue, IgnoreCollectionOrder = ignoreCollectionOrder, MaxStructDepth = 3 };
			if (membersToIgnore.Any())
			{
				configuration.MembersToIgnore = membersToIgnore.ToList();
			}

			var logic = new CompareLogic(configuration);
			var result = logic.Compare(expected, actual);
			process?.Invoke(result.AreEqual);
			Assert.IsTrue(result.AreEqual, result.DifferencesString);
		}

		public static void Dump(this object value)
		{
			Console.WriteLine(value.ToString());
		}

		public static StringBuilder Dump(this byte[] item, string prefix = null, StringBuilder builder = null)
		{
			var result = builder ?? new StringBuilder();

			if (prefix != null)
			{
				result.Append(prefix);
			}

			foreach (var i in item)
			{
				result.Append($"0x{i:X2}, ");
			}

			result = result.Remove(result.Length - 1, 1);

			Console.WriteLine(result);

			return result;
		}

		#endregion
	}
}