#region References

using System;
using System.Collections.Generic;
using System.Linq;

#endregion

namespace Sproto.Nmea
{
	public abstract class GsaMessage : NmeaMessage
	{
		#region Constructors

		protected GsaMessage(NmeaMessageType type) : base(type)
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
		/// <param name="prn"></param>
		public void AddPrn(string prn)
		{
			if (!string.IsNullOrEmpty(prn))
			{
				PrnsOfSatellitesUsedForFix.Add(Convert.ToInt32(prn));
			}
		}

		public override void Parse(string nmeaLine)
		{
			if (PrnsOfSatellitesUsedForFix.Any(x => x <= 32))
			{
				PrnsOfSatellitesUsedForFix.Sort();

				OnNmeaMessageParsed(this);

				PrnsOfSatellitesUsedForFix.Clear();
			}

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

			PositionDilutionOfPrecision = Convert.ToDouble(GetValueOrDefault(items[14], "0"));
			HorizontalDilutionOfPrecision = Convert.ToDouble(GetValueOrDefault(items[15], "0"));
			VerticalDilutionOfPrecision = Convert.ToDouble(GetValueOrDefault(items[16], "0"));
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