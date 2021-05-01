#region References

using System;
using System.Collections.Generic;
using System.Linq;
using Speedy;
using Speedy.Extensions;

#endregion

namespace Sproto.Nmea
{
	public class NmeaParser
	{
		#region Fields

		private readonly Dictionary<NmeaMessageType, NmeaMessage> _parsers;
		private readonly object _parsersLock;

		#endregion

		#region Constructors

		public NmeaParser()
		{
			_parsers = new Dictionary<NmeaMessageType, NmeaMessage>();
			_parsersLock = new object();

			var values = Enum.GetValues(typeof(NmeaMessageType))
				.Cast<NmeaMessageType>()
				.Except(new[] { NmeaMessageType.Unknown });

			values.ForEach(AddMessageParser);
		}

		#endregion

		#region Methods

		public void AddMessageParser(NmeaMessageType type)
		{
			lock (_parsersLock)
			{
				if (_parsers.ContainsKey(type))
				{
					_parsers.Remove(type);
				}

				var parser = CreateMessageParser(type);
				if (parser == null)
				{
					return;
				}

				parser.NmeaMessageParsed += OnMessageParsed;

				_parsers.AddOrUpdate(type, parser);
			}
		}

		public NmeaMessage Parse(string nmeaLine, DateTime? timestamp = null)
		{
			try
			{
				if (nmeaLine.EndsWith("\r") || nmeaLine.EndsWith("\n"))
				{
					nmeaLine = nmeaLine.TrimEnd('\r', '\n');
				}

				if (string.IsNullOrWhiteSpace(nmeaLine))
				{
					return null;
				}

				if (nmeaLine.Length < 6)
				{
					return null;
				}

				var type = GetMessageType(nmeaLine);

				lock (_parsersLock)
				{
					if (_parsers.ContainsKey(type))
					{
						var p = _parsers[type];
						if (p == null)
						{
							return null;
						}
						p.TimestampUtc = timestamp ?? TimeService.UtcNow;
						p.Parse(nmeaLine);
						return p;
					}
				}

				return null;
			}
			catch (NmeaParseChecksumException)
			{
				return null;
			}
		}

		private NmeaMessage CreateMessageParser(NmeaMessageType filter)
		{
			return filter switch
			{
				NmeaMessageType.Gbgsv => new GbgsvMessage(),
				NmeaMessageType.Glgsv => new GlgsvMessage(),
				NmeaMessageType.Gngga => new GnggaMessage(),
				NmeaMessageType.Gngll => new GngllMessage(),
				NmeaMessageType.Gngsa => new GngsaMessage(),
				NmeaMessageType.Gnrmc => new GnrmcMessage(),
				NmeaMessageType.Gntxt => new GntxtMessage(),
				NmeaMessageType.Gnvtg => new GnvtgMessage(),
				NmeaMessageType.Gpgga => new GpggaMessage(),
				NmeaMessageType.Gpgsa => new GpgsaMessage(),
				NmeaMessageType.Gpgsv => new GpgsvMessage(),
				NmeaMessageType.Gprmc => new GprmcMessage(),
				NmeaMessageType.Gpvtg => new GpvtgMessage(),
				_ => null
			};
		}

		private NmeaMessageType GetMessageType(string nmeaLine)
		{
			if (nmeaLine.Length < 6)
			{
				return NmeaMessageType.Unknown;
			}

			var type = nmeaLine.Substring(1, 5);
			return Enum.TryParse(type, true, out NmeaMessageType messageType) ? messageType : NmeaMessageType.Unknown;
		}

		private void OnMessageParsed(object sender, NmeaMessage e)
		{
			MessageParsed?.Invoke(this, e);
		}

		#endregion

		#region Events

		public event EventHandler<NmeaMessage> MessageParsed;

		#endregion
	}
}