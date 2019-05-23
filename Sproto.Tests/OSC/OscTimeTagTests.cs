#region References

using System;
using System.Collections.Generic;
using System.Diagnostics;
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

		#endregion
	}
}