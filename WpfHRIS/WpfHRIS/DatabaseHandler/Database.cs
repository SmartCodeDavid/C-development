using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

using WpfHRIS.Teaching;

namespace WpfHRIS.DatabaseHandler
{
    class Database
    {
        public MySqlConnection mysqlcon;
        public enum kName
        {
            givenName = 0,
            familyName,
            title,
            campus,
            phone,
            room,
            email,
            photo,
            category
        }
        public Database()
        {
            string db = "kit206";
            string user = "kit206";
            string password = "kit206";
            string server = "alacritas.cis.utas.edu.au";

            string connectionString = String.Format("Database={0};Data Source={1};User Id={2};Password={3}", db, server, user, password);
            mysqlcon = new MySqlConnection(connectionString); //connect the database               
        }
        public int countNum()
        {
            int count = -1;//indicates error
            mysqlcon.Open();
            MySqlCommand cmd = new MySqlCommand("select COUNT(*) from staff", mysqlcon);
            count = int.Parse(cmd.ExecuteScalar().ToString());
            mysqlcon.Close();
            return count;
        }

        //Retrieve data for class
        public List<Class> retrieveDataClass(string unitCode, string campus) {
            List<Class> classList = new List<Class>();
            string com;
            if (campus.Equals("All"))
            {
                com = string.Format("select campus, day, start, end, type, room, staff from class where unit_code='{0}' order by start asc, end asc", unitCode);
            } else {
                com = string.Format("select campus, day, start, end, type, room, staff from class where unit_code='{0}' AND campus='{1}' order by start asc, end asc", unitCode, campus);
            }

            mysqlcon.Open();
            MySqlCommand cmd = new MySqlCommand(com, mysqlcon);
            MySqlDataReader dataReader = cmd.ExecuteReader();
            while (dataReader.Read()) {
                string time = dataReader[2].ToString() + "-" + dataReader[3].ToString();
                Class c = new Class(unitCode, dataReader[0].ToString(), dataReader[1].ToString(), time, dataReader[4].ToString(), dataReader[5].ToString(), dataReader[6].ToString());
                classList.Add(c);
            }
            dataReader.Close();

            //get staff name as well
            foreach (var c in classList) {
                com = string.Format("select given_name, family_name from staff where id='{0}'", c.staff_id);
                cmd = new MySqlCommand(com, mysqlcon);
                dataReader = cmd.ExecuteReader();
                while (dataReader.Read()) {
                    string staffName = dataReader[0].ToString() + " " + dataReader[1].ToString();
                    c.staff_name = staffName;
                }
                dataReader.Close();
            }
            return classList;
        }

        //Retrieve data for unit
        public List<Unit> retrieveDataUnit() {
            List<Unit> unitList = new List<Unit>();
            string com = "select code, title from unit";
            mysqlcon.Open();
            MySqlCommand cmd = new MySqlCommand(com, mysqlcon);
            MySqlDataReader dataReader = cmd.ExecuteReader();
            while (dataReader.Read()) {
                Unit u = new Unit(dataReader[0].ToString(), dataReader[1].ToString());
                unitList.Add(u);
            }
            // unitList.Sort()
            unitList.Sort(new CompareUnit());
            dataReader.Close();
            return unitList;
        }

        //Retrieve data for person
        public List<Person> retrieveData(string option)
        {
            string com;
            if (!option.Equals("All"))
            {
                com = string.Format("select given_name, family_name, title, campus, phone, room, email, photo, category from staff where category='{0}'", option);
            }
            else {
                com = "select given_name, family_name, title, campus, phone, room, email, photo, category from staff";
            }
            mysqlcon.Open();
            MySqlCommand cmd = new MySqlCommand(com, mysqlcon);
            MySqlDataReader dataReader = cmd.ExecuteReader();
            List<Person> person = new List<Person>();
            while (dataReader.Read())
            {
                Person p = new Person(dataReader[0].ToString(), dataReader[1].ToString(), dataReader[2].ToString(), dataReader[3].ToString(),
                    dataReader[4].ToString(), dataReader[5].ToString(), dataReader[6].ToString(), dataReader[7].ToString(), dataReader[8].ToString()
                    );
                person.Add(p);
            }
            person.Sort(new Compare());
            dataReader.Close();

            //retrieve consultation time for each person
            foreach (var p in person)
            {
                com = string.Format("select day, start, end from consultation where staff_id IN (select id from staff where given_name='{0}' AND family_name='{1}')",
                            p.givenName, p.familyName
                    );
                // com = "select day, start, end from consultation where staff_id = 123460";
                cmd = new MySqlCommand(com, mysqlcon);
                dataReader = cmd.ExecuteReader();
                while (dataReader.Read())
                {
                    string consultationTime = dataReader[1].ToString() + "-" + dataReader[2].ToString() + " " + dataReader[0].ToString() + "\r\n";
                    p.consultation += consultationTime;
                }
                dataReader.Close();
            }

            //retrieve teachingTime for each person
            foreach (var p in person)
            {
                com = string.Format("select code, title from unit where coordinator IN (select id from staff where given_name='{0}' AND family_name='{1}')",
                            p.givenName, p.familyName
                    );
                // com = "select day, start, end from consultation where staff_id = 123460";
                cmd = new MySqlCommand(com, mysqlcon);
                dataReader = cmd.ExecuteReader();
                while (dataReader.Read())
                {
                    string teachingTime = dataReader[0].ToString() + " " + dataReader[1].ToString() + "\r\n";
                    p.teachingTime += teachingTime;
                }
                dataReader.Close();
            }
            return person;
        }
        public List<string[]> retrieveTime(string fName, string gName, string option)
        {
            MySqlCommand cmd;
            MySqlDataReader dataReader;
            List<string[]> timeList = new List<string[]>();
            string campus = "";
            string com = "";
            if (option.Equals("HeatMap"))
            {
                campus = (gName.Equals("All")) ? "" : string.Format(" where campus='{0}'", gName);
                //grab class time for heat map firstly -- > fName indicates consultation, class time or both when option is HeatMap
                if (fName.Equals("Unit Class") || fName.Equals("All"))
                {
                    com = "select start, end, day from class" + campus;
                    // com = string.Format("select start, end, day from class where campus='{0}'", campus);
                }
            } else if (option.Equals("CrashMap")) { //when retrieve data for showing crash map then the fName and gName indicate unitcode and campus respectively
                campus = (gName.Equals("All")) ? "" : string.Format(" AND campus='{0}'", gName);
                com = string.Format("select start, end, day from class where unit_code='{0}'", fName) + campus;
            } else { //otherwise grab class time for staff
                //grab class time firstly with family name and given name
                com = string.Format("select start, end, day from class where staff in (select id from staff where given_name='{0}' AND family_name='{1}')", gName, fName);
            }

            mysqlcon.Open();
            if (! com.Equals("")) {
                cmd = new MySqlCommand(com, mysqlcon);
                dataReader = cmd.ExecuteReader();
                while (dataReader.Read())
                {
                    //string startToEnd = string.Format("{0}-{1}", dataReader[0].ToString(), dataReader[1].ToString());
                    string startT = dataReader[0].ToString();
                    string endT = dataReader[1].ToString();
                    string day = dataReader[2].ToString();
                    //string[] array = new string[] {startToEnd, day};
                    string[] array = new string[] { startT, endT, day };

                    timeList.Add(array);
                }
                dataReader.Close();
            }


            if (option.Equals("HeatMap")) {
                //grab Consultation time for heat map
                if (fName.Equals("Consultation Time") || fName.Equals("All")) {
                    // com = string.Format("select start, end, day from consultation;
                    com = "select start, end, day from consultation";
                }
            } else if (option.Equals("CrashMap")) { 
                //grab consultation time for crash map
                com = string.Format("select start, end, day from consultation where staff_id in (select staff from class where unit_code='{0}'{1})", fName, campus);
            } else {
                //grab consultation time secondly with family name and given name
                com = string.Format("select start, end, day from consultation where staff_id in (select id from staff where given_name='{0}' AND family_name='{1}')", gName, fName);
            }
            
            cmd = new MySqlCommand(com, mysqlcon);
            dataReader = cmd.ExecuteReader();
            while (dataReader.Read())
            {
                //string startToEnd = string.Format("{0}-{1}", dataReader[0].ToString(), dataReader[1].ToString());
                string startT = dataReader[0].ToString();
                string endT = dataReader[1].ToString();
                string day = dataReader[2].ToString();
                //string[] array = new string[] {startToEnd, day};
                string[] array = new string[] { startT, endT, day };

                timeList.Add(array);
            }

            return timeList;
        }
    }
}
