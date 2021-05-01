namespace Sproto.Nmea
{
	public class GpgsvMessage : GsvMessage
	{
		#region Constructors

		public GpgsvMessage() : base(NmeaMessageType.Gpgsv)
		{
		}

		#endregion
	}
}