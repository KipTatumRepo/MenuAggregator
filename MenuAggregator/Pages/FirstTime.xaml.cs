using System;
using System.Collections.Generic;
using System.Data;
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
    /// Interaction logic for FirstTime.xaml
    /// </summary>
    public partial class FirstTime : Page
    {
        private static string Cafe;
        private string User;
        private string UserName;
        int CafeBuilt;
        int AdminLevel;
        private CheckBox conceptCheckBox;
        private List<int> activeConcepts = new List<int>();
        BIDataSet ds = new BIDataSet();
        MenuBuilderDataSet ds1 = new MenuBuilderDataSet();

        public FirstTime()
        {
            int i = 0;

            User = MainWindow.UserName; 
            UserName = User;

            InitializeComponent();

            #region Database Stuff
            BIDataSetTableAdapters.CostCentersTableAdapter cafeAdapter = new BIDataSetTableAdapters.CostCentersTableAdapter();
            BIDataSet.CostCentersDataTable table = new BIDataSet.CostCentersDataTable();
            MenuBuilderDataSetTableAdapters.MenuBuilder_ConceptsTableAdapter conceptAdapter = new MenuBuilderDataSetTableAdapters.MenuBuilder_ConceptsTableAdapter();
            try
            {
                cafeAdapter.CafeFillOnly(table);
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            conceptAdapter.Fill(ds1._MenuBuilder_Concepts);
            #endregion

            //Greet User with user V-
            topRow.Text = "Hello " + User + " I see this is your first time using the program, please select your cafe from the options below";

            //build combobox with all available cafes from Operations.CostCenters in DB
            foreach (DataRow row in table) //ds.LOCATIONS)
            {
                cafeCombobox.Items.Add(row[4]);
            }
            
            //add checkboxes to screen after a cafe is selected
            foreach (DataRow row in ds1._MenuBuilder_Concepts)
            {
                conceptCheckBox = MakeCheckbox(ds1._MenuBuilder_Concepts, i);
                i++;
            }
        }

        #region Custom Methods
        //To make a checkbox, get all concepts from Concept Table and assign to Content
        //The built checkbox then gets added to a Wrap Panel as a child
        private CheckBox MakeCheckbox(MenuBuilderDataSet._MenuBuilder_ConceptsDataTable ds, int i)
        {
            CheckBox conceptCheckBox = new CheckBox();
            conceptCheckBox.Margin = new Thickness(0, 0, 25, 15);
            conceptCheckBox.FontSize = 36;
            conceptCheckBox.VerticalContentAlignment = VerticalAlignment.Center;
            conceptCheckBox.Content = ds.Rows[i][1].ToString();
            conceptCheckBox.Tag = ds.Rows[i][0].ToString();
            conceptWrapPanel.Children.Add(conceptCheckBox);
            conceptCheckBox.AddHandler(CheckBox.CheckedEvent, new RoutedEventHandler(conceptCheckBox_Checked));
            conceptCheckBox.AddHandler(CheckBox.UncheckedEvent, new RoutedEventHandler(conceptCheckBox_Unchecked));

            return conceptCheckBox;
        }
        #endregion

        #region Click and Selection Events

        //When a cafe is selected we need to see if it is already built, if it is a button prompting the user to add the user to the Users table will appear.
        //If the cafe is not built, checkboxes with concepts will appear to build the cafe. the cafe will be added to BuiltCafes table and the user will be added to the Users table.
        private void cafeCombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MenuBuilderDataSetTableAdapters.MenuBuilder_BuiltCafesTableAdapter builtCafeAdapter = new MenuBuilderDataSetTableAdapters.MenuBuilder_BuiltCafesTableAdapter();

            Cafe = cafeCombobox.SelectedItem.ToString();

            //This checks to see if cafe is built, if the returned int is > 0 there are rows in the DB and therefore the cafe exists
            CafeBuilt = builtCafeAdapter.SeeIfCafeExists(ds1._MenuBuilder_BuiltCafes, Cafe);

            if (CafeBuilt >= 1)
            {
                thirdRow.Visibility = Visibility.Visible;
                thirdRow.Text = "Your cafe is already built, click the button below to be added as a user";
                sendButton.Content = "Add Me";
                sendButton.Visibility = Visibility.Visible;
            }
            else if (cafeCombobox.SelectedItem != null)
            {
                thirdRow.Visibility = Visibility.Visible;
                thirdRow.Text = "Great, now please select the stations that are CURRENTLY available at " + Cafe;
                conceptWrapPanel.Visibility = Visibility.Visible;
            }
            //This make userId however, this might be useless
            UserName += Cafe;
        }

        //based on value of CafeBuilt either insert User to Users table and insert cafe and concepts in BuiltCafes table or
        //insert user into Users table
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MenuBuilderDataSetTableAdapters.MenuBuilder_UsersTableAdapter userAdapter = new MenuBuilderDataSetTableAdapters.MenuBuilder_UsersTableAdapter();
            MenuBuilderDataSetTableAdapters.MenuBuilder_BuiltCafesTableAdapter cafeAdapter = new MenuBuilderDataSetTableAdapters.MenuBuilder_BuiltCafesTableAdapter();

            if (User == "v-datatu" || User == "v-chluzi" || User == "v-brif" || User == "v-laive")
            {
                AdminLevel = 1;
            }
            
            if (CafeBuilt <= 0)
            {
                
                userAdapter.Insert(UserName, Cafe, User, AdminLevel);

                for (int j = 0; j <= activeConcepts.Count() - 1; j++)
                {
                    cafeAdapter.Insert(Cafe, activeConcepts[j]);
                }
            }

            else
            {
                userAdapter.Insert(UserName, Cafe, User, AdminLevel);
            }

            MainWindow.Cafe = Cafe;
            NavigationService.Navigate(
                new Uri("Pages/Home.xaml", UriKind.Relative));
        }

        //When a concept is checked add to activeConcepts List to be insert into DB
        private void conceptCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            string stationId;
            int StationId;
            CheckBox cb = new CheckBox();
            cb = e.OriginalSource as CheckBox;
            stationId = cb.Tag.ToString();
            StationId = Int32.Parse(stationId);
            activeConcepts.Add(StationId);
            sendButton.Content = "Set Up My Cafe";
            sendButton.Visibility = Visibility.Visible;
            
        }

        //If a concept is unchecked, it is removed from activeConcepts List
        private void conceptCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            string stationId;
            int StationId;
            CheckBox cb = new CheckBox();
            cb = e.OriginalSource as CheckBox;
            stationId = cb.Tag.ToString();
            StationId = Int32.Parse(stationId);

            for (int i = 0; i <= activeConcepts.Count() - 1; i++)
            {
                if (StationId == activeConcepts[i])
                {
                    activeConcepts.RemoveAt(i);
                    return;
                }
            }

        }
        #endregion
    }
}
