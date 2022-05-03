using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSR.Common
{
    static public class ImgProcession
    {
		
		public static string ImageToBase64(string fileFullName)
		{
			//try
			//{
				Bitmap bmp = new Bitmap(fileFullName);
				MemoryStream ms = new MemoryStream();
				var suffix = fileFullName.Substring(fileFullName.LastIndexOf('.') + 1,
									fileFullName.Length - fileFullName.LastIndexOf('.') - 1).ToLower();
				var suffixName = suffix == "png"
									? ImageFormat.Png
									: suffix == "jpg" || suffix == "jpeg"
										? ImageFormat.Jpeg
										: suffix == "bmp"
											? ImageFormat.Bmp
											: suffix == "gif"
												? ImageFormat.Gif
												: ImageFormat.Jpeg;
				bmp.Save(ms, suffixName);
				byte[] arr = new byte[ms.Length];
				ms.Position = 0;
				ms.Read(arr, 0, (int)ms.Length);
				ms.Close();
				string base64String = "data:image/" + suffix + ";base64," + Convert.ToBase64String(arr);
				return base64String;
			//}

			//catch (Exception ex)
			//{
			
			//}
		}
		public static string ImageToBase64(string FolderPath, string FileName)
			{
				return ImageToBase64(Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["MusicImagesFolder"] + "\\" + music.MusicImage));
			}
		public static Image Base64ToImage(string base64)
        {
			//string[] Splited = base64.Split(',');
			//base64 = Splited[1];//将base64头部信息替换
			byte[] bytes = Convert.FromBase64String(base64);
			MemoryStream memStream = new MemoryStream(bytes);
			return Image.FromStream(memStream);
		}
	}
}
