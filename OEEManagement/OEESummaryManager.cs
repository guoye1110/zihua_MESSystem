using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

using MESSystem.common;

namespace MESSystem.OEEManagement
{
    public partial class OEESummaryManager : Form
    {
        /**************************************************** Constant ****************************************************/
        private float TITLE_FONT_SIZE = 22F;

        //TabOEESummary
        private const string SUMMARY_CHAR_NAME = "Summary";
        private const string SUMMARY_CHART_TITLE = "设备OEE";
        private const string SUMMARY_SERIES = "Summary";
        //TabHoursRate
        private const string HOURS_CHART_NAME = "HoursRate";
        private const string HOURS_SERIES_AVAILABLE = "开机";
        private const string HOURS_SERIES_ACTIVE = "生产";
        private const string HOURS_CHART_TITLE = "时间开动率";

        // TabPerformance
        private const string PERFORMANCE_CHART_NAME = "PerformanceRate";
        private const string PERFORMANCE_SERIES_PLANNED = "理论";
        private const string PERFORMANCE_SERIES_REAL = "实际";
        private const string PERFORMANCE_CHART_TITLE = "性能开动率";


        // TabQuality
        private const string QUALITY_CHART_NAME = "QualityRate";
        private const string QUALITY_CHART_TITLE = "产品合格率";
        private Color quality_background = Color.DarkSlateGray;
        private Color realColor = Color.Lime;
        private const string QUALITY_SERIES_REAL = "产出";
        private const string QUALITY_SERIES_QUALIFIED = "合格";

        private const int PERCENTAGE_GROUP_POSITION_Y = 20;
        private const int PERCENTAGE_RECT_HEIGHT = 20;
        private const int PERCENTAGE_GROUP_PADDING = 10;
        /***************************************************** Types ******************************************************/


        /********************************************************Variables*************************************************/
        private OEEMachineGroup[] machineGroups;
        private OEEMachine[] machines;

        private int selectedMachineGroup;

        private Chart oeeSummaryChart;
        private ChartArea oeeSummaryChartArea;
        private Series seriesSummary;

        private Chart hoursRateChart;
        private ChartArea hoursRateChartArea;
        private Series seriesAvailable;
        private Series seriesActive;

        private Chart performanceChart;
        private ChartArea performanceChartArea;
        private Series seriesPlanned;
        private Series seriesPerformanceReal;

        private Chart qualityBarChart;
        private ChartArea qualityBarChartArea;
        private Series seriesQualityReal;
        private Series seriesQualified;

        private List<double> summaryPercentage;
        private List<double> hoursRatePercentage;
        private List<double> performancePercentage;
        private List<double> qualityPercentage;

        private static string[] queryPeriod = { "年", "月" };

        DateTime startTime;
        DateTime endTime;

        /***************************************************** Property ***************************************************/

        public OEESummaryManager(OEEFactory factory)
        {
            machineGroups = factory.MachineGroups;
            InitializeComponent();
            InitializeParameters();
            InitializeOEESummary();
            InitializeTabPerformance();
            InitializeTabHoursRate();
            InitialTabQualityRate();

            DoQuery();
        }

        private void InitializeParameters()
        {
            DateTime now = System.DateTime.Now;

            summaryPercentage = new List<double>();
            hoursRatePercentage = new List<double>();
            qualityPercentage = new List<double>();
            performancePercentage = new List<double>();

            for (int i = 0; i < machineGroups.Count(); i++)
                this.CmbMachineType.Items.Add(machineGroups[i].Name);
            this.CmbMachineType.SelectedIndex = 0;


            for (int i = 0; i < queryPeriod.Count(); i++)
                this.CmbQueryPeriod.Items.Add(queryPeriod[i]);
            this.CmbQueryPeriod.SelectedIndex = 0;


            for (int i = OEETypes.YEAR_START; i <= OEETypes.YEART_END; i++)
                this.CmbYear.Items.Add(i.ToString());
            this.CmbYear.Text = now.Year.ToString();


            for (int i = OEETypes.MONTH_START; i <= OEETypes.MONTH_END; i++)
                this.CmbMonth.Items.Add(i.ToString());
            this.CmbMonth.Text = now.Month.ToString();


        }

        // TabOEESummary
        private void InitializeOEESummary()
        {
            InitializeOEESummaryChart();
            this.LblOEESummary.Font = new Font(OEETypes.FONT, TITLE_FONT_SIZE, System.Drawing.FontStyle.Bold);
            this.LblOEESummary.Text = SUMMARY_CHART_TITLE;
            this.LblOEESummary.ForeColor = Color.White;

            this.TlpOEESummary.BackColor = quality_background;

            this.TlpOEESummary.Controls.Add(this.PnlOEESummaryPercentage, 0, 1);
            this.PnlOEESummaryPercentage.Paint += new PaintEventHandler(PnlOEESummaryPercentage_PercentageChanged);
        }

        private void InitializeOEESummaryChart()
        {
            oeeSummaryChart = new Chart();
            oeeSummaryChartArea = new ChartArea();
            seriesSummary = new Series();

            oeeSummaryChart.ChartAreas.Add(oeeSummaryChartArea);
            oeeSummaryChart.Series.Add(seriesSummary);

            SetOeeSummaryChartStyle();
            SetOeeSummaryChartAreaStyle();
            SetSummarySeriesStyle();

            this.TlpOEESummary.Controls.Add(oeeSummaryChart, 0, 2);
        }

        private void SetOeeSummaryChartStyle()
        {
            oeeSummaryChart.Name = SUMMARY_CHAR_NAME;
            oeeSummaryChart.BackColor = quality_background;
            oeeSummaryChart.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom;
            oeeSummaryChart.Dock = DockStyle.Fill;
            oeeSummaryChart.Location = new System.Drawing.Point(3, 3);
        }

        private void SetOeeSummaryChartAreaStyle()
        {
            oeeSummaryChartArea.BackColor = quality_background;

            oeeSummaryChartArea.AxisX.MajorGrid.Enabled = true;
            oeeSummaryChartArea.AxisX.MajorGrid.LineDashStyle = ChartDashStyle.Solid;
            oeeSummaryChartArea.AxisX.MajorGrid.LineColor = Color.Gray;
            oeeSummaryChartArea.AxisX.MajorTickMark.Enabled = false;

            oeeSummaryChartArea.AxisY.MajorGrid.Enabled = true;
            oeeSummaryChartArea.AxisY.MajorGrid.LineDashStyle = ChartDashStyle.Solid;
            oeeSummaryChartArea.AxisY.MajorGrid.LineColor = Color.Gray;
            oeeSummaryChartArea.AxisY.MajorTickMark.Enabled = false;

            oeeSummaryChartArea.AxisX.IsMarginVisible = false;
            oeeSummaryChartArea.AxisX.IsStartedFromZero = true;
            oeeSummaryChartArea.Area3DStyle.Enable3D = true;
            oeeSummaryChartArea.Area3DStyle.Rotation = 0;
            oeeSummaryChartArea.Area3DStyle.IsClustered = true;

            oeeSummaryChartArea.AxisX.Interval = 0.4;
            oeeSummaryChartArea.AxisX.LabelStyle.ForeColor = Color.White;
            oeeSummaryChartArea.AxisX.IsLabelAutoFit = false;
            oeeSummaryChartArea.AxisX.LabelStyle.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular);
            oeeSummaryChartArea.AxisX.LabelStyle.Angle = 30;
            oeeSummaryChartArea.AxisX.LabelStyle.Enabled = true;

            oeeSummaryChartArea.AxisY.IsLabelAutoFit = false;
            oeeSummaryChartArea.AxisY.LabelStyle.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular);
            oeeSummaryChartArea.AxisY.LabelStyle.ForeColor = Color.White;
            oeeSummaryChartArea.AxisY.LabelStyle.Enabled = true;
        }

        private void SetSummarySeriesStyle()
        {
            seriesSummary.ChartType = SeriesChartType.Column;
            seriesSummary.Name = SUMMARY_SERIES;
            seriesSummary.IsValueShownAsLabel = true;
            seriesSummary.CustomProperties = "DrawingStyle = Cylinder";
            seriesSummary.LabelForeColor = Color.White;
            seriesSummary.Color = Color.White;
        }

        // TabHoursRate
        private void InitializeTabHoursRate()
        {
            InitializeHoursRateChart();
            this.LblHoursTitle.Font = new Font(OEETypes.FONT, TITLE_FONT_SIZE, System.Drawing.FontStyle.Bold);
            this.LblHoursTitle.Text = PERFORMANCE_CHART_TITLE;
            this.LblHoursTitle.ForeColor = Color.White;

            this.TlpHoursRate.BackColor = quality_background;

            this.TlpHoursRate.Controls.Add(this.PnlHoursRatePercentage, 0, 1);
            this.PnlHoursRatePercentage.Paint += new PaintEventHandler(PnlHoursRatePercentage_PercentageChanged);
        }

        private void InitializeHoursRateChart()
        {
            hoursRateChart = new Chart();
            hoursRateChartArea = new ChartArea();
            seriesAvailable = new Series();
            seriesActive = new Series();

            SetHoursRateChartStyle();
            SetHoursRateChartAreaStyle();
            SetAvailableSeriesStyle();
            SetActiveSeriesStyle();

            hoursRateChart.ChartAreas.Add(hoursRateChartArea);

            hoursRateChart.Series.Add(seriesAvailable);
            hoursRateChart.Series.Add(seriesActive);

            this.TlpHoursRate.Controls.Add(hoursRateChart, 0, 2);
        }

        private void SetHoursRateChartStyle()
        {
            hoursRateChart.Name = HOURS_CHART_NAME;
            hoursRateChart.BackColor = quality_background;
            hoursRateChart.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom;
            hoursRateChart.Dock = DockStyle.Fill;
            hoursRateChart.Location = new System.Drawing.Point(3, 3);
        }

        private void SetHoursRateChartAreaStyle()
        {
            hoursRateChartArea.BackColor = quality_background;

            hoursRateChartArea.AxisX.MajorGrid.Enabled = true;
            hoursRateChartArea.AxisX.MajorGrid.LineDashStyle = ChartDashStyle.Solid;
            hoursRateChartArea.AxisX.MajorGrid.LineColor = Color.Gray;
            hoursRateChartArea.AxisX.MajorTickMark.Enabled = false;

            hoursRateChartArea.AxisY.MajorGrid.Enabled = true;
            hoursRateChartArea.AxisY.MajorGrid.LineDashStyle = ChartDashStyle.Solid;
            hoursRateChartArea.AxisY.MajorGrid.LineColor = Color.Gray;
            hoursRateChartArea.AxisY.MajorTickMark.Enabled = false;

            hoursRateChartArea.AxisX.IsMarginVisible = false;
            hoursRateChartArea.AxisX.IsStartedFromZero = true;
            hoursRateChartArea.Area3DStyle.Enable3D = true;
            hoursRateChartArea.Area3DStyle.Rotation = 0;
            hoursRateChartArea.Area3DStyle.IsClustered = true;

            hoursRateChartArea.AxisX.Interval = 0.4;
            hoursRateChartArea.AxisX.LabelStyle.ForeColor = Color.White;
            hoursRateChartArea.AxisX.IsLabelAutoFit = false;
            hoursRateChartArea.AxisX.LabelStyle.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular);
            hoursRateChartArea.AxisX.LabelStyle.Angle = 30;
            hoursRateChartArea.AxisX.LabelStyle.Enabled = true;

            hoursRateChartArea.AxisY.IsLabelAutoFit = false;
            hoursRateChartArea.AxisY.LabelStyle.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular);
            hoursRateChartArea.AxisY.LabelStyle.ForeColor = Color.White;
            hoursRateChartArea.AxisY.LabelStyle.Enabled = true;

            for (int i = 1; i < machineGroups[selectedMachineGroup].Count + 1; i++)
            {
                hoursRateChartArea.AxisX.CustomLabels.Add(i - hoursRateChartArea.AxisX.Interval, i, HOURS_SERIES_AVAILABLE);
                hoursRateChartArea.AxisX.CustomLabels.Add(i, i + hoursRateChartArea.AxisX.Interval, HOURS_SERIES_ACTIVE);
            }
        }

        private void SetAvailableSeriesStyle()
        {
            seriesAvailable.ChartType = SeriesChartType.Column;
            seriesAvailable.Name = HOURS_SERIES_AVAILABLE;
            seriesAvailable.IsValueShownAsLabel = true;
            seriesAvailable.CustomProperties = "DrawingStyle = Cylinder";
            seriesAvailable.LabelForeColor = Color.White;
            seriesAvailable.Color = Color.White;
        }

        private void SetActiveSeriesStyle()
        {
            seriesActive.ChartType = SeriesChartType.Column;
            seriesActive.Name = HOURS_SERIES_ACTIVE;
            seriesActive.IsValueShownAsLabel = true;
            seriesActive.CustomProperties = "DrawingStyle = Cylinder";
            seriesActive.LabelForeColor = Color.White;
            seriesActive.Color = realColor;
        }

        // TabPerformance
        private void InitializeTabPerformance()
        {
            InitializePerformanceChart();
            this.LblPerformanceTitle.Font = new Font(OEETypes.FONT, TITLE_FONT_SIZE, System.Drawing.FontStyle.Bold);
            this.LblPerformanceTitle.Text = PERFORMANCE_CHART_TITLE;
            this.LblPerformanceTitle.ForeColor = Color.White;

            this.TlpPerformance.BackColor = quality_background;

            this.TlpPerformance.Controls.Add(this.PnlPerformancePercentage, 0, 1);
            this.PnlPerformancePercentage.Paint += new PaintEventHandler(PnlPerformancePercentage_PercentageChanged);
        }

        private void InitializePerformanceChart()
        {
            performanceChart = new Chart();
            performanceChartArea = new ChartArea();
            seriesPlanned = new Series();
            seriesPerformanceReal = new Series();

            SetPerformanceChartStyle();
            SetPerformanceChartAreaStyle();
            SetPlannedSeriesStyle();
            SetPerformanceRealSeriesStyle();

            performanceChart.ChartAreas.Add(performanceChartArea);

            performanceChart.Series.Add(seriesPlanned);
            performanceChart.Series.Add(seriesPerformanceReal);

            this.TlpPerformance.Controls.Add(performanceChart, 0, 2);
        }

        private void SetPerformanceChartStyle()
        {
            performanceChart.Name = PERFORMANCE_CHART_NAME;
            performanceChart.BackColor = quality_background;
            performanceChart.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom;
            performanceChart.Dock = DockStyle.Fill;
            performanceChart.Location = new System.Drawing.Point(3, 3);
        }



        private void SetPerformanceChartAreaStyle()
        {
            performanceChartArea.BackColor = quality_background;

            performanceChartArea.AxisX.MajorGrid.Enabled = true;
            performanceChartArea.AxisX.MajorGrid.LineDashStyle = ChartDashStyle.Solid;
            performanceChartArea.AxisX.MajorGrid.LineColor = Color.Gray;
            performanceChartArea.AxisX.MajorTickMark.Enabled = false;

            performanceChartArea.AxisY.MajorGrid.Enabled = true;
            performanceChartArea.AxisY.MajorGrid.LineDashStyle = ChartDashStyle.Solid;
            performanceChartArea.AxisY.MajorGrid.LineColor = Color.Gray;
            performanceChartArea.AxisY.MajorTickMark.Enabled = false;

            performanceChartArea.AxisX.IsMarginVisible = false;
            performanceChartArea.AxisX.IsStartedFromZero = true;
            performanceChartArea.Area3DStyle.Enable3D = true;
            performanceChartArea.Area3DStyle.Rotation = 0;
            performanceChartArea.Area3DStyle.IsClustered = true;

            performanceChartArea.AxisX.Interval = 0.4;
            performanceChartArea.AxisX.LabelStyle.ForeColor = Color.White;
            performanceChartArea.AxisX.IsLabelAutoFit = false;
            performanceChartArea.AxisX.LabelStyle.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular);
            performanceChartArea.AxisX.LabelStyle.Angle = 30;
            performanceChartArea.AxisX.LabelStyle.Enabled = true;

            performanceChartArea.AxisY.IsLabelAutoFit = false;
            performanceChartArea.AxisY.LabelStyle.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular);
            performanceChartArea.AxisY.LabelStyle.ForeColor = Color.White;
            performanceChartArea.AxisY.LabelStyle.Enabled = true;

            for (int i = 1; i < machineGroups[selectedMachineGroup].Count + 1; i++)
            {
                performanceChartArea.AxisX.CustomLabels.Add(i - performanceChartArea.AxisX.Interval, i, PERFORMANCE_SERIES_PLANNED);
                performanceChartArea.AxisX.CustomLabels.Add(i, i + performanceChartArea.AxisX.Interval, PERFORMANCE_SERIES_REAL);
            }
        }

        private void SetPlannedSeriesStyle()
        {
            seriesPlanned.ChartType = SeriesChartType.Column;
            seriesPlanned.Name = PERFORMANCE_SERIES_PLANNED;
            seriesPlanned.IsValueShownAsLabel = true;
            seriesPlanned.CustomProperties = "DrawingStyle = Cylinder";
            seriesPlanned.LabelForeColor = Color.White;
            seriesPlanned.Color = Color.White;
        }

        private void SetPerformanceRealSeriesStyle()
        {
            seriesPerformanceReal.ChartType = SeriesChartType.Column;
            seriesPerformanceReal.Name = PERFORMANCE_SERIES_REAL;
            seriesPerformanceReal.IsValueShownAsLabel = true;
            seriesPerformanceReal.CustomProperties = "DrawingStyle = Cylinder";
            seriesPerformanceReal.LabelForeColor = Color.White;
            seriesPerformanceReal.Color = realColor;
        }

        // TabQualityRate
        private void InitialTabQualityRate()
        {
            InitializeQualityChart();

            this.LblQualityTitle.Font = new Font(OEETypes.FONT, TITLE_FONT_SIZE, System.Drawing.FontStyle.Bold);
            this.LblQualityTitle.Text = QUALITY_CHART_TITLE;
            this.LblQualityTitle.ForeColor = Color.White;

            this.TlpQualityRate.BackColor = quality_background;

            this.TlpQualityRate.Controls.Add(this.PnlQualityPercentage, 0, 1);
            this.PnlQualityPercentage.Paint += new PaintEventHandler(PnlQualityPercentage_PercentageChanged);

        }

        private void InitializeQualityChart()
        {
            qualityBarChart = new Chart();
            qualityBarChartArea = new ChartArea();
            seriesQualityReal = new Series();
            seriesQualified = new Series();

            SetQualityChartStyle();
            SetQualityChartAreaStyle();
            SetQualityRealSeriesStyle();
            SetQualifiedSeriesStyle();

            qualityBarChart.ChartAreas.Add(qualityBarChartArea);
            qualityBarChart.Series.Add(seriesQualityReal);
            qualityBarChart.Series.Add(seriesQualified);

            this.TlpQualityRate.Controls.Add(qualityBarChart, 0, 2);
        }

        private void SetQualityChartStyle()
        {
            qualityBarChart.Name = QUALITY_CHART_NAME;
            qualityBarChart.BackColor = quality_background;
            qualityBarChart.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom;
            qualityBarChart.Dock = DockStyle.Fill;
            qualityBarChart.Location = new System.Drawing.Point(3, 3);
        }

        private void SetQualityChartAreaStyle()
        {
            qualityBarChartArea.BackColor = quality_background;

            qualityBarChartArea.AxisX.MajorGrid.Enabled = true;
            qualityBarChartArea.AxisX.MajorGrid.LineDashStyle = ChartDashStyle.Solid;
            qualityBarChartArea.AxisX.MajorGrid.LineColor = Color.Gray;
            qualityBarChartArea.AxisX.MajorTickMark.Enabled = false;

            qualityBarChartArea.AxisY.MajorGrid.Enabled = true;
            qualityBarChartArea.AxisY.MajorGrid.LineDashStyle = ChartDashStyle.Solid;
            qualityBarChartArea.AxisY.MajorGrid.LineColor = Color.Gray;
            qualityBarChartArea.AxisY.MajorTickMark.Enabled = false;

            qualityBarChartArea.AxisX.IsMarginVisible = false;
            qualityBarChartArea.AxisX.IsStartedFromZero = true;
            qualityBarChartArea.Area3DStyle.Enable3D = true;
            qualityBarChartArea.Area3DStyle.Rotation = 0;
            qualityBarChartArea.Area3DStyle.IsClustered = true;


            qualityBarChartArea.AxisX.Interval = 0.4;
            qualityBarChartArea.AxisX.LabelStyle.ForeColor = Color.White;
            qualityBarChartArea.AxisX.IsLabelAutoFit = false;
            qualityBarChartArea.AxisX.LabelStyle.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular);
            qualityBarChartArea.AxisX.LabelStyle.Angle = 30;
            qualityBarChartArea.AxisX.LabelStyle.Enabled = true;

            qualityBarChartArea.AxisY.IsLabelAutoFit = false;
            qualityBarChartArea.AxisY.LabelStyle.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular);
            qualityBarChartArea.AxisY.LabelStyle.ForeColor = Color.White;
            qualityBarChartArea.AxisY.LabelStyle.Enabled = true;

            for (int i = 1; i < machineGroups[selectedMachineGroup].Count + 1; i++)
            {
                qualityBarChartArea.AxisX.CustomLabels.Add(i - qualityBarChartArea.AxisX.Interval, i, QUALITY_SERIES_REAL);
                qualityBarChartArea.AxisX.CustomLabels.Add(i, i + qualityBarChartArea.AxisX.Interval, QUALITY_SERIES_QUALIFIED);
            }
        }


        private void SetQualityRealSeriesStyle()
        {
            seriesQualityReal.ChartType = SeriesChartType.Column;
            seriesQualityReal.Name = QUALITY_SERIES_REAL;
            seriesQualityReal.IsValueShownAsLabel = true;
            seriesQualityReal.CustomProperties = "DrawingStyle = Cylinder";
            seriesQualityReal.LabelForeColor = Color.White;
            seriesQualityReal.Color = realColor;
        }

        private void SetQualifiedSeriesStyle()
        {
            seriesQualified.ChartType = SeriesChartType.Column;
            seriesQualified.Name = QUALITY_SERIES_QUALIFIED;
            seriesQualified.IsValueShownAsLabel = true;
            seriesQualified.CustomProperties = "DrawingStyle = Cylinder";
            seriesQualified.LabelForeColor = Color.White;
            seriesQualified.Color = Color.White;
        }

        private void CmbMachineType_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedMachineGroup = this.CmbMachineType.SelectedIndex;
            machines = machineGroups[selectedMachineGroup].Machines;
            summaryPercentage.Clear();
            hoursRatePercentage.Clear();
            performancePercentage.Clear();
            qualityPercentage.Clear();
        }

        private void TabSummary_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShowResult();
        }

        private void BtnQuery_Click(object sender, EventArgs e)
        {
            DoQuery();
        }

        private void DoQuery()
        {
            QueryMachineStatus();
            CalculateHoursRate();
            CalculatePerformanceRate();
            CalculateQualityRate();
            DoSummary();

            ShowResult();

        }

        private void QueryMachineStatus()
        {
            int year = Convert.ToInt32(this.CmbYear.Text);
           
            if (this.CmbQueryPeriod.SelectedIndex == 0)
            {
                startTime = new DateTime(year, OEETypes.MONTH_START, OEETypes.DAY_START);
                endTime = new DateTime(year, OEETypes.MONTH_END, OEETypes.DAY_SOLAR_END);
            }
            else
            {
                int month = Convert.ToInt32(this.CmbMonth.Text);
                startTime = OEEUtils.ConvertToDayStartOfMonth(year, month);
                endTime = OEEUtils.ConvertToDayEndOfMonth(year, month);
            }

            foreach (OEEMachine machine in machines)
            {
                machine.QueryMachineStatus(startTime, endTime);
            }
        }


        private void CalculateHoursRate()
        {
            foreach (OEEMachine machine in machineGroups[selectedMachineGroup].Machines)
            {
                string cmd;
                DataRow[] dr;
                DataTable dt = machine.Status.hoursDataTable;
                int alarmHours = 0;


                cmd = "value1>='" + gVariable.MACHINE_STATUS_DEVICE_ALARM + "'";
                if (dt != null)
                {
                    dr = dt.Select(cmd);
                    alarmHours = dr.Length / 3600;
                }


                int totalHours = (int)Math.Ceiling(endTime.Subtract(startTime).TotalHours);
                int plannedHours = totalHours - machine.Status.maintainanceHours;
                int runningHours = plannedHours - alarmHours - machine.Status.prepareHours;

                if (plannedHours > 0)
                {
                    double percentage = (double)runningHours / (double)plannedHours;
                    hoursRatePercentage.Add(percentage);
                }
                else
                    hoursRatePercentage.Add(0);


            }
        }

        private void CalculatePerformanceRate()
        {
            foreach (OEEMachine machine in machineGroups[selectedMachineGroup].Machines)
            {
                if (machine.Status.outputReal > 0)
                {
                    double percentage = (double)machine.Status.outputReal / (double)machine.Status.outputPlanned;
                    performancePercentage.Add(percentage);
                }
                else
                    performancePercentage.Add(0);
            }
        }

        private void CalculateQualityRate()
        {
            foreach (OEEMachine machine in machineGroups[selectedMachineGroup].Machines)
            {
                if (machine.Status.outputReal > 0)
                {
                    double percentage = (double)machine.Status.outputQualified / (double)machine.Status.outputReal;
                    qualityPercentage.Add(percentage);
                }
                else
                    qualityPercentage.Add(0);
            }
        }

        private void DoSummary()
        {
            for (int i = 0; i < machineGroups[selectedMachineGroup].Count; i++)
            {
                double percentage = hoursRatePercentage[i] * performancePercentage[i] * qualityPercentage[i] / 100 / 3;
                if (percentage > 0)
                    summaryPercentage.Add(percentage);
                else
                    summaryPercentage.Add(0);
            }
        }

        private void ShowResult()
        {
            switch (this.TabSummary.SelectedIndex)
            {
                case 0: //OEE
                    ShowOeeSummary();
                    break;
                case 1: //Time rate
                    ShowHoursRate();
                    break;
                case 2: //performance rate
                    ShowPerformanceRate();
                    break;
                case 3: //quality rate
                    ShowQualityRate();
                    break;
            }
        }

        private void ShowOeeSummary()
        {
            seriesSummary.Points.Clear();
            foreach (OEEMachine machine in machines)
            {
                seriesSummary.Points.AddY(machine.Status.outputReal);
            }
            this.PnlOEESummaryPercentage.Invalidate();
        }

        private void ShowHoursRate()
        {
            seriesAvailable.Points.Clear();
            seriesActive.Points.Clear();
            foreach (OEEMachine machine in machines)
            {
                seriesAvailable.Points.AddY(machine.Status.outputReal);
                seriesActive.Points.AddY(machine.Status.outputQualified);
            }
            this.PnlHoursRatePercentage.Invalidate();
        }

        private void ShowPerformanceRate()
        {
            seriesPlanned.Points.Clear();
            seriesPerformanceReal.Points.Clear();
            foreach (OEEMachine machine in machines)
            {
                seriesPlanned.Points.AddY(machine.Status.outputPlanned);
                seriesPerformanceReal.Points.AddY(machine.Status.outputReal);
            }
        }

        private void ShowQualityRate()
        {
            seriesQualityReal.Points.Clear();
            seriesQualified.Points.Clear();
            foreach (OEEMachine machine in machines)
            {
                seriesQualityReal.Points.AddY(machine.Status.outputReal);
                seriesQualified.Points.AddY(machine.Status.outputQualified);
            }
            this.PnlQualityPercentage.Invalidate();
        }

        private void CmbQueryPeriod_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.CmbQueryPeriod.Text == queryPeriod[0])
            {
                this.LblMonth.Visible = false;
                this.CmbMonth.Visible = false;
            }
            else
            {
                this.LblMonth.Visible = true;
                this.CmbMonth.Visible = true;
            }
        }


        private void PnlOEESummaryPercentage_PercentageChanged(object sender, PaintEventArgs e)
        {
            float x = this.oeeSummaryChartArea.Position.X;
            Panel panel = (Panel)sender;

            double width = this.oeeSummaryChartArea.AxisX.GetPosition(1) * this.oeeSummaryChart.Width / 100 * 0.8;
            int locY = PERCENTAGE_GROUP_POSITION_Y + PERCENTAGE_RECT_HEIGHT;

            for (int index = 0; index < machineGroups[selectedMachineGroup].Count; index++)
            {
                OEEMachine machine = machineGroups[selectedMachineGroup].Machines[index];
                using (Font font = new Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular))
                using (SolidBrush realBrush = new SolidBrush(realColor), baseBrush = new SolidBrush(Color.Silver),
                    fontBrush = new SolidBrush(Color.White))
                {
                    double locX = this.oeeSummaryChartArea.Position.X + this.oeeSummaryChartArea.InnerPlotPosition.X + width;
                    double axisX = this.oeeSummaryChartArea.AxisX.GetPosition(index) * this.oeeSummaryChart.Width / 100;

                    locX += axisX;
                    float padding = ((float)width - e.Graphics.MeasureString(machine.Name, font).Width) / 2;
                    e.Graphics.DrawString(machine.Name, font, fontBrush, (float)locX + padding, PERCENTAGE_GROUP_POSITION_Y);

                    Rectangle baseRect = new Rectangle((int)locX, locY, (int)width, PERCENTAGE_RECT_HEIGHT);
                    e.Graphics.FillRectangle(baseBrush, baseRect);

                    double percentage = summaryPercentage[index];
                    Rectangle realRect = new Rectangle((int)locX, locY, (int)(width * percentage), PERCENTAGE_RECT_HEIGHT);
                    e.Graphics.FillRectangle(realBrush, realRect);
                    locX += width + 3;
                    string value = Convert.ToString(Math.Round(percentage, 2) * 100) + "%";
                    e.Graphics.DrawString(value, font, fontBrush, (int)locX, locY);

                }
            }
        }

        private void PnlHoursRatePercentage_PercentageChanged(object sender, PaintEventArgs e)
        {

            float x = this.hoursRateChartArea.Position.X;
            Panel panel = (Panel)sender;


            double width = this.hoursRateChartArea.AxisX.GetPosition(1) * this.hoursRateChart.Width / 100 * 0.8;
            int locY = PERCENTAGE_GROUP_POSITION_Y + PERCENTAGE_RECT_HEIGHT;

            for (int index = 0; index < machineGroups[selectedMachineGroup].Count; index++)
            {
                OEEMachine machine = machineGroups[selectedMachineGroup].Machines[index];
                using (Font font = new Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular))
                using (SolidBrush realBrush = new SolidBrush(realColor), baseBrush = new SolidBrush(Color.Silver),
                    fontBrush = new SolidBrush(Color.White))
                {
                    double locX = this.hoursRateChartArea.Position.X + this.hoursRateChartArea.InnerPlotPosition.X + width;
                    double axisX = this.hoursRateChartArea.AxisX.GetPosition(index) * this.hoursRateChart.Width / 100;

                    locX += axisX;
                    float padding = ((float)width - e.Graphics.MeasureString(machine.Name, font).Width) / 2;
                    e.Graphics.DrawString(machine.Name, font, fontBrush, (float)locX + padding, PERCENTAGE_GROUP_POSITION_Y);

                    Rectangle baseRect = new Rectangle((int)locX, locY, (int)width, PERCENTAGE_RECT_HEIGHT);
                    e.Graphics.FillRectangle(baseBrush, baseRect);


                    double percentage = hoursRatePercentage[index];
                    Rectangle realRect = new Rectangle((int)locX, locY, (int)(width * percentage), PERCENTAGE_RECT_HEIGHT);
                    e.Graphics.FillRectangle(realBrush, realRect);
                    locX += width + 3;
                    string value = Convert.ToString(Math.Round(percentage, 2) * 100) + "%";
                    e.Graphics.DrawString(value, font, fontBrush, (int)locX, locY);

                }
            }

        }

        private void PnlPerformancePercentage_PercentageChanged(object sender, PaintEventArgs e)
        {
            float x = this.performanceChartArea.Position.X;
            Panel panel = (Panel)sender;

            double width = this.performanceChartArea.AxisX.GetPosition(1) * this.performanceChart.Width / 100 * 0.8;
            int locY = PERCENTAGE_GROUP_POSITION_Y + PERCENTAGE_RECT_HEIGHT;

            for (int index = 0; index < machineGroups[selectedMachineGroup].Count; index++)
            {
                OEEMachine machine = machineGroups[selectedMachineGroup].Machines[index];
                using (Font font = new Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular))
                using (SolidBrush realBrush = new SolidBrush(realColor), baseBrush = new SolidBrush(Color.Silver),
                    fontBrush = new SolidBrush(Color.White))
                {
                    double locX = this.performanceChartArea.Position.X + this.performanceChartArea.InnerPlotPosition.X + width;
                    double axisX = this.performanceChartArea.AxisX.GetPosition(index) * this.performanceChart.Width / 100;

                    locX += axisX;
                    float padding = ((float)width - e.Graphics.MeasureString(machine.Name, font).Width) / 2;
                    e.Graphics.DrawString(machine.Name, font, fontBrush, (float)locX + padding, PERCENTAGE_GROUP_POSITION_Y);

                    Rectangle baseRect = new Rectangle((int)locX, locY, (int)width, PERCENTAGE_RECT_HEIGHT);
                    e.Graphics.FillRectangle(baseBrush, baseRect);

                    double percentage = performancePercentage[index];
                    Rectangle realRect = new Rectangle((int)locX, locY, (int)(width * percentage), PERCENTAGE_RECT_HEIGHT);
                    e.Graphics.FillRectangle(realBrush, realRect);
                    locX += width + 3;
                    string value = Convert.ToString(Math.Round(percentage, 2) * 100) + "%";
                    e.Graphics.DrawString(value, font, fontBrush, (int)locX, locY);

                }
            }

        }

        private void PnlQualityPercentage_PercentageChanged(object sender, PaintEventArgs e)
        {
            float x = this.qualityBarChartArea.Position.X;
            Panel panel = (Panel)sender;


            double width = this.qualityBarChartArea.AxisX.GetPosition(1) * this.qualityBarChart.Width / 100 * 0.8;
            int locY = PERCENTAGE_GROUP_POSITION_Y + PERCENTAGE_RECT_HEIGHT;

            for (int index = 0; index < machineGroups[selectedMachineGroup].Count; index++)
            {
                OEEMachine machine = machineGroups[selectedMachineGroup].Machines[index];
                using (Font font = new Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular))
                using (SolidBrush realBrush = new SolidBrush(realColor), baseBrush = new SolidBrush(Color.Silver),
                    fontBrush = new SolidBrush(Color.White))
                {
                    double locX = this.qualityBarChartArea.Position.X + this.qualityBarChartArea.InnerPlotPosition.X + width;
                    double axisX = this.qualityBarChartArea.AxisX.GetPosition(index) * this.qualityBarChart.Width / 100;

                    locX += axisX;
                    float padding = ((float)width - e.Graphics.MeasureString(machine.Name, font).Width) / 2;
                    e.Graphics.DrawString(machine.Name, font, fontBrush, (float)locX + padding, PERCENTAGE_GROUP_POSITION_Y);

                    Rectangle baseRect = new Rectangle((int)locX, locY, (int)width, PERCENTAGE_RECT_HEIGHT);
                    e.Graphics.FillRectangle(baseBrush, baseRect);



                    double percentage = qualityPercentage[index];
                    Rectangle realRect = new Rectangle((int)locX, locY, (int)(width * percentage), PERCENTAGE_RECT_HEIGHT);
                    e.Graphics.FillRectangle(realBrush, realRect);
                    locX += width + 3;
                    string value = Convert.ToString(Math.Round(percentage, 2) * 100) + "%";
                    e.Graphics.DrawString(value, font, fontBrush, (int)locX, locY);

                }
            }

        }

    }
}
