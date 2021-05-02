using System;
using Dapper.Contrib.Extensions;

namespace SuperoBusiness.SuperOEntity.dbo
{
    [Serializable]
    [Table("dbo.Ders")]
    public sealed class Ders
    {
        [Key]
        public int DersId { get; set; }
        
        public int? KurumId { get; set; }
        
        public int? SubeId { get; set; }
        
        public int? SinifId { get; set; }
        
        public int? AnaDersId { get; set; }
        
        public string DersAdi { get; set; }
        
        public bool? MebTemelDersMi { get; set; }
        
        public bool? Aktif { get; set; }
        
        public int? KayitEdenKullaniciId { get; set; }
        
        public DateTime? KayitTarihi { get; set; }
        
    }
}
