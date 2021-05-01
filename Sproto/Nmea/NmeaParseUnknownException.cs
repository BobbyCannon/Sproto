#region References

using System;

#endregion

namespace Sproto.Nmea
{
	public class NmeaParseUnknownException : Exception
	{
		#region Constructors

		public NmeaParseUnknownException()
		{
		}

		public NmeaParseUnknownException(string message) : base(message)
		{
		}

		#endregion
	}
}