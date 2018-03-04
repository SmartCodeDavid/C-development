using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfHRIS.Teaching
{
    class Person
    {
        public string givenName { get; set; }
        public string familyName { get; set; }
        public string title { get; set; }
        public string campus { get; set; }
        public string phone { get; set; }
        public string room { get; set; }
        public string email { get; set; }
        public string photo { get; set; }
        public string category { get; set; }
        public string consultation { get; set; }
        public string teachingTime { get; set; }


        public Person(string givenName, string familyName, string title, string campus,
            string phone, string room, string email, string photo, string category)
        {
            this.givenName = givenName;
            this.familyName = familyName;
            this.title = title;
            this.campus = campus;
            this.phone = phone;
            this.room = room;
            this.email = email;
            this.photo = photo;
            this.category = category;
        }
        public Person() { }
    }

    class Compare : IComparer<Person>
    {
        int IComparer<Person>.Compare(Person x, Person y)
        {
            return (x.givenName.CompareTo(y.givenName));
        }
    }
}
