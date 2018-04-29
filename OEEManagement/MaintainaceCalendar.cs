using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using MESSystem.mainUI;
using MESSystem.common;
using CustomControl.MonthCalendarControl;

namespace MESSystem.OEEManagement
{
    public partial class MaintainaceCalendar : Form, IGridDataAttributeProvider
    {
        private const string MAINTIANANCE_TITILE = "设备维护日历";
        private const string MACHINE_LIST = "设备列表:";
        CustomMonthCalendar[] calendar;
        Dictionary<int, string> machineList;
        int selectedMachine;
        string global_startTime = null;
        string global_endTime = null;
        int global_id = 0;

        public MaintainaceCalendar()
        {
            InitializeComponent();
            InitializeParameters();
            InitializeCalendar();
            InitializeMachineList();
           

        }

        private void InitializeParameters()
        {
            machineList = new Dictionary<int, string>();
            for (int index = 0; index < gVariable.machineNameArray.Length; index++)
                machineList.Add(index, gVariable.machineNameArray[index]);
            calendar = new CustomMonthCalendar[gVariable.machineNameArray.Length];
        }

        private void InitializeMachineList()
        {
            this.LblMaintain.Font = new Font(OEETypes.FONT, 22F, System.Drawing.FontStyle.Bold);
            this.LblMaintain.Text = MAINTIANANCE_TITILE;
            this.LblMachineList.Font = new Font(OEETypes.FONT, 10F, System.Drawing.FontStyle.Regular);
            this.LblMachineList.Text = MACHINE_LIST;

            this.LstMachine.Font = new Font(OEETypes.FONT, 15F, System.Drawing.FontStyle.Regular);

            Dictionary<int, string>.ValueCollection values = machineList.Values;
            foreach (string value in values)
                this.LstMachine.Items.Add(value);

            this.LstMachine.SelectedIndex = 0;

            selectedMachine = this.LstMachine.SelectedIndex;

            this.LstMachine.SelectedIndexChanged += new EventHandler(LstMachine_SelectedIndexChanged);
        }

        private void InitializeCalendar()
        {
            Dictionary<string, Color> attr = new Dictionary<string, Color>();
            attr.Add("Maintained", Color.RoyalBlue);
            for (int i = 0; i < calendar.Length; i++)
            {
                calendar[i] = new CustomMonthCalendar(2, 2, attr, this);
                calendar[i].SelectedCellOptionStyle = CellOptionStyle.TimeInterval;
                calendar[i].Location = new Point(20, 20);
            }
            this.PnlCalendar.Controls.Add(calendar[selectedMachine]);
        }

        private void MaintainaceCalendar_Load(object sender, EventArgs e)
        {

        }

        private void MaintainaceCalendar_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (false == IntegrityCheck(global_id, global_startTime, global_endTime))
                    e.Cancel = true;
                else
                    firstScreen.firstScreenClass.Show();
            }
            catch (Exception ex)
            {
                Console.Write("MaintainaceCalendar_FormClosing failed" + ex);
            }

        }

        private void LstMachine_SelectedIndexChanged(object sender, EventArgs e)
        {
            ListBox machineList = (ListBox)sender;
            if (machineList.SelectedIndex != selectedMachine)
            {
                if (false == IntegrityCheck(global_id, global_startTime, global_endTime))
                    machineList.SelectedIndex = selectedMachine;
                else
                {
                    this.PnlCalendar.Controls.Remove(calendar[selectedMachine]);
                    selectedMachine = machineList.SelectedIndex;
                    this.PnlCalendar.Controls.Add(calendar[selectedMachine]);
                }
            }
        }


        public List<CustomDataType> GetProviderDate(string attributeName, int year, int month)
        {
            int i;
            int machineID;
            int startTimeStamp;
            int endTimeStamp;
            string databaseName;
            string tableName;
            string commandText;
            DateTime start;
            DateTime end;
            string[,] tableArray;
            List<CustomDataType> repaired = null;
            List<CustomDataType> scheduled = null;
            List<CustomDataType> maintained = null;
            List<CustomDataType> customData = new List<CustomDataType>();

            machineID = selectedMachine + 1 ;
          //  machineID = 8;
            commandText = null;
            databaseName = gVariable.DBHeadString + machineID.ToString().PadLeft(3, '0');
            tableName = gVariable.machineWorkingPlanTableName;

            //start of the month 
            start = new DateTime(year, month, 1);
            startTimeStamp = toolClass.ConvertDateTimeInt(start);

            //get the end of month 2018-04-30 23:59:59
            end = start.AddMonths(1).AddSeconds(-1);
            endTimeStamp = toolClass.ConvertDateTimeInt(end);

            //get possible position for a plan(including repaired/scheduled/aintained)
            if (attributeName == "Repaired")
            {
                repaired = new List<CustomDataType>();
                commandText = "select * from `" + tableName + "` where ((timeStamp1 < " + startTimeStamp + " and timeStamp2 > " + startTimeStamp + ") or (timeStamp1 >= " +
                               startTimeStamp + " and timeStamp1 < " + endTimeStamp + ")) and typeID = 0";
            }
            else if (attributeName == "Scheduled")
            {
                scheduled = new List<CustomDataType>();
                commandText = "select * from `" + tableName + "` where ((timeStamp1 < " + startTimeStamp + " and timeStamp2 > " + startTimeStamp + ") or (timeStamp1 >= " +
                               startTimeStamp + " and timeStamp1 < " + endTimeStamp + ")) and typeID = 1";
            }
            else if (attributeName == "Maintained")
            {
                maintained = new List<CustomDataType>();
                commandText = "select * from `" + tableName + "` where ((timeStamp1 < " + startTimeStamp + " and timeStamp2 > " + startTimeStamp + ") or (timeStamp1 >= " +
                               startTimeStamp + " and timeStamp1 < " + endTimeStamp + ")) and typeID > 1";
            }
            tableArray = mySQLClass.databaseCommonReading(databaseName, commandText);
            if (tableArray != null)
            {
                for (i = 0; i < tableArray.GetLength(0); i++)
                {
                    CustomDataType customDate = new CustomDataType();
                    customDate.id = Convert.ToInt32(tableArray[i, 1]);
                    customDate.attr = attributeName;

                    if (Convert.ToInt32(tableArray[i, 5]) < startTimeStamp)  //start from prevous month
                        customDate.startTime = start.ToString();
                    else
                        customDate.startTime = tableArray[i, 4];

                    if (Convert.ToInt32(tableArray[i, 8]) > endTimeStamp)  //end in next month
                        customDate.endTime = end.ToString();
                    else
                        customDate.endTime = tableArray[i, 7];

                    if (attributeName == "Repaired")
                        repaired.Add(customDate);
                    else if (attributeName == "Scheduled")
                        scheduled.Add(customDate);
                    else if (attributeName == "Maintained")
                        maintained.Add(customDate);
                }
            }

            if (attributeName == "Repaired")
                return repaired;
            else if (attributeName == "Scheduled")
                return scheduled;
            else if (attributeName == "Maintained")
                return maintained;
            else
                return null;
        }

        public bool SetSelectedPeroid(string attributeName, int id, string start, string end)
        {
            int maxID;
            int stamp1, stamp2;
            int machineID;
            string tableName;
            string databaseName;
            string commandText;
            //CustomDataType customDate;
            string[,] tableArray;

            System.Console.WriteLine("attributeName:{0}, id:{1}, start:{2}, end:{3}", attributeName, id, start, end);

            if (false == IntegrityCheck(id, start, end))
                return false;

            if ((id > 0) && (start == null) && (end == null))
                DeleteSelectedPeroid(attributeName, id);

            global_id = id;
            global_startTime = start;
            global_endTime = end;

            machineID = selectedMachine + 1;
            // machineID = 8;
            tableName = gVariable.machineWorkingPlanTableName;
            databaseName = gVariable.DBHeadString + machineID.ToString().PadLeft(3, '0');

            if (id == -1)
            {
                //typeID: = 0 repaired
                //        = 1 APS planned
                //        > 1 maintained
                //get max id value for a maintained plan then increase by one to generate a new id
                commandText = "select * from `" + tableName + "` where typeID > 1 order by typeID desc";
                tableArray = mySQLClass.databaseCommonReading(databaseName, commandText);
                if (tableArray == null)
                {
                    maxID = 2;
                }
                else
                {
                    maxID = Convert.ToInt32(tableArray[0, 1]);
                }

                if (end == null)  //new with start
                {
                    //record this plan into database by using this id
                    mySQLClass.writeDataToWorkingPlanTableFromCalendar(databaseName, tableName, attributeName, maxID + 1, start, end);
                    global_id = maxID + 1;
                    return true;
                }
                else
                {
                    id = maxID;
                }
            }

            //get max id value for a maintained plan then increase by one to generate a new id
            commandText = "select * from `" + tableName + "` where typeID = '" + id + "'";
            tableArray = mySQLClass.databaseCommonReading(databaseName, commandText);
            if (tableArray == null)
            {
                Console.WriteLine("Input id was not found, id error!!!");
                return false;
            }

            //get start and end time if the user modified one of them
            if (start == null)  //user modified end time
            {
                start = tableArray[0, 4];
            }
            else if (end == null)  //user modified start time
            {
                end = tableArray[0, 7];
            }

            stamp1 = toolClass.timeStringToTimeStamp(start);
            stamp2 = toolClass.timeStringToTimeStamp(end);
            commandText = "update `" + tableName + "` set typeID = '" + id + "', planTime1 = '" + start + "', timeStamp1 = '" + stamp1 +
                          "', duration = '" + (stamp2 - stamp1) + "', plantime2 = '" + end + "', timestamp2 = '" + stamp2 + "' where typeID = '" + id + "'";
            mySQLClass.pureDatabaseNonQueryAction(databaseName, commandText);
            //finished one maintained plan
            global_id = id;
            global_startTime = null;
            global_endTime = null;

            return true;
        }

        public bool DeleteSelectedPeroid(string attributeName, int id)
        {
            int machineID;
            string tableName;
            string databaseName;
            string commandText;

            //if ((id == 0) || (false == IntegrityCheck(id, start, end)))
            //    return false;

            machineID = selectedMachine + 1;
            // machineID = 8;
            tableName = gVariable.machineWorkingPlanTableName;
            databaseName = gVariable.DBHeadString + machineID.ToString().PadLeft(3, '0');

            commandText = "delete from `" + tableName + "` where typeID = '" + id + "'";
            mySQLClass.pureDatabaseNonQueryAction(databaseName, commandText);
            mySQLClass.redoIDIncreamentAfterRecordDeleted(databaseName, tableName);  //re-order IDs in this table

            global_id = id;
            global_startTime = null;
            global_endTime = null;

            return true;
        }


        private bool IntegrityCheck(int id, string start, string end)
        {
            System.Console.WriteLine("IntegrityCheck id:{0}, start:{1}, end:{2}", id, start, end);
            System.Console.WriteLine("IntegrityCheck global_id:{0}, global_start:{1}, global_end:{2}", global_id, global_startTime, global_endTime);
            if ((id == -1) && (global_startTime != null))
            {
                if (start != global_startTime)
                {
                    IntegrityErrorMessage("不能连续设定起始时间");
                    return false;
                }

                if (end == null)
                {
                    IntegrityErrorMessage("没有设置终止时间");
                    return false;
                }

                if (start == null)
                {
                    IntegrityErrorMessage("没有起始时间不能设置终止时间");
                    return false;
                }

                if (start == null && end == null)
                    return false;

                if(start != null && end != null && (string.Compare(start,end) >= 0))
                {
                    IntegrityErrorMessage("起始时间必须早于终止时间");
                    return false;
                }
            }
        
            return true;
        }

        private void IntegrityErrorMessage(string text)
        {
            MessageBox.Show(text, "提示信息");
        }

    }
}
