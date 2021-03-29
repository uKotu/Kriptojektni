using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Windows;

namespace Kriptojektni
{
    class LogIn
    {
        public static bool tryLogIn(string username, string passwordHash, string certificateLocation)
        {

            bool found = false;
            try
            {
                using (StreamReader streamReader = new StreamReader(Application.Current.Properties["lookupTableLocation"].ToString()))
                {
                    string fileLine = streamReader.ReadLine();
                    while (!found && (fileLine != "EOF"))
                    {

                        string[] usernamePassComboArray = fileLine.Split('\t');
                        if (usernamePassComboArray.Length == 3)
                        {
                            //if username and password hash are validated
                            if (usernamePassComboArray[0] == username && usernamePassComboArray[1].ToLower() == passwordHash.ToLower())
                            {
                                X509Certificate2 userCertificate = new X509Certificate2(certificateLocation);
                                if (userCertificate.Thumbprint == usernamePassComboArray[2])
                                {

                                    /*if (!CertificateChecker.checkCertificateUserConection(certificateLocation, username))
                                    {
                                        MessageBox.Show("Invalid certificate for given username");
                                        return found;
                                    }*/
                                    if (!CertificateChecker.certificateValid(certificateLocation))
                                    {
                                        return found;
                                    }
                                    //certificate is valid
                                    found = true;
                                    return found;
                                }
                                else
                                {
                                    MessageBox.Show("Invalid certificate for given username");
                                    return found;
                                }
                            }

                        }
                        fileLine = streamReader.ReadLine();

                    }
                }
            }
            catch (Exception) { }

            return found;
        }

        public static List<Tuple<string, string, string>> getUserCertificateTuples()
        {
            List<Tuple<string, string, string>> outputTuples = new List<Tuple<string, string, string>>();
            try
            {
                using (StreamReader streamReader = new StreamReader(Application.Current.Properties["lookupTableLocation"].ToString()))
                {
                    string fileLine = streamReader.ReadLine();
                    while (fileLine != "EOF")
                    {

                        string[] usernamePassComboArray = fileLine.Split('\t');
                        if (usernamePassComboArray.Length == 3)
                        {
                            outputTuples.Add(new Tuple<string, string, string>(usernamePassComboArray[0], usernamePassComboArray[1], usernamePassComboArray[2]));

                        }
                        fileLine = streamReader.ReadLine();

                    }
                }
            }
            catch (Exception)
            {

            }
            return outputTuples;
        }

        public static X509Certificate2 getUsersCertificateByName(string username)
        {
            var userCertTuple = getUserCertificateTuples();
            string userCertThumbprint = "";
            foreach (var x in userCertTuple)
            {
                if (x.Item1 == username)
                {
                    userCertThumbprint = x.Item3;
                    break;
                }
            }
            if (userCertThumbprint.Length == 0)
            {
                throw new Exception("User's certificate not found");
            }
            DirectoryInfo directory = new DirectoryInfo(Application.Current.Properties["certsLocation"].ToString());
            foreach (var file in directory.GetFiles("*.crt"))
            {
                X509Certificate2 cert = new X509Certificate2(file.FullName);
                if (cert.Thumbprint == userCertThumbprint)
                    return cert;
            }
            throw new Exception("User's certificate not found");

        }
    }

}

