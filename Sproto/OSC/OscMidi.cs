#region References

using System;
using System.Globalization;
using System.Runtime.InteropServices;

#endregion

namespace Sproto.OSC
{
	[StructLayout(LayoutKind.Explicit)]
	public struct OscMidi
	{
		#region Constructors

		public OscMidi(uint value)
		{
			Port = 0;
			Status = 0;
			Data1 = 0;
			Data2 = 0;
			FullMessage = value;
		}

		public OscMidi(byte port, byte status, byte data1, byte data2)
		{
			FullMessage = 0;
			Port = port;
			Status = status;
			Data1 = data1;
			Data2 = data2;
		}

		#endregion

		#region Properties

		[FieldOffset(0)]
		public readonly uint FullMessage;

		[FieldOffset(3)]
		public readonly byte Port;

		[FieldOffset(2)]
		public readonly byte Status;

		[FieldOffset(1)]
		public readonly byte Data1;

		[FieldOffset(0)]
		public readonly byte Data2;

		#endregion

		#region Methods

		public override bool Equals(object obj)
		{
			switch (obj)
			{
				case uint u:
					return FullMessage.Equals(u);

				case OscMidi midi:
					return FullMessage.Equals(midi.FullMessage);

				default:
					return FullMessage.Equals(obj);
			}
		}

		public override int GetHashCode()
		{
			return FullMessage.GetHashCode();
		}

		public static bool operator ==(OscMidi a, OscMidi b)
		{
			return a.Equals(b);
		}

		public static bool operator !=(OscMidi a, OscMidi b)
		{
			return !a.Equals(b);
		}

		public static OscMidi Parse(string value, IFormatProvider provider)
		{
			if (string.IsNullOrWhiteSpace(value))
			{
				throw new Exception($"Not a midi message '{value}'");
			}

			var parts = value.Split(',');

			if (parts.Length < 4)
			{
				throw new Exception($"Not a midi message '{value}'");
			}

			var index = 0;
			var port = byte.Parse(parts[index++].Trim(), provider);

			if (byte.TryParse(parts[index].Trim(), NumberStyles.Integer, provider, out var status) == false)
			{
				if (byte.TryParse(parts[index].Trim(), out var messageType))
				{
					index++;
					var channel = byte.Parse(parts[index++].Trim(), NumberStyles.Integer, provider);

					if (channel > 15)
					{
						throw new ArgumentOutOfRangeException(nameof(channel));
					}

					status = (byte) (messageType | channel);

					if (parts.Length < 5)
					{
						throw new Exception($"Not a midi message '{value}'");
					}
				}
				else
				{
					throw new Exception($"Not a midi message '{value}'");
				}
			}

			var data1 = byte.Parse(parts[index++].Trim(), NumberStyles.Integer, provider);
			if (data1 > 0x7F)
			{
				throw new ArgumentOutOfRangeException(nameof(data1));
			}

			var data2 = byte.Parse(parts[index++].Trim(), NumberStyles.Integer, provider);
			if (data2 > 0x7F)
			{
				throw new ArgumentOutOfRangeException(nameof(data2));
			}

			if (index != parts.Length)
			{
				throw new Exception($"Not a midi message '{value}'");
			}

			return new OscMidi(port, status, data1, data2);
		}

		#endregion
	}
}