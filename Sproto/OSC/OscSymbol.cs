namespace Sproto.OSC
{
	public struct OscSymbol
	{
		#region Constructors

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
				case OscSymbol s1:
					return Value == s1.Value;

				case string s2:
					return Value == s2;

				default:
					return false;
			}
		}

		public override int GetHashCode()
		{
			return Value.GetHashCode();
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