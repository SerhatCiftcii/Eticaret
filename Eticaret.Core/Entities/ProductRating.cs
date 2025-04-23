using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Eticaret.Core.Entities
{
    public class ProductRating : IEntity
    {
        public int Id { get; set; }

        [Range(1, 5)]
        [Display(Name = "Puan")]
        public int Rating { get; set; } // 1-5 yıldız

        [Display(Name = "Yorum")]
        public string? Comment { get; set; }

        [Display(Name = "Puanlama Tarihi")]
        public DateTime RatedAt { get; set; } = DateTime.Now;

        // İlişkiler
        public int ProductId { get; set; }
        public Product Product { get; set; }

        public int AppUserId { get; set; }
        public AppUser AppUser { get; set; }
    }
}
