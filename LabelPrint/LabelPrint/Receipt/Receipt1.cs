using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
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
using System.IO;
using LabelPrint.Data;
namespace LabelPrint.Receipt
{
    public class Receipt1 : ReceiptPrintPattern
    {

        void CreateDefaultPrintLabel()
        {

        }
        public static List<PrintInfo> printProductLabel(Graphics g, int width, int height)
        {

            float x1 = Pixel2MM(12.0F);
            float y1 = Pixel2MM(26.0F);
            float h = Pixel2MM(20);
            float x2 = Pixel2MM(width / 2);
            int mmwidth = (int)Pixel2MM(width);
            int line_x1 = (int)Pixel2MM(85);
            int line_x2 = (int)Pixel2MM(width / 2 + 75);
            int start_y1 = (int)Pixel2MM(12);
            int margin_1 = (int)Pixel2MM(5);
            Pen pen2 = new Pen(Color.Black, Pixel2MM(2));
            Pen pen1 = new Pen(Color.Black, Pixel2MM(1));

            PrintHelper helper = new PrintHelper();
            List<PrintInfo> lstPrintInfos = new List<PrintInfo>();


            PrintInfo p18 = new PrintInfo()
            {
                PrtType = PrintType.PRINT_TEXT,
                PrtColor = Color.Black,
                Content = "中国.上海惠普贸易有限公司",
                Size = 12,
                FontStyle = FontStyle.Regular,
                Start = new Point_float((int)Pixel2MM(80), (int)Pixel2MM(26)),
            };
            PrintInfo p17 = new PrintInfo()
            {
                PrtType = PrintType.PRINT_TEXT,
                PrtColor = Color.Black,
                Content = "客户姓名",
                Size = 10,
                FontStyle = FontStyle.Regular,
                Start = new Point_float((int)x1, (int)(y1 + h)),
            };
            PrintInfo p16 = new PrintInfo()
            {
                PrtType = PrintType.PRINT_TEXT,
                PrtColor = Color.Black,
                Content = "原材料代码",
                Size = 10,
                FontStyle = FontStyle.Regular,
                Start = new Point_float((int)x1, (int)(y1 + 2 * h)),
            };

            PrintInfo p15 = new PrintInfo()
            {
                PrtType = PrintType.PRINT_TEXT,
                PrtColor = Color.Black,
                Content = "原材料代码",
                Size = 10,
                FontStyle = FontStyle.Regular,
                Start = new Point_float((int)x1, (int)(y1 + 2 * h)),
            };
            PrintInfo p14 = new PrintInfo()
            {
                PrtType = PrintType.PRINT_TEXT,
                PrtColor = Color.Black,
                Content = "材料名称",
                Size = 10,
                FontStyle = FontStyle.Regular,
                Start = new Point_float((int)x1, (int)(y1 + 3 * h)),
            };
            PrintInfo p13 = new PrintInfo()
            {
                PrtType = PrintType.PRINT_TEXT,
                PrtColor = Color.Black,
                Content = "宽度",
                Size = 10,
                FontStyle = FontStyle.Regular,
                Start = new Point_float((int)x1, (int)(y1 + 4 * h)),
            };
            PrintInfo p12 = new PrintInfo()
            {
                PrtType = PrintType.PRINT_TEXT,
                PrtColor = Color.Black,
                Content = "产品批号",
                Size = 10,
                FontStyle = FontStyle.Regular,
                Start = new Point_float((int)x2, (int)(y1 + h)),
            };

            PrintInfo p11 = new PrintInfo()
            {
                PrtType = PrintType.PRINT_TEXT,
                PrtColor = Color.Black,
                Content = "生产者",
                Size = 10,
                FontStyle = FontStyle.Regular,
                Start = new Point_float((int)x2, (int)(y1 + 2 * h)),
            };


            PrintInfo p10 = new PrintInfo()
            {
                PrtType = PrintType.PRINT_LINE,
                PrtColor = Color.Black,
                PenWidth = Pixel2MM(1),
                Start = new Point_float(line_x1, (int)(y1 + h + start_y1)),
                End = new Point_float(mmwidth / 2 - margin_1, (int)(y1 + h + start_y1)),
            };

            PrintInfo p9 = new PrintInfo()
            {
                PrtType = PrintType.PRINT_LINE,
                PrtColor = Color.Black,
                PenWidth = Pixel2MM(1),
                Start = new Point_float(line_x1, (int)(y1 + 2 * h + start_y1)),
                End = new Point_float(mmwidth / 2 - margin_1, (int)(y1 + 2 * h + start_y1)),
            };

            PrintInfo p8 = new PrintInfo()
            {
                PrtType = PrintType.PRINT_LINE,
                PrtColor = Color.Black,
                PenWidth = Pixel2MM(1),
                Start = new Point_float(line_x1, (int)(y1 + 3 * h + start_y1)),
                End = new Point_float(mmwidth / 2 - margin_1, (int)(y1 + 3 * h + start_y1)),
            };

            PrintInfo p7 = new PrintInfo()
            {
                PrtType = PrintType.PRINT_LINE,
                PrtColor = Color.Black,
                PenWidth = Pixel2MM(1),
                Start = new Point_float(line_x1, (int)(y1 + 4 * h + start_y1)),
                End = new Point_float(mmwidth / 2 - margin_1, (int)(y1 + 4 * h + start_y1)),
            };

            PrintInfo p6 = new PrintInfo()
            {
                PrtType = PrintType.PRINT_LINE,
                PrtColor = Color.Black,
                PenWidth = Pixel2MM(1),
                Start = new Point_float(line_x2, (int)(y1 + h + start_y1)),
                End = new Point_float(mmwidth - margin_1, (int)(y1 + h + start_y1)),
            };

            PrintInfo p5 = new PrintInfo()
            {
                PrtType = PrintType.PRINT_LINE,
                PrtColor = Color.Black,
                PenWidth = Pixel2MM(1),
                Start = new Point_float(line_x2, (int)(y1 + 2 * h + start_y1)),
                End = new Point_float(mmwidth - margin_1, (int)(y1 + 2 * h + start_y1)),
            };

            PrintInfo p4 = new PrintInfo()
            {
                PrtType = PrintType.PRINT_LINE,
                PrtColor = Color.Black,
                PenWidth = Pixel2MM(1),
                Start = new Point_float(line_x2, (int)(y1 + 3 * h + start_y1)),
                End = new Point_float(mmwidth - margin_1, (int)(y1 + 3 * h + start_y1)),
            };

            PrintInfo p3 = new PrintInfo()
            {
                PrtType = PrintType.PRINT_LINE,
                PrtColor = Color.Black,
                PenWidth = Pixel2MM(1),
                Start = new Point_float((int)line_x2, (int)(y1 + 4 * h + start_y1)),
                End = new Point_float(mmwidth - margin_1, (int)(y1 + 4 * h + start_y1)),
            };

            PrintInfo p2 = new PrintInfo()
            {
                PrtType = PrintType.PRINT_TEXT,
                PrtColor = Color.Black,
                Content = "卷号",
                Size = 10,
                FontStyle = FontStyle.Regular,
                Start = new Point_float((int)x2, (int)(y1 + 3 * h)),
            };

            PrintInfo p1 = new PrintInfo()
            {
                PrtType = PrintType.PRINT_TEXT,
                PrtColor = Color.Black,
                Content = "卷重/卷长",
                Size = 10,
                FontStyle = FontStyle.Regular,
                Start = new Point_float((int)x2, (int)(y1 + 4 * h)),
            };
            lstPrintInfos.Add(p18);
            lstPrintInfos.Add(p17);
            lstPrintInfos.Add(p16);
            lstPrintInfos.Add(p15);
            lstPrintInfos.Add(p14);
            lstPrintInfos.Add(p13);
            lstPrintInfos.Add(p12);
            lstPrintInfos.Add(p11);
            lstPrintInfos.Add(p10);
            lstPrintInfos.Add(p9);
            lstPrintInfos.Add(p8);
            lstPrintInfos.Add(p7);
            lstPrintInfos.Add(p6);
            lstPrintInfos.Add(p5);
            lstPrintInfos.Add(p4);
            lstPrintInfos.Add(p3);
            lstPrintInfos.Add(p2);
            lstPrintInfos.Add(p1);

            /////////////////////////////////////////////
            CutSampleData SysData;

            SysData = CutSampleData.Instance;

          //  CutProductItem ProdItem = SysData.GetCurProductItem();
         //   PrintInfo p19 = new PrintInfo()
         //   {
         //       PrtType = PrintType.PRINT_CUSTOM,
         //       DynamicTextData = DynamicTextPrintType.CUSTOMER_NAME,
         //       PrtColor = Color.Black,
         // //      Content = ProdItem.CustomerName[0],
         //       Size = 10,
         //       FontStyle = FontStyle.Regular,
         //       Start = new Point_float(line_x1, (y1 + h) - 0.8f),
         //   };
         //   lstPrintInfos.Add(p19);

         //   PrintInfo p21 = new PrintInfo()
         //   {
         //       PrtType = PrintType.PRINT_CUSTOM,
         //       DynamicTextData = DynamicTextPrintType.BATCH_NO,
         //       PrtColor = Color.Black,
         ////       Content = ProdItem.CustomerName[0],
         //       Size = 10,
         //       FontStyle = FontStyle.Regular,
         //       Start = new Point_float(line_x2, (y1 +  h) - 0.8f),
         //   };

         //   lstPrintInfos.Add(p21);
         //   PrintInfo p20 = new PrintInfo()
         //   {
         //       PrtType = PrintType.PRINT_CUSTOM,
         //       DynamicTextData = DynamicTextPrintType.RAW_MATERIAL_CODE,
         //       PrtColor = Color.Black,
         ////       Content = ProdItem.CustomerName[0],
         //       Size = 10,
         //       FontStyle = FontStyle.Regular,
         //       Start = new Point_float(line_x1, (y1 + 2*h) - 0.8f),
         //   };
         //   lstPrintInfos.Add(p20);



         //   PrintInfo p22 = new PrintInfo()
         //   {
         //       PrtType = PrintType.PRINT_CUSTOM,
         //       DynamicTextData = DynamicTextPrintType.WORKER_NO,
         //       PrtColor = Color.Black,
         //   //    Content = ProdItem.CustomerName[0],
         //       Size = 10,
         //       FontStyle = FontStyle.Regular,
         //       Start = new Point_float(line_x2, (y1 + 2 * h) - 0.8f),
         //   };
         //   lstPrintInfos.Add(p22);

         //   PrintInfo p23 = new PrintInfo()
         //   {
         //       PrtType = PrintType.PRINT_CUSTOM,
         //       DynamicTextData = DynamicTextPrintType.MATERIAL_NAME,
         //       PrtColor = Color.Black,
         //   //    Content = ProdItem.CustomerName[0],
         //       Size = 10,
         //       FontStyle = FontStyle.Regular,
         //       Start = new Point_float(line_x1, (y1 + 3 * h) - 0.8f),
         //   };
         //   lstPrintInfos.Add(p23);

         //   PrintInfo p24 = new PrintInfo()
         //   {
         //       PrtType = PrintType.PRINT_CUSTOM,
         //       DynamicTextData = DynamicTextPrintType.LITTLE_ROLL_NO,
         //       PrtColor = Color.Black,
         //    //   Content = ProdItem.CustomerName[0],
         //       Size = 10,
         //       FontStyle = FontStyle.Regular,
         //       Start = new Point_float(line_x2, (y1 + 3 * h) - 0.8f),
         //   };
         //   lstPrintInfos.Add(p24);
         //   PrintInfo p25 = new PrintInfo()
         //   {
         //       PrtType = PrintType.PRINT_CUSTOM,
         //       DynamicTextData = DynamicTextPrintType.WIDTH,
         //       PrtColor = Color.Black,
         //    //   Content = ProdItem.CustomerName[0],
         //       Size = 10,
         //       FontStyle = FontStyle.Regular,
         //       Start = new Point_float(line_x1, (y1 + 4 * h) - 0.8f),
         //   };
         //   lstPrintInfos.Add(p25);

         //   PrintInfo p26 = new PrintInfo()
         //   {
         //       PrtType = PrintType.PRINT_CUSTOM,
         //       DynamicTextData = DynamicTextPrintType.ROLL_WEIGHT_LENGTH,
         //       PrtColor = Color.Black,
         //     //  Content = ProdItem.CustomerName[0],
         //       Size = 10,
         //       FontStyle = FontStyle.Regular,
         //       Start = new Point_float(line_x2, (y1 + 4 * h) - 0.8f),
         //   };
         //   lstPrintInfos.Add(p26);
            PrintInfo p27 = new PrintInfo()
            {
                PrtType = PrintType.PRINT_2DBAR,
                PrtColor = Color.Black,
              //  Content = ProdItem.CustomerName[0],
                Size = 10,
                Start = new Point_float(line_x2-10f, (y1 + 4 * h) +3f),
                End = new Point_float(mmwidth - margin_1, (y1 + 8* h)+10f ),
            };
         //   lstPrintInfos.Add(p27);
            #region test dynamic print labe;
            //DynamicPrintLabelData prinData = new DynamicPrintLabelData();
            SystemSetting SysSetting;
            SysSetting = GlobalConfig.Setting;

            // prinData.CustomName = ;
            #endregion
            return lstPrintInfos;
//            UpdateReceiptDynamicPrintData(lstPrintInfos, SysSetting.DynPrintData);
  //          helper.PrintInfos = lstPrintInfos;
   //         helper.Print(g);
            //printProductLabel1(g, width, height);
        }





    }



    public class Receipt1PrintPatternFactory : ReceiptPatternFactory
    {
        public override ReceiptPrintPattern CreateReceiptPrintPattern()
        {
            return new Receipt1();
        }
    }
}
