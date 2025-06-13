using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace ECF.Models
{
    public class Animal
    {
        [Key]
        public int AnimalId { get; set; }

        [Required]
        [MaxLength(50)]
        [Display(Name = "Nom")]
        public string Name { get; set; } = null!;

        [Required]
        [MaxLength(2000)]
        [Display(Name = "Description")]
        public string Description { get; set; } = null!;

        [Required]
        [ForeignKey("Breed")]
        [Display(Name = "RaceId")]
        public int BreedId { get; set; }
        public virtual Breed Breed { get; set; } = null!;
    }
}
