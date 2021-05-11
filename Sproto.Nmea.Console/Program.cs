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
using Speedy;
using Speedy.Extensions;

#endregion

namespace Sproto.Nmea.Console
{
	internal class Program
	{
		#region Fields

		private static NmeaParser _parser;
		private static UdpClient _network;
		private static IPEndPoint _networkEndPoint;

		#endregion

		#region Properties

		public static string DataFilePath { get; set; }

		#endregion

		#region Methods

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

		private static void Main(string[] args)
		{
			_parser = new NmeaParser();
			_network = new UdpClient { ExclusiveAddressUse = false };
			_network.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
			_network.Client.Bind(new IPEndPoint(IPAddress.Any, 22030));
			_networkEndPoint = new IPEndPoint(IPAddress.Broadcast, 22030);

			RunAsSerialPort(args);

			//int port = 22030;
			//var udpClient = new UdpClient { EnableBroadcast = true, ExclusiveAddressUse = false };
			//udpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
			//udpClient.Client.Bind(new IPEndPoint(IPAddress.Any, port));

			//var from = new IPEndPoint(0, 0);
			//Task.Run(() =>
			//{
			//	while (true)
			//	{
			//		var buffer = udpClient.Receive(ref from);
			//		System.Console.WriteLine(from + " >> " + Encoding.UTF8.GetString(buffer));
			//	}
			//});

			//for (var i = 0; i < 10000; i++)
			//{
			//	var data = Encoding.UTF8.GetBytes($"{udpClient.Client.LocalEndPoint}: Hello World");
			//	udpClient.Send(data, data.Length, "255.255.255.255", port);
			//	Thread.Sleep(1000);
			//}

			System.Console.ReadKey();
		}

		public static void RunAsSerialPort(string[] args)
		{
			SerialPort port = null;
			
			var test = Assembly.GetExecutingAssembly().GetName().CodeBase;
			var directory = Path.GetDirectoryName(test)?.Replace("file:\\", "") ?? string.Empty;
			DataFilePath = Path.Combine(directory, "data.txt");
			new FileInfo(DataFilePath).SafeDelete();

			if (args.Length == 2)
			{
				try
				{
					port = new SerialPort(args[0], int.Parse(args[1]), Parity.None, 8, StopBits.One);
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

			ReadTimeout = args.Length == 3 ? int.Parse(args[2]) : 30000;

			if (port == null)
			{
				System.Console.WriteLine("Failed to find the GPS device.");
			}
			else
			{
				port.ReadTimeout = ReadTimeout;

				System.Console.WriteLine($"Found GPS device on port {port.PortName}:{port.BaudRate}, Read Timeout: {port.ReadTimeout}");
				port.DataReceived += PortOnDataReceived;
			}
		}

		public static int ReadTimeout { get; set; }

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

		#endregion
	}
}