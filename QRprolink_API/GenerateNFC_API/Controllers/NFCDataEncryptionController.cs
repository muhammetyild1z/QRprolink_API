using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

[Route("api/[controller]")]
[ApiController]
public class NFCDataEncryptionController : ControllerBase
{
    [HttpPost]
    public IActionResult EncryptNFCDataAndStore( string nfcData)
    {
        if (string.IsNullOrWhiteSpace(nfcData))
        {
            return BadRequest("NFC verisi boş olamaz.");
        }

        try
        {
            // Rastgele anahtar ve IV oluştur
            byte[] key = GenerateRandomKey();
            byte[] iv = GenerateRandomIV();

           
            byte[] encryptedData = NFCDataProcessor.EncryptNFCData(nfcData, key, iv);

           
            return Ok(new { EncryptedData = encryptedData, Key = key, IV = iv });
        }
        catch (Exception ex)
        {
            return BadRequest("NFC verisi şifrelenirken bir hata oluştu: " + ex.Message);
        }
    }

    private byte[] GenerateRandomKey()
    {
        using (var aes = Aes.Create())
        {
            aes.GenerateKey();
            return aes.Key;
        }
    }

    private byte[] GenerateRandomIV()
    {
        using (var aes = Aes.Create())
        {
            aes.GenerateIV();
            return aes.IV;
        }
    }
}

public class NFCDataProcessor
{
    public static byte[] EncryptNFCData(string nfcData, byte[] key, byte[] iv)
    {
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = key;
            aesAlg.IV = iv;

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
}
