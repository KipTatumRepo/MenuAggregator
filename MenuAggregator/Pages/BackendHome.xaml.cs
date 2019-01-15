using MenuAggregator.Classes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
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
        public static string NavigateFrom;
        static WeekChooser WkObjectBack = MainWindow.Wk;
        PeriodChooser PkObjectBack = MainWindow.Pk;
        int minWeek = 1;
        public static int mondayCount = 0;
        public string Cafe;
        string greenCafe;
        MenuBuilderDataSet ds = new MenuBuilderDataSet();
        BIDataSet biDs = new BIDataSet();
        BIDataSet.CostCentersDataTable table = new BIDataSet.CostCentersDataTable();
        MenuBuilderDataSet._MenuBuilder_WeeklyMenusDataTable thisWeeksMenus = new MenuBuilderDataSet._MenuBuilder_WeeklyMenusDataTable();
        List<string> builtCafes = new List<string>();
        List<TextBox> textBoxes = new List<TextBox>();
        TextBox tb;

        public BackendHome()
        {
            InitializeComponent();
            Tag = "BackendHome";

            MenuBuilderDataSetTableAdapters.MenuBuilder_WeeklyMenusTableAdapter weeklyMenuAdapter = new MenuBuilderDataSetTableAdapters.MenuBuilder_WeeklyMenusTableAdapter();
            MenuBuilderDataSetTableAdapters.MenuBuilder_BuiltCafesTableAdapter builtCafesTA = new MenuBuilderDataSetTableAdapters.MenuBuilder_BuiltCafesTableAdapter();
            BIDataSetTableAdapters.CostCentersTableAdapter adapter = new BIDataSetTableAdapters.CostCentersTableAdapter();

            string space = "             ";
            Separator sep = new Separator();

            tlbFlash.Items.Add(PkObjectBack);
            tlbFlash.Items.Add(space);
            tlbFlash.Items.Add(sep);
            tlbFlash.Items.Add(space);
            tlbFlash.Items.Add(WkObjectBack);
            PkObjectBack.SelectAllEnabled = true;

            weeklyMenuAdapter.MakeBackendButtons(ds._MenuBuilder_WeeklyMenus, MainWindow.currentPeriod, MainWindow.currentWeek);
            weeklyMenuAdapter.GetAllMenuThisWeek(thisWeeksMenus, MainWindow.currentPeriod, MainWindow.currentWeek);

            int i = 0;
            foreach (var row in ds._MenuBuilder_WeeklyMenus)
            {
                NewButton button = CreateButton(ds._MenuBuilder_WeeklyMenus, i);
                withChangesStackPanel.Children.Add(button);
                i++;
            }

            var buttonTemplate = new FrameworkElementFactory(typeof(Button));
            buttonTemplate.SetBinding(Button.ContentProperty, new Binding("isComplete"));
            buttonTemplate.AddHandler(Button.ClickEvent, new RoutedEventHandler(dataGridButton_Click));

            backEndDataGrid.Columns.Add(new DataGridTemplateColumn()
            {
                Header = "Updated",
                HeaderStyle = FindResource("CenterGridHeader") as Style,
                CellTemplate = new DataTemplate() { VisualTree = buttonTemplate }
                 
            });
            WkObjectBack.PropertyChanged += new PropertyChangedEventHandler(WeekChanged);
            PkObjectBack.PropertyChanged += new PropertyChangedEventHandler(PeriodChanged);
            
            adapter.CafeFillOnly(table);

            foreach (var cafe in table)
            {
                tb = new TextBox();
                tb.Width = 100;
                tb.TextAlignment = TextAlignment.Center;
                tb.FontSize = 14;
                tb.Tag = cafe[4];
                tb.Background = Brushes.Red;
                tb.Foreground = Brushes.White;
                builtCafes.Add(cafe[4].ToString());
                tb.Text = cafe[4].ToString();
                cafeBoxes.Children.Add(tb);
                textBoxes.Add(tb);

                foreach (var currentMenu in thisWeeksMenus) //ds._MenuBuilder_WeeklyMenus)
                {
                    string currentCafe = currentMenu[4].ToString();
                    if (tb.Tag.ToString() == currentCafe)
                    {
                        tb.Background = Brushes.Green;
                    }
                }
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

            b = e.OriginalSource as NewButton;
            Cafe = b.Content.ToString();

            cafeNameTextBox.Text = Cafe;
            greenCafe = Cafe;

            weeklyMenuAdapter.FillDataGrid(table, Cafe);

            backEndDataGrid.ItemsSource = table;
        }

        private void dataGridButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = new Button();
            button = e.OriginalSource as Button;

            var Day = backEndDataGrid.SelectedCells[1];
            var menuItem = backEndDataGrid.SelectedCells[3];
            
            var dayToUpdate = (Day.Column.GetCellContent(Day.Item) as TextBlock).Text;
            var menuItemToUpdate = (menuItem.Column.GetCellContent(menuItem.Item) as TextBlock).Text;
           
            MenuBuilderDataSetTableAdapters.MenuBuilder_WeeklyMenusTableAdapter updateRow = new MenuBuilderDataSetTableAdapters.MenuBuilder_WeeklyMenusTableAdapter();
            updateRow.UpdateIsChanged(PkObjectBack.CurrentPeriod, WkObjectBack.CurrentWeek, dayToUpdate, menuItemToUpdate);
            updateRow.UpdateIsComplete(PkObjectBack.CurrentPeriod, WkObjectBack.CurrentWeek, dayToUpdate, menuItemToUpdate);
            button.IsEnabled = false;
            button.Content = "Done";
        }

        public IEnumerable<DataGridRow> GetDataGridRows(DataGrid grid)
        {
            var itemsSource = grid.ItemsSource as IEnumerable;
            if (null == itemsSource) yield return null;
            foreach (var item in itemsSource)
            {
                var row = grid.ItemContainerGenerator.ContainerFromItem(item) as DataGridRow;
                if (null != row) yield return row;
            }
        }

        private void cafeButton_Click(object sender, RoutedEventArgs e)
        {
            tlbFlash.Items.Remove(PkObjectBack);
            tlbFlash.Items.Remove(WkObjectBack);

            NavigateFrom = Tag.ToString();
            NavigationService.Navigate(
                new Uri("Pages/Home.xaml", UriKind.Relative));
        }

        private void dbButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Psych This Button Doesn't Actually Do Anything Yet, but Will Allow DB Updates When Complete!!!!!!!");
        }

        private void exitButton_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void WeekChanged(object sender, PropertyChangedEventArgs e)
        {
            withChangesStackPanel.Children.Clear();
            
            MenuBuilderDataSet._MenuBuilder_WeeklyMenusDataTable table = new MenuBuilderDataSet._MenuBuilder_WeeklyMenusDataTable();
            MenuBuilderDataSetTableAdapters.MenuBuilder_WeeklyMenusTableAdapter weeklyMenuAdapter = new MenuBuilderDataSetTableAdapters.MenuBuilder_WeeklyMenusTableAdapter();
            weeklyMenuAdapter.FillDataGridByDate(table, Cafe, PkObjectBack.CurrentPeriod, WkObjectBack.CurrentWeek);
            weeklyMenuAdapter.MakeBackendButtons(ds._MenuBuilder_WeeklyMenus, PkObjectBack.CurrentPeriod, WkObjectBack.CurrentWeek);
            int i = 0;
            foreach (var row in ds._MenuBuilder_WeeklyMenus)
            {
                NewButton button = CreateButton(ds._MenuBuilder_WeeklyMenus, i);
                withChangesStackPanel.Children.Add(button);
                i++;
            }
            backEndDataGrid.ItemsSource = table;
        }

        private void PeriodChanged(object sender, PropertyChangedEventArgs e)
        {
            withChangesStackPanel.Children.Clear();

            MenuBuilderDataSet._MenuBuilder_WeeklyMenusDataTable table = new MenuBuilderDataSet._MenuBuilder_WeeklyMenusDataTable();
            MenuBuilderDataSetTableAdapters.MenuBuilder_WeeklyMenusTableAdapter weeklyMenuAdapter = new MenuBuilderDataSetTableAdapters.MenuBuilder_WeeklyMenusTableAdapter();
            weeklyMenuAdapter.FillDataGridByDate(table, Cafe, PkObjectBack.CurrentPeriod, WkObjectBack.CurrentWeek);
            weeklyMenuAdapter.MakeBackendButtons(ds._MenuBuilder_WeeklyMenus, PkObjectBack.CurrentPeriod, WkObjectBack.CurrentWeek);
            int i = 0;
            foreach (var row in ds._MenuBuilder_WeeklyMenus)
            {
                NewButton button = CreateButton(ds._MenuBuilder_WeeklyMenus, i);
                withChangesStackPanel.Children.Add(button);
                i++;
            }
            backEndDataGrid.ItemsSource = table;
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            MenuBuilderDataSet._MenuBuilder_WeeklyMenusDataTable table = new MenuBuilderDataSet._MenuBuilder_WeeklyMenusDataTable();
            MenuBuilderDataSetTableAdapters.MenuBuilder_WeeklyMenusTableAdapter weeklyMenuAdapter = new MenuBuilderDataSetTableAdapters.MenuBuilder_WeeklyMenusTableAdapter();
           
            int getBox = builtCafes.IndexOf(greenCafe);
            TextBox newBox = textBoxes[getBox];
            weeklyMenuAdapter.FillDataGrid(table, Cafe);

            if (table.Count != 0)
            {
                backEndDataGrid.ItemsSource = table;
            }
            else
            {
                withChangesStackPanel.Children.Clear();
                newBox.Background = Brushes.Red;
                textBoxes.RemoveAt(getBox);
                textBoxes.Insert(getBox, newBox);
                backEndDataGrid.ItemsSource = table;
                weeklyMenuAdapter.MakeBackendButtons(ds._MenuBuilder_WeeklyMenus, MainWindow.currentPeriod, MainWindow.currentWeek);
                int i = 0;
                foreach (var row in ds._MenuBuilder_WeeklyMenus)
                {
                    NewButton button = CreateButton(ds._MenuBuilder_WeeklyMenus, i);
                    withChangesStackPanel.Children.Add(button);
                    i++;
                }

            }
        }
    }
}
