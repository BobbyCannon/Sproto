#region References

using System;

#endregion

namespace Sproto.Nmea.Exceptions
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