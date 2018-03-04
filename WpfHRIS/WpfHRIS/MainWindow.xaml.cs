using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data;
using System.Text.RegularExpressions;

using WpfHRIS.Teaching;
using WpfHRIS.DatabaseHandler;

namespace WpfHRIS
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ComboBox cb;
        List<Person> person;
        List<Unit> unit;
        List<string> orderedPerson;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            //configure the comboBox for staff category
            string[] category = { "All", "Academic", "Technical", "Admin", "Casual" };
            foreach(var item in category) {
                comboBox1.Items.Add(item);
            }
            comboBox1.SelectedIndex = 0;

            //configure the datagrid for staff
            HRISController.setUpDataGridStaff(ref dataGrid);

            //set up unit list
            unit = HRISController.readDataUnit();
            foreach (var u in unit) {
                string s = u.code + " " + u.title;
                listBoxUnitList.Items.Add(s);
            }

            //hide the label campus and comboxCampus. it will display when specific unit is selected
            label10.Visibility = Visibility.Hidden;
            comboBoxCampus.Visibility = Visibility.Hidden;


            //set up heatmap page
            string[] campus = { "All", "Hobart", "Launceston" };
            foreach(var c in campus) {
                comboBoxHeatMapCampus.Items.Add(c);
            }
            comboBoxHeatMapCampus.SelectedIndex = 0;
            HRISController.showHeatMap(ref dataGridHeatMap);
        }

        private void comboBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            textBox.Text = "";
            cb = sender as ComboBox;
            person = HRISController.readData(cb.SelectedValue.ToString());
            orderedPerson = new List<string>();
            foreach (var p in person)
            {
                string str = string.Format("{0} {1}({2})", p.givenName, p.familyName, p.title);
                orderedPerson.Add(str);
            }
            //orderedPerson.Sort();
            listBox.Items.Clear();
            
            foreach (var p in orderedPerson)
            {
                listBox.Items.Add(p);
            }
        }

        private void listBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Update the staff detail
            ListBox lb = sender as ListBox;
            int sIndex = lb.SelectedIndex;
            if (sIndex != -1) {
                Person p = person[sIndex];
                labelName.Content = string.Format("{0} {1} {2}", p.title, p.givenName, p.familyName);
                labelCampus.Content = p.campus;
                labelPhone.Content = p.phone;
                labelRoom.Content = p.room;
                labelEmail.Content = p.email;
                textBoxConsultation.Text = p.consultation;
                textBoxTeachingTime.Text = p.teachingTime;


                //image view
                BitmapImage bi = new BitmapImage();
                bi.BeginInit();
                bi.UriSource = new Uri(p.photo, UriKind.RelativeOrAbsolute);
                bi.EndInit();
                image.Source = bi;

                //hide the gridView
                dataGrid.Visibility = Visibility.Hidden;
                buttonActivityGrid.Content = "Activity Grid";
            }
        }

        private void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            //retrieve data with filter characters.
            person = HRISController.readData(comboBox1.SelectedValue.ToString());
            orderedPerson = new List<string>();

            //filter data
            TextBox textBox = sender as TextBox;
            string pattern = textBox.Text;
            for (int i = 0; i < person.Count; i++)
            {
                if (Regex.IsMatch(person[i].familyName, pattern) || Regex.IsMatch(person[i].givenName, pattern, RegexOptions.IgnoreCase))
                {
                    string str = string.Format("{0} {1}({2})", person[i].givenName, person[i].familyName, person[i].title);
                    orderedPerson.Add(str);
                }
                else
                {
                    person.RemoveAt(i);
                    i--;
                }
            }

            //update listbox
            listBox.Items.Clear();
            foreach (var p in orderedPerson)
            {
                listBox.Items.Add(p);
            }
        }

        private void buttonActivityGrid_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;

            if (btn.Content.Equals("Activity Grid"))
            {
                
                dataGrid.Visibility = Visibility.Visible;
                buttonActivityGrid.Content = "Close Activity Grid";
                //dataGrid.ClearSelection();
                int selectedIndex = listBox.SelectedIndex; //get the currently selected person from list box
                HRISController.showGridView(person[selectedIndex].familyName, person[selectedIndex].givenName, ref dataGrid); //pass value to controller
            }
            else {
                dataGrid.Visibility = Visibility.Hidden;
                buttonActivityGrid.Content = "Activity Grid";
            }
        }

        private void UnitTab_Loaded(object sender, RoutedEventArgs e)
        {
           
        }

        private void listBoxUnitList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox lb = sender as ListBox;

            //show class table for selected unit with all campus
            HRISController.showClassTable(unit[lb.SelectedIndex].code, ref dataGridUnit, ref comboBoxCampus, "All");

            //show the label campus, buttoncrashmap and combox campus
            label10.Visibility = Visibility.Visible;
            comboBoxCampus.Visibility = Visibility.Visible;
            buttonClashMap.Visibility = Visibility.Visible;

            //update the crashmap view
            string unitCode = unit[listBoxUnitList.SelectedIndex].code;
            string campus = comboBoxCampus.SelectedValue.ToString();
            HRISController.updateCrashMap(ref dataGridCrashMap, unitCode, campus);

        }

        private void comboBoxCampus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cb = sender as ComboBox;
            if (cb.Items.Count > 1) {
                //update the class table
                string selectedUnitCode = unit[listBoxUnitList.SelectedIndex].code;
                HRISController.showClassTable(selectedUnitCode, ref dataGridUnit, ref comboBoxCampus, cb.SelectedValue.ToString());

                //update the crashmap view
                string unitCode = unit[listBoxUnitList.SelectedIndex].code;
                string campus = comboBoxCampus.SelectedValue.ToString();
                HRISController.updateCrashMap(ref dataGridCrashMap, unitCode, campus);
            }
        }

        private void radioButtonAll_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton rb = sender as RadioButton;
            string UnitOrConsul = rb.Content.ToString();
            string campus = comboBoxHeatMapCampus.SelectedValue.ToString();

            HRISController.UpdateHeatMap(ref dataGridHeatMap, UnitOrConsul, campus);
        }

        private void radioButtonUnitClass_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton rb = sender as RadioButton;
            string UnitOrConsul = rb.Content.ToString();
            string campus = comboBoxHeatMapCampus.SelectedValue.ToString();
            HRISController.UpdateHeatMap(ref dataGridHeatMap, UnitOrConsul, campus);
        }

        private void radioButtonConsultation_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton rb = sender as RadioButton;
            string UnitOrConsul = rb.Content.ToString();
            string campus = comboBoxHeatMapCampus.SelectedValue.ToString();
            HRISController.UpdateHeatMap(ref dataGridHeatMap, UnitOrConsul, campus);
        }

        private void dataGridHeatMap_Loaded(object sender, RoutedEventArgs e)
        {
            
            string campus = comboBoxHeatMapCampus.SelectedValue.ToString();
            string UnitOrConsul;
            if (radioButtonAll.IsChecked == true) {
                UnitOrConsul = radioButtonAll.Content.ToString();
            }
            else if (radioButtonConsultation.IsChecked == true)
            {
                UnitOrConsul = radioButtonConsultation.Content.ToString();
            }
            else {
                UnitOrConsul = radioButtonUnitClass.Content.ToString();
            }
            HRISController.UpdateHeatMap(ref dataGridHeatMap, UnitOrConsul, campus);
            radioButtonAll.IsChecked = true;
        }

        private void comboBoxHeatMapCampus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string campus = comboBoxHeatMapCampus.SelectedValue.ToString();
            string UnitOrConsul = "";
            if (radioButtonAll.IsChecked == true || radioButtonUnitClass.IsChecked == true || radioButtonConsultation.IsChecked ==true) {
                if (radioButtonAll.IsChecked == true)
                {
                    UnitOrConsul = radioButtonAll.Content.ToString();
                }
                else if (radioButtonUnitClass.IsChecked == true)
                {
                    UnitOrConsul = radioButtonUnitClass.Content.ToString();
                }
                else if (radioButtonConsultation.IsChecked == true)
                {
                    UnitOrConsul = radioButtonConsultation.Content.ToString();
                }
                HRISController.UpdateHeatMap(ref dataGridHeatMap, UnitOrConsul, campus);
            }

        }

        private void buttonClashMap_Click(object sender, RoutedEventArgs e)
        {
            //get the value of unitcode and campus which will be used when searching data from database
            string unitCode = unit[listBoxUnitList.SelectedIndex].code;
            string campus = comboBoxCampus.SelectedValue.ToString();

            if (buttonClashMap.Content.Equals("Generates ClashMap")) {
                HRISController.updateCrashMap(ref dataGridCrashMap, unitCode, campus);
                dataGridCrashMap.Visibility = Visibility.Visible;
                buttonClashMap.Content = "Close Crash Map";
            }
            else {
                dataGridCrashMap.Visibility = Visibility.Hidden;
                buttonClashMap.Content = "Generates ClashMap";
            }
        }

        private void dataGridCrashMap_Loaded(object sender, RoutedEventArgs e)
        {
            HRISController.showCrashMap(ref dataGridCrashMap);
        }

        /*private void TabItem_MouseUp(object sender, MouseButtonEventArgs e)
        {
            RadioButton rb = sender as RadioButton;
            string UnitOrConsul = rb.Content.ToString();
            string campus = comboBoxHeatMapCampus.SelectedValue.ToString();
            HRISController.UpdateHeatMap(ref dataGridHeatMap, UnitOrConsul, campus);
        }*/
    }
}
