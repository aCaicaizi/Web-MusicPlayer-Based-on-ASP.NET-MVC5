using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LSR.Models;
using LSR.Common;
using System.IO;

namespace LSR.Controllers
{
    public class PlayerController : Controller
    {
        LSREntities db = new LSREntities();
        // GET: Player
        
        public ActionResult LyricsPage(long? s)
        {
            if (s == null) return null;
            var fileName = db.MusicSet.Find(s).MusicLyrics;
            fileName = Server.MapPath($"{System.Configuration.ConfigurationManager.AppSettings["MusicLyricsFolder"]}\\{fileName}");
            StreamReader LyricsSR = new StreamReader(fileName, System.Text.Encoding.UTF8);
            var LyricsText = LyricsSR.ReadToEnd().Split('\n').ToList();
            List<string> LyricsList = new List<string>();
            foreach (var item in LyricsText) LyricsList.Add(item.Replace("\n", "").Replace(" ", "").Replace("\t", "").Replace("\r", ""));
            LyricsSR.Close();
            return PartialView(LyricsList);
        }
        
    }
}