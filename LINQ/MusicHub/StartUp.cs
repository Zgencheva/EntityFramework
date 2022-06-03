namespace MusicHub
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using Data;
    using Initializer;
    using Microsoft.EntityFrameworkCore;

    public class StartUp
    {
        public static void Main(string[] args)
        {
            MusicHubDbContext context = 
                new MusicHubDbContext();

           // DbInitializer.ResetDatabase(context);
            Console.WriteLine(ExportSongsAboveDuration(context, 4));
            


        }

        public static string ExportAlbumsInfo(MusicHubDbContext context, int producerId)
        {
            StringBuilder sb = new StringBuilder();
            var query = context.Albums
                .Where(x => x.ProducerId == producerId)
                .Select(a => new
                {
                    a.Name,
                    a.ReleaseDate,
                    ProducerName = a.Producer.Name,
                    Songs = a.Songs.Select(s => new
                    {
                        s.Name,
                        s.Price,
                        SongWriterName = s.Writer.Name,
                    })
                })
                .ToList();

            foreach (var album in query.OrderByDescending(x=> x.Songs.Sum(p=> p.Price)))
            {
                sb.AppendLine($"-AlbumName: {album.Name}");
                sb.AppendLine($"-ReleaseDate: {album.ReleaseDate.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture)}"); //FORMAT!!
                sb.AppendLine($"-ProducerName: {album.ProducerName}");
                sb.AppendLine("-Songs:");
                int counter = 0;
                foreach (var song in album.Songs.OrderByDescending(x=> x.Name).ThenBy(x=> x.SongWriterName))
                {
                    counter++;
                    sb.AppendLine($"---#{counter}");
                    sb.AppendLine($"---SongName: {song.Name}");
                    sb.AppendLine($"---Price: {song.Price:f2}");
                    sb.AppendLine($"---Writer: {song.SongWriterName}");
                }
                var totalPrice = album.Songs.Sum(x => x.Price);
                sb.AppendLine($"-AlbumPrice: {totalPrice:f2}");
            }

            return sb.ToString().TrimEnd();
        }

        public static string ExportSongsAboveDuration(MusicHubDbContext context, int duration)
        {
            StringBuilder sb = new StringBuilder();

            var songsAboveDurations = context.Songs
                .Include(x => x.Writer)
                .Include(x => x.Album)
                .ThenInclude(z => z.Producer)
                .Include(x => x.SongPerformers)
                .ThenInclude(s => s.Performer)
                .ToList()
                .Where(x => x.Duration.TotalSeconds > duration)
                .Select(x => new
                 {
                     SongName = x.Name,
                     WriterName = x.Writer.Name,
                     Performer = x.SongPerformers.Select(x => x.Performer.FirstName + " " + x.Performer.LastName).FirstOrDefault(),
                    AlbumProducer = x.Album.Producer.Name,
                     Duration = x.Duration,

                 })
                .OrderBy(x => x.SongName)
                .ThenBy(x => x.WriterName)
                .ThenBy(x => x.Performer)
                .ToList();


            int counter = 0;
            foreach (var song in songsAboveDurations)
            {
                counter++;
                sb.AppendLine($"-Song #{counter}");
                sb.AppendLine($"---SongName: {song.SongName}");
                sb.AppendLine($"---Writer: {song.WriterName}");
               sb.AppendLine($"---Performer: {song.Performer}");
                sb.AppendLine($"---AlbumProducer: {song.AlbumProducer}");
                sb.AppendLine($"---Duration: {song.Duration:c}");
            }
            return sb.ToString().TrimEnd();
        }

   
    }
}
