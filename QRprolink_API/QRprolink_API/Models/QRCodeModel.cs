using System.ComponentModel.DataAnnotations;
using System.Drawing;

namespace QRprolink_API.Models
{
    public class QRCodeModel
    {

        //QR customize
        public string bgcolor { get; set; }
        public string qrcolor { get; set; }
        //QR customize End
        public int QRCodeType { get; set; }
        //for url qr code
        public string QRImageURL { get; set; }
        //for url qr code
        public string MusicURL { get; set; }
        //for bookmark qr code
        public string BookmarkTitle { get; set; }
        public string BookmarkURL { get; set; }
        //for instagram qr code
        public string igUserName { get; set; }
        //for x qr code
        public string xUserName { get; set; }
        // for email qr codes
        public string ReceiverEmailAddress { get; set; }
        public string EmailSubject { get; set; }
        public string EmailMessage { get; set; }
        //for sms qr codes
        public string SMSPhoneNumber { get; set; }
        public string SMSBody { get; set; }
        //for website
        public string WebsiteURL { get; set; }
        // for whatsapp qr message code
        public string WhatsAppNumber { get; set; }
        public string WhatsAppMessage { get; set; }
        // for wifi qr code
        public string WIFIName { get; set; }
        public string WIFIPassword { get; set; }
        //for btc adress to qr code
        public string BtcAddress { get; set; }
        public double? BtcAmount { get; set; }
        public string BtcMessage { get; set; }
        //for litecoin adress to qr code
        public string LitecoinAddress { get; set; }
        public double? LitecoinAmount { get; set; }
        public string LitecoinMessage { get; set; }
        // for Geolocation  qr code
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        //for phone number to qr code
        [MaxLength(11)]
        public string phoneNumber { get; set; }
        //for One-Time-Password to qr code
        public string Secret { get; set; }
        public string Issuer { get; set; }
        public string Label { get; set; }
        public int Period { get; set; }
        //for vCard  to qr code
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Nickname { get; set; }
        public string MobilePhone { get; set; }
        public string Email { get; set; }
        public string Website { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string Note { get; set; }
        // .vcf to qr code
        public string vcfFilePath { get; set; }
    }
}
