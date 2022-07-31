namespace Theatre.DataProcessor
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using Theatre.Data;
    using Theatre.Data.Models;
    using Theatre.Data.Models.Enums;
    using Theatre.DataProcessor.ImportDto;
    using Theatre.DataProcessor.XmlHelper;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfulImportPlay
            = "Successfully imported {0} with genre {1} and a rating of {2}!";

        private const string SuccessfulImportActor
            = "Successfully imported actor {0} as a {1} character!";

        private const string SuccessfulImportTheatre
            = "Successfully imported theatre {0} with #{1} tickets!";

        public static string ImportPlays(TheatreContext context, string xmlString)
        {
            StringBuilder sb = new StringBuilder();
            var root = "Plays";
            var plays = new List<Play>();
            var playDtos = XmlConverter.Deserializer<PlayXmlImportModel>(xmlString, root);
            foreach (var playDto in playDtos)
            {
                if (!IsValid(playDto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }
                if (TimeSpan.ParseExact(playDto.Duration, "c", CultureInfo.InvariantCulture).Hours < 1)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }


                var currentPlay = new Play
                {
                    Title = playDto.Title,
                    Duration = TimeSpan.ParseExact(playDto.Duration, "c", CultureInfo.InvariantCulture),
                    Rating = playDto.Rating,
                    Genre = Enum.Parse<Genre>(playDto.Genre),
                    Description = playDto.Description,
                    Screenwriter = playDto.Screenwriter,
                };

                plays.Add(currentPlay);
                sb.AppendLine($"Successfully imported {playDto.Title} with genre {playDto.Genre} and a rating of {playDto.Rating}!");
            }
            context.Plays.AddRange(plays);
            context.SaveChanges();
            return sb.ToString().TrimEnd();
        }

        public static string ImportCasts(TheatreContext context, string xmlString)
        {
            StringBuilder sb = new StringBuilder();
            var root = "Casts";
            var casts = new List<Cast>();
            var castDtos = XmlConverter.Deserializer<CastXmlInputModel>(xmlString, root);
            foreach (var castDto in castDtos)
            {
                if (!IsValid(castDto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }
              var currentCast = new Cast
                {
                   FullName = castDto.FullName,
                   IsMainCharacter = castDto.IsMainCharacter,
                   PhoneNumber = castDto.PhoneNumber,
                   PlayId = castDto.PlayId,
                };
                var carracter = castDto.IsMainCharacter ? "main" : "lesser";
                casts.Add(currentCast);
                sb.AppendLine($"Successfully imported actor {currentCast.FullName} as a {carracter} character!") ;
            }
            context.Casts.AddRange(casts);
            context.SaveChanges();
            return sb.ToString().TrimEnd();
        }

        public static string ImportTtheatersTickets(TheatreContext context, string jsonString)
        {
            var thatresDtos =
               JsonConvert.DeserializeObject<IEnumerable<TheatreJsonImortModel>>(jsonString);
            var sb = new StringBuilder();
            var theatres = new List<Theatre>();
            foreach (var theatreDto in thatresDtos)
            {

                if (!IsValid(theatreDto))             
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                Data.Models.Theatre theatre = new Data.Models.Theatre
                {
                    Name = theatreDto.Name,
                    NumberOfHalls = theatreDto.NumberOfHalls,
                    Director = theatreDto.Director
                };
                foreach (var ticketDto in theatreDto.Tickets)
                {
                    if (!IsValid(ticketDto))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }
                   //var currentPlay = context.Plays.FirstOrDefault(p => p.Id == ticketDto.PlayId);
                    var currentTicket = new Ticket
                    {
                        Price = ticketDto.Price,
                        RowNumber = ticketDto.RowNumber,
                        PlayId = ticketDto.PlayId, 
                    };

                    theatre.Tickets.Add(currentTicket);
                }
                theatres.Add(theatre);
                context.SaveChanges();
                sb.AppendLine($"Successfully imported theatre {theatreDto.Name} with #{theatre.Tickets.Count()} tickets!");
            }
            context.Theatres.AddRange(theatres);
            context.SaveChanges();
            return sb.ToString().TrimEnd();
        }


        private static bool IsValid(object obj)
        {
            var validator = new ValidationContext(obj);
            var validationRes = new List<ValidationResult>();

            var result = Validator.TryValidateObject(obj, validator, validationRes, true);
            return result;
        }
    }
}
