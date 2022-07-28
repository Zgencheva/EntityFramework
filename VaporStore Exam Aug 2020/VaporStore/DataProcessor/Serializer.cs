namespace VaporStore.DataProcessor
{
	using System;
    using System.Linq;
    using Data;
    using Microsoft.EntityFrameworkCore;

    public static class Serializer
	{
		public static string ExportGamesByGenres(VaporStoreDbContext context, string[] genreNames)
		{
			var result = context.Games
				.Include(x => x.Genre)
				.Include(x => x.Purchases)
				.Where(x=> genreNames.Contains(x.Genre.Name) && x.Purchases.Count>0)
				.Select(x=> new 
				{ 
					Id = x.Genre.Id,
					Genre = x.Genre.Name,
					Games = new 
					{
						Id = x.Id,
						Title = x.Name,
						Developer = x.Developer.Name,
						Tags = string.Join(",", x.GameTags.Select(t=> t.Tag.Name)),
						Players = x.Purchases.Count,
					}

				});

			return null;
		}

		public static string ExportUserPurchasesByType(VaporStoreDbContext context, string storeType)
		{
			throw new NotImplementedException();
		}
	}
}