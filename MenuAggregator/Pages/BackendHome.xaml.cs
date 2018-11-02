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
        WeekChooser WkObject = new WeekChooser(0,0,0);
        PeriodChooser PkObject = new PeriodChooser(Wk,0,0,0);
        int currentPeriod;
        static int currentWeek;
        int minWeek = 1;
        int mondayCount = 0;
        int isChecked;
        MenuBuilderDataSet ds = new MenuBuilderDataSet();

        public BackendHome()
        {
            InitializeComponent();

            
            MenuBuilderDataSetTableAdapters.MenuBuilder_WeeklyMenusTableAdapter weeklyMenuAdapter = new MenuBuilderDataSetTableAdapters.MenuBuilder_WeeklyMenusTableAdapter();

            CountMonday countMonday = new CountMonday();

            mondayCount = countMonday.CountMondays(firstOfMonth, endOfMonth);

            currentPeriod = GetPeriod(today);

            if (currentWeek == 0)
            {
                currentWeek = Home.GetWeek();
                Wk = new WeekChooser(minWeek, mondayCount, currentWeek);
            }
            else
            {
                Wk = new WeekChooser(0, 5, 5);
            }
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

            //WkObject.CurrentWeek = Wk.CurrentWeek;
            //PkObject.CurrentPeriod = Pk.CurrentPeriod;

            weeklyMenuAdapter.MakeBackendButtons(ds._MenuBuilder_WeeklyMenus, currentPeriod, currentWeek);

            int i = 0;
            foreach (var row in ds._MenuBuilder_WeeklyMenus)
            {
                NewButton button = CreateButton(ds._MenuBuilder_WeeklyMenus, i);
                withChangesStackPanel.Children.Add(button);
                i++;
            }

           
        }

        private NewButton CreateButton(MenuBuilderDataSet._MenuBuilder_WeeklyMenusDataTable dt, int i)
        {
           
            string bid;
            NewButton button = new NewButton();
            Style style = FindResource("custButton") as Style;
            button.Name = "changedMenuButton";
            bid = dt.Rows[i][0].ToString();
            button.Bid = Int32.Parse(bid);
            button.Tag = dt.Rows[i][2].ToString();
            button.Content = dt.Rows[i][4];
            button.Style = style;
            button.Margin = new Thickness(3, 0, 3, 6);
            button.AddHandler(NewButton.ClickEvent, new RoutedEventHandler(ChangedMenuButton_Click));

            return button;
        }

        private void ChangedMenuButton_Click(object sender, RoutedEventArgs e)
        {
            NewButton b = new NewButton();
            MenuBuilderDataSet._MenuBuilder_WeeklyMenusDataTable table = new MenuBuilderDataSet._MenuBuilder_WeeklyMenusDataTable();
            MenuBuilderDataSetTableAdapters.MenuBuilder_WeeklyMenusTableAdapter weeklyMenuAdapter = new MenuBuilderDataSetTableAdapters.MenuBuilder_WeeklyMenusTableAdapter();
            string Cafe;
            b = e.OriginalSource as NewButton;
            Cafe = b.Content.ToString();

            cafeNameTextBox.Text = Cafe;

            weeklyMenuAdapter.FillDataGrid(table, Cafe);

            backEndDataGrid.ItemsSource = table;

        }

        public static int GetPeriod(DateTime today)
        {

            string dayOfWeek = today.DayOfWeek.ToString();
            int returnedPeriod = 0;
            int currentPeriod;
            string sMonth;
            if (dayOfWeek == "Monday" && today < firstOfMonth.AddDays(7))
            {

                sMonth = DateTime.Now.ToString("MM");
                currentPeriod = Convert.ToInt32(sMonth);
                returnedPeriod = currentPeriod;
                currentWeek = 0;
            }
            else
            {
                sMonth = DateTime.Today.AddMonths(-1).ToString("MM");
                currentPeriod = Convert.ToInt32(sMonth);
                returnedPeriod = currentPeriod;
                currentWeek = 5;
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
