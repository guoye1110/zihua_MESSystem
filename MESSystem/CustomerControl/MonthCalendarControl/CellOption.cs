using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CustomControl.MonthCalendarControl
{
    public partial class CellOption : Form
    {

        public delegate void CellOptionHandler(object sender, string selectedItem, TimeIntervalType intervalType);
        public CellOptionHandler ItemSelected;

        private const string TIME_INTERVAL_GROUPPANEL = "TimeIntervalPanel";
        private const string TIME_INTERVAL_LIST_NAME = "TimeIntervalList";
        private const string TIME_INTERVAL_RADIO_START_NAME = "TimeIntervalRadioStart";
        private const string TIME_INTERVAL_RADIO_END_NAME = "TimeIntervalRadioEnd";
        private const string TIME_INTERVAL_RADIO_DELETE_NAME = "TimeIntervalRadioDelete";
        private const string TIME_INTERVAL_CONFIRM_BUTTON_NAME = "TimeIntervalConfirmButton";
        private const string TIME_INTERVAL_CANCEL_BUTTON_NAME = "TimeIntervalCancelButton";


     
        private const int TIME_INTERVAL_GROUPPANEL_HEIGHT = 60;
        private const int TIME_INTERVAL_RADIOBUTTON_HEIGHT = 25;
        private const int TIME_INTERVAL_LIST_HEIGHT = 150;
        private const int TIME_INTERVAL_BUTTON_HEIGHT = 30;

        private const int TIME_INTERVAL_FORM_WIDTH = 100;
        private const int TIME_INTERVAL_FORM_HEIGHT = TIME_INTERVAL_GROUPPANEL_HEIGHT + TIME_INTERVAL_LIST_HEIGHT + TIME_INTERVAL_BUTTON_HEIGHT;

        private CellOptionStyle cellOptionStyle;
        private int _timeInterval;
        private Point loc;
        private string value;

        private IOptionDataProvider dataProvider;
        private Dictionary<string, string> optionData;

        public int TimeInterval
        {
            set { _timeInterval = value; }
        }

        public CellOption(CellOptionStyle style, Point location, IOptionDataProvider optDataProvider)
        {
            InitializeComponent();

            int screenHeight = Screen.PrimaryScreen.WorkingArea.Bottom;
            int screenWidth = Screen.PrimaryScreen.WorkingArea.Right;

            dataProvider = optDataProvider;
            if (location.Y + TIME_INTERVAL_FORM_HEIGHT > screenHeight)
                location.Y = screenHeight - TIME_INTERVAL_FORM_HEIGHT;
            if (location.X + TIME_INTERVAL_FORM_WIDTH > screenWidth)
                location.X = screenWidth - TIME_INTERVAL_FORM_WIDTH;
            loc = location;
            cellOptionStyle = style;

            SetFormStyle();
            SetOptionStyle();
            this.Activated += new System.EventHandler(this.CellOption_Activated);
            this.Load += new System.EventHandler(this.CellOption_Load);

           


        }

      
        private void CellOption_Load(object sender, EventArgs args)
        {
           
            this.Location = loc;
            switch (cellOptionStyle)
            {
                case CellOptionStyle.TimeInterval:
                    this.Size = new Size(TIME_INTERVAL_FORM_WIDTH,
                     TIME_INTERVAL_GROUPPANEL_HEIGHT + TIME_INTERVAL_LIST_HEIGHT + TIME_INTERVAL_BUTTON_HEIGHT);
                    break;
            }
        }

        private void CellOption_Activated(object sender, EventArgs args)
        {
            //System.Console.WriteLine("activated");
            switch (cellOptionStyle)
            {
                case CellOptionStyle.TimeInterval:
                        ListBox lb = (ListBox)this.Controls[TIME_INTERVAL_LIST_NAME];
                    break;
            }
        }

        private void SetFormStyle()
        {
            this.FormBorderStyle = FormBorderStyle.None;
            //this.TopMost = true;

        }

        private void SetOptionStyle()
        {
            switch (cellOptionStyle)
            {
                case CellOptionStyle.TimeInterval:
                    CreateTimeIntervalStyle();
                    break;
            }
        }

    


        private void CreateTimeIntervalStyle()
        {
            int i;
          
            Panel timeGroup = new Panel();
            RadioButton startRadio = new RadioButton();
            RadioButton endRadio = new RadioButton();
            RadioButton deleteRadio = new RadioButton();
            ListBox timeIntervalList = new ListBox();

            int locX = 2;
            int locY = 0;

            optionData = dataProvider.GetOptionData(CellOptionStyle.TimeInterval);

            timeGroup.Name = TIME_INTERVAL_GROUPPANEL;
            timeGroup.Text = "";
            timeGroup.Size = new Size(TIME_INTERVAL_FORM_WIDTH, TIME_INTERVAL_GROUPPANEL_HEIGHT);
            timeGroup.Location = new Point(locX, locY);

            timeGroup.Controls.Add(startRadio);
            timeGroup.Controls.Add(endRadio);
            timeGroup.Controls.Add(deleteRadio);
            this.Controls.Add(timeGroup);


            startRadio.Name = TIME_INTERVAL_RADIO_START_NAME;
            startRadio.Font = new Font("Microsoft Sans Serif", 8, FontStyle.Regular);
            startRadio.Text = "保养起始时间";
            startRadio.Location = new Point(locX, locY);
            startRadio.Size = new Size(TIME_INTERVAL_FORM_WIDTH, TIME_INTERVAL_GROUPPANEL_HEIGHT / 3);

            endRadio.Name = TIME_INTERVAL_RADIO_END_NAME;
            endRadio.Font = new Font("Microsoft Sans Serif", 8, FontStyle.Regular);
            endRadio.Text = "保养终止时间";
            endRadio.Location = new Point(locX, locY  + TIME_INTERVAL_GROUPPANEL_HEIGHT / 3);
            endRadio.Size = new Size(TIME_INTERVAL_FORM_WIDTH, TIME_INTERVAL_GROUPPANEL_HEIGHT / 3);

            deleteRadio.Name = TIME_INTERVAL_RADIO_DELETE_NAME;
            deleteRadio.Font = new Font("Microsoft Sans Serif", 8, FontStyle.Regular);
            deleteRadio.Text = "删除保养周期";
            deleteRadio.Location = new Point(locX, locY + TIME_INTERVAL_GROUPPANEL_HEIGHT * 2 / 3);
            deleteRadio.Size = new Size(TIME_INTERVAL_FORM_WIDTH, TIME_INTERVAL_GROUPPANEL_HEIGHT / 3);


            locX = 0;
            locY += TIME_INTERVAL_GROUPPANEL_HEIGHT;

            timeIntervalList.Name = TIME_INTERVAL_LIST_NAME;
            timeIntervalList.TabIndex = 0;
            timeIntervalList.SelectionMode = SelectionMode.One;
            timeIntervalList.Size = new Size(TIME_INTERVAL_FORM_WIDTH, TIME_INTERVAL_LIST_HEIGHT);
            timeIntervalList.Location = new Point(0, locY);

            for (i = 0; i <24; i++)
            {
                timeIntervalList.Items.Add(i.ToString() + ":00");
                timeIntervalList.Items.Add(i.ToString() + ":30");
            }
            this.Controls.Add(timeIntervalList);
            timeIntervalList.Click += new EventHandler(TimeIntervalList_Click);

            locY += TIME_INTERVAL_LIST_HEIGHT;

            Button timeIntervalConfirmButton = new Button();
            timeIntervalConfirmButton.Font = new Font("Microsoft Sans Serif", 8, FontStyle.Regular);
            timeIntervalConfirmButton.Name = TIME_INTERVAL_CONFIRM_BUTTON_NAME;
            timeIntervalConfirmButton.Location = new Point(locX, locY);
            timeIntervalConfirmButton.Size = new Size(TIME_INTERVAL_FORM_WIDTH / 2, TIME_INTERVAL_BUTTON_HEIGHT);
            timeIntervalConfirmButton.Text = "确认";
            this.Controls.Add(timeIntervalConfirmButton);

            locX += TIME_INTERVAL_FORM_WIDTH / 2;

          
            Button timeIntervalCancelButton = new Button();
            timeIntervalCancelButton.Font = new Font("Microsoft Sans Serif", 8, FontStyle.Regular);
            timeIntervalCancelButton.Name = TIME_INTERVAL_CANCEL_BUTTON_NAME;
            timeIntervalCancelButton.Location = new Point(locX, locY);
            timeIntervalCancelButton.Size = new Size(TIME_INTERVAL_FORM_WIDTH / 2, TIME_INTERVAL_BUTTON_HEIGHT);
            timeIntervalCancelButton.Text = "取消";
            this.Controls.Add(timeIntervalCancelButton);

            startRadio.CheckedChanged += new EventHandler(TimeIntervalRadioButton_CheckedChanged);
            endRadio.CheckedChanged += new EventHandler(TimeIntervalRadioButton_CheckedChanged);
            deleteRadio.CheckedChanged += new EventHandler(TimeIntervalRadioButton_CheckedChanged);

            if ((optionData["StartTime"] == "") || (optionData["EndTime"] == ""))
                deleteRadio.Enabled = false;
            else
                deleteRadio.Enabled = true;
            
            startRadio.Checked = true;

            timeIntervalConfirmButton.Click += new EventHandler(TimeIntervalButton_Click);
            timeIntervalCancelButton.Click += new EventHandler(TimeIntervalButton_Click);
        }

        private void TimeIntervalList_Click(object sender, EventArgs e)
        {
            ListBox lv = (ListBox)sender;
            value = lv.SelectedItem.ToString();
            System.Console.WriteLine("list {0}", value);
        }

        private void TimeIntervalButton_Click(object sender, EventArgs e)
        {
            // System.Console.WriteLine("button {0}", value);
            ListBox lv = (ListBox)this.Controls[TIME_INTERVAL_LIST_NAME];
            Button btn = (Button)sender;
            RadioButton startRadio = (RadioButton)this.Controls[TIME_INTERVAL_GROUPPANEL].Controls[TIME_INTERVAL_RADIO_START_NAME];
            RadioButton endRadio = (RadioButton)this.Controls[TIME_INTERVAL_GROUPPANEL].Controls[TIME_INTERVAL_RADIO_END_NAME];
            RadioButton deleteRadio = (RadioButton)this.Controls[TIME_INTERVAL_GROUPPANEL].Controls[TIME_INTERVAL_RADIO_DELETE_NAME];

            bool selected = false;
            TimeIntervalType type = TimeIntervalType.Start;

            if (btn.Name == TIME_INTERVAL_CONFIRM_BUTTON_NAME)
                selected = true;
            else if (btn.Name == TIME_INTERVAL_CANCEL_BUTTON_NAME)
                selected = false;

           // System.Console.WriteLine("TimeIntervalButton_Click {0}, {1}", selected, lv.Text);
            if (startRadio.Checked)
                type = TimeIntervalType.Start;
            else if (endRadio.Checked)
                type = TimeIntervalType.End;
            else
                type = TimeIntervalType.Delete;

            if (selected && (lv.Text != null))
                ItemSelected(this, lv.Text, type);
            else
                ItemSelected(this, "", type);
   

            lv.ClearSelected();
        }

        private void TimeIntervalRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton radioBtn = (RadioButton)sender;
            ListBox lv = (ListBox)this.Controls[TIME_INTERVAL_LIST_NAME];
            if (radioBtn.Name == TIME_INTERVAL_RADIO_START_NAME)
                lv.Text = optionData["StartTime"];
            else if (radioBtn.Name == TIME_INTERVAL_RADIO_END_NAME)
                lv.Text = optionData["EndTime"];
            else
                lv.ClearSelected();
        }

        protected override void OnGotFocus(EventArgs e)
        {
           // System.Console.WriteLine("get focused");

        }

        protected override void OnLostFocus(EventArgs e)
        {
           // System.Console.WriteLine("lost focused");
           // base.OnLostFocus(e);
        }

    }

   
    public interface  IOptionDataProvider
    {
        Dictionary<string, string> GetOptionData(CellOptionStyle style );
    }
}
