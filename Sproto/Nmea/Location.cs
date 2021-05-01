﻿#region References

using System;

#endregion

namespace Sproto.Nmea
{
	/// <summary>
	/// Position - location
	/// </summary>
	public class Location
	{
		#region Constructors

		public Location(string degree)
		{
			Degree = degree;
		}

		#endregion

		#region Properties

		public string Degree { get; }

		#endregion

		#region Methods

		/// <summary>
		/// XXYY.YYYY = XX + (YYYYYY / 600000) graden.
		/// (d)dd + (mm.mmmm/60) (* -1 for W and S)
		/// </summary>
		/// <returns> </returns>
		public double ToDecimal()
		{
			if (string.IsNullOrEmpty(Degree))
			{
				return -1;
			}

			var list = Degree.Split('.');
			var minuteMajor = list[0].Substring(list[0].Length - 2);
			var degree = list[0].Substring(0, list[0].Length - 2);
			var nesw = list[1].Substring(list[1].Length - 1);
			var minuteMinor = list[1].Substring(0, list[1].Length - 1);
			var minute = minuteMajor + "." + minuteMinor;
			var plusMinus = nesw == "S" || nesw == "W" ? -1 : 1;
			var result = (Convert.ToDouble(degree) + Convert.ToDouble(minute) / 60.0) * plusMinus;
			return result;
		}

		public override string ToString()
		{
			return ToDecimal().ToString("N8");
		}

		#endregion
	}
}