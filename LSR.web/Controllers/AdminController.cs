using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using LSR.Common;
using LSR.Models;

namespace LSR.Controllers
{
    public class AdminController : Controller
    {
        private LSREntities db = new LSREntities();

        // GET: Admin
        public ActionResult Index()
        {
            return View(db.User_InfoSet.ToList());
        }

        // GET: Admin/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User_InfoSet user_InfoSet = db.User_InfoSet.Find(id);
            ViewBag.Birthday = ((DateTime)user_InfoSet.Birthday).Date;
            if (user_InfoSet == null)
            {
                return HttpNotFound();
            }
            return View(user_InfoSet);
        }

        // POST: Admin/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性；有关
        // 更多详细信息，请参阅 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "UserId,UserName,PassWord,Gender,UserEmail,Phone,Birthday")] User_InfoSet user_InfoSet)
        {
            if (ModelState.IsValid)
            {
                db.Entry(user_InfoSet).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(user_InfoSet);
        }

        // GET: Admin/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User_InfoSet user_InfoSet = db.User_InfoSet.Find(id);
            if (user_InfoSet == null)
            {
                return HttpNotFound();
            }
            return View(user_InfoSet);
        }

        // POST: Admin/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            User_InfoSet user_InfoSet = db.User_InfoSet.Find(id);
            db.User_InfoSet.Remove(user_InfoSet);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        public ActionResult AdminLogin()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AdminLogin(AdminLogin AdminLogin)
        {
            if (ModelState.IsValid)
            {
                var userLogining = db.AdminSet.FirstOrDefault(model => (model.Name.Equals(AdminLogin.Name)/*||model.UserEmail.Equals(login.UserEmail)*/) && model.Pwd.Equals(AdminLogin.Pwd));
                if (userLogining != null)
                {
                    Session["UserId"] = userLogining.AdminId;
                    return RedirectToAction("Index", "Admin");
                }
                else Response.Write("<script>alert('用户名或密码有误，请检查！')</script>");
            }
            return View();
        }
        
        public ActionResult UserMusic(long? UserId)
        {
            var userMusic = db.MusicSet.Where(m => m.UserId == UserId).ToList();
            return View(userMusic);
        }

        private ActionResult View(Func<ActionResult> userMusic)
        {
            throw new NotImplementedException();
        }
    }
}
