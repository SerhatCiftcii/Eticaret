namespace Eticaret.Core.Entities
{
    public class Category : IEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? Image { get; set; }
        public bool IsActive { get; set; }
        public bool IsTopMenu { get; set; }//katogriy üst menuda gözüksün mü
        public int? ParentId { get; set; }//ana katogry alt kategory oluturma
        public int OrderNo { get; set; }//sıralama
        public DateTime CreateDate { get; set; }=DateTime.Now;
        public IList<Product>? Products { get; set; }
    }
}
