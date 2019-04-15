using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PetClinic.Models
{
    public class Vet
    {
        public Vet()
        {
            this.Procedures = new List<Procedure>();
        }

        public int Id { get; set; }

        [Required]
        [MinLength(3), MaxLength(40)]
        public string Name { get; set; }

        [Required]
        [MinLength(3), MaxLength(50)]
        public string Profession { get; set; }

        [Range(22, 65)]
        public int Age { get; set; }

        [Required]
        [RegularExpression(@"^\+359\d{9}$|^0\d{9}$")]
        public string PhoneNumber { get; set; }

        public List<Procedure> Procedures { get; set; }
    }
}
