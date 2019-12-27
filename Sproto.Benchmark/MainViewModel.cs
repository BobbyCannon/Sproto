#region References

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using PropertyChanged;
using Sproto.OSC;

#endregion

namespace OSC.Benchmark
{
	[AddINotifyPropertyChangedInterface]
	public class MainViewModel
	{
		#region Constructors

		public MainViewModel()
		{
			StartCommand = new RelayCommand(x => Start());
			Tests = new ObservableCollection<BenchmarkTest>
			{
				new BenchmarkTest { Name = "Create Message", Enabled = true, Iterations = 10000, TestMethod = TestCreateMessage },
				new BenchmarkTest { Name = "Read Message", Enabled = true, Iterations = 10000, TestMethod = TestReadMessage },
				new BenchmarkTest { Name = "Encode Message", Enabled = true, Iterations = 10000, TestMethod = TestEncodeMessage },
				new BenchmarkTest { Name = "Create Bundle", Enabled = true, Iterations = 10000, TestMethod = TestCreateBundle },
				new BenchmarkTest { Name = "Encode Bundle", Enabled = true, Iterations = 10000, TestMethod = TestEncodeBundle },
				new BenchmarkTest { Name = "Encode Extended Bundle", Enabled = true, Iterations = 10000, TestMethod = TestEncodeExtendedBundle }
			};

			MessageValues = new Dictionary<string, object[]>
			{
				{ "/noValues", new object[0] },
				{ "/boolTrueValue", new object[] { true } },
				{ "/boolFalseValue", new object[] { false } },
				{ "/stringValue", new object[] { "123456" } },
				{ "/intValue", new object[] { 123456 } },
				{ "/floatValue", new object[] { 123456.0f } },
				{ "/doubleValue", new object[] { 123456.0d } }
			};

			var packets = new List<OscPacket>();

			foreach (var item in MessageValues)
			{
				packets.Add(new OscMessage(OscTimeTag.UtcNow, item.Key, item.Value));
			}

			Packets = packets.ToArray();
			MessageData = new List<byte[]>();

			foreach (var item in Packets)
			{
				MessageData.Add(item.ToByteArray());
			}

			Statistics = new Dictionary<string, string>
			{
				{ "Messages", MessageValues.Keys.Count.ToString() },
				{ "Packets", Packets.Length.ToString() }
			};
		}

		#endregion

		#region Properties

		public List<byte[]> MessageData { get; }

		public Dictionary<string, object[]> MessageValues { get; }

		public OscPacket[] Packets { get; }

		public ICommand StartCommand { get; }

		public Dictionary<string, string> Statistics { get; }

		public ObservableCollection<BenchmarkTest> Tests { get; }

		#endregion

		#region Methods

		private void Start()
		{
			foreach (var row in Tests)
			{
				if (row.Enabled)
				{
					var timeStart = DateTime.Now.Ticks;
					row.TestMethod(row.Iterations);
					var timeEnd = DateTime.Now.Ticks;

					var diff = timeEnd - (double) timeStart;
					var av = diff / row.Iterations;

					row.Average = av / TimeSpan.TicksPerMillisecond;
					row.Total = diff / TimeSpan.TicksPerMillisecond;
				}
				else
				{
					row.Average = -1;
					row.Total = -1;
				}
			}
		}

		private void TestCreateBundle(int count)
		{
			for (var i = 0; i < count; i++)
			{
				var bundle = new OscBundle(DateTime.UtcNow, Packets);
			}
		}

		private void TestCreateMessage(int count)
		{
			var keys = MessageValues.Keys.ToList();
			var keyOffset = 0;

			for (var i = 0; i < count; i++)
			{
				var key = keys[keyOffset];
				var message = new OscMessage(OscTimeTag.UtcNow, key, MessageValues[key]);
				keyOffset = (keyOffset + 1) % keys.Count;
			}
		}

		private void TestEncodeBundle(int count)
		{
			for (var i = 0; i < count; i++)
			{
				var bundle = new OscBundle(DateTime.UtcNow, Packets);
				var buffer = bundle.ToByteArray();
			}
		}

		private void TestEncodeExtendedBundle(int count)
		{
			for (var i = 0; i < count; i++)
			{
				var bundle = new OscBundle(DateTime.UtcNow, Packets) { IsExtended = true };
				var buffer = bundle.ToByteArray();
			}
		}

		private void TestEncodeMessage(int count)
		{
			var messageOffset = 0;

			for (var i = 0; i < count; i++)
			{
				var packet = Packets[messageOffset];
				var buffer = packet.ToByteArray();
				messageOffset = (messageOffset + 1) % MessageData.Count;
			}
		}

		private void TestReadMessage(int count)
		{
			var messageOffset = 0;

			for (var i = 0; i < count; i++)
			{
				var data = MessageData[messageOffset];
				var message = OscPacket.Parse(data);
				messageOffset = (messageOffset + 1) % MessageData.Count;
			}
		}

		#endregion
	}
}