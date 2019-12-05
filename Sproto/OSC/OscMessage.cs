#region References

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Sproto.Internal;

#endregion

namespace Sproto.OSC
{
	public class OscMessage : OscPacket, IEnumerable<object>
	{
		#region Constructors

		/// <summary>
		/// Instantiates an instance of an OSC message for the provided address and arguments.
		/// </summary>
		/// <param name="address"> The address. </param>
		/// <param name="args"> The arguments. </param>
		/// <remarks>
		/// Do NOT call this constructor with an object[] unless you want a message with a single
		/// object of type object[]. Because an object[] is an object the parameter is seen as a
		/// single entry array.
		/// </remarks>
		public OscMessage(string address, params object[] args)
			: this(OscTimeTag.UtcNow, address, args)
		{
		}

		/// <summary>
		/// Instantiates an instance of an OSC message for the provided address and arguments.
		/// </summary>
		/// <param name="time"> The time. </param>
		/// <param name="address"> The address. </param>
		/// <param name="args"> The arguments. </param>
		/// <remarks>
		/// Do NOT call this constructor with an object[] unless you want a message with a single
		/// object of type object[]. Because an object[] is an object the parameter is seen as a
		/// single entry array.
		/// </remarks>
		public OscMessage(OscTimeTag time, string address, params object[] args)
		{
			Address = address;
			Arguments = new List<object>();
			Arguments.AddRange(args);
			Time = time;
		}

		#endregion

		#region Properties

		/// <summary>
		/// The address of the message.
		/// </summary>
		public string Address { get; set; }

		/// <summary>
		/// The arguments of this message.
		/// </summary>
		public List<object> Arguments { get; set; }

		/// <summary>
		/// The number of arguments in the message.
		/// </summary>
		public int Count => Arguments.Count;

		/// <summary>
		/// The argument list is empty.
		/// </summary>
		public bool IsEmpty => !Arguments.Any();

		/// <summary>
		/// Access Arguments by index
		/// </summary>
		/// <param name="index"> the index of the argument </param>
		/// <returns> argument at the supplied index </returns>
		public object this[int index] => Arguments[index];

		#endregion

		#region Methods

		/// <summary>
		/// They'll be times when you want to instantiate an message with an actually object array. Used this factory method. If you pass
		/// an object[] to the constructor it will actually be an object[] with one entry of that object[].
		/// </summary>
		/// <param name="address"> The address. </param>
		/// <param name="args"> The arguments. </param>
		/// <returns> The message for the address and arguments. </returns>
		public static OscMessage FromObjectArray(string address, IEnumerable<object> args)
		{
			return FromObjectArray(OscTimeTag.UtcNow, address, args);
		}

		/// <summary>
		/// They'll be times when you want to instantiate an message with an actually object array. Used this factory method. If you pass
		/// an object[] to the constructor it will actually be an object[] with one entry of that object[].
		/// </summary>
		/// <param name="time"> The time. </param>
		/// <param name="address"> The address. </param>
		/// <param name="args"> The arguments. </param>
		/// <returns> The message for the address and arguments. </returns>
		public static OscMessage FromObjectArray(OscTimeTag time, string address, IEnumerable<object> args)
		{
			var response = new OscMessage(time, address);
			response.Arguments.AddRange(args);
			return response;
		}

		/// <summary>
		/// Gets the argument as the specified type. Does a direct cast so if the type is wrong then it will exception.
		/// </summary>
		/// <typeparam name="T"> The type the argument is. </typeparam>
		/// <param name="index"> The index of the argument to cast. </param>
		/// <param name="defaultValue"> The default value if the argument index does not exists. </param>
		/// <returns> The typed argument. </returns>
		public T GetArgument<T>(int index, T defaultValue = default)
		{
			if (index >= Arguments.Count)
			{
				return defaultValue;
			}

			return (T) Arguments[index];
		}

		/// <inheritdoc />
		public IEnumerator<object> GetEnumerator()
		{
			return Arguments.GetEnumerator();
		}

		/// <summary>
		/// Takes in an OSC bundle package in byte form and parses it into a more usable OscBundle object
		/// </summary>
		/// <param name="time"> </param>
		/// <param name="data"> </param>
		/// <param name="length"> </param>
		/// <returns> Message containing various arguments and an address </returns>
		public static OscPacket Parse(OscTimeTag time, byte[] data, int length)
		{
			var index = 0;
			var arguments = new List<object>();
			var mainArray = arguments; // used as a reference when we are parsing arrays to Get the main array back

			// Get address
			var address = GetAddress(data, index);
			index += data.FirstIndexAfter(address.Length, x => x == ',');

			if (index % 4 != 0)
			{
				return new OscError(OscTimeTag.UtcNow, OscError.Message.InvalidMessageAddressMisAligned);
			}

			// Get type tags
			var types = GetTypes(data, index);
			index += types.Length;

			while (index % 4 != 0)
			{
				index++;
			}

			var commaParsed = false;

			foreach (var type in types)
			{
				// skip leading comma
				if (type == ',' && !commaParsed)
				{
					commaParsed = true;
					continue;
				}

				switch (type)
				{
					case '\0':
						break;

					case 'i':
						var iValue = GetInt(data, index);
						arguments.Add(iValue);
						index += 4;
						break;

					case 'f':
						var fValue = GetFloat(data, index);
						arguments.Add(fValue);
						index += 4;
						break;

					case 's':
						var sValue = GetString(data, ref index);
						arguments.Add(sValue);
						break;

					case 'b':
						var bValue = GetBlob(data, index);
						arguments.Add(bValue);
						index += 4 + bValue.Length;
						break;

					case 'h':
						var hValue = GetLong(data, index);
						arguments.Add(hValue);
						index += 8;
						break;

					case 't':
						var tValue = GetULong(data, index);
						arguments.Add(new OscTimeTag(tValue));
						index += 8;
						break;

					case 'd':
						var dValue = GetDouble(data, index);
						arguments.Add(dValue);
						index += 8;
						break;

					case 'S':
						var s2Value = GetString(data, ref index);
						arguments.Add(new OscSymbol(s2Value));
						break;

					case 'c':
						var cValue = GetChar(data, index);
						arguments.Add(cValue);
						index += 4;
						break;

					case 'r':
						var rValue = GetRgba(data, index);
						arguments.Add(rValue);
						index += 4;
						break;

					case 'm':
						var mValue = GetMidi(data, index);
						arguments.Add(mValue);
						index += 4;
						break;

					case 'T':
						arguments.Add(true);
						break;

					case 'F':
						arguments.Add(false);
						break;

					case 'N':
						arguments.Add(null);
						break;

					case 'I':
						arguments.Add(double.PositiveInfinity);
						break;

					case 'C':
						var crcValue = GetCrc(data, index);
						arguments.Add(crcValue);
						index += 2;
						break;

					case '[':
						if (arguments != mainArray)
						{
							return new OscError(OscTimeTag.UtcNow, OscError.Message.UnsupportedNestedArrays);
						}
						arguments = new List<object>(); // make arguments point to a new object array
						break;

					case ']':
						mainArray.Add(arguments); // add the array to the main array
						arguments = mainArray; // make arguments point back to the main array
						break;

					default:
						return new OscError(OscTimeTag.UtcNow, OscError.Message.UnknownTagType, type);
				}

				while (index % 4 != 0)
				{
					index++;
				}
			}

			return new OscMessage(time, address, arguments.ToArray());
		}

		/// <summary>
		/// Parse a message from a string using the default provider InvariantCulture.
		/// </summary>
		/// <param name="value"> A string containing the OSC message data. </param>
		/// <returns> The parsed OSC message. </returns>
		public new static OscPacket Parse(string value)
		{
			return Parse(value, CultureInfo.InvariantCulture);
		}

		/// <summary>
		/// Parse a message from a string using the supplied provider.
		/// </summary>
		/// <param name="value"> A string containing the OSC message data. </param>
		/// <param name="provider"> The format provider to use during parsing. </param>
		/// <returns> The parsed OSC message. </returns>
		public new static OscPacket Parse(string value, IFormatProvider provider)
		{
			return Parse(OscTimeTag.UtcNow, value, provider);
		}

		/// <summary>
		/// Parse a message from a string using the supplied provider.
		/// </summary>
		/// <param name="time"> The time to represent the message. </param>
		/// <param name="value"> A string containing the OSC message data. </param>
		/// <param name="provider"> The format provider to use during parsing. </param>
		/// <returns> The parsed OSC message. </returns>
		public new static OscPacket Parse(OscTimeTag time, string value, IFormatProvider provider)
		{
			if (string.IsNullOrWhiteSpace(value))
			{
				return new OscError(OscTimeTag.UtcNow, OscError.Message.InvalidParseOscPacketInput);
			}

			var index = value.IndexOf(',');

			if (index <= 0)
			{
				// could be an argument less message				
				index = value.Length;
			}

			var address = value.Substring(0, index).Trim();

			if (string.IsNullOrWhiteSpace(address))
			{
				return new OscError(OscTimeTag.UtcNow, OscError.Message.InvalidMessageAddressWasEmpty);
			}

			if (OscAddress.IsValidAddress(address) == false)
			{
				return new OscError(OscTimeTag.UtcNow, OscError.Message.InvalidMessageAddress);
			}

			var arguments = new List<object>();

			try
			{
				Extensions.ParseArguments(value, arguments, index + 1, provider);
			}
			catch (Exception ex)
			{
				return new OscError(OscTimeTag.UtcNow, OscError.Message.FailedParsingArguments, ex.Message);
			}

			return new OscMessage(time, address, arguments.ToArray());
		}

		public override byte[] ToByteArray()
		{
			var parts = new List<byte[]>();
			var currentList = Arguments;
			var argumentsIndex = 0;
			var typeString = ",";
			var i = 0;

			while (i < currentList.Count)
			{
				var arg = currentList[i];
				switch (arg)
				{
					case int iArg:
						typeString += "i";
						parts.Add(SetInt(iArg));
						break;

					case float sArg:
						if (float.IsPositiveInfinity(sArg) || float.IsNegativeInfinity(sArg))
						{
							typeString += "I";
						}
						else
						{
							typeString += "f";
							parts.Add(SetFloat(sArg));
						}
						break;

					case string s:
						typeString += "s";
						parts.Add(SetString(s));
						break;

					case byte[] b:
						typeString += "b";
						parts.Add(SetBlob(b));
						break;

					case long i64:
						typeString += "h";
						parts.Add(SetLong(i64));
						break;

					case ulong ui64:
						typeString += "t";
						parts.Add(SetULong(ui64));
						break;

					case OscTimeTag timeTag:
						typeString += "t";
						parts.Add(SetULong(timeTag.Value));
						break;

					case double dValue:
						if (double.IsPositiveInfinity(dValue) || double.IsNegativeInfinity(dValue))
						{
							typeString += "I";
						}
						else
						{
							typeString += "d";
							parts.Add(SetDouble(dValue));
						}
						break;

					case OscSymbol s2Value:
						typeString += "S";
						parts.Add(SetString(s2Value.Value));
						break;

					case char character:
						typeString += "c";
						parts.Add(SetChar(character));
						break;

					case OscRgba rgba:
						typeString += "r";
						parts.Add(SetRgba(rgba));
						break;

					case OscMidi midi:
						typeString += "m";
						parts.Add(SetMidi(midi));
						break;

					case OscCrc crc:
						typeString += "C";
						parts.Add(SetCrc(crc));
						break;

					case bool boolean:
						typeString += boolean ? "T" : "F";
						break;

					case null:
						typeString += "N";
						break;

					// This part handles arrays. It points currentList to the array and reSets i
					// The array is processed like normal and when it is finished we replace  
					// currentList back with Arguments and continue from where we left off
					case object[] _:
					case List<object> _:
						if (arg.GetType() == typeof(object[]))
						{
							arg = ((object[]) arg).ToList();
						}

						if (Arguments != currentList)
						{
							throw new Exception("Nested Arrays are not supported");
						}
						typeString += "[";
						currentList = (List<object>) arg;
						argumentsIndex = i;
						i = 0;
						continue;

					default:
						throw new Exception("Unable to transmit values of type " + arg.GetType());
				}

				i++;

				if (currentList != Arguments && i == currentList.Count)
				{
					// End of array, go back to main Argument list
					typeString += "]";
					currentList = Arguments;
					i = argumentsIndex + 1;
				}
			}

			var addressLen = string.IsNullOrEmpty(Address) ? 0 : Address.AlignedStringLength();
			var typeLen = typeString.AlignedStringLength();
			var total = addressLen + typeLen + parts.Sum(x => x.Length);
			var output = new byte[total];
			i = 0;

			Encoding.ASCII.GetBytes(Address).CopyTo(output, i);
			i += addressLen;

			Encoding.ASCII.GetBytes(typeString).CopyTo(output, i);
			i += typeLen;

			foreach (var part in parts)
			{
				part.CopyTo(output, i);
				i += part.Length;
			}

			return output;
		}

		public string ToHexString()
		{
			return ToString(CultureInfo.InvariantCulture, true);
		}

		public override string ToString()
		{
			return ToString(CultureInfo.InvariantCulture, false);
		}

		public string ToString(IFormatProvider provider, bool numberAsHex)
		{
			var sb = new StringBuilder();

			sb.Append(Address);

			if (IsEmpty)
			{
				return sb.ToString();
			}

			sb.Append(", ");

			ArgumentsToString(sb, numberAsHex, provider, Arguments);

			return sb.ToString();
		}

		internal static void ArgumentsToString(StringBuilder sb, bool hex, IFormatProvider provider, IEnumerable<object> args)
		{
			var first = true;

			foreach (var obj in args)
			{
				if (first == false)
				{
					sb.Append(", ");
				}
				else
				{
					first = false;
				}

				switch (obj)
				{
					case object[] objects:
						sb.Append('[');
						ArgumentsToString(sb, hex, provider, objects);
						sb.Append(']');
						break;

					case int i:
						sb.Append(hex ? $"0x{i.ToString("X8", provider)}" : i.ToString(provider));
						break;

					case long l:
						sb.Append(hex ? $"0x{l.ToString("X16", provider)}" : $"{l.ToString(provider)}L");
						break;

					case float f:
					{
						var value = f;

						if (float.IsInfinity(value) || float.IsNaN(value))
						{
							sb.Append(f.ToString(provider));
						}
						else
						{
							sb.Append(f.ToString(provider) + "f");
						}
						break;
					}

					case double d:
						sb.Append(d.ToString(provider) + "d");
						break;

					case byte b:
						sb.Append($"'{(char) b}'");
						break;

					case OscRgba rgba:
						sb.Append($"{{ Color: {rgba} }}");
						break;

					case OscTimeTag tag:
						sb.Append($"{{ Time: {tag} }}");
						break;

					case OscMidi midi:
						sb.Append($"{{ Midi: {midi} }}");
						break;

					case bool b:
						sb.Append(b.ToString());
						break;

					case null:
						sb.Append("null");
						break;

					case string sValue:
						sb.Append($"\"{sValue.Escape()}\"");
						break;

					case OscSymbol symbol:
						sb.Append(symbol.Value.Escape());
						break;

					case byte[] bytes:
						sb.Append($"{{ Blob: {bytes.ToStringBlob()} }}");
						break;

					default:
						throw new Exception($"Unsupported argument type '{obj.GetType()}'");
				}
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion
	}
}