namespace Eticaret.Core.Entities
{
    public class AppUser :IEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string? Phone { get; set; }
        public string Password { get; set; }
        public string? UserName { get; set; }
        public bool IsActive { get; set; } //kullanıcı aktif pasif
        public bool IsAdmin { get; set; } //kullanıcı admin mi
        public DateTime CreateDate { get; set; }
        public Guid? UserGuid { get; set; }= Guid.NewGuid();  
        //public DateTime? UpdateDate { get; set; }


    }
}
