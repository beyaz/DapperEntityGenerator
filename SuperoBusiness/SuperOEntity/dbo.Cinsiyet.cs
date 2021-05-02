using System;
using Dapper.Contrib.Extensions;

namespace SuperoBusiness.SuperOEntity.dbo
{
    [Serializable]
    [Table("dbo.Cinsiyet")]
    public sealed class Cinsiyet
    {
        [Key]
        public int CinsiyetId { get; set; }
        
        public string CinsiyetAdi { get; set; }
        
    }
}
