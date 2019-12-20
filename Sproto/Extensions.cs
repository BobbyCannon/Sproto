#region References

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO.Ports;
using System.Linq;
using System.Reflection;
using System.Text;
using Sproto.OSC;

#endregion

namespace Sproto
{
	public static class Extensions
	{
		#region Fields

		private static readonly ushort[] _crcTable =
		{
			0x0000, 0x1189, 0x2312, 0x329B, 0x4624, 0x57AD, 0x6536, 0x74BF,
			0x8C48, 0x9DC1, 0xAF5A, 0xBED3, 0xCA6C, 0xDBE5, 0xE97E, 0xF8F7,
			0x1081, 0x0108, 0x3393, 0x221A, 0x56A5, 0x472C, 0x75B7, 0x643E,
			0x9CC9, 0x8D40, 0xBFDB, 0xAE52, 0xDAED, 0xCB64, 0xF9FF, 0xE876,
			0x2102, 0x308B, 0x0210, 0x1399, 0x6726, 0x76AF, 0x4434, 0x55BD,
			0xAD4A, 0xBCC3, 0x8E58, 0x9FD1, 0xEB6E, 0xFAE7, 0xC87C, 0xD9F5,
			0x3183, 0x200A, 0x1291, 0x0318, 0x77A7, 0x662E, 0x54B5, 0x453C,
			0xBDCB, 0xAC42, 0x9ED9, 0x8F50, 0xFBEF, 0xEA66, 0xD8FD, 0xC974,
			0x4204, 0x538D, 0x6116, 0x709F, 0x0420, 0x15A9, 0x2732, 0x36BB,
			0xCE4C, 0xDFC5, 0xED5E, 0xFCD7, 0x8868, 0x99E1, 0xAB7A, 0xBAF3,
			0x5285, 0x430C, 0x7197, 0x601E, 0x14A1, 0x0528, 0x37B3, 0x263A,
			0xDECD, 0xCF44, 0xFDDF, 0xEC56, 0x98E9, 0x8960, 0xBBFB, 0xAA72,
			0x6306, 0x728F, 0x4014, 0x519D, 0x2522, 0x34AB, 0x0630, 0x17B9,
			0xEF4E, 0xFEC7, 0xCC5C, 0xDDD5, 0xA96A, 0xB8E3, 0x8A78, 0x9BF1,
			0x7387, 0x620E, 0x5095, 0x411C, 0x35A3, 0x242A, 0x16B1, 0x0738,
			0xFFCF, 0xEE46, 0xDCDD, 0xCD54, 0xB9EB, 0xA862, 0x9AF9, 0x8B70,
			0x8408, 0x9581, 0xA71A, 0xB693, 0xC22C, 0xD3A5, 0xE13E, 0xF0B7,
			0x0840, 0x19C9, 0x2B52, 0x3ADB, 0x4E64, 0x5FED, 0x6D76, 0x7CFF,
			0x9489, 0x8500, 0xB79B, 0xA612, 0xD2AD, 0xC324, 0xF1BF, 0xE036,
			0x18C1, 0x0948, 0x3BD3, 0x2A5A, 0x5EE5, 0x4F6C, 0x7DF7, 0x6C7E,
			0xA50A, 0xB483, 0x8618, 0x9791, 0xE32E, 0xF2A7, 0xC03C, 0xD1B5,
			0x2942, 0x38CB, 0x0A50, 0x1BD9, 0x6F66, 0x7EEF, 0x4C74, 0x5DFD,
			0xB58B, 0xA402, 0x9699, 0x8710, 0xF3AF, 0xE226, 0xD0BD, 0xC134,
			0x39C3, 0x284A, 0x1AD1, 0x0B58, 0x7FE7, 0x6E6E, 0x5CF5, 0x4D7C,
			0xC60C, 0xD785, 0xE51E, 0xF497, 0x8028, 0x91A1, 0xA33A, 0xB2B3,
			0x4A44, 0x5BCD, 0x6956, 0x78DF, 0x0C60, 0x1DE9, 0x2F72, 0x3EFB,
			0xD68D, 0xC704, 0xF59F, 0xE416, 0x90A9, 0x8120, 0xB3BB, 0xA232,
			0x5AC5, 0x4B4C, 0x79D7, 0x685E, 0x1CE1, 0x0D68, 0x3FF3, 0x2E7A,
			0xE70E, 0xF687, 0xC41C, 0xD595, 0xA12A, 0xB0A3, 0x8238, 0x93B1,
			0x6B46, 0x7ACF, 0x4854, 0x59DD, 0x2D62, 0x3CEB, 0x0E70, 0x1FF9,
			0xF78F, 0xE606, 0xD49D, 0xC514, 0xB1AB, 0xA022, 0x92B9, 0x8330,
			0x7BC7, 0x6A4E, 0x58D5, 0x495C, 0x3DE3, 0x2C6A, 0x1EF1, 0x0F78
		};

		private static readonly ConcurrentDictionary<string, string> _enumErrorCache;
		private static readonly ConcurrentDictionary<string, PropertyInfo[]> _propertyInfos;

		#endregion

		#region Constructors

		static Extensions()
		{
			_enumErrorCache = new ConcurrentDictionary<string, string>();
			_propertyInfos = new ConcurrentDictionary<string, PropertyInfo[]>();
		}

		#endregion

		#region Methods

		public static string ArgumentsToString(this object[] args)
		{
			return ArgumentsToString(args, CultureInfo.InvariantCulture);
		}

		public static string ArgumentsToString(this object[] args, IFormatProvider provider)
		{
			if (args == null || args.Length == 0)
			{
				return string.Empty;
			}

			var sb = new StringBuilder();

			OscMessage.ArgumentsToString(sb, false, provider, args);

			return sb.ToString();
		}

		public static ushort CalculateCrc16(this byte[] data)
		{
			return CalculateCrc16(data, data.Length);
		}

		public static ushort CalculateCrc16(this byte[] data, int length)
		{
			// CRC-16/KERMIT
			ushort crc = 0;
			var index = 0;

			while (length-- > 0)
			{
				crc = (ushort) ((crc >> 8) ^ _crcTable[(crc ^ data[index++]) & 0xFF]);
			}

			return crc;
		}

		/// <summary>
		/// To literal version of the string.
		/// </summary>
		/// <param name="input"> The string input. </param>
		/// <returns> The literal version of the string. </returns>
		public static string Escape(this string input)
		{
			if (input == null)
			{
				return "null";
			}

			var literal = new StringBuilder(input.Length);

			foreach (var c in input)
			{
				switch (c)
				{
					case '\'':
						literal.Append(@"\'");
						continue;
					case '\"':
						literal.Append("\\\"");
						continue;
					case '\\':
						literal.Append(@"\\");
						continue;
					case '\0':
						literal.Append(@"\0");
						continue;
					case '\a':
						literal.Append(@"\a");
						continue;
					case '\b':
						literal.Append(@"\b");
						continue;
					case '\f':
						literal.Append(@"\f");
						continue;
					case '\n':
						literal.Append(@"\n");
						continue;
					case '\r':
						literal.Append(@"\r");
						continue;
					case '\t':
						literal.Append(@"\t");
						continue;
					case '\v':
						literal.Append(@"\v");
						continue;
					default:
						// ASCII printable character
						if (c >= 0x20 && c <= 0x7e)
						{
							literal.Append(c);
							// As UTF16 escaped character
						}
						else
						{
							literal.Append(@"\u");
							literal.Append(((int) c).ToString("x4"));
						}
						continue;
				}
			}

			return literal.ToString();
		}

		/// <summary>
		/// Turn a byte array into a readable, escaped string
		/// </summary>
		/// <param name="bytes"> bytes </param>
		/// <returns> a string </returns>
		public static string Escape(this byte[] bytes)
		{
			var data = Encoding.UTF8.GetString(bytes);
			return Escape(data);
		}

		public static void FixMaxBaudRateIssue(this SerialPort port)
		{
			try
			{
				var field = port.GetType().GetField("internalSerialStream", BindingFlags.NonPublic | BindingFlags.Instance)
					?? port.GetType().GetField("_internalSerialStream", BindingFlags.NonPublic | BindingFlags.Instance);

				if (field == null)
				{
					return;
				}

				var stream = field.GetValue(port);
				var commPropField = stream.GetType().GetField("commProp", BindingFlags.NonPublic | BindingFlags.Instance)
					?? stream.GetType().GetField("_commProp", BindingFlags.NonPublic | BindingFlags.Instance);

				if (commPropField == null)
				{
					return;
				}

				var commProp = commPropField.GetValue(stream);
				if (commProp == null)
				{
					return;
				}

				var dwMaxBaudField = commProp.GetType().GetField("dwMaxBaud", BindingFlags.Public | BindingFlags.Instance)
					?? commProp.GetType().GetField("_dwMaxBaud", BindingFlags.Public | BindingFlags.Instance);

				if (dwMaxBaudField == null)
				{
					return;
				}

				dwMaxBaudField.SetValue(commProp, int.MaxValue);
				commPropField.SetValue(stream, commProp);
			}
			catch
			{
				// Ignore any issues...
			}
		}

		/// <summary>
		/// Gets a list of property types for the provided object type. The results are cached so the next query is much faster.
		/// </summary>
		/// <param name="value"> The value to get the properties for. </param>
		/// <param name="flags"> The flags to find properties by. Defaults to Public, Instance, Flatten Heiarchy </param>
		/// <returns> The list of properties for the type of the value. </returns>
		public static IList<PropertyInfo> GetCachedProperties(this object value, BindingFlags? flags = null)
		{
			return value.GetType().GetCachedProperties(flags);
		}

		/// <summary>
		/// Gets a list of property types for the provided type. The results are cached so the next query is much faster.
		/// </summary>
		/// <param name="type"> The type to get the properties for. </param>
		/// <param name="flags"> The flags to find properties by. Defaults to Public, Instance, Flatten Heiarchy </param>
		/// <returns> The list of properties for the type. </returns>
		public static IList<PropertyInfo> GetCachedProperties(this Type type, BindingFlags? flags = null)
		{
			PropertyInfo[] response;

			if (_propertyInfos.ContainsKey(type.FullName ?? throw new InvalidOperationException()))
			{
				if (_propertyInfos.TryGetValue(type.FullName, out response))
				{
					return response;
				}
			}

			response = type.GetProperties(flags ?? BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);
			return _propertyInfos.AddOrUpdate(type.FullName, response, (s, infos) => response);
		}

		public static string GetDescription<T>(this T enumerationValue, params object[] arguments) where T : struct
		{
			var type = enumerationValue.GetType();
			if (!type.IsEnum)
			{
				throw new ArgumentException("EnumerationValue must be of Enum type", nameof(enumerationValue));
			}

			var key = $"{type.FullName}+{enumerationValue}";
			var value = _enumErrorCache.GetOrAdd(key, x =>
			{
				// Tries to find a DescriptionAttribute for a potential friendly name for the enum
				var memberInfo = type.GetMember(enumerationValue.ToString());

				if (memberInfo != null && memberInfo.Length > 0)
				{
					var attrs = memberInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);

					if (attrs != null && attrs.Length > 0)
					{
						// Pull out the description value
						return ((DescriptionAttribute) attrs[0]).Description;
					}
				}

				// If we have no description attribute, just return the ToString of the enum
				return enumerationValue.ToString();
			});

			return arguments.Any() ? string.Format(value, arguments) : value;
		}

		/// <summary>
		/// Parse a single argument
		/// </summary>
		/// <param name="str"> string contain the argument to parse </param>
		/// <returns> the parsed argument </returns>
		public static object ParseArgument(this string str)
		{
			return ParseArgument(str, CultureInfo.InvariantCulture);
		}

		/// <summary>
		/// Parse a single argument
		/// </summary>
		/// <param name="str"> string contain the argument to parse </param>
		/// <param name="provider"> format provider to use </param>
		/// <returns> the parsed argument </returns>
		public static object ParseArgument(this string str, IFormatProvider provider)
		{
			long value64;
			float valueFloat;
			double valueDouble;

			var argString = str.Trim();

			if (argString.Length == 0)
			{
				throw new Exception("Argument is empty");
			}

			// try to parse a hex value
			if (argString.Length > 2 && argString.StartsWith("0x"))
			{
				var hexString = argString.Substring(2);

				if (hexString.Length <= 8)
				{
					// parse a int32
					if (uint.TryParse(hexString, NumberStyles.HexNumber, provider, out var value))
					{
						return unchecked((int) value);
					}
				}
				else
				{
					// parse a int64
					if (ulong.TryParse(hexString, NumberStyles.HexNumber, provider, out var value))
					{
						return unchecked((long) value);
					}
				}
			}

			// parse uint64
			if (argString.EndsWith("U"))
			{
				if (ulong.TryParse(argString.Substring(0, argString.Length - 1), NumberStyles.Integer, provider, out var u64))
				{
					return u64;
				}
			}

			// parse int64
			if (argString.EndsWith("L"))
			{
				if (long.TryParse(argString.Substring(0, argString.Length - 1), NumberStyles.Integer, provider, out value64))
				{
					return value64;
				}
			}

			// parse uint32
			if (argString.EndsWith("u"))
			{
				if (uint.TryParse(argString.Substring(0, argString.Length - 1), NumberStyles.Integer, provider, out var u32))
				{
					return u32;
				}
			}

			// parse int32
			if (int.TryParse(argString, NumberStyles.Integer, provider, out var value32))
			{
				return value32;
			}

			// parse int64
			if (long.TryParse(argString, NumberStyles.Integer, provider, out value64))
			{
				return value64;
			}

			// parse double
			if (argString.EndsWith("d"))
			{
				var argument = argString.Substring(0, argString.Length - 1);
				if (double.TryParse(argument, NumberStyles.Float, provider, out valueDouble))
				{
					return valueDouble;
				}

				if (double.TryParse(argument, out valueDouble))
				{
					return valueDouble;
				}
			}

			// parse float
			if (argString.EndsWith("f"))
			{
				var argument = argString.Substring(0, argString.Length - 1);
				if (float.TryParse(argument, NumberStyles.Float, provider, out valueFloat))
				{
					return valueFloat;
				}
			}

			if (argString.Equals(float.PositiveInfinity.ToString(provider)))
			{
				return float.PositiveInfinity;
			}

			if (argString.Equals(float.NegativeInfinity.ToString(provider)))
			{
				return float.NegativeInfinity;
			}

			if (argString.Equals(float.NaN.ToString(provider)))
			{
				return float.NaN;
			}

			// parse float 
			if (float.TryParse(argString, NumberStyles.Float, provider, out valueFloat))
			{
				return valueFloat;
			}

			// parse double
			if (double.TryParse(argString, NumberStyles.Float, provider, out valueDouble))
			{
				return valueDouble;
			}

			// parse bool
			if (bool.TryParse(argString, out var valueBool))
			{
				return valueBool;
			}

			// parse char
			if (argString.Length == 3 && argString[0] == '\'' && argString[2] == '\'')
			{
				var c = str.Trim()[1];
				return c;
			}

			// parse null
			if (argString.Equals("null", StringComparison.OrdinalIgnoreCase) || argString.Equals("nil", StringComparison.OrdinalIgnoreCase))
			{
				return null;
			}

			// parse string
			if (argString[0] == '\"')
			{
				var end = argString.LastIndexOf('"');

				if (end < argString.Length - 1)
				{
					// some kind of other value tacked on the end of a string! 
					throw new Exception($"Malformed string argument '{argString}'");
				}

				return Unescape(argString.Substring(1, argString.Length - 2));
			}

			// if all else fails then its a symbol i guess (?!?) 
			return new OscSymbol(Unescape(argString));
		}

		public static byte[] ParseBlob(string str, IFormatProvider provider)
		{
			if (string.IsNullOrWhiteSpace(str))
			{
				return new byte[0];
			}

			var trimmed = str.Trim();

			if (trimmed.StartsWith("64x"))
			{
				return Convert.FromBase64String(trimmed.Substring(3));
			}
			if (str.StartsWith("0x"))
			{
				trimmed = trimmed.Substring(2);

				if (trimmed.Length % 2 != 0)
				{
					// this is an error
					throw new Exception("Invalid blob string length");
				}

				var length = trimmed.Length / 2;

				var bytes = new byte[length];

				for (var i = 0; i < bytes.Length; i++)
				{
					bytes[i] = byte.Parse(trimmed.Substring(i * 2, 2), NumberStyles.HexNumber, provider);
				}

				return bytes;
			}
			else
			{
				var parts = str.Split(',');

				var bytes = new byte[parts.Length];

				for (var i = 0; i < bytes.Length; i++)
				{
					bytes[i] = byte.Parse(parts[i], NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite, provider);
				}

				return bytes;
			}
		}

		public static OscTimeTag ToOscTimeTag(this DateTime time)
		{
			return new OscTimeTag(time);
		}

		public static string ToStringBlob(this byte[] bytes)
		{
			// if the default is to be Base64 encoded
			// return "64x" + System.Convert.ToBase64String(bytes);

			var sb = new StringBuilder(bytes.Length * 2 + 2);

			sb.Append("0x");

			foreach (var b in bytes)
			{
				sb.Append(b.ToString("X2"));
			}

			return sb.ToString();
		}

		/// <summary>
		/// Turn a readable string into a byte array
		/// </summary>
		/// <param name="value"> a string, optionally with escape sequences in it </param>
		/// <returns> The unescape version of the provided value. </returns>
		public static string Unescape(this string value)
		{
			var count = 0;
			var isEscaped = false;
			var parseHexNext = false;
			var parseHexCount = 0;
			var parseHexCounts = new List<int>();

			// first we count the number of chars we will be returning
			for (var i = 0; i < value.Length; i++)
			{
				var c = value[i];

				if (parseHexNext)
				{
					if (Uri.IsHexDigit(c))
					{
						parseHexCount++;
					}
					else
					{
						parseHexCounts.Add(parseHexCount);
						count += parseHexCount;
						parseHexNext = false;
						parseHexCount = 0;
					}
				}

				// if we are not in  an escape sequence and the char is a escape char
				else if (isEscaped == false && c == '\\')
				{
					// escape
					isEscaped = true;

					// increment count
					count++;
				}

				// else if we are escaped
				else if (isEscaped)
				{
					// reset escape state
					isEscaped = false;

					// check the char against the set of known escape chars
					switch (char.ToLower(c))
					{
						case '0':
						case 'a':
						case 'b':
						case 'f':
						case 'n':
						case 'r':
						case 't':
						case 'v':
						case '\'':
						case '\\':
						case '"':
							// do not increment count
							break;

						case 'u':
							// Skip the 4 value because they are unicode values
							i += 4;
							break;

						case 'x':
							// do not increment count
							parseHexNext = true;
							parseHexCount = 0;
							break;

						default:
							// this is not a valid escape sequence
							throw new Exception($"Invalid escape sequence at char of [{c}] at offset {i - 1}.");
					}
				}
				else
				{
					// normal char increment count
					count++;
				}
			}

			if (parseHexNext)
			{
				throw new Exception($"Invalid escape sequence at char '{value.Length - 1}' missing hex value.");
			}

			if (isEscaped)
			{
				throw new Exception($"Invalid escape sequence at char '{value.Length - 1}'.");
			}

			// create a character array for the result
			var chars = new char[count];
			var hexCountIndex = 0;
			var j = 0;
			
			// actually populate the array
			for (var i = 0; i < value.Length; i++)
			{
				var c = value[i];

				// if we are not in  an escape sequence and the char is a escape char
				if (isEscaped == false && c == '\\')
				{
					// escape
					isEscaped = true;
				}

				// else if we are escaped
				else if (isEscaped)
				{
					// reset escape state
					isEscaped = false;

					// check the char against the set of known escape chars
					switch (char.ToLower(value[i]))
					{
						case '0':
							chars[j++] = '\0';
							break;

						case 'a':
							chars[j++] = '\a';
							break;

						case 'b':
							chars[j++] = '\b';
							break;

						case 'f':
							chars[j++] = '\f';
							break;

						case 'n':
							chars[j++] = '\n';
							break;

						case 'r':
							chars[j++] = '\r';
							break;
						
						case 't':
							chars[j++] = '\t';
							break;

						case 'v':
							chars[j++] = '\v';
							break;

						case '\\':
							chars[j++] = '\\';
							break;

						case '\'':
							chars[j++] = '\'';
							break;
						
						case '"':
							chars[j++] = '"';
							break;

						case 'x':
							chars[j++] = '\\';
							chars[j++] = c;
							var hexCount = parseHexCounts[hexCountIndex++];
							for (var h = 0; h < hexCount; h++)
							{
								chars[j++] = value[++i];
							}
							break;
						
						case 'u':
							chars[j++] = (char) ((Uri.FromHex(value[++i]) << 12) | (Uri.FromHex(value[++i]) << 8) | (Uri.FromHex(value[++i]) << 4) | Uri.FromHex(value[++i]));
							break;
					}
				}
				else
				{
					// normal char
					chars[j++] = c;
				}
			}

			return new string(chars);
		}

		/// <summary>
		/// Parse arguments
		/// </summary>
		/// <param name="str"> string to parse </param>
		/// <param name="arguments"> the list to put the parsed arguments into </param>
		/// <param name="index"> the current index within the string </param>
		/// <param name="provider"> the format to use </param>
		internal static void ParseArguments(string str, List<object> arguments, int index, IFormatProvider provider)
		{
			while (true)
			{
				if (index >= str.Length)
				{
					return;
				}

				// scan forward for the first control char ',', '[', '{', '"'
				var controlChar = str.IndexOfAny(new[] { ',', '[', '{', '"' }, index);

				if (controlChar == -1)
				{
					// no control char found 
					var argument = str.Substring(index, str.Length - index);
					arguments.Add(ParseArgument(argument, provider));
					return;
				}

				var c = str[controlChar];

				switch (c)
				{
					case ',':
					{
						if (index == controlChar)
						{
							index++;
							continue;
						}

						var argument = str.Substring(index, controlChar - index);
						arguments.Add(ParseArgument(argument, provider));
						index = controlChar + 1;
						break;
					}

					case '[':
					{
						var end = ScanForwardInArray(str, controlChar);
						var array = new List<object>();

						ParseArguments(str.Substring(controlChar + 1, end - (controlChar + 1)), array, 0, provider);

						arguments.Add(array.ToArray());

						end++;

						if (end >= str.Length)
						{
							return;
						}

						if (str[end] != ',')
						{
							controlChar = str.IndexOfAny(new[] { ',' }, end);

							if (controlChar == -1)
							{
								return;
							}

							if (string.IsNullOrWhiteSpace(str.Substring(end, controlChar - end)) == false)
							{
								throw new Exception($"Malformed array '{str.Substring(index, controlChar - end)}'");
							}

							index = controlChar;
						}
						else
						{
							index = end + 1;
						}

						break;
					}

					case '{':
					{
						var end = ScanForwardObject(str, controlChar);

						arguments.Add(ParseObject(str.Substring(controlChar + 1, end - (controlChar + 1)), provider));

						end++;

						if (end >= str.Length)
						{
							return;
						}

						if (str[end] != ',')
						{
							controlChar = str.IndexOfAny(new[] { ',' }, end);

							if (controlChar == -1)
							{
								return;
							}

							if (string.IsNullOrWhiteSpace(str.Substring(end, controlChar - end)) == false)
							{
								throw new Exception($"Malformed object '{str.Substring(index, controlChar - end)}'");
							}

							index = controlChar;
						}
						else
						{
							index = end + 1;
						}

						break;
					}

					case '"':
					{
						var start = controlChar + 1;
						var nextQuote = ScanForwardUntil(str, start, '"', '\\');
						var argument = str.Substring(start, nextQuote - start);
						arguments.Add(Unescape(argument));
						index = nextQuote + 1;
						break;
					}
				}
			}
		}

		/// <summary>
		/// Scan for object start and end control chars
		/// </summary>
		/// <param name="str"> the string to scan </param>
		/// <param name="controlChar"> the index of the starting control char </param>
		/// <returns> the index of the end char </returns>
		internal static int ScanForwardObject(string str, int controlChar)
		{
			return ScanForward(str, controlChar, '{', '}', "Expected '}'");
		}

		/// <summary>
		/// Parse an object
		/// </summary>
		/// <param name="str"> string contain the object to parse </param>
		/// <param name="provider"> format provider to use </param>
		/// <returns> the parsed argument </returns>
		private static object ParseObject(string str, IFormatProvider provider)
		{
			var strTrimmed = str.Trim();

			var colon = strTrimmed.IndexOf(':');

			if (colon <= 0)
			{
				throw new Exception($"Malformed object '{strTrimmed}', missing type name");
			}

			var name = strTrimmed.Substring(0, colon).Trim();
			var nameLower = name.ToLowerInvariant();

			if (name.Length == 0)
			{
				throw new Exception($"Malformed object '{strTrimmed}', missing type name");
			}

			if (colon + 1 >= strTrimmed.Length)
			{
				throw new Exception($"Malformed object '{strTrimmed}'");
			}

			switch (nameLower)
			{
				case "midi":
				case "m":
					return OscMidi.Parse(strTrimmed.Substring(colon + 1).Trim(), provider);

				case "time":
				case "t":
					return OscTimeTag.Parse(strTrimmed.Substring(colon + 1).Trim(), provider);

				case "color":
				case "c":
					return OscRgba.Parse(strTrimmed.Substring(colon + 1).Trim(), provider);

				case "blob":
				case "b":
				case "data":
				case "d":
					return ParseBlob(strTrimmed.Substring(colon + 1).Trim(), provider);

				default:
					throw new Exception($"Unknown object type '{name}'");
			}
		}

		/// <summary>
		/// Scan for start and end control chars
		/// </summary>
		/// <param name="str"> the string to scan </param>
		/// <param name="controlChar"> the index of the starting control char </param>
		/// <param name="startChar"> start control char </param>
		/// <param name="endChar"> end control char </param>
		/// <param name="errorString"> string to use in the case of an error </param>
		/// <returns> the index of the end char </returns>
		private static int ScanForward(string str, int controlChar, char startChar, char endChar, string errorString)
		{
			var found = false;
			var count = 0;
			var index = controlChar + 1;
			var insideString = false;

			while (index < str.Length)
			{
				if (str[index] == '"')
				{
					insideString = !insideString;
				}
				else
				{
					if (insideString == false)
					{
						if (str[index] == startChar)
						{
							count++;
						}
						else if (str[index] == endChar)
						{
							if (count == 0)
							{
								found = true;

								break;
							}

							count--;
						}
					}
				}

				index++;
			}

			if (insideString)
			{
				throw new Exception(@"Expected '""'");
			}

			if (count > 0)
			{
				throw new Exception(errorString);
			}

			if (found == false)
			{
				throw new Exception(errorString);
			}

			return index;
		}

		/// <summary>
		/// Scan for array start and end control chars
		/// </summary>
		/// <param name="str"> the string to scan </param>
		/// <param name="controlChar"> the index of the starting control char </param>
		/// <returns> the index of the end char </returns>
		private static int ScanForwardInArray(string str, int controlChar)
		{
			return ScanForward(str, controlChar, '[', ']', "Expected ']'");
		}

		/// <summary>
		/// Scan for start and end control chars
		/// </summary>
		/// <param name="str"> the string to scan </param>
		/// <param name="index"> the index to start from </param>
		/// <param name="endChar"> end control char </param>
		/// <param name="escapeCharacter"> the escape character </param>
		/// <returns> the index of the end char </returns>
		private static int ScanForwardUntil(string str, int index, char endChar, char escapeCharacter)
		{
			var hasEscape = false;

			while (index < str.Length)
			{
				if (str[index] == endChar && !hasEscape)
				{
					return index;
				}

				hasEscape = !hasEscape && str[index] == escapeCharacter;
				index++;
			}

			return -1;
		}

		#endregion
	}
}