using System;
using Dapper.Contrib.Extensions;

namespace SuperoBusiness.SuperOEntity.dbo
{
    [Serializable]
    [Table("dbo.Kullanici")]
    public sealed class Kullanici
    {
        [Key]
        public int KullaniciId { get; set; }
        
        public int? KurumId { get; set; }
        
        public int? SubeId { get; set; }
        
        public string TcKimlikNo { get; set; }
        
        public string Ad { get; set; }
        
        public string Soyad { get; set; }
        
        public string Email { get; set; }
        
        public string KontakEmail { get; set; }
        
        public string Sifre { get; set; }
        
        public string Telefon { get; set; }
        
        public DateTime? DogumTarihi { get; set; }
        
        public int? CinsiyetId { get; set; }
        
        public bool? AktifMi { get; set; }
        
        public string AktivasyonKodu { get; set; }
        
        public bool? KolejKullanicisiMi { get; set; }
        
        public DateTime KayitTarihi { get; set; }
        
        public string LastAuthenticationToken { get; set; }
        
    }
}
