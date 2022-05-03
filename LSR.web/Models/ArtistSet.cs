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

    public partial class ArtistSet
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ArtistSet()
        {
            this.AlbumSet = new HashSet<AlbumSet>();
            this.MusicSet = new HashSet<MusicSet>();
        }
    
        public long ArtistId { get; set; }
        [Display(Name = "歌手")]
        public string Name { get; set; }
        public string Info { get; set; }
        public string Image { get; set; }
        public string Region { get; set; }
        public string Gender { get; set; }
        public Nullable<int> StyleId { get; set; }
        public Nullable<bool> IsChecked { get; set; }
        public string Initial { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AlbumSet> AlbumSet { get; set; }
        public virtual StyleSet StyleSet { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MusicSet> MusicSet { get; set; }
    }
}
