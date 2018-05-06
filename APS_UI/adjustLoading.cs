using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms.DataVisualization.Charting;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MESSystem.common;

namespace MESSystem.APS_UI
{
    public partial class adjustLoading : Form
    {
        float screenRatioX, screenRatioY;

        int[] rowDataArray1 = { 80, 65, 73, 88, 72, 4, 5 }; //new int[7];
        int[] rowDataArray2 = { 82, 50, 92, 57, 18, 70, 63, 88, 82, 67 }; //new int[10];
        string[] columnArray1 = new string[7];
        string[] columnArray2 = new string[10];
        Color backgroundColor;
        Color foregroundColor;
        Chart[] columnChartArray = new Chart[2];

        System.Windows.Forms.Timer aTimer;
        static SolidBrush colorYellowBrush = new SolidBrush(Color.Yellow);
        
        public adjustLoading()
        {
            InitializeComponent();
            initialization();
            resizeForScreen();
            initialization2();
        }

        void initialization()
        {
            int i;

            for (i = 0; i < 7; i++)
            {
                columnArray1[i] = gVariable.machineNameArrayAPS[i];
            }

            for (i = 0; i < 10; i++)
            {
                columnArray2[i] = gVariable.machineNameArrayAPS[i + 7];
            }

            this.listView1.View = System.Windows.Forms.View.Details;
            this.Icon = new Icon(gVariable.logoInTitleArray[gVariable.CompanyIndex]);
        }

        private void initialization2()
        {
            foregroundColor = Color.Black;
            backgroundColor = Color.Transparent;

            columnChartArray[0] = chart1;
            columnChartArray[1] = chart2;

            initColumnChart(chart1, 0);
            initColumnChart(chart2, 1);
        }
        
        void resizeForScreen()
        {
            int i;
            int x, y, w, h;
            int x1, x2, x3, x4, x5, x6;
            int y1, y2;
            int gapX, gapX2, gapY;
            int textBoxW1, textBoxW2, textBoxH;
            float fontSize;
            GroupBox[] groupBoxArray = { groupBox1, groupBox2, groupBox3, groupBox4 };
            Label[] labelArray = { label8, label9 };
            //TextBox[] textBoxArray = { textBox10, textBox7, textBox14, textBox16, textBox17, textBox33 };
            Chart[] chartArray = { chart1, chart2 };
            Button[] buttonArray = { button1, button6 };
            ListView[] listviewArray = { listView1 };
            DateTimePicker[] timePickerArray = { dateTimePicker3, dateTimePicker4 };

            float[,] commonFontSize = { 
                                        { 7F,  8F,  9F,  10F, 11F,  12F}, 
                                        { 6F,  7F,  8F, 8.5F, 9F,  10F},  
                                        { 5.5F, 6F, 6.5F, 7F, 7.5F, 8F},  
                                     };

            int[] titleFontSize = { 20, 22, 23, 24, 25, 28 };

            x1 = 8;
            x2 = 179;
            x3 = 349;
            x4 = 8;
            x5 = 151;
            x6 = 295;

            y1 = 40;
            gapX = 60;
            gapX2 = 68;
            gapY = 41;
            y2 = 38;

            textBoxW1 = 92;
            textBoxW2 = 73;
            textBoxH = 20;

            fontSize = commonFontSize[gVariable.dpiValue, gVariable.resolutionLevel];

            screenRatioX = gVariable.screenRatioX;
            screenRatioY = gVariable.screenRatioY;

            x = (int)(label1.Location.X * screenRatioX);
            y = (int)(label1.Location.Y * screenRatioY);
            label1.Font = new System.Drawing.Font("Microsoft Sans Serif", titleFontSize[gVariable.resolutionLevel], System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            label1.Location = new System.Drawing.Point(x, y);

            for (i = 0; i < groupBoxArray.Length; i++)
            {
                groupBoxArray[i].Font = new System.Drawing.Font("Microsoft Sans Serif", fontSize, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
                w = (int)(groupBoxArray[i].Size.Width * screenRatioX);
                h = (int)(groupBoxArray[i].Size.Height * screenRatioY);
                groupBoxArray[i].Size = new System.Drawing.Size(w, h);
                x = (int)(groupBoxArray[i].Location.X * screenRatioX);
                y = (int)(groupBoxArray[i].Location.Y * screenRatioY);
                groupBoxArray[i].Location = new System.Drawing.Point(x, y);
            }

            for (i = 0; i < labelArray.Length; i++)
            {
                //labelArray1[i].Font = new System.Drawing.Font("Microsoft Sans Serif", fontSize, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
                x = (int)(labelArray[i].Location.X * screenRatioX);
                y = (int)((labelArray[i].Location.Y) * screenRatioY);
                labelArray[i].Location = new System.Drawing.Point(x, y);
            }

            for (i = 0; i < chartArray.Length; i++)
            {
                chartArray[i].Font = new System.Drawing.Font("Microsoft Sans Serif", fontSize, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
                w = (int)(chartArray[i].Size.Width * screenRatioX);
                h = (int)(chartArray[i].Size.Height * screenRatioY);
                chartArray[i].Size = new System.Drawing.Size(w, h);
                x = (int)(chartArray[i].Location.X * screenRatioX);
                y = (int)(chartArray[i].Location.Y * screenRatioY);
                chartArray[i].Location = new System.Drawing.Point(x, y);
            }

            for (i = 0; i < buttonArray.Length; i++)
            {
                buttonArray[i].Font = new System.Drawing.Font("Microsoft Sans Serif", fontSize, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
                w = (int)(buttonArray[i].Size.Width * screenRatioX);
                h = (int)(buttonArray[i].Size.Height * screenRatioY);
                buttonArray[i].Size = new System.Drawing.Size(w, h);
                x = (int)(buttonArray[i].Location.X * screenRatioX);
                y = (int)(buttonArray[i].Location.Y * screenRatioY);
                buttonArray[i].Location = new System.Drawing.Point(x, y);
            }

            for (i = 0; i < timePickerArray.Length; i++)
            {
                w = (int)(timePickerArray[i].Size.Width * screenRatioX);
                h = (int)(timePickerArray[i].Size.Height * screenRatioY);
                timePickerArray[i].Size = new System.Drawing.Size(w, h);
                x = (int)(timePickerArray[i].Location.X * screenRatioX);
                y = (int)(timePickerArray[i].Location.Y * screenRatioY);
                timePickerArray[i].Location = new System.Drawing.Point(x, y);
            }

            /*
            for (i = 0; i < listviewArray.Length; i++)
            {
                w = (int)(listviewArray[i].Size.Width * screenRatioX);
                h = (int)(listviewArray[i].Size.Height * screenRatioY);
                listviewArray[i].Size = new System.Drawing.Size(w, h);
                x = (int)(listviewArray[i].Location.X * screenRatioX);
                y = (int)(listviewArray[i].Location.Y * screenRatioY);
                listviewArray[i].Location = new System.Drawing.Point(x, y);
            }

            listView1.Font = new System.Drawing.Font("Microsoft Sans Serif", fontSize, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
             * */
        }


        private void timer_listview(Object source, EventArgs e)
        {
            updateScreen();
        }

        void updateScreen()
        {
            int i, j, k;
            int length;
            string commandText;
            string tableName;
            string databaseName;
            int[] dispatchIndex = new int[100];
            int[] outputQuantity = new int[100];
            string[] batchNumArray = new string[100];
            string[] batchStartArray = new string[100];
            string[] batchEndArray = new string[100];
            //string[] batchNumArray = new string[100];
            string[,] tableArray;

            int[] productBatchLenArray = 
            { 
                1, 42, 90, 90, 100, 90, 80, 100, 110, 110, 70 
            };
            string[] productBatchListHeader = 
            {
                "", "序号", "产品批号", "订单号", "产品编码", "交货日期", "需求数量", "客户名", "计划开工", "计划完工", "订单状态"
            };

            try
            {
                aTimer.Stop();

                listView1.Clear();
                listView1.BeginUpdate();

                commandText = "select * from `" + gVariable.globalDispatchTableName + "` where machineID = " + "'" + 4 + "' and status = -2";
                //find generated dispatches
                tableArray = mySQLClass.databaseCommonReading(gVariable.globalDatabaseName, commandText);
                if (tableArray == null)
                {
                    aTimer.Start();
                    return;
                }

                k = 0;
                length = tableArray.GetLength(0);
                for (i = 0; i < length; i++)
                {
                    for (j = 0; j < k; j++)
                    {
                        if (tableArray[i, mySQLClass.BATCH_NUMBER_IN_DISPATCHLIST_DATABASE] == batchNumArray[j])
                        {
                            batchEndArray[j] = tableArray[i, mySQLClass.PLANNED_COMPLETETIME_IN_DISPATCHLIST_DATABASE];
                            outputQuantity[j] += Convert.ToInt32(tableArray[i, mySQLClass.FORCAST_NUM_IN_DISPATCHLIST_DATABASE]);
                            break;
                        }
                    }

                    if (j >= k)
                    {
                        dispatchIndex[k] = i;
                        batchNumArray[k] = tableArray[i, mySQLClass.BATCH_NUMBER_IN_DISPATCHLIST_DATABASE];
                        batchStartArray[k] = tableArray[i, mySQLClass.PLANNED_STARTTIME_IN_DISPATCHLIST_DATABASE];
                        batchEndArray[k] = tableArray[i, mySQLClass.PLANNED_COMPLETETIME_IN_DISPATCHLIST_DATABASE];
                        outputQuantity[k] = Convert.ToInt32(tableArray[i, mySQLClass.FORCAST_NUM_IN_DISPATCHLIST_DATABASE]);
                        k++;  //means how many batchNum we have by now
                    }
                }

                listView1.GridLines = true;
                listView1.Dock = DockStyle.Fill;

                for (i = 0; i < productBatchLenArray.Length; i++)
                    listView1.Columns.Add(productBatchListHeader[i], (int)(productBatchLenArray[i] * screenRatioX), HorizontalAlignment.Center);

                for (i = 0; i < k; i++)
                {
                    ListViewItem OptionItem = new ListViewItem();

                    j = dispatchIndex[i];
                    OptionItem.SubItems.Add((i + 1).ToString());
                    OptionItem.SubItems.Add(tableArray[j, mySQLClass.BATCH_NUMBER_IN_DISPATCHLIST_DATABASE]);
                    OptionItem.SubItems.Add(tableArray[j, mySQLClass.SALESORDER_CODE_IN_DISPATCHLIST_DATABASE]);
                    OptionItem.SubItems.Add(tableArray[j, mySQLClass.PRODUCT_CODE_IN_DISPATCHLIST_DATABASE]);
                    OptionItem.SubItems.Add("2019-06-13"); //tableArray[j, mySQLClass.]);
                    OptionItem.SubItems.Add(outputQuantity[i].ToString());
                    OptionItem.SubItems.Add(tableArray[j, mySQLClass.CUSTOMER_IN_DISPATCHLIST_DATABASE]);
                    OptionItem.SubItems.Add(batchStartArray[i]);
                    OptionItem.SubItems.Add(batchEndArray[i]);

                    switch (Convert.ToInt16(tableArray[j, mySQLClass.STATUS_IN_SALESORDER_DATABASE]))
                    {
                        case gVariable.MACHINE_STATUS_DISPATCH_GENERATED:
                            OptionItem.SubItems.Add("预排程");
                            break;
                        case gVariable.MACHINE_STATUS_DISPATCH_CONFIRMED:
                            OptionItem.SubItems.Add("已排程");
                            break;
                        case gVariable.MACHINE_STATUS_DISPATCH_UNPUBLISHED:
                            OptionItem.SubItems.Add("已排程");
                            break;
                        case gVariable.MACHINE_STATUS_DISPATCH_PUBLISHED:
                            OptionItem.SubItems.Add("已发布");
                            break;
                        case gVariable.MACHINE_STATUS_DISPATCH_APPLIED:
                            OptionItem.SubItems.Add("已申请");
                            break;
                        case gVariable.MACHINE_STATUS_DISPATCH_STARTED:
                            OptionItem.SubItems.Add("已开工");
                            break;
                        case gVariable.MACHINE_STATUS_DISPATCH_COMPLETED:
                            OptionItem.SubItems.Add("已完工");
                            break;
                        default:
                            break;
                    }
                    listView1.Items.Add(OptionItem);
                }

                listView1.EndUpdate();

                for(i = 0; i < columnChartArray.Length; i++)
                    columnChartForMachine(columnChartArray[i], i);

                aTimer.Start();

            }
            catch (Exception ex)
            {
                Console.Write("workshop reports timer_listview() failed :" + ex);
            }
        }


        //draw a column chart
        private void initColumnChart(Chart chart, int chartIndex)
        {
            string[] title = {"流延设备负载表", "印刷分切设备负载表"};

            try
            {
                //标题
                chart.Titles.Add(title[chartIndex]);
                chart.Titles[0].ForeColor = foregroundColor;
                chart.Titles[0].Font = new Font("微软雅黑", 12f, FontStyle.Bold);
                chart.Titles[0].Alignment = ContentAlignment.TopCenter;
                //chart.Titles[1].ForeColor = foregroundColor;
                //chart.Titles[1].Font = new Font("微软雅黑", 8f, FontStyle.Regular);
                //chart.Titles[1].Alignment = ContentAlignment.TopRight;

                //控件背景
                //chart.BackColor = Color.Transparent;
                chart.BackColor = backgroundColor;
                //图表区背景
                chart.ChartAreas[0].BackColor = Color.Transparent;
                chart.ChartAreas[0].BorderColor = Color.Transparent;
                //X轴标签间距
                chart.ChartAreas[0].AxisX.Interval = 1;
                chart.ChartAreas[0].AxisX.LabelStyle.IsStaggered = true;
                chart.ChartAreas[0].AxisX.LabelStyle.Angle = -45;
                chart.ChartAreas[0].AxisX.TitleFont = new Font("微软雅黑", 10f, FontStyle.Regular);
                chart.ChartAreas[0].AxisX.TitleForeColor = foregroundColor;

                //X坐标轴颜色
                chart.ChartAreas[0].AxisX.LineColor = ColorTranslator.FromHtml("#38587a"); ;
                chart.ChartAreas[0].AxisX.LabelStyle.ForeColor = foregroundColor;
                chart.ChartAreas[0].AxisX.LabelStyle.Font = new Font("微软雅黑", 10f, FontStyle.Regular);

                //X坐标轴标题
                //chart.ChartAreas[0].AxisX.Title = "数量(宗)";
                //chart.ChartAreas[0].AxisX.TitleFont = new Font("微软雅黑", 10f, FontStyle.Regular);
                //chart.ChartAreas[0].AxisX.TitleForeColor = foregroundColor;
                //chart.ChartAreas[0].AxisX.TextOrientation = TextOrientation.Horizontal;
                //chart.ChartAreas[0].AxisX.ToolTip = "数量(宗)";
                //X轴网络线条
                chart.ChartAreas[0].AxisX.MajorGrid.Enabled = true;
                chart.ChartAreas[0].AxisX.MajorGrid.LineColor = ColorTranslator.FromHtml("#2c4c6d");

                //Y坐标轴颜色
                chart.ChartAreas[0].AxisY.LineColor = ColorTranslator.FromHtml("#38587a");
                chart.ChartAreas[0].AxisY.LabelStyle.ForeColor = foregroundColor;
                chart.ChartAreas[0].AxisY.LabelStyle.Font = new Font("微软雅黑", 10f, FontStyle.Regular);
                //Y坐标轴标题
                chart.ChartAreas[0].AxisY.Title = "产能负荷百分比(%)";
                chart.ChartAreas[0].AxisY.TitleFont = new Font("微软雅黑", 10f, FontStyle.Regular);
                chart.ChartAreas[0].AxisY.TitleForeColor = foregroundColor;
                chart.ChartAreas[0].AxisY.TextOrientation = TextOrientation.Rotated270;
                chart.ChartAreas[0].AxisY.ToolTip = "产能负荷百分比(%)";
                //Y轴网格线条
                chart.ChartAreas[0].AxisY.MajorGrid.Enabled = true;
                chart.ChartAreas[0].AxisY.MajorGrid.LineColor = ColorTranslator.FromHtml("#2c4c6d");

                chart.ChartAreas[0].AxisY2.LineColor = Color.Transparent;

                chart.ChartAreas[0].Area3DStyle.Enable3D = true;
                chart.ChartAreas[0].BackGradientStyle = GradientStyle.TopBottom;
                Legend legend = new Legend("legend");
                legend.Title = "legendTitle";

                chart.Series[0].XValueType = ChartValueType.String;  //设置X轴上的值类型
                chart.Series[0].Label = "#VAL";                //设置显示X Y的值    
                chart.Series[0].LabelForeColor = Color.Lime; // foregroundColor;
                chart.Series[0].ToolTip = "#VALX:#VAL";     //鼠标移动到对应点显示数值
                chart.Series[0].ChartType = SeriesChartType.Column;    //图类型(折线)

                chart.Series[0].XValueType = ChartValueType.String;
                chart.Series[0].YValueType = ChartValueType.Int32;

                chart.Series[0].Color = Color.Lime;
                chart.Series[0].LegendText = legend.Name;
                chart.Series[0].IsValueShownAsLabel = true;
                chart.Series[0].LabelForeColor = Color.Blue; // foregroundColor;
                chart.Series[0].CustomProperties = "DrawingStyle = Cylinder";
                chart.Legends.Add(legend);
                chart.Legends[0].Position.Auto = false;
            }
            catch (Exception e)
            {
                Console.Write("init column charts failed :" + e);
            }
        }


        private void columnChartForMachine(Chart chart, int chartIndex)
        {
            try
            {
                //绑定数据
                if(chartIndex == 0)
                    chart.Series[0].Points.DataBindXY(columnArray1, rowDataArray1);
                else
                    chart.Series[0].Points.DataBindXY(columnArray2, rowDataArray2);

                //chart.Series[0].Points[0].Color = foregroundColor1;
            }
            catch (Exception e)
            {
                Console.Write("draw column charts failed :" + e);
            }
        }


        private void adjustLoading_Load(object sender, EventArgs e)
        {
            int ret;

            try
            {
                ret = mySQLClass.databaseExistsOrNot(gVariable.DBHeadString + "001");
                if (gVariable.buildNewDatabase == 1 || ret == 0)
                {
                    mySQLClass.buildBasicDatabase();
                }

                this.Enabled = true;

                this.Text = gVariable.programTitle + "设备负荷展示与调整";

                aTimer = new System.Windows.Forms.Timer();

                //refresh screen every 100 ms
                aTimer.Interval = 1000;
                aTimer.Enabled = true;

                aTimer.Tick += new EventHandler(timer_listview);

                updateScreen();
            }
            catch (Exception ex)
            {
                Console.WriteLine("adjustLoading_Load() failed!" + ex);
            }
        }

        private void adjustLoading_FormClosing(object sender, EventArgs e)
        {
            aTimer.Stop();
        }


        //period of time for the machine work loading display
        private void button6_Click(object sender, EventArgs e)
        {

        }
    }
}
