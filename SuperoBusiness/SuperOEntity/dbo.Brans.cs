using System;
using Dapper.Contrib.Extensions;

namespace SuperoBusiness.SuperOEntity.dbo
{
    [Serializable]
    [Table("dbo.Brans")]
    public sealed class Brans
    {
        [Key]
        public int BransId { get; set; }
        
        public string BransAdi { get; set; }
        
    }
}
