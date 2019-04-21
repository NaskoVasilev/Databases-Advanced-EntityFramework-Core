using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace MusicHub.DataProcessor.ImportDtos
{
    [XmlType("Performer")]
    public class PerformerImportDto
    {
        [StringLength(20, MinimumLength = 3)]
        public string FirstName { get; set; }

        [StringLength(20, MinimumLength = 3)]
        public string LastName { get; set; }

        [Range(18, 70)]
        public int Age { get; set; }

        [Range(typeof(decimal), "0.00", " 79228162514264337593543950335")]
        public decimal NetWorth { get; set; }

        [XmlArray("PerformersSongs")]
        public PerformerSongsImportDto[] PerformersSongs { get; set; }
    }
}
