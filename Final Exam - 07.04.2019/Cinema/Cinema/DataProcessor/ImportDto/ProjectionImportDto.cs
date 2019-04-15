using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace Cinema.DataProcessor.ImportDto
{
    [XmlType("Projection")]
    public class ProjectionImportDto
    {
        public int MovieId { get; set; }

        public int HallId { get; set; }

        [Required]
        public string DateTime { get; set; }
    }
}
