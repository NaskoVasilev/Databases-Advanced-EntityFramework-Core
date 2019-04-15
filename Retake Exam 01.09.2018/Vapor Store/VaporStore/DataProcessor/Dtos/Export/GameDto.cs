using System.Xml.Serialization;

namespace VaporStore.DataProcessor.Dtos.Export
{
    [XmlType("Game")]
    public class GameDto
    {
        [XmlAttribute("title")]
        public string Title { get; set; }

        [XmlElement]
        public string Genre { get; set; }

        [XmlElement]
        public decimal Price { get; set; }

    }
}
