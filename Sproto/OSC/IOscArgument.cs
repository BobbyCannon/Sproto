namespace Sproto.OSC
{
	public interface IOscArgument
	{
		#region Methods

		char GetOscBinaryType();

		string GetOscStringType();

		byte[] GetOscValueBytes();

		string GetOscValueString();

		void ParseOscValue(byte[] value, int index);

		void ParseOscValue(string value);

		#endregion
	}
}