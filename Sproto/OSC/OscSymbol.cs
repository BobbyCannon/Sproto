﻿#region References

using System;
using Sproto.Internal;

#endregion

namespace Sproto.Osc
{
	public struct OscSymbol : IEquatable<OscSymbol>
	{
		#region Constructors

		/// <summary>
		/// Represents an alternate type for systems that differentiate "symbols" from "strings".
		/// </summary>
		/// <param name="value"> The symbol value. </param>
		public OscSymbol(string value)
		{
			Value = value;
		}

		#endregion

		#region Properties

		public string Value { get; set; }

		#endregion

		#region Methods

		public override bool Equals(object obj)
		{
			switch (obj)
			{
				case OscSymbol symbol:
					return Equals(symbol);

				case string s2:
					return Value == s2;

				default:
					return false;
			}
		}

		public bool Equals(OscSymbol other)
		{
			return Value == other.Value;
		}

		public override int GetHashCode()
		{
			return Value != null ? Value.GetStableHashCode() : 0;
		}

		public static bool operator ==(OscSymbol a, OscSymbol b)
		{
			return a.Equals(b);
		}

		public static bool operator !=(OscSymbol a, OscSymbol b)
		{
			return !a.Equals(b);
		}

		public override string ToString()
		{
			return Value;
		}

		#endregion
	}
}