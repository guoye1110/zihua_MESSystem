using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MESSystem.common;

namespace MESSystem.alarmFun
{
    public partial class addEmailAddress : Form
    {
        public addEmailAddress()
        {
            InitializeComponent();
//            this.Icon = new Icon(gVariable.icoLogo);
        }


        //confirmed
        private void button1_Click(object sender, EventArgs e)
        {
            if (gVariable.mailListAlarm == null)
                gVariable.mailListAlarm += textBox1.Text;
            else
                gVariable.mailListAlarm += ";" + textBox1.Text;

            this.Close();
        }

        //exit
        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
