using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfHRIS.Teaching
{
    class Class
    {
        public string unitCode { get; set; }
        public string campus { get; set; }
        public string day { get; set; }
        public string time { get; set; }
        public string type { get; set; }
        public string room { get; set; }
        public string staff_name { get; set; }
        public string staff_id { get; set; }

        public Class(string unitCode, string campus, string day, string time, string type, string room, string staff_id) {
            this.unitCode = unitCode;
            this.campus = campus;
            this.day = day;
            this.time = time;
            this.type = type;
            this.room = room;
            this.staff_id = staff_id;
        }
        public Class() { }
    }
}
