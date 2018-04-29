using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MESSystem.common;

namespace MESSystem.mainUI
{
    public partial class login : Form
    {
        public static login loginClass = null;

        int loginScreenCompleted = 0;
        int returnValue;

        //from : 0 enter
        //       1 exit 
        public login(int from)
        {
            InitializeComponent();
            this.Icon = new Icon(gVariable.logoInTitleArray[gVariable.CompanyIndex]);
            textBox1.Text = "9999";
            textBox2.Text = "****";

            linkLabel1.Text = "修改密码";
            linkLabel2.Text = "忘记密码";

            if (from == 0) //user enter system
            {
                linkLabel1.Visible = false;
                linkLabel2.Visible = false;
                button3.Visible = false;

                label4.Visible = false;
                textBox3.Visible = false;
                textBox2.Text = "*******";
                button1.Focus();
            }
            else if (from == 1)  //user exit system
            {
                linkLabel1.Visible = false;
                linkLabel2.Visible = false;
                button3.Visible = false;
                textBox2.Text = null;
            }
            else //if (from == 2) //user change password or set privilege
            {
                linkLabel1.Visible = true;
                linkLabel2.Visible = true;
                button3.Visible = true;

                label4.Visible = false;
                textBox3.Visible = false;
                button1.Visible = false;
                button2.Visible = false;
            }
        }

        public int enterLoginScreen()
        {
            while (loginScreenCompleted == 0)
            {
                toolClass.nonBlockingDelay(500);
            }
            this.Close();
            return returnValue;
        }

        //confirm account/password
        private void button1_Click(object sender, EventArgs e)
        {
            int flag;
            string str;
            DataTable dt;
            string strPassword;

            flag = 0;
            gVariable.userAccount = textBox1.Text;
//            strPassword = toolClass.MD5Encrypt(textBox2.Text);
            strPassword = "1111"; // textBox2.Text;

            if(textBox3.Text != null)
            {
                gVariable.infoWriter.WriteLine("Exit reason:" + textBox3.Text);
                gVariable.infoWriter.WriteLine(DateTime.Now.ToString() + ": Server exit");
            }

            dt = toolClass.getDataTableForAccount(gVariable.userAccount);
            foreach (DataRow dr in dt.Rows)
            {
                flag = 1;
                if (strPassword == dr[mySQLClass.EMPLOYEE_PASSWORD_COLIUMN].ToString())
                {
                    str = dr[mySQLClass.EMPLOYEE_RANK_COLIUMN].ToString();
                    if (toolClass.isDigitalNum(str) == 1)
                    {
                        gVariable.privilegeLevel = Convert.ToInt16(dr[mySQLClass.EMPLOYEE_RANK_COLIUMN]);
                    }
                    else
                    {
                        MessageBox.Show("很抱歉，该帐号权限有误，请检查文件 employee.xlsx，谢谢！", "信息提示", MessageBoxButtons.OK);
                        return;
                    }
                    returnValue = 1;
                    loginScreenCompleted = 1;
                }
            }

            if (flag == 0)
            {
                MessageBox.Show("很抱歉，帐号输入有误，请输入姓名或者工号，谢谢！", "信息提示", MessageBoxButtons.OK);
                return;
            }

            if (loginScreenCompleted == 0)
            {
                MessageBox.Show("很抱歉，密码输入有误，请重新输入，谢谢！", "信息提示", MessageBoxButtons.OK);
                return;
            }
        }

        //exit program
        private void button2_Click(object sender, EventArgs e)
        {
            loginScreenCompleted = 1;
            returnValue = -1;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {

        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {

        }

    }
}
