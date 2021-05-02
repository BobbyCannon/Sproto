#region References

using System;
using Sproto.Nmea.Exceptions;

#endregion

namespace Sproto.Nmea.Messages
{
	public class GllMessage : NmeaMessage
	{
		#region Constructors

		public GllMessage() : base(NmeaMessageType.LatitudeLongitudeData)
		{
		}

		#endregion

		#region Properties

		public string DataValid { get; set; }

		public string FixTaken { get; set; }

		public Location Latitude { get; set; }

		public Location Longitude { get; set; }

		public ModeIndicator ModeIndicator { get; set; }

		#endregion

		#region Methods

		public override void Parse(string sentence)
		{
			//        0          1 2           3 4         5 6 7   
			// $GNGLL,4513.13795,N,01859.19702,E,143717.00,A,A*72
			//
			// .      0       1 2        3 4         5 6
			//        |       | |        | |         | |
			// $--GLL,llll.ll,a,yyyyy.yy,a,hhmmss.ss,A*hh
			//
			// 0) Latitude
			// 1) N or S (North or South)
			// 2) Longitude
			// 3) E or W (East or West)
			// 4) Time (UTC)
			// 5) Status
			//    A - Data Valid
			//    V - Data Invalid
			// 6) Mode
			//    A = Autonomous
			//    D = DGPS
			//    E = DR (This field is only present in NMEA version 3.0)
			// 7) Checksum

			var items = StartParse(sentence);

			Latitude = new Location(items[0], items[1]);
			Longitude = new Location(items[2], items[3]);
			FixTaken = items[4];
			DataValid = items[5];

			ModeIndicator = new ModeIndicator(GetValueOrDefault(items, 6, ""));
			ModeIndicator = items.Length > 6
				? new ModeIndicator(items[6])
				: new ModeIndicator("");

			OnNmeaMessageParsed(this);
		}

		public override void Reset()
		{
		}

		public override string ToString()
		{
			var result = $"{Type} Latitude:{Latitude} Longitude:{Longitude} FixTaken:{FixTaken} Valid:{DataValid} Mode:{ModeIndicator}";
			return result;
		}

		#endregion
	}
}