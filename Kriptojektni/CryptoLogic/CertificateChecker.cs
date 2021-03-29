using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;
using System.IO;
using System.Windows;

namespace Kriptojektni
{
    class CertificateChecker
    {
        public static bool certificateValid(string certificateLocation)
        {

            FileStream fs = new FileStream(certificateLocation, FileMode.Open);
            X509CertificateParser certParser = new X509CertificateParser();

            X509Certificate userCertificate = certParser.ReadCertificate(fs);
            fs.Close();

            string CA = Application.Current.Properties["CA"].ToString();
            string CRL = Application.Current.Properties["CRL"].ToString();

            fs = new FileStream(CA, FileMode.Open);
            X509Certificate CACertificate = certParser.ReadCertificate(fs);
            fs.Close();


            X509CrlParser crlParser = new X509CrlParser();

            /*fs = new FileStream(CRL, FileMode.Open);
            X509Crl CRLCertificate = crlParser.ReadCrl(fs);
            fs.Close();*/

            //verify that the certificate is signed by the CA
            try
            {
                userCertificate.Verify(CACertificate.GetPublicKey());
            }
            catch (GeneralSecurityException)
            {
                MessageBox.Show("Your certificate is not signed by an authorized CA");
                return false;
            }
            /*
            //verify that the crl is signed by the CA
            try
            {
                CRLCertificate.Verify(CACertificate.GetPublicKey());
            }
            catch (GeneralSecurityException)
            {
                MessageBox.Show("Your CRL is not signed by an authorized CA");
                return false;
            }

            //verify that the certificate is not revoked
            if(CRLCertificate.IsRevoked(userCertificate))
            {
                MessageBox.Show("Your certificate has been revoked");
                return false;
            }
            */
            //verify the certificate time validity
            if (!userCertificate.IsValidNow)
            {
                MessageBox.Show("Your certificate is not valid");
                return false;
            }
            return true;
        }

    }
}
