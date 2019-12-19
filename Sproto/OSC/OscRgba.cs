#region References

using System;
using System.Globalization;

#endregion

namespace Sproto.OSC
{
	public struct OscRgba
	{
		#region Constructors

		public OscRgba(byte red, byte green, byte blue, byte alpha)
		{
			R = red;
			G = green;
			B = blue;
			A = alpha;
		}

		#endregion

		#region Properties

		public byte A { get; set; }

		public byte B { get; set; }

		public byte G { get; set; }

		public byte R { get; set; }

		#endregion

		#region Methods

		public override bool Equals(object obj)
		{
			switch (obj)
			{
				case OscRgba oscRgba:
					return R == oscRgba.R && G == oscRgba.G && B == oscRgba.B && A == oscRgba.A;

				case byte[] bytes:
					return R == bytes[0] && G == bytes[1] && B == bytes[2] && A == bytes[3];

				default:
					return false;
			}
		}

		public override int GetHashCode()
		{
			return (R << 24) + (G << 16) + (B << 8) + A;
		}

		public static bool operator ==(OscRgba a, OscRgba b)
		{
			return a.Equals(b);
		}

		public static bool operator !=(OscRgba a, OscRgba b)
		{
			return !a.Equals(b);
		}

		public override string ToString()
		{
			return $"{R},{G},{B},{A}";
		}

		public static object Parse(string value, IFormatProvider provider)
		{
			var pieces = value.Split(',');

			if (pieces.Length != 4)
			{
				throw new Exception($"Invalid color \'{value}\'");
			}

			var r = byte.Parse(pieces[0].Trim(), NumberStyles.None, provider);
			var g = byte.Parse(pieces[1].Trim(), NumberStyles.None, provider);
			var b = byte.Parse(pieces[2].Trim(), NumberStyles.None, provider);
			var a = byte.Parse(pieces[3].Trim(), NumberStyles.None, provider);

			return new OscRgba(r, g, b, a);
		}

		#endregion
	}
}