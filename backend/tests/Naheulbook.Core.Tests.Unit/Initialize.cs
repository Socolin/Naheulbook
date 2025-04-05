using Naheulbook.Data.EntityFrameworkCore.Extensions;
using NUnit.Framework;

namespace Naheulbook.Core.Tests.Unit;

[SetUpFixture]
public static class Initialize
{
	private static IDisposable _cancelSkipRelatedEntityCheck;

	[OneTimeSetUp]
	public static void OneTimeSetup()
	{
		_cancelSkipRelatedEntityCheck = EntityExtensions.SkipRelatedEntityCheck();
	}

	[OneTimeTearDown]
	public static void TearDown()
	{
		_cancelSkipRelatedEntityCheck.Dispose();
	}
}