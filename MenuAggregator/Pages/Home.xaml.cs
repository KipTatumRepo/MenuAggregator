using MenuAggregator.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

        static WeekChooser WkObject = MainWindow.Wk;
        PeriodChooser PkObject = MainWindow.Pk;
        public string isWeekly;
        
        
        int k = 0;
        int l = 0;
        
        
        string itemToAdd;
        int isChanged;
        
        int insertCount;
        string selectedConceptName;
        string cafe;
       
        int? BID;
       

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
        List<int?> listIsChanged = new List<int?>();
        List<int> returnList = new List<int>();
        Dictionary<string, string> dictionaryMenuItemToAdd = new Dictionary<string, string>();
        Dictionary<string, string> dictionaryPriceItemToAdd = new Dictionary<string, string>();
        Dictionary<string, string> dictionaryTextItemToAdd = new Dictionary<string, string>();
        Dictionary<string, int> dictionaryListIsChanged = new Dictionary<string, int>();
        ComboBox menucbObject = new ComboBox();
        ComboBox pricecbObject = new ComboBox();
        TextBox textObject = new TextBox();

        List<GroupBox> gbList = new List<GroupBox>();
        TextBlock tb = new TextBlock();

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
               
                //add Period and Week Chooser to top of the page
                string space = "             ";
                Separator sep = new Separator();
               
                tlbFlash.Items.Add(PkObject);
                tlbFlash.Items.Add(space);
                tlbFlash.Items.Add(sep);
                tlbFlash.Items.Add(space);
                tlbFlash.Items.Add(WkObject);
                PkObject.SelectAllEnabled = true;

            }

            catch (Exception ex)
            {
                MessageBox.Show("Something went wrong while trying to load the page " + ex);
            }

        }
       
        #region Custom Methods

        //Build groupbox fill with 2 comboboxes; 1 for menu items and 1 for price.  Also fill with on textbox for notes.  bid parameter is for
        //determining how many groupboxes we need, 1 for weekly menu, 5 for daily menuu
        private NewGroupBox BuildGroupBox(string day, /*List<string> item, List<string> price, string notes, */int? bid, int j, string conceptName)
        {
            int i = 0;
            int gridRowCount;
            string concept = conceptName;
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
            
            MenuBuilderDataSetTableAdapters.MenuBuilder_PriceTableAdapter priceAdapter = new MenuBuilderDataSetTableAdapters.MenuBuilder_PriceTableAdapter();
            MenuBuilderDataSetTableAdapters.MenuBuilder_SubMenusTableAdapter subMenuAdapter = new MenuBuilderDataSetTableAdapters.MenuBuilder_SubMenusTableAdapter();
            MenuBuilderDataSetTableAdapters.MenuBuilder_WeeklyMenusTableAdapter weeklyMenuAdapter = new MenuBuilderDataSetTableAdapters.MenuBuilder_WeeklyMenusTableAdapter();
            MenuBuilderDataSetTableAdapters.MenuBuilder_ConceptsTableAdapter conceptsAdapter = new MenuBuilderDataSetTableAdapters.MenuBuilder_ConceptsTableAdapter();

            //we need to get last period and week to set text in comboboxes and textboxes 
            MenuBuilderDataSet._MenuBuilder_WeeklyMenusDataTable periodWeekTable = new MenuBuilderDataSet._MenuBuilder_WeeklyMenusDataTable();
            weeklyMenuAdapter.GetLastWeek(periodWeekTable, selectedConceptName, cafe);

            if (periodWeekTable.Rows.Count >= 1)
            {
                fillCBPeriod = PkObject.CurrentPeriod;
                fillCBWeek = WkObject.CurrentWeek - 1;
            }

            //create a table and fill with all prices
            priceAdapter.Fill(ds._MenuBuilder_Price);
            DataTable priceTable = ds._MenuBuilder_Price as DataTable;

            //create a table that only fills with menu items associated with the button pressed
            DataTable subMenuTable = ds._MenuBuilder_SubMenus as DataTable;
            subMenuAdapter.FillByConceptId(ds._MenuBuilder_SubMenus, bid);

            //since we got the previous week and period above we create a table that fills with items from the previous week
            MenuBuilderDataSet._MenuBuilder_WeeklyMenusDataTable table = new MenuBuilderDataSet._MenuBuilder_WeeklyMenusDataTable();
            MenuBuilderDataSet._MenuBuilder_WeeklyMenusDataTable compareTable = new MenuBuilderDataSet._MenuBuilder_WeeklyMenusDataTable();
            weeklyMenuAdapter.FillComboBox(table, conceptName, fillCBPeriod, fillCBWeek, cafe);
            weeklyMenuAdapter.FillComboBoxCompare(compareTable, conceptName, fillCBPeriod, WkObject.CurrentWeek/*Wk.CurrentWeek*/, cafe);
            

            //Look at the conepts table to determine if the the price of this concept is editable by the cafe
            MenuBuilderDataSet._MenuBuilder_ConceptsDataTable cTable = new MenuBuilderDataSet._MenuBuilder_ConceptsDataTable();
            conceptsAdapter.GetPriceEditable(cTable, conceptName);

            DataRow cTableRow = cTable.Rows[0];

            string priceEditable = cTableRow["priceEditable"].ToString();
           
            //if there is no data in the weekly menus table associated with the concept
            if (table.Rows.Count == 0)
            {
                setMenuCBDispalyList.Add("Select Menu");
                setPriceCBDisplayList.Add("Price");
                setNotesTBDisplayList.Add("");
            }
            else
            {
                if (compareTable.Count < 1)
                {
                    foreach (DataRow rows in table)
                    {
                        setMenuCBDispalyList.Add(rows["menuItem"].ToString());
                        setPriceCBDisplayList.Add(rows["Price"].ToString());
                        setNotesTBDisplayList.Add(rows["Notes"].ToString());
                    }
                }
                else
                {
                    foreach(DataRow rows in compareTable)
                    {
                        setMenuCBDispalyList.Add(rows["menuItem"].ToString());
                        setPriceCBDisplayList.Add(rows["Price"].ToString());
                        setNotesTBDisplayList.Add(rows["Notes"].ToString());
                    }
                }
            }

            NewGroupBox box = new NewGroupBox(); //change back to GroupBox
            box.BorderBrush = Brushes.Black;
            box.BorderThickness = new Thickness(2);
            box.Margin = new Thickness(0, 0, 3, 6);
            box.FontSize = 24;
            box.Height = 98;
            box.IsChanged = 0;
            Grid grid = new Grid();
            ColumnDefinition column1 = new ColumnDefinition();
            column1.Width = new GridLength(235);
            ColumnDefinition column2 = new ColumnDefinition();
            column2.Width = new GridLength(95);
            ColumnDefinition column3 = new ColumnDefinition();

            grid.ColumnDefinitions.Add(column1);
            grid.ColumnDefinitions.Add(column2);
            grid.ColumnDefinitions.Add(column3);

            int gridRow = 0;
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
                    

                    if (table.Rows.Count == 0)
                    {
                        for (int z = 0; z <= 1; z++)
                        {
                            setMenuCBDispalyList.Add("Select Menu");
                            setPriceCBDisplayList.Add("Price");
                            setNotesTBDisplayList.Add("");
                        }
                    }
                }
                else if (gridRowCount == 2)
                {
                    box.Height = 130;
                    RowDefinition row1 = new RowDefinition();
                    row1.Height = new GridLength(40);

                    grid.RowDefinitions.Add(row1);
                   
                    setMenuCBDispalyList.Add("Select Menu");
                    setPriceCBDisplayList.Add("Price");
                    setNotesTBDisplayList.Add("");
                     
                }
                else
                {
                    box.Height = 80;
                    
                    if (table.Rows.Count == 0)
                    {
                        setMenuCBDispalyList.Add("Select Menu");
                        setPriceCBDisplayList.Add("Price");
                        setNotesTBDisplayList.Add("");
                    }
                }

                ComboBox menucb = new ComboBox();
                menucb.Width = 230;
                menucb.FontSize = 16;
                menucb.Height = 30;
                menucb.Tag = "MenuCb" + j + gridRow + 0;

                ComboBox pricecb = new ComboBox();
                pricecb.Width = 90;
                pricecb.FontSize = 16;
                pricecb.Height = 30;
                
                pricecb.Tag = "PriceCb" + j + gridRow + 1;

                TextBox text = new TextBox();
                text.FontSize = 16;
                text.Height = 30;
                text.Text = null;
                text.Tag = "TextBox" + j + gridRow + 2; //tag is set to iterator for accessing later

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

                //might need to uncomment this move this back to after addhandlers
                menucb.SelectedItem = setMenuCBDispalyList[l];
                pricecb.SelectedItem = setPriceCBDisplayList[l];

                menucb.Text = setMenuCBDispalyList[l];
                pricecb.Text = setPriceCBDisplayList[l];
                text.Text = setNotesTBDisplayList[l];
                
                dictionaryMenuItemToAdd.Add(menucb.Tag.ToString(), menucb.Text);
                dictionaryPriceItemToAdd.Add(pricecb.Tag.ToString(), pricecb.Text);

                dictionaryListIsChanged.Add(menucb.Tag.ToString(), 0);
                dictionaryListIsChanged.Add(pricecb.Tag.ToString(), 0);
                dictionaryListIsChanged.Add(text.Tag.ToString(), 0);

                //might get rid of this and change Grid.Set... back to menucb etc
                menucbObject = menucb;
                pricecbObject = pricecb;
                textObject = text;

                if (priceEditable == "0")
                {
                    column2.Width = new GridLength(0);
                    pricecb.Visibility = Visibility.Hidden;
                }


                //add menucb and price cb to grid created in groupbox 

                Grid.SetColumn(menucbObject, 0);
                Grid.SetRow(menucbObject, c);
                grid.Children.Add(menucbObject);

                Grid.SetColumn(pricecbObject, 1);
                Grid.SetRow(pricecbObject, c);
                grid.Children.Add(pricecbObject);
               
                Grid.SetColumn(textObject, 2);
                Grid.SetRow(textObject, c);
                grid.Children.Add(textObject);

                menucb.AddHandler(ComboBox.SelectionChangedEvent, new SelectionChangedEventHandler(menucb_SelectionChanged));
                pricecb.AddHandler(ComboBox.SelectionChangedEvent, new SelectionChangedEventHandler(pricecb_SelectionChanged));
                text.AddHandler(TextBox.LostKeyboardFocusEvent, new KeyboardFocusChangedEventHandler(text_LostFocus));
              
                k++;
                gridRow++;
                l++;
            }
            
            box.Header = day;

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

        private List<int> GetIsChangedFromDictionary(Dictionary<string, int> d, int dy)
        {
            int addIsChanged;
            int day = dy;
            int gridRow = 0;
            int counter = 0;
           
                for (int j = 0; j < insertCount; j++)
                {
                    returnList.Add(0);
                    string keyMenu = "MenuCb" + day + gridRow + 0;
                    string keyPrice = "PriceCb" + day + gridRow + 1;
                    string keyText = "TextBox" + day + gridRow + 2;

                    int menuValue = d[keyMenu];
                    int priceValue = d[keyPrice];
                    int textValue = d[keyText];
                   
                    if (menuValue == 1 || priceValue == 1 || textValue == 1)
                    {
                        addIsChanged = 1;
                    }
                    else
                    {
                        addIsChanged = 0;
                    }
                    returnList.RemoveAt(counter);
                    returnList.Insert(counter, addIsChanged);
                    counter++;
                    if (gridRow > 2)
                    {
                        gridRow = 0;
                    }
                    else
                    {
                        gridRow++;
                    }
                }
               
            return returnList;
        }

        //extract strings from supplied lists at position[i]
        private string getItem(List<string> list, int i)
        {
           
            string returnItem;
            if (list.Count == 0 || list.Count - 1 < i)
            {
                returnItem =  null;
            }
            else
            {
                if (list[i] == null) 
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

        private int? getIntItem(List<int?> list, int i)
        {

            int? returnItem;
            if (list.Count == 0 || list.Count - 1 < i)
            {
                returnItem = null;
            }
            else
            {
                if (list[i] == null)
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
            BID = b.Bid;
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
                            itemStackPanel.Children.Add(BuildGroupBox(day, b.Bid, j, selectedConceptName));
                            dayNames.Add(day);
                        }
                        else if (j == 1)
                        {
                            string day = "Tuesday";
                            itemStackPanel.Children.Add(BuildGroupBox(day, b.Bid, j, selectedConceptName));
                            dayNames.Add(day);
                        }
                        else if (j == 2)
                        {
                            string day = "Wednesday";
                            itemStackPanel.Children.Add(BuildGroupBox(day, b.Bid, j, selectedConceptName));
                            dayNames.Add(day);
                        }
                        else if (j == 3)
                        {
                            string day = "Thursday";
                            itemStackPanel.Children.Add(BuildGroupBox(day,  b.Bid, j, selectedConceptName));
                            dayNames.Add(day);
                        }
                        else if (j == 4)
                        {
                            string day = "Friday";
                            itemStackPanel.Children.Add(BuildGroupBox(day,  b.Bid, j, selectedConceptName));
                            dayNames.Add(day);
                        }
                    }
                }
                else
                {
                    string day = "Weekly";
                    itemStackPanel.Children.Add(BuildGroupBox(day,  b.Bid, 0, selectedConceptName));
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
            
            WkObject.PropertyChanged += new PropertyChangedEventHandler(WeekChanged);
            PkObject.PropertyChanged += new PropertyChangedEventHandler(PeriodChanged);
        }

        //when a selection from the menu box is made the value is put into a dictionary(where the name of the combobox is the key and the selected value is the value) so that if the selection is changed(either right away or later)
        //the correct value can be found and replaced
        private void menucb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cb = new ComboBox();
            string selectedCBTag;
            cb = e.OriginalSource as ComboBox;
            tb = e.OriginalSource as TextBlock;
            selectedCBTag = cb.Tag.ToString();

            if (cb.SelectedItem != null)
            {
                itemToAdd = cb.SelectedItem.ToString();
            }

            if (dictionaryMenuItemToAdd.Count < 1)
            {
                dictionaryMenuItemToAdd.Add(selectedCBTag, itemToAdd);
                dictionaryListIsChanged.Add(selectedCBTag, 1);
            }
            else 
            {
                if(dictionaryMenuItemToAdd.ContainsKey(selectedCBTag))
                {
                    dictionaryMenuItemToAdd.Remove(selectedCBTag);
                    dictionaryListIsChanged.Remove(selectedCBTag);
                    dictionaryMenuItemToAdd.Add(selectedCBTag, itemToAdd);
                    dictionaryListIsChanged.Add(selectedCBTag, 1);
                }
                else
                {
                    dictionaryMenuItemToAdd.Add(selectedCBTag, itemToAdd);
                    dictionaryListIsChanged.Add(selectedCBTag, 1);
                }
            }
        }

        private void pricecb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
            ComboBox cb = new ComboBox();
            string selectedCBTag;
           
            cb = e.OriginalSource as ComboBox;
            
            selectedCBTag = cb.Tag.ToString();

            if(cb.SelectedItem != null)
            { 
                itemToAdd = cb.SelectedItem.ToString();
            }

            if (dictionaryPriceItemToAdd.Count < 1)
            {
                dictionaryPriceItemToAdd.Add(selectedCBTag, itemToAdd);
                dictionaryListIsChanged.Add(selectedCBTag, 1);
            }
            else
            {
                if(dictionaryPriceItemToAdd.ContainsKey(selectedCBTag))
                {
                    dictionaryPriceItemToAdd.Remove(selectedCBTag);
                    dictionaryListIsChanged.Remove(selectedCBTag);
                    dictionaryPriceItemToAdd.Add(selectedCBTag, itemToAdd);
                    dictionaryListIsChanged.Add(selectedCBTag, 1);
                }
                else
                {
                    dictionaryPriceItemToAdd.Add(selectedCBTag, itemToAdd);
                    dictionaryListIsChanged.Add(selectedCBTag, 1);
                }
            }
        }

        private void text_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox Tb = new TextBox();
            string selectedTBTag;
            string text;
            Tb = e.OriginalSource as TextBox;

            selectedTBTag = Tb.Tag.ToString(); //get tag from textbox so we can get to the right place in the List
            text = Tb.Text;
            dictionaryTextItemToAdd[selectedTBTag] = text;
            dictionaryListIsChanged[selectedTBTag] = 1;
        }

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            l = 0;
            MenuBuilderDataSetTableAdapters.MenuBuilder_WeeklyMenusTableAdapter menuAdapter = new MenuBuilderDataSetTableAdapters.MenuBuilder_WeeklyMenusTableAdapter();
            
            conceptStackPanel.Visibility = Visibility.Visible;

            {
                List<string> LastWeekMenuList = new List<string>();
                List<int> isChangedLists = new List<int>();
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

                    int dayNameiterator = 0;
                    int addIterator = 0;
                    
                    for (int i = 0; i <= numberOfDayIterator; i++) 
                    {
                        for (int j = 0; j <= insertCount - 1; j++)
                        {
                            int u = 0;
                            isChangedLists = GetIsChangedFromDictionary(dictionaryListIsChanged, i);
                            menuAdapter.Insert(PkObject.CurrentPeriod, WkObject.CurrentWeek, dayNames[dayNameiterator], cafe, buttonNames[0], getItem(menuItemToAdd, addIterator), getItem(priceItemToAdd, addIterator), getItem(notesToAdd, addIterator), getItem(LastWeekMenuList, addIterator), isChangedLists[j], "Open", MainWindow.today, MainWindow.thisYear);
                            addIterator++;
                            u++;

                            if (addIterator % insertCount == 0)
                            {
                                dayNameiterator++;
                            }
                            isChanged = 0;
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
            BID = null;
            buttonNames.Clear();
            menuItemToAdd.Clear();
            priceItemToAdd.Clear();
            notesToAdd.Clear();
            menuInput.Clear();
            listIsChanged.Clear();
            returnList.Clear();
            dictionaryMenuItemToAdd.Clear();
            dictionaryPriceItemToAdd.Clear();
            dictionaryTextItemToAdd.Clear();
            dictionaryListIsChanged.Clear();
            itemStackPanel.Children.Clear();
            cancelButton.Visibility = Visibility.Hidden;
            
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
            BID = null;
            dayNames.Clear();
            buttonNames.Clear();
            menuItemToAdd.Clear();
            priceItemToAdd.Clear();
            notesToAdd.Clear();
            listIsChanged.Clear();
            menuInput.Clear();
            returnList.Clear();
            dictionaryMenuItemToAdd.Clear();
            dictionaryPriceItemToAdd.Clear();
            dictionaryTextItemToAdd.Clear();
            dictionaryListIsChanged.Clear();
            l = 0;
            conceptStackPanel.Visibility = Visibility.Visible;
            cancelButton.Visibility = Visibility.Hidden;
            menuInput.Text = "";
        }

        private void copyButton_Click(object sender, RoutedEventArgs e)
        {
            ComboBox cb = new ComboBox();
            
        }

        private void WeekChanged(object sender, PropertyChangedEventArgs e)
        {
            l = 0;
           
            dayNames.Clear();
            setMenuCBDispalyList.Clear();
            setNotesTBDisplayList.Clear();
            setPriceCBDisplayList.Clear();
            menuItemToAdd.Clear();
            priceItemToAdd.Clear();
            notesToAdd.Clear();
            listIsChanged.Clear();
            menuInput.Clear();
            returnList.Clear();
            itemStackPanel.Children.Clear();
            dictionaryMenuItemToAdd.Clear();
            dictionaryPriceItemToAdd.Clear();
            dictionaryTextItemToAdd.Clear();
            dictionaryListIsChanged.Clear();

            if (buttonNames.Count < 1)
            {
                return;
            }

            if (isWeekly == "1")
            {
                for (int j = 0; j <= 4; j++)
                {
                    if (j == 0)
                    {
                        string day = "Monday";
                        itemStackPanel.Children.Add(BuildGroupBox(day, BID, j, selectedConceptName));
                        dayNames.Add(day);
                    }
                    else if (j == 1)
                    {
                        string day = "Tuesday";
                        itemStackPanel.Children.Add(BuildGroupBox(day, BID, j, selectedConceptName));
                        dayNames.Add(day);
                    }
                    else if (j == 2)
                    {
                        string day = "Wednesday";
                        itemStackPanel.Children.Add(BuildGroupBox(day, BID, j, selectedConceptName));
                        dayNames.Add(day);
                    }
                    else if (j == 3)
                    {
                        string day = "Thursday";
                        itemStackPanel.Children.Add(BuildGroupBox(day, BID, j, selectedConceptName));
                        dayNames.Add(day);
                    }
                    else if (j == 4)
                    {
                        string day = "Friday";
                        itemStackPanel.Children.Add(BuildGroupBox(day, BID, j, selectedConceptName));
                        dayNames.Add(day);
                    }
                }
            }
            else
            {
                string day = "Weekly";
                itemStackPanel.Children.Add(BuildGroupBox(day, BID, 0, selectedConceptName));
                dayNames.Add(day);
            }
        }

        private void PeriodChanged(object sender, PropertyChangedEventArgs e)
        {
            l = 0;

            dayNames.Clear();
            setMenuCBDispalyList.Clear();
            setNotesTBDisplayList.Clear();
            setPriceCBDisplayList.Clear();
            //buttonNames.Clear();
            menuItemToAdd.Clear();
            priceItemToAdd.Clear();
            notesToAdd.Clear();
            listIsChanged.Clear();
            menuInput.Clear();
            returnList.Clear();
            itemStackPanel.Children.Clear();
            dictionaryMenuItemToAdd.Clear();
            dictionaryPriceItemToAdd.Clear();
            dictionaryTextItemToAdd.Clear();
            dictionaryListIsChanged.Clear();

            if (buttonNames.Count < 1)
            {
                return;
            }

            if (isWeekly == "1")
            {
                for (int j = 0; j <= 4; j++)
                {
                    if (j == 0)
                    {
                        string day = "Monday";
                        itemStackPanel.Children.Add(BuildGroupBox(day, /*items, price, notes, */ BID, j, selectedConceptName));
                        dayNames.Add(day);
                        //l++;
                    }
                    else if (j == 1)
                    {
                        string day = "Tuesday";
                        itemStackPanel.Children.Add(BuildGroupBox(day, /*items, price, notes, */ BID, j, selectedConceptName));
                        dayNames.Add(day);
                        //l++;
                    }
                    else if (j == 2)
                    {
                        string day = "Wednesday";
                        itemStackPanel.Children.Add(BuildGroupBox(day, /*items, price, notes, */ BID, j, selectedConceptName));
                        dayNames.Add(day);
                        //l++;
                    }
                    else if (j == 3)
                    {
                        string day = "Thursday";
                        itemStackPanel.Children.Add(BuildGroupBox(day, /*items, price, notes, */ BID, j, selectedConceptName));
                        dayNames.Add(day);
                        //l++;
                    }
                    else if (j == 4)
                    {
                        string day = "Friday";
                        itemStackPanel.Children.Add(BuildGroupBox(day, /*items, price, notes, */ BID, j, selectedConceptName));
                        dayNames.Add(day);
                        //l = 0;
                    }
                }
            }
            else
            {
                string day = "Weekly";
                itemStackPanel.Children.Add(BuildGroupBox(day, /*items, price, notes, */ BID, 0, selectedConceptName));
                dayNames.Add(day);
            }

        }

        private void backendButton_Click(object sender, RoutedEventArgs e)
        {
            tlbFlash.Items.Remove(PkObject);
            tlbFlash.Items.Remove(WkObject);
            NavigationService.Navigate(
                new Uri("Pages/BAckendHome.xaml", UriKind.Relative));
        }

        #endregion


    }
}
