using RedisLikeTtz.Core;
using RedisLikeTtz.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedisLikeTtz
{
    class Program
    {
        static void Main(string[] args)
        {
            
            var key = "TestCleanLog";
            var dictionary = new Dictionary<string, string>();
            dictionary.Add("kanalKey1", "kanalDeger1");
            dictionary.Add("kanalKey2", "kanalDeger2");

            var log = new BbsTemizlemeLog
            {
                bbs_kanal=dictionary,
                bbs_kanalId="kanalId",
                bbs_kolon1="kolon1",
                bbs_kolon1Clean="kalan1",
                bbs_kolon2 = "kolon2",
                bbs_kolon2Clean = "kalan2",
                bbs_temizlemeaciklama="Gayet güzel temizlenmistir"
            };

            var log_2 = new BbsTemizlemeLog
            {
                bbs_kanal = dictionary,
                bbs_kanalId = "kanalId",
                bbs_kolon1 = "kolon1",
                bbs_kolon1Clean = "kalan1",
                bbs_kolon2 = "kolon2",
                bbs_kolon2Clean = "kalan2",
                bbs_temizlemeaciklama = "Gayet güzel temizlenmistir"
            };

            RedisLogger redLogger = new RedisLogger();
            redLogger.InsertItem(key, log);

            var size = redLogger.GetListSize(key);
            var list=redLogger.GetListItemRange<BbsTemizlemeLog>(key, 0, size);
            redLogger.RemoveList(key);
            Console.ReadKey();
        }
    }
}
