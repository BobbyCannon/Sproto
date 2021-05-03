#region References

using System;
using System.Collections.Generic;

#endregion

namespace Sproto.Nmea.Messages
{
	public class GsaMessage : NmeaMessage
	{
		#region Constructors

		public GsaMessage() : base(NmeaMessageType.OverallSatelliteData)
		{
			PrnsOfSatellitesUsedForFix = new List<int>();
		}

		#endregion

		#region Properties

		public string AutoSelection { get; private set; }

		public string Fix3D { get; private set; }

		public double HorizontalDilutionOfPrecision { get; private set; }

		public double PositionDilutionOfPrecision { get; private set; }

		/// <summary>
		/// Pseudo-Random Noise for satellites used for fix.
		/// </summary>
		public List<int> PrnsOfSatellitesUsedForFix { get; }

		public double VerticalDilutionOfPrecision { get; private set; }

		#endregion

		#region Methods

		/// <summary>
		/// Add pseudo-random noise for a satellite.
		/// </summary>
		/// <param name="prn"> </param>
		public void AddPrn(string prn)
		{
			if (!string.IsNullOrEmpty(prn))
			{
				PrnsOfSatellitesUsedForFix.Add(Convert.ToInt32(prn));
			}
		}

		public override void Parse(string sentence)
		{
			// $GNGSA,A,3,01,18,32,08,11,,,,,,,,6.16,1.86,5.88*16
			//
			//.       0 1 2                           14  15  16  17
			//	      | | |                           |   |   |   |
			// $--GSA,a,a,x,x,x,x,x,x,x,x,x,x,x,x,x,x,x.x,x.x,x.x*hh
			//
			// 0) Selection mode
			// 1) Mode
			// 2) ID of 1st satellite used for fix
			// 3) ID of 2nd satellite used for fix
			// ...
			// 13) ID of 12th satellite used for fix
			// 14) PDOP in meters
			// 15) HDOP in meters
			// 16) VDOP in meters
			// 17) Checksum

			StartParse(sentence);

			AutoSelection = GetArgument(0);
			Fix3D = GetArgument(1);

			AddPrn(GetArgument(2));
			AddPrn(GetArgument(3));
			AddPrn(GetArgument(4));
			AddPrn(GetArgument(5));
			AddPrn(GetArgument(6));
			AddPrn(GetArgument(7));
			AddPrn(GetArgument(8));
			AddPrn(GetArgument(9));
			AddPrn(GetArgument(10));
			AddPrn(GetArgument(11));
			AddPrn(GetArgument(12));
			AddPrn(GetArgument(13));

			PositionDilutionOfPrecision = Convert.ToDouble(GetArgument(14, "0"));
			HorizontalDilutionOfPrecision = Convert.ToDouble(GetArgument(15, "0"));
			VerticalDilutionOfPrecision = Convert.ToDouble(GetArgument(16, "0"));
		}

		public override void Reset()
		{
			PrnsOfSatellitesUsedForFix.Clear();
			base.Reset();
		}

		public override string ToString()
		{
			var prnsOfSatellitesUsedForFix = string.Empty;

			foreach (var prn in PrnsOfSatellitesUsedForFix)
			{
				prnsOfSatellitesUsedForFix += $"{prn} ";
			}

			prnsOfSatellitesUsedForFix = prnsOfSatellitesUsedForFix.Trim();

			var result = $"{Type} AutoSelection:{AutoSelection} Fix3D:{Fix3D} Prns:{prnsOfSatellitesUsedForFix} PDop:{PositionDilutionOfPrecision:N1} HDop:{HorizontalDilutionOfPrecision:N1} VDop:{VerticalDilutionOfPrecision:N1} ";

			return result;
		}

		#endregion
	}
}