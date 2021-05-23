#region References

using System;
using System.Linq;

#endregion

namespace Sproto.Internal
{
	internal static class InternalExtensions
	{
		#region Methods

		public static int AlignedStringLength(this string val)
		{
			var len = val.Length + (4 - val.Length % 4);
			if (len <= val.Length)
			{
				len += 4;
			}

			return len;
		}

		public static int FirstIndexAfter<T>(this T[] items, int start, Func<T, bool> predicate)
		{
			if (items == null)
			{
				throw new ArgumentNullException(nameof(items));
			}
			
			if (predicate == null)
			{
				throw new ArgumentNullException(nameof(predicate));
			}
			
			if (start >= items.Count())
			{
				throw new ArgumentOutOfRangeException(nameof(start));
			}

			var retVal = 0;
			foreach (var item in items)
			{
				if (retVal >= start && predicate(item))
				{
					return retVal;
				}
				retVal++;
			}
			return -1;
		}

		public static int GetStableHashCode(this string value)
		{
			unchecked
			{
				var hash1 = 5381;
				var hash2 = hash1;

				for (var i = 0; i < value.Length && value[i] != '\0'; i += 2)
				{
					hash1 = ((hash1 << 5) + hash1) ^ value[i];
					if (i == value.Length - 1 || value[i + 1] == '\0')
					{
						break;
					}
					hash2 = ((hash2 << 5) + hash2) ^ value[i + 1];
				}

				return hash1 + hash2 * 1566083941;
			}
		}

		public static T[] SubArray<T>(this T[] data, int index, int length)
		{
			var result = new T[length];
			Array.Copy(data, index, result, 0, length);
			return result;
		}

		#endregion
	}
}