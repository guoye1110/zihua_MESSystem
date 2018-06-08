using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CustomControl.MonthCalendarControl
{
    public partial class CustomMonthCalendar : Control, IOptionDataProvider
    {
        private const int BASE_RESOLUTION_WIDTH = 800;
        private const int BASE_RESOLUTION_HEIGHT = 600;

        private const int LENGEND_HEIGHT = 30;
        private const int LENGEND_WIDTH = 40;

        private const int CELL_WIDTH = 43;
        private const int CELL_HEIGHT = 28;

        private const int HEADER_HEIGHT = 20;

        private const int NAVIGATION_HEIGHT = NavigationBar.BAR_HEIGHT;
        private const int CUSTOM_ATTR_START = 2;
        private static string[] headers = { "日", "一", "二", "三", "四", "五", "六" };

        private Color tempColor = Color.LightSalmon;

        private CellOptionStyle _cellOptionStyle = CellOptionStyle.None;
        private string _selectionAttr;

        private struct CustomDimensions
        {
            public int height;
            public int width;
        };
        //  bool drawTransparent;

        public string SelectionAttribute
        {
            get { return _selectionAttr; }
            set
            {
                _selectionAttr = value;
                SetSelectionColor();
            }
        }

        private NavigationBar[] navigationBar;
        private CellOption cellOption;
        private CustomDimensions dimensions;
        private Font headerFont = new Font("Microsoft Sans Serif", 15, FontStyle.Regular);
        private int calendarNumber;

        private MonthGridData[] monthGridData;
        private DataGridView[] monthGridView;
        private Dictionary<DataGridView, MonthGridData> monthCalendarView
            = new Dictionary<DataGridView, MonthGridData>();

        private Lengend lengend;

        private int selectedCellRow = -1;
        private int selectedCellColumn = -1;

        private DataGridView selectedGridView = null;

        private DateTime startTime;
        private int startGridDataIndex;

        private IGridDataAttributeProvider attrProvider;

        private Dictionary<string, Color> attribute;
        private Dictionary<string, CustomAttribute> customAttribute;
        private Color selectionColor;

        private int cell_width, cell_height;
        private int grid_width, grid_height;
        private int lengend_height;

        public CellOptionStyle SelectedCellOptionStyle
        {
            set
            {
                _cellOptionStyle = value;
                UpdateCellOptionStyle();
            }
        }

        public int TimeInterval
        {
            set { cellOption.TimeInterval = value; }
        }

        public CustomMonthCalendar(int width,
            int height,
            Dictionary<string, CustomAttribute> customAttr,
            IGridDataAttributeProvider provider)
        {
            dimensions = new CustomDimensions();
            dimensions.width = width;
            dimensions.height = height;
            attrProvider = provider;
            customAttribute = customAttr;

            double screenWidth = Screen.PrimaryScreen.WorkingArea.Width;
            double screenHeight = Screen.PrimaryScreen.WorkingArea.Height;

            double ratioX = screenWidth / BASE_RESOLUTION_WIDTH;
            double rationY = screenHeight / BASE_RESOLUTION_HEIGHT;

            if (ratioX > 0)
                cell_width = (int)Math.Ceiling(CELL_WIDTH * ratioX);
            else
                cell_width = CELL_WIDTH;

            grid_width = cell_width * MonthCommon.GRID_COLUMN_COUNT;

            if (rationY > 0)
                cell_height = (int)Math.Ceiling(CELL_HEIGHT * rationY);
            else
                cell_height = CELL_HEIGHT;

            grid_height = cell_height * MonthCommon.GRID_ROW_COUNT;

            lengend_height = (int)Math.Ceiling(LENGEND_HEIGHT * rationY);

            attribute = new Dictionary<string, Color>();

            foreach (string key in customAttr.Keys)
                attribute.Add(key, customAttr[key].Color);

            attribute.Add("Holiday", Color.SlateGray);
            attribute.Add("Today", Color.Red);

            InitializeComponent();
            InitializeParameters();
            //this.BackColor = Color.LawnGreen;

        }

        private void SetSelectionColor()
        {
            foreach (string key in customAttribute.Keys)
            {
                if (key.CompareTo(_selectionAttr) == 0)
                    selectionColor = customAttribute[key].Color;
            }
        }

        private void InitializeParameters()
        {
            this.Size = new Size(dimensions.width * grid_width,
               dimensions.height * (grid_height + HEADER_HEIGHT +
               NAVIGATION_HEIGHT) + lengend_height);

            CreateLengendPanel();
            CreateMonthGridView();
            CreateNavigationPanel();
            SetupLayout();
            InitialMonthGridData();
/*
            if (lengend.Style == Lengend.LayoutStyle.horizontal)
                this.Size = new Size(dimensions.width * grid_width,
                dimensions.height * (grid_height + HEADER_HEIGHT +
                NAVIGATION_HEIGHT) + LENGEND_HEIGHT);
            else
                this.Size = new Size(dimensions.width * grid_width + LENGEND_WIDTH,
                dimensions.height * (grid_height + HEADER_HEIGHT +
                NAVIGATION_HEIGHT) + LENGEND_HEIGHT);
                */

            for (int i = 0; i < calendarNumber; i++)
                monthCalendarView.Add(monthGridView[i], monthGridData[i]);

            this.Location = new Point(0, 0);

        }

        private void CreateMonthGridView()
        {
            calendarNumber = dimensions.width * dimensions.height;

            monthGridView = new DataGridView[calendarNumber];
            monthGridData = new MonthGridData[calendarNumber];

            InitialMonthGridView();
        }

        private void CreateLengendPanel()
        {
            lengend = new Lengend();
            lengend.Style = Lengend.LayoutStyle.horizontal;

            foreach (CustomAttribute attr in customAttribute.Values)
                lengend.Labels.Add(attr.Color, attr.Text);

            if (lengend.Style == Lengend.LayoutStyle.horizontal)
                lengend.Size = new Size(this.Width, lengend_height);
            else
                lengend.Size = new Size(LENGEND_WIDTH, this.Height);
            this.Controls.Add(lengend);
        }

        private void CreateNavigationPanel()
        {
            navigationBar = new NavigationBar[dimensions.height];
            for (int i = 0; i < dimensions.height; i++)
            {
                if (i == 0)
                    navigationBar[i] = new NavigationBar(NavigationBar.BarStyle.ArrowMode, dimensions.width);
                else
                    navigationBar[i] = new NavigationBar(NavigationBar.BarStyle.None, dimensions.width);

                navigationBar[i].Size = new Size(this.Width, NAVIGATION_HEIGHT);

                navigationBar[i].OnLeftArrow += new NavigationBar.LeftArrow(NavigationBar_LeftArrowClick);
                navigationBar[i].OnRightArrow += new NavigationBar.RightArrow(NavigationBar_RightArrowClick);

                this.Controls.Add(navigationBar[i]);
            }
        }

        private void InitialMonthGridView()
        {
            for (int i = 0; i < calendarNumber; i++)
            {
                monthGridView[i] = new DataGridView();
                SetMonthGridViewStyle(i);

                monthGridView[i].CellPainting +=
                    new DataGridViewCellPaintingEventHandler(MonthGridView_CellPainting);

                //  monthGridView[i].CellMouseDoubleClick +=
                //      new DataGridViewCellMouseEventHandler(MonthGridView_CellMouseDoubleClick);

                for (int row = 0; row < MonthCommon.GRID_ROW_COUNT; row++)
                {

                    monthGridView[i].Rows.Add();

                    monthGridView[i].Rows[row].ReadOnly = true;
                    monthGridView[i].Rows[row].Resizable = DataGridViewTriState.False;
                    monthGridView[i].ClearSelection();
                }
            }
        }



        private void SetMonthGridViewStyle(int index)
        {
            DataGridView gridView = monthGridView[index];
            DataGridViewCellStyle cellStyle = new DataGridViewCellStyle();

            cellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            gridView.ColumnCount = MonthCommon.GRID_COLUMN_COUNT;
            gridView.ColumnHeadersDefaultCellStyle.BackColor = Color.LightGray;
            gridView.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
            gridView.ColumnHeadersDefaultCellStyle.Font = headerFont;
            gridView.ColumnHeadersDefaultCellStyle = cellStyle;
            gridView.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Raised;

            gridView.ColumnHeadersHeight = HEADER_HEIGHT;
            gridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;

            gridView.Name = "MonthGridView" + index.ToString();

            gridView.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            gridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;

            gridView.BorderStyle = BorderStyle.None;
            gridView.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            gridView.CellBorderStyle = DataGridViewCellBorderStyle.None;

            gridView.RowHeadersVisible = false;
            gridView.RowTemplate.Height = cell_height;

            gridView.AllowUserToResizeColumns = false;
            gridView.AllowUserToResizeRows = false;
            gridView.AllowUserToAddRows = false;

            gridView.SelectionMode = DataGridViewSelectionMode.CellSelect;
            gridView.MultiSelect = false;

            gridView.DefaultCellStyle.BackColor = Color.Gainsboro;
            gridView.DefaultCellStyle.Font = new Font("Microsoft Sans Serif", 15, FontStyle.Bold);
            gridView.DefaultCellStyle.SelectionForeColor = Color.White;
            gridView.DefaultCellStyle.SelectionBackColor = Color.LightSkyBlue;

            gridView.Size = new Size(grid_width, grid_height + HEADER_HEIGHT);

            for (int i = 0; i < MonthCommon.GRID_COLUMN_COUNT; i++)
            {
                Size size = TextRenderer.MeasureText(headers[i], headerFont);
                gridView.Columns[i].Name = headers[i];
                gridView.Columns[i].Width = cell_width;
                gridView.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                gridView.Columns[i].Resizable = DataGridViewTriState.False;
                gridView.Columns[i].Frozen = true;
                gridView.Columns[i].ReadOnly = true;
                gridView.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
                gridView.Columns[i].DefaultCellStyle = cellStyle;
            }

            this.Controls.Add(gridView);
        }

        private void InitialMonthGridData()
        {
            CultureInfo cultureInfo = CultureInfo.CurrentCulture;
            DateTime now = System.DateTime.Now;
            int startYear = cultureInfo.Calendar.GetYear(now);
            int startMonth = cultureInfo.Calendar.GetMonth(now);
            List<string> attrList = new List<string>();

            foreach (string key in attribute.Keys)
                attrList.Add(key);

            for (int i = 0; i < calendarNumber; i++)
            {
                monthGridData[i] = new MonthGridData(i, attrList);
                monthGridData[i].OnPropertyChanged +=
                    new MonthGridData.UpdateMonthInfo(MonthGridData_UpdateMonthInfo);
            }

            InitialGridData(startYear, startMonth);
        }

        private void InitialGridData(int startYear, int startMonth)
        {
            int year = startYear;
            int month = startMonth;
            int offset = 0;

            for (int i = 0; i < calendarNumber; i++)
            {
                if (startMonth + (i - offset) > 12)
                {
                    year++;
                    month = 1;
                    offset = i;
                    startMonth = month;
                    startYear = year;
                }
                monthGridData[i].Year = year;
                monthGridData[i].Month = month;
                GetCustomAttributeData(i);
                month++;
            }
        }

        private void ShowGridData()
        {
            foreach (var item in monthCalendarView)
            {
                DataTable dt = item.Value.Data;
                DataGridView dgv = item.Key;

                for (int row = 0; row < dt.Rows.Count; row++)
                {
                    string[] values = new string[dt.Columns.Count];
                    for (int col = 0; col < dt.Columns.Count; col++)
                        values[col] = dt.Rows[row][col].ToString();

                    dgv.Rows.Add(values);

                    dgv.Rows[row].ReadOnly = true;
                    dgv.Rows[row].Resizable = DataGridViewTriState.False;
                    dgv.ClearSelection();
                }

            }
        }

        private void UpdateCellOptionStyle()
        {
            for (int i = 0; i < calendarNumber; i++)
            {
                if (_cellOptionStyle == CellOptionStyle.TimeInterval)
                {
                    monthGridView[i].CellMouseClick +=
                  new DataGridViewCellMouseEventHandler(MonthGridView_CellMouseClick);
                }
            }
        }

        private void UpdateMonthCalendarData(int index)
        {
            DataGridView dgv = monthGridView[index];
            DataTable dt = monthGridData[index].Data;

            for (int row = 0; row < dt.Rows.Count; row++)
            {
                for (int col = 0; col < dt.Columns.Count; col++)
                {
                    string value = dt.Rows[row][col].ToString();
                    if (value == "")
                        dgv.Rows[row].Cells[col].Value = null;
                    else
                        dgv.Rows[row].Cells[col].Value = value;
                }

            }

            int year = monthGridData[index].Year;
            int month = monthGridData[index].Month;
            int navIndex = index / dimensions.width;

            navigationBar[navIndex].UpdateLabelText(year, month, index % dimensions.width);

        }

        private void SetupLayout()
        {
            int locX = this.Location.X;
            int locY = this.Location.Y;
            int x = locX, y = locY;

            lengend.Location = new Point(locX, locY);
            locY += lengend_height;
            
            for (int row = 0; row < dimensions.height; row++)
            {
                y = locY + row * (grid_height + HEADER_HEIGHT + NAVIGATION_HEIGHT);
                navigationBar[row].Location = new Point(x, y);

                y += NAVIGATION_HEIGHT;

                for (int col = 0; col < dimensions.width; col++)
                {
                    DataGridView gridView = monthGridView[row * dimensions.width + col];
                    x += col * grid_width;
                    gridView.Location = new Point(x, y);
                    // System.Console.WriteLine("{0}, {1}, {2}, {3}, {4}", row, x, y, gridView.Width, gridView.Height);
                }
                x = this.Location.X;
            }
        }

        public void ForceToReloadCustomData()
        {
            for (int i = 0; i < calendarNumber; i++)
                GetCustomAttributeData(i);
        }

        private void CellOption_ItemSelected(object sender, string item, TimeIntervalType type)
        {
            MonthGridData gridData = monthCalendarView[selectedGridView];
            DataGridViewCell selectedCell = selectedGridView.Rows[selectedCellRow].Cells[selectedCellColumn];
            DataTable dtAttr = gridData.DataAttribute;
            int day = Convert.ToInt32(selectedCell.Value);
            cellOption.Close();
            if (item.Length > 0)
            {
                int hour = Convert.ToInt32(item.Split(':')[0]);
                int minutes = Convert.ToInt32(item.Split(':')[1]);
                DateTime dateTime = new DateTime(gridData.Year, gridData.Month,
                    day, hour, minutes, 0);

                int id = Convert.ToInt32(dtAttr.Rows[day]["id"]);

                if (id == -1)
                {
                    if (type == TimeIntervalType.Start)
                    {
                        bool checkPassed = attrProvider.SetSelectedPeroid(_selectionAttr, id, dateTime.ToString(), null);
                        if (checkPassed)
                        {
                            dtAttr.Rows[day][_selectionAttr] = true;
                            //attribute[_selectionAttr] = tempColor;

                            GetCustomAttributeData(gridData.Index);
                            selectedGridView.InvalidateCell(selectedCell);
                            startTime = dateTime;
                            startGridDataIndex = gridData.Index;
                        }

                    }

                    if (type == TimeIntervalType.End)
                    {
                        attribute[_selectionAttr] = selectionColor;
                        attrProvider.SetSelectedPeroid(_selectionAttr, id, null, dateTime.ToString());
                        for (int index = 0; index < calendarNumber; index++)
                            GetCustomAttributeData(index);
                    }
                }
                else
                {
                    switch (type)
                    {
                        case TimeIntervalType.Start:
                            attrProvider.SetSelectedPeroid(_selectionAttr, id, dateTime.ToString(), null);
                            break;
                        case TimeIntervalType.End:
                            attrProvider.SetSelectedPeroid(_selectionAttr, id, null, dateTime.ToString());
                            break;
                        case TimeIntervalType.Delete:
                            attrProvider.SetSelectedPeroid(_selectionAttr, id, null, null);
                            break;
                    }

                    for (int index = 0; index < calendarNumber; index++)
                        GetCustomAttributeData(index);
                }
            }

            selectedGridView.ClearSelection();
        }



        private void MonthGridData_UpdateMonthInfo(object sender, int index)
        {
            UpdateMonthCalendarData(index);
        }

        private void NavigationBar_LeftArrowClick()
        {
            for (int i = 0; i < calendarNumber; i++)
            {
                monthGridData[i].Month--;
                GetCustomAttributeData(i);
            }

        }

        private void NavigationBar_RightArrowClick()
        {
            for (int i = 0; i < calendarNumber; i++)
            {
                monthGridData[i].Month++;
                GetCustomAttributeData(i);
            }

        }

        private void GetCustomAttributeData(int index)
        {
            int year = monthGridData[index].Year;
            int month = monthGridData[index].Month;
            foreach (string attr in customAttribute.Keys)
            {
                monthGridData[index].ClearExternalDateAttribute(attr);
                monthGridData[index].CustomData =
                    attrProvider.GetProviderDate(attr, year, month);
            }
            monthGridView[index].Invalidate();
        }

        private void MonthGridView_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            DataGridView dgv = (DataGridView)sender;
            DataGridViewCell cell = dgv.Rows[e.RowIndex].Cells[e.ColumnIndex];

            if (!e.Button.HasFlag(MouseButtons.Right))
            {
                System.Console.WriteLine("cellmousedown no selected");
                cell.Selected = false;
            }


        }

        private void MonthGridView_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            DataGridView dgv = (DataGridView)sender;
            DataGridViewCell cell = dgv.Rows[e.RowIndex].Cells[e.ColumnIndex];
            if (e.Button.HasFlag(MouseButtons.Right))
            {
                if (cell.Value != null)
                {
                    selectedGridView = dgv;
                    selectedCellRow = e.RowIndex;
                    selectedCellColumn = e.ColumnIndex;
                    cell.Selected = true;

                    cellOption = new CellOption(_cellOptionStyle, MousePosition, this);
                    cellOption.ItemSelected += new CellOption.CellOptionHandler(CellOption_ItemSelected);
                    cellOption.ShowDialog();

                }
            }
            else
                cell.Selected = false;
        }

        private void MonthGridView_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            DataGridView dgv = (DataGridView)sender;
            MonthGridData mgd = monthCalendarView[dgv];
            DataTable dt = mgd.Data;
            DataTable dtAttr = mgd.DataAttribute;
            object value = dt.Rows[e.RowIndex][e.ColumnIndex];

            if (value.ToString() != null)
            {
                int attrIndex = Convert.ToInt32(value);
                int id = Convert.ToInt32(dtAttr.Rows[attrIndex]["id"]);
                attrProvider.SetSelectedPeroid(_selectionAttr, id, null, null);
                GetCustomAttributeData(mgd.Index);
            }

        }

        private void MonthGridView_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            DataGridView dgv = (DataGridView)sender;
            Color valueColor = dgv.DefaultCellStyle.ForeColor;
            const int padding = 5;
            int height = e.CellBounds.Height - 2 * padding;
            int width = e.CellBounds.Width - 2 * padding;
            int x = e.CellBounds.Left;
            int y = e.CellBounds.Top;

            int highLightX = x;
            int highLightY = y;

            if (e.RowIndex >= 0)
            {
                // System.Console.WriteLine("Painting {0}, {1}", e.RowIndex, e.ColumnIndex);
                if (height > width)
                {
                    height = width;
                    highLightY += (e.CellBounds.Height - height) / 2;
                }
                else
                {
                    width = height;
                    highLightX += (e.CellBounds.Width - width) / 2;
                }

                Rectangle highLightRect = new Rectangle(highLightX, highLightY, width, height);

                using (SolidBrush backColorBrush = new SolidBrush(dgv.DefaultCellStyle.BackColor))
                {
                    e.Graphics.FillRectangle(backColorBrush, e.CellBounds);
                }

                MonthGridData mgd = monthCalendarView[dgv];
                DataTable dt = mgd.Data;
                DataTable dtAttr = mgd.DataAttribute;
                object value = dt.Rows[e.RowIndex][e.ColumnIndex];

                if (value.ToString() == "")
                {
                    e.Handled = true;
                    return;
                }

                if (e.State.HasFlag(DataGridViewElementStates.Selected))
                {
                    using (SolidBrush highLightBrush =
                            new SolidBrush(Color.FromArgb(100, dgv.DefaultCellStyle.SelectionBackColor)))
                    {
                        valueColor = dgv.DefaultCellStyle.SelectionForeColor;
                        e.Graphics.FillEllipse(highLightBrush, highLightRect);
                    }
                }
                else
                {

                    // System.Console.WriteLine("value: {0}", value.ToString());
                    int attrIndex = Convert.ToInt32(value);
                    for (int i = 0; i < attribute.Count; i++)
                    {
                        Color color;
                        string attrKey = dtAttr.Columns[i].ColumnName;
                        if ((attrKey == SelectionAttribute)
                            && (Convert.ToString(dtAttr.Rows[attrIndex]["StartTime"]) != "")
                            && (Convert.ToString(dtAttr.Rows[attrIndex]["EndTime"]) == ""))
                            color = tempColor;
                        else
                            color = attribute[attrKey];
                        //  System.Console.WriteLine("{0}, {1}", attrKey, dtAttr.Rows[attrIndex][attrKey]);
                        bool attrFlag = Convert.ToBoolean(dtAttr.Rows[attrIndex][attrKey]);
                        if (attrFlag)
                        {
                            switch (attrKey)
                            {
                                case "Today":
                                    using (Pen attrPen = new Pen(color, 2))
                                    {
                                        valueColor = color;
                                        e.Graphics.DrawEllipse(attrPen, highLightRect);
                                    }
                                    break;
                                case "Holiday":
                                    valueColor = color;
                                    break;
                                default:
                                    using (SolidBrush customBrush =
                                        new SolidBrush(Color.FromArgb(125, color)))
                                    {
                                        e.Graphics.FillEllipse(customBrush, highLightRect);
                                    }

                                    break;
                            }
                        }
                    }
                }

                if (e.Value != null)
                {
                    using (SolidBrush valueBrush = new SolidBrush(valueColor))
                    {
                        Font valueFont = dgv.DefaultCellStyle.Font;
                        string cellValue = e.Value.ToString();
                        SizeF valueSize = e.Graphics.MeasureString(cellValue, valueFont);
                        int valueX = highLightX;
                        int valueY = highLightY;
                        valueX += (width - (int)(valueSize.Width)) / 2;
                        valueY += (height - (int)(valueSize.Height)) / 2;
                        Rectangle valueRect = new Rectangle(valueX, valueY,
                            (int)Math.Ceiling(valueSize.Width), (int)Math.Ceiling(valueSize.Height));
                        e.Graphics.DrawString(cellValue, valueFont, valueBrush, valueRect);
                    }
                }

                e.Handled = true;
            }

        }

        public Dictionary<string, string> GetOptionData(CellOptionStyle style)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            switch (style)
            {
                case CellOptionStyle.TimeInterval:

                    MonthGridData gridData = monthCalendarView[selectedGridView];
                    DataGridViewCell selectedCell = selectedGridView.Rows[selectedCellRow].Cells[selectedCellColumn];
                    int day = Convert.ToInt32(selectedCell.Value.ToString());
                    DataTable dtAttr = gridData.DataAttribute;
                    dic.Add("StartTime", dtAttr.Rows[day]["StartTime"].ToString());
                    dic.Add("EndTime", dtAttr.Rows[day]["EndTime"].ToString());
                    break;
            }
            return dic;
        }
    }

}