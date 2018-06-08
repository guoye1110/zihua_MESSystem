using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MESSystem.dispatchManagement
{
    public partial class cancelPublish : Form
    {
        public cancelPublish(string[,] batchTableArray, int index)
        {
            InitializeComponent();

            textBox2.Text = batchTableArray[index, 19];
            textBox9.Text = batchTableArray[index, 1];
            textBox1.Text = batchTableArray[index, 6];
            textBox5.Text = batchTableArray[index, 4];
            textBox8.Text = batchTableArray[index, 8];
            textBox7.Text = batchTableArray[index, 13];
        }

        private void cancelPublish_Load(object sender, EventArgs e)
        {

        }

        
        private void button1_Click(object sender, EventArgs e)
        {
            dispatchPublish.publishScreenRefresh = 2;
            dispatchPublish.cancelReason = textBox6.Text;
            this.Close();
        }

        //exit
        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
