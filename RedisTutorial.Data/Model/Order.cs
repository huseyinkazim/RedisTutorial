using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedisTutorial.Data.Model
{
    public class Order : BaseEntity
    {
        public string OrderName { get; set; }
    }
}
