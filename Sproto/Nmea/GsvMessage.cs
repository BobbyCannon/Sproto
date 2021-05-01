#region References

using System;
using System.Collections.Generic;

#endregion

namespace Sproto.Nmea
{
	public abstract class GsvMessage : NmeaMessage
	{
		#region Constructors

		protected GsvMessage(NmeaMessageType type) : base(type)
		{
			Satellites = new List<Satellite>();
		}

		#endregion

		#region Properties

		public int NumberOfSatellitesInView { get; private set; }

		public int NumberOfSentences { get; private set; }

		public List<Satellite> Satellites { get; }

		public int SentenceNr { get; private set; }

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

			NumberOfSentences = Convert.ToInt32(items[0]);
			SentenceNr = Convert.ToInt32(items[1]);
			NumberOfSatellitesInView = Convert.ToInt32(items[2]);

			var satelliteCount = GetSatelliteCount(
				Convert.ToInt32(NumberOfSatellitesInView),
				Convert.ToInt32(NumberOfSentences),
				Convert.ToInt32(SentenceNr));

			for (var i = 0; i < satelliteCount; i++)
			{
				Satellites.Add(
					new Satellite
					{
						SatellitePrnNumber = items[3 + i * 4 + 0],
						ElevationDegrees = items[3 + i * 4 + 1],
						AzimuthDegrees = items[3 + i * 4 + 2],
						SignalStrength = items[3 + i * 4 + 3]
					});
			}

			if (NumberOfSentences == SentenceNr)
			{
				OnNmeaMessageParsed(this);

				Satellites.Clear();
			}
		}

		public override string ToString()
		{
			var result = $"{Type} InView:{NumberOfSatellitesInView} ";

			foreach (var s in Satellites)
			{
				result += $"{s.SatellitePrnNumber}: Azi={s.AzimuthDegrees}° Ele={s.ElevationDegrees}° Str={s.SignalStrength}; ";
			}

			return result;
		}

		private int GetSatelliteCount(int numberOfSatellitesInView, int numberOfSentences, int sentenceNr)
		{
			if (numberOfSentences != sentenceNr)
			{
				return 4;
			}

			return numberOfSatellitesInView - (sentenceNr - 1) * 4;
		}

		#endregion
	}
}