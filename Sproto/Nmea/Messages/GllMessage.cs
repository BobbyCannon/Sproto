﻿#region References

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
			// 0) Latitude - DDmm.mm
			// 1) Direction
			//    N - North
			//    S - South
			// 2) Longitude - DDDmm.mm
			// 3) Direction
			//    E - East
			//    W - West
			// 4) Time (UTC) hhmmss.ss
			// 5) Status
			//    A - Data Valid
			//    V - Data Invalid
			// 6) Mode Indicator
			//    A = Autonomous
			//    D = Differential
			//    E - Estimated
			//    M - Manual
			//    N - No Fix
			//    P - Precise
			//    R - Real Time Kinematic
			//    S - Simulator
			// 7) Checksum

			StartParse(sentence);

			Latitude = new Location(GetArgument(0), GetArgument(1));
			Longitude = new Location(GetArgument(2), GetArgument(3));
			FixTaken = GetArgument(4);
			DataValid = GetArgument(5);

			ModeIndicator = Arguments.Count > 6
				? new ModeIndicator(GetArgument(6))
				: null;

			OnNmeaMessageParsed(this);
		}

		public override string ToString()
		{
			var result = $"{Type} Latitude:{Latitude} Longitude:{Longitude} FixTaken:{FixTaken} Valid:{DataValid} Mode:{ModeIndicator}";
			return result;
		}

		#endregion
	}
}