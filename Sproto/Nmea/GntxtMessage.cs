using System;

namespace Sproto.Nmea
{
	public class GntxtMessage : NmeaMessage
	{
		#region Constructors

		public GntxtMessage() : base(NmeaMessageType.Gntxt)
		{
		}

		#endregion

		#region Properties

		public string Text { get; private set; }

		#endregion

		#region Methods

		public override void Parse(string nmeaLine)
		{
			if (string.IsNullOrWhiteSpace(nmeaLine)
				|| !nmeaLine.StartsWith($"${Type}", StringComparison.OrdinalIgnoreCase))
			{
				throw new NmeaParseMismatchException();
			}

			ParseChecksum(nmeaLine);

			if (MandatoryChecksum != ExtractChecksum(nmeaLine))
			{
				throw new NmeaParseChecksumException();
			}

			// remove identifier plus first comma
			var sentence = nmeaLine.Remove(0, $"${Type}".Length + 1);

			// remove checksum and star
			sentence = sentence.Remove(sentence.IndexOf('*'));

			var items = sentence.Split(',');

			Text = items[3];

			OnNmeaMessageParsed(this);
		}

		public override string ToString()
		{
			var result = $"{Type} Text:{Text} ";

			return result;
		}

		#endregion
	}
}