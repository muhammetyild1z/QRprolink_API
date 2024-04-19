using Microsoft.AspNetCore.Http;


namespace QRprolink_API.Models
{
    public class QRCustomizeModel
    {
        //QR customize
        public string bgcolor { get; set; }
        public string qrcolor { get; set; }
        public int qrsize { get; set; }
        public string icon { get; set; }
        public int iconSizePercent { get; set; }
        public int iconSizeBorder { get; set; }
    }
}
