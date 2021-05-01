#region References

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Speedy;

#endregion

namespace Sproto.Tests
{
	public abstract class BaseTests
	{
		#region Methods

		[TestInitialize]
		public virtual void TestInitialize()
		{
			TimeService.NowProvider = () => new DateTime(2021, 02, 17, 08, 54, 00, DateTimeKind.Local);
			TimeService.UtcNowProvider = () => new DateTime(2021, 02, 18, 01, 54, 00, DateTimeKind.Utc);
		}

		#endregion
	}
}