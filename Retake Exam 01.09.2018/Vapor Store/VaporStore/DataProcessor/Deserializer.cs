namespace VaporStore.DataProcessor
{
    using System;
    using Data;
    using Newtonsoft.Json;
    using VaporStore.DataProcessor.Dtos.Import;
    using System.Collections.Generic;
    using VaporStore.Data.Models;
    using System.ComponentModel.DataAnnotations;
    using System.Text;
    using System.Linq;
    using System.Globalization;
    using VaporStore.Data.Models.Enums;
    using System.Xml.Serialization;
    using System.IO;

    public static class Deserializer
    {
        public static string ImportGames(VaporStoreDbContext context, string jsonString)
        {
            var gameDtos = JsonConvert.DeserializeObject<ImportGameDto[]>(jsonString);
            List<Game> games = new List<Game>();
            var sb = new StringBuilder();

            foreach (var gameDto in gameDtos)
            {
                if (!IsValid(gameDto) || gameDto.Tags.Count == 0)
                {
                    sb.AppendLine("Invalid Data");
                    continue;
                }

                Developer developer = GetDeveloper(context, gameDto.Developer);
                Genre genre = GetGenre(context, gameDto.Genre);

                Game game = new Game()
                {
                    Name = gameDto.Name,
                    Price = gameDto.Price,
                    ReleaseDate = DateTime.ParseExact(gameDto.ReleaseDate, "yyyy-MM-dd", CultureInfo.InvariantCulture),
                    Developer = developer,
                    Genre = genre
                };

                foreach (var tagName in gameDto.Tags)
                {
                    Tag tag = GetTag(context, tagName);

                    game.GameTags.Add(new GameTag()
                    {
                        Tag = tag
                    });
                }

                games.Add(game);
                sb.AppendLine($"Added {game.Name} ({genre.Name}) with {game.GameTags.Count} tags");
            }

            context.Games.AddRange(games);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportUsers(VaporStoreDbContext context, string jsonString)
        {
            var userDtos = JsonConvert.DeserializeObject<ImportUserDto[]>(jsonString);
            StringBuilder sb = new StringBuilder();
            List<User> users = new List<User>();
                 
            foreach (var userDto in userDtos)
            {
                if (!IsValid(userDto) || userDto.Cards.Count == 0
                    || !userDto.Cards.All(IsValid))
                {
                    sb.AppendLine("Invalid Data");
                    continue;
                }

                bool cardsAreValid = true;
                foreach (var card in userDto.Cards)
                {
                    cardsAreValid = Enum.TryParse<CardType>(card.Type, out CardType type);

                    if (!cardsAreValid)
                    {
                        break;
                    }
                }

                if (!cardsAreValid)
                {
                    sb.AppendLine("Invalid Data");
                    continue;
                }

                var user = new User()
                {
                    Username = userDto.Username,
                    FullName = userDto.FullName,
                    Age = userDto.Age,
                    Email = userDto.Email,
                    Cards = userDto.Cards.Select(c => new Card()
                    {
                        Number = c.Number,
                        Type = Enum.Parse<CardType>(c.Type),
                        Cvc = c.CVC
                    })
                    .ToList()
                };

                users.Add(user);
                sb.AppendLine($"Imported {user.Username} with {user.Cards.Count} cards");
            }

            context.Users.AddRange(users);
            context.SaveChanges();
            return sb.ToString().TrimEnd();
        }

        public static string ImportPurchases(VaporStoreDbContext context, string xmlString)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(ImportPurchaseDto[]), new XmlRootAttribute("Purchases"));
            ImportPurchaseDto[] purchaseDtos = (ImportPurchaseDto[])serializer.Deserialize(new StringReader(xmlString));

            StringBuilder sb = new StringBuilder();
            List<Purchase> purchases = new List<Purchase>();

            foreach (var purchaseDto in purchaseDtos)
            {
                if (!IsValid(purchaseDto))
                {
                    sb.AppendLine("Invalid Data");
                    continue;
                }

                bool typeIsValid = Enum.TryParse(purchaseDto.Type, out PurchaseType purchaseType);
                if (!typeIsValid)
                {
                    sb.AppendLine("Invalid Data");
                    continue;
                }

                Game game = context.Games.FirstOrDefault(g => g.Name == purchaseDto.Title);
                Card card = context.Cards.FirstOrDefault(c => c.Number == purchaseDto.Card);

                if(card == null || game== null)
                {
                    sb.AppendLine("Invalid Data");
                    continue;
                }

                Purchase purchase = new Purchase()
                {
                    ProductKey = purchaseDto.Key,
                    Date = DateTime.ParseExact(purchaseDto.Date, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture),
                    Type = purchaseType,
                    GameId = game.Id,
                    CardId = card.Id
                };

                purchases.Add(purchase);
                sb.AppendLine($"Imported {game.Name} for {card.User.Username}");
            }

            context.Purchases.AddRange(purchases);
            context.SaveChanges();
            return sb.ToString().TrimEnd();
        }

        private static Tag GetTag(VaporStoreDbContext context, string tagName)
        {
            Tag tag = context.Tags.FirstOrDefault(t => t.Name == tagName);

            if (tag == null)
            {
                tag = new Tag()
                {
                    Name = tagName
                };

                context.Tags.Add(tag);
                context.SaveChanges();
            }

            return tag;
        }

        private static Genre GetGenre(VaporStoreDbContext context, string genreName)
        {
            Genre genre = context.Genres.FirstOrDefault(g => g.Name == genreName);

            if(genre == null)
            {
                genre = new Genre()
                {
                    Name = genreName
                };

                context.Genres.Add(genre);
                context.SaveChanges();
            }

            return genre;
        }

        private static Developer GetDeveloper(VaporStoreDbContext context, string developerName)
        {
            Developer developer = context.Developers.FirstOrDefault(d => d.Name == developerName);

            if (developer == null)
            {
                developer = new Developer
                {
                    Name = developerName
                };

                context.Developers.Add(developer);
                context.SaveChanges();
            }

            return developer;
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