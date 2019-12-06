#region References

using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#endregion

namespace Sproto.OSC
{
	public abstract class OscCommand : INotifyPropertyChanged
	{
		#region Constructors

		protected OscCommand(string address)
		{
			Address = address;
			OscMessage = new OscMessage(Address);
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

		public IEnumerable<object> GetArguments(params object[] collection)
		{
			var response = new List<object>();
			foreach (var item in collection)
			{
				if (item is IOscArrayableValue arrayable)
				{
					response.AddRange(arrayable.ToArray());
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

		public bool ReadBoolean(ref int index)
		{
			return OscMessage.Arguments[index] is bool ? (bool) OscMessage.Arguments[index++] : bool.Parse(OscMessage.Arguments[index++].ToString());
		}

		public double ReadDouble(ref int index)
		{
			return OscMessage.Arguments[index] is double ? (double) OscMessage.Arguments[index++] : double.Parse(OscMessage.Arguments[index++].ToString());
		}

		public float ReadFloat(ref int index)
		{
			return OscMessage.Arguments[index] is float ? (float) OscMessage.Arguments[index++] : float.Parse(OscMessage.Arguments[index++].ToString());
		}

		public int ReadInteger(ref int index)
		{
			return OscMessage.Arguments[index] is int ? (int) OscMessage.Arguments[index++] : int.Parse(OscMessage.Arguments[index++].ToString());
		}

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
		/// <typeparam name="T"> The type of the argument expected. </typeparam>
		/// <param name="index"> The index of the argument. </param>
		/// <param name="defaultValue"> The default value to return if not found. </param>
		/// <returns> The argument if found or default value if not. </returns>
		protected T GetArgument<T>(int index, T defaultValue)
		{
			return OscMessage.Arguments.Count <= index ? defaultValue : (T) OscMessage.Arguments[index];
		}

		/// <summary>
		/// Gets the argument or returns the default value if the index is not found.
		/// </summary>
		/// <param name="index"> The index of the argument. </param>
		/// <param name="defaultValue"> The default value to return if not found. </param>
		/// <returns> The argument if found or default value if not. </returns>
		protected double GetArgument(int index, double defaultValue)
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