using DeliveryManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Excel = Microsoft.Office.Interop.Excel;
using System.Drawing;
using System.IO;
using System.Windows;

namespace DeliveryManagement.Helper
{
    public class ExcelHelper
    {
        public static bool CopyExcelFile(string source, string destination)
        {
            try
            {
                if (File.Exists(source))
                {
                    File.Copy(source, destination, true);
                    return true;
                }
                else
                {
                    return false;
                }
            } catch 
            {
                return false;
            }
        }

        public static bool CreateBill(string filepath, Order order, string QRpath, string pdfOutputPath)
        {
            // open a process Excel, open file .xlsx, focus to sheet1
            Excel.Application application = new Excel.Application();
            Excel.Workbook book = application.Workbooks.Open(filepath);
            Excel.Worksheet sheet = book.Sheets[1];

            try
            {
                // set value of order to sheet
                sheet.Cells[6, 1].Value = order.SenderName;     // cell A6
                sheet.Cells[7, 1].Value = order.SenderPhone;     // cell A7
                sheet.Cells[8, 1].Value = order.SenderAddress;     // cell A8
                sheet.Cells[11, 1].Value = order.ReceiverName;     // cell A11
                sheet.Cells[12, 1].Value = order.ReceiverPhone;     // cell A12
                sheet.Cells[13, 1].Value = order.ReceiverAddress;     // cell A13
                sheet.Cells[16, 1].Value = order.Fee;     // cell A16
                sheet.Cells[16, 1].HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
                if (order.Paid)
                {
                    sheet.Cells[17, 1].Value = "Đã thanh toán";     // cell A17
                }
                else
                {
                    sheet.Cells[17, 1].Value = "Chưa thanh toán";     // cell A17
                }

                // check QR image
                if (File.Exists(QRpath))
                {
                    // image is exist, get image size first
                    int width;
                    //// note: used "using" to avoid exception "Out of memory"
                    //using (Image image = Image.FromFile(QRpath))
                    //{
                    //    width = (int)Math.Floor(image.Width * 0.5);
                    //}
                    width = (int)(sheet.Range["D5"].Left - sheet.Range["C5"].Left);
                    sheet.Shapes.AddPicture(QRpath, Microsoft.Office.Core.MsoTriState.msoFalse, Microsoft.Office.Core.MsoTriState.msoCTrue,
                                            sheet.Range["C5"].Left, sheet.Range["C5"].Top, width, width);

                    // DELETE image file after import
                    // File.Delete(QRpath);

                    sheet.Cells[4, 3].Value = order.OrderID;     // cell C4
                    sheet.Cells[4, 3].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                }

                // save to PDF
                //string pdfFilename = Path.GetFileName(Path.ChangeExtension(filepath,"pdf"));
                book.ExportAsFixedFormat(Excel.XlFixedFormatType.xlTypePDF, pdfOutputPath);

                // save and close workbook
                book.Save();
                book.Close();

                //// if export success, DELETE excel file
                //if (File.Exists(Constants.Path_PDF + pdfFilename))
                //{
                //    File.Delete(filepath);
                //}

                // release  memory
                System.Runtime.InteropServices.Marshal.ReleaseComObject(sheet);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(book);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(application);

                return true;
            } 
            catch   //(Exception ex)
            {
                book.Save();
                book.Close();
                return false;
            }
        }
    }
}