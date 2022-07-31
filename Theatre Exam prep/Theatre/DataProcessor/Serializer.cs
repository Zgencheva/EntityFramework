namespace Theatre.DataProcessor
{
    using Microsoft.EntityFrameworkCore;
    using Newtonsoft.Json;
    using System;
    using System.Linq;
    using Theatre.Data;
    using Theatre.DataProcessor.ExportDto;
    using Theatre.DataProcessor.XmlHelper;

    public class Serializer
    {
        public static string ExportTheatres(TheatreContext context, int numbersOfHalls)
        {
            var theaters = context.Theatres
                .Include(x => x.Tickets)
                .ToList()
                .Where(t => (int)t.NumberOfHalls >= numbersOfHalls && t.Tickets.Count() >= 20)
                .Select(x => new
                {
                    Name = x.Name,
                    Halls = x.NumberOfHalls,
                    TotalIncome = x.Tickets
                            .Where(t => t.RowNumber >= 1 && t.RowNumber <= 5)
                            .Sum(x => x.Price),
                    Tickets = x.Tickets
                            .Where(t => t.RowNumber >= 1 && t.RowNumber <= 5)
                            .Select(r=> new 
                            {
                                Price = decimal.Parse(r.Price.ToString("f2")),
                                RowNumber = r.RowNumber,
                            })
                            .OrderByDescending(x=> x.Price)
                            .ToArray()
                })
                .OrderByDescending(x=> x.Halls)
                .ThenBy(x=> x.Name)
                .ToList();
            var result = JsonConvert.SerializeObject(theaters, Formatting.Indented);
            return result;
        }

        public static string ExportPlays(TheatreContext context, double rating)
        {
            var parsedRating = float.Parse(rating.ToString());

            var root = "Plays";
            var plays = context.Plays
                .ToList()
                .Where(x => x.Rating <= parsedRating)
                .Select(x => new ExportPlaysXmlDto
                {
                    Title = x.Title,
                    Duration = x.Duration.ToString("c"),
                    Rating = x.Rating == 0 ? "Premier" : x.Rating.ToString(),
                    Genre = x.Genre.ToString(),
                    Actors = x.Casts.Where(x=> x.IsMainCharacter == true)
                    .Select(a => new ExportActorXmlDto
                    {
                        FullName = a.FullName,
                        MainCharacter = $"Plays main character in '{x.Title}'."
                    })
                    .OrderByDescending(x => x.FullName)
                    .ToArray()
                })
                .OrderBy(x => x.Title)
                .ThenByDescending(x => x.Genre)
                .ToArray();
                
            var result = XmlConverter.Serialize<ExportPlaysXmlDto>(plays, root);
            //XmlSerializer xmlSerializer = new XmlSerializer(typeof(UserPurchasesExportDto[]),
            //     new XmlRootAttribute(root));
            //var sw = new StringWriter();
            //xmlSerializer.Serialize(sw, users);
            return result.ToString().TrimEnd(); ;
        }
    }
}
