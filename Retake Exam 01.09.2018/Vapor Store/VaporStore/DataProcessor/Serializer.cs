namespace VaporStore.DataProcessor
{
    using System;
    using Data;
    using System.Linq;
    using Newtonsoft.Json;
    using VaporStore.Data.Models.Enums;
    using VaporStore.DataProcessor.Dtos.Export;
    using System.Globalization;
    using AutoMapper;
    using System.Xml.Serialization;
    using System.Text;
    using System.IO;
    using System.Collections.Generic;
    using System.Xml;

    public static class Serializer
    {
        public static string ExportGamesByGenres(VaporStoreDbContext context, string[] genreNames)
        {
            var genres = context.Genres
                .Where(g => genreNames.Contains(g.Name))
                .Select(g => new
                {
                    Id = g.Id,
                    Genre = g.Name,
                    Games = g.Games
                    .Where(game => game.Purchases.Count > 0)
                    .Select(game => new
                    {
                        Id = game.Id,
                        Title = game.Name,
                        Developer = game.Developer.Name,
                        Tags = string.Join(", ", game.GameTags.Select(t => t.Tag.Name).ToList()),
                        Players = game.Purchases.Count
                    })
                    .OrderByDescending(x => x.Players)
                    .ThenBy(x => x.Id)
                    .ToList(),
                    TotalPlayers = g.Games.Sum(game => game.Purchases.Count)
                })
                .OrderByDescending(g => g.TotalPlayers)
                .ThenBy(g => g.Id)
                .ToList();

            string result = JsonConvert.SerializeObject(genres);
            return result;
        }

        public static string ExportUserPurchasesByType(VaporStoreDbContext context, string storeType)
        {
            PurchaseType purchaseType = Enum.Parse<PurchaseType>(storeType);

            List<UserDto> users = context.Users.
                Where(u => u.Cards.SelectMany(c => c.Purchases).Any(p => p.Type == purchaseType))
                .Select(u => new UserDto()
                {
                    Username = u.Username,
                    Purchases = u.Cards.SelectMany(c => c.Purchases)
                    .Where(p => p.Type == purchaseType)
                    .OrderBy(p => p.Date)
                    .Select(p => new PurchaseDto()
                    {
                        Card = p.Card.Number,
                        Cvc = p.Card.Cvc,
                        Date = p.Date.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture),
                        Game = Mapper.Map<GameDto>(p.Game)
                    })
                    .ToList(),
                    TotalSpent = u.Cards.SelectMany(c => c.Purchases)
                    .Where(p => p.Type == purchaseType)
                    .Sum(p => p.Game.Price)
                })
                .OrderByDescending(u => u.TotalSpent)
                .ThenBy(u => u.Username)
                .ToList();

            XmlSerializer serializer = new XmlSerializer(typeof(List<UserDto>), new XmlRootAttribute("Users"));
            StringBuilder sb = new StringBuilder();
            StringWriter reader = new StringWriter(sb);

            var namesapces = new XmlSerializerNamespaces(new[]
            {
                XmlQualifiedName.Empty
            });
            serializer.Serialize(reader, users, namesapces);
            return sb.ToString().TrimEnd();
        }
    }
}