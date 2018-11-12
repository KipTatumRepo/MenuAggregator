using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Mail;
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

namespace MenuAggregator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public static int changesMade = 0;
        public static string UserName = Environment.UserName;   
        //public static string UserName = "haah";
        //public static string UserName = "v-fitatu";
        //public static string UserName = "v-idunno";
        //public static string UserName = "v-dumbass";
        //public static string UserName = "v-dipshit";
        //public static string UserName = "v-test";
        public static string Cafe;
        public static int numberOfCafes;
        public static int IsAdmin;
        
        public MainWindow()
        {
            try
            { 
                MenuBuilderDataSet ds = new MenuBuilderDataSet();
                MenuBuilderDataSet._MenuBuilder_UsersDataTable table = new MenuBuilderDataSet._MenuBuilder_UsersDataTable();
                MenuBuilderDataSetTableAdapters.MenuBuilder_UsersTableAdapter userAdapter = new MenuBuilderDataSetTableAdapters.MenuBuilder_UsersTableAdapter();

                InitializeComponent();

                userAdapter.IsAuth(table, UserName);
                numberOfCafes = table.Count;

                if(table.Count >= 1)
                { 
                    string isAdmin = table.Rows[0][4].ToString();
                    IsAdmin = Int32.Parse(isAdmin);
                }

                if (IsAdmin == 1)
                {
                    mainFrame.Source = new Uri("pages\\BackendHome.xaml", UriKind.Relative);
                }

                else if ( numberOfCafes >= 1)
                {
                    mainFrame.Source = new Uri("pages\\Home.xaml", UriKind.Relative);
                    Cafe = table.Rows[0][2].ToString(); //ds._MenuBuilder_Users.Rows[0][2].ToString();
                }
                else
                {
                    mainFrame.Source = new Uri("pages\\FirstTime.xaml", UriKind.Relative);
                }
                
            }
            catch (Exception ex)
            {
                MessageBox.Show("There was a problem trying to load the first page: \n" + ex);
            }
        }

        /*private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            string to = "v-datatu@microsoft.com";
            string from = "anything@anything.com";
            string subject = Cafe + "'s menu has been updated!";
            string body = "how to get the values we need to format this";
            string server = "smtp.gmail.com";
            string login = "kiptatum@gmail.com";
            string pword = "lucius6500";

            var smptClient = new SmtpClient(server, 587)
            {
                Credentials = new NetworkCredential(login, pword),
                EnableSsl = true

            };

            MailMessage message = new MailMessage(to, from, subject, body);

            smptClient.Send(message);
            
            

        }*/
    }
}
