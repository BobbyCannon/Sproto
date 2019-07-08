#region References

using System;
using System.Globalization;
using System.Text;
using Sproto.Internal;

#endregion

namespace Sproto.OSC
{
	public abstract class OscPacket
	{
		#region Properties
		
		/// <summary>
		/// Gets the time of this bundle.
		/// </summary>
		public OscTimeTag Time { get; set; }

		#endregion

		#region Methods

		public static OscPacket GetPacket(byte[] buffer, int length)
		{
			return GetPacket(OscTimeTag.UtcNow, buffer, length);
		}

		public static OscPacket GetPacket(OscTimeTag time, byte[] buffer, int length)
		{
			try
			{
				if (buffer[0] == '#' || buffer[0] == '+')
				{
					return OscBundle.Parse(buffer, length);
				}

				return OscMessage.Parse(time, buffer, length);
			}
			catch (Exception)
			{
				return new OscError(OscTimeTag.UtcNow, OscError.Message.InvalidParsedMessage);
			}
		}

		/// <summary>
		/// Parse a packet from a string using the default provider InvariantCulture.
		/// </summary>
		/// <param name="value"> A string containing the OSC packet data. </param>
		/// <returns> The parsed OSC packet. </returns>
		public static OscPacket Parse(string value)
		{
			return Parse(value, CultureInfo.InvariantCulture);
		}

		/// <summary>
		/// Parse a packet from a string using the supplied provider.
		/// </summary>
		/// <param name="value"> A string containing the OSC packet data. </param>
		/// <param name="provider"> The format provider to use during parsing. </param>
		/// <returns> The parsed OSC packet. </returns>
		public static OscPacket Parse(string value, IFormatProvider provider)
		{
			return Parse(OscTimeTag.UtcNow, value, provider);
		}
		
		/// <summary>
		/// Parse a packet from a string using the supplied provider.
		/// </summary>
		/// <param name="value"> A string containing the OSC packet data. </param>
		/// <param name="provider"> The format provider to use during parsing. </param>
		/// <returns> The parsed OSC packet. </returns>
		public static OscPacket Parse(OscTimeTag time, string value, IFormatProvider provider)
		{
			if (value.StartsWith("#") || value.StartsWith("+"))
			{
				return OscBundle.Parse(value, provider);
			}

			return OscMessage.Parse(time, value, provider);
		}

		public abstract byte[] ToByteArray();

		protected static string GetAddress(byte[] msg, int index)
		{
			var i = index;
			var address = "";

			for (; i < msg.Length; i += 4)
			{
				if (msg[i] != ',')
				{
					continue;
				}

				if (i == 0)
				{
					return string.Empty;
				}

				address = Encoding.ASCII.GetString(msg.SubArray(index, i - 1));
				break;
			}

			if (i >= msg.Length && address == null)
			{
				throw new Exception("no comma found");
			}

			return address.Replace("\0", "");
		}

		protected static byte[] GetBlob(byte[] msg, int index)
		{
			var size = GetInt(msg, index);
			return msg.SubArray(index + 4, size);
		}

		protected static char GetChar(byte[] msg, int index)
		{
			return (char) msg[index + 3];
		}

		protected static OscCrc GetCrc(byte[] msg, int index)
		{
			return new OscCrc(BitConverter.ToUInt16(msg, index));
		}

		protected static double GetDouble(byte[] msg, int index)
		{
			var var = new byte[8];
			var[7] = msg[index];
			var[6] = msg[index + 1];
			var[5] = msg[index + 2];
			var[4] = msg[index + 3];
			var[3] = msg[index + 4];
			var[2] = msg[index + 5];
			var[1] = msg[index + 6];
			var[0] = msg[index + 7];
			return BitConverter.ToDouble(var, 0);
		}

		protected static float GetFloat(byte[] msg, int index)
		{
			var reversed = new byte[4];
			reversed[3] = msg[index];
			reversed[2] = msg[index + 1];
			reversed[1] = msg[index + 2];
			reversed[0] = msg[index + 3];
			return BitConverter.ToSingle(reversed, 0);
		}

		protected static int GetInt(byte[] msg, int index)
		{
			return (msg[index] << 24) + (msg[index + 1] << 16) + (msg[index + 2] << 8) + (msg[index + 3] << 0);
		}

		protected static long GetLong(byte[] msg, int index)
		{
			var var = new byte[8];
			var[7] = msg[index];
			var[6] = msg[index + 1];
			var[5] = msg[index + 2];
			var[4] = msg[index + 3];
			var[3] = msg[index + 4];
			var[2] = msg[index + 5];
			var[1] = msg[index + 6];
			var[0] = msg[index + 7];
			return BitConverter.ToInt64(var, 0);
		}

		protected static OscMidi GetMidi(byte[] msg, int index)
		{
			return new OscMidi(msg[index], msg[index + 1], msg[index + 2], msg[index + 3]);
		}

		protected static OscRgba GetRgba(byte[] msg, int index)
		{
			return new OscRgba(msg[index], msg[index + 1], msg[index + 2], msg[index + 3]);
		}

		protected static string GetString(byte[] msg, ref int index)
		{
			string output = null;
			var i = index + 4;

			for (; i - 1 < msg.Length; i += 4)
			{
				if (msg[i - 1] == 0)
				{
					output = Encoding.ASCII.GetString(msg.SubArray(index, i - index));
					break;
				}
			}

			if (i >= msg.Length && output == null)
			{
				throw new Exception("No null terminator after type string");
			}

			index = i;
			return output?.Replace("\0", "");
		}

		protected static char[] GetTypes(byte[] msg, int index)
		{
			var i = index + 4;
			char[] types = null;

			for (; i <= msg.Length; i += 4)
			{
				if (msg[i - 1] == 0)
				{
					types = Encoding.ASCII.GetChars(msg.SubArray(index, i - index));
					break;
				}
			}

			if (i >= msg.Length && types == null)
			{
				throw new Exception("No null terminator after type string");
			}

			return types;
		}

		protected static ulong GetULong(byte[] msg, int index)
		{
			var val = ((ulong) msg[index] << 56) + ((ulong) msg[index + 1] << 48) + ((ulong) msg[index + 2] << 40) + ((ulong) msg[index + 3] << 32)
				+ ((ulong) msg[index + 4] << 24) + ((ulong) msg[index + 5] << 16) + ((ulong) msg[index + 6] << 8) + ((ulong) msg[index + 7] << 0);
			return val;
		}

		protected static byte[] SetBlob(byte[] value)
		{
			var len = value.Length + 4;
			len = len + (4 - len % 4);

			var msg = new byte[len];
			var size = SetInt(value.Length);
			size.CopyTo(msg, 0);
			value.CopyTo(msg, 4);
			return msg;
		}

		protected static byte[] SetChar(char value)
		{
			var output = new byte[4];
			output[0] = 0;
			output[1] = 0;
			output[2] = 0;
			output[3] = (byte) value;
			return output;
		}

		protected static byte[] SetCrc(OscCrc crc)
		{
			return BitConverter.GetBytes(crc.Value);
		}

		protected static byte[] SetDouble(double value)
		{
			var rev = BitConverter.GetBytes(value);
			var output = new byte[8];
			output[0] = rev[7];
			output[1] = rev[6];
			output[2] = rev[5];
			output[3] = rev[4];
			output[4] = rev[3];
			output[5] = rev[2];
			output[6] = rev[1];
			output[7] = rev[0];
			return output;
		}

		protected static byte[] SetFloat(float value)
		{
			var msg = new byte[4];
			var bytes = BitConverter.GetBytes(value);
			msg[0] = bytes[3];
			msg[1] = bytes[2];
			msg[2] = bytes[1];
			msg[3] = bytes[0];
			return msg;
		}

		protected static byte[] SetInt(int value)
		{
			var msg = new byte[4];
			var bytes = BitConverter.GetBytes(value);
			msg[0] = bytes[3];
			msg[1] = bytes[2];
			msg[2] = bytes[1];
			msg[3] = bytes[0];
			return msg;
		}

		protected static byte[] SetLong(long value)
		{
			var rev = BitConverter.GetBytes(value);
			var output = new byte[8];
			output[0] = rev[7];
			output[1] = rev[6];
			output[2] = rev[5];
			output[3] = rev[4];
			output[4] = rev[3];
			output[5] = rev[2];
			output[6] = rev[1];
			output[7] = rev[0];
			return output;
		}

		protected static byte[] SetMidi(OscMidi midi)
		{
			var output = new byte[4];
			output[0] = midi.Port;
			output[1] = midi.Status;
			output[2] = midi.Data1;
			output[3] = midi.Data2;
			return output;
		}

		protected static byte[] SetRgba(OscRgba value)
		{
			var output = new byte[4];
			output[0] = value.R;
			output[1] = value.G;
			output[2] = value.B;
			output[3] = value.A;
			return output;
		}

		protected static byte[] SetString(string value)
		{
			// Make sure we have room for the null terminator
			var len = value.Length + (4 - value.Length % 4);
			if (len <= value.Length)
			{
				len = len + 4;
			}

			var msg = new byte[len];
			var bytes = Encoding.ASCII.GetBytes(value);

			bytes.CopyTo(msg, 0);

			return msg;
		}

		protected static byte[] SetULong(ulong value)
		{
			var rev = BitConverter.GetBytes(value);
			var output = new byte[8];
			output[0] = rev[7];
			output[1] = rev[6];
			output[2] = rev[5];
			output[3] = rev[4];
			output[4] = rev[3];
			output[5] = rev[2];
			output[6] = rev[1];
			output[7] = rev[0];
			return output;
		}

		#endregion
	}
}