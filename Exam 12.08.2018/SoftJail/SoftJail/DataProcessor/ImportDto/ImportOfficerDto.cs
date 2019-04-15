using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace SoftJail.DataProcessor.ImportDto
{
    [XmlType("Officer")]
    public class ImportOfficerDto
    {
        [Required]
        [StringLength(30, MinimumLength = 3)]
        public string Name { get; set; }

        [Range(typeof(decimal), "0.00", " 79228162514264337593543950335")]
        public decimal Money { get; set; }

        [XmlElement]
        [Required]
        public string Position { get; set; }

        [XmlElement]
        [Required]
        public string Weapon { get; set; }

        public int DepartmentId { get; set; }

        [XmlArray("Prisoners")]
        public List<ImportOfficerPrisonerDto> Prisoners { get; set; }
    }
}
