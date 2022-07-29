using System.ComponentModel.DataAnnotations;

namespace BookStore.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [MaxLength(500, ErrorMessage = "Description cannot have more than 500 chars.")]
        public string? Description { get; set; }

        [Display(Name = "Display Order")]
        [Range(1, 100, ErrorMessage = "Diplay order must be from 1 to 100 only")]
        public int DisplayOrder { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
