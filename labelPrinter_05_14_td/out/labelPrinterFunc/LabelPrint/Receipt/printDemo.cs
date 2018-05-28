using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using System;
//using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Printing;
using System.Drawing.Imaging;
using ZXing.QrCode;
using ZXing;
using ZXing.Common;
using ZXing.Rendering;
using LabelPrinting;
//using LabelPrinting.Receipt;
using LabelPrint.Receipt;
using LabelPrint.Label;
using LabelPrint.Data;
using LabelPrint.Util;
using System.IO;
namespace LabelPrint
{


    public class PrintHelper
    {
        #region constructor and properties

        /// <summary>
        /// 需要打印的内容
        /// </summary>
        public List<PrintInfo> PrintInfos { get; set; }

        public PrintHelper()
        {

        }

        #endregion

        /// <summary>
        /// 打印
        /// </summary>
        /// <param name="g"></param>
        public void Print(Graphics g)
        {
            try
            {
                if (this.PrintInfos != null && this.PrintInfos.Count > 0)
                {
                    foreach (PrintInfo p in this.PrintInfos)
                    {
                        switch (p.PrtType)
                        {
                            case PrintType.PRINT_CUSTOM:
                            case PrintType.PRINT_TEXT:
                                if (p.Content != null && p.Content != "")
                                {
                                    Font tFont = new Font("宋体", p.Size, p.FontStyle);
                                    Brush b = new SolidBrush(p.PrtColor);
                                    g.DrawString(p.Content, tFont, b, p.Start.X, p.Start.Y);
                                }
                                break;
                            case PrintType.PRINT_TABLE:
                                float distance_h = (p.End.Y - p.Start.Y) * 1.0f / p.Row;//横线之间的间距
                                float distance_w = (p.End.X - p.Start.X) * 1.0f / p.Column;//竖线之间的间距
                                Pen lineColor = new Pen(p.PrtColor, 0.2f);
                                for (int i = 0; i < p.Row + 1; i++)
                                {
                                    //画横线
                                    float y = p.Start.Y + (i) * distance_h;
                                    g.DrawLine(lineColor, new PointF(p.Start.X, y), new PointF(p.End.X, y));
                                }
                                for (int i = 0; i < p.Column + 1; i++)
                                {
                                    //画竖线
                                    float x = p.Start.X + (i) * distance_w;
                                    g.DrawLine(lineColor, new PointF(x, p.Start.Y), new PointF(x, p.End.Y));
                                }
                                break;
                            case PrintType.PRINT_LINE:
                                Pen pen1 = new Pen(p.PrtColor, p.PenWidth);
                                g.DrawLine(pen1, p.Start.X, p.Start.Y, p.End.X, p.End.Y);
                                break;
                            case PrintType.PRINT_RECT:
                                g.DrawRectangle(new Pen(p.PrtColor, 1), p.Start.X, p.Start.Y, (p.End.X - p.Start.X), (p.End.Y - p.Start.Y));
                                break;

                            case PrintType.PRINT_BAR:
                                {
                                    try
                                    {
                                        if (p.Content == null || p.Content == "")
                                            continue;
                                        //Bitmap bm = ReceiptPrintPattern.GetCod39ZXingNet(p.Content, (int)ReceiptPrintPattern.MM2Pixel(p.End.X - p.Start.X), (int)ReceiptPrintPattern.MM2Pixel(p.End.Y - p.Start.Y));
                                        Bitmap bm = ReceiptPrintPattern.GetCod128ZXingNet(p.Content, (int)ReceiptPrintPattern.MM2Pixel(p.End.X - p.Start.X), (int)ReceiptPrintPattern.MM2Pixel(p.End.Y - p.Start.Y));
                                        if (bm == null)
                                            continue;

                                        //e.DrawImage(bm, new Point(10, (int)(y1 + 4 * h + 12 + 10)));
                                        //Bitmap bm = GetQRCodeByZXingNet("Hello world.Hahahaha!", this.NodeWith, this.NodeHeight);
                                        g.DrawImage(bm, new PointF(p.Start.X, p.Start.Y));

                                        bm.Dispose();
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine("Print graphic failed in barcode printing!" + ex);
                                    }
                                }
                                break;
                            case PrintType.PRINT_2DBAR:
                                {
                                    try
                                    {
                                        if (p.Content == null || p.Content == "")
                                            continue;

                                        Bitmap bm = ReceiptPrintPattern.GetQRCodeByZXingNet(p.Content, (int)ReceiptPrintPattern.MM2Pixel(p.End.X - p.Start.X), (int)ReceiptPrintPattern.MM2Pixel(p.End.Y - p.Start.Y));

                                        if (bm == null)
                                            continue;
                                        //e.DrawImage(bm, new Point(10, (int)(y1 + 4 * h + 12 + 10)));
                                        //Bitmap bm = GetQRCodeByZXingNet("Hello world.Hahahaha!", this.NodeWith, this.NodeHeight);
                                        g.DrawImage(bm, new PointF(p.Start.X, p.Start.Y));
                                        g.DrawRectangle(new Pen(p.PrtColor, ReceiptPrintPattern.Pixel2MM(1)), p.Start.X, p.Start.Y, (p.End.X - p.Start.X), (p.End.Y - p.Start.Y));

                                        bm.Dispose();
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine("Print graphic failed in 2D mode!" + ex);
                                    }
                                }
                                break;
                            case PrintType.PRINT_IMAGE:
                                {

                                }
                                break;
                            case PrintType.PRINT_BLOCK:
                                {
                                    g.FillRectangle(new SolidBrush(p.PrtColor), p.Start.X, p.Start.Y, (p.End.X - p.Start.X), (p.End.Y - p.Start.Y));
                                }
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Print graphic failed!" + ex);
            }
        }
    }

    public class Point_float
    {
        public Point_float(float x, float y)
        {
            X = x;
            Y = y;
        }
        //
        // Summary:
        //     Gets or sets the x-coordinate of this System.Drawing.Point.
        //
        // Returns:
        //     The x-coordinate of this System.Drawing.Point.
        public float X { get; set; }
        //
        // Summary:
        //     Gets or sets the y-coordinate of this System.Drawing.Point.
        //
        // Returns:
        //     The y-coordinate of this System.Drawing.Point.
        public float Y { get; set; }

    }
    public class PrintInfo
    {

        /// <summary>
        /// 打印类型
        /// </summary>
        public PrintType PrtType { get; set; }

        /// <summary>
        /// 打印颜色
        /// </summary>
        public Color PrtColor { get; set; }

        public float PenWidth { get; set; }
        /// <summary>
        /// 起始位置
        /// </summary>

        public Point_float Start { get; set; }

        /// <summary>
        /// 结束位置
        /// </summary>
        public Point_float End { get; set; }

        /// <summary>
        /// 大小
        /// </summary>
        public float Size { get; set; }

        public FontStyle FontStyle { get; set; }

        /// <summary>
        /// 打印内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 列数
        /// </summary>
        public int Column { get; set; }

        /// <summary>
        /// 行数
        /// </summary>
        public int Row { get; set; }

        public DynamicTextPrintType DynamicTextData { get; set; }
    }

    /// <summary>
    /// 打印类型
    /// </summary>
    public enum PrintType
    {
        PRINT_TEXT = 0,
        PRINT_TABLE = 1,
        PRINT_LINE = 2,
        PRINT_RECT = 3,
        PRINT_BAR = 4,
        PRINT_2DBAR = 5,
        PRINT_IMAGE = 6,
        PRINT_BLOCK = 7,
        PRINT_CUSTOM = 100,
    }
    class LabelPattern
    {
        public String Desc;
        public String Version;
        public int Width;
        public int Height;
        public List<PrintInfo> receiptInfos;
    };

    public class ReceiptPrintPattern
    {

        public List<PrintInfo> PrintInfo;

        public static int MM2Pixel(float mm)
        {
            return (int)(mm * 96.0f / 25.4f);
        }
        public static float Pixel2MM(float pixel)
        {
            return (float)(pixel * 25.4 / 96.0f);
        }
        public static Bitmap GetCod39ZXingNet(String strMessage, Int32 width, Int32 height)
        {
            Bitmap result = null;
            if (strMessage == null || strMessage == "")
                return null;
            try
            {
                BarcodeWriter barcodewritter = new BarcodeWriter
                {
                    Format = ZXing.BarcodeFormat.CODE_39,
                    Options = new ZXing.Common.EncodingOptions
                    {
                        Height = height,
                        Width = width,
                        Margin = 30,
                        PureBarcode = false
                    }
                };

                ZXing.Common.BitMatrix bm = barcodewritter.Encode(strMessage);
                result = barcodewritter.Write(bm);
            }
            catch (Exception ex)
            {
                throw ex;
                //????
            }
            return result;
        }
        public static Bitmap GetCod128ZXingNet(String strMessage, Int32 width, Int32 height, Boolean showStr=true)
        {
            Bitmap result = null;
            if (strMessage == null || strMessage == "")
                return null;
            try
            {
                BarcodeWriter barcodewritter = new BarcodeWriter
                {
                    Format = ZXing.BarcodeFormat.CODE_128,
                    Options = new ZXing.Common.EncodingOptions
                    {
                        Height = height,
                        Width = width,
                        Margin = 30,
                        PureBarcode = false,
                    }
                };

                if (showStr)
                    result = barcodewritter.Write(strMessage);
                else { 
                    ZXing.Common.BitMatrix bm = barcodewritter.Encode(strMessage);
                    result = barcodewritter.Write(bm);
                }
            }
            catch (Exception ex)
            {
                throw ex;
                //????
            }
            return result;
        }
        public static Bitmap GetQRCodeByZXingNet(String strMessage, Int32 width, Int32 height)
        {
            Bitmap result = null;
            if (strMessage == null || strMessage == "")
                return null;
            try
            {
                BarcodeWriter barCodeWriter = new BarcodeWriter();
                barCodeWriter.Format = BarcodeFormat.QR_CODE;
                barCodeWriter.Options.Hints.Add(EncodeHintType.CHARACTER_SET, "UTF-8");
                barCodeWriter.Options.Hints.Add(EncodeHintType.ERROR_CORRECTION, ZXing.QrCode.Internal.ErrorCorrectionLevel.H);
                barCodeWriter.Options.Height = height;
                barCodeWriter.Options.Width = width;
                barCodeWriter.Options.Margin = 0;
                ZXing.Common.BitMatrix bm = barCodeWriter.Encode(strMessage);
                result = barCodeWriter.Write(bm);
            }
            catch (Exception ex)
            {
                throw ex;
                //????
            }
            return result;
        }

        public static void printProductLabel1(Graphics g, int width, int height)
        {
            float x1 = 12.0F;
            float y1 = 26.0F;
            float h = 20;
            float x2 = width / 2;
            Pen pen2 = new Pen(Color.Black, 2);
            Pen pen1 = new Pen(Color.Black, 1);
            // g.DrawRectangle(pen2, 10, 10, width - 10, height - 10);
            string str = "中国.上海惠普贸易有限公司";
            g.DrawString(str, new Font(new FontFamily("宋体"), 12), System.Drawing.Brushes.Black, 20, 26);
            //  g.DrawLine(pen1, new Point(50, 26+12), new Point(200,26+12));
            str = "客户姓名";
            g.DrawString(str, new Font(new FontFamily("宋体"), 10), System.Drawing.Brushes.Black, x1, y1 + h);

            g.DrawLine(pen1, new Point(85, (int)(y1 + h + 12)), new Point(width / 2 - 5, (int)(y1 + h + 12)));
            str = "原材料代码";
            g.DrawString(str, new Font(new FontFamily("宋体"), 10), System.Drawing.Brushes.Black, x1, y1 + 2 * h);
            g.DrawLine(pen1, new Point(85, (int)(y1 + 2 * h + 12)), new Point(width / 2 - 5, (int)(y1 + 2 * h + 12)));
            str = "材料名称";
            g.DrawString(str, new Font(new FontFamily("宋体"), 10), System.Drawing.Brushes.Black, x1, y1 + 3 * h);
            g.DrawLine(pen1, new Point(85, (int)(y1 + 3 * h + 12)), new Point(width / 2 - 5, (int)(y1 + 3 * h + 12)));
            str = "宽度";
            g.DrawString(str, new Font(new FontFamily("宋体"), 10), System.Drawing.Brushes.Black, x1, y1 + 4 * h);
            g.DrawLine(pen1, new Point(85, (int)(y1 + 4 * h + 12)), new Point(width / 2 - 5, (int)(y1 + 4 * h + 12)));
            str = "产品批号";
            g.DrawString(str, new Font(new FontFamily("宋体"), 10), System.Drawing.Brushes.Black, x2, y1 + h);
            g.DrawLine(pen1, new Point(width / 2 + 75, (int)(y1 + h + 12)), new Point(width - 5, (int)(y1 + h + 12)));
            str = "生产者";
            g.DrawString(str, new Font(new FontFamily("宋体"), 10), System.Drawing.Brushes.Black, x2, y1 + 2 * h);
            g.DrawLine(pen1, new Point(width / 2 + 75, (int)(y1 + 2 * h + 12)), new Point(width - 5, (int)(y1 + 2 * h + 12)));
            str = "卷号";
            g.DrawString(str, new Font(new FontFamily("宋体"), 10), System.Drawing.Brushes.Black, x2, y1 + 3 * h);
            g.DrawLine(pen1, new Point(width / 2 + 75, (int)(y1 + 3 * h + 12)), new Point(width - 5, (int)(y1 + 3 * h + 12)));
            str = "卷重/卷长";

            PrintInfo p = new PrintInfo()
            {
                PrtType = PrintType.PRINT_TEXT,
                PrtColor = Color.Black,
                Content = str,
                Size = 10,
                FontStyle = FontStyle.Regular,
                Start = new Point_float((int)x2, (int)(y1 + 4 * h)),
            };
            PrintHelper helper = new PrintHelper();
            List<PrintInfo> lstPrintInfos = new List<PrintInfo>();
            lstPrintInfos.Add(p);


            helper.PrintInfos = lstPrintInfos;
            helper.Print(g);

            // g.DrawString(str, new Font(new FontFamily("宋体"), 10), System.Drawing.Brushes.Black, x2, y1 + 4 * h);
            g.DrawLine(pen1, new Point(width / 2 + 75, (int)(y1 + 4 * h + 12)), new Point(width - 5, (int)(y1 + 4 * h + 12)));
            Bitmap bm = GetQRCodeByZXingNet("Hello world.Hahahaha!", 100, 100);
            g.DrawImage(bm, new Point(200, (int)(y1 + 4 * h + 12 + 10)));
            bm = GetCod39ZXingNet("123456", 150, 30);
            g.DrawImage(bm, new Point(10, (int)(y1 + 4 * h + 12 + 10)));
        }


        public static void printProductLabel2(Graphics g, int width, int height)
        {
            PrintHelper helper = new PrintHelper();
            List<PrintInfo> lstPrintInfos = new List<PrintInfo>();
            helper.PrintInfos = lstPrintInfos;

            PrintInfo p = new PrintInfo()
            {
                PrtType = PrintType.PRINT_TEXT,
                PrtColor = Color.Black,
                Content = "王芳",
                Size = 10,
                FontStyle = FontStyle.Regular,
                Start = new Point_float((int)Pixel2MM(99), (int)Pixel2MM(218)),
            };

            lstPrintInfos.Add(p);
            helper.Print(g);
        }

        int mLabelWidth = (int)(100 / 25.4 * 100 + 0.5);
        int mLabelHeight = (int)(60 / 25.4 * 100 + 0.5);

      //  Boolean ShowPreview = true;
        public void PrintReceipt()
        {
            PrintDocument pd = new PrintDocument();
            pd.DefaultPageSettings.PaperSize = new PaperSize("newPage70X40"
           , mLabelWidth
           , mLabelHeight);

            pd.PrintPage += new PrintPageEventHandler(pd_PrintPage);

            if (gVariable.bShowPreview)
            {
                PrintPreviewDialog cppd = new PrintPreviewDialog();
                cppd.Document = pd;
                cppd.ShowDialog();
            }
            else { 
                pd.Print();
            }
        }

        public void printok_image(Graphics g)
        {
            Bitmap me = new Bitmap(mLabelWidth, mLabelHeight);
            TextureBrush Txbrus = new TextureBrush(me);
            Txbrus.WrapMode = System.Drawing.Drawing2D.WrapMode.Tile;
            g.FillRectangle(Txbrus, new Rectangle(0, 0, mLabelWidth, mLabelHeight));

            PrintHelper  helper= new PrintHelper();
            helper.PrintInfos = PrintInfo;
            helper.Print(g);

//            Receipt1.printProductLabel(g, mLabelWidth, mLabelHeight);
        }


        void pd_PrintPage(object sender, PrintPageEventArgs e)
        {
            e.Graphics.PageScale = 1;//原始尺寸
            e.Graphics.PageUnit = GraphicsUnit.Millimeter;//单位毫米
            printok_image(e.Graphics);
        }

        public static void UpdateTextDataItemInfo(PrintInfo dataItem, String content)
        {
            dataItem.Content = content;
        }
        public static void UpdateReceiptDynamicTextPrintData(PrintInfo info, DynamicPrintLabelData printData)
        {

            switch (info.DynamicTextData)
            {
                case DynamicTextPrintType.WorkProcess:
                    ReceiptPrintPattern.UpdateTextDataItemInfo(info, printData.WorkProcess);
                    break;
                case DynamicTextPrintType.ProductCode:
                    ReceiptPrintPattern.UpdateTextDataItemInfo(info, printData.ProductCode);
                    break;
                case DynamicTextPrintType.CustomName:
                    ReceiptPrintPattern.UpdateTextDataItemInfo(info, printData.CustomName);
                    break;
                case DynamicTextPrintType.WorkerNo:
                    ReceiptPrintPattern.UpdateTextDataItemInfo(info, printData.WorkerNo);
                    break;
                case DynamicTextPrintType.RawMaterialCode:
                    ReceiptPrintPattern.UpdateTextDataItemInfo(info, printData.RawMaterialCode);
                    break;
                case DynamicTextPrintType.MaterialName:
                    ReceiptPrintPattern.UpdateTextDataItemInfo(info, printData.RecipeCode);
                    //ReceiptPrintPattern.UpdateTextDataItemInfo(info, printData.MaterialName);
                    break;
                case DynamicTextPrintType.BatchNo:
                    ReceiptPrintPattern.UpdateTextDataItemInfo(info, printData.BatchNo);
                    break;
                case DynamicTextPrintType.Width:
                    ReceiptPrintPattern.UpdateTextDataItemInfo(info, printData.Width);
                    break;
                case DynamicTextPrintType.WorkNo:
                    ReceiptPrintPattern.UpdateTextDataItemInfo(info, printData.WorkNo);
                    break;
                case DynamicTextPrintType.DataTime:
                    ReceiptPrintPattern.UpdateTextDataItemInfo(info, printData.DataTime);
                    break;
                case DynamicTextPrintType.RollWeightLength:
                    ReceiptPrintPattern.UpdateTextDataItemInfo(info, printData.RollWeightLength);
                    break;
                case DynamicTextPrintType.BigRollNoStr:
                    ReceiptPrintPattern.UpdateTextDataItemInfo(info, printData.BigRollNoStr);
                    break;
                case DynamicTextPrintType.LittleRollNoStr:
                    ReceiptPrintPattern.UpdateTextDataItemInfo(info, printData.LittleRollNoStr);
                    break;
                case DynamicTextPrintType.MachineNo:
                    ReceiptPrintPattern.UpdateTextDataItemInfo(info, printData.MachineNo);
                    break;
                case DynamicTextPrintType.Quality:
                    ReceiptPrintPattern.UpdateTextDataItemInfo(info, printData.Quality);
                    break;
                case DynamicTextPrintType.RawMaterialBagCount:
                    ReceiptPrintPattern.UpdateTextDataItemInfo(info, printData.RawMaterialBagCount);
                    break;
                case DynamicTextPrintType.JIZhong:
                    ReceiptPrintPattern.UpdateTextDataItemInfo(info, printData.JIZhong);
                    break;
                case DynamicTextPrintType.SeXiPeiBi:
                    ReceiptPrintPattern.UpdateTextDataItemInfo(info, printData.SeXiPeiBi);
                    break;
                case DynamicTextPrintType.RecipeCode:
                    ReceiptPrintPattern.UpdateTextDataItemInfo(info, printData.RecipeCode);
                    break;
                case DynamicTextPrintType.CheckResult:
                    ReceiptPrintPattern.UpdateTextDataItemInfo(info, printData.CheckResult);
                    break;
                default:
                    break;
            }

        }

        public static void UpdateReceiptPatternDynamicPrintData(List<PrintInfo> listInfo, DynamicPrintLabelData printData)
        {
            foreach (PrintInfo p in listInfo)
            {
                switch (p.PrtType)
                {
                    case PrintType.PRINT_BAR:
                        //ReceiptPrintPattern.UpdateTextDataItemInfo(p, printData.QRBarCode);
                        //break;
                    case PrintType.PRINT_2DBAR:
                        ReceiptPrintPattern.UpdateTextDataItemInfo(p, printData.QRBarCode);
                        break;
                    case PrintType.PRINT_CUSTOM:
                        UpdateReceiptDynamicTextPrintData(p, printData);
                        break;
                    default:
                        break;
                }
            }
        }


        int CalcLabelDimension(int len)
        {
           return  (int)(len / 25.4 * 100 + 0.5);
        }

        public List<PrintInfo> GetPrintLabelFromJsonFile()
        {
            Receipt1 a = new Receipt1();
            //PrintInfo = Receipt1.printProductLabel(null, 400, 400);
            //string fp = System.Windows.Forms.Application.StartupPath + "\\label\\FilmPrint\\FilmPrint1.json";
            String fp = GlobalConfig.Setting.GetCurrentLabelFullName();
            //string fp = System.Windows.Forms.Application.StartupPath + "\\label\\FilmPrint\\";
            //  string[] aaa = GetJsonFileUnderPath(fp);
            if (!File.Exists(fp))  // 判断是否已有相同文件 
            {
                GlobalConfig.Setting.RestoreDefaultLabel();
                GlobalConfig.Setting.SaveSystemSettingsToJsonFile();
                fp = GlobalConfig.Setting.GetCurrentLabelFullName();
            }
            LabelPattern p = JsonHelper.FromJson<LabelPattern>(File.ReadAllText(fp));
            PrintInfo = p.receiptInfos;
            mLabelWidth = CalcLabelDimension(p.Width);
            mLabelHeight = CalcLabelDimension( p.Height);
            return PrintInfo;
        }

        public List<PrintInfo> GetCutPackLabelFromJsonFile()
        {
            Receipt1 a = new Receipt1();

            string fp = System.Windows.Forms.Application.StartupPath + "\\label\\cutpack\\default.json";
            if (!File.Exists(fp))  // 判断是否已有相同文件 
            {
                return null;
            }
            LabelPattern p = JsonHelper.FromJson<LabelPattern>(File.ReadAllText(fp));
            PrintInfo = p.receiptInfos;
            mLabelWidth = CalcLabelDimension(p.Width);
            mLabelHeight = CalcLabelDimension(p.Height);
            return PrintInfo;
        }
    }
}
