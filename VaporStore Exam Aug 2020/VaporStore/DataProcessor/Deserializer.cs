namespace VaporStore.DataProcessor
{
	using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using Data;
    using Newtonsoft.Json;
    using VaporStore.Data.Models;
    using VaporStore.Data.Models.Enums;
    using VaporStore.DataProcessor.Dto.Import;

    public static class Deserializer
	{
		public static string ImportGames(VaporStoreDbContext context, string jsonString)
		{
            var gamesDtos =
                JsonConvert.DeserializeObject<IEnumerable<GameImportDto>>(jsonString);
            var sb = new StringBuilder();
            var games = new List<Game>();
            var developers = new List<Developer>();
            var genres = new List<Genre>();
            var tags = new List<Tag>();
            foreach (var gameDto in gamesDtos)
            {

                if (!IsValid(gameDto))       
                {
                    sb.AppendLine("Invalid Data");
                    continue;
                }
                var gameTags = new List<GameTag>();
                foreach (var tag in gameDto.Tags)
                {
                    var currentTag = tags.FirstOrDefault(x=> x.Name == tag);
                    if (currentTag == null)
                    {
                        currentTag = new Tag { Name = tag };
                        tags.Add(currentTag);
                    }
                    gameTags.Add(new GameTag {Tag = currentTag });
                }
                Developer developer = developers.FirstOrDefault(x=> x.Name == gameDto.Developer);
                if (developer == null)
                {
                    developer = new Developer { Name = gameDto.Developer };
                    developers.Add(developer);
                }
                var genre = genres.FirstOrDefault(x => x.Name == gameDto.Genre);
                if (genre == null)
                {
                    genre = new Genre { Name = gameDto.Genre };
                    genres.Add(genre);
                }
                var game = new Game
                {
                    Name = gameDto.Name,
                    Price = gameDto.Price,
                    ReleaseDate = DateTime.ParseExact(gameDto.ReleaseDate, "yyyy-MM-dd", CultureInfo.InvariantCulture),
                    Developer = developer,
                    Genre = genre,
                    GameTags = gameTags,

                };

                games.Add(game);
                sb.AppendLine($"Added {game.Name} ({game.Genre.Name}) with {game.GameTags.Count} tags");
            }
            context.Games.AddRange(games);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

  		public static string ImportUsers(VaporStoreDbContext context, string jsonString)
		{
			var userDtos = JsonConvert.DeserializeObject<IEnumerable<UserInputModel>>(jsonString);
            var sb = new StringBuilder();
            var users = new List<User>();
            foreach (var userDto in userDtos)
            {
                if (!IsValid(userDto) || !userDto.Cards.All(IsValid))
                {
                    sb.AppendLine("Invalid Data");
                    continue;
                }
                var currentUser = new User
                {
                    FullName = userDto.FullName,
                    Username = userDto.Username,
                    Email = userDto.Email,
                    Age = userDto.Age,
                    Cards = userDto.Cards.Select(c=> new Card 
                    { 
                        Number = c.Number,
                        Cvc = c.CVC,
                        Type = Enum.Parse<CardType>(c.Type),
                    }).ToList()
                };
                users.Add(currentUser);
                sb.AppendLine($"Imported {currentUser.Username} with {currentUser.Cards.Count()} cards");
            }
            context.Users.AddRange(users);
            context.SaveChanges();
            return sb.ToString().TrimEnd();
        }

		public static string ImportPurchases(VaporStoreDbContext context, string xmlString)
		{
            StringBuilder sb = new StringBuilder();
            
            string root = "Purchases";
            var purchases = new List<Purchase>();
            var purchaseDtos = XmlConverter.Deserializer<PurchaseInputDto>(xmlString, root);
            foreach (var purchaseDto in purchaseDtos)
            {
                if (!IsValid(purchaseDto))
                {
                    sb.AppendLine("Invalid Data");
                    continue;
                }
                var currentGame = context.Games.FirstOrDefault(x => x.Name == purchaseDto.Title);
                if (currentGame == null)
                {
                    sb.AppendLine("Invalid Data");
                    continue;
                }
                var currentCard = context.Cards.FirstOrDefault(x => x.Number == purchaseDto.Card);
                if (currentCard == null)
                {
                    sb.AppendLine("Invalid Data");
                    continue;
                }
                var currentUSer = context.Users.FirstOrDefault(x => x.Cards.Any(c => c.Number == purchaseDto.Card));
                var purchase = new Purchase
                {
                    Type = Enum.Parse<PurchaseType>(purchaseDto.Type),
                    ProductKey = purchaseDto.Key,
                    Date = DateTime.ParseExact(purchaseDto.Date, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture),
                    Card=currentCard,
                    Game = currentGame,
                };

                purchases.Add(purchase);
                sb.AppendLine($"Imported {purchase.Game.Name} for {currentUSer.Username}");
            }
            context.Purchases.AddRange(purchases);
            context.SaveChanges();
            return sb.ToString().TrimEnd();
        }

		private static bool IsValid(object dto)
		{
			var validationContext = new ValidationContext(dto);
			var validationResult = new List<ValidationResult>();

			return Validator.TryValidateObject(dto, validationContext, validationResult, true);
		}
	}
}