using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;




using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;
using System.Data;
#pragma warning disable CS0105 // The using directive for 'System' appeared previously in this namespace
using System;
#pragma warning restore CS0105 // The using directive for 'System' appeared previously in this namespace

/// <summary>
/// 打印，打印预览
/// 唐小熊 2013-08-01
/// </summary>
public class ToPrint3
{

    //以下用户可自定义
    //当前要打印文本的字体及字号
    private static Font TableFont = new Font("Verdana", 10, FontStyle.Regular);
    //表头字体
    private Font HeadFont = new Font("Verdana", 20, FontStyle.Bold);
    //表头文字
    private string HeadText = string.Empty;
    //表头高度
    private int HeadHeight = 40;
    //表的基本单位
    private int[] XUnit;
    private int YUnit = TableFont.Height * 2;
    //以下为模块内部使用
    private PrintDocument DataTablePrinter;
    private DataRow DataGridRow;
    private DataTable DataTablePrint;
    //当前要所要打印的记录行数,由计算得到
    private int PageRecordNumber;
    //正要打印的页号
    private int PrintingPageNumber = 1;
    //已经打印完的记录数
    private int PrintRecordComplete;
    private int PLeft;
    private int PTop;

    private int PRight;

    private int PBottom;
    private int PWidth;
    private int PHeigh;
    //当前画笔颜色
    private SolidBrush DrawBrush = new SolidBrush(Color.Black);
    
    
    
    //每页打印的记录条数
    private int PrintRecordNumber;
    //第一页打印的记录条数
    private int FirstPrintRecordNumber;
    //总共应该打印的页数
    private int TotalPage;
    //与列名无关的统计数据行的类目数（如，总计，小计......）
    public int TotalNum = 0;

    /// <summary>
    /// 打印
    /// </summary>
    /// <param name="dt">要打印的DataTable</param>
    /// <param name="Title">打印文件的标题</param>
    public void Print(DataTable dt, string Title)
    {
        try
        {
            CreatePrintDocument(dt, Title).Print();
        }

        catch (Exception ex)
        {
            MessageBox.Show("打印错误，请检查打印设置！");

        }
    }

    /// <summary>
    /// 打印预览
    /// </summary>
    /// <param name="dt">要打印的DataTable</param>
    /// <param name="Title">打印文件的标题</param>
    public void PrintPriview(DataTable dt, string Title)
    {
        try
        {
            PrintPreviewDialog PrintPriview = new PrintPreviewDialog();
            PrintPriview.Document = CreatePrintDocument(dt, Title);
            PrintPriview.WindowState = FormWindowState.Maximized;
            PrintPriview.ShowDialog();
        }
        catch (Exception ex)

        {
            MessageBox.Show("打印错误，请检查打印设置！");

        }
    }

    /// <summary>
    /// 创建打印文件
    /// </summary>
    private PrintDocument CreatePrintDocument(DataTable dt, string Title)
    {

        DataTablePrint = dt;
        HeadText = Title;
        DataTablePrinter = new PrintDocument();

        PageSetupDialog PageSetup = new PageSetupDialog();
        PageSetup.Document = DataTablePrinter;
        DataTablePrinter.DefaultPageSettings = PageSetup.PageSettings;
        DataTablePrinter.DefaultPageSettings.Landscape = false;//设置打印横向还是纵向
        //PLeft = 30; //DataTablePrinter.DefaultPageSettings.Margins.Left;
        PTop = DataTablePrinter.DefaultPageSettings.Margins.Top;
        //  DataTablePrinter.DefaultPageSettings.PaperSize = new PaperSize("newPage70X40"
        //   , 827
        //    , 1169);
        //PRight = DataTablePrinter.DefaultPageSettings.Margins.Right;
        PBottom = DataTablePrinter.DefaultPageSettings.Margins.Bottom;
        PWidth = DataTablePrinter.DefaultPageSettings.Bounds.Width;
        PHeigh = DataTablePrinter.DefaultPageSettings.Bounds.Height;



        XUnit = new int[DataTablePrint.Columns.Count];
        PrintRecordNumber = Convert.ToInt32((PHeigh - PTop - PBottom - YUnit) / YUnit);
        FirstPrintRecordNumber = Convert.ToInt32((PHeigh - PTop - PBottom - HeadHeight - YUnit) / YUnit);




        if (DataTablePrint.Rows.Count > PrintRecordNumber)
        {
            if ((DataTablePrint.Rows.Count - FirstPrintRecordNumber) % PrintRecordNumber == 0)
            {
                TotalPage = (DataTablePrint.Rows.Count - FirstPrintRecordNumber) / PrintRecordNumber + 1;
            }
            else
            {
                TotalPage = (DataTablePrint.Rows.Count - FirstPrintRecordNumber) / PrintRecordNumber + 2;
            }
        }
        else
        {
            TotalPage = 1;
        }

        DataTablePrinter.PrintPage += new PrintPageEventHandler(DataTablePrinter_PrintPage);
        DataTablePrinter.DocumentName = HeadText;

        return DataTablePrinter;
    }

    /// <summary>
    /// 打印当前页
    /// </summary>
    private void DataTablePrinter_PrintPage(object sende, PrintPageEventArgs Ev)
    {


        int tableWith = 0;
        string ColumnText;

        StringFormat sf = new StringFormat();
        sf.Alignment = StringAlignment.Center;

        //打印表格线格式
        Pen Pen = new Pen(Brushes.Black, 1);

        #region 设置列宽

        foreach (DataRow dr in DataTablePrint.Rows)
        {
            for (int i = 0; i < DataTablePrint.Columns.Count; i++)
            {
                int colwidth = Convert.ToInt32(Ev.Graphics.MeasureString(dr[i].ToString().Trim(), TableFont).Width);
                if (colwidth > XUnit[i])
                {
                    XUnit[i] = colwidth;
                }
            }
        }

        if (PrintingPageNumber == 1)
        {
            for (int Cols = 0; Cols <= DataTablePrint.Columns.Count - 1; Cols++)
            {
                ColumnText = DataTablePrint.Columns[Cols].ColumnName.ToString().Trim();
                int colwidth = Convert.ToInt32(Ev.Graphics.MeasureString(ColumnText, TableFont).Width);
                if (colwidth > XUnit[Cols])
                {
                    XUnit[Cols] = colwidth;
                }
            }
        }
        for (int i = 0; i < XUnit.Length; i++)
        {
            tableWith += XUnit[i];
        }
        #endregion

        PLeft = (Ev.PageBounds.Width - tableWith) / 2;
        int x = PLeft;
        int y = PTop;
        int stringY = PTop + (YUnit - TableFont.Height) / 2;
        int rowOfTop = PTop;

        //第一页
        if (PrintingPageNumber == 1)
        {
            //打印表头
            Ev.Graphics.DrawString(HeadText, HeadFont, DrawBrush, new Point(Ev.PageBounds.Width / 2, PTop), sf);


            //设置为第一页时行数
            PageRecordNumber = FirstPrintRecordNumber;
            if (DataTablePrint.Rows.Count < PrintRecordNumber)
                PageRecordNumber = DataTablePrint.Rows.Count;
            rowOfTop = y = PTop + HeadFont.Height + 10;
            stringY = PTop + HeadFont.Height + 10 + (YUnit - TableFont.Height) / 2;
        }
        else
        {
            //计算,余下的记录条数是否还可以在一页打印,不满一页时为假
            if (DataTablePrint.Rows.Count - PrintRecordComplete >= PrintRecordNumber)
            {
                PageRecordNumber = PrintRecordNumber;
            }
            else
            {
                PageRecordNumber = DataTablePrint.Rows.Count - PrintRecordComplete;
            }
        }

        #region 列名
        if (PrintingPageNumber == 1 || PageRecordNumber > TotalNum)//最后一页只打印统计行时不打印列名
        {
            //得到datatable的所有列名
            for (int Cols = 0; Cols <= DataTablePrint.Columns.Count - 1; Cols++)
            {
                ColumnText = DataTablePrint.Columns[Cols].ColumnName.ToString().Trim();

                int colwidth = Convert.ToInt32(Ev.Graphics.MeasureString(ColumnText, TableFont).Width);
                Ev.Graphics.DrawString(ColumnText, TableFont, DrawBrush, x, stringY);
                x += XUnit[Cols];
            }
        }
        #endregion



        Ev.Graphics.DrawLine(Pen, PLeft, rowOfTop, x, rowOfTop);
        stringY += YUnit;
        y += YUnit;
        Ev.Graphics.DrawLine(Pen, PLeft, y, x, y);

        //当前页面已经打印的记录行数
        int PrintingLine = 0;
        while (PrintingLine < PageRecordNumber)
        {
            x = PLeft;
            //确定要当前要打印的记录的行号
            DataGridRow = DataTablePrint.Rows[PrintRecordComplete];
            for (int Cols = 0; Cols <= DataTablePrint.Columns.Count - 1; Cols++)
            {
                Ev.Graphics.DrawString(DataGridRow[Cols].ToString().Trim(), TableFont, DrawBrush, x, stringY);
                x += XUnit[Cols];
            }
            stringY += YUnit;
            y += YUnit;
            Ev.Graphics.DrawLine(Pen, PLeft, y, x, y);

            PrintingLine += 1;
            PrintRecordComplete += 1;
            if (PrintRecordComplete >= DataTablePrint.Rows.Count)
            {
                Ev.HasMorePages = false;
                PrintRecordComplete = 0;
            }
        }

        Ev.Graphics.DrawLine(Pen, PLeft, rowOfTop, PLeft, y);
        x = PLeft;
        for (int Cols = 0; Cols < DataTablePrint.Columns.Count; Cols++)
        {
            x += XUnit[Cols];
            Ev.Graphics.DrawLine(Pen, x, rowOfTop, x, y);
        }



        PrintingPageNumber += 1;

        if (PrintingPageNumber > TotalPage)
        {
            Ev.HasMorePages = false;
            PrintingPageNumber = 1;
            PrintRecordComplete = 0;
        }
        else
        {
            Ev.HasMorePages = true;
        }


    }

}