using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

using MESSystem.common;

namespace MESSystem.OEEManagement
{
    public partial class OEEStaffManager : Form
    {
        /**************************************************** Constant ****************************************************/
        private const int WORKSHOP_NUM = 2;
        private const string HOURS_OUTPUT_CHART_NAME = "HoursOutput";
        private const string SERIES_HOURS = "Hours";
        private const string SERIES_OUTPUT = "Output";
        private const string EXCEL_TEMPLATE_NAME = "StaffHours.xlsx";
        /***************************************************** Types ******************************************************/


        /********************************************************Variables*************************************************/
        // private OEEStaff staff;
        private OEEFactory factory;
        private OEEWorkshop[] workshops;
        private Dictionary<string, OEEStaff> staffs;


        private static string[] monthViewHeader = { "序号", "员工", "生产车间", "月份", "工时数", "产品数" };
        private static int[] monthViewWidth = { 5, 7, 15, 10, 10, 10 };
        private static string[] detailViewHeader = { "工单号", "产品编码", "产品名称", "工序", "批次号", "工时数", "合格数", "不合格数", "设备编号", "设备名称" };
        private static int[] detailViewWidth = { 10, 10, 18, 10, 15, 7, 7, 7, 10, 10 };
        private int width_scale = 9;

        private int selectedWorkshop = 0;
        private OEEStaff selectedStaff;

        private int totalIndex;
        private string selectedMonth;
        /***************************************************** Property ***************************************************/

        /***************************************************** Functions ***************************************************/
        public OEEStaffManager(OEEFactory thefactory)
        {
            factory = thefactory;
            workshops = factory.Workshops;
            staffs = factory.Staffs;
            InitializeComponent();
            InitializeParameters();

        }

        private void InitializeParameters()
        {
            InitialWorkshopCmbBox();
            InitialYearCmbBox();
            InitialMonthCmbBox();
            InitialStaffCmbBox();
            InitialLists();
        }

        private void InitialWorkshopCmbBox()
        {
            for (int i = 0; i < workshops.Count(); i++)
                this.CmbWorkshop.Items.Add(workshops[i].Name);
            this.CmbWorkshop.SelectedIndex = 0;

        }

        private void InitialYearCmbBox()
        {
            int nowYear = System.DateTime.Now.Year;
            for (int i = OEETypes.YEAR_START; i <= OEETypes.YEART_END; i++)
            {
                this.CmbYearStart.Items.Add(i);
                this.CmbYearEnd.Items.Add(i);
            }

            this.CmbYearStart.SelectedIndex = nowYear - OEETypes.YEAR_START;
            this.CmbYearEnd.SelectedIndex = nowYear - OEETypes.YEAR_START;
        }

        private void InitialMonthCmbBox()
        {
            int nowMonth = System.DateTime.Now.Month;
            for (int i = OEETypes.MONTH_START; i <= OEETypes.MONTH_END; i++)
            {
                this.CmbMonthStart.Items.Add(i);
                this.CmbMonthEnd.Items.Add(i);
            }

            this.CmbMonthStart.SelectedIndex = nowMonth - OEETypes.MONTH_START;
            this.CmbMonthEnd.SelectedIndex = nowMonth - OEETypes.MONTH_START;
        }

        private void InitialStaffCmbBox()
        {
            var stafflist = from staff in staffs
                            where staff.Value.Workshop == this.CmbWorkshop.Text
                            select staff;

            foreach (var staff in stafflist)
                this.CmbStaffName.Items.Add(staff.Key + " " + staff.Value.Name);

            selectedWorkshop = this.CmbWorkshop.SelectedIndex = 0;
        }

        private void InitialLists()
        {
            for (int i = 0; i < monthViewHeader.Length; i++)
            {
                ColumnHeader header = new ColumnHeader();
                header.AutoResize(ColumnHeaderAutoResizeStyle.None);
                header.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
                header.Text = monthViewHeader[i];
                header.Width = monthViewWidth[i] * width_scale;
                header.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
                this.LvwMonthView.Columns.Add(header);
            }
            SetMonthViewStyle();

            this.LvwDetailView.View = View.Details;
            Graphics g = CreateGraphics();
            for (int i = 0; i < detailViewHeader.Length; i++)
            {
                ColumnHeader header = new ColumnHeader();
                header.AutoResize(ColumnHeaderAutoResizeStyle.None);
                header.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
                header.Text = detailViewHeader[i];
                header.Width = detailViewWidth[i] * width_scale;
                header.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
                this.LvwDetailView.Columns.Add(header);
            }
            SetDetailViewStyle();

        }

        private void SetMonthViewStyle()
        {
            this.LvwMonthView.View = View.Details;
            this.LvwMonthView.GridLines = true;
            this.LvwMonthView.MultiSelect = false;
            this.LvwMonthView.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            this.LvwMonthView.AutoArrange = false;
            this.LvwMonthView.FullRowSelect = true;

            ImageList il = new ImageList();
            il.ImageSize = new Size(1, 30);
            this.LvwMonthView.SmallImageList = il;
        }

        private void SetDetailViewStyle()
        {
            this.LvwDetailView.View = View.Details;
            this.LvwDetailView.GridLines = true;
            this.LvwDetailView.MultiSelect = false;
            this.LvwDetailView.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            this.LvwDetailView.AutoArrange = false;

        }

        private void CmbWorkshop_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (selectedWorkshop != this.CmbWorkshop.SelectedIndex)
            {
                selectedWorkshop = this.CmbWorkshop.SelectedIndex;

                var stafflist = from staff in staffs
                                where staff.Value.Workshop == this.CmbWorkshop.Text
                                select staff;

                this.CmbStaffName.Items.Clear();

                foreach (var staff in stafflist)
                    this.CmbStaffName.Items.Add(staff.Key + " " + staff.Value.Name);
            }

        }

        private void BtnStaffQuery_Click(object sender, EventArgs e)
        {
            int yearStart = Convert.ToInt32(this.CmbYearStart.Text);
            int yearEnd = Convert.ToInt32(this.CmbYearEnd.Text);
            int monthStart = Convert.ToInt32(this.CmbMonthStart.Text);
            int monthEnd = Convert.ToInt32(this.CmbMonthEnd.Text);

            DateTime startTime = OEEUtils.ConvertToDayStartOfMonth(yearStart, monthStart);
            DateTime endTime = OEEUtils.ConvertToDayEndOfMonth(yearEnd, monthEnd);

            totalIndex = 1;

            this.LvwMonthView.Items.Clear();
            this.LvwDetailView.Items.Clear();

            if (DateTime.Compare(startTime, endTime) > 0)
            {
                MessageBox.Show("起始时间不能大于终止时间", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                string id = this.CmbStaffName.Text.Split(' ').First();
                selectedStaff = staffs[id];

                for (int year = yearStart; year <= yearEnd; year++)
                {
                    if (monthEnd < monthStart)
                        monthEnd = OEETypes.MONTH_END;

                    for (int month = monthStart; month <= monthEnd; month++)
                    {
                        OEETypes.OutputInHours outputInHours;
                        DateTime start = OEEUtils.ConvertToDayStartOfMonth(year, month);
                        DateTime end = OEEUtils.ConvertToDayEndOfMonth(year, month);
                        outputInHours = selectedStaff.QueryLabourHour(start, end, factory);
                        ShowMonthList(outputInHours, selectedStaff.Name, year, month);
                    }

                    monthStart = 1;
                }

            }
        }


        private void ShowMonthList(OEETypes.OutputInHours outputInHours, string name, int year, int month)
        {
            this.LvwMonthView.BeginUpdate();
            if (outputInHours.output > 0)
            {

                ListViewItem item = new ListViewItem(totalIndex.ToString()); ;
                item.SubItems.Add(name);
                item.SubItems.Add(workshops[selectedWorkshop].Name);
                item.SubItems.Add(year.ToString() + "-" + month.ToString());
                item.SubItems.Add(outputInHours.hours.TotalHours.ToString("f0"));
                item.SubItems.Add(outputInHours.output.ToString());
                this.LvwMonthView.Items.Add(item);
            }
            this.LvwMonthView.EndUpdate();
        }


        private void OEEStaffManager_Load(object sender, EventArgs e)
        {

        }

        private void LvwMonthView_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            DataTable dispatches;
            ListViewItem selectedItem = e.Item;
            ListViewItem.ListViewSubItem subItem = selectedItem.SubItems[3];
            string text = subItem.Text;
            int year = Convert.ToInt32(text.Split('-').ElementAt(0));
            int month = Convert.ToInt32(text.Split('-').ElementAt(1));
            selectedMonth = text;
            LvwDetailView.Items.Clear();
            dispatches = selectedStaff.QueryLabourHourDetails(year, month, selectedStaff.ID, factory);
            ShowDetailedList(dispatches);
        }

        private void ShowDetailedList(DataTable dispatches)
        {
            try
            {
                this.LvwDetailView.BeginUpdate();

                for (int i = 0; i < dispatches.Rows.Count; i++)
                {
                    int index = 0;

                    TimeSpan span;

                    DataRow dr = dispatches.Rows[i];
                    ListViewItem item = new ListViewItem(dr[index++].ToString()); //dispatchCode
                    item.SubItems.Add(dr[index++].ToString()); //productCode
                    item.SubItems.Add(dr[index++].ToString()); //productName
                    item.SubItems.Add(dr[index++].ToString()); //processName
                    item.SubItems.Add(dr[index++].ToString()); //seriesNumber
                    DateTime startTime = Convert.ToDateTime(dr[index++].ToString());
                    DateTime endTime = Convert.ToDateTime(dr[index++].ToString());
                    span = endTime.Subtract(startTime);
                    item.SubItems.Add(span.TotalHours.ToString("f0"));
                    item.SubItems.Add(dr[index++].ToString()); //qualifiedNum
                    item.SubItems.Add(dr[index++].ToString()); //unqualifiedNum
                    int machineID = Convert.ToInt32(dr[index++].ToString());
                    item.SubItems.Add(machineID.ToString()); //machinID
                    item.SubItems.Add(gVariable.machineNameArrayAPS[machineID]); //machinName

                    this.LvwDetailView.Items.Add(item);
                }
                this.LvwDetailView.EndUpdate();
            }
            catch (Exception)
            {

            }
        }

        private void BtnExport_Click(object sender, EventArgs e)
        {

            if (this.LvwMonthView.Items.Count == 0)
                return;

            try
            {
                string templatePath = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
               + "\\template\\" + EXCEL_TEMPLATE_NAME;
                FileStream template = new FileStream(templatePath, FileMode.Open, FileAccess.Read);

                XSSFWorkbook staffHourBook = new XSSFWorkbook(template);

                ISheet sheet1 = staffHourBook.GetSheet("Sheet1");
                ISheet sheet2 = staffHourBook.GetSheet("Sheet2");


                ICellStyle cellStyle = staffHourBook.CreateCellStyle();
                cellStyle.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Center;
                cellStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                cellStyle.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                cellStyle.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                cellStyle.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                cellStyle.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;

                int row = 2;
                foreach (ListViewItem item in this.LvwMonthView.Items)
                {
                    IRow currentRow = sheet1.CreateRow(row);

                    for (int col = 0; col < item.SubItems.Count; col++)
                    {
                        ICell currentCell = currentRow.CreateCell(col);
                        currentCell.CellStyle = cellStyle;
                        currentCell.SetCellType(CellType.String);
                        currentCell.SetCellValue(item.SubItems[col].Text);
                        sheet1.AutoSizeColumn(col);
                    }
                    row++;
                }

                row = 1;
                ICell nameCell = sheet2.GetRow(row).CreateCell(1);
                nameCell.CellStyle = cellStyle;
                nameCell.SetCellValue(selectedStaff.Name);

                ICell monthCell = sheet2.GetRow(row).CreateCell(4);
                monthCell.CellStyle = cellStyle;
                monthCell.SetCellValue(selectedMonth);
                row += 2;
                foreach (ListViewItem item in this.LvwDetailView.Items)
                {
                    IRow currentRow = sheet2.CreateRow(row);

                    for (int col = 0; col < item.SubItems.Count; col++)
                    {
                        ICell currentCell = currentRow.CreateCell(col);
                        currentCell.CellStyle = cellStyle;
                        currentCell.SetCellType(CellType.String);
                        currentCell.SetCellValue(item.SubItems[col].Text);
                        sheet2.AutoSizeColumn(col);
                    }
                    row++;
                }

                //string tempDir = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                string tempDir = System.Environment.GetEnvironmentVariable("TEMP");
                string fileName = EXCEL_TEMPLATE_NAME.Split('.')[0] + "_" + DateTime.Now.ToString("hhss");
                string extenstion = EXCEL_TEMPLATE_NAME.Split('.')[1];
                fileName = fileName + "." + extenstion;
                FileStream file = new FileStream(tempDir + "\\" + fileName, FileMode.Create);
                System.Console.WriteLine("filename {0}", tempDir + "\\" + fileName);

                staffHourBook.Write(file);

                file.Close();
                template.Close();

                toolClass.nonBlockingDelay(2000);
                System.Diagnostics.Process.Start(tempDir + "\\" + fileName);
            }
            catch (Exception)
            {

            }

        }
    }
}
