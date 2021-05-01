#region References

using System.IO.Ports;
using System.Threading;

#endregion

namespace Sproto.Nmea.Console
{
	internal class Program
	{
		#region Fields

		private static NmeaParser _parser;

		#endregion

		#region Methods

		private static SerialPort FindGpsDevice()
		{
			var rates = new[] { 4800, 9600 };

			for (var i = 1; i <= 255; i++)
			{
				var port = new SerialPort($"COM{i}", 4800);

				try
				{
					port.Open();

					foreach (var rate in rates)
					{
						port.BaudRate = rate;
						port.ReadTimeout = 1000;

						for (var j = 0; j < 5; j++)
						{
							var line = port.ReadLine();
							var message = _parser.Parse(line);
							if (message != null)
							{
								return port;
							}

							Thread.Sleep(25);
						}
					}
				}
				catch
				{
					// ignore this
				}
			}

			return null;
		}

		private static void Main(string[] args)
		{
			_parser = new NmeaParser();

			var port = FindGpsDevice();
			if (port == null)
			{
				System.Console.WriteLine("Failed to find the GPS device.");
			}
			else
			{
				System.Console.WriteLine($"Found GPS device on port {port.PortName}:{port.BaudRate}");
				port.DataReceived += PortOnDataReceived;
			}
			System.Console.ReadKey();
		}

		private static void PortOnDataReceived(object sender, SerialDataReceivedEventArgs e)
		{
			var port = (SerialPort) sender;
			var line = port.ReadLine();

			System.Console.WriteLine("\t\t >>> " + line);

			var message = _parser.Parse(line);
			if (message != null)
			{
				System.Console.WriteLine(message);
			}
		}

		#endregion
	}
}