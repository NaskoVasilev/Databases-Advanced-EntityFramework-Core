using System.ComponentModel.DataAnnotations;

namespace VaporStore.DataProcessor.Dtos.Import
{
    public class ImportCardDto
    {
        [Required]
        [RegularExpression(@"^\d{4}\s+\d{4}\s+\d{4}\s+\d{4}$")]
        public string Number { get; set; }

        public string Type { get; set; }

        [Required]
        [RegularExpression(@"^\d{3}$")]
        public string CVC { get; set; }
    }
}
