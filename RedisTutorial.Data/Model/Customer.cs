using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedisTutorial.Data.Model
{
    public class Customer : BaseEntity
    {
        public string Name { get; set; }
        public List<Order> Orders { get; set; }
    }
}
