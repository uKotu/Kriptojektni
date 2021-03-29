using System.Security.Cryptography;
using System.Text;

namespace Kriptojektni
{
    class Hash
    {
        public static string getSHA256Hash(string input)
        {
            SHA256 sha = SHA256.Create();
            return GetHash(sha, input);
        }
        public static string getSHA384Hash(string input)
        {

            SHA384 sha = SHA384.Create();
            return GetHash(sha, input);
        }
        public static string getSHA512Hash(string input)
        {
            SHA512 sha = SHA512.Create();
            return GetHash(sha, input);
        }
        private static string GetHash(HashAlgorithm hashAlgorithm, string input)
        {

            // Convert the input string to a byte array and compute the hash.
            byte[] data = hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(input));
            var sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hex hash
            return sBuilder.ToString();
        }
    }
}
