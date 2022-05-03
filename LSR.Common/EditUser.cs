using System;
using System.ComponentModel.DataAnnotations;

namespace LSR.Common
{
    public class EditUser
    {
        public EditUser()
        {

        }
        
        [Display(Name = "UserName")]
        [Required(ErrorMessage = "请填写用户名")]
        public string UserName { get; set; }

        [Display(Name = "Gender")]
        public string Gender { get; set; }

        [Display(Name = "UserEmail")]
        [Required(ErrorMessage = "请填写电子邮箱")]
        [DataType(DataType.EmailAddress)]
        public string UserEmail { get; set; }

        [Display(Name = "Phone")]
        [Required(ErrorMessage = "请填写电话号码")]
        [DataType(DataType.PhoneNumber)]
        public long Phone { get; set; }

        [Display(Name = "Birthday")]
        [DataType(DataType.Date)]
        public DateTime Birthday { get; set; }
        
    }
}