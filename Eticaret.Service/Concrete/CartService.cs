using Eticaret.Core.Entities;
using Eticaret.Service.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eticaret.Service.Concrete
{
  
    public class CartService : ICartService
    {
        public List<CartLine> CartLines = new();
        public void AddProduct(Product product, int quantity)
        {
            throw new NotImplementedException();
        }

        public void ClearAll()
        {
            throw new NotImplementedException();
        }

        public void RemoveProduct(Product product)
        {
            throw new NotImplementedException();
        }

        public decimal TotalPrice()
        {
            throw new NotImplementedException();
        }

        public void UpdateProduct(Product product, int quantity)
        {
            throw new NotImplementedException();
        }
    }
}
