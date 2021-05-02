using System;
using Dapper.Contrib.Extensions;

namespace SuperoBusiness.SuperOEntity.dbo
{
    [Serializable]
    [Table("dbo.EditorDersYetki")]
    public sealed class EditorDersYetki
    {
        [Key]
        public int Id { get; set; }
        
        public int DersId { get; set; }
        
        public int KullaniciId { get; set; }
        
    }
}
