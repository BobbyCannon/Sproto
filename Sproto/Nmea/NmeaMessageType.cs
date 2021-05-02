#region References

using System.ComponentModel.DataAnnotations;

#endregion

namespace Sproto.Nmea
{
	public enum NmeaMessageType
	{
		[Display(ShortName = "---", Name = "Unknown")]
		Unknown = 0,

		[Display(ShortName = "AAM", Name = "Waypoint Arrival Alarm")]
		WaypointArrivalAlarm = 1,

		[Display(ShortName = "ALM", Name = "Almanac Data")]
		AlmanacData = 2,

		[Display(ShortName = "APA", Name = "Auto Pilot A")]
		AutoPilotA = 3,

		[Display(ShortName = "APB", Name = "Auto Pilot B")]
		AutoPilotB = 4,

		[Display(ShortName = "BOD", Name = "Bearing Origin To Destination")]
		BearingOriginToDestination = 5,

		[Display(ShortName = "BWC", Name = "Bearing using Great Circle Route")]
		BearingUsingGreatCircleRoute = 6,

		[Display(ShortName = "DTM", Name = "Datum Being Used")]
		DatumBeingUsed = 7,

		/// <summary>
		/// Time, position, and fix related data
		/// </summary>
		/// <remarks>
		/// An example of the GBS message string is:
		/// $GPGGA,172814.0,3723.46587704,N,12202.26957864,W, 2,6,1.2,18.893,M,-25.669,M,2.0,0031*4F
		/// Also note - The data string exceeds the NMEA standard length.
		/// </remarks>
		[Display(ShortName = "GGA", Name = "GGA Time, Position, and Fix related data")]
		GgaFixInformation = 8,

		/// <summary>
		/// GNSS fix data.
		/// GNSS capable receivers will always output this message with the GN talker ID.
		/// GNSS capable receivers will also output this message with the GP and/or GL talker ID
		/// when using more than one constellation for the position fix.
		/// </summary>
		/// <remarks>
		/// An example of the GNS message output from a GNSS capable receiver is:
		/// $GNGNS,014035.00,4332.69262,S,17235.48549,E,RR,13,0.9,25.63,11.24,,*70&lt;CR&gt;&lt;LF&gt;
		/// $GPGNS,014035.00,,,,,,8,,,,1.0,23*76&lt;CR&gt;&lt;LF&gt;
		/// $GLGNS,014035.00,,,,,,5,,,,1.0,23*67&lt;CR&gt;&lt;LF&gt;
		/// </remarks>
		[Display(ShortName = "GNS", Name = "GNSS Fix Information")]
		GnssFixInformation = 9,

		[Display(ShortName = "GLL", Name = "Latitude Longitude Data")]
		LatitudeLongitudeData = 10,

		[Display(ShortName = "GRS", Name = "GPS Range Residuals")]
		GpsRangeResiduals = 11,

		/// <summary>
		/// GSA - Overall Satellite Data
		/// </summary>
		[Display(ShortName = "GSA", Name = "Overall Satellite Data")]
		OverallSatelliteData = 12,

		[Display(ShortName = "GST", Name = "GPS Pseudorange Noise Statistics")]
		GpsPseudorangeNoiseStatistics = 13,

		/// <summary>
		/// GSV - Detailed Satellite Data
		/// </summary>
		[Display(ShortName = "GSV", Name = "Detailed Satellite Data")]
		DetailedSatelliteData = 14,

		[Display(ShortName = "MSK", Name = "Send Control For a Beacon Receiver")]
		SendControlForaBeaconReceiver = 15,

		[Display(ShortName = "MSS", Name = "Beacon Receiver Status Information")]
		BeaconReceiverStatusInformation = 16,

		[Display(ShortName = "RMA", Name = "Recommended Loran Data")]
		RecommendedLoranData = 17,

		[Display(ShortName = "RMB", Name = "Recommended Navigation Data For GPS")]
		RecommendedNavigationDataForGps = 18,

		[Display(ShortName = "RMC", Name = "Recommended Minimum Data For GPS")]
		RecommendedMinimumDataForGps = 19,

		[Display(ShortName = "RTE", Name = "Route Message")]
		RouteMessage = 20,

		[Display(ShortName = "TRF", Name = "Transit Fix Data")]
		TransitFixData = 21,

		[Display(ShortName = "STN", Name = "Multiple Data ID")]
		MultipleDataId = 22,

		[Display(ShortName = "VBW", Name = "Dual Ground / Water Speed")]
		DualGroundWaterSpeed = 23,

		[Display(ShortName = "VTG", Name = "Vector Track Of Speed Over The Ground")]
		VectorTrackOfSpeedOverTheGround = 24,

		[Display(ShortName = "WCV", Name = "Waypoint Closure Velocity (Velocity Made Good)")]
		WaypointClosureVelocity = 25,

		[Display(ShortName = "WPL", Name = "Waypoint Location Information")]
		WaypointLocationInformation = 26,

		[Display(ShortName = "XTC", Name = "Cross Track Error")]
		CrossTrackError = 27,

		[Display(ShortName = "XTE", Name = "Measured Cross Track Error")]
		MeasuredCrossTrackError = 28,

		[Display(ShortName = "ZTG", Name = "Zulu (UTC) Time And Time To Go (to destination)")]
		ZuluTimeAndTimeToGo = 29,

		[Display(ShortName = "ZDA", Name = "Date and Time")]
		DateAndTime = 30,

		/// <summary>
		/// For the transmission of short text messages, longer text messages may be transmitted
		/// by using multiple sentences. This sentence is intended to convey human readable textual
		/// information for display purposes
		/// </summary>
		[Display(ShortName = "TXT", Name = "Text Transmission")]
		TextTransmission = 31
	}
}