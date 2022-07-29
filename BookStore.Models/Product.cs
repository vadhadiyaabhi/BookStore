using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BookStore.Models
{
    public class Product
    {
        //public Product()
        //{
        //    this.CoverType = new CoverType();
        //    this.Category = new Category();
        //}

        [Key]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string? Description { get; set; }

        [Required]
        public string? ISBN { get; set; }

        [Required]
        public string? Author { get; set; }

        [Required]
        [Range(1, 10000)]
        [Column(TypeName = "decimal(8, 2)")]            // This is required in case of decimal, Otherwise SqlServer will not understand which type to use
        // We must need to specify precision, So it will understand 
        public decimal Price { get; set; }

        [Required]
        [Range(1, 10000)]
        [Column(TypeName = "decimal(8, 2)")]
        [Display(Name = "Price For 50+")]
        public decimal Price50 { get; set; }

        [Required]
        [Range(1, 10000)]
        [Column(TypeName = "decimal(8, 2)")]
        [Display(Name = "Price For 100+")]
        public decimal Price100 { get; set; }

        public string? ImageUrl { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Required]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }

        // No need of this, It will understand by default from proper naming convensions, But better for safety purpose
        [ForeignKey("CategoryId")]
        [ValidateNever]                                             // Note down:  this will not be validated while submitting form using Product class
        //[NotMapped]
        public Category Category { get; set; }

        [Required]
        [Display(Name = "Cover Type")]
        public int CoverTypeId { get; set; }

        [ForeignKey("CoverTypeId")]
        [ValidateNever]
        //[NotMapped]
        public CoverType CoverType { get; set; }

    }
}
