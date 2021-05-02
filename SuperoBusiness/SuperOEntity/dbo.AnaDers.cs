using System;
using Dapper.Contrib.Extensions;

namespace SuperoBusiness.SuperOEntity.dbo
{
    [Serializable]
    [Table("dbo.AnaDers")]
    public sealed class AnaDers
    {
        [Key]
        public int AnaDersId { get; set; }
        
        public string AnaDersAdi { get; set; }
        
        public int OgrenimSeviyeId { get; set; }
        
    }
}
