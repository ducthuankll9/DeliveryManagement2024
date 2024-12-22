using QRCoder;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;

namespace DeliveryManagement.Helper
{
    public class MyQRCodeHelper
    {
        public static string GenerateAndSaveQRCode(string data)
        {
            try
            {
                // path to folder OutputData
                //string folderPath = Server.MapPath("~/OutputData/");
                string folderPath = Constants.Path_Image;

                // Check folder is exist or not, if not generate new folder
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                // full path to QR img
                string fullPath = Path.Combine(folderPath, data + ".png");

                // Generate QRCode from data
                QRCodeGenerator generator = new QRCodeGenerator();
                QRCodeData qrCodeData = generator.CreateQrCode(data, QRCodeGenerator.ECCLevel.Q);
                QRCode qrCode = new QRCode(qrCodeData);
                // note: used "using" to avoid exception
                using (Bitmap qrCodeImg = qrCode.GetGraphic(10))
                {
                    // Save img to folder on server as png file
                    qrCodeImg.Save(fullPath, ImageFormat.Png);
                }

                // return the path to QR file
                return fullPath;
            }
            catch
            {
                string path500Err = @"\Images\500-internal-server-error.png";
                return path500Err;
            }

        }
    }
}