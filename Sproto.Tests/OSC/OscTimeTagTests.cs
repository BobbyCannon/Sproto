#region References

using System;
using System.Collections.Generic;
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
		public void GetHashCodeShouldSucceed()
		{
			Assert.AreEqual(0, new OscTimeTag().GetHashCode());
			Assert.AreEqual(0, OscTimeTag.MinValue.GetHashCode());
			Assert.AreEqual(1895321856, new OscTimeTag(new DateTime(2020, 02, 14, 04, 35, 12, DateTimeKind.Utc)).GetHashCode());
			Assert.AreEqual(1878481506, new OscTimeTag(16136033268821655552).GetHashCode());
			Assert.AreEqual(2147483647, OscTimeTag.MaxValue.GetHashCode());
		}

		[TestMethod]
		public void AddTimeSpan()
		{
			var span = TimeSpan.FromMilliseconds(123);
			var datetime = new DateTime(2019, 1, 20, 08, 50, 12, DateTimeKind.Utc);
			var expected = new OscTimeTag(datetime.Add(span));
			var timetag = new OscTimeTag(datetime);

			Assert.AreEqual(expected, timetag.Add(span));
		}

		[TestMethod]
		public void Compare()
		{
			var expected = new DateTime(2019, 1, 20, 08, 50, 12, DateTimeKind.Utc);
			var time1 = OscTimeTag.FromDateTime(expected);
			var time2 = new OscTimeTag(16136033268821655552);
			Assert.IsTrue(time1 == time2);
			Assert.AreEqual(time1, time2);

			time1 = OscTimeTag.MinValue;
			time2 = OscTimeTag.MaxValue;
			Assert.IsTrue(time1 < time2);
			Assert.IsFalse(time1 > time2);

			time1 = OscTimeTag.MinValue;
			time2 = OscTimeTag.MinValue;
			Assert.IsTrue(time1 == time2);
			Assert.IsFalse(time1 != time2);

			time1 = OscTimeTag.MinValue;
			time2 = OscTimeTag.MinValue;
			Assert.IsTrue(time1 >= time2);
			Assert.IsTrue(time1 <= time2);
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
			var time = OscTimeTag.FromDateTime(OscTimeTag.MinDateTime);
			var actual = time.Value;
			Assert.AreEqual(0u, actual);
		}

		[TestMethod]
		public void FromSmallTime()
		{
			var time = OscTimeTag.FromDateTime(OscTimeTag.MinDateTime.AddMilliseconds(1234.56));
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
		public void MaxValue()
		{
			var expected = new OscTimeTag(0xffffffffffffffff);
			Assert.AreEqual(expected, OscTimeTag.MaxValue);
		}

		[TestMethod]
		public void MinValue()
		{
			var expected = new OscTimeTag(0);
			Assert.AreEqual(expected, OscTimeTag.MinValue);
		}

		[TestMethod]
		public void ParseTime()
		{
			var values = new Dictionary<string, ulong>
			{
				{ "2019-04-05T00:00:59.1234Z", 16163728279333186305 },
				{ "2019-04-05T00:00:59Z", 16163728278803185664 },
				{ "2019-04-05", 16163728025400115200 },
				{ "2019-05-23T12:37:23.7150Z", 16181735293039820143 }
			};

			foreach (var e in values)
			{
				var actual = OscTimeTag.Parse(e.Key);
				actual.Value.Dump();
				Assert.AreEqual(e.Value, actual.Value);
			}
		}

		[TestMethod]
		public void Subtract()
		{
			var expected = new DateTime(2019, 1, 20, 08, 50, 12, DateTimeKind.Utc);
			var span = TimeSpan.FromMilliseconds(123);
			var t1 = new OscTimeTag(expected);
			var t2 = new OscTimeTag(expected.Add(span));
			var actual = t2 - t1;
			Assert.AreEqual(123, actual.TotalMilliseconds);
			Assert.AreEqual(t1, t2 - span);
		}

		[TestMethod]
		public void ToFromUtcNowUsingParse()
		{
			var time = OscTimeTag.UtcNow;
			var text = time.ToString();
			var time2 = OscTimeTag.Parse(text);

			time.Dump();
			time2.Dump();
			Assert.AreEqual(time.ToString(), time2.ToString());

			time = OscTimeTag.Now;
			text = time.ToString();
			time2 = OscTimeTag.Parse(text);

			time.Dump();
			time2.Dump();
			Assert.AreEqual(time.ToString(), time2.ToString());
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
			var actual = new OscTimeTag(0);
			Assert.AreEqual(OscTimeTag.MinValue, actual);
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