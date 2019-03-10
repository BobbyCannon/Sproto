﻿namespace Sproto.OSC
{
	public class OscDataReceivedEventArgs
	{
		#region Constructors

		public OscDataReceivedEventArgs(byte[] buffer, int length)
		{
			Buffer = buffer;
			Length = length;
		}

		#endregion

		#region Properties

		public byte[] Buffer { get; }

		public int Length { get; }

		#endregion
	}
}