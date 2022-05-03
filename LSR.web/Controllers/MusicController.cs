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
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using LSR.Common;
using LSR.Models;

namespace LSR.Controllers
{
    public class MusicController : Controller
    {
        private LSREntities db = new LSREntities();

        public string CreateSessionWithCookie()
        {
            string userId = CreateSessionWithCookie_NonRedirect();
            if (string.IsNullOrEmpty(userId))
            {
                Response.Write("<script>alert('您还没有登录哦，请您登录哦！！(๑•̀ㅂ•́)و✧====>>(ノ｀Д´)ノへ┻┻');window.location.href='/LSR/User/Login'</script>");
                return null;

            }            
            else return userId;
        }
        public string CreateSessionWithCookie_NonRedirect()
        {
            if (Session["UserId"] == null)
            {
                var cookie = Request.Cookies.Get($"UserInfo");
                if (cookie == null)
                {
                    return null;
                }
                var UserId = cookie.Values["UserId"];
                var Pwd = cookie.Values["Password"];
                var user = db.User_InfoSet.Find(Convert.ToInt64(UserId));
                if (user == null || user.PassWord != Pwd)
                {
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

        // GET: Music
        public ActionResult Index(bool? Logout)
        {
            if (Logout == true)
            {

                Session.RemoveAll();
                Response.Cookies.Remove("UserInfo");
                return View();
            }
            else
            {
                _ = CreateSessionWithCookie_NonRedirect();
                return View();
            }                   
        }

        // GET: Music/Create
        public ActionResult Create()
        {
            CreateSessionWithCookie();
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
            CreateSessionWithCookie();
            if (Session["UserId"] == null)
            {
                Response.Write("<script>alert('走开太久咯，请您新登录哦！！(๑•̀ㅂ•́)و✧====>>(ノ｀Д´)ノへ┻┻');window.location.href='../../User/Login'</script>");
                return Content("登录状态已过期");
            }
            //if(Request.Params["MusicId"]==null) return RedirectToAction("Search", "Music", new { s = "李宇春" });
            var userId = Session["UserId"].ToString();
            var musicIdOn = Request["MusicIdOn"];
            var musicIdOff = Request["MusicIdOff"];
            if (musicIdOn != null && musicIdOn != "") {
                var PlayListCookie = Request.Cookies.Get("PlayListCookie-u" + userId);
                if (PlayListCookie == null)
                {
                    PlayListCookie = new HttpCookie("PlayListCookie-u" + userId);


                }
                var PlayList = PlayListCookie.Values["PlayList"].ToString();

                //播放列表是一个字符串类似于1,2,3,4,5,6
                //播放列表中没有内容
                if (PlayList == null || PlayList == "") PlayList = musicIdOn;
                //这首歌存在于播放列表，而且这首歌在播放列表的中间或尾巴
                else if (PlayList.Contains("," + musicIdOn)) PlayList = PlayList.Replace("," + musicIdOn, "") + "," + musicIdOn;
                //这首歌还没有存在于播放列表或者这首歌在播放列表的头头上
                else PlayList = PlayList.Replace(musicIdOn + ",", "") + "," + musicIdOn;

                PlayListCookie.Values["PlayList"] = PlayList;
                PlayListCookie.Values["UserId"] = userId;
                PlayListCookie.Expires = DateTime.Now.AddMonths(1);
                Response.Cookies.Add(PlayListCookie);

                return null;
            }
            else if (musicIdOff != null && musicIdOff != "")
            {
                var PlayListCookie = Request.Cookies.Get("PlayListCookie-u" + userId);
                if (PlayListCookie == null)
                {
                    PlayListCookie = new HttpCookie("PlayListCookie-u" + userId);


                }
                var PlayList = PlayListCookie.Values["PlayList"].ToString();

                //播放列表是一个字符串类似于1,2,3,4,5,6
                //播放列表中没有内容
                if (PlayList == null || PlayList == "") PlayList = musicIdOff;
                //这首歌存在于播放列表，而且这首歌在播放列表的中间或尾巴
                else if (PlayList.Contains("," + musicIdOff)) PlayList = PlayList.Replace("," + musicIdOff, "") + "," + musicIdOff;
                //这首歌还没有存在于播放列表或者这首歌在播放列表的头头上
                else PlayList = PlayList.Replace(musicIdOff + ",", "") + "," + musicIdOff;

                PlayListCookie.Values["PlayList"] = PlayList;
                PlayListCookie.Values["UserId"] = userId;
                PlayListCookie.Expires = DateTime.Now.AddMonths(1);
                Response.Cookies.Add(PlayListCookie);

                var PlayListMusicNames = PlayList.Split(',');
                List<MusicSet> MusicList = new List<MusicSet>();
                foreach (var item in PlayListMusicNames)
                {
                    var music = db.MusicSet.Find(Convert.ToInt64(item));
                    MusicList.Add(music);
                }
                return View(MusicList);
            }

            //ViewBag.MusicName =music.MusicName;
            //ViewBag.Author = music.ArtistSet.Name;
            //ViewBag.Album = music.AlbumSet.Name;
            //ViewBag.ImagePath = System.Configuration.ConfigurationManager.AppSettings["_MusicImagesFolder"] +"/"+music.MusicId+/*music.MusicFormat*/".jpg";
            //ViewBag.LyricsPath = System.Configuration.ConfigurationManager.AppSettings["_MusicLyricsFolder"] + "/" + music.MusicId + ".lrc";
            //ViewBag.MusicPath = System.Configuration.ConfigurationManager.AppSettings["_MusicFileFolder"] + "/" + music.MusicId + ".mp3";
            else
            {
                var PlayListCookie = Request.Cookies.Get("PlayListCookie-u" + userId);
                if (PlayListCookie == null)
                {
                    PlayListCookie = new HttpCookie("PlayListCookie-u" + userId);


                }
                var PlayList = PlayListCookie.Values["PlayList"].ToString();


            }
            return new HttpStatusCodeResult(501);
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
            var userId = Convert.ToInt64(CreateSessionWithCookie());
            return View();
        }
        public ActionResult SearchLibrary(int? Type)
        {
            switch (Type)
            {
                case 1:
                    {
                        ViewBag.type = 1;
                        return PartialView("SearchLibrary");
                    }
                default: return new HttpStatusCodeResult(500);
            }
        }
        [HttpPost]
        public void Upload()
        {
            var userId = Convert.ToInt64(CreateSessionWithCookie());
            string musicName = Request.Form["MusicName"];
            string ArtistName = Request.Form["Author"];
            string AlbumName = Request.Form["AlbumName"];
            string Duration = Request.Form["Duration"];
            string LyricsString= Request.Form["LyricsString"];
            string ImageBase64String= Request.Form["ImageBase64String"];

            var mfile = Request.Files["Music"];
            
            var lfile = Request.Files["Lyrics"];
            var ifile = Request.Files["Image"];

            var newSong = new MusicSet
            {
                MusicName = musicName,
                UserId = userId,
                FileSize = mfile.ContentLength,
                UploadDate = DateTime.Now.Date
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
                        var initial = stringParse.getSpell(ArtistName);
                        Artist = new ArtistSet
                        {
                            Name = ArtistName,
                            Initial=(initial<=122&&initial>=97?initial:'#').ToString()
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
                if(!string.IsNullOrEmpty(LyricsString))
                {
                    newSong.MusicLyrics = DateTime.Now.Ticks + ".lrc";
                    string lyricsFilePath = Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["MusicLyricsFolder"]) + "\\" + newSong.MusicLyrics;
                    StreamWriter file = new StreamWriter(lyricsFilePath);
                    file.Write(LyricsString);
                    file.Close();
                }
                else if (lfile != null)
                {
                    newSong.MusicLyrics = DateTime.Now.Ticks + ".lrc";
                    string lyricsFilePath = Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["MusicLyricsFolder"]) + "\\" + newSong.MusicLyrics;
                    lfile.SaveAs(lyricsFilePath);
                }

                if (!string.IsNullOrEmpty(ImageBase64String))
                {
                    newSong.MusicImage = DateTime.Now.Ticks + ".jpg";
                    string imageFilePath = Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["MusicImagesFolder"]) + "\\" + newSong.MusicImage;
                    var mImage = ImgProcession.Base64ToImage(ImageBase64String);
                    Bitmap bp = new Bitmap(mImage);
                    bp.Save(imageFilePath, System.Drawing.Imaging.ImageFormat.Jpeg);
                }
                else if (ifile != null)
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


        public ActionResult Search(string s,int? Type)
        {
            CreateSessionWithCookie();
            if (Type == null) Type = 1;
            switch (Type)
            {
                case 1:
                    {
                        ViewBag.type = 1;
                        return View((object)s);
                    }
                case 2:
                    {
                        ViewBag.type = 2; 
                        return PartialView((object)s);
                    }
                case 3:
                    {
                        ViewBag.type = 3;
                        return PartialView((object)s);
                    }
                default:return View((object)s);

            }
            
        }
        
        public ActionResult MusicSearch(string s,int? Type)
        {

            ViewBag.UserId = CreateSessionWithCookie() ;

            if (Type==0) return new HttpStatusCodeResult(500);
            //if (type == 0) return null;

            switch (Type)
            {
                //type=1,按歌名、歌手名、专辑名来搜索
                case 1:
                    {
                        if (s == null || s == "") return PartialView(db.MusicSet.ToList());
                        var MusicList = db.MusicSet.Where(m => m.MusicName.Contains(s) || m.ArtistSet.Name.Contains(s) || m.AlbumSet.Name.Contains(s));
                        return PartialView(/*"MusicSearch",*/MusicList);
                    }
                //type=2,按歌手id搜索
                case 2:
                    {
                        if (s == null || s == "") return new HttpStatusCodeResult(501);
                        var qsLong= Convert.ToInt64(s);
                        var MusicList = db.MusicSet.Where(m => m.ArtistId == qsLong).ToList();
                        return PartialView(MusicList);
                    }
                //type=3,按专辑id搜索
                case 3:
                    {
                        if (s == null || s == "") return new HttpStatusCodeResult(502);
                        var qsLong = Convert.ToInt64(s);
                        var MusicList = db.MusicSet.Where(m => m.AlbumId == qsLong).ToList();
                        return PartialView(MusicList);
                    }
                case 4:
                    {
                        if (s == null || s == "") return new HttpStatusCodeResult(502);
                        var qsLong = Convert.ToInt64(s);
                        var rels = db.MusicSheetSet.FirstOrDefault(m => m.MusicSheetId == qsLong).Music_MusicSheet_rel;
                        List<MusicSet> MusicList=new List<MusicSet>();
                        foreach(var log in rels)
                        {
                            MusicList.Add(log.MusicSet);
                        }
                        return PartialView(MusicList);
                    }
                //按用户收藏
                case 5:
                    {
                        if (s == null || s == "") return new HttpStatusCodeResult(505);
                        var qsLong = Convert.ToInt64(s);
                        List<MusicSet> MusicList = new List<MusicSet>();
                        foreach (var rel in db.User_InfoSet.Find(qsLong).User_Music_Like_rel)
                        {
                            MusicList.Add(rel.MusicSet);
                        }
                        return PartialView(MusicList);
                    }
                //按用户上传
                case 6:
                    {
                        if (s == null || s == "") return new HttpStatusCodeResult(505);
                        var qsLong = Convert.ToInt64(s);
                        var MusicList = db.User_InfoSet.Find(qsLong).MusicSet.ToList();
                        return PartialView(MusicList);
                    }
                default:
                    {
                        return new HttpStatusCodeResult(503);
                    }
            }                     
        }
        public ActionResult AlbumSearch(int? Type,string s)
        {
            CreateSessionWithCookie();         
            if (s == null || s == "") return PartialView(db.AlbumSet.ToList());
            switch (Type)
            {
                //type=1,按专辑名，歌手名来搜索
                case 1:
                    {
                        if (s == null || s == "") return PartialView(db.AlbumSet.ToList());
                        var AlbumList = db.AlbumSet.Where(m => m.Name.Contains(s) || m.ArtistSet.Name.Contains(s));
                        
                        foreach(var log in AlbumList)
                        {
                            if (log.Image == null || log.Image == "")
                            {
                                foreach (var music in log.MusicSet)
                                {
                                    string path;
                                    if (music.MusicImage != null)
                                    {
                                        path = Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["MusicImagesFolder"] + "\\" + music.MusicImage);

                                        try
                                        {
                                            log.Image = ImgProcession.ImageToBase64(path);break;
                                        }
                                        catch (Exception) { return Content($"image: {path}"); }                                        
                                    }
                                
                                }
                                if (log.Image == null || log.Image == "") log.Image = ImgProcession.ImageToBase64(Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["AvatarFolder_cs"] + "\\" + System.Configuration.ConfigurationManager.AppSettings["DefaultAvatar"]));
                            }
                            else log.Image = ImgProcession.ImageToBase64(Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["MusicImagesFolder"] + "\\" + log.Image));

                        }
                        return PartialView(/*"MusicSearch",*/AlbumList);
                    }
                //type=2,按歌手id搜索
                case 2:
                    {
                        if (s == null || s == "") return new HttpStatusCodeResult(501);
                        var qsLong = Convert.ToInt64(s);
                        var AlbumList = db.AlbumSet.Where(m => m.ArtistId == qsLong).ToList();
                        foreach (var log in AlbumList)
                        {
                            if (log.Image == null || log.Image == "")
                            {
                                foreach (var music in log.MusicSet) { if (music.MusicImage != null) { log.Image = ImgProcession.ImageToBase64(Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["MusicImagesFolder"] + "\\" + music.MusicImage)); break; } }
                                if (log.Image == null || log.Image == "") log.Image = ImgProcession.ImageToBase64(Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["AvatarFolder_cs"] + "\\" + System.Configuration.ConfigurationManager.AppSettings["DefaultAvatar"]));
                            }
                            else log.Image = ImgProcession.ImageToBase64(Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["MusicImagesFolder"] + "\\" + log.Image));

                        }
                        return PartialView(AlbumList);
                    }
                    //
                case 3:
                    {
                        return new HttpStatusCodeResult(502);
                    }
                default:
                    {
                        return new HttpStatusCodeResult(502);
                    }
            }
        }
        public PartialViewResult LyricsSearch(string s)
        {
            CreateSessionWithCookie();
            var MusicList = db.MusicSet.Where(m => m.MusicName.Contains(s) || m.ArtistSet.Name.Contains(s) || m.AlbumSet.Name.Contains(s));
            MusicList = MusicList.Where(m => m.MusicLyrics != null);
            List<List<string>> LyricsTextList = new List<List<string>>();
            foreach (var item in MusicList)
            {
                List<string> LyricsText = new List<string>();
                LyricsText.Add(item.MusicId.ToString());
                LyricsText.Add(item.MusicName);
                LyricsText.Add(item.ArtistSet.Name);
                var LyricsPath = Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["MusicLyricsFolder"]) + "\\" + item.MusicLyrics;
                StreamReader LyricsSR = new StreamReader(LyricsPath, Encoding.UTF8);
                var Lyrics = LyricsSR.ReadToEnd();
                var LyricsLine = Lyrics.Split('\n');
                foreach (var per in LyricsLine)
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
        public ActionResult ShowLyrics(long? s)
        {
            var item = db.MusicSet.Find(s);
            if (item == null) return new HttpStatusCodeResult(504);
            List<string> LyricsText = new List<string>();
            
            var LyricsPath = Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["MusicLyricsFolder"]) + "\\" + item.MusicLyrics;
            StreamReader LyricsSR = new StreamReader(LyricsPath, Encoding.UTF8);
            var Lyrics = LyricsSR.ReadToEnd();
            var LyricsLine = Lyrics.Split('\n');
            foreach (var per in LyricsLine)
            {
                //var perLine = Regex.Replace(per, @"/.*\[(\d{ 2}):(\d{ 2})(\.(\d{ 2,3}))?]/ g", "");
                var perLine = Regex.Replace(per, @"[[]{1}(.+)]", "");
                //perLine = Regex.Replace(perLine, @"/< (\d{ 2}):(\d{ 2})(\.(\d{ 2,3}))?>/ g", "");
                //perLine = Regex.Replace(perLine, @"/ ^\s +|\s +$/ g", "");
                LyricsText.Add(perLine);
            }
            LyricsSR.Close();
           
            return PartialView("ShowLyrics",LyricsText);
        }
        public ActionResult AddLike(long? s, int? Type)
        {          
            var userId = Convert.ToInt64(CreateSessionWithCookie());

            if(s==null||Type==null) return new HttpStatusCodeResult(503);

            switch (Type)
            {
                //添加一首歌
                case 1:
                    {
                        var LikeRel = db.User_InfoSet.FirstOrDefault(m => m.UserId == userId).User_Music_Like_rel.FirstOrDefault(m => m.MusicId == s);
                        if(LikeRel == null)
                        {
                            db.User_Music_Like_rel.Add(new User_Music_Like_rel { UserId = userId, MusicId = (long)s });
                            db.SaveChanges();
                        }
                        else
                        {
                            db.User_Music_Like_rel.Remove(LikeRel);
                            db.SaveChanges();
                        }
                        return Content("操作成功");
                    }
                //添加一个专辑的歌
                case 2:
                    {
                        var album = db.AlbumSet.Find(s).MusicSet;
                        foreach(var item in album)
                        {
                            var LikeRel = db.User_InfoSet.Find(userId).User_Music_Like_rel.FirstOrDefault(m => m.MusicId == s);

                            if (LikeRel == null)
                            {
                                db.User_Music_Like_rel.Add(new User_Music_Like_rel { UserId = userId, MusicId = (long)s });
                            }
                            else continue;
                            db.SaveChanges();                            
                        }
                        return Content("操作成功");
                    }
                //添加一批歌
                case 3: return new HttpStatusCodeResult(503);
                    //添加一个歌单
                case 4:
                    {
                        var sheet = db.MusicSheetSet.Find(s);
                        var LikeRel = db.User_InfoSet.Find(userId).User_MusicSheet_rel.FirstOrDefault(m=>m.MusicSheetId==s);
                        if (LikeRel == null)
                        {
                            db.User_MusicSheet_rel.Add(new User_MusicSheet_rel { UserId = userId, MusicSheetId = (long)s });
                            db.SaveChanges();
                            return Content("收藏成功");
                        }
                        else
                        {
                            db.User_MusicSheet_rel.Remove(LikeRel);
                            db.SaveChanges();
                            return Content("取消收藏成功");
                        }
                        
                    }
                default:return new HttpStatusCodeResult(504);
            }

        }
        public FileResult Download(FormCollection form)

        {
            CreateSessionWithCookie();
            //if (form["MuiscId"] == null) return null;
            long QueryString = Convert.ToInt64(form["MusicId"]);
            var music = db.MusicSet.FirstOrDefault(m => m.MusicId == QueryString);
            string MusicFile = System.Configuration.ConfigurationManager.AppSettings["MusicFileFolder"] + "/" + music.MusicFile;
            return File(Server.MapPath(MusicFile), "audio/mpeg3", music.MusicName + ".mp3");

        }
        public ActionResult DisplaySheet(long? s,int? Type)
        {
            var userId = Convert.ToInt64(CreateSessionWithCookie());

            ViewBag.theMusicId = Request["MusicId"];
            var MusicSheets = db.User_InfoSet.Find(userId).MusicSheetSet;
            return PartialView(MusicSheets.ToList());
            
            if (Type == null) return new HttpStatusCodeResult(501);
            switch (Type)
            {
                //用于MusicSearch
                case 1:
                    {
                        ViewBag.type = 1; 
                        ViewBag.theMusicId = s; 
                        break;
                    }
                //用于AlbumSearch
                case 2:
                    {
                        ViewBag.type = 2;
                        ViewBag.theAlbumId = s;
                        break;
                    }
                default:
                    {
                        return new HttpStatusCodeResult(501);
                    }
            }
            ViewBag.IsLiking= IsLiking((long)s, Convert.ToInt64(CreateSessionWithCookie())) ? "true" : "false";
            return PartialView(MusicSheets.ToList());

        }
        public ActionResult CreateSheet()
        {

            var userId = Convert.ToInt64(CreateSessionWithCookie());

            var SheetName = Request["SheetName"]; var user = db.User_InfoSet.Find(userId);
            var TheSheet = user.MusicSheetSet.FirstOrDefault(m => m.Name == SheetName);
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
        public ActionResult AddToSheet(long? s,long? sheetId,int? Type)
        {
            var userId = Convert.ToInt64(CreateSessionWithCookie());            
            if (s * sheetId * userId * Type == 0) return new HttpStatusCodeResult(500);
            
            var theSheet = db.MusicSheetSet.Find(sheetId);
            switch (Type)
            {
                //添加一首歌到某歌单
                case 1:
                    {                       
                        var theSong = theSheet.Music_MusicSheet_rel.FirstOrDefault(m => m.MusicId == s);
                        if (theSong != null) return new HttpStatusCodeResult(501);
                        else
                        {
                            theSheet.Music_MusicSheet_rel.Add(new Music_MusicSheet_rel
                            {
                                MusicId = (long)s
                            });
                          
                            db.SaveChanges();
                        }
                        return Content("添加成功");
                    }
                //添加一个专辑到某歌单
                case 2:
                    {
                        var album = db.AlbumSet.Find(s).MusicSet;
                        foreach(var item in album)
                        {                                   
                            var theSong = theSheet.Music_MusicSheet_rel.FirstOrDefault(m => m.MusicId == item.MusicId);
                            if (theSong != null) continue;
                            else
                            {
                                theSheet.Music_MusicSheet_rel.Add(new Music_MusicSheet_rel
                                {
                                    MusicId = item.MusicId
                                });                       
                            }
                            db.SaveChanges();                     
                        }
                        return Content("添加成功");
                    }
                //添加一批歌到某歌单
                case 3:
                    {
                        return new HttpStatusCodeResult(503);
                    }
                default:
                    {
                        return new HttpStatusCodeResult(503);
                    }
            }            
        }
        
        public void AddToPlayList(long? s, int? Type)
        {

            var userId = CreateSessionWithCookie();

            if (s == null && Type != 4) return;

            var PlayListCookie = Request.Cookies.Get("PlayListCookie-u" + userId);
            if (PlayListCookie == null)
            {
                PlayListCookie = new HttpCookie("PlayListCookie-u" + userId);
            }
            var PlayList = PlayListCookie.Values["PlayList"];

            switch (Type)
            {
                //添加一首歌到播放列表
                case 1:
                    {
                        AddSingleToPlayListString(s, PlayList);
                        PlayListCookie.Values["PlayList"] = PlayList;
                        PlayListCookie.Values["UserId"] = userId;
                        PlayListCookie.Expires = DateTime.Now.AddMonths(1);
                        Response.Cookies.Add(PlayListCookie);
                        break;
                    }
                //添加一个专辑到播放列表
                case 2:
                    {
                        var album = db.AlbumSet.Find(s).MusicSet.ToList();
                        for (int i = album.Count() - 1; i >= 0; i--)
                        {
                            AddSingleToPlayListString(album[i].MusicId, PlayList);
                        }
                        PlayListCookie.Values["PlayList"] = PlayList;
                        PlayListCookie.Values["UserId"] = userId;
                        PlayListCookie.Expires = DateTime.Now.AddMonths(1);
                        Response.Cookies.Add(PlayListCookie);
                        break;
                    }
                //添加一个歌手的歌30首到播放列表
                case 3:
                    {
                        var artistMusicSets = db.ArtistSet.Find(s).MusicSet.Take(30).ToList();
                        for (int i = artistMusicSets.Count() - 1; i >= 0; i--)
                        {
                            AddSingleToPlayListString(artistMusicSets[i].MusicId, PlayList);
                        }
                        PlayListCookie.Values["PlayList"] = PlayList;
                        PlayListCookie.Values["UserId"] = userId;
                        PlayListCookie.Expires = DateTime.Now.AddMonths(1);
                        Response.Cookies.Add(PlayListCookie);
                        break;
                    }
                //添加一个歌单到播放列表
                case 4:
                    {
                        var sheet = db.MusicSheetSet.Find(s).Music_MusicSheet_rel.ToList();
                        for (int i = sheet.Count() - 1; i >= 0; i--)
                        {
                            AddSingleToPlayListString(sheet[i].MusicId, PlayList);
                        }
            else if (PlayList.Contains(',' + musicId)) PlayList = PlayList.Replace(',' + musicId, "") + ',' + musicId;
            else PlayList = PlayList.Replace(musicId + ',', "") + ',' + musicId;
                        PlayListCookie.Values["PlayList"] = PlayList;
                        PlayListCookie.Values["UserId"] = userId;
                        PlayListCookie.Expires = DateTime.Now.AddMonths(1);
                        Response.Cookies.Add(PlayListCookie);
                        break;
                    }
                default:
                    {
                        return;
                    }
            }
        }

        public string AddSingleToPlayListString(long? s,string PlayList)
        {
            var s_str = s.ToString();
            //播放列表是一个字符串类似于1,2,3,4,5,6
            //播放列表中没有内容
            if (PlayList == null || PlayList == "") PlayList = s_str;

            else if (!PlayList.Contains(s_str + ",") && PlayList != s_str) PlayList = s_str + "," + PlayList.Replace("," + s_str, "");
            //这首歌存在于播放列表，而且这首歌在播放列表的中间或尾巴
            return PlayList;
        }
        public List<MusicSet> GetPlayListMusicSets()
        {
            var userId = CreateSessionWithCookie();
            var PlayListCookie = Request.Cookies.Get("PlayListCookie-u" + userId);
            if (PlayListCookie == null)
            {
                return new List<MusicSet>();
            }           
            return GetPlayListMusicSets(PlayListCookie);
        }
        public List<MusicSet> GetPlayListMusicSets(HttpCookie PlayListCookie)
        {           
            var PlayList = PlayListCookie.Values["PlayList"];
            return GetPlayListMusicSets(PlayList);            
        }
        public List<MusicSet> GetPlayListMusicSets(string PlayList)
        {
            var musicSets = new List<MusicSet>();
            foreach (var id in stringParse.PlayListStringToListLong(PlayList))
            {
                MusicSet log;
                if ((log = db.MusicSet.Find(id)) != null)
                {
                    ParseMusic(log);
                    musicSets.Add(log);
                }
            }
            return musicSets;
        }
        public PartialViewResult ArtistOrAlbum()
        {
            var QueryString = Request["s"];
            if (QueryString == null || QueryString == "") return null;
            ArtistSet artist; AlbumSet album;

            if ((artist = db.ArtistSet.FirstOrDefault(m => m.Name == QueryString)) != null)
            {
                if (artist.Image == null || artist.Image == "")
                {
                    foreach (var music in artist.MusicSet) { if (music.MusicImage != null) { artist.Image = ImgProcession.ImageToBase64(Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["MusicImagesFolder"] + "\\" + music.MusicImage)); break; } }
                    if (artist.Image == null || artist.Image == "") artist.Image = ImgProcession.ImageToBase64(Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["AvatarFolder_cs"] + "\\" + System.Configuration.ConfigurationManager.AppSettings["DefaultAvatar"]));

                }
                else artist.Image = ImgProcession.ImageToBase64(Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["MusicImagesFolder"] + "\\" + artist.Image));
                return PartialView("ArtistCard", artist);
            }
            else if ((album = db.AlbumSet.FirstOrDefault(m => m.Name == QueryString)) != null)
            {
                if (album.Image == null || album.Image == "")
                {
                    foreach (var music in album.MusicSet) { if (music.MusicImage != null) { album.Image = ImgProcession.ImageToBase64(Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["MusicImagesFolder"] + "\\" + music.MusicImage)); break; } }
                    if (album.Image == null || album.Image == "") album.Image = ImgProcession.ImageToBase64(Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["AvatarFolder_cs"] + "\\" + System.Configuration.ConfigurationManager.AppSettings["DefaultAvatar"])); ;
                }
                else album.Image = ImgProcession.ImageToBase64(Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["MusicImagesFolder"] + "\\" + album.Image));
                return PartialView("AlbumCard", album);
            }
            else return null;
        }
        public ActionResult Main(long? s,int? Type)
        {
            _ = CreateSessionWithCookie();

            if (Type * s == 0) return new HttpStatusCodeResult(502);
            switch(Type){
                //type=1显示音乐主页
                case 1:
                    {
                        var log=db.MusicSet.Find(s);
                        ParseMusic(log);
                        return View("MusicMain",log);
                    }
                    //type=3，显示专辑主页
                case 3:
                    {
                        var log = db.AlbumSet.Find(s);
                        ParseImg(log);
                        return View("AlbumMain",log);
                    }
                    //type=2，显示歌手单曲主页
                case 2:
                    {
                        var log = db.ArtistSet.Find(s);
                        ParseImg(log);
                        ViewBag.type = 1;
                        return View("ArtistMain", log);
                        
                    }
                    //type=4，显示歌手专辑主页
                case 4:
                    {
                        var log = db.ArtistSet.Find(s);
                        ParseImg(log);
                        ViewBag.type = 2;
                        return View("ArtistMain", log);
                    }
                //type=5，显示歌手MV主页
                case 5:
                    {
                        var log = db.ArtistSet.Find(s);
                        ParseImg(log);
                        ViewBag.type = 3;
                        return View("ArtistMain", log);
                    }
                    //歌单主页
                case 6:
                    {
                        var log = db.MusicSheetSet.Find(s);
                        ParseImg(log);
                        //ViewBag.type = 3;
                        return View("SheetMain", log);
                    }
                //case 7://显示MV主页
            }
            return new HttpStatusCodeResult(503);
        }
        public ActionResult SheetSearch(string s, int? Type)
        {
            CreateSessionWithCookie();

            if (s == null||Type==null) return new HttpStatusCodeResult(500);

            switch (Type)
            {
                //按歌单名搜索
                case 1:
                    {
                        var Sheets = db.MusicSheetSet.Where(m=>m.Name.Contains(s));
                        
                        foreach(var log in Sheets)
                        {
                            if (log.Image == null || log.Image == "")
                            {
                                foreach (var rel in log.Music_MusicSheet_rel) { if (rel.MusicSet.MusicImage != null) { log.Image = ImgProcession.ImageToBase64(Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["MusicImagesFolder"] + "\\" + rel.MusicSet.MusicImage)); break; } }
                                if (log.Image == null || log.Image == "") log.Image = ImgProcession.ImageToBase64(Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["AvatarFolder_cs"] + "\\" + System.Configuration.ConfigurationManager.AppSettings["DefaultAvatar"]));
                            }
                            else log.Image = ImgProcession.ImageToBase64(Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["MusicImagesFolder"] + "\\" + log.Image));

                        }
                        if (Sheets != null) return PartialView(Sheets.ToList());
                        else return new HttpStatusCodeResult(501);
                    }
                //按用户收藏搜索
                case 2:
                    {
                        var qsLong = long.Parse(s);
                        List<MusicSheetSet> SheetList = new List<MusicSheetSet>();
                        foreach(var rel in db.User_InfoSet.Find(qsLong).User_MusicSheet_rel)
                        {
                            var log=rel.MusicSheetSet;
                            if (log.Image == null || log.Image == "")
                            {
                                foreach (var item in log.Music_MusicSheet_rel) { if (item.MusicSet.MusicImage != null) { log.Image = ImgProcession.ImageToBase64(Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["MusicImagesFolder"] + "\\" + item.MusicSet.MusicImage)); break; } }
                                if (log.Image == null || log.Image == "") log.Image = ImgProcession.ImageToBase64(Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["AvatarFolder_cs"] + "\\" + System.Configuration.ConfigurationManager.AppSettings["DefaultAvatar"]));
                            }
                            else log.Image = ImgProcession.ImageToBase64(Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["MusicImagesFolder"] + "\\" + log.Image));

                            SheetList.Add(rel.MusicSheetSet);
                            
                        }
                        return PartialView(SheetList);
                    }
                default: return new HttpStatusCodeResult(504);
            }
        }
        public ActionResult MVSearch(string s, int? Type)
        {
            CreateSessionWithCookie();

            if (s == null || Type == null) return new HttpStatusCodeResult(500);

            switch (Type)
            {
                //按MV名搜索
                case 1:
                    {
                        var log = db.MVSet.Where(m => m.Name.Contains(s));
                        if (log != null) return PartialView(log.ToList());
                        else return new HttpStatusCodeResult(501);

                        
                    }
                default: return new HttpStatusCodeResult(504);
            }
        }
        public string IsLiking_(long? s)
        {
            if (s == null) return "false";
            return IsLiking((long)s, Convert.ToInt64(CreateSessionWithCookie())) ? "true" : "false";
        }
        public bool IsLiking(long s, long u)
        {
            var theUser = db.User_InfoSet.Find(u);
            return IsLiking(s, theUser);
        }
        public bool IsLiking(long s, User_InfoSet theUser)
        {
            if ((_ = theUser.User_Music_Like_rel.FirstOrDefault(m => m.MusicId == s)) == null)
            {
                return false;
            }
            else return true;
        }
        public ActionResult Player(long? s, int? Type)
        {
            //1歌曲2专辑3歌手4歌单 空仅创建
            switch (Type)
            {

                    //创建播放器并立即播放某歌曲
                case 1:
                    {
                        //将此歌插到播放列表第一位
                        if(s==null) return new HttpStatusCodeResult(501);
                        AddToPlayList(s, 1);
                        return View(GetPlayListMusicSets());
                        //创建播放器
                        
                    }
                //创建播放器并立即播放某专辑
                case 2:
                    {
                        if(s==null) return new HttpStatusCodeResult(502);
                        AddToPlayList(s, 2);
                        return View(GetPlayListMusicSets());
                    }
                    
                case 3:
                    {
                        if (s == null) return new HttpStatusCodeResult(503);
                        AddToPlayList(s, 3);
                        return View(GetPlayListMusicSets());
                    }
                case 4:
                    {
                        if (s == null) return new HttpStatusCodeResult(502);
                        AddToPlayList(s, 4);
                        return View(GetPlayListMusicSets());
                    }
                //仅创建播放器
                default:
                    {
                        return View(GetPlayListMusicSets());
                    }
            }
        }
        //ajax方法调用播放单（多）歌曲、歌手、专辑、歌单
        //public ActionResult PlayNow(long? s,int? Type)
        //{
        //    //Type：1、播放一只单曲 2、播放多只选择的单曲 3、播放歌手热门歌曲 4、播放专辑全部歌曲 5、播放歌单全部歌曲
        //    switch (Type)
        //    {
        //        case 1:
        //            {

        //            }
                    
        //    }
        //}
        public object GetSingleMusicSet(MusicSet log)
        {
            
            if (log != null)
            {
                ParseMusic(log);                
            }
            else return new {
                id = 0
            };
            return new
            {
                id = log.MusicId,
                name = log.MusicName,
                artistId = log.ArtistId,
                artistName = log.ArtistSet.Name,
                albumId = log.AlbumId,
                albumName = log.AlbumSet.Name,
                img = log.MusicImage,
                duration = log.MusicDuration,
                lrc = log.MusicLyrics
            };
        }
        public object GetSingleMusicSet(long? s)
        {
            var log = db.MusicSet.Find(s);
            return GetSingleMusicSet(log);
        }
        //获取Type 1单曲、2专辑、3歌单、4歌手的Id，查询并以Json数组返回一批音乐的所有信息
        public JsonResult GetMusicSets(long? s,int? Type)
        {
            if (s == null) return null;
            List<long> musicIds = new List<long>();
            switch (Type)
            {
                case 1:
                    {                                       
                        return Json(new List<object> { GetSingleMusicSet(s)});
                    }
                case 2:
                    {                        
                        var musicSets = db.AlbumSet.Find(s).MusicSet;
                        var MusicList = new List<object>();
                        foreach(var log in musicSets)
                        {
                            MusicList.Add(GetSingleMusicSet(log));
                        }
                        return Json(MusicList);
                    }
                case 3:
                    {
                        var rels = db.MusicSheetSet.Find(s).Music_MusicSheet_rel;
                        var musicSets = new List<MusicSet>();
                        var MusicList = new List<object>();
                        foreach (var rel in rels)
                        {
                            musicSets.Add(rel.MusicSet);
                        }
                        
                        foreach (var log in musicSets)
                        {
                            MusicList.Add(GetSingleMusicSet(log));
                        }
                        return Json(MusicList);
                    }
                case 4:
                    {
                        var musicSets = db.ArtistSet.Find(s).MusicSet.ToList();
                        var MusicList = new List<object>();
                        foreach (var log in musicSets)
                        {
                            MusicList.Add(GetSingleMusicSet(log));
                        }
                        return Json(MusicList);
                    }
                default:return null;
            }            
        }

        internal void ParseMusic(MusicSet log)
        {
            
            //把音乐的图像处理成Base64字符串
            if (log.MusicImage == null || log.MusicImage == "")
            {
                foreach (var music in log.AlbumSet.MusicSet) { if (music.MusicImage != null) { log.MusicImage = ImgProcession.ImageToBase64(Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["MusicImagesFolder"] + "\\" + music.MusicImage)); break; } }
                if (log.MusicImage == null || log.MusicImage == "") log.MusicImage = ImgProcession.ImageToBase64(Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["AvatarFolder_cs"] + "\\" + System.Configuration.ConfigurationManager.AppSettings["DefaultAvatar"]));
            }
            else log.MusicImage = ImgProcession.ImageToBase64(Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["MusicImagesFolder"] + "\\" + log.MusicImage));

            //把音乐的音乐文件路径和歌词文件路径进行处理
            log.MusicFile = $"{System.Configuration.ConfigurationManager.AppSettings["_MusicFileFolder"]}/{log.MusicFile}";
            log.MusicLyrics = $"{System.Configuration.ConfigurationManager.AppSettings["_MusicLyricsFolder"]}/{log.MusicLyrics}";

            //TODO:把歌词文件读取成字符串
            //TODO:把音乐文件读取为流
        }
        internal void ParseImg(ArtistSet log)
        {
            if (log.Image == null || log.Image == "")
            {
                foreach (var music in log.MusicSet) { if (music.MusicImage != null) { log.Image = ImgProcession.ImageToBase64(Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["MusicImagesFolder"] + "\\" + music.MusicImage)); break; } }
                if (log.Image == null || log.Image == "") log.Image = ImgProcession.ImageToBase64(Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["AvatarFolder_cs"] + "\\" + System.Configuration.ConfigurationManager.AppSettings["DefaultAvatar"]));
            }
            else log.Image = ImgProcession.ImageToBase64(Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["MusicImagesFolder"] + "\\" + log.Image));

        }
        internal void ParseImg(AlbumSet log)
        {
            if (log.Image == null || log.Image == "")
            {
                foreach (var music in log.MusicSet) { if (music.MusicImage != null) { log.Image = ImgProcession.ImageToBase64(Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["MusicImagesFolder"] + "\\" + music.MusicImage)); break; } }
                if (log.Image == null || log.Image == "") log.Image = ImgProcession.ImageToBase64(Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["AvatarFolder_cs"] + "\\" + System.Configuration.ConfigurationManager.AppSettings["DefaultAvatar"]));
            }
            else log.Image = ImgProcession.ImageToBase64(Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["MusicImagesFolder"] + "\\" + log.Image));

        }
        internal void ParseImg(MusicSheetSet log)
        {
            if (log.Image == null || log.Image == "")
            {
                foreach (var rel in log.Music_MusicSheet_rel) { if (rel.MusicSet.MusicImage != null) { log.Image = ImgProcession.ImageToBase64(Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["MusicImagesFolder"] + "\\" + rel.MusicSet.MusicImage)); break; } }
                if (log.Image == null || log.Image == "") log.Image = ImgProcession.ImageToBase64(Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["AvatarFolder_cs"] + "\\" + System.Configuration.ConfigurationManager.AppSettings["DefaultAvatar"]));
            }
            else log.Image = ImgProcession.ImageToBase64(Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["MusicImagesFolder"] + "\\" + log.Image));

        }
        internal void ParseImg(MVSet log)
        {
            if (log.Image == null || log.Image == "")
            {
                foreach (var music in log.MusicSet.AlbumSet.MusicSet) { if (music.MusicImage != null) { log.Image = ImgProcession.ImageToBase64(Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["MusicImagesFolder"] + "\\" + music.MusicImage)); break; } }
                if (log.Image == null || log.Image == "") log.Image = ImgProcession.ImageToBase64(Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["AvatarFolder_cs"] + "\\" + System.Configuration.ConfigurationManager.AppSettings["DefaultAvatar"]));
            }
            else log.Image = ImgProcession.ImageToBase64(Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["MusicImagesFolder"] + "\\" + log.Image));

        }
        public ActionResult ListPage()
        {
            return PartialView("~/Player/ListPage",GetPlayListMusicSets());
        }
        public string GetStyles()
        {
            var styleSets = db.StyleSet.ToList();
            string styleStr = "";
            foreach(var item in styleSets)
            {
                styleStr += item.Name + ",";
            }
            styleStr = styleStr.Remove(styleStr.Count()-1, 1);
            return styleStr;
        }
        public ActionResult SearchCardView(int? Type,string Letter,string Gender,string Region,int? Style)
        {
            switch (Type)
            {
                case 1:
                    {
                        var artists = db.ArtistSet.Where(m=>true);
                        
                        if (Letter != "0"&&Letter!="#") artists = artists.Where(m => m.Initial == Letter);
                        else if(Letter=="#") artists = artists.Where(m => m.Initial[0] > 122 && m.Initial[0] < 97);

                        if (Gender != "0") artists = artists.Where(m => m.Gender == Gender);

                        if (Region != "0") artists = artists.Where(m => m.Region == Region);

                        if (Style != 0) artists = artists.Where(m => m.StyleId==Style);

                        List<Dictionary<string, string>> dicList = new List<Dictionary<string, string>>();
                        foreach (var item in artists.ToList())
                        {
                            ParseImg(item);
                            var dic = new Dictionary<string, string>();


                            dic.Add("ImageUrl", item.Image);
                            dic.Add("Content", item.Name);
                            dic.Add("Type", "4");
                            dic.Add("Id", item.ArtistId.ToString());
                            dicList.Add(dic);
                        }
                        //ViewBag.type = 1;
                        return PartialView(dicList);
                        
                    }
                default:return null;
            }

        }
        public char ACCA(string str)
        {
            return stringParse.getSpell(str);
        }
        
    }
}
