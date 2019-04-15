using System.Collections.Generic;
using System.Xml.Serialization;

namespace VaporStore.DataProcessor.Dtos.Export
{
    [XmlType("User")]
    public class UserDto
    {
        [XmlAttribute("username")]
        public string Username { get; set; }

        [XmlArray]
        public List<PurchaseDto> Purchases { get; set; }

        [XmlElement]
        public decimal TotalSpent { get; set; }
    }
}
