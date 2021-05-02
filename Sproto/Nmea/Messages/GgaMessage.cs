#region References

using System;
using Sproto.Nmea.Exceptions;

#endregion

namespace Sproto.Nmea.Messages
{
	public class GgaMessage : NmeaMessage
	{
		#region Constructors

		public GgaMessage() : base(NmeaMessageType.GgaFixInformation)
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

		public override void Parse(string sentence)
		{
			// $GNGGA,143718.00,4513.13793,N,01859.19704,E,1,05,1.86,108.1,M,38.1,M,,*40
			//
			// .      0         1       2 3        4 5 6  7   8   9 10  11 12 13   14
			//	      |         |       | |        | | |  |   |   | |   |  |  |    |
			// $--GGA,hhmmss.ss,llll.ll,a,yyyyy.yy,a,x,xx,x.x,x.x,M,x.x,M,x.x,xxxx*hh
			//
			//  0) Time (UTC)
			//  1) Latitude
			//  2) N or S (North or South)
			//  3) Longitude
			//  4) E or W (East or West)
			//  5) GPS Quality Indicator,
			//     0 - fix not available,
			//     1 - GPS fix,
			//     2 - Differential GPS fix
			//  6) Number of satellites in view, 00 - 12
			//  7) Horizontal Dilution of precision
			//  8) Antenna Altitude above/below mean-sea-level (geoid)
			//  9) Units of antenna altitude, meters
			// 10) Geoidal separation, the difference between the WGS-84 earth ellipsoid and mean-sea-level (geoid), "-" means mean-sea-level below ellipsoid
			// 11) Units of geoidal separation, meters
			// 12) Age of differential GPS data, time in seconds since last SC104 type 1 or 9 update, null field when DGPS is not used
			// 13) Differential reference station ID, 0000-1023
			// 14) Checksum

			var items = StartParse(sentence);

			FixTaken = Convert.ToDouble(GetValueOrDefault(items, 0, "0"));
			Latitude = new Location(items[1], items[2]);
			Longitude = new Location(items[3], items[4]);

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
			HorizontalDilutionOfPrecision = Convert.ToDouble(GetValueOrDefault(items, 7, "0"));
			Altitude = Convert.ToDouble(GetValueOrDefault(items, 8, "0"));
			HeightOfGeoid = Convert.ToDouble(GetValueOrDefault(items, 10, "0"));
			SecondsSinceLastUpdateDgps = items[12];
			StationIdNumberDgps = items[13];

			OnNmeaMessageParsed(this);
		}

		public override void Reset()
		{
		}

		public override string ToString()
		{
			var result = $"{Type} Latitude:{Latitude} Longitude:{Longitude} FixTaken:{FixTaken} Quality:{FixQuality} SatCount:{NumberOfSatellites} HDop:{HorizontalDilutionOfPrecision:N1} Altitude:{Altitude} Geoid:{HeightOfGeoid} LastUpdate:{SecondsSinceLastUpdateDgps} DGPS:{StationIdNumberDgps} ";
			return result;
		}

		#endregion
	}
}