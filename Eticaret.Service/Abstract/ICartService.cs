using Eticaret.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eticaret.Service.Abstract
{
    public interface ICartService
    {
        void AddProduct(Product product, int quantity);
        void UpdateProduct(Product product, int quantity);
        int RemoveProduct(Product product); // Dönüş tipi int olarak değiştirildi
        decimal TotalPrice(); //toplam fiyatı döndürür
        void ClearAll(); //
    }
}
