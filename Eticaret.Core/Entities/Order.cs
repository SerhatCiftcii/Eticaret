using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eticaret.Core.Entities
{
   public class Order : IEntity
    {
        public int Id { get; set; }
        [Display(Name ="Sipariş No"), StringLength(50)]
        public string OrderNumber { get; set; } //her siprişe özel number
        [Display(Name = "Sipariş Toplamı")]
        public decimal TotalPrice { get; set; }
        [Display(Name = "Müşteri No")]
        public int AppUserId { get; set; } //hangi kullanıcı sipraiş verdi
        [Display(Name = "Müşteri"), StringLength(50)]
        public string CustomerId { get; set; }
        [Display(Name = "Fatura Adresi"), StringLength(100)]
        public string BillingAddress { get; set; }
        [Display(Name = "Teslimat Adresi"), StringLength(100)]
        public string DeliveryAddress { get; set; }
        [Display(Name = "Sipariş Tarihi")]

        public DateTime OrderDate { get; set; } = DateTime.Now;
        public List<OrderLine>? OrderLines { get; set; }
        [Display(Name = "Müşteri")]
        public AppUser? AppUser { get; set; }
        [Display(Name = "Sipariş Durumu")]
        public EnumOrderState OrderState { get; set; }
    }
    //enum tipinde sipariş durumu :sıralama veriri enum
    public enum EnumOrderState
    {
        [Display(Name = "Onay Bekliyor")]
        Waiting, //onay beklıyor
        [Display(Name = "Onaylandı")]
        Approved, //onaylandı
        [Display(Name = "Kargoya Verildi")]
        Shipped, //kargoya verildi
        [Display(Name = "Tamamlandı")]
        Completed,
       [Display(Name = "İptal Edildi")]
        Cancelled,
       [Display(Name = "İade Edildi")]
       Returned
    }
}
