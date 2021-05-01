namespace Sproto.Nmea
{
	public class GprmcMessage : RmcMessage
	{
		#region Constructors

		public GprmcMessage() : base(NmeaMessageType.Gprmc)
		{
		}

		#endregion
	}
}