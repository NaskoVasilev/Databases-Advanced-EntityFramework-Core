using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PetClinic.Models
{
    public class Procedure
    {
        public Procedure()
        {
            this.ProcedureAnimalAids = new List<ProcedureAnimalAid>();
        }

        public int Id { get; set; }

        public int AnimalId { get; set; }
        public Animal Animal { get; set; }

        public int VetId { get; set; }
        public Vet Vet { get; set; }

        public decimal Cost => this.ProcedureAnimalAids.Sum(p => p.AnimalAid.Price);

        public DateTime DateTime { get; set; }

        public ICollection<ProcedureAnimalAid> ProcedureAnimalAids { get; set; }
    }
}
