#region References

using System;

#endregion

namespace Sproto.Nmea
{
	/// <summary>
	/// Base message
	/// </summary>
	public abstract class NmeaMessage
	{
		#region Constructors

		protected NmeaMessage(NmeaMessageType type)
		{
			Type = type;
		}

		#endregion

		#region Properties

		public string MandatoryChecksum { get; set; }

		public DateTime TimestampUtc { get; set; }

		public NmeaMessageType Type { get; }

		#endregion

		#region Methods

		/// <summary>
		/// Take the last characters which should be the checksum
		/// </summary>
		/// <param name="sentence"> </param>
		/// <returns> </returns>
		public string ExtractChecksum(string sentence)
		{
			var index = sentence.LastIndexOf('*');
			if (index == -1)
			{
				return string.Empty;
			}

			return sentence.Substring(index + 1);
		}

		public abstract void Parse(string nmeaLine);

		/// <summary>
		/// Calculate checksum of Nmea sentence.
		/// </summary>
		/// <param name="sentence"> The Nmea sentence </param>
		/// <returns> The hexidecimal checksum </returns>
		/// <remarks>
		/// Example taken from https://gist.github.com/maxp/1193206
		/// </remarks>
		public void ParseChecksum(string sentence)
		{
			//Start with first Item
			int checksum = Convert.ToByte(sentence[sentence.IndexOf('$') + 1]);

			// Loop through all chars to get a checksum
			for (var i = sentence.IndexOf('$') + 2; i < sentence.IndexOf('*'); i++)
			{
				// No. XOR the checksum with this character's value
				checksum ^= Convert.ToByte(sentence[i]);
			}

			// Return the checksum formatted as a two-character hexadecimal
			MandatoryChecksum = checksum.ToString("X2");
		}

		protected string GetValueOrDefault(string item, string defaultValue)
		{
			return string.IsNullOrEmpty(item) ? defaultValue : item;
		}

		protected virtual void OnNmeaMessageParsed(NmeaMessage e)
		{
			if (NmeaMessageParsed != null)
			{
				NmeaMessageParsed(this, e);
			}
		}

		#endregion

		#region Events

		public event EventHandler<NmeaMessage> NmeaMessageParsed;

		#endregion
	}
}