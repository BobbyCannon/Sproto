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
			// Mode Indicator:
			_modeIndicators = new Dictionary<char, string>
			{
				{ 'A', "Autonomous" },
				{ 'D', "Differential" },
				{ 'E', "Estimated" },
				{ 'F', "Float RTK" },
				{ 'M', "Manual" },
				{ 'N', "No Fix" },
				{ 'P', "Precise" },
				{ 'R', "Real Time Kinematic" },
				{ 'S', "Simulator" },
			};

			if (!string.IsNullOrEmpty(modeIndicator) && _modeIndicators.ContainsKey(modeIndicator[0]))
			{
				Mode = modeIndicator[0];
				ModeName = _modeIndicators[Mode];
			}
			else
			{
				Mode = ' ';
				ModeName = string.Empty;
			}
		}

		#endregion

		#region Properties

		public char Mode { get; }

		public string ModeName { get; }

		#endregion

		#region Methods

		public bool IsSet()
		{
			return _modeIndicators.ContainsKey(Mode);
		}

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