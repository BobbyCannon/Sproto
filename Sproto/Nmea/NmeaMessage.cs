#region References

using System;
using System.Collections.Generic;
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
			Arguments = new List<string>();
			Type = type;
		}

		#endregion

		#region Properties

		/// <summary>
		/// The checksum for this message.
		/// </summary>
		public string Checksum { get; set; }

		/// <summary>
		/// The prefix known as Talker ID.
		/// </summary>
		public NmeaMessagePrefix Prefix { get; set; }

		public DateTime TimestampUtc { get; set; }

		public NmeaMessageType Type { get; }

		/// <summary>
		/// The arguments of this message.
		/// </summary>
		protected List<string> Arguments { get; set; }

		#endregion

		#region Methods

		/// <summary>
		/// Take the last characters which should be the checksum
		/// </summary>
		/// <param name="sentence"> </param>
		/// <returns> </returns>
		public string ExtractChecksum(string sentence)
		{
			var index = sentence?.LastIndexOf('*') ?? -1;
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
		public string ParseChecksum(string sentence)
		{
			// Calculate the checksum formatted as a two-character hexadecimal
			Checksum = CalculateChecksum(sentence);
			return Checksum;
		}

		/// <summary>
		/// Reset the message to default.
		/// </summary>
		public virtual void Reset()
		{
			Arguments.Clear();
		}

		/// <summary>
		/// Update the Checksum property by using ToString() value.
		/// </summary>
		public void UpdateChecksum()
		{
			Checksum = CalculateChecksum(ToString());
		}

		/// <summary>
		/// Gets the argument for the index offset.
		/// </summary>
		/// <param name="index"> The index of the argument to cast. </param>
		/// <param name="defaultValue"> The default value if the argument index does not exists. </param>
		/// <returns> The typed argument. </returns>
		protected string GetArgument(int index, string defaultValue = "")
		{
			var response = index >= Arguments.Count ? defaultValue : Arguments[index];
			return string.IsNullOrWhiteSpace(response) ? defaultValue : response;
		}

		protected virtual void OnNmeaMessageParsed(NmeaMessage e)
		{
			NmeaMessageParsed?.Invoke(this, e);
		}

		protected void StartParse(string sentence)
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
				throw new NmeaParseChecksumException($"{Checksum} != {ExtractChecksum(sentence)}");
			}

			// remove identifier plus first comma
			var values = sentence.Remove(0, value.Length);

			// remove checksum and star
			values = values.Remove(values.IndexOf('*'));

			// Assign the values as arguments
			Arguments.AddRange(values.Split(','));
		}

		/// <summary>
		/// Generates a checksum for a NMEA sentence
		/// </summary>
		/// <param name="sentence"> The sentence to calculate for. </param>
		/// <returns> The checksum in a two-character hexadecimal format. </returns>
		private static string CalculateChecksum(string sentence)
		{
			if (string.IsNullOrWhiteSpace(sentence) || sentence.Length < 2)
			{
				return 0.ToString("X2");
			}

			// Start with first Item
			int checksum = Convert.ToByte(sentence[sentence.IndexOf('$') + 1]);

			// Loop through all chars to get a checksum
			var start = sentence.IndexOf('$') + 2;
			var end = sentence.IndexOf('*');

			if (start > sentence.Length)
			{
				start = sentence.Length;
			}

			if (end == -1)
			{
				end = sentence.Length;
			}

			if (start > end)
			{
				start = end;
			}

			for (var i = start; i < end; i++)
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