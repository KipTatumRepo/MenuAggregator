using MenuAggregator.Classes;
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

namespace MenuAggregator.Pages
{
    /// <summary>
    /// Interaction logic for BackendHome.xaml
    /// </summary>
    public partial class BackendHome : Page
    {
        public static DateTime today = DateTime.Today;
        public static DateTime firstOfMonth = new DateTime(today.Year, today.Month, 1);
        public static DateTime endOfMonth = new DateTime(today.Year,
                                           today.Month,
                                                        DateTime.DaysInMonth(today.Year,
                                                                 today.Month));
        PeriodChooser Pk;
        static WeekChooser Wk;
        int currentPeriod;
        int currentWeek;
        int minWeek = 1;


        public BackendHome()
        {
            InitializeComponent();

            CountMonday countMonday = new CountMonday();
            int mondayCount = 0;
            mondayCount = countMonday.CountMondays(firstOfMonth, endOfMonth);
            currentPeriod = Home.GetPeriod();
            currentWeek = Home.GetWeek();

            Wk = new WeekChooser(minWeek, mondayCount, currentWeek);
            Pk = new PeriodChooser(Wk, 1, currentPeriod, currentPeriod);
            string space = "             ";

            currentWeek = Wk.CurrentWeek;
            currentPeriod = Pk.CurrentPeriod;

            Separator sep = new Separator();
            tlbFlash.Items.Add(Pk);
            tlbFlash.Items.Add(space);
            tlbFlash.Items.Add(sep);
            tlbFlash.Items.Add(space);
            tlbFlash.Items.Add(Wk);
            Pk.SelectAllEnabled = true;
        }
    }
}
