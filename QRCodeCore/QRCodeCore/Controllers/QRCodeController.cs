using Microsoft.AspNetCore.Mvc;
using QRCodeCore.Models;
using QRCoder;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using static QRCoder.PayloadGenerator;

namespace QRCodeCore.Controllers
{
    public class QRCodeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(QRCodeModel model)
        {
            Payload payload = null;
            switch (model.QRCodeType)
            {
                case "web":
                    payload = new Url(model.WebSiteURL);
                    break;
                case "bookmark":
                    payload = new Bookmark(model.BookMarkURL, model.BookMarkURL);
                    break;
                case "sms":
                    payload = new SMS(model.SMSPhoneNumber, model.SMSBody);
                    break;
                case "email":
                    payload = new Mail(model.ReceiverEmailAddress, model.EmailSubject, model.EmailMessage);
                    break;
                case "wifi":
                    payload = new WiFi(model.WIFIName, model.WIFIPassWord, WiFi.Authentication.WPA);
                    break;
            }

            QRCodeGenerator generator = new QRCodeGenerator();
            QRCodeData qrCodeData = generator.CreateQrCode(payload);
            QRCode qrCode = new QRCode(qrCodeData);
            var qrCodeAsBitmap = qrCode.GetGraphic(20);

            string base64String = Convert.ToBase64String(BitmapToByteArray(qrCodeAsBitmap));
            model.QRImageURL = "data:image/png;base64," + base64String;
            return View("Index", model);
        }

        private byte[] BitmapToByteArray(Bitmap bitmap)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                bitmap.Save(ms, ImageFormat.Png);
                return ms.ToArray();
            }
        }
    }
}
