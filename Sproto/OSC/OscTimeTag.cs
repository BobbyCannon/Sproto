﻿#region References

using System;
using System.Globalization;

#endregion

namespace Sproto.OSC
{
	public struct OscTimeTag
	{
		#region Constructors

		public OscTimeTag(ulong value)
		{
			Value = value;
		}

		public OscTimeTag(DateTime value)
			: this(FromDateTime(value).Value)
		{
		}

		#endregion

		#region Properties

		/// <summary>
		/// The minimum date for any OscTimeTag.
		/// </summary>
		public static readonly DateTime BaseDate = new DateTime(1900, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

		/// <summary>
		/// Gets the number of seconds since midnight on January 1, 1900. This is the first 32 bits of the 64 bit fixed point OscTimeTag value.
		/// </summary>
		public uint Seconds => (uint) (Value >> 32);

		/// <summary>
		/// Gets the fractional parts of a second. This is the 32 bits of the 64 bit fixed point OscTimeTag value.
		/// </summary>
		public uint SubSeconds => (uint) Value;

		/// <summary>
		/// Gets a OscTimeTag object that is set to the current date and time on this computer, expressed as the local time.
		/// </summary>
		public static OscTimeTag Now => FromDateTime(DateTime.Now);

		/// <summary>
		/// Gets a OscTimeTag object that is set to the current date and time on this computer, expressed as the Coordinated Universal Time (UTC).
		/// </summary>
		public static OscTimeTag UtcNow => FromDateTime(DateTime.UtcNow);

		/// <summary>
		/// Gets or set the value of the tag.
		/// </summary>
		public ulong Value { get; set; }

		/// <summary>
		/// Gets the number of seconds including fractional parts since midnight on January 1, 1900.
		/// </summary>
		public decimal PreciseValue => Seconds + (decimal) (SubSeconds / (double) uint.MaxValue);

		#endregion

		#region Methods

		/// <summary>
		/// Get the equivalent DateTime value from the OscTimeTag.
		/// </summary>
		/// <returns>
		/// The equivalent value as DateTime type.
		/// </returns>
		public DateTime ToDateTime()
		{
			// Kas: http://stackoverflow.com/questions/5206857/convert-ntp-timestamp-to-utc
			var seconds = Seconds;
			var fraction = SubSeconds;
			var milliseconds = fraction / (double) uint.MaxValue * 1000;
			var datetime = BaseDate.AddSeconds(seconds).AddMilliseconds(milliseconds);
			return datetime;
		}

		/// <summary>
		/// Get a OscTimeTag from a DateTime value.
		/// </summary>
		/// <param name="datetime"> DateTime value. </param>
		/// <returns> The equivalent value as an osc time tag. </returns>
		public static OscTimeTag FromDateTime(DateTime datetime)
		{
			var span = datetime.Subtract(BaseDate);
			return FromTimeSpan(span);
		}

		/// <summary>
		/// Get a OscTimeTag from a TimeSpan value.
		/// </summary>
		/// <param name="span"> The span of time. </param>
		/// <returns> The equivalent value as an osc time tag. </returns>
		public static OscTimeTag FromTimeSpan(TimeSpan span)
		{
			var seconds = span.TotalSeconds;
			var secondsUInt = (uint) seconds;
			var milliseconds = span.TotalMilliseconds - (double) secondsUInt * 1000;
			var fraction = milliseconds / 1000 * uint.MaxValue;
			return new OscTimeTag(((ulong) (secondsUInt & 0xFFFFFFFF) << 32) | ((ulong) fraction & 0xFFFFFFFF));
		}

		public static OscTimeTag FromMilliseconds(float value)
		{
			var span = TimeSpan.FromSeconds(value);
			return FromTimeSpan(span);
		}

		public override bool Equals(object obj)
		{
			switch (obj)
			{
				case OscTimeTag tag:
					return Value == tag.Value;

				case ulong value:
					return Value == value;

				default:
					return false;
			}
		}

		public override int GetHashCode()
		{
			return (int) (((uint) (Value >> 32) + (uint) (Value & 0x00000000FFFFFFFF)) / 2);
		}

		public static bool operator ==(OscTimeTag a, OscTimeTag b)
		{
			return a.Equals(b);
		}

		public static bool operator !=(OscTimeTag a, OscTimeTag b)
		{
			return a.Equals(b);
		}

		public static OscTimeTag Parse(string value)
		{
			return Parse(value, CultureInfo.InvariantCulture);
		}

		public static OscTimeTag Parse(string value, IFormatProvider provider)
		{
			var style = DateTimeStyles.AssumeLocal;

			if (value.Trim().EndsWith("Z"))
			{
				style = DateTimeStyles.AdjustToUniversal;
				value = value.Trim().TrimEnd('Z');
			}

			// https://en.wikipedia.org/wiki/ISO_8601
			// yyyy = four-digit year
			// MM   = two-digit month (01=January, etc.)
			// dd   = two-digit day of month (01 through 31)
			// HH   = two digits of hour (00 through 23) (am/pm NOT allowed)
			// mm   = two digits of minute (00 through 59)
			// ss   = two digits of second (00 through 59)
			// f    = one or more digits representing a decimal fraction of a second
			// TZD  = time zone designator (Z or +hh:mm or -hh:mm)

			// Examples
			// Year: YYYY (eg 1997)
			// Year and month: YYYY-MM (eg 1997-07)
			// Complete date: YYYY-MM-DD (eg 1997-07-16)
			// Complete date plus hours and minutes:
			//		YYYY-MM-DDThh:mmTZD (eg 1997-07-16T19:20+01:00)
			// Complete date plus hours, minutes and seconds:
			//		YYYY-MM-DDThh:mm:ssTZD (eg 1997-07-16T19:20:30+01:00)
			// Complete date plus hours, minutes, seconds and a decimal fraction of a second
			//		YYYY-MM-DDThh:mm:ss.sTZD (eg 1997-07-16T19:20:30.45+01:00)
			string[] formats =
			{
				"yyyy",
				"yyyy-MM",
				"yyyy-MM-dd",
				"HH:mm",
				"HH:mm:ss",
				"HH:mm:ss.ffff",
				"yyyy-MM-ddTHH:mm:ss",
				"yyyy-MM-ddTHH:mm",
				"yyyy-MM-ddTHH:mm:ss.ffff"
			};

			if (DateTime.TryParseExact(value, formats, provider, style, out var datetime))
			{
				return FromDateTime(datetime);
			}

			if (value.StartsWith("0x") && ulong.TryParse(value.Substring(2), NumberStyles.HexNumber, provider, out var value64))
			{
				return new OscTimeTag(value64);
			}

			if (ulong.TryParse(value, NumberStyles.Integer, provider, out value64))
			{
				return new OscTimeTag(value64);
			}

			throw new Exception($"Invalid OscTimeTag string \'{value}\'");
		}

		public override string ToString()
		{
			return ToString("yyyy-MM-ddTHH:mm:ss.ffffZ");
		}

		public string ToString(string format)
		{
			return ToDateTime().ToString(format);
		}

		#endregion

		public double ToMilliseconds()
		{
			return ToDateTime().Subtract(BaseDate).TotalMilliseconds;
		}
	}
}