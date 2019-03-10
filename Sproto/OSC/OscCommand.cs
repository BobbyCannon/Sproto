﻿#region References

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
		/// Gets the timestamp of the parsed message.
		/// </summary>
		public OscTimeTag Timestamp { get; set; }

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
			var date = time ?? Timestamp;
			return new OscBundle(date.Value, ToMessage());
		}

		public OscMessage ToMessage(bool includeData = true)
		{
			if (!includeData)
			{
				return new OscMessage(Address);
			}

			UpdateMessage();
			return OscMessage;
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