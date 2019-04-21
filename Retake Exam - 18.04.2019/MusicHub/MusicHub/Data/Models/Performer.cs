using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MusicHub.Data.Models
{
    public class Performer
    {
        public Performer()
        {
            this.PerformerSongs = new List<SongPerformer>();
        }

        public int Id { get; set; }

        [StringLength(20, MinimumLength = 3)]
        public string FirstName { get; set; }

        [StringLength(20, MinimumLength = 3)]
        public string LastName { get; set; }

        [NotMapped]
        public string FullName => this.FirstName + ' ' + this.LastName;

        [Range(18, 70)]
        public int Age { get; set; }

        [Range(typeof(decimal), "0.00", " 79228162514264337593543950335")]
        public decimal NetWorth { get; set; }

        public ICollection<SongPerformer> PerformerSongs { get; set; }
    }
}
