#region References

using System;
using PropertyChanged;

#endregion

namespace Sproto.Benchmark
{
	[AddINotifyPropertyChangedInterface]
	public class BenchmarkTest
	{
		#region Properties

		public double Average { get; set; }

		public bool Enabled { get; set; }

		public int Iterations { get; set; }

		public string Name { get; set; }

		public Action<int> TestMethod { get; set; }

		public double Total { get; set; }

		#endregion
	}
}