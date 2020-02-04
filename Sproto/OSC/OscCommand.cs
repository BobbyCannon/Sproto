#region References

using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#endregion

namespace Sproto.OSC
{
	public abstract class OscCommand : INotifyPropertyChanged
	{
		#region Fields

		private int _argumentIndex;
		private bool _loadingMessage;

		#endregion

		#region Constructors

		protected OscCommand(string address)
		{
			Address = address;
			OscMessage = new OscMessage(Address);

			StartArgumentProcessing();
		}

		#endregion

		#region Properties

		/// <summary>
		/// Get the address of the parsed message.
		/// </summary>
		public string Address { get; }

		/// <summary>
		/// This indicates a value has been read from the server.
		/// </summary>
		public bool HasBeenRead { get; set; }

		/// <summary>
		/// Indicates modifications has been made since loaded from the message.
		/// </summary>
		public bool HasBeenUpdated { get; set; }

		/// <summary>
		/// Gets the time of the
		/// </summary>
		public OscTimeTag Time
		{
			get => OscMessage.Time;
			set => OscMessage.Time = value;
		}

		/// <summary>
		/// The message that represents this command.
		/// </summary>
		protected OscMessage OscMessage { get; set; }

		#endregion

		#region Methods

		/// <summary>
		/// Gets an OscCommand from an OscMessage
		/// </summary>
		/// <typeparam name="T"> The type to be returned. </typeparam>
		/// <param name="message"> The message to be loaded. </param>
		/// <returns> The OscCommand with the message loaded. </returns>
		public static T FromMessage<T>(OscMessage message) where T : OscCommand, new()
		{
			var t = new T();
			t.Load(message);
			return t;
		}

		/// <summary>
		/// Gets the argument or returns the default value if the index is not found.
		/// </summary>
		/// <typeparam name="T"> The type of the argument expected. </typeparam>
		/// <param name="defaultValue"> The default value to return if not found. </param>
		/// <returns> The argument if found or default value if not. </returns>
		public T GetArgument<T>(T defaultValue = default)
		{
			return GetArgument(_argumentIndex++, defaultValue);
		}

		/// <summary>
		/// Gets the argument or returns the default value if the index is not found.
		/// </summary>
		/// <typeparam name="T"> The type of the argument expected. </typeparam>
		/// <param name="index"> The index of the argument. </param>
		/// <param name="defaultValue"> The default value to return if not found. </param>
		/// <returns> The argument if found or default value if not. </returns>
		public T GetArgument<T>(int index, T defaultValue)
		{
			return defaultValue switch
			{
				byte[] dValue => (T) (object) GetArgumentAsBlob(index, dValue),
				bool dValue => (T) (object) GetArgumentAsBoolean(index, dValue),
				byte dValue => (T) (object) GetArgumentAsByte(index, dValue),
				DateTime dValue => (T) (object) GetArgumentAsDateTime(index, dValue),
				double dValue => (T) (object) GetArgumentAsDouble(index, dValue),
				float dValue => (T) (object) GetArgumentAsFloat(index, dValue),
				Guid dValue => (T) (object) GetArgumentAsGuid(index, dValue),
				int dValue => (T) (object) GetArgumentAsInteger(index, dValue),
				long dValue => (T) (object) GetArgumentAsLong(index, dValue),
				OscTimeTag dValue => (T) (object) GetArgumentAsOscTimeTag(index, dValue),
				string dValue => (T) (object) GetArgumentAsString(index, dValue),
				TimeSpan dValue => (T) (object) GetArgumentAsTimeSpan(index, dValue),
				uint dValue => (T) (object) GetArgumentAsUnsignedInteger(index, dValue),
				ulong dValue => (T) (object) GetArgumentAsUnsignedLong(index, dValue),
				_ => (OscMessage.Arguments.Count <= index ? defaultValue : (T) OscMessage.Arguments[index])
			};
		}

		/// <summary>
		/// Gets the argument or returns the default value if the index is not found.
		/// </summary>
		/// <returns> The argument if found or default value if not. </returns>
		public byte[] GetArgumentAsBlob()
		{
			return GetArgumentAsBlob(_argumentIndex++);
		}

		/// <summary>
		/// Gets the argument or returns the default value if the index is not found.
		/// </summary>
		/// <param name="index"> The index of the argument. </param>
		/// <param name="defaultValue"> The default value to return if not found. </param>
		/// <returns> The argument if found or default value if not. </returns>
		public byte[] GetArgumentAsBlob(int index, byte[] defaultValue = default)
		{
			if (OscMessage.Arguments.Count <= index)
			{
				return defaultValue;
			}

			var value = OscMessage.Arguments[index];
			return value is byte[] aValue ? aValue : defaultValue;
		}

		/// <summary>
		/// Gets the argument or returns the default value if the index is not found.
		/// </summary>
		/// <returns> The argument if found or default value if not. </returns>
		public bool GetArgumentAsBoolean()
		{
			return GetArgumentAsBoolean(_argumentIndex++);
		}

		/// <summary>
		/// Gets the argument or returns the default value if the index is not found.
		/// </summary>
		/// <param name="index"> The index of the argument. </param>
		/// <param name="defaultValue"> The default value to return if not found. </param>
		/// <returns> The argument if found or default value if not. </returns>
		public bool GetArgumentAsBoolean(int index, bool defaultValue = default)
		{
			if (OscMessage.Arguments.Count <= index)
			{
				return defaultValue;
			}

			var value = OscMessage.Arguments[index];
			if (value is bool bValue)
			{
				return bValue;
			}

			return bool.TryParse(value.ToString(), out var result) ? result : defaultValue;
		}

		/// <summary>
		/// Gets the argument or returns the default value if the index is not found.
		/// </summary>
		/// <returns> The argument if found or default value if not. </returns>
		public byte GetArgumentAsByte()
		{
			return GetArgumentAsByte(_argumentIndex++);
		}

		/// <summary>
		/// Gets the argument or returns the default value if the index is not found.
		/// </summary>
		/// <param name="index"> The index of the argument. </param>
		/// <param name="defaultValue"> The default value to return if not found. </param>
		/// <returns> The argument if found or default value if not. </returns>
		public byte GetArgumentAsByte(int index, byte defaultValue = default)
		{
			if (OscMessage.Arguments.Count <= index)
			{
				return defaultValue;
			}

			var value = OscMessage.Arguments[index];

			return value switch
			{
				byte bValue => bValue,
				DateTime time => (byte) time.Ticks,
				OscTimeTag time => (byte) time.Value,
				char cValue => (byte) cValue,
				int iValue => (byte) iValue,
				uint uiValue => (byte) uiValue,
				long lValue => (byte) lValue,
				ulong ulValue => (byte) ulValue,
				float fValue => (byte) fValue,
				double dValue => (byte) dValue,
				_ => (byte.TryParse(value.ToString(), out var result) ? result : defaultValue)
			};
		}

		/// <summary>
		/// Gets the argument or returns the default value if the index is not found.
		/// </summary>
		/// <returns> The argument if found or default value if not. </returns>
		public DateTime GetArgumentAsDateTime()
		{
			return GetArgumentAsDateTime(_argumentIndex++);
		}

		/// <summary>
		/// Gets the argument or returns the default value if the index is not found.
		/// </summary>
		/// <param name="index"> The index of the argument. </param>
		/// <param name="defaultValue"> The default value to return if not found. </param>
		/// <returns> The argument if found or default value if not. </returns>
		public DateTime GetArgumentAsDateTime(int index, DateTime defaultValue = default)
		{
			if (OscMessage.Arguments.Count <= index)
			{
				return defaultValue;
			}

			var value = OscMessage.Arguments[index];

			return value switch
			{
				DateTime time => time,
				OscTimeTag time => time.ToDateTime(),
				byte bValue => new DateTime(bValue),
				char cValue => new DateTime(cValue),
				int iValue => new DateTime(iValue),
				uint uiValue => new DateTime(uiValue),
				long lValue => new DateTime(lValue),
				ulong ulValue => new DateTime((long) ulValue),
				float fValue => new DateTime((long) fValue),
				double dValue => new DateTime((long) dValue),
				_ => (DateTime.TryParse(value.ToString(), out var result) ? result : defaultValue)
			};
		}

		/// <summary>
		/// Gets the argument or returns the default value if the index is not found.
		/// </summary>
		/// <returns> The argument if found or default value if not. </returns>
		public double GetArgumentAsDouble()
		{
			return GetArgumentAsDouble(_argumentIndex++);
		}

		/// <summary>
		/// Gets the argument or returns the default value if the index is not found.
		/// </summary>
		/// <param name="index"> The index of the argument. </param>
		/// <param name="defaultValue"> The default value to return if not found. </param>
		/// <returns> The argument if found or default value if not. </returns>
		public double GetArgumentAsDouble(int index, double defaultValue = default)
		{
			if (OscMessage.Arguments.Count <= index)
			{
				return defaultValue;
			}

			var value = OscMessage.Arguments[index];

			if (value is OscSymbol symbol)
			{
				switch (symbol.Value)
				{
					case "Infinity":
					case "Infinityd":
						return double.PositiveInfinity;

					case "-Infinity":
					case "-Infinityd":
						return double.NegativeInfinity;
				}
			}

			return value switch
			{
				double dValue => dValue,
				DateTime time => time.Ticks,
				OscTimeTag time => time.Value,
				byte bValue => bValue,
				char cValue => cValue,
				int iValue => iValue,
				uint uiValue => uiValue,
				long lValue => lValue,
				ulong ulValue => ulValue,
				float fValue => fValue,
				_ => (double.TryParse(value.ToString(), out var result) ? result : defaultValue)
			};
		}

		/// <summary>
		/// Gets the argument or returns the default value if the index is not found.
		/// </summary>
		/// <returns> The argument if found or default value if not. </returns>
		public float GetArgumentAsFloat()
		{
			return GetArgumentAsFloat(_argumentIndex++);
		}

		/// <summary>
		/// Gets the argument or returns the default value if the index is not found.
		/// </summary>
		/// <param name="index"> The index of the argument. </param>
		/// <param name="defaultValue"> The default value to return if not found. </param>
		/// <returns> The argument if found or default value if not. </returns>
		public float GetArgumentAsFloat(int index, float defaultValue = default)
		{
			if (OscMessage.Arguments.Count <= index)
			{
				return defaultValue;
			}

			var value = OscMessage.Arguments[index];

			if (value is OscSymbol symbol)
			{
				switch (symbol.Value)
				{
					case "Infinity":
					case "Infinityd":
						return float.PositiveInfinity;

					case "-Infinity":
					case "-Infinityd":
						return float.NegativeInfinity;
				}
			}

			return value switch
			{
				float fValue => fValue,
				DateTime time => time.Ticks,
				OscTimeTag time => time.Value,
				byte bValue => bValue,
				char cValue => cValue,
				int iValue => iValue,
				uint uiValue => uiValue,
				long lValue => lValue,
				ulong ulValue => ulValue,
				double dValue => (float) dValue,
				_ => (float.TryParse(value.ToString(), out var result) ? result : defaultValue)
			};
		}

		/// <summary>
		/// Gets the argument or returns the default value if the index is not found.
		/// </summary>
		/// <returns> The argument if found or default value if not. </returns>
		public Guid GetArgumentAsGuid()
		{
			return GetArgumentAsGuid(_argumentIndex++);
		}

		/// <summary>
		/// Gets the argument or returns the default value if the index is not found.
		/// </summary>
		/// <param name="index"> The index of the argument. </param>
		/// <param name="defaultValue"> The default value to return if not found. </param>
		/// <returns> The argument if found or default value if not. </returns>
		public Guid GetArgumentAsGuid(int index, Guid defaultValue = default)
		{
			if (OscMessage.Arguments.Count <= index)
			{
				return defaultValue;
			}

			var value = OscMessage.Arguments[index];
			return Guid.TryParse(value.ToString(), out var result) ? result : defaultValue;
		}

		/// <summary>
		/// Gets the argument or returns the default value if the index is not found.
		/// </summary>
		/// <returns> The argument if found or default value if not. </returns>
		public int GetArgumentAsInteger()
		{
			return GetArgumentAsInteger(_argumentIndex++);
		}

		/// <summary>
		/// Gets the argument or returns the default value if the index is not found.
		/// </summary>
		/// <param name="index"> The index of the argument. </param>
		/// <param name="defaultValue"> The default value to return if not found. </param>
		/// <returns> The argument if found or default value if not. </returns>
		public int GetArgumentAsInteger(int index, int defaultValue = default)
		{
			if (OscMessage.Arguments.Count <= index)
			{
				return defaultValue;
			}

			var value = OscMessage.Arguments[index];

			return value switch
			{
				int iValue => iValue,
				DateTime time => (int) time.Ticks,
				OscTimeTag time => (int) time.Value,
				Enum eValue => Convert.ToInt32(eValue),
				byte bValue => bValue,
				char cValue => cValue,
				uint uiValue => (int) uiValue,
				long lValue => (int) lValue,
				ulong ulValue => (int) ulValue,
				float fValue => (int) fValue,
				double dValue => (int) dValue,
				_ => (int.TryParse(value.ToString(), out var result) ? result : defaultValue)
			};
		}

		/// <summary>
		/// Gets the argument or returns the default value if the index is not found.
		/// </summary>
		/// <returns> The argument if found or default value if not. </returns>
		public long GetArgumentAsLong()
		{
			return GetArgumentAsLong(_argumentIndex++);
		}

		/// <summary>
		/// Gets the argument or returns the default value if the index is not found.
		/// </summary>
		/// <param name="index"> The index of the argument. </param>
		/// <param name="defaultValue"> The default value to return if not found. </param>
		/// <returns> The argument if found or default value if not. </returns>
		public long GetArgumentAsLong(int index, long defaultValue = default)
		{
			if (OscMessage.Arguments.Count <= index)
			{
				return defaultValue;
			}

			var value = OscMessage.Arguments[index];

			return value switch
			{
				long lValue => lValue,
				DateTime time => time.Ticks,
				OscTimeTag time => (long) time.Value,
				Enum eValue => Convert.ToInt64(eValue),
				byte bValue => bValue,
				char cValue => cValue,
				int iValue => iValue,
				uint uiValue => uiValue,
				ulong ulValue => (long) ulValue,
				float fValue => (long) fValue,
				double dValue => (long) dValue,
				_ => (long.TryParse(value.ToString(), out var result) ? result : defaultValue)
			};
		}

		/// <summary>
		/// Gets the argument or returns the default value if the index is not found.
		/// </summary>
		/// <returns> The argument if found or default value if not. </returns>
		public OscTimeTag GetArgumentAsOscTimeTag()
		{
			return GetArgumentAsOscTimeTag(_argumentIndex++);
		}

		/// <summary>
		/// Gets the argument or returns the default value if the index is not found.
		/// </summary>
		/// <param name="index"> The index of the argument. </param>
		/// <param name="defaultValue"> The default value to return if not found. </param>
		/// <returns> The argument if found or default value if not. </returns>
		public OscTimeTag GetArgumentAsOscTimeTag(int index, OscTimeTag defaultValue = default)
		{
			if (OscMessage.Arguments.Count <= index)
			{
				return defaultValue;
			}

			var value = OscMessage.Arguments[index];

			return value switch
			{
				OscTimeTag time => time,
				DateTime time => time.ToOscTimeTag(),
				byte bValue => new OscTimeTag(bValue),
				char cValue => new OscTimeTag(cValue),
				int iValue => new OscTimeTag((ulong) iValue),
				uint uiValue => new OscTimeTag(uiValue),
				long lValue => new OscTimeTag((ulong) lValue),
				ulong ulValue => new OscTimeTag(ulValue),
				float fValue => new OscTimeTag((ulong) fValue),
				double dValue => new OscTimeTag((ulong) dValue),
				_ => (OscTimeTag.TryParse(value.ToString(), out var result) ? result : defaultValue)
			};
		}

		/// <summary>
		/// Gets the argument or returns the default value if the index is not found.
		/// </summary>
		/// <returns> The argument if found or default value if not. </returns>
		public string GetArgumentAsString()
		{
			return GetArgumentAsString(_argumentIndex++);
		}

		/// <summary>
		/// Gets the argument or returns the default value if the index is not found.
		/// </summary>
		/// <param name="index"> The index of the argument. </param>
		/// <param name="defaultValue"> The default value to return if not found. </param>
		/// <returns> The argument if found or default value if not. </returns>
		public string GetArgumentAsString(int index, string defaultValue = default)
		{
			if (OscMessage.Arguments.Count <= index)
			{
				return defaultValue;
			}

			var value = OscMessage.Arguments[index];
			return value is string sValue ? sValue : value.ToString();
		}

		/// <summary>
		/// Gets the argument or returns the default value if the index is not found.
		/// </summary>
		/// <returns> The argument if found or default value if not. </returns>
		public TimeSpan GetArgumentAsTimeSpan()
		{
			return GetArgumentAsTimeSpan(_argumentIndex++);
		}

		/// <summary>
		/// Gets the argument or returns the default value if the index is not found.
		/// </summary>
		/// <param name="index"> The index of the argument. </param>
		/// <param name="defaultValue"> The default value to return if not found. </param>
		/// <returns> The argument if found or default value if not. </returns>
		public TimeSpan GetArgumentAsTimeSpan(int index, TimeSpan defaultValue = default)
		{
			if (OscMessage.Arguments.Count <= index)
			{
				return defaultValue;
			}

			var value = OscMessage.Arguments[index];

			return value switch
			{
				DateTime time => time.TimeOfDay,
				OscTimeTag time => time.ToDateTime().TimeOfDay,
				byte bValue => new TimeSpan(bValue),
				char cValue => new TimeSpan(cValue),
				int iValue => new TimeSpan(iValue),
				uint uiValue => new TimeSpan(uiValue),
				long lValue => new TimeSpan(lValue),
				ulong ulValue => new TimeSpan((long) ulValue),
				float fValue => new TimeSpan((long) fValue),
				double dValue => new TimeSpan((long) dValue),
				_ => (TimeSpan.TryParse(value.ToString(), out var result) ? result : defaultValue)
			};
		}

		/// <summary>
		/// Gets the argument or returns the default value if the index is not found.
		/// </summary>
		/// <returns> The argument if found or default value if not. </returns>
		public uint GetArgumentAsUnsignedInteger()
		{
			return GetArgumentAsUnsignedInteger(_argumentIndex++);
		}

		/// <summary>
		/// Gets the argument or returns the default value if the index is not found.
		/// </summary>
		/// <param name="index"> The index of the argument. </param>
		/// <param name="defaultValue"> The default value to return if not found. </param>
		/// <returns> The argument if found or default value if not. </returns>
		public uint GetArgumentAsUnsignedInteger(int index, uint defaultValue = default)
		{
			if (OscMessage.Arguments.Count <= index)
			{
				return defaultValue;
			}

			var value = OscMessage.Arguments[index];

			return value switch
			{
				uint uiValue => uiValue,
				DateTime time => (uint) time.Ticks,
				OscTimeTag time => (uint) time.Value,
				Enum eValue => Convert.ToUInt32(eValue),
				byte bValue => bValue,
				char cValue => cValue,
				int iValue => (uint) iValue,
				long lValue => (uint) lValue,
				ulong ulValue => (uint) ulValue,
				float fValue => (uint) fValue,
				double dValue => (uint) dValue,
				_ => (uint.TryParse(value.ToString(), out var result) ? result : defaultValue)
			};
		}

		/// <summary>
		/// Gets the argument or returns the default value if the index is not found.
		/// </summary>
		/// <returns> The argument if found or default value if not. </returns>
		public ulong GetArgumentAsUnsignedLong()
		{
			return GetArgumentAsUnsignedLong(_argumentIndex++);
		}

		/// <summary>
		/// Gets the argument or returns the default value if the index is not found.
		/// </summary>
		/// <param name="index"> The index of the argument. </param>
		/// <param name="defaultValue"> The default value to return if not found. </param>
		/// <returns> The argument if found or default value if not. </returns>
		public ulong GetArgumentAsUnsignedLong(int index, ulong defaultValue = default)
		{
			if (OscMessage.Arguments.Count <= index)
			{
				return defaultValue;
			}

			var value = OscMessage.Arguments[index];

			return value switch
			{
				ulong ulValue => ulValue,
				DateTime time => (ulong) time.Ticks,
				OscTimeTag time => time.Value,
				Enum eValue => Convert.ToUInt64(eValue),
				byte bValue => bValue,
				char cValue => cValue,
				int iValue => (ulong) iValue,
				uint uiValue => uiValue,
				long lValue => (ulong) lValue,
				float fValue => (ulong) fValue,
				double dValue => (ulong) dValue,
				_ => (ulong.TryParse(value.ToString(), out var result) ? result : defaultValue)
			};
		}

		public bool Load(OscMessage message)
		{
			_loadingMessage = true;

			try
			{
				OscMessage = message;
				LoadMessage();
				HasBeenRead = true;
				HasBeenUpdated = false;
				return true;
			}
			finally
			{
				_loadingMessage = false;
			}
		}

		public virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			if (propertyName != nameof(HasBeenUpdated))
			{
				HasBeenUpdated = true;
			}

			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		/// <summary>
		/// Sets the arguments of the OscMessage.
		/// </summary>
		/// <param name="arguments"> The arguments for the OscMessage. </param>
		public void SetArguments(params object[] arguments)
		{
			OscMessage.SetArguments(arguments);
		}

		/// <summary>
		/// Resets the index for sequential argument processing. Call this before calling "GetArgument" methods that do *not* provide an index.
		/// </summary>
		public void StartArgumentProcessing()
		{
			_argumentIndex = 0;
		}

		/// <summary>
		/// Converts the OscCommand into an OscBundle.
		/// </summary>
		/// <param name="time"> On optional time for the bundle. Defaults to OscTimeTag.UtcNow. </param>
		/// <returns> The OscBundle containing this OscCommand as an OscMessage. </returns>
		public virtual OscBundle ToBundle(OscTimeTag? time = null)
		{
			return new OscBundle(time ?? Time, ToMessage());
		}

		/// <summary>
		/// Converts this OSC command to an OSC Message.
		/// </summary>
		/// <param name="includeArguments"> Option to include arguments in message. Defaults to true. If true then UpdateMessage will be called to populate the message. </param>
		/// <returns> The OSC message. </returns>
		public OscMessage ToMessage(bool includeArguments = true)
		{
			if (!includeArguments)
			{
				return new OscMessage(Time, Address);
			}

			if (!_loadingMessage)
			{
				UpdateMessage();
				OscMessage.Time = Time;
			}

			return OscMessage;
		}

		/// <summary>
		/// Returns the OscMessage string value.
		/// </summary>
		/// <returns> The string value in OscMessage format. </returns>
		public override string ToString()
		{
			if (!_loadingMessage)
			{
				UpdateMessage();
			}

			return OscMessage.ToString();
		}

		/// <summary>
		/// Reloads the original message which undo all changes.
		/// </summary>
		public void UndoChanges()
		{
			// Reload the original message, resetting the state.
			Load(OscMessage);
		}

		protected abstract void LoadMessage();

		protected abstract void UpdateMessage();

		#endregion

		#region Events

		public event PropertyChangedEventHandler PropertyChanged;

		#endregion
	}
}