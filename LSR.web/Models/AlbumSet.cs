//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace LSR.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class AlbumSet
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public AlbumSet()
        {
            this.MusicSet = new HashSet<MusicSet>();
        }
    
        public long AlbumId { get; set; }
        public long ArtistId { get; set; }
        [Display(Name = "发行日期")]
        public System.DateTime PublishDate { get; set; }
        public string Info { get; set; }
        
        [Display(Name = "专辑")]
        public string Name { get; set; }
        public string Image { get; set; }
        public Nullable<bool> IsChecked { get; set; }
        public Nullable<bool> IsAvailable { get; set; }
    
        public virtual ArtistSet ArtistSet { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MusicSet> MusicSet { get; set; }
    }
}
