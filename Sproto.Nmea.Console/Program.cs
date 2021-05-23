#region References

using System;
using System.IO;
using System.IO.Ports;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CommandLineParser.Arguments;
using Speedy;
using Speedy.Extensions;

#endregion

namespace Sproto.Nmea.Console
{
	public class Program
	{
		#region Fields

		private static UdpClient _network;
		private static IPEndPoint _networkEndPoint;
		private static NmeaParser _parser;

		#endregion

		#region Properties

		public static string DataFilePath { get; set; }

		public static int ReadTimeout { get; set; }

		#endregion

		#region Methods

		public static void Main(string[] args)
		{
			var arguments = new CommandLineParser.CommandLineParser();
			var comPort = new ValueArgument<string>('c', "com", "The COM port to be used.") { DefaultValue = string.Empty, ValueOptional = true };
			var portArgument = new ValueArgument<string>('p', "port", "The port to be used.");
			var shareIncomingPort = new SwitchArgument('s', "share", "Option to share the incoming port.", false);
			arguments.Arguments.Add(comPort);
			arguments.Arguments.Add(portArgument);
			arguments.Arguments.Add(shareIncomingPort);
			arguments.ParseCommandLine(args);

			if (!portArgument.Parsed || !int.TryParse(portArgument.Value, out var port))
			{
				System.Console.WriteLine("Sproto.Nmea.Console -p [port] -s");
				return;
			}

			_parser = new NmeaParser();
			_network = new UdpClient { ExclusiveAddressUse = !shareIncomingPort.Parsed || !shareIncomingPort.Value };

			if (!_network.ExclusiveAddressUse)
			{
				_network.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
			}

			_network.Client.Bind(new IPEndPoint(IPAddress.Any, port));
			_networkEndPoint = new IPEndPoint(IPAddress.Broadcast, port);

			if (comPort.Parsed)
			{
				RunAsSerialPort(comPort.Value);
			}

			var from = new IPEndPoint(0, 0);

			Task.Run(() =>
			{
				while (!System.Console.KeyAvailable)
				{
					var buffer = _network.Receive(ref from);
					System.Console.WriteLine(from + " >> " + Encoding.UTF8.GetString(buffer));
				}
			});
		
			System.Console.ReadKey();
		}

		private static SerialPort FindGpsDevice()
		{
			var rates = new[] { 4800, 9600 };

			for (var i = 1; i <= 255; i++)
			{
				var port = new SerialPort($"COM{i}", 4800, Parity.None, 8, StopBits.One);

				try
				{
					foreach (var rate in rates)
					{
						port.BaudRate = rate;
						port.Open();
						port.ReadTimeout = 10000;

						System.Console.WriteLine($"{port.PortName} : {port.BaudRate}");

						for (var j = 0; j < 5; j++)
						{
							try
							{
								var line = port.ReadLine();
								var message = _parser.Parse(line, TimeService.UtcNow);
								if (message != null)
								{
									return port;
								}
							}
							catch (TimeoutException)
							{
								// Try again due to timeout
								Thread.Sleep(1000);
							}
						}

						port.Close();
						port.Dispose();
					}
				}
				catch (Exception ex)
				{
					System.Console.WriteLine(ex.Message);
				}
			}

			return null;
		}

		private static void PortOnDataReceived(object sender, SerialDataReceivedEventArgs e)
		{
			var port = (SerialPort) sender;

			try
			{
				var line = port.ReadLine();
				var data = Encoding.UTF8.GetBytes(line);

				// Broadcast the data as a UDP broadcast
				_network.Send(data, data.Length, _networkEndPoint);

				File.AppendAllText(DataFilePath, line);
				System.Console.WriteLine("\t\t >>> " + line);

				var message = _parser.Parse(line, TimeService.UtcNow);
				if (message != null)
				{
					System.Console.WriteLine(message);
				}
			}
			catch (Exception ex)
			{
				System.Console.WriteLine(ex.Message);
			}
		}

		private static void RunAsSerialPort(string portName)
		{
			SerialPort port = null;

			var test = Assembly.GetExecutingAssembly().GetName().CodeBase;
			var directory = Path.GetDirectoryName(test)?.Replace("file:\\", "") ?? string.Empty;
			DataFilePath = Path.Combine(directory, "data.txt");
			new FileInfo(DataFilePath).SafeDelete();

			if (!string.IsNullOrWhiteSpace(portName))
			{
				try
				{
					port = new SerialPort(portName, 4800, Parity.None, 8, StopBits.One);
					System.Console.WriteLine($"{port.PortName} : {port.BaudRate}");
					port.Open();
				}
				catch
				{
					port?.Dispose();
					port = null;
				}
			}
			else
			{
				port = FindGpsDevice();
			}

			if (port == null)
			{
				System.Console.WriteLine("Failed to find the GPS device.");
			}
			else
			{
				port.ReadTimeout = 30000;

				System.Console.WriteLine($"Found GPS device on port {port.PortName}:{port.BaudRate}, Read Timeout: {port.ReadTimeout}");
				port.DataReceived += PortOnDataReceived;
			}
		}

		#endregion
	}
}