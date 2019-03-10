#region References

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sproto.OSC;

#endregion

namespace OSC.Tests.OSC
{
	[TestClass]
	public class OscTimeTagTests
	{
		#region Methods

		[TestMethod]
		public void Compare()
		{
			var expected = new DateTime(2019, 1, 20, 08, 50, 12, DateTimeKind.Utc);
			var time1 = OscTimeTag.FromDateTime(expected);
			var time2 = new OscTimeTag(16136033268821655552);
			Assert.IsTrue(time1 == time2);
			Assert.AreEqual(time1, time2);
		}

		[TestMethod]
		public void FromDateTime()
		{
			var expected = new DateTime(2019, 1, 20, 08, 50, 12, DateTimeKind.Utc);
			var time = OscTimeTag.FromDateTime(expected);
			var actual = time.ToDateTime();
			Assert.AreEqual(expected, actual);
			time.Value.Dump();

			time = new OscTimeTag(16136033268821655552);
			actual = time.ToDateTime();
			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void FromMillisecond()
		{
			var a = OscTimeTag.FromMilliseconds(1234);
			Assert.AreEqual(5299989643264u, a.Value);
		}

		[TestMethod]
		public void FromMinimalDate()
		{
			var time = OscTimeTag.FromDateTime(OscTimeTag.BaseDate);
			var actual = time.Value;
			Assert.AreEqual(0u, actual);
		}

		[TestMethod]
		public void FromSmallTime()
		{
			var time = OscTimeTag.FromDateTime(OscTimeTag.BaseDate.AddMilliseconds(1234.56));
			var actual = time.Value;
			Assert.AreEqual(5304284610u, actual);
		}

		[TestMethod]
		public void FromTimespan()
		{
			var span = new TimeSpan(0, 0, 0, 1, 234);
			var t = OscTimeTag.FromTimeSpan(span);
			Assert.AreEqual(5299989643u, t.Value);
			Assert.AreEqual(1234, t.ToMilliseconds());
		}

		[TestMethod]
		public void ToMillisecond()
		{
			var t = OscTimeTag.FromMilliseconds(1.234f);
			Assert.AreEqual(5299989643u, t.Value);
		}

		[TestMethod]
		public void ToMinimalDate()
		{
			var time = new OscTimeTag(0);
			var actual = time.ToDateTime();
			Assert.AreEqual(OscTimeTag.BaseDate, actual);
		}

		[TestMethod]
		public void ToSmallTime()
		{
			var time = new OscTimeTag(5304284610u);
			time.Value.Dump();
			Assert.AreEqual(1235, time.ToMilliseconds());
		}

		#endregion
	}
}