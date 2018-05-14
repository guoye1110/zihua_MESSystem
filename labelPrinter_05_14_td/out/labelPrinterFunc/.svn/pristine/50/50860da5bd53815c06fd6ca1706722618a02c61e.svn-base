using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using LabelPrint.Util;
namespace LabelPrint.PrintForms
{
    public partial class ScanForm : Form
    {
        BardCodeHooK BarCodeHook = new BardCodeHooK();
        public ScanForm()
        {
            InitializeComponent();
        }

        private void ScanForm_Load(object sender, EventArgs e)
        {

            BarCodeHook.BarCodeEvent += new BardCodeHooK.BardCodeDeletegate(BarCode_BarCodeEvent);
            BarCodeHook.Start();
            tb_BarCode.Text = "";
            tb_BarCode.Enabled = false;
        }
        delegate void AsynBarCode_BarCodeEvent(BardCodeHooK.BarCodes barCode);
        void BarCode_BarCodeEvent(BardCodeHooK.BarCodes barCode)
        {
            Log.d("HH", "Final Barcode=" + barCode.BarCode);
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new AsynBarCode_BarCodeEvent(BarCode_BarCodeEvent), new object[] { barCode });
            }
            else
            {
                tb_BarCode.Text = barCode.BarCode;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }

            //WidthP.Text = barCode.BarCode;
        }

        private void bt_Ok_Click1(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        public String GetBarCodeValue()
        {
            return tb_BarCode.Text;
        }

        private void bt_Ok_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void bt_Cancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void ScanForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            BarCodeHook.Stop();
        }
    }
}
