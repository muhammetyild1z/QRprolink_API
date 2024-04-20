using System.Security.Cryptography;
using System.Text;

namespace GenerateNFC_API.Concrate
{
    public class NFCDataProcessor
    {
        private static readonly byte[] Key = Encoding.UTF8.GetBytes("bu-bir-anahtar-0123456789"); // 16 bytes (128 bits)
        private static readonly byte[] IV = Encoding.UTF8.GetBytes("bu-bir-iv-012345"); // 8 bytes (64 bits)

        public static byte[] EncryptNFCData(string nfcData)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(nfcData);
                        }
                        return msEncrypt.ToArray();
                    }
                }
            }
        }

        public static string DecryptNFCData(byte[] encryptedData)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msDecrypt = new MemoryStream(encryptedData))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            return srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
        }

        public static string GenerateOneTimePassword()
        {
            const string allowedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            const int passwordLength = 10;

            Random rng = new Random();
            char[] password = new char[passwordLength];

            for (int i = 0; i < passwordLength; i++)
            {
                password[i] = allowedChars[rng.Next(0, allowedChars.Length)];
            }

            return new string(password);
        }
    }
}
