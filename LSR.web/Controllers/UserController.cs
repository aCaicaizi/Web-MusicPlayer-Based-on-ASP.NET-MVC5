using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using LSR.Common;
using LSR.Models;

namespace LSR.Controllers
{
    public class UserController : Controller
    {
        private LSREntities db = new LSREntities();

        // GET: User/Create
        public ActionResult Create()
        {
            if (Session["UserId"] == null)
            {
                Response.Write("<script>alert('走开太久咯，请您新登录哦！！(๑•̀ㅂ•́)و✧');window.location.href='../User/Login'</script>");
                return Content("登录状态已过期");
            }
            return View();
        }

        // POST: User/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性；有关
        // 更多详细信息，请参阅 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include ="UserEmail,PassWord")]User_InfoSet userCreating)
        {
            if (Session["UserId"] == null)
            {
                Response.Write("<script>alert('走开太久咯，请您新登录哦！！(๑•̀ㅂ•́)و✧');window.location.href='../User/Login'</script>");
                return Content("登录状态已过期");

            }
            if (!ModelState.IsValid) return View();
                
                using (var db = new LSREntities())
                {
                    db.User_InfoSet.Add(userCreating);
                    db.SaveChanges();
                    var userLogining = db.User_InfoSet.FirstOrDefault(model => model.UserEmail.Equals(userCreating.UserEmail));
                    
                    Login(userLogining);

                }
            return RedirectToAction("Index", "Music");

        }

        public ActionResult Edit()
        {
            long? id;
            if ((id=(long ? )Session["UserId"]) == null)
            {
                Response.Write("<script>alert('走开太久咯，请您新登录哦！！(๑•̀ㅂ•́)و✧');window.location.href='../User/Login'</script>");
                return Content("登录状态已过期");

            var id = Convert.ToInt64(CreateSessionWithCookie());
            
            User_InfoSet user;
            try { 
                user = db.User_InfoSet.Find(id); 
                ViewBag.Gender = user.Gender;            
                return View(user); 
            }
            catch (Exception e) { Response.Write(e); }
            //return RedirectToAction("Index", "Music");
            return Content("");
            
        }

        // POST: User/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性；有关
        // 更多详细信息，请参阅 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "UserName,Gender,Phone,Birthday,Image")]User_InfoSet user)
        {
            if (Session["UserId"] == null)
            {
                Response.Write("<script>alert('走开太久咯，请您新登录哦！！(๑•̀ㅂ•́)و✧');window.location.href='../User/Login'</script>");
                return Content("登录状态已过期");

            }
            //if (ModelState.IsValid)
            //{

            try
            {
                    var userEditing = db.User_InfoSet.Find((long?)Session["UserId"]);

                if (user.Image.Length > 100) 
                { 
                    var mImage = ImgProcession.Base64ToImage(user.Image);
                    Bitmap bp = new Bitmap(mImage);
                    var thisFilePath = DateTime.Now.Ticks + ".jpg";
                    var thisFileFullPath = Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["AvatarFolder_cs"]) + "\\" + thisFilePath;
                    bp.Save(thisFileFullPath, System.Drawing.Imaging.ImageFormat.Jpeg);
                    user.Image = thisFilePath;
                }
                    
                    
                    userEditing.Modified(user);

                   
                    db.Entry(userEditing).State = EntityState.Modified;
                    db.SaveChanges();


                    Session["UserAvatar_js"] = System.Configuration.ConfigurationManager.AppSettings["AvatarFolder_js"]+ "/" +userEditing.Image;
                    Session["AvatarBase64"] = ImgProcession.ImageToBase64(Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["AvatarFolder_cs"] + "\\" + userEditing.Image)); 
                    Session["UserAvatar_cs"] = System.Configuration.ConfigurationManager.AppSettings["AvatarFolder_cs"] + "/" + userEditing.Image;
            }
                catch (DbEntityValidationException ex)
                {
                    StringBuilder str = new StringBuilder();
                    foreach (var p in ex.EntityValidationErrors)
                    {
                        foreach (var i in p.ValidationErrors)
                        {
                            str.AppendFormat("{0}|", i.ErrorMessage);
                        }
                    }
                Response.Write(str.ToString());
                Response.End();
                }
            return RedirectToAction("Index", "Music");
            //}
            //else Response.Write("模型验证不成功");
            //Response.Write("<script>alert('保存成功');window.location.href='../Music/Index'</script>");
            //return Content("");
        }
        //[HttpPost]
        //public ActionResult AvatarUpload(string AvatarBase64)
        //{
        //    //此处查询字符串赋值失败

        //    //if (AvatarBase64 == null || AvatarBase64 == "") { AvatarBase64 = ImgProcession.ImageToBase64(Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["AvatarFolder_cs"]) + "\\" + System.Configuration.ConfigurationManager.AppSettings["DefaultAvatar"]); }
        //        var mImage = ImgProcession.Base64ToImage(AvatarBase64); 
        //        Bitmap bp = new Bitmap(mImage);

        //        var thisFilePath = DateTime.Now.Ticks + ".jpg";
        //        var thisFileFullPath = Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["AvatarFolder_cs"]) + "\\" + thisFilePath;
        //        bp.Save(thisFileFullPath, System.Drawing.Imaging.ImageFormat.Jpeg);
        //        return Content(thisFilePath);


        //    //注意保存路径
            
        //    //return Content(AvatarBase64);}

        //}

        public ActionResult Login()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login([Bind(Include ="UserEmail,PassWord")] User_InfoSet user)
        {
            if (ModelState.IsValid)
            {
                var userLogining = db.User_InfoSet.FirstOrDefault(model => (model.UserEmail.Equals(user.UserEmail)/*||model.UserEmail.Equals(login.UserEmail)*/) && model.PassWord.Equals(user.PassWord));
                if (userLogining != null)
                {
                    Session["UserId"] = userLogining.UserId;
                    Session["UserName"] = userLogining.UserName;

                    if (userLogining.Image != null && userLogining.Image != "")
                    {
                        Session["UserAvatar_cs"] = System.Configuration.ConfigurationManager.AppSettings["AvatarFolder_cs"] + "/" + userLogining.Image;
                        Session["UserAvatar_js"] = System.Configuration.ConfigurationManager.AppSettings["AvatarFolder_js"] + "/" + userLogining.Image;
                        Session["AvatarBase64"] = ImgProcession.ImageToBase64(Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["AvatarFolder_cs"] + "\\" + userLogining.Image));
                    }


                    else 
                    {
                        Session["UserAvatar_cs"] = System.Configuration.ConfigurationManager.AppSettings["AvatarFolder_cs"] + "/" + System.Configuration.ConfigurationManager.AppSettings["DefaultAvatar"];
                        Session["AvatarBase64"] = ImgProcession.ImageToBase64(Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["AvatarFolder_cs"] + "\\" + System.Configuration.ConfigurationManager.AppSettings["DefaultAvatar"]));
                        Session["UserAvatar_js"] = System.Configuration.ConfigurationManager.AppSettings["AvatarFolder_js"] + "/" + System.Configuration.ConfigurationManager.AppSettings["DefaultAvatar"];
                    }
                    //Response.Write($"<script>alert('{Session["UserAvatar"]}')</script>");
                    HttpCookie cookie = new HttpCookie($"UserInfo");
                    cookie.Expires = DateTime.Now.AddDays(1);
                    cookie.Values["UserId"] = userLogining.UserId.ToString();
                    cookie.Values["Password"] = userLogining.PassWord.ToString();
                    
                    Response.Cookies.Add(cookie);

                    return RedirectToAction("Index", "Music");
                }
                else Response.Write("<script>alert('电子邮箱或密码有误，请检查！')</script>");
            }
            return View();
        }
        [HttpPost]
        public void EmailAddrAuthentication(string SinUpEmailAddr)
        {
            string code = Email.CreateValidateCode(6);
            Email email = new Email
            {
                host = "smtp.qq.com",
                mailBody = code,
                mailFrom = System.Configuration.ConfigurationManager.AppSettings["EmailAddr"],
                mailPwd = System.Configuration.ConfigurationManager.AppSettings["EmailPwd"],
                mailSubject = "欢迎您加入天籁之音",
                mailToArray = new string[] { SinUpEmailAddr }
            };
            if (email.Send()) Session["code"] = code;

        }
        [HttpPost]
        public ActionResult codeConfirm(string code)
        {
            string CorrectCode = (string)Session["code"];
            if (code != null && code != CorrectCode)
            {

                //return Content(DateTime.Now.ToString() + " code: " + code + " 失败 "+"服务端code是 "+CorrectCode);
                return Content("document.getElementById('codeError').setAttribute(\"style\", \"display: inline - block; \");");
            }
            else
                return Content("document.forms[0].submit();");
            //return Content($"alert('code= {code}  CorrectCode= {CorrectCode}')");
            //return Content(DateTime.Now.ToString() + "code: " + code + " 成功天哪");

        }
        [HttpPost]
        public ActionResult CheckEmail(string email)
        {
            var theUser = db.User_InfoSet.FirstOrDefault(m => m.UserEmail.Equals(email));
            if (theUser != null) return Content("$('#emailOK').fadeOut();$('#emailExist').fadeIn();");
            else return  Content("$('#emailExist').fadeOut();$('#emailOK').fadeIn();");
        }
        public string UserAvatarFileNameToBase64String(string fileName)
        {
            if (fileName != null && fileName != "")
            {
                return ImgProcession.ImageToBase64(Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["AvatarFolder_cs"] + "\\" + fileName));
            }


            else
            {
                return ImgProcession.ImageToBase64(Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["AvatarFolder_cs"] + "\\" + System.Configuration.ConfigurationManager.AppSettings["DefaultAvatar"]));
            }
        }
        public ActionResult UserSearch(string s, int? Type)
        {
            var userId = Convert.ToInt64(CreateSessionWithCookie());
            if (s == null || Type == null) return new HttpStatusCodeResult(500);

            
            switch (Type)
            {
                case 1:
                    {
                        var userList = db.User_InfoSet.Where(m => m.UserName.Contains(s)).ToList();
                        foreach(var log in userList)
                        {
                            log.Image = UserAvatarFileNameToBase64String(log.Image);
                        }
                        ViewBag.type = 1;
                        ViewBag.userId = userId;
                        return PartialView(userList);
                    }
                    //根据UserId找到该User所关注的User，FollowId
                case 2:
                    {
                        var theUser = db.User_InfoSet.Find(Convert.ToInt64(s));
                        List<User_InfoSet> FollowList = new List<User_InfoSet>();
                        //theUser.Image = UserAvatarFileNameToBase64String(theUser.Image);
                        //FollowList.Add(theUser);
                        foreach(var rel in theUser.User_User_rel)
                        {
                            var Idol = db.User_InfoSet.Find(rel.FollowUserId);
                            Idol.Image = UserAvatarFileNameToBase64String(Idol.Image);
                            FollowList.Add(Idol);
                        }
                        ViewBag.type = 2;
                        ViewBag.userId = theUser.UserId;
                        return PartialView(FollowList);
                    }
                //根据FollowId找到该User的粉丝User，UserId
                case 3:
                    {
                        var S = Convert.ToInt64(s);
                        var rels = db.User_User_rel.Where(m => m.FollowUserId == S);
                        List<User_InfoSet> FanList = new List<User_InfoSet>();
                        var theUser = db.User_InfoSet.Find(S);
                        //theUser.Image = UserAvatarFileNameToBase64String(theUser.Image);
                        //FanList.Add(theUser);
                        foreach(var rel in rels)
                        {
                            var fan = rel.User_InfoSet;
                            fan.Image = UserAvatarFileNameToBase64String(fan.Image);
                            FanList.Add(fan);
                        }
                        ViewBag.type = 3;
                        return PartialView(FanList);
                    }
                default:
                    {
                        return new HttpStatusCodeResult(502);
                    }
            }

            
        }
        
        //Ajax异步关注用户
        public ActionResult ToggleFollow(long? s,int? Type)
        {
            var userId = Convert.ToInt64(CreateSessionWithCookie());
            if (s == null) return new HttpStatusCodeResult(500);

            var theUser = db.User_InfoSet.Find(userId);


            if (IsFollowing((long)s, theUser)) { 
                db.User_User_rel.Remove(theUser.User_User_rel.FirstOrDefault(m=>m.FollowUserId==s)) ;
                db.SaveChanges();
            }
            else
            {
                theUser.User_User_rel.Add(new User_User_rel
                {
                    FollowUserId = (long)s
                });
                db.SaveChanges();
            }
            switch (Type)
            {
                case 1:return Content($"粉丝数： {GetFanCountInt(s)}");
                case 2:return Content($"{GetFanCountInt(s)}人关注");
                default:return Content($"粉丝数： {GetFanCountInt(s)}");
            }
            
        }
        
        public string IsFollowing_(long? s)
        {
            var userId = Convert.ToInt64(CreateSessionWithCookie());
            if (s == null) return "false";
            return IsFollowing((long)s, userId)?"true":"false";
        }
        public bool IsFollowing(long s,long u)
        {
            var theUser = db.User_InfoSet.Find(u);
            return IsFollowing(s, theUser);
        }
        public bool IsFollowing(long s,User_InfoSet theUser)
        {
            if ((_ = theUser.User_User_rel.FirstOrDefault(m => m.FollowUserId == s)) == null)
            {
                return false;
            }
            else return true;
        }

        public string CreateSessionWithCookie()
        {
            if (Session["UserId"] == null)
            {
                var cookie = Request.Cookies.Get($"UserInfo");
                if (cookie == null)
                {
                    Response.Write("<script>alert('您还没有登录哦，请您登录哦！！(๑•̀ㅂ•́)و✧====>>(ノ｀Д´)ノへ┻┻');window.location.href='../User/Login'</script>");
                    return null;
                }
                var UserId = Convert.ToInt64(cookie.Values["UserId"]);
                var Pwd = cookie.Values["Password"];
                var user = db.User_InfoSet.Find(UserId);
                if (user == null || user.PassWord != Pwd)
                {
                    Response.Write("<script>alert('您还没有登录哦，请您登录哦！！(๑•̀ㅂ•́)و✧====>>(ノ｀Д´)ノへ┻┻');window.location.href='../User/Login'</script>");
                    return null;
                }
                else
                {
                    Session["UserId"] = user.UserId;
                    Session["UserName"] = user.UserName;
                    
                    if (user.Image != null && user.Image != "")
                    {
                        Session["UserAvatar_cs"] = System.Configuration.ConfigurationManager.AppSettings["AvatarFolder_cs"] + "/" + user.Image;
                        Session["UserAvatar_js"] = System.Configuration.ConfigurationManager.AppSettings["AvatarFolder_js"] + "/" + user.Image;
                        Session["AvatarBase64"] = ImgProcession.ImageToBase64(Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["AvatarFolder_cs"] + "\\" + user.Image));
                    }


                    else
                    {
                        Session["UserAvatar_cs"] = System.Configuration.ConfigurationManager.AppSettings["AvatarFolder_cs"] + "/" + System.Configuration.ConfigurationManager.AppSettings["DefaultAvatar"];
                        Session["AvatarBase64"] = ImgProcession.ImageToBase64(Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["AvatarFolder_cs"] + "\\" + System.Configuration.ConfigurationManager.AppSettings["DefaultAvatar"]));
                        Session["UserAvatar_js"] = System.Configuration.ConfigurationManager.AppSettings["AvatarFolder_js"] + "/" + System.Configuration.ConfigurationManager.AppSettings["DefaultAvatar"];
                    }
                    return Session["UserId"].ToString();
                }
            }
            else return Session["UserId"].ToString();
        }
        public int GetFanCountInt(long? s)
        {
            if (s == null) return 0;
            return db.User_User_rel.Count(m => m.FollowUserId == s);
        }
        public string GetFanCountString(long? s)
        {
            if (s == null) return "0";
            var total = GetFanCountInt(s);
             
                if (total < 10000)
                {
                return total.ToString();
            }
            else
            {
                var w = total / 10000; var k = (total - w * 10000) / 1000;
                return $"{w}.{k}万";
            }


        }


        public ActionResult Main(long? s,int? Type)
        {
            var userId = Convert.ToInt64(CreateSessionWithCookie());           

            switch (Type)
            {
                //显示别人
                case 1:
                    {
                        if (s == null) return new HttpStatusCodeResult(501);
                        var user = db.User_InfoSet.Find(s);
                        user.Image = UserAvatarFileNameToBase64String(user.Image);
                        ViewBag.type = 1;
                        return View(user);
                    }
                //显示自己
                case 2:
                    {
                        var user = db.User_InfoSet.Find(userId);
                        user.Image = UserAvatarFileNameToBase64String(user.Image);
                        ViewBag.type = 2;
                        return View(user);
                    }
                default:return new HttpStatusCodeResult(503);
            }          
        }

    }
}   
    



