namespace Sproto.Nmea.Messages
{
	public class GrsMessage : NmeaMessage
	{
		#region Constructors

		public GrsMessage() : base(NmeaMessageType.GRS)
		{
		}

		#endregion

		#region Methods

		public override void Parse(string sentence)
		{
		}

		#endregion
	}
}