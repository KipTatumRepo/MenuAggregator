using MenuAggregator.Classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
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
        List<string> menuItemToAdd = new List<string>();
        List<string> priceItemToAdd = new List<string>();
        List<string> notesToAdd = new List<string>();
        List<string> buttonNames = new List<string>();
        List<string> dayNames = new List<string>();
        public WeekChooser Wk;
        
        string itemToAdd;
        string dayToAdd;
        string conceptToAdd;
        int currentPeriod;
        int currentWeek;

        public Home()
        {
            
            InitializeComponent();

            #region Database Stuff
            MenuBuilderDataSetTableAdapters.MenuBuilder_ConceptsTableAdapter da = new MenuBuilderDataSetTableAdapters.MenuBuilder_ConceptsTableAdapter();
           
            MenuBuilderDataSetTableAdapters.MenuBuilder_UsersTableAdapter usersTableAdapter = new MenuBuilderDataSetTableAdapters.MenuBuilder_UsersTableAdapter();

           
            MenuBuilderDataSet._MenuBuilder_UsersDataTable userTable = new MenuBuilderDataSet._MenuBuilder_UsersDataTable();

            da.Fill(ds._MenuBuilder_Concepts);

           
            #endregion

            if (MainWindow.numberOfCafes == 1)
            {
                MenuBuilderDataSetTableAdapters.MenuBuilder_ConceptsTableAdapter builtCafeAdapter = new MenuBuilderDataSetTableAdapters.MenuBuilder_ConceptsTableAdapter();
                MenuBuilderDataSet._MenuBuilder_ConceptsDataTable table = new MenuBuilderDataSet._MenuBuilder_ConceptsDataTable();

                //Fill Textbox at top of screen with Cafe name
                headerTextBox.Text = "Cafe " + MainWindow.Cafe;
                builtCafeAdapter.FillByCafe(table, MainWindow.Cafe);

                //create NewButtons and add to left Stack Panel for cafe based on what concepts are used in that cafe, these concepts were selected by user first time they started program
                int i = 0;
                foreach (DataRow row in table)
                {
                    NewButton button = CreateButton(table, i);
                    conceptStackPanel.Children.Add(button);
                    i++;
                }
            }
            else
            {
                int j = 0;
                usersTableAdapter.UserHasMultipleCafes(userTable, MainWindow.UserName);

                multipleCafeCombobox.FontSize = 24;
                headerTextBox.Width = 690;
                headerTextBox.TextAlignment = TextAlignment.Center;
                headerTextBox.HorizontalAlignment = HorizontalAlignment.Left;
                headerTextBox.Text = "Select a Cafe ->";

                foreach (DataRow row in userTable)
                {
                    multipleCafeCombobox.Items.Add(row[2].ToString());
                    j++;
                }

                multipleCafeCombobox.Visibility = Visibility.Visible;
                multipleCafeCombobox.SelectedItem = -1;
            }

            CountMonday countMonday = new CountMonday();
            int mondayCount = 0;
            mondayCount = countMonday.CountMondays(firstOfMonth, endOfMonth);
            currentPeriod = GetPeriod();
            currentWeek = GetWeek();

            WeekChooser Wk = new WeekChooser(minWeek, mondayCount, currentWeek);
            
            PeriodChooser Pk = new PeriodChooser(Wk, 1, currentPeriod, currentPeriod);
            string space = "             ";

            //currentWeek = Wk.;
            currentPeriod = Pk.CurrentPeriod;
            
            Separator sep = new Separator();
            tlbFlash.Items.Add(Pk); 
            tlbFlash.Items.Add(space);
            tlbFlash.Items.Add(sep); 
            tlbFlash.Items.Add(space);
            tlbFlash.Items.Add(Wk); 
            Pk.SelectAllEnabled = true;

        }
        #region Custom Methods

        //Build groupbox fill with 2 comboboxes; 1 for menu items and 1 for price.  Also fill with on textbox for notes.  bid parameter is for
        //determining how many groupboxes we need, 1 for weekly menu, 5 for daily menuu
        private GroupBox BuildGroupBox(string day, List<string> item, List<string> price, string notes, int bid)
        {
            int i = 0;
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


            menucb.AddHandler(ComboBox.SelectionChangedEvent, new SelectionChangedEventHandler(menucb_SelectionChanged));
            pricecb.AddHandler(ComboBox.SelectionChangedEvent, new SelectionChangedEventHandler(pricecb_SelectionChanged));
            text.AddHandler(TextBox.LostKeyboardFocusEvent, new KeyboardFocusChangedEventHandler(text_LostFocus));

            //text.AddHandler(TextBox.MouseLeaveEvent, new MouseEventHandler(text_MouseLeave));

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
            i++;
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

        //extract strings from supplied lists
        private string getMenuItem(List<string> list, int i)
        {
            string returnItem;
            returnItem = list[i];
            return returnItem;
        }
        #endregion

        #region Button and Selection Events
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
                            dayNames.Add(day);
                        }
                        else if (j == 1)
                        {
                            string day = "Tuesday";
                            itemStackPanel.Children.Add(BuildGroupBox(day, items, price, notes, b.Bid));
                            dayNames.Add(day);
                        }
                        else if (j == 2)
                        {
                            string day = "Wednesday";
                            itemStackPanel.Children.Add(BuildGroupBox(day, items, price, notes, b.Bid));
                            dayNames.Add(day);
                        }
                        else if (j == 3)
                        {
                            string day = "Thursday";
                            itemStackPanel.Children.Add(BuildGroupBox(day, items, price, notes, b.Bid));
                            dayNames.Add(day);
                        }
                        else if (j == 4)
                        {
                            string day = "Friday";
                            itemStackPanel.Children.Add(BuildGroupBox(day, items, price, notes, b.Bid));
                            dayNames.Add(day);
                        }
                    }
                }
                else
                {
                    string day = "Weekly";
                    itemStackPanel.Children.Add(BuildGroupBox(day, items, price, notes, b.Bid));
                    dayNames.Add(day);
                }
                if (buttonNames.Count > 0)
                {
                    buttonNames.Clear();
                }
                buttonNames.Add(b.Content.ToString());
            }
        }

        private void menucb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cb = new ComboBox();
            cb = e.OriginalSource as ComboBox;
            itemToAdd = cb.SelectedItem.ToString();
            menuItemToAdd.Add(itemToAdd);
        }

        private void pricecb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cb = new ComboBox();
            cb = e.OriginalSource as ComboBox;
            itemToAdd = cb.SelectedItem.ToString();
            priceItemToAdd.Add(itemToAdd);
        }

        private void text_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = new TextBox();
            string text;
            tb = e.OriginalSource as TextBox;
            text = tb.Text;
            notesToAdd.Add(text);

        }

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            MenuBuilderDataSetTableAdapters.MenuBuilder_WeeklyMenusTableAdapter menuAdapter = new MenuBuilderDataSetTableAdapters.MenuBuilder_WeeklyMenusTableAdapter();
            if (itemStackPanel.Children.Count <= 1)
            {
                menuAdapter.Insert(currentPeriod, currentWeek, getMenuItem(dayNames, 0), MainWindow.Cafe, getMenuItem(buttonNames, 0), getMenuItem(menuItemToAdd, 0), getMenuItem(priceItemToAdd, 0), getMenuItem(notesToAdd, 0));
                
            }
            else
            {
                for (int i = 0; i <= 4; i++)
                {
                    menuAdapter.Insert(currentPeriod, currentWeek, getMenuItem(dayNames, i), MainWindow.Cafe, getMenuItem(buttonNames, 0), getMenuItem(menuItemToAdd, i), getMenuItem(priceItemToAdd, i), getMenuItem(notesToAdd, i));
                }
            }
            dayNames.Clear();
            buttonNames.Clear();
            menuItemToAdd.Clear();
            priceItemToAdd.Clear();
            notesToAdd.Clear();
        }

        private void multipleCafeCombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MenuBuilderDataSetTableAdapters.MenuBuilder_ConceptsTableAdapter builtCafeAdapter = new MenuBuilderDataSetTableAdapters.MenuBuilder_ConceptsTableAdapter();
            MenuBuilderDataSet._MenuBuilder_ConceptsDataTable table = new MenuBuilderDataSet._MenuBuilder_ConceptsDataTable();
            builtCafeAdapter.FillByCafe(table, multipleCafeCombobox.SelectedItem.ToString());
            headerTextBox.Text = "Cafe " + multipleCafeCombobox.SelectedItem.ToString();

            int i = 0;
            conceptStackPanel.Children.Clear();

            foreach (DataRow row in table)
            {
                NewButton button = CreateButton(table, i);
                conceptStackPanel.Children.Add(button);
                i++;
            }
        }
        #endregion
    }
}
