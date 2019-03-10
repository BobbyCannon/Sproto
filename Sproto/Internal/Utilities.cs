#region References

using System;

#endregion

namespace Sproto.Internal
{
	internal class Utilities
	{
		#region Methods

		public static int AlignedStringLength(string val)
		{
			var len = val.Length + (4 - val.Length % 4);
			if (len <= val.Length)
			{
				len += 4;
			}

			return len;
		}

		public static ulong DateTimeToTimetag(DateTime value)
		{
			ulong seconds = (uint) (value - DateTime.Parse("1900-01-01 00:00:00.000")).TotalSeconds;
			ulong fraction = (uint) (0xFFFFFFFF * ((double) value.Millisecond / 1000));

			var output = (seconds << 32) + fraction;
			return output;
		}

		public static DateTime TimetagToDateTime(ulong val)
		{
			if (val == 1)
			{
				return DateTime.Now;
			}

			var seconds = (uint) (val >> 32);
			var time = DateTime.Parse("1900-01-01 00:00:00");
			time = time.AddSeconds(seconds);
			var fraction = TimetagToFraction(val);
			time = time.AddSeconds(fraction);
			return time;
		}

		public static double TimetagToFraction(ulong val)
		{
			if (val == 1)
			{
				return 0.0;
			}

			var seconds = (uint) (val & 0x00000000FFFFFFFF);
			var fraction = (double) seconds / 0xFFFFFFFF;
			return fraction;
		}

		#endregion
	}
}