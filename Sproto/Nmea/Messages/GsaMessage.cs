#region References

using System;
using System.Collections.Generic;
using System.Linq;
using Sproto.Nmea.Exceptions;

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

			var items = StartParse(sentence);

			AutoSelection = items[0];
			Fix3D = items[1];

			AddPrn(items[2]);
			AddPrn(items[3]);
			AddPrn(items[4]);
			AddPrn(items[5]);
			AddPrn(items[6]);
			AddPrn(items[7]);
			AddPrn(items[8]);
			AddPrn(items[9]);
			AddPrn(items[10]);
			AddPrn(items[11]);
			AddPrn(items[12]);
			AddPrn(items[13]);

			PositionDilutionOfPrecision = Convert.ToDouble(GetValueOrDefault(items, 14, "0"));
			HorizontalDilutionOfPrecision = Convert.ToDouble(GetValueOrDefault(items, 15, "0"));
			VerticalDilutionOfPrecision = Convert.ToDouble(GetValueOrDefault(items, 16, "0"));
		}

		public override void Reset()
		{
			PrnsOfSatellitesUsedForFix.Clear();
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