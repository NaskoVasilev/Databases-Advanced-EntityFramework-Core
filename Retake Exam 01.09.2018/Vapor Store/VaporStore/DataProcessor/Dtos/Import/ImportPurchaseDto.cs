using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Xml.Serialization;

namespace VaporStore.DataProcessor.Dtos.Import
{
    [XmlType("Purchase")]
    public class ImportPurchaseDto
    {
        [Required]
        [XmlAttribute("title")]
        public string Title { get; set; }

        [Required]
        [XmlElement]
        public string Type { get; set; }

        [RegularExpression("^[A-Z0-9]{4}-[A-Z0-9]{4}-[A-Z0-9]{4}$")]
        [Required]
        public string Key { get; set; }

        [Required]
        [RegularExpression(@"^\d{4}\s+\d{4}\s+\d{4}\s+\d{4}$")]
        [XmlElement]
        public string Card { get; set; }

        [Required]
        [XmlElement]
        public string Date { get; set; }
    }
}
