namespace VaporStore.DataProcessor
{
	using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Xml.Serialization;
    using AutoMapper.QueryableExtensions;
    using Data;
    using Microsoft.EntityFrameworkCore;
    using Newtonsoft.Json;
    using VaporStore.Data.Models.Enums;
    using VaporStore.DataProcessor.Dto.Export;

    public static class Serializer
	{
		public static string ExportGamesByGenres(VaporStoreDbContext context, string[] genreNames)
		{
			var genres = context.Genres
                .Include(x => x.Games)
                .ThenInclude(x => x.Purchases)
                .ToList()
                .Where(x => x.Games.Any(x => x.Purchases.Count() > 0) && genreNames.Contains(x.Name))
                .Select(x => new
                {
                    Id = x.Id,
                    Genre = x.Name,
                    Games = x.Games.Where(w=> w.Purchases.Count() > 0)
                        .Select(g => new
                        {
                            Id = g.Id,
                            Title = g.Name,
                            Developer = g.Developer.Name,
                            Tags = string.Join(", ", g.GameTags.Select(t => t.Tag.Name)),
                            Players = g.Purchases.Count(),
                        })
                        .OrderByDescending(x => x.Players)
                        .ThenBy(x => x.Id)
                        .ToList(),

                    TotalPlayers = x.Games.Sum(x => x.Purchases.Count())
                })
                .OrderByDescending(x=> x.TotalPlayers)
                .ThenBy(x => x.Id)
                .ToList();

            var result = JsonConvert.SerializeObject(genres, Formatting.Indented);
			return result;
		
		}

		public static string ExportUserPurchasesByType(VaporStoreDbContext context, string storeType)
		{   
            var root = "Users";
            var users = context.Users
            .ToList()
            .Where(x => x.Cards.Any(c => c.Purchases.Any(p => p.Type.ToString() == storeType)))
            .Select(x => new UserPurchasesExportDto
            {
                Username = x.Username,
                Purchases = x.Cards.SelectMany(c => c.Purchases).Where(p => p.Type.ToString() == storeType)
                    .Select(p => new PurchasesExportDto
                    {
                        Card = p.Card.Number,
                        Cvc = p.Card.Cvc,
                        Date = p.Date.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture),
                        Game = new GameExportDto
                        {
                            Title = p.Game.Name,
                            Genre = p.Game.Genre.Name,
                            Price = p.Game.Price
                        }
                    })
                    .OrderBy(x=> x.Date)
                    .ToArray(),
                TotalSpent = x.Cards.SelectMany(c => c.Purchases)
                    .Where(p => p.Type.ToString() == storeType)
                    .Sum(p => p.Game.Price),
            })
            .OrderByDescending(x=> x.TotalSpent)
            .ThenBy(x=> x.Username)
            .ToArray();

            var result = XmlConverter.Serialize<UserPurchasesExportDto>(users, root);
            //XmlSerializer xmlSerializer = new XmlSerializer(typeof(UserPurchasesExportDto[]),
            //     new XmlRootAttribute(root));
            //var sw = new StringWriter();
            //xmlSerializer.Serialize(sw, users);
            return result.ToString().TrimEnd(); ;
        
      
        }
	}
}