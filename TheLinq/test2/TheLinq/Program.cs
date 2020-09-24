using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheLinq
{
    class Program
    {
        //abc
        static void Main(string[] args)
        {
            var lnkbl = new Linquable<Item>();
            var query = lnkbl.Where(i => i.Id == 3);
            var res = query.ToList();
            var one = query.First();


            var lnkblPrvdbl = new LinquableProvidable<Item>();
            var q = lnkblPrvdbl.Where(i => i.Id < 10);
            var res1 = q.ToList();
            var one1 = q.First();
        }
    }

    class Item
    {
        public int Id { get; set; }
        public string Title { get; set; }
    }
}
