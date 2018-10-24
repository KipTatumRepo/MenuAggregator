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
        public static string Cafe;
        public static string User;
        public static string UserName;

        public FirstTime()
        {
            int i = 0;
            
            User = Environment.UserName;
            UserName = User;

            InitializeComponent();

            BIDataSet ds = new BIDataSet();
            MenuBuilderDataSet ds1 = new MenuBuilderDataSet();
            BIDataSetTableAdapters.MasterBuildingListTableAdapter cafeAdapter = new BIDataSetTableAdapters.MasterBuildingListTableAdapter();
            MenuBuilderDataSetTableAdapters.MenuBuilder_ConceptsTableAdapter conceptAdapter = new MenuBuilderDataSetTableAdapters.MenuBuilder_ConceptsTableAdapter();
            

            topRow.Text = "Hello " + User + " This is your first time using the program, please select your cafe from the options below";

            cafeAdapter.Fill(ds.MasterBuildingList);
            conceptAdapter.Fill(ds1._MenuBuilder_Concepts);

            foreach (DataRow row in ds.MasterBuildingList)
            {
                cafeCombobox.Items.Add(row[1]);
            }

            foreach (DataRow row in ds1._MenuBuilder_Concepts)
            {
                CheckBox conceptCheckBox = MakeCheckbox(ds1._MenuBuilder_Concepts, i);
                i++;
            }

           
        }

        private CheckBox MakeCheckbox(MenuBuilderDataSet._MenuBuilder_ConceptsDataTable ds, int i)
        {
            CheckBox conceptCheckBox = new CheckBox();
            conceptCheckBox.Margin = new Thickness(0, 0, 25, 15);
            conceptCheckBox.FontSize = 24;
            conceptCheckBox.VerticalContentAlignment = VerticalAlignment.Center;
            conceptCheckBox.Content = ds.Rows[i][1].ToString();
            conceptWrapPanel.Children.Add(conceptCheckBox);
            return conceptCheckBox;
        }

        private void cafeCombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Cafe = cafeCombobox.SelectedItem.ToString();
            
            if (cafeCombobox.SelectedItem != null)
            {
                thirdRow.Visibility = Visibility.Visible;
                thirdRow.Text = "Great please select the stations that are CURRENTLY available at " + Cafe;
                conceptWrapPanel.Visibility = Visibility.Visible;
            }
            UserName += Cafe;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MenuBuilderDataSetTableAdapters.MenuBuilder_UsersTableAdapter userAdapter = new MenuBuilderDataSetTableAdapters.MenuBuilder_UsersTableAdapter();
            userAdapter.Insert(UserName, Cafe, User);
        }
    }
}
