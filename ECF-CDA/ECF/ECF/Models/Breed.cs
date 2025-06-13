using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ECF.Models
{
    public class Breed
    {
        [Key]
        public int BreedId { get; set; }

        [Required, MaxLength(50)]
        public string BreedName { get; set; } = null!;

        [Required]
        [MaxLength(2000)]
        [Display(Name = "Description")]
        public string Description { get; set; } = null!;

        public virtual ICollection<Animal> Animals { get; set; } = new List<Animal>();
    }
}
