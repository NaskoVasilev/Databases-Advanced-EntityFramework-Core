using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Instagraph.Models
{
    public class Picture
    {
        public Picture()
        {
            this.Users = new List<User>();
            this.Posts = new List<Post>();
        }

        public int Id { get; set; }

        [Required]
        public string Path { get; set; }

        [Range(typeof(decimal), "0.01", " 79228162514264337593543950335")]
        public decimal Size { get; set; }

        public ICollection<User> Users { get; set; }

        public ICollection<Post> Posts { get; set; }
    }
}
