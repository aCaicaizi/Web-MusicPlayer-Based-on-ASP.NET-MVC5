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
    
    public partial class User_Music_Like_rel
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public long MusicId { get; set; }
    
        public virtual MusicSet MusicSet { get; set; }
        public virtual User_InfoSet User_InfoSet { get; set; }
    }
}
