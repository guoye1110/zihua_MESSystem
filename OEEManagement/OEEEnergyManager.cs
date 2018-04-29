using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

using MESSystem.common;
namespace MESSystem.OEEManagement
{
    public partial class OEEEnergyManager : Form
    {
        /**************************************************** Constant ****************************************************/
        private const string ENERGY_CHART_NAME = "energy";
        private const string ENERGY_CHART_TITLE = "title";
        private const string SERIES_ENERGY = "seriesname";
        /***************************************************** Types ******************************************************/


        /********************************************************Variables*************************************************/
        private OEEMachineGroup[] machineGroups;
        private Chart barChart;
        private ChartArea barChartArea;
        private Series seriesEnergy;

        private static string[] energyViewHeader = { "工单号", "能耗"};
        /***************************************************** Property ***************************************************/


        public OEEEnergyManager(OEEFactory factory)
        {
            machineGroups = factory.MachineGroups;
            InitializeComponent();
            InitializeParameters();
            InitializeChart();
            InitializeList();
        }

        private void InitializeParameters()
        {
            for (int i = 0; i < machineGroups.Count(); i++)
                this.CmbMachineType.Items.Add(machineGroups[i].Name);
        }

        private void InitializeChart()
        {
            barChart = new Chart();
            barChartArea = new ChartArea();
            seriesEnergy = new Series();

            SetChartStyle();
            SetChartAreaStyle();
            SetSeriesStyle();

            barChart.ChartAreas.Add(barChartArea);
            barChart.Series.Add(seriesEnergy);

            this.PageEnergyChart.Controls.Add(barChart);
        }

        private void SetChartStyle()
        {
            barChart.Name = ENERGY_CHART_NAME;
            barChart.Titles.Add(ENERGY_CHART_TITLE);
            barChart.BackColor = Color.DarkSlateGray;
            barChart.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom;
            barChart.Dock = DockStyle.Fill;
            barChart.Location = new System.Drawing.Point(3, 3);

        }

        private void SetChartAreaStyle()
        {
            barChartArea.AxisX.MajorGrid.Enabled = true;
            barChartArea.AxisX.MajorGrid.LineDashStyle = ChartDashStyle.Solid;
            barChartArea.AxisX.MajorGrid.LineColor = Color.Gray;
            barChartArea.AxisX.MajorTickMark.Enabled = false;

            barChartArea.AxisY.MajorGrid.Enabled = true;
            barChartArea.AxisY.MajorGrid.LineDashStyle = ChartDashStyle.Solid;
            barChartArea.AxisY.MajorGrid.LineColor = Color.Gray;
            barChartArea.AxisY.MajorTickMark.Enabled = false;

            barChartArea.AxisX.IsMarginVisible = true;
            barChartArea.Area3DStyle.Enable3D = true;
        }
        
        private void SetSeriesStyle()
        {
            seriesEnergy.ChartType = SeriesChartType.Column;
            seriesEnergy.Name = SERIES_ENERGY;
            seriesEnergy.IsValueShownAsLabel = true;
        }


        private void InitializeList()
        {
            for (int i = 0; i < energyViewHeader.Length; i++)
            {
                ColumnHeader header = new ColumnHeader();
                header.AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
                header.TextAlign = HorizontalAlignment.Center;
                header.Text = energyViewHeader[i];
                this.LvwEnergyView.Columns.Add(header);
            }
        }
        private void BtnQuery_Click(object sender, EventArgs e)
        {
            DateTime startTime = Convert.ToDateTime(this.DtpStart.Text);
            DateTime endTime = Convert.ToDateTime(this.DtpEnd.Text);

            int selectedGroup = this.CmbMachineType.SelectedIndex;
            OEEMachineGroup machinGroup = machineGroups[selectedGroup];
            int[] energyConsumption = new int[machinGroup.Count];
            List<OEETypes.EnergyConsumption> energyList = new List<OEETypes.EnergyConsumption>();

            int index = 0;
            foreach (OEEMachine machine in machinGroup.Machines)
                energyConsumption[index] = machine.QueryEnergyConsumption(startTime, endTime, ref energyList);

            switch (this.TabEnenryStatus.SelectedIndex)
            {
                case 0:
                    ShowEnergyChart(energyConsumption);
                    break;
                case 1:
                    ShowEnergyList(energyList);
                    break;
            }     
        }

        private void ShowEnergyChart(int[] energyConsumption)
        {
            seriesEnergy.Points.Clear();
            foreach (var energy in energyConsumption)
                seriesEnergy.Points.AddXY(SERIES_ENERGY, energy);
        }

        private void ShowEnergyList(List<OEETypes.EnergyConsumption> energyList)
        {
            this.LvwEnergyView.BeginUpdate();
            foreach (var energy in energyList)
            {
                ListViewItem item = new ListViewItem();
                item.SubItems.Add(energy.dispatchCode);
                item.SubItems.Add(energy.powerConsumed.ToString());
            }
            this.LvwEnergyView.EndUpdate();
        }
    }
}
