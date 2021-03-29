using Microsoft.Win32;
using System.Windows;

namespace Kriptojektni
{
    /// <summary>
    /// Interaction logic for RegisterWindow.xaml
    /// </summary>
    public partial class RegisterWindow : Window
    {
        public RegisterWindow()
        {
            InitializeComponent();
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            var result = openFileDialog.ShowDialog();

            if (result == true)
            {
                certificateBox.Text = openFileDialog.FileName;
            }
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            if (usernameBox.Text.Length == 0)
            {
                MessageBox.Show("Username too short");
                return;
            }
            if (passwordBox.Password.Length == 0)
            {
                MessageBox.Show("Password too short");
                return;
            }
            if (passwordBox.Password == passwordBoxRepeat.Password)
            {
                if (CertificateChecker.certificateValid(certificateBox.Text))
                {
                    RegisterUser.Register(usernameBox.Text, Hash.getSHA512Hash(passwordBox.Password), certificateBox.Text);
                    this.Close();
                }
                else { MessageBox.Show("Invalid certificate"); }

                return;
            }
        }
    }
}
