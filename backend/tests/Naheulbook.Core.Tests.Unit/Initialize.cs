using AwesomeAssertions;
using Naheulbook.Data.EntityFrameworkCore.Extensions;
using Newtonsoft.Json.Linq;
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

		// By default, equivalency only compares the keys of a JObject, not the values.
		AssertionConfiguration.Current.Equivalency.Modify(options => options
			.Using<JToken>(ctx => JToken.DeepEquals(ctx.Subject, ctx.Expectation).Should().BeTrue($"expected {ctx.Expectation} but found {ctx.Subject}"))
			.When(info => typeof(JToken).IsAssignableFrom(info.Type)));
	}

	[OneTimeTearDown]
	public static void TearDown()
	{
		_cancelSkipRelatedEntityCheck.Dispose();
	}
}