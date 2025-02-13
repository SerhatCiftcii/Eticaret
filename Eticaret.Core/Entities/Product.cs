namespace Eticaret.Core.Entities
{
    public class Product:IEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? Image { get; set; }
        public Decimal Price { get; set; }
        public string? ProductCode { get; set; }//ürünkodu
        public int Stock { get; set; }
        public bool IsActive { get; set; }
        public bool IsHome { get; set; }
        public int? CategoryId { get; set; }
        public Category? Category { get; set; } 
        public int? BrandId { get; set; }
        public Brand? Brand { get; set; }
        public string OrderNo { get; set; }//sıralama
        public DateTime CreateDate { get; set; }
    }
}
