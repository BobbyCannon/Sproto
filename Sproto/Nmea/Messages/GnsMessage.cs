#region References

using System;
using Sproto.Nmea.Exceptions;

#endregion

namespace Sproto.Nmea.Messages
{
	public class GnsMessage : NmeaMessage
	{
		#region Constructors

		public GnsMessage() : base(NmeaMessageType.GnssFixInformation)
		{
		}

		#endregion

		#region Methods

		public override void Parse(string sentence)
		{
			// $GNGNS,014035.00,4332.69262,S,17235.48549,E,RR,13,0.9,25.63,11.24,,*70
			//
			// .      0   1   2 3   4 5 6  7   8   9   10 11   12
			//        |   |   | |   | | |  |   |   |   |  |    |
			// $--GNS,y.y,l.l,N,l.l,E,N,XX,x.x,x.x,x.x,xx,xxxx*hh
			//
			//  0) UTC of position fix
			//  1) Latitude
			//  2) Direction of latitude:
			//     N: North
			//     S: South
			//  3) Longitude
			//  4) Direction of longitude:
			//     E: East
			//     W: West
			//  5) Mode indicator:
			//     * Variable character field with one character for each supported constellation.
			//     * First character is for GPS
			//     * Second character is for GLONASS
			//     * Subsequent characters will be added for new constellation
			//
			//     Each character will be one of the following:
			//     N = No fix. Satellite system not used in position fix, or fix not valid
			//     A = Autonomous. Satellite system used in non-differential mode in position fix
			// 	   D = Differential (including all OmniSTAR services). Satellite system used in differential mode in position fix
			//     P = Precise. Satellite system used in precision mode. Precision mode is defined as: no deliberate degradation (such as Selective Availability) and higher resolution code (P-code) is used to compute position fix
			//     R = Real Time Kinematic. Satellite system used in RTK mode with fixed integers
			//     F = Float RTK. Satellite system used in real time kinematic mode with floating integers
			//     E = Estimated (dead reckoning) Mode
			//     M = Manual Input Mode
			//     S = Simulator Mode
			//  6) Number of SVs in use, range 00–99
			//  7) HDOP calculated using all the satellites (GPS, GLONASS, and any future satellites) used in computing the solution reported in each GNS sentence.
			//  8) Orthometric height in meters (MSL reference)
			//  9) Geoidal separation in meters - the difference between the earth ellipsoid surface and mean-sea-level (geoid) surface defined by the reference datum used in the position solution
			//     “-” = mean-sea-level surface below ellipsoid.
			// 10) Age of differential data - Null if talker ID is GN, additional GNS messages follow with GP and/or GL Age of differential data
			// 11) Reference station ID1, range 0000-4095
			//     - Null if talker ID is GN, additional GNS messages follow with GP and/or GL Reference station ID
			// 12) Checksum

			var items = StartParse(sentence);

			OnNmeaMessageParsed(this);
		}

		public override void Reset()
		{
		}

		public override string ToString()
		{
			var result = $"{Type} ";
			return result;
		}

		#endregion
	}
}