#region References

using System;
using System.Collections.Generic;
using Sproto.Nmea.Exceptions;

#endregion

namespace Sproto.Nmea.Messages
{
	public class GsvMessage : NmeaMessage
	{
		#region Constructors

		public GsvMessage() : base(NmeaMessageType.DetailedSatelliteData)
		{
			Satellites = new List<Satellite>();
		}

		#endregion

		#region Properties

		public int NumberOfSatellitesInView { get; private set; }

		public int NumberOfSentences { get; private set; }

		public List<Satellite> Satellites { get; }

		public int SentenceNr { get; private set; }

		#endregion

		#region Methods

		public override void Parse(string sentence)
		{
			// $GPGSV,3,1,10,01,50,304,26,03,24,245,16,08,56,204,28,10,21,059,20*77
			//
			// .      0 1 2 3 4 5 6     n
			//        | | | | | | |     |
			// $--GSV,x,x,x,x,x,x,x,...*hh
			//
			// 0) total number of messages
			// 1) message number
			// 2) satellites in view
			// 3) satellite number
			// 4) elevation in degrees
			// 5) azimuth in degrees to true
			// 6) SNR in dB
			//    more satellite infos like 4)-7)
			// n) Checksum

			var items = StartParse(sentence);

			NumberOfSentences = Convert.ToInt32(items[0]);
			SentenceNr = Convert.ToInt32(items[1]);
			NumberOfSatellitesInView = Convert.ToInt32(items[2]);

			var satelliteCount = GetSatelliteCount(
				Convert.ToInt32(NumberOfSatellitesInView),
				Convert.ToInt32(NumberOfSentences),
				Convert.ToInt32(SentenceNr));

			for (var i = 0; i < satelliteCount; i++)
			{
				Satellites.Add(
					new Satellite
					{
						SatellitePrnNumber = items[3 + i * 4 + 0],
						ElevationDegrees = items[3 + i * 4 + 1],
						AzimuthDegrees = items[3 + i * 4 + 2],
						SignalStrength = items[3 + i * 4 + 3]
					});
			}

			if (NumberOfSentences == SentenceNr)
			{
				OnNmeaMessageParsed(this);

				Satellites.Clear();
			}
		}

		public override void Reset()
		{
		}

		public override string ToString()
		{
			var result = $"{Type} InView:{NumberOfSatellitesInView} ";

			foreach (var s in Satellites)
			{
				result += $"{s.SatellitePrnNumber}: Azi={s.AzimuthDegrees}° Ele={s.ElevationDegrees}° Str={s.SignalStrength}; ";
			}

			return result;
		}

		private int GetSatelliteCount(int numberOfSatellitesInView, int numberOfSentences, int sentenceNr)
		{
			if (numberOfSentences != sentenceNr)
			{
				return 4;
			}

			return numberOfSatellitesInView - (sentenceNr - 1) * 4;
		}

		#endregion
	}
}