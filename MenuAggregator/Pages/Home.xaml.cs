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
using System.Windows.Threading;

namespace MenuAggregator.Pages
{
    /// <summary>
    /// Interaction logic for Home.xaml
    /// </summary>
    public partial class Home : Page
    {
        #region Variable initialization
        //static DateTime test = new DateTime(2019, 1, 7, 00, 00, 00);
        public static DateTime today = DateTime.Today;
        public static DateTime firstOfMonth = new DateTime(today.Year, today.Month, 1);
        public static DateTime endOfMonth = new DateTime(today.Year,
                                           today.Month,
                                                        DateTime.DaysInMonth(today.Year,
                                                                 today.Month));
        PeriodChooser Pk;
        static WeekChooser Wk;
        WeekChooser WkObject = new WeekChooser(0, 0, 0);
        PeriodChooser PkObject = new PeriodChooser(Wk, 0,0,0);
        
        int minWeek = 1;
        int k = 0;
        int l = 0;
        int currentPeriod;
        static int currentWeek;
        string notes;
        string itemToAdd;
        string heldCBTag;
        string priceHeldCBTag;
        int isChanged = 0;
        int mondayCount = 0;
        string selectedConceptName;

        List<string> items = new List<string>();
        List<string> price = new List<string>();
        List<string> menuItemToAdd = new List<string>();
        List<string> priceItemToAdd = new List<string>();
        List<string> notesToAdd = new List<string>();
        List<string> buttonNames = new List<string>();
        List<string> dayNames = new List<string>();
        List<string> setMenuCBDispalyList = new List<string>();
        List<string> setPriceCBDisplayList = new List<string>();
        List<string> setNotesTBDisplayList = new List<string>();


        List<GroupBox> gbList = new List<GroupBox>();
        
        MenuBuilderDataSet ds = new MenuBuilderDataSet();
        DataTable dt = new DataTable();
        #endregion

        public Home()
        {
            
            InitializeComponent();

            try
            {
                #region Database Stuff

                MenuBuilderDataSetTableAdapters.MenuBuilder_ConceptsTableAdapter da = new MenuBuilderDataSetTableAdapters.MenuBuilder_ConceptsTableAdapter();
                MenuBuilderDataSetTableAdapters.MenuBuilder_UsersTableAdapter usersTableAdapter = new MenuBuilderDataSetTableAdapters.MenuBuilder_UsersTableAdapter();

                MenuBuilderDataSet._MenuBuilder_UsersDataTable userTable = new MenuBuilderDataSet._MenuBuilder_UsersDataTable();

                da.Fill(ds._MenuBuilder_Concepts);
                #endregion

                
                if (MainWindow.numberOfCafes <= 1)
                {
                    MenuBuilderDataSetTableAdapters.MenuBuilder_ConceptsTableAdapter builtCafeAdapter = new MenuBuilderDataSetTableAdapters.MenuBuilder_ConceptsTableAdapter();
                    MenuBuilderDataSet._MenuBuilder_ConceptsDataTable table = new MenuBuilderDataSet._MenuBuilder_ConceptsDataTable();

                    //Fill Textbox at top of screen with Cafe name
                    headerTextBox.Text = MainWindow.Cafe;
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

                #region Admin has navigated from BackendHome.xaml or if there is more than 1 cafe associated with the user. 
                else
                {
                    if (BackendHome.NavigateFrom == "BackendHome")
                    {
                        BIDataSet ds = new BIDataSet();
                        BIDataSetTableAdapters.LOCATIONSTableAdapter builtCafes = new BIDataSetTableAdapters.LOCATIONSTableAdapter();
                        BIDataSet.LOCATIONSDataTable cafeTable = new BIDataSet.LOCATIONSDataTable();
                        builtCafes.Fill(cafeTable);

                        multipleCafeCombobox.FontSize = 24;
                        headerTextBox.Width = 690;
                        headerTextBox.TextAlignment = TextAlignment.Center;
                        headerTextBox.HorizontalAlignment = HorizontalAlignment.Left;
                        headerTextBox.Text = "Select a Cafe ->";

                        foreach (DataRow row in cafeTable)
                        {
                            multipleCafeCombobox.Items.Add(row[2].ToString());
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
                    }

                    multipleCafeCombobox.Visibility = Visibility.Visible;
                    multipleCafeCombobox.SelectedItem = -1;
                    
                }
                #endregion

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
                    Wk = new WeekChooser(0, 5, 5);
                }
                
                Pk = new PeriodChooser(Wk, 1, 12, currentPeriod);
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

            catch (Exception ex)
            {
                MessageBox.Show("Something went wrong while trying to load the page " + ex);
            }

        }
        #region Custom Methods

        //Build groupbox fill with 2 comboboxes; 1 for menu items and 1 for price.  Also fill with on textbox for notes.  bid parameter is for
        //determining how many groupboxes we need, 1 for weekly menu, 5 for daily menuu
        private GroupBox BuildGroupBox(string day, List<string> item, List<string> price, string notes, int bid, int j, string conceptName)
        {
            int i = 0;
            string setMenuCBDispaly = "";
            string setPriceCBDisplay = "";
            string setNotesTBDisplay = "";
            string sFillCBPeriod;
            string sFillCBWeek;
            int fillCBPeriod = 0;
            int fillCBWeek = 0;


            WkObject.CurrentWeek = Wk.CurrentWeek;
            PkObject.CurrentPeriod = Pk.CurrentPeriod;

            MenuBuilderDataSetTableAdapters.MenuBuilder_PriceTableAdapter priceAdapter = new MenuBuilderDataSetTableAdapters.MenuBuilder_PriceTableAdapter();
            MenuBuilderDataSetTableAdapters.MenuBuilder_SubMenusTableAdapter subMenuAdapter = new MenuBuilderDataSetTableAdapters.MenuBuilder_SubMenusTableAdapter();
            MenuBuilderDataSetTableAdapters.MenuBuilder_WeeklyMenusTableAdapter weeklyMenuAdapter = new MenuBuilderDataSetTableAdapters.MenuBuilder_WeeklyMenusTableAdapter();
            GroupBox box = new GroupBox();
            box.BorderBrush = Brushes.Black;
            box.BorderThickness = new Thickness(2);
            box.Margin = new Thickness(0, 0, 3, 6);
            box.FontSize = 24;
            box.Height = 98;
            Grid grid = new Grid();
            ColumnDefinition column1 = new ColumnDefinition();
            column1.Width = new GridLength(235);
            ColumnDefinition column2 = new ColumnDefinition();
            column2.Width = new GridLength(95);
            ColumnDefinition column3 = new ColumnDefinition();

            grid.ColumnDefinitions.Add(column1);
            grid.ColumnDefinitions.Add(column2);
            grid.ColumnDefinitions.Add(column3);

            ComboBox menucb = new ComboBox();
            menucb.FontSize = 16;
            menucb.Width = 230;
            menucb.Height = 30;
            menucb.Tag = "MenuCb" + j;
            

            ComboBox pricecb = new ComboBox();
            pricecb.Width = 90;
            pricecb.FontSize = 16;
            pricecb.Height = 30;
            pricecb.Tag = "PriceCb" + j;

            TextBox text = new TextBox();
            text.FontSize = 16;
            text.Height = 30;
            text.Text = null;
            text.Tag = k; //tag is set to iterator for accessing later
            notesToAdd.Add(text.Text); //text is set to null and added to notesToAdd for inserting to dB later
            k++;
            
            box.Header = day;

            //get menu's associated with selected concept from available concepts and fill menucb with those items
            #region fill menucb

            subMenuAdapter.FillByConceptId(ds._MenuBuilder_SubMenus, bid);
            
            DataTable subMenuTable = ds._MenuBuilder_SubMenus as DataTable;
            MenuBuilderDataSet._MenuBuilder_WeeklyMenusDataTable table = new MenuBuilderDataSet._MenuBuilder_WeeklyMenusDataTable();
            MenuBuilderDataSet._MenuBuilder_WeeklyMenusDataTable periodWeekTable = new MenuBuilderDataSet._MenuBuilder_WeeklyMenusDataTable();


            weeklyMenuAdapter.GetLastWeek(periodWeekTable, selectedConceptName);
            if (periodWeekTable.Rows.Count >= 1)
            {
                DataRow rows = periodWeekTable.Rows[0];
                sFillCBPeriod = rows["Period"].ToString();
                sFillCBWeek = rows["Week"].ToString();
                fillCBPeriod = Int32.Parse(sFillCBPeriod);
                fillCBWeek = Int32.Parse(sFillCBWeek);
            }

            weeklyMenuAdapter.FillComboBox(table, conceptName, fillCBPeriod , fillCBWeek);  //currentPeriod, currentWeek); //currentPeriod, currentWeek instead of 11, 1
            

            if (table.Rows.Count >= 1)
            {
                foreach (DataRow rows in table)
                {
                    setMenuCBDispalyList.Add(rows["menuItem"].ToString());
                    setPriceCBDisplayList.Add(rows["Price"].ToString());
                    setNotesTBDisplayList.Add(rows["Notes"].ToString());
                }
            }

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

            menucb.Text = setMenuCBDispalyList[l]; //setMenuCBDispaly;
            pricecb.Text = setPriceCBDisplayList[l]; //setPriceCBDisplay;
            text.Text = setNotesTBDisplayList[l]; //setNotesTBDisplay;

            //add menucb and price cb to grid created in groupbox 
            grid.Children.Add(menucb);
            Grid.SetColumn(menucb, 0);

            grid.Children.Add(pricecb);
            Grid.SetColumn(pricecb, 1);

            grid.Children.Add(text);
            Grid.SetColumn(text, 2);

            //add grid to groupbox
            box.Content = grid;

            menucb.AddHandler(ComboBox.SelectionChangedEvent, new SelectionChangedEventHandler(menucb_SelectionChanged));
            pricecb.AddHandler(ComboBox.SelectionChangedEvent, new SelectionChangedEventHandler(pricecb_SelectionChanged));
            text.AddHandler(TextBox.LostKeyboardFocusEvent, new KeyboardFocusChangedEventHandler(text_LostFocus));

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

        //extract strings from supplied lists
        private string getItem(List<string> list, int i)
        {
            //this is messing up
            //need to get list.count
            string returnItem;
            if (list.Count == 0 || list.Count - 1 < i)
            {
                returnItem =  null;
            }
            else
            {
                if (list[i] == null) // this is returning index out of bounds
                {
                    returnItem = null;
                }
                else
                {
                    returnItem = list[i];
                }
               
            }
            return returnItem;
        }
        #endregion

        #region Button and Selection Events
        private void NewButton_Click(object sender, RoutedEventArgs e)
        {
            NewButton b = new NewButton();
            
            b = e.OriginalSource as NewButton;
            menuInput.Text = b.Content.ToString();
            selectedConceptName = b.Content.ToString();
            itemStackPanel.Children.Clear();
            conceptStackPanel.Visibility = Visibility.Collapsed;
            cancelButton.Visibility = Visibility.Visible;

            if (b != null)
            {
                if (b.Tag.ToString() == "1")
                {
                    for (int j = 0; j <= 4; j++)
                    {
                        if (j == 0)
                        {
                            string day = "Monday";
                            itemStackPanel.Children.Add(BuildGroupBox(day, items, price, notes, b.Bid, j, selectedConceptName));
                            /*var BuiltGroupBox = BuildGroupBox(day, items, price, notes, b.Bid);
                            itemStackPanel.Children.Add(BuiltGroupBox);
                            gbList.Add(BuiltGroupBox);*/
                            dayNames.Add(day);
                            l++;
                        }
                        else if (j == 1)
                        {
                            string day = "Tuesday";
                            itemStackPanel.Children.Add(BuildGroupBox(day, items, price, notes, b.Bid, j, selectedConceptName));
                            dayNames.Add(day);
                            l++;
                        }
                        else if (j == 2)
                        {
                            string day = "Wednesday";
                            itemStackPanel.Children.Add(BuildGroupBox(day, items, price, notes, b.Bid, j, selectedConceptName));
                            dayNames.Add(day);
                            l++;
                        }
                        else if (j == 3)
                        {
                            string day = "Thursday";
                            itemStackPanel.Children.Add(BuildGroupBox(day, items, price, notes, b.Bid, j, selectedConceptName));
                            dayNames.Add(day);
                            l++;
                        }
                        else if (j == 4)
                        {
                            string day = "Friday";
                            itemStackPanel.Children.Add(BuildGroupBox(day, items, price, notes, b.Bid, j, selectedConceptName));
                            dayNames.Add(day);
                            l = 0;
                        }
                    }
                }
                else
                {
                    string day = "Weekly";
                    itemStackPanel.Children.Add(BuildGroupBox(day, items, price, notes, b.Bid, 0, selectedConceptName));
                    dayNames.Add(day);
                }
                if (buttonNames.Count > 0)
                {
                    buttonNames.Clear();
                }
                buttonNames.Add(b.Content.ToString());
                
            }
            setMenuCBDispalyList.Clear();
            setNotesTBDisplayList.Clear();
            setPriceCBDisplayList.Clear();
            k = 0;
        }

        private void menucb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            isChanged = 1;
            ComboBox cb = new ComboBox();
            string selectedCBTag;
            cb = e.OriginalSource as ComboBox;
            selectedCBTag = cb.Tag.ToString();
            itemToAdd = cb.SelectedItem.ToString();

            if (menuItemToAdd.Count < 1)
            {
                menuItemToAdd.Add(itemToAdd);
                heldCBTag = cb.Tag.ToString();
                
            }
            else 
            {
                string lastInList = menuItemToAdd.Last();
                if (selectedCBTag == heldCBTag && lastInList != itemToAdd)
                {
                    int lastIndex = menuItemToAdd.Count - 1;
                    menuItemToAdd.RemoveAt(lastIndex);
                    menuItemToAdd.Add(itemToAdd);
                    heldCBTag = cb.Tag.ToString();
                }
                else
                {
                    menuItemToAdd.Add(itemToAdd);
                    heldCBTag = cb.Tag.ToString();
                }
            }
        }

        private void pricecb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            isChanged = 1;
            ComboBox cb = new ComboBox();
            string selectedCBTag;
            cb = e.OriginalSource as ComboBox;
            selectedCBTag = cb.Tag.ToString();
            itemToAdd = cb.SelectedItem.ToString();

            if (priceItemToAdd.Count < 1)
            {
                priceItemToAdd.Add(itemToAdd);
                priceHeldCBTag = cb.Tag.ToString();
               
            }
            else
            {
                string lastInList = priceItemToAdd.Last();
                if (selectedCBTag == priceHeldCBTag && lastInList != itemToAdd)
                {
                    int lastIndex = priceItemToAdd.Count - 1;
                    priceItemToAdd.RemoveAt(lastIndex);
                    priceItemToAdd.Add(itemToAdd);
                    priceHeldCBTag = cb.Tag.ToString();
                   
                }
                else
                {
                    priceItemToAdd.Add(itemToAdd);
                    priceHeldCBTag = cb.Tag.ToString();
                   
                }
            }
        }

        private void text_LostFocus(object sender, RoutedEventArgs e)
        {

            isChanged = 1;
            TextBox tb = new TextBox();
            string text;
            
            tb = e.OriginalSource as TextBox;
            string tag = tb.Tag.ToString(); //get tag from textbox so we can get to the right place in the List
            int i = Int32.Parse(tag); 
            notesToAdd.RemoveAt(i); //remove anything that may be in the List at the position associated with this textbox day
            text = tb.Text;
            notesToAdd.Insert(i, text); //add the updated text to that position
            
        }

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            MenuBuilderDataSetTableAdapters.MenuBuilder_WeeklyMenusTableAdapter menuAdapter = new MenuBuilderDataSetTableAdapters.MenuBuilder_WeeklyMenusTableAdapter();
            WkObject.CurrentWeek = Wk.CurrentWeek;
            PkObject.CurrentPeriod = Pk.CurrentPeriod;
            conceptStackPanel.Visibility = Visibility.Visible;

            //If there is only 1 item in itemStackPanel, the selected concept is a weekly menu so there is only 1 row to insert in DB
            if (itemStackPanel.Children.Count <= 1)
            {
                string LastWeekMenuItem = "";
                string LastWeekPriceItem = "";
                string LastWeekMenuItemAdd = "maybe Delete This Column";
                try
                {
                    //need to get last weeks menu items to insert into current weeks row
                    menuAdapter.GetMenuItemForLastMenu(ds._MenuBuilder_WeeklyMenus, MainWindow.Cafe, buttonNames[0]);

                    foreach (DataRow row in ds._MenuBuilder_WeeklyMenus.Rows)
                    {
                        LastWeekMenuItem = row["menuItem"].ToString(); //"Maybe Delete This Column";//row[6].ToString();
                        LastWeekPriceItem = row["Price"].ToString();
                    }

                    if (menuItemToAdd.Count <= 0)
                    {
                        menuItemToAdd.Add(LastWeekMenuItem);
                    }

                    if (priceItemToAdd.Count <= 0)
                    {
                        priceItemToAdd.Add(LastWeekPriceItem);
                    }

                    menuAdapter.Insert(PkObject.CurrentPeriod, WkObject.CurrentWeek, dayNames[0], MainWindow.Cafe, buttonNames[0], menuItemToAdd[0], priceItemToAdd[0], notesToAdd[0], today, LastWeekMenuItemAdd, isChanged, 0);
                    isChanged = 0;
                    MessageBox.Show(buttonNames[0] + " Has been updated");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Something went wrong " + ex);
                }
            }

            //If there is more than 1 item in itemStackPanel, the selected concept has daily menu's and we must insert Mon-Fri into the DB
            else
            {
                List<string> LastWeekMenuList = new List<string>();
                try
                {
                    //need to get last weeks menu items to insert into current weeks row
                    menuAdapter.GetMenuItemForLastMenu(ds._MenuBuilder_WeeklyMenus, MainWindow.Cafe, buttonNames[0]);

                    foreach (DataRow row in ds._MenuBuilder_WeeklyMenus.Rows)
                    {
                        LastWeekMenuList.Add("Maybe Delete This Column" /*row[6].ToString()*/);
                    }

                    for (int i = 0; i <= 4; i++)
                    {
                        menuAdapter.Insert(PkObject.CurrentPeriod, WkObject.CurrentWeek, dayNames[i], MainWindow.Cafe, buttonNames[0], getItem(menuItemToAdd, i), getItem(priceItemToAdd, i), getItem(notesToAdd, i), today, getItem(LastWeekMenuList, i), isChanged, 0);
                        
                    }
                    MessageBox.Show(buttonNames[0] + " Has been updated");
                    isChanged = 0;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Something went wrong " + ex);
                }
            }
            dayNames.Clear();
            buttonNames.Clear();
            menuItemToAdd.Clear();
            priceItemToAdd.Clear();
            notesToAdd.Clear();
            itemStackPanel.Children.Clear();
        }

        private void multipleCafeCombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
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
            catch (Exception ex)
            {
                MessageBox.Show("There was a problem with selecting a cafe when there are multiple available " + ex);
            }
            
           
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            itemStackPanel.Children.Clear();
            conceptStackPanel.Visibility = Visibility.Visible;
            cancelButton.Visibility = Visibility.Hidden;
        }
        #endregion


    }
}
