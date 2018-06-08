using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MESSystem.common;
using MESSystem.communication;
using MESSystem.dispatchManagement;

namespace MESSystem.mainUI
{
    public partial class showDispatchList : Form
    {
        const int FUNC_TYPE_GLOBAL = 0;
        const int FUNC_TYPE_MACHINE = 1;

        const int DISPATCH_NUM_ONE_SCREN = 30;
        const int MAX_DISPATCH_ONE_MACHINE = 10000;

        int timeCheckingType;
        int whereFrom;
        int dispatchStatus;
//        int functionType;
        string databaseName;
        string tableName;

//        private string dispachSelected;

        public static showDispatchList dispatchListClass = null; //用来引用主窗口

        public showDispatchList(string databaseName_, int dispatchStatus_, int timeCheckingType_, int whereFrom_)
        {
            InitializeComponent();
            this.Icon = new Icon(gVariable.logoInTitleArray[gVariable.CompanyIndex]);

            databaseName = databaseName_;
            dispatchStatus = dispatchStatus_;
            timeCheckingType = timeCheckingType_;
            whereFrom = whereFrom_;
            if (databaseName == gVariable.globalDatabaseName)
            {
                tableName = gVariable.globalDispatchTableName;
//                functionType = FUNC_TYPE_GLOBAL;
            }
            else
            {
                tableName = gVariable.dispatchListTableName;
//                functionType = FUNC_TYPE_MACHINE;
            }
        }

        private void listView_Load(object sender, EventArgs e)
        {
            int i;
            int nameIndex;
            gVariable.dispatchSheetStruct[] dispatchList;

            try
            {
                dispatchList = mySQLClass.getDispatchListInPeriodOfTime(databaseName, tableName, "2010-12-24", "2030-12-31", dispatchStatus, timeCheckingType);
                if (dispatchList != null)
                {
                    this.listView1.BeginUpdate();
                    listView1.GridLines = true;
                    listView1.Dock = DockStyle.Fill;
                    listView1.Columns.Add(" ", 1, HorizontalAlignment.Left);
//                    listView1.Columns.Add("设备编码", 120, HorizontalAlignment.Left);
                    listView1.Columns.Add("设备名称", 90, HorizontalAlignment.Left);
                    listView1.Columns.Add("工单号", 100, HorizontalAlignment.Left);
                    listView1.Columns.Add("产品编码", 80, HorizontalAlignment.Left);
                    listView1.Columns.Add("产品名称", 130, HorizontalAlignment.Left);
                    listView1.Columns.Add("工序", 60, HorizontalAlignment.Left);
                    listView1.Columns.Add("工单状态", 60, HorizontalAlignment.Left);
                    listView1.Columns.Add("作业员", 60, HorizontalAlignment.Left);
                    listView1.Columns.Add("预计开工", 120, HorizontalAlignment.Left);
                    listView1.Columns.Add("预计完工", 120, HorizontalAlignment.Left);
                    listView1.Columns.Add("实际开工", 120, HorizontalAlignment.Left);
                    listView1.Columns.Add("首检合格", 120, HorizontalAlignment.Left);
                    listView1.Columns.Add("实际完工", 120, HorizontalAlignment.Left);
                    listView1.Columns.Add("计划数", 60, HorizontalAlignment.Left);
                    listView1.Columns.Add("合格数", 60, HorizontalAlignment.Left);
                    listView1.Columns.Add("不合格数", 60, HorizontalAlignment.Left);

                    i = 0;
                    for (i = 0; i < dispatchList.Length; i++ )
                    {
                        if (dispatchList[i].dispatchCode == null)
                            break;

                        if (dispatchList[i].dispatchCode == "dummy")
                            continue;

                        ListViewItem OptionItem = new ListViewItem();

                        //                        OptionItem.SubItems.Add(dispatchList[i].machineCode);
                        nameIndex = Convert.ToInt16(dispatchList[i].machineID) - 1;
                        if (nameIndex > gVariable.machineNameArrayDatabase.Length)
                            nameIndex = 0;
                        OptionItem.SubItems.Add(gVariable.machineNameArrayDatabase[nameIndex]);
                        OptionItem.SubItems.Add(dispatchList[i].dispatchCode);
                        OptionItem.SubItems.Add(dispatchList[i].productCode);
                        OptionItem.SubItems.Add(dispatchList[i].productName);
                        OptionItem.SubItems.Add(dispatchList[i].processName);

                        switch (dispatchList[i].status)
                        {
                            case gVariable.MACHINE_STATUS_SHUTDOWN:
                            case gVariable.MACHINE_STATUS_DISPATCH_DUMMY:
                                OptionItem.SubItems.Add("未开工");
                                break;
                            case gVariable.MACHINE_STATUS_DISPATCH_STARTED:
                                OptionItem.SubItems.Add("已开工");
                                break;
                            case gVariable.MACHINE_STATUS_DISPATCH_COMPLETED:
                                OptionItem.SubItems.Add("已完工");
                                break;
                            default:
                                OptionItem.SubItems.Add("未开工");
                                break;
                        }

                        OptionItem.SubItems.Add(dispatchList[i].operatorName);
                        OptionItem.SubItems.Add(dispatchList[i].planTime1);
                        OptionItem.SubItems.Add(dispatchList[i].planTime2);
                        OptionItem.SubItems.Add(dispatchList[i].realStartTime);
                        OptionItem.SubItems.Add(dispatchList[i].prepareTimePoint);
                        OptionItem.SubItems.Add(dispatchList[i].realFinishTime);
                        OptionItem.SubItems.Add(dispatchList[i].plannedNumber.ToString());
                        OptionItem.SubItems.Add(dispatchList[i].qualifiedNumber.ToString());
                        OptionItem.SubItems.Add(dispatchList[i].unqualifiedNumber.ToString());
                        //                        OptionItem.SubItems.Add(dispatchList[i].wastedOutput.ToString() + "/" + dispatchList[i].waitForCheck.ToString());

                        listView1.Items.Add(OptionItem);
                    }
                    this.listView1.EndUpdate();
                }

                this.Text = gVariable.programTitle + "工单列表查询";
            }
            catch (Exception ex)
            {
                Console.WriteLine("showDispatchList listView_load failed! " + ex);
            }
        }

        private void showDispatchList_FormClosing(object sender, EventArgs e)
        {
            if(whereFrom == gVariable.FUNCTION_DISPATCH_LIST_UI)
                dispatchUI.dispatchUIClass.Show();
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.listView1.SelectedItems.Count > 0)
                {
                    //we selected a dispatch from list view, first we need to get the information about this dispatch
                    gVariable.contemporarydispatchUI = 0;
                    gVariable.dispatchUnderReview = this.listView1.SelectedItems[0].SubItems[mySQLClass.DISPATCH_CODE_IN_DISPATCHLIST_DATABASE].Text;
                    dispatchTools.getCurveInfoIngVariable(gVariable.currentCurveDatabaseName, gVariable.boardIndexSelected, gVariable.HISTORY_READING);

                    if(whereFrom == gVariable.FUNCTION_DISPATCH_LIST_UI)
                        dispatchUI.dispatchUIClass.Show();

                    this.Close();
                }
            }
            catch (Exception ex)
            {
                Console.Write("listView1_SelectedIndexChanged for dispatchList failed. ");
                Console.WriteLine(ex.ToString());
                return;
            }
        }
    }
}
