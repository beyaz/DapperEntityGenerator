using System;
using Dapper.Contrib.Extensions;

namespace SuperoBusiness.SuperOEntity.dbo
{
    [Serializable]
    [Table("dbo.EgitimOgretimYil")]
    public sealed class EgitimOgretimYil
    {
        [Key]
        public int EgitimOgretimYilId { get; set; }
        
        public string EgitimOgretimYili { get; set; }
        
        public bool AktifMi { get; set; }
        
    }
}
