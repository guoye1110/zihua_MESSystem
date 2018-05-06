using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;
using MySql.Data;
using MySql.Data.MySqlClient;
using NPOI;
using NPOI.SS.Util;
using NPOI.SS.UserModel;
using NPOI.SS.Formula.Eval;
using NPOI.SS.Formula.Functions;
using NPOI.HPSF;
using NPOI.HSSF.Util;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.POIFS.FileSystem;

namespace MESSystem.common
{
    public partial class excelClass
    {
        public void slitReportFunc()
        {
            float[] outputWeight = { 21.3f,   22.4f,    24.5f,    23.3f,    23.1f,    22.5f,   22.5f,    23.1f,    23.2f,    24.0f,    21.9f,  
                                     21.3f,   22.4f,    24.5f,    23.3f,    23.1f,    22.5f,   22.5f,    23.1f,    23.2f,    24.0f,    21.9f,  };
            string[] rollIndex = {"001-01", "001-02", "001-03", "001-04", "001-05", "001-06","002-01", "002-02", "002-03", "002-04", "003-01",
                                    "001-01", "001-02", "001-03", "001-04", "001-05", "001-06","002-01", "002-02", "002-03", "002-04", "003-01" };
            string[] qualityCode = {"",       "",       "",       "",       "",       "",      "",       "",       "",       "",       "", 
                                    "",       "",       "",       "",       "C",      "",      "",       "",       "",       "",       "", };
            string[] statusCode = { "合格品", "合格品", "合格品", "合格品", "合格品", "合格品", "合格品", "合格品", "合格品", "合格品", "合格品", 
                                    "合格品", "合格品", "合格品", "合格品", "不合格", "合格品", "合格品", "合格品", "合格品", "合格品", "合格品", };

            slitReport(1, "2018-04-05", "乙班", "日班", "1804107", "CPE014025", outputWeight, rollIndex, qualityCode, statusCode, "aaaabbbb");
        }

        public void slitReport(int machineID, string date, string team, string shift, string batchNum, string productCode, float[] outputWeight, string[] rollIndex, string[] qualityCode, string[] statusCode, string notes )
        {
            try
            {
                const string DIGIT_NUM = "f1";
                const int MAX_NUM_SMALL_ROLL = 36;

                int i;
                int len1, len2;
                float weight1, weight2;
                string commandText;
                string[,] tableArray;
                //read the template via FileStream, it is suggested to use FileAccess.Read to prevent file lock.
                //book1.xls is an Excel-2007-generated file, so some new unknown BIFF records are added.
                FileStream file = new FileStream(@"d:\\tmp\\分切生产日报表.xlsx", FileMode.Open, FileAccess.Read);

                XSSFWorkbook hssfworkbook = new
                XSSFWorkbook(file);

                ISheet sheet1 = hssfworkbook.GetSheet("Sheet1");

                commandText = "select * from productSpec where productCode = '" + productCode + "'";
                tableArray = mySQLClass.databaseCommonReading(gVariable.basicInfoDatabaseName, commandText);
                if (tableArray == null)
                    return;

                
                sheet1.GetRow(1).GetCell(2).SetCellValue(date);
                //machine id
                sheet1.GetRow(1).GetCell(4).SetCellValue(machineID + "号分切机");
                sheet1.GetRow(2).GetCell(2).SetCellValue(team);
                sheet1.GetRow(2).GetCell(4).SetCellValue(shift);
                sheet1.GetRow(2).GetCell(6).SetCellValue(batchNum);

                sheet1.GetRow(3).GetCell(2).SetCellValue(tableArray[0,3]); //productName
                sheet1.GetRow(3).GetCell(5).SetCellValue(tableArray[0, 4]); //ingredient
                sheet1.GetRow(3).GetCell(8).SetCellValue(tableArray[0,6] + "g/m2"); //weight
                sheet1.GetRow(4).GetCell(2).SetCellValue(tableArray[0,2]); //productCode
                sheet1.GetRow(4).GetCell(5).SetCellValue(" 膜宽：" + tableArray[0, 8] + "    膜长：" + tableArray[0, 10] + "    卷径：" + tableArray[0, 7]); //product width + product length + product diameter

                if (outputWeight.Length <= MAX_NUM_SMALL_ROLL / 2)
                {
                    len1 = outputWeight.Length;
                    len2 = 0;
                }
                else if (outputWeight.Length <= MAX_NUM_SMALL_ROLL)
                {
                    len1 = MAX_NUM_SMALL_ROLL / 2;
                    len2 = outputWeight.Length - MAX_NUM_SMALL_ROLL / 2;
                }
                else
                {
                    len1 = MAX_NUM_SMALL_ROLL / 2;
                    len2 = len1;
                }

                weight1 = 0;
                weight2 = 0;
                for (i = 0; i < len1; i++)
                {
                    if (statusCode[i] == "合格品")
                        weight1 += outputWeight[i];
                    else
                        weight2 += outputWeight[i];

                    sheet1.GetRow(12 + i).GetCell(2).SetCellValue(outputWeight[i].ToString(DIGIT_NUM) + "kg");
                    sheet1.GetRow(12 + i).GetCell(3).SetCellValue(rollIndex[i]);
                    sheet1.GetRow(12 + i).GetCell(4).SetCellValue(team);
                    sheet1.GetRow(12 + i).GetCell(5).SetCellValue(qualityCode[i]);
                    sheet1.GetRow(12 + i).GetCell(6).SetCellValue(statusCode[i]);
                }
                for (i = 0; i < len2; i++)
                {
                    if (statusCode[i] == "合格品")
                        weight1 += outputWeight[i];
                    else
                        weight2 += outputWeight[i];

                    sheet1.GetRow(12 + i).GetCell(8).SetCellValue(outputWeight[i + len1].ToString(DIGIT_NUM) + "kg");
                    sheet1.GetRow(12 + i).GetCell(9).SetCellValue(rollIndex[i + len1]);
                    sheet1.GetRow(12 + i).GetCell(10).SetCellValue(team);
                    sheet1.GetRow(12 + i).GetCell(11).SetCellValue(qualityCode[i + len1]);
                    sheet1.GetRow(12 + i).GetCell(12).SetCellValue(statusCode[i + len1]);
                }
                sheet1.GetRow(31).GetCell(3).SetCellValue(weight1.ToString(DIGIT_NUM));
                sheet1.GetRow(32).GetCell(3).SetCellValue(weight2.ToString(DIGIT_NUM));

                sheet1.GetRow(37).GetCell(5).SetCellValue(notes);
                
                //Force excel to recalculate all the formula
                sheet1.ForceFormulaRecalculation = true;

                file = new FileStream(@"d:\\tmp\\test.xlsx", FileMode.Create);

                hssfworkbook.Write(file);

                file.Close();

                toolClass.nonBlockingDelay(2000);

                System.Diagnostics.Process.Start(@"d:\\tmp\\test.xlsx");

                //another method to open an excel file, need to add Microsoft Excel 11.0 Object Library in add reference (COM column) "
                /*
                Excel.Application excel = new Excel.Application();
                Excel.Workbook book = excel.Application.Workbooks.Add("d:\\tmp\\test.xlsx");
                excel.Visible = true;
                */
            }
            catch (Exception ex)
            {
                Console.WriteLine("generate excel file failed, probably the file is opened by someone!" + ex);
            }
        }

        public void standardExcelOuput()
        {
            try
            {
                FileStream fs;
                HSSFWorkbook wk = new HSSFWorkbook();
                ICellStyle cellStyle1 = wk.CreateCellStyle();
                ICellStyle cellStyle2 = wk.CreateCellStyle();
                ISheet sheet = wk.CreateSheet("例子");
                //sheet.IsGridsPrinted = true;

                IRow row = sheet.CreateRow(0);
                ICell cell = row.CreateCell(0);

                cell.SetCellValue("测试 code");

                cellStyle1.BorderTop = NPOI.SS.UserModel.BorderStyle.Thick;
                cellStyle1.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                cellStyle1.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                cellStyle1.BorderRight = NPOI.SS.UserModel.BorderStyle.Thick;
                cellStyle1.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                cellStyle1.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Center;
                //cellStyle.WrapText = true;  
                cellStyle1.ShrinkToFit = true;
                 
                cellStyle1.FillBackgroundColor = IndexedColors.Red.Index;                  
                cell.CellStyle = cellStyle1;

                cell = sheet.CreateRow(1).CreateCell(1);

                cellStyle2.CloneStyleFrom(cellStyle1);  

                cell.SetCellValue("cell 2, 2");
                cellStyle2.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Left;

                ICell sheetTitle = sheet.CreateRow(0).CreateCell(0); 
                sheetTitle.SetCellValue("This is a title");
                //sheetTitle.CellStyle = GetTitleCellStyle(wk);
                //sheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, 9));  //合并单元格

                sheet.AddMergedRegion(new CellRangeAddress(4, 5, 5, 6));
                IRow row0 = sheet.CreateRow(3);
                row0.CreateCell(0).SetCellValue("0-0");
                row0.CreateCell(1).SetCellValue("0-1");
                row0.CreateCell(3).SetCellValue("0-3");

                sheet.SetColumnWidth(3, 50 * 256);

                row0.Height = 20 * 20;

                //read a cell
                //cell = sheet.GetRow(0).GetCell(0); 

                using (fs = File.OpenWrite("d:\\excel.xls"))
                {
                    wk.Write(fs);//向打开的这个xls文件中写入并保存。  
                }

                fs.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("generate excel file failed, probably the file is opened by someone!" + ex);
            }
        }
    }
}
