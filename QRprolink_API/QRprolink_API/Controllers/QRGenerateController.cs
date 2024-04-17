using Microsoft.AspNetCore.Mvc;
using QRprolink_API.Models;
using System.IO;
using System;
using QRCoder;
using System.Drawing;
using System.Drawing.Imaging;
using static QRCoder.PayloadGenerator;
using System.Security.Policy;
using System.Threading.Tasks;

namespace QRprolink_API.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class QRGenerateController : Controller
    {
        [HttpPost("QRCodeGenerate")]
        public IActionResult QRCodeGenerate(QRCodeModel model)
        {
            Payload payload = null;
            switch (model.QRCodeType)
            {
                case 1: // website url
                    payload = new PayloadGenerator.Url(model.WebsiteURL);
                    break;
                case 2: // bookmark url
                    payload = new Bookmark(model.BookmarkURL, model.BookmarkURL);
                    break;
                case 3: // compose sms
                    payload = new SMS(model.SMSPhoneNumber, model.SMSBody);
                    break;
                case 4: // compose whatsapp message
                    payload = new WhatsAppMessage(model.WhatsAppNumber, model.WhatsAppMessage);
                    break;
                case 5: //compose email
                    payload = new Mail(model.ReceiverEmailAddress, model.EmailSubject, model.EmailMessage);
                    break;
                case 6: // wifi qr code
                    payload = new WiFi(model.WIFIName, model.WIFIPassword, WiFi.Authentication.WPA);
                    break;
                case 7: // ig qr code
                    payload = new QRCoder.PayloadGenerator.Url("https://instagram.com/" + model.igUserName);
                    break;
                case 8: // X qr code
                    payload = new QRCoder.PayloadGenerator.Url("https://twitter.com/" + model.igUserName);
                    break;
                case 9: // btc address to qr code TEST EDILMEDI!!!                
                    payload = new QRCoder.PayloadGenerator.BitcoinAddress(model.BtcAddress, model.BtcAmount, model.BtcMessage);
                    break;
                case 10: // Geolocation  qr code
                    payload = new QRCoder.PayloadGenerator.Geolocation(model.Latitude.ToString(), model.Longitude.ToString());
                    break;
                case 11: // phone number  qr code       
                    payload = new QRCoder.PayloadGenerator.PhoneNumber(model.phoneNumaber);
                    break;
                case 12: // OneTimePassword to qr code
                    OneTimePassword generator = new OneTimePassword()
                    {
                        Secret = model.Secret,
                        Issuer = model.Issuer,
                        Label = model.Label,
                        Period = model.Period,
                    };
                    payload = generator;
                    break;
                case 13: // Litecoin to qr code
                    payload = new QRCoder.PayloadGenerator.LitecoinAddress(model.LitecoinAddress, model.LitecoinAmount, model.LitecoinMessage);
                    break;

                case 14: // vcard tp qr code
                    ContactData vcardData = new ContactData(
                        ContactData.ContactOutputType.VCard3,
                        model.Firstname, model.Lastname, model.Nickname,
                        null, model.MobilePhone,
                        null, model.Email, null, model.Website, null, null, model.City,
                        null, model.Country, model.Note, null
                        );
                    payload = vcardData;
                    break;
                case 15: // .vcf to qr code TEST EDILECEK
                    payload = new vcfPayload(model.vcfFilePath);
                    break;
                case 16: // music to qr code  her platforma ayri mi olacak yoksa platform platform mu???
                    PayloadGenerator.Url musicGeneratorUrl = new PayloadGenerator.Url(model.MusicURL);
                    payload = musicGeneratorUrl;
                    break;
            }

            // QR kodunu oluştur
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(payload);
            QRCode qrCode = new QRCode(qrCodeData);
            //var qrCodeAsBitmap = qrCode.GetGraphic(20);
            //   use this when you want to show your logo in middle of QR Code and change color of qr code
           // Bitmap logoImage = new Bitmap(@"wwwroot/img/Virat-Kohli.jpg");
            var qrCodeAsBitmap = qrCode.GetGraphic(20, $"{model.qrcolor}", $"{model.bgcolor}" /*logoImage*/);
            // QR kodunu bir PNG dosyasına dönüştürerek kaydet
            string outputPath = "output.png";
            qrCodeAsBitmap.Save(outputPath, System.Drawing.Imaging.ImageFormat.Png);

            // QR kodunu Base64 dizeye dönüştürerek model'e ekle
            string base64String = Convert.ToBase64String(BitmapToByteArray(qrCodeAsBitmap));
            model.QRImageURL = base64String;

            // Başarılı işlem durumunda PNG dosyasını ve Base64 dizesini döndür
            byte[] imageData = Convert.FromBase64String(model.QRImageURL);
            return File(imageData, "image/png", "qrcode.png");
        }

        //  Bitmap to byte array
        private byte[] BitmapToByteArray(Bitmap bitmap)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                return stream.ToArray();
            }
        }

        private class vcfPayload : Payload
        {
            private string vcfPath;
            public vcfPayload(string vcfPath1)
            {
                vcfPath = vcfPath1;
            }

            public override string ToString()
            {
                return vcfPath;
            }

        }

    }
}
