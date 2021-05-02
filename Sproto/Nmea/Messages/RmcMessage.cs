namespace Sproto.Nmea.Messages
{
	public sealed class RmcMessage : NmeaMessage
	{
		#region Constructors

		public RmcMessage() : base(NmeaMessageType.RecommendedMinimumDataForGps)
		{
		}

		#endregion

		#region Properties

		public string Course { get; set; }

		public string DateOfFix { get; set; }

		public Location Latitude { get; set; }

		public Location Longitude { get; set; }

		public string MagneticVariation { get; set; }

		public string MagneticVariationUnit { get; set; }

		public ModeIndicator ModeIndicator { get; set; }

		public NavigationStatus NavigationStatus { get; set; }

		public string Speed { get; set; }

		public string Status { get; set; }

		public string TimeOfFix { get; set; }

		#endregion

		#region Methods

		public override void Parse(string sentence)
		{
			// $GNRMC,143718.00,A,4513.13793,N,01859.19704,E,0.050,,290719,,,A*65
			//
			// .      0         1 2         3 4         5 6   7   8      9  10 1112
			//        |         | |         | |         | |   |   |      |   | | |
			// $--RMC,hhmmss.ss,A,ddmm.mmmm,N,ddmm.mmmm,W,x.x,x.x,ddmmyy,x.x,W,a*hh
			//
			//  0) Time (UTC)
			//  1) Status
			//     A = Active
			//     V = Void
			//  2) Latitude, ddmm.mmmm
			//  3) N or S
			//  4) Longitude, ddmm.mmmm
			//  5) E or W
			//  6) Speed over ground, knots
			//  7) Course over ground, degrees true
			//  8) Date, ddmmyy
			//  9) Magnetic Variation, degrees
			// 10) E or W
			// 11) Mode
			//     A = Autonomous
			//     D = DGPS
			//     E = DR
			// 12) Checksum

			var items = StartParse(sentence);

			TimeOfFix = items[0];
			Status = items[1];
			Latitude = new Location(items[2], items[3]);
			Longitude = new Location(items[4], items[5]);
			Speed = items[6];
			Course = items[7];
			DateOfFix = items[8];
			MagneticVariation = items[9];
			MagneticVariationUnit = items[10];
			ModeIndicator = new ModeIndicator(GetValueOrDefault(items, 11, ""));
			NavigationStatus = items.Length >= 13 ? new NavigationStatus(GetValueOrDefault(items, 12, "")) : null;

			OnNmeaMessageParsed(this);
		}

		public override void Reset()
		{
		}

		public override string ToString()
		{
			var start = string.Join(",",
				NmeaParser.GetSentenceStart(this),
				TimeOfFix,
				Status,
				Latitude.Degree,
				Latitude.Indicator,
				Longitude.Degree,
				Longitude.Indicator,
				Speed,
				Course,
				DateOfFix,
				MagneticVariation,
				MagneticVariationUnit,
				ModeIndicator
			);

			if (NavigationStatus != null && NavigationStatus.IsValid())
			{
				start += $",{NavigationStatus}";
			}

			return $"{start}*{Checksum}";
		}

		#endregion
	}
}