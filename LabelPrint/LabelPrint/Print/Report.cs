using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;




using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;
using System.Data;
using LabelPrint.Util;
namespace LabelPrint.Print
{
    public abstract class Report
    {
        //以下用户可自定义
        //当前要打印文本的字体及字号
        protected static Font TableFont = new Font("宋体", 8, FontStyle.Regular);
        //表头字体
        protected Font HeadFont = new Font("宋体", 20, FontStyle.Bold);
        //表头文字
        protected string HeadText = string.Empty;
        //表头高度
        protected int HeadHeight = 40;
        //表的基本单位
        protected int[] XUnit;
        protected int YUnit =(int) (TableFont.Height * 2 );

        protected int SubTitleRows = 0;
        protected int PLeft;
        protected int PTop;
        protected int PRight;
        protected int PBottom;
        protected int PWidth;
        protected int PHeigh;
        protected SolidBrush DrawBrush = new SolidBrush(Color.Black);

        //以下为模块内部使用
        protected PrintDocument DataTablePrinter;
        protected DataRow DataGridRow;
        protected DataTable DataTablePrint;
        protected List<DataTable>.Enumerator dtEumerator;
        protected int LastCountInCurDT = 0;

        protected List<DataTable> DataTableList;
        public List<DataTable>.Enumerator enumer;

        protected int PrintingTotal = 1;
        //正要打印的页号
        protected int PrintingPageNumber = 1;
        //已经打印完的记录数
        protected int PrintRecordComplete = 0;
        protected Graphics g;

        protected const int COUNT_PER_PAGE = 3;
        protected StringFormat Vsf = new StringFormat(StringFormatFlags.DirectionVertical);
        StringFormat Hsf = new StringFormat();
        StringFormat HsfAL = new StringFormat();

        int Margin = (TableFont.Height ) / 2;



        public float[] outputWeight = { 21.3f,   22.4f,    24.5f,    23.3f,    23.1f,    22.5f,   22.5f,    23.1f,    23.2f,    24.0f,    21.9f,
                                     21.3f,   22.4f,    24.5f,    23.3f,    23.1f,    22.5f,   22.5f,    23.1f,    23.2f,    24.0f,    21.9f,
                                     21.3f,   22.4f,    24.5f,    23.3f,    23.1f,    22.5f,   22.5f,    23.1f,    23.2f,    24.0f,    21.9f,
                                     21.3f,   22.4f,    24.5f,    23.3f,    23.1f,    22.5f,   22.5f,    23.1f,    23.2f,    24.0f,    21.9f,
                                     21.3f,   22.4f,    24.5f,    23.3f,    23.1f,    22.5f,   22.5f,    23.1f,    23.2f,    24.0f,    21.9f,
                                     21.3f,   22.4f,    24.5f,    23.3f,    23.1f,    22.5f,   22.5f,    23.1f,    23.2f,    24.0f,    21.9f,
                                   };
        public string[] rollIndex = {"001-01", "001-02", "001-03", "001-04", "001-05", "001-06","002-01", "002-02", "002-03", "002-04", "003-01",
                                  "001-01", "001-02", "001-03", "001-04", "001-05", "001-06","002-01", "002-02", "002-03", "002-04", "003-01",
                                  "001-01", "001-02", "001-03", "001-04", "001-05", "001-06","002-01", "002-02", "002-03", "002-04", "003-01",
                                  "001-01", "001-02", "001-03", "001-04", "001-05", "001-06","002-01", "002-02", "002-03", "002-04", "003-01",
                                  "001-01", "001-02", "001-03", "001-04", "001-05", "001-06","002-01", "002-02", "002-03", "002-04", "003-01",
                                  "001-01", "001-02", "001-03", "001-04", "001-05", "001-06","002-01", "002-02", "002-03", "002-04", "003-01",
                                 };
        public string[] qualityCode = {"",       "",       "",       "",       "",       "",      "",       "",       "",       "",       "",
                                    "",       "",       "",       "",       "C",      "",      "",       "",       "",       "",       "",
                                    "",       "",       "",       "",       "C",      "",      "",       "",       "",       "",       "",
                                    "",       "",       "",       "",       "C",      "",      "",       "",       "",       "",       "",
                                    "",       "",       "",       "",       "C",      "",      "",       "",       "",       "",       "",
                                    "",       "",       "",       "",       "C",      "",      "",       "",       "",       "",       "",
                                   };
        public string[] statusCode = { "合格品", "合格品", "合格品", "合格品", "合格品", "合格品", "合格品", "合格品", "合格品", "合格品", "合格品",
                                    "合格品", "合格品", "合格品", "合格品", "不合格", "合格品", "合格品", "合格品", "合格品", "合格品", "合格品",
                                    "合格品", "合格品", "合格品", "合格品", "不合格", "合格品", "合格品", "合格品", "合格品", "合格品", "合格品",
                                    "合格品", "合格品", "合格品", "合格品", "不合格", "合格品", "合格品", "合格品", "合格品", "合格品", "合格品",
                                    "合格品", "合格品", "合格品", "合格品", "不合格", "合格品", "合格品", "合格品", "合格品", "合格品", "合格品",
                                    "合格品", "合格品", "合格品", "合格品", "不合格", "合格品", "合格品", "合格品", "合格品", "合格品", "合格品",
                                  };

        public int[] jointNum = { 1, 0, 0, 0, 0, 1, 0, 0, 1, 0, 1,
                               1, 0, 0, 0, 0, 1, 0, 0, 1, 0, 1,
                               1, 0, 0, 0, 0, 1, 0, 0, 1, 0, 1,
                               1, 0, 0, 0, 0, 1, 0, 0, 1, 0, 1,
                               1, 0, 0, 0, 0, 1, 0, 0, 1, 0, 1,
                               1, 0, 0, 0, 0, 1, 0, 0, 1, 0, 1,
                             };

        public Report()
        {
            Vsf.Alignment = StringAlignment.Center;
            Hsf.Alignment = StringAlignment.Center;
            //HsfAL.Alignment = StringAlignment.Far;
        }
        public virtual int GetCurrentItemsPerPage()
        {
            return COUNT_PER_PAGE;
        }

        public DataTable GetCurrentDataTable()
        {
            Boolean val = false;
            int count_per_page = GetCurrentItemsPerPage();
            if (DataTablePrint!=null)
            {
                if (LastCountInCurDT > count_per_page)
                {
                    LastCountInCurDT -= count_per_page;
                }
                else
                    LastCountInCurDT = 0;
            }

            if (LastCountInCurDT == 0) { 
                val = dtEumerator.MoveNext();
                if (val)
                {
                    DataTable dt = dtEumerator.Current;
                    LastCountInCurDT = dt.Rows.Count;
                    return dtEumerator.Current;
                }
            }
            else
            {
                return DataTablePrint;
            }

            return null;
        }

        public void Print(List<DataTable> dtList, string Title)
        {

           // DataTableList = dtList;
            dtEumerator = dtList.GetEnumerator();

            try
            {
                CreatePrintDocument(dtList, Title).Print();
            }
            catch (Exception ex)
            {
                Log.d("ProcessData", ex.Message);
                MessageBox.Show("打印错误，请检查打印设置！");
            }
        }
        public void Print(DataTable dt, string Title)
        {

            // DataTableList = dtList;
            //dtEumerator = dt.GetEnumerator();

            try
            {
                CreatePrintDocument(dt, Title).Print();
            }
            catch (Exception ex)
            {
                Log.d("ProcessData", ex.Message);
                MessageBox.Show("打印错误，请检查打印设置！");
            }
        }

        public void PrintPriview(List<DataTable> dtList, string Title)
        {
            // DataTableList = dtList;
            //dtEumerator = DataTableList.GetEnumerator();
            int page_per_count = GetCurrentItemsPerPage();
            try
            {
                PrintPreviewDialog PrintPriview = new PrintPreviewDialog();
                PrintingTotal = 0;
                foreach (DataTable dt in dtList)
                {

                    PrintingTotal += (dt.Rows.Count + page_per_count - 1) / page_per_count;
                }
                 //= CreatePrintDocument(dtList, Title);
                PrintDocument pd = CreatePrintDocument(dtList, Title); ;
                PrintPriview.Document = pd;
                pd.Print();
                PrintPriview.WindowState = FormWindowState.Maximized;
                //PrintPriview.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show("打印错误，请检查打印设置！");
                Log.d("Report", ex.Message);

            }
        }
        public void PrintPriview(DataTable dt, string Title)
        {
            // DataTableList = dtList;
            //dtEumerator = DataTableList.GetEnumerator();
            try
            {
                PrintPreviewDialog PrintPriview = new PrintPreviewDialog();
                PrintPriview.Document = CreatePrintDocument(dt, Title);
                PrintPriview.WindowState = FormWindowState.Maximized;
                PrintPriview.ShowDialog();
            }
            catch (Exception ex)
            {
                Log.d("ProcessData", ex.Message);
                MessageBox.Show("打印错误，请检查打印设置！");

            }
        }

        protected virtual void InitialVariableForPrint()
        {

            PLeft = DataTablePrinter.DefaultPageSettings.Margins.Left;
            PTop = DataTablePrinter.DefaultPageSettings.Margins.Top;
            
            PRight = DataTablePrinter.DefaultPageSettings.Margins.Right;
            PBottom = DataTablePrinter.DefaultPageSettings.Margins.Bottom;
            PWidth = DataTablePrinter.DefaultPageSettings.Bounds.Width;
            PHeigh = DataTablePrinter.DefaultPageSettings.Bounds.Height;




            GetTotalPrintPage();
        }

        protected  PrintDocument CreatePrintDocument(List<DataTable> dtList, string Title)
        {

           // DataTablePrint = dt;
            DataTableList = dtList;
            dtEumerator = DataTableList.GetEnumerator();
            HeadText = Title;
            DataTablePrinter = new PrintDocument();

            InitialVariableForPrint();


            DataTablePrinter.PrintPage += new PrintPageEventHandler(DataTablePrinter_PrintPage);
            DataTablePrinter.DocumentName = HeadText;
            return DataTablePrinter;
        }
        protected PrintDocument CreatePrintDocument(DataTable dt, string Title)
        {

            DataTablePrint = dt;
            //DataTableList = dtList;
            dtEumerator = DataTableList.GetEnumerator();
            HeadText = Title;
            DataTablePrinter = new PrintDocument();

            InitialVariableForPrint();


            DataTablePrinter.PrintPage += new PrintPageEventHandler(DataTablePrinter_PrintPage);
            DataTablePrinter.DocumentName = HeadText;
            return DataTablePrinter;
        }

        protected abstract int GetTotalPrintPage();

        protected abstract void PrinterPageSetting();
        protected abstract void DataTablePrinter_PrintPage(object sende, PrintPageEventArgs Ev);
        Pen LittlePen = new Pen(Brushes.Black, 1);


        int stringOffset = (TableFont.Height) / 2;


        protected int GetMiddleX(int startx, int width)
        {
            return startx + width / 2;
        }

        protected int GetColumnOffsetX(int[] colsw, int n)
        {
            int offset = 0;
            if (n == 0)
                return 0;
            for (int i = 0; i < n; i++)
                offset += colsw[i];
            return offset;
        }

        protected int[] GetColumOffsetXs(int[] colsw)
        {
            int[] offsets = new int[colsw.Length];

            for (int i = 0; i < colsw.Length; i++)
            {
                offsets[i] = GetColumnOffsetX(colsw, i);
            }
            return offsets;
        }

        protected int[] GetColumMidOffsetXs(int[] colsw)
        {
            int[] offsets = new int[colsw.Length];

            for (int i = 0; i < colsw.Length; i++)
            {
                offsets[i] = GetColumnOffsetX(colsw, i) + colsw[i] / 2;
            }
            return offsets;
        }
        protected int[] GetOffsetedXs(int[] colsw,int width)
        {
            int[] offsets = new int[colsw.Length];
            offsets[0] = width;
            for (int i = 1; i < colsw.Length; i++)
            {
                offsets[i] = colsw[i-1] + offsets[i-1];
            }
            return offsets;
        }

        protected int GetTotalLen(int[] colsw)
        {
            int len = 0;
            for (int i = 0; i < colsw.Length; i++)
                len += colsw[i];
            return len;
        }
        protected int GetTotalLen(int[] colsw, int arrayLen)
        {
            int len = 0;
            for (int i = 0; i < arrayLen; i++)
                len += colsw[i];
            return len;
        }
        protected int[] GetRealColsw(int width, int[] colsw)
        {
            int[] realColsw = new int[colsw.Length];
            int totalLen = GetTotalLen(colsw);
            for (int i = 0; i < colsw.Length; i++)
            {
                realColsw[i] = (int)(width * 1.0 * colsw[i] / totalLen);
            }
            realColsw[colsw.Length - 1] = width - GetTotalLen(realColsw, colsw.Length - 1);
            return realColsw;

        }

        protected void Drawtablevertical( int startx, int starty, int width, int rows, int heighperrow, int[] colsw)
        {

            int height = rows * heighperrow;
            int[] colOffsets = GetColumOffsetXs(colsw);

            for (int i = 0; i < colsw.Length; i++)
            {
                g.DrawLine(LittlePen, startx + colOffsets[i], starty, startx + colOffsets[i], starty + height);
            }
        }
        protected void PrintTableByCols( int startx, int starty, int width, int rows, int yunit, int[] colsw)
        {
             g.DrawRectangle(LittlePen, startx, starty, width, yunit * rows );
   
            for (int i = 1; i < rows; i++)
            {

                g.DrawLine(LittlePen, startx, starty + i * yunit, startx + width, starty + i * yunit);

            }
            Drawtablevertical( startx, starty, width, rows, yunit, colsw);
        }

  


        protected void PrintTableWithTitleByCols(int startx, int starty, int width, int rows, int Yunit, int[] colsw,String[] titles,int offsety)
        {
            int[] realColsw = GetRealColsw(width, colsw);
            int[] midColsw = GetColumMidOffsetXs(realColsw);
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
     
            for (int i = 0; i < midColsw.Length; i++)
            {

                g.DrawString(titles[i], TableFont, DrawBrush, new Point(startx + midColsw[i], starty+ offsety), sf);
            }
            PrintTableByCols(startx, starty, width, rows, Yunit, realColsw);
        }



        //top line is not drawn.
        protected void DrawTableRow(int startx, int starty, int width, int rows, int heighPerRow)
        {

            Pen PenLittle = new Pen(Brushes.Black, 1);
            for (int i = 1; i < rows; i++)
            {
                // g.DrawString(i.ToString(), TableFont, DrawBrush, new Point(c2_startx + 40 / 2, stringstarty + i * YUnit), sf);

                g.DrawLine(PenLittle, startx, starty + i * YUnit, startx + width, starty + i * heighPerRow);
            }
        }

        protected void DrawTableVertical( int startx, int starty, int width, int rows, int heighPerRow, int cols)
        {
            int colCellWidth = width / cols;
            int height = rows * heighPerRow;

            Pen PenLittle = new Pen(Brushes.Black, 1);
            for (int i = 1; i < cols; i++)
            {
                // g.DrawString(i.ToString(), TableFont, DrawBrush, new Point(c2_startx + 40 / 2, stringstarty + i * YUnit), sf);

                g.DrawLine(PenLittle, startx + i * colCellWidth, starty, startx + i * colCellWidth, starty + height);
            }
        }

        protected void DrawHString(String str, int startx, int starty)
        {
            DrawHString(str, startx, starty, Margin, Margin);
        }
        protected void DrawHString(String str, float startx, float starty, float marginx, float marginy)
        {
            g.DrawString(str, TableFont, DrawBrush, new PointF(startx + marginx, starty + marginy));
        }

        protected void DrawHStringAlignCenter(String str, int startx, int starty)
        {
            DrawHtringAlignCenter(str, startx, starty, Margin);
        }

        protected void DrawHtringAlignCenter(String str, int startx, int starty, int marginx)
        {
            g.DrawString(str, TableFont, DrawBrush, startx , starty + marginx, Hsf);
        }

        protected void DrawVStringAlignCenter(String str,int startx, int starty)
        {
            DrawVStringAlignCenter(str, startx, starty, Margin);
        }

        protected void DrawVStringAlignCenter(String str, int startx, int starty,int marginx)
        {
            g.DrawString(str, TableFont, DrawBrush, startx + marginx, starty , Vsf);
        }
      //  protected void DrawHStringAlignRight(String str, int startx, int starty)
      //  {
     //       DrawHStringAlignLeft(str, startx, starty, Margin);
     //   }

        protected void DrawHStringAlignRight(String str, int startx, int starty, int marginx)
        {
            g.DrawString(str, TableFont, DrawBrush, startx, starty + marginx, Hsf);
        }
        protected void DrawVDigitalIndex(int startx, int starty, int firstIndex, int count )
        {
            for (int i = 0; i < count; i++)
            {
                DrawHStringAlignCenter((firstIndex+i).ToString(), startx, starty + i * YUnit);
            }
        }

        protected void DrawVFixStr(String fixstr,int startx, int starty, int count)
        {
            for (int i = 0; i < count; i++)
            {
                DrawHStringAlignCenter(fixstr, startx, starty + i * YUnit);
            }
        }
        public static string[] GetColumnsByIndex(DataTable dt, int idx, int start, int maxcount)
        {
            int count = dt.Rows.Count;
            int realCount = (count - start) > maxcount ? maxcount : (count - start);
            String[] array = new String[realCount];


            for (int i = start; i < (start + realCount); i++)
                array[i - start] = dt.Rows[i][idx].ToString();

            return array;

        }
    }
}
