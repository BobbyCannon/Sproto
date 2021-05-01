#region References

using System;
using System.IO.Ports;

#endregion

namespace Sproto.Osc
{
	public class OscSerial : IDisposable
	{
		#region Fields

		private readonly SerialPort _port;
		private readonly byte[] _readBuffer;
		private readonly OscSlip _slip;

		#endregion

		#region Constructors

		public OscSerial(string portName, int baudRate = 115200, int bufferSize = 2048)
		{
			_port = new SerialPort(portName, baudRate, Parity.None, 8, StopBits.One);
			_port.DataReceived += PortOnDataReceived;
			_readBuffer = new byte[bufferSize];
			_slip = new OscSlip(bufferSize);

			Statistics = new OscCommunicationStatistics();
		}

		#endregion

		#region Properties

		public int BaudRate
		{
			get => _port.BaudRate;
			set => _port.BaudRate = value;
		}

		public int DataBits
		{
			get => _port.DataBits;
			set => _port.DataBits = value;
		}

		public bool IsOpen => _port.IsOpen;

		public Parity Parity
		{
			get => _port.Parity;
			set => _port.Parity = value;
		}

		public string PortName
		{
			get => _port.PortName;
			set => _port.PortName = value;
		}

		public TimeSpan ReadTimeout
		{
			get => TimeSpan.FromSeconds(_port.ReadTimeout);
			set => _port.ReadTimeout = (int) value.TotalSeconds;
		}

		public OscCommunicationStatistics Statistics { get; }

		public StopBits StopBits
		{
			get => _port.StopBits;
			set => _port.StopBits = value;
		}

		public TimeSpan WriteTimeout
		{
			get => TimeSpan.FromSeconds(_port.WriteTimeout);
			set => _port.WriteTimeout = (int) value.TotalSeconds;
		}

		#endregion

		#region Methods

		public void Close()
		{
			if (!_port.IsOpen)
			{
				return;
			}

			_port.Close();
		}

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		public void Open()
		{
			if (_port.IsOpen)
			{
				return;
			}

			var rate = _port.BaudRate;

			_port.BaudRate = 115200;
			_port.Open();
			_port.FixMaxBaudRateIssue();
			_port.BaudRate = rate;
		}

		public void Write(OscPacket packet)
		{
			Statistics.PacketsSent.Increment();

			switch (packet)
			{
				case OscMessage _:
					Statistics.MessagesSent.Increment();
					break;

				case OscBundle b:
					if (b.IsExtended)
					{
						Statistics.ExtendedBundlesSent.Increment();
					}
					else
					{
						Statistics.BundlesSent.Increment();
					}
					break;
			}

			var data = OscSlip.EncodePacket(packet);
			Write(data, 0, data.Length);
		}

		public void Write(byte[] data, int offset, int count)
		{
			_port.Write(data, offset, count);

			Statistics.BytesSent.Increment(count);
		}

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		/// <param name="disposing"> True if disposing and false if otherwise. </param>
		protected virtual void Dispose(bool disposing)
		{
			if (!disposing)
			{
				return;
			}

			_port.Dispose();
		}

		protected virtual void OnDataReceived(OscDataReceivedEventArgs e)
		{
			DataReceived?.Invoke(this, e);
		}

		protected virtual void OnPacketReceived(OscPacket e)
		{
			PacketReceived?.Invoke(this, e);
		}

		private void PortOnDataReceived(object sender, SerialDataReceivedEventArgs e)
		{
			var read = 0;

			try
			{
				var processed = 0;
				var bytesToRead = Math.Min(_port.BytesToRead, _readBuffer.Length);
				read = _port.Read(_readBuffer, 0, bytesToRead);

				Statistics.BytesReceived.Increment(read);

				OscPacket packet;

				do
				{
					packet = _slip.ProcessBytes(_readBuffer, ref processed, read - processed);

					if (packet != null)
					{
						ProcessPacket(packet);
					}
				} while (packet != null);
			}
			catch
			{
				// Ignore any receive errors
			}

			// Pass data event to other listeners
			OnDataReceived(new OscDataReceivedEventArgs(_readBuffer, read));
		}

		private void ProcessPacket(OscPacket packet)
		{
			Statistics.PacketsReceived.Increment();

			switch (packet)
			{
				case OscMessage _:
					Statistics.MessagesReceived.Increment();
					break;

				case OscBundle bundle:
					if (bundle.IsExtended)
					{
						Statistics.ExtendedBundlesReceived.Increment();
					}
					else
					{
						Statistics.BundlesReceived.Increment();
					}
					break;
			}

			OnPacketReceived(packet);
		}

		#endregion

		#region Events

		public event EventHandler<OscDataReceivedEventArgs> DataReceived;
		public event EventHandler<OscPacket> PacketReceived;

		#endregion
	}
}