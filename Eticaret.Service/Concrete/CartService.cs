using Eticaret.Core.Entities;
using Eticaret.Service.Abstract;
using System.Collections.Generic;
using System.Linq;

namespace Eticaret.Service.Concrete
{
    public class CartService : ICartService
    {
        public List<CartLine> CartLines = new();

        public void AddProduct(Product product, int quantity)
        {
            var urun = CartLines.FirstOrDefault(p => p.Product.Id == product.Id);
            if (urun != null)
            {
                urun.Quantity += quantity;
            }
            else
            {
                CartLines.Add(new CartLine
                {
                    Quantity = quantity,
                    Product = product
                });
            }
        }

        public void ClearAll()
        {
            CartLines.Clear();
        }

        // Geriye çıkarılan ürünün adedini döndürecek şekilde güncellendi
        public int RemoveProduct(Product product)
        {
            var cartLine = CartLines.FirstOrDefault(p => product.Id == p.Product.Id);
            if (cartLine != null)
            {
                int removedQuantity = cartLine.Quantity;
                CartLines.RemoveAll(p => product.Id == p.Product.Id);
                return removedQuantity; // Çıkarılan ürünün adedini döndür
            }
            return 0; // Ürün sepette bulunamazsa 0 döndür
        }

        public decimal TotalPrice() //SEPET TOPLAMI
        {
            return CartLines.Sum(c => c.Product.Price * c.Quantity);
        }

        public void UpdateProduct(Product product, int quantity)
        {
            var urun = CartLines.FirstOrDefault(p => p.Product.Id == product.Id);
            if (urun != null)
            {
                urun.Quantity = quantity;
            }
            else
            {
                CartLines.Add(new CartLine
                {
                    Quantity = quantity,
                    Product = product
                });
            }
        }
    }
}
        
    
