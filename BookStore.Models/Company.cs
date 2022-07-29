using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.Models
{
    public class Company
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string CompanyName { get; set; }

        [StringLength(100)]
        [Display(Prompt = "Stree address")]
        public string? StreetAddress { get; set; }

        [StringLength(100)]
        public string? City { get; set; }

        [Required]
        [StringLength(6)]
        public string PostalCode { get; set; }

        [Required]
        [StringLength(100)]
        public string State { get; set; }

        [Required]
        [StringLength(20)]
        [MinLength(10, ErrorMessage = "Please enter valid Mobile number"), MaxLength(10, ErrorMessage = "Please enter valid Mobile number")]
        [Display(Prompt = "Mobile number")]
        public string MobileNo { get; set; }
    }
}
