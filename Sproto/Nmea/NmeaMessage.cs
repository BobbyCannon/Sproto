#region References

using System;
using Sproto.Nmea.Exceptions;

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

		public string Checksum { get; set; }

		public NmeaMessagePrefix Prefix { get; set; }

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

		public abstract void Parse(string sentence);

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
			// Calculate the checksum formatted as a two-character hexadecimal
			Checksum = CalculateChecksum(sentence);
		}

		public abstract void Reset();

		/// <summary>
		/// Update the Checksum property by using ToString() value.
		/// </summary>
		public void UpdateChecksum()
		{
			Checksum = CalculateChecksum(ToString());
		}

		/// <summary>
		/// Get the value or return the provided default value.
		/// </summary>
		/// <param name="items"> The items. </param>
		/// <param name="index"> The index of the item. </param>
		/// <param name="defaultValue"> The default value to use if the value is not found. </param>
		/// <returns> </returns>
		protected string GetValueOrDefault(string[] items, int index, string defaultValue)
		{
			var item = index >= items.Length
				? defaultValue
				: items[index];

			return string.IsNullOrEmpty(item) ? defaultValue : item;
		}

		protected virtual void OnNmeaMessageParsed(NmeaMessage e)
		{
			NmeaMessageParsed?.Invoke(this, e);
		}

		protected string[] StartParse(string sentence)
		{
			Reset();

			var (prefix, type, value) = NmeaParser.ExtractPrefixAndType(sentence);

			if (type != Type)
			{
				throw new NmeaParseMismatchException();
			}

			Prefix = prefix;
			ParseChecksum(sentence);

			if (Checksum != ExtractChecksum(sentence))
			{
				throw new NmeaParseChecksumException();
			}

			// remove identifier plus first comma
			var values = sentence.Remove(0, value.Length);

			// remove checksum and star
			values = values.Remove(values.IndexOf('*'));

			return values.Split(',');
		}

		/// <summary>
		/// Generates a checksum for a NMEA sentence
		/// </summary>
		/// <param name="sentence"> The sentence to calculate for. </param>
		/// <returns> The checksum in a two-character hexadecimal format. </returns>
		private static string CalculateChecksum(string sentence)
		{
			// Start with first Item
			int checksum = Convert.ToByte(sentence[sentence.IndexOf('$') + 1]);

			// Loop through all chars to get a checksum
			for (var i = sentence.IndexOf('$') + 2; i < sentence.IndexOf('*'); i++)
			{
				// No. XOR the checksum with this character's value
				checksum ^= Convert.ToByte(sentence[i]);
			}

			// Return the checksum formatted as a two-character hexadecimal
			return checksum.ToString("X2");
		}

		#endregion

		#region Events

		public event EventHandler<NmeaMessage> NmeaMessageParsed;

		#endregion
	}
}