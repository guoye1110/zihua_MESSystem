using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;
using System.Data;
using LabelPrint.Data;
namespace LabelPrint.Print
{
    class Report_Cut : Report
    {


        public String ProductCode;
        public String BatchNo;
        public String MDate;
        public String PlateNo;
        public String VendorName;
        public String BanBie;
        public String Banci;
        public String Recipe;
        public String KeZhong;
        public String width;
        public String Length;
        public String MoJuanJing;
        public String ProductName;


        string[] BRollIdxs;
        string[] LRollIdxs;
        string[] rollIdxs;
        string[] Weights;
        string[] Joints;
        string[] BanBies;
        string[] Qualities;
        string[] ProductStates;

        private int Tabx;
        private int Taby;
        int Count = 0;


        String ChanChuCol1 = "  合格品总量：	";
        String ChanChuCol2 = "  不合格总量：	";
        String ChanChuCol3 = "  待 处理品 量： ";
        String ChanChuCol4 = "  等级品量：	";
        String ChanChuCol5 = "  可再 生料：	";
        String ChanChuCol6 = "  废    料：	";
        String ChanChuCol7 = "  半 成 品：	";
        String ChanChuCol8 = "  操作者：	";
        List<String> mList = new List<String>();
        float totalGood = 0;
        float totalBad = 0;
        void ConvertTotalInfo()
        {

        }

        void ConvertDtToPrintItem()
        {

        }
        public override int GetCurrentItemsPerPage()
        {
            return 36;
        }
        protected override void InitialVariableForPrint()
        {
            base.InitialVariableForPrint();
            if (DataTablePrint != null)
            XUnit = new int[DataTablePrint.Columns.Count];
        }


        // PrintRecordNumber = Convert.ToInt32((PHeigh - PTop - PBottom - YUnit) / YUnit);
        // FirstPrintRecordNumber = Convert.ToInt32((PHeigh - PTop - PBottom - HeadHeight - YUnit) / YUnit);

        override protected int GetTotalPrintPage() { return 0; }

        override protected void PrinterPageSetting()
        {
            PageSetupDialog PageSetup = new PageSetupDialog();
            PageSetup.Document = DataTablePrinter;
            DataTablePrinter.DefaultPageSettings = PageSetup.PageSettings;
            DataTablePrinter.DefaultPageSettings.Landscape = false;//设置打印横向还是纵向
        }

        void FillPrintInfo()
        {

            int ProductCodeIndex = 0;
            ProductCodeIndex = CutUserinputData.GetIndexByString("ProductCode");
            ProductCode = DataTablePrint.Rows[0][ProductCodeIndex].ToString();
            int BatchNoIndex = 0;
            BatchNoIndex = CutUserinputData.GetIndexByString("BatchNo");
            BatchNo = DataTablePrint.Rows[0][ProductCodeIndex].ToString();

            int TMDateIndex = 0;
            TMDateIndex = CutUserinputData.GetIndexByString("Date");
            MDate = DataTablePrint.Rows[0][TMDateIndex].ToString();

            int PlateNoIndex = 0;
            PlateNoIndex = CutUserinputData.GetIndexByString("PlateNo");
            PlateNo = DataTablePrint.Rows[0][PlateNoIndex].ToString();

            int VendorNameIndex = 0;
            VendorNameIndex = CutUserinputData.GetIndexByString("CustomerName");
            VendorName = DataTablePrint.Rows[0][VendorNameIndex].ToString();


            int BanBieIndex = CutUserinputData.GetIndexByString("WorkClassType");
            BanBie = DataTablePrint.Rows[0][BanBieIndex].ToString();
            int BanciIndex = CutUserinputData.GetIndexByString("WorkTimeType");
            Banci = DataTablePrint.Rows[0][BanciIndex].ToString();
            int RecipeIndex = CutUserinputData.GetIndexByString("Recipe");
            Recipe = DataTablePrint.Rows[0][RecipeIndex].ToString();

            int KeZhongIndex = CutUserinputData.GetIndexByString("ProductWeight");
            KeZhong = DataTablePrint.Rows[0][KeZhongIndex].ToString();

            int widthIndex = CutUserinputData.GetIndexByString("Width");
            width = DataTablePrint.Rows[0][widthIndex].ToString();

            int LengthIndex = CutUserinputData.GetIndexByString("ProductLength");
            Length = DataTablePrint.Rows[0][LengthIndex].ToString();

            int ProductNameidx = CutUserinputData.GetIndexByString("ProductName");
            ProductName = DataTablePrint.Rows[0][ProductNameidx].ToString();

            //int MoJuanJingIndex = CutUserinputData.GetIndexByString("Width");
            // MoJuanJing = DataTablePrint.Rows[0][MoJuanJingIndex].ToString();

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

           // BanBies
                   //         int LittleWeightIdx = VendorNameIndex = CutUserinputData.GetIndexByString("LittleWeight");
            BanBies = GetColumnsByIndex(DataTablePrint, BanBieIndex, Count - LastCountInCurDT, count_per_page);

            
            int QualitiesIdx = VendorNameIndex = CutUserinputData.GetIndexByString("Quality");
            Qualities = GetColumnsByIndex(DataTablePrint, QualitiesIdx, Count - LastCountInCurDT, count_per_page);


            int ProductStateIdx = VendorNameIndex = CutUserinputData.GetIndexByString("State");
            ProductStates = GetColumnsByIndex(DataTablePrint, ProductStateIdx, Count - LastCountInCurDT, count_per_page);

        }

        override protected void DataTablePrinter_PrintPage(object sende, PrintPageEventArgs Ev)
        {
            DataTablePrint = GetCurrentDataTable();
            FillPrintInfo();
            if (DataTablePrint == null)
            {
                Ev.HasMorePages = false;
                return;
            }

            g = Ev.Graphics;
            PrintReportHeader(Ev);
            PrintReportSubHeaders(Ev);
            PrintTableFirstPart(Ev);
            PrintTableSecond(Ev);
            PrintTableThird(Ev);
            PrintTableFourth(Ev);

            PrintRecordComplete++;


            if (PrintRecordComplete >= PrintingTotal)
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
            HeadText = "分切生产日报表";

                StringFormat sf = new StringFormat();
                sf.Alignment = StringAlignment.Center;
                g.DrawString(HeadText, HeadFont, DrawBrush, new Point(Ev.PageBounds.Width / 2, PTop), sf);

        }
        void PrintReportSubHeaders(PrintPageEventArgs Ev)
        {
                g.DrawString("班别: "+ BanBie, TableFont, DrawBrush, new Point(PLeft + 20, PTop + 2 * YUnit));
                g.DrawString("班次: "+ Banci, TableFont, DrawBrush, new Point(Ev.PageBounds.Width * 1 / 3 + PLeft, PTop + 2 * YUnit));
                g.DrawString("配方号: "+ Recipe, TableFont, DrawBrush, new Point(Ev.PageBounds.Width * 3 / 5 + PLeft, PTop + 2 * YUnit));
                g.DrawString("生产日期: "+ MDate, TableFont, DrawBrush, new Point(PLeft + 20, PTop + YUnit));
        }

        void PrintTableFirstPart(PrintPageEventArgs Ev)
        {
            int starty = PTop + 3 * YUnit;
            int startx = PLeft;
            int endx = PWidth - PRight;
            int width = endx - startx;
            int height = 2 * YUnit;


            Pen PenLittle = new Pen(Brushes.Black, 1);
            Pen Penbold = new Pen(Brushes.Black, 3);
            g.DrawRectangle(Penbold, startx, starty, width, height);
            g.DrawLine(PenLittle, startx, starty + YUnit, endx, starty + YUnit);

            int dtx = startx;
            int dty = starty;
            int[] colsw1 = new int[6];
            colsw1[0] = 20;
            colsw1[1] = 35;
            colsw1[2] = 16;
            colsw1[3] = 41;
            colsw1[4] = 13;
            colsw1[5] = 85;

            String[] Titles1 = { "产品名称", "", "膜宽", "", "克重", "" };
            PrintTableWithTitleByCols(startx, starty, width, 1, YUnit, colsw1, Titles1, (YUnit - TableFont.Height) / 2);
            int[] realColsw1 = GetRealColsw(width, colsw1);
            //int[] realColMid1 = GetColumMidOffsetXs(realColsw1);
            int[] offsetXs = GetOffsetedXs(realColsw1, startx);

            DrawInfoInFirstPart(offsetXs, starty);
            int[] colsw2 = new int[4];
            colsw2[0] = 20;
            colsw2[1] = 35;
            colsw2[2] = 16;
            colsw2[3] = 139;
            String[] Titles2 = { "产品代号", "", "分切规格", "" };
            PrintTableWithTitleByCols(startx, starty+YUnit, width, 1, YUnit, colsw2, Titles2, (YUnit - TableFont.Height) / 2);
        }
        void DrawInfoInFirstPart(int[] ColXs, int starty)
        {
            DrawHString(ProductName, ColXs[1], starty);
            DrawHString(width,ColXs[3], starty);
            DrawHString(KeZhong, ColXs[5], starty);
            DrawHString(ProductCode, ColXs[1], starty+YUnit);
            String guige;
            guige = "膜宽: " + width + "  " + "    膜长:" + Length + "  " + "  膜卷径: ";
            DrawHString(guige, ColXs[3], starty + YUnit);
        }

        void PrintTableSecond(PrintPageEventArgs Ev)
        {

            int rows = 6;
            int starty = PTop + 5 * YUnit;
            int startx = PLeft;
            int endx = PWidth - PRight;
            int width = endx - startx;
            int height = rows * YUnit;
            int c1_width = 35;
            int Bigcellwidth = (width - c1_width) / 2;

            int c2_startx = startx + c1_width;
            int c2_width = 35;
            int c2_mid_x = GetMiddleX(c2_startx, c2_width);

            int c3_start = c2_width + c2_startx;
            int c3_width = (width - c1_width - 2 * c2_width) / 4;
            int stringstarty = starty + (YUnit - TableFont.Height) / 2;

            int[] xw = new int[6];


            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;

            Pen PenLittle = new Pen(Brushes.Black, 1);
            Pen Penbold = new Pen(Brushes.Black, 3);
            g.DrawRectangle(Penbold, startx, starty, width, height);

            int[] colsw = new int[6];

            colsw[0] = 8;
            colsw[1] = 35;
            colsw[2] = 55;
            colsw[3] = 8;
            colsw[4] = 35;
            colsw[5] = 55;

            String[] Titles = { "序号", "投入公斤数", "上道工序卷号", "序号", "投入公斤数", "上道工序卷号" };
            PrintTableWithTitleByCols(startx + 35, starty, width - 35, rows, YUnit, colsw, Titles, (YUnit - TableFont.Height) / 2);
            int dty = starty;
            int[] realColsw1 = GetRealColsw(width-35, colsw);
            int[] realColMid1 = GetColumMidOffsetXs(realColsw1);
            int[] offsetXs = GetOffsetedXs(realColsw1, startx+35);

            DrawVFixStr("kg", offsetXs[2]-10, starty+YUnit, rows-1);
            DrawVFixStr("kg", offsetXs[5] - 10, starty +YUnit, rows-1);


            DrawVDigitalIndex(realColMid1[0] + startx + 35, dty + YUnit, 1, 5);
            DrawVDigitalIndex(realColMid1[3]+startx+35, dty + YUnit, 6, 5);

            DrawVStringAlignCenter("投 入 量", startx, starty + (int)(6 * YUnit) / 2);
        }



        int[] realColsw;
        void PrintTableThird(PrintPageEventArgs Ev)
        {

            int rows = 19;

            int starty = PTop + 11 * YUnit;
            int startx = PLeft;
            int endx = PWidth - PRight;
            int width = endx - startx;
            int height = rows * YUnit;
            int stringstarty = starty + (YUnit - TableFont.Height) / 2;
            int Bigcellwidth = (width - 35) / 3;
            int c1_width = 35;

            int c2_startx = startx + c1_width;
            int c2_width = 35;
            int c2_mid_x = GetMiddleX(c2_startx, c2_width);

            int c3_start = c2_width + c2_startx;
            int c3_width = (width - c1_width - 2 * c2_width) / 10;

            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;


            Pen PenLittle = new Pen(Brushes.Black, 1);
            Pen Penbold = new Pen(Brushes.Black, 3);

            int dtx = startx + 35;
            int dty = starty + (int)(1.5 * YUnit);
            int stringoffset = (YUnit - TableFont.Height) / 2;
            int dty_stroffset = dty + stringoffset;
            int tablew = width - 35;


            g.DrawRectangle(Penbold, startx, starty, width, (int)(height + 0.5 * YUnit));



            int[] colsw = new int[12];
            colsw[0] = 8;
            colsw[1] = 20;
            colsw[2] = 15;
            colsw[3] = 15;
            colsw[4] = 20;
            colsw[5] = 20;
            colsw[6] = 8;
            colsw[7] = 20;
            colsw[8] = 15;
            colsw[9] = 15;
            colsw[10] = 20;
            colsw[11] = 20;
            realColsw = GetRealColsw(width - 35, colsw);
            int[] unitmids = GetColumMidOffsetXs(realColsw);

            int DRowCount = rows - 1;


            DrawVDigitalIndex(dtx + unitmids[0], dty , 1, DRowCount);
            DrawVDigitalIndex(dtx + unitmids[6], dty , DRowCount+1, DRowCount);

            String[] Titles = { "序号", "产出公斤数", "膜卷号", "", "质量问题", "产品状态", "序号", "产出公斤数", "膜卷号", "", "质量问题", "产品状态" };
            PrintTableWithTitleByCols(startx + 35, starty, width - 35, 1, (int)(1.5 * YUnit), colsw, Titles, (int)(1.5 * YUnit - TableFont.Height) / 2);


            PrintTableByCols(dtx, dty, tablew, rows - 1, YUnit, realColsw);

            g.DrawString("日期/班", TableFont, DrawBrush, dtx + unitmids[3], starty + 2, sf);


            //for (int i = 0; i < 10; i++)
            //{
            //    DrawHStringAlignCenter(rollIndex[i].ToString(), dtx + unitmids[2], dty + i * YUnit);
            //}
            PrintRollInfoInThird(dtx + unitmids[1], dtx + unitmids[2], dtx + unitmids[3], dtx + unitmids[4], dtx + unitmids[5], dtx + unitmids[6], dtx + unitmids[7], dtx + unitmids[8], dtx + unitmids[9], dtx + unitmids[10], dty);
            DrawVStringAlignCenter("产 出 量", startx, starty+ (int)(19.5 * YUnit)/2);

        }



        const int ROLL_IN_DATE = 18;
        void PrintRollInfoInThird(int x1, int x2, int x3, int x4, int x5, int x6, int x7, int x8, int x9, int x10, int y)
        {
            int starty = y;
            int starty2 = y;

            for (int i = 0; i < rollIdxs.Length && i < 36; i++)
            {

                if (i < 18)
                {

                    DrawHStringAlignCenter(Weights[i].ToString(), x1, starty);
                    DrawHStringAlignCenter(rollIndex[i].ToString(), x2, starty);
                    DrawHStringAlignCenter( BanBies[i].ToString(), x3, starty);
                    DrawHStringAlignCenter(Qualities[i].ToString(), x4, starty);
                    DrawHStringAlignCenter(ProductStates[i].ToString(), x5, starty);
                }
                else
                {
                    if (i == 18)
                        starty = starty2;
                    DrawHStringAlignCenter(Weights[i].ToString(), x6, starty);
                    DrawHStringAlignCenter(rollIndex[i].ToString(), x7, starty);
                    DrawHStringAlignCenter(BanBies[i].ToString(), x8, starty);
                    DrawHStringAlignCenter(Qualities[i].ToString(), x9, starty);
                    DrawHStringAlignCenter(ProductStates[i].ToString(), x10, starty);
                }
                starty += YUnit;
            }
        }




        void BuildChanChu()
        {
            mList.Clear();
            CalcTotalWeights();
            String HeGeZongLiao = ChanChuCol1 + totalGood.ToString() +"kg";
            mList.Add(HeGeZongLiao);
            String BuHeGe = ChanChuCol2 + totalBad.ToString() + "kg";
            mList.Add(BuHeGe);
            mList.Add(ChanChuCol3);
            mList.Add(ChanChuCol4);
            mList.Add(ChanChuCol5);
            mList.Add(ChanChuCol6);
            mList.Add(ChanChuCol7);
            mList.Add(ChanChuCol8);
        }


        void PrintTableFourth(PrintPageEventArgs Ev)
        {

            int colums = 9;
            int starty = (int)(PTop + 30 * YUnit + 0.5 * YUnit);
            int startx = PLeft;

            int margin = (YUnit - TableFont.Height) / 2;
            int endx = PWidth - PRight;
            int width = endx - startx;
            int height = colums * YUnit;

            Pen PenLittle = new Pen(Brushes.Black, 1);
            Pen Penbold = new Pen(Brushes.Black, 3);
            int dw = GetTotalLen(realColsw, 4) + 35;
            g.DrawLine(PenLittle, startx, starty + YUnit, startx + dw, starty + YUnit);

            g.DrawRectangle(Penbold, startx, starty, width, height);
            g.DrawString("投 入 总 量:", TableFont, DrawBrush, startx + margin, starty + margin);

            int midy = starty + (colums - 2) * YUnit / 2;
            DrawVStringAlignCenter("产 出 量", startx, midy + YUnit);

            BuildChanChu();

            for (int i = 1; i < colums; i++)
            {
                if (i != (colums - 1)) {
                    g.DrawLine(PenLittle, startx + 35, starty + i * YUnit, startx + dw, starty + i * YUnit);
                    g.DrawString(mList[i-1], TableFont, DrawBrush, startx + 35, starty + i * YUnit + margin);

                }
                else { 
                    g.DrawLine(PenLittle, startx, starty + i * YUnit, startx + dw, starty + i * YUnit);
                    g.DrawString(mList[i - 1], TableFont, DrawBrush, startx, starty + i * YUnit + margin);
                }
            }

            g.DrawLine(PenLittle, startx + 35, starty + 1 * YUnit, startx + 35, starty + (colums - 1) * YUnit);
            int dw2 = realColsw[10] + realColsw[11];
            g.DrawRectangle(Penbold, startx + dw, starty, width - dw2 - dw, colums * YUnit);


            g.DrawRectangle(Penbold, startx + dw, starty, width - dw2 - dw, 6 * YUnit);
            g.DrawRectangle(Penbold, startx + width - dw2, starty, dw2, colums * YUnit);
            DrawHString("实际生产台时：", startx+dw, starty);
            DrawHString("停产台时：", startx + dw , starty+YUnit);
            DrawHString("停产原因及处理情况：", startx + dw, starty + 2*YUnit);
            DrawHString("生产工时：", startx + dw+150, starty);
            DrawHString("机长：", startx + dw+ 150, starty+5*YUnit);
            DrawHString("备注：", startx + dw, starty+6*YUnit);
            DrawHString("分切机号：1#□2#□3#□4#□5#□", startx + dw+75, starty + 6 * YUnit);


            // vertical center specified.
            TextFormatFlags flags = TextFormatFlags.HorizontalCenter |
                TextFormatFlags.VerticalCenter | TextFormatFlags.WordBreak;

            String ShuoMingstr = "说明状态一栏中，合格品不用标示，不合格品标示为“×”待处理标示为“△”等级品标示“☆”";
            // Draw the text and the surrounding rectangle.
            TextRenderer.DrawText(g, ShuoMingstr, TableFont, new Rectangle(startx + width - dw2, starty, dw2, colums * YUnit), Color.Black, flags);
            //TextRenderer.DrawText(g)
            StringFormat sf = new StringFormat();
            sf.LineAlignment = StringAlignment.Center;
            sf.Alignment = StringAlignment.Center;
            g.DrawString(ShuoMingstr, TableFont, DrawBrush, new RectangleF(startx + width - dw2, starty, dw2, colums * YUnit), sf);
        }



        void CalcTotalWeights()
        {
            totalGood = 0;
            totalBad = 0;
            for (int i =0; i<Qualities.Length; i++)
            {
                if (ProductStates[i]=="合格品")
                {
                    if (Weights[i]!=null&& Weights[i] != "")
                    totalGood += float.Parse(Weights[i]);
                }
                else
                {
                    if (Weights[i] != null&& Weights[i] != "")
                        totalBad += float.Parse(Weights[i]);
                }
            }

            
        }

    }
}
