using Kriptojektni.CryptoLogic;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Windows;

namespace Kriptojektni
{
    class UserActions
    {
        public static void uploadFile(string filePath, string targetDirectoryPath)
        {
            byte[] content = File.ReadAllBytes(filePath);
            EncryptedFile outFile = Encrypt.encryptDataForLocalFile(content);


            if (!File.Exists(Path.Combine(targetDirectoryPath, Path.GetFileName(filePath))))
            {

                FileStream file = File.Create((Path.Combine(targetDirectoryPath, Path.GetFileName(filePath))));

                BinaryFormatter binFormat = new BinaryFormatter();
                binFormat.Serialize(file, outFile);
                file.Close();

            }

        }
        public static void createNewTextFile(string content, string fileName, string targetDirectoryPath)
        {
            EncryptedFile outFile = Encrypt.encryptDataForLocalFile(Encoding.UTF8.GetBytes(content));

            //encrypt content

            if (!File.Exists(Path.Combine(targetDirectoryPath, fileName)))
            {

                FileStream file = File.Create((Path.Combine(targetDirectoryPath, fileName)));

                BinaryFormatter binFormat = new BinaryFormatter();
                binFormat.Serialize(file, outFile);
                file.Close();

            }

        }
        public static void openFile(string filePath)
        {
            try
            {

                BinaryFormatter formatter = new BinaryFormatter();
                FileStream streamOfLocalFile = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                //decrypt file and put its stream

                EncryptedFile file = (EncryptedFile)formatter.Deserialize(streamOfLocalFile);
                byte[] decryptedFileContent = Decrypt.decryptLocalFile(file);

                string newTempFileName = System.IO.Path.GetTempFileName() + Guid.NewGuid().ToString() + Path.GetExtension(filePath);

                File.WriteAllBytes(newTempFileName, decryptedFileContent);

                Process fileopener = new Process();
                fileopener.StartInfo.FileName = "explorer";
                fileopener.StartInfo.Arguments = "\"" + newTempFileName + "\"";
                fileopener.Start();
                streamOfLocalFile.Close();
            }
            catch (Exception)
            {
                MessageBox.Show("File corrupted");
            }

        }
        public static void downloadFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                Stream myStream;
                if (saveFileDialog.ShowDialog() == true)
                {
                    if ((myStream = saveFileDialog.OpenFile()) != null)
                    {
                        BinaryFormatter formatter = new BinaryFormatter();
                        FileStream streamOfLocalFile = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                        //decrypt file and put its stream

                        EncryptedFile file = (EncryptedFile)formatter.Deserialize(streamOfLocalFile);

                        //decrypt content
                        byte[] outputBytes = Decrypt.decryptLocalFile(file);

                        myStream.Write(outputBytes);
                        myStream.Flush();
                        myStream.Close();
                    }
                }
            }
        }
        public static void editFile(string existingFilePath)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream streamOfLocalFile = new FileStream(existingFilePath, FileMode.Open, FileAccess.Read);

            EncryptedFile file = (EncryptedFile)formatter.Deserialize(streamOfLocalFile);
            streamOfLocalFile.Close();
            byte[] decryptedData = Decrypt.decryptLocalFile(file);

            NewFileWindow newFileWindow = new NewFileWindow();
            newFileWindow.Title.Text = Path.GetFileName(existingFilePath);
            newFileWindow.Title.IsEnabled = false;

            newFileWindow.Content.Text = Encoding.UTF8.GetString(decryptedData);
            newFileWindow.ShowDialog();

            byte[] newFileData = Encoding.UTF8.GetBytes(newFileWindow.Content.Text);

            newFileWindow.Close();
            var outFile = Encrypt.encryptDataForLocalFile(newFileData);

            File.Delete(existingFilePath);

            FileStream fileStream = File.Create(existingFilePath);

            BinaryFormatter binFormat = new BinaryFormatter();
            binFormat.Serialize(fileStream, outFile);
            fileStream.Close();

            return;
        }
        public static void deleteFile(string targetFilePath)
        {
            string path = Directory.GetCurrentDirectory();

            if (File.Exists(targetFilePath))
            {
                File.Delete(targetFilePath);
            }
        }
        public static void openSharedFile(string filePath)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream streamOfLocalFile = new FileStream(filePath, FileMode.Open, FileAccess.Read);

            EncryptedFile file = (EncryptedFile)formatter.Deserialize(streamOfLocalFile);
            string senderName = Encoding.UTF8.GetString(file.getSender());
            streamOfLocalFile.Close();

            var userCertTuple = LogIn.getUserCertificateTuples();
            X509Certificate2 senderCertificate = null;

            foreach (var x in userCertTuple)
            {
                if (x.Item1 == senderName)
                {
                    senderCertificate = LogIn.getUsersCertificateByName(senderName);
                    break;
                }

            }
            if (senderCertificate == null)
            {
                throw new CryptographicException("Sender's certificate not found");
            }
            byte[] decryptedFileContent = Decrypt.decryptSharedFile(file, senderCertificate);

            string newTempFileName = System.IO.Path.GetTempFileName() + Guid.NewGuid().ToString() + Path.GetExtension(filePath);

            File.WriteAllBytes(newTempFileName, decryptedFileContent);

            Process fileopener = new Process();
            fileopener.StartInfo.FileName = "explorer";
            fileopener.StartInfo.Arguments = "\"" + newTempFileName + "\"";
            fileopener.Start();
        }
        public static void shareFile(string filepath, string targetUser)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream streamOfLocalFile = new FileStream(filepath, FileMode.Open, FileAccess.Read);

            EncryptedFile file = (EncryptedFile)formatter.Deserialize(streamOfLocalFile);
            streamOfLocalFile.Close();
            byte[] decryptedData = Decrypt.decryptLocalFile(file);

            //find users certificate
            X509Certificate2 targetCertificate = LogIn.getUsersCertificateByName(targetUser);
            EncryptedFile outputFile = Encrypt.encryptDataForSharedFile(decryptedData, targetCertificate);
            try
            {
                string filename = Path.GetFileName(filepath);
                FileStream fileStream = File.Create(Application.Current.Properties["sharedFolderLocation"].ToString() + "\\" + filename);

                BinaryFormatter binFormat = new BinaryFormatter();
                binFormat.Serialize(fileStream, outputFile);
                fileStream.Close();
            }
            catch (Exception)
            {
                MessageBox.Show("File already exists");
                return;
            }


        }

    }
}
