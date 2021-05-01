using System;

namespace Sproto.Nmea
{
	public class GnvtgMessage : NmeaMessage
	{
		#region Constructors

		public GnvtgMessage() : this(NmeaMessageType.Gnvtg)
		{
		}

		internal GnvtgMessage(NmeaMessageType type) : base(type)
		{
		}

		#endregion

		#region Properties

		public string GroundSpeedKilometersPerHour { get; private set; }

		public string GroundSpeedKnots { get; private set; }

		public string MagneticTrackMadeGood { get; private set; }

		public ModeIndicator ModeIndicator { get; private set; }

		public string TrueTrackMadeGood { get; private set; }

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

			TrueTrackMadeGood = items[0] + items[1];
			MagneticTrackMadeGood = items[2] + items[3];
			GroundSpeedKnots = items[4] + items[5];
			GroundSpeedKilometersPerHour = items[6] + items[7];

			ModeIndicator = items.Length > 8
				? new ModeIndicator(items[8])
				: new ModeIndicator("");

			OnNmeaMessageParsed(this);
		}

		public override string ToString()
		{
			var result = $"{Type} Truetrack:{TrueTrackMadeGood} MagneticTrack:{MagneticTrackMadeGood} Speed:{GroundSpeedKnots}/{GroundSpeedKilometersPerHour} Mode:{ModeIndicator}";

			return result;
		}

		#endregion
	}
}