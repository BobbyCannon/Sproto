namespace Sproto.Nmea
{
	public class GpggaMessage : GnggaMessage
	{
		#region Constructors

		public GpggaMessage() : base(NmeaMessageType.Gpgga)
		{
		}

		#endregion
	}
}