#region References

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;

#endregion

namespace Sproto.OSC
{
	public abstract class OscCommand : INotifyPropertyChanged
	{
		#region Fields

		private int _argumentIndex;

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
		/// The message that represents this command.
		/// </summary>
		protected OscMessage OscMessage { get; set; }

		#endregion

		#region Methods

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
		public T GetArgument<T>(T defaultValue)
		{
			_argumentIndex++;
			return GetArgument(_argumentIndex, defaultValue);
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
			return OscMessage.Arguments.Count <= index ? defaultValue : (T) OscMessage.Arguments[index];
		}

		/// <summary>
		/// Gets the argument or returns the default value if the index is not found.
		/// </summary>
		/// <param name="defaultValue"> The default value to return if not found. </param>
		/// <returns> The argument if found or default value if not. </returns>
		public double GetArgument(double defaultValue)
		{
			_argumentIndex++;
			return GetArgument(_argumentIndex, defaultValue);
		}

		/// <summary>
		/// Gets the argument or returns the default value if the index is not found.
		/// </summary>
		/// <param name="index"> The index of the argument. </param>
		/// <param name="defaultValue"> The default value to return if not found. </param>
		/// <returns> The argument if found or default value if not. </returns>
		public double GetArgument(int index, double defaultValue)
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
					case "Infinityd":
						return double.PositiveInfinity;

					case "-Infinityd":
						return double.NegativeInfinity;
				}
			}

			return (double) value;
		}

		public byte[] GetArgumentAsBlob()
		{
			_argumentIndex++;
			return GetArgumentAsBlob(_argumentIndex);
		}

		public byte[] GetArgumentAsBlob(int index)
		{
			return OscMessage.Arguments[index] is byte[] ? (byte[]) OscMessage.Arguments[index] : throw new InvalidDataException("The data type is not of type blob.");
		}

		public bool GetArgumentAsBoolean()
		{
			_argumentIndex++;
			return GetArgumentAsBoolean(_argumentIndex);
		}

		public bool GetArgumentAsBoolean(int index)
		{
			return OscMessage.Arguments[index] is bool ? (bool) OscMessage.Arguments[index] : bool.Parse(OscMessage.Arguments[index].ToString());
		}

		public byte GetArgumentAsByte()
		{
			_argumentIndex++;
			return GetArgumentAsByte(_argumentIndex);
		}

		public byte GetArgumentAsByte(int index)
		{
			return OscMessage.Arguments[index] is char ? (byte) (char) OscMessage.Arguments[index] : byte.Parse(OscMessage.Arguments[index].ToString());
		}

		public DateTime GetArgumentAsDateTime()
		{
			_argumentIndex++;
			return GetArgumentAsDateTime(_argumentIndex);
		}

		public DateTime GetArgumentAsDateTime(int index)
		{
			return OscMessage.Arguments[index] is OscTimeTag ? ((OscTimeTag) OscMessage.Arguments[index]).ToDateTime() : DateTime.Parse(OscMessage.Arguments[index].ToString());
		}

		public double GetArgumentAsDouble()
		{
			_argumentIndex++;
			return GetArgumentAsDouble(_argumentIndex);
		}

		public double GetArgumentAsDouble(int index)
		{
			return OscMessage.Arguments[index] is double ? (double) OscMessage.Arguments[index] : double.Parse(OscMessage.Arguments[index].ToString());
		}

		public float GetArgumentAsFloat()
		{
			_argumentIndex++;
			return GetArgumentAsFloat(_argumentIndex);
		}

		public float GetArgumentAsFloat(int index)
		{
			return OscMessage.Arguments[index] is float ? (float) OscMessage.Arguments[index] : float.Parse(OscMessage.Arguments[index].ToString());
		}

		public int GetArgumentAsInteger()
		{
			_argumentIndex++;
			return GetArgumentAsInteger(_argumentIndex);
		}

		public int GetArgumentAsInteger(int index)
		{
			return OscMessage.Arguments[index] is int ? (int) OscMessage.Arguments[index] : int.Parse(OscMessage.Arguments[index].ToString());
		}

		public long GetArgumentAsLong()
		{
			_argumentIndex++;
			return GetArgumentAsLong(_argumentIndex);
		}

		public long GetArgumentAsLong(int index)
		{
			return OscMessage.Arguments[index] is long ? (long) OscMessage.Arguments[index] : long.Parse(OscMessage.Arguments[index].ToString());
		}

		public string GetArgumentAsString()
		{
			_argumentIndex++;
			return GetArgumentAsString(_argumentIndex);
		}

		public string GetArgumentAsString(int index)
		{
			return OscMessage.Arguments[index] is string ? (string) OscMessage.Arguments[index] : OscMessage.Arguments[index].ToString();
		}

		public IEnumerable<object> GetArguments(params object[] collection)
		{
			var response = new List<object>();
			foreach (var item in collection)
			{
				if (item is IOscArrayableValue arrayValue)
				{
					response.AddRange(arrayValue.ToArray());
				}
				else
				{
					response.Add(item);
				}
			}
			return response;
		}

		public bool Load(OscMessage message)
		{
			OscMessage = message;
			LoadMessage();
			HasBeenRead = true;
			HasBeenUpdated = false;
			return true;
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
		/// Resets the index for sequential argument processing. Call this before calling "GetArgument" methods that do *not* provide an index.
		/// </summary>
		public void StartArgumentProcessing()
		{
			_argumentIndex = -1;
		}

		/// <summary>
		/// Converts the OscCommand into an OscBundle.
		/// </summary>
		/// <param name="time"> On optional time for the bundle. Defaults to OscTimeTag.UtcNow. </param>
		/// <returns> The OscBundle containing this OscCommand as an OscMessage. </returns>
		public virtual OscBundle ToBundle(OscTimeTag? time = null)
		{
			return new OscBundle(time ?? OscTimeTag.UtcNow, ToMessage());
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
				return new OscMessage(OscTimeTag.UtcNow, Address);
			}

			UpdateMessage();
			return OscMessage;
		}

		/// <summary>
		/// Returns the OscMessage string value.
		/// </summary>
		/// <returns> The string value in OscMessage format. </returns>
		public override string ToString()
		{
			UpdateMessage();
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

		/// <summary>
		/// Gets the argument or returns the default value if the index is not found.
		/// </summary>
		/// <param name="index"> The index of the argument. </param>
		/// <param name="defaultValue"> The default value to return if not found. </param>
		/// <returns> The argument if found or default value if not. </returns>
		protected float GetArgument(int index, float defaultValue)
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
					case "Infinityd":
						return float.PositiveInfinity;

					case "-Infinityd":
						return float.NegativeInfinity;
				}
			}

			return (float) value;
		}

		protected abstract void LoadMessage();

		protected abstract void UpdateMessage();

		#endregion

		#region Events

		public event PropertyChangedEventHandler PropertyChanged;

		#endregion
	}
}