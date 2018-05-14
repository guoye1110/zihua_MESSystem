using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;




using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;
using System.Data;
using LabelPrint.Data;
namespace LabelPrint.Print
{
    public class Report_BangMa : Report
    {
        public String ProductCode;
        public String BatchNo;
        public String MDate;
        public String PlateNo;
        public String VendorName;
        public String BanBie;
        public String Banci;
        //public String Recipe;
        //public String KeZhong;
        //public String width;
        //public String Length;
        //public String MoJuanJing;


        int Count = 0;

        private int Tabx;
        private int Taby;

        string[] BRollIdxs;
        string[] LRollIdxs;
        string[] rollIdxs;
        string[] Weights;
        string[] Joints;


        String Hearder = "上海紫华企业有限公司";
        String SubHearder = "磅 码 单";
        String TProductCode = "产品代号: ";
        String TBatchNo = "生产批号: ";
        String TMDate = "生产日期: ";
        String TPlateNo = "铲板号: ";
        String TVendorName = "供应商名称: ";
        String TTotalWeight = "合计重量:";
        String TBanZu = "班组";
        String TCaoZuoGong = "操作工";
        protected override void InitialVariableForPrint()
        {
            base.InitialVariableForPrint();
            if (DataTablePrint!=null)
            XUnit = new int[DataTablePrint.Columns.Count];
            //   Tabx = SubTitleRows *YUnit +
        }


        String[] Titles = { "序号", "大卷号/小卷号", "重量(kg)", "接头数", "序号", "大卷号/小卷号", "重量(kg)", "接头数" };
        Font Header1 = new Font("Verdana", 15, FontStyle.Bold);


        override protected int GetTotalPrintPage() { return 1; }

        override protected void PrinterPageSetting()
        {
            PageSetupDialog PageSetup = new PageSetupDialog();
            PageSetup.Document = DataTablePrinter;
            DataTablePrinter.DefaultPageSettings = PageSetup.PageSettings;
            DataTablePrinter.DefaultPageSettings.Landscape = false;//设置打印横向还是纵向
        }

        public override int GetCurrentItemsPerPage()
        {
            return COUNT_PER_PAGE;
        }



        void FillPrintInfo()
        {
            
            int ProductCodeIndex = CutUserinputData.GetIndexByString("ProductCode");
            ProductCode = DataTablePrint.Rows[0][ProductCodeIndex].ToString();

            int BatchNoIndex = CutUserinputData.GetIndexByString("BatchNo");
            BatchNo = DataTablePrint.Rows[0][ProductCodeIndex].ToString();

            int TMDateIndex = CutUserinputData.GetIndexByString("Date");
            MDate = DataTablePrint.Rows[0][TMDateIndex].ToString();


            int PlateNoIndex = CutUserinputData.GetIndexByString("PlateNo");
            PlateNo = DataTablePrint.Rows[0][PlateNoIndex].ToString();

            int VendorNameIndex = CutUserinputData.GetIndexByString("CustomerName");
            VendorName = DataTablePrint.Rows[0][VendorNameIndex].ToString();


 

        Count = DataTablePrint.Rows.Count;

            int count_per_page = GetCurrentItemsPerPage();
            int LittleRollIdx = VendorNameIndex = CutUserinputData.GetIndexByString("LittleRollNo");
            LRollIdxs = GetColumnsByIndex(DataTablePrint, LittleRollIdx, Count - LastCountInCurDT, count_per_page);
            int BigRollIdx = VendorNameIndex = CutUserinputData.GetIndexByString("BigRollNo");
            BRollIdxs = GetColumnsByIndex(DataTablePrint, BigRollIdx, Count - LastCountInCurDT, count_per_page);


            rollIdxs = new string[BRollIdxs.Length];
            for (int i = 0; i < BRollIdxs.Length; i++)
                rollIdxs[i] = BRollIdxs[i] + '-' + LRollIdxs[i];


            int LittleWeightIdx = VendorNameIndex = CutUserinputData.GetIndexByString("LittleWeight");
            Weights = GetColumnsByIndex(DataTablePrint, LittleWeightIdx, Count - LastCountInCurDT, count_per_page);

        }
        override protected void DataTablePrinter_PrintPage(object sende, PrintPageEventArgs Ev)
        {
            g = Ev.Graphics;

            DataTablePrint = GetCurrentDataTable();
            if (DataTablePrint==null)
            {
                Ev.HasMorePages = false;
                return;
            }

            FillPrintInfo();

            PrintReportHeader(Ev);
            PrintReportSubHeaders(Ev);
            PrintTableFirstPart(Ev);

            PrintRecordComplete++;


            if (PrintRecordComplete>=PrintingTotal)
            { 
                Ev.HasMorePages = false;
            }
            else
            {
                Ev.HasMorePages = true;
            }

        }

        void PrintReportHeader(PrintPageEventArgs Ev)
        {
                StringFormat sf = new StringFormat();
                sf.Alignment = StringAlignment.Center;
                g.DrawString(Hearder, HeadFont, DrawBrush, new Point(Ev.PageBounds.Width / 2, PTop), sf);
                g.DrawString(SubHearder, new Font("Verdana", 15, FontStyle.Bold), DrawBrush, new Point(Ev.PageBounds.Width / 2, PTop + HeadFont.Height), sf);
        }


        //String ProductCode
        void PrintReportSubHeaders(PrintPageEventArgs Ev)
        {
                DrawHString(TProductCode + ProductCode, PLeft + 5, PTop + 2 * YUnit + (YUnit - TableFont.Height) / 2);
                DrawHString(TBatchNo + BatchNo, Ev.PageBounds.Width * 1 / 4 + 40, PTop + 2 * YUnit + (YUnit - TableFont.Height) / 2);
                DrawHString(TMDate + MDate,Ev.PageBounds.Width * 2 / 4 + 20, PTop + 2 * YUnit + (YUnit - TableFont.Height) / 2);
                DrawHString(TPlateNo + PlateNo,Ev.PageBounds.Width * 3 / 4, PTop + 2 * YUnit + (YUnit - TableFont.Height) / 2);
                DrawHString(TVendorName + VendorName, PLeft + 5, PTop + 3 * YUnit);
        }

        void PrintTableFirstPart(PrintPageEventArgs Ev)
        {
            const int colCount = 8;
            int starty = PTop + 4 * YUnit;
            int startx = PLeft;
            int endx = PWidth - PRight;
            int width = endx - startx;
            int height = 32 * YUnit;
            int rows = 33;

            int c1_width = 40;
            int c2_startx = startx + c1_width;
            int c2_width = 40;
            int c2_mid_x = GetMiddleX(c2_startx, c2_width);

            int c3_start = c2_width + c2_startx;
            int c3_width = (width - 2 * c2_width) / 4;
            int stringstarty = starty + (YUnit - TableFont.Height) / 2;

            int[] colsw = new int[colCount];
            colsw[0] = 40;
            colsw[1] = (width / 4) - 40;
            colsw[2] = (int)((width / 4) * 3.0 / 5);
            colsw[3] = (int)((width / 4) * 2.0 / 5);
            colsw[4] = 40;
            colsw[5] = (width / 4) - 40;
            colsw[6] = (int)((width / 4) * 3.0 / 5);
            colsw[7] = (int)((width / 4) * 2.0 / 5);
            Pen PenLittle = new Pen(Brushes.Black, 1);
            //  Pen Penbold = new Pen(Brushes.Black, 1);
            g.DrawRectangle(PenLittle, startx, starty, width, height + YUnit);
            DrawTableRow(startx, starty, width, rows, YUnit);
            Drawtablevertical(startx, starty, width, rows, YUnit, colsw);

            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            for (int i = 1; i < rows; i++)
            {
                g.DrawString(i.ToString(), TableFont, DrawBrush, new Point(startx + c1_width / 2, stringstarty + i * YUnit), sf);
            }

            for (int i = 1; i < rows; i++)
            {
                g.DrawString((i + 32).ToString(), TableFont, DrawBrush, new Point(startx + width / 2 + c1_width / 2, stringstarty + i * YUnit), sf);
            }
            int x = startx;
            int stringY = YUnit / 2 + starty;
            int[] unitmids = GetColumMidOffsetXs(colsw);
            for (int Cols = 0; Cols < colCount; Cols++)
            {
                String ColumnText = Titles[Cols];

                Ev.Graphics.DrawString(ColumnText, TableFont, DrawBrush, startx + unitmids[Cols], stringstarty, sf);

                x += unitmids[Cols];

            }

            PrintRollInfoFirstPart(startx + unitmids[1], startx + unitmids[2], startx + unitmids[3], startx + unitmids[5], startx + unitmids[6], startx + unitmids[7], stringstarty);


            DrawHStringAlignCenter(TBanZu, startx + width/3, starty + (rows + 1) * YUnit);
            DrawHStringAlignCenter(TCaoZuoGong, startx + width*2 / 3, starty + (rows + 1) * YUnit);
            float total = CalcTotalWeight();
            DrawHString(TTotalWeight + total.ToString()+"kg", startx, starty + rows * YUnit);
        }

        void PrintRollInfoFirstPart(int x1, int x2, int x3, int x4, int x5, int x6, int y)
        {

            int start = DataTablePrint.Rows.Count - LastCountInCurDT;

            for (int items = 0; items < rollIdxs.Length && items < 64; items++)
            {
                if (items < 32)
                {
                    DrawHStringAlignCenter(rollIdxs[items],x1,y + YUnit * (items + 1));
                    DrawHStringAlignCenter(Weights[items], x2, y + YUnit * (items + 1));
                    //DrawHStringAlignCenter(rollIdxs[items], x3, y);
                    //Ev.Graphics.DrawString(rollIndex[items].ToString(), TableFont, DrawBrush, x1, y + YUnit * (items + 1), sf);
                    //Ev.Graphics.DrawString(outputWeight[items].ToString(), TableFont, DrawBrush, x2, y + YUnit * (items + 1), sf);
                    //Ev.Graphics.DrawString(jointNum[items].ToString(), TableFont, DrawBrush, x3],  y + YUnit * (items + 1), sf);
                }
                else
                {
                    DrawHStringAlignCenter(rollIdxs[items], x5, y + YUnit * (items - 31));
                    DrawHStringAlignCenter(Weights[items], x6, y + YUnit * (items - 31));
                    //DrawHStringAlignCenter(rollIdxs[items], x7, y);

                    //Ev.Graphics.DrawString(rollIndex[items - 32].ToString(), TableFont, DrawBrush, startx + unitmids[5], y + YUnit * (items - 31), sf);
                    //Ev.Graphics.DrawString(outputWeight[items - 32].ToString(), TableFont, DrawBrush, startx + unitmids[6], y + YUnit * (items - 31), sf);
                    //Ev.Graphics.DrawString(jointNum[items - 32].ToString(), TableFont, DrawBrush, startx + unitmids[7], y + YUnit * (items - 31), sf);
                }
            }
        }


        float CalcTotalWeight()
        {
            float total = 0.0f;
            for (int i = 0; i < Weights.Length && i < 64; i++)
            {
                float temp;
                Boolean v = float.TryParse(Weights[i], out temp);
                if (v)
                {
                    total += temp;
                }
            }
            return total;
        }


    }
}
