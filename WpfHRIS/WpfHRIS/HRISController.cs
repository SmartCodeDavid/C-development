using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Data;
using System.Windows.Media;
using System.Windows.Controls.Primitives;

using WpfHRIS.Teaching;
using WpfHRIS.DatabaseHandler;

namespace WpfHRIS
{
    class HRISController : MainWindow
    {
        //set up rows
        static string[] dayTime = { "9:00:00-10:00:00",
                                 "10:00:00-11:00:00",
                                 "11:00:00-12:00:00",
                                 "12:00:00-13:00:00",
                                 "13:00:00-14:00:00",
                                 "14:00:00-15:00:00",
                                 "15:00:00-16:00:00",
                                 "16:00:00-17:00:00",
                                 "17:00:00-18:00:00"};

        //set up rows
        static string[] dayHour = { "09:00:00",
                             "10:00:00",
                             "11:00:00",
                             "12:00:00",
                             "13:00:00",
                             "14:00:00",
                             "15:00:00",
                             "16:00:00",
                             "17:00:00",
                             "18:00:00"};

        static string[] dataWeek = { "Time", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday" };

        static string[] unitTab = { "Type", "Day", "Time", "Staff Member", "Room", "Campus"};

        enum UNITTABLE {
            type = 0,
            day,
            time,
            staff_member,
            room,
            campus
        }



        public static void showClassTable(string unitCode, ref DataGrid dg, ref ComboBox cb, string campus) {
            //retrieve data in relation to the class from database
            Database db = new Database();
            List<Class> classList = db.retrieveDataClass(unitCode, campus);

            //update the datagridunit
            DataTable dt = new DataTable();
            foreach (var d in unitTab)
            {
                dt.Columns.Add(d);
            }


            //clear all items in comboxCampus
            if (campus.Equals("All")) {
                cb.Items.Clear();
                cb.Items.Add("All");
                cb.SelectedIndex = 0;
            }

            foreach (var c in classList) {
                DataRow dr = dt.NewRow();
                dr["Type"] = c.type;
                dr["Day"] = c.day;
                dr["Time"] = c.time;
                dr["Staff Member"] = c.staff_name;
                dr["Room"] = c.room;
                dr["Campus"] = c.campus;
                dt.Rows.Add(dr);

                //add campus to comboxCampus if it does not exist the campus
                if (! cb.Items.Contains(c.campus)) {
                    cb.Items.Add(c.campus);
                }
            }


            dg.ItemsSource = dt.DefaultView;
            dg.CanUserAddRows = false;
            dg.CanUserDeleteRows = false;
            
            dg.IsReadOnly = true;

            //set width for each tabs
            dg.Columns[(int)UNITTABLE.type].Width = 50;
            dg.Columns[(int)UNITTABLE.day].Width = 75;
            dg.Columns[(int)UNITTABLE.time].Width = 110;
            dg.Columns[(int)UNITTABLE.staff_member].Width = 115;
            dg.Columns[(int)UNITTABLE.room].Width = 45;
            dg.Columns[(int)UNITTABLE.campus].Width = 76;
        }

        public static void setUpDataGridUnit(ref DataGrid dg)
        {
            DataTable dt = new DataTable();

            foreach (var d in unitTab)
            {
                dt.Columns.Add(d);
            }

            dg.ItemsSource = dt.DefaultView;
            dg.IsReadOnly = true;
            for (int i = 0; i < dg.Columns.Count; i++)
            {
                dg.Columns[i].Width = 77;
            }
        }

        public static void setUpDataGridStaff(ref DataGrid dg) {
            DataTable dt = new DataTable();

            foreach (var d in dataWeek)
            {
                dt.Columns.Add(d);
            }

            //set up rows
            foreach (var t in dayTime)
            {
                DataRow dr = dt.NewRow();
                dr["Time"] = t;
                dt.Rows.Add(dr);
            }

            dg.ItemsSource = dt.DefaultView;
            dg.IsReadOnly = true;
            
            dg.Columns[0].Width = 152;
            for (int i = 1; i < dg.Columns.Count; i++)
            {
                dg.Columns[i].Width = 77;
            }
        }

        //read data for listing unit
        public static List<Unit> readDataUnit() {
            Database db = new Database();
            return db.retrieveDataUnit();
        }

        public static void updateCrashMap(ref DataGrid dg, string unitCode, string campus) {
            //clear the previous background color
            clearDataGridBackgroundColor(ref dg);

            Database db = new Database();
            List<string[]> time = db.retrieveTime(unitCode, campus, "CrashMap");
            int row;
            int col;
            foreach (var t in time)
            {
                col = Array.IndexOf(dataWeek, t[2]);
                row = Array.IndexOf(dayHour, t[0]);
                int startT = int.Parse(t[0].Substring(0, 2));
                int endT = int.Parse(t[1].Substring(0, 2));
                for (int i = 0; i < endT - startT; i++)
                {
                    DataGridRow dr = (DataGridRow)dg.ItemContainerGenerator.ContainerFromIndex(row);
                    DataGridCellsPresenter presenter = GetVisualChild<DataGridCellsPresenter>(dr);
                    DataGridCell dc = presenter.ItemContainerGenerator.ContainerFromIndex(col) as DataGridCell;
                    SolidColorBrush colorGreen = new SolidColorBrush(Colors.Green);
                    //compare between current cell color and green color, if they are same then the color of this cell will be changed to red
                    if (dc.Background.ToString().Equals(colorGreen.Color.ToString()))
                    {
                        dc.Background = new SolidColorBrush(Colors.Red);
                    }
                    else {
                        dc.Background = new SolidColorBrush(Colors.Green);
                    }
                    row++;
                }
                row = 0;
            }
        }

        public static void showCrashMap(ref DataGrid dg) {
            //set up columns and rows for crash map
            DataTable dt = new DataTable();

            foreach (var d in dataWeek)
            {
                dt.Columns.Add(d);
            }

            //set up rows
            foreach (var t in dayTime)
            {
                DataRow dr = dt.NewRow();
                dr["Time"] = t;
                dt.Rows.Add(dr);
            }

            dg.ItemsSource = dt.DefaultView;
            dg.IsReadOnly = true;
            if (dg.Columns.Count != 0) {
                dg.Columns[0].Width = 120;
                for (int i = 1; i < dg.Columns.Count; i++)
                {
                    dg.Columns[i].Width = 70;
                }        
            }

        }

        public static void UpdateHeatMap(ref DataGrid dg, string unitOrConsul, string campus)
        {
            //set width for each cells in heatmap
            if (dg.Columns.Count != 0) {
                dg.Columns[0].Width = 147;
                for (int i = 1; i < dg.Columns.Count; i++)
                {
                    dg.Columns[i].Width = 120;
                }

                //clear the previous background color
                clearDataGridBackgroundColor(ref dg);

                //retrieve consultation time and teaching time from database
                Database db = new Database();
                List<string[]> time = db.retrieveTime(unitOrConsul, campus, "HeatMap");

                int row;
                int col;
                foreach (var t in time)
                {
                    col = Array.IndexOf(dataWeek, t[2]);
                    row = Array.IndexOf(dayHour, t[0]);
                    int startT = int.Parse(t[0].Substring(0, 2));
                    int endT = int.Parse(t[1].Substring(0, 2));
                    for (int i = 0; i < endT - startT; i++)
                    {
                        DataGridRow dr = (DataGridRow)dg.ItemContainerGenerator.ContainerFromIndex(row);
                        DataGridCellsPresenter presenter = GetVisualChild<DataGridCellsPresenter>(dr);
                        DataGridCell dc = presenter.ItemContainerGenerator.ContainerFromIndex(col) as DataGridCell;

                        dc.Background = new SolidColorBrush(Colors.Red);
                        row++;
                    }
                    row = 0;
                }         
            }
           
        }

        public static void showHeatMap(ref DataGrid dg)
        {
            DataTable dt = new DataTable();

            foreach (var d in dataWeek)
            {
                dt.Columns.Add(d);
            }

            //set up rows
            foreach (var t in dayTime)
            {
                DataRow dr = dt.NewRow();
                dr["Time"] = t;
                dt.Rows.Add(dr);
            }

            dg.ItemsSource = dt.DefaultView;
            dg.IsReadOnly = true;
            if (dg.Columns.Count != 0)
            {
                dg.Columns[0].Width = 152;
                for (int i = 1; i < dg.Columns.Count; i++)
                {
                    dg.Columns[i].Width = 77;
                }
            }

        }


        /// read person data
        public static List<Person> readData(string option)
        {
            Database db = new Database();
            return db.retrieveData(option);

        }

        //show gridview for staff consultation and teaching time
        public static void showGridView(string fName, string gName, ref DataGrid dg)
        {
            //clear the previous background color
            clearDataGridBackgroundColor(ref dg);


            //retrieve consultation time and teaching time from database
            Database db = new Database();
            List<string[]> time = db.retrieveTime(fName, gName, "");
            int row;
            int col;
            foreach (var t in time)
            {
                col = Array.IndexOf(dataWeek, t[2]);
                row = Array.IndexOf(dayHour, t[0]);
                int startT = int.Parse(t[0].Substring(0, 2));
                int endT = int.Parse(t[1].Substring(0, 2));
                for (int i = 0; i < endT - startT; i++)
                {
                    DataGridRow dr = (DataGridRow)dg.ItemContainerGenerator.ContainerFromIndex(row);
                    DataGridCellsPresenter presenter = GetVisualChild<DataGridCellsPresenter>(dr);
                    DataGridCell dc = presenter.ItemContainerGenerator.ContainerFromIndex(col) as DataGridCell;

                    //dc.Focus();
                    dc.Background = new SolidColorBrush(Colors.Red);
                    // dg.Rows[row].Cells[col].Selected = true;
                    row++;
                }
                row = 0;
            }
        }

        private static T GetVisualChild<T>(Visual parent) where T : Visual
        {
            T c = default(T);
            int numVisuals = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < numVisuals; i++)
            {
                Visual v = (Visual)VisualTreeHelper.GetChild(parent, i);
                c = v as T;
                if (c == null) {
                    c = GetVisualChild<T>(v);
                }
                if (c != null) {
                    break;
                }
            }
            return c;
        }

        private static void clearDataGridBackgroundColor(ref DataGrid dg) {
            int row = dg.Items.Count;
            for (; row-- > 0 ;) {
                //get row
                DataGridRow dr = (DataGridRow)dg.ItemContainerGenerator.ContainerFromIndex(row);
                DataGridCellsPresenter presenter = GetVisualChild<DataGridCellsPresenter>(dr);

                //set default color for each cell in current row.
                for (int col = 0; col < dg.Columns.Count; col++) {
                    DataGridCell dc = presenter.ItemContainerGenerator.ContainerFromIndex(col) as DataGridCell;
                    dc.Background = new SolidColorBrush(Colors.White);
                }
            }
        }
    }
}
