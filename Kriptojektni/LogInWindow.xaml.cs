using Microsoft.Win32;
using System.Windows;

namespace Kriptojektni
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class LogInWindow : Window
    {

        public LogInWindow()
        {
            //setup of config locations, could be extracted to an outside file for more flexibility
            InitializeComponent();
            Application.Current.Properties.Add("CA", "C:\\Users\\Lenovo\\Desktop\\kripto\\ca\\CA.crt");
            Application.Current.Properties.Add("CRL", "C:\\Users\\Lenovo\\Desktop\\kripto\\ca\\root.crl");
            Application.Current.Properties.Add("lookupTableLocation", "C:\\Users\\Lenovo\\Desktop\\kripto\\lookuptable.txt");
            Application.Current.Properties.Add("root", "C:\\Users\\Lenovo\\Desktop\\kripto\\root");
            Application.Current.Properties.Add("sharedFolderLocation", "C:\\Users\\Lenovo\\Desktop\\kripto\\root\\shared");
            Application.Current.Properties.Add("privateKeysLocation", "C:\\Users\\Lenovo\\Desktop\\kripto\\keys\\");
            Application.Current.Properties.Add("certsLocation", "C:\\Users\\Lenovo\\Desktop\\kripto\\");
        }

        private void LogIn_Click(object sender, RoutedEventArgs e)
        {
            if (usernameBox.Text.Length <= 0)
            {
                MessageBox.Show("Username field empty");
            }
            if (passwordBox.Password.Length <= 0)
            {
                MessageBox.Show("Password field empty");
            }
            else //try to login
            {
                if (LogIn.tryLogIn(usernameBox.Text, Hash.getSHA512Hash(passwordBox.Password), certificateTextBox.Text))
                {
                    //SUCCESSFULL LOGIN
                    //////////////////////////////////////////////////////////
                    Application.Current.Properties.Add("Username", usernameBox.Text);
                    Application.Current.Properties.Add("userCertificateLocation", certificateTextBox.Text);
                    Application.Current.Properties.Add("userPrivateKeyLocation", Application.Current.Properties["privateKeysLocation"].ToString() + usernameBox.Text + ".key");


                    MainWindow window = new MainWindow();
                    window.Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Wrong credentials!");
                }
            }

        }

        private void browseButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            //openFileDialog.Filter = "(.p12)|*.p12";
            var result = openFileDialog.ShowDialog();

            if (result == true)
            {
                certificateTextBox.Text = openFileDialog.FileName;
            }
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            var regWindow = new RegisterWindow();
            regWindow.Show();
        }
    }
}
