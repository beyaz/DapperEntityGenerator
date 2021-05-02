using System;
using Dapper.Contrib.Extensions;

namespace SuperoBusiness.SuperOEntity.dbo
{
    [Serializable]
    [Table("dbo.Duyuru")]
    public sealed class Duyuru
    {
        [Key]
        public int Id { get; set; }
        
        public string Tipi { get; set; }
        
        public string DuyuruAdi { get; set; }
        
        public DateTime BaslangicTarihi { get; set; }
        
        public int SubeId { get; set; }
        
    }
}
