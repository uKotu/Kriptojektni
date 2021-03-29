using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Windows;

namespace Kriptojektni.CryptoLogic
{
    class Encrypt
    {
        public static EncryptedFile encryptDataForLocalFile(byte[] inputBytes)
        {
            byte[] encryptedData_DsCombo;

            byte[] symmetricKey;

            Aes aes = Aes.Create();
            symmetricKey = aes.Key;

            var cer = File.ReadAllText((Application.Current.Properties["userCertificateLocation"].ToString()));
            var eccPem = File.ReadAllText(Application.Current.Properties["userPrivateKeyLocation"].ToString());
            var cert = X509Certificate2.CreateFromPem(cer, eccPem);


            //digital signature, hash of the original document, encrypted by users private key
            RSACng rsa = (RSACng)cert.GetRSAPrivateKey();
            byte[] signedHash = rsa.SignData(inputBytes, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);


            //data + digitalsignature (sha256 - 256bits)
            byte[] data_ds_combo = combine(inputBytes, signedHash);

            encryptedData_DsCombo = encryptBytes(aes, data_ds_combo);
            //[data + digital signature]

            byte[] keyEncrypted = encryptBytesPublicKey(symmetricKey, cert);//userCertificate);

            return new EncryptedFile(encryptedData_DsCombo, keyEncrypted, aes.IV);
        }
        public static EncryptedFile encryptDataForSharedFile(byte[] inputBytes, X509Certificate2 targetUserCertificate)
        {
            byte[] encryptedData_DsCombo;

            byte[] symmetricKey;

            TripleDESCng des = new TripleDESCng();
            symmetricKey = des.Key;

            var cer = File.ReadAllText((Application.Current.Properties["userCertificateLocation"].ToString()));
            var eccPem = File.ReadAllText(Application.Current.Properties["userPrivateKeyLocation"].ToString());
            var cert = X509Certificate2.CreateFromPem(cer, eccPem);


            //digital signature, hash of the original document, encrypted by users private key
            RSACng rsa = (RSACng)cert.GetRSAPrivateKey();
            byte[] signedHash = rsa.SignData(inputBytes, HashAlgorithmName.SHA384, RSASignaturePadding.Pkcs1);


            //data + digitalsignature (sha384 - 384bits)
            byte[] data_ds_combo = combine(inputBytes, signedHash);

            encryptedData_DsCombo = encryptBytes(des, data_ds_combo);
            //[data + digital signature]

            byte[] keyEncrypted = encryptBytesPublicKey(symmetricKey, targetUserCertificate);//userCertificate);

            EncryptedFile outFile = new EncryptedFile(encryptedData_DsCombo, keyEncrypted, des.IV);
            //initialize sender, so we know whose certificate to use for data verification
            outFile.setSender(Encoding.UTF8.GetBytes(Application.Current.Properties["Username"].ToString()));
            return outFile;
        }
        private static byte[] encryptBytes(SymmetricAlgorithm algorithm, byte[] inputBytes)
        {
            byte[] encryptedOutput;

            ICryptoTransform cryptoTransform = algorithm.CreateEncryptor(algorithm.Key, algorithm.IV);

            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, cryptoTransform, CryptoStreamMode.Write))
                {
                    cryptoStream.Write(inputBytes, 0, inputBytes.Length);
                }
                encryptedOutput = memoryStream.ToArray();
            }
            return encryptedOutput;
        }
        private static byte[] encryptBytesPublicKey(byte[] data, X509Certificate2 certificate)
        {
            using (RSACng rsa = (RSACng)certificate.GetRSAPublicKey())
            {
                return rsa.Encrypt(data, RSAEncryptionPadding.OaepSHA256);
            }
        }
        private static byte[] encryptBytesPrivateKey(byte[] data, X509Certificate2 certificate)
        {
            using (RSACng rsa = (RSACng)certificate.GetRSAPrivateKey())
            {
                return rsa.Encrypt(data, RSAEncryptionPadding.OaepSHA256);
            }

        }
        private static byte[] combine(byte[] first, byte[] second)
        {
            return first.Concat(second).ToArray();
        }
    }
}
