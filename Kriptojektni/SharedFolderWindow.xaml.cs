using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace Kriptojektni
{
    /// <summary>
    /// Interaction logic for SharedFolderWindow.xaml
    /// </summary>
    public partial class SharedFolderWindow : Window
    {
        object dummyNode = null;
        public SharedFolderWindow()
        {
            InitializeComponent();
            foreach (string s in Directory.GetFileSystemEntries(Application.Current.Properties["sharedFolderLocation"].ToString()))
            {
                TreeViewItem item = new TreeViewItem();

                string[] array = s.Split("\\");

                item.Header = array[array.Length - 1];

                item.Tag = s;
                item.FontWeight = FontWeights.Normal;
                item.Items.Add(dummyNode);

                Tree.Items.Add(item);
            }
        }

        private void OpenSharedFileButton_Click(object sender, RoutedEventArgs e)
        {
            if (Tree.SelectedItem != null)
            {
                string filePath = ((TreeViewItem)Tree.SelectedItem).Tag.ToString();
                FileAttributes fileAttributes = File.GetAttributes(filePath);
                if (fileAttributes.HasFlag(FileAttributes.Directory))
                {
                    MessageBox.Show("You must select a file!");
                    return;
                }

                try
                {
                    UserActions.openSharedFile(filePath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error!");
                    return;
                }
            }
            else
            {
                MessageBox.Show("No file has been selected");
                return;
            }

        }
    }

}
