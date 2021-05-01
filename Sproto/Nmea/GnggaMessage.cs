#region References

using System;

#endregion

namespace Sproto.Nmea
{
	public class GnggaMessage : NmeaMessage
	{
		#region Constructors

		public GnggaMessage() : this(NmeaMessageType.Gngga)
		{
		}

		internal GnggaMessage(NmeaMessageType type) : base(type)
		{
		}

		#endregion

		#region Properties

		public double Altitude { get; private set; }

		public string FixQuality { get; private set; }

		public double FixTaken { get; private set; }

		public double HeightOfGeoid { get; private set; }

		public double HorizontalDilutionOfPrecision { get; private set; }

		public Location Latitude { get; private set; }

		public Location Longitude { get; private set; }

		public int NumberOfSatellites { get; private set; }

		public string SecondsSinceLastUpdateDgps { get; private set; }

		public string StationIdNumberDgps { get; private set; }

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

			FixTaken = Convert.ToDouble(GetValueOrDefault(items[0], "0"));
			Latitude = new Location(items[1] + items[2]);
			Longitude = new Location(items[3] + items[4]);

			var fixQuality = "Invalid"; // 0 or other values

			switch (items[5])
			{
				case "1":
					fixQuality = "GPS fix";
					break;

				case "2":
					fixQuality = "DGPS fix";
					break;
			}

			FixQuality = fixQuality;

			NumberOfSatellites = Convert.ToInt32(items[6]);
			HorizontalDilutionOfPrecision = Convert.ToDouble(GetValueOrDefault(items[7], "0"));
			Altitude = Convert.ToDouble(GetValueOrDefault(items[8], "0"));
			HeightOfGeoid = Convert.ToDouble(GetValueOrDefault(items[10], "0"));
			SecondsSinceLastUpdateDgps = items[12];
			StationIdNumberDgps = items[13];

			OnNmeaMessageParsed(this);
		}

		public override string ToString()
		{
			var result = $"{Type} Latitude:{Latitude} Longitude:{Longitude} FixTaken:{FixTaken} Quality:{FixQuality} SatCount:{NumberOfSatellites} HDop:{HorizontalDilutionOfPrecision:N1} Altitude:{Altitude} Geoid:{HeightOfGeoid} LastUpdate:{SecondsSinceLastUpdateDgps} DGPS:{StationIdNumberDgps} ";
			return result;
		}

		#endregion
	}
}