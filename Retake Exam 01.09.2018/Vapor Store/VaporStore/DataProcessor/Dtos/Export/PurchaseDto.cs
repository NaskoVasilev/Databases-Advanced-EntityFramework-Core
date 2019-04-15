using System.Xml.Serialization;

namespace VaporStore.DataProcessor.Dtos.Export
{
    [XmlType("Purchase")]
    public class PurchaseDto
    {
        [XmlElement]
        public string Card { get; set; }

        [XmlElement]
        public string Cvc { get; set; }

        [XmlElement]
        public string Date { get; set; }

        public GameDto Game { get; set; }
    }
}
