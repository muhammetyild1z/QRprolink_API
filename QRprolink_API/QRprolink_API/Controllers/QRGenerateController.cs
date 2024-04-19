using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QRCoder;
using QRprolink_API.Models;
using System.Drawing;
using System.IO;
using static QRCoder.PayloadGenerator;
using static QRCoder.PayloadGenerator.BitcoinLikeCryptoCurrencyAddress;

namespace QRprolink_API.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class QRGenerateController : Controller
    {
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
        private string urlClear(string url)
        {
            url.Replace("ö", "o").Replace("ü", "u").Replace("ş", "s").Replace("ı", "i").Replace("ğ", "g").Replace("ç", "c")
                .Replace("Ö", "O").Replace("Ü", "U").Replace("Ş", "S").Replace("İ", "I").Replace("Ğ", "G").Replace("Ç", "C").Normalize();
            return url;
        }
        private QRCustomizeModel modelCreate(string bgcolor, string qrcolor, int qrsize, string icon, int iconSizePercent, int iconSizeBorder)
        {

            QRCustomizeModel model = new QRCustomizeModel
            {
                bgcolor = bgcolor,
                qrcolor = qrcolor,
                qrsize = qrsize,
                icon = icon,
                iconSizePercent = iconSizePercent,
                iconSizeBorder = iconSizeBorder

            };

            if (model.iconSizePercent == 0) model.iconSizePercent = 20;
            if (model.iconSizeBorder == 0) model.iconSizeBorder = 20;
            if (model.iconSizePercent < 200) model.iconSizePercent = 20;
            if (model.iconSizeBorder < 200) model.iconSizeBorder = 20;


            if (model.qrcolor == "red") model.qrcolor = "f00";
            if (model.qrcolor == "blue") model.qrcolor = "00f";
            if (model.qrcolor == "green") model.qrcolor = "080";
            if (model.qrcolor == "orange") model.qrcolor = "FFA500";
            if (model.qrcolor == "gray") model.qrcolor = "808080";
            if (model.qrcolor != null && model.qrcolor.Length < 3) model.qrcolor = "000";
            if (model.qrcolor == null) model.qrcolor = "000";
            if (model.bgcolor == null) model.bgcolor = "fff";
            if (model.bgcolor == "red") model.qrcolor = "f00";
            if (model.bgcolor == "blue") model.qrcolor = "00f";
            if (model.bgcolor == "green") model.qrcolor = "080";
            if (model.bgcolor == "orange") model.qrcolor = "FFA500";
            if (model.bgcolor == "gray") model.qrcolor = "808080";
            if (model.qrcolor != null && model.bgcolor.Length < 3) model.bgcolor = "fff";
            if (model.qrsize == 0) model.qrsize = 20;

            return model;
        }

        [HttpPost]
        private IActionResult GenerateQRCodeFile(Payload payload, QRCustomizeModel model)
        {
            //C:/Users/muham/OneDrive/Belgeler/GitHub/AnimeX/AnimeX/AnimeX/wwwroot/animeX/img/log.png
            // QR kodunu oluştur
            Bitmap qrIconFromBitmap;
            if (model.icon == null) qrIconFromBitmap = null;
            else qrIconFromBitmap = new Bitmap(model.icon);
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(payload);
            QRCode qrCode = new QRCode(qrCodeData);
            var qrCodeAsBitmap = qrCode.GetGraphic(model.qrsize, ColorTranslator.FromHtml($"#{model.qrcolor}"), ColorTranslator.FromHtml($"#{model.bgcolor}"), qrIconFromBitmap, model.iconSizePercent, model.iconSizeBorder, false, null);

            string outputPath = "output.png";
            qrCodeAsBitmap.Save(outputPath, System.Drawing.Imaging.ImageFormat.Png);
            byte[] fileBytes = System.IO.File.ReadAllBytes(outputPath);
            System.IO.File.Delete(outputPath);

            return File(fileBytes, "image/png", "qrcode.png");
        }

        [HttpPost("url")]
        public IActionResult GenerateQRCodeFromURL(string url, string bgcolor, string qrcolor, int qrsize, string icon, int iconSizePercent, int iconSizeBorder)
        {
            var model = modelCreate(bgcolor, qrcolor, qrsize, icon, iconSizePercent, iconSizeBorder);

            if (url.Length > 100)
            {
                return BadRequest("url en fazla 100 karakter olmalıdır");
            }
            var newUrl = urlClear(url);
            var payload = new PayloadGenerator.Url(newUrl);
            return GenerateQRCodeFile(payload, model);
        }

        [HttpPost("bookmark")]
        public IActionResult GenerateQRCodeFromBookmark(string BookmarkTitle, string BookmarkURL, string bgcolor, string qrcolor, int qrsize, string icon, int iconSizePercent, int iconSizeBorder)
        {
            var model = modelCreate(bgcolor, qrcolor, qrsize, icon, iconSizePercent, iconSizeBorder);
            if (BookmarkTitle.Length > 100 || BookmarkURL.Length > 100)
            {
                return BadRequest("bookmark url veya bookmark baslık en fazla 100 karakter olmalıdır");
            }
            var payload = new Bookmark(BookmarkTitle, BookmarkURL);
            return GenerateQRCodeFile(payload, model);
        }

        [HttpPost("sms")]
        public IActionResult GenerateQRCodeFromSms(string SMSPhoneNumber, string SMSBody, string bgcolor, string qrcolor, int qrsize, string icon, int iconSizePercent, int iconSizeBorder)
        {
            var model = modelCreate(bgcolor, qrcolor, qrsize, icon, iconSizePercent, iconSizeBorder);
            if (SMSPhoneNumber.Length > 11)
            {
                return BadRequest("sms telefon numarası en fazla 11 karakter olmalıdır");
            }
            if (SMSBody.Length > 300)
            {
                return BadRequest("sms mesajı en fazla 300 karakter olmalıdır");
            }
            var payload = new SMS(SMSPhoneNumber, SMSBody);
            return GenerateQRCodeFile(payload, model);
        }

        [HttpPost("whatsapp")]
        public IActionResult GenerateQRCodeFromWhatsapp(string WhatsAppNumber, string WhatsAppMessage, string bgcolor, string qrcolor, int qrsize, string icon, int iconSizePercent, int iconSizeBorder)
        {
            if (WhatsAppNumber.Length > 11)
            {
                return BadRequest("whatsapp numarası en fazla 11 karakter olmalıdır");
            }
            if (WhatsAppMessage.Length > 300)
            {
                return BadRequest("whatsapp mesajı en fazla 300 karakter olmalıdır");
            }
            var payload = new WhatsAppMessage(WhatsAppNumber, WhatsAppMessage);
            var model = modelCreate(bgcolor, qrcolor, qrsize, icon, iconSizePercent, iconSizeBorder);
            return GenerateQRCodeFile(payload, model);
        }

        [HttpPost("email")]
        public IActionResult GenerateQRCodeFromEmail(string ReceiverEmailAddress, string EmailSubject, string EmailMessage, string bgcolor, string qrcolor, int qrsize, string icon, int iconSizePercent, int iconSizeBorder)
        {
            var model = modelCreate(bgcolor, qrcolor, qrsize, icon, iconSizePercent, iconSizeBorder);
            Payload payload = new Mail(ReceiverEmailAddress, EmailSubject, EmailMessage);
            return GenerateQRCodeFile(payload, model);
        }

        [HttpPost("wifi")]
        public IActionResult GenerateQRCodeFromWifi(string WIFIName, string WIFIPassword, string bgcolor, string qrcolor, int qrsize, string icon, int iconSizePercent, int iconSizeBorder)
        {
            var model = modelCreate(bgcolor, qrcolor, qrsize, icon, iconSizePercent, iconSizeBorder);
            Payload payload = new WiFi(WIFIName, WIFIPassword, WiFi.Authentication.WPA);
            return GenerateQRCodeFile(payload, model);
        }

        [HttpPost("ig")]
        public IActionResult GenerateQRCodeFromInstagram(string igUserName, string bgcolor, string qrcolor, int qrsize, string icon, int iconSizePercent, int iconSizeBorder)
        {
            var model = modelCreate(bgcolor, qrcolor, qrsize, icon, iconSizePercent, iconSizeBorder);
            Payload payload = new QRCoder.PayloadGenerator.Url("https://instagram.com/" + igUserName);
            return GenerateQRCodeFile(payload, model);
        }

        [HttpPost("twitter")]
        public IActionResult GenerateQRCodeFromTwitter(string xUserName, string bgcolor, string qrcolor, int qrsize, string icon, int iconSizePercent, int iconSizeBorder)
        {
            var model = modelCreate(bgcolor, qrcolor, qrsize, icon, iconSizePercent, iconSizeBorder);
            Payload payload = new QRCoder.PayloadGenerator.Url("https://twitter.com/" + xUserName);
            return GenerateQRCodeFile(payload, model);
        }

        [HttpPost("btcaddress")]
        public IActionResult GenerateQRCodeFromBtcAddress(string btcAddress, double? BtcAmount, string BtcMessage, string bgcolor, string qrcolor, int qrsize, string icon, int iconSizePercent, int iconSizeBorder)
        {
            var model = modelCreate(bgcolor, qrcolor, qrsize, icon, iconSizePercent, iconSizeBorder);
            BitcoinLikeCryptoCurrencyType currencyType = BitcoinLikeCryptoCurrencyType.Bitcoin;
            Payload payload = new QRCoder.PayloadGenerator.BitcoinLikeCryptoCurrencyAddress(currencyType, btcAddress, BtcAmount, null, BtcMessage);
            return GenerateQRCodeFile(payload, model);
        }

        [HttpPost("geo")]
        public IActionResult GenerateQRCodeFromGeo(double latitude, double longitude, string bgcolor, string qrcolor, int qrsize, string icon, int iconSizePercent, int iconSizeBorder)
        {
            var model = modelCreate(bgcolor, qrcolor, qrsize, icon, iconSizePercent, iconSizeBorder);
            Payload payload = new QRCoder.PayloadGenerator.Geolocation(latitude.ToString(), longitude.ToString());
            return GenerateQRCodeFile(payload, model);
        }

        [HttpPost("phone")]
        public IActionResult GenerateQRCodeFromPhone(string phoneNumber, string bgcolor, string qrcolor, int qrsize, string icon, int iconSizePercent, int iconSizeBorder)
        {
            var model = modelCreate(bgcolor, qrcolor, qrsize, icon, iconSizePercent, iconSizeBorder);
            if (phoneNumber.Length > 11)
            {
                return BadRequest("telefon numarası en fazla 11 karakter olmalıdır");
            }
            Payload payload = new QRCoder.PayloadGenerator.PhoneNumber(phoneNumber);
            return GenerateQRCodeFile(payload, model);
        }

        [HttpPost("onetimepassword")]
        public IActionResult GenerateQRCodeFromOneTimePassword(string Secret, string Issuer, string Label, int Period, string bgcolor, string qrcolor, int qrsize, string icon, int iconSizePercent, int iconSizeBorder)
        {
            var model = modelCreate(bgcolor, qrcolor, qrsize, icon, iconSizePercent, iconSizeBorder);
            OneTimePassword generator = new OneTimePassword
            {
                Secret = Secret,
                Issuer = Issuer,
                Label = Label,
                Period = Period,
            };
            Payload payload = generator;
            return GenerateQRCodeFile(payload, model);
        }

        [HttpPost("litecoinaddress")]
        public IActionResult GenereteQRCodeFromLitecoinAddress(string LitecoinAddress, double? LitecoinAmount, string LitecoinMessage, string bgcolor, string qrcolor, int qrsize, string icon, int iconSizePercent, int iconSizeBorder)
        {
            var model = modelCreate(bgcolor, qrcolor, qrsize, icon, iconSizePercent, iconSizeBorder);
            Payload payload = new QRCoder.PayloadGenerator.LitecoinAddress(LitecoinAddress, LitecoinAmount, LitecoinMessage);
            return GenerateQRCodeFile(payload, model);
        }

        [HttpPost("vcard")]
        public IActionResult GenereteQRCodeFromVCF(string firstname, string lastname, string nickname, string mobilePhone, string country, string note, string website, string city, string email, string bgcolor, string qrcolor, int qrsize, string icon, int iconSizePercent, int iconSizeBorder)
        {

            if (mobilePhone.Length > 11) return BadRequest("Telefon numarası en fazla 11 karakter olmalıdır");
            if (website.Length > 100) return BadRequest("Website URL en fazla 100 karakter olmalıdır");
            if (city.Length > 100) return BadRequest("Şehir en fazla 100 karakter olmalıdır");
            if (email.Length > 100) return BadRequest("E-mail en fazla 100 karakter olmalıdır");
            if (country.Length > 100) return BadRequest("Ülke en fazla 100 karakter olmalıdır");
            if (note.Length > 200) return BadRequest("Not en fazla 200 karakter olmalıdır");
            var model = modelCreate(bgcolor, qrcolor, qrsize, icon, iconSizePercent, iconSizeBorder);
            ContactData vcardData = new ContactData(
                ContactData.ContactOutputType.VCard3,
                firstname, lastname, nickname,
                null, mobilePhone,
                null, email, null, website, null, null, city,
                null, country, note, null
                );
            return GenerateQRCodeFile(vcardData, model);

        }

        [HttpPost("vfc")]
        public IActionResult GenerateQRCodeFromVCF(string vcfPath, string bgcolor, string qrcolor, int qrsize, string icon, int iconSizePercent, int iconSizeBorder)
        {
            var model = modelCreate(bgcolor, qrcolor, qrsize, icon, iconSizePercent, iconSizeBorder);
            Payload payload = new vcfPayload(vcfPath);
            return GenerateQRCodeFile(payload, model);
        }

        [HttpPost("music")]
        public IActionResult GenerateQRCodeFromMusic(string musicURL, string bgcolor, string qrcolor, int qrsize, string icon, int iconSizePercent, int iconSizeBorder)
        {
            Payload payload = new PayloadGenerator.Url(musicURL);
            var model = modelCreate(bgcolor, qrcolor, qrsize, icon, iconSizePercent, iconSizeBorder);
            return GenerateQRCodeFile(payload, model);
        }
    }
}

