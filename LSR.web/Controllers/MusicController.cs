using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using LSR.Common;
using LSR.Models;

namespace LSR.Controllers
{
    public class MusicController : Controller
    {
        LSREntities db = new LSREntities();

        // GET: Music
        public ActionResult Index(bool? Logout)
        {
            
            if (Logout == true) { Session.RemoveAll(); }
            return View();
        }

        // GET: Music/Create
        public ActionResult Create()
        {
            if (Session["UserId"] == null)
            {
                Response.Write("<script>alert('您还没有登录哦，请您登录哦！！(๑•̀ㅂ•́)و✧====>>(ノ｀Д´)ノへ┻┻');window.location.href='../User/Login'</script>");
                return Content("登录状态已过期");

            }
            return View();
        }

        // POST: Music/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性；有关
        // 更多详细信息，请参阅 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MusicId,MusicName,Author,AlbumName,MusicDuration,UploadDate,UploaderName,FileSize,MusicFormat")] MusicSet music)
        {
            if (Session["UserId"] == null)
            {
                Response.Write("<script>alert('您还没有登录哦，请您登录哦！！(๑•̀ㅂ•́)و✧====>>(ノ｀Д´)ノへ┻┻');window.location.href='../User/Login'</script>");
                return Content("登录状态已过期");

            }
            if (ModelState.IsValid)
            {
                db.MusicSet.Add(music);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(music);
        }

        // GET: Music/Edit/5
        public ActionResult Edit(long? id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MusicSet music = db.MusicSet.Find(id);
            if (music == null)
            {
                return HttpNotFound();
            }
            return View(music);
        }

        // POST: Music/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性；有关
        // 更多详细信息，请参阅 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MusicId,MusicName,Author,AlbumName,MusicDuration,UploadDate,User,FileSize,MusicFormat")] MusicSet music)
        {
            if (ModelState.IsValid)
            {
                db.Entry(music).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(music);
        }

        // GET: Music/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MusicSet music = db.MusicSet.Find(id);
            if (music == null)
            {
                return HttpNotFound();
            }
            return View(music);
        }

        // POST: Music/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            MusicSet music = db.MusicSet.Find(id);
            db.MusicSet.Remove(music);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        //public ActionResult Play()
        //{
        //    return View();
        //}
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Play()
        {
            if (Session["UserId"] == null)
            {
                Response.Write("<script>alert('走开太久咯，请您新登录哦！！(๑•̀ㅂ•́)و✧====>>(ノ｀Д´)ノへ┻┻');window.location.href='../../User/Login'</script>");
                return Content("登录状态已过期");
            }
            //if(Request.Params["MusicId"]==null) return RedirectToAction("Search", "Music", new { s = "李宇春" });
            var musicId = Convert.ToInt64(Request["MusicId"]);
            
            MusicSet music = db.MusicSet.Find(musicId);
            //ViewBag.MusicName =music.MusicName;
            //ViewBag.Author = music.ArtistSet.Name;
            //ViewBag.Album = music.AlbumSet.Name;
            //ViewBag.ImagePath = System.Configuration.ConfigurationManager.AppSettings["_MusicImagesFolder"] +"/"+music.MusicId+/*music.MusicFormat*/".jpg";
            //ViewBag.LyricsPath = System.Configuration.ConfigurationManager.AppSettings["_MusicLyricsFolder"] + "/" + music.MusicId + ".lrc";
            //ViewBag.MusicPath = System.Configuration.ConfigurationManager.AppSettings["_MusicFileFolder"] + "/" + music.MusicId + ".mp3";
            if (music != null) return View(music);
            else return Content("发生错误了");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public ActionResult Library()
        {
            if (Session["UserId"] == null)
            {
                Response.Write("<script>alert('走开太久咯，请您新登录哦！！(๑•̀ㅂ•́)و✧====>>(ノ｀Д´)ノへ┻┻');window.location.href='../User/Login'</script>");
                return Content("登录状态已过期");

            }
            var searchStr = Request.QueryString["searchBar"];
            ViewBag.searched = searchStr;
            if (searchStr != null)
            {
                var musicList = db.MusicSet.Where(m => m.MusicName.Contains(searchStr)||m.ArtistSet.Name.Contains(searchStr));
                
                return View(musicList.ToList());
            }
            else
            return View(db.MusicSet.ToList());
        }
        [HttpPost]
        public void Upload()
        {
            
            string musicName = Request["MusicName"];
            string ArtistName = Request["Author"];
            string AlbumName = Request["AlbumName"];
            string Duration = Request["Duration"];

            var mfile = Request.Files["Music"];
            var lfile= Request.Files["Lyrics"];
            var ifile = Request.Files["Image"];

            var newSong = new MusicSet
            {
                MusicName = musicName,
                UserId = Session["UserId"] == null ? (long?)Session["UserId"] : (long?)2,
                FileSize = mfile.ContentLength
            };
            try
            {
                if (Duration != null)
                {
                    int total = Convert.ToInt32(Duration);
                    var h = (total % 3600) - 1;
                    var m = ((total - 3600 * h) % 60) - 1;
                    var s = total - 3600 * h - 60 * m;
                    newSong.MusicDuration = new TimeSpan(h, m, s);
                }

                if (ArtistName != null)
                {
                    var Artist = db.ArtistSet.FirstOrDefault(a => a.Name == ArtistName);
                    if (Artist == null)
                    {
                        Artist = new ArtistSet
                        {
                            Name = ArtistName,
                        };
                        db.ArtistSet.Add(Artist);
                        db.SaveChanges();
                        newSong.ArtistId = db.ArtistSet.Max(m => m.ArtistId);
                    }
                    else newSong.ArtistId = Artist.ArtistId;
                }
                if (AlbumName != null)
                {
                    var Album = db.AlbumSet.FirstOrDefault(a => a.Name == AlbumName);
                    if (Album == null)
                    {
                        Album = new AlbumSet
                        {
                            Name = AlbumName,
                            ArtistId = newSong.ArtistId,
                            PublishDate = DateTime.Now,

                        };
                        db.AlbumSet.Add(Album);
                        db.SaveChanges();
                        newSong.AlbumId = db.AlbumSet.Max(m => m.AlbumId);
                    }
                    else newSong.AlbumId = Album.AlbumId;
                }
                if (mfile != null)
                {
                    newSong.MusicFile = DateTime.Now.Ticks + ".mp3";
                    string musicFilePath = Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["MusicFileFolder"]) + "\\" + newSong.MusicFile;
                    mfile.SaveAs(musicFilePath);
                }
                if (lfile != null)
                {
                    newSong.MusicLyrics = DateTime.Now.Ticks + ".lrc";
                    string lyricsFilePath = Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["MusicLyricsFolder"]) + "\\" + newSong.MusicLyrics;
                    lfile.SaveAs(lyricsFilePath);
                }

                if (ifile != null)
                {
                    newSong.MusicImage = DateTime.Now.Ticks + ".jpg";
                    string imageFilePath = Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["MusicImagesFolder"]) + "\\" + newSong.MusicImage;
                    ifile.SaveAs(imageFilePath);
                }
                db.MusicSet.Add(newSong);
                db.SaveChanges();
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
            }
            }
            
            
        public ActionResult Search()
        {
            if (Session["UserId"] == null)
            {
                Response.Write("<script>alert('走开太久咯，请您新登录哦！！(๑•̀ㅂ•́)و✧====>>(ノ｀Д´)ノへ┻┻');window.location.href='../User/Login'</script>");
                return Content("登录状态已过期");

            }
            object searchStr = Request["s"];
            return View(searchStr);
        }
        public PartialViewResult MusicSearch()
        {
            if (Session["UserId"] == null)
            {
                Response.Write("<script>alert('走开太久咯，请您新登录哦！！(๑•̀ㅂ•́)و✧====>>(ノ｀Д´)ノへ┻┻');window.location.href='../User/Login'</script>");

            }
            ViewBag.UserId = Session["UserId"];
            string QueryString = Request["s"];
            if (QueryString == null || QueryString == "") return PartialView(db.MusicSet.ToList());
            var MusicList = db.MusicSet.Where(m=>m.MusicName.Contains(QueryString)||m.ArtistSet.Name.Contains(QueryString)).ToList();
            return PartialView(/*"MusicSearch",*/MusicList);
        }
        public PartialViewResult AlbumSearch()
        {
            if (Session["UserId"] == null)
            {
                Response.Write("<script>alert('走开太久咯，请您新登录哦！！(๑•̀ㅂ•́)و✧====>>(ノ｀Д´)ノへ┻┻');window.location.href='../User/Login'</script>");

            }
            string QueryString = Request["s"];
            if (QueryString == null || QueryString == "") return PartialView(db.AlbumSet.ToList());
            var AlbumList = db.AlbumSet.Where(m=>m.Name.Contains(QueryString)||m.ArtistSet.Name.Contains(QueryString)).ToList();
            return PartialView(/*"MusicSearch",*/ AlbumList);
        }
        public PartialViewResult LyricsSearch()
        {
            if (Session["UserId"] == null)
            {
                Response.Write("<script>alert('走开太久咯，请您新登录哦！！(๑•̀ㅂ•́)و✧====>>(ノ｀Д´)ノへ┻┻');window.location.href='../User/Login'</script>");

            }
            string QueryString = Request["s"];
            var MusicList = db.MusicSet.Where(m => m.MusicName.Contains(QueryString) || m.ArtistSet.Name.Contains(QueryString));
            MusicList = MusicList.Where(m => m.MusicLyrics != null);
            List<List<string>> LyricsTextList=new List<List<string>>();
            foreach(var item in MusicList)
            {
                List<string> LyricsText = new List<string>();
                LyricsText.Add(item.MusicId.ToString());
                LyricsText.Add(item.MusicName);
                LyricsText.Add(item.ArtistSet.Name);
                var LyricsPath = Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["MusicLyricsFolder"])+"\\"+item.MusicLyrics;
                StreamReader LyricsSR = new StreamReader(LyricsPath, Encoding.UTF8);
                var Lyrics = LyricsSR.ReadToEnd();
                var LyricsLine = Lyrics.Split('\n');
                foreach(var per in LyricsLine)
                {
                    //var perLine = Regex.Replace(per, @"/.*\[(\d{ 2}):(\d{ 2})(\.(\d{ 2,3}))?]/ g", "");
                    var perLine = Regex.Replace(per, @"[[]{1}(.+)]", "");
                    //perLine = Regex.Replace(perLine, @"/< (\d{ 2}):(\d{ 2})(\.(\d{ 2,3}))?>/ g", "");
                    //perLine = Regex.Replace(perLine, @"/ ^\s +|\s +$/ g", "");
                    LyricsText.Add(perLine);
                }
                LyricsSR.Close();
                LyricsTextList.Add(LyricsText);
            }
            return PartialView(/*"MusicSearch",*/ LyricsTextList);
        }
        public ActionResult AddLike()
        {
            if (Session["UserId"] == null)
            {
                Response.Write("<script>alert('走开太久咯，请您新登录哦！！(๑•̀ㅂ•́)و✧====>>(ノ｀Д´)ノへ┻┻');window.location.href='../User/Login'</script>");
                return Content("登录状态已过期");

            }

            var userId = (long)Session["UserId"];
            long musicId;
            if (Request["MusicId"] != null) musicId = Convert.ToInt64(Request["MusicId"]);
            else throw new InvalidOperationException("哎呀！发生错误了！(ノ｀Д´)ノへ┻┻");

            var LikeRel = db.User_InfoSet.FirstOrDefault(m => m.UserId == userId).User_Music_Like_rel.FirstOrDefault(m => m.MusicId == musicId);
            if(LikeRel==null)
            {
                db.User_Music_Like_rel.Add(new User_Music_Like_rel { UserId = userId, MusicId = musicId });
                db.SaveChanges();
            }
            else
            {
                db.User_Music_Like_rel.Remove(LikeRel);
                db.SaveChanges();
            }
            return Content("操作成功");
        }
        public FileResult Download(FormCollection form)

        {
            if (Session["UserId"] == null)
            {
                Response.Write("<script>alert('走开太久咯，请您新登录哦！！(๑•̀ㅂ•́)و✧====>>(ノ｀Д´)ノへ┻┻');window.location.href='../User/Login'</script>");
            }
            //if (form["MuiscId"] == null) return null;
            long QueryString = Convert.ToInt64(form["MusicId"]);
            var music = db.MusicSet.FirstOrDefault(m => m.MusicId == QueryString);            
            string MusicFile = System.Configuration.ConfigurationManager.AppSettings["MusicFileFolder"] + "/" + music.MusicFile;
            return File(Server.MapPath(MusicFile), "audio/mpeg3", music.MusicName + ".mp3");

        }
        public PartialViewResult DisplaySheet ()
        {
            if (Session["UserId"] == null)
            {
                Response.Write("<script>alert('走开太久咯，请您新登录哦！！(๑•̀ㅂ•́)و✧====>>(ノ｀Д´)ノへ┻┻');window.location.href='../User/Login'</script>");
                return null;
            }
            var userId = (long)Session["UserId"];

            ViewBag.theMusicId = Request["MusicId"];
            var MusicSheets = db.User_InfoSet.Find(userId).MusicSheetSet;
            return PartialView(MusicSheets.ToList());

        }
        public ActionResult CreateSheet()
        {
            if (Session["UserId"] == null)
            {
                Response.Write("<script>alert('走开太久咯，请您新登录哦！！(๑•̀ㅂ•́)و✧====>>(ノ｀Д´)ノへ┻┻');window.location.href='../User/Login'</script>");
                return Content("请先登录");
            }
            var userId = (long)Session["UserId"];

            var SheetName = Request["SheetName"];var user = db.User_InfoSet.Find(userId);
            var TheSheet = user.MusicSheetSet.FirstOrDefault(m=>m.Name==SheetName);
            if (TheSheet != null) return new HttpStatusCodeResult(500);
            else
            {
                user.MusicSheetSet.Add(new MusicSheetSet
                {
                    Name = SheetName,
                    //UserId = user.UserId,
                    SetDate = DateTime.Now
                });
                try
                {
                    db.SaveChanges();
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
                    return new HttpStatusCodeResult(500);
                }
            }
            return Content("创建成功");
        }
        public ActionResult AddToSheet()
        {
            if (Session["UserId"] == null)
            {
                Response.Write("<script>alert('走开太久咯，请您新登录哦！！(๑•̀ㅂ•́)و✧====>>(ノ｀Д´)ノへ┻┻');window.location.href='../User/Login'</script>");
                return Content("请先登录");
            }
            long musicId, sheetId, userId;
            try 
            {
                 musicId = Convert.ToInt64(Request.Params["musicId"]);
                 sheetId = Convert.ToInt64(Request.Params["sheetId"]);
                 userId = Convert.ToInt64(Session["UserId"]);
                if (musicId * sheetId * userId == 0) throw new Exception() ;
            }
            catch (Exception)
            {
                return new HttpStatusCodeResult(505);
            }

            var theSheet = db.MusicSheetSet.Find(sheetId);
            var theSong = theSheet.Music_MusicSheet_rel.FirstOrDefault(m => m.MusicId == musicId);
            if(theSong!=null) return new HttpStatusCodeResult(502);
            else
            {
                theSheet.Music_MusicSheet_rel.Add(new Music_MusicSheet_rel
                {
                    MusicId = musicId
                });
                db.SaveChanges();
            }
            return Content("添加成功");
        }
        public ActionResult AddToPlayList()
        {
            if (Session["UserId"] == null)
            {
                Response.Write("<script>alert('走开太久咯，请您新登录哦！！(๑•̀ㅂ•́)و✧====>>(ノ｀Д´)ノへ┻┻');window.location.href='../User/Login'</script>");
                return Content("请先登录");
            }
            var musicId = Request.Params["MusicId"];
            var userId = Session["UserId"].ToString();
            if (musicId ==null || userId == null || musicId == "" || userId == "") return new HttpStatusCodeResult(504);
            var PlayListCookie = Response.Cookies.Get("PlayListCookie_u" + userId);
            if (PlayListCookie == null)
            {
                PlayListCookie = new HttpCookie("PlayListCookie_u"+userId);
                PlayListCookie.Values["UserId"] = userId;
            }
            var PlayList = PlayListCookie.Values["PlayList"];
            if (PlayList == null || PlayList == "")
            {
                PlayList = musicId;
            }
            else if (PlayList.Contains(',' + musicId)) PlayList = PlayList.Replace(',' + musicId, "") + ',' + musicId;
            else PlayList = PlayList.Replace(musicId + ',', "") + ',' + musicId;
            PlayListCookie.Values["PlayList"] = PlayList;
            Response.Cookies.Add(PlayListCookie);
            return Content("添加成功" + PlayListCookie.Values["PlayList"]);
        }
        public PartialViewResult ArtistOrAlbum()
        {
            var QueryString = Request["s"];
            if (QueryString == null || QueryString == "") return null;
            ArtistSet artist;AlbumSet album;
            var searchRes = (artist = db.ArtistSet.FirstOrDefault(m => m.Name == QueryString)) != null ?
                artist : (album = db.AlbumSet.FirstOrDefault(m => m.Name == QueryString)) != null ?
                album : "";
            
            if((artist = db.ArtistSet.FirstOrDefault(m => m.Name == QueryString)) != null) 
            {
                if(artist.Image==null|| artist.Image == "")
                {
                    foreach(var music in artist.MusicSet) { if (music.MusicImage != null) { artist.Image =  ImgProcession.ImageToBase64(Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["MusicImagesFolder"] + "\\" + music.MusicImage));break; }  }
                    if (artist.Image == null || artist.Image == "") artist.Image =  ImgProcession.ImageToBase64(Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["AvatarFolder_cs"] + "\\" + System.Configuration.ConfigurationManager.AppSettings["DefaultAvatar"]));
                    return PartialView("ArtistCard",artist);
                }
                else artist.Image= ImgProcession.ImageToBase64(Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["MusicImagesFolder"] + "\\" + artist.Image));
            }
            else if((album = db.AlbumSet.FirstOrDefault(m => m.Name == QueryString)) != null)
            {
                if (artist.Image == null || artist.Image == "")
                {
                    foreach (var music in artist.MusicSet) { if (music.MusicImage != null) { artist.Image = ImgProcession.ImageToBase64(Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["AvatarFolder_cs"] + "\\" + music.MusicImage)); break; } }
                    if (artist.Image == null || artist.Image == "") artist.Image = ImgProcession.ImageToBase64(Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["AvatarFolder_cs"] + "\\" + System.Configuration.ConfigurationManager.AppSettings["DefaultAvatar"])); ;
                }
            }
            
        }
    }
}
