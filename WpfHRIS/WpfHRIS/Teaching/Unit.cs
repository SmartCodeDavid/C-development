using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfHRIS.Teaching
{
    class Unit
    {
        public string type { get; set; }
        public string day { get; set; }
        public string time { get; set; }
        public string staff { get; set; }
        public string room { get; set; }
        public string campus { get; set; }
        public string title{ get; set; }
        public string code { get; set; }

        public Unit(string code, string title) {
            this.code = code;
            this.title = title;
        }
        public Unit() { }
    }

    class CompareUnit : IComparer<Unit>
    {
        int IComparer<Unit>.Compare(Unit x, Unit y)
        {
            return (x.code.CompareTo(y.code));
        }
    }
}
