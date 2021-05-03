#region References

using System;
using System.Runtime.Serialization;

#endregion

namespace Sproto.Nmea.Exceptions
{
	[Serializable]
	public class NmeaParseChecksumException : Exception
	{
		#region Constructors

		public NmeaParseChecksumException()
		{
		}

		public NmeaParseChecksumException(string message) : base(message)
		{
		}

		public NmeaParseChecksumException(string message, Exception inner) : base(message, inner)
		{
		}

		protected NmeaParseChecksumException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		#endregion
	}
}