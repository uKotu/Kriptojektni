using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Windows;

namespace Kriptojektni
{
    class RegisterUser
    {
        public static void Register(string username, string passwordHash, string certificatePath)
        {
            string newUserDirectoryPath = Application.Current.Properties["root"].ToString() + "\\" + username;

            if (Directory.Exists(newUserDirectoryPath))
            {
                MessageBox.Show("Username taken");
                return;
            }
            else
            {
                Directory.CreateDirectory(newUserDirectoryPath);
                Directory.CreateDirectory(newUserDirectoryPath + "\\home");
            }
            X509Certificate2 userCertificate = new X509Certificate2(certificatePath);

            FileStream fileStream = new FileStream(Application.Current.Properties["lookupTableLocation"].ToString(), FileMode.Open);
            fileStream.Seek(-3, SeekOrigin.End);

            StreamWriter fileWriter = new StreamWriter(fileStream);

            fileWriter.Write(username + "\t" + passwordHash + "\t" + userCertificate.Thumbprint + "\n" + "EOF");
            fileWriter.Flush();
            fileWriter.Close();

            return;
        }
    }
}
