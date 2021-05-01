namespace Sproto.Nmea
{
	public class GnrmcMessage : RmcMessage
	{
		#region Constructors

		public GnrmcMessage() : base(NmeaMessageType.Gnrmc)
		{
		}

		#endregion
	}
}