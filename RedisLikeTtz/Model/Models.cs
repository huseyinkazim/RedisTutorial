using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedisLikeTtz.Model
{
    public class BbsTemizlemeLog
    {
        //public string Id { get; set; }
        public Dictionary<string, string> bbs_kanal { get; set; }
        public string bbs_kanalId { get; set; }
        public string bbs_temizlemeaciklama { get; set; }

        public string bbs_kolon1 { get; set; }
        public string bbs_kolon1Clean { get; set; }
        public string bbs_kolon2 { get; set; }
        public string bbs_kolon2Clean { get; set; }
    }
}
