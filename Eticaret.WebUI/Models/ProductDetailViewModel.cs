using Eticaret.Core.Entities;
using System;
using System.Collections.Generic;

namespace Eticaret.WebUI.Models
{
    public class ProductDetailViewModel
    {
        public Product? Product { get; set; }

        public IEnumerable<Product>? RelatedProducts { get; set; }

 
    }
}