using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using LabelPrint.Util;

namespace LabelPrint
{
    public partial class LoginForm : Form
    {
        int loginScreenCompleted = 0;
      //  int returnValue;
        public LoginForm()
        {
            InitializeComponent();
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            linkLabel1.Visible = false;
            linkLabel2.Visible = false;
            button3.Visible = false;

            //label4.Visible = false;
           // textBox3.Visible = false;
            textBox2.Text = "";
            button1.Focus();
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            mySQLClass a = new mySQLClass();
            int flag;
            string str;
            DataTable dt;
            string strPassword;

            flag = 0;
            gVariable.userAccount = textBox1.Text;
            //            strPassword = toolClass.MD5Encrypt(textBox2.Text);
            strPassword = textBox2.Text;

            //if (textBox3.Text != null)
            {
                //  gVariable.infoWriter.WriteLine("Exit reason:" + textBox3.Text);
                //   gVariable.infoWriter.WriteLine(DateTime.Now.ToString() + ": Server exit");
            }

            if (gVariable.userAccount.Length >= 4)
            {
                flag = 1;
                //returnValue = 1;
                loginScreenCompleted = 1;
            }
            else
            {

                dt = mySQLClass.getDataTableForAccount(gVariable.userAccount);
                if (dt != null)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        flag = 1;
                        if (strPassword == dr[mySQLClass.EMPLOYEE_PASSWORD_COLIUMN].ToString())
                        {
                            str = dr[mySQLClass.EMPLOYEE_RANK_COLIUMN].ToString();
                            //if (toolClass.isDigitalNum(str) == 1)
                            //{
                            //    gVariable.privilegeLevel = Convert.ToInt16(dr[mySQLClass.EMPLOYEE_RANK_COLIUMN]);
                            //}
                            //else
                            //{
                            //    MessageBox.Show("很抱歉，该帐号权限有误，请检查文件 employee.xlsx，谢谢！", "信息提示", MessageBoxButtons.OK);
                            //    return;
                            //}
                            //returnValue = 1;
                            loginScreenCompleted = 1;
                        }
                    }
                }
            }

            if (flag == 0)
            {
                MessageBox.Show("很抱歉，帐号输入有误，请输入姓名或者工号，谢谢！", "信息提示", MessageBoxButtons.OK);
              //  return;
            }
            else 
            if (loginScreenCompleted == 0)
            {
                MessageBox.Show("很抱歉，密码输入有误，请重新输入，谢谢！", "信息提示", MessageBoxButtons.OK);
             //   return;
            }
            else
            {
                MainForm main;
                this.Visible = false;
                main =  new MainForm();
                main.ShowDialog();
                Close();
            }
        }

    }
}
