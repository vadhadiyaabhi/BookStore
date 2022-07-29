using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace BookStore.Models.ViewModels
{
    public class ProductViewModel
    {
        public Product Product { get; set; }

        [ValidateNever]                                             // Note down:  this will not be validated while submitting form using Product class
        public IEnumerable<SelectListItem> CategoryList { get; set; }

        [ValidateNever]                                             // Note down:  this will not be validated while submitting form using Product class
        public IEnumerable<SelectListItem> CoverTypeList { get; set; }

    }
}
