namespace MusicHub.DataProcessor
{
    using Data;
    using MusicHub.DataProcessor.ExportDtos;
    using Newtonsoft.Json;
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;

    public class Serializer
    {
        public static string ExportAlbumsInfo(MusicHubDbContext context, int producerId)
        {
            var albums = context.Producers
                .FirstOrDefault(x => x.Id == producerId)
                .Albums
                .OrderByDescending(a => a.Price)
                .Select(a => new
                {
                    AlbumName = a.Name,
                    ReleaseDate = a.ReleaseDate.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture),
                    ProducerName = a.Producer.Name,
                    Songs = a.Songs.Select(s => new
                    {
                        SongName = s.Name,
                        Price = s.Price.ToString("F2"),
                        Writer = s.Writer.Name
                    })
                    .OrderByDescending(s => s.SongName)
                    .ThenBy(s => s.Writer)
                    .ToArray(),
                    AlbumPrice = a.Price.ToString("F2")
                })
                .ToArray();

            return JsonConvert.SerializeObject(albums, Newtonsoft.Json.Formatting.Indented);
        }

        public static string ExportSongsAboveDuration(MusicHubDbContext context, int duration)
        {
            var songs = context.Songs
                .Where(s => s.Duration > TimeSpan.FromSeconds(duration))
                .Select(s => new SongDto()
                {
                    SongName = s.Name,
                    Writer = s.Writer.Name,
                    Performer = s.SongPerformers
                    .OrderBy(sp => sp.Performer.FirstName)
                    .ThenBy(sp => sp.Performer.LastName)
                    .FirstOrDefault().Performer.FullName,
                    AlbumProducer = s.Album.Producer.Name,
                    Duration = s.Duration.ToString("c", CultureInfo.InvariantCulture)
                })
                .OrderBy(s => s.SongName)
                .ThenBy(s => s.Writer)
                .ThenBy(s => s.Performer)
                .ToArray();

            XmlSerializer serializer = new XmlSerializer(typeof(SongDto[]), new XmlRootAttribute("Songs"));
            StringBuilder sb = new StringBuilder();
            StringWriter reader = new StringWriter(sb);

            var namesapces = new XmlSerializerNamespaces(new[]
            {
                XmlQualifiedName.Empty
            });
            serializer.Serialize(reader, songs, namesapces);
            return sb.ToString().TrimEnd();
        }
    }
}