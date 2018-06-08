using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MESSystem.commonControl
{
    public partial class manualInput : Form
    {
        public manualInput()
        {
            InitializeComponent();
            initVariables();
        }

        void initVariables()
        {
            comboBox1.Items.Add("男");
            comboBox1.Items.Add("女");
        }
    }
}
