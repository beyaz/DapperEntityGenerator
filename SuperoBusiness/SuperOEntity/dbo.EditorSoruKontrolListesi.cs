using System;
using Dapper.Contrib.Extensions;

namespace SuperoBusiness.SuperOEntity.dbo
{
    [Serializable]
    [Table("dbo.EditorSoruKontrolListesi")]
    public sealed class EditorSoruKontrolListesi
    {
        [Key]
        public int Id { get; set; }
        
        public string KontrolListesi { get; set; }
        
    }
}
