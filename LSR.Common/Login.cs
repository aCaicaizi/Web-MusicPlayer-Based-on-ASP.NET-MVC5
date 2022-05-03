using System.ComponentModel.DataAnnotations;
namespace LSR.Common
{
    public class Login
    {
        //[Required(ErrorMessage = "请输入电话号码")]
        //[Display(Name = "Phone")]
        //[DataType(DataType.PhoneNumber)]
        //public string Phone { get; set; }

        [Display(Name = "UserEmail")]
        [Required(ErrorMessage = "请填写电子邮箱")]
        [DataType(DataType.EmailAddress)]
        public string UserEmail { get; set; }

        [Required(ErrorMessage = "请输入您的密码")]
        [Display(Name = "PassWord")]
        [DataType(DataType.Password)]
        public string PassWord { get; set; }
        //public User_Info GetLogin(string Phone,string Password)
        //{
        //    using(LSREntities db = new LSREntities())
        //    {
        //        return db.User_InfoSet.FirstOrDefault(model=>model.Equals(Phone)&&model.Equals(Password));
        //    }
        //}
        
    }
}