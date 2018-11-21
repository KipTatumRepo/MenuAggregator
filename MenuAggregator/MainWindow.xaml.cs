using MenuAggregator.Classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Mail;
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

namespace MenuAggregator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static PeriodChooser Pk;
        public static WeekChooser Wk;
        int minWeek = 1;
        public static string UserName = Environment.UserName;
        //static DateTime test = new DateTime(2018, 12, 3, 00, 00, 00);
        //public static DateTime today = test;
        public static DateTime today = DateTime.Now;
        public static int thisYear = today.Year;
        static DateTime firstOfMonth = new DateTime(today.Year, today.Month, 1);
        static DateTime endOfMonth = new DateTime(today.Year,
                                           today.Month,
                                                        DateTime.DaysInMonth(today.Year,
                                                                 today.Month));
        public static string Cafe;
        public static int numberOfCafes;
        public static int IsAdmin;
        public static int mondayCount = 0;
        public static int currentPeriod;
        public static int currentWeek;

        public MainWindow()
        {
            try
            { 
                MenuBuilderDataSet ds = new MenuBuilderDataSet();
                MenuBuilderDataSet._MenuBuilder_UsersDataTable table = new MenuBuilderDataSet._MenuBuilder_UsersDataTable();
                MenuBuilderDataSetTableAdapters.MenuBuilder_UsersTableAdapter userAdapter = new MenuBuilderDataSetTableAdapters.MenuBuilder_UsersTableAdapter();

                InitializeComponent();

                CountMonday countMonday = new CountMonday();

                mondayCount = countMonday.CountMondays(firstOfMonth, endOfMonth);

                currentPeriod = GetPeriod(today);

                if (currentWeek == 0)
                {
                    currentWeek = GetWeek();
                    Wk = new WeekChooser(minWeek, mondayCount, currentWeek);
                }
                else
                {
                    Wk = new WeekChooser(0, mondayCount, 5);
                }

                Pk = new PeriodChooser(Wk, 1, 12, currentPeriod);

                userAdapter.IsAuth(table, UserName);
                numberOfCafes = table.Count;

                if(table.Count >= 1)
                { 
                    string isAdmin = table.Rows[0][4].ToString();
                    IsAdmin = Int32.Parse(isAdmin);
                }

                if (IsAdmin == 1)
                {
                    mainFrame.Source = new Uri("pages\\BackendHome.xaml", UriKind.Relative);
                }

                else if ( numberOfCafes >= 1)
                {
                    mainFrame.Source = new Uri("pages\\Home.xaml", UriKind.Relative);
                    Cafe = table.Rows[0][2].ToString(); 
                }
                else
                {
                    mainFrame.Source = new Uri("pages\\FirstTime.xaml", UriKind.Relative);
                }
                
            }
            catch (Exception ex)
            {
                MessageBox.Show("There was a problem trying to load the first page: \n" + ex);
            }
        }

        public static int GetPeriod(DateTime today)
        {

            string dayOfWeek = today.DayOfWeek.ToString();
            int returnedPeriod = 0;
            int currentPeriod;
            string sMonth;
            DateTime addSevenDays = today.AddDays(7);
            if (dayOfWeek == "Monday" && today >= addSevenDays)
            {
                sMonth = DateTime.Today.AddMonths(-1).ToString("MM");
                currentPeriod = Convert.ToInt32(sMonth);
                returnedPeriod = currentPeriod;
                currentWeek = 5;
            }
            else
            {
                sMonth = DateTime.Now.ToString("MM");
                currentPeriod = Convert.ToInt32(sMonth);
                returnedPeriod = currentPeriod;
                currentWeek = 0;
            }

            return returnedPeriod;
        }

        public static int GetWeek()
        {
            CountMonday monday = new CountMonday();
            int currentMonday = monday.CountMondays(firstOfMonth, today);
            return currentMonday;
        }
    }
}
