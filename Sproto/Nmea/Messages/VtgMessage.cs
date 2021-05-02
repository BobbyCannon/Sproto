namespace Sproto.Nmea.Messages
{
	public class VtgMessage : NmeaMessage
	{
		#region Constructors

		public VtgMessage() : base(NmeaMessageType.VectorTrackOfSpeedOverTheGround)
		{
		}

		#endregion

		#region Properties

		public string GroundSpeed { get; set; }

		public string GroundSpeedKilometersPerHour { get; set; }

		public string GroundSpeedKilometersPerHourUnit { get; set; }

		public string GroundSpeedUnit { get; set; }

		public string MagneticCourse { get; set; }

		public string MagneticCourseUnit { get; set; }

		public ModeIndicator ModeIndicator { get; set; }

		public string TrueCourse { get; set; }

		public string TrueCourseUnit { get; set; }

		#endregion

		#region Methods

		public override void Parse(string sentence)
		{
			// $GPVTG,140.88,T,,M,8.04,N,14.89,K,D*05
			//
			// .      0   1 2   3 4   5 6   7 8 9
			//        |   | |   | |   | |   | | |
			// $--VTG,x.x,a,x.x,a,x.x,a,x.x,a,a*hh
			//
			// 0) Track made good (degrees true)
			// 1) T: track made good is relative to true north
			// 2) Track made good (degrees magnetic)
			// 3) M: track made good is relative to magnetic north
			// 4) Speed, in knots
			// 5) N: speed is measured in knots
			// 6) Speed over ground in kilometers/hour (kph)
			// 7) K: speed over ground is measured in kph
			// 8) Mode indicator:
			//    A: Autonomous mode
			//    D: Differential mode
			//    E: Estimated (dead reckoning) mode
			//    M: Manual Input mode
			//    S: Simulator mode
			//    N: Data not valid
			// 9) Checksum

			var items = StartParse(sentence);

			TrueCourse = items[0];
			TrueCourseUnit = items[1];
			MagneticCourse = items[2];
			MagneticCourseUnit = items[3];
			GroundSpeed = items[4];
			GroundSpeedUnit = items[5];
			GroundSpeedKilometersPerHour = items[6];
			GroundSpeedKilometersPerHourUnit = items[7];
			ModeIndicator = new ModeIndicator(GetValueOrDefault(items, 8, ""));

			OnNmeaMessageParsed(this);
		}

		public override void Reset()
		{
		}

		public override string ToString()
		{
			var start = string.Join(",",
				NmeaParser.GetSentenceStart(this),
				TrueCourse,
				TrueCourseUnit,
				MagneticCourse,
				MagneticCourseUnit,
				GroundSpeed,
				GroundSpeedUnit,
				GroundSpeedKilometersPerHour,
				GroundSpeedKilometersPerHourUnit,
				ModeIndicator
			);

			return $"{start}*{Checksum}";
		}

		#endregion
	}
}