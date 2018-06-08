﻿using System;
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
using LabelPrint.Util;
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

namespace LabelPrint.excelOuput
{
    public partial class excelClass
    {
            float[] outputWeight = { 21.3f,   22.4f,    24.5f,    23.3f,    23.1f,    22.5f,   22.5f,    23.1f,    23.2f,    24.0f,    21.9f,  
                                     21.3f,   22.4f,    24.5f,    23.3f,    23.1f,    22.5f,   22.5f,    23.1f,    23.2f,    24.0f,    21.9f,
                                     21.3f,   22.4f,    24.5f,    23.3f,    23.1f,    22.5f,   22.5f,    23.1f,    23.2f,    24.0f,    21.9f,  
                                     21.3f,   22.4f,    24.5f,    23.3f,    23.1f,    22.5f,   22.5f,    23.1f,    23.2f,    24.0f,    21.9f,  
                                     21.3f,   22.4f,    24.5f,    23.3f,    23.1f,    22.5f,   22.5f,    23.1f,    23.2f,    24.0f,    21.9f,  
                                     21.3f,   22.4f,    24.5f,    23.3f,    23.1f,    22.5f,   22.5f,    23.1f,    23.2f,    24.0f,    21.9f,  
                                   };
            string[] rollIndex = {"001-01", "001-02", "001-03", "001-04", "001-05", "001-06","002-01", "002-02", "002-03", "002-04", "003-01",
                                  "001-01", "001-02", "001-03", "001-04", "001-05", "001-06","002-01", "002-02", "002-03", "002-04", "003-01", 
                                  "001-01", "001-02", "001-03", "001-04", "001-05", "001-06","002-01", "002-02", "002-03", "002-04", "003-01", 
                                  "001-01", "001-02", "001-03", "001-04", "001-05", "001-06","002-01", "002-02", "002-03", "002-04", "003-01", 
                                  "001-01", "001-02", "001-03", "001-04", "001-05", "001-06","002-01", "002-02", "002-03", "002-04", "003-01",
                                  "001-01", "001-02", "001-03", "001-04", "001-05", "001-06","002-01", "002-02", "002-03", "002-04", "003-01", 
                                 };
            string[] qualityCode = {"",       "",       "",       "",       "",       "",      "",       "",       "",       "",       "", 
                                    "",       "",       "",       "",       "C",      "",      "",       "",       "",       "",       "", 
                                    "",       "",       "",       "",       "C",      "",      "",       "",       "",       "",       "", 
                                    "",       "",       "",       "",       "C",      "",      "",       "",       "",       "",       "", 
                                    "",       "",       "",       "",       "C",      "",      "",       "",       "",       "",       "", 
                                    "",       "",       "",       "",       "C",      "",      "",       "",       "",       "",       "", 
                                   };
            string[] statusCode = { "合格品", "合格品", "合格品", "合格品", "合格品", "合格品", "合格品", "合格品", "合格品", "合格品", "合格品", 
                                    "合格品", "合格品", "合格品", "合格品", "不合格", "合格品", "合格品", "合格品", "合格品", "合格品", "合格品", 
                                    "合格品", "合格品", "合格品", "合格品", "不合格", "合格品", "合格品", "合格品", "合格品", "合格品", "合格品", 
                                    "合格品", "合格品", "合格品", "合格品", "不合格", "合格品", "合格品", "合格品", "合格品", "合格品", "合格品", 
                                    "合格品", "合格品", "合格品", "合格品", "不合格", "合格品", "合格品", "合格品", "合格品", "合格品", "合格品", 
                                    "合格品", "合格品", "合格品", "合格品", "不合格", "合格品", "合格品", "合格品", "合格品", "合格品", "合格品", 
                                  };

            int[] jointNum = { 1, 0, 0, 0, 0, 1, 0, 0, 1, 0, 1, 
                               1, 0, 0, 0, 0, 1, 0, 0, 1, 0, 1, 
                               1, 0, 0, 0, 0, 1, 0, 0, 1, 0, 1, 
                               1, 0, 0, 0, 0, 1, 0, 0, 1, 0, 1, 
                               1, 0, 0, 0, 0, 1, 0, 0, 1, 0, 1, 
                               1, 0, 0, 0, 0, 1, 0, 0, 1, 0, 1, 
                             };

        public void slitReportFunc()
        {
            slitReport(1, "2018-04-05", "乙班", "日班", "1804107", "CPE003014", outputWeight, rollIndex, qualityCode, statusCode, "aaaabbbb");
        }

        public void weightListFunc()
        {
            weightList(1, "2018-04-05", "乙班", "日班", "1804107", "2201", "CPE003014", outputWeight, rollIndex, jointNum);
        }

        //生成分切日报表
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
                FileStream file = new FileStream(@"..\\..\\tables\\分切生产日报表.xlsx", FileMode.Open, FileAccess.Read);

                XSSFWorkbook hssfworkbook = new
                XSSFWorkbook(file);

                ISheet sheet1 = hssfworkbook.GetSheet("Sheet1");

                commandText = "select * from productSpec where productCode = '" + productCode + "'";
                tableArray = databaseCommonReading(gVariable.basicInfoDatabaseName, commandText);
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

                file = new FileStream(@"..\\..\\outputTables\\分切日报表\\" + date + ".xlsx", FileMode.Create);

                hssfworkbook.Write(file);

                file.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("generate daily repoort excel file failed, probably the file is opened by someone!" + ex);
            }
        }

        //生成磅码单报表
        public void weightList(int machineID, string date, string team, string shift, string batchNum, string plateNum, string productCode, float[] outputWeight, string[] rollIndex, int[] jointNum)
        {
            try
            {
                const string DIGIT_NUM = "f1";
                const int MAX_NUM_SMALL_ROLL = 64;

                int i;
                int len1, len2;
                float weight1, weight2;
                string commandText;
                string[,] tableArray;
                //read the template via FileStream, it is suggested to use FileAccess.Read to prevent file lock.
                //book1.xls is an Excel-2007-generated file, so some new unknown BIFF records are added.
                FileStream file = new FileStream(@"..\\..\\tables\\磅码单.xlsx", FileMode.Open, FileAccess.Read);

                XSSFWorkbook hssfworkbook = new
                XSSFWorkbook(file);

                ISheet sheet1 = hssfworkbook.GetSheet("Sheet1");

                commandText = "select * from productSpec where productCode = '" + productCode + "'";
                tableArray = databaseCommonReading(gVariable.basicInfoDatabaseName, commandText);
                if (tableArray == null)
                    return;

                sheet1.GetRow(2).GetCell(7).SetCellValue(machineID + "号分切机");
                sheet1.GetRow(3).GetCell(1).SetCellValue(productCode);
                sheet1.GetRow(3).GetCell(3).SetCellValue(batchNum);
                sheet1.GetRow(3).GetCell(5).SetCellValue(date);
                sheet1.GetRow(3).GetCell(7).SetCellValue(plateNum);

                sheet1.GetRow(4).GetCell(0).SetCellValue("供应商名称： " + tableArray[0, 1]); //productName
                sheet1.GetRow(5).GetCell(0).SetCellValue("规格(宽度 * 克重 * 长度)： " + tableArray[0, 8] + " X " + tableArray[0, 6] + " X " + tableArray[0, 10]);

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
                    sheet1.GetRow(7 + i).GetCell(1).SetCellValue(outputWeight[i].ToString(DIGIT_NUM) + "kg");
                    sheet1.GetRow(7 + i).GetCell(2).SetCellValue(rollIndex[i]);
                    sheet1.GetRow(7 + i).GetCell(3).SetCellValue(jointNum[i].ToString());
                    weight1 += outputWeight[i];
                }
                for (i = 0; i < len2; i++)
                {
                    sheet1.GetRow(7 + i).GetCell(5).SetCellValue(outputWeight[i].ToString(DIGIT_NUM) + "kg");
                    sheet1.GetRow(7 + i).GetCell(6).SetCellValue(rollIndex[i]);
                    sheet1.GetRow(7 + i).GetCell(7).SetCellValue(jointNum[i]);
                    weight2 += outputWeight[i];
                }

                sheet1.GetRow(MAX_NUM_SMALL_ROLL / 2 + 7).GetCell(0).SetCellValue("合计重量/铲：" + weight1);
                sheet1.GetRow(MAX_NUM_SMALL_ROLL / 2 + 7).GetCell(4).SetCellValue("合计重量/铲：" + weight2);

                file = new FileStream(@"..\\..\\outputTables\\磅码单\\" + date + ".xlsx", FileMode.Create);

                hssfworkbook.Write(file);

                file.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("generate weight list excel file failed, probably the file is opened by someone!" + ex);
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

        public static string[,] databaseCommonReading(string databaseName, string commandText)
        {
            int i, j;
            string[,] tableArray;
            DataTable dTable;

            try
            {
                tableArray = null;
                dTable = mySQLClass.queryDataTableAction(databaseName, commandText, null);
                if (dTable != null && dTable.Rows.Count != 0)
                {
                    j = 0;

                    tableArray = new string[dTable.Rows.Count, dTable.Rows[0].ItemArray.Length];
                    for (i = 0; i < dTable.Rows.Count; i++)
                    {
                        for (j = 0; j < dTable.Rows[0].ItemArray.Length; j++)
                        {
                            tableArray[i, j] = dTable.Rows[i].ItemArray[j].ToString();
                        }
                    }
                }
                return tableArray;
            }
            catch (Exception ex)
            {
                Console.WriteLine("databaseCommonReading failed!" + ex);
            }
            return null;
        }
    }
}