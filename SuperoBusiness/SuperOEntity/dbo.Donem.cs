using System;
using Dapper.Contrib.Extensions;

namespace SuperoBusiness.SuperOEntity.dbo
{
    [Serializable]
    [Table("dbo.Donem")]
    public sealed class Donem
    {
        [Key]
        public int DonemId { get; set; }
        
        public string DonemAdi { get; set; }
        
    }
}
