using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Windows;

namespace Kriptojektni.CryptoLogic
{
    class Decrypt
    {
        public static byte[] decryptLocalFile(EncryptedFile encryptedFile)
        {
            Aes aes = Aes.Create();

            var cer = File.ReadAllText((Application.Current.Properties["userCertificateLocation"].ToString()));
            var eccPem = File.ReadAllText(Application.Current.Properties["userPrivateKeyLocation"].ToString());
            var cert = X509Certificate2.CreateFromPem(cer, eccPem);


            //separate from input data
            byte[] encryptedKey = encryptedFile.getKey();

            byte[] decryptedKey = decryptBytesPrivateKey(encryptedKey, cert);// userCertificateWithPrivateKey);

            byte[] encryptedData_DsCombo = encryptedFile.getData();

            //feed the aes class with the needed parameters
            aes.IV = encryptedFile.getIV(); aes.Key = decryptedKey;
            byte[] decryptedData_DsCombo = decryptBytes(aes, encryptedData_DsCombo);

            //sha256 signature takes 256bytes from the end
            byte[] signedHash = decryptedData_DsCombo.TakeLast(256).ToArray();

            byte[] data = decryptedData_DsCombo.Take(decryptedData_DsCombo.Length - 256).ToArray();

            RSACng rsa = (RSACng)cert.GetRSAPrivateKey();

            if (rsa.VerifyData(data, signedHash, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1))
            {
                return data;
            }
            else
            {
                MessageBox.Show("File corrupted!");
                throw new CryptographicException("File corrupted");
            }
        }
        public static byte[] decryptSharedFile(EncryptedFile encryptedFile, X509Certificate2 senderCertificate)
        {
            TripleDESCng aes = new TripleDESCng();

            var cer = File.ReadAllText((Application.Current.Properties["userCertificateLocation"].ToString()));
            var eccPem = File.ReadAllText(Application.Current.Properties["userPrivateKeyLocation"].ToString());
            var cert = X509Certificate2.CreateFromPem(cer, eccPem);


            //separate from input data
            byte[] encryptedKey = encryptedFile.getKey();

            byte[] decryptedKey = decryptBytesPrivateKey(encryptedKey, cert);

            byte[] encryptedData_DsCombo = encryptedFile.getData();

            //feed the aes class with the needed parameters
            aes.IV = encryptedFile.getIV(); aes.Key = decryptedKey;
            byte[] decryptedData_DsCombo = decryptBytes(aes, encryptedData_DsCombo);

            //sha256 signature takes 256bytes from the end
            byte[] signedHash = decryptedData_DsCombo.TakeLast(256).ToArray();

            byte[] data = decryptedData_DsCombo.Take(decryptedData_DsCombo.Length - 256).ToArray();

            RSACng rsa = (RSACng)senderCertificate.GetRSAPublicKey();

            if (rsa.VerifyData(data, signedHash, HashAlgorithmName.SHA384, RSASignaturePadding.Pkcs1))
            {
                return data;
            }
            else
            {
                //dignuti upozorenje iznad
                //MessageBox.Show("File corrupted!");
                throw new CryptographicException("File corrupted");
            }
        }
        private static byte[] decryptBytes(SymmetricAlgorithm algorithm, byte[] inputBytes)
        {
            byte[] encryptedOutput;

            ICryptoTransform cryptoTransform = algorithm.CreateDecryptor(algorithm.Key, algorithm.IV);

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
        private static byte[] decryptBytesPublicKey(byte[] data, X509Certificate2 certificate)
        {
            /*
            byte[] chunk = new byte[16];
            byte[] outputBytes = new byte[data.Length];
            int position = 0;
            int leftover = 0;

            //rsa needs to decrypt in blocks of data 
            bool notDecrypted = true;
            while (notDecrypted)
            {
                if (position + 16 < data.Length)
                { //if we have not reached the end, copy next 16 bytes in the chunk of data to decrypt
                    Array.Copy(data, position, chunk, position, 16);
                    position += 16;
                }
                else
                {
                    Array.Copy(data, position, chunk, position, data.Length - position);
                    leftover = data.Length - position;
                    notDecrypted = false;
                }*/
            if (data.Length < 256)
            {
                var x = new byte[256 - data.Length];
                data = combine(x, data);
            }

            using (RSACng rsa = (RSACng)certificate.GetRSAPublicKey())
            {
                return rsa.Decrypt(data, RSAEncryptionPadding.OaepSHA256);
                //Array.Copy(x, 0, outputBytes, position, leftover > 0 ? leftover : position);
            }

        }
        private static byte[] decryptBytesPrivateKey(byte[] data, X509Certificate2 certificate)
        {
            using (RSACng rsa = (RSACng)certificate.GetRSAPrivateKey())
            {
                return rsa.Decrypt(data, RSAEncryptionPadding.OaepSHA256);
            }
        }
        private static byte[] combine(byte[] first, byte[] second)
        {
            return first.Concat(second).ToArray();
        }
    }
}
