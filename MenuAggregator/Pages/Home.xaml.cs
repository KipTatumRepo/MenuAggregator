using MenuAggregator.Classes;
using System;
using System.Collections.Generic;
using System.Data;
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
    /// Interaction logic for Home.xaml
    /// </summary>
    public partial class Home : Page
    {
       
        public static DateTime today = DateTime.Today;  
        public static DateTime firstOfMonth = new DateTime(today.Year, today.Month, 1);
        public static DateTime endOfMonth = new DateTime(today.Year,
                                           today.Month,
                                                        DateTime.DaysInMonth(today.Year,
                                                                 today.Month));

        int minWeek = 1;
        MenuBuilderDataSet ds = new MenuBuilderDataSet();
        DataTable dt = new DataTable();
        List<string> items = new List<string>();
        List<string> price = new List<string>();
        string notes;
        
        public Home()
        {

            InitializeComponent();

            //Fill Textbox at top of screen with Cafe name
            headerTextBox.Text = MainWindow.Cafe;

            CountMonday countMonday = new CountMonday();
            int mondayCount = 0;
            mondayCount = countMonday.CountMondays(firstOfMonth, endOfMonth);
            int currentPeriod = GetPeriod();

            int currentWeek = GetWeek();
            WeekChooser Wk = new WeekChooser(minWeek, mondayCount, currentWeek);
            PeriodChooser Pk = new PeriodChooser(Wk, 1, currentPeriod, currentPeriod);
            string space = "             ";
            
            Separator sep = new Separator();
            tlbFlash.Items.Add(Pk); 
            tlbFlash.Items.Add(space);
            tlbFlash.Items.Add(sep); 
            tlbFlash.Items.Add(space);
            tlbFlash.Items.Add(Wk); 
            Pk.SelectAllEnabled = false;

            #region Database Stuff
            MenuBuilderDataSetTableAdapters.MenuBuilder_ConceptsTableAdapter da = new MenuBuilderDataSetTableAdapters.MenuBuilder_ConceptsTableAdapter();
            MenuBuilderDataSetTableAdapters.MenuBuilder_ConceptsTableAdapter builtCafeAdapter = new MenuBuilderDataSetTableAdapters.MenuBuilder_ConceptsTableAdapter();
            MenuBuilderDataSet._MenuBuilder_ConceptsDataTable table = new MenuBuilderDataSet._MenuBuilder_ConceptsDataTable();

            da.Fill(ds._MenuBuilder_Concepts);
            
            builtCafeAdapter.FillByCafe(table, MainWindow.Cafe);
            #endregion

            int i = 0;

            //create NewButtons and add to left Stack Panel for cafe based on what concepts are used in that cafe, these concepts were selected by user first time they started program
            foreach ( DataRow row in table) 
            {
                NewButton button = CreateButton(table, i);     
                conceptStackPanel.Children.Add(button);
                i++;
            }
        }
        #region Custom Methods

        //Build groupbox fill with 2 comboboxes; 1 for menu items and 1 for price.  Also fill with on textbox for notes.  bid parameter is for
        //determining how many groupboxes we need, 1 for weekly menu, 5 for daily menuu
        private GroupBox BuildGroupBox(string day, List<string> item, List<string> price, string notes, int bid)
        {

            MenuBuilderDataSetTableAdapters.MenuBuilder_PriceTableAdapter priceAdapter = new MenuBuilderDataSetTableAdapters.MenuBuilder_PriceTableAdapter();
            MenuBuilderDataSetTableAdapters.MenuBuilder_SubMenusTableAdapter subMenuAdapter = new MenuBuilderDataSetTableAdapters.MenuBuilder_SubMenusTableAdapter();
            GroupBox box = new GroupBox();
            box.BorderBrush = Brushes.Black;
            box.BorderThickness = new Thickness(2);
            box.Margin = new Thickness(0, 0, 3, 6);
            box.FontSize = 24;
            box.Height = 98;
            Grid grid = new Grid();
            ColumnDefinition column1 = new ColumnDefinition();
            column1.Width = new GridLength(160);
            ColumnDefinition column2 = new ColumnDefinition();
            column2.Width = new GridLength(95);
            ColumnDefinition column3 = new ColumnDefinition();

            grid.ColumnDefinitions.Add(column1);
            grid.ColumnDefinitions.Add(column2);
            grid.ColumnDefinitions.Add(column3);

            ComboBox menucb = new ComboBox();
            menucb.Text = "Pick an Option";
            menucb.FontSize = 16;
            menucb.Width = 155;
            menucb.Height = 30;
            ComboBox pricecb = new ComboBox();
            pricecb.Width = 90;
            pricecb.FontSize = 16;
            pricecb.Height = 30;
            TextBox text = new TextBox();
            text.FontSize = 16;
            text.Height = 30;

            box.Header = day;

            //get menu's associated with selected concept from available concepts and fill menucb with those items
            #region fill menucb
            subMenuAdapter.FillByConceptId(ds._MenuBuilder_SubMenus, bid);

            DataTable subMenuTable = ds._MenuBuilder_SubMenus as DataTable;

            foreach (DataRow row in subMenuTable.Rows)
            {
                menucb.Items.Add(row[1]);
            }
            #endregion
            //get prices and fill pricecb with those items
            #region pricecb
            priceAdapter.Fill(ds._MenuBuilder_Price);

            DataTable priceTable = ds._MenuBuilder_Price as DataTable;

            foreach (DataRow row in priceTable.Rows)
            {
                pricecb.Items.Add(row[1]);
            }
            #endregion

            //add menucb and price cb to grid created in groupbox 
            grid.Children.Add(menucb);
            Grid.SetColumn(menucb, 0);

            grid.Children.Add(pricecb);
            Grid.SetColumn(pricecb, 1);

            grid.Children.Add(text);
            Grid.SetColumn(text, 2);

            //add grid to groupbox
            box.Content = grid;

            return box;
        }

        //creates buttons of custom type NewButton which as assignable bid.  Takes a MenuBuilder.ConceptsDataTable and counter as parameters
        private NewButton CreateButton(MenuBuilderDataSet._MenuBuilder_ConceptsDataTable ds, int i) 
        {
            string bid;
            NewButton button = new NewButton();
            Style style = FindResource("custButton") as Style;
            button.Name = "ConceptButton";
            bid = ds.Rows[i][0].ToString();
            button.Bid = Int32.Parse(bid);
            button.Tag = ds.Rows[i][2].ToString();
            button.Content = ds.Rows[i][1]; 
            button.Style = style;
            button.Margin = new Thickness(3, 0, 3, 6);
            button.AddHandler(NewButton.ClickEvent, new RoutedEventHandler(NewButton_Click));

            return button;
        }

        
        public int GetPeriod()
        {
            string sMonth = DateTime.Now.ToString("MM");
            int currentPeriod = Convert.ToInt32(sMonth);
            return currentPeriod;
        }

        public int GetWeek()
        {
            CountMonday monday = new CountMonday();
            int currentMonday = monday.CountMondays(firstOfMonth, today);
            
            return currentMonday;
        }
        #endregion

        #region Button Events
        private void NewButton_Click(object sender, RoutedEventArgs e)
        {
            NewButton b = new NewButton();
            b = e.OriginalSource as NewButton;
            itemStackPanel.Children.Clear();
            if (b != null)
            {
                if (b.Tag.ToString() == "1")
                {
                    for (int j = 0; j <= 4; j++)
                    {
                        if (j == 0)
                        {
                            string day = "Monday";
                            itemStackPanel.Children.Add(BuildGroupBox(day, items, price, notes, b.Bid));
                        }
                        else if (j == 1)
                        {
                            string day = "Tuesday";
                            itemStackPanel.Children.Add(BuildGroupBox(day, items, price, notes, b.Bid));
                        }
                        else if (j == 2)
                        {
                            string day = "Wednesday";
                            itemStackPanel.Children.Add(BuildGroupBox(day, items, price, notes, b.Bid));
                        }
                        else if (j == 3)
                        {
                            string day = "Thursday";
                            itemStackPanel.Children.Add(BuildGroupBox(day, items, price, notes, b.Bid));
                        }
                        else if (j == 4)
                        {
                            string day = "Friday";
                            itemStackPanel.Children.Add(BuildGroupBox(day, items, price, notes, b.Bid));
                        }
                    }
                }
                else
                {
                    string day = "Weekly";
                    itemStackPanel.Children.Add(BuildGroupBox(day, items, price, notes, b.Bid));
                }
            }
        }
        #endregion
    }
}
