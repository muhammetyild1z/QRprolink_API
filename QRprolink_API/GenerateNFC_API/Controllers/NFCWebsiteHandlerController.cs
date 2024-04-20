using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

[Route("api/[controller]")]
[ApiController]
public class NFCWebsiteHandlerController : ControllerBase
{
    [HttpPost]
    public IActionResult HandleNFCDataAndRedirect([FromBody] string nfcData)
    {
        if (string.IsNullOrWhiteSpace(nfcData))
        {
            return BadRequest("NFC verisi boş olamaz.");
        }

        try
        {
            
            NFCDataEncryptionController encryptionController = new NFCDataEncryptionController();
            IActionResult encryptionResult = encryptionController.EncryptNFCDataAndStore(nfcData);
            if (encryptionResult is BadRequestObjectResult)
            {
                return BadRequest(encryptionResult);
            }

            // URL formatını kontrol et
            if (IsValidUrl(nfcData))
            {
                
                var encryptionData = encryptionResult as OkObjectResult;
                var encryptionDataValue = encryptionData.Value as dynamic; // Cast to dynamic for flexible access

            
                byte[] encryptedData = encryptionDataValue.EncryptedData;
                byte[] key = encryptionDataValue.Key;
                byte[] iv = encryptionDataValue.IV;

                
                if (encryptedData == null || key == null || iv == null)
                {
                    return BadRequest("Şifrelenmiş NFC verisi, anahtar veya IV boş olamaz.");
                }

              
                RedirectUser(nfcData, encryptedData, key, iv);

                return Ok("Kullanıcıyı şu URL'ye yönlendir: " + nfcData);
            }
            else
            {
                return BadRequest("Geçersiz URL!");
            }
        }
        catch (Exception ex)
        {
            return BadRequest("NFC verisi işlenirken bir hata oluştu: " + ex.Message);
        }
    }

    private bool IsValidUrl(string url)
    {
        Uri uriResult;
        bool isValidUrl = Uri.TryCreate(url, UriKind.Absolute, out uriResult)
            && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);

        return isValidUrl;
    }

    private void RedirectUser(string url, byte[] encryptedData, byte[] key, byte[] iv)
    {
        
        Console.WriteLine("Kullanıcıyı şu URL'ye yönlendir: " + url);

        try
        {
           
            string base64EncryptedData = Convert.ToBase64String(encryptedData);

           
            string base64Key = Convert.ToBase64String(key);
            string base64IV = Convert.ToBase64String(iv);

           
            string urlWithEncryptedData = url + "?encrypted_data=" + base64EncryptedData + "&key=" + base64Key + "&iv=" + base64IV;
            Console.WriteLine("Kullanıcıya yönlendirilen URL (Şifrelenmiş NFC Verisi ve Anahtar/IV ile): " + urlWithEncryptedData);
        }
        catch (Exception ex)
        {
            Console.WriteLine("URL oluşturulurken bir hata oluştu: " + ex.Message);
        }
    }
}
