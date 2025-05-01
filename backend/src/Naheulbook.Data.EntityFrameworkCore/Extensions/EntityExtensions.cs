using System.Runtime.CompilerServices;

namespace Naheulbook.Data.EntityFrameworkCore.Extensions;

public static class EntityExtensions
{
	public static bool SkipThrow;

	private class CancelSkip : IDisposable
	{
		public void Dispose()
		{
			SkipThrow = false;
		}
	}

	public static IDisposable SkipRelatedEntityCheck()
	{
		SkipThrow = true;
		return new CancelSkip();
	}

	public static T ThrowIfNotLoaded<T>(this T? entity, [CallerArgumentExpression("entity")] string? expression = null)
	{
		if (entity != null)
			return entity;
		if (SkipThrow)
			return default!;
		throw new RelatedEntitiesNotLoadedException(expression!);
	}

	public static T? ThrowIfNotLoadedAndNotNull<T, TProperty>(this T? entity, TProperty? foreignKe, [CallerArgumentExpression("entity")] string? expression = null)
		where T : class
	{
		if (entity != null)
			return entity;
		if (foreignKe is null)
			return null;
		if (SkipThrow)
			return null;
		throw new RelatedEntitiesNotLoadedException(expression!);
	}
}