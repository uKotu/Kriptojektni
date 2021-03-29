using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace Kriptojektni
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string rootFolder = Application.Current.Properties["root"].ToString() + "\\" + Application.Current.Properties["Username"].ToString();
        private object dummyNode = null;

        public MainWindow()
        {
            InitializeComponent();

            foreach (string s in Directory.GetFileSystemEntries(rootFolder))
            {
                TreeViewItem item = new TreeViewItem();

                string[] array = s.Split("\\");

                item.Header = array[array.Length - 1];

                item.Tag = s;
                item.FontWeight = FontWeights.Normal;
                item.Items.Add(dummyNode);
                item.Expanded += new RoutedEventHandler(folder_Expanded);
                Tree.Items.Add(item);
            }
        }
        void refresh()
        {
            Tree.Items.Clear();
            foreach (string s in Directory.GetFileSystemEntries(rootFolder))
            {
                TreeViewItem item = new TreeViewItem();

                string[] array = s.Split("\\");

                item.Header = array[array.Length - 1];

                item.Tag = s;
                item.FontWeight = FontWeights.Normal;
                item.Items.Add(dummyNode);
                item.Expanded += new RoutedEventHandler(folder_Expanded);
                Tree.Items.Add(item);

            }

        }
        void folder_Expanded(object sender, RoutedEventArgs e)
        {
            TreeViewItem item = (TreeViewItem)sender;
            if (item.Items.Count == 1 && item.Items[0] == dummyNode)
            {
                item.Items.Clear();
                try
                {
                    foreach (string s in Directory.GetFileSystemEntries(item.Tag.ToString()))
                    {
                        TreeViewItem subitem = new TreeViewItem();
                        subitem.Header = s.Substring(s.LastIndexOf("\\") + 1);
                        subitem.Tag = s;
                        subitem.FontWeight = FontWeights.Normal;
                        subitem.Items.Add(dummyNode);

                        /*FileAttributes attr = File.GetAttributes(s);
                        if (!attr.HasFlag(FileAttributes.Directory))
                            subitem.is*/

                        subitem.Expanded += new RoutedEventHandler(folder_Expanded);
                        item.Items.Add(subitem);
                    }
                }
                catch (Exception) { }
            }
        }

        private void DownloadButton_Click(object sender, RoutedEventArgs e)
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
                //success
                UserActions.downloadFile(filePath);
            }
            else
            {
                MessageBox.Show("No file has been selected");
                return;
            }
            refresh();
        }

        private void OpenButton_Click(object sender, RoutedEventArgs e)
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
                //success
                UserActions.openFile(filePath);
            }
            else
            {
                MessageBox.Show("No file has been selected");
                return;
            }
            refresh();
        }

        private void NewTextFileButton_Click(object sender, RoutedEventArgs e)
        {

            if (Tree.SelectedItem != null)
            {
                string filePath = ((TreeViewItem)Tree.SelectedItem).Tag.ToString();
                FileAttributes fileAttributes = File.GetAttributes(filePath);
                if (!fileAttributes.HasFlag(FileAttributes.Directory))
                {
                    MessageBox.Show("You must select a folder!");
                    return;
                }


                NewFileWindow window = new NewFileWindow();


                window.ShowDialog();
                string content = ((NewFileWindow)Application.Current.MainWindow).Content.Text;
                string fileName = ((NewFileWindow)Application.Current.MainWindow).Title.Text;


                Application.Current.MainWindow = this;
                if (File.Exists(filePath + "\\" + fileName))
                {
                    MessageBox.Show("File with the same name on that location already exists");
                    return;
                }
                ///sucess
                UserActions.createNewTextFile(content, fileName, filePath);

            }
            else
            {
                MessageBox.Show("No location has been selected");
                return;
            }
            refresh();
        }

        private void ShareFileButton_Click(object sender, RoutedEventArgs e)
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
                ShareFileWindow shareFileWindow = new ShareFileWindow();
                shareFileWindow.ShowDialog();

                string targetUser = ((ShareFileWindow)Application.Current.MainWindow).userComboBox.SelectedItem.ToString();
                Application.Current.MainWindow.Close();
                Application.Current.MainWindow = this;

                //sign it and encrypt with public key of the target
                //and place file in the shared folder 
                try
                {
                    UserActions.shareFile(filePath, targetUser);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return;
                }

            }
            else
            {
                MessageBox.Show("No file has been selected");
                return;
            }
            refresh();
        }

        private void UploadButton_Click(object sender, RoutedEventArgs e)
        {

            if (Tree.SelectedItem != null)
            {
                string filePath = ((TreeViewItem)Tree.SelectedItem).Tag.ToString();
                FileAttributes fileAttributes = File.GetAttributes(filePath);
                if (!fileAttributes.HasFlag(FileAttributes.Directory))
                {
                    MessageBox.Show("You must select a folder!");
                    return;
                }
                OpenFileDialog dialog = new OpenFileDialog();
                FileStream fileStream;

                if (dialog.ShowDialog() == true)
                {
                    if ((fileStream = (FileStream)dialog.OpenFile()) != null)
                    {
                        UserActions.uploadFile(fileStream.Name, filePath);
                    }
                }
            }
            else
            {
                MessageBox.Show("No folder selected!");
            }

        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {

            if (Tree.SelectedItem != null)
            {
                string filePath = ((TreeViewItem)Tree.SelectedItem).Tag.ToString();
                FileAttributes fileAttributes = File.GetAttributes(filePath);
                if (fileAttributes.HasFlag(FileAttributes.Directory))
                {
                    if(filePath == Application.Current.Properties["root"].ToString() + "\\" + Application.Current.Properties["Username"].ToString())
                    {
                        MessageBox.Show("You cant delete your home folder!");
                        return;
                    }
                    var res = MessageBox.Show("This will delete all items in the contained folder, are you sure?", "Warning", MessageBoxButton.OKCancel);

                    if (res == MessageBoxResult.OK)

                    {
                        Directory.Delete(filePath, true);
                    }
                }
                else
                {
                    File.Delete(filePath);
                }

            }
            else
            {
                MessageBox.Show("Nothing selected");
            }
            refresh();
        }

        private void NewFolderButton_Click(object sender, RoutedEventArgs e)
        {

            if (Tree.SelectedItem != null)
            {
                string filePath = ((TreeViewItem)Tree.SelectedItem).Tag.ToString();
                FileAttributes fileAttributes = File.GetAttributes(filePath);
                if (fileAttributes.HasFlag(FileAttributes.Directory))
                {
                    string newFolderName = new InputBox("Insert name of the new folder", "New Folder", "Arial", 12).ShowDialog();
                    Directory.CreateDirectory(filePath + "\\" + newFolderName);
                }

            }
            else
            {
                MessageBox.Show("You must select a folder!");
                return;
            }
            refresh();
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {

            if (Tree.SelectedItem != null)
            {
                string filePath = ((TreeViewItem)Tree.SelectedItem).Tag.ToString();
                FileAttributes fileAttributes = File.GetAttributes(filePath);
                if (!fileAttributes.HasFlag(FileAttributes.Directory) && System.IO.Path.GetExtension(filePath).ToString() == ".txt")
                {
                    UserActions.editFile(filePath);
                }
                else
                {
                    MessageBox.Show("You must select a text file!");
                }

            }
            else
            {
                MessageBox.Show("No file selected");
                return;
            }
            refresh();
        }

        private void ViewSharedFolderButton_Click(object sender, RoutedEventArgs e)
        {
            SharedFolderWindow window = new SharedFolderWindow();
            window.ShowDialog();
        }
    }
}
