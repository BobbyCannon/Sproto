#region References

using System.Collections.Generic;

#endregion

namespace Sproto.OSC
{
	public interface IOscArrayableValue
	{
		#region Methods

		IEnumerable<object> ToArray();

		#endregion
	}
}