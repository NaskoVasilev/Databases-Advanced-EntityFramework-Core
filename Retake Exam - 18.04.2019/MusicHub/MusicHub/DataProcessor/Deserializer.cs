namespace MusicHub.DataProcessor
{
    using Data;
    using MusicHub.Data.Models;
    using MusicHub.Data.Models.Enums;
    using MusicHub.DataProcessor.ImportDtos;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data";

        private const string SuccessfullyImportedWriter
            = "Imported {0}";
        private const string SuccessfullyImportedProducerWithPhone
            = "Imported {0} with phone: {1} produces {2} albums";
        private const string SuccessfullyImportedProducerWithNoPhone
            = "Imported {0} with no phone number produces {1} albums";
        private const string SuccessfullyImportedSong
            = "Imported {0} ({1} genre) with duration {2}";
        private const string SuccessfullyImportedPerformer
            = "Imported {0} ({1} songs)";

        public static string ImportWriters(MusicHubDbContext context, string jsonString)
        {
            Writer[] writers = JsonConvert.DeserializeObject<Writer[]>(jsonString);
            List<Writer> validWriters = new List<Writer>();
            StringBuilder sb = new StringBuilder();

            foreach (var writer in writers)
            {
                if (!IsValid(writer))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                validWriters.Add(writer);
                sb.AppendLine(string.Format(SuccessfullyImportedWriter, writer.Name));
            }

            context.Writers.AddRange(validWriters);
            context.SaveChanges();
            return sb.ToString().TrimEnd();
        }

        public static string ImportProducersAlbums(MusicHubDbContext context, string jsonString)
        {
            Producer[] producers = JsonConvert.DeserializeObject<Producer[]>(jsonString, new JsonSerializerSettings
            {
                DateFormatString = "dd/MM/yyyy",
            });
            List<Producer> validProducers = new List<Producer>();
            StringBuilder sb = new StringBuilder();

            foreach (var producer in producers)
            {
                if (!IsValid(producer) || !producer.Albums.All(IsValid))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                validProducers.Add(producer);
                string result = "";
                if (producer.PhoneNumber != null)
                {
                    result = string.Format(SuccessfullyImportedProducerWithPhone,
                        producer.Name, producer.PhoneNumber, producer.Albums.Count());
                }
                else
                {
                    result = string.Format(SuccessfullyImportedProducerWithNoPhone, producer.Name, producer.Albums.Count);
                }

                sb.AppendLine(result);
            }

            context.Producers.AddRange(validProducers);
            context.SaveChanges();
            return sb.ToString().TrimEnd();
        }

        public static string ImportSongs(MusicHubDbContext context, string xmlString)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(SongImportDto[]), new XmlRootAttribute("Songs"));
            SongImportDto[] songs = (SongImportDto[])serializer.Deserialize(new StringReader(xmlString));

            HashSet<int> albumIds = context.Albums.Select(a => a.Id).ToHashSet();
            HashSet<int> writerIds = context.Writers.Select(w => w.Id).ToHashSet();
            StringBuilder sb = new StringBuilder();
            List<Song> validSongs = new List<Song>();

            foreach (var song in songs)
            {
                if (!IsValid(song) || !writerIds.Contains(song.WriterId) ||
                    (song.AlbumId.HasValue && !albumIds.Contains(song.AlbumId.Value)))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                bool validGenre = Enum.TryParse(song.Genre, out Genre genre);
                if (!validGenre)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                Song validSong = new Song()
                {
                    Name = song.Name,
                    Price = song.Price,
                    CreatedOn = DateTime.ParseExact(song.CreatedOn, "dd/MM/yyyy", CultureInfo.InvariantCulture),
                    Duration = TimeSpan.ParseExact(song.Duration, "c", CultureInfo.InvariantCulture),
                    AlbumId = song.AlbumId,
                    WriterId = song.WriterId
                };
                validSongs.Add(validSong);
                sb.AppendLine(string.Format(SuccessfullyImportedSong, song.Name, song.Genre, song.Duration));
            }

            context.Songs.AddRange(validSongs);
            context.SaveChanges();
            return sb.ToString().TrimEnd();
        }

        public static string ImportSongPerformers(MusicHubDbContext context, string xmlString)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(PerformerImportDto[]), new XmlRootAttribute("Performers"));
            PerformerImportDto[] performers = (PerformerImportDto[])serializer.Deserialize(new StringReader(xmlString));

            HashSet<int> songIds = context.Songs.Select(s => s.Id).ToHashSet();
            StringBuilder sb = new StringBuilder();
            List<Performer> validPerformers = new List<Performer>();

            foreach (var performer in performers)
            {
                if (!IsValid(performer) || !performer.PerformersSongs.All(x => songIds.Contains(x.Id)))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                Performer validPerformer = new Performer()
                {
                    FirstName = performer.FirstName,
                    LastName = performer.LastName,
                    Age = performer.Age,
                    NetWorth = performer.NetWorth,
                    PerformerSongs = performer.PerformersSongs.Select(s => new SongPerformer()
                    {
                        SongId = s.Id
                    })
                   .ToList()
                };
                validPerformers.Add(validPerformer);
                sb.AppendLine(string.Format(SuccessfullyImportedPerformer, performer.FirstName, performer.PerformersSongs.Length));
            }

            context.Performers.AddRange(validPerformers);
            context.SaveChanges();
            return sb.ToString().TrimEnd();
        }

        private static bool IsValid(object entity)
        {
            ValidationContext context = new ValidationContext(entity);
            List<ValidationResult> validationResults = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(entity, context, validationResults, true);
            return isValid;
        }
    }
}