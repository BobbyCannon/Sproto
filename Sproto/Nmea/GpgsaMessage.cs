namespace Sproto.Nmea
{
	public class GpgsaMessage : GsaMessage
	{
		#region Constructors

		public GpgsaMessage() : base(NmeaMessageType.Gpgsa)
		{
		}

		#endregion
	}
}