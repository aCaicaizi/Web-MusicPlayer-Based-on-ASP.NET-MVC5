using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSR.Common
{
    public class stringParse
    {
        public static List<long> PlayListStringToListLong(string PlayList)
        {
            var ToPlaySet = PlayList.Split(',');
            var MusicList = new List<long>();
            foreach (var str in ToPlaySet)
            {
                MusicList.Add(long.Parse(str));

            }
            return MusicList;
        }
        public static char getSpell(string strText)
        {
            var cnChar = strText.Substring(0, 1);
            byte[] arrCN = Encoding.Default.GetBytes(cnChar);
            if (arrCN.Length > 1)
            {
                int area = (short)arrCN[0];
                int pos = (short)arrCN[1];
                int code = (area << 8) + pos;
                int[] areacode = { 45217, 45253, 45761, 46318, 46826, 47010, 47297, 47614, 48119, 48119, 49062, 49324, 49896, 50371, 50614, 50622, 50906, 51387, 51446, 52218, 52698, 52698, 52698, 52980, 53689, 54481 };
                for (int i = 0; i < 26; i++)
                {
                    int max = 55290;
                    if (i != 25) max = areacode[i + 1];
                    if (areacode[i] <= code && code < max)
                    {
                        return Encoding.Default.GetString(new byte[] { (byte)(65 + i) }).ToLower()[0];
                    }
                }
                return '*';
            }
            else return cnChar.ToLower()[0];
        }
    }

    
}
