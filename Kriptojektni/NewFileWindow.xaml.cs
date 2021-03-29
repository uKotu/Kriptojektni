using System.Windows;

namespace Kriptojektni
{
    /// <summary>
    /// Interaction logic for NewFileWindow.xaml
    /// </summary>
    public partial class NewFileWindow : Window
    {
        public NewFileWindow()
        {
            InitializeComponent();
            Application.Current.MainWindow = this;
        }

        private void CreateFileButton_Click(object sender, RoutedEventArgs e)
        {
            if (Content.Text.Length == 0)
            {
                MessageBox.Show("No content");
                return;
            }

            if (Title.Text.Length == 0)
            {
                MessageBox.Show("No title");
                return;
            }

            this.Hide();

        }
    }
}
