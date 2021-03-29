using System.Windows;

namespace Kriptojektni
{
    /// <summary>
    /// Interaction logic for ShareFileWindow.xaml
    /// </summary>
    public partial class ShareFileWindow : Window
    {
        public ShareFileWindow()
        {
            InitializeComponent();
            Application.Current.MainWindow = this;
            foreach (var x in LogIn.getUserCertificateTuples())
            {
                if (x.Item1 != Application.Current.Properties["Username"].ToString())
                {
                    userComboBox.Items.Add(x.Item1);
                }
            }
        }

        private void SelectUserButton_Click(object sender, RoutedEventArgs e)
        {
            if (userComboBox.SelectedItem != null)
            {

                this.Hide();
            }
        }
    }
}
