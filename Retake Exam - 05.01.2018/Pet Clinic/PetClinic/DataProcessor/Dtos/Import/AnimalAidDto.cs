using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace PetClinic.DataProcessor.Dtos.Import
{
    [XmlType("AnimalAid")]
    public class AnimalAidDto
    {
        [Required]
        [MinLength(3), MaxLength(30)]
        public string Name { get; set; }
    }
}
