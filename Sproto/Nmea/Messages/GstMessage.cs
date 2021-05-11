namespace Sproto.Nmea.Messages
{
	public class GstMessage : NmeaMessage
	{
		#region Constructors

		public GstMessage() : base(NmeaMessageType.GST)
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