using System.ComponentModel.DataAnnotations;

namespace LSR.Common
{
    public class AdminLogin
    {
        //[Required(ErrorMessage = "请输入电话号码")]
        //[Display(Name = "Phone")]
        //[DataType(DataType.PhoneNumber)]
        //public string Phone { get; set; }

        [Display(Name = "Name")]
        //[Required(ErrorMessage = "请填写电子邮箱")]
        //[DataType(DataType.EmailAddress)]
        public string  Name { get; set; }

        //[Required(ErrorMessage = "请输入您的密码")]
        [Display(Name = "Pwd")]
        //[DataType(DataType.Password )]
        public string Pwd { get; set; }
        //public User_Info GetLogin(string Phone,string Password)
        //{
        //    using(LSREntities db = new LSREntities())
        //    {
        //        return db.User_InfoSet.FirstOrDefault(model=>model.Equals(Phone)&&model.Equals(Password));
        //    }
        //}
    }
}