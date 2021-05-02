#region References

using System;
using Sproto.Nmea.Exceptions;

#endregion

namespace Sproto.Nmea.Messages
{
	public class TxtMessage : NmeaMessage
	{
		#region Constructors

		public TxtMessage() : base(NmeaMessageType.TextTransmission)
		{
		}

		#endregion

		#region Properties

		public string Text { get; private set; }

		#endregion

		#region Methods

		public override void Parse(string sentence)
		{
			// $GPTXT,01,01,25,DR MODE - ANTENNA FAULT^21*38
			//
			// .      0  1  2  3   4
			//        |  |  |  |   |
			// $--TXT,xx,xx,xx,c-c*hh
			//
			// 0) Total Number of Sentences 01 to 99
			// 1) Sentence Number 01 to 99
			// 2) Text Identifier
			// 3) Text Message
			// 4) Checksum

			var items = StartParse(sentence);

			Text = items[3];

			OnNmeaMessageParsed(this);
		}

		public override void Reset()
		{
		}

		public override string ToString()
		{
			var result = $"{Type} Text:{Text} ";

			return result;
		}

		#endregion
	}
}