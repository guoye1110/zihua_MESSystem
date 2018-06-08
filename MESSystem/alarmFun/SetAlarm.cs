using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using MESSystem.common;
using MESSystem.quality;
using MESSystem.communication;

namespace MESSystem.alarmFun
{
    public partial class SetAlarmClass : Form
    {
        const int HTML_MODE = 0;  //
        const int LINE_EDITOR_MODE = 1;

        const string HTMLLineFeed = "<br/>";
        const string editorLineFeed = "\r\n";
        const int COMMUNICATION_TYPE_EMAIL_FORWARDER = 0xfe;  //email sending client

        int alarmID;
        int disapearIndex;

        int boardIndex;

        string alarmDatabaseName;
        string alarmListTableName;

        string dispatchCode;
        string feedBinID;
        string errorDesc;
        string alarmFailureCode;
        string machineName;
        string operatorName;
        string workshop;
        string time;
        string signer;
        string time1;
        string completer;
        string time2;
        int status;
//        int inHistory = 0;
//        int startID;
        int indexInTable;
        int type;
        int category = 0;
        string mailList;
        string discuss;
        string solution;

        public gVariable.alarmTableStruct alarmTableStructImpl;

        //private Thread thread1;

        public static SetAlarmClass[] activeAlarmInstanceArray = new SetAlarmClass[gVariable.maxActiveAlarmNum];

        System.Windows.Forms.Timer aTimer;

//        public static SetAlarmClass setAlarm;
        public SetAlarmClass()
        {
            InitializeComponent();
            this.Icon = new Icon(gVariable.logoInTitleArray[gVariable.CompanyIndex]);
            disapearIndex = 0;

            closeAlarmTimer();
        }


        //there is no one to close this alarm popup message for plant 23 product, so we need to close this window by ourself
        private void closeAlarmTimer()
        {
            aTimer = new System.Windows.Forms.Timer();
            aTimer.Interval = 1000;
            aTimer.Enabled = true;

            aTimer.Tick += new System.EventHandler(this.closeAlarmTick);
        }

        //one second passed, we need to update mail list, then check whether an hour had passed and we need to close it
        private void closeAlarmTick(object sender, EventArgs e)
        {
            //int i;
            //string[] addrArray;

            disapearIndex++;

            /*
            //mail list changed, we need to do listbox refresh
            if (mailList != gVariable.mailListAlarm) // && gVariable.mailListAlarm != null)
            {
                listBox1.Items.Clear();

                addrArray = gVariable.mailListAlarm.Split(';');

                for(i = 0; i < addrArray.Length; i++)
                    listBox1.Items.Add(addrArray[i]);

                mailList = gVariable.mailListAlarm;
            }

            listBox1.Show();
            */

            if (disapearIndex >= 60 * 30)
            {
                aTimer.Stop();
                this.Close();
            }
        } 


        //display an alarm on screen
        //idOfAlarm means the index for this alarm in current machine's alarm table
        public SetAlarmClass(string databaseName, gVariable.alarmTableStruct alarmTableStructImpl, int alarmID_)
        {
            try
            {
                InitializeComponent();

                this.Icon = new Icon(gVariable.logoInTitleArray[gVariable.CompanyIndex]);
//                this.TopMost = true;

                alarmID = alarmID_;

                closeAlarmTimer();

                alarmDatabaseName = databaseName;
                alarmListTableName = gVariable.alarmListTableName;

                boardIndex = toolClass.getBoardIndexByDatabaseName(alarmDatabaseName);

                dispatchCode = alarmTableStructImpl.dispatchCode;
                feedBinID = alarmTableStructImpl.feedBinID;
                errorDesc = alarmTableStructImpl.errorDesc;
                alarmFailureCode = alarmTableStructImpl.alarmFailureCode;
                machineName = alarmTableStructImpl.machineName;
                operatorName = alarmTableStructImpl.operatorName;
                workshop = alarmTableStructImpl.workshop;
                category = alarmTableStructImpl.category;
                time = alarmTableStructImpl.time;
                type = alarmTableStructImpl.type;
                status = alarmTableStructImpl.status;
                indexInTable = alarmTableStructImpl.indexInTable;
                mailList = alarmTableStructImpl.mailList;
                discuss = alarmTableStructImpl.discuss;
                solution = alarmTableStructImpl.solution;

                if (type == gVariable.ALARM_TYPE_DEVICE)
                {
                    this.Text = workshop + "设备" + machineName + "发出设备安灯报警指令";
                    button12.Visible = false;
                }
                else if (type == gVariable.ALARM_TYPE_MATERIAL)
                {
                    this.Text = workshop + "设备" + machineName + "的" + feedBinID + "号料仓发出物料报警指令";
                    button10.Visible = false;
                    button11.Visible = false;
                    button12.Visible = false;
                }
                else if (type == gVariable.ALARM_TYPE_QUALITY_DATA)
                {
                    this.Text = workshop + "设备" + machineName + "发出质量数据报警指令";
                }
                else if (type == gVariable.ALARM_TYPE_CRAFT_DATA)
                {
                    this.Text = workshop + "设备" + machineName + "发出工艺参数报警指令";
                }

                mailList = gVariable.basicmailListAlarm;

                //SetAlarmDataForEmail();

                textBox1.Text = setAlarmText(LINE_EDITOR_MODE);
                textBox2.Text = discuss;
                textBox4.Text = solution;

                if (gVariable.thisIsHostPC != true)
                {
                    gVariable.clientalarmStatus = status;
                    gVariable.clientMailList = mailList;
                    gVariable.clientDiscussInfo = discuss;
                }

                if(status == gVariable.ALARM_STATUS_COMPLETED || status == gVariable.ALARM_STATUS_CANCELLED)
                {
                    button3.Visible = false;
                    button7.Visible = false;

                    button1.Enabled = false;
                    button2.Enabled = false;
                    button8.Enabled = false;
                    button9.Enabled = false;
                }
                else
                {
                    //no one can close an alarm before it is completed or cancelled
//                    button6.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Console.Write("new alarm class failed!" + ex);
            }
        }

        // 1. set new content of alarm to screen, including discuss/solution/email/button
        // 2. set new content of this alarm to database no matter this is a server PC or client PC, any modification made by this account will be recorded in database
        public void SetAlarmDataOnScreen(string signer_, string time1_, string completer_, string time2_, int status_, string mailList_, string discuss_, string solution_)
        {
            string updateStr;

            try
            {
                if (signer_ != null)
                    signer = signer_;

                if(time1_ != null)
                    time1 = time1_;

                if(completer_ != null)
                    completer = completer_;

                if(time2_ != null)
                    time2 = time2_;

                if (status_ != gVariable.ALARM_STATUS_UNCHANGED)
                    status = status_;

                if(mailList_ != null)
                { 
                    mailList = mailList_;
                    //SetAlarmDataForEmail();
                    //textBox1.Text = setAlarmText(LINE_EDITOR_MODE);
                }

                if (discuss_ != null)
                {
                    discuss = discuss_;
                    textBox2.Text = discuss;
                }

                if (solution_ != null)
                {
                    solution = solution_;
                    textBox4.Text = solution;
                }

                updateStr = "update `" + alarmListTableName + "` set signer = '" + signer + "', time1 = '" + time1_ + "', completer = '" + completer + "', time2 = '" +
                             time2_ + "', status = '" + status + "', mailList = '" + mailList + "', discuss = '" + discuss + "', solution = '" + solution + "' where id = '" + alarmID + "'";
                mySQLClass.updateTableItems(alarmDatabaseName, updateStr);

                updateStr = "update `" + gVariable.globalAlarmListTableName + "` set signer = '" + signer + "', time1 = '" + time1_ + "', completer = '" + completer + "', time2 = '" +
                              time2_ + "', status = '" + status + "', mailList = '" + mailList + "', discuss = '" + discuss + "', solution = '" + solution + "' where alarmFailureCode = '" + alarmFailureCode + "'";
                mySQLClass.updateTableItems(gVariable.globalDatabaseName, updateStr);

                if (status == gVariable.ALARM_STATUS_COMPLETED || status == gVariable.ALARM_STATUS_CANCELLED)
                {
                    button1.Enabled = false;
                    button2.Enabled = false;
                    button3.Enabled = false;
                    //button4.Enabled = false;
                    //button5.Enabled = false;
                    button7.Enabled = false;

                    button6.Enabled = true;
                }

                setAlarmText(LINE_EDITOR_MODE);
            }
            catch (Exception ex)
            {
                Console.Write("setup alarm data failed!" + ex);
            }
        }

        /*
        public void SetAlarmDataForEmail()
        {
            int i;
            string[] mailListArray =  new string[gVariable.maxNumMailList];

            try
            {
                if (mailList == null) //no mail address
                    return;

                mailListArray = mailList.Split(';');

                listBox1.Items.Clear();
                for (i = 0; i < mailListArray.Length; i++)
                {
                    if (mailListArray[i] == null || mailListArray[i].Length <= 5)  //no email or email account too short
                        break;

                    listBox1.Items.Add(mailListArray[i]);
                }

                if (i == 0 && mailListArray[i].Length > 5)  //for the case of only one mail account exists
                    mailListArray[0] = mailList;

                if (mailListArray[0].Length >= 5)
                    listBox1.SetSelected(0, true);

                thread1 = new Thread(emailSendingThread);
                thread1.Start();
            }
            catch (Exception ex)
            {
                Console.Write("setup alarm failed!" + ex);
            }
        }
        */

        public string setAlarmText(int alarmTextMode)
        {
            string strText;
            string statusStr;
            string typeStr;
            string lineFeed;

            try
            {
                if (alarmTextMode == LINE_EDITOR_MODE)
                {
                    lineFeed = editorLineFeed;
                }
                else
                {
                    lineFeed = HTMLLineFeed;
                }
                strText = "【报警单号】：" + alarmFailureCode + lineFeed;
                //strText += "【报警工单】：" + dispatchCode + lineFeed;
                strText += "【报警设备】：" + machineName + lineFeed;

                if (type == gVariable.ALARM_TYPE_MATERIAL)
                {
                    strText += "【报警设备料仓】：" + feedBinID + "号料仓" + lineFeed;
                }

                if (status < gVariable.strAlarmStatus.Length)
                    statusStr = gVariable.strAlarmStatus[status];
                else
                    statusStr = gVariable.strAlarmStatus[0];

                typeStr = gVariable.strAlarmTypeInDetail[type];

                if (type < gVariable.ALARM_TYPE_ALL_IN_DETAIL)
                {
                    strText += "【报警原因】：" + errorDesc + lineFeed;
                }
                else
                {
                    typeStr = "报警类型未知";
                }

                if (type == gVariable.ALARM_TYPE_QUALITY_DATA || type == gVariable.ALARM_TYPE_CRAFT_DATA)
                {
                    strText += "【报警员工】：" + gVariable.SPCMonitoringSystem + lineFeed;
                }
                else
                {
                    strText += "【报警员工】：" + operatorName + "; " + "【报警部门】：" + workshop + lineFeed;
                }
                strText += "【报警时间】：" + time + lineFeed;
                strText += "【报警类型】：" + typeStr + lineFeed;
                strText += "【报警状态】：" + statusStr;
            }
            catch (Exception ex)
            {
                Console.WriteLine("setup alarm failed!" + ex);
                strText = "setup alarm failed!" + ex;
            }

            return strText;
        }

        private void emailSendingThread()
        {
            int i;
            int len;
            //when we send this email data out to email forwarder, we need to combine email-address\title\content into one string for data sending, we use this cutter to combine,
            //when email forward get the combined string, it can restore the origoinal email-address\title\content by this cutter string
            string MAIL_CUTTER = "1Wc7gUx";  
            string subject;
            string body = null;
            string mailString;
            byte[] outArray;
            byte[] mailByte;

            try
            {
                mailString = null;

                if (gVariable.emailForwarderHeartBeatNum == 0)  //email server not connected
                    return;

                //if there is no mail account
                if (gVariable.mailListAlarm == null)
                    return;

                subject = "东风生产监控系统报告错误，编号：" + alarmFailureCode;

                body = setAlarmText(HTML_MODE);

                mailString = gVariable.mailListAlarm;

                mailString += MAIL_CUTTER + subject + MAIL_CUTTER + body;

                mailByte = System.Text.Encoding.Default.GetBytes(mailString);
                len = 9 + mailByte.Length;

                outArray = new byte[len];

                outArray[0] = (byte)'w';
                outArray[1] = (byte)'I';
                outArray[2] = (byte)'F';
                outArray[3] = (byte)'i';
                outArray[4] = (byte)len;
                outArray[5] = (byte)(len / 0x100);
                outArray[6] = COMMUNICATION_TYPE_EMAIL_FORWARDER;

                for (i = 0; i < len - 9; i++)
                {
                    outArray[i + 7] = mailByte[i];
                }
                toolClass.addCrc16Code(outArray, len);

                gVariable.emailForwarderSocket.Send(outArray, len, 0);   //response OK
            }
            catch (Exception ex)
            {
                gVariable.infoWriter.WriteLine("send mail to mail serve failed!" + ex);
            }

        }


        private void setAlarm_Load(object sender, EventArgs e)
        {
            //this is not a good idea, but we need it. 
            //C# 2.0 and later forbids other thread to modify form contents, but we need this function, so disabled this thread checking,
            //as a result, any thread can modify alarm content displayed on screen
            Control.CheckForIllegalCrossThreadCalls = false;
        }

        private void setAlarmClosing(object sender, FormClosingEventArgs e)
        {
            if(aTimer != null)
                aTimer.Stop();

            toolClass.activeAlarmClosed(alarmDatabaseName, alarmID);
            gVariable.typeAlarmAlreadyAlive[type, boardIndex] = 0;

            //if this is an alarm triggered by SPC checking, cancel this alarm means need to be recorded by SPCTriggeredAlarmArray, if SPCTriggeredAlarmArray[] is empty, we can resume SPC checking  
            boardIndex = toolClass.getBoardIndexByDatabaseName(alarmDatabaseName);
            //alarmID this is a SPC triggered alarm, clear this flag, so SPC monitor can continue to work
            if (gVariable.SPCTriggeredAlarmArray[boardIndex] == alarmID)
                gVariable.SPCTriggeredAlarmArray[boardIndex] = -1;
        }


        //send discuss
        private void button1_Click(object sender, EventArgs e)
        {
            string name;
            string discuss_;

            name = toolClass.getNameByIDAndIDByName(null, gVariable.userAccount);
            
            //get all information from database
            alarmTableStructImpl = mySQLClass.getAlarmTableContent(alarmDatabaseName, gVariable.alarmListTableName, alarmID);

            discuss_ = alarmTableStructImpl.discuss + name + " (" + DateTime.Now.ToString() + "):" + textBox3.Text + editorLineFeed;

            SetAlarmDataOnScreen(null, null, null, null, gVariable.ALARM_STATUS_UNCHANGED, null, discuss_, null);

            textBox3.Text = "";

            tellCounterpartAlarmUpdated(alarmDatabaseName, alarmID);
        }

        //clear discussion
        private void button2_Click(object sender, EventArgs e)
        {
            textBox3.Text = "";
        }

        //cancel alarm, not exit from screen, but set this alarm to cancelled mode 
        private void button3_Click(object sender, EventArgs e)
        {
//            int i;
//            int boardIndex;
            string now;
            string name;
            string updateStr;

            now = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            name = toolClass.getNameByIDAndIDByName(null, gVariable.userAccount);

            solution += name + " (" + DateTime.Now.ToString() + "):" + "确认报警取消" + editorLineFeed;
            status = gVariable.ALARM_STATUS_CANCELLED;

            SetAlarmDataOnScreen(null, null, null, null, status, null, null, solution);

            button1.Enabled = false;
            button2.Enabled = false;
            button3.Enabled = false;
            //button4.Enabled = false;
            //button5.Enabled = false;
            button7.Enabled = false;
            button8.Enabled = false;
            button9.Enabled = false;

            button6.Enabled = true;

            if (aTimer != null)
                aTimer.Stop();

            tellCounterpartAlarmUpdated(alarmDatabaseName, alarmID);

            updateStr = "update `" + gVariable.globalAlarmListTableName + "` set signer = '" + name + "', time1 = '" + now + "' where id = '" + alarmID + "'";
            mySQLClass.updateTableItems(gVariable.globalDatabaseName, updateStr);

            updateStr = "update `" + alarmListTableName + "` set signer = '" + name + "', time1 = '" + now + "' where id = '" + alarmID + "'";
            mySQLClass.updateTableItems(alarmDatabaseName, updateStr);

            //this alarm will be closed, new alarm can be displayed
//            gVariable.typeAlarmAlreadyAlive[type, boardIndex] = 0;
//            this.Close();
        }

        //complete alarm
        private void button7_Click(object sender, EventArgs e)
        {
            string now;
            string name;
            string updateStr;

            now = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            name = toolClass.getNameByIDAndIDByName(null, gVariable.userAccount);

            solution += name + " (" + DateTime.Now.ToString() + "):" + "确认报警处理完毕" + editorLineFeed;
            status = gVariable.ALARM_STATUS_COMPLETED;

            SetAlarmDataOnScreen(null, null, null, null, status, null, null, solution);

            button1.Enabled = false;
            button2.Enabled = false;
            button3.Enabled = false;
            //button4.Enabled = false;
            //button5.Enabled = false;
            button7.Enabled = false;
            button8.Enabled = false;
            button9.Enabled = false;

            button6.Enabled = true;

            if (aTimer != null)
                aTimer.Stop();

            //id this is a SPC triggered alarm, clear this flag, so SPC monitor can continue to work
            if (gVariable.SPCTriggeredAlarmArray[boardIndex] == alarmID)
                gVariable.SPCTriggeredAlarmArray[boardIndex] = -1;

            tellCounterpartAlarmUpdated(alarmDatabaseName, alarmID);

            updateStr = "update `" + gVariable.globalAlarmListTableName + "` set completer = '" + name + "', time2 = '" + now + "' where id = '" + alarmID + "'";
            mySQLClass.updateTableItems(gVariable.globalDatabaseName, updateStr);

            updateStr = "update `" + alarmListTableName + "` set completer = '" + name + "', time2 = '" + now + "' where id = '" + alarmID + "'";
            mySQLClass.updateTableItems(alarmDatabaseName, updateStr);

            //this alarm will be closed, new alarm can be displayed
//            gVariable.typeAlarmAlreadyAlive[type, boardIndex] = 0;
//            this.Close();
        }

        //alarm exit from screen
        private void button6_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /*
        //add new mail account
        private void button4_Click(object sender, EventArgs e)
        {
            addEmailAddress addEmailAddressClass = new addEmailAddress();
            addEmailAddressClass.Show();
        }

        //remove mail account in listbox
        private void button5_Click(object sender, EventArgs e)
        {
            int i;

            listBox1.Items.Remove(listBox1.Items[listBox1.SelectedIndex]);

            for(i = 0; i < listBox1.Items.Count; i++)
            {
                gVariable.mailListAlarm += listBox1.SelectedItem + ";";
            }
        }
        */

        //send solution
        private void button8_Click(object sender, EventArgs e)
        {
            string name;
            string solution_;

            name = toolClass.getNameByIDAndIDByName(null, "0314"); //gVariable.userAccount);

            //get all information from database
            alarmTableStructImpl = mySQLClass.getAlarmTableContent(alarmDatabaseName, gVariable.alarmListTableName, alarmID);

            solution_ = alarmTableStructImpl.solution + name + " (" + DateTime.Now.ToString() + "):" + textBox5.Text + editorLineFeed;

            SetAlarmDataOnScreen(null, null, null, null, gVariable.ALARM_STATUS_UNCHANGED, null, null, solution_);

            textBox4.Text = solution_;
            textBox5.Text = "";

            tellCounterpartAlarmUpdated(alarmDatabaseName, alarmID);
        }


        //clear solution
        private void button9_Click(object sender, EventArgs e)
        {
            textBox5.Text = "";
        }

        private void tellCounterpartAlarmUpdated(string alarmDatabaseName, int alarmID)
        {
            int index;

            index = toolClass.getIndexInActiveAlarmArray(alarmDatabaseName, alarmID);
            if (index >= 0)
            {
                //this alarm changed something in its content, need to tell client PC if this is an host server, or tell host server if this is an client PC
                gVariable.activeAlarmNewStatus[index] = gVariable.ACTIVE_ALARM_STATUS_CHANGED;
                gVariable.activeAlarmInfoUpdatedLocally = 1;
                if (gVariable.thisIsHostPC == true) 
                    gVariable.activeAlarmInfoUpdatedCounterpart = 1;  //for client PC, this flag is not used

                if (gVariable.thisIsHostPC == true)
                {
                    //for host server, we need to tell all client PC about this, but we don't do it here, serverPCFunc.cs will do this, by check the flag of activeAlarmInfoUpdatedCounterpart
                }
                else
                {
                    //for client PC, simply tell host server about this update is OK, host server will tell other client about this update
                    communicate.updatedAlarmInfoToServerPC(alarmDatabaseName, alarmID);
                }
            }
            else
            {
                //problem occurred, we changed something in this alarm, but this alarm is not in our active alarm list
            }
        }

        //get histroy alarm list
        private void button10_Click(object sender, EventArgs e)
        {
            historyListReview historyListReviewClass = new historyListReview(alarmDatabaseName, type, category, indexInTable);

            if (historyListReviewClass.getCheckResult() < 0)
                return;
            historyListReviewClass.informParentEvent += new historyListReview.myDelegateInformParent(historyListSelected);
            historyListReviewClass.Show();
        }

        private void historyListSelected()
        {
            textBox3.Text = gVariable.alarmHistoryDiscuss;
            textBox5.Text = gVariable.alarmHistorySolution;
        }

        //add to history list
        private void button11_Click(object sender, EventArgs e)
        {
            string updateStr;

            //for server, we refresh display and send to client
            if (gVariable.thisIsHostPC == true)
            {
                updateStr = "update `" + alarmListTableName + "` set inHistory = '" + gVariable.ALARM_INHISTORY_TRUE + "' where id = '" + alarmID + "'";
                mySQLClass.updateTableItems(alarmDatabaseName, updateStr);
//                mySQLClass.updateAlarmTable(alarmDatabaseName, alarmListTableName, alarmID, null, null, null, null, gVariable.ALARM_STATUS_UNCHANGED, gVariable.ALARM_INHISTORY_TRUE, null, null, null);
            }
            else  //for client, we only send data to server, waiting for server's rebounce data to update display
            {
            }
        }

        //detailed alarm info
        private void button12_Click(object sender, EventArgs e)
        {
            //four paramters:
            //1: database name
            //2: where comes this requirement, from alarm or quality
            //3: alarmID means alarm index in alarm table
            //4: this is a quaity data or craft data triggered alarm 
            SPCAnalyze SPCAnalyzeClass = new SPCAnalyze(alarmDatabaseName, gVariable.FROM_ALARM_DISPLAY_FUNC, alarmID, type);
            SPCAnalyzeClass.Show();
        }

    }
}
