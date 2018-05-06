using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MESSystem.common;

namespace MESSystem.machine
{
    public partial class popupEditBox : Form
    {
        static int popupMeesageComplete = 0;
 
        public static popupEditBox popupEditBoxClass = null;

        public popupEditBox()
        {
            InitializeComponent();
            this.Icon = new Icon(gVariable.logoInTitleArray[gVariable.CompanyIndex]);

            popupMeesageComplete = 0;
            textBox1.Text = null;
            this.TopMost = true;
        }

        public string popupEditBoxStarted()
        {
            string str;

            while(popupMeesageComplete == 0)
            {
                toolClass.nonBlockingDelay(500);
            }
            str = textBox1.Text;
            this.Close();
            return str;
        }

        private void popup_FormClosing(object sender, EventArgs e)
        {
        }

        //exit
        private void button2_Click(object sender, EventArgs e)
        {
            textBox1.Text = null;
            popupMeesageComplete = 1;
            this.Close();
        }

        //confirmed
        private void button1_Click(object sender, EventArgs e)
        {
            popupMeesageComplete = 1;
        }
    }
}
