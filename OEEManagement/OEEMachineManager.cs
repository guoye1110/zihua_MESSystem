using System;
using System.Collections.Generic;
using System.IO;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

using WinCharting = System.Windows.Forms.DataVisualization.Charting;

using MESSystem.common;

using CustomCharting = CustomControl.TimeLineControl;

namespace MESSystem.OEEManagement
{
    public partial class OEEMachineManager : Form
    {

        /***************************************** Constant ****************************************/
        private const int MARGIN_LEVEL_1 = 4;
        private const int MARGIN_LEVEL_2 = 8;
        private const int MARGIN_LEVEL_3 = 12;
        private const int MARGIN_LEVEL_4 = 16;

        private const string DATE_FORMAT = "yyyy/MM/dd";
        // Global constants
        private float TITLE_FONT_SIZE = 22F;

        // TabOutputCapacity constants
        private const string OUTPUT_CAPACITY_CHART_TITLE = "设备生产目标达成率";
        private const string OUTPUT_CAPACITY_SERIES_PLANNED = "计划";
        private const string OUTPUT_CAPACITY_SERIES_REAL = "实际";
        private const string OUTPUT_CAPACITY_CHART_NAME = "OutputCapacity";
        private Color backgroudColor = Color.DarkSlateGray;
        private Color realColor = Color.Lime;
        private Color qualifiedColor = Color.White;


        private const int PERCENTAGE_GROUP_POSITION_Y = 20;
        private const int PERCENTAGE_RECT_HEIGHT = 20;
        private const int PERCENTAGE_GROUP_PADDING = 10;

        // TabOutputStatus constants
        private const string OUTPUT_STATUS_CHART_TITLE = "设备生产状态查询";
        private static string[] OUTPUT_STATUS_LENGENDS = { "设备停机", "工单待机", "工单启动", "设备报警" };
        private Color outputStatus_backgroudColor = Color.LightBlue;
        private const float PIE_INTERVAL_X = 50;
        private const float PIE_INTERVAL_Y = 50;
        private const int PIE_ROWS = 2;
        private Color[] LENGENDS_COLOR = { Color.Gray, Color.SlateGray, Color.Lime, Color.Red };

        private const string EXCEL_TEMPLATE_NAME = "MachineHours.xlsx";
        /********************************************** Type *****************************************/
        private struct HoursSpan
        {
            public TimeSpan off;
            public TimeSpan idle;
            public TimeSpan alarm;
            public TimeSpan valid;
        };

        private struct HoursInfoInDay
        {
            //public DateTime workDay;
            //public HoursSpan hoursSpan;
            //public TimeSpan planned;

        }

        /******************************************** Variable **************************************/
        private OEEMachineGroup[] machineGroups;
        private int selectedMachineGroup;
        private int selectedMachine = 0;

        private DateTime startDate, endDate;

        private bool queryOneDayStatus = false;

        private WinCharting.Chart barChart;
        private WinCharting.ChartArea barChartArea;
        private WinCharting.Series seriesPlanned;
        private WinCharting.Series seriesReal;

        private WinCharting.Chart pieChart;
        private WinCharting.ChartArea[] pieChartArea;
        private WinCharting.Series[] seriesStatus;
        private int pieIndex;

        private CustomCharting.Series[] seriesTimeLine;
        private CustomCharting.TimeLine timeLine;

        private string[] header1;
        private string[] header2;
        private int sectionWidth;
        private int sectionX;
        private int sectionY;

        private string[] MACHINEHOUR_LIST_HEADER = new string[] { "计划", "关机", "待机", "工作", "报警", "有效工时" };
        private string[] MACHINEHOUR_LIST_ADDITION = new string[] { "序号", "日期" };
        private string[] DISPATCH_LIST_HEADER = new string[] { "工单号", "产品名称", "客户名", "需求量", "产量", "合格品", "操作员" };

        private bool tableGenerated = false;
        /****************************************** Function ****************************************/
        public OEEMachineManager(OEEFactory factory)
        {
            machineGroups = factory.MachineGroups;
            InitializeComponent();
            InitializeQueryCondition();
            InitialTabOutputCapacity();
            InitialTabOutputStatus();
            InitialTabOutputHours();
            InitialOutputCapacityChartInfo();
            DoQuery();
        }


        private void InitializeQueryCondition()
        {

            selectedMachineGroup = 0;

            for (int i = 0; i < machineGroups.Count(); i++)
                this.CmbMachineType.Items.Add(machineGroups[i].Name);

            this.CmbMachineType.SelectedIndex = selectedMachineGroup;

            startDate = DateTime.Now;
            endDate = startDate.AddMonths(1);

            this.DtpMachineDateStart.Value = startDate;
            this.DtpMachineDateEnd.Value = endDate;

        }

        private void InitialTabOutputCapacity()
        {
            barChart = new WinCharting.Chart();
            barChartArea = new WinCharting.ChartArea();
            seriesPlanned = new WinCharting.Series();
            seriesReal = new WinCharting.Series();

            SetOutputCapacityChartStyle();
            SetOutCapacityChartAreaStyle();
            SetPlannedSeriesStyle();
            SetRealSeriesStyle();

            barChart.ChartAreas.Add(barChartArea);

            barChart.Series.Add(seriesPlanned);
            barChart.Series.Add(seriesReal);

            this.TlpChart.BackColor = backgroudColor;
            this.TlpChart.Controls.Add(barChart, 0, 2);

        }

        private void InitialTabOutputStatus()
        {
            this.LblOutputStatus.Font = new Font(OEETypes.FONT, TITLE_FONT_SIZE, System.Drawing.FontStyle.Bold);
            this.LblOutputStatus.Text = OUTPUT_STATUS_CHART_TITLE;
            this.LblOutputStatus.ForeColor = Color.Black;

            this.TlpOutputStatus.BackColor = outputStatus_backgroudColor;

            InitialTimeLine();
            InitialPieChart();
        }

        private void InitialTimeLine()
        {
            string[] labelsY, labelsX;
            int count = machineGroups[selectedMachineGroup].Count;

            labelsY = new string[count];
            labelsX = new string[24];

            timeLine = new CustomCharting.TimeLine(24 * 60, 1, 2 * 60);
            timeLine.Dock = DockStyle.Fill;
            timeLine.Location = new Point(0, 0);
            timeLine.Bars = count;
            timeLine.BackColor = outputStatus_backgroudColor;

            timeLine.LabelFont = new Font("Microsoft Sans Serif", 10F, FontStyle.Regular);
            timeLine.LengendFont = new Font("Microsoft Sans Serif", 10F, FontStyle.Bold);

            seriesTimeLine = new CustomCharting.Series[LENGENDS_COLOR.Length];

            for (int i = 0; i < LENGENDS_COLOR.Length; i++)
            {
                seriesTimeLine[i] = new CustomCharting.Series();
                seriesTimeLine[i].Color = LENGENDS_COLOR[i];
                seriesTimeLine[i].Lengend = OUTPUT_STATUS_LENGENDS[i];
                timeLine.Series.Add(seriesTimeLine[i]);

            }

            for (int i = 0; i < 13; i++)
                labelsX[i] = (i * 2).ToString() + ":00";

            for (int i = 0; i < count; i++)
                labelsY[i] = (i + 1).ToString() + "号" + machineGroups[selectedMachineGroup].Name;

            timeLine.LabelsX = labelsX;
            timeLine.LabelsY = labelsY;


        }

        private void InitialPieChart()
        {

            int machineCount = machineGroups[selectedMachineGroup].Count;
            pieChart = new WinCharting.Chart();
            pieChartArea = new WinCharting.ChartArea[machineCount];
            seriesStatus = new WinCharting.Series[machineCount];

            float areaWidth = this.TabMachineOuputStatus.Width;
            float areaHeight = this.TabMachineOuputStatus.Height;
            int pines_in_line1 = machineCount / PIE_ROWS;
            int pines_in_line2 = machineCount - pines_in_line1;
            float pieWidth = (areaWidth - (pines_in_line2 + 1) * PIE_INTERVAL_X) / pines_in_line2;
            float pieHeight = (areaHeight - (PIE_ROWS + 1) * PIE_INTERVAL_Y) / PIE_ROWS;


            float line1_totalWidth = pieWidth * pines_in_line1 + PIE_INTERVAL_X * (pines_in_line1 - 1);
            float line1_locX = (areaWidth - line1_totalWidth) / 2;
            float line2_LocX = PIE_INTERVAL_X;

            pieChart.BackColor = outputStatus_backgroudColor;

            for (int i = 0; i < machineCount; i++)
            {
                pieChartArea[i] = new WinCharting.ChartArea();
                pieChartArea[i].Name = machineGroups[selectedMachineGroup].Name + i.ToString();
                pieChart.ChartAreas.Add(pieChartArea[i]);
                //pieChartArea[i].BackColor = Color.Green;
                pieChartArea[i].Position.Width = pieWidth / areaWidth * 100;
                pieChartArea[i].Position.Height = pieHeight / areaHeight * 100;
                if (i / pines_in_line1 == 0)
                {
                    pieChartArea[i].Position.X = (line1_locX + i * (pieWidth + PIE_INTERVAL_X)) / areaWidth * 100;
                    pieChartArea[i].Position.Y = PIE_INTERVAL_Y / areaHeight * 100;
                }
                else
                {
                    pieChartArea[i].Position.X = (line2_LocX + (i - pines_in_line1) * (pieWidth + PIE_INTERVAL_X)) / areaWidth * 100;
                    pieChartArea[i].Position.Y = (PIE_ROWS * PIE_INTERVAL_Y + pieHeight) / areaHeight * 100;
                }
            }

            for (int i = 0; i < machineCount; i++)
            {
                seriesStatus[i] = new WinCharting.Series();
                pieChart.Series.Add(seriesStatus[i]);
                seriesStatus[i].ChartArea = pieChartArea[i].Name;
                seriesStatus[i].ChartType = WinCharting.SeriesChartType.Pie;
            }

            pieChart.Dock = DockStyle.Fill;
        }

        private void InitialTabOutputHours()
        {
            DataGridView gridView = this.HoursDataGridView;

            gridView.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            gridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;

            gridView.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            gridView.CellBorderStyle = DataGridViewCellBorderStyle.Single;

            gridView.RowHeadersVisible = false;

            gridView.RowTemplate.Height = 20;

            gridView.AllowUserToResizeColumns = false;
            gridView.AllowUserToResizeRows = false;

            gridView.DefaultCellStyle.Font = new Font(OEETypes.FONT, 8);
            gridView.DefaultCellStyle.WrapMode = DataGridViewTriState.False;

            gridView.ColumnHeadersVisible = false;
            int headerColumnCount = MACHINEHOUR_LIST_ADDITION.Length + MACHINEHOUR_LIST_HEADER.Length * machineGroups[selectedMachine].Count;
            gridView.ColumnCount = headerColumnCount;


            int i;
            header1 = new string[headerColumnCount];
            header2 = new string[headerColumnCount];
            for (i = 0; i < MACHINEHOUR_LIST_ADDITION.Length; i++)
            {
                header1[i] = "";
                header2[i] = MACHINEHOUR_LIST_ADDITION[i];

            }

            for (int machineIndex = 0; machineIndex < machineGroups[selectedMachineGroup].Count; machineIndex++)
            {
                int j;
                OEEMachine[] machines = machineGroups[selectedMachineGroup].Machines;
                header1[i] = machines[machineIndex].Name;
                header2[i] = MACHINEHOUR_LIST_HEADER[0];
                for (j = 1; j < MACHINEHOUR_LIST_HEADER.Length; j++)
                {
                    header1[i + j] = "";
                    header2[i + j] = MACHINEHOUR_LIST_HEADER[j];
                }
                i = i + j;
            }

            Graphics e = CreateGraphics();

            for (int col = 0; col < headerColumnCount; col++)
            {

                float columnWidth = e.MeasureString(header2[col], gridView.DefaultCellStyle.Font).Width;
                gridView.Columns[col].Width = (int)Math.Ceiling(columnWidth) + 10;
            }
            e.Dispose();

            sectionWidth = 0;
            for (int col = MACHINEHOUR_LIST_ADDITION.Length;
                col < MACHINEHOUR_LIST_ADDITION.Length + MACHINEHOUR_LIST_HEADER.Length; col++)
                sectionWidth += gridView.Columns[col].Width;

            gridView.CellPainting += new DataGridViewCellPaintingEventHandler(HoursDataGridView_CellPainting);

            this.BtnExport.Enabled = false;
        }


        private void AddDataGridViewHeaders()
        {
            this.HoursDataGridView.Rows.Add(header1);
            this.HoursDataGridView.Rows.Add(header2);
        }

        private void InitialOutputCapacityChartInfo()
        {
            this.LblChartQueryPeriod.Font = new Font(OEETypes.FONT, 10F, System.Drawing.FontStyle.Regular);
            this.LblChartQueryPeriod.Text = startDate.ToString("yyyy/MM/dd") + " - " + endDate.ToString("yyyy/MM/dd");
            this.LblChartQueryPeriod.ForeColor = Color.White;

            this.LblChartTitle.Font = new Font(OEETypes.FONT, TITLE_FONT_SIZE, System.Drawing.FontStyle.Bold);
            this.LblChartTitle.Text = OUTPUT_CAPACITY_CHART_TITLE;
            this.LblChartTitle.ForeColor = Color.White;

            this.TlpChart.Controls.Add(this.PnlPercentage, 0, 1);
            this.PnlPercentage.Paint += new PaintEventHandler(PnlPercentage_PercentageChanged);
        }

        private void SetOutputCapacityChartStyle()
        {
            barChart.Name = OUTPUT_CAPACITY_CHART_NAME;
            barChart.BackColor = backgroudColor;
            barChart.Dock = DockStyle.Fill;
            barChart.Location = new System.Drawing.Point(3, 3);

        }

        private void SetOutCapacityChartAreaStyle()
        {
            barChartArea.BackColor = backgroudColor;

            barChartArea.AxisX.MajorGrid.Enabled = true;
            barChartArea.AxisX.MajorGrid.LineDashStyle = WinCharting.ChartDashStyle.Solid;
            barChartArea.AxisX.MajorGrid.LineColor = Color.Gray;
            barChartArea.AxisX.MajorTickMark.Enabled = false;

            barChartArea.AxisY.MajorGrid.Enabled = true;
            barChartArea.AxisY.MajorGrid.LineDashStyle = WinCharting.ChartDashStyle.Solid;
            barChartArea.AxisY.MajorGrid.LineColor = Color.Gray;
            barChartArea.AxisY.MajorTickMark.Enabled = false;

            barChartArea.AxisX.IsMarginVisible = false;
            barChartArea.AxisX.IsStartedFromZero = true;
            barChartArea.Area3DStyle.Enable3D = true;
            barChartArea.Area3DStyle.IsClustered = true;

            barChartArea.AxisX.Interval = 0.4;
            barChartArea.AxisX.LabelStyle.ForeColor = Color.White;
            barChartArea.AxisX.IsLabelAutoFit = false;
            barChartArea.AxisX.LabelStyle.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular);
            barChartArea.AxisX.LabelStyle.Angle = 30;
            barChartArea.AxisX.LabelStyle.Enabled = true;

            barChartArea.AxisY.IsLabelAutoFit = false;
            barChartArea.AxisY.LabelStyle.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular);
            barChartArea.AxisY.LabelStyle.ForeColor = Color.White;
            barChartArea.AxisY.LabelStyle.Enabled = true;

            for (int i = 1; i < machineGroups[selectedMachineGroup].Count + 1; i++)
            {
                barChartArea.AxisX.CustomLabels.Add(i - barChartArea.AxisX.Interval, i, OUTPUT_CAPACITY_SERIES_PLANNED);
                barChartArea.AxisX.CustomLabels.Add(i, i + barChartArea.AxisX.Interval, OUTPUT_CAPACITY_SERIES_REAL);

            }
        }

        private void SetPlannedSeriesStyle()
        {
            seriesPlanned.ChartType = WinCharting.SeriesChartType.Column;
            seriesPlanned.Name = OUTPUT_CAPACITY_SERIES_PLANNED;
            seriesPlanned.IsValueShownAsLabel = true;
            seriesPlanned.Color = Color.White;
            seriesPlanned.LabelForeColor = Color.White;
            seriesPlanned.CustomProperties = "DrawingStyle=Cylinder";
        }

        private void SetRealSeriesStyle()
        {
            seriesReal.ChartType = WinCharting.SeriesChartType.Column;
            seriesReal.Name = OUTPUT_CAPACITY_SERIES_REAL;
            seriesReal.IsValueShownAsLabel = true;
            seriesReal.Color = realColor;
            seriesReal.LabelForeColor = Color.White;
            seriesReal.CustomProperties = "DrawingStyle=Cylinder";
        }

        private void OEEManager_Load(object sender, EventArgs e)
        {
        }

        private void DtpTimeStart_ValueChanged(object sender, EventArgs e)
        {
            startDate = this.DtpMachineDateStart.Value;
        }

        private void DtpDateEnd_ValueChanged(object sender, EventArgs e)
        {
            endDate = this.DtpMachineDateEnd.Value;
        }

        private void BtnDeviceQuery_Click(object sender, EventArgs e)
        {
            int compareResult = DateTime.Compare(startDate, endDate);
            if (compareResult > 0)
            {
                MessageBox.Show("起始时间不能大于终止时间", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                SetQueryOneDayFlag(compareResult);

                DoQuery();
            }
        }

        private void CmbMachineType_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedMachineGroup = this.CmbMachineType.SelectedIndex;

        }



        private void TabMachineOuputStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShowQueryResult();
        }

        private void HoursDataGridView_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            DataGridView dgv = (DataGridView)sender;

            using (SolidBrush backColorBrush = new SolidBrush(e.CellStyle.BackColor))
            {
                Rectangle rect = new Rectangle(e.CellBounds.X, e.CellBounds.Y,
                e.CellBounds.Width, e.CellBounds.Height);

                e.Graphics.FillRectangle(backColorBrush, rect);

                if (e.RowIndex == 0)
                    DrawMachineNameLine(sender, e);
                else
                    DrawDataCell(sender, e);
            }

            e.Handled = true;
        }

        private void DrawMachineNameLine(object sender, DataGridViewCellPaintingEventArgs e)
        {
            DataGridView dgv = (DataGridView)sender;
            Graphics g = e.Graphics;
            using (SolidBrush backColorBrush = new SolidBrush(e.CellStyle.BackColor),
                valBrush = new SolidBrush(Color.Black))
            using (Pen linePen = new Pen(Color.Gray), backColorPen = new Pen(e.CellStyle.BackColor))
            {

                if (e.ColumnIndex < MACHINEHOUR_LIST_ADDITION.Length)
                    g.DrawLine(linePen, e.CellBounds.Right - 1, e.CellBounds.Top, e.CellBounds.Right - 1, e.CellBounds.Bottom - 1);
                else if ((e.ColumnIndex - MACHINEHOUR_LIST_ADDITION.Length) % MACHINEHOUR_LIST_HEADER.Length == 0)
                {
                    sectionX = e.CellBounds.Left;
                    sectionY = e.CellBounds.Top;
                }
                else
                {
                    if ((e.ColumnIndex + 1 - MACHINEHOUR_LIST_ADDITION.Length) % MACHINEHOUR_LIST_HEADER.Length == 0)
                    {
                        int column = e.ColumnIndex + 1 - MACHINEHOUR_LIST_HEADER.Length;
                        object value = dgv.Rows[e.RowIndex].Cells[column].Value;
                        if (value != null)
                        {
                            SizeF valSize = g.MeasureString(value.ToString(), dgv.DefaultCellStyle.Font);
                            float valLeft = (sectionWidth - valSize.Width) / 2;
                            float valTop = (e.CellBounds.Height - valSize.Height) / 2;

                            g.DrawString(value.ToString(), dgv.DefaultCellStyle.Font, valBrush,
                                sectionX + valLeft, valTop);
                        }
                        g.DrawLine(linePen, e.CellBounds.Right - 1, e.CellBounds.Top, e.CellBounds.Right - 1, e.CellBounds.Bottom - 1);
                    }
                }

            }
        }

        private void DrawDataCell(object sender, DataGridViewCellPaintingEventArgs e)
        {
            DataGridView dgv = (DataGridView)sender;
            Graphics g = e.Graphics;


            using (Pen linePen = new Pen(Color.Gray))
            using (SolidBrush valueBrush = new SolidBrush(Color.Black))
            {
                g.DrawLine(linePen, e.CellBounds.Left, e.CellBounds.Top, e.CellBounds.Right, e.CellBounds.Top);
                g.DrawLine(linePen, e.CellBounds.Right - 1, e.CellBounds.Top, e.CellBounds.Right - 1, e.CellBounds.Bottom - 1);
                if (e.Value != null)
                {
                    SizeF valueSize = g.MeasureString(e.Value.ToString(), e.CellStyle.Font);
                    float left = (e.CellBounds.Width - valueSize.Width) / 2;
                    float top = (e.CellBounds.Height - valueSize.Height) / 2;
                    g.DrawString(e.Value.ToString(), e.CellStyle.Font, valueBrush,
                        e.CellBounds.Left + left, e.CellBounds.Top + top);
                }

            }

        }


        private void DoQuery()
        {
            foreach (var machine in machineGroups[selectedMachineGroup].Machines)
                machine.QueryMachineStatus(startDate, endDate);

            ShowQueryResult();
        }

        private void ShowQueryResult()
        {
            switch (this.TabMachineOuputStatus.SelectedTab.Name)
            {
                case "PageOuputCapacity":
                    ShowOutputCapacity();
                    break;
                case "PageOutputStatus":
                    if (queryOneDayStatus)
                    {
                        if (this.TlpOutputStatus.Controls.Contains(pieChart))
                            this.TlpOutputStatus.Controls.Remove(pieChart);
                        this.TlpOutputStatus.Controls.Add(timeLine, 0, 1);
                        ShowMachineHoursInPrecise();
                    }

                    else
                    {
                        if (this.TlpOutputStatus.Controls.Contains(timeLine))
                            this.TlpOutputStatus.Controls.Remove(timeLine);
                        this.TlpOutputStatus.Controls.Add(pieChart, 0, 1);
                        ShowMachineHoursInRough();
                    }

                    break;
                case "PageOuputHours":
                    if (tableGenerated == false)
                    {
                        this.BtnExport.Enabled = true;
                        tableGenerated = true;
                    }
                        
                    ShowMachineHoursList();
                    break;
                default:
                    break;
            }
        }

        private void SetQueryOneDayFlag(int compareResult)
        {
            if (compareResult == 0)
                queryOneDayStatus = true;
            else
                queryOneDayStatus = false;
        }

        private void ShowOutputCapacity()
        {
            AddPlannedSeriesPoints();
            AddRealSeriesPoints();
            this.PnlPercentage.Invalidate();
        }

        private void PnlPercentage_PercentageChanged(object sender, PaintEventArgs e)
        {
            float x = this.barChartArea.Position.X;
            Panel panel = (Panel)sender;

            double width = this.barChartArea.AxisX.GetPosition(1) * this.barChart.Width / 100 * 0.8;
            int locY = PERCENTAGE_GROUP_POSITION_Y + PERCENTAGE_RECT_HEIGHT;

            for (int index = 0; index < machineGroups[selectedMachineGroup].Count; index++)
            {
                OEEMachine machine = machineGroups[selectedMachineGroup].Machines[index];
                using (Font font = new Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular))
                using (SolidBrush realBrush = new SolidBrush(realColor), baseBrush = new SolidBrush(Color.Silver),
                    fontBrush = new SolidBrush(Color.White))
                {
                    double locX = this.barChartArea.Position.X + this.barChartArea.InnerPlotPosition.X +
                        (float)(Math.Cos(Math.PI * 30 / 180.0) * width);
                    double axisX = this.barChartArea.AxisX.GetPosition(index) * this.barChart.Width / 100;

                    locX += axisX;
                    float padding = ((float)width - e.Graphics.MeasureString(machine.Name, font).Width) / 2;
                    e.Graphics.DrawString(machine.Name, font, fontBrush, (float)locX + padding, PERCENTAGE_GROUP_POSITION_Y);

                    Rectangle baseRect = new Rectangle((int)locX, locY, (int)width, PERCENTAGE_RECT_HEIGHT);
                    e.Graphics.FillRectangle(baseBrush, baseRect);


                    if (machine.Status.outputPlanned > 0)
                    {
                        double percentage = (double)machine.Status.outputQualified / (double)machine.Status.outputPlanned;
                        Rectangle realRect = new Rectangle((int)locX, locY, (int)(width * percentage), PERCENTAGE_RECT_HEIGHT);
                        e.Graphics.FillRectangle(realBrush, realRect);
                        locX += width + 3;
                        string value = Convert.ToString(Math.Round(percentage, 2) * 100) + "%";
                        e.Graphics.DrawString(value, font, fontBrush, (int)locX, locY);
                    }
                    else
                    {
                        locX += width + 3;
                        e.Graphics.DrawString("0%", font, fontBrush, (int)locX, locY);
                    }
                }
            }
        }

        private void AddPlannedSeriesPoints()
        {
            seriesPlanned.Points.Clear();
            foreach (var machine in machineGroups[selectedMachineGroup].Machines)
                seriesPlanned.Points.AddY(machine.Status.outputPlanned);
        }

        private void AddRealSeriesPoints()
        {
            seriesReal.Points.Clear();
            foreach (var machine in machineGroups[selectedMachineGroup].Machines)
                seriesReal.Points.AddY(machine.Status.outputQualified);
        }

        private void ShowMachineHoursInPrecise()
        {
            int index;

            for (int i = 0; i < seriesTimeLine.Length; i++)
                seriesTimeLine[i].Points.Clear();

            try
            {
                foreach (var machine in machineGroups[selectedMachineGroup].Machines)
                {
                    index = 0;
                    List<OEETypes.StatusPoint> statusPoint = machine.Status.statusPoints;
                    DateTime startPoint = statusPoint[0].time;
                    int startStatus = statusPoint[0].status;
                    int startMinutes = 0;
                    for (int i = 1; i < statusPoint.Count; i++)
                    {
                        DateTime endPoint = statusPoint[i].time;
                        TimeSpan timeSpan = endPoint - startPoint;
                        int minutes = Convert.ToInt32(timeSpan.TotalMinutes);
                        int colorIndex = 0;
                        switch (startStatus)
                        {
                            case gVariable.MACHINE_STATUS_DOWN:
                                colorIndex = 0;
                                break;
                            case gVariable.MACHINE_STATUS_IDLE:
                                colorIndex = 1;
                                break;
                            case gVariable.MACHINE_STATUS_STARTED:
                                colorIndex = 2;
                                break;
                            case gVariable.MACHINE_STATUS_DATA_ALARM:
                            case gVariable.MACHINE_STATUS_MATERIAL_ALARM:
                            case gVariable.MACHINE_STATUS_DEVICE_ALARM:
                                colorIndex = 3;
                                break;
                        }
                        seriesTimeLine[colorIndex].Points.Add(startMinutes, index, minutes);
                    }
                    index++;
                }
            }
            catch (Exception)
            {

            }
            // A temprary solution here
            timeLine.Invalidate();
        }

        private void ShowMachineHoursInRough()
        {
            int amount = machineGroups[selectedMachineGroup].Count;
            HoursSpan[] hourSpan = new HoursSpan[amount];

            CaculateMachineHoursInRough(ref hourSpan);
            FillPieChartData(ref hourSpan);
        }


        private void CaculateMachineHoursInRough(ref HoursSpan[] hourSpan)
        {
            int index = 0;

            foreach (var machine in machineGroups[selectedMachineGroup].Machines)
            {
                hourSpan[index].off = new TimeSpan(0);
                hourSpan[index].idle = new TimeSpan(0);
                hourSpan[index].alarm = new TimeSpan(0);
                hourSpan[index].valid = new TimeSpan(0);

                try
                {
                    List<OEETypes.StatusPoint> statusPoint = machine.Status.statusPoints;
                    DateTime startPoint = statusPoint[0].time;
                    int startStatus = statusPoint[0].status;
                    for (int i = 1; i < statusPoint.Count; i++)
                    {
                        DateTime endPoint = statusPoint[i].time;
                        CalculateHours(startPoint, endPoint, startStatus, ref hourSpan[index]);
                        startPoint = statusPoint[i].time;
                        startStatus = statusPoint[i].status;
                    }
                    if (statusPoint.Count % 2 != 0)
                    {
                        DateTime endPoint = OEEUtils.ConvertToEndOfDay(startPoint);
                        CalculateHours(startPoint, endPoint, startStatus, ref hourSpan[index]);
                    }
                    index++;
                }
                catch (Exception)
                {

                }

            }
        }

        private void FillPieChartData(ref HoursSpan[] hourSpan)
        {
            int machineCount = machineGroups[selectedMachineGroup].Count;
            TimeSpan querySpan = endDate.Subtract(startDate);
            long totalTicks = querySpan.Ticks;

            for (int i = 0; i < machineCount; i++)
            {
                pieIndex = 0;
                seriesStatus[i].Points.Clear();
                AddPieSeriesValue(seriesStatus[i], hourSpan[i].off.Ticks, totalTicks);
                AddPieSeriesValue(seriesStatus[i], hourSpan[i].idle.Ticks, totalTicks);
                AddPieSeriesValue(seriesStatus[i], hourSpan[i].valid.Ticks, totalTicks);
                AddPieSeriesValue(seriesStatus[i], hourSpan[i].alarm.Ticks, totalTicks);
            }
        }

        private void AddPieSeriesValue(WinCharting.Series series, long ticks, long totalTicks)
        {
            double percentage = ticks * 100 / totalTicks;

            if (percentage > 0)
            {
                series.Points.AddY(percentage);
                series.Points[pieIndex].Color = LENGENDS_COLOR[pieIndex];
                pieIndex++;
            }


        }

        private void ShowMachineHoursList()
        {
            OEETypes.GROUP_TYPE groupType = (OEETypes.GROUP_TYPE)selectedMachineGroup;
            switch (groupType)
            {
                case OEETypes.GROUP_TYPE.CASTING:
                    ShowCastingHoursList();
                    break;
            }
        }

        private void ShowCastingHoursList()
        {
            DataGridView gridView = this.HoursDataGridView;
            gridView.Rows.Clear();
            AddDataGridViewHeaders();
            int index = 0;

            for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
            {
                int paramsLength = machineGroups[selectedMachineGroup].Count * MACHINEHOUR_LIST_HEADER.Length + MACHINEHOUR_LIST_ADDITION.Length;
                string[] value = new string[paramsLength];
                int col = 0;
                int total = (int)Math.Ceiling((endDate - startDate).TotalMinutes);
                DateTime qStart = OEEUtils.ConvertToStartOfDay(date); ;
                DateTime qEnd = OEEUtils.ConvertToEndOfDay(date);

                DateTime startTimePoint = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
                long startTime_offset = (long)Math.Ceiling((startDate - startTimePoint).TotalSeconds);
                long endTime_offset = (long)Math.Ceiling((qEnd - startTimePoint).TotalSeconds);

                value[col++] = index.ToString();
                value[col++] = date.ToString("MM/dd");
                
                foreach (OEEMachine machine in machineGroups[selectedMachineGroup].Machines)
                {
                    DataRow[] dr;
                    int off = 0;
                    int idle = 0;
                    int work = 0;
                    int alarm = 0;
                    int valid = 0;
                    string cmd;
                    OEETypes.MachineStatus machineStatus = machine.Status;
                    DataTable dt = machineStatus.hoursDataTable;
                   
                    try
                    {
                        value[col++] = total.ToString();
                        cmd = "time>'" + startTime_offset + "' and time<'" + endTime_offset + "' and value1='" + gVariable.MACHINE_STATUS_DOWN + "'";
                        dr = dt.Select(cmd);
                        off = dr.Count();
                        value[col++] = off.ToString();
                        cmd = "time>'" + startTime_offset + "' and time<'" + endTime_offset + "' and value1='" + gVariable.MACHINE_STATUS_IDLE + "'";
                        dr = dt.Select(cmd);
                        idle = dr.Count();
                        value[col++] = idle.ToString();
                        cmd = "time>'" + startTime_offset + "' and time<'" + endTime_offset + "' and value1='" + gVariable.MACHINE_STATUS_STARTED + "'";
                        dr = dt.Select(cmd);
                        work = dr.Count();
                        value[col++] = work.ToString();
                        cmd = "time>'" + startTime_offset + "' and time<'" + endTime_offset + "' and value1>='" + gVariable.MACHINE_STATUS_DEVICE_ALARM + "'";
                        dr = dt.Select(cmd);
                        alarm = dr.Count();
                        value[col++] = alarm.ToString();
                        valid = total - idle - alarm;
                        value[col++] = valid.ToString();
                    }
                    catch (Exception)
                    {
                        off = 0;
                        value[col++] = off.ToString();
                        idle = 0;
                        value[col++] = idle.ToString();
                        work = 0;
                        value[col++] = work.ToString();
                        alarm = 0;
                        value[col++] = alarm.ToString();
                        valid = 0;
                        value[col++] = valid.ToString();
                    }
                  
                }
                index++;
                gridView.Rows.Add(value);
            }
        }

        private void BtnExport_Click(object sender, EventArgs e)
        {
            DataGridView gridView = this.HoursDataGridView;

            if (gridView.RowCount <= 2) return;

            string templatePath = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) 
                + "\\template\\" + EXCEL_TEMPLATE_NAME;
            FileStream template = new FileStream(templatePath, FileMode.Open, FileAccess.Read);

            XSSFWorkbook machineHourBook = new XSSFWorkbook(template);

            ISheet sheet1 = machineHourBook.GetSheet("Sheet1");
            ICellStyle cellStyle = machineHourBook.CreateCellStyle();
            cellStyle.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Center;
            cellStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
            cellStyle.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
            cellStyle.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
            cellStyle.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
            cellStyle.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;



            for (int row = 2; row < gridView.RowCount - 1; row++)
            {
                IRow currentRow = sheet1.CreateRow(row);
                for (int col = 0; col < gridView.ColumnCount; col++)
                {
                    ICell currentCell = currentRow.CreateCell(col);
                    currentCell.CellStyle = cellStyle;
                    currentCell.SetCellType(CellType.String);
                    currentCell.SetCellValue(gridView[col, row].Value.ToString());
                    sheet1.AutoSizeColumn(col);
                }
              
            }

            //string tempDir = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string tempDir = System.Environment.GetEnvironmentVariable("TEMP");
            string fileName = EXCEL_TEMPLATE_NAME.Split('.')[0] + "_" + DateTime.Now.ToString("hhss");
            string extenstion = EXCEL_TEMPLATE_NAME.Split('.')[1];
            fileName = fileName + "." + extenstion;
            System.Console.WriteLine("filename {0}", tempDir + "\\" + fileName);
        
            FileStream file = new FileStream(tempDir + "\\" + fileName, FileMode.Create);

            machineHourBook.Write(file);

            file.Close();
            template.Close();
            toolClass.nonBlockingDelay(2000);
            System.Diagnostics.Process.Start(tempDir + "\\" + fileName);
        }

        private void CalculateHours(DateTime startPoint, DateTime endPoint, int startStatus, ref HoursSpan hoursSpan)
        {
            TimeSpan start = new TimeSpan(startPoint.Ticks);
            TimeSpan end = new TimeSpan(endPoint.Ticks);
            TimeSpan span = start.Subtract(end).Duration();
            switch (startStatus)
            {
                case gVariable.MACHINE_STATUS_DOWN:
                    hoursSpan.off = hoursSpan.off.Add(span);
                    break;
                case gVariable.MACHINE_STATUS_IDLE:
                    hoursSpan.idle = hoursSpan.idle.Add(span);
                    break;
                case gVariable.MACHINE_STATUS_STARTED:
                    hoursSpan.valid = hoursSpan.valid.Add(span);
                    break;
                case gVariable.MACHINE_STATUS_DATA_ALARM:
                case gVariable.MACHINE_STATUS_MATERIAL_ALARM:
                case gVariable.MACHINE_STATUS_DEVICE_ALARM:
                    hoursSpan.alarm = hoursSpan.alarm.Add(span);
                    break;
            }
        }
    }
}
