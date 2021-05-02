#region References

using System.Collections.Generic;

#endregion

namespace Sproto.Nmea
{
	public class ModeIndicator
	{
		#region Fields

		private readonly Dictionary<char, string> _modeIndicators;

		#endregion

		#region Constructors

		public ModeIndicator(string modeIndicator)
		{
			_modeIndicators = new Dictionary<char, string>
			{
				{ 'A', "Autonomous" },
				{ 'D', "Differential" },
				{ 'E', "Estimated(dead reckoning) mode" },
				{ 'M', "Manual input" },
				{ 'N', "Data not valid" },
				{ '*', "Not implemented" }
			};

			if (!string.IsNullOrEmpty(modeIndicator) && _modeIndicators.ContainsKey(modeIndicator[0]))
			{
				Mode = modeIndicator[0];
				ModeName = _modeIndicators[Mode];
			}
			else
			{
				Mode = '*';
				ModeName = _modeIndicators[Mode];
			}
		}

		#endregion

		#region Properties

		public char Mode { get; }

		public string ModeName { get; }

		#endregion

		#region Methods

		public bool IsValid()
		{
			return Mode != 'N';
		}

		public override string ToString()
		{
			return Mode.ToString();
		}

		#endregion
	}
}