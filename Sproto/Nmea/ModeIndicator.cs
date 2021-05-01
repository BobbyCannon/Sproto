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
			_modeIndicators = new Dictionary<char, string>();
			_modeIndicators.Add('A', "Autonomous");
			_modeIndicators.Add('D', "Differential");
			_modeIndicators.Add('E', "Estimated(dead reckoning) mode");
			_modeIndicators.Add('M', "Manual input");
			_modeIndicators.Add('N', "Data not valid");
			_modeIndicators.Add('*', "Not implemented");

			if (!string.IsNullOrEmpty(modeIndicator)
				&& _modeIndicators.ContainsKey(modeIndicator[0]))
			{
				Mode = _modeIndicators[modeIndicator[0]];
			}
			else
			{
				Mode = _modeIndicators['*'];
			}
		}

		#endregion

		#region Properties

		public string Mode { get; }

		#endregion

		#region Methods

		public bool IsValid()
		{
			return Mode != _modeIndicators['N'];
		}

		public override string ToString()
		{
			return Mode;
		}

		#endregion
	}
}