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
        static DateTime test = new DateTime(2019, 11, 12, 00, 00, 00);
        public static DateTime today = test; //DateTime.Today;
        public static DateTime firstOfMonth = new DateTime(today.Year, today.Month, 1);
        public static DateTime endOfMonth = new DateTime(today.Year,
                                           today.Month,
                                                        DateTime.DaysInMonth(today.Year,
                                                                 today.Month));
        PeriodChooser Pk;
        static WeekChooser Wk;
        WeekChooser WkObject = new WeekChooser(0, 0, 0);
        PeriodChooser PkObject = new PeriodChooser(Wk, 0,0,0);
        public string isWeekly;
        
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
        int insertCount;
        string selectedConceptName;
        string cafe;

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
        Dictionary<string, string> dictionaryMenuItemToAdd = new Dictionary<string, string>();
        Dictionary<string, string> dictionaryPriceItemToAdd = new Dictionary<string, string>();
        Dictionary<string, string> dictionaryTextItemToAdd = new Dictionary<string, string>();


        List<GroupBox> gbList = new List<GroupBox>();
        
        MenuBuilderDataSet ds = new MenuBuilderDataSet();
        DataTable dt = new DataTable();
        #endregion

        public Home()
        {
            
            InitializeComponent();

            cafe = MainWindow.Cafe;

            try
            {
                #region Database Stuff

                MenuBuilderDataSetTableAdapters.MenuBuilder_ConceptsTableAdapter da = new MenuBuilderDataSetTableAdapters.MenuBuilder_ConceptsTableAdapter();
                MenuBuilderDataSetTableAdapters.MenuBuilder_UsersTableAdapter usersTableAdapter = new MenuBuilderDataSetTableAdapters.MenuBuilder_UsersTableAdapter();

                MenuBuilderDataSet._MenuBuilder_UsersDataTable userTable = new MenuBuilderDataSet._MenuBuilder_UsersDataTable();

                da.Fill(ds._MenuBuilder_Concepts);
                #endregion

                
                if (MainWindow.numberOfCafes <= 1 && MainWindow.IsAdmin != 1)
                {
                    MenuBuilderDataSetTableAdapters.MenuBuilder_ConceptsTableAdapter builtCafeAdapter = new MenuBuilderDataSetTableAdapters.MenuBuilder_ConceptsTableAdapter();
                    MenuBuilderDataSet._MenuBuilder_ConceptsDataTable table = new MenuBuilderDataSet._MenuBuilder_ConceptsDataTable();

                    //Fill Textbox at top of screen with Cafe name
                    headerTextBox.Text = cafe;
                    builtCafeAdapter.FillByCafe(table, cafe);

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
                        backendButton.Visibility = Visibility.Visible;
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
            int gridRowCount;
            string concept = conceptName;
            string setMenuCBDispaly = "";
            string setPriceCBDisplay = "";
            string setNotesTBDisplay = "";
            string sFillCBPeriod;
            string sFillCBWeek;
            int fillCBPeriod = 0;
            int fillCBWeek = 0;

            if (concept == "Chefs Table")
            {
                gridRowCount = 3;
                
            }
            else if (concept == "Street Eats")
            {
                gridRowCount = 2;
            }
            else
            {
                gridRowCount = 1;
            }
            insertCount = gridRowCount;

            WkObject.CurrentWeek = Wk.CurrentWeek;
            PkObject.CurrentPeriod = Pk.CurrentPeriod;

            MenuBuilderDataSetTableAdapters.MenuBuilder_PriceTableAdapter priceAdapter = new MenuBuilderDataSetTableAdapters.MenuBuilder_PriceTableAdapter();
            MenuBuilderDataSetTableAdapters.MenuBuilder_SubMenusTableAdapter subMenuAdapter = new MenuBuilderDataSetTableAdapters.MenuBuilder_SubMenusTableAdapter();
            MenuBuilderDataSetTableAdapters.MenuBuilder_WeeklyMenusTableAdapter weeklyMenuAdapter = new MenuBuilderDataSetTableAdapters.MenuBuilder_WeeklyMenusTableAdapter();

            //we need to get last period and week to set text in comboboxes and textboxes 
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

            //create a table and fill with all prices
            priceAdapter.Fill(ds._MenuBuilder_Price);
            DataTable priceTable = ds._MenuBuilder_Price as DataTable;

            //create a table that only fills with menu items associated with the button pressed
            DataTable subMenuTable = ds._MenuBuilder_SubMenus as DataTable;
            subMenuAdapter.FillByConceptId(ds._MenuBuilder_SubMenus, bid);

            //since with got the previous week and period above we create a table that fills with items from the previous week
            MenuBuilderDataSet._MenuBuilder_WeeklyMenusDataTable table = new MenuBuilderDataSet._MenuBuilder_WeeklyMenusDataTable();
            weeklyMenuAdapter.FillComboBox(table, conceptName, fillCBPeriod, fillCBWeek);  //currentPeriod, currentWeek); //currentPeriod, currentWeek instead of 11, 1
           

            //if there is no data in the weekly menus table associated with the concept
            if (table.Rows.Count == 0)
            {
                setMenuCBDispalyList.Add("Select Menu");
                setPriceCBDisplayList.Add("Price");
                setNotesTBDisplayList.Add("");
            }
            else
            {
                foreach (DataRow rows in table)
                {
                    setMenuCBDispalyList.Add(rows["menuItem"].ToString());
                    setPriceCBDisplayList.Add(rows["Price"].ToString());
                    setNotesTBDisplayList.Add(rows["Notes"].ToString());
                }
            }

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

            int m = 0;
            for (int c = 0; c <= gridRowCount - 1; c++)
            {
                
                if (gridRowCount == 3)
                {
                    box.Height = 160;
                    RowDefinition row1 = new RowDefinition();
                    row1.Height = new GridLength(40);
                    RowDefinition row2 = new RowDefinition();
                    row2.Height = new GridLength(40);

                    grid.RowDefinitions.Add(row1);
                    grid.RowDefinitions.Add(row2);
                }
                else if (gridRowCount == 2)
                {
                    box.Height = 130;
                    RowDefinition row1 = new RowDefinition();
                    row1.Height = new GridLength(40);
                    grid.RowDefinitions.Add(row1);
                }
                
                
                ComboBox menucb = new ComboBox();
                menucb.FontSize = 16;
                menucb.Width = 230;
                menucb.Height = 30;
                menucb.Tag = "MenuCb" + j + m;


                ComboBox pricecb = new ComboBox();
                pricecb.Width = 90;
                pricecb.FontSize = 16;
                pricecb.Height = 30;
                pricecb.Tag = "PriceCb" + j + m;

                TextBox text = new TextBox();
                text.FontSize = 16;
                text.Height = 30;
                text.Text = null;
                text.Tag = "TextBox" + j + m; //tag is set to iterator for accessing later
                //notesToAdd.Add(text.Text); //text is set to null and added to notesToAdd for inserting to dB later
                dictionaryTextItemToAdd.Add(text.Tag.ToString(), text.Text);

                //add menu name from sub menu table to the combobox
                foreach (DataRow row in subMenuTable.Rows)
                {
                    menucb.Items.Add(row[1]);
                }

                //add price text from price table to combobox
                foreach (DataRow row in priceTable.Rows)
                {
                    pricecb.Items.Add(row[1]);
                }

                //set display text of combo and text boxes 
                menucb.Text = setMenuCBDispalyList[l]; //setMenuCBDispaly;
                pricecb.Text = setPriceCBDisplayList[l]; //setPriceCBDisplay;
                text.Text = setNotesTBDisplayList[l]; //setNotesTBDisplay;

                dictionaryMenuItemToAdd.Add(menucb.Tag.ToString(), menucb.Text);
                dictionaryPriceItemToAdd.Add(pricecb.Tag.ToString(), pricecb.Text);
               

                //add menucb and price cb to grid created in groupbox 
                Grid.SetColumn(menucb, 0);
                Grid.SetRow(menucb, c);
                grid.Children.Add(menucb);
                
                Grid.SetColumn(pricecb, 1);
                Grid.SetRow(pricecb, c);
                grid.Children.Add(pricecb);
               
                Grid.SetColumn(text, 2);
                Grid.SetRow(text, c);
                grid.Children.Add(text);

                menucb.SelectedItem = -1;
                pricecb.SelectedItem = -1;
               

                menucb.AddHandler(ComboBox.SelectionChangedEvent, new SelectionChangedEventHandler(menucb_SelectionChanged));
                pricecb.AddHandler(ComboBox.SelectionChangedEvent, new SelectionChangedEventHandler(pricecb_SelectionChanged));
                text.AddHandler(TextBox.LostKeyboardFocusEvent, new KeyboardFocusChangedEventHandler(text_LostFocus));

                menucb.SelectedItem = setMenuCBDispalyList[l];
                pricecb.SelectedItem = setPriceCBDisplayList[l];

                k++;
                m++;
                l++;
            }
            
            box.Header = day;

            //get menu's associated with selected concept from available concepts and fill menucb with those items
            #region fill menucb

            
            
            


            
            

           

            

            
           
            #endregion
            //get prices and fill pricecb with those items
            #region pricecb
            

            
            #endregion

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

        public static int GetPeriod(DateTime today)
        {

            string dayOfWeek = today.DayOfWeek.ToString();
            int returnedPeriod = 0;
            int currentPeriod;
            string sMonth;
            DateTime addSevenDays = firstOfMonth.AddDays(7);
            if (dayOfWeek == "Monday" && today > addSevenDays)
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

        //This will extract the values from the dictionary and put in a list for iterating through
        private List<string> GetDictionaryItem(Dictionary<string, string> d)
        {
            List<string> returnList = new List<string>(); 

            foreach (var item in d.Values)
            {
                returnList.Add(item);
            }
            return returnList;
        }

        //extract strings from supplied lists at position[i]
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
            isWeekly = b.Tag.ToString();

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
                            //l++;
                        }
                        else if (j == 1)
                        {
                            string day = "Tuesday";
                            itemStackPanel.Children.Add(BuildGroupBox(day, items, price, notes, b.Bid, j, selectedConceptName));
                            dayNames.Add(day);
                            //l++;
                        }
                        else if (j == 2)
                        {
                            string day = "Wednesday";
                            itemStackPanel.Children.Add(BuildGroupBox(day, items, price, notes, b.Bid, j, selectedConceptName));
                            dayNames.Add(day);
                            //l++;
                        }
                        else if (j == 3)
                        {
                            string day = "Thursday";
                            itemStackPanel.Children.Add(BuildGroupBox(day, items, price, notes, b.Bid, j, selectedConceptName));
                            dayNames.Add(day);
                            //l++;
                        }
                        else if (j == 4)
                        {
                            string day = "Friday";
                            itemStackPanel.Children.Add(BuildGroupBox(day, items, price, notes, b.Bid, j, selectedConceptName));
                            dayNames.Add(day);
                            //l = 0;
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

        //when a selection from the menu box is made the value is put into a dictionary(where the name of the combobox is the key and the selected value is the value) so that if the selection is changed(either right away or later)
        //the correct value can be found and replaced
        private void menucb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            isChanged = 1;
            ComboBox cb = new ComboBox();
            string selectedCBTag;
            cb = e.OriginalSource as ComboBox;
            selectedCBTag = cb.Tag.ToString();
            itemToAdd = cb.SelectedItem.ToString();

            if (dictionaryMenuItemToAdd.Count < 1)//menuItemToAdd.Count < 1
            {
                //menuItemToAdd.Add(itemToAdd);
                dictionaryMenuItemToAdd.Add(selectedCBTag, itemToAdd);
            }
            else 
            {
                //string lastInList = menuItemToAdd.Last();
                if(dictionaryMenuItemToAdd.ContainsKey(selectedCBTag))
                //if (selectedCBTag == heldCBTag && lastInList != itemToAdd)
                {
                    dictionaryMenuItemToAdd.Remove(selectedCBTag);
                    dictionaryMenuItemToAdd.Add(selectedCBTag, itemToAdd);
                }
                else
                {
                    //menuItemToAdd.Add(itemToAdd);
                    dictionaryMenuItemToAdd.Add(selectedCBTag, itemToAdd);
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

            if (dictionaryPriceItemToAdd.Count < 1)
            {
                //priceItemToAdd.Add(itemToAdd);
                dictionaryPriceItemToAdd.Add(selectedCBTag, itemToAdd);
                //priceHeldCBTag = cb.Tag.ToString();
               
            }
            else
            {
                //string lastInList = priceItemToAdd.Last();
                //if (selectedCBTag == priceHeldCBTag && lastInList != itemToAdd)
                if(dictionaryPriceItemToAdd.ContainsKey(selectedCBTag))
                {
                    //int lastIndex = priceItemToAdd.Count - 1;
                    //priceItemToAdd.RemoveAt(lastIndex);
                    //priceItemToAdd.Add(itemToAdd);
                    //priceHeldCBTag = cb.Tag.ToString();
                    dictionaryPriceItemToAdd.Remove(selectedCBTag);
                    dictionaryPriceItemToAdd.Add(selectedCBTag, itemToAdd);
                   
                }
                else
                {
                    //priceItemToAdd.Add(itemToAdd);
                    //priceHeldCBTag = cb.Tag.ToString();
                    dictionaryPriceItemToAdd.Add(selectedCBTag, itemToAdd);
                   
                   
                }
            }
        }

        private void text_LostFocus(object sender, RoutedEventArgs e)
        {

            isChanged = 1;
            TextBox tb = new TextBox();
            string selectedTBTag;
            string text;
            tb = e.OriginalSource as TextBox;
            selectedTBTag = tb.Tag.ToString(); //get tag from textbox so we can get to the right place in the List
            text = tb.Text;
            dictionaryTextItemToAdd[selectedTBTag] = text;
            //int i = Int32.Parse(tag); 
            //if (dictionaryTextItemToAdd.Count < 1)
            //{
            //    dictionaryTextItemToAdd.Add(selectedTBTag, text);
            //}
            //else
            //{
            //    if (dictionaryTextItemToAdd.ContainsKey(selectedTBTag))
            //    {
            //        dictionaryTextItemToAdd.Remove(selectedTBTag);
            //        dictionaryTextItemToAdd.Add(selectedTBTag, text);
            //    }
            //    else
            //    {
            //        dictionaryTextItemToAdd.Add(selectedTBTag, text);
            //    }
            //}


            //notesToAdd.RemoveAt(i); //remove anything that may be in the List at the position associated with this textbox day

            //notesToAdd.Insert(i, text); //add the updated text to that position

        }

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            MenuBuilderDataSetTableAdapters.MenuBuilder_WeeklyMenusTableAdapter menuAdapter = new MenuBuilderDataSetTableAdapters.MenuBuilder_WeeklyMenusTableAdapter();
            WkObject.CurrentWeek = Wk.CurrentWeek;
            PkObject.CurrentPeriod = Pk.CurrentPeriod;
            conceptStackPanel.Visibility = Visibility.Visible;

            //If there is only 1 item in itemStackPanel, the selected concept is a weekly menu so there is only 1 row to insert in DB
            /*if (itemStackPanel.Children.Count <= 1)
            {
                string LastWeekMenuItem = "";
                string LastWeekPriceItem = "";
                string LastWeekMenuItemAdd = "";
                menuItemToAdd = GetDictionaryItem(dictionaryMenuItemToAdd);
                priceItemToAdd = GetDictionaryItem(dictionaryPriceItemToAdd);
                try
                {
                    //need to get last weeks menu items to insert into current weeks row
                    menuAdapter.GetMenuItemForLastMenu(ds._MenuBuilder_WeeklyMenus, cafe, buttonNames[0]);
                    
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

                    menuAdapter.Insert(PkObject.CurrentPeriod, WkObject.CurrentWeek, dayNames[0], cafe, buttonNames[0], menuItemToAdd[0], priceItemToAdd[0], notesToAdd[0], today, LastWeekMenuItemAdd, isChanged, 0);
                    isChanged = 0;
                    MessageBox.Show(buttonNames[0] + " Has been updated");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Something went wrong " + ex);
                }
            }*/

            //If there is more than 1 item in itemStackPanel, the selected concept has daily menu's and we must insert Mon-Fri into the DB
            //else
            {
                List<string> LastWeekMenuList = new List<string>();
                int numberOfDayIterator = 0;
                if (isWeekly == "1")
                {
                    numberOfDayIterator = 4;
                }

                menuItemToAdd = GetDictionaryItem(dictionaryMenuItemToAdd);
                priceItemToAdd = GetDictionaryItem(dictionaryPriceItemToAdd);
                notesToAdd = GetDictionaryItem(dictionaryTextItemToAdd);
                try
                {
                    //need to get last weeks menu items to insert into current weeks row
                    menuAdapter.GetMenuItemForLastMenu(ds._MenuBuilder_WeeklyMenus, cafe, buttonNames[0]);

                    foreach (DataRow row in ds._MenuBuilder_WeeklyMenus.Rows)
                    {
                        LastWeekMenuList.Add(row[6].ToString());
                    }

                    int t = 0;
                    int dayNameiterator = 0;
                    int addIterator = 0;
                    for (int i = 0; i <= numberOfDayIterator; i++) //4
                    {
                        for (int j = 0; j <= insertCount - 1; j++)
                        {
                            menuAdapter.Insert(PkObject.CurrentPeriod, WkObject.CurrentWeek, dayNames[dayNameiterator], cafe, buttonNames[0], getItem(menuItemToAdd, addIterator), getItem(priceItemToAdd, addIterator), getItem(notesToAdd, addIterator), today, getItem(LastWeekMenuList, addIterator), isChanged, 0);
                            addIterator++;

                            if (addIterator % insertCount == 0)
                            {
                                dayNameiterator++;
                            }
                        }
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
            dictionaryMenuItemToAdd.Clear();
            dictionaryPriceItemToAdd.Clear();
            dictionaryTextItemToAdd.Clear();
            itemStackPanel.Children.Clear();
            l = 0;
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
                cafe = multipleCafeCombobox.SelectedItem.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show("There was a problem with selecting a cafe when there are multiple available " + ex);
            }
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            itemStackPanel.Children.Clear();
            dayNames.Clear();
            buttonNames.Clear();
            menuItemToAdd.Clear();
            priceItemToAdd.Clear();
            notesToAdd.Clear();
            dictionaryMenuItemToAdd.Clear();
            dictionaryPriceItemToAdd.Clear();
            dictionaryTextItemToAdd.Clear();
            l = 0;
            conceptStackPanel.Visibility = Visibility.Visible;
            cancelButton.Visibility = Visibility.Hidden;
        }

        #endregion

        private void backendButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(
                new Uri("Pages/BAckendHome.xaml", UriKind.Relative));
        }
    }
}
